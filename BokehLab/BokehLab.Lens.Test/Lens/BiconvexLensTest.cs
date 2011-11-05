namespace BokehLab.RayTracing.Test.Lens
{
    using System;
    using BokehLab.Math;
    using BokehLab.RayTracing.Lens;
    using OpenTK;
    using Xunit;

    class BiconvexLensTest
    {
        [Fact]
        public void TraceCenteredRay()
        {
            BiconvexLens lens = new BiconvexLens();
            lens.ApertureRadius = 2;
            lens.CurvatureRadius = 2.5;

            Vector3d objectPos = new Vector3d(0, 0, 10);
            Vector3d lensPos = lens.GetBackSurfaceSample(new Vector2d(1, 1));
            Ray result = lens.Transfer(objectPos, lensPos);

            Assert.NotNull(result);
            Assert.Equal(new Vector3d(0, 0, -1), Vector3d.Normalize(result.Direction));
            Assert.Equal(-lensPos, result.Origin);
        }

        [Fact]
        public void TraceParallelRays()
        {
            BiconvexLens lens = new BiconvexLens();
            lens.ApertureRadius = 2;
            lens.CurvatureRadius = 4;

            Sampler sampler = new Sampler();
            int sampleCount = 64;
            int sqrtSampleCount = (int)Math.Sqrt(sampleCount);
            Vector3d objectPos = new Vector3d(10, 0, 100);
            foreach (Vector2d sample in sampler.GenerateJitteredSamples(sqrtSampleCount))
            {
                Vector3d lensPos = lens.GetBackSurfaceSample(sample);
                //Vector3d objectPos = lensPos + 10 * Vector3d.UnitZ + 2 * Vector3d.UnitX;
                Ray result = lens.Transfer(objectPos, lensPos);
            }
        }
    }
}
