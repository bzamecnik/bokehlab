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

        private FloatMapImage[] colorLayers;

        private int width = 0;
        private int height = 0;

        public int Width { get { return width; } internal set { width = value; } }
        public int Height { get { return height; } internal set { height = value; } }

        private int layerCount;
        public int LayerCount { get { return layerCount; } }

        private int colorBands;

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

        public HeightField(IEnumerable<FloatMapImage> depthLayers, IEnumerable<FloatMapImage> colorLayers)
        {
            Debug.Assert(depthLayers != null);
            Debug.Assert(colorLayers != null);
            //Debug.Assert(depthLayers.Length > 0);
            this.depthLayers = depthLayers.ToArray();
            this.colorLayers = colorLayers.ToArray();
            this.layerCount = this.depthLayers.Length;
            if (layerCount > 0)
            {
                this.width = (int)this.depthLayers[0].Width;
                this.height = (int)this.depthLayers[0].Height;
            }
            if (this.colorLayers.Length > 0)
            {
                colorBands = (int)this.colorLayers[0].TotalChannelsCount;
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

        public float[] GetColor(int x, int y, int layer)
        {
            Debug.Assert(layer < layerCount);
            Debug.Assert(x >= 0);
            Debug.Assert(y >= 0);
            Debug.Assert(x < width);
            Debug.Assert(y < height);
            var image = colorLayers[layer].Image;
            float[] color = new float[colorBands];
            for (int band = 0; band < colorBands; band++)
            {
                color[band] = image[x, y, band];
            }
            return color;
        }

        public float GetDepthBilinear(Vector2 position, int layer)
        {
            Debug.Assert(layer < layerCount);
            Debug.Assert(position.X >= 0);
            Debug.Assert(position.Y >= 0);
            Debug.Assert(position.X < width);
            Debug.Assert(position.Y < height);

            int xFloor = (int)Math.Floor(position.X);
            int yFloor = (int)Math.Floor(position.Y);
            int xCeil = (xFloor < width - 1) ? xFloor + 1 : xFloor;
            int yCeil = (yFloor < height - 1) ? yFloor + 1 : yFloor;

            float xFloat = position.X - xFloor;
            float yFloat = position.Y - yFloor;

            var image = depthLayers[layer].Image;
            float a = xFloat * image[xFloor, yFloor, 0] + (1 - xFloat) * image[xFloor, yCeil, 0];
            float b = xFloat * image[xCeil, yFloor, 0] + (1 - xFloat) * image[xCeil, yCeil, 0];

            return yFloat * a + (1 - yFloat) * b;
        }
    }
}
