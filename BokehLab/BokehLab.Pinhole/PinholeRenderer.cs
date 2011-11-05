#region --- License ---
/* Licensed under the MIT/X11 license.
 * Copyright (c) 2006-2008 the OpenTK Team.
 * This notice may not be removed from any source distribution.
 * See license.txt for licensing details.
 */
#endregion

namespace BokehLab.Pinhole
{
    using System;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using OpenTK;
    using OpenTK.Input;
    using OpenTK.Graphics;
    using OpenTK.Graphics.OpenGL;
    using System.Runtime.InteropServices;
    using BokehLab.Math;
    using System.Collections.Generic;
    using System.Linq;

    public class PinholeRenderer : GameWindow
    {
        public PinholeRenderer()
            : base(800, 600)
        {
        }

        Matrix4 scenePerspective;
        Matrix4 sceneModelView;

        float fieldOfView = OpenTK.MathHelper.PiOver4;
        float near = 0.1f;
        float far = 1000f;

        float apertureRadius = 0.05f;
        Vector2 pinholePos = Vector2.Zero; // position of the pinhole on the lens aperture
        float zFocal = 5; // focal plane depth

        Camera camera = new Camera();

        Scene scene;

        Vector2[] unitDiskSamples;

        bool mouseButtonLeftPressed = false;
        bool mouseButtonRightPressed = false;

        bool enableDofAccumulation = false;

        // Indicates that previously accumulated DoF is obsolete,
        // in other words the scene or the parameters fo the view changed.
        bool isAccumulationObsolete = true;
        int accumIterations = 0;

        Sampler sampler = new Sampler();

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
            scene = GenerateScene();

            Keyboard.KeyUp += KeyUp;
            Keyboard.KeyRepeat = true;
            Mouse.Move += MouseMove;
            Mouse.ButtonDown += MouseButtonHandler;
            Mouse.ButtonUp += MouseButtonHandler;
            Mouse.WheelChanged += MouseWheelChanged;

            camera.Position = new Vector3(0, 0, 3);

            unitDiskSamples = CreateLensSamples(16).ToArray();

            //GL.Enable(EnableCap.Lighting);
            //GL.Enable(EnableCap.Light0);
            //GL.Light(LightName.Light0, LightParameter.Ambient, new Color4(0.5f, 0.5f, 0.5f, 1));
            //GL.Light(LightName.Light0, LightParameter.Diffuse, new Color4(0.8f, 0.8f, 0.8f, 1));
            //GL.Light(LightName.Light0, LightParameter.Position, new Vector4(1, 5, 1, 1));
        }

        /// <summary>
        /// Generate a set of jittered uniform samples of a unit circle.
        /// </summary>
        /// <returns></returns>
        private IEnumerable<Vector2> CreateLensSamples(int sampleCount)
        {
            var jitteredSamples = sampler.GenerateJitteredSamples(sampleCount);
            var diskSamples = jitteredSamples.Select((sample) => (Vector2)Sampler.ConcentricSampleDisk(sample));
            var diskSamplesList = diskSamples.ToList();
            Shuffle<Vector2>(diskSamplesList);
            return diskSamplesList;
        }

