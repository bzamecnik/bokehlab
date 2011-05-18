namespace BokehLab.RayTracing
{
    using System;
    using BokehLab.Math;
    using OpenTK;

    public class Sphere : IIntersectable
    {
        public double Radius { get; set; }

        public Vector3d Center { get; set; }

        #region IIntersectable Members

        public Intersection Intersect(Ray ray)
        {
            // Method: Zara et al.: Modern Computer Graphics

            // T is a point in the middle of the segment between the two
            // intersections (if they exist).

            Vector3d b = Center - ray.Origin;
            // |b|^2
            double bLengthSqr = Vector3d.Dot(b, b);
            // length od projection of b to ray.Direction
            // t0 = |(T-Center)| = |b.direction|
            // t0 is a ray parameter, T = Origin + t0 * Direction
            double t0 = Math.Abs(Vector3d.Dot(b, ray.Direction));
            // d = |(S-T)|, d^2 = |b|^2 - t0^2 (Pythagorean theorem)
            double dSqr = bLengthSqr - t0 * t0;
            // td ... distance from T to the intersection(s)
            // td^2 = Radius^2 - d^2 (Pythagorean theorem again)
            // td also acts as a ray parameter
            double tdSqr = Radius * Radius - dSqr;

            Vector3d intersection;
            // TODO: use a better epsilon
            if (tdSqr > Double.Epsilon)
            {
                // two intersections, at Center + (t0 +/- td) * Direction
                // we're intereseted only in the first intersection
                double td = Math.Sqrt(tdSqr);
                intersection = Center + (t0 - td) * ray.Direction;
            }
            else if (tdSqr < -Double.Epsilon)
            {
                // no intersection
                return null;
            }
            else // if (tdSqr == 0)
            {
                // one (double) intersection, at Center + t0 * Direction
                intersection = Center + t0 * ray.Direction;
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
