namespace BokehLab.InteractiveDof
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using OpenTK;

    /// <summary>
    /// Represents the intrinsic camera parameters.
    /// </summary>
    class Camera
    {
        private float focalZ;
        // focal plane depth
        public float FocalZ
        {
            get { return focalZ; }
            set
            {
                focalZ = value;
                sensorZ = Lens.Transform(new Vector3(0, 0, value)).Z;
            }
        }

        private float sensorZ;
        // sensor depth
        public float SensorZ
        {
            get { return sensorZ; }
            set
            {
                sensorZ = value;
                focalZ = Lens.Transform(new Vector3(0, 0, value)).Z;
            }
        }

        public static readonly float DefaultFieldOfView = OpenTK.MathHelper.PiOver4;

        float fieldOfView = DefaultFieldOfView;
        public float FieldOfView
        {
            get { return fieldOfView; }
            set
            {
                fieldOfView = BokehLab.Math.MathHelper.Clamp(value,
                    0.0000001f, OpenTK.MathHelper.Pi - 0.1f);
            }
        }

        public float aspectRatio = 1.0f;
        public float AspectRatio
        {
            get { return aspectRatio; }
            set
            {
                aspectRatio = value;
            }
        }

        public Vector2 SensorSize
        {
            get
            {
                float height = 2 * sensorZ * (float)System.Math.Tan(0.5f * fieldOfView);
                float width = height * aspectRatio;
                return new Vector2(width, height);
            }
        }

        public ThinLens Lens { get; private set; }


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

        public Camera()
        {
            Lens = new ThinLens() { ApertureNumber = 2.8f, FocalLength = 0.1f };
            FocalZ = -(20 * Lens.FocalLength);
        }

        public void UpdatePerspective()
        {
            Perspective = GetPerspective();
        }

        private Matrix4 GetPerspective()
        {
            return Matrix4.CreatePerspectiveFieldOfView(FieldOfView, AspectRatio, near, far);
        }

        public Vector2 GetPinholePos(Vector2 lensSample)
        {
            return Lens.ApertureRadius * lensSample;
        }

        public Matrix4 GetMultiViewPerspective(Vector2 pinholePos)
        {
            return CreatePerspectiveFieldOfViewOffCenter(FieldOfView, AspectRatio, near, far, pinholePos, -FocalZ);
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
