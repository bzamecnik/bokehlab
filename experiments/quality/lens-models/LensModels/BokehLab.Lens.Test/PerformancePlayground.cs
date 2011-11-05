using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BokehLab.Math;
using System.Diagnostics;
using BokehLab.RayTracing;
using BokehLab.RayTracing.Lens;
using OpenTK;

namespace BokehLab.RayTracing.Test
{
    class PerformancePlayground
    {
        public void GenerateJitteredSamples()
        {
            Stopwatch sw = Stopwatch.StartNew();

            Sampler sampler = new Sampler();
            //foreach (var sample in sampler.GenerateJitteredSamples(512)) ;
            foreach (var sample in sampler.GenerateUniformPoints(256 * 1024)) ;

            sw.Stop();
            Console.WriteLine("Total time: {0}", sw.ElapsedMilliseconds);
            Console.WriteLine("Thoughput: {0} smpl/sec", 512 * 512 * (1000 / (float)sw.ElapsedMilliseconds));
        }

        public void LensTransform()
        {
            //ThinLens lens = new ThinLens(10, 1);
            PinholeLens lens = new PinholeLens();
            //ComplexLens lens = ComplexLens.CreatePetzvalLens(Materials.Fixed.AIR, 4.0);
            //ComplexLens lens = ComplexLens.CreateDoubleGaussLens(Materials.Fixed.AIR, 4.0);
            //ComplexLens lens = ComplexLens.CreateBiconvexLens(150, 100, 0);
            var sensorPos = new Vector3d(2.5, 3.5, 15.7);
            var lensPos = new Vector3d(0.5, 0.25, 0);

            //int samples = 10 * 1024 * 1024;
            int samples = 1000 * 1000;
            Stopwatch sw = Stopwatch.StartNew();
            for (int i = 0; i < samples; i++)
            {
                lens.Transfer(sensorPos, lensPos);
            }
            sw.Stop();

            Console.WriteLine("Total time: {0}", sw.ElapsedMilliseconds);
            Console.WriteLine("Thoughput: {0} smpl/sec", samples * (1000 / (float)sw.ElapsedMilliseconds));
        }
    }
}
