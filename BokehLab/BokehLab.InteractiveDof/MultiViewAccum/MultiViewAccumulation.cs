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

        private bool accumulate;
        public bool Accumulate
        {
            get { return accumulate; }
            set
            {
                accumulate = value;
                MaxIterations = (accumulate) ? unitDiskSamples.Length : ViewsPerFrame;
            }
        }

        public MultiViewAccumulation()
        {
            ViewsPerFrame = 9; // 16 good for float16, 4 or 8 for float32
            // TODO: support creating samples for a cropped aperture (hexagon etc.)
            unitDiskSamples = sampler.CreateShuffledLensSamplesFloat(sqrtSampleCount).ToArray();
            Accumulate = true;
            MaxIterations = unitDiskSamples.Length;
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

        public override void OnKeyUp(object sender, OpenTK.Input.KeyboardKeyEventArgs e)
        {
            if (Enabled && (e.Key == OpenTK.Input.Key.Tab))
            {
                Accumulate = !Accumulate;
                return;
            }

            base.OnKeyUp(sender, e);
        }
    }
}
