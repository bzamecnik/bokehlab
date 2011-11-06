namespace BokehLab.InteractiveDof
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using OpenTK;
    using OpenTK.Graphics.OpenGL;
    using OpenTK.Input;

    /// <summary>
    /// Enables interaction with the extrinsic parameters of the camera model
    /// and translates them into a suitable perspective projection for
    /// rasterization.
    /// </summary>
    class Navigation
    {
        public Camera Camera { get; set; }

        // Indicates that the scene or the parameters of the view changed.
        // If there is any incremental rendering it should be refreshed.
        public bool IsViewDirty { get; set; }

        static readonly float DeltaShift = 0.1f;

        float near = 0.1f;
        public float Near
        {
            get { return near; }
            set
            {
                near = value;
                Perspective = GetPerspective();
            }
        }

        float far = 1000f;
        public float Far
        {
            get { return far; }
            set
            {
                far = value;
                Perspective = GetPerspective();
            }
        }

        public Matrix4 Perspective { get; private set; }

        public Navigation()
        {
            Reset();
        }

        public void Reset()
        {
            Camera = new Camera();
            Camera.Position = new Vector3(0, 0, 3);
            Perspective = GetPerspective();
        }

        public void OnUpdateFrame(FrameEventArgs e, KeyboardDevice Keyboard)
        {
            if (Keyboard[Key.W])
            {
                Camera.Position += DeltaShift * Camera.View;
                IsViewDirty = true;
            }
            else if (Keyboard[Key.S])
            {
                Camera.Position -= DeltaShift * Camera.View;
                IsViewDirty = true;
            }

            if (Keyboard[Key.D])
            {
                Camera.Position -= DeltaShift * Camera.Right;
                IsViewDirty = true;
            }
            else if (Keyboard[Key.A])
            {
                Camera.Position += DeltaShift * Camera.Right;
                IsViewDirty = true;
            }

            if (Keyboard[Key.E])
            {
                Camera.Position -= DeltaShift * Camera.Up;
                IsViewDirty = true;
            }
            else if (Keyboard[Key.Q])
            {
                Camera.Position += DeltaShift * Camera.Up;
                IsViewDirty = true;
            }

            bool perspectiveChanged = false;
            if (Keyboard[Key.Insert])
            {
                Camera.FieldOfView /= 1.1f;
                perspectiveChanged = true;
            }
            else if (Keyboard[Key.Delete])
            {
                Camera.FieldOfView *= 1.1f;
                perspectiveChanged = true;
            }

            if (Keyboard[Key.R])
            {
                // reset Camera configuration
                Reset();
                IsViewDirty = true;
                perspectiveChanged = true;
            }

            float viewRotDelta = 0.025f;
            if (Keyboard[Key.Up])
            {
                RotateViewVertical(-viewRotDelta);
            }
            else if (Keyboard[Key.Down])
            {
                RotateViewVertical(viewRotDelta);
            }
            if (Keyboard[Key.Right])
            {
                RotateViewHorizontal(viewRotDelta);
            }
            else if (Keyboard[Key.Left])
            {
                RotateViewHorizontal(-viewRotDelta);
            }

            if (Keyboard[Key.Home])
            {
                Camera.Lens.ApertureNumber *= 1.1f;
                perspectiveChanged = true;
            }
            else if (Keyboard[Key.End])
            {
                Camera.Lens.ApertureNumber /= 1.1f;
                perspectiveChanged = true;
            }
            if (Keyboard[Key.PageUp])
            {
                Camera.FocalZ = Camera.FocalZ * 1.1f;
                perspectiveChanged = true;
            }
            else if (Keyboard[Key.PageDown])
            {
                Camera.FocalZ = Camera.FocalZ / 1.1f;
                perspectiveChanged = true;
            }
            if (perspectiveChanged)
            {
                Perspective = GetPerspective();
                Matrix4 perspective = Perspective;

                GL.MatrixMode(MatrixMode.Projection);
                GL.LoadMatrix(ref perspective);
                IsViewDirty = true;
            }
        }

        public void MouseMoveUpdateView(Vector2 delta)
        {
            if (delta.X != 0)
            {
                RotateViewHorizontal(delta.X);
            }

            if (delta.Y != 0)
            {
                RotateViewVertical(delta.Y);
            }
        }

        private void RotateViewVertical(float delta)
        {
            float angle = 2 * Camera.FieldOfView * delta;
            Matrix4 rot = Matrix4.CreateFromAxisAngle(Camera.Right, angle);
            Camera.View = Vector3.TransformVector(Camera.View, rot);
            Camera.Up = Vector3.TransformVector(Camera.Up, rot);
            IsViewDirty = true;
        }

        private void RotateViewHorizontal(float delta)
        {
            float angle = 2 * Camera.FieldOfView * delta;
            Matrix4 rot = Matrix4.CreateRotationY(angle);
            Camera.View = Vector3.TransformVector(Camera.View, rot);
            Camera.Up = Vector3.TransformVector(Camera.Up, rot);
            IsViewDirty = true;
        }

        public void MouseMoveUpdateFocus(Vector2 delta)
        {
            if (delta.Y != 0)
            {
                Camera.FocalZ += delta.Y;
                IsViewDirty = true;
            }
        }

        public void MouseWheelUpdateFocus(float delta)
        {
            if (delta != 0)
            {
                Camera.FocalZ *= (float)Math.Pow(1.1, delta);
                IsViewDirty = true;
            }
        }

        public void UpdatePerspective()
        {
            Perspective = GetPerspective();
        }

        private Matrix4 GetPerspective()
        {
            return Matrix4.CreatePerspectiveFieldOfView(Camera.FieldOfView, Camera.AspectRatio, near, far);
        }

        public Vector2 GetPinholePos(Vector2 lensSample)
        {
            //pinholePos = apertureRadius * (Vector2)Sampler.ConcentricSampleDisk(sampler.GenerateUniformPoint());
            return Camera.Lens.ApertureRadius * lensSample;
        }

        public Matrix4 GetMultiViewPerspective(Vector2 pinholePos)
        {
            return CreatePerspectiveFieldOfViewOffCenter(Camera.FieldOfView, Camera.AspectRatio, near, far, pinholePos, -Camera.FocalZ);
        }

        private Matrix4 CreatePerspectiveFieldOfViewOffCenter(
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

            float mag = -zNear / zFocal;
            float right = xMax + lensShift.X * mag;
            float left = xMin + lensShift.X * mag;
            float top = yMax + lensShift.Y * mag;
            float bottom = yMin + lensShift.Y * mag;

            return Matrix4.CreatePerspectiveOffCenter(left, right, bottom, top, zNear, zFar);
        }
    }
}
