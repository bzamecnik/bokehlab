using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using BokehLab.Lens;
using BokehLab.FloatMap;

namespace BokehLab.ImageBasedRayCasting
{
    class Scene
    {
        // For simplicity the scene is a rectangle with center at
        // Position (0,0,z) and normal (0,0,1) aligned with the
        // optical axis.
        //
        // The scene shading is precomputed and colors stored in
        // a float map.

        public Vector3d Position { get; set; }

        public FloatMapImage Image { get; set; }

        public Intersection Intersect(Ray ray)
        {
            // intersect
            Vector3d intersectionPos = new Vector3d();
            // compute 2D position in image coordinates
            // retrieve color
            int x = 0;
            int y = 0;
            float color = Image.Image[x, y, 0];
            return new Intersection(intersectionPos, color);
        }
    }

    struct Intersection
    {
        public Vector3d position;
        public float color;

        public Intersection(Vector3d position, float color)
        {
            this.position = position;
            this.color = color;
        }
    }
}
