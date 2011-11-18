﻿namespace BokehLab.InteractiveDof.NBuffers
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
        static readonly string VertexShaderPath = "NBuffers/NBuffersVS.glsl";
        static readonly string FragmentShaderPath = "NBuffers/NBuffersFS.glsl";
        static readonly string Level0FragmentShaderPath = "NBuffers/NBuffersLevel0FS.glsl";

        uint[] nBuffersTextures;
        public uint[] NBuffersTextures { get { return nBuffersTextures; } }

        uint fboHandle;
        public uint FboHandle { get; set; }

        int level0vertexShader;
        int level0fragmentShader;
        int level0shaderProgram;

        int vertexShader;
        int fragmentShader;
        int shaderProgram;

        /// <summary>
        /// The NBuffer level (number of layers to cover the least square into
        /// which fits the rectangle of size (Width, Height). This is suitable
        /// for single-value queries. In case the more efficient four-value
        /// queries are done level less by one is required.
        /// </summary>
        private int LayerCount { get; set; }

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

            GL.Ext.BindFramebuffer(FramebufferTarget.FramebufferExt, fboHandle);

            uint packedDepthTexture = peeler.PackedDepthTextures[0];

            GL.DepthFunc(DepthFunction.Always);

            GL.UseProgram(level0shaderProgram);

            GL.Uniform1(GL.GetUniformLocation(level0shaderProgram, "packedDepthTextureMin"), 0);
            GL.Uniform1(GL.GetUniformLocation(level0shaderProgram, "packedDepthTextureMax"), 1);

            // the first level is taken directly from the original packed depth texture (without offsets)

            GL.Uniform3(GL.GetUniformLocation(level0shaderProgram, "offset"), new Vector3(0, 0, 0));

            GL.Ext.FramebufferTexture2D(
                    FramebufferTarget.FramebufferExt, FramebufferAttachment.ColorAttachment0Ext,
                    TextureTarget.Texture2D, nBuffersTextures[0], 0);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, packedDepthTexture);

            GL.ActiveTexture(TextureUnit.Texture1);
            GL.BindTexture(TextureTarget.Texture2D, packedDepthTexture);

            LayerHelper.DrawQuad();

            // the following levels are constructed from the previous ones (with offsets)

            GL.UseProgram(shaderProgram);

            Vector3 offset = new Vector3(1, 1, 0);

            GL.Uniform1(GL.GetUniformLocation(shaderProgram, "packedDepthTextureMin"), 0);
            GL.Uniform1(GL.GetUniformLocation(shaderProgram, "packedDepthTextureMax"), 1);
            for (int i = 1; i < LayerCount; i++)
            {
                GL.Uniform3(GL.GetUniformLocation(shaderProgram, "offset"), offset);
                GL.Ext.FramebufferTexture2D(
                    FramebufferTarget.FramebufferExt, FramebufferAttachment.ColorAttachment0Ext,
                    TextureTarget.Texture2D, nBuffersTextures[i], 0);

                // NOTE: binding one texture to multiple texture units to get a different border value
                // Hope this works.
                GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindTexture(TextureTarget.Texture2D, nBuffersTextures[i - 1]);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureBorderColor, 1.0f); // for min N-buffers

                GL.ActiveTexture(TextureUnit.Texture1);
                GL.BindTexture(TextureTarget.Texture2D, nBuffersTextures[i - 1]);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureBorderColor, 0.0f); // for max N-buffers

                LayerHelper.DrawQuad();

                offset *= 2.0f;
            }
            GL.DepthFunc(DepthFunction.Less);

            GL.Ext.BindFramebuffer(FramebufferTarget.FramebufferExt, 0);
        }

        ///// <summary>
        ///// Copy the source texture to the destination one.
        ///// </summary>
        ///// <remarks>
        ///// Assume that both textures are set up. Assume the FBO is bound.
        ///// </remarks>
        ///// <param name="source"></param>
        ///// <param name="dest"></param>
        //private void CopyTexture(uint source, uint dest)
        //{
        //    GL.BindTexture(TextureTarget.Texture2D, dest);
        //    GL.Ext.FramebufferTexture2D(
        //        FramebufferTarget.FramebufferExt, FramebufferAttachment.ColorAttachment0Ext,
        //        TextureTarget.Texture2D, source, 0);
        //    // copy the source texture from the frame buffer to the destination texture
        //    GL.CopyTexSubImage2D(TextureTarget.Texture2D, 0, 0, 0, 0, 0, Width, Height);
        //}

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
            // for sinle-value queries this level is sufficient:
            LayerCount = (int)Math.Ceiling(Math.Log(Math.Max(Width, Height), 2));
            // for four-value queries this level is sufficient:
            // LevelCount = (int)Math.Floor(Math.Log(Math.Max(Width, Height), 2));
            CreateLayerTextures(Width, Height, LayerCount);
        }

        protected override void Disable()
        {
            if (fboHandle != 0)
                GL.Ext.DeleteFramebuffers(1, ref fboHandle);

            if (nBuffersTextures != null)
                GL.DeleteTextures(1, nBuffersTextures);
        }

        #endregion

        private void CreateLayerTextures(int width, int height, int squareSize)
        {
            if (LayerCount < 1)
            {
                throw new ArgumentException("At least one layer is needed.");
            }

            nBuffersTextures = new uint[LayerCount];
            GL.GenTextures(LayerCount, nBuffersTextures);

            for (int i = 0; i < LayerCount; i++)
            {
                // N-buffer levels containing min and max value in (x, y) components
                GL.BindTexture(TextureTarget.Texture2D, nBuffersTextures[i]);
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba16f, width, height, 0, PixelFormat.Rg, PixelType.HalfFloat, IntPtr.Zero);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToBorder);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToBorder);
            }
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }
    }
}