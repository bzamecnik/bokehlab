namespace BokehLab.InteractiveDof.DepthPeeling
{
    using System.Diagnostics;
    using BokehLab.InteractiveDof;
    using OpenTK.Graphics.OpenGL;
    using OpenTK.Input;

    class LayerVisualizer : AbstractRendererModule
    {
        public DepthPeeler DepthPeeler { get; set; }

        // layer index [0; DepthPeeler.LayerCount]
        int selectedLayer = 0;
        // show color or depth?
        bool showColor = true;

        public void Draw()
        {
            Debug.Assert(DepthPeeler != null);

            GL.ClearColor(0f, 0f, 0f, 1f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            uint texId = (showColor ? DepthPeeler.ColorTextures : DepthPeeler.DepthTextures)[selectedLayer];
            GL.BindTexture(TextureTarget.Texture2D, texId);
            LayerHelper.DrawQuad();
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        public override void OnKeyUp(object sender, KeyboardKeyEventArgs e)
        {
            base.OnKeyUp(sender, e);

            if (e.Key == Key.P)
            {
                selectedLayer = Mod(selectedLayer + 1, DepthPeeler.LayerCount);
            }
            else if (e.Key == Key.O)
            {
                selectedLayer = Mod(selectedLayer - 1, DepthPeeler.LayerCount);
            }
            else if (e.Key == Key.Tab)
            {
                showColor = !showColor;
            }
        }

        /// <summary>
        /// Compute the modular division of a given number taking care even of
        /// negative numbers.
        /// </summary>
        /// <remarks>
        /// Converts a given number N from the Z group into the Z/M group
        /// where M is the modulus.
        /// </remarks>
        /// <param name="number"></param>
        /// <param name="modulus"></param>
        /// <returns></returns>
        public static int Mod(int number, int modulus)
        {
            int r = number % modulus;
            return r >= 0 ? r : r + modulus;
        }
    }
}
