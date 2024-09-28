using System.Runtime.InteropServices;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Erwine.Leonard.T.GDIPlus.Palette.Helpers.Pixels.Indexed
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct PixelData4Indexed : IIndexedPixel
    {
        // raw component values
        private byte index;

        // get - index method
        public readonly byte GetIndex(int offset) => (byte)GetBitRange(8 - offset - 4, 7 - offset);

        // set - index method
        public void SetIndex(int offset, byte value) => SetBitRange(8 - offset - 4, 7 - offset, value);

        private readonly int GetBitRange(int startOffset, int endOffset)
        {
            int result = 0;
            byte bitIndex = 0;

            for (int offset = startOffset; offset <= endOffset; offset++)
            {
                int bitValue = 1 << bitIndex;
                result += GetBit(offset) ? bitValue : 0;
                bitIndex++;
            }

            return result;
        }

        private readonly bool GetBit(int offset) => (index & (1 << offset)) != 0;

        private void SetBitRange(int startOffset, int endOffset, int value)
        {
            byte bitIndex = 0;

            for (int offset = startOffset; offset <= endOffset; offset++)
            {
                int bitValue = 1 << bitIndex;
                SetBit(offset, (value & bitValue) != 0);
                bitIndex++;
            }
        }

        private void SetBit(int offset, bool value)
        {
            if (value)
            {
                index |= (byte) (1 << offset);
            }
            else
            {
                index &= (byte) ~(1 << offset);
            }
        }
    }
}
