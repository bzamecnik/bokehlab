namespace BokehLab.InteractiveDof.MultiViewAccum
{
    interface IAccumulator : IRendererModule
    {
        void PreAccumulate();
        void PostAccumulate();

        void PreDraw();
        void PostDraw();

        void Show();
        void Clear();
    }
}
