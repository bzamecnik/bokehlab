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

namespace BokehLab.LrtfVisualizer
{
    public class LrtfVisualizerGpu : GameWindow
    {
        int textureSize = 128;

        public LrtfVisualizerGpu()
            : base(512, 512)
        {
        }

        int Texture;
        float UniformZCoord = 0;
        float zCoordStep = 0;

        Vector4 UniformValueMask = Vector4.UnitX;
        LrtfComponent selectedLrtfComponent = LrtfComponent.PositionTheta;

        string lrtfFilename = @"..\..\..\lrtf_double_gauss_128.bin";

        int vertexShaderObject, fragmentShaderObject, shaderProgram;

        public static void RunExample()
        {
            using (LrtfVisualizerGpu example = new LrtfVisualizerGpu())
            {
                example.Run(30.0, 0.0);
            }
        }

        #region GameWindow event handlers

        protected override void OnLoad(EventArgs e)
        {
            GL.Enable(EnableCap.DepthTest);
            GL.ClearDepth(1.0f);
            GL.DepthFunc(DepthFunction.Lequal);

            GL.Disable(EnableCap.CullFace);
            GL.PolygonMode(MaterialFace.Back, PolygonMode.Fill);

            GL.Enable(EnableCap.Texture2D);

            GL.Enable(EnableCap.Texture3DExt);

            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusDstAlpha);

            using (StreamReader vs = new StreamReader("Data/Shaders/VertexShader.glsl"))
            using (StreamReader fs = new StreamReader("Data/Shaders/FragmentShader.glsl"))
                CreateShaders(vs.ReadToEnd(), fs.ReadToEnd(),
                    out vertexShaderObject, out fragmentShaderObject,
                    out shaderProgram);

            //Create2dTexture(Width, Height);

            //Create3dTexture(textureSize, textureSize, textureSize);

            textureSize = Create3dTextureFromLrtf(lrtfFilename);

            zCoordStep = 1 / (float)textureSize;

            Keyboard.KeyUp += KeyUp;
        }

