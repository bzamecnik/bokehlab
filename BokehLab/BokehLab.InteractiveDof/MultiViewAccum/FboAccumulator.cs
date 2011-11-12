namespace BokehLab.InteractiveDof.MultiViewAccum
{
    using System;
    using OpenTK.Graphics.OpenGL;
    using BokehLab.InteractiveDof;

    /// <summary>
    /// FrameBuffer accumulator, a float buffer.
    /// </summary>
    /// <remarks>
    /// It seems that float32 is needed for high sample counts, eg. 1024.
    /// However, it is considerably slower tahn float16.
    /// </remarks>
    class FboAccumulator : AbstractRendererModule, IAccumulator
    {
        static readonly string VertexShaderPath = "MultiViewAccum/AccumVS.glsl";
        static readonly string FragmentShaderPath = "MultiViewAccum/AccumFS.glsl";

        // indices:
        // 0 - current frame
        // 1, 2 - average, updated average (these two will be swapped together)
        uint[] textures = new uint[3];
        uint currentFrameTexture;
        uint currentFrameDepthTexture;
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
            GL.FramebufferTexture2D(FramebufferTarget.FramebufferExt,
                FramebufferAttachment.DepthAttachmentExt, TextureTarget.Texture2D,
                currentFrameDepthTexture, 0);
        }

        public void PostDraw()
        {
            //DEBUG
            //GL.Ext.BindFramebuffer(FramebufferTarget.FramebufferExt, 0);

            // target texture
            GL.FramebufferTexture2D(FramebufferTarget.FramebufferExt,
                FramebufferAttachment.ColorAttachment0Ext, TextureTarget.Texture2D,
                updatedAverageTexture, 0);

            GL.ClearColor(0, 0, 0, 1);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

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
            LayerHelper.DrawQuad();

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

            LayerHelper.DrawQuad();
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        public void Clear()
        {
            iteration = 0;
        }

        public override void Initialize(int width, int height)
        {
            base.Initialize(width, height);

            ShaderLoader.CreateShaderFromFiles(
                VertexShaderPath, FragmentShaderPath,
                out accumVertexShader, out accumFragmentShader, out accumShaderProgram);

            GL.Enable(EnableCap.Texture2D);
        }

        protected override void Enable()
        {
            // Create color texture
            GL.GenTextures(3, textures);
            currentFrameTexture = textures[0];
            averageTexture = textures[1];
            updatedAverageTexture = textures[2];

            foreach (var texId in textures)
            {
                GL.BindTexture(TextureTarget.Texture2D, texId);
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb32f, Width, Height, 0, PixelFormat.Rgb, PixelType.Float, IntPtr.Zero);
                //GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb16f, Width, Height, 0, PixelFormat.Rgb, PixelType.HalfFloat, IntPtr.Zero);
                //GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb8, Width, Height, 0, PixelFormat.Rgb, PixelType.UnsignedByte, IntPtr.Zero);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToBorder);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToBorder);
            }


            GL.GenTextures(1, out currentFrameDepthTexture);

            GL.BindTexture(TextureTarget.Texture2D, currentFrameDepthTexture);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.DepthComponent32f, Width, Height, 0, PixelFormat.DepthComponent, PixelType.Float, IntPtr.Zero);
            //GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.DepthComponent32, Width, Height, 0, PixelFormat.DepthComponent, PixelType.UnsignedInt, IntPtr.Zero);
            //GL.TexImage2D(TextureTarget.Texture2D, 0, (PixelInternalFormat)All.DepthComponent16, Width, Height, 0, PixelFormat.DepthComponent, PixelType.UnsignedShort, IntPtr.Zero);
            // things go horribly wrong if DepthComponent's Bitcount does not match the main Framebuffer's Depth
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

            GL.BindTexture(TextureTarget.Texture2D, 0);

            // Create a FBO and attach the texture
            GL.Ext.GenFramebuffers(1, out fboHandle);
        }

        protected override void Disable()
        {
            if (fboHandle != 0)
                GL.Ext.DeleteFramebuffers(1, ref fboHandle);

            if (textures != null)
                GL.DeleteTextures(textures.Length, textures);
        }

        public override void Dispose()
        {
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
