namespace BokehLab.Spreading.Test
{
    using System.Drawing;
    using BokehLab.FloatMap;
    using BokehLab.Math;
    using BokehLab.Spreading;
    using Xunit;

    public class HybridSpreadingFilterTest
    {
        private static readonly float CRITERION_THRESHOLD = 0.3f;

        [Fact]
        public void MakeUnthresholdedCriterionImages()
        {
            FloatMapImage origImage = ((Bitmap)Bitmap.FromFile("chessRGB.jpg")).ToFloatMap();
            HybridSpreadingFilter.FilterSelectionCriterion criterion =
                new HybridSpreadingFilter.FilterSelectionCriterion()
                {
                    Threshold = CRITERION_THRESHOLD
                };
            criterion.OriginalImage = origImage;
            criterion.OriginalImageSAT = origImage.Integrate();

            int width = (int)origImage.Width;
            int height = (int)origImage.Height;
            FloatMapImage criterionImage = new FloatMapImage(origImage.Width, origImage.Height, PixelFormat.Greyscale);
            FloatMapImage diffImage = new FloatMapImage(origImage.Width, origImage.Height, PixelFormat.Greyscale);

            float[, ,] sat = criterion.OriginalImageSAT.Image;

            int minPsfRadius = 1;
            int maxPsfRadius = 100;
            int step = 10;
            for (int psfRadius = minPsfRadius; psfRadius < maxPsfRadius; psfRadius += step)
            {
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        float sourceIntensity = origImage.Image[x, y, 0];

                        int left = MathHelper.Clamp<int>(x - psfRadius - 1, 0, width - 1);
                        int right = MathHelper.Clamp<int>(x + psfRadius, 0, width - 1);
                        int top = MathHelper.Clamp<int>(y - psfRadius - 1, 0, height - 1);
                        int bottom = MathHelper.Clamp<int>(y + psfRadius, 0, height - 1);

                        float psfArea = (right - left) * (bottom - top);
                        float psfSum = sat[right, bottom, 0] + sat[left, top, 0]
                            - sat[left, bottom, 0] - sat[right, top, 0];
                        // average over neighborhood of the current pixel within the PSF radius
                        // (except the current pixel itself)
                        float averageOverPsf = (psfSum - sourceIntensity) / (psfArea - 1);
                        //criterionImage.Image[x, y, 0] = (sourceIntensity > averageOverPsf) ? 1 : 0;
                        diffImage.Image[x, y, 0] = sourceIntensity - averageOverPsf;
                        //diffImage.Image[x, y, 0] = Math.Abs(sourceIntensity - averageOverPsf);
                        //criterionImage.Image[x, y, 0] = criterion.SelectFilter(x, y, psfRadius);
                    }
                }
                //criterionImage.ToBitmap().Save(string.Format("chessRGB_criterion_{0:000}.png", psfRadius), System.Drawing.Imaging.ImageFormat.Png);
                diffImage.ToBitmap(false).Save(string.Format("chessRGB_diff_nonabs_{0:000}.png", psfRadius), System.Drawing.Imaging.ImageFormat.Png);
            }
        }

        [Fact]
        public void MakeCriterionImages()
        {
            FloatMapImage origImage = ((Bitmap)Bitmap.FromFile("chessRGB.jpg")).ToFloatMap();
            HybridSpreadingFilter.FilterSelectionCriterion criterion =
                new HybridSpreadingFilter.FilterSelectionCriterion()
                {
                    Threshold = CRITERION_THRESHOLD
                };
            criterion.OriginalImage = origImage;
            criterion.OriginalImageSAT = origImage.Integrate();

            int width = (int)origImage.Width;
            int height = (int)origImage.Height;
            FloatMapImage criterionImage = new FloatMapImage(origImage.Width, origImage.Height, PixelFormat.Greyscale);
            float[, ,] sat = criterion.OriginalImageSAT.Image;

            int minPsfRadius = 1;
            int maxPsfRadius = 100;
            int step = 10;
            for (int psfRadius = minPsfRadius; psfRadius < maxPsfRadius; psfRadius += step)
            {
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        criterionImage.Image[x, y, 0] = criterion.SelectFilter(x, y, psfRadius);
                    }
                }
                criterionImage.ToBitmap().Save(string.Format("chessRGB_criterion_{0:000}.png", psfRadius), System.Drawing.Imaging.ImageFormat.Png);
            }
        }
    }
}
