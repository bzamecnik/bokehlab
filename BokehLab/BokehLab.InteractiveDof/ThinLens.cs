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

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("ThinLens {");
            sb.AppendFormat("  Focal length: {0},", FocalLength);
            sb.AppendLine();
            sb.AppendFormat("  Aperture number: {0},", ApertureNumber);
            sb.AppendLine();
            sb.AppendFormat("  Aperture radius: {0}", ApertureRadius);
            sb.AppendLine();
            sb.AppendLine("}");
            return sb.ToString();
        }
    }
}
