namespace BokehLab.RayTracing.Lens.Test
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
            var defaultParameters = new LensRayTransferFunction.Parameters(0.5, 0.5, 1.0, 0.0);
            var table = lrtf.SampleLrtf(defaultParameters,
                LensRayTransferFunction.VariableParameter.DirectionTheta, 100);
            int i = 0;
            foreach (LensRayTransferFunction.Parameters rayParams in table)
            {
                Console.WriteLine("[{0}]: {1}", i, rayParams);
                if (rayParams != null)
                {
                    Console.WriteLine("  {0}", lens.ConvertParametersToFrontSurfaceRay(rayParams));
                }
                i++;
            }
        }

        [Fact]
        public void ConvertParametersToBackSurfaceRayAndBack()
        {
            ComplexLens lens = ComplexLens.CreateDoubleGaussLens(Materials.Fixed.AIR, 4.0);
            var origParameters = new LensRayTransferFunction.Parameters(0.5, 0.0, 1.0, 0.0);
            Ray incomingRay = lens.ConvertParametersToBackSurfaceRay(origParameters);
            var parameters = lens.ConvertBackSurfaceRayToParameters(incomingRay);
            Assert.Equal(origParameters.PositionTheta, parameters.PositionTheta, 5);
            Assert.Equal(origParameters.PositionPhi, parameters.PositionPhi, 5);
            Assert.Equal(origParameters.DirectionTheta, parameters.DirectionTheta, 5);
            Assert.Equal(origParameters.DirectionPhi, parameters.DirectionPhi, 5);
        }

        [Fact]
        public void ConvertParametersToFrontSurfaceRayAndBack()
        {
            ComplexLens lens = ComplexLens.CreateDoubleGaussLens(Materials.Fixed.AIR, 4.0);
            var origParameters = new LensRayTransferFunction.Parameters(0.5, 0.0, 1.0, 0.0);
            Ray incomingRay = lens.ConvertParametersToFrontSurfaceRay(origParameters);
            var parameters = lens.ConvertFrontSurfaceRayToParameters(incomingRay);
            Assert.Equal(origParameters.PositionTheta, parameters.PositionTheta, 5);
            Assert.Equal(origParameters.PositionPhi, parameters.PositionPhi, 5);
            Assert.Equal(origParameters.DirectionTheta, parameters.DirectionTheta, 5);
            Assert.Equal(origParameters.DirectionPhi, parameters.DirectionPhi, 5);
        }
    }
}
