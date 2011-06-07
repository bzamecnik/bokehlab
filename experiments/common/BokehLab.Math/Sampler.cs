namespace BokehLab.Math
{
    using System;
    using System.Collections.Generic;
    using OpenTK;

    public class Sampler
    {
        private Random random = new Random();

        /// <summary>
        /// Generates a point on a unit square [0; 1] x [0; 1].
        /// </summary>
        /// <returns>Random numbers from a square
        /// [0; 1] x [0; 1].</returns>
        public Vector2d GenerateUniformPoint()
        {
            return new Vector2d(random.NextDouble(), random.NextDouble());
        }

        public IEnumerable<Vector2d> GenerateUniformPoints(int sampleCount)
        {
            for (int i = 0; i < sampleCount; i++)
            {
                yield return GenerateUniformPoint();
            }
        }

        /// <summary>
        /// Maps points from a unit square to a unit circle.
        /// </summary>
        /// <param name="randomNumbers">Random numbers from a square
        /// [0; 1] x [0; 1].</param>
        /// <returns>Random numbers from a disk of radius 1 centered at [0; 0].
        /// </returns>
        public static Vector2d PolarSampleDisk(Vector2d randomNumbers)
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
        public static Vector2d ConcentricSampleDisk(Vector2d randomNumbers)
        {
            // convert from [0; 1]^2 to [-1; 1]^2
            double a = 2 * randomNumbers.X - 1;
            double b = 2 * randomNumbers.Y - 1;

            double phi;
            double r;
            if (a > -b)
            {
                // region 1 or 2
                if (a > b)
                {
                    // region 1, |a| > |b|
                    r = a;
                    phi = b / a;
                }
                else
                {
                    // region 2, |b| > |a|
                    r = b;
                    phi = 2 - (a / b);
                }
            }
            else
            {
                // region 3 or 4
                if (a < b)
                {
                    // region 3, |a| >= |b|, a != 0
                    r = -a;
                    phi = 4 + (b / a);
                }
                else
                {
                    // region 4, |b| >= |a|, (a == 0) && (b == 0) is possible
                    r = -b;
                    if (b != 0)
                    {
                        phi = 6 - (a / b);
                    }
                    else
                    {
                        phi = 0;
                    }
                }
            }
            phi *= 0.25 * Math.PI;
            // transform back from polar coordinates
            var diskSamples = new Vector2d(
                r * Math.Cos(phi),
                r * Math.Sin(phi));
            return diskSamples;
        }

        /// <summary>
        /// Generates a uniform random sample point on a unit hemisphere with
        /// its base at the XY plane and normal pointing to Z direction.
        /// </summary>
        /// <param name="randomNumbers">Random numbers from a square
        /// [0; 1] x [0; 1].</param>
        /// <returns>Point on the hemisphere.</returns>
        public static Vector3d UniformSampleHemisphere(Vector2d randomNumbers)
        {
            double sinTheta = randomNumbers.X;
            double cosTheta = Math.Sqrt(1 - sinTheta * sinTheta);
            double phi = 2 * Math.PI * randomNumbers.Y;
            Vector3d hemisphereSample = new Vector3d(
                Math.Cos(phi) * cosTheta,
                Math.Sin(phi) * cosTheta,
                sinTheta);
            return hemisphereSample;
        }

        /// <summary>
        /// Generates a uniform random sample point on a unit sphere or its
        /// strip (truncated from poles no Z axis).
        /// </summary>
        /// <remarks>
        /// <para>
        /// (minSinTheta, maxSinTheta):
        /// (-1,1) - full sphere
        /// (-1,0) - lower hemisphere
        /// (0,1)  - upper hemisphere
        /// (sin(theta),1) - upper spherical cap
        /// 
        /// Theta is the elevation angle (measured from the XY plane).
        /// -PI/2 = lower pole (-Z),
        /// 0     = XY plane,
        /// PI/2  = upper pole (Z)
        /// </para>
        /// <para>
        /// This mapping is based on Achimedes' Theorem. See
        /// Shao, Badler: Spherical Sampling by Archimedes' Theorem [shao1996]
        /// http://repository.upenn.edu/cgi/viewcontent.cgi?article=1188&context=cis_reports
        /// </para>
        /// <para>
        /// The mapping preserves uniform differential areas. It means the
        /// samples are distributed less dense near poles. Thus it might not
        /// be well suitable for LRTF sampling as the most important are the
        /// areas around the poles. However, this is suitable to be used in
        /// conjunction with stratified sampling, eg. later while evaluating the
        /// LRTF.
        /// </para>
        /// </remarks>
        /// <param name="randomNumbers">Random numbers from a square
        /// [0; 1] x [0; 1].</param>
        /// <param name="minSinTheta">Sine of minimum elevation angle.</param>
        /// <param name="maxSinTheta">Sine of maximum elevation angle.</param>
        /// <returns>Point on the sphere.</returns>
        public static Vector3d UniformSampleSphereWithEqualArea(
            Vector2d randomNumbers,
            double minSinTheta,
            double maxSinTheta)
        {
            minSinTheta = Math.Max(minSinTheta, -1);
            maxSinTheta = Math.Min(maxSinTheta, 1);
            double sinTheta = minSinTheta + (maxSinTheta - minSinTheta) * randomNumbers.X;
            double cosTheta = Math.Sqrt(1 - sinTheta * sinTheta);
            double phi = 2 * Math.PI * randomNumbers.Y;
            Vector3d sphereSample = new Vector3d(
                Math.Cos(phi) * cosTheta,
                Math.Sin(phi) * cosTheta,
                sinTheta);
            return sphereSample;
        }

        public static Vector3d SampleSphereWithUniformSpacing(
            Vector2d randomNumbers,
            double minSinTheta,
            double maxSinTheta)
        {
            double theta = 0.5 * Math.PI * randomNumbers.X;
            minSinTheta = Math.Max(minSinTheta, -1);
            maxSinTheta = Math.Min(maxSinTheta, 1);
            double sinTheta = Math.Sin(theta);
            sinTheta = minSinTheta + (maxSinTheta - minSinTheta) * sinTheta;
            //double cosTheta = Math.Cos(angle);
            double cosTheta = Math.Sqrt(1 - sinTheta * sinTheta);
            double phi = 2 * Math.PI * randomNumbers.Y;
            Vector3d sphereSample = new Vector3d(
                Math.Cos(phi) * cosTheta,
                Math.Sin(phi) * cosTheta,
                sinTheta);
            return sphereSample;
        }

        /// <summary>
        /// Convert a 3D point on a hemispherical cap surface to its
        /// parametric representation. Inverse operation to
        /// SampleSphereWithUniformSpacing().
        /// </summary>
        /// <param name="point">3D point in cartesian camera space, assumed to
        /// be on the sphere surface.</param>
        /// <param name="minSinTheta"></param>
        /// <param name="maxSinTheta"></param>
        /// <returns></returns>
        public static Vector2d SampleSphereWithUniformSpacingInverse(
            Vector3d point,
            double minSinTheta,
            double maxSinTheta)
        {
            Vector2d parameters = new Vector2d();

            double cosTheta = Math.Sqrt(1 - point.Z * point.Z);

            minSinTheta = Math.Max(minSinTheta, -1);
            maxSinTheta = Math.Min(maxSinTheta, 1);
            double sinTheta = (point.Z - minSinTheta) / (maxSinTheta - minSinTheta);
            double theta = Math.Asin(sinTheta);
            parameters.X = theta / (0.5 * Math.PI);

            double phi = Math.Acos(point.X / cosTheta);
            parameters.Y = phi / (2 * Math.PI);

            return parameters;
        }

        /// <summary>
        /// Generate a NxN matrix of point samples inside the [0; 1] x [0; 1]
        /// interval using the stratified sampling method with semi-jittering.
        /// </summary>
        /// <remarks>
        /// The interval is divided into NxN square blocks and one sample is
        /// generated inside each block. The semiJittering parameter controls
        /// the amplitude of semi-jittering in each block from full jittering
        /// to regular sampling (where only block centers are chosen).
        /// </remarks>
        /// <param name="sampleCount">number of samples in one direction (N);
        /// in total NxN samples will be generated</param>
        /// <param name="semiJittering">amount of semi-jittering; from
        /// [0.0; 1.0] 0.0 - no jittering, 1.0 - full jittering</param>
        /// <param name="samples">pre-allocated do size NxN or null for
        /// being created here</param>
        /// <returns></returns>
        public IEnumerable<Vector2d> GenerateSemiJitteredSamples(int sampleCount, double semiJittering)
        {
            int totalSampleCount = sampleCount * sampleCount;

            double step = 1.0 / sampleCount;
            double amplitude = semiJittering * step;
            double origin = 0.5 * (step - amplitude);

            double y = origin; // sample block origin positions
            for (int j = 0; j < sampleCount; j++)
            {
                double x = origin;
                for (int i = 0; i < sampleCount; i++)
                {
                    Vector2d sample = new Vector2d(
                        x + random.NextDouble() * amplitude,
                        y + random.NextDouble() * amplitude);
                    yield return sample;
                    x += step;
                }
                y += step;
            }
        }

        /// <summary>
        /// Generate a NxN matrix of point samples inside the [0; 1] x [0; 1]
        /// square using the stratified sampling method with full jittering.
        /// </summary>
        /// <param name="sampleCount"></param>
        /// <returns></returns>
        public IEnumerable<Vector2d> GenerateJitteredSamples(int sampleCount)
        {
            int totalSampleCount = sampleCount * sampleCount;
            double step = 1.0 / sampleCount;
            double y = 0;
            for (int j = 0; j < sampleCount; j++)
            {
                double x = 0;
                for (int i = 0; i < sampleCount; i++)
                {
                    Vector2d sample = new Vector2d(
                        x + random.NextDouble() * step,
                        y + random.NextDouble() * step);
                    yield return sample;
                    x += step;
                }
                y += step;
            }
        }

    }
}
