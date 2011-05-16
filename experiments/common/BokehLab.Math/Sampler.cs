﻿namespace BokehLab.Math
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
            double phi;
            double r;

            // convert from [0; 1]^2 to [-1; 1]^2
            double a = 2 * randomNumbers.X - 1;
            double b = 2 * randomNumbers.Y - 1;

            if (a > -b)
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