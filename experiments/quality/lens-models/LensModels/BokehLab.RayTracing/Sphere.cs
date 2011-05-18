namespace BokehLab.RayTracing
{
    using System;
    using BokehLab.Math;
    using OpenTK;

    public class Sphere : IIntersectable
    {
        public double Radius { get; set; }

        public Vector3d Center { get; set; }

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
            double t0 = Math.Abs(Vector3d.Dot(b, direction));
            // d = |(T-Center)|, d^2 = |b|^2 - t_0^2 (Pythagorean theorem)
            double dSqr = bLengthSqr - t0 * t0;
            // t_d ... distance from T to the intersection(s)
            // t_d^2 = Radius^2 - d^2 (Pythagorean theorem again)
            // t_d also acts as a ray parameter
            double tdSqr = Radius * Radius - dSqr;

            Vector3d intersection;
            // TODO: use a better epsilon
            if (tdSqr > Double.Epsilon)
            {
                // two intersections, at Origin + (t_0 +/- t_d) * Direction
                // we're intereseted only in the first intersection
                double td = Math.Sqrt(tdSqr);
                double t = t0 - td;
                if (Math.Abs(t) < double.Epsilon)
                {
                    // ray origin is the first intersection
                    t = t0 + td;
                }
                intersection = ray.Origin + t * direction;
            }
            else if (tdSqr < -Double.Epsilon)
            {
                // no intersection
                return null;
            }
            else // if ((t_d)^2 == 0)
            {
                // one (double) intersection, at Origin + t_0 * Direction
                intersection = ray.Origin + t0 * direction;
            }
            return new Intersection(intersection, null);
        }

        public Vector3d GetNormal(Vector3d surfacePoint)
        {
            return (surfacePoint - Center) / Radius;
        }

        #endregion
    }
}
