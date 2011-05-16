namespace BokehLab.Lens.Test
{
    using System.Collections.Generic;
    using BokehLab.Math;

    internal class RayEqualityComparer : EqualityComparer<Ray>
    {
        double Epsilon { get; set; }

        public RayEqualityComparer(double precision)
        {
            Epsilon = precision;
        }
        public override bool Equals(Ray x, Ray y)
        {
            if ((x == null) && (y == null))
            {
                return true;
            }

            // TODO: this is not a nice epsilon comparison :/
            if ((x.Origin.X - y.Origin.X) > Epsilon)
            {
                return false;
            }
            if ((x.Origin.Y - y.Origin.Y) > Epsilon)
            {
                return false;
            }
            if ((x.Origin.Z - y.Origin.Z) > Epsilon)
            {
                return false;
            }
            if ((x.Direction.X - y.Direction.X) > Epsilon)
            {
                return false;
            }
            if ((x.Direction.Y - y.Direction.Y) > Epsilon)
            {
                return false;
            }
            if ((x.Direction.Z - y.Direction.Z) > Epsilon)
            {
                return false;
            }
            return true;
        }
        public override int GetHashCode(Ray obj)
        {
            return obj.GetHashCode();
        }
    }
}
