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

        static readonly string VertexShaderPath = "DepthPeeling/VisualizerVS.glsl";
        static readonly string FragmentShaderPath = "DepthPeeling/VisualizerFS.glsl";

        int vertexShader;
        int fragmentShader;
        int shaderProgram;

        public void Draw()
        {
            Debug.Assert(DepthPeeler != null);

            GL.ClearColor(0f, 0f, 0f, 1f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.UseProgram(shaderProgram);
            GL.Uniform1(GL.GetUniformLocation(shaderProgram, "layerTexture"), 0);
            GL.Uniform1(GL.GetUniformLocation(shaderProgram, "layerIndex"), GetLayerIndex());

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2DArray, GetTexture());
            LayerHelper.DrawQuad();
            GL.BindTexture(TextureTarget.Texture2DArray, 0);

            GL.UseProgram(0);
        }

        private uint GetTexture()
        {
            switch (selectedLayerType)
            {
                case LayerType.ColorImage:
                    return DepthPeeler.ColorTextures;
                case LayerType.DepthImage:
                    return DepthPeeler.DepthTextures;
                case LayerType.PackedDepthImage:
                    return DepthPeeler.PackedDepthTextures;
                default:
                    Debug.Assert(false);
                    return 0;
            }
        }

        private int GetLayerIndex()
        {
            switch (selectedLayerType)
            {
                case LayerType.ColorImage:
                    return selectedLayer;
                case LayerType.DepthImage:
                    return selectedLayer;
                case LayerType.PackedDepthImage:
                    return selectedPackedLayer;
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

        public override void Initialize(int width, int height)
        {
            base.Initialize(width, height);

            ShaderLoader.CreateSimpleShaderProgram(
               VertexShaderPath, FragmentShaderPath,
               out vertexShader, out fragmentShader, out shaderProgram);
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

        enum LayerType
        {
            ColorImage = 0,
            DepthImage,
            PackedDepthImage
        }
    }
}
