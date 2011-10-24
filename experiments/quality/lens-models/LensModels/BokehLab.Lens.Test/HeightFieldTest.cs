using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BokehLab.Math;
using OpenTK;
using BokehLab.RayTracing;
using BokehLab.FloatMap;

namespace BokehLab.RayTracing.Test
{
    class HeightFieldTest
    {
        public void TrySimpleHeightfield1x1()
        {
            FloatMapImage data = new FloatMapImage(1, 1, PixelFormat.Greyscale);
            data.Image[0, 0, 0] = 0.5f;
            HeightField heightfield = new HeightField(new[] { data });

            Vector3d start = new Vector3d(0.1, 0.1, 0.25);
            Vector3d end = new Vector3d(0.2, 0.2, 0.75);
            IntersectAndReport(heightfield, start, end);
        }

        public void TrySimpleHeightfield1x1WithPerpendicularRay()
        {
            FloatMapImage data = new FloatMapImage(1, 1, PixelFormat.Greyscale);
            data.Image[0, 0, 0] = 0.5f;
            HeightField heightfield = new HeightField(new[] { data });

            Vector3d start = new Vector3d(0.1, 0.1, 0.25);
            Vector3d end = new Vector3d(0.1, 0.1, 0.75);
            IntersectAndReport(heightfield, start, end);
        }

        public void TrySimpleHeightfield2x2()
        {
            FloatMapImage data = new FloatMapImage(2, 2, PixelFormat.Greyscale);
            data.Image[0, 0, 0] = 0.5f;
            data.Image[0, 1, 0] = 0.5f;
            data.Image[1, 0, 0] = 0.5f;
            data.Image[1, 1, 0] = 0.5f;
            HeightField heightfield = new HeightField(new[] { data });

            Vector3d start = new Vector3d(0.5, 1.5, 0.0);
            Vector3d end = new Vector3d(2, 2, 1);
            IntersectAndReport(heightfield, start, end);
        }

        private static void IntersectAndReport(HeightField heightfield, Vector3d start, Vector3d end)
        {
            Ray ray = new Ray(start, end - start);
            Intersection intersection = heightfield.Intersect(ray);
            Console.WriteLine((intersection != null) ? intersection.Position.ToString() : "no intersection");
        }
    }
}
