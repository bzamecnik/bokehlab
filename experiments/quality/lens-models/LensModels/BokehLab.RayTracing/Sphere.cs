namespace BokehLab.RayTracing
{
    using System;
    using BokehLab.Math;
    using OpenTK;

    public class Sphere : IIntersectable, INormalField
    {
        public double Radius { get; set; }

        public Vector3d Center { get; set; }

        private static readonly double epsilon = 1e-6;

        public Sphere()
        {
            Radius = 1;
            Center = Vector3d.Zero;
        }

        #region IIntersectable Members

        public Intersection Intersect(Ray ray)
        {
            // Method: Zara et al.: Modern Computer Graphics

            // T is a point in the middle of the segment between the two
            // intersections (if they exist).
            // T is also projection of Center to the ray, thus (T-Center) is
            // perpendicular to (T-Origin).

            // we must work with normalized ray direction
            Vector3d direction = ray.Direction;
            direction.Normalize();

            Vector3d b = Center - ray.Origin;
            // |b|^2
            double bLengthSqr = Vector3d.Dot(b, b);

            // t_0 is the length od projection of b to ray.Direction
            // t_0 = |(T-Origin)| = |b.direction|
            // t_0 is a ray parameter, T = Origin + t_0 * Direction
            double bDotDirection = Vector3d.Dot(b, direction);
            double t0 = Math.Abs(bDotDirection);
            // d = |(T-Center)|, d^2 = |b|^2 - t_0^2 (Pythagorean theorem)
            double dSqr = bLengthSqr - t0 * t0;
            // t_d ... distance from T to the intersection(s)
            // t_d^2 = Radius^2 - d^2 (Pythagorean theorem again)
            // t_d also acts as a ray parameter
            double tdSqr = Radius * Radius - dSqr;

            Vector3d intersection;
            if (tdSqr > epsilon)
            {
                // two intersections, at Origin + (t_0 +/- t_d) * Direction
                // we're intereseted only in the first intersection
                // NOTE: bDotDirection is the signed t0
                double td = Math.Sqrt(tdSqr);
                double t = bDotDirection - td;
                if (t < epsilon)
                {
                    // the first intersection is behind the ray or
                    // the ray origin is the first intersection
                    t = bDotDirection + td;
                }
                if (t < epsilon)
                {
                    // even the second intersection is behind the ray
                    return null;
                }
                intersection = ray.Origin + t * direction;
            }
            else if (tdSqr < -epsilon)
            {
                // no intersection
                return null;
            }
            else // if ((t_d)^2 == 0)
            {
                // one (double) intersection, at Origin + t_0 * Direction
                double t = bDotDirection;
                if (t < epsilon)
                {
                    // intersection is behind the ray
                    return null;
                }
                intersection = ray.Origin + t * direction;
            }
            return new Intersection(intersection, null);
        }

        #endregion

        #region INormalField Members

        public Vector3d GetNormal(Vector3d surfacePoint)
        {
            return (surfacePoint - Center) / Radius;
        }

        #endregion

        public double GetCapElevationAngleSine(double baseRadius)
        {
            // theta is the elevation angle from the base plane to the start
            // of the spherical cap
            // cos(theta) = sin(pi/2 - theta)
            double cosTheta = baseRadius / Radius;
            double sinTheta = Math.Sqrt(1 - cosTheta * cosTheta);
            return sinTheta;
        }

        public Vector3d GetCapCenter(double baseRadius, Vector3d direction)
        {
            if (Radius < baseRadius)
            {
                throw new ArgumentException(
                    "Spherical cap base circle radius must not be greater than sphere radius.");
            }
            return Math.Sqrt(Radius * Radius - baseRadius * baseRadius)
                 * direction;
        }

        /// <summary>
        /// Computes the height of a spherical cap.
        /// </summary>
        /// <param name="radius">Radius of the sphere</param>
        /// <param name="apertureRadius">Radius of the cap base</param>
        /// <returns></returns>
        public double GetCapHeight(double radius, double baseRadius)
        {
            return radius - Math.Sqrt(radius * radius - baseRadius * baseRadius);
        }
    }
}
