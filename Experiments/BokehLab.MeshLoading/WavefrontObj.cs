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

        struct Face
        {
            uint vertexIndex;
            uint normalIndex;
            uint textureCoordsIndex;
        }

        List<uint> faces = new List<uint>();
        List<Vector3> vertices = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        List<Vector2> texCoords = new List<Vector2>();

        public static void Parse(string fileName)
        {
            WavefrontObj obj = new WavefrontObj();

            using (StreamReader fs = new StreamReader(fileName))
            {
                int lineNumber = 0;
                while (!fs.EndOfStream)
                {
                    lineNumber++;
                    string line = fs.ReadLine();
                    if ((line.Length <= 0) || (line[0] == '#'))
                    {
                        continue;
                    }
                    string[] parts = line.Split(new[] { ' ' });
                    switch (parts[0])
                    {
                        case "v":
                            if (parts.Length != 4)
                            {
                                throw new ApplicationException(string.Format("Bad vertex vector size at line: {0}", lineNumber));
                            }
                            obj.vertices.Add(ParseVector3(parts[1], parts[2], parts[3]));
                            break;
                        case "vn":
                            if (parts.Length != 4)
                            {
                                throw new ApplicationException(string.Format("Bad normal vector size at line: {0}", lineNumber));
                            }
                            Vector3 normal = ParseVector3(parts[1], parts[2], parts[3]);
                            normal.Normalize();
                            obj.normals.Add(normal);
                            break;
                        case "vt":
                            if (parts.Length != 3)
                            {
                                throw new ApplicationException(string.Format("Bad texture coordinates vector size at line: {0}", lineNumber));
                            }
                            obj.texCoords.Add(ParseVector2(parts[1], parts[2]));
                            break;
                        case "f":
                            if (parts.Length != 4)
                            {
                                throw new ApplicationException(string.Format("Face being not a triangle at line: {0}", lineNumber));
                            }
                            int[] vertexIndices = new int[3];
                            for (int i = 0; i < 3; i++)
                            {
                                obj.faces.Add(uint.Parse(parts[i + 1]) - 1);
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        private static Vector3 ParseVector3(string x, string y, string z)
        {
            return new Vector3(
                float.Parse(x, CultureInfo.InvariantCulture),
                float.Parse(y, CultureInfo.InvariantCulture),
                float.Parse(z, CultureInfo.InvariantCulture)
            );
        }

        private static Vector2 ParseVector2(string x, string y)
        {
            return new Vector2(
                float.Parse(x, CultureInfo.InvariantCulture),
                float.Parse(y, CultureInfo.InvariantCulture)
            );
        }

        private static float ParseFloat(string x)
        {
            return float.Parse(x, CultureInfo.InvariantCulture);
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
