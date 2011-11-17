namespace BokehLab.MeshLoading
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.IO;
    using System.Globalization;
    using OpenTK;
    using OpenTK.Graphics.OpenGL;
    using System.Runtime.InteropServices;

    public class WavefrontObj
    {
        [StructLayout(LayoutKind.Sequential)]
        struct Vertex
        {
            // mimic InterleavedArrayFormat.T2fN3fV3f
            //public Vector2 TexCoord;
            //public Vector3 Normal;
            public Vector3 Position;
        }

        public struct Triangle
        {
            public Vector3 Vertex0;
            public Vector3 Vertex1;
            public Vector3 Vertex2;
        }

        List<Vector3> vertices = new List<Vector3>();
        List<Triangle> triangles = new List<Triangle>();
        List<Vector3> normals = new List<Vector3>();

        public static void Parse(string fileName)
        {
            WavefrontObj obj = new WavefrontObj();

            using (StreamReader fs = new StreamReader(fileName))
            {
                while (!fs.EndOfStream)
                {
                    string line = fs.ReadLine();
                    string[] parts;
                    switch (line[0])
                    {
                        case 'v':
                            parts = line.Split(new[] { ' ' }, 4);
                            obj.vertices.Add(
                                new Vector3(
                                    float.Parse(parts[1], CultureInfo.InvariantCulture),
                                    float.Parse(parts[2], CultureInfo.InvariantCulture),
                                    float.Parse(parts[3], CultureInfo.InvariantCulture)
                                ));
                            break;
                        case 'f':
                            parts = line.Split(new[] { ' ' }, 4);
                            Triangle tri = new Triangle();
                            int[] vertexIndices = new int[3];
                            for (int i = 0; i < 3; i++)
                            {
                                vertexIndices[i] = int.Parse(parts[i + 1]) - 1;
                            }
                            tri.Vertex0 = obj.vertices[vertexIndices[0]];
                            tri.Vertex1 = obj.vertices[vertexIndices[1]];
                            tri.Vertex2 = obj.vertices[vertexIndices[2]];
                            obj.triangles.Add(tri);
                            obj.normals.Add(GetNormal(tri.Vertex0, tri.Vertex1, tri.Vertex2));
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        public static Vector3 GetNormal(Vector3 a, Vector3 b, Vector3 c)
        {
            var normal = Vector3.Cross(a - b, a - c);
            normal.Normalize();
            return normal;
        }

        public void LoadToVertexBufferObject()
        {
            //GL.vertexpo
        }
    }
}
