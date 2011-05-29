#region --- License ---
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
        static readonly int LayerCount = 5;

        // index of currently displayed layer [0; LayerCount - 1]
        int activeLayer = 0;
        // show color (true) or depth texture?
        bool showColorTexture = true;

        public DepthPeeling()
            : base(512, 512)
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

            CreateShaders();

            CreateLayerTextures(LayerCount, Width, Height);

            // Create a FBO and attach the textures
            GL.Ext.GenFramebuffers(1, out FBOHandle);
            BindFramebufferWithLayerTextures(FBOHandle, activeLayer);

            DrawIntoLayers();

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
            for (int i = 0; i < layerCount; i++)
            {
                // create and setup COLOR texture
                GL.GenTextures(layerCount, out ColorTextures[i]);

                GL.BindTexture(TextureTarget.Texture2D, ColorTextures[i]);
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba8, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToBorder);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToBorder);

                // create and setup DEPTH texture
                GL.GenTextures(LayerCount, out DepthTextures[i]);

                GL.BindTexture(TextureTarget.Texture2D, DepthTextures[i]);
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.DepthComponent32f, width, height, 0, PixelFormat.DepthComponent, PixelType.Float, IntPtr.Zero);
                //GL.TexImage2D(TextureTarget.Texture2D, 0, (PixelInternalFormat)All.DepthComponent16, width, height, 0, PixelFormat.DepthComponent, PixelType.UnsignedShort, IntPtr.Zero);
                // things go horribly wrong if DepthComponent's Bitcount does not match the main Framebuffer's Depth
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

                // TODO: this must be enabled, find out why it leaves the depth buffer empty
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

        private void BindFramebufferWithLayerTextures(uint fboHandle, int layerIndex)
        {
            GL.Ext.BindFramebuffer(FramebufferTarget.FramebufferExt, fboHandle);
            GL.Ext.FramebufferTexture2D(
                FramebufferTarget.FramebufferExt,
                FramebufferAttachment.ColorAttachment0Ext,
                TextureTarget.Texture2D,
                ColorTextures[layerIndex], 0);
            GL.Ext.FramebufferTexture2D(
                FramebufferTarget.FramebufferExt,
                FramebufferAttachment.DepthAttachmentExt,
                TextureTarget.Texture2D,
                DepthTextures[layerIndex], 0);

            #region Test for Error

            switch (GL.Ext.CheckFramebufferStatus(FramebufferTarget.FramebufferExt))
            {
                case FramebufferErrorCode.FramebufferCompleteExt:
                    {
                        Console.WriteLine("FBO: The framebuffer is complete and valid for rendering.");
                        break;
                    }
                case FramebufferErrorCode.FramebufferIncompleteAttachmentExt:
                    {
                        Console.WriteLine("FBO: One or more attachment points are not framebuffer attachment complete. This could mean there’s no texture attached or the format isn’t renderable. For color textures this means the base format must be RGB or RGBA and for depth textures it must be a DEPTH_COMPONENT format. Other causes of this error are that the width or height is zero or the z-offset is out of range in case of render to volume.");
                        break;
                    }
                case FramebufferErrorCode.FramebufferIncompleteMissingAttachmentExt:
                    {
                        Console.WriteLine("FBO: There are no attachments.");
                        break;
                    }
                /* case  FramebufferErrorCode.GL_FRAMEBUFFER_INCOMPLETE_DUPLICATE_ATTACHMENT_EXT: 
                     {
                         Console.WriteLine("FBO: An object has been attached to more than one attachment point.");
                         break;
                     }*/
                case FramebufferErrorCode.FramebufferIncompleteDimensionsExt:
                    {
                        Console.WriteLine("FBO: Attachments are of different size. All attachments must have the same width and height.");
                        break;
                    }
                case FramebufferErrorCode.FramebufferIncompleteFormatsExt:
                    {
                        Console.WriteLine("FBO: The color attachments have different format. All color attachments must have the same format.");
                        break;
                    }
                case FramebufferErrorCode.FramebufferIncompleteDrawBufferExt:
                    {
                        Console.WriteLine("FBO: An attachment point referenced by GL.DrawBuffers() doesn’t have an attachment.");
                        break;
                    }
                case FramebufferErrorCode.FramebufferIncompleteReadBufferExt:
                    {
                        Console.WriteLine("FBO: The attachment point referenced by GL.ReadBuffers() doesn’t have an attachment.");
                        break;
                    }
                case FramebufferErrorCode.FramebufferUnsupportedExt:
                    {
                        Console.WriteLine("FBO: This particular FBO configuration is not supported by the implementation.");
                        break;
                    }
                default:
                    {
                        Console.WriteLine("FBO: Status unknown. (yes, this is really bad.)");
                        break;
                    }
            }

            // using FBO might have changed states, e.g. the FBO might not support stereoscopic views or double buffering
            int[] queryinfo = new int[6];
            GL.GetInteger(GetPName.MaxColorAttachmentsExt, out queryinfo[0]);
            GL.GetInteger(GetPName.AuxBuffers, out queryinfo[1]);
            GL.GetInteger(GetPName.MaxDrawBuffers, out queryinfo[2]);
            GL.GetInteger(GetPName.Stereo, out queryinfo[3]);
            GL.GetInteger(GetPName.Samples, out queryinfo[4]);
            GL.GetInteger(GetPName.Doublebuffer, out queryinfo[5]);
            Console.WriteLine("max. ColorBuffers: " + queryinfo[0] + " max. AuxBuffers: " + queryinfo[1] + " max. DrawBuffers: " + queryinfo[2] +
                               "\nStereo: " + queryinfo[3] + " Samples: " + queryinfo[4] + " DoubleBuffer: " + queryinfo[5]);

            Console.WriteLine("Last GL Error: " + GL.GetError());

            #endregion Test for Error

        }

        private void UnbindFramebuffer()
        {
            GL.Ext.BindFramebuffer(FramebufferTarget.FramebufferExt, 0);
        }

        #endregion

        #region  Setting up shaders

        private void CreateShaders()
        {
            // create depth-peeling fragment shader
            using (StreamReader fs = new StreamReader("Data/Shaders/DepthPeeling.glsl"))
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

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(-1, 1, -1, 1, 1, -1);

            GL.MatrixMode(MatrixMode.Modelview);
            Matrix4 lookat = Matrix4.LookAt(0, 0, 0, 0, 0, -1, 0, 1, 0);
            GL.LoadMatrix(ref lookat);

            GL.PushAttrib(AttribMask.ViewportBit);
            {
                GL.Viewport(0, 0, Width, Height);

                // clear the screen in red, to make it very obvious what the clear affected. only the FBO, not the real framebuffer
                GL.ClearColor(1f, 0f, 0f, 0f);
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                // smack 50 random triangles into the FBO's textures
                GL.Begin(BeginMode.Triangles);
                {
                    scene.Draw();

                    //GL.Color3(0.5, 0, 0); GL.Vertex3(0, 0, 0);
                    //GL.Color3(0, 0.5, 0); GL.Vertex3(1, 1, 0);
                    //GL.Color3(0, 0, 0.5); GL.Vertex3(0, 1, 0);

                    //GL.Color3(0.5, 0, 0); GL.Vertex3(0, 0, -1);
                    //GL.Color3(0, 0.5, 0); GL.Vertex3(1, 1, 0);
                    //GL.Color3(0, 0, 0.5); GL.Vertex3(0, 1, 1);

                    //GL.Color3(0.5, 0, 0); GL.Vertex3(-1, -1, -1);
                    //GL.Color3(0, 0.5, 0); GL.Vertex3(-1, 1, -1);
                    //GL.Color3(0, 0, 0.5); GL.Vertex3(1, -1, 1);
                }
                GL.End();
            }
            GL.PopAttrib();
        }

        private void DrawIntoLayers()
        {
            Scene scene = Scene.CreateRandomTriangles(10);

            GL.Disable(EnableCap.Blend);

            GL.BindTexture(TextureTarget.Texture2D, 0);
            BindFramebufferWithLayerTextures(FBOHandle, 0);
            DrawOriginalScene(scene);

            // enable peeling shader
            GL.UseProgram(shaderProgram);

            //GL.ActiveTexture(TextureUnit.Texture0);
            for (int i = 1; i < LayerCount; i++)
            {
                BindFramebufferWithLayerTextures(FBOHandle, i);
                GL.BindTexture(TextureTarget.Texture2D, DepthTextures[i - 1]);
                DrawOriginalScene(scene);
            }
            // disable peeling shader
            GL.UseProgram(0);

            UnbindFramebuffer(); // disable rendering into the FBO
        }

        private void DisplayLayers()
        {
            //GL.MatrixMode(MatrixMode.Projection);
            //GL.LoadMatrix(ref scenePerspective);

            //GL.MatrixMode(MatrixMode.Modelview);
            //GL.LoadMatrix(ref sceneModelView);

            GL.Enable(EnableCap.Blend);

            GL.Viewport(0, 0, Width, Height);

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(-1, 1, -1, 1, 1, -1);

            GL.MatrixMode(MatrixMode.Modelview);
            Matrix4 lookat = Matrix4.LookAt(0, 0, 0, 0, 0, -1, 0, 1, 0);
            GL.LoadMatrix(ref lookat);

            GL.ClearColor(.1f, .2f, .3f, 0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.Color3(1f, 1f, 1f);

            GL.PushMatrix();
            {
                if (showColorTexture)
                {
                    GL.BindTexture(TextureTarget.Texture2D, ColorTextures[activeLayer]);
                }
                else
                {
                    GL.BindTexture(TextureTarget.Texture2D, DepthTextures[activeLayer]);
                }
                GL.Begin(BeginMode.Quads);
                {
                    GL.TexCoord2(0.0f, 1.0f); GL.Vertex2(-1.0f, 1.0f);
                    GL.TexCoord2(0.0f, 0.0f); GL.Vertex2(-1.0f, -1.0f);
                    GL.TexCoord2(1.0f, 0.0f); GL.Vertex2(1.0f, -1.0f);
                    GL.TexCoord2(1.0f, 1.0f); GL.Vertex2(1.0f, 1.0f);
                }
                GL.End();
            }
            GL.PopMatrix();

            GL.BindTexture(TextureTarget.Texture2D, 0);
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
            else if (e.Key == Key.F5)
            {
                SaveScreenshot();
            }
            else if (e.Key == Key.F6)
            {
                SaveColorScreenshot();
            }
            else if (e.Key == Key.F7)
            {
                SaveDepthScreenshot();
            }
            else if (e.Key == Key.F8)
            {
                SaveColorScreenshot();
                SaveDepthScreenshot();
            }
        }

        #endregion

        #region Saving images to files

        private void SaveColorScreenshot()
        {
            using (Bitmap bmp = new Bitmap(Width, Height))
            {
                System.Drawing.Imaging.BitmapData data =
                    bmp.LockBits(new System.Drawing.Rectangle(0, 0, Width, Height),
                                 System.Drawing.Imaging.ImageLockMode.WriteOnly,
                                 System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                GL.BindTexture(TextureTarget.Texture2D, ColorTextures[activeLayer]);
                GL.GetTexImage(TextureTarget.Texture2D, 0, PixelFormat.Bgr, PixelType.UnsignedByte, data.Scan0);
                GL.BindTexture(TextureTarget.Texture2D, 0);
                bmp.UnlockBits(data);
                bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);
                bmp.Save("color_" + DateTime.Now.ToString("yyyy-MM-dd_hh-mm-ss") + ".png", System.Drawing.Imaging.ImageFormat.Png);
            }
        }

        private void SaveDepthScreenshot()
        {
            // allocate 32-bit unit unmanaged array to grab the depth buffer
            IntPtr depthBufferUInt32Ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(UInt32)) * Width * Height);
            GL.BindTexture(TextureTarget.Texture2D, DepthTextures[activeLayer]);
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
                bmp.Save("depth_" + DateTime.Now.ToString("yyyy-MM-dd_hh-mm-ss") + ".png", System.Drawing.Imaging.ImageFormat.Png);
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
                bmp.Save("screenshot_" + DateTime.Now.ToString("yyyy-MM-dd_hh-mm-ss") + ".png", System.Drawing.Imaging.ImageFormat.Png);
            }
        }

        #endregion

        private class Scene
        {
            int vertexCount;
            Vector3[] colors;
            Vector3[] vertices;

            public static Scene CreateRandomTriangles(int triangleCount)
            {
                Scene scene = new Scene();
                scene.vertexCount = 3 * triangleCount;
                scene.colors = new Vector3[scene.vertexCount];
                scene.vertices = new Vector3[scene.vertexCount];
                for (int i = 0; i < scene.vertexCount; i++)
                {
                    scene.colors[i] = new Vector3(0, GetRandom0to1(), GetRandom0to1());
                    scene.vertices[i] = new Vector3(GetRandom(), GetRandom(), GetRandom());
                }
                return scene;
            }

            public void Draw()
            {
                for (int i = 0; i < colors.Length; i++)
                {
                    GL.Color3(colors[i]);
                    GL.Vertex3(vertices[i]);
                }
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