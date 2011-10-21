namespace BokehLab.Spreading
{
    using BokehLab.FloatMap;
    using BokehLab.Math;

    public class RectangleSpreadingFilter : AbstractSpreadingFilter
    {
        internal override void SpreadPSF(int x, int y, int radius, float weight, float[, ,] origImage, float[, ,] spreadingImage, float[, ,] normalizationImage, int tableWidth, int tableHeight, uint bands)
        {
            // assert: radius > 0
            // assert: x in [0; width - 1]
            // assert: y in [0; height - 1]

            float psfSide = radius * 2 + 1; // side of a square PSF
            float areaInv = weight / (psfSide * psfSide);

            int top = MathHelper.ClampMin<int>(y - radius, 0);
            int bottom = MathHelper.ClampMin<int>(y + radius + 1, 0);
            int left = MathHelper.ClampMin<int>(x - radius, 0);
            int right = MathHelper.ClampMin<int>(x + radius + 1, 0);

            for (int band = 0; band < bands; band++)
            {
                float cornerValue = origImage[x, y, band] * areaInv;
                PutCorners(spreadingImage, top, bottom, left, right, band, cornerValue, tableWidth, tableHeight);
            }
            // Note: intensity for the normalization is 1.0
            PutCorners(normalizationImage, top, bottom, left, right, 0, areaInv, tableWidth, tableHeight);
        }

        private static void PutCorners(float[, ,] table, int top, int bottom, int left, int right, int band, float value, int width, int height)
        {
            if (value != 0)
            {
                table[left, top, band] += value;
                if (right < width)
                {
                    table[right, top, band] -= value;
                }
                if (bottom < height)
                {
                    table[left, bottom, band] -= value;
                }
                if ((right < width) && (bottom < height))
                {
                    table[right, bottom, band] += value;
                }
                //table[left, top, band] += value;
                //table[right, top, band] -= value;
                //table[left, bottom, band] -= value;
                //table[right, bottom, band] += value;
            }
        }
        protected override void Filter(FloatMapImage inputImage, FloatMapImage spreadingTable, FloatMapImage normalizationTable)
        {
            // phase 1: distribute corners into the table
            Spread(inputImage, spreadingTable, normalizationTable);

            //spreadingTable.ToBitmap(true).Save("spreading-diffs.png");
            //normalizationTable.ToBitmap(true).Save("normalization-diffs.png");

            // phase 2: accumulate the corners into rectangles
            IntegrateHorizontally(spreadingTable, normalizationTable);

            //spreadingTable.ToBitmap(true).Save("spreading-horiz.png");
            //normalizationTable.ToBitmap(true).Save("normalization-horiz.png");

            IntegrateVertically(spreadingTable, normalizationTable);
            //spreadingTable.ToBitmap(true).Save("spreading-horiz-vert.png");
            //normalizationTable.ToBitmap(true).Save("normalization-horiz-vert.png");
        }
    }
}
