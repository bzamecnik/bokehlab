using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SphericLens
{
    public class Vector
    {
        public double X { get; set; }
        public double Y { get; set; }

        public Vector() {
            X = 0.0;
            Y = 0.0;
        }

        public Vector(double x, double y)
        {
            X = x;
            Y = y;
        }

        public static Vector FromPoint(Point point) {
            return new Vector(point.X, point.Y);
        }

        public static Vector operator + (Vector p1, Vector p2) {
            return new Vector(p1.X + p2.X, p1.Y + p2.Y);
        }

        public static Vector operator *(double t, Vector v)
        {
            return new Vector(t * v.X, t * v.Y);
        }

        public static Vector operator *(Vector v, double t)
        {
            return v * t;
        }

        public static Vector operator -(Vector v)
        {
            return new Vector(-v.X, -v.Y);
        }

        public static double Dot(Vector v1, Vector v2) {
            return v1.X * v2.X + v1.Y * v2.Y;
        }

        /// <summary>
        /// Refract a ray on a border of different optical environments.
        /// </summary>
        /// <remarks>
        /// Source optical environment's index of refraction is n_1.
        /// Destinations's one is n_2.
        /// A total internal reflection can occur.
        /// </remarks>
        /// <param name="eta">the ratio of indices of refraction n_1 / n_2</param>
        /// <returns></returns>
        public static Vector refract(Vector incident, Vector normal, double eta)
        {
            // alpha = incident angle (incident vector to normal)
            // beta = refracted angle (refracted vector to normal)
            double cosAlpha = Dot(-incident, normal);
            double cosBeta2 = 1.0 - eta * eta * (1.0 - cosAlpha * cosAlpha);
            Vector refracted = eta * incident + ((eta * cosAlpha - Math.Sqrt(Math.Abs(cosBeta2))) * normal);
            return (cosBeta2 > 0) ? refracted : -refracted;
        }
    }
}
