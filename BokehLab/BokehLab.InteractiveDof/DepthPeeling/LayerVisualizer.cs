namespace BokehLab.InteractiveDof.DepthPeeling
{
    using System.Diagnostics;
    using BokehLab.InteractiveDof;
    using BokehLab.Math;
    using OpenTK.Graphics.OpenGL;
    using OpenTK.Input;
    using System;

    class LayerVisualizer : AbstractRendererModule
    {
        public DepthPeeler DepthPeeler { get; set; }

        // layer index [0; DepthPeeler.LayerCount]
        private int selectedLayer = 0;
        private int SelectedLayer
        {
            get { return selectedLayer; }
            set
            {
                selectedLayer = MathHelper.Mod(value, DepthPeeler.LayerCount);
            }
        }
        private int selectedPackedLayer = 0;
        private int SelectedPackedLayer
        {
            get { return selectedPackedLayer; }
            set
            {
                selectedPackedLayer = MathHelper.Mod(value, DepthPeeler.PackedLayerCount);
            }
        }

        LayerType selectedLayerType = LayerType.ColorImage;

        public void Draw()
        {
            Debug.Assert(DepthPeeler != null);

            GL.ClearColor(0f, 0f, 0f, 1f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            uint texId = GetTexture();
            GL.BindTexture(TextureTarget.Texture2D, texId);
            LayerHelper.DrawQuad();
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        private uint GetTexture()
        {
            switch (selectedLayerType)
            {
                case LayerType.ColorImage:
                    return DepthPeeler.ColorTextures[selectedLayer];
                case LayerType.DepthImage:
                    return DepthPeeler.DepthTextures[selectedLayer];
                case LayerType.PackedDepthImage:
                    return DepthPeeler.PackedDepthTextures[selectedPackedLayer];
                default:
                    Debug.Assert(false);
                    return 0;
            }
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
                SelectedPackedLayer += 1;
            }
            else if (e.Key == Key.O)
            {
                SelectedLayer -= 1;
                SelectedPackedLayer -= 1;
            }
            else if (e.Key == Key.U)
            {
                SelectedLayer = 0;
                SelectedPackedLayer = 0;
            }
            else if (e.Key == Key.Tab)
            {
                selectedLayerType = (LayerType)MathHelper.Mod(((int)selectedLayerType + 1), Enum.GetValues(typeof(LayerType)).Length);
            }
        }

        enum LayerType
        {
            ColorImage = 0,
            DepthImage,
            PackedDepthImage
        }
    }
}
