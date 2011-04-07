using BokehLab.FloatMap;

namespace BokehLab.Spreading
{
    public class RectangleSpreadingFilter : AbstractSpreadingFilter
    {
        protected override void SpreadPSF(int x, int y, int radius, float weight, float[, ,] origImage, float[, ,] spreadingImage, int tableWidth, int tableHeight, uint bands, bool hasAlpha)
        {
            float psfSide = radius * 2 + 1; // side of a square PSF
            float areaInv = weight / (psfSide * psfSide);

            int top = MathHelper.Clamp<int>(y - radius, 0, tableHeight - 1);
            int bottom = MathHelper.Clamp<int>(y + radius + 1, 0, tableHeight - 1);
            int left = MathHelper.Clamp<int>(x - radius, 0, tableWidth - 1);
            int right = MathHelper.Clamp<int>(x + radius + 1, 0, tableWidth - 1);

            int alphaBand = (int)bands;
            float originalAlpha = 1.0f;
            if (hasAlpha)
            {
                originalAlpha = origImage[x, y, alphaBand];
            }
            for (int band = 0; band < bands; band++)
            {
                float cornerValue = origImage[x, y, band] * originalAlpha * areaInv;
                PutCorners(spreadingImage, top, bottom, left, right, band, cornerValue);
            }
            // Note: intensity for the normalization is 1.0
            //PutCorners(spreadingImage, top, bottom, left, right, alphaBand, origImage[x, y, alphaBand] * areaInv);
            PutCorners(spreadingImage, top, bottom, left, right, alphaBand, areaInv);
        }

        private static void PutCorners(float[, ,] table, int top, int bottom, int left, int right, int band, float value)
        {
            table[left, top, band] += value;
            table[right, top, band] -= value;
            table[left, bottom, band] -= value;
            table[right, bottom, band] += value;
        }

        protected override void Integrate(FloatMapImage spreadingTable)
        {
            IntegrateHorizontally(spreadingTable);
            IntegrateVertically(spreadingTable);
        }
    }
}
