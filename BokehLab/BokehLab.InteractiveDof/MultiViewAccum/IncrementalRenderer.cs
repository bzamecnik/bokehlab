namespace BokehLab.InteractiveDof.MultiViewAccum
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using BokehLab.Math;
    using OpenTK;
    using OpenTK.Graphics.OpenGL;

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

        /// <summary>
        /// Number of samples per frame (a single rendering cycle).
        /// </summary>
        public int SampleCount { get; set; }

        public static readonly int SqrtMaxTotalSampleCount = 32; // max 1024 samples

        protected int MaxTotalSampleCount { get { return SqrtMaxTotalSampleCount * SqrtMaxTotalSampleCount; } }

        int totalSampleCount;
        /// <summary>
        /// Number of samples per the whole incremental rendering.
        /// </summary>
        public int TotalSampleCount
        {
            get { return totalSampleCount; }
            set
            {
                totalSampleCount = BokehLab.Math.MathHelper.Clamp(value, SampleCount, MaxTotalSampleCount);
                MaxIterations = SingleFrameIterations;
                if (Navigation != null)
                {
                    Navigation.IsViewDirty = true;
                }
            }
        }

        // total number of rendering cycles to be incrementally accumulated
        public int SingleFrameIterations { get { return (int)Math.Ceiling(TotalSampleCount / (float)SampleCount); } }

        public bool ShuffleLensSamples { get; set; }

        public Navigation Navigation { get; set; }

        public IncrementalRenderer()
        {
            ViewsPerFrame = 1;

            SampleCount = 16;
            TotalSampleCount = MaxTotalSampleCount;
            ShuffleLensSamples = true;

            // draw all views at once
            //viewsPerFrame = maxIterations;

            //accumulator = new BufferAccumulator();
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
                stopwatch.Stop();
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

        public override void OnKeyUp(object sender, OpenTK.Input.KeyboardKeyEventArgs e)
        {
            if (Enabled)
            {
                if (e.Key == OpenTK.Input.Key.BracketLeft)
                {
                    TotalSampleCount /= 2;
                    return;
                }
                else if (e.Key == OpenTK.Input.Key.BracketRight)
                {
                    TotalSampleCount *= 2;
                    return;
                }
            }

            base.OnKeyUp(sender, e);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(this.GetType().Name + " {");
            sb.AppendFormat("  Maximum total samples: {0},", MaxTotalSampleCount);
            sb.AppendLine();
            sb.AppendFormat("  Current total samples per accumulation: {0},", TotalSampleCount);
            sb.AppendLine();
            sb.AppendFormat("  Samples per view: {0},", SampleCount);
            sb.AppendLine();
            sb.AppendFormat("  Views per frame: {0},", ViewsPerFrame);
            sb.AppendLine();
            sb.AppendFormat("  Total iterations: {0}", MaxIterations);
            sb.AppendLine();
            sb.AppendLine("}");
            return sb.ToString();
        }
    }
}
