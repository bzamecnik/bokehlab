using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace spreading.PSF.Perimeter
{
    public struct Delta
    {
        public float value;
        public int x;
        public int y;

        public Delta(int x, int y, float value)
        {
            this.x = x;
            this.y = y;
            this.value = value;
        }
    }

    public struct PSFDescription
    {
        public Delta[][] deltasByRadius;
        public int MaxRadius
        {
            get
            {
                return deltasByRadius.Length - 1;
            }
        }
    }
}
