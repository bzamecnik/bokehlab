using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace spreading.PSF.Perimeter
{
    public class CirclePSFGenerator : IPSFGenerator
    {
        public PSFDescription GeneratePSF(int maxRadius)
        {
            PSFDescription desc = new PSFDescription();
            desc.deltasByRadius = new Delta[maxRadius + 1][];
            for (int radius = 0; radius < maxRadius + 1; radius++)
            {
                List<Delta> deltas = new List<Delta>();
                // TODO: sample the deltas using a circle eqation

                // dummy code:
                deltas.Add(new Delta(-radius, 0, 1));
                deltas.Add(new Delta(radius + 1, 0, -1));

                desc.deltasByRadius[radius] = deltas.ToArray();
            }
            return desc;
        }


    }
}
