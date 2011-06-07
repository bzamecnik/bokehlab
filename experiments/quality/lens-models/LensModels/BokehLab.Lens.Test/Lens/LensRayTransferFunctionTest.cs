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
            var table = lrtf.SampleLrtf(new Vector2d(0.5, 0.0), new Vector2d(1.0, 0.0),
                LensRayTransferFunction.VariableParameter.DirectionPhi, 20);
            foreach (Ray ray in table)
            {
                Console.WriteLine(ray);
            }
        }
    }
}
