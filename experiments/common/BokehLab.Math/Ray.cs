namespace BokehLab.Math
{
    using System;
    using OpenTK;

    public class Ray
    {
        public Vector3d Origin { get; set; }
        public Vector3d Direction { get; set; }

        public Ray(Vector3d origin, Vector3d direction)
        {
            Origin = origin;
            Direction = direction;
        }

        public Ray Transform(Matrix4d matrix)
        {
            Vector3d newOrigin = Vector3d.Transform(Origin, matrix);
            Vector3d newDirection = Vector3d.Transform(Direction, matrix);
            return new Ray(newOrigin, newDirection);
        }

        /// <summary>
        /// Evaluates the ray at parameter value t.
        /// </summary>
        /// <param name="t">Ray parameter, t >= 0</param>
        /// <returns>Point of ray, or null if the parameter is less than zero.
        /// </returns>
        public Vector3d? Evaluate(double t)
        {
            if (t < 0)
            {
                return null;
            }
            return Origin + t * Direction;
        }

        public void NormalizeDirection()
        {
            Direction = Vector3d.Normalize(Direction);
        }

        public static Ray NormalizeDirection(Ray ray)
        {
            return new Ray(ray.Origin, Vector3d.Normalize(ray.Direction));
        }

        public Ray Reflect(Vector3d normal)
        {
            return Reflect(this, normal);
        }

        public Ray Refract(Vector3d normal, double n1, double n2)
        {
            return Refract(this, normal, n1, n2);
        }

        public static Ray Reflect(Ray incoming, Vector3d normal)
        {
            Ray reflected = new Ray(
                incoming.Origin,
                Reflect(incoming.Direction, normal));
            return reflected;
        }

        public static Ray Refract(Ray incoming, Vector3d normal, double n1, double n2)
        {
            Vector3d direction = Refract(incoming.Direction, normal, n1, n2, false);
            Ray refracted = new Ray(incoming.Origin, direction);
            return refracted;
        }

        /// <summary>
        /// Reflects the ray incoming at a surface with given normal.
        /// </summary>
        /// <param name="incoming">Incoming ray direction. Must be normalized.
        /// </param>
        /// <param name="normal">Direction of normal to the surface. Must be
        /// normalized.</param>
        /// <returns>Reflected ray direction. Is normalized.</returns>
        public static Vector3d Reflect(Vector3d incoming, Vector3d normal)
        {
            double cosIncoming = Vector3d.Dot(normal, incoming);
            Vector3d reflected = incoming - (2 * cosIncoming) * normal;
            return reflected;
        }

        /// <summary>
        /// Refracts the ray incoming at a surface with given normal.
        /// </summary>
        /// <remarks>
        /// The result is normalized.
        /// </remarks>
        /// <param name="incoming">Incoming ray direction. Must be normalized.
        /// Direction of the incoming ray points to the opposite half-space as
        /// the normal points.
        /// </param>
        /// <param name="normal">Direction of normal to the surface. Must be
        /// normalized. Must be in from the same half-space as the incoming ray
        /// comes from (also the normal must point to the material with index
        /// of refraction n1).
        /// </param>
        /// <param name="n1">Index of refraction of material which the
        /// incoming ray comes from. Normal point to this material.</param>
        /// <param name="n2">Index of refraction of material where the
        /// incoming ray might get refracted.
        /// </param>
        /// <param name="computeTotalInternalReflection">
        /// Indicates whether the method should compute the vector of total
        /// internal reflection or just return a zero vector.
        /// </param>
        /// <returns>Refracted vector (the result should be normalized),
        /// reflected vector in case of TIR (and its computation computation
        /// is enabled) or zero vector (if TIR computation is disabled).</returns>
        public static Vector3d Refract(
            Vector3d incoming,
            Vector3d normal,
            double n1,
            double n2, bool computeTotalInternalReflection)
        {
            double cosIncoming = -Vector3d.Dot(normal, incoming);
            double eta = n1 / n2;
            double sinRefractedSqr = eta * eta * (1 - cosIncoming * cosIncoming);
            Vector3d result;
            if (sinRefractedSqr <= 1)
            {
                // refraction
                // the result should be normalized
                double cosRefracted = Math.Sqrt(1 - sinRefractedSqr);
                result = (eta * incoming) + (eta * cosIncoming - cosRefracted) * normal;
            }
            else
            {
                // total internal reflection - compute only if wanted
                if (computeTotalInternalReflection)
                {
                    result = Reflect(incoming, normal);
                }
                else
                {
                    result = Vector3d.Zero;
                }
            }
            return result;
        }

        // override object.Equals
        public override bool Equals(object obj)
        {
            //       
            // See the full list of guidelines at
            //   http://go.microsoft.com/fwlink/?LinkID=85237  
            // and also the guidance for operator== at
            //   http://go.microsoft.com/fwlink/?LinkId=85238
            //

            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            Ray otherRay = (Ray)obj;
            return Vector3d.Equals(Origin, otherRay.Origin) &&
                Vector3d.Equals(Direction, otherRay.Direction);
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            // TODO: write your implementation of GetHashCode() here
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return String.Format("[{0} -> {1}]", Origin, Direction);
        }
    }
}
