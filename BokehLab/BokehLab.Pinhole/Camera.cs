using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;

namespace BokehLab.Pinhole
{
    class Camera
    {
        public float ZFocal = 5; // focal plane depth
        public float ApertureRadius = 0.05f;
        public Vector2 PinholePos { get; set; }

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

        public Camera()
        {
            Position = Vector3.Zero;
            View = -Vector3.UnitZ;
            Up = Vector3.UnitY;
            Right = Vector3.UnitX;

            modelView = ComputeModelView();
        }

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
    }
}
