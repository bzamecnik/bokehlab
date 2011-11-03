namespace BokehLab.RayTracing.HeightField
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using BokehLab.FloatMap;
    using BokehLab.Math;
    using OpenTK;
    using System.Diagnostics;

    public class HeightField
    {
        // [X,Y,layer]
        // layers ordered from near to far depth
        private FloatMapImage[] depthLayers;

        private int width = 0;
        private int height = 0;

        public int Width { get { return width; } internal set { width = value; } }
        public int Height { get { return height; } internal set { height = value; } }

        private int layerCount;
        public int LayerCount { get { return layerCount; } }

        public HeightField(int width, int height)
            : this(new FloatMapImage[] { })
        {
            this.width = width;
            this.height = height;
        }

        public HeightField(IEnumerable<FloatMapImage> depthLayers)
        {
            Debug.Assert(depthLayers != null);
            //Debug.Assert(depthLayers.Length > 0);
            this.depthLayers = depthLayers.ToArray();
            this.layerCount = this.depthLayers.Length;
            if (layerCount > 0)
            {
                this.width = (int)this.depthLayers[0].Width;
                this.height = (int)this.depthLayers[0].Height;
            }
        }

        public float GetDepth(int x, int y, int layer)
        {
            Debug.Assert(layer < layerCount);
            Debug.Assert(x >= 0);
            Debug.Assert(y >= 0);
            Debug.Assert(x < width);
            Debug.Assert(y < height);
            return depthLayers[layer].Image[x, y, 0];
        }

        public float GetDepthBilinear(int x, int y, int layer)
        {
            Debug.Assert(layer < layerCount);
            Debug.Assert(x >= 0);
            Debug.Assert(y >= 0);
            Debug.Assert(x < width);
            Debug.Assert(y < height);

            // TODO

            return depthLayers[layer].Image[x, y, 0];
        }
    }
}
