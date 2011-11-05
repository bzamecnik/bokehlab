namespace BokehLab.InteractiveDof
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using OpenTK.Graphics.OpenGL;

    class LayerHelper
    {
        public static void DrawQuad()
        {
            GL.MatrixMode(MatrixMode.Modelview);
            GL.PushMatrix();
            GL.LoadIdentity();

            GL.MatrixMode(MatrixMode.Projection);
            GL.PushMatrix();
            GL.LoadIdentity();

            GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, (int)TextureEnvMode.Replace);
            GL.Begin(BeginMode.Quads);
            {
                GL.TexCoord2(0f, 1f);
                GL.Vertex2(-1.0f, 1.0f);
                GL.TexCoord2(0.0f, 0.0f);
                GL.Vertex2(-1.0f, -1.0f);
                GL.TexCoord2(1.0f, 0.0f);
                GL.Vertex2(1.0f, -1.0f);
                GL.TexCoord2(1.0f, 1.0f);
                GL.Vertex2(1.0f, 1.0f);
            }
            GL.End();

            GL.PopMatrix();
            GL.MatrixMode(MatrixMode.Modelview);
            GL.PopMatrix();
        }
    }
}
