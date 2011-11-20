namespace BokehLab.InteractiveDof.NeighborhoodBuffers
{
    using System.Diagnostics;
    using BokehLab.InteractiveDof;
    using BokehLab.Math;
    using OpenTK;
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
                selectedLayer = BokehLab.Math.MathHelper.Mod(value, NBuffers.LayerCount);
            }
        }

        static readonly string VertexShaderPath = "NeighborhoodBuffers/NBuffersVS.glsl";
        static readonly string FragmentShaderPath = "NeighborhoodBuffers/NBuffersVisualizerFS.glsl";

        int vertexShader;
        int fragmentShader;
        int shaderProgram;

        private int selectedColorMask = 0;
        private int SelectedColorMask
        {
            get { return selectedColorMask; }
            set
            {
                selectedColorMask = BokehLab.Math.MathHelper.Mod(value, colorMasks.Length);
            }
        }

        Vector2[] colorMasks = { new Vector2(1, 1), new Vector2(1, 0), new Vector2(0, 1) };

        public void Draw()
        {
            Debug.Assert(NBuffers != null);

            GL.ClearColor(0f, 0f, 0f, 1f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.UseProgram(shaderProgram);
            GL.Uniform1(GL.GetUniformLocation(shaderProgram, "nBufferLayerTexture"), 0);
            GL.Uniform2(GL.GetUniformLocation(shaderProgram, "colorMask"), colorMasks[selectedColorMask]);
            GL.Uniform1(GL.GetUniformLocation(shaderProgram, "layer"), selectedLayer);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2DArray, NBuffers.NBuffersTextures);

            LayerHelper.DrawQuad();

            GL.BindTexture(TextureTarget.Texture2D, 0);

            GL.UseProgram(0);
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
            if (e.Key == Key.Tab)
            {
                SelectedColorMask += 1;
            }
            else if (e.Key == Key.U)
            {
                SelectedLayer = 0;
                SelectedColorMask = 0;
            }
        }

        public override void Initialize(int width, int height)
        {
            base.Initialize(width, height);

            ShaderLoader.CreateShaderFromFiles(
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
    }
}
