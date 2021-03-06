﻿#region --- License ---
/* Licensed under the MIT/X11 license.
 * Copyright (c) 2006-2008 the OpenTK Team.
 * This notice may not be removed from any source distribution.
 * See license.txt for licensing details.
 */
#endregion

using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using OpenTK;
using OpenTK.Input;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System.Runtime.InteropServices;

namespace BokehLab.DepthPeeling
{
    public class DepthPeeling : GameWindow
    {
        /// <summary>
        /// Number of depth peeling layers (color and depth textures).
        /// </summary>
        /// <remarks>
        /// 8 layers are almost always enough.
        /// </remarks>
        static readonly int LayerCount = 4;

        // index of currently displayed layer [0; LayerCount - 1]
        int activeLayer = 0;
        // show color (true) or depth texture?
        bool showColorTexture = true;
        // show all layers blended or just the active layer?
        bool showBlendedLayers = true;

        public DepthPeeling()
            : base(1024, 768)
        {
        }

        uint[] ColorTextures = new uint[LayerCount];
        uint[] DepthTextures = new uint[LayerCount];
        /// <summary>
        /// Frame-buffer Object to which the current color and depth texture
        /// can be attached.
        /// </summary>
        uint FBOHandle;

        int fragmentShaderObject;
        int shaderProgram;

        Matrix4 scenePerspective;
        Matrix4 sceneModelView;

        public static void RunExample()
        {
            using (DepthPeeling example = new DepthPeeling())
            {
                example.Run(30.0, 0.0);
            }
        }

        #region GameWindow event handlers

        protected override void OnLoad(EventArgs e)
        {
            if (!GL.GetString(StringName.Extensions).Contains("EXT_framebuffer_object"))
            {
                System.Windows.Forms.MessageBox.Show(
                     "Your video card does not support Framebuffer Objects. Please update your drivers.",
                     "FBOs not supported",
                     System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Exclamation);
                Exit();
            }

            GL.Enable(EnableCap.DepthTest);
            GL.ClearDepth(1.0f);
            GL.DepthFunc(DepthFunction.Lequal);

            GL.Disable(EnableCap.CullFace);
            GL.PolygonMode(MaterialFace.Back, PolygonMode.Fill);

            GL.Enable(EnableCap.Texture2D);

            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusDstAlpha);

            CreateShaders();

            //CreateLayerTextures(LayerCount, Width, Height);

            // Create a FBO and attach the textures
            GL.Ext.GenFramebuffers(1, out FBOHandle);
            //BindFramebufferWithLayerTextures(FBOHandle, activeLayer);

            //DrawIntoLayers();

            GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, (int)TextureEnvMode.Replace);

