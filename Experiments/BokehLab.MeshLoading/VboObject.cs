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

    public class VboObject
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct Vertex
        {
            // mimic InterleavedArrayFormat.T2fN3fV3f
            //public Vector2 TexCoord;
            //public Vector3 Normal;
            public Vector3 Position;

            public Vertex(float posX, float posY, float posZ)
            {
                Position = new Vector3(posX, posY, posZ);
            }

            public const byte SizeInBytes = 3 * sizeof(float);
        }

        uint[] VBOid = new uint[2];

        ushort[] Indices;
        Vertex[] Vertices;

        public VboObject(ushort[] indices, Vertex[] vertices)
        {
            this.Indices = indices;
            this.Vertices = vertices;
        }

        public static VboObject CreateCube()
        {
            return new VboObject(
                 new ushort[]{
                    0, 1, 2, 2, 3, 0, // front
                    3, 2, 6, 6, 7, 3, // top
                    7, 6, 5, 5, 4, 7, // back
                    4, 0, 3, 3, 7, 4, // left
                    0, 1, 5, 5, 4, 0, // bottom
                    1, 5, 6, 6, 2, 1, // right
                },
                new Vertex[]{
                    new Vertex(-1.0f, -1.0f,  1.0f),
                    new Vertex( 1.0f, -1.0f,  1.0f),
                    new Vertex( 1.0f,  1.0f,  1.0f),
                    new Vertex(-1.0f,  1.0f,  1.0f),
                    new Vertex(-1.0f, -1.0f, -1.0f),
                    new Vertex( 1.0f, -1.0f, -1.0f), 
                    new Vertex( 1.0f,  1.0f, -1.0f),
                    new Vertex(-1.0f,  1.0f, -1.0f) 
                }
            );
        }

        public void Initialize()
        {
            GL.GenBuffers(2, VBOid);
        }

        public void Dispose()
        {

            GL.DeleteBuffers(2, VBOid);
        }

        public void Load()
        {
            // load indexes
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, VBOid[0]);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(Indices.Length * sizeof(ushort)), Indices, BufferUsageHint.StaticDraw);

            // load vertices and related data
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBOid[1]);
            // or use (IntPtr)(Vertices.Length * BlittableValueType.StrideOf(Vertices))
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(Vertices.Length * Vertex.SizeInBytes), Vertices, BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        }

        public void Draw()
        {
            //GL.EnableClientState(ArrayCap.ColorArray);
            GL.EnableClientState(ArrayCap.VertexArray);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, VBOid[0]);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBOid[1]);

            GL.VertexPointer(3, VertexPointerType.Float, BlittableValueType.StrideOf(Vertices), new IntPtr(0));
            //GL.InterleavedArrays(InterleavedArrayFormat.V3f, 0, IntPtr.Zero);

            GL.DrawElements(BeginMode.Triangles, Indices.Length, DrawElementsType.UnsignedShort, IntPtr.Zero);
        }
    }
}
