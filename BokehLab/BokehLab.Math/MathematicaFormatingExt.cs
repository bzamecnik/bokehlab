namespace BokehLab.Math
{
    using System;
    using System.Text;
    using System.Globalization;
    using OpenTK;

    public static class MathematicaFormatingExt
    {
        public static string ToMathematica(this Vector3d vector)
        {
            return String.Format(CultureInfo.InvariantCulture.NumberFormat,
                    "{{ {0:0.##},{1:0.##},{2:0.##} }}",
                    vector.X, vector.Y, vector.Z);
        }

        public static string ToArrow(this Ray ray)
        {
            Vector3d origin = ray.Origin;
            Vector3d target = ray.Origin + ray.Direction;
            return ToArrow(origin.ToMathematica(), target.ToMathematica());
        }

        public static string ToLine(this Ray ray)
        {
            Vector3d origin = ray.Origin;
            Vector3d target = ray.Origin + ray.Direction;
            return ToLine(origin.ToMathematica(), target.ToMathematica());
        }

        public static string ToPoint(this Vector3d vector)
        {
            return ToPoint(vector.ToMathematica());
        }

        public static string ToPoint(string point)
        {
            return String.Format("Point[{0}]", point);
        }

        public static string ToArrow(string beginning, string end)
        {
            return String.Format("Arrow[{{ {0}, {1} }}]", beginning, end);
        }
        public static string ToLine(string beginning, string end)
        {
            return String.Format("Line[{{ {0}, {1} }}]", beginning, end);
        }

    }
}
