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

// TODO:
// - with mouse movements only control the derivative of rotation (pitch, yaw)
// - construct the model-view matrix from the camera position and pitch, yaw
// - do not modify the right vector

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
            Mouse.Move += MouseMove;
            Mouse.ButtonDown += MouseButtonHandler;
            Mouse.ButtonUp += MouseButtonHandler;
        }

        protected override void OnUnload(EventArgs e)
        {
        }

        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);

            float aspectRatio = Width / (float)Height;

            GL.MatrixMode(MatrixMode.Projection);
            scenePerspective = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, 1, 64);
            GL.LoadMatrix(ref scenePerspective);

            //GL.MatrixMode(MatrixMode.Modelview);
            //sceneModelView = Matrix4.LookAt(0, 0, 3, 0, 0, 0, 0, 1, 0);
            //GL.LoadMatrix(ref sceneModelView);
            camera.Translate(new Vector3(0, 0, 3));

            DrawScene();

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
            DrawScene();
            this.SwapBuffers();
        }

        #endregion

        #region Drawing the scene

        private void DrawScene()
        {
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref scenePerspective);

            //GL.MatrixMode(MatrixMode.Modelview);
            //GL.LoadMatrix(ref sceneModelView);

            //GL.MatrixMode(MatrixMode.Projection);
            //GL.LoadIdentity();
            //GL.Ortho(-1, 1, -1, 1, 1, -1);

            //GL.MatrixMode(MatrixMode.Modelview);
            //Matrix4 lookat = Matrix4.LookAt(0, 0, 0, 0, 0, -1, 0, 1, 0);
            //GL.LoadMatrix(ref lookat);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref camera.Modelview);

            GL.PushAttrib(AttribMask.ViewportBit);
            {
                GL.Viewport(0, 0, Width, Height);

                // clear the screen in red, to make it very obvious what the clear affected. only the FBO, not the real framebuffer
                //GL.ClearColor(1f, 0f, 0f, 1f);
                GL.ClearColor(0, 0, 0, 1);
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                scene.Draw();
            }
            GL.PopAttrib();
        }

        #endregion

        #region User interaction

        private void KeyUp(object sender, KeyboardKeyEventArgs e)
        {
            float rotationAngle = 0.2f;
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
            else if (e.Key == Key.W)
            {
                camera.Translate(Vector3.UnitZ);
            }
            else if (e.Key == Key.S)
            {
                camera.Translate(-Vector3.UnitZ);
            }
            else if (e.Key == Key.D)
            {
                camera.Translate(-Vector3.UnitX);
            }
            else if (e.Key == Key.A)
            {
                camera.Translate(Vector3.UnitX);
            }
            else if (e.Key == Key.Right)
            {
                camera.Yaw(rotationAngle);
            }
            else if (e.Key == Key.Left)
            {
                camera.Yaw(-rotationAngle);
            }
            else if (e.Key == Key.Up)
            {
                camera.Pitch(rotationAngle);
            }
            else if (e.Key == Key.Down)
            {
                camera.Pitch(-rotationAngle);
            }
            Console.WriteLine("Position: {0}", camera.Position);
            Console.WriteLine("View: {0}", camera.View);
            Console.WriteLine("Up: {0}", camera.Up);
            Console.WriteLine("Right: {0}", camera.Right);
        }

        private void MouseMove(object sender, MouseMoveEventArgs e)
        {
            if (mouseButtonLeftPressed)
            {
                if (e.XDelta != 0)
                {
                    camera.Yaw(MathHelper.Pi * e.XDelta / (float)Width);
                }
                if (e.YDelta != 0)
                {
                    camera.Pitch(MathHelper.Pi * e.YDelta / (float)Width);
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