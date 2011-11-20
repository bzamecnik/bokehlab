namespace BokehLab.InteractiveDof
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using Meshomatic;
    using OpenTK.Graphics.OpenGL;

    class Scene
    {
        uint texture;
        Mesh mesh;

        public string ResourcePath { get; set; }

        //RandomTriangleScene triangles;

        public Scene()
        {
            ResourcePath = @"..\..\data";
            //texture = LoadTex(Path.Combine(ResourcePath, "rue2.jpg"));
            //mesh = new Mesh(Path.Combine(ResourcePath, "medstreet.obj"));

            texture = LoadTex(Path.Combine(ResourcePath, "CrateNoParachute.png"));
            mesh = new Mesh(Path.Combine(ResourcePath, "CrateNoParachuteOBJ.obj"));

            //mesh = new Mesh(Path.Combine(ResourcePath, "DW-Ormesh-05.obj"));
            //mesh = new Mesh(Path.Combine(ResourcePath, "DW-Fungau.obj"));
            //texture = LoadTex(Path.Combine(ResourcePath, "checker_large.gif"));
            //mesh = new Mesh(Path.Combine(ResourcePath, "teapot.obj"));

            //mesh = new Mesh(Path.Combine(ResourcePath, "dragon_vrip_res2.obj"));

            GL.EnableClientState(ArrayCap.VertexArray);
            GL.EnableClientState(ArrayCap.NormalArray);
            GL.EnableClientState(ArrayCap.TextureCoordArray);
            mesh.LoadBuffers();

            //triangles = RandomTriangleScene.CreateRandomTriangles(20);
        }

        public void Draw()
        {
            GL.ClearColor(0.8f, 0.8f, 0.8f, 1f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.BindTexture(TextureTarget.Texture2D, texture);

            mesh.Draw();

            GL.BindTexture(TextureTarget.Texture2D, 0);

            //triangles.Draw();
        }

        static uint LoadTex(string file)
        {
            Bitmap bitmap = new Bitmap(file);

            uint texture;
            GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);

            GL.GenTextures(1, out texture);
            GL.BindTexture(TextureTarget.Texture2D, texture);

            BitmapData data = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0,
                OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
            bitmap.UnlockBits(data);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            GL.Ext.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            return texture;
        }

        // adapted from Meshomatic.DisplayMesh
        class Mesh
        {
            MeshData mesh;
            uint dataBuffer;
            uint indexBuffer;

            int vertexOffset, normalOffset, texCoordOffset;

            public Mesh(string fileName)
            {
                mesh = ObjLoader.LoadFile(fileName);
            }

            public void LoadBuffers()
            {
                float[] verts, norms, texcoords;
                uint[] indices;
                mesh.OpenGLArrays(out verts, out norms, out texcoords, out indices);

                GL.GenBuffers(1, out dataBuffer);
                GL.GenBuffers(1, out indexBuffer);

                // Set up data for VBO.
                // We're going to use one VBO for all geometry, and stick it in 
                // in (VVVVNNNNCCCC) order.  Non interleaved.
                int buffersize = (verts.Length + norms.Length + texcoords.Length);
                float[] bufferdata = new float[buffersize];
                vertexOffset = 0;
                normalOffset = verts.Length;
                texCoordOffset = (verts.Length + norms.Length);

                verts.CopyTo(bufferdata, vertexOffset);
                norms.CopyTo(bufferdata, normalOffset);
                texcoords.CopyTo(bufferdata, texCoordOffset);

                bool v = false;
                for (int i = texCoordOffset; i < bufferdata.Length; i++)
                {
                    if (v)
                    {
                        bufferdata[i] = 1 - bufferdata[i];
                        v = false;
                    }
                    else
                    {
                        v = true;
                    }
                }

                // Load vertices
                GL.BindBuffer(BufferTarget.ArrayBuffer, dataBuffer);
                GL.BufferData<float>(BufferTarget.ArrayBuffer, (IntPtr)(buffersize * sizeof(float)), bufferdata,
                              BufferUsageHint.StaticDraw);

                // Load indices
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBuffer);
                GL.BufferData<uint>(BufferTarget.ElementArrayBuffer,
                              (IntPtr)(indices.Length * sizeof(uint)), indices, BufferUsageHint.StaticDraw);
            }

            public void Draw()
            {
                GL.PushClientAttrib(ClientAttribMask.ClientVertexArrayBit);

                GL.BindBuffer(BufferTarget.ArrayBuffer, dataBuffer);
                GL.NormalPointer(NormalPointerType.Float, 0, (IntPtr)(normalOffset * sizeof(float)));
                GL.TexCoordPointer(2, TexCoordPointerType.Float, 0, (IntPtr)(texCoordOffset * sizeof(float)));
                GL.VertexPointer(3, VertexPointerType.Float, 0, (IntPtr)(vertexOffset * sizeof(float)));

                GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBuffer);
                GL.DrawElements(BeginMode.Triangles, mesh.Tris.Length * 3, DrawElementsType.UnsignedInt, IntPtr.Zero);

                GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

                GL.PopClientAttrib();
            }
        }
    }
}
