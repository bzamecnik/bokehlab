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
    public class BiconvexLens : ILens, IIntersectable
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

        private double sinTheta;

        private Sphere backSurface;
        private Sphere frontSurface;

        public double RefractiveIndex { get; set; }

        public BiconvexLens()
        {
            RefractiveIndex = Materials.Fixed.GLASS_CROWN_K7;
            apertureRadius = 2;
            curvatureRadius = 2.5;
            backSurface = new Sphere();
            frontSurface = new Sphere();
            UpdateSpheres();
        }

        #region ILens Members

        public Ray Transfer(Vector3d objectPos, Vector3d lensPos)
        {
            // lensPos should be already an intersection of of the incoming
            // ray with the back surface

            if (lensPos.Z <= 0)
            {
                return null;
            }

            // DEBUG
            //string incomingStr = new Ray(objectPos, lensPos - objectPos).ToString();

            // refract the incoming ray
            Vector3d incomingDir = Vector3d.Normalize(lensPos - objectPos);
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
            direction = Vector3d.Normalize(intersection.Position - lensPos);
            direction = Ray.Refract(direction, -frontSurface.GetNormal(intersection.Position),
                RefractiveIndex, Materials.Fixed.AIR, false);
            if (direction == Vector3d.Zero)
            {
                return null;
            }
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
            //Vector3d unitSphereSample = Sampler.UniformSampleHemisphere(sample);
            Vector3d unitSphereSample = Sampler.UniformSampleSphere(sample, sinTheta, 1);
            return backSurface.Center + backSurface.Radius * unitSphereSample;
        }

        public Vector3d GetFrontSurfaceSample(Vector2d sample)
        {
            // TODO: implement proper sampling
            // front surface is in the -Z hemisphere
            //Vector3d unitSphereSample = Sampler.UniformSampleHemisphere(sample);
            Vector3d unitSphereSample = Sampler.UniformSampleSphere(sample, sinTheta, 1);
            return frontSurface.Center + frontSurface.Radius * (-unitSphereSample);
        }

        #endregion

        private void UpdateSpheres()
        {
            double r = CurvatureRadius;
            backSurface.Radius = r;
            frontSurface.Radius = r;
            Vector3d center = Math.Sqrt(r * r - ApertureRadius * ApertureRadius)
                * Vector3d.UnitZ;
            backSurface.Center = -center;
            frontSurface.Center = center;
            // theta is the elevation angle from the base plane to the start
            // of the spherical cap
            // cos(theta) = sin(pi/2 - theta)
            double cosTheta = ApertureRadius / CurvatureRadius;
            sinTheta = Math.Sqrt(1 - cosTheta * cosTheta);
        }

        #region IIntersectable Members

        public Intersection Intersect(Ray ray)
        {
            return backSurface.Intersect(ray);
        }

        #endregion
    }
}
