using System.Runtime.InteropServices;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Erwine.Leonard.T.GDIPlus.Palette.Helpers.Pixels.NonIndexed
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    /// <summary>
    /// Name |     Blue     |    Green     |     Red      | Unused
    /// Bit  |00|01|02|03|04|05|06|07|08|09|10|11|12|13|14|15|
    /// Byte |00000000000000000000000|11111111111111111111111|
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Size = 2)]
    public struct PixelDataRgb555 : INonIndexedPixel
    {
        // raw component values
        [FieldOffset(0)] private byte blue;     // 00 - 04
        [FieldOffset(0)] private ushort green;  // 05 - 09
        [FieldOffset(1)] private byte red;      // 10 - 14

        // raw high-level values
        [FieldOffset(0)] private ushort raw;    // 00 - 15

        // processed component values
        public readonly int Alpha => 0xFF;
        public readonly int Red => (red >> 2) & 0xF;
        public readonly int Green => (green >> 5) & 0xF;
        public readonly int Blue => blue & 0xF;

        /// <summary>
        /// See <see cref="INonIndexedPixel.Argb"/> for more details.
        /// </summary>
        public readonly int Argb => Pixel.AlphaMask | raw;

        /// <summary>
        /// See <see cref="INonIndexedPixel.GetColor"/> for more details.
        /// </summary>
        public readonly Color GetColor() => Color.FromArgb(Argb);

        /// <summary>
        /// See <see cref="INonIndexedPixel.SetColor"/> for more details.
        /// </summary>
        public void SetColor(Color color)
        {
            red = (byte) (color.R >> 3);
            green = (byte) (color.G >> 3);
            blue = (byte) (color.B >> 3);
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
