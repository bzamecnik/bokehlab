namespace BokehLab.Math
{
    using System;
    using OpenTK;

    public class MathHelper
    {
        public static T Clamp<T>(T number, T min, T max)
            where T : IComparable<T>
        {
            if (number.CompareTo(min) < 0) return min;
            if (number.CompareTo(max) > 0) return max;
            return number;
        }

        public static T ClampMax<T>(T number, T max)
            where T : IComparable<T>
        {
            return (number.CompareTo(max) > 0) ? max : number;
        }

        public static T ClampMin<T>(T number, T min)
            where T : IComparable<T>
        {
            return (number.CompareTo(min) < 0) ? min : number;
        }

        public static double Clamp(double number)
        {
            return Clamp(number, 0.0, 1.0);
        }

        public static float Clamp(float number)
        {
            return Clamp(number, 0.0f, 1.0f);
        }

        /// <summary>
        /// Linearly interpolate between values A and B.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="ratio">Ratio of B in resulting blend. [0.0; 1.0]</param>
        /// <returns></returns>
        public static double Lerp(double a, double b, double ratio)
        {
            return (1 - ratio) * a + ratio * b;
        }

        /// <summary>
        /// Compute the modular division of a given number taking care even of
        /// negative numbers.
        /// </summary>
        /// <remarks>
        /// Converts a given number N from the Z group into the Z/M group
        /// where M is the modulus.
        /// </remarks>
        /// <param name="number"></param>
        /// <param name="modulus"></param>
        /// <returns></returns>
        public static int Mod(int number, int modulus)
        {
            int r = number % modulus;
            return r >= 0 ? r : r + modulus;
        }
    }
}
