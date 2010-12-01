using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SphericLens
{
    public class Sphere
    {
        public double Radius { get; set; }

        public Sphere()
        {
            Radius = 1.0;
        }

        public bool IntersectRay(Ray ray, out Point intersection)
        {
            intersection = new Point();

            double a = ray.Direction.X * ray.Direction.X + ray.Direction.Y * ray.Direction.Y;
            double b = 2 * (ray.Origin.X * ray.Direction.X + ray.Origin.Y * ray.Direction.Y);
            double c = ray.Origin.X * ray.Origin.X + ray.Origin.Y * ray.Origin.Y - Radius * Radius;
            
            double discriminant = b * b - 4 * a * c; 
            if (discriminant < 0)
            {
                return false;
            }
            //// float-friendly quadratic equation
            //double q = -0.5 * (b + Math.Sign(b) * Math.Sqrt(discriminant));
            //double t1 = q / a;
            //double t2 = c / q;
            // normal quadratic equation
            double t1 = (-b + Math.Sqrt(discriminant)) / (2 * a);
            double t2 = (-b - Math.Sqrt(discriminant)) / (2 * a);
            
            if (t1 > t2)
            {
                double swap = t1;
                t1 = t2;
                t2 = swap;
            }
            if (t2 < 0) {
                return false;
            }
            double t = t1;
            if (t1 < 0)
            {
                t = t2;
            }
            intersection = ray.Evaluate(t);
            return true;
        }
    }
}