        protected override void OnUnload(EventArgs e)
        {
            // Clean up what we allocated before exiting
            if (Texture != 0)
                GL.DeleteTexture(Texture);

            if (shaderProgram != 0)
                GL.DeleteProgram(shaderProgram);
            if (fragmentShaderObject != 0)
                GL.DeleteShader(fragmentShaderObject);
        }

        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);

            //float aspectRatio = Width / (float)Height;

            //GL.MatrixMode(MatrixMode.Projection);
            //scenePerspective = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, 1, 64);
            //GL.LoadMatrix(ref scenePerspective);

            //GL.MatrixMode(MatrixMode.Modelview);
            //sceneModelView = Matrix4.LookAt(0, 0, 3, 0, 0, 0, 0, 1, 0);
            //GL.LoadMatrix(ref sceneModelView);

            Draw();

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
            this.Title = "FPS: " + 1 / e.Time;

            Draw();

            this.SwapBuffers();
        }

        #endregion

        #region Setting up layer textures

        private void Create2dTexture(int width, int height)
        {
            Texture = GL.GenTexture();

            //// one-channel byte 2D texture
            //IntPtr texturePtr = CreateOneChannel2dTexture(width, height);
            //GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Luminance8, width, height, 0, PixelFormat.Luminance, PixelType.UnsignedByte, texturePtr);

            //// RGB byte 2D texture
            IntPtr texturePtr = CreateRgb2dTexture(width, height);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb8, width, height, 0, PixelFormat.Rgb, PixelType.UnsignedByte, texturePtr);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
        }

        private static IntPtr CreateOneChannel2dTexture(int width, int height)
        {
            // allocate texture memory on host
            IntPtr texturePtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(Byte)) * width * height);
            unsafe
            {
                int stride = width;
                for (int y = 0; y < height; y++)
                {
                    byte* row = (byte*)texturePtr + (y * stride);
                    for (int x = 0; x < width; x++)
                    {
                        float intensity = (float)(0.25 * (2 + Math.Cos(0.03 * x) + Math.Cos(0.03 * y)));
                        row[x] = (byte)(255 * intensity);
                    }
                }
            }
            return texturePtr;
        }

        private static IntPtr CreateRgb2dTexture(int width, int height)
        {
            int bands = 3;
            IntPtr texturePtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(Byte)) * width * height * bands);
            unsafe
            {
                int stride = bands * width;
                for (int y = 0; y < height; y++)
                {
                    byte* row = (byte*)texturePtr + (y * stride);
                    for (int x = 0; x < width; x++)
                    {
                        for (int band = 0; band < bands; band++)
                        {
                            float intensity = (float)(0.25 * (2 + Math.Cos(0.03 * x + band) + Math.Cos(0.03 * y + band)));
                            row[x * bands + band] = (byte)(255 * intensity);
                        }
                    }
                }
            }
            return texturePtr;
        }

        private void Create3dTexture(int width, int height, int depth)
        {
            Texture = GL.GenTexture();

            //// one-channel byte 2D texture
            //IntPtr texturePtr = CreateOneChannel3dTexture(width, height, depth);
            //GL.TexImage3D(TextureTarget.Texture3D, 0, PixelInternalFormat.Luminance8,
            //    width, height, depth, 0, PixelFormat.Luminance, PixelType.UnsignedByte, texturePtr);

            //// RGB byte 2D texture
            //IntPtr texturePtr = CreateRgb3dTexture(width, height, depth);
            //GL.TexImage3D(TextureTarget.Texture3D, 0, PixelInternalFormat.Rgb8,
            //    width, height, depth, 0, PixelFormat.Rgb, PixelType.UnsignedByte, texturePtr);

            //// RGB float 2D texture
            //IntPtr texturePtr = CreateRgb3dFloatTexture(width, height, depth);
            //GL.TexImage3D(TextureTarget.Texture3D, 0, PixelInternalFormat.Rgb8,
            //    width, height, depth, 0, PixelFormat.Rgb, PixelType.Float, texturePtr);

            IntPtr texturePtr = CreateRgb3dFloatTexture(width, height, depth);
            GL.TexImage3D(TextureTarget.Texture3D, 0, PixelInternalFormat.Rgb8,
                width, height, depth, 0, PixelFormat.Rgb, PixelType.Float, texturePtr);

            GL.TexParameter(TextureTarget.Texture3D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture3D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture3D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture3D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture3D, TextureParameterName.TextureWrapR, (int)TextureWrapMode.ClampToEdge);
        }

        private int Create3dTextureFromLrtf(string filename)
        {
            IntPtr texturePtr;
            int size = LoadLrtfFloat4To3dTexture(filename, out texturePtr);
            GL.TexImage3D(TextureTarget.Texture3D, 0, PixelInternalFormat.Rgba,
                size, size, size, 0, PixelFormat.Rgba, PixelType.Float, texturePtr);

            GL.TexParameter(TextureTarget.Texture3D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture3D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture3D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture3D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture3D, TextureParameterName.TextureWrapR, (int)TextureWrapMode.ClampToEdge);

            return size;

        }

        private static IntPtr CreateOneChannel3dTexture(int width, int height, int depth)
        {
            // allocate texture memory on host
            IntPtr texturePtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(Byte)) * width * height * depth);
            unsafe
            {
                int zStride = width * height;
                for (int z = 0; z < depth; z++)
                {
                    int yStride = width;
                    for (int y = 0; y < height; y++)
                    {
                        byte* row = (byte*)texturePtr + (y * yStride) + (z * zStride);
                        for (int x = 0; x < width; x++)
                        {
                            //float intensity = (float)(0.25 * (2 + Math.Cos(0.03 * x) + Math.Cos(0.03 * y)));
                            float intensity = (float)(x / (float)width * y / (float)height * z / (float)depth);
                            row[x] = (byte)(255 * intensity);
                        }
                    }
                }
            }
            return texturePtr;
        }

        private static IntPtr CreateRgb3dTexture(int width, int height, int depth)
        {
            int bands = 3;
            // allocate texture memory on host
            IntPtr texturePtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(Byte)) * width * height * depth * bands);
            unsafe
            {
                int zStride = bands * width * height;
                for (int z = 0; z < depth; z++)
                {
                    int yStride = bands * width;
                    for (int y = 0; y < height; y++)
                    {
                        byte* row = (byte*)texturePtr + (y * yStride) + (z * zStride);
                        for (int x = 0; x < width; x++)
                        {
                            //float intensity = (float)(0.25 * (2 + Math.Cos(0.03 * x) + Math.Cos(0.03 * y)));
                            float r = x / (float)width;
                            float g = y / (float)height;
                            float b = z / (float)depth;
                            row[bands * x] = (byte)(255 * r);
                            row[bands * x + 1] = (byte)(255 * g);
                            row[bands * x + 2] = (byte)(255 * b);
                        }
                    }
                }
            }
            return texturePtr;
        }

        private static IntPtr CreateRgb3dFloatTexture(int width, int height, int depth)
        {
            int bands = 3;
            // allocate texture memory on host
            IntPtr texturePtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(float)) * width * height * depth * bands);
            unsafe
            {
                int zStride = bands * width * height;
                for (int z = 0; z < depth; z++)
                {
                    int yStride = bands * width;
                    for (int y = 0; y < height; y++)
                    {
                        float* row = (float*)texturePtr + (y * yStride) + (z * zStride);
                        for (int x = 0; x < width; x++)
                        {
                            //float intensity = (float)(0.25 * (2 + Math.Cos(0.03 * x) + Math.Cos(0.03 * y)));
                            //float r = x / (float)width;
                            //float g = y / (float)height;
                            //float b = z / (float)depth;
                            float r = (float)(0.5 * (1 + Math.Cos(0.1 * x)));
                            float g = (float)(0.5 * (1 + Math.Cos(0.1 * y)));
                            float b = (float)(0.5 * (1 + Math.Cos(0.1 * z)));
                            row[bands * x] = r;
                            row[bands * x + 1] = g;
                            row[bands * x + 2] = b;
                        }
                    }
                }
            }
            return texturePtr;
        }

        public int LoadLrtfFloat4To3dTexture(string filename, out IntPtr texturePtr)
        {
            using (BinaryReader bw = new BinaryReader(File.Open(filename, FileMode.Open, FileAccess.Read)))
            {
                int size = bw.ReadUInt16();

                int bands = 4;
                texturePtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(float)) * size * size * size * bands);

                unsafe
                {
                    int zStride = bands * size * size;
                    for (int z = 0; z < size; z++)
                    {
                        int yStride = bands * size;
                        for (int y = 0; y < size; y++)
                        {
                            for (int x = 0; x < size; x++)
                            {
                                float* row = (float*)texturePtr + (y * yStride) + (z * zStride);
                                for (int band = 0; band < bands; band++)
                                {
                                    float value = (float)bw.ReadDouble();
                                    row[bands * x + band] = value;
                                }
                            }
                        }
                    }
                }
                return size;
            }
        }

        private void DeallocateTextures()
        {
            if (Texture != 0)
            {
                GL.DeleteTexture(Texture);
            }
        }

        #endregion

        #region  Setting up shaders

        void CreateShaders(string vs, string fs,
            out int vertexObject, out int fragmentObject,
            out int program)
        {
            int statusCode;
            string info;

            vertexObject = GL.CreateShader(ShaderType.VertexShader);
            fragmentObject = GL.CreateShader(ShaderType.FragmentShader);

            // Compile vertex shader
            GL.ShaderSource(vertexObject, vs);
            GL.CompileShader(vertexObject);
            GL.GetShaderInfoLog(vertexObject, out info);
            GL.GetShader(vertexObject, ShaderParameter.CompileStatus, out statusCode);

            if (statusCode != 1)
                throw new ApplicationException(info);

            // Compile vertex shader
            GL.ShaderSource(fragmentObject, fs);
            GL.CompileShader(fragmentObject);
            GL.GetShaderInfoLog(fragmentObject, out info);
            GL.GetShader(fragmentObject, ShaderParameter.CompileStatus, out statusCode);

            if (statusCode != 1)
                throw new ApplicationException(info);

            program = GL.CreateProgram();
            GL.AttachShader(program, fragmentObject);
            GL.AttachShader(program, vertexObject);

            GL.LinkProgram(program);
            GL.UseProgram(program);
        }

        #endregion

        #region Drawing the scene

        private void Draw()
        {
            //GL.MatrixMode(MatrixMode.Projection);
            //GL.LoadMatrix(ref scenePerspective);

            //GL.MatrixMode(MatrixMode.Modelview);
            //GL.LoadMatrix(ref sceneModelView);

            GL.Viewport(0, 0, Width, Height);

            //GL.MatrixMode(MatrixMode.Projection);
            //GL.LoadIdentity();
            //GL.Ortho(-1, 1, -1, 1, 1, -1);

            //GL.MatrixMode(MatrixMode.Modelview);
            //Matrix4 lookat = Matrix4.LookAt(0, 0, 0, 0, 0, -1, 0, 1, 0);
            //GL.LoadMatrix(ref lookat);

            GL.ClearColor(0f, 0f, 0f, 1f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            //GL.Color3(1f, 1f, 1f);


            //GL.Disable(EnableCap.Blend);

            // back-most layer must be fully opaque
            GL.BindTexture(TextureTarget.Texture2D, Texture);
            GL.UseProgram(shaderProgram);

            //GL.Uniform1(GL.GetUniformLocation(shaderProgram, "foo2dTexture"), Texture);
            GL.Uniform1(GL.GetUniformLocation(shaderProgram, "foo3dTexture"), Texture);
            GL.Uniform1(GL.GetUniformLocation(shaderProgram, "zCoord"), UniformZCoord);
            GL.Uniform4(GL.GetUniformLocation(shaderProgram, "valueMask"), UniformValueMask);

            DrawFullScreenQuad();
            GL.UseProgram(0);

            GL.Enable(EnableCap.Blend);

            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        private static void DrawFullScreenQuad()
        {
            GL.Begin(BeginMode.Quads);
            {
                // texture X goes from left to right, Y goes from bottom to top
                GL.TexCoord2(0, 1); GL.Vertex2(-1.0f, 1.0f); // top left
                GL.TexCoord2(0, 0); GL.Vertex2(-1.0f, -1.0f); // bottom left
                GL.TexCoord2(1, 0); GL.Vertex2(1.0f, -1.0f); // bottom right
                GL.TexCoord2(1, 1); GL.Vertex2(1.0f, 1.0f); // top right
            }
            GL.End();
        }

        #endregion

        #region User interaction

        private void KeyUp(object sender, KeyboardKeyEventArgs e)
        {
            if (e.Key == Key.F5)
            {
                SaveScreenshot();
            }
            else if (e.Key == Key.F11)
            {
                bool isFullscreen = (WindowState == WindowState.Fullscreen);
                WindowState = isFullscreen ? WindowState.Normal : WindowState.Fullscreen;
            }
            else if (e.Key == Key.Up)
            {
                UniformZCoord += zCoordStep;
                UniformZCoord = Math.Min(UniformZCoord, 1);
            }
            else if (e.Key == Key.Down)
            {
                UniformZCoord -= zCoordStep;
                UniformZCoord = Math.Max(UniformZCoord, 0);
            }
            else if (e.Key == Key.PageUp)
            {
                UniformZCoord += 10 * zCoordStep;
                UniformZCoord = Math.Min(UniformZCoord, 1);
            }
            else if (e.Key == Key.PageDown)
            {
                UniformZCoord -= 10 * zCoordStep;
                UniformZCoord = Math.Max(UniformZCoord, 0);
            }
            else if (e.Key == Key.C)
            {
                // select next LRTF value component
                switch (selectedLrtfComponent)
                {
                    case LrtfComponent.PositionTheta:
                        UniformValueMask = Vector4.UnitY;
                        selectedLrtfComponent = LrtfComponent.PositionPhi;
                        break;
                    case LrtfComponent.PositionPhi:
                        UniformValueMask = Vector4.UnitZ;
                        selectedLrtfComponent = LrtfComponent.DirectionTheta;
                        break;
                    case LrtfComponent.DirectionTheta:
                        UniformValueMask = Vector4.UnitW;
                        selectedLrtfComponent = LrtfComponent.DirectionPhi;
                        break;
                    case LrtfComponent.DirectionPhi:
                        UniformValueMask = Vector4.UnitX;
                        selectedLrtfComponent = LrtfComponent.PositionTheta;
                        break;
                    default:
                        break;
                }
            }
        }

        private static string GetScreenshotId()
        {
            return DateTime.Now.ToString("yyyy-MM-dd_hh-mm-ss");
        }

        #endregion

        #region Saving images to files

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

        enum LrtfComponent
        {
            PositionTheta,
            PositionPhi,
            DirectionTheta,
            DirectionPhi,
        }
    }
}