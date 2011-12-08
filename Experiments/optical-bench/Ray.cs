using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SphericLens
{
    public class Ray
    {
        public Point Origin { get; set; }
        private Vector direction;
        public Vector Direction {
            get { return direction; }
            set { direction = value; }
        }

        public Ray() :
            this(new Point(), new Vector())
        {
        }

        public Ray(Point origin, Vector direction)
        {
            Origin = new Point(origin);
            Direction = new Vector(direction);
        }

        public Ray(Ray ray)
            : this(ray.Origin, ray.direction)
        {
        }

        public Point Evaluate(double t)
        {
            if (t < 0)
            {
                throw new ArgumentException("Parameter t must not be negative");
            }
            return Origin + t * Direction;
        }

        public Ray Translate(Vector translation)
        {
            return new Ray(Origin + translation, Direction);
        }
    }
}
