namespace BokehLab.InteractiveDof
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using OpenTK;

    class ThinLens
    {
        public float FocalLength { get; set; }

        public float ApertureNumber { get; set; }

        public float ApertureRadius { get { return FocalLength / ApertureNumber; } }

        public ThinLens()
        {
            FocalLength = 1.0f;
            ApertureNumber = 1.0f;
        }

        public Vector3 Transform(Vector3 point)
        {
            float absZ = Math.Abs(point.Z);
            if (absZ != FocalLength)
            {
                return point / (1 - absZ / FocalLength);
            }
            else
            {
                return point;
            }
        }
    }
}
