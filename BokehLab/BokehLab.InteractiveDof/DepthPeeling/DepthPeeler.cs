namespace BokehLab.InteractiveDof.DepthPeeling
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using BokehLab.Math;
    using OpenTK;
    using OpenTK.Graphics.OpenGL;
    using BokehLab.InteractiveDof;

    class DepthPeeler : AbstractRendererModule
    {
        /// <summary>
        /// Number of depth peeling layers (color and depth textures).
        /// </summary>
        /// <remarks>
        /// 8 layers are almost always enough.
        /// </remarks>
        public static readonly int LayerCount = 4;

        static readonly string VertexShaderPath = "DepthPeeling/DepthPeelerVS.glsl";
        static readonly string FragmentShaderPath = "DepthPeeling/DepthPeelerFS.glsl";

        uint[] colorTextures = new uint[LayerCount];
        uint[] depthTextures = new uint[LayerCount];

        public uint[] ColorTextures { get { return colorTextures; } }
        public uint[] DepthTextures { get { return depthTextures; } }

        /// <summary>
        /// Frame-buffer Object to which the current color and depth texture
        /// can be attached.
        /// </summary>
        uint fboHandle;

        public uint FboHandle { get; set; }

        int vertexShader;
        int fragmentShader;
        int shaderProgram;

        public void PeelLayers(Scene scene)
        {
            // put the results into color and depth textures via FBO
            GL.Ext.BindFramebuffer(FramebufferTarget.FramebufferExt, fboHandle);

            GL.Enable(EnableCap.Texture2D);

            // draw the first layer without the depth peeling shader
            // (there is no previous depth layer to compare)
            GL.BindTexture(TextureTarget.Texture2D, 0);
            AttachLayerTextures(0);
            scene.Draw();

            // draw the rest of layers with depth peeling
            GL.UseProgram(shaderProgram); // enable the peeling shader
            for (int i = 1; i < LayerCount; i++)
            {
                AttachLayerTextures(i);
                // use the previous depth layer for manual depth comparisons
                GL.BindTexture(TextureTarget.Texture2D, depthTextures[i - 1]);
                GL.Uniform1(GL.GetUniformLocation(shaderProgram, "depthTexture"), 0); // TextureUnit.Texture0
                scene.Draw();
            }
            GL.UseProgram(0); // disable the peeling shader
            GL.BindTexture(TextureTarget.Texture2D, 0);

            UnbindFramebuffer(); // disable rendering into the FBO
        }

        public void DisplayLayers()
        {
            // Simple application of depth peeling:
            // Order-independent transparency [everitt2001].

            GL.ClearColor(0f, 0f, 0f, 1f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            bool showTransparency = true;
            bool showTwoColorLayers = false;
            if (showTransparency)
            {
                // back-most layer must be fully opaque
                GL.Disable(EnableCap.Blend);
                GL.BindTexture(TextureTarget.Texture2D, colorTextures[LayerCount - 1]);
                LayerHelper.DrawQuad();

                GL.DepthFunc(DepthFunction.Lequal);
                GL.Enable(EnableCap.Blend);
                GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusDstAlpha);
                for (int i = LayerCount - 2; i >= 0; i--)
                {
                    GL.BindTexture(TextureTarget.Texture2D, colorTextures[i]);
                    LayerHelper.DrawQuad();
                }
                GL.Disable(EnableCap.Blend);
                GL.DepthFunc(DepthFunction.Less);
            }
            else if (showTwoColorLayers)
            {
                // show the second layer opaque and the first one transparent

                GL.Disable(EnableCap.Blend);
                GL.BindTexture(TextureTarget.Texture2D, colorTextures[1]);
                LayerHelper.DrawQuad();

                GL.DepthFunc(DepthFunction.Lequal);
                GL.Enable(EnableCap.Blend);
                GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusDstAlpha);
                GL.BindTexture(TextureTarget.Texture2D, colorTextures[0]);
                LayerHelper.DrawQuad();
                GL.Disable(EnableCap.Blend);
                GL.DepthFunc(DepthFunction.Less);
            }
            else
            {
                GL.BindTexture(TextureTarget.Texture2D, colorTextures[1]);
                LayerHelper.DrawQuad();
            }

            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        #region IRendererModule Members

        public override void Initialize(int width, int height)
        {
            base.Initialize(width, height);

            ShaderLoader.CreateShaderFromFiles(
               VertexShaderPath, FragmentShaderPath,
               out vertexShader, out fragmentShader, out shaderProgram);

            GL.Ext.GenFramebuffers(1, out fboHandle);

            GL.Enable(EnableCap.Texture2D);
        }

        public override void Dispose()
        {
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
            CreateLayerTextures(Width, Height);
        }

        protected override void Disable()
        {
            if (fboHandle != 0)
                GL.Ext.DeleteFramebuffers(1, ref fboHandle);

            if (colorTextures != null)
                GL.DeleteTextures(1, colorTextures);
            if (depthTextures != null)
                GL.DeleteTextures(1, depthTextures);
        }

        #endregion

        private void CreateLayerTextures(int width, int height)
        {
            if (LayerCount < 1)
            {
                throw new ArgumentException("At least one layer is needed.");
            }

            // create textures
            GL.GenTextures(LayerCount, colorTextures);
            GL.GenTextures(LayerCount, depthTextures);

            for (int i = 0; i < LayerCount; i++)
            {
                // setup color texture
                GL.BindTexture(TextureTarget.Texture2D, colorTextures[i]);
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba16f, width, height, 0, PixelFormat.Rgba, PixelType.HalfFloat, IntPtr.Zero);
                //GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba8, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

                // setup depth texture
                GL.BindTexture(TextureTarget.Texture2D, depthTextures[i]);
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.DepthComponent32f, width, height, 0, PixelFormat.DepthComponent, PixelType.Float, IntPtr.Zero);
                //GL.TexImage2D(TextureTarget.Texture2D, 0, (PixelInternalFormat)All.DepthComponent16, width, height, 0, PixelFormat.DepthComponent, PixelType.UnsignedShort, IntPtr.Zero);
                // things go horribly wrong if DepthComponent's Bitcount does not match the main Framebuffer's Depth
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
            }
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        private void AttachLayerTextures(int layerIndex)
        {
            // Attach color and depth from the selected layer.
            // Assumes that a FBO is bound.
            GL.Ext.FramebufferTexture2D(
                FramebufferTarget.FramebufferExt, FramebufferAttachment.ColorAttachment0Ext,
                TextureTarget.Texture2D, colorTextures[layerIndex], 0);
            GL.Ext.FramebufferTexture2D(
                FramebufferTarget.FramebufferExt, FramebufferAttachment.DepthAttachmentExt,
                TextureTarget.Texture2D, depthTextures[layerIndex], 0);

            var result = GL.Ext.CheckFramebufferStatus(FramebufferTarget.FramebufferExt);
            if (result != FramebufferErrorCode.FramebufferCompleteExt)
            {
                throw new ApplicationException(string.Format("Bad FBO: {0}", result));
            }
        }

        private void UnbindFramebuffer()
        {
            // detach textures - TODO: really needed?
            GL.Ext.FramebufferTexture2D(
                FramebufferTarget.FramebufferExt, FramebufferAttachment.ColorAttachment0Ext,
                TextureTarget.Texture2D, 0, 0);
            GL.Ext.FramebufferTexture2D(
                FramebufferTarget.FramebufferExt, FramebufferAttachment.DepthAttachmentExt,
                TextureTarget.Texture2D, 0, 0);
            GL.Ext.BindFramebuffer(FramebufferTarget.FramebufferExt, 0);
        }
    }
}