            Keyboard.KeyUp += KeyUp;
        }

        protected override void OnUnload(EventArgs e)
        {
            // Clean up what we allocated before exiting
            DeallocateLayerTextures();

            if (shaderProgram != 0)
                GL.DeleteProgram(shaderProgram);
            if (fragmentShaderObject != 0)
                GL.DeleteShader(fragmentShaderObject);

            if (FBOHandle != 0)
                GL.Ext.DeleteFramebuffers(1, ref FBOHandle);
        }

        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);

            float aspectRatio = Width / (float)Height;

            GL.MatrixMode(MatrixMode.Projection);
            scenePerspective = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, 1, 64);
            GL.LoadMatrix(ref scenePerspective);

            GL.MatrixMode(MatrixMode.Modelview);
            sceneModelView = Matrix4.LookAt(0, 0, 3, 0, 0, 0, 0, 1, 0);
            GL.LoadMatrix(ref sceneModelView);

            DeallocateLayerTextures();
            CreateLayerTextures(LayerCount, Width, Height);

            DrawIntoLayers();

            base.OnResize(e);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            if (Keyboard[Key.Escape])
            {
                this.Exit();
            }
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            //DrawIntoLayers();
            DisplayLayers();

            this.SwapBuffers();
        }

        #endregion

        #region Setting up layer textures

        private void CreateLayerTextures(int layerCount, int width, int height)
        {
            if ((layerCount < 1))
            {
                throw new ArgumentException("At least one layer is needed.");
            }

            // create textures
            GL.GenTextures(layerCount, ColorTextures);
            GL.GenTextures(layerCount, DepthTextures);

            for (int i = 0; i < layerCount; i++)
            {
                // setup COLOR texture
                GL.BindTexture(TextureTarget.Texture2D, ColorTextures[i]);
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba8, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

                // setup DEPTH texture
                GL.BindTexture(TextureTarget.Texture2D, DepthTextures[i]);
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.DepthComponent32f, width, height, 0, PixelFormat.DepthComponent, PixelType.Float, IntPtr.Zero);
                //GL.TexImage2D(TextureTarget.Texture2D, 0, (PixelInternalFormat)All.DepthComponent16, width, height, 0, PixelFormat.DepthComponent, PixelType.UnsignedShort, IntPtr.Zero);
                // things go horribly wrong if DepthComponent's Bitcount does not match the main Framebuffer's Depth
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

                // TODO: this must be enabled, find out why it leaves the depth buffer empty
                // it seems to work only for rectangle textures (not 2D)
                //GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureCompareMode, (int)TextureCompareMode.CompareRToTexture);
                //GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureCompareFunc, (int)DepthFunction.Lequal);
            }
        }

        private void DeallocateLayerTextures()
        {
            for (int i = 0; i < LayerCount; i++)
            {
                if (ColorTextures[i] != 0)
                    GL.DeleteTextures(1, ref ColorTextures[i]);

                if (DepthTextures[i] != 0)
                    GL.DeleteTextures(1, ref DepthTextures[i]);
            }
        }

        private void AttachLayerTextures(int layerIndex)
        {
            // DEBUG: detach previous textures
            //GL.Ext.FramebufferTexture2D(
            //    FramebufferTarget.FramebufferExt, FramebufferAttachment.ColorAttachment0Ext,
            //    TextureTarget.Texture2D, 0, 0);
            //GL.Ext.FramebufferTexture2D(
            //    FramebufferTarget.FramebufferExt, FramebufferAttachment.DepthAttachmentExt,
            //    TextureTarget.Texture2D, 0, 0);

            // attach color and depth from the selected layer
            GL.Ext.FramebufferTexture2D(
                FramebufferTarget.FramebufferExt, FramebufferAttachment.ColorAttachment0Ext,
                TextureTarget.Texture2D, ColorTextures[layerIndex], 0);
            GL.Ext.FramebufferTexture2D(
                FramebufferTarget.FramebufferExt, FramebufferAttachment.DepthAttachmentExt,
                TextureTarget.Texture2D, DepthTextures[layerIndex], 0);

            var result = GL.Ext.CheckFramebufferStatus(FramebufferTarget.FramebufferExt);
            if (result != FramebufferErrorCode.FramebufferCompleteExt)
            {
                throw new ApplicationException(string.Format("Bad FBO: {0}", result));
            }
        }

        private void UnbindFramebuffer()
        {
            //Console.WriteLine("UnbindFramebuffer");
            // detach textures - TODO: really needed?
            GL.Ext.FramebufferTexture2D(
                FramebufferTarget.FramebufferExt,
                FramebufferAttachment.ColorAttachment0Ext,
                TextureTarget.Texture2D,
                0, 0);
            GL.Ext.FramebufferTexture2D(
                FramebufferTarget.FramebufferExt,
                FramebufferAttachment.DepthAttachmentExt,
                TextureTarget.Texture2D,
                0, 0);
            GL.Ext.BindFramebuffer(FramebufferTarget.FramebufferExt, 0);
            //Console.WriteLine("UnbindFramebuffer: OK");
        }

        #endregion

        #region  Setting up shaders

        private void CreateShaders()
        {
            // create depth-peeling fragment shader
            using (StreamReader fs = new StreamReader("DepthPeeling.glsl"))
            {
                string fragmentSourceCode = fs.ReadToEnd();
                CreateFragmentShader(fragmentSourceCode, out fragmentShaderObject, out shaderProgram);
            }
        }

        void CreateFragmentShader(string sourceCode, out int shaderObject, out int program)
        {
            shaderObject = GL.CreateShader(ShaderType.FragmentShader);

            GL.ShaderSource(shaderObject, sourceCode);
            GL.CompileShader(shaderObject);
            string info;
            GL.GetShaderInfoLog(shaderObject, out info);
            int statusCode;
            GL.GetShader(shaderObject, ShaderParameter.CompileStatus, out statusCode);

            if (statusCode != 1)
            {
                throw new ApplicationException(info);
            }

            program = GL.CreateProgram();
            GL.AttachShader(program, shaderObject);

            GL.LinkProgram(program);
            //GL.UseProgram(program);
        }

        #endregion

        #region Drawing the scene and layers

        private void DrawOriginalScene(Scene scene)
        {
            //GL.MatrixMode(MatrixMode.Projection);
            //GL.LoadMatrix(ref scenePerspective);

            //GL.MatrixMode(MatrixMode.Modelview);
            //GL.LoadMatrix(ref sceneModelView);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.PushMatrix();
            GL.LoadIdentity();

            GL.MatrixMode(MatrixMode.Projection);
            GL.PushMatrix();
            GL.LoadIdentity();

            GL.Disable(EnableCap.Blend);

            GL.ClearColor(0, 0, 0, 1);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            scene.Draw();

            GL.PopMatrix();
            GL.MatrixMode(MatrixMode.Modelview);
            GL.PopMatrix();
        }

        private void DrawIntoLayers()
        {
            Scene scene = Scene.CreateRandomTriangles(10);

            GL.Disable(EnableCap.Blend);

            GL.Ext.BindFramebuffer(FramebufferTarget.FramebufferExt, FBOHandle);

            // draw the first layer without depth peeling

            GL.BindTexture(TextureTarget.Texture2D, 0);
            AttachLayerTextures(0);
            //GL.Enable(EnableCap.DepthTest);
            //GL.ClearDepth(1);

            DrawOriginalScene(scene);

            // draw the rest of layers with depth peeling

            // enable peeling shader
            GL.UseProgram(shaderProgram);

            // mark areas with no objects with depth 0
            //GL.Disable(EnableCap.DepthTest);
            //GL.ClearDepth(0);

            //GL.ActiveTexture(TextureUnit.Texture0);
            for (int i = 1; i < LayerCount; i++)
            {
                AttachLayerTextures(i);

                GL.BindTexture(TextureTarget.Texture2D, DepthTextures[i - 1]);
                DrawOriginalScene(scene);
            }
            // disable peeling shader
            GL.UseProgram(0);

            //GL.ClearDepth(1);
            //GL.Enable(EnableCap.DepthTest);

            UnbindFramebuffer(); // disable rendering into the FBO
        }

        private void DisplayLayers()
        {
            GL.MatrixMode(MatrixMode.Modelview);
            GL.PushMatrix();
            GL.LoadIdentity();

            GL.MatrixMode(MatrixMode.Projection);
            GL.PushMatrix();
            GL.LoadIdentity();

            GL.ClearColor(0f, 0f, 0f, 1f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            if (showBlendedLayers)
            {
                GL.Disable(EnableCap.Blend);

                // back-most layer must be fully opaque
                GL.BindTexture(TextureTarget.Texture2D, ColorTextures[LayerCount - 1]);
                DrawFullScreenQuad();

                GL.Enable(EnableCap.Blend);

                for (int i = LayerCount - 2; i >= 0; i--)
                {
                    GL.BindTexture(TextureTarget.Texture2D, ColorTextures[i]);
                    DrawFullScreenQuad();
                }
            }
            else
            {
                GL.Disable(EnableCap.Blend);
                if (showColorTexture)
                {
                    GL.BindTexture(TextureTarget.Texture2D, ColorTextures[activeLayer]);
                }
                else
                {
                    GL.BindTexture(TextureTarget.Texture2D, DepthTextures[activeLayer]);
                }

                DrawFullScreenQuad();
            }

            GL.PopMatrix();
            GL.MatrixMode(MatrixMode.Modelview);
            GL.PopMatrix();

            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        private static void DrawFullScreenQuad()
        {
            GL.Begin(BeginMode.Quads);
            {
                GL.TexCoord2(0, 1); GL.Vertex2(-1.0f, 1.0f);
                GL.TexCoord2(0, 0); GL.Vertex2(-1.0f, -1.0f);
                GL.TexCoord2(1, 0); GL.Vertex2(1.0f, -1.0f);
                GL.TexCoord2(1, 1); GL.Vertex2(1.0f, 1.0f);
            }
            GL.End();
        }

        #endregion

        #region User interaction

        private void KeyUp(object sender, KeyboardKeyEventArgs e)
        {
            if (e.Key == Key.R)
            {
                // recompute geometry and redraw layers
                DrawIntoLayers();
            }
            else if (e.Key == Key.P)
            {
                // show previous layer
                activeLayer = Math.Max(activeLayer - 1, 0);
            }
            else if (e.Key == Key.N)
            {
                // show next layer
                activeLayer = Math.Min(activeLayer + 1, LayerCount - 1);
            }
            else if (e.Key == Key.T)
            {
                // show color or depth texture
                showColorTexture = !showColorTexture;
            }
            else if (e.Key == Key.B)
            {
                // show color or depth texture
                showBlendedLayers = !showBlendedLayers;
            }
            else if (e.Key == Key.F5)
            {
                SaveScreenshot();
            }
            else if (e.Key == Key.F6)
            {
                SaveColorScreenshot(activeLayer, GetScreenshotId());
            }
            else if (e.Key == Key.F7)
            {
                SaveDepthScreenshotByte(activeLayer, GetScreenshotId());
            }
            else if (e.Key == Key.F8)
            {
                string id = GetScreenshotId();
                SaveColorScreenshot(activeLayer, id);
                SaveDepthScreenshotByte(activeLayer, id);
            }
            else if (e.Key == Key.F9)
            {
                string id = GetScreenshotId();
                SaveScreenshotOfAllLayers(id);
            }
            else if (e.Key == Key.F11)
            {
                bool isFullscreen = (WindowState == WindowState.Fullscreen);
                WindowState = isFullscreen ? WindowState.Normal : WindowState.Fullscreen;
            }
        }

        private static string GetScreenshotId()
        {
            return DateTime.Now.ToString("yyyy-MM-dd_hh-mm-ss");
        }

        #endregion

        #region Saving images to files

        private void SaveScreenshotOfAllLayers(string id)
        {
            for (int layer = 0; layer < LayerCount; layer++)
            {
                SaveColorScreenshot(layer, id);
                SaveDepthScreenshotByte(layer, id);
            }
        }

        private void SaveColorScreenshot(int layer, string id)
        {
            using (Bitmap bmp = new Bitmap(Width, Height))
            {
                System.Drawing.Imaging.BitmapData data =
                    bmp.LockBits(new System.Drawing.Rectangle(0, 0, Width, Height),
                                 System.Drawing.Imaging.ImageLockMode.WriteOnly,
                                 System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                GL.BindTexture(TextureTarget.Texture2D, ColorTextures[layer]);
                GL.GetTexImage(TextureTarget.Texture2D, 0, PixelFormat.Bgr, PixelType.UnsignedByte, data.Scan0);
                GL.BindTexture(TextureTarget.Texture2D, 0);
                bmp.UnlockBits(data);
                bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);
                bmp.Save(string.Format("{0}_color_{1}.png", id, layer), System.Drawing.Imaging.ImageFormat.Png);
            }
        }

        private void SaveDepthScreenshotByte(int layer, string id)
        {
            // allocate 32-bit unit unmanaged array to grab the depth buffer
            IntPtr depthBufferBytePtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(Byte)) * Width * Height);
            GL.BindTexture(TextureTarget.Texture2D, DepthTextures[layer]);
            GL.GetTexImage(TextureTarget.Texture2D, 0, PixelFormat.DepthComponent, PixelType.UnsignedByte, depthBufferBytePtr);
            GL.BindTexture(TextureTarget.Texture2D, 0);
            // - convert single bytes to byte RGB triplets with the same value
            using (Bitmap bmp = new Bitmap(Width, Height))
            {
                System.Drawing.Imaging.BitmapData bmpDataPtr =
                    bmp.LockBits(new System.Drawing.Rectangle(0, 0, Width, Height),
                                 System.Drawing.Imaging.ImageLockMode.WriteOnly,
                                 System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                unsafe
                {
                    int inputStride = Width;
                    for (int y = 0; y < Height; y++)
                    {
                        byte* outputRow = (byte*)bmpDataPtr.Scan0 + (y * bmpDataPtr.Stride);
                        byte* inputRow = (byte*)depthBufferBytePtr + (y * inputStride);
                        for (int x = 0; x < Width; x++)
                        {
                            byte colorByte = inputRow[x];
                            for (int band = 0; band < 3; band++)
                            {
                                outputRow[x * 3 + band] = colorByte;
                            }
                        }
                    }
                }
                bmp.UnlockBits(bmpDataPtr);
                bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);
                bmp.Save(string.Format("{0}_depth_{1}.png", id, layer), System.Drawing.Imaging.ImageFormat.Png);
            }
        }

        private void SaveDepthScreenshotUInt32(int layer, string id)
        {
            // allocate 32-bit unit unmanaged array to grab the depth buffer
            IntPtr depthBufferUInt32Ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(UInt32)) * Width * Height);
            GL.BindTexture(TextureTarget.Texture2D, DepthTextures[activeLayer]);
            // TODO:
            // - specify PixelType.UnsignedByte in order to avoid the further conversion by hand!
            GL.GetTexImage(TextureTarget.Texture2D, 0, PixelFormat.DepthComponent, PixelType.UnsignedInt, depthBufferUInt32Ptr);
            GL.BindTexture(TextureTarget.Texture2D, 0);
            // -convert 32-bit uints to (3x grayscale) 8-bit RGB values
            //   in order to store the pixels to an ordinary image file
            //   - for now the loss of precision doesn't matter)
            //   - later the buffer can be stored in a FloatMap without
            //     any precision loss
            using (Bitmap bmp = new Bitmap(Width, Height))
            {
                System.Drawing.Imaging.BitmapData bmpDataPtr =
                    bmp.LockBits(new System.Drawing.Rectangle(0, 0, Width, Height),
                                 System.Drawing.Imaging.ImageLockMode.WriteOnly,
                                 System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                unsafe
                {
                    int inputStride = Width;
                    float conversionFactor = 255 / (float)UInt32.MaxValue;
                    for (int y = 0; y < Height; y++)
                    {
                        byte* outputRow = (byte*)bmpDataPtr.Scan0 + (y * bmpDataPtr.Stride);
                        UInt32* inputRow = (UInt32*)depthBufferUInt32Ptr + (y * inputStride);
                        for (int x = 0; x < Width; x++)
                        {
                            byte color8bit = (byte)(inputRow[x] * conversionFactor);
                            for (int band = 0; band < 3; band++)
                            {
                                outputRow[x * 3 + band] = color8bit;
                            }
                        }
                    }
                }
                bmp.UnlockBits(bmpDataPtr);
                bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);
                bmp.Save(string.Format("{0}_depth_{1}.png", id, layer), System.Drawing.Imaging.ImageFormat.Png);
            }
        }

        private void SaveScreenshot()
        {
            using (Bitmap bmp = new Bitmap(this.Width, this.Height))
            {
                System.Drawing.Imaging.BitmapData data =
                    bmp.LockBits(new System.Drawing.Rectangle(0, 0, this.Width, this.Height),
                                 System.Drawing.Imaging.ImageLockMode.WriteOnly,
                                 System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                GL.ReadPixels(0, 0, this.Width, this.Height,
                              OpenTK.Graphics.OpenGL.PixelFormat.Bgr,
                              OpenTK.Graphics.OpenGL.PixelType.UnsignedByte,
                              data.Scan0);
                bmp.UnlockBits(data);
                bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);
                bmp.Save(string.Format("{0}_screenshot.png", GetScreenshotId()), System.Drawing.Imaging.ImageFormat.Png);
            }
        }

        #endregion

        private class Scene
        {
            int vertexCount;
            Vector4[] colors;
            Vector3[] vertices;

            public static Scene CreateRandomTriangles(int triangleCount)
            {
                Scene scene = new Scene();
                scene.vertexCount = 3 * triangleCount;
                scene.colors = new Vector4[scene.vertexCount];
                scene.vertices = new Vector3[scene.vertexCount];
                for (int i = 0; i < scene.vertexCount; i++)
                {
                    scene.colors[i] = new Vector4(GetRandom0to1(), GetRandom0to1(), GetRandom0to1(), 0.5f);
                    scene.vertices[i] = new Vector3(GetRandom(), GetRandom(), GetRandom());
                }
                return scene;
            }

            public void Draw()
            {
                GL.Begin(BeginMode.Triangles);
                {
                    for (int i = 0; i < colors.Length; i++)
                    {
                        GL.Color4(colors[i]);
                        GL.Vertex3(vertices[i]);
                    }
                }
                GL.End();
            }

            static Random rnd = new Random();
            public const float scale = 2f;

            /// <summary>Returns a random Float in the range [-0.5*scale..+0.5*scale]</summary>
            public static float GetRandom()
            {
                return (float)(rnd.NextDouble() - 0.5) * scale;
            }

            /// <summary>Returns a random Float in the range [0..1]</summary>
            public static float GetRandom0to1()
            {
                return (float)rnd.NextDouble();
            }
        }
    }
}