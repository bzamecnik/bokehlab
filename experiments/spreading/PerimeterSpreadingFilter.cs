using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libpfm;
using mathHelper;
using spreading.PSF.Perimeter;
using System.Diagnostics;

namespace spreading
{
    public class PerimeterSpreadingFilter : AbstractSpreadingFilter
    {
        // [radius][deltaIndex]
        // radius: [0; maxRadius]
        // deltaIndex: [0; maxDeltaIndex(radius)]
        private PSFDescription psf;

        public PerimeterSpreadingFilter(PSFDescription psf)
        {
            this.psf = psf;
        }

        protected override void SpreadPSF(int x, int y, int radius, float weight, float[, ,] origImage, float[, ,] spreadingImage, float[, ,] normalizationImage, int tableWidth, int tableHeight, uint bands)
        {
            if (radius > psf.MaxRadius)
            {
                return;
            }
            Delta[] deltas = psf.deltasByRadius[radius];
            Debug.Assert(deltas != null);
            for (int i = 0; i < deltas.Length; i++)
            {
                Delta delta = deltas[i];
                int u = MathHelper.Clamp<int>(x + delta.x, 0, tableWidth - 1);
                int v = MathHelper.Clamp<int>(y + delta.y, 0, tableHeight - 1);
                float value = weight * delta.value;
                for (int band = 0; band < bands; band++)
                {
                    spreadingImage[u, v, band] += value * origImage[x, y, band];
                }
                normalizationImage[u, v, 0] += value;
            }
        }

        protected override void Integrate(PFMImage spreadingTable, PFMImage normalizationTable)
        {
            IntegrateHorizontally(spreadingTable, normalizationTable);
        }
    }
}
