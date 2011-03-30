using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace spreading.PSF.Perimeter
{
    // - final goal: use an arbitrary image as a PSF
    // - temporary implementation: draw a circle

    public class ImageBasedPSFGenerator : IPSFGenerator
    {
        //Bitmap Image { get; set; }

        //public ImageBasedPSFGenerator(Bitmap image)
        //{
        //    Image = image;
        //}

        public PSFDescription GeneratePSF(int maxRadius)
        {
            PSFDescription desc = new PSFDescription();
            desc.deltasByRadius = new Delta[maxRadius + 1][];
            for (int radius = 0; radius < maxRadius + 1; radius++)
            {
                List<Delta> deltas = new List<Delta>();

                // scale the original image to size [2*radius; 2*radius]

                //

                // for each X
                //   for each Y
                //     compute derivative approximation (X difference)
                //     if (diffX != 0)
                //       add a new delta
                
                //deltas.Add(new Delta(-radius, 0, 1));

                desc.deltasByRadius[radius] = deltas.ToArray();
            }
            return desc;
        }
    }
}
