#region --- License ---
/* Licensed under the MIT/X11 license.
 * Copyright (c) 2006-2008 the OpenTK Team.
 * This notice may not be removed from any source distribution.
 * See license.txt for licensing details.
 */
#endregion

namespace BokehLab.IbrtGpu
{
    using System;
    using System.Drawing;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;
    using BokehLab.Math;
    using OpenTK;
    using OpenTK.Graphics.OpenGL;
    using OpenTK.Input;

    public class IbrtGpu : GameWindow
    {
        public IbrtGpu()
            : base(800, 600)
        {
            UniformSampleCount = 1;
            UniformLensFocalLength = 1;
        }

        int color2dTexture = 0;
        int depth2dTexture = 0;
        // jittered sample lens positions
        // - samples for a pixel in Z axis
        // - grouped into repeatable tiles in XY axes
        // - pairs of values can be grouped to fit a 4-channel texture value
        int lensSamplesTexture = 0;
        //int noise2dTexture = 0;

        private float lensFocalLength;
        float UniformLensFocalLength
        {
            get { return lensFocalLength; }
            set
            {
                lensFocalLength = value;
                uniformThinLensMatrix = new Matrix4(
                    1, 0, 0, 0,
                    0, 1, 0, 0,
                    0, 0, 1, -1 / lensFocalLength,
                    0, 0, 0, 1
                );

            }
        }

        private float AspectRatio { get { return Height / (float)Width; } }

        int UniformSampleCount { get; set; }
        float UniformSampleCountInv { get { return 1.0f / (float)UniformSampleCount; } }
        float UniformLensApertureRadius { get; set; }
        Matrix4 uniformThinLensMatrix;

        string colorTextureFilename = @"..\..\..\color_0.png";
        string depthTextureFilename = @"..\..\..\depth_0.png";

        int vertexShaderObject, fragmentShaderObject, shaderProgram;

        public static void RunExample()
        {
            using (IbrtGpu example = new IbrtGpu())
            {
                example.Run(30.0, 0.0);
            }
        }

        #region GameWindow event handlers

        protected override void OnLoad(EventArgs e)
        {
            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.Texture3DExt);

            using (StreamReader vs = new StreamReader("Data/Shaders/VertexShader.glsl"))
            using (StreamReader fs = new StreamReader("Data/Shaders/FragmentShader.glsl"))
                CreateShaders(vs.ReadToEnd(), fs.ReadToEnd(),
                    out vertexShaderObject, out fragmentShaderObject,
                    out shaderProgram);

            using (Bitmap image = (Bitmap)Bitmap.FromFile(colorTextureFilename))
            {
                color2dTexture = Load2dColorTexture(image, true);
                this.Width = image.Width;
                this.Height = image.Height;
            }
            using (Bitmap image = (Bitmap)Bitmap.FromFile(depthTextureFilename))
            {
                if ((image.Width != Width) || (image.Height != Height))
                {
                    throw new ArgumentException("bad depth texture size");
                }
                depth2dTexture = Load2dColorTexture(image, false);
            }
            //noise2dTexture = GenerateRandom2dTexture(Width, Height);
            RegenerateLensSamplesTexture(32, 16, 0.01f);

            Keyboard.KeyUp += KeyUp;
        }

