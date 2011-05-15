using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;

namespace BokehLab.Lens
{
    public class Sampler
    {
        private Random random = new Random();

        public Vector2d GenerateRandomTuple()
        {
            return new Vector2d((float)random.NextDouble(), (float)random.NextDouble());
        }

        public Vector2d UniformSampleDisk(Vector2d randomNumbers)
        {
            double radius = Math.Sqrt(randomNumbers.X);
            double theta = 2.0 * Math.PI * randomNumbers.Y;
            Vector2d diskSamples = new Vector2d();
            diskSamples.X = (float)(radius * Math.Cos(theta));
            diskSamples.Y = (float)(radius * Math.Sin(theta));
            return diskSamples;
        }
    }
}
