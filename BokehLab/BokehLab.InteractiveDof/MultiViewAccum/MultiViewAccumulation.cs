namespace BokehLab.InteractiveDof.MultiViewAccum
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using BokehLab.Math;
    using OpenTK;
    using OpenTK.Graphics.OpenGL;

    class MultiViewAccumulation : AbstractRendererModule
    {
        // How many views should be accumulated during one frame rendering.
        int viewsPerFrame = 4; // 16 good for float16, 4 or 8 for float32

        int sqrtSampleCount = 32;

        int iteration = 0;
        Vector2[] unitDiskSamples;
        Sampler sampler = new Sampler();
        int maxIterations;
        IAccumulator accumulator;

        public MultiViewAccumulation()
        {
            unitDiskSamples = CreateLensSamples(sqrtSampleCount).ToArray();
            maxIterations = unitDiskSamples.Length;

            // draw all views at once
            //viewsPerFrame = maxIterations;

            //accumulator = new BufferAccumulator() { TotalIterations = maxIterations };
            accumulator = new FboAccumulator();
        }

        public override void Initialize(int width, int height)
        {
            base.Initialize(width, height);
            accumulator.Initialize(width, height);
        }

        public override void Dispose()
        {
            accumulator.Dispose();
        }

        protected override void Enable()
        {
            accumulator.Enabled = true;
        }

        protected override void Disable()
        {
            accumulator.Enabled = false;
        }

        public override void Resize(int width, int height)
        {
            base.Resize(width, height);
            accumulator.Resize(width, height);
        }

        public void AccumulateAndDraw(Scene scene, Navigation navigation)
        {
            if (navigation.IsViewDirty)
            {
                accumulator.Clear();
                navigation.IsViewDirty = false;
                iteration = 0;
            }

            if (iteration < maxIterations)
            {
                accumulator.PreAccumulate();

                for (int i = 0; (i < viewsPerFrame) && (iteration < maxIterations); i++)
                {
                    accumulator.PreDraw();

                    Vector2 localPinholePos = navigation.GetPinholePos(unitDiskSamples[iteration]);
                    Matrix4 perspective = navigation.GetMultiViewPerspective(localPinholePos);

                    GL.MatrixMode(MatrixMode.Projection);
                    GL.PushMatrix();
                    GL.LoadMatrix(ref perspective);

                    GL.Translate(-localPinholePos.X, -localPinholePos.Y, 0);

                    scene.Draw();

                    GL.PopMatrix();

                    accumulator.PostDraw();
                    iteration++;
                }
                accumulator.PostAccumulate();
            }

            accumulator.Show();
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
