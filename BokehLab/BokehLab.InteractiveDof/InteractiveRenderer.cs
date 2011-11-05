namespace BokehLab.InteractiveDof
{
    using System;
    using BokehLab.InteractiveDof.MultiViewAccum;
    using OpenTK;
    using OpenTK.Graphics.OpenGL;
    using OpenTK.Input;

    // TODO:
    // - integrate depth peeling
    // - create height field intersection routines
    // - integrate image-based ray tracing
    // - FPS counter
    // - time counter for the whole multi-view accumulation
    // - create a configuration panel to switch the methods and control parameters

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
            CheckFboExtension();

            scene = GenerateScene();

            Keyboard.KeyUp += KeyUp;
            Keyboard.KeyRepeat = true;
            Mouse.Move += MouseMove;
            Mouse.ButtonDown += MouseButtonHandler;
            Mouse.ButtonUp += MouseButtonHandler;
            Mouse.WheelChanged += MouseWheelChanged;

            navigation.Camera.Position = new Vector3(0, 0, 3);

            multiViewAccum.Initialize(Width, Height);

            OnResize(new EventArgs());
        }

        private void CheckFboExtension()
        {
            if (!GL.GetString(StringName.Extensions).Contains("EXT_framebuffer_object"))
            {
                System.Windows.Forms.MessageBox.Show(
                     "Framebuffer Objects are not supported by the GPU.",
                     "FBOs not supported",
                     System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Exclamation);
                Exit();
            }
        }

        protected override void OnUnload(EventArgs e)
        {
            multiViewAccum.Dispose();
        }

        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);

            navigation.AspectRatio = Width / (float)Height;

            GL.MatrixMode(MatrixMode.Projection);
            Matrix4 perspective = navigation.Perspective;
            GL.LoadMatrix(ref perspective);

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

            this.SwapBuffers();
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
