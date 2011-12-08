namespace BokehLab.NBuffers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using BokehLab.FloatMap;
    using System.Diagnostics;

    // TODO:
    // - evaluate with four smaller squares
    // - solve storing min/max N-buffers
    //   - at best support multi-band floatmaps
    // - compute min/max at once
    // - try separable passes
    // - rewrite on a GPU (fragment shader, CUDA)

    public class NeighborhoodBuffer
    {
        internal int Width { get; set; }
        internal int Height { get; set; }

        internal FloatMapImage[] minLevels;
        internal FloatMapImage[] maxLevels;
        internal int LevelCount { get; set; }

        public float UndefinedValue { get; set; }

        public NeighborhoodBuffer(FloatMapImage depthMap)
        {
            UndefinedValue = 1.0f;

            Width = (int)depthMap.Width;
            Height = (int)depthMap.Height;
            // For simple evaluation with a single bounding power-of-2 square
            // there is Ceiling function, otherwise when evaluating with four
            // smaller rectangles Floor is ok (we need one less level).
            LevelCount = (int)Math.Ceiling(Math.Log(Math.Max(Width, Height), 2));
            minLevels = new FloatMapImage[LevelCount];
            maxLevels = new FloatMapImage[LevelCount];

            // The fist level has no neighborhood so it is a copy of the
            // original depth map.
            minLevels[0] = CopySingleChannelDepthMap(depthMap);
            maxLevels[0] = minLevels[0];

            // The subsequent level take maximum from neighborhood 2^i.
            // This is just a simple single-pass construction.
            int offset = 1;
            for (int i = 1; i < LevelCount; i++)
            {
                minLevels[i] = ConstructLevel(offset, i, Math.Min, 1.0f, minLevels[i - 1]);
                maxLevels[i] = ConstructLevel(offset, i, Math.Max, 0.0f, maxLevels[i - 1]);
                offset *= 2;
            }
        }

        private FloatMapImage CopySingleChannelDepthMap(FloatMapImage depthMap)
        {
            if (depthMap.PixelFormat == PixelFormat.Greyscale)
            {
                return (FloatMapImage)depthMap.Clone();
            }
            else
            {
                return depthMap.ExtractChannel(0);
            }
        }

        private FloatMapImage ConstructLevel(int offset, int i, Func<float, float, float> func, float defaultValue, FloatMapImage prevLevel)
        {
            var level = new FloatMapImage((uint)Width, (uint)Height, PixelFormat.Greyscale);
            var current = level.Image;
            var prev = prevLevel.Image;
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    bool withinWidth = x + offset < Width;
                    bool withinHeight = y + offset < Height;

                    float topLeft = prev[x, y, 0];
                    //if (topLeft == UndefinedValue) topLeft = defaultValue;

                    float bottomLeft = withinHeight ? prev[x, y + offset, 0] : defaultValue;
                    //if (bottomLeft == UndefinedValue) bottomLeft = defaultValue;

                    float topRight = withinWidth ? prev[x + offset, y, 0] : defaultValue;
                    //if (topRight == UndefinedValue) topRight = defaultValue;

                    float bottomRight = (withinWidth && withinHeight) ? prev[x + offset, y + offset, 0] : defaultValue;
                    //if (bottomRight == UndefinedValue) bottomRight = defaultValue;

                    current[x, y, 0] = func(func(topLeft, bottomLeft),
                        func(topRight, bottomRight));
                }
            }
            return level;
        }

        /// <summary>
        /// Evaluates the minimum depth map value in given rectangle.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public float EvaluateMin(int x, int y, int width, int height)
        {
            int requiredLevel = (int)Math.Ceiling(Math.Log(Math.Max(Width, Height), 2));
            Debug.Assert(requiredLevel >= 0);
            Debug.Assert(requiredLevel < LevelCount);

            return minLevels[requiredLevel].Image[x, y, 0];
        }

        /// <summary>
        /// Evaluates the maximum depth map value in given rectangle.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public float EvaluateMax(int x, int y, int width, int height)
        {
            int requiredLevel = (int)Math.Ceiling(Math.Log(Math.Max(Width, Height), 2));
            Debug.Assert(requiredLevel >= 0);
            Debug.Assert(requiredLevel < LevelCount);

            return maxLevels[requiredLevel].Image[x, y, 0];
        }

        /// <summary>
        /// Evaluates both the minimum and maximum depth map value in given rectangle.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public void EvaluateMinMax(int x, int y, int width, int height, out float min, out float max)
        {
            int requiredLevel = (int)Math.Ceiling(Math.Log(Math.Max(Width, Height), 2));
            Debug.Assert(requiredLevel >= 0);
            Debug.Assert(requiredLevel < LevelCount);

            min = minLevels[requiredLevel].Image[x, y, 0];
            max = maxLevels[requiredLevel].Image[x, y, 0];
        }
    }
}
