namespace BokehLab.InteractiveDof.NeighborhoodBuffers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using BokehLab.Math;
    using OpenTK;
    using OpenTK.Graphics.OpenGL;
    using BokehLab.InteractiveDof;
    using BokehLab.InteractiveDof.DepthPeeling;

    class NBuffers : AbstractRendererModule
    {
        static readonly string VertexShaderPath = "NeighborhoodBuffers/NBuffersVS.glsl";
        static readonly string FragmentShaderPath = "NeighborhoodBuffers/NBuffersFS.glsl";
        static readonly string Level0FragmentShaderPath = "NeighborhoodBuffers/NBuffersLevel0FS.glsl";

        uint[] nBuffersTextures;
        public uint[] NBuffersTextures { get { return nBuffersTextures; } }

        // previous n-buffer texture which gets the border color for min calculation
        uint prevMinTexture;

        uint fboHandle;
        public uint FboHandle { get; set; }

        int level0vertexShader;
        int level0fragmentShader;
        int level0shaderProgram;

        int vertexShader;
        int fragmentShader;
        int shaderProgram;

        int nbuffersWidth;
        int nbuffersHeight;

        /// <summary>
        /// The NBuffer level (number of layers to cover the least square into
        /// which fits the rectangle of size (Width, Height). This is suitable
        /// for single-value queries. In case the more efficient four-value
        /// queries are done level less by one is required.
        /// </summary>
        public int LayerCount { get; private set; }

        public void CreateNBuffers(DepthPeeler peeler)
        {
            // NOTE: we are only considering the first 4 depth layers packed into 1 image

            // - create the level 0 N-buffer from the original packedDepthTexture
            //   - TODO: at best copy it at 1/4 resolution using mip-mapping
            // - create the rest of N-buffer level from the previous levels
            //   - for each level i in [1; LevelCount]:
            //     - attach level i-1 as the source texture
            //     - attach level i as the render target
            //     - render a quad

            // Different shaders are used for the first level and the rest of levels.
            // Since we are creating single set of N-buffers for four depth images
            // in parallel in the first level we must find the extrema even within
            // the vector components.

            //for (int i = 0; i < LayerCount; i++)
            //{
            //    GL.BindTexture(TextureTarget.Texture2D, nBuffersTextures[i]);
            //    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            //}

            GL.PushAttrib(AttribMask.ViewportBit);

            GL.Viewport(0, 0, nbuffersWidth, nbuffersHeight);

            GL.Ext.BindFramebuffer(FramebufferTarget.FramebufferExt, fboHandle);

            GL.DepthFunc(DepthFunction.Always);

            // the first level is taken directly from the original packed depth texture (without offsets)
            GL.UseProgram(level0shaderProgram);

            uint packedDepthTexture = peeler.PackedDepthTextures[0];

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, packedDepthTexture);

            GL.Uniform1(GL.GetUniformLocation(level0shaderProgram, "packedDepthTexture"), 0);

            GL.Ext.FramebufferTexture2D(
                    FramebufferTarget.FramebufferExt, FramebufferAttachment.ColorAttachment0Ext,
                    TextureTarget.Texture2D, nBuffersTextures[0], 0);

            LayerHelper.DrawQuad();

            // the following levels are constructed from the previous ones (with offsets)

            GL.UseProgram(shaderProgram);

            // (x, y, 0)
            // (1,1,0) converted from [width; height] to [0.0; 1.0]^2 texture coordinates
            Vector3 offset = new Vector3(1.0f / nbuffersWidth, 1.0f / nbuffersHeight, 0);

            GL.Uniform1(GL.GetUniformLocation(shaderProgram, "prevLevelMinTexture"), 0);
            GL.Uniform1(GL.GetUniformLocation(shaderProgram, "prevLevelMaxTexture"), 1);
            for (int i = 1; i < LayerCount; i++)
            {
                GL.Uniform3(GL.GetUniformLocation(shaderProgram, "offset"), offset);

                // NOTE: Although it is not possible to bind a single texture
                // to multiple texture units the border color is unique to the
                // texture itself, not the texture unit (it get overwritten).
                // So it is needed to duplicate the previous texture to a texture
                // with a different border color.

                CopyTexture(nBuffersTextures[i - 1], prevMinTexture, nbuffersWidth, nbuffersHeight);

                GL.Ext.FramebufferTexture2D(
                    FramebufferTarget.FramebufferExt, FramebufferAttachment.ColorAttachment0Ext,
                    TextureTarget.Texture2D, nBuffersTextures[i], 0);

                GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindTexture(TextureTarget.Texture2D, prevMinTexture);

                GL.ActiveTexture(TextureUnit.Texture1);
                GL.BindTexture(TextureTarget.Texture2D, nBuffersTextures[i - 1]);

                LayerHelper.DrawQuad();

                offset *= 2.0f;
            }

            GL.DepthFunc(DepthFunction.Less);

            GL.UseProgram(0);

            GL.ActiveTexture(TextureUnit.Texture1);
            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, 0);

            GL.Ext.BindFramebuffer(FramebufferTarget.FramebufferExt, 0);

            GL.PopAttrib();

            //for (int i = 0; i < LayerCount; i++)
            //{
            //    GL.BindTexture(TextureTarget.Texture2D, nBuffersTextures[i]);
            //    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
            //    GL.Ext.GenerateMipmap(GenerateMipmapTarget.Texture2D);
            //}
            //GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        /// <summary>
        /// Copy the source texture to the destination one.
        /// </summary>
        /// <remarks>
        /// Assume that both textures are set up. Assume the FBO is bound.
        /// </remarks>
        /// <param name="source"></param>
        /// <param name="dest"></param>
        private void CopyTexture(uint source, uint dest, int width, int height)
        {
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, dest);
            GL.Ext.FramebufferTexture2D(
                FramebufferTarget.FramebufferExt, FramebufferAttachment.ColorAttachment0Ext,
                TextureTarget.Texture2D, source, 0);
            // copy the source texture from the frame buffer to the destination texture
            GL.CopyTexSubImage2D(TextureTarget.Texture2D, 0, 0, 0, 0, 0, width, height);
            GL.Ext.FramebufferTexture2D(
                FramebufferTarget.FramebufferExt, FramebufferAttachment.ColorAttachment0Ext,
                TextureTarget.Texture2D, 0, 0);
        }

        #region IRendererModule Members

        public override void Initialize(int width, int height)
        {
            base.Initialize(width, height);

            ShaderLoader.CreateShaderFromFiles(
               VertexShaderPath, Level0FragmentShaderPath,
               out level0vertexShader, out level0fragmentShader, out level0shaderProgram);

            ShaderLoader.CreateShaderFromFiles(
               VertexShaderPath, FragmentShaderPath,
               out vertexShader, out fragmentShader, out shaderProgram);

            GL.Ext.GenFramebuffers(1, out fboHandle);

            GL.Enable(EnableCap.Texture2D);
        }

        public override void Dispose()
        {
            if (fboHandle != 0)
                GL.Ext.DeleteFramebuffers(1, ref fboHandle);

            if (level0shaderProgram != 0)
                GL.DeleteProgram(level0shaderProgram);
            if (level0vertexShader != 0)
                GL.DeleteShader(level0vertexShader);
            if (level0fragmentShader != 0)
                GL.DeleteShader(level0fragmentShader);

            if (shaderProgram != 0)
                GL.DeleteProgram(shaderProgram);
            if (vertexShader != 0)
                GL.DeleteShader(vertexShader);
            if (fragmentShader != 0)
                GL.DeleteShader(fragmentShader);

            base.Dispose();
        }

        protected override void Enable()
        {
            nbuffersWidth = Width;
            nbuffersHeight = Height;
            //nbuffersWidth = Width / 2;
            //nbuffersHeight = Height / 2;

            // for sinle-value queries this level is sufficient:
            LayerCount = (int)Math.Ceiling(Math.Log(Math.Max(nbuffersWidth, nbuffersHeight), 2));
            // for four-value queries this level is sufficient:
            // LevelCount = (int)Math.Floor(Math.Log(Math.Max(nbuffersWidth, nbuffersHeight), 2));
            CreateLayerTextures(nbuffersWidth, nbuffersHeight);
        }

        protected override void Disable()
        {
            if (nBuffersTextures != null)
                GL.DeleteTextures(nBuffersTextures.Length, nBuffersTextures);
        }

        #endregion

        private void CreateLayerTextures(int width, int height)
        {
            if (LayerCount < 1)
            {
                throw new ArgumentException("At least one layer is needed.");
            }

            nBuffersTextures = new uint[LayerCount];
            GL.GenTextures(LayerCount, nBuffersTextures);
            prevMinTexture = (uint)GL.GenTexture();

            for (int i = 0; i < LayerCount; i++)
            {
                SetTextureParameters(nBuffersTextures[i], width, height);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureBorderColor, new float[] { 0, 0, 0, 0 });  // for max N-buffers
            }

            SetTextureParameters(prevMinTexture, width, height);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureBorderColor, new float[] { 1, 1, 1, 1 });  // for min N-buffers

            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        private static void SetTextureParameters(uint textureId, int width, int height)
        {
            GL.BindTexture(TextureTarget.Texture2D, textureId);
            // N-buffer levels containing min and max value in (x, y) components
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rg16f, width, height, 0, PixelFormat.Rg, PixelType.HalfFloat, IntPtr.Zero);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToBorder);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToBorder);
        }
    }
}
