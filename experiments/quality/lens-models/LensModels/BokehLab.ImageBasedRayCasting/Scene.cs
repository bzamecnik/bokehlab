namespace BokehLab.ImageBasedRayCasting
{
    using BokehLab.Math;

    // TODO:
    // - create a similar transform as is in Senzor

    class Scene
    {
        public ImageLayer Layer { get; set; }

        public Scene()
        {
            Layer = new ImageLayer();
        }

        public Intersection Intersect(Ray ray)
        {
            return Layer.Intersect(ray);
        }
    }
}
