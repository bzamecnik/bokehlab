using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;

namespace BokehLab.ImageBasedRayCasting
{
    class Intersection
    {
        public Vector3d position;
        public float[] color;

        public Intersection(Vector3d position, float[] color)
        {
            this.position = position;
            this.color = color;
        }
    }
}
