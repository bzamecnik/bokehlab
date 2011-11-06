namespace BokehLab.InteractiveDof
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using BokehLab.InteractiveDof.DepthPeeling;
    using BokehLab.InteractiveDof.MultiViewAccum;
    using BokehLab.InteractiveDof.RayTracing;
    using OpenTK;
    using OpenTK.Graphics.OpenGL;
    using OpenTK.Input;

    // TODO:
    // - create height field intersection routines
    // - integrate image-based ray tracing
    // - FPS counter
    // - time counter for the whole multi-view accumulation
    // - create a configuration panel to switch the methods and control parameters
    // - umbra depth peeling, extended-umbra depth peeling

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

        Mode renderingMode = Mode.Pinhole;

        MultiViewAccumulation multiViewAccum = new MultiViewAccumulation();
        DepthPeeler depthPeeler = new DepthPeeler();
        ImageBasedRayTracer ibrt = new ImageBasedRayTracer();

        List<IRendererModule> modules = new List<IRendererModule>();

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

            modules.Add(multiViewAccum);
            modules.Add(depthPeeler);
            modules.Add(ibrt);
            foreach (var module in modules)
            {
                module.Initialize(Width, Height);
            }
            ibrt.DepthPeeler = depthPeeler;

            GL.Enable(EnableCap.DepthTest);
            GL.ClearDepth(1.0f);

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
            foreach (var module in modules)
            {
                module.Dispose();
            }
        }

        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);

            navigation.Camera.AspectRatio = Width / (float)Height;
            navigation.UpdatePerspective();

            GL.MatrixMode(MatrixMode.Projection);
            Matrix4 perspective = navigation.Perspective;
            GL.LoadMatrix(ref perspective);
            GL.MatrixMode(MatrixMode.Modelview);

            foreach (var module in modules)
            {
                module.Resize(Width, Height);
            }

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
            this.Title = "BokehLab - FPS: " + (1f / e.Time).ToString("0.0");

            GL.MatrixMode(MatrixMode.Modelview);
            Matrix4 modelView = navigation.Camera.ModelView;
            GL.LoadMatrix(ref modelView);

            switch (renderingMode)
            {
                case Mode.Pinhole:
                    scene.Draw();
                    break;
                case Mode.MultiViewAccum:
                    multiViewAccum.AccumulateAndDraw(scene, navigation);
                    break;
                case Mode.OrderIndependentTransparency:
                    depthPeeler.PeelLayers(scene);
                    depthPeeler.DisplayLayers();
                    break;
                case Mode.ImageBasedRayTracing:
                    depthPeeler.PeelLayers(scene);
                    ibrt.DrawIbrtImage();
                    break;
                default:
                    Debug.Assert(false, "Unknown rendering mode");
                    break;
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
                    if (renderingMode != Mode.Pinhole)
                    {
                        foreach (var module in modules)
                        {
                            module.Enabled = false;
                        }
                        renderingMode = Mode.Pinhole;
                        navigation.IsViewDirty = true;
                    }
                }
                else if (e.Key == Key.F3)
                {
                    if (renderingMode != Mode.MultiViewAccum)
                    {
                        foreach (var module in modules)
                        {
                            module.Enabled = false;
                        }
                        multiViewAccum.Enabled = true;
                        renderingMode = Mode.MultiViewAccum;
                        navigation.IsViewDirty = true;
                    }
                }
                else if (e.Key == Key.F5)
                {
                    if (renderingMode != Mode.ImageBasedRayTracing)
                    {
                        foreach (var module in modules)
                        {
                            module.Enabled = false;
                        }
                        depthPeeler.Enabled = true;
                        ibrt.Enabled = true;
                        renderingMode = Mode.ImageBasedRayTracing;
                        navigation.IsViewDirty = true;
                    }
                }
                else if (e.Key == Key.F4)
                {
                    if (renderingMode != Mode.OrderIndependentTransparency)
                    {
                        foreach (var module in modules)
                        {
                            module.Enabled = false;
                        }
                        depthPeeler.Enabled = true;
                        renderingMode = Mode.OrderIndependentTransparency;
                        navigation.IsViewDirty = true;
                    }
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

        enum Mode
        {
            Pinhole,
            MultiViewAccum,
            ImageBasedRayTracing,
            OrderIndependentTransparency,
        }
    }
}
