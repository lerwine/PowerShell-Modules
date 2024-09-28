using System.Runtime.InteropServices;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Erwine.Leonard.T.GDIPlus.Palette.Helpers.Pixels.Indexed
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct PixelData8Indexed : IIndexedPixel
    {
        // raw component values
        private byte index;

        // index methods 
        public readonly byte GetIndex(int offset) { return index; }
        public void SetIndex(int offset, byte value) { index = value; }
    }
}
