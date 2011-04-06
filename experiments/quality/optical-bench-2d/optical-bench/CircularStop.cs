using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SphericLens
{
    public class CircularStop : OpticalElement
    {
        public double Aperture { get; set; }

        public override Vector TranslationToLocal()
        {
            return new Vector(0, 0);
        }

        public override bool IntersectRay(Ray ray, out Point intersection) {
            intersection = new Point();
            double t = -ray.Origin.Y / ray.Direction.Y;
            if ((t < 0) || (ray.Direction.X < Double.Epsilon))
            {
                return false;
            }
            intersection.Y = ray.Origin.Y - ray.Origin.X * ray.Direction.Y / ray.Direction.X;
            return Math.Abs(intersection.Y) <= Aperture;
        }
    }
}
