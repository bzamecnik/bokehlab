namespace BokehLab.InteractiveDof
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using BokehLab.Math;
    using Meshomatic;
    using OpenTK;
    using OpenTK.Graphics.OpenGL;

    class Scene
    {
        Mesh crateMesh;
        uint crateTexture;

        Mesh streetMesh;
        uint streetTexture;

        Mesh dragonMesh;

        uint starsTexture;
        uint groundTexture;

        public string ResourcePath { get; set; }

        //RandomTriangleScene triangles;

        public Scene()
        {
            ResourcePath = @"..\..\data";
            starsTexture = GenerateHdrStarsTex(512, 512);
            groundTexture = LoadTex(Path.Combine(ResourcePath, "dirt_01.jpg"));

            GL.EnableClientState(ArrayCap.VertexArray);
            GL.EnableClientState(ArrayCap.NormalArray);
            GL.EnableClientState(ArrayCap.TextureCoordArray);

            crateTexture = LoadTex(Path.Combine(ResourcePath, "CrateNoParachute.png"));
            crateMesh = new Mesh(Path.Combine(ResourcePath, "CrateNoParachuteOBJ.obj"));
            crateMesh.LoadBuffers();

            streetTexture = LoadTex(Path.Combine(ResourcePath, "rue2.jpg"));
            streetMesh = new Mesh(Path.Combine(ResourcePath, "medstreet.obj"));
            streetMesh.LoadBuffers();

            //mesh = new Mesh(Path.Combine(ResourcePath, "DW-Ormesh-05.obj"));
            //mesh = new Mesh(Path.Combine(ResourcePath, "DW-Fungau.obj"));
            //texture = LoadTex(Path.Combine(ResourcePath, "checker_large.gif"));

            //mesh = new Mesh(Path.Combine(ResourcePath, "teapot.obj"));

            dragonMesh = new Mesh(Path.Combine(ResourcePath, "dragon_vrip_res2.obj"));
            dragonMesh.LoadBuffers();

            //mesh.LoadBuffers();

            //triangles = RandomTriangleScene.CreateRandomTriangles(20);
        }

        public void Draw()
        {
            GL.ClearColor(0.8f, 0.8f, 0.8f, 1f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            // stars
            GL.BindTexture(TextureTarget.Texture2D, starsTexture);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.PushMatrix();
            GL.Translate(0, 20, 40);
            GL.Scale(20, 20, 1);
            DrawQuad();
            GL.PopMatrix();

            // ground
            GL.BindTexture(TextureTarget.Texture2D, groundTexture);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.PushMatrix();
            GL.Translate(0, 0, 20);
            GL.Rotate(90f, Vector3.UnitX);
            GL.Scale(20, 20, 1);
            DrawQuad(20);
            GL.PopMatrix();

            // crates
            GL.BindTexture(TextureTarget.Texture2D, crateTexture);
            GL.PushMatrix();
            GL.Translate(1, 0, 5);
            GL.Scale(0.5, 0.5, 0.5);
            crateMesh.Draw();
            GL.PopMatrix();

            GL.PushMatrix();
            GL.Translate(-1, 0, 10);
            GL.Scale(0.5, 0.5, 0.5);
            crateMesh.Draw();
            GL.PopMatrix();

            GL.PushMatrix();
            GL.Translate(-3, 0, 15);
            GL.Scale(0.5, 0.5, 0.5);
            crateMesh.Draw();
            GL.PopMatrix();

            // medieval street
            GL.BindTexture(TextureTarget.Texture2D, streetTexture);
            GL.PushMatrix();
            GL.Translate(10, 0, 15);
            GL.Rotate(-90.0, Vector3d.UnitY);
            GL.Scale(0.5, 0.5, 0.5);
            streetMesh.Draw();
            GL.PopMatrix();

            // dragon
            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.PushMatrix();
            GL.Translate(-5, -1, 10);
            GL.Rotate(-45.0, Vector3d.UnitY);
            GL.Scale(20, 20, 20);
            dragonMesh.Draw();
            GL.PopMatrix();

            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.PushMatrix();
            GL.Translate(-7, -1, 15);
            GL.Rotate(-45.0, Vector3d.UnitY);
            GL.Scale(20, 20, 20);
            dragonMesh.Draw();
            GL.PopMatrix();

            GL.BindTexture(TextureTarget.Texture2D, 0);

            // NOTE: it needs DepthPeelerFS.glsl -> shadeFragment to output gl_FrontColor
            //triangles.Draw();
        }

        private static void DrawQuad()
        {
            DrawQuad(1.0f);
        }

        private static void DrawQuad(float textureRepeat)
        {
            GL.Begin(BeginMode.Quads);
            {
                GL.TexCoord2(0f, textureRepeat);
                GL.Vertex2(-1.0f, 1.0f);
                GL.TexCoord2(0.0f, 0.0f);
                GL.Vertex2(-1.0f, -1.0f);
                GL.TexCoord2(textureRepeat, 0.0f);
                GL.Vertex2(1.0f, -1.0f);
                GL.TexCoord2(textureRepeat, textureRepeat);
                GL.Vertex2(1.0f, 1.0f);
            }
            GL.End();
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

        static uint GenerateHdrStarsTex(int width, int height)
        {
            uint texture = (uint)GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, texture);

            int bands = 3;
            int textureSize = bands * width * height;

            IList<Star> stars = Star.GenerateStars((int)(width * height * 0.002), 1e2f, false, 1);

            IntPtr texturePtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(Half)) * textureSize);
            unsafe
            {
                // zero out the texture
                Half zero = (Half)0.0;
                for (int y = 0; y < height; y++)
                {
                    Half* row = (Half*)texturePtr + bands * y * width;
                    int index = 0;
                    for (int x = 0; x < width; x++)
                    {
                        for (int band = 0; band < bands; band++)
                        {
                            row[index++] = zero;
                        }
                    }
                }

                // put the stars into the image
                Half* image = (Half*)texturePtr;
                foreach (var star in stars)
                {
                    int x = (int)(star.Position.X * width);
                    int y = (int)(star.Position.Y * height);
                    int index = bands * (y * width + x);
                    image[index] = (Half)star.Color.X;
                    image[index + 1] = (Half)star.Color.Y;
                    image[index + 2] = (Half)star.Color.Z;
                }
            }

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb16f,
                width, height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Rgb, PixelType.HalfFloat, texturePtr);
            Marshal.FreeHGlobal(texturePtr);

            //GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            //GL.Ext.GenerateMipmap(GenerateMipmapTarget.Texture2D);

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

        class Star
        {
            public Vector2 Position;
            public Vector3 Color;

            public static IList<Star> GenerateStars(int count, float intensity, bool colorize, int seed)
            {
                Sampler sampler = new Sampler(seed);
                Random random = new Random(seed);
                var starPositions = sampler.GenerateJitteredSamples((int)Math.Sqrt(count)).ToList();
                List<Star> stars = new List<Star>(starPositions.Count());

                foreach (var starPosition in starPositions)
                {
                    Vector3 color = new Vector3(intensity, intensity, intensity);
                    if (colorize)
                    {
                        color = Vector3.Multiply(color, new Vector3(
                            (float)random.NextDouble(),
                            (float)random.NextDouble(),
                            (float)random.NextDouble()));
                    }
                    else
                    {
                        color = Vector3.Multiply(color, (float)random.NextDouble());
                    }
                    Vector2 position = new Vector2((float)starPosition.X, (float)starPosition.Y);
                    stars.Add(new Star() { Position = position, Color = color });
                }
                return stars;
            }
        }
    }
}
