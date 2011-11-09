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
    /// Represents the extrinsic camera parameters defining the camera space,
    /// such as camera position and orientation.
    /// </summary>
    class Navigation
    {
        public Camera Camera { get; set; }

        // Indicates that the scene or the parameters of the view changed.
        // If there is any incremental rendering it should be refreshed.
        public bool IsViewDirty { get; set; }

        static readonly float DeltaShift = 0.1f;

        #region Camera position

        private Vector3 position;

        public Vector3 Position
        {
            get { return position; }
            set { position = value; modelViewDirty = true; }
        }
        private Vector3 view;

        public Vector3 View
        {
            get { return view; }
            set { view = value; modelViewDirty = true; }
        }
        private Vector3 up;

        public Vector3 Up
        {
            get { return up; }
            set { up = value; modelViewDirty = true; }
        }
        private Vector3 right;

        public Vector3 Right
        {
            get { return right; }
            set { right = value; modelViewDirty = true; }
        }

        private Matrix4 modelView;
        public Matrix4 ModelView
        {
            get
            {
                if (modelViewDirty)
                {
                    modelView = ComputeModelView();
                    modelViewDirty = false;
                }
                return modelView;
            }
        }
        bool modelViewDirty = true;

        /// <summary>
        /// Produce a first-person shooter model view matrix.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The goal is a comfortable control witout rotation around the view vector.
        /// </para>
        /// <para>
        /// The formulas were inspired by Kevin R. Harris: http://www.codesampler.com/
        /// </para>
        /// </remarks>
        /// <returns></returns>
        private Matrix4 ComputeModelView()
        {
            view.Normalize();
            right = Vector3.Cross(view, Up);
            right.Normalize();
            up = Vector3.Cross(right, View);
            up.Normalize();

            Vector4 lastRow = new Vector4(
                Vector3.Dot(position, right),
                Vector3.Dot(position, up),
                Vector3.Dot(position, view),
                1);

            Matrix4 m = new Matrix4(
                right.X, up.X, view.X, 0,
                right.Y, up.Y, view.Y, 0,
                right.Z, up.Z, view.Z, 0,
                lastRow.X, lastRow.Y, lastRow.Z, lastRow.W);
            return m;
        }

        #endregion

        public Navigation()
        {
            Camera = new Camera();
            Reset();
        }

        public void Reset()
        {
            Position = new Vector3(0, 0, 4);
            View = -Vector3.UnitZ;
            Up = Vector3.UnitY;
            Right = Vector3.UnitX;

            modelView = ComputeModelView();

            float aspectRatio = Camera.AspectRatio;
            Camera = new Camera();
            Camera.AspectRatio = aspectRatio;
        }

        public void OnUpdateFrame(FrameEventArgs e, KeyboardDevice Keyboard)
        {
            if (Keyboard[Key.W])
            {
                Position += DeltaShift * View;
                IsViewDirty = true;
            }
            else if (Keyboard[Key.S])
            {
                Position -= DeltaShift * View;
                IsViewDirty = true;
            }

            if (Keyboard[Key.D])
            {
                Position -= DeltaShift * Right;
                IsViewDirty = true;
            }
            else if (Keyboard[Key.A])
            {
                Position += DeltaShift * Right;
                IsViewDirty = true;
            }

            if (Keyboard[Key.E])
            {
                Position -= DeltaShift * Up;
                IsViewDirty = true;
            }
            else if (Keyboard[Key.Q])
            {
                Position += DeltaShift * Up;
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
                Camera.Lens.ApertureNumber *= 1.05f;
                perspectiveChanged = true;
            }
            else if (Keyboard[Key.End])
            {
                Camera.Lens.ApertureNumber /= 1.05f;
                perspectiveChanged = true;
            }
            if (Keyboard[Key.PageUp])
            {
                Camera.FocalZ = Camera.FocalZ * 1.05f;
                perspectiveChanged = true;
            }
            else if (Keyboard[Key.PageDown])
            {
                Camera.FocalZ = Camera.FocalZ / 1.05f;
                perspectiveChanged = true;
            }

            if (Keyboard[Key.I])
            {
                float shiftY = BokehLab.Math.MathHelper.Clamp(Camera.Shift.Y + 0.01f, -Camera.Lens.ApertureRadius, Camera.Lens.ApertureRadius);
                Camera.Shift = new Vector2(Camera.Shift.X, shiftY);
                IsViewDirty = true;
            }
            else if (Keyboard[Key.K])
            {
                float shiftY = BokehLab.Math.MathHelper.Clamp(Camera.Shift.Y - 0.01f, -Camera.Lens.ApertureRadius, Camera.Lens.ApertureRadius);
                Camera.Shift = new Vector2(Camera.Shift.X, shiftY);
                IsViewDirty = true;
            }
            else if (Keyboard[Key.L])
            {
                float shiftX = BokehLab.Math.MathHelper.Clamp(Camera.Shift.X + 0.01f, -Camera.Lens.ApertureRadius, Camera.Lens.ApertureRadius);
                Camera.Shift = new Vector2(shiftX, Camera.Shift.Y);
                IsViewDirty = true;
            }
            else if (Keyboard[Key.J])
            {
                float shiftX = BokehLab.Math.MathHelper.Clamp(Camera.Shift.X - 0.01f, -Camera.Lens.ApertureRadius, Camera.Lens.ApertureRadius);
                Camera.Shift = new Vector2(shiftX, Camera.Shift.Y);
                IsViewDirty = true;
            }

            if (perspectiveChanged)
            {
                Camera.UpdatePerspective();
                Matrix4 perspective = Camera.Perspective;

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
            Matrix4 rot = Matrix4.CreateFromAxisAngle(Right, angle);
            View = Vector3.TransformVector(View, rot);
            Up = Vector3.TransformVector(Up, rot);
            IsViewDirty = true;
        }

        private void RotateViewHorizontal(float delta)
        {
            float angle = 2 * Camera.FieldOfView * delta;
            Matrix4 rot = Matrix4.CreateRotationY(angle);
            View = Vector3.TransformVector(View, rot);
            Up = Vector3.TransformVector(Up, rot);
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
    }
}
