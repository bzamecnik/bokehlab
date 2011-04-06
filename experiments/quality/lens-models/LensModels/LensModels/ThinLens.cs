using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using OpenTK;

namespace BokehLab.Lens
{
    public class ThinLens
    {
        /// <summary>
        /// ApertureRadius must be > 0
        /// </summary>
        public float ApertureRadius { get; private set; }

        /// <summary>
        /// FocalLength must be > 0
        /// </summary>
        public float FocalLength { get; private set; }

        private Matrix4d transferMatrix;
        private Matrix4d transferMatrixInv;

        private static float DEFAULT_APERTURE_RADIUS = 1;

        private static float DEFAULT_FOCAL_LENGTH = 1;

        public ThinLens()
            : this(DEFAULT_FOCAL_LENGTH, DEFAULT_APERTURE_RADIUS)
        {
        }

        public ThinLens(float focalLength, float apertureRadius)
        {
            FocalLength = focalLength;
            ApertureRadius = apertureRadius;
            transferMatrix = new Matrix4d(
                1, 0, 0, 0,
                0, 1, 0, 0,
                0, 0, 1, -1 / FocalLength,
                0, 0, 0, 1
                );
            transferMatrixInv = new Matrix4d(
                1, 0, 0, 0,
                0, 1, 0, 0,
                0, 0, 1, 1 / FocalLength,
                0, 0, 0, 1
                );
        }

        private Matrix4d GetTransferMatrix(double z)
        {
            if (z > 0) return transferMatrix;
            else return transferMatrixInv;
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
            } else {
                lensPos = origin;
                origin = -incomingRay.Direction;
            }
            return Transfer(origin, lensPos);
        }

        public Ray Transfer(Vector3d objectPos, Vector3d lensPos)
        {
            Debug.Assert(Math.Abs(lensPos.Z) < double.Epsilon);
            if (!IsPointWithinLens(lensPos))
            {
                return null;
            }

            Vector4d transformedPos = Vector4d.Transform(
                (new Vector4d(objectPos) + Vector4d.UnitW),
                GetTransferMatrix(objectPos.Z));

            if (Math.Abs(transformedPos.W) > double.Epsilon)
            {
                Vector4d.Divide(ref transformedPos, transformedPos.W, out transformedPos);
            }
            Vector3d outputDirection = transformedPos.Xyz;
            if (Math.Abs(transformedPos.W) > double.Epsilon)
            {
                outputDirection -= lensPos;
            }
            if (Math.Abs(objectPos.Z) < FocalLength)
            {
                // the image of the incoming ray origin was in the same
                // half-plane as the origin itself
                outputDirection = -outputDirection;
            }
            return new Ray(lensPos, outputDirection);
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
