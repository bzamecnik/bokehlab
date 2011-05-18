namespace BokehLab.RayTracing.Lens
{
    using System;
    using BokehLab.Math;
    using OpenTK;

    /// <summary>
    /// A simple lens composed of two spherical surfaces.
    /// </summary>
    /// <remarks>
    /// The lens body is in fact the intersection of two spheres of the same
    /// size, only with different position.
    /// </remarks>
    public class BiconvexLens : ILens
    {
        private double apertureRadius;

        /// <summary>
        /// Radius of the circle of intersection of the two spherical
        /// surfaces.
        /// </summary>
        public double ApertureRadius
        {
            get { return apertureRadius; }
            set
            {
                apertureRadius = value;
                UpdateSpheres();
            }
        }

        private double curvatureRadius;
        /// <summary>
        /// Radius of curvature of both spherical surfaces, ie. radius of
        /// both spheres.
        /// </summary>
        public double CurvatureRadius
        {
            get { return curvatureRadius; }
            set
            {
                curvatureRadius = value;
                UpdateSpheres();
            }
        }

        private Sphere backSurface;
        private Sphere frontSurface;

        public double RefractiveIndex { get; set; }

        public BiconvexLens()
        {
            RefractiveIndex = Materials.Fixed.GLASS_CROWN_K7;
            apertureRadius = 1;
            curvatureRadius = 1;
            //backSurface = new Sphere() { Radius = 2, Center = 0.7 * -2 * Vector3d.UnitZ };
            //frontSurface = new Sphere() { Radius = 2, Center = 0.7 * 2 * Vector3d.UnitZ };
            backSurface = new Sphere();
            frontSurface = new Sphere();
            UpdateSpheres();
        }

        #region ILens Members

        public Ray Transfer(Vector3d objectPos, Vector3d lensPos)
        {
            // lensPos should be already an intersection of of the incoming
            // ray with the back surface

            // DEBUG
            //string incomingStr = new Ray(objectPos, lensPos - objectPos).ToString();

            // refract the incoming ray
            Vector3d incomingDir = lensPos - objectPos;
            incomingDir.Normalize();
            Vector3d direction = Ray.Refract(incomingDir, backSurface.GetNormal(lensPos),
                Materials.Fixed.AIR, RefractiveIndex, false);
            if (direction == Vector3d.Zero)
            {
                return null;
            }
            // intersect the ray with the front surface
            Intersection intersection = frontSurface.Intersect(new Ray(lensPos, direction));
            if (intersection == null)
            {
                return null;
            }
            // DEBUG
            //Console.WriteLine(new Ray(lensPos, intersection.Position - lensPos));
            //string innerStr = new Ray(lensPos, intersection.Position - lensPos).ToString();
            // refract the ray again
            direction = intersection.Position - lensPos;
            direction.Normalize();
            direction = Ray.Refract(-direction, -frontSurface.GetNormal(intersection.Position),
                RefractiveIndex, Materials.Fixed.AIR, false);
            if (direction == Vector3d.Zero)
            {
                return null;
            }
            direction.Normalize();
            Ray transferredRay = new Ray(intersection.Position, direction);
            //DEBUG
            //Console.WriteLine(transferredRay);
            //string transferredStr = transferredRay.ToString();
            //Console.WriteLine("Black, {0}, Green, {1}, Red, {2},", incomingStr, innerStr, transferredStr);
            return transferredRay;
        }

        public Vector3d GetBackSurfaceSample(Vector2d sample)
        {
            // TODO: implement proper sampling with respect to the cap angle
            Vector3d unitSphereSample = Sampler.UniformSampleHemisphere(sample);
            return backSurface.Center + backSurface.Radius * unitSphereSample;
        }

        public Vector3d GetFrontSurfaceSample(Vector2d sample)
        {
            // TODO: implement proper sampling
            // front surface is in the -Z hemisphere
            Vector3d unitSphereSample = Sampler.UniformSampleHemisphere(sample);
            return frontSurface.Center + frontSurface.Radius * (-unitSphereSample);
        }

        #endregion

        private void UpdateSpheres()
        {
            // backSurface.Center = ...
            // backSurface.Radius = ...
            // frontSurface.Center = ...
            // frontSurface.Radius = ...
        }
    }
}
