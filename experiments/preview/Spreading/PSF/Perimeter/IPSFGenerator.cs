namespace BokehLab.Spreading.PSF.Perimeter
{
    public interface IPSFGenerator
    {
        PSFDescription GeneratePSF(int maxRadius);
    }
}
