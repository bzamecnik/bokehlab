namespace BokehLab.Pinhole
{
    using System;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using OpenTK;
    using OpenTK.Input;
    using OpenTK.Graphics;
    using OpenTK.Graphics.OpenGL;
    using System.Runtime.InteropServices;
    using BokehLab.Math;
    using System.Collections.Generic;
    using System.Linq;

    class MultiViewAccumulation
    {
        int accumIterations = 0;
        Vector2[] unitDiskSamples;
        Sampler sampler = new Sampler();
        int maxIterations;

        // How many views should be accumulated during one frame rendering.
        int viewsPerFrame = 16;

        IAccumulator accumulator;

        public MultiViewAccumulation()
        {
            unitDiskSamples = CreateLensSamples(16).ToArray();
            maxIterations = unitDiskSamples.Length;
            accumulator = new BufferAccumulator() { TotalIterations = maxIterations };
        }

        public void AccumulateAndDraw(Scene scene, Navigation navigation)
        {
            if (navigation.IsViewDirty)
            {
                accumulator.Clear();
                navigation.IsViewDirty = false;
                accumIterations = 0;
            }

            for (int i = 0; i < viewsPerFrame; i++)
            {
                if (accumIterations >= maxIterations)
                {
                    break;
                }

                GL.PushMatrix();

                Vector2 localPinholePos = navigation.GetPinholePos(unitDiskSamples[accumIterations]);
                Matrix4 perspective = navigation.GetMultiViewPerspective(localPinholePos);
                GL.MatrixMode(MatrixMode.Projection);
                GL.LoadMatrix(ref perspective);

                GL.Translate(-localPinholePos.X, -localPinholePos.Y, 0);

                scene.Draw();

                GL.PopMatrix();

                accumulator.Accumulate();
                accumIterations++;
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

        interface IAccumulator
        {
            void Accumulate();
            void Show();
            void Clear();

            void Initialize(int width, int height);
            void Dispose();
        }

        /// <summary>
        /// Accumulation buffer accumulator, an integer buffer.
        /// </summary>
        class BufferAccumulator : IAccumulator
        {
            public int TotalIterations { get; set; }
            int iteration = 0;

            #region IAccumulator Members

            public void Accumulate()
            {
                //GL.Accum(AccumOp.Accum, 1f / maxIterations);
                GL.Accum(AccumOp.Mult, 1 - 1f / (iteration + 1));
                GL.Accum(AccumOp.Accum, 1f / (iteration + 1));
                iteration++;
            }

            public void Show()
            {
                //GL.Accum(AccumOp.Return, maxIterations / (float)accumIterations);
                GL.Accum(AccumOp.Return, 1f);
            }

            public void Clear()
            {
                GL.Clear(ClearBufferMask.AccumBufferBit);
                iteration = 0;
            }

            public void Initialize(int width, int height)
            {
            }

            public void Dispose()
            {
            }

            #endregion
        }

        // FrameBuffer accumulator, a float buffer.
        class FboAccumulator : IAccumulator
        {
            #region IAccumulator Members

            public void Accumulate()
            {
                // TODO
                throw new NotImplementedException();
            }

            public void Show()
            {
                // TODO
                throw new NotImplementedException();
            }

            public void Clear()
            {
                throw new NotImplementedException();
            }

            public void Initialize(int width, int height)
            {
                throw new NotImplementedException();
            }

            public void Dispose()
            {
                throw new NotImplementedException();
            }

            #endregion
        }
    }
}
