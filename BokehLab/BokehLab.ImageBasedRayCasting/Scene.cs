﻿namespace BokehLab.ImageBasedRayCasting
{
    using BokehLab.Math;
    using BokehLab.RayTracing;

    public class Scene : IIntersectable
    {
        public ImageLayer Layer { get; set; }

        public Scene()
        {
            Layer = new ImageLayer();
        }

        #region IIntersectable Members

        public Intersection Intersect(Ray ray)
        {
            return Layer.Intersect(ray);
        }

        #endregion
    }
}
