namespace BokehLab.Spreading
{
    using System.Diagnostics;
    using BokehLab.FloatMap;
    using BokehLab.Math;
    using BokehLab.Spreading.PSF.Perimeter;

    public class PerimeterSpreadingFilter : AbstractSpreadingFilter
    {
        // [radius][deltaIndex]
        // radius: [0; maxRadius]
        // deltaIndex: [0; maxDeltaIndex(radius)]
        public PSFDescription Psf { get; set; }

        // for debugging purposes
        // can be lower than the real maximum available PSF radius
        public int ForceMaxRadius { get; set; }

        internal override void SpreadPSF(int x, int y, int radius, float weight, float[, ,] origImage, float[, ,] spreadingImage, float[, ,] normalizationImage, int tableWidth, int tableHeight, uint bands)
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

        protected override void Filter(FloatMapImage inputImage, FloatMapImage spreadingTable, FloatMapImage normalizationTable)
        {
            Spread(inputImage, spreadingTable, normalizationTable);
            IntegrateHorizontally(spreadingTable, normalizationTable);
        }
    }
}
