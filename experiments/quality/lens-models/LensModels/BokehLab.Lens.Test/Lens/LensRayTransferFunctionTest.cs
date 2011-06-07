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
            var table = lrtf.SampleLrtf(new Vector2d(0.5, 0.5), new Vector2d(1.0, 0.0),
                LensRayTransferFunction.VariableParameter.DirectionTheta, 100);
            int i = 0;
            foreach (LensRayTransferFunction.Parameters rayParams in table)
            {
                Console.WriteLine("[{0}]: {1}", i, rayParams);
                //Console.WriteLine("  {0}", lens.ConvertParametersToFrontSurfaceRay(rayParams));
                i++;
            }
        }

        [Fact]
        public void ConvertParametersToBackSurfaceRayAndBack()
        {
            ComplexLens lens = ComplexLens.CreateDoubleGaussLens(Materials.Fixed.AIR, 4.0);
            Vector4d origParameters = new Vector4d(0.5, 0.0, 1.0, 0.0);
            Ray incomingRay = lens.ConvertParametersToBackSurfaceRay(
                    origParameters.Xy, new Vector2d(origParameters.Z, origParameters.W));
            Vector4d parameters = lens.ConvertBackSurfaceRayToParameters(incomingRay);
            Assert.Equal(origParameters.X, parameters.X, 5);
            Assert.Equal(origParameters.Y, parameters.Y, 5);
            Assert.Equal(origParameters.Z, parameters.Z, 5);
            Assert.Equal(origParameters.W, parameters.W, 5);
        }

        [Fact]
        public void ConvertParametersToFrontSurfaceRayAndBack()
        {
            ComplexLens lens = ComplexLens.CreateDoubleGaussLens(Materials.Fixed.AIR, 4.0);
            Vector4d origParameters = new Vector4d(0.5, 0.0, 1.0, 0.0);
            Ray incomingRay = lens.ConvertParametersToFrontSurfaceRay(
                    origParameters.Xy, new Vector2d(origParameters.Z, origParameters.W));
            Vector4d parameters = lens.ConvertFrontSurfaceRayToParameters(incomingRay);
            Assert.Equal(origParameters.X, parameters.X, 5);
            Assert.Equal(origParameters.Y, parameters.Y, 5);
            Assert.Equal(origParameters.Z, parameters.Z, 5);
            Assert.Equal(origParameters.W, parameters.W, 5);
        }
    }
}
