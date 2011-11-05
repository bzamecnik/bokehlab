namespace BokehLab.RayTracing.Lens
{
    using System;
    using System.Diagnostics;
    using BokehLab.Math;
    using OpenTK;

    /// <summary>
    /// Represents the thin-lens model.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Thin-lens is an approximation to more complex lenses.
    /// The lens consists of a circle of radius equal to ApertureRadius,
    /// centered at (0, 0, 0) in camera space. The optical axis is aligned
    /// with the Z axis. Object space (scene) is within the negative Z
    /// halfspace, senzor (image) space is within the positive Z halfspace.
    /// </para>
    /// <para>
    /// Thin-lens transforms points with a projective transform from one
    /// half-space into the other one. Every set of rays intersecting at a
    /// single point A is transformed into another set of rays intersecting
    /// at a single point B which is obtained from A by the lens' projective
    /// transform. In particular every set of coherent rays (meeting at
    /// infinity) is transformed into a set of rays which meet at one point
    /// on the focal plane (x, y, +/- FocalLength).
    /// </para>
    /// </remarks>
    public class ThinLens : ILens
    {
        /// <summary>
        /// Radius of the lens aperture. The value must be > 0.
        /// </summary>
        public double ApertureRadius { get; set; }

        private double focalLength;
        /// <summary>
        /// Focal length of the lens (distance from the lens plane to a focal
        /// plane). The value must be > 0.
        /// </summary>
        public double FocalLength
        {
            get { return focalLength; }
            set
            {
                focalLength = value;
                transferMatrix = new Matrix4d(
                    1, 0, 0, 0,
                    0, 1, 0, 0,
                    0, 0, 1, -1 / focalLength,
                    0, 0, 0, 1
                );
                transferMatrixInv = new Matrix4d(
                    1, 0, 0, 0,
                    0, 1, 0, 0,
                    0, 0, 1, 1 / focalLength,
                    0, 0, 0, 1
                );
            }
        }

        private Matrix4d transferMatrix;
        private Matrix4d transferMatrixInv;

        private static double DEFAULT_APERTURE_RADIUS = 1;

        private static double DEFAULT_FOCAL_LENGTH = 1;

        private static readonly double epsilon = 1e-6;

        public ThinLens()
            : this(DEFAULT_FOCAL_LENGTH, DEFAULT_APERTURE_RADIUS)
        {
        }

        public ThinLens(double focalLength, double apertureRadius)
        {
            FocalLength = focalLength;
            ApertureRadius = apertureRadius;
        }


        public Ray Transfer(Ray incomingRay)
        {
            Vector3d origin = incomingRay.Origin;
            Vector3d lensPos;
            if (origin.Z != 0)
            {
                // make sure lensPos is on the lens
                // - if it is not on the lens plane (z=0) intersect the ray with this plane
                lensPos = IntersectRayWithLensPlane(incomingRay);
            }
            else
            {
                lensPos = origin;
                origin = -incomingRay.Direction;
            }
            return Transfer(origin, lensPos);
        }

        #region ILens Members

        public Ray Transfer(Vector3d objectPos, Vector3d lensPos)
        {
            Debug.Assert(Math.Abs(lensPos.Z) < epsilon);
            if (!IsPointWithinLens(lensPos))
            {
                return null;
            }

            Vector4d transformedPos = Vector4d.Transform(
                (new Vector4d(objectPos) + Vector4d.UnitW),
                GetTransferMatrix(objectPos.Z));

            if (Math.Abs(transformedPos.W) > epsilon)
            {
                Vector4d.Divide(ref transformedPos, transformedPos.W, out transformedPos);
            }
            Vector3d outputDirection = transformedPos.Xyz;
            if (Math.Abs(transformedPos.W) > epsilon)
            {
                outputDirection -= lensPos;
            }
            if (Math.Abs(objectPos.Z) <= FocalLength)
            {
                // the image of the incoming ray origin was in the same
                // half-plane as the origin itself
                outputDirection = -outputDirection;
            }
            return new Ray(lensPos, outputDirection);
        }

        public Vector3d GetBackSurfaceSample(Vector2d sample)
        {
            //var sample2d = Sampler.UniformSampleDisk(sample);
            var sample2d = Sampler.ConcentricSampleDisk(sample);
            return new Vector3d(ApertureRadius * sample2d);
        }

        public Vector3d GetFrontSurfaceSample(Vector2d sample)
        {
            // the surfaces are the same
            return GetBackSurfaceSample(sample);
        }

        #endregion

        private Matrix4d GetTransferMatrix(double z)
        {
            if (z > 0) return transferMatrix;
            else return transferMatrixInv;
        }

        private bool IsPointWithinLens(Vector3d point)
        {
            return point.X * point.X + point.Y * point.Y < ApertureRadius * ApertureRadius;
        }

        private Vector3d IntersectRayWithLensPlane(Ray incomingRay)
        {
            Vector3d origin = incomingRay.Origin;
            return new Vector3d(origin.X - origin.Z, origin.Y - origin.Z, 0);
        }

        // TODO: compute circle of confusion
    }
}
