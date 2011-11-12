namespace BokehLab.InteractiveDof.MultiViewAccum
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using BokehLab.Math;
    using OpenTK;
    using OpenTK.Graphics.OpenGL;
    using System.Diagnostics;

    /// <summary>
    /// Many pinhole views are rasterized and accumulated and averaged to
    /// estimate the light transport through a lens with a finite aperture.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This method is based on [haeberli1990] and is extended to incremental
    /// rendering (the moving average is displayed during rendering). Original
    /// accumulation buffer can be used, however a modern variant is to use
    /// FBO, render to textures and to the averaging in a shader. FBO textures
    /// support floating point pixel formats, in contrast to the accumulation
    /// buffer. This prevents artifact even for a thousand of accumulated views.
    /// </para>
    /// <para>
    /// [haeberli1990] Haeberli, P. & Akeley, K.: The accumulation buffer:
    /// hardware support for high-quality rendering, 1999.
    /// </para>
    /// </remarks>
    class MultiViewAccumulation : IncrementalRenderer
    {
        static readonly int sqrtSampleCount = 32;

        Vector2[] unitDiskSamples;
        Sampler sampler = new Sampler();

        public MultiViewAccumulation()
            : base(sqrtSampleCount * sqrtSampleCount)
        {
            // TODO: support creating samples for a cropped aperture (hexagon etc.)
            unitDiskSamples = CreateLensSamples(sqrtSampleCount).ToArray();
        }

        protected override void DrawSingleFrame(int iteration, Scene scene, Navigation navigation)
        {
            Vector2 localPinholePos = navigation.Camera.GetPinholePos(unitDiskSamples[iteration]);
            Matrix4 perspective = navigation.Camera.GetMultiViewPerspective(localPinholePos);

            GL.MatrixMode(MatrixMode.Projection);
            GL.PushMatrix();
            GL.LoadMatrix(ref perspective);

            GL.Translate(-localPinholePos.X, -localPinholePos.Y, 0);

            scene.Draw();

            GL.PopMatrix();
        }

        /// <summary>
        /// Generate a set of jittered uniform samples of a unit circle.
        /// </summary>
        /// <returns></returns>
        private IEnumerable<Vector2> CreateLensSamples(int sampleCount)
        {
            var jitteredSamples = sampler.GenerateJitteredSamples(sampleCount);
            var diskSamples = jitteredSamples.Select((sample) => (Vector2)Sampler.ConcentricSampleDisk(sample));
            var diskSamplesList = diskSamples.ToList();
            // shuffle the samples to prevent temporal correlation
            // in incremental rendering
            Shuffle<Vector2>(diskSamplesList);
            return diskSamplesList;
        }

        //http://stackoverflow.com/questions/273313/randomize-a-listt-in-c-sharp
        public static void Shuffle<T>(IList<T> list)
        {
            Random rng = new Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}
