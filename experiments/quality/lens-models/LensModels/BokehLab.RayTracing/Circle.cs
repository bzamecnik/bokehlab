namespace BokehLab.RayTracing
{
    using System;
    using BokehLab.Math;
    using OpenTK;

    public class Circle : IIntersectable
    {
        public double Radius { get; set; }

        private double z = 0;
        public double Z
        {
            get { return z; }
            set
            {
                z = value;
                plane.Origin = z * Vector3d.UnitZ;
            }
        }

        private Plane plane = new Plane()
        {
            Normal = Vector3d.UnitZ,
            Origin = Vector3d.Zero
        };

        //public bool Inverted = false;

        public Circle()
        {
            Radius = 1;
        }

        #region IIntersectable Members

        public Intersection Intersect(Ray ray)
        {
            //// there is an implicit assumption that the circle is on the XY
            //// plane and centered at (0,0,0)

            //Vector3d origin = ray.Origin;

            //// transform to XY plane
            //origin.Z -= Z;

            //double t = -origin.Y / ray.Direction.Y;
            //// TODO: use a better epsilon
            //if ((t < 0) || (ray.Direction.X < Double.Epsilon))
            //{
            //    // no intersection with the circle plane
            //    return null;
            //}
            //Vector3d intersectionPos = new Vector3d();
            //intersectionPos.Y = origin.Y - origin.X * ray.Direction.Y / ray.Direction.X;
            //if (Math.Abs(intersectionPos.Y) > Radius)
            //{
            //    // there is an intersection but outside the circle
            //    return null;
            //}

            //// transform from XY plane
            //intersectionPos.Z += Z;

            Intersection intersection = plane.Intersect(ray);
            if (intersection == null)
            {
                return null;
            }
            bool inside = intersection.Position.Xy.LengthSquared <= Radius * Radius;
            //if (Inverted)
            //{
            //    inside = !inside;
            //}
            return (inside) ? intersection : null;
        }

        #endregion
    }
}
