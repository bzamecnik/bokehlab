using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BokehLab.Lens;

namespace BokehLab.ImageBasedRayCasting
{
    class Camera
    {
        public Sensor Sensor { get; set; }

        public ThinLens Lens { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x">Position on senzor raster</param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Ray GenerateRay(int x, int y)
        {
            // for a position on the sensor generate a position on the lens
            return null;
        }
    }
}
