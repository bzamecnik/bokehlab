namespace BokehLab.InteractiveDof.RayTracing
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using BokehLab.InteractiveDof;
    using BokehLab.InteractiveDof.DepthPeeling;
    using BokehLab.Math;
    using OpenTK;
    using OpenTK.Graphics.OpenGL;

    class ImageBasedRayTracer : AbstractRendererModule
    {
        static readonly string VertexShaderPath = "RayTracing/IbrtVS.glsl";
        static readonly string FragmentShaderPath = "RayTracing/IbrtFS.glsl";

        int vertexShader;
        int fragmentShader;
        int shaderProgram;

        public DepthPeeler DepthPeeler { get; set; }

        public void DrawIbrtImage(Camera camera)
        {
            Debug.Assert(DepthPeeler != null);

            GL.ClearColor(0f, 0f, 0f, 1f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            // bind color and depth textures
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, DepthPeeler.ColorTextures[0]);
            GL.ActiveTexture(TextureUnit.Texture1);
            GL.BindTexture(TextureTarget.Texture2D, DepthPeeler.DepthTextures[0]);

            // enable IBRT shader
            GL.UseProgram(shaderProgram);

            // set shader parameters (textures, lens model, ...)
            GL.Uniform1(GL.GetUniformLocation(shaderProgram, "colorTexture"), 0); // TextureUnit.Texture0
            GL.Uniform1(GL.GetUniformLocation(shaderProgram, "depthTexture"), 1); // TextureUnit.Texture1

            // draw the quad
            LayerHelper.DrawQuad();

            // disable shader
            GL.UseProgram(0);

            // unbind textures
            GL.ActiveTexture(TextureUnit.Texture1);
            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        public override void Initialize(int width, int height)
        {
            base.Initialize(width, height);

            ShaderLoader.CreateShaderFromFiles(
               VertexShaderPath, FragmentShaderPath,
               out vertexShader, out fragmentShader, out shaderProgram);

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
    }
}
