using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Threading;
using System.Drawing;
using System.Drawing.Imaging;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;


namespace Meshomatic.DisplayMesh
{
    public class DisplayMesh : GameWindow
    {
        const float rotation_speed = 40.0f;
        float angle;

        MeshData m;
        uint dataBuffer;
        uint indexBuffer;
        uint tex;
        int vertOffset, normOffset, texcoordOffset;
        OpenTK.Vector3d up = new OpenTK.Vector3d(0.0, 1.0, 0.0);
        OpenTK.Vector3d viewDirection = new OpenTK.Vector3d(1.0, 1.0, 1.0);
        double viewDist = 1.0;

        string vShaderSource = @"
void main() {
	gl_Position = ftransform();
	gl_TexCoord[0] = gl_MultiTexCoord0;
}
";
        string fShaderSource = @"
uniform sampler2D tex;
void main() {
	gl_FragColor = texture2D(tex, gl_TexCoord[0].st);
}
";

        public DisplayMesh()
            : base(800, 600, new GraphicsMode(16, 16))
        { }


        int CompileShaders()
        {
            int programHandle, vHandle, fHandle;
            vHandle = GL.CreateShader(ShaderType.VertexShader);
            fHandle = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(vHandle, vShaderSource);
            GL.ShaderSource(fHandle, fShaderSource);
            GL.CompileShader(vHandle);
            GL.CompileShader(fHandle);
            Console.Write(GL.GetShaderInfoLog(vHandle));
            Console.Write(GL.GetShaderInfoLog(fHandle));

            programHandle = GL.CreateProgram();
            GL.AttachShader(programHandle, vHandle);
            GL.AttachShader(programHandle, fHandle);
            GL.LinkProgram(programHandle);
            Console.Write(GL.GetProgramInfoLog(programHandle));
            return programHandle;
        }

        void LoadBuffers(MeshData m)
        {
            float[] verts, norms, texcoords;
            uint[] indices;
            m.OpenGLArrays(out verts, out norms, out texcoords, out indices);
            GL.GenBuffers(1, out dataBuffer);
            GL.GenBuffers(1, out indexBuffer);

            // Set up data for VBO.
            // We're going to use one VBO for all geometry, and stick it in 
            // in (VVVVNNNNCCCC) order.  Non interleaved.
            int buffersize = (verts.Length + norms.Length + texcoords.Length);
            float[] bufferdata = new float[buffersize];
            vertOffset = 0;
            normOffset = verts.Length;
            texcoordOffset = (verts.Length + norms.Length);

            verts.CopyTo(bufferdata, vertOffset);
            norms.CopyTo(bufferdata, normOffset);
            texcoords.CopyTo(bufferdata, texcoordOffset);

            bool v = false;
            for (int i = texcoordOffset; i < bufferdata.Length; i++)
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

            // Load geometry data
            GL.BindBuffer(BufferTarget.ArrayBuffer, dataBuffer);
            GL.BufferData<float>(BufferTarget.ArrayBuffer, (IntPtr)(buffersize * sizeof(float)), bufferdata,
                          BufferUsageHint.StaticDraw);

            // Load index data
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBuffer);
            GL.BufferData<uint>(BufferTarget.ElementArrayBuffer,
                          (IntPtr)(indices.Length * sizeof(uint)), indices, BufferUsageHint.StaticDraw);
        }

        void DrawBuffer()
        {
            // Push current Array Buffer state so we can restore it later
            GL.PushClientAttrib(ClientAttribMask.ClientVertexArrayBit);

            GL.ClientActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, tex);

            GL.BindBuffer(BufferTarget.ArrayBuffer, dataBuffer);
            // Normal buffer
            GL.NormalPointer(NormalPointerType.Float, 0, (IntPtr)(normOffset * sizeof(float)));

            // TexCoord buffer
            GL.TexCoordPointer(2, TexCoordPointerType.Float, 0, (IntPtr)(texcoordOffset * sizeof(float)));

            // Vertex buffer
            GL.VertexPointer(3, VertexPointerType.Float, 0, (IntPtr)(vertOffset * sizeof(float)));

