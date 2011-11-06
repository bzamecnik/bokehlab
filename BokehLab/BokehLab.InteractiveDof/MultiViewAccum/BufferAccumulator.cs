namespace BokehLab.InteractiveDof.MultiViewAccum
{
    using OpenTK.Graphics.OpenGL;

    /// <summary>
    /// Accumulation buffer accumulator, an integer buffer.
    /// </summary>
    class BufferAccumulator : AbstractRendererModule, IAccumulator
    {
        public int TotalIterations { get; set; }
        int iteration = 0;

        #region IAccumulator Members

        public void PreAccumulate()
        {
        }

        public void PostAccumulate()
        {
        }

        public void PreDraw()
        {
        }

        public void PostDraw()
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
        #endregion
    }
}
