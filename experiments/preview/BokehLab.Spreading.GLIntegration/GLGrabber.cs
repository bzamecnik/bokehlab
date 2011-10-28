#region --- License ---
/* Licensed under the MIT/X11 license.
 * Copyright (c) 2006-2008 the OpenTK Team.
 * This notice may not be removed from any source distribution.
 * See license.txt for licensing details.
 */
#endregion

using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using BokehLab.FloatMap;
using BokehLab.Spreading;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace BokehLab.Spreading.GLIntegration
{
    public class GLGrabber : GameWindow
    {
        public GLGrabber()
            : base(1100, 600)
        {
            spreadingFilter = new RectangleSpreadingFilter();
            thinLensBlur = new ThinLensDepthMapBlur(2, 10, 1, -1, 0);
            spreadingFilter.Blur = thinLensBlur;
        }

        uint ColorTexture;
        uint DepthTexture;
        uint SpreadedTexture;
        uint FBOHandle;

        Size TextureSize = new Size(512, 512);//(int)(0.75 * 512));

        int vertexShaderObject, fragmentShaderObject, shaderProgram;

        AbstractSpreadingFilter spreadingFilter;
        ThinLensDepthMapBlur thinLensBlur;

        Bitmap spreaded;

        #region Randoms

        Random rnd = new Random();
        public const float scale = 2f;

        /// <summary>Returns a random Float in the range [-0.5*scale..+0.5*scale]</summary>
        public float GetRandom()
        {
            return (float)(rnd.NextDouble() - 0.5) * scale;
        }

        /// <summary>Returns a random Float in the range [0..1]</summary>
        public float GetRandom0to1()
        {
            return (float)rnd.NextDouble();
        }

        #endregion Randoms

        protected override void OnLoad(EventArgs e)
        {
            if (!GL.GetString(StringName.Extensions).Contains("EXT_framebuffer_object"))
            {
                System.Windows.Forms.MessageBox.Show(
                     "Your video card does not support Framebuffer Objects. Please update your drivers.",
                     "FBOs not supported",
                     System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Exclamation);
                Exit();
            }

            GL.Enable(EnableCap.DepthTest);
            GL.ClearDepth(1.0f);
            GL.DepthFunc(DepthFunction.Lequal);

            GL.Disable(EnableCap.CullFace);
            GL.PolygonMode(MaterialFace.Back, PolygonMode.Fill);


            //GL.Light(LightName.Light0, LightParameter.Position, new Vector4(10, 10, 0, 1));
            //GL.Light(LightName.Light0, LightParameter.Ambient, new Vector4(0, 0, 0, 1));
            //GL.Light(LightName.Light0, LightParameter.Diffuse, new Vector4(1, 1, 1, 1));
            //GL.Light(LightName.Light0, LightParameter.Specular, new Vector4(1, 1, 1, 1));
            //GL.LightModel(LightModelParameter.LightModelAmbient, 0.2f);

            //GL.Enable(EnableCap.Lighting);
            //GL.Enable(EnableCap.Light0);
            //GL.Enable(EnableCap.ColorMaterial);

            //GL.ColorMaterial(MaterialFace.Front, ColorMaterialParameter.AmbientAndDiffuse);
            //GL.Material(MaterialFace.Front, MaterialParameter.Specular, new Vector4(1, 1, 1, 1));
            //GL.Material(MaterialFace.Front, MaterialParameter.Emission, new Vector4(0, 0, 0, 1));


            // Create Color Tex
            GL.GenTextures(1, out ColorTexture);
            GL.BindTexture(TextureTarget.Texture2D, ColorTexture);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb32f, TextureSize.Width, TextureSize.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Rgb, PixelType.Float, IntPtr.Zero);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToBorder);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToBorder);
            // GL.Ext.GenerateMipmap( GenerateMipmapTarget.Texture2D );

            // Create Depth Tex
            GL.GenTextures(1, out DepthTexture);
            GL.BindTexture(TextureTarget.Texture2D, DepthTexture);
            GL.TexImage2D(TextureTarget.Texture2D, 0, (PixelInternalFormat)All.DepthComponent32, TextureSize.Width, TextureSize.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.DepthComponent, PixelType.UnsignedInt, IntPtr.Zero);
            //GL.TexImage2D(TextureTarget.Texture2D, 0, (PixelInternalFormat)All.DepthComponent16, TextureSize.Width, TextureSize.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.DepthComponent, PixelType.UnsignedShort, IntPtr.Zero);
            // things go horribly wrong if DepthComponent's Bitcount does not match the main Framebuffer's Depth
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToBorder);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToBorder);

            // Create spreaded image texture
            GL.GenTextures(1, out SpreadedTexture);
            GL.BindTexture(TextureTarget.Texture2D, SpreadedTexture);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, TextureSize.Width, TextureSize.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Rgb, PixelType.UnsignedByte, IntPtr.Zero);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToBorder);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToBorder);


            //GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureCompareMode, (int)TextureCompareMode.CompareRToTexture);

            // GL.Ext.GenerateMipmap( GenerateMipmapTarget.Texture2D );

            // Create a FBO and attach the textures
            GL.Ext.GenFramebuffers(1, out FBOHandle);
            GL.Ext.BindFramebuffer(FramebufferTarget.FramebufferExt, FBOHandle);
            GL.Ext.FramebufferTexture2D(FramebufferTarget.FramebufferExt, FramebufferAttachment.ColorAttachment0Ext, TextureTarget.Texture2D, ColorTexture, 0);
            GL.Ext.FramebufferTexture2D(FramebufferTarget.FramebufferExt, FramebufferAttachment.DepthAttachmentExt, TextureTarget.Texture2D, DepthTexture, 0);

            #region Test for Error

            switch (GL.Ext.CheckFramebufferStatus(FramebufferTarget.FramebufferExt))
            {
                case FramebufferErrorCode.FramebufferCompleteExt:
                    {
                        Console.WriteLine("FBO: The framebuffer is complete and valid for rendering.");
                        break;
                    }
                case FramebufferErrorCode.FramebufferIncompleteAttachmentExt:
                    {
                        Console.WriteLine("FBO: One or more attachment points are not framebuffer attachment complete. This could mean there’s no texture attached or the format isn’t renderable. For color textures this means the base format must be RGB or RGBA and for depth textures it must be a DEPTH_COMPONENT format. Other causes of this error are that the width or height is zero or the z-offset is out of range in case of render to volume.");
                        break;
                    }
                case FramebufferErrorCode.FramebufferIncompleteMissingAttachmentExt:
                    {
                        Console.WriteLine("FBO: There are no attachments.");
                        break;
                    }
                /* case  FramebufferErrorCode.GL_FRAMEBUFFER_INCOMPLETE_DUPLICATE_ATTACHMENT_EXT: 
                     {
                         Console.WriteLine("FBO: An object has been attached to more than one attachment point.");
                         break;
                     }*/
                case FramebufferErrorCode.FramebufferIncompleteDimensionsExt:
                    {
                        Console.WriteLine("FBO: Attachments are of different size. All attachments must have the same width and height.");
                        break;
                    }
                case FramebufferErrorCode.FramebufferIncompleteFormatsExt:
                    {
                        Console.WriteLine("FBO: The color attachments have different format. All color attachments must have the same format.");
                        break;
                    }
                case FramebufferErrorCode.FramebufferIncompleteDrawBufferExt:
                    {
                        Console.WriteLine("FBO: An attachment point referenced by GL.DrawBuffers() doesn’t have an attachment.");
                        break;
                    }
                case FramebufferErrorCode.FramebufferIncompleteReadBufferExt:
                    {
                        Console.WriteLine("FBO: The attachment point referenced by GL.ReadBuffers() doesn’t have an attachment.");
                        break;
                    }
                case FramebufferErrorCode.FramebufferUnsupportedExt:
                    {
                        Console.WriteLine("FBO: This particular FBO configuration is not supported by the implementation.");
                        break;
                    }
                default:
                    {
                        Console.WriteLine("FBO: Status unknown. (yes, this is really bad.)");
                        break;
                    }
            }

            // using FBO might have changed states, e.g. the FBO might not support stereoscopic views or double buffering
            int[] queryinfo = new int[6];
            GL.GetInteger(GetPName.MaxColorAttachmentsExt, out queryinfo[0]);
            GL.GetInteger(GetPName.AuxBuffers, out queryinfo[1]);
            GL.GetInteger(GetPName.MaxDrawBuffers, out queryinfo[2]);
            GL.GetInteger(GetPName.Stereo, out queryinfo[3]);
            GL.GetInteger(GetPName.Samples, out queryinfo[4]);
            GL.GetInteger(GetPName.Doublebuffer, out queryinfo[5]);
            Console.WriteLine("max. ColorBuffers: " + queryinfo[0] + " max. AuxBuffers: " + queryinfo[1] + " max. DrawBuffers: " + queryinfo[2] +
                               "\nStereo: " + queryinfo[3] + " Samples: " + queryinfo[4] + " DoubleBuffer: " + queryinfo[5]);

            Console.WriteLine("Last GL Error: " + GL.GetError());

            #endregion Test for Error

            using (StreamReader vs = new StreamReader("VertexShader.glsl"))
            using (StreamReader fs = new StreamReader("FragmentShader.glsl"))
                CreateShaders(vs.ReadToEnd(), fs.ReadToEnd(),
                    out vertexShaderObject, out fragmentShaderObject,
                    out shaderProgram);


            DrawRandomScene();
            Spread();

            Keyboard.KeyUp += KeyUp;
        }

        private void KeyUp(object sender, KeyboardKeyEventArgs e)
        {
            if (e.Key == Key.R)
            {
                DrawRandomScene();
                Spread();
            }
            if (e.Key == Key.F5)
            {
                spreaded.Save("spreaded.png", System.Drawing.Imaging.ImageFormat.Png);
            }
        }

        private void Spread()
        {
            FloatMapImage color = GetColorTexture();
            FloatMapImage depth = GetDepthTexture();
            spreaded = SpreadFrame(color, depth).ToBitmap(true);
            LoadSpreadedImageTexture(spreaded);
        }

        private FloatMapImage GetColorTexture()
        {
            // pixel format: unsinged int

            //Bitmap bmp = new Bitmap(TextureSize.Width, TextureSize.Height);
            //System.Drawing.Imaging.BitmapData data =
            //    bmp.LockBits(new System.Drawing.Rectangle(0, 0, TextureSize.Width, TextureSize.Height),
            //                 System.Drawing.Imaging.ImageLockMode.WriteOnly,
            //                 System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            //GL.BindTexture(TextureTarget.Texture2D, ColorTexture);
            //GL.GetTexImage(TextureTarget.Texture2D, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgr, PixelType.UnsignedByte, data.Scan0);
            //GL.BindTexture(TextureTarget.Texture2D, 0);
            //bmp.UnlockBits(data);
            ////bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);
            //return bmp.ToFloatMap();

            int bands = 3; // RGB

            IntPtr colorTextureFloatPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(float)) * bands * TextureSize.Width * TextureSize.Height);
            GL.BindTexture(TextureTarget.Texture2D, ColorTexture);
            GL.GetTexImage(TextureTarget.Texture2D, 0, OpenTK.Graphics.OpenGL.PixelFormat.Rgb, PixelType.Float, colorTextureFloatPtr);
            GL.BindTexture(TextureTarget.Texture2D, 0);

            FloatMapImage colorImage = new FloatMapImage((uint)TextureSize.Width, (uint)TextureSize.Height, BokehLab.FloatMap.PixelFormat.RGB);
            var image = colorImage.Image;

            unsafe
            {
                int inputStride = bands * TextureSize.Width;
                for (int y = 0; y < TextureSize.Height; y++)
                {
                    float* inputRow = (float*)colorTextureFloatPtr + (y * inputStride);
                    int xIndex = 0;
                    for (int x = 0; x < TextureSize.Width; x++)
                    {
                        for (int band = 0; band < bands; band++)
                        {
                            image[x, y, band] = inputRow[xIndex];
                            xIndex++;
                        }
                    }
                }
            }
            return colorImage;
        }

        private FloatMapImage GetDepthTexture()
        {
            // allocate 32-bit unit unmanaged array to grab the depth buffer
            IntPtr depthBufferUInt32Ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(UInt32)) * TextureSize.Width * TextureSize.Height);
            GL.BindTexture(TextureTarget.Texture2D, DepthTexture);
            GL.GetTexImage(TextureTarget.Texture2D, 0, OpenTK.Graphics.OpenGL.PixelFormat.DepthComponent, PixelType.UnsignedInt, depthBufferUInt32Ptr);
            GL.BindTexture(TextureTarget.Texture2D, 0);

            FloatMapImage depthImage = new FloatMapImage((uint)TextureSize.Width, (uint)TextureSize.Height, BokehLab.FloatMap.PixelFormat.Greyscale);
            var image = depthImage.Image;

            unsafe
            {
                int inputStride = TextureSize.Width;
                float conversionFactor = 1 / (float)UInt32.MaxValue;
                for (int y = 0; y < TextureSize.Height; y++)
                {
                    UInt32* inputRow = (UInt32*)depthBufferUInt32Ptr + (y * inputStride);
                    for (int x = 0; x < TextureSize.Width; x++)
                    {
                        image[x, y, 0] = inputRow[x] * conversionFactor;
                    }
                }
            }
            return depthImage;
        }

        private void LoadSpreadedImageTexture(Bitmap image)
        {
            var imageData = image.LockBits(
                new Rectangle(0, 0, image.Width, image.Height),
                System.Drawing.Imaging.ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            GL.BindTexture(TextureTarget.Texture2D, SpreadedTexture);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb8,
                image.Width, image.Height, 0,
                OpenTK.Graphics.OpenGL.PixelFormat.Bgr, PixelType.UnsignedByte, imageData.Scan0);

            image.UnlockBits(imageData);
        }


        private FloatMapImage SpreadFrame(FloatMapImage color, FloatMapImage depth)
        {
            thinLensBlur.DepthMap = depth;
            return spreadingFilter.FilterImage(color, null);
        }

        private void DrawRandomScene()
        {
            GL.Disable(EnableCap.Texture2D);

            //GL.Enable(EnableCap.Lighting);
            //GL.Enable(EnableCap.Light0);

            GL.Ext.BindFramebuffer(FramebufferTarget.FramebufferExt, FBOHandle);

            GL.UseProgram(shaderProgram);

            GL.MatrixMode(MatrixMode.Projection);
            GL.PushMatrix();
            {
                GL.LoadIdentity();
                GL.Ortho(-1, 1, -1, 1, 1, -1);
                GL.MatrixMode(MatrixMode.Modelview);
                GL.PushMatrix();
                {
                    Matrix4 lookat = Matrix4.LookAt(0, 0, 0, 0, 0, -100, 0, 1, 0);
                    GL.LoadMatrix(ref lookat);

                    GL.PushAttrib(AttribMask.ViewportBit);
                    {
                        GL.Viewport(0, 0, TextureSize.Width, TextureSize.Height);

                        // clear the screen in red, to make it very obvious what the clear affected. only the FBO, not the real framebuffer
                        GL.ClearColor(0f, 0f, 0f, 0f);
                        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                        // smack 50 random triangles into the FBO's textures
                        GL.Begin(BeginMode.Triangles);
                        {
                            // TODO: the HDR color have to be done in a shader

                            for (int i = 0; i < 10; i++)
                            {
                                GL.Color3(GetRandom0to1(), GetRandom0to1(), GetRandom0to1());
                                GL.Vertex3(GetRandom(), GetRandom(), GetRandom());
                                GL.Color3(GetRandom0to1(), GetRandom0to1(), GetRandom0to1());
                                GL.Vertex3(GetRandom(), GetRandom(), GetRandom());
                                GL.Color3(GetRandom0to1(), GetRandom0to1(), GetRandom0to1());
                                GL.Vertex3(GetRandom(), GetRandom(), GetRandom());
                            }

                            //GL.Color3(0.5, 0, 0); GL.Vertex3(0, 0, 0);
                            //GL.Color3(0, 0.5, 0); GL.Vertex3(1, 1, 0);
                            //GL.Color3(0, 0, 0.5); GL.Vertex3(0, 1, 0);

                            //GL.Color3(0.5, 0, 0); GL.Vertex3(0, 0, -1);
                            //GL.Color3(0, 0.5, 0); GL.Vertex3(1, 1, 0);
                            //GL.Color3(0, 0, 0.5); GL.Vertex3(0, 1, 1);

                            //GL.Color3(0.5, 0, 0); GL.Vertex3(-1, -1, -1);
                            //GL.Color3(0, 0.5, 0); GL.Vertex3(-1, 1, -1);
                            //GL.Color3(0, 0, 0.5); GL.Vertex3(1, -1, 1);
                        }
                        GL.End();
                    }
                    GL.PopAttrib();
                }
                GL.PopMatrix();
            }
            GL.MatrixMode(MatrixMode.Projection);
            GL.PopMatrix();
            GL.Ext.BindFramebuffer(FramebufferTarget.FramebufferExt, 0); // disable rendering into the FBO

            GL.UseProgram(0);
            //GL.Disable(EnableCap.Lighting);

            GL.Enable(EnableCap.Texture2D); // enable Texture Mapping
        }

        protected override void OnUnload(EventArgs e)
        {
            // Clean up what we allocated before exiting
            if (ColorTexture != 0)
                GL.DeleteTextures(1, ref ColorTexture);

            if (DepthTexture != 0)
                GL.DeleteTextures(1, ref DepthTexture);

            if (FBOHandle != 0)
                GL.Ext.DeleteFramebuffers(1, ref FBOHandle);

            if (shaderProgram != 0)
                GL.DeleteProgram(shaderProgram);
            if (fragmentShaderObject != 0)
                GL.DeleteShader(fragmentShaderObject);
        }

        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);

            double aspect_ratio = Width / (double)Height;

            OpenTK.Matrix4 perspective = OpenTK.Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, (float)aspect_ratio, 1, 64);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref perspective);

            Matrix4 lookat = Matrix4.LookAt(0, 0, 3, 0, 0, 0, 0, 1, 0);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref lookat);

            base.OnResize(e);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            if (Keyboard[Key.Escape])
            {
                this.Exit();
            }
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.ClearColor(.1f, .2f, .3f, 0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.Color3(1f, 1f, 1f);

            GL.BindTexture(TextureTarget.Texture2D, 0); // bind default texture

            GL.PushMatrix();
            {
                float aspect = TextureSize.Height / (float)TextureSize.Width;
                // Draw the Color Texture
                GL.Translate(-1.1f, 0f, 0f);
                GL.BindTexture(TextureTarget.Texture2D, ColorTexture);
                GL.Begin(BeginMode.Quads);
                {
                    GL.TexCoord2(0f, 1f);
                    GL.Vertex2(-1.0f, aspect);
                    GL.TexCoord2(0.0f, 0.0f);
                    GL.Vertex2(-1.0f, -aspect);
                    GL.TexCoord2(1.0f, 0.0f);
                    GL.Vertex2(1.0f, -aspect);
                    GL.TexCoord2(1.0f, 1.0f);
                    GL.Vertex2(1.0f, aspect);
                }
                GL.End();

                // Draw the Depth Texture
                GL.Translate(+2.2f, 0f, 0f);
                GL.BindTexture(TextureTarget.Texture2D, SpreadedTexture);
                GL.Begin(BeginMode.Quads);
                {
                    GL.TexCoord2(0f, 1f);
                    GL.Vertex2(-1.0f, aspect);
                    GL.TexCoord2(0.0f, 0.0f);
                    GL.Vertex2(-1.0f, -aspect);
                    GL.TexCoord2(1.0f, 0.0f);
                    GL.Vertex2(1.0f, -aspect);
                    GL.TexCoord2(1.0f, 1.0f);
                    GL.Vertex2(1.0f, aspect);
                }
                GL.End();
            }
            GL.PopMatrix();

            GL.BindTexture(TextureTarget.Texture2D, 0);

            this.SwapBuffers();
        }

        #region  Setting up shaders

        void CreateShaders(string vs, string fs,
            out int vertexObject, out int fragmentObject,
            out int program)
        {
            int statusCode;
            string info;

            vertexObject = GL.CreateShader(ShaderType.VertexShader);
            fragmentObject = GL.CreateShader(ShaderType.FragmentShader);

            // Compile vertex shader
            GL.ShaderSource(vertexObject, vs);
            GL.CompileShader(vertexObject);
            GL.GetShaderInfoLog(vertexObject, out info);
            GL.GetShader(vertexObject, ShaderParameter.CompileStatus, out statusCode);

            if (statusCode != 1)
                throw new ApplicationException(info);

            // Compile fragment shader
            GL.ShaderSource(fragmentObject, fs);
            GL.CompileShader(fragmentObject);
            GL.GetShaderInfoLog(fragmentObject, out info);
            GL.GetShader(fragmentObject, ShaderParameter.CompileStatus, out statusCode);

            if (statusCode != 1)
                throw new ApplicationException(info);

            program = GL.CreateProgram();
            GL.AttachShader(program, fragmentObject);
            GL.AttachShader(program, vertexObject);

            GL.LinkProgram(program);
            //GL.UseProgram(program);
        }

        #endregion

        public static void RunExample()
        {
            using (GLGrabber example = new GLGrabber())
            {
                example.Run(30.0, 0.0);
            }
        }
    }
}