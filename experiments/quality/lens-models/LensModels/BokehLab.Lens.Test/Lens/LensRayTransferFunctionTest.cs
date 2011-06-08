﻿namespace BokehLab.RayTracing.Lens.Test
{
    using System;
    using System.Collections.Generic;
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
            var table = lrtf.SampleLrtf(defaultParameters,
                LensRayTransferFunction.VariableParameter.DirectionTheta, 101);
            int i = 0;
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
                    Assert.InRange(outgoingParams.DirectionTheta, 0.0, 1.0);
                    Assert.InRange(outgoingParams.DirectionPhi, 0.0, 1.0);
                    Assert.InRange(outgoingParams.PositionTheta, 0.0, 1.0);
                    Assert.InRange(outgoingParams.PositionPhi, 0.0, 1.0);
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
    }
}