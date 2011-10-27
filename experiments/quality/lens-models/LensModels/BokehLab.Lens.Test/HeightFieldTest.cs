namespace BokehLab.RayTracing.Test
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.Linq;
    using System.Text;
    using BokehLab.FloatMap;
    using BokehLab.Math;
    using BokehLab.NBuffers;
    using BokehLab.RayTracing;
    using OpenTK;

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

        private static void MeasureIntersectionPerformance(HeightField heightfield, Vector3d start, Vector3d end, int iterations)
        {
            Vector3d direction = end - start;
            Ray ray = new Ray(start, direction);

            HeightField.FootprintDebugInfo debugInfo = new HeightField.FootprintDebugInfo();
            Intersection isec = heightfield.Intersect(ray, ref debugInfo);
            int visitedPixels = debugInfo.VisitedPixels.Count;

            Stopwatch sw = Stopwatch.StartNew();
            for (int i = 0; i < iterations; i++)
            {
                Intersection intersection = heightfield.Intersect(ray);
            }
            sw.Stop();

            Console.WriteLine("Intersection?: {0}", isec != null);
            Console.WriteLine("Ray length: {0:0.0}", direction.Length);
            Console.WriteLine("Visited pixels: {0}", visitedPixels);
            Console.WriteLine("Iterations: {0}", iterations);
            Console.WriteLine("Total time: {0} ms", sw.ElapsedMilliseconds);
            Console.WriteLine("Average time: {0:0.000} ms", sw.ElapsedMilliseconds / (double)iterations);
            double throughput = iterations / ((double)sw.ElapsedMilliseconds * 0.001);
            Console.WriteLine("Throughput: {0:0.000} traversals/s", throughput);
            Console.WriteLine("Throughput of visited pixels: {0:0.000} Mpx/s", throughput * visitedPixels * 1e-6);
        }

        private static void TryFootprintTraversalPerformance()
        {
            //HeightField heightfield = new HeightField(1000, 1000);
            //Vector3d start = new Vector3d(2.5, 1.5, 0);
            //Vector3d end = new Vector3d(999.5, 998.5, 1);

            HeightField heightfield = new HeightField(200, 200);
            //Vector3d start = new Vector3d(169, 181, 0);
            //Vector3d end = new Vector3d(14, 191, 1);
            //Vector3d start = new Vector3d(178, 180, 0);
            //Vector3d end = new Vector3d(33, 38, 1);
            //Vector3d start = new Vector3d(9, 190, 0);
            //Vector3d end = new Vector3d(131, 19, 1);
            Vector3d start = new Vector3d(100, 100, 0);
            Vector3d end = new Vector3d(104.5, 105.5, 1);

            //HeightField heightfield = new HeightField(200, 200);
            //Vector3d start = new Vector3d(100, 100.1, 0);
            //Vector3d end = new Vector3d(102.1, 102.2, 1);

            int iterations = 100000;
            MeasureIntersectionPerformance(heightfield, start, end, iterations);
        }

        private static void TryIntersectionPerformance()
        {
            List<FloatMapImage> layers = new List<FloatMapImage>();
            layers.Add(((Bitmap)Bitmap.FromFile("../../data/2011-05-30_04-50-47_depth_0.png")).ToFloatMap());
            layers.Add(((Bitmap)Bitmap.FromFile("../../data/2011-05-30_04-50-47_depth_1.png")).ToFloatMap());
            layers.Add(((Bitmap)Bitmap.FromFile("../../data/2011-05-30_04-50-47_depth_2.png")).ToFloatMap());
            layers.Add(((Bitmap)Bitmap.FromFile("../../data/2011-05-30_04-50-47_depth_3.png")).ToFloatMap());
            layers.Add(((Bitmap)Bitmap.FromFile("../../data/2011-05-30_04-50-47_depth_4.png")).ToFloatMap());
            HeightField heightfield = new HeightField(layers);
            // no isec
            //Vector3d start = new Vector3d(169, 181, 0);
            //Vector3d end = new Vector3d(14, 191, 1);
            // isec at layer 4
            //Vector3d start = new Vector3d(178, 180, 0);
            //Vector3d end = new Vector3d(33, 38, 1);
            // no isec with 5 layers, traversing the filled areas
            //Vector3d start = new Vector3d(9, 190, 0);
            //Vector3d end = new Vector3d(131, 19, 1);
            Vector3d start = new Vector3d(100, 100, 0);
            Vector3d end = new Vector3d(117.5, 115.5, 1);
            int iterations = 100000;
            MeasureIntersectionPerformance(heightfield, start, end, iterations);
        }

        private static void CreateNBuffersFromOneDepthMap()
        {
            var depthMap = ((Bitmap)Bitmap.FromFile("../../data/2011-05-30_04-50-47_depth_0.png")).ToFloatMap();
            Stopwatch sw = Stopwatch.StartNew();
            NeighborhoodBuffer nbuffer = new NeighborhoodBuffer(depthMap);
            sw.Stop();
            Console.WriteLine("N-Buffers created in {0} ms", sw.ElapsedMilliseconds);
            Console.WriteLine("Size: {0}x{1}", nbuffer.Width, nbuffer.Height);
            Console.WriteLine("Number of levels: {0}", nbuffer.LevelCount);
            for (int i = 0; i < nbuffer.LevelCount; i++)
            {
                nbuffer.minLevels[i].ToBitmap().Save(String.Format("nbuffer_min_{0}.png", i));
                nbuffer.maxLevels[i].ToBitmap().Save(String.Format("nbuffer_max_{0}.png", i));
            }
        }
    }
}
