namespace BokehLab.InteractiveDof.NeighborhoodBuffers
{
    using System.Diagnostics;
    using BokehLab.InteractiveDof;
    using BokehLab.Math;
    using OpenTK.Graphics.OpenGL;
    using OpenTK.Input;

    class NBuffersVisualizer : AbstractRendererModule
    {
        public NBuffers NBuffers { get; set; }

        // layer index [0; NBuffersManager.LayerCount]
        private int selectedLayer = 0;
        private int SelectedLayer
        {
            get { return selectedLayer; }
            set
            {
                selectedLayer = MathHelper.Mod(value, NBuffers.LayerCount);
            }
        }

        public void Draw()
        {
            Debug.Assert(NBuffers != null);

            GL.ClearColor(0f, 0f, 0f, 1f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            uint texId = NBuffers.NBuffersTextures[selectedLayer];
            GL.BindTexture(TextureTarget.Texture2D, texId);
            LayerHelper.DrawQuad();
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        public override void OnKeyUp(object sender, KeyboardKeyEventArgs e)
        {
            base.OnKeyUp(sender, e);

            if (!Enabled)
            {
                return;
            }

            if (e.Key == Key.P)
            {
                SelectedLayer += 1;
            }
            else if (e.Key == Key.O)
            {
                SelectedLayer -= 1;
            }
            else if (e.Key == Key.U)
            {
                SelectedLayer = 0;
            }
        }
    }
}
