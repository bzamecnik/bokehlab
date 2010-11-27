using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SphericLens
{
    public class Vector
    {
        private double x;
        public double X { get { return x; } set { x = value; ToPolar(); } }
        private double y;
        public double Y { get { return y; } set { y = value; ToPolar(); } }

        private double phi;
        public double Phi { get { return phi; } set { phi = value; FromPolar(); } }
        private double radius;
        public double Radius { get { return radius; } set { radius = value; FromPolar(); } }

        public double Length {get {return Radius;}}

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

        public static Vector operator /(Vector v, double d)
        {
            return new Vector(v.X / d, v.Y / d);
        }

        public static double Dot(Vector v1, Vector v2) {
            return v1.X * v2.X + v1.Y * v2.Y;
        }

        public Vector Normalize() {
            return this / Length;
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
            double criticalAngle = (eta > 1.0) ? Math.PI + Math.Asin(1 / eta) : Math.Asin(eta);
            double alpha = incident.Phi - normal.Phi;
            //// not total internal reflection ? refract : reflect
            double beta = (alpha > criticalAngle) ? Math.Asin(eta * Math.Sin(alpha)) : -alpha;
            Vector refracted = normal;
            refracted.Phi += beta;
            return refracted;
        }

        private void ToPolar() {
            double p = Math.Atan2(Y, X);
            phi = (phi < 0.0) ? (phi + 2.0 * Math.PI) : phi;
            radius = Math.Sqrt(Y * Y + X * X);
        }

        private void FromPolar()
        {
            x = radius * Math.Cos(phi);
            y = radius * Math.Sin(phi);
        }

        public static Vector FromPolar(double phi, double radius) {
            return new Vector(radius * Math.Cos(phi), radius * Math.Sin(phi));
        }
    }
}
