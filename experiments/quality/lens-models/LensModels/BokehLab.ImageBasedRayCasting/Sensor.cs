namespace BokehLab.ImageBasedRayCasting
{
    using System.Drawing;
    using OpenTK;

    // Normalized senzor space:
    // 2D space [0; 1] x [0; AspectRatio]. X goes right, Y goes up.
    // When senzor is not tilted or shifted in any way it matches with camera
    // space XY plane. Thus we are looking at the senzor in the -Z direction
    // (towards lens and scene) and camera space Z axis extends in front of
    // the senzor (toward the viewer).

    class Sensor
    {
        Size rasterSize { get; set; }
        public Size RasterSize
        {
            get { return rasterSize; }
            set
            {
                rasterSize = value; UpdateSenzorToCamera();
            }
        }

        private double width;
        /// <summary>
        /// Senzor width in camera space. Its height is determined by the
        /// aspect ratio.
        /// </summary>
        public double Width
        {
            get { return width; }
            set { width = value; UpdateSenzorToCamera(); }
        }

        /// <summary>
        /// Senzor height in camera space. It is determined by the
        /// senzor width and aspect ratio.
        /// </summary>
        public double Height { get { return Width * AspectRatio; } }

        /// <summary>
        /// The ratio of height/width.
        /// </summary>
        public double AspectRatio
        {
            get
            {
                return (double)RasterSize.Height / (double)RasterSize.Width;
            }
        }

        private Vector3d shift;
        /// <summary>
        /// Senzor shift: (shiftX, shiftY, senzor Z distance).
        /// </summary>
        public Vector3d Shift
        {
            get { return shift; }
            set { shift = value; UpdateSenzorToCamera(); }
        }

        private Vector3d tilt;
        /// <summary>
        /// Senzor tilt around X, X and Z axes (in this order).
        /// </summary>
        public Vector3d Tilt
        {
            get { return tilt; }
            set { tilt = value; UpdateSenzorToCamera(); }
        }

        private Matrix4d senzorToCamera;
        /// <summary>
        /// Transforms from normalized senzor space to camera space.
        /// Contains information on senzor position and orientation.
        /// </summary>
        public Matrix4d SenzorToCamera
        {
            get { return senzorToCamera; }
            set
            {
                senzorToCamera = value;
                CameraToSenzor = Matrix4d.Invert(senzorToCamera);
            }
        }

        /// <summary>
        /// Transforms from camera space to normalized senzor space.
        /// Inverse of SenzorToCamera.
        /// </summary>
        public Matrix4d CameraToSenzor { get; private set; }

        public Sensor()
        {
            Width = 1.0;
            RasterSize = new Size(1, 1);
            Shift = new Vector3d(0, 0, 2);
            senzorToCamera = Matrix4d.Identity;
            CameraToSenzor = Matrix4d.Identity;
            tilt = Vector3d.Zero;
        }

        private void UpdateSenzorToCamera()
        {
            // - translate by (-0.5, -aspectRatio * 0.5)
            // - scale by (width, height, 1)
            // - translate by (shiftX, shiftY, distanceZ)
            // - [tilt around X by tiltX, ...]
            var matrix = Matrix4d.CreateTranslation(-0.5, AspectRatio * -0.5, 0);
            matrix = Matrix4d.Mult(matrix, Matrix4d.Scale(Width, Width, 1));
            matrix = Matrix4d.Mult(matrix, Matrix4d.CreateTranslation(Shift));
            if (Tilt.X != 0)
            {
                matrix = Matrix4d.Mult(matrix, Matrix4d.CreateRotationX(Tilt.X));
            }
            if (Tilt.Y != 0)
            {
                matrix = Matrix4d.Mult(matrix, Matrix4d.CreateRotationY(Tilt.Y));
            }
            if (Tilt.Z != 0)
            {
                matrix = Matrix4d.Mult(matrix, Matrix4d.CreateRotationZ(Tilt.Z));
            }
            SenzorToCamera = matrix;
        }

        public Vector3d ImageToCamera(Vector2d imagePos)
        {
            Vector3d senzorPos3 = new Vector3d(ImageToSenzorNormalized(imagePos));
            return Vector3d.Transform(senzorPos3, SenzorToCamera);
        }

        public Vector2d CameraToImage(Vector3d cameraPos)
        {
            return SenzorNormalizedToImage(
                Vector3d.Transform(cameraPos, CameraToSenzor).Xy);
        }

        public Vector2d ImageToSenzorNormalized(Vector2d imagePos)
        {
            return SenzorRasterToNormalized(ImageToSenzorRaster(imagePos));
        }

        public Vector2d SenzorNormalizedToImage(Vector2d imagePos)
        {
            return SenzorRasterToImage(SenzorNormalizedToRaster(imagePos));
        }

        /// <summary>
        /// Transforms from output image raster space to senzor raster space.
        /// </summary>
        /// <param name="imagePos"></param>
        /// <returns></returns>
        public Vector2d ImageToSenzorRaster(Vector2d imagePos)
        {
            return new Vector2d(RasterSize.Width - imagePos.X, RasterSize.Height - imagePos.Y);
        }

        /// <summary>
        /// Transforms from senzor raster space to output image raster space.
        /// </summary>
        /// <param name="senzorPos"></param>
        /// <returns></returns>
        public Vector2d SenzorRasterToImage(Vector2d senzorPos)
        {
            // ImageToSenzorRaster is inverse of itself
            return ImageToSenzorRaster(senzorPos);
        }

        /// <summary>
        /// Transforms from senzor raster space to normalized senzor space.
        /// </summary>
        /// <param name="senzorPos"></param>
        /// <returns></returns>
        public Vector2d SenzorRasterToNormalized(Vector2d senzorPos)
        {
            return senzorPos / (double)RasterSize.Width;
        }

        public Vector2d SenzorNormalizedToRaster(Vector2d senzorPos)
        {
            return senzorPos * RasterSize.Width;
        }
    }
}
