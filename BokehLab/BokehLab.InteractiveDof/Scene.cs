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
        public MaterialShaderManager ShaderManager { get; private set; }

        Mesh crateMesh;
        uint crateTexture;

        Mesh streetMesh;
        uint streetTexture;

        Mesh dragonMesh;

        Mesh teapotMesh;

        uint starsTexture;
        uint colorStarsTexture;
        uint groundTexture;

        int TotalBigModelsCount = 4;
        private int bigModelsEnabledCount = 3;
        public int BigModelsEnabledCount
        {
            get
            {
                return bigModelsEnabledCount;
            }
            set
            {
                bigModelsEnabledCount = BokehLab.Math.MathHelper.Mod(
                    value, TotalBigModelsCount + 1);
            }
        }

        public bool BigModelsEnabled { get; set; }
        public bool ColorizeStars { get; set; }

        public string ResourcePath { get; set; }

        //RandomTriangleScene triangles;

        /// <summary>
        /// Creates a new scene. This can be done after GL context has been created.
        /// </summary>
        public Scene()
        {
            BigModelsEnabled = true;

            ShaderManager = new MaterialShaderManager();

            LoadResources();
            LoadMaterials();
        }

        public void Draw()
        {
            //GL.ClearColor(0.8f, 0.8f, 0.8f, 1f);
            GL.ClearColor(0f, 0f, 0f, 1f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            int shaderProgram = ShaderManager.UseMaterial("singleTexture");
            GL.Uniform1(GL.GetUniformLocation(shaderProgram, "texture0"), 0); // texture unit 0
            GL.ActiveTexture(TextureUnit.Texture0);

            // stars
            GL.BindTexture(TextureTarget.Texture2D, ColorizeStars ? colorStarsTexture : starsTexture);
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

            // medieval street
            GL.BindTexture(TextureTarget.Texture2D, streetTexture);
            GL.PushMatrix();
            GL.Translate(15, 0, 17);
            GL.Rotate(-90.0, Vector3d.UnitY);
            GL.Scale(0.75, 0.75, 0.75);
            streetMesh.Draw();
            GL.PopMatrix();

            GL.BindTexture(TextureTarget.Texture2D, streetTexture);
            GL.PushMatrix();
            GL.Translate(-15, 0, 20);
            GL.Rotate(90.0, Vector3d.UnitY);
            GL.Scale(0.75, 0.75, 0.75);
            streetMesh.Draw();
            GL.PopMatrix();

            GL.PushMatrix();
            GL.Translate(5, 0, 10);

            // crates
            GL.BindTexture(TextureTarget.Texture2D, crateTexture);
            GL.PushMatrix();
            GL.Translate(3, 0, 0);
            GL.Scale(0.5, 0.5, 0.5);
            crateMesh.Draw();
            GL.PopMatrix();

            GL.PushMatrix();
            GL.Translate(2, 0, 5);
            GL.Scale(0.5, 0.5, 0.5);
            crateMesh.Draw();
            GL.PopMatrix();

            GL.PushMatrix();
            GL.Translate(1, 0, 10);
            GL.Scale(0.5, 0.5, 0.5);
            crateMesh.Draw();
            GL.PopMatrix();

            GL.PushMatrix();
            GL.Translate(0, 0, 15);
            GL.Scale(0.5, 0.5, 0.5);
            crateMesh.Draw();
            GL.PopMatrix();

            shaderProgram = ShaderManager.UseMaterial("diffuseLighting");
            GL.Uniform1(GL.GetUniformLocation(shaderProgram, "diffuseCoeff"), 0.7f);
            GL.Uniform1(GL.GetUniformLocation(shaderProgram, "ambient"), 0.2f);
            GL.BindTexture(TextureTarget.Texture2D, 0);

            if (BigModelsEnabled)
            {
                if (BigModelsEnabledCount > 0)
                {
                    GL.Uniform3(GL.GetUniformLocation(shaderProgram, "baseColor"), new Vector3(0.75f, 0.75f, 1.0f));
                    GL.PushMatrix();
                    GL.Translate(-2, -1, 0);
                    GL.Rotate(-45.0, Vector3d.UnitY);
                    GL.Scale(20, 20, 20);
                    dragonMesh.Draw();
                    GL.PopMatrix();

                    GL.PushMatrix();
                    GL.Translate(-7, 0, 0);
                    GL.Scale(0.25, 0.25, 0.25);
                    teapotMesh.Draw();
                    GL.PopMatrix();
                }

                if (BigModelsEnabledCount > 1)
                {
                    GL.Uniform3(GL.GetUniformLocation(shaderProgram, "baseColor"), new Vector3(0.75f, 1.0f, 0.75f));
                    GL.PushMatrix();
                    GL.Translate(-3, -1, 5);
                    GL.Rotate(-45.0, Vector3d.UnitY);
                    GL.Scale(20, 20, 20);
                    dragonMesh.Draw();
                    GL.PopMatrix();

                    GL.PushMatrix();
                    GL.Translate(-8, 0, 5);
                    GL.Scale(0.25, 0.25, 0.25);
                    teapotMesh.Draw();
                    GL.PopMatrix();
                }

                if (BigModelsEnabledCount > 2)
                {
                    GL.Uniform3(GL.GetUniformLocation(shaderProgram, "baseColor"), new Vector3(1.0f, 0.75f, 0.75f));
                    GL.PushMatrix();
                    GL.Translate(-4, -1, 10);
                    GL.Rotate(-45.0, Vector3d.UnitY);
                    GL.Scale(20, 20, 20);
                    dragonMesh.Draw();
                    GL.PopMatrix();

                    GL.PushMatrix();
                    GL.Translate(-9, 0, 10);
                    GL.Scale(0.25, 0.25, 0.25);
                    teapotMesh.Draw();
                    GL.PopMatrix();
                }
                if (BigModelsEnabledCount > 3)
                {
                    GL.Uniform3(GL.GetUniformLocation(shaderProgram, "baseColor"), new Vector3(1f, 1f, 1f));
                    GL.PushMatrix();
                    GL.Translate(-5, -1, 15);
                    GL.Rotate(-45.0, Vector3d.UnitY);
                    GL.Scale(20, 20, 20);
                    dragonMesh.Draw();
                    GL.PopMatrix();

                    GL.PushMatrix();
                    GL.Translate(-10, 0, 15);
                    GL.Scale(0.25, 0.25, 0.25);
                    teapotMesh.Draw();
                    GL.PopMatrix();
                }
            }

            GL.PopMatrix();

            // NOTE: it needs DepthPeelerFS.glsl -> shadeFragment to output gl_FrontColor
            //shaderProgram = ShaderManager.UseMaterial("default");
            //triangles.Draw();

            GL.UseProgram(0);
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

        private void LoadMaterials()
        {
            ShaderManager.AddMaterial(
                "default",
                new[] { "Materials/DefaultVS.glsl" },
                new[] { "Materials/SingleColorFS.glsl" });
            ShaderManager.AddMaterial(
                "singleTexture",
                new[] { "Materials/DefaultVS.glsl" },
                new[] { "Materials/SingleTextureFS.glsl" });
            ShaderManager.AddMaterial(
                "diffuseLighting",
                new[] { "Materials/LightingVS.glsl" },
                new[] { "Materials/LightingFS.glsl" });
        }

        private void LoadResources()
        {
            ResourcePath = @"data";
            starsTexture = GenerateHdrStarsTex(512, 512, (int)(512 * 512 * 0.002), 1e2f, false, 1);
            colorStarsTexture = GenerateHdrStarsTex(512, 512, (int)(512 * 512 * 0.002), 1e2f, true, 1);
            groundTexture = LoadTexture(Path.Combine(ResourcePath, "dirt_01.jpg"));

            GL.EnableClientState(ArrayCap.VertexArray);
            GL.EnableClientState(ArrayCap.NormalArray);
            GL.EnableClientState(ArrayCap.TextureCoordArray);

            GL.ShadeModel(ShadingModel.Flat);

            crateTexture = LoadTexture(Path.Combine(ResourcePath, "CrateNoParachute.png"));
            crateMesh = new Mesh(Path.Combine(ResourcePath, "CrateNoParachuteOBJ.obj"));
            crateMesh.LoadBuffers();

            streetTexture = LoadTexture(Path.Combine(ResourcePath, "rue2.jpg"));
            streetMesh = new Mesh(Path.Combine(ResourcePath, "medstreet.obj"));
            streetMesh.LoadBuffers();

            GL.ShadeModel(ShadingModel.Smooth);

            dragonMesh = new Mesh(Path.Combine(ResourcePath, "dragon_vrip_res2.obj"));
            dragonMesh.LoadBuffers();

            teapotMesh = new Mesh(Path.Combine(ResourcePath, "teapot.obj"));
            teapotMesh.LoadBuffers();

            //mesh = new Mesh(Path.Combine(ResourcePath, "DW-Ormesh-05.obj"));
            //mesh = new Mesh(Path.Combine(ResourcePath, "DW-Fungau.obj"));
            //texture = LoadTexture(Path.Combine(ResourcePath, "checker_large.gif"));

            //mesh.LoadBuffers();

            //triangles = RandomTriangleScene.CreateRandomTriangles(20);
        }

        static uint LoadTexture(string file)
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

        static uint GenerateHdrStarsTex(int width, int height, int starCount, float intensity, bool colorize, int seed)
        {
            uint texture = (uint)GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, texture);

            int bands = 3;
            int textureSize = bands * width * height;

            IList<Star> stars = Star.GenerateStars(starCount, intensity, colorize, seed);

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
