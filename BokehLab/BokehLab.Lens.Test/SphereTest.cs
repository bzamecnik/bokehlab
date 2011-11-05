namespace BokehLab.RayTracing.Test.Lens
{

    using System;
    using BokehLab.Math;
    using OpenTK;
    using Xunit;

    class SphereTest
    {
        [Fact]
        public void IntersectSphere()
        {
            Sphere sphere = new Sphere()
            {
                Radius = 2
            };
            double biggerSphereFactor = 3;

            Sampler sampler = new Sampler();
            int sampleCount = 64;
            int sqrtSampleCount = (int)Math.Sqrt(sampleCount);
            foreach (Vector2d sample in sampler.GenerateJitteredSamples(sqrtSampleCount))
            {
                // shoot rays at the sphere center from a bigger concontric sphere
                Vector3d unitSphereSample = Sampler.UniformSampleSphereWithEqualArea(sample, -1, 1);
                Vector3d sourcePos = biggerSphereFactor * sphere.Radius * unitSphereSample;
                Ray ray = new Ray(sourcePos, sphere.Center - sourcePos);
                Intersection intersection = sphere.Intersect(ray);
                Assert.NotNull(intersection);
                Vector3d intPos = intersection.Position;
                Console.WriteLine("Black, {0},", ray.ToLine());
                Console.WriteLine(String.Format("Red, {0},", intPos.ToPoint()));
            }
        }
    }
}
