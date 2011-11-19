namespace BokehLab.InteractiveDof.DepthPeeling
{
    using System.Diagnostics;
    using BokehLab.InteractiveDof;
    using OpenTK.Graphics.OpenGL;

    class OrderIndependentTransparency : AbstractRendererModule
    {
        public DepthPeeler DepthPeeler { get; set; }

        // Simple application of depth peeling:
        // Order-independent transparency [everitt2001].
        public void Draw()
        {
            Debug.Assert(DepthPeeler != null);

            GL.ClearColor(0f, 0f, 0f, 1f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            // the back-most layer must be fully opaque
            GL.Disable(EnableCap.Blend);
            GL.BindTexture(TextureTarget.Texture2D, DepthPeeler.ColorTextures[DepthPeeler.LayerCount - 1]);
            LayerHelper.DrawQuad();

            GL.DepthFunc(DepthFunction.Lequal);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusDstAlpha);
            for (int i = DepthPeeler.LayerCount - 2; i >= 0; i--)
            {
                GL.BindTexture(TextureTarget.Texture2D, DepthPeeler.ColorTextures[i]);
                LayerHelper.DrawQuad();
            }
            GL.Disable(EnableCap.Blend);
            GL.DepthFunc(DepthFunction.Less);
        }
    }
}
