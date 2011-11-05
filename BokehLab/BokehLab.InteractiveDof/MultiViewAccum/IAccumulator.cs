namespace BokehLab.InteractiveDof.MultiViewAccum
{
    interface IAccumulator
    {
        void PreAccumulate();
        void PostAccumulate();

        void PreDraw();
        void PostDraw();

        void Show();
        void Clear();

        void Initialize(int width, int height);
        void Dispose();
    }
}
