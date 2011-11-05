namespace BokehLab.RayTracing.Lens.Test
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using BokehLab.Math;
    using BokehLab.RayTracing.Lens;
    using OpenTK;
    using Xunit;

    class LensRayTransferFunctionTest
    {
        [Fact]
        public void SampleLrtf()
        {
            ComplexLens lens = ComplexLens.CreateDoubleGaussLens(Materials.Fixed.AIR, 4.0);
            LensRayTransferFunction lrtf = new LensRayTransferFunction(lens);
            var defaultParameters = new LensRayTransferFunction.Parameters(0.5, 0.5, 1.0, 0.5);
            var table = lrtf.SampleLrtf1D(defaultParameters,
                LensRayTransferFunction.VariableParameter.DirectionTheta, 101);
            //int i = 0;
            //foreach (LensRayTransferFunction.Parameters rayParams in table)
            //{
            //    Console.WriteLine("[{0}]: {1}", i, rayParams);
            //    if (rayParams != null)
            //    {
            //        //Console.WriteLine("  {0}", lens.ConvertParametersToFrontSurfaceRay(rayParams));
            //    }
            //    i++;
            //}
            Console.WriteLine("{{ {0} }}", string.Join(",\n", table.Select((item) => (item != null) ? item.ToString() : "Null").ToArray()));
        }

        [Fact]
        public void ComputeLrtf()
        {
            ComplexLens lens = ComplexLens.CreateDoubleGaussLens(Materials.Fixed.AIR, 4.0);
            LensRayTransferFunction lrtf = new LensRayTransferFunction(lens);
            var incomingParams = new LensRayTransferFunction.Parameters(0.5, 0.5, 0.7000000000000004, 0.0);
            LensRayTransferFunction.Parameters outgoingParams = lrtf.ComputeLrtf(incomingParams);
            Console.WriteLine("IN: {0}", incomingParams);
            Console.WriteLine("OUT: {0}", outgoingParams);
            if (outgoingParams != null)
            {
                Console.WriteLine("  {0}", lens.ConvertParametersToFrontSurfaceRay(outgoingParams));
            }
        }

        [Fact]
        public void ComputeLrtfForRandomInput()
        {
            ComplexLens lens = ComplexLens.CreateDoubleGaussLens(Materials.Fixed.AIR, 4.0);
            LensRayTransferFunction lrtf = new LensRayTransferFunction(lens);
            Random random = new Random();
            for (int i = 0; i < 1000; i++)
            {
                var incomingParams = new LensRayTransferFunction.Parameters(
                    random.NextDouble(), random.NextDouble(),
                    random.NextDouble(), random.NextDouble()
                    );
                LensRayTransferFunction.Parameters outgoingParams = lrtf.ComputeLrtf(incomingParams);
                if (outgoingParams != null)
                {
                    if (outgoingParams.DirectionTheta < 0 || outgoingParams.DirectionTheta > 1 ||
                        outgoingParams.DirectionPhi < 0 || outgoingParams.DirectionPhi > 1 ||
                        outgoingParams.PositionTheta < 0 || outgoingParams.PositionTheta > 1 ||
                        outgoingParams.PositionPhi < 0 || outgoingParams.PositionPhi > 1)
                    {
                        Console.WriteLine("Warning: parameter outside [0; 1] interval.");
                        Console.WriteLine("incoming: {0}", incomingParams);
                        Console.WriteLine("outgoing: {0}", outgoingParams);
                        Console.WriteLine();
                    }
                    //Assert.InRange(outgoingParams.DirectionTheta, 0.0, 1.0);
                    //Assert.InRange(outgoingParams.DirectionPhi, 0.0, 1.0);
                    //Assert.InRange(outgoingParams.PositionTheta, 0.0, 1.0);
                    //Assert.InRange(outgoingParams.PositionPhi, 0.0, 1.0);
                }
            }
        }

        [Fact]
        public void ConvertSomeParametersToBackSurfaceRayAndBack()
        {
            ConvertParametersToBackSurfaceRayAndBack(
                new LensRayTransferFunction.Parameters(
                    0.00223778841606689, 0.5, -0.811103608898064, 0.5));
            //ConvertParametersToBackSurfaceRayAndBack(
            //    new LensRayTransferFunction.Parameters(0.5, 0.0, 1.0, 0.0));
        }

        private static void ConvertParametersToBackSurfaceRayAndBack(
            LensRayTransferFunction.Parameters inputParams)
        {
            ConvertParametersToRayAndBack(inputParams, (parameters, lens) =>
                lens.ConvertBackSurfaceRayToParameters(lens.ConvertParametersToBackSurfaceRay(parameters))
            );
        }

        [Fact]
        public void ConvertSomeParametersToFrontSurfaceRayAndBack()
        {
            ConvertParametersToFrontSurfaceRayAndBack(
                new LensRayTransferFunction.Parameters(
                    0.00223778841606689, 0.5, -0.811103608898064, 0.5));
            //ConvertParametersToBackSurfaceRayAndBack(
            //    new LensRayTransferFunction.Parameters(0.5, 0.0, 1.0, 0.0));
        }

        private static void ConvertParametersToFrontSurfaceRayAndBack(
            LensRayTransferFunction.Parameters inputParams)
        {
            ConvertParametersToRayAndBack(inputParams, (parameters, lens) =>
                lens.ConvertFrontSurfaceRayToParameters(lens.ConvertParametersToFrontSurfaceRay(parameters))
            );
        }

        private static void ConvertParametersToRayAndBack(
            LensRayTransferFunction.Parameters inputParams,
            Func<LensRayTransferFunction.Parameters, ComplexLens, LensRayTransferFunction.Parameters> func)
        {
            ComplexLens lens = ComplexLens.CreateDoubleGaussLens(Materials.Fixed.AIR, 4.0);
            var outputParams = func(inputParams, lens);
            Assert.Equal(inputParams.PositionTheta, outputParams.PositionTheta, 5);
            Assert.Equal(inputParams.PositionPhi, outputParams.PositionPhi, 5);
            Assert.Equal(inputParams.DirectionTheta, outputParams.DirectionTheta, 5);
            Assert.Equal(inputParams.DirectionPhi, outputParams.DirectionPhi, 5);
        }

        [Fact]
        private void ConvertToHemisphericalPositionAndBack()
        {
            Vector2d origParams = new Vector2d(0.897631192121816, 0.0200000000000735);
            double sinTheta = 0.96780557421958624;
            Vector3d pos = Sampler.SampleSphereWithUniformSpacing(origParams, sinTheta, 1);
            Vector2d recoveredParams = Sampler.SampleSphereWithUniformSpacingInverse(pos, sinTheta, 1);
            Assert.Equal(origParams.X, recoveredParams.X, 5);
            Assert.Equal(origParams.Y, recoveredParams.Y, 5);
        }

        [Fact]
        private void ConvertFromHemisphericalPositionAndBack()
        {
            double sinTheta = 0.96780557421958624;
            Vector3d origPos = new Vector3d(0.0488656908648108, -0.00932162907092131, 0.998761859247623);
            Vector2d parameters = Sampler.SampleSphereWithUniformSpacingInverse(origPos, sinTheta, 1);
            Vector3d recoveredPos = Sampler.SampleSphereWithUniformSpacing(parameters, sinTheta, 1);
            Assert.Equal(origPos.X, recoveredPos.X, 5);
            Assert.Equal(origPos.Y, recoveredPos.Y, 5);
            Assert.Equal(origPos.Z, recoveredPos.Z, 5);
        }

        [Fact]
        public void ComputeLrtfResultInCorrectInterval()
        {
            ComplexLens lens = ComplexLens.CreateDoubleGaussLens(Materials.Fixed.AIR, 4.0);
            LensRayTransferFunction lrtf = new LensRayTransferFunction(lens);
            var incomingParams = new LensRayTransferFunction.Parameters(0.835057026164167, 0.375245163857585, 0.854223355117358, 0.000161428470239708);
            LensRayTransferFunction.Parameters outgoingParams = lrtf.ComputeLrtf(incomingParams);
            Console.WriteLine(outgoingParams);
        }

        [Fact]
        public void SampleLrtfSaveLoadAndCompare()
        {
            ComplexLens lens = ComplexLens.CreateDoubleGaussLens(Materials.Fixed.AIR, 4.0);
            LensRayTransferFunction lrtf = new LensRayTransferFunction(lens);

            int sampleCount = 16;

            var table = lrtf.SampleLrtf3D(sampleCount);

            Console.WriteLine("Size: {0}x{0}x{0}", sampleCount);

            string filename = string.Format("lrtf_double_gauss_{0}.bin", sampleCount);

            table.Save(filename);

            Console.WriteLine("Saved sampled LRTF into file: {0}", filename);

            Console.WriteLine("Trying to load sampled LRTF from file and compare...");

            var recoveredTable = LensRayTransferFunction.Table3d.Load(filename);

            Assert.Equal(sampleCount, recoveredTable.Size);

            for (int i = 0; i < sampleCount; i++)
            {
                for (int j = 0; j < sampleCount; j++)
                {
                    for (int k = 0; k < sampleCount; k++)
                    {
                        Vector4d orig = table.Table[i, j, k];
                        Vector4d recovered = recoveredTable.Table[i, j, k];
                        AssertEqualVector4d(orig, recovered);
                    }
                }
            }

            Console.WriteLine("Compared OK");
        }

        [Fact]
        public void CompareInterpolatedLrtfValueWithOriginalOnes()
        {
            ComplexLens lens = ComplexLens.CreateDoubleGaussLens(Materials.Fixed.AIR, 4.0);
            LensRayTransferFunction lrtf = new LensRayTransferFunction(lens);

            int sampleCount = 128;
            string filename = string.Format(@"..\..\..\lrtf_double_gauss_{0}.bin", sampleCount);
            var table = lrtf.SampleLrtf3DCached(sampleCount, filename);

            Random random = new Random();
            for (int i = 0; i < 1000; i++)
            {
                var incomingParams = new LensRayTransferFunction.Parameters(
                    random.NextDouble(), random.NextDouble(),
                    random.NextDouble(), random.NextDouble()
                    );
                var outgoingParamsOriginal = lrtf.ComputeLrtf(incomingParams).ToVector4d();
                var outgoingParamsInterpolated = table.EvaluateLrtf3D(incomingParams).ToVector4d();
                //AssertEqualVector4d(outgoingParamsOriginal, outgoingParamsInterpolated);
            }
        }

        [Fact]
        public void CompareEvaluationTime()
        {
            ComplexLens lens = ComplexLens.CreateDoubleGaussLens(Materials.Fixed.AIR, 4.0);
            LensRayTransferFunction lrtf = new LensRayTransferFunction(lens);

            int sampleCount = 128;
            Console.WriteLine("LRTF table size: {0}x{0}x{0}", sampleCount);
            string filename = string.Format(@"..\..\..\lrtf_double_gauss_{0}.bin", sampleCount);
            var table = lrtf.SampleLrtf3DCached(sampleCount, filename);

            int valueCount = 1000000;
            Console.WriteLine("Number of values to evaluate: {0}", valueCount);

            Random random = new Random();
            var inParams = new List<LensRayTransferFunction.Parameters>();
            for (int i = 0; i < valueCount; i++)
            {
                inParams.Add(new LensRayTransferFunction.Parameters(
                    random.NextDouble(), random.NextDouble(),
                    random.NextDouble(), random.NextDouble()
                    ));
            }
            Stopwatch stopwatch = Stopwatch.StartNew();
            foreach (var inParam in inParams)
            {
                lrtf.ComputeLrtf(inParam);
            }
            stopwatch.Stop();
            Console.WriteLine("Ray tracing: {0} ms", stopwatch.ElapsedMilliseconds);
            stopwatch.Reset();
            stopwatch.Start();
            foreach (var inParam in inParams)
            {
                table.EvaluateLrtf3D(inParam);
            }
            stopwatch.Stop();
            Console.WriteLine("LRTF table interpolation: {0} ms", stopwatch.ElapsedMilliseconds);
        }

        // TODO: turn to a extension method
        private void AssertEqualVector4d(Vector4d expected, Vector4d actual)
        {
            //Assert.True((expected == null) ^ (actual == null));
            Assert.Equal(expected.X, actual.X);
            Assert.Equal(expected.Y, actual.Y);
            Assert.Equal(expected.Z, actual.Z);
            Assert.Equal(expected.W, actual.W);
        }
    }
}
