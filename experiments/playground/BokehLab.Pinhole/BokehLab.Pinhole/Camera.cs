using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;

namespace BokehLab.Pinhole
{
    class Camera
    {
        public Matrix4 Modelview;
        public Vector3 Position { get; private set; }
        public Vector3 View { get { return Modelview.Column2.Xyz; } }
        public Vector3 Up { get { return Modelview.Column1.Xyz; } }
        public Vector3 Right { get { return Modelview.Column0.Xyz; } }

        public Camera()
        {
            Position = Vector3.Zero;
            Modelview = Matrix4.Identity;
        }

        public void Translate(Vector3 shift)
        {
            Position += shift;
            Modelview *= Matrix4.CreateTranslation(shift);
        }

        public void Roll(float angle)
        {
            Modelview *= Matrix4.CreateFromAxisAngle(View, -angle);
        }

        public void Yaw(float angle)
        {
            Modelview *= Matrix4.CreateFromAxisAngle(Up, angle);
        }

        public void Pitch(float angle)
        {
            Modelview *= Matrix4.CreateFromAxisAngle(Right, angle);
        }
    }
}