            // Index array
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBuffer);
            GL.DrawElements(BeginMode.Triangles, m.Tris.Length * 3, DrawElementsType.UnsignedInt, IntPtr.Zero);

            // Restore the state
            GL.PopClientAttrib();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            //MeshData m = new ColladaLoader().LoadFile("dice.dae");
            //m = ObjLoader.LoadFile("cube.obj");
            //tex = LoadTex("cube.png");

            //m = ObjLoader.LoadFile("teapot.obj");

            //m = ObjLoader.LoadFile("house.obj");
            //tex = LoadTex("house_diffuse.png");

            //m = ObjLoader.LoadFile("happy_vrip_res4.obj");
            //m = ObjLoader.LoadFile("bun_zipper.obj");
            //tex = LoadTex("bun_zipper_color.png");

            //m = ObjLoader.LoadFile("wateringcan.obj");
            //m = ObjLoader.LoadFile("toyplane.obj");

            //m = ObjLoader.LoadFile("CrateNoParachuteOBJ.obj");
            //tex = LoadTex("CrateNoParachute.png");

            //m = ObjLoader.LoadFile("Armadillo.obj");
            //m = ObjLoader.LoadFile("dragon_vrip_res2.obj");
            //m = ObjLoader.LoadFile("DW-Ormesh-05.obj"); // [lee2008] - per-pixel splatting
            //m = ObjLoader.LoadFile("DW-Fungau.obj");

            m = ObjLoader.LoadFile("medstreet.obj");
            tex = LoadTex("rue2.jpg");

            //m = ObjLoader.LoadFile("FontariolV3.obj"); // not ok
            //tex = LoadTex("fonatriolT.jpg");


            //tex = LoadTex("checker_large.gif");
            //tex = LoadTex("dirt_01.jpg");

            //m = ObjLoader.LoadFile("CrateNoParachuteOBJ.obj");
            //tex = LoadTex("CrateNoParachute.png");


            //m = new Ms3dLoader().LoadFile("test.ms3d");
            //m = ObjLoader.LoadFile("test.obj");
            //tex = LoadTex("test.png");

            // We do some heuristics to try to auto-zoom to a reasonable distance.  And it generally works!
            double w, l, h;
            double maxdim;
            m.Dimensions(out w, out l, out h);
            Console.WriteLine("Model dimensions: {0} x {0} x {0} (theoretically)", w, l, h);
            maxdim = Math.Max(Math.Max(w, l), h);
            viewDist = (float)(maxdim * 2);

            GL.ClearColor(Color.MidnightBlue);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Texture2D);
            GL.EnableClientState(ArrayCap.VertexArray);
            GL.EnableClientState(ArrayCap.NormalArray);
            GL.EnableClientState(ArrayCap.TextureCoordArray);

            GL.UseProgram(CompileShaders());
            LoadBuffers(m);

            Console.WriteLine("Use a and z to zoom in and out, escape to quit.");
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, Width, Height);

            double aspect_ratio = Width / (double)Height;

            OpenTK.Matrix4 perspective = OpenTK.Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, (float)aspect_ratio, 0.1f, 64000f);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref perspective);
        }


        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            if (Keyboard[OpenTK.Input.Key.Escape])
            {
                this.Exit();
                return;
            }
            if (Keyboard[OpenTK.Input.Key.Z])
            {
                viewDist *= 1.1f;
                Console.WriteLine("View distance: {0}", viewDist);
            }
            else if (Keyboard[OpenTK.Input.Key.A])
            {
                viewDist *= 0.9f;
                Console.WriteLine("View distance: {0}", viewDist);
            }
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();

            // SET CAMERA, THEN DRAW STUFF DAMMIT
            Matrix4d camera = Matrix4d.LookAt(OpenTK.Vector3d.Multiply(viewDirection, viewDist),
                                                OpenTK.Vector3d.Zero, up);
            GL.LoadMatrix(ref camera);

            angle += rotation_speed * (float)e.Time;
            GL.Rotate(angle, 0.0f, 1.0f, 0.0f);
            DrawBuffer();

            SwapBuffers();
            Thread.Sleep(1);
        }

        // Example of how you would draw things in (deprecated) immediate mode.
        /*
        private void DrawMesh() {
            GL.BindTexture(TextureTarget.Texture2D, tex);
            GL.Begin(BeginMode.Triangles);
			
            foreach(Tri t in m.Tris) {
                foreach(Point p in t.Points()) {
                    Vector3 v = m.Vertices[p.Vertex];
                    Vector3 n = m.Normals[p.Normal];
                    Vector2 tc = m.TexCoords[p.TexCoord];
                    GL.Normal3(n.X, n.Y, n.Z);
                    GL.TexCoord2(tc.X, 1- tc.Y);
                    GL.Vertex3(v.X, v.Y, v.Z);
                }
            }
			
            GL.End();
        }
        */

        [STAThread]
        public static void Main()
        {
            using (DisplayMesh example = new DisplayMesh())
            {
                example.Run(30.0, 0.0);
            }
        }

        // XXX: TODO: Make this work
        static bool IsPowerOf2(Bitmap b)
        {
            //uint w = bitmap.Width;
            //uint h = bitmap.Height;

            return true;
        }

        // Maybe factor this out into another part of the library?
        // This loads the texture mirrored along one axis, but you can
        // easily fix this by changing all your UVs' second coordinate from 
        // v to 1-v
        static uint LoadTex(string file)
        {
            Bitmap bitmap = new Bitmap(file);
            if (!IsPowerOf2(bitmap))
            {
                // XXX: FormatException isn't really the best here, buuuut...
                throw new FormatException("Texture sizes must be powers of 2!");
            }

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
    }
}
