namespace BokehLab.RayTracing.Test.Lens
{
    using System;
    using System.Collections.Generic;
    using BokehLab.Math;
    using BokehLab.RayTracing.Lens;
    using OpenTK;
    using Xunit;

    class ComplexLensTest
    {
        [Fact]
        public void TraceSingleRay()
        {
            ComplexLens lens = ComplexLens.CreateBiconvexLens(4, 2);

            Sampler sampler = new Sampler();
            int sampleCount = 64;
            int sqrtSampleCount = (int)Math.Sqrt(sampleCount);
            Vector3d objectPos = new Vector3d(10, 0, 100);
            Vector3d direction = new Vector3d(0, 0, 0);
            Ray ray = new Ray(objectPos, direction);
            Intersection isec = lens.Intersect(ray);
            if (isec == null)
            {
                return;
            }
            Ray result = lens.Transfer(objectPos, isec.Position);
        }

        [Fact]
        public void TraceRays()
        {
            ComplexLens lens = ComplexLens.CreateBiconvexLens(4, 2);

            Sampler sampler = new Sampler();
            int sampleCount = 64;
            int sqrtSampleCount = (int)Math.Sqrt(sampleCount);
            Vector3d objectPos = new Vector3d(10, 0, 100);
            foreach (Vector2d sample in sampler.GenerateJitteredSamples(sqrtSampleCount))
            {
                Vector3d lensPos = lens.GetBackSurfaceSample(sample);
                Ray result = lens.Transfer(objectPos, lensPos);
            }
        }


    }
}
