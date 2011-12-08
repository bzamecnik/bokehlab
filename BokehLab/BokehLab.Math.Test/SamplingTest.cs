namespace BokehLab.Math.Test
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Text;
    using BokehLab.Math;
    using OpenTK;
    using Xunit;

    class SamplingTest
    {
        private static readonly int SAMPLE_COUNT = 4 * 1024;

        [Fact]
        public void UniformSample()
        {
            SampleAllMappings(GetUniformSampler(), SAMPLE_COUNT, "uniform");
        }

        [Fact]
        public void JitteredSample()
        {
            SampleAllMappings(GetJitteredSampler(), (int)Math.Sqrt(SAMPLE_COUNT), "jittered");
        }

        [Fact]
        public void SemiJitteredSample()
        {
            SampleAllMappings(GetSemiJitteredSampler(), (int)Math.Sqrt(SAMPLE_COUNT), "semi-jittered");
        }

        [Fact]
        public void RegularSample()
        {
            SampleAllMappings(GetRegularSampler(), (int)Math.Sqrt(SAMPLE_COUNT), "regular");
        }

        private void SampleAllMappings(Func<int, IEnumerable<Vector2d>> samplingFunc, int sampleCount, string samplerDesc)
        {
            SampleSquare(samplingFunc, sampleCount, samplerDesc);
            SampleDiskPolar(samplingFunc, sampleCount, samplerDesc);
            SampleDiskConcentric(samplingFunc, sampleCount, samplerDesc);
        }

        private void SampleSquare(Func<int, IEnumerable<Vector2d>> samplingFunc, int sampleCount, string samplerDesc)
        {
            Sample(samplingFunc, GetSquareMapping(),
                sampleCount, samplerDesc, "square");
        }

        private void SampleDiskPolar(Func<int, IEnumerable<Vector2d>> samplingFunc, int sampleCount, string samplerDesc)
        {
            Sample(samplingFunc, GetPolarDiskMapping(),
                sampleCount, samplerDesc, "disk_polar");
        }

        private void SampleDiskConcentric(Func<int, IEnumerable<Vector2d>> samplingFunc, int sampleCount, string samplerDesc)
        {
            Sample(samplingFunc, GetConcentricDiskMapping(),
                sampleCount, samplerDesc, "disk_concentric");
        }

        private void Sample(
            Func<int, IEnumerable<Vector2d>> samplingFunc,
            Func<Vector2d, Vector2d> mappingFunc,
            int sampleCount,
            string samplerDescription,
            string mappingDescription)
        {
            int width = 256;
            Bitmap image = new Bitmap(width, width);

            using (Graphics g = Graphics.FromImage(image))
            {
                g.FillRectangle(Brushes.Black, 0, 0, width - 1, width - 1);
            }

            int totalSampleCount = 0;
            foreach (Vector2d squarePoint in samplingFunc(sampleCount))
            {
                Vector2d pos = width * mappingFunc(squarePoint);
                image.SetPixel((int)pos.X, (int)pos.Y, Color.FromArgb(
                    (int)(squarePoint.X * 255),
                    (int)(squarePoint.Y * 255),
                    (int)((2 - squarePoint.X - squarePoint.Y) * 0.5 * 255)));
                totalSampleCount++;
            }

            image.Save(string.Format("{0}_{1}_{2}_samples.png",
                samplerDescription, mappingDescription, totalSampleCount));
        }

        private static Func<int, IEnumerable<Vector2d>> GetUniformSampler()
        {
            Sampler sampler = new Sampler();
            return (sampleCount) => sampler.GenerateUniformPoints(sampleCount);
        }

        private static Func<int, IEnumerable<Vector2d>> GetJitteredSampler()
        {
            Sampler sampler = new Sampler();
            return (sampleCount) => sampler.GenerateJitteredSamples(sampleCount);
        }

        private static Func<int, IEnumerable<Vector2d>> GetSemiJitteredSampler()
        {
            Sampler sampler = new Sampler();
            return (sampleCount) => sampler.GenerateSemiJitteredSamples(sampleCount, 0.5);
        }

        private static Func<int, IEnumerable<Vector2d>> GetRegularSampler()
        {
            Sampler sampler = new Sampler();
            return (sampleCount) => sampler.GenerateSemiJitteredSamples(sampleCount, 0.0);
        }

        private static Func<Vector2d, Vector2d> GetSquareMapping()
        {
            return (squarePoint) => squarePoint;
        }

        private static Func<Vector2d, Vector2d> GetPolarDiskMapping()
        {
            return (squarePoint) => 0.5 * (Vector2d.One + Sampler.PolarSampleDisk(squarePoint));
        }

        private static Func<Vector2d, Vector2d> GetConcentricDiskMapping()
        {
            return (squarePoint) => 0.5 * (Vector2d.One + Sampler.ConcentricSampleDisk(squarePoint));
        }
    }
}
