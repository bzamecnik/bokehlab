namespace BokehLab.Pinhole
{
    using System;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using OpenTK;
    using OpenTK.Input;
    using OpenTK.Graphics;
    using OpenTK.Graphics.OpenGL;
    using System.Runtime.InteropServices;
    using BokehLab.Math;
    using System.Collections.Generic;
    using System.Linq;

    class MultiViewAccumulation
    {
        int iteration = 0;
        Vector2[] unitDiskSamples;
        Sampler sampler = new Sampler();
        int maxIterations;

        // How many views should be accumulated during one frame rendering.
        int viewsPerFrame = 8;

        IAccumulator accumulator;

        public MultiViewAccumulation()
        {
            unitDiskSamples = CreateLensSamples(32).ToArray();
            maxIterations = unitDiskSamples.Length;
            //viewsPerFrame = maxIterations;
            //accumulator = new BufferAccumulator() { TotalIterations = maxIterations };
            accumulator = new FboAccumulator();
        }

        public void Initialize(int width, int height)
        {
            accumulator.Initialize(width, height);
        }

        public void Dispose()
        {
            accumulator.Dispose();
        }

        public void AccumulateAndDraw(Scene scene, Navigation navigation)
        {
            if (navigation.IsViewDirty)
            {
                accumulator.Clear();
                navigation.IsViewDirty = false;
                iteration = 0;
            }

            if (iteration < maxIterations)
            {
                accumulator.PreAccumulate();

                for (int i = 0; (i < viewsPerFrame) && (iteration < maxIterations); i++)
                {
                    accumulator.PreDraw();

                    Vector2 localPinholePos = navigation.GetPinholePos(unitDiskSamples[iteration]);
                    Matrix4 perspective = navigation.GetMultiViewPerspective(localPinholePos);

                    GL.MatrixMode(MatrixMode.Projection);
                    GL.PushMatrix();
                    GL.LoadMatrix(ref perspective);

                    GL.Translate(-localPinholePos.X, -localPinholePos.Y, 0);

                    scene.Draw();

                    GL.PopMatrix();

                    accumulator.PostDraw();
                    iteration++;
                }
                accumulator.PostAccumulate();
            }

            accumulator.Show();
        }

        /// <summary>
        /// Generate a set of jittered uniform samples of a unit circle.
        /// </summary>
        /// <returns></returns>
        private IEnumerable<Vector2> CreateLensSamples(int sampleCount)
        {
            var jitteredSamples = sampler.GenerateJitteredSamples(sampleCount);
            var diskSamples = jitteredSamples.Select((sample) => (Vector2)Sampler.ConcentricSampleDisk(sample));
            var diskSamplesList = diskSamples.ToList();
            Shuffle<Vector2>(diskSamplesList);
            return diskSamplesList;
        }

        //http://stackoverflow.com/questions/273313/randomize-a-listt-in-c-sharp
        public static void Shuffle<T>(IList<T> list)
        {
            Random rng = new Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        interface IAccumulator
        {
            void PreAccumulate();
            void PostAccumulate();

            void PreDraw();
            void PostDraw();

            void Show();
            void Clear();

            void Initialize(int width, int height);
            void Dispose();
        }

        /// <summary>
        /// Accumulation buffer accumulator, an integer buffer.
        /// </summary>
        class BufferAccumulator : IAccumulator
        {
            public int TotalIterations { get; set; }
            int iteration = 0;

            #region IAccumulator Members

            public void PreAccumulate()
            {
            }

            public void PostAccumulate()
            {
            }

            public void PreDraw()
            {
            }

            public void PostDraw()
            {
                //GL.Accum(AccumOp.Accum, 1f / maxIterations);
                GL.Accum(AccumOp.Mult, 1 - 1f / (iteration + 1));
                GL.Accum(AccumOp.Accum, 1f / (iteration + 1));
                iteration++;
            }

            public void Show()
            {
                //GL.Accum(AccumOp.Return, maxIterations / (float)accumIterations);
                GL.Accum(AccumOp.Return, 1f);
            }

            public void Clear()
            {
                GL.Clear(ClearBufferMask.AccumBufferBit);
                iteration = 0;
            }

            public void Initialize(int width, int height)
            {
            }

            public void Dispose()
            {
            }
            #endregion
        }

        // FrameBuffer accumulator, a float buffer.
        class FboAccumulator : IAccumulator
        {
            // indices:
            // 0 - current frame
            // 1, 2 - average, updated average (these two will be swapped together)
            uint[] textures = new uint[3];
            uint currentFrameTexture;
            uint averageTexture;
            uint updatedAverageTexture;
            uint FBOHandle;

            int iteration = 0;

            int accumShaderProgram;
            int accumFragmentShader;
            int accumVertexShader;

            #region IAccumulator Members

            public void PreAccumulate()
            {
                GL.Ext.BindFramebuffer(FramebufferTarget.FramebufferExt, FBOHandle);
            }

            public void PostAccumulate()
            {
                GL.Ext.BindFramebuffer(FramebufferTarget.FramebufferExt, 0);
            }

            public void PreDraw()
            {
                // target texture
                GL.FramebufferTexture2D(FramebufferTarget.FramebufferExt,
                    FramebufferAttachment.ColorAttachment0Ext, TextureTarget.Texture2D,
                    currentFrameTexture, 0);
            }

            public void PostDraw()
            {
                // target texture
                GL.FramebufferTexture2D(FramebufferTarget.FramebufferExt,
                    FramebufferAttachment.ColorAttachment0Ext, TextureTarget.Texture2D,
                    updatedAverageTexture, 0);

                GL.ClearColor(0, 0, 0, 1);
                GL.Clear(ClearBufferMask.ColorBufferBit);

                // source textures
                GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindTexture(TextureTarget.Texture2D, currentFrameTexture);
                GL.ActiveTexture(TextureUnit.Texture1);
                GL.BindTexture(TextureTarget.Texture2D, averageTexture);

                GL.UseProgram(accumShaderProgram);

                GL.Uniform1(GL.GetUniformLocation(accumShaderProgram, "currentFrameTexture"), 0); // TextureUnit.Texture0
                GL.Uniform1(GL.GetUniformLocation(accumShaderProgram, "averageTexture"), 1); // TextureUnit.Texture1
                float frameWeight = 1 / (float)(iteration + 1);
                GL.Uniform1(GL.GetUniformLocation(accumShaderProgram, "frameWeight"), frameWeight);

                // accumulate the current frame
                // - read current frame and average textures
                // - make linear interpolation thereof inside a fragment shader
                // - render the result into updated average texture
                DrawQuad();

                GL.UseProgram(0);

                //GL.ActiveTexture(TextureUnit.Texture1);
                GL.BindTexture(TextureTarget.Texture2D, 0);
                GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindTexture(TextureTarget.Texture2D, 0);

                // swap the two average textures
                uint tmp = updatedAverageTexture;
                updatedAverageTexture = averageTexture;
                averageTexture = tmp;

                iteration++;
            }

            public void Show()
            {
                GL.ClearColor(0, 0, 0, 1);
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                GL.BindTexture(TextureTarget.Texture2D, averageTexture);

                DrawQuad();
                GL.BindTexture(TextureTarget.Texture2D, 0);
            }

            private static void DrawQuad()
            {
                GL.MatrixMode(MatrixMode.Modelview);
                GL.PushMatrix();
                GL.LoadIdentity();

                GL.MatrixMode(MatrixMode.Projection);
                GL.PushMatrix();
                GL.LoadIdentity();

                GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, (int)TextureEnvMode.Replace);

                GL.Begin(BeginMode.Quads);
                {
                    GL.TexCoord2(0f, 1f);
                    GL.Vertex2(-1.0f, 1.0f);
                    GL.TexCoord2(0.0f, 0.0f);
                    GL.Vertex2(-1.0f, -1.0f);
                    GL.TexCoord2(1.0f, 0.0f);
                    GL.Vertex2(1.0f, -1.0f);
                    GL.TexCoord2(1.0f, 1.0f);
                    GL.Vertex2(1.0f, 1.0f);
                }
                GL.End();

                GL.PopMatrix();
                GL.MatrixMode(MatrixMode.Modelview);
                GL.PopMatrix();
            }

            public void Clear()
            {
                iteration = 0;
            }

            public void Initialize(int width, int height)
            {
                ShaderLoader.CreateShaderFromFiles("AccumVS.glsl", "AccumFS.glsl",
                    out accumVertexShader, out accumFragmentShader, out accumShaderProgram);

                GL.Enable(EnableCap.Texture2D);

                // Create color texture
                GL.GenTextures(3, textures);
                currentFrameTexture = textures[0];
                averageTexture = textures[1];
                updatedAverageTexture = textures[2];

                foreach (var texId in textures)
                {
                    GL.BindTexture(TextureTarget.Texture2D, texId);
                    GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb32f, width, height, 0, PixelFormat.Rgb, PixelType.Float, IntPtr.Zero);
                    //GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb8, width, height, 0, PixelFormat.Rgb, PixelType.UnsignedByte, IntPtr.Zero);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToBorder);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToBorder);
                }
                GL.BindTexture(TextureTarget.Texture2D, 0);

                // Create a FBO and attach the texture
                GL.Ext.GenFramebuffers(1, out FBOHandle);
            }

            public void Dispose()
            {
                if (FBOHandle != 0)
                    GL.Ext.DeleteFramebuffers(1, ref FBOHandle);

                if (textures != null)
                    GL.DeleteTextures(textures.Length, textures);

                if (accumShaderProgram != 0)
                    GL.DeleteProgram(accumShaderProgram);
                if (accumVertexShader != 0)
                    GL.DeleteShader(accumVertexShader);
                if (accumFragmentShader != 0)
                    GL.DeleteShader(accumFragmentShader);
            }

            #endregion
        }
    }
}
