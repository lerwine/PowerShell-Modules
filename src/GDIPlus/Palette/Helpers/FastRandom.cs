using System;

namespace Erwine.Leonard.T.GDIPlus.Palette.Helpers
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class FastRandom
    {
        private const Double RealUnitInt = 1.0/(Int32.MaxValue + 1.0);

        private UInt32 x, y, z, w;

        public FastRandom(UInt32 seed)
        {
            x = seed;
            y = 842502087;
            z = 3579807591;
            w = 273326509;
        }

        public Int32 Next(Int32 upperBound)
        {
            UInt32 t = (x ^ (x << 11)); x = y; y = z; z = w;
            return (Int32) ((RealUnitInt*(Int32) (0x7FFFFFFF & (w = (w ^ (w >> 19)) ^ (t ^ (t >> 8)))))*upperBound);
        }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
