namespace BokehLab.RayTracing
{
    using BokehLab.Math;
    using OpenTK;

    public class Plane : IIntersectable
    {
        public Vector3d Origin { get; set; }

        public Vector3d Normal { get; set; }

        #region IIntersectable Members

        public Intersection Intersect(Ray ray)
        {
            double t = Vector3d.Dot((Origin - ray.Origin), Normal)
                / Vector3d.Dot(ray.Direction, Normal);
            Vector3d? intersectionPos = ray.Evaluate(t);
            return intersectionPos.HasValue ?
                new Intersection(intersectionPos.Value) : null;
        }

        public Vector3d GetNormal(Vector3d position)
        {
            return Normal;
        }

        #endregion
    }
}
