namespace BokehLab.RayTracing
{
    using System;
    using BokehLab.Math;
    using OpenTK;

    // TODO:
    // - it would be possible to use Plane.Intersect() routine
    //   - on the other hand the circle should be a normalized primitive
    //     (at least centered at (0,0,0) and aligned with the XY plane)
    // - add a flag to invert the circle (a circular hole)

    public class Circle : IIntersectable
    {
        public double Radius { get; set; }

        #region IIntersectable Members

        public Intersection Intersect(Ray ray)
        {
            // there is an implicit assumption that the circle is on the XY
            // plane and centered at (0,0,0)

            double t = -ray.Origin.Y / ray.Direction.Y;
            // TODO: use a better epsilon
            if ((t < 0) || (ray.Direction.X < Double.Epsilon))
            {
                // no intersection with the circle plane
                return null;
            }
            Vector3d intersectionPos = new Vector3d();
            intersectionPos.Y = ray.Origin.Y - ray.Origin.X * ray.Direction.Y / ray.Direction.X;
            if (Math.Abs(intersectionPos.Y) > Radius)
            {
                // there is an intersection but outside the circle
                return null;
            }
            Intersection intersection = new Intersection(intersectionPos, null);
            return intersection;
        }

        #endregion
    }
}
