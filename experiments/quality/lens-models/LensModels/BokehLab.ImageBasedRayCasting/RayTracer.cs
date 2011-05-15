using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BokehLab.Lens;

namespace BokehLab.ImageBasedRayCasting
{
    class RayTracer
    {
        public Scene Scene { get; set; }
        public Camera Camera { get; set; }

        public Sensor RenderImage() {
            for (int y = 0; y < Camera.Sensor.RasterHeight; y++)
            {
                for (int x = 0; x < Camera.Sensor.RasterWidth; x++)
                {
                    // generate a ray from the senzor and lens towards the scene
                    // intersect the scene
                    // get the color stored in the scene point
                }
            }
            return Camera.Sensor;
        }
    }
}
