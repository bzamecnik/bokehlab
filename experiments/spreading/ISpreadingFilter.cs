using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libpfm;

namespace spreading
{
    public interface ISpreadingFilter
    {
        PFMImage FilterImage(PFMImage inputImage, PFMImage outputImage);
    }
}
