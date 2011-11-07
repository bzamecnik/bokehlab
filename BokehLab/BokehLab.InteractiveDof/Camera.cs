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
    /// <remarks>
    /// We are assuming the thin lens model with untilted sensor with an
    /// aperture at the lens principal plane.
    /// </remarks>
    class Camera
    {
        private float focalZ;
        /// <summary>
        /// Depth (signed Z coordinate) of the focal plane (sensor center
        /// image) in the camera space.
        /// </summary>
        /// <remarks>
        /// It should lie in the -z half-space.
        /// </remarks>
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
        /// <summary>
        /// Depth (signed Z coordinate) of the sensor center in camera space.
        /// </summary>
        /// <remarks>
        /// It should lie in the +z half-space.
        /// </remarks>
        public float SensorZ
        {
            get { return sensorZ; }
            set
            {
                sensorZ = value;
                focalZ = Lens.Transform(new Vector3(0, 0, value)).Z;
            }
        }

        // vertiacal angle of view 27 degrees for 50mm lens on full frame film (36x24mm)
        public static readonly float DefaultFieldOfView = 0.471238f;

        float fieldOfView = DefaultFieldOfView;
        /// <summary>
        /// Field of view of the camera.
        /// </summary>
        /// <remarks>
        /// Relates the sensor depth and size. Assuming the aperture coincides
        /// with the lens principal plane this is ok.
        /// </remarks>
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
        /// <summary>
        /// Sensor aspect ratio (width/height).
        /// </summary>
        public float AspectRatio
        {
            get { return aspectRatio; }
            set
            {
                aspectRatio = value;
            }
        }

        /// <summary>
        /// Sensor size in camera space.
        /// </summary>
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


        float near = 0.25f;
        /// <summary>
        /// Unsigned near plane distance. The plane lies on -Near.
        /// </summary>
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
        /// <summary>
        /// Unsigned far plane distance. The plane lies on -Far.
        /// </summary>
        public float Far
        {
            get { return far; }
            set
            {
                far = value;
                Perspective = GetPerspective();
            }
        }

        /// <summary>
        /// Perspective matrix of the center of the aperture. Indended to be
        /// used by OpenGL for rasterization.
        /// </summary>
        public Matrix4 Perspective { get; private set; }

        /// <summary>
        /// Frustum bounds (right, left, top,bottom).
        /// </summary>
        public Vector4 FrustumBounds { get; set; }

        public Camera()
        {
            Lens = new ThinLens() { ApertureNumber = 1.4f, FocalLength = 0.05f };
            //FocalZ = -(20 * Lens.FocalLength);
            FocalZ = -4f;
            UpdatePerspective();
        }

        public void UpdatePerspective()
        {
            Perspective = GetPerspective();
            float yMax = near * (float)System.Math.Tan(0.5f * fieldOfView);
            float yMin = -yMax;
            float xMin = yMin * aspectRatio;
            float xMax = yMax * aspectRatio;
            FrustumBounds = new Vector4(xMax, xMin, yMax, yMin);
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
