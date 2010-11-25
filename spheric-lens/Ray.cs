using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SphericLens
{
    public class Ray
    {
        public Point Origin { get; set; }
        public Vector Direction { get; set; }

        public Ray() {
            Origin = new Point();
            Direction = new Vector();
        }

        public Ray(Point origin, Vector direction)
        {
            Origin = origin;
            Direction = direction;
        }

        public Point Evaluate(double t)
        {
            if (t < 0)
            {
                throw new ArgumentException("Parameter t must not be negative");
            }
            return Origin + t * Direction;
        }
    }
}
