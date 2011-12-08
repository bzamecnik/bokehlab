namespace BokehLab.InteractiveDof
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using OpenTK.Graphics.OpenGL;
    using OpenTK;

    class MaterialShaderManager
    {
        private Dictionary<string, ShaderInfo> shaders;
        private Dictionary<string, ShaderInfo> depthPeelingShaders;

        string depthPeelerFragmentShaderPath = "DepthPeeling/DepthPeelerFS.glsl";
        int depthPeelerFragmentShader;

        string defaultFragmentShaderPath = "Materials/DefaultFS.glsl";
        int defaultFragmentShader;

        public DepthPeelingData DepthPeelingData { get; set; }

        public MaterialShaderManager()
        {
            shaders = new Dictionary<string, ShaderInfo>();
            depthPeelingShaders = new Dictionary<string, ShaderInfo>();
            defaultFragmentShader = ShaderLoader.CompileShaderFromFile(defaultFragmentShaderPath, ShaderType.FragmentShader);
            depthPeelerFragmentShader = ShaderLoader.CompileShaderFromFile(depthPeelerFragmentShaderPath, ShaderType.FragmentShader);
            DepthPeelingData = new DepthPeelingData();
        }

        public void AddMaterial(
            string name,
            IEnumerable<string> vertexShaderPaths,
            IEnumerable<string> fragmentShaderPaths)
        {
            IList<int> vertexShaders;
            IList<int> fragmentShaders;

            // compile the shaders
            ShaderLoader.CompileShaders(vertexShaderPaths, fragmentShaderPaths, out vertexShaders, out fragmentShaders);

            // create two programs - with and without depth peeling
            int defaultProgram = ShaderLoader.CreateShaderProgram(vertexShaders, fragmentShaders.Concat(new[] { defaultFragmentShader }));
            int depthPeelingProgram = ShaderLoader.CreateShaderProgram(vertexShaders, fragmentShaders.Concat(new[] { depthPeelerFragmentShader }));

            shaders.Add(name, new ShaderInfo(defaultProgram));
            depthPeelingShaders.Add(name, new ShaderInfo(depthPeelingProgram));
        }

        public int UseMaterial(string name)
        {
            int shader = ((DepthPeelingData.Enabled) ? depthPeelingShaders : shaders)[name].ShaderProgram;
            GL.UseProgram(shader);
            if (DepthPeelingData.Enabled)
            {
                GL.Uniform1(GL.GetUniformLocation(shader, "depthTexture"), DepthPeelingData.DepthTexture);
                GL.Uniform2(GL.GetUniformLocation(shader, "depthTextureSizeInv"), DepthPeelingData.DepthTextureSizeInv);
                GL.Uniform1(GL.GetUniformLocation(shader, "prevLayer"), DepthPeelingData.PreviousLayerIndex);
            }
            return shader;
        }

        private class ShaderInfo
        {
            public int ShaderProgram;

            public ShaderInfo(int shaderProgram)
            {
                this.ShaderProgram = shaderProgram;
            }
        }
    }

    public class DepthPeelingData
    {
        public int DepthTexture; // texture unit id
        public Vector2 DepthTextureSizeInv;
        public int PreviousLayerIndex;
        public bool Enabled;
    }
}
