namespace BokehLab.LrtfSampler
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using BokehLab.RayTracing.Lens;

    class Program
    {
        static void Main(string[] args)
        {
            //ComplexLens lens = ComplexLens.CreateDoubleGaussLens(Materials.Fixed.AIR, 4.0);
            //string lensName = "double_gauss";

            ComplexLens lens = ComplexLens.CreatePetzvalLens(Materials.Fixed.AIR, 4.0);
            string lensName = "petzval";

            //ComplexLens lens = ComplexLens.CreateBiconvexLens(150, 100, 0);
            //string lensName = "biconvex";

            LensRayTransferFunction lrtf = new LensRayTransferFunction(lens);

            int sampleCount = 128;

            Stopwatch stopwatch = Stopwatch.StartNew();

            var table = lrtf.SampleLrtf3D(sampleCount);

            stopwatch.Stop();

            //for (int i = 0; i < sampleCount; i++)
            //{
            //    for (int j = 0; j < sampleCount; j++)
            //    {
            //        for (int k = 0; k < sampleCount; k++)
            //        {
            //            var value = new LensRayTransferFunction.Parameters(table.Table[i, j, k]);
            //            Console.WriteLine((value != null) ? value.ToString() : "null");
            //        }
            //        Console.WriteLine();
            //    }
            //}

            Console.WriteLine("Size: {0}x{0}x{0}, elapsed time: {1} ms", sampleCount, stopwatch.ElapsedMilliseconds);

            string filename = string.Format(@"..\..\..\lrtf_{0}_{1}.bin", lensName, sampleCount);

            stopwatch.Reset();
            stopwatch.Start();
            table.Save(filename);
            stopwatch.Stop();

            Console.WriteLine("Saved sampled LRTF into file: {0}, elapsed time: {1} ms", filename, stopwatch.ElapsedMilliseconds);
        }
    }
}
