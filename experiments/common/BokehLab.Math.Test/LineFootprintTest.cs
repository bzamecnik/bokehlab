using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BokehLab.Math;
using OpenTK;

namespace BokehLab.Math.Test
{
    class LineFootprintTest
    {
        public void TrySimpleHeightfield1x1()
        {
            LineFootprint.HeightField heightfield = new LineFootprint.HeightField();
            heightfield.Data = new float[1, 1, 1];
            heightfield.Data[0, 0, 0] = 0.5f;

            Vector3d start = new Vector3d(0.1, 0.1, 0.25);
            Vector3d end = new Vector3d(0.2, 0.2, 0.75);
            IntersectAndReport(heightfield, start, end);
        }

        public void TrySimpleHeightfield1x1WithPerpendicularRay()
        {
            LineFootprint.HeightField heightfield = new LineFootprint.HeightField();
            heightfield.Data = new float[1, 1, 1];
            heightfield.Data[0, 0, 0] = 0.5f;

            Vector3d start = new Vector3d(0.1, 0.1, 0.25);
            Vector3d end = new Vector3d(0.1, 0.1, 0.75);
            IntersectAndReport(heightfield, start, end);
        }


        public void TrySimpleHeightfield2x2()
        {
            LineFootprint.HeightField heightfield = new LineFootprint.HeightField();
            heightfield.Data = new float[2, 2, 1];
            heightfield.Data[0, 0, 0] = 0.5f;
            heightfield.Data[0, 1, 0] = 0.5f;
            heightfield.Data[1, 0, 0] = 0.5f;
            heightfield.Data[1, 1, 0] = 0.5f;

            Vector3d start = new Vector3d(0.5, 1.5, 0.0);
            Vector3d end = new Vector3d(2, 2, 1);
            IntersectAndReport(heightfield, start, end);
        }

        private static void IntersectAndReport(LineFootprint.HeightField heightfield, Vector3d start, Vector3d end)
        {
            Ray ray = new Ray(start, end - start);
            Vector3 intersection;
            Console.WriteLine(LineFootprint.IntersectHeightField(ray, heightfield, out intersection));
            Console.WriteLine(intersection);
        }
    }
}
