#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Erwine.Leonard.T.GDIPlus.Palette.Helpers
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    public class FastRandom(uint seed)
    {
        private const double RealUnitInt = 1.0 / (int.MaxValue + 1.0);

        private uint x = seed, y = 842502087, z = 3579807591, w = 273326509;

        public int Next(int upperBound)
        {
            uint t = x ^ (x << 11); x = y; y = z; z = w;
            return (int) (RealUnitInt * (int) (0x7FFFFFFF & (w = w ^ (w >> 19) ^ t ^ (t >> 8))) * upperBound);
        }
    }
}
