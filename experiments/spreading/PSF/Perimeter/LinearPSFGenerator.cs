using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace spreading.PSF.Perimeter
{
    public class LinearPSFGenerator : IPSFGenerator
    {
        public PSFDescription GeneratePSF(int maxRadius)
        {
            PSFDescription psf = new PSFDescription();
            psf.deltasByRadius = new Delta[maxRadius + 1][];
            for (int radius = 0; radius < maxRadius + 1; radius++)
            {
                Delta[] deltas = new Delta[2];
                deltas[0] = new Delta(-radius, 0, 1);
                deltas[1] = new Delta(radius + 1, 0, -1);
                psf.deltasByRadius[radius] = deltas;
            }
            return psf;
        }
    }
}
