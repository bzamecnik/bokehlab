namespace BokehLab.InteractiveDof
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using OpenTK;
    using OpenTK.Input;

    class Navigation
    {
        public Camera Camera { get; set; }

        // Indicates that the scene or the parameters of the view changed.
        // If there is any incremental rendering it should be refreshed.
        public bool IsViewDirty { get; set; }

        static readonly float DeltaShift = 0.1f;

        static readonly float DefaultFieldOfView = OpenTK.MathHelper.PiOver4;
        float fieldOfView = DefaultFieldOfView;
        public float FieldOfView
        {
            get { return fieldOfView; }
            set
            {
                fieldOfView = value;
                if (fieldOfView > (OpenTK.MathHelper.Pi - 0.1f))
                {
                    fieldOfView = OpenTK.MathHelper.Pi - 0.1f;
                }
                else if (fieldOfView < 0.0000001f)
                {
                    fieldOfView = 0.0000001f;
                }
            }
        }
        float near = 0.1f;
        float far = 1000f;

        public float Near
        {
            get { return near; }
            set
            {
                near = value;
                Perspective = GetPerspective();
            }
        }
        public float Far
        {
            get { return far; }
            set
            {
                far = value;
                Perspective = GetPerspective();
            }
        }

        public float aspectRatio = 1.0f;
        public float AspectRatio
        {
            get { return aspectRatio; }
            set
            {
                aspectRatio = value;
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
            FieldOfView = OpenTK.MathHelper.PiOver4;
            Camera = new Camera();
            Camera.Position = new Vector3(0, 0, 3);
            Camera.PinholePos = Vector2.Zero;
            Camera.ZFocal = 5;
            Camera.ApertureRadius = 0.01f;
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
                fieldOfView /= 1.1f;
                perspectiveChanged = true;
            }
            else if (Keyboard[Key.Delete])
            {
                fieldOfView *= 1.1f;
                perspectiveChanged = true;
            }
            if (Keyboard[Key.T])
            {
                Camera.PinholePos += new Vector2(0, 0.1f);
                perspectiveChanged = true;
            }
            else if (Keyboard[Key.G])
            {
                Camera.PinholePos += new Vector2(0, -0.1f);
                perspectiveChanged = true;
            }
            else if (Keyboard[Key.H])
            {
                Camera.PinholePos += new Vector2(0.1f, 0);
                perspectiveChanged = true;
            }
            else if (Keyboard[Key.F])
            {
                Camera.PinholePos += new Vector2(-0.1f, 0);
                perspectiveChanged = true;
            }
            if (Keyboard[Key.R])
            {
                // reset Camera configuration
                Reset();
                IsViewDirty = true;
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
                Camera.ApertureRadius *= 1.1f;
                perspectiveChanged = true;
            }
            else if (Keyboard[Key.End])
            {
                Camera.ApertureRadius /= 1.1f;
                perspectiveChanged = true;
            }
            if (Keyboard[Key.PageUp])
            {
                Camera.ZFocal *= 1.1f;
                perspectiveChanged = true;
            }
            else if (Keyboard[Key.PageDown])
            {
                Camera.ZFocal /= 1.1f;
                perspectiveChanged = true;
            }
            if (perspectiveChanged)
            {
                Perspective = GetPerspective();
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
            float angle = 2 * fieldOfView * delta;
            Matrix4 rot = Matrix4.CreateFromAxisAngle(Camera.Right, angle);
            Camera.View = Vector3.TransformVector(Camera.View, rot);
            Camera.Up = Vector3.TransformVector(Camera.Up, rot);
            IsViewDirty = true;
        }

        private void RotateViewHorizontal(float delta)
        {
            float angle = 2 * fieldOfView * delta;
            Matrix4 rot = Matrix4.CreateRotationY(angle);
            Camera.View = Vector3.TransformVector(Camera.View, rot);
            Camera.Up = Vector3.TransformVector(Camera.Up, rot);
            IsViewDirty = true;
        }

        public void MouseMoveUpdateFocus(Vector2 delta)
        {
            if (delta.Y != 0)
            {
                Camera.ZFocal += delta.Y;
                IsViewDirty = true;
            }
        }

        public void MouseWheelUpdateFocus(float delta)
        {
            if (delta != 0)
            {
                Camera.ZFocal *= (float)Math.Pow(1.1, delta);
                IsViewDirty = true;
            }
        }

        public Matrix4 GetPerspective()
        {
            return Matrix4.CreatePerspectiveFieldOfView(FieldOfView, AspectRatio, near, far);
        }

        public Vector2 GetPinholePos(Vector2 lensSample)
        {
            //pinholePos = apertureRadius * (Vector2)Sampler.ConcentricSampleDisk(sampler.GenerateUniformPoint());
            return Camera.ApertureRadius * lensSample;
        }

        public Matrix4 GetMultiViewPerspective(Vector2 pinholePos)
        {
            return CreatePerspectiveFieldOfViewOffCenter(FieldOfView, AspectRatio, near, far, pinholePos, Camera.ZFocal);
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
