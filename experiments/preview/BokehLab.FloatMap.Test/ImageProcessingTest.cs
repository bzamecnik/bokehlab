using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using BokehLab.FloatMap;
using System.Drawing;

namespace BokehLab.FloatMap.Test
{
    public class ImageProcessingTest
    {
        [Fact]
        public void TestCreatingSAT()
        {
            FloatMapImage origImage = ((Bitmap)Bitmap.FromFile("chessRGB.jpg")).ToFloatMap();
            FloatMapImage sat = origImage.Integrate();
            sat.ToBitmap(true).Save("chessRGB_sat.png", System.Drawing.Imaging.ImageFormat.Png);
            FloatMapImage differentiatedSat = sat.Differentiate();
            differentiatedSat.ToBitmap(true).Save("chessRGB_satDiff.png", System.Drawing.Imaging.ImageFormat.Png);
        }
    }
}
