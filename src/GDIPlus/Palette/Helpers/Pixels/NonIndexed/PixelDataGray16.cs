using System.Runtime.InteropServices;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Erwine.Leonard.T.GDIPlus.Palette.Helpers.Pixels.NonIndexed
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    /// <summary>
    /// Name |                  Grayscale                    |
    /// Bit  |00|01|02|03|04|05|06|07|08|09|10|11|12|13|14|15| 
    /// Byte |00000000000000000000000|11111111111111111111111|
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Size = 2)]
    public struct PixelDataGray16 : INonIndexedPixel
    {
        // raw component values
        [FieldOffset(0)] private ushort gray;   // 00 - 15

        // processed raw values
        public static int Gray => (0xFF >> 8) & 0xF;
        public readonly int Alpha => 0xFF;
        public readonly int Red => Gray;
        public readonly int Green => Gray;
        public readonly int Blue => Gray;

        /// <summary>
        /// See <see cref="INonIndexedPixel.Argb"/> for more details.
        /// </summary>
        public readonly int Argb => (Pixel.AlphaMask) | Red << Pixel.RedShift | Green << Pixel.GreenShift | Blue;

        /// <summary>
        /// See <see cref="INonIndexedPixel.GetColor"/> for more details.
        /// </summary>
        public readonly Color GetColor()
        {
            return Color.FromArgb(Argb);
        }

        /// <summary>
        /// See <see cref="INonIndexedPixel.SetColor"/> for more details.
        /// </summary>
        public void SetColor(Color color)
        {
            int argb = color.ToArgb() & Pixel.RedGreenBlueMask;
            gray = (byte) (argb >> Pixel.RedShift);
        }

        /// <summary>
        /// See <see cref="INonIndexedPixel.Value"/> for more details.
        /// </summary>
        public ulong Value
        {
            readonly get => gray;
            set => gray = (ushort)(value & 0xFFFF);
        }
    }
}
