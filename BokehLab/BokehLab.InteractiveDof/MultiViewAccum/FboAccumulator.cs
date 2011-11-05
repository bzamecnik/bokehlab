namespace BokehLab.InteractiveDof.MultiViewAccum
{
    using System;
    using OpenTK.Graphics.OpenGL;

    /// <summary>
    /// // FrameBuffer accumulator, a float buffer.
    /// </summary>
    /// <remarks>
    /// It seems that float32 is needed for high sample counts, eg. 1024.
    /// However, it is considerably slower tahn float16.
    /// </remarks>
    class FboAccumulator : IAccumulator
    {
        static readonly string VertexShaderPath = "MultiViewAccum/AccumVS.glsl";
        static readonly string FragmentShaderPath = "MultiViewAccum/AccumFS.glsl";

        // indices:
        // 0 - current frame
        // 1, 2 - average, updated average (these two will be swapped together)
        uint[] textures = new uint[3];
        uint currentFrameTexture;
        uint averageTexture;
        uint updatedAverageTexture;
        uint fboHandle;

        int accumShaderProgram;
        int accumFragmentShader;
        int accumVertexShader;

        int iteration = 0;

        #region IAccumulator Members

        public void PreAccumulate()
        {
            GL.Ext.BindFramebuffer(FramebufferTarget.FramebufferExt, fboHandle);
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
            ShaderLoader.CreateShaderFromFiles(
                VertexShaderPath, FragmentShaderPath,
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
                //GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb16f, width, height, 0, PixelFormat.Rgb, PixelType.HalfFloat, IntPtr.Zero);
                //GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb8, width, height, 0, PixelFormat.Rgb, PixelType.UnsignedByte, IntPtr.Zero);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToBorder);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToBorder);
            }
            GL.BindTexture(TextureTarget.Texture2D, 0);

            // Create a FBO and attach the texture
            GL.Ext.GenFramebuffers(1, out fboHandle);
        }

        public void Dispose()
        {
            if (fboHandle != 0)
                GL.Ext.DeleteFramebuffers(1, ref fboHandle);

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
