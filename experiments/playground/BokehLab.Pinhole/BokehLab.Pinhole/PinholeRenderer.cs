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
using OpenTK;
using OpenTK.Input;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System.Runtime.InteropServices;

namespace BokehLab.Pinhole
{
    public class PinholeRenderer : GameWindow
    {
        public PinholeRenderer()
            : base(800, 600)
        {
            scene = GenerateScene();
        }

        Matrix4 scenePerspective;
        Matrix4 sceneModelView;

        float fieldOfView = MathHelper.PiOver4;
        float near = 0.1f;
        float far = 1000f;

        Camera camera = new Camera();

        Scene scene;

        bool mouseButtonLeftPressed = false;

        public static void RunExample()
        {

            using (PinholeRenderer example = new PinholeRenderer())
            {
                example.Run(30.0, 0.0);
            }
        }

        #region GameWindow event handlers

        protected override void OnLoad(EventArgs e)
        {
            Keyboard.KeyUp += KeyUp;
            Keyboard.KeyRepeat = true;
            Mouse.Move += MouseMove;
            Mouse.ButtonDown += MouseButtonHandler;
            Mouse.ButtonUp += MouseButtonHandler;

            //GL.Enable(EnableCap.Lighting);
            //GL.Enable(EnableCap.Light0);
            //GL.Light(LightName.Light0, LightParameter.Ambient, new Color4(0.5f, 0.5f, 0.5f, 1));
            //GL.Light(LightName.Light0, LightParameter.Diffuse, new Color4(0.8f, 0.8f, 0.8f, 1));
            //GL.Light(LightName.Light0, LightParameter.Position, new Vector4(1, 5, 1, 1));
        }

        protected override void OnUnload(EventArgs e)
        {
        }

        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);

            UpdatePerspective();

            camera.Position = new Vector3(0, 0, 3);

            DrawScene();

            base.OnResize(e);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            float deltaShift = 0.1f;

            if (Keyboard[Key.Escape])
            {
                this.Exit();
                return;
            }

            if (Keyboard[Key.W])
            {
                camera.Position += deltaShift * camera.View;
            }
            else if (Keyboard[Key.S])
            {
                camera.Position -= deltaShift * camera.View;
            }

            if (Keyboard[Key.D])
            {
                camera.Position -= deltaShift * camera.Right;
            }
            else if (Keyboard[Key.A])
            {
                camera.Position += deltaShift * camera.Right;
            }

            if (Keyboard[Key.E])
            {
                camera.Position -= deltaShift * camera.Up;
            }
            else if (Keyboard[Key.Q])
            {
                camera.Position += deltaShift * camera.Up;
            }

            bool fovChanged = false;
            if (Keyboard[Key.PageUp])
            {
                fieldOfView /= 1.1f;
                fovChanged = true;
            }
            else if (Keyboard[Key.PageDown])
            {
                fieldOfView *= 1.1f;
                fovChanged = true;
            }
            if (fovChanged)
            {
                UpdatePerspective();
            }
        }

        private void UpdatePerspective()
        {
            if (fieldOfView > (MathHelper.Pi - 0.1f))
            {
                fieldOfView = MathHelper.Pi - 0.1f;
            }
            else if (fieldOfView < 0.0000001f)
            {
                fieldOfView = 0.0000001f;
            }
            float aspectRatio = Width / (float)Height;
            scenePerspective = Matrix4.CreatePerspectiveFieldOfView(fieldOfView, aspectRatio, near, far);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref scenePerspective);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            DrawScene();
            this.SwapBuffers();
        }

        #endregion

        #region Drawing the scene

        private void DrawScene()
        {
            GL.MatrixMode(MatrixMode.Modelview);
            Matrix4 modelView = camera.ModelView;
            GL.LoadMatrix(ref modelView);

            GL.PushAttrib(AttribMask.ViewportBit);
            {
                GL.Viewport(0, 0, Width, Height);

                // clear the screen in red, to make it very obvious what the clear affected. only the FBO, not the real framebuffer
                //GL.ClearColor(1f, 0f, 0f, 1f);
                GL.ClearColor(0, 0, 0, 1);
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                GL.Begin(BeginMode.Lines);

                GL.Color3(Color.Red);
                GL.Vertex3(0, 0, 0);
                GL.Vertex3(1, 0, 0);

                GL.Color3(Color.Green);
                GL.Vertex3(0, 0, 0);
                GL.Vertex3(0, 1, 0);

                GL.Color3(Color.Blue);
                GL.Vertex3(0, 0, 0);
                GL.Vertex3(0, 0, 1);

                GL.End();

                GL.Color3(Color.Gray);
                GL.Begin(BeginMode.Triangles);

                GL.Vertex3(0, 0, 0);
                GL.Vertex3(0.5, 0, 0);
                GL.Vertex3(0, 0.5, 0);


                GL.Vertex3(0, 0, 0);
                GL.Vertex3(0.5, 0, 0);
                GL.Vertex3(0, 0, 0.5);

                GL.Vertex3(0, 0, 0);
                GL.Vertex3(0, 0.5, 0);
                GL.Vertex3(0, 0, 0.5);

                GL.End();

                //scene.Draw();
            }
            GL.PopAttrib();
        }

        #endregion

        #region User interaction

        private void KeyUp(object sender, KeyboardKeyEventArgs e)
        {
            if (e.Key == Key.R)
            {
                // recompute geometry and redraw layers
                scene = GenerateScene();
                DrawScene();
            }
            else if (e.Key == Key.F11)
            {
                bool isFullscreen = (WindowState == WindowState.Fullscreen);
                WindowState = isFullscreen ? WindowState.Normal : WindowState.Fullscreen;
            }
        }

        private void MouseMove(object sender, MouseMoveEventArgs e)
        {
            if (mouseButtonLeftPressed)
            {
                if (e.XDelta != 0)
                {
                    float angle = 2 * fieldOfView * e.XDelta / (float)Width;
                    Matrix4 rot = Matrix4.CreateRotationY(angle);
                    camera.View = Vector3.TransformVector(camera.View, rot);
                    camera.Up = Vector3.TransformVector(camera.Up, rot);
                }

                if (e.YDelta != 0)
                {
                    float angle = 2 * fieldOfView * e.YDelta / (float)Height;
                    Matrix4 rot = Matrix4.CreateFromAxisAngle(camera.Right, angle);
                    camera.View = Vector3.TransformVector(camera.View, rot);
                    camera.Up = Vector3.TransformVector(camera.Up, rot);
                }
            }
        }

        private void MouseButtonHandler(object sender, MouseButtonEventArgs e)
        {
            if (e.Button == MouseButton.Left)
            {
                mouseButtonLeftPressed = e.IsPressed;
            }
        }

        private Scene GenerateScene()
        {
            return Scene.CreateRandomTriangles(10);
        }

        #endregion

    }
}