        protected override void OnUnload(EventArgs e)
        {
            // Clean up what we allocated before exiting
            if (color2dTexture != 0)
                GL.DeleteTexture(color2dTexture);
            if (depth2dTexture != 0)
                GL.DeleteTexture(depth2dTexture);
            //if (noise2dTexture != 0)
            //    GL.DeleteTexture(noise2dTexture);
            if (lensSamplesTexture != 0)
                GL.DeleteTexture(lensSamplesTexture);

            if (shaderProgram != 0)
                GL.DeleteProgram(shaderProgram);
            if (fragmentShaderObject != 0)
                GL.DeleteShader(fragmentShaderObject);
        }

        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);
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

        private int Load2dColorTexture(Bitmap image, bool enableLerp)
        {
            var imageData = image.LockBits(
                new Rectangle(0, 0, image.Width, image.Height),
                System.Drawing.Imaging.ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            int textureId = GL.GenTexture();

            GL.BindTexture(TextureTarget.Texture2D, textureId);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb8,
                image.Width, image.Height, 0,
                PixelFormat.Bgr, PixelType.UnsignedByte, imageData.Scan0);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
                (int)(enableLerp ? TextureMinFilter.Linear : TextureMinFilter.Nearest));
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
                (int)(enableLerp ? TextureMagFilter.Linear : TextureMagFilter.Nearest));
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

            image.UnlockBits(imageData);

            return textureId;
        }

        private int GenerateRandom2dTexture(int width, int height)
        {
            int textureId = GL.GenTexture();

            GL.BindTexture(TextureTarget.Texture2D, textureId);

            Random random = new Random();
            IntPtr texturePtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(float)) * width * height);
            unsafe
            {
                int stride = width;
                for (int y = 0; y < height; y++)
                {
                    float* row = (float*)texturePtr + (y * stride);
                    for (int x = 0; x < width; x++)
                    {
                        row[x] = (float)random.NextDouble();
                    }
                }
            }

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.One,
                width, height, 0, PixelFormat.Luminance, PixelType.Float, texturePtr);


            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

            return textureId;
        }

        private void GenerateLensSamplesTexture(int textureId, int tileSize, int sqrtSampleCount, int totalSampleCount, float lensApertureRadius)
        {
            GL.BindTexture(TextureTarget.Texture3D, textureId);
            // size of a group of samples for a single pixel
            int bands = 2;
            int groupSize = bands * totalSampleCount;
            int textureSize = groupSize * tileSize * tileSize;

            Sampler sampler = new Sampler();
            IntPtr texturePtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(float)) * textureSize);
            unsafe
            {
                int zStride = bands * tileSize * tileSize;
                for (int y = 0; y < tileSize; y++)
                {
                    for (int x = 0; x < tileSize; x++)
                    {
                        float* row = (float*)texturePtr + bands * (y * tileSize + x);
                        int index = 0;
                        // Z dimension, totalSampleCount times
                        foreach (Vector2d sample in
                            sampler.GenerateJitteredSamples(sqrtSampleCount))
                        {
                            Vector2d lensPos = lensApertureRadius *
                                Sampler.ConcentricSampleDisk(sample);
                            row[index] = (float)lensPos.X;
                            row[index + 1] = (float)lensPos.Y;
                            index += zStride;
                        }
                    }
                }
            }

            // TODO: could be an unsigned byte instead of a float
            // TODO: two sample pair could be stored in one 4-channel value
            GL.TexImage3D(TextureTarget.Texture3D, 0, PixelInternalFormat.Rg32f,
                tileSize, tileSize, totalSampleCount, 0,
                PixelFormat.Rg, PixelType.Float, texturePtr);

            GL.TexParameter(TextureTarget.Texture3D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture3D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture3D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture3D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture3D, TextureParameterName.TextureWrapR, (int)TextureWrapMode.Clamp);
        }

        private void RegenerateLensSamplesTexture(int tileSize, int sampleCount, float lensApertureRadius)
        {
            int sqrtSampleCount = (int)Math.Sqrt(sampleCount);
            int totalSampleCount = sqrtSampleCount * sqrtSampleCount;

            UniformSampleCount = totalSampleCount;
            UniformLensApertureRadius = lensApertureRadius;
            if (lensSamplesTexture == 0)
            {
                lensSamplesTexture = GL.GenTexture();
            }
            GenerateLensSamplesTexture(lensSamplesTexture, tileSize, sqrtSampleCount, totalSampleCount, lensApertureRadius);
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
            GL.Viewport(0, 0, Width, Height);

            //GL.ClearColor(0f, 0f, 0f, 1f);
            //GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            //GL.Color3(1f, 1f, 1f);

            GL.UseProgram(shaderProgram);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, color2dTexture);
            GL.ActiveTexture(TextureUnit.Texture1);
            GL.BindTexture(TextureTarget.Texture2D, depth2dTexture);
            //GL.ActiveTexture(TextureUnit.Texture2);
            //GL.BindTexture(TextureTarget.Texture2D, noise2dTexture);
            GL.ActiveTexture(TextureUnit.Texture2);
            GL.BindTexture(TextureTarget.Texture3D, lensSamplesTexture);

            //GL.ActiveTexture(TextureUnit.Texture0);

            // texture unit numbers must be passed here, not texture ids!
            // texture units are numered from 0
            // Texture0 = 0
            // Texture1 = 1
            GL.Uniform1(GL.GetUniformLocation(shaderProgram, "colorTexture"), 0);
            GL.Uniform1(GL.GetUniformLocation(shaderProgram, "depthTexture"), 1);
            //GL.Uniform1(GL.GetUniformLocation(shaderProgram, "noiseTexture"), 2);
            GL.Uniform1(GL.GetUniformLocation(shaderProgram, "lensSamples"), 2);
            GL.Uniform1(GL.GetUniformLocation(shaderProgram, "sampleCount"), UniformSampleCount);
            GL.Uniform1(GL.GetUniformLocation(shaderProgram, "sampleCountInv"), UniformSampleCountInv);
            GL.Uniform1(GL.GetUniformLocation(shaderProgram, "lensApertureRadius"), UniformLensApertureRadius);
            GL.Uniform1(GL.GetUniformLocation(shaderProgram, "lensFocalLength"), UniformLensFocalLength);
            GL.UniformMatrix4(GL.GetUniformLocation(shaderProgram, "thinLens"), false, ref uniformThinLensMatrix);

            DrawFullScreenQuad();

            GL.UseProgram(0);

            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        private void DrawFullScreenQuad()
        {
            GL.Begin(BeginMode.Quads);
            {
                // texture X goes from left to right, Y goes from top to bottom
                GL.TexCoord2(0, 0); GL.Vertex2(-1.0f, 1.0f); // bottom left
                GL.TexCoord2(0, 1); GL.Vertex2(-1.0f, -1.0f); // top left
                GL.TexCoord2(1, 1); GL.Vertex2(1.0f, -1.0f); // top right
                GL.TexCoord2(1, 0); GL.Vertex2(1.0f, 1.0f); // bottom right
            }
            GL.End();
        }

        #endregion

        #region User interaction

        private void KeyUp(object sender, KeyboardKeyEventArgs e)
        {
            if (e.Key == Key.F1)
            {
                MessageBox.Show(
@"===== Program help =====

==== Key controls ====

F1 - show help
F11 - toggle full screen
F12 - save screen shot

==== Credits ====

Implementation - Bohumír Zamečník, 2011, MFF UK
Program skeleton - OpenTK Library Examples
", "BokehLab");
            }
            else if (e.Key == Key.F12)
            {
                SaveScreenshot();
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
    }
}