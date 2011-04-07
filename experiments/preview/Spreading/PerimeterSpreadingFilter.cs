using System.Diagnostics;
using BokehLab.FloatMap;
using BokehLab.Spreading.PSF.Perimeter;

namespace BokehLab.Spreading
{
    public class PerimeterSpreadingFilter : AbstractSpreadingFilter
    {
        // [radius][deltaIndex]
        // radius: [0; maxRadius]
        // deltaIndex: [0; maxDeltaIndex(radius)]
        public PSFDescription Psf { get; set; }

        // for debugging purposes
        // can be lower than the real maximum available PSF radius
        public int ForceMaxRadius { get; set; }

        protected override void SpreadPSF(int x, int y, int radius, float weight, float[, ,] origImage, float[, ,] spreadingImage, int tableWidth, int tableHeight, uint bands, bool hasAlpha)
        {
            Debug.Assert(Psf != null);

            if ((radius > Psf.MaxRadius) || (radius > ForceMaxRadius))
            {
                // TODO: a missing PSF sample for the current radius
                // could be generated and added to the Psf description

                //Console.WriteLine("Warning: Trying to spread a PSF with larger radius ({0}) than is available {1}.", radius, Psf.MaxRadius);
                
                // visualize where the PSF of requested radius is unavalable
                radius = 0; // Psf.MaxRadius;
            }
            Delta[] deltas = Psf.deltasByRadius[radius];
            Debug.Assert(deltas != null);
            uint alphaBand = bands;
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
                spreadingImage[u, v, alphaBand] += value;
            }
        }

        protected override void Integrate(FloatMapImage spreadingTable)
        {
            IntegrateHorizontally(spreadingTable);
        }
    }
}
