namespace BokehLab.Math
{
    using System;
    using OpenTK;

    public class Sampler
    {
        private Random random = new Random();

        /// <summary>
        /// Generates a point on a unit square [0; 1] x [0; 1].
        /// </summary>
        /// <returns>Random numbers from a square
        /// [0; 1] x [0; 1].</returns>
        public Vector2d GenerateRandomTuple()
        {
            return new Vector2d((float)random.NextDouble(), (float)random.NextDouble());
        }

        /// <summary>
        /// Maps points from a unit square to a unit circle.
        /// </summary>
        /// <param name="randomNumbers">Random numbers from a square
        /// [0; 1] x [0; 1].</param>
        /// <returns>Random numbers from a disk of radius 1 centered at [0; 0].
        /// </returns>
        public Vector2d UniformSampleDisk(Vector2d randomNumbers)
        {
            double radius = Math.Sqrt(randomNumbers.X);
            double theta = 2.0 * Math.PI * randomNumbers.Y;
            Vector2d diskSamples = new Vector2d(
                radius * Math.Cos(theta),
                radius * Math.Sin(theta));
            return diskSamples;
        }

        /// <summary>
        /// Maps points from a unit square to a unit circle, preserving
        /// adjacency and fractional area. This is useful for stratified
        /// sampling.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Shirley, P. & Chiu, K.: A Low Distortion Map Between Disk and
        /// Square. Journal of Graphics, GPU, and Game Tools, 1997, 2, 45-52.
        /// http://jgt.akpeters.com/papers/ShirleyChiu97/
        /// </para>
        /// <para>
        /// It maps concentric squares to concentric disks.
        /// Quadrants of the original square (diagonal slices) are:
        /// (1) east, phi = 0,  (2) north, phi = 1/2 * PI,
        /// (3) west, phi = PI, (4) south, phi = 3/2 * PI.
        /// </para>
        /// </remarks>
        /// <param name="randomNumbers">Random numbers from a square
        /// [0; 1] x [0; 1].</param>
        /// <returns>Random numbers from a disk of radius 1 centered at [0; 0].
        /// </returns>
        public Vector2d ConcentricSampleDisk(Vector2d randomNumbers)
        {
            double phi;
            double r;

            // convert from [0; 1]^2 to [-1; 1]^2
            double a = 2 * randomNumbers.X - 1;
            double b = 2 * randomNumbers.Y - 1;

            if (a < -b)
            {
                // region 1 or 2
                if (a > b)
                {
                    // region 1, |a| > |b|
                    r = a;
                    phi = 0.25 * Math.PI * (b / a);
                }
                else
                {
                    // region 2, |b| > |a|
                    r = b;
                    phi = 0.25 * Math.PI * (2 - (a / b));
                }
            }
            else
            {
                // region 3 or 4
                if (a < b)
                {
                    // region 3, |a| >= |b|, a != 0
                    r = -a;
                    phi = 0.25 * Math.PI * (4 + (b / a));
                }
                else
                {
                    // region 4, |b| >= |a|, (a == 0) && (b == 0) is possible
                    r = -b;
                    if (b != 0)
                    {
                        phi = 0.25 * Math.PI * (6 - (a / b));
                    }
                    else
                    {
                        phi = 0;
                    }
                }
            }
            // transform back from polar coordinates
            var diskSamples = new Vector2d(
                r * Math.Cos(phi),
                r * Math.Sin(phi));
            return diskSamples;
        }
    }
}