        protected override void OnUnload(EventArgs e)
        {
        }

        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);

            UpdatePerspective();

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
                isAccumulationObsolete = true;
            }
            else if (Keyboard[Key.S])
            {
                camera.Position -= deltaShift * camera.View;
                isAccumulationObsolete = true;
            }

            if (Keyboard[Key.D])
            {
                camera.Position -= deltaShift * camera.Right;
                isAccumulationObsolete = true;
            }
            else if (Keyboard[Key.A])
            {
                camera.Position += deltaShift * camera.Right;
                isAccumulationObsolete = true;
            }

            if (Keyboard[Key.E])
            {
                camera.Position -= deltaShift * camera.Up;
                isAccumulationObsolete = true;
            }
            else if (Keyboard[Key.Q])
            {
                camera.Position += deltaShift * camera.Up;
                isAccumulationObsolete = true;
            }

            bool perspectiveChanged = false;
            if (Keyboard[Key.PageUp])
            {
                fieldOfView /= 1.1f;
                perspectiveChanged = true;
            }
            else if (Keyboard[Key.PageDown])
            {
                fieldOfView *= 1.1f;
                perspectiveChanged = true;
            }
            if (Keyboard[Key.T])
            {
                pinholePos += new Vector2(0, 0.1f);
                perspectiveChanged = true;
            }
            else if (Keyboard[Key.G])
            {
                pinholePos += new Vector2(0, -0.1f);
                perspectiveChanged = true;
            }
            else if (Keyboard[Key.H])
            {
                pinholePos += new Vector2(0.1f, 0);
                perspectiveChanged = true;
            }
            else if (Keyboard[Key.F])
            {
                pinholePos += new Vector2(-0.1f, 0);
                perspectiveChanged = true;
            }
            if (Keyboard[Key.R])
            {
                // reset camera configuration
                pinholePos = Vector2.Zero;
                zFocal = 5;
                apertureRadius = 0.01f;
                fieldOfView = OpenTK.MathHelper.PiOver4;
                camera = new Camera();
                camera.Position = new Vector3(0, 0, 3);
                perspectiveChanged = true;
            }
            if (Keyboard[Key.Home])
            {
                apertureRadius *= 1.1f;
                perspectiveChanged = true;
            }
            else if (Keyboard[Key.End])
            {
                apertureRadius /= 1.1f;
                perspectiveChanged = true;
            }
            if (Keyboard[Key.Up])
            {
                zFocal *= 1.1f;
                perspectiveChanged = true;
            }
            else if (Keyboard[Key.Down])
            {
                zFocal /= 1.1f;
                perspectiveChanged = true;
            }
            if (perspectiveChanged)
            {
                UpdatePerspective();
                isAccumulationObsolete = true;
            }
        }

        private void UpdatePerspective()
        {
            if (fieldOfView > (OpenTK.MathHelper.Pi - 0.1f))
            {
                fieldOfView = OpenTK.MathHelper.Pi - 0.1f;
            }
            else if (fieldOfView < 0.0000001f)
            {
                fieldOfView = 0.0000001f;
            }
            float aspectRatio = Width / (float)Height;
            scenePerspective = CreatePerspectiveFieldOfViewOffCenter(fieldOfView, aspectRatio, near, far, pinholePos, zFocal);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref scenePerspective);
        }

        Matrix4 CreatePerspectiveFieldOfViewOffCenter(
            float fovy,
            float aspect,
            float zNear,
            float zFar,
            Vector2 lensShift,
            float zFocal)
        {
            float yMax = zNear * (float)System.Math.Tan(0.5f * fovy);
            float yMin = -yMax;
            float xMin = yMin * aspect;
            float xMax = yMax * aspect;

            float mag = -near / zFocal;
            float right = xMax + lensShift.X * mag;
            float left = xMin + lensShift.X * mag;
            float top = yMax + lensShift.Y * mag;
            float bottom = yMin + lensShift.Y * mag;

            return Matrix4.CreatePerspectiveOffCenter(left, right, bottom, top, zNear, zFar);
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

            GL.ClearColor(0, 0, 0, 1);

            GL.PushAttrib(AttribMask.ViewportBit);
            {
                GL.Viewport(0, 0, Width, Height);

                if (enableDofAccumulation)
                {
                    if (isAccumulationObsolete)
                    {
                        GL.Clear(ClearBufferMask.AccumBufferBit);
                        isAccumulationObsolete = false;
                        accumIterations = 0;
                    }
                    int maxIterations = unitDiskSamples.Length;

                    for (int i = 0; i < 16; i++)
                    {
                        if (accumIterations >= maxIterations)
                        {
                            break;
                        }
                        //int maxIterations = 16;
                        //float iterationsInv = 1f / maxIterations;
                        float iterationsInv = 1f / accumIterations;
                        //for (int i = 0; i < iterations; i++)
                        //{
                        GL.PushMatrix();
                        //pinholePos = apertureRadius * (Vector2)Sampler.ConcentricSampleDisk(sampler.GenerateUniformPoint());
                        pinholePos = apertureRadius * unitDiskSamples[accumIterations];
                        UpdatePerspective();
                        GL.Translate(-pinholePos.X, -pinholePos.Y, 0);

                        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
                        scene.Draw();

                        GL.PopMatrix();

                        //GL.Accum(AccumOp.Accum, 1f / maxIterations);
                        GL.Accum(AccumOp.Mult, 1 - 1f / (accumIterations + 1));
                        GL.Accum(AccumOp.Accum, 1f / (accumIterations + 1));
                        accumIterations++;
                    }

                    //GL.Accum(AccumOp.Return, maxIterations / (float)accumIterations);
                    GL.Accum(AccumOp.Return, 1f);
                }
                else
                {
                    GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
                    scene.Draw();
                }
            }
            GL.PopAttrib();
        }

        #endregion

        #region User interaction

        private void KeyUp(object sender, KeyboardKeyEventArgs e)
        {
            //if (e.Key == Key.R)
            //{
            //    // recompute geometry and redraw layers
            //    scene = GenerateScene();
            //    DrawScene();
            //}
            //else
            if (e.Key == Key.F11)
            {
                bool isFullscreen = (WindowState == WindowState.Fullscreen);
                WindowState = isFullscreen ? WindowState.Normal : WindowState.Fullscreen;
            }
            else if (e.Key == Key.F2)
            {
                enableDofAccumulation = !enableDofAccumulation;
                pinholePos = Vector2.Zero;
                UpdatePerspective();
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
                    isAccumulationObsolete = true;
                }

                if (e.YDelta != 0)
                {
                    float angle = 2 * fieldOfView * e.YDelta / (float)Height;
                    Matrix4 rot = Matrix4.CreateFromAxisAngle(camera.Right, angle);
                    camera.View = Vector3.TransformVector(camera.View, rot);
                    camera.Up = Vector3.TransformVector(camera.Up, rot);
                    isAccumulationObsolete = true;
                }
            }
            if (mouseButtonRightPressed)
            {
                if (e.YDelta != 0)
                {
                    zFocal += (e.YDelta / (float)Height);
                    isAccumulationObsolete = true;
                }
            }
        }

        private void MouseButtonHandler(object sender, MouseButtonEventArgs e)
        {
            if (e.Button == MouseButton.Left)
            {
                mouseButtonLeftPressed = e.IsPressed;
            }
            if (e.Button == MouseButton.Right)
            {
                mouseButtonRightPressed = e.IsPressed;
            }
        }

        private void MouseWheelChanged(object sender, MouseWheelEventArgs e)
        {
            zFocal *= (float)Math.Pow(1.1, e.DeltaPrecise);
            isAccumulationObsolete = true;
        }

        private Scene GenerateScene()
        {
            return Scene.CreateRandomTriangles(10);
        }

        #endregion

        //http://stackoverflow.com/questions/273313/randomize-a-listt-in-c-sharp
        public static void Shuffle<T>(IList<T> list)
        {
            Random rng = new Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

    }
}
