using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BokehLab.FloatMap;
using OpenTK;

namespace BokehLab.ImageBasedRayCasting
{
    class Sensor
    {
        /// <summary>
        /// Raster spans from [0; 0] to [RasterSize.X; Raster.SizeY].
        /// Left to right, top to bottom.
        /// </summary>
        public int RasterWidth { get; set; }
        public int RasterHeight { get; set; }

        /// <summary>
        /// Senzor spans from [-SensorSize.X/2; -SensorSize.Y/2] to
        /// [SensorSize.X/2; SensorSize.Y/2]. [0;0] is at the center
        /// corresponding to SenzorPosition.
        /// </summary>
        Vector2d SensorSize { get; set; }

        public FloatMapImage FloatMap { get; set; }

        /// <summary>
        /// Transform from position in senzor space to raster space.
        /// </summary>
        /// <param name="senzorPos"></param>
        /// <returns></returns>
        public Vector2d SenzorToRaster(Vector2d senzorPos)
        {
            return new Vector2d(
                (senzorPos.X / (1 * SensorSize.X) + 0.5) * RasterWidth,
                (1 - (senzorPos.Y / (1 * SensorSize.Y) + 0.5)) * RasterHeight);
        }

        public Vector2d RasterToSenzor(Vector2d rasterPos)
        {
            return new Vector2d(
                ((rasterPos.X / RasterWidth) - 0.5) * SensorSize.X,
                (0.5 - (rasterPos.Y / RasterHeight)) * SensorSize.Y);
        }
    }
}
