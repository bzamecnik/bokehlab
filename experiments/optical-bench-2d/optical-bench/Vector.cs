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

        public Vector() 
            : this(0.0, 0.0)
        {
        }

        public Vector(double x, double y)
        {
            X = x;
            Y = y;
        }

        public Vector(Vector vector) 
            : this(vector.X, vector.Y)
        {
        }

        public static Vector FromPoint(Point point) {
            return new Vector(point.X, point.Y);
        }

        public static Vector FromPolar(double phi, double radius)
        {
            return new Vector(radius * Math.Cos(phi), radius * Math.Sin(phi));
        }

        public static Vector operator + (Vector p1, Vector p2) {
            return new Vector(p1.X + p2.X, p1.Y + p2.Y);
        }

        public static Vector operator -(Vector p1, Vector p2)
        {
            return new Vector(p1.X - p2.X, p1.Y - p2.Y);
        }

        public static Vector operator *(double t, Vector v)
        {
            return new Vector(t * v.X, t * v.Y);
        }

        public static Vector operator *(Vector v, double t)
        {
            return t * v;
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
            return Vector.FromPolar(Phi, 1.0);
        }

        /// <summary>
        /// Refract a ray on a border of different optical environments.
        /// </summary>
        /// <remarks>
        /// In case of total internal reflection a zero vector is returned.
        /// 
        /// The code is based on the PBRT implementation.
        /// </remarks>
        /// <param name="incident">incident vector in the world coordinates</param>
        /// <param name="normal">normal vector pointing outside the surface</param>
        /// <param name="etaI">index of refraction of the outer medium</param>
        /// <param name="etaT">index of refraction of the inner medium</param>
        /// <returns></returns>
        public static Vector refract(Vector incident, Vector normal, double etaI, double etaT)
        {
            Vector incidentNormalized = -incident.Normalize();

            // transform incident vector to the local coordinates with normal = (0, 1)
            double transformToLocalPhi = 0.5 * Math.PI - normal.Phi;
            incidentNormalized.Phi += transformToLocalPhi;

            // alpha = incident angle (incident vector to normal)
            // beta = refracted angle (refracted vector to normal)
            double cosi = incidentNormalized.Y;
            bool entering = cosi > 0.0;

            //// swap the indices of the ray is going from inside the surface
            //double ei = entering ? etaI : etaT;
            //double et = entering ? etaT : etaI;
            double ei = etaI;
            double et = etaT;

            double sini2 = Math.Max(0.0, 1.0 - incidentNormalized.Y * incidentNormalized.Y);
            double eta = ei / et;
            double sint2 = eta * eta * sini2;

            Vector refracted;
            if (sint2 < 1.0)
            {
                // compute the refraction vector
                double cost = Math.Sqrt(Math.Max(0.0, 1.0 - sint2));
                if (entering)
                {
                    cost = -cost;
                }
                double sintOverSini = eta;
                refracted = new Vector(sintOverSini * -incidentNormalized.X, cost);
                // transform back from the local coordinates
                refracted.Phi -= transformToLocalPhi;
            }
            else
            {
                // total internal reflection
                refracted = new Vector(incident);
                refracted.Phi += transformToLocalPhi;
                refracted.phi *= -1.0;
                refracted.Phi -= transformToLocalPhi;

                // or posibly return a dummy vector
                //return new Vector(0.0, 0.0);
            }
            return refracted;
        }

        private void ToPolar() {
            double p = Math.Atan2(Y, X);
            phi = (p < 0.0) ? (p + 2.0 * Math.PI) : p;
            radius = Math.Sqrt(Y * Y + X * X);
        }

        private void FromPolar()
        {
            x = radius * Math.Cos(phi);
            y = radius * Math.Sin(phi);
        }
    }
}
