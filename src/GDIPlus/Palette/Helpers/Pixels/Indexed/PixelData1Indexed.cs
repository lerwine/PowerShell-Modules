using System.Runtime.InteropServices;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Erwine.Leonard.T.GDIPlus.Palette.Helpers.Pixels.Indexed
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct PixelData1Indexed : IIndexedPixel
    {
        // raw component values
        private byte index;

        // get - index method
        public readonly byte GetIndex(int offset) => (index & 1 << (7 - offset)) != 0 ? Pixel.One : Pixel.Zero;

        // set - index method
        public void SetIndex(int offset, byte value)
        {
            value = value == 0 ? Pixel.One : Pixel.Zero;

            if (value == 0)
            {
                index |= (byte) (1 << (7 - offset));
            }
            else
            {
                index &= (byte) ~(1 << (7 - offset));
            }
        }
    }
}
