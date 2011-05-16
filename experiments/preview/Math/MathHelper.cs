namespace BokehLab.Math
{
    using System;

    public class MathHelper
    {
        public static T Clamp<T>(T number, T min, T max)
            where T : IComparable<T>
        {
            if (number.CompareTo(min) < 0) return min;
            if (number.CompareTo(max) > 0) return max;
            return number;
        }

        public static double Clamp(double number)
        {
            return Clamp(number, 0.0, 1.0);
        }

        public static float Clamp(float number)
        {
            return Clamp(number, 0.0f, 1.0f);
        }

    }
}
