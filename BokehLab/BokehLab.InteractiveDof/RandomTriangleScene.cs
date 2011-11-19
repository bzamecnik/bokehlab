using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace BokehLab.InteractiveDof
{
    class RandomTriangleScene
    {
        int vertexCount;
        Vector4[] colors;
        Vector3[] vertices;

        private RandomTriangleScene()
        {
        }

        public static RandomTriangleScene CreateRandomTriangles(int triangleCount)
        {
            RandomTriangleScene scene = new RandomTriangleScene();
            scene.vertexCount = 3 * triangleCount;
            scene.colors = new Vector4[scene.vertexCount];
            scene.vertices = new Vector3[scene.vertexCount];
            for (int i = 0; i < scene.vertexCount; i++)
            {
                scene.colors[i] = new Vector4(GetRandom0to1(), GetRandom0to1(), GetRandom0to1(), 0.5f);
                scene.vertices[i] = new Vector3(GetRandom(), GetRandom(), GetRandom());
            }
            return scene;
        }

        public void Draw()
        {
            GL.ClearColor(0.25f, 0.25f, 0.25f, 1);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.Color4(1, 1, 1, 0.5);
            GL.Begin(BeginMode.Quads);
            {
                GL.Vertex3(-1.0f, -1, 1.0f);
                GL.Vertex3(-1.0f, -1, -1.0f);
                GL.Vertex3(1.0f, -1, -1.0f);
                GL.Vertex3(1.0f, -1, 1.0f);
            }
            GL.End();

            GL.Begin(BeginMode.Triangles);
            for (int i = 0; i < colors.Length; i++)
            {
                GL.Color4(colors[i]);
                GL.Vertex3(vertices[i]);
            }
            GL.End();
        }

        static Random rnd = new Random();
        public const float scale = 2f;

        /// <summary>Returns a random Float in the range [-0.5*scale..+0.5*scale]</summary>
        public static float GetRandom()
        {
            return (float)(rnd.NextDouble() - 0.5) * scale;
        }

        /// <summary>Returns a random Float in the range [0..1]</summary>
        public static float GetRandom0to1()
        {
            return (float)rnd.NextDouble();
        }
    }
}
