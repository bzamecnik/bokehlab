namespace BokehLab.InteractiveDof.MultiViewAccum
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using BokehLab.Math;
    using OpenTK;
    using OpenTK.Graphics.OpenGL;
    using System.Diagnostics;

    abstract class IncrementalRenderer : AbstractRendererModule
    {
        // How many views should be accumulated during one frame rendering.
        // NOTE: navigation.IsViewDirty must be set to true
        public int ViewsPerFrame { get; set; }

        int iteration = 0;

        // NOTE: navigation.IsViewDirty must be set to true
        public int MaxIterations { get; set; }
        IAccumulator accumulator;

        Stopwatch stopwatch = new Stopwatch();
        public long CumulativeMilliseconds { get; set; }
        public float AverageFrameTime { get { return CumulativeMilliseconds / (float)iteration; } }

        public IncrementalRenderer()
        {
            ViewsPerFrame = 1;

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
                stopwatch.Reset();
                CumulativeMilliseconds = 0;
                accumulator.Clear();
                iteration = 0;
                stopwatch.Start();
                navigation.IsViewDirty = false;
            }

            if (iteration < MaxIterations)
            {
                accumulator.PreAccumulate();

                for (int i = 0; (i < ViewsPerFrame) && (iteration < MaxIterations); i++)
                {
                    accumulator.PreDraw();

                    DrawSingleFrame(iteration, scene, navigation);

                    accumulator.PostDraw();
                    iteration++;
                }
                accumulator.PostAccumulate();
                CumulativeMilliseconds = stopwatch.ElapsedMilliseconds;
            }
            if (iteration == MaxIterations)
            {
                stopwatch.Stop();
            }

            accumulator.Show();
        }

        protected abstract void DrawSingleFrame(int iteration, Scene scene, Navigation navigation);
    }
}
