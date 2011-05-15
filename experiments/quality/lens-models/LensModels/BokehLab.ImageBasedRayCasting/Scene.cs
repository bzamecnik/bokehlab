﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using BokehLab.Lens;
using BokehLab.FloatMap;

namespace BokehLab.ImageBasedRayCasting
{
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
