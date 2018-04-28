using System;

namespace Erwine.Leonard.T.GDIPlus.Palette.Helpers.Pixels
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public interface IIndexedPixel
    {
        // index methods
        Byte GetIndex(Int32 offset);
        void SetIndex(Int32 offset, Byte value);
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
