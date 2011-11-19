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
        public static readonly int PackedLayerCount = LayerCount / 4;

        static readonly string PeelingVertexShaderPath = "DepthPeeling/DepthPeelerVS.glsl";
        static readonly string PeelingFragmentShaderPath = "DepthPeeling/DepthPeelerFS.glsl";

        static readonly string PackingVertexShaderPath = "DepthPeeling/DepthPackerVS.glsl";
        static readonly string PackingFragmentShaderPath = "DepthPeeling/DepthPackerFS.glsl";

        uint[] colorTextures = new uint[LayerCount];
        uint[] depthTextures = new uint[LayerCount];
        uint[] packedDepthTextures = new uint[PackedLayerCount];

        public uint[] ColorTextures { get { return colorTextures; } }
        public uint[] DepthTextures { get { return depthTextures; } }
        public uint[] PackedDepthTextures { get { return packedDepthTextures; } }

        /// <summary>
        /// Frame-buffer Object to which the current color and depth texture
        /// can be attached.
        /// </summary>
        uint fboHandle;

        public uint FboHandle { get; set; }

        int peelingVertexShader;
        int peelingFragmentShader;
        int peelingShaderProgram;

        int packingVertexShader;
        int packingFragmentShader;
        int packingShaderProgram;

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
            GL.UseProgram(peelingShaderProgram); // enable the peeling shader
            for (int i = 1; i < LayerCount; i++)
            {
                AttachLayerTextures(i);

                // Use an other texture unit than 0 as drawing the scene with
                // fixed-function pipeline might use it by default.
                GL.ActiveTexture(TextureUnit.Texture8);
                // Use the previous depth layer for manual depth comparisons.
                GL.BindTexture(TextureTarget.Texture2D, depthTextures[i - 1]);
                GL.ActiveTexture(TextureUnit.Texture0);
                GL.Uniform1(GL.GetUniformLocation(peelingShaderProgram, "depthTexture"), 8); // TextureUnit.Texture8
                GL.Uniform1(GL.GetUniformLocation(peelingShaderProgram, "texture0"), 0);
                GL.Uniform2(GL.GetUniformLocation(peelingShaderProgram, "depthTextureSizeInv"),
                    new Vector2(1.0f / Width, 1.0f / Height));

                scene.Draw();
            }
            GL.UseProgram(0); // disable the peeling shader
            GL.BindTexture(TextureTarget.Texture2D, 0);

            GL.Ext.FramebufferTexture2D(
                FramebufferTarget.FramebufferExt, FramebufferAttachment.DepthAttachmentExt,
                TextureTarget.Texture2D, 0, 0);
            GL.Ext.FramebufferTexture2D(
                FramebufferTarget.FramebufferExt, FramebufferAttachment.ColorAttachment0Ext,
                TextureTarget.Texture2D, 0, 0);

            PackDepthImages();

            GL.Ext.BindFramebuffer(FramebufferTarget.FramebufferExt, 0);
        }

        /// <summary>
        /// Packs four single-channel depth images into one four-channel image.
        /// </summary>
        private void PackDepthImages()
        {
            // for visualization
            //GL.Ext.BindFramebuffer(FramebufferTarget.FramebufferExt, 0);

            GL.ClearColor(0f, 0f, 0f, 1f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.Ext.FramebufferTexture2D(
                FramebufferTarget.FramebufferExt, FramebufferAttachment.ColorAttachment0Ext,
                TextureTarget.Texture2D, packedDepthTextures[0], 0);

            var result = GL.Ext.CheckFramebufferStatus(FramebufferTarget.FramebufferExt);
            if (result != FramebufferErrorCode.FramebufferCompleteExt)
            {
                throw new ApplicationException(string.Format("Bad FBO: {0}", result));
            }

            GL.UseProgram(packingShaderProgram);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, depthTextures[0]);
            GL.Uniform1(GL.GetUniformLocation(packingShaderProgram, "depthTexture0"), 0);
            GL.ActiveTexture(TextureUnit.Texture1);
            GL.BindTexture(TextureTarget.Texture2D, depthTextures[1]);
            GL.Uniform1(GL.GetUniformLocation(packingShaderProgram, "depthTexture1"), 1);
            GL.ActiveTexture(TextureUnit.Texture2);
            GL.BindTexture(TextureTarget.Texture2D, depthTextures[2]);
            GL.Uniform1(GL.GetUniformLocation(packingShaderProgram, "depthTexture2"), 2);
            GL.ActiveTexture(TextureUnit.Texture3);
            GL.BindTexture(TextureTarget.Texture2D, depthTextures[3]);
            GL.Uniform1(GL.GetUniformLocation(packingShaderProgram, "depthTexture3"), 3);

            LayerHelper.DrawQuad();

            GL.UseProgram(0);

            GL.ActiveTexture(TextureUnit.Texture3);
            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.ActiveTexture(TextureUnit.Texture2);
            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.ActiveTexture(TextureUnit.Texture1);
            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        #region IRendererModule Members

        public override void Initialize(int width, int height)
        {
            base.Initialize(width, height);

            ShaderLoader.CreateShaderFromFiles(
               PeelingVertexShaderPath, PeelingFragmentShaderPath,
               out peelingVertexShader, out peelingFragmentShader, out peelingShaderProgram);

            ShaderLoader.CreateShaderFromFiles(
               PackingVertexShaderPath, PackingFragmentShaderPath,
               out packingVertexShader, out packingFragmentShader, out packingShaderProgram);

            GL.Ext.GenFramebuffers(1, out fboHandle);

            GL.Enable(EnableCap.Texture2D);
        }

        public override void Dispose()
        {
            if (peelingShaderProgram != 0)
                GL.DeleteProgram(peelingShaderProgram);
            if (peelingVertexShader != 0)
                GL.DeleteShader(peelingVertexShader);
            if (peelingFragmentShader != 0)
                GL.DeleteShader(peelingFragmentShader);

            if (packingShaderProgram != 0)
                GL.DeleteProgram(packingShaderProgram);
            if (packingVertexShader != 0)
                GL.DeleteShader(packingVertexShader);
            if (packingFragmentShader != 0)
                GL.DeleteShader(packingFragmentShader);

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
            if (packedDepthTextures != null)
                GL.DeleteTextures(1, packedDepthTextures);
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
            GL.GenTextures(PackedLayerCount, packedDepthTextures);

            for (int i = 0; i < LayerCount; i++)
            {
                // setup color texture
                GL.BindTexture(TextureTarget.Texture2D, colorTextures[i]);
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba16f, width, height, 0, PixelFormat.Rgba, PixelType.HalfFloat, IntPtr.Zero);
                //GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba8, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

                // setup depth texture
                GL.BindTexture(TextureTarget.Texture2D, depthTextures[i]);
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.DepthComponent32f, width, height, 0, PixelFormat.DepthComponent, PixelType.Float, IntPtr.Zero);
                //GL.TexImage2D(TextureTarget.Texture2D, 0, (PixelInternalFormat)All.DepthComponent32, width, height, 0, PixelFormat.DepthComponent, PixelType.UnsignedInt, IntPtr.Zero);
                //GL.TexImage2D(TextureTarget.Texture2D, 0, (PixelInternalFormat)All.DepthComponent16, width, height, 0, PixelFormat.DepthComponent, PixelType.UnsignedShort, IntPtr.Zero);
                // things go horribly wrong if DepthComponent's Bitcount does not match the main Framebuffer's Depth
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
            }
            for (int i = 0; i < PackedLayerCount; i++)
            {
                // setup depth texture
                GL.BindTexture(TextureTarget.Texture2D, packedDepthTextures[i]);
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba16f, width, height, 0, PixelFormat.Rgba, PixelType.HalfFloat, IntPtr.Zero);
                //GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
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
    }
}
