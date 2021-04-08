using System;
using System.Runtime.InteropServices;

namespace Erwine.Leonard.T.GDIPlus.Palette.Helpers.Pixels.Indexed
{
    [StructLayout(LayoutKind.Sequential, Size = 1)]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public struct PixelData8Indexed : IIndexedPixel
    {
        // raw component values
        private Byte index;

        // index methods 
        public Byte GetIndex(Int32 offset) { return index; }
        public void SetIndex(Int32 offset, Byte value) { index = value; }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
