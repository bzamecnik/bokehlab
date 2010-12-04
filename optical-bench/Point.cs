using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SphericLens
{
    public class Point
    {
        public double X { get; set; }
        public double Y { get; set; }

        public Point() : this(0.0, 0.0)
        {
        }

        public Point(double x, double y)
        {
            X = x;
            Y = y;
        }

        public Point(Point point)
            : this(point.X, point.Y)
        {
        }

        public static Point FromVector(Vector vector)
        {
            return new Point(vector.X, vector.Y);
        }

        public static Point operator +(Point p, Vector v)
        {
            return new Point(p.X + v.X, p.Y + v.Y);
        }

        public static Point operator +(Vector v, Point p)
        {
            return p + v;
        }

    }
}
