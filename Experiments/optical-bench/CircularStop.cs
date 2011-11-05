using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;

namespace SphericLens
{
    public class CircularStop : OpticalElement
    {
        //private Vector3d Origin = Vector3d.Zero;

        //private Vector3d Normal = Vector3d.UnitZ;

        public double Aperture { get; set; }

        public override Vector TranslationToLocal()
        {
            return new Vector(0, 0);
        }

        public override bool IntersectRay(Ray ray, out Point intersection)
        {
            intersection = new Point();

            //Vector3d rayOrigin = new Vector3d(ray.Origin.X, 0, ray.Origin.Y);
            //Vector3d rayDirection = new Vector3d(ray.Direction.X, 0, ray.Direction.Y);

            //double t = Vector3d.Dot(Origin - rayOrigin, Normal)
            //        / Vector3d.Dot(Vector3d, Normal);
            //Point intersectionPos = ray.Evaluate(t);
            //return intersectionPos.HasValue ?
            //    new Intersection(intersectionPos.Value) : null;


            double t = -ray.Origin.Y / ray.Direction.Y;
            if ((t < 0))// || (ray.Direction.X < Double.Epsilon)
            {
                return false;
            }
            intersection.Y = ray.Origin.Y - ray.Origin.X * ray.Direction.Y / ray.Direction.X;
            return Math.Abs(intersection.Y) <= Aperture;
        }
    }
}
