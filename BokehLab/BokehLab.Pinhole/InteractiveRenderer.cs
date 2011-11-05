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

    public class InteractiveRenderer : GameWindow
    {
        public InteractiveRenderer()
            : base(800, 600)
        {
        }

        Navigation navigation = new Navigation() { Camera = new Camera() };

        Scene scene;

        bool mouseButtonLeftPressed = false;
        bool mouseButtonRightPressed = false;

        bool enableDofAccumulation = false;

        MultiViewAccumulation multiViewAccum = new MultiViewAccumulation();

        public static void RunExample()
        {

            using (InteractiveRenderer example = new InteractiveRenderer())
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

            navigation.Camera.Position = new Vector3(0, 0, 3);

            OnResize(new EventArgs());

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

            navigation.AspectRatio = Width / (float)Height;

            GL.MatrixMode(MatrixMode.Projection);
            Matrix4 perspective = navigation.Perspective;
            GL.LoadMatrix(ref perspective);

            DrawScene();

            base.OnResize(e);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            if (Keyboard[Key.Escape])
            {
                this.Exit();
                return;
            }

            navigation.OnUpdateFrame(e, Keyboard);
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
            Matrix4 modelView = navigation.Camera.ModelView;
            GL.LoadMatrix(ref modelView);


            if (enableDofAccumulation)
            {
                multiViewAccum.AccumulateAndDraw(scene, navigation);
            }
            else
            {
                scene.Draw();
            }
        }

        #endregion

        #region User interaction

        private void KeyUp(object sender, KeyboardKeyEventArgs e)
        {
            if (e.Key == Key.G)
            {
                // recompute geometry and redraw layers
                scene = GenerateScene();
                navigation.IsViewDirty = true;
                DrawScene();
            }
            else
                if (e.Key == Key.F11)
                {
                    bool isFullscreen = (WindowState == WindowState.Fullscreen);
                    WindowState = isFullscreen ? WindowState.Normal : WindowState.Fullscreen;
                }
                else if (e.Key == Key.F2)
                {
                    enableDofAccumulation = !enableDofAccumulation;
                }
        }

        private void MouseMove(object sender, MouseMoveEventArgs e)
        {
            Vector2 delta = new Vector2(e.XDelta / (float)Width, e.YDelta / (float)Height);
            if (mouseButtonLeftPressed)
            {
                navigation.MouseMoveUpdateView(delta);
            }
            if (mouseButtonRightPressed)
            {
                navigation.MouseMoveUpdateFocus(delta);
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
            navigation.MouseWheelUpdateFocus(e.DeltaPrecise);
        }

        private Scene GenerateScene()
        {
            return Scene.CreateRandomTriangles(10);
        }

        #endregion
    }
}
