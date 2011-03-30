using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace spreading.PSF.Perimeter
{
    public interface IPSFGenerator
    {
        PSFDescription GeneratePSF(int maxRadius);
    }
}
