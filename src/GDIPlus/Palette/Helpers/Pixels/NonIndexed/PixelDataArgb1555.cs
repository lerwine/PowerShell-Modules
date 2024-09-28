using System.Runtime.InteropServices;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Erwine.Leonard.T.GDIPlus.Palette.Helpers.Pixels.NonIndexed
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    /// <summary>
    /// Name |     Blue     |    Green     |     Red      | Alpha (bit)
    /// Bit  |00|01|02|03|04|05|06|07|08|09|10|11|12|13|14|15|
    /// Byte |00000000000000000000000|11111111111111111111111|
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Size = 2)]
    public struct PixelDataArgb1555 : INonIndexedPixel
    {
        // raw component values
        [FieldOffset(0)] private byte blue;     // 00 - 04
        [FieldOffset(0)] private ushort green;  // 05 - 09
        [FieldOffset(1)] private byte red;      // 10 - 14
        [FieldOffset(1)] private byte alpha;    // 15

        // raw high-level values
        [FieldOffset(0)] private ushort raw;    // 00 - 15

        // processed raw values
        public readonly int Alpha => (alpha >> 7) & 0x1;
        public readonly int Red => (red >> 2) & 0xF;
        public readonly int Green => (green >> 5) & 0xF;
        public readonly int Blue => blue & 0xF;

        /// <summary>
        /// See <see cref="INonIndexedPixel.Argb"/> for more details.
        /// </summary>
        public readonly int Argb => (Alpha == 0 ? 0 : Pixel.AlphaMask) | Red << Pixel.RedShift | Green << Pixel.GreenShift | Blue;

        /// <summary>
        /// See <see cref="INonIndexedPixel.GetColor"/> for more details.
        /// </summary>
        public readonly Color GetColor() => Color.FromArgb(Argb);

        /// <summary>
        /// See <see cref="INonIndexedPixel.SetColor"/> for more details.
        /// </summary>
        public void SetColor(Color color)
        {
            int argb = color.ToArgb();
            alpha = (argb >> Pixel.AlphaShift) > Pixel.ByteMask ? Pixel.Zero : Pixel.One;
            red = (byte) (argb >> Pixel.RedShift);
            green = (byte) (argb >> Pixel.GreenShift);
            blue = (byte) (argb >> Pixel.BlueShift);
        }

        /// <summary>
        /// See <see cref="INonIndexedPixel.Value"/> for more details.
        /// </summary>
        public ulong Value
        {
            readonly get => raw;
            set => raw = (ushort)(value & 0xFFFF);
        }
    }
}
