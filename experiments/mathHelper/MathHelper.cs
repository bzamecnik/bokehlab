using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mathHelper
{
    public class MathHelper
    {
        // TODO: use generics

        public static double clamp(double number, double min, double max)
        {
            if (number < min) return min;
            if (number > max) return max;
            return number;
        }

        public static float clamp(float number, float min, float max)
        {
            if (number < min) return min;
            if (number > max) return max;
            return number;
        }

        public static int clamp(int number, int min, int max)
        {
            if (number < min) return min;
            if (number > max) return max;
            return number;
        }

        public static double clamp(double number)
        {
            return clamp(number, 0.0, 1.0);
        }

        public static float clamp(float number)
        {
            return clamp(number, 0.0f, 1.0f);
        }

    }
}
