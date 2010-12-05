using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SphericLens
{
    /// <summary>
    /// A single spherical cap surface.
    /// The front side is oriented in the positive direction of the optical axis.
    /// </summary>
    public class SphericalCap
    {
        private double radius;

        /// <summary>
        /// Radius of curvature. Radius of the sphere from which is the
        /// spherical cap carved.
        /// When setting it can be positive or negative. For reading absolute
        /// value is returned.
        /// Positive - convex front side.
        /// Negative - concave front side.
        /// </summary>
        public double Radius { get { return radius; } set { radius = value; Update(); } }

        public bool convex;
        public bool Convex { get { return convex; } set { convex = value; Update(); } }

        private double aperture;

        /// <summary>
        /// Radius of the circle which bounds the spherical cap and is
        /// perpendicular to the optical axis.
        /// </summary>
        public double Aperture { get { return aperture; } set { aperture = Math.Min(value, Radius); Update(); } }

        public double NextRefractiveIndex { get; set; }

        /// <summary>
        /// The distance from the aperture circle to the apex of the spherical cap.
        /// </summary>
        public double Thickness { get; private set; }

        /// <summary>
        /// The angle between two ray staring in the sphere center,
        /// one along the optical axis and the second to the border of the aperture.
        /// </summary>
        public double Angle { get; private set; }

        public double xMin;
        public double xMax;

        /// <summary>
        /// Distance from this element's apex to the next element's apex.
        /// </summary>
        public double DistanceToNext { get; set; }

        public SphericalCap()
        {
            DistanceToNext = 0.0;
            radius = 1.0;
            aperture = 1.0;
            NextRefractiveIndex = RefractiveIndices.CROWN_GLASS;
            Update();
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
            intersection = ray.Evaluate(t + 10e-5f);
            if (((xMin > -Radius) && (intersection.X < xMin)) ||
                ((xMax < Radius) && (intersection.X > xMax)))
            {
                if (t == t2) { return false; }
                t = t2;
                intersection = ray.Evaluate(t + 10e-5f);
                if (((xMin > -Radius) && (intersection.X < xMin)) ||
                ((xMax < Radius) && (intersection.X > xMax)))
                {
                    return false;
                }
            }
            return true;
        }

        private void Update() {
            Angle = Math.Asin(Aperture / Radius);
            Thickness = Radius * (1.0 - Math.Cos(Angle));
            xMin = Convex ? (Radius - Thickness) : -Radius;
            xMax = Convex ? Radius : (-Radius + Thickness);
        }
    }
}
