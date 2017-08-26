using System;
using System.Drawing;

namespace Erwine.Leonard.T.GDIPlus.Palette.Helpers.Pixels
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public interface INonIndexedPixel
    {
        // components
        Int32 Alpha { get; }
        Int32 Red { get; }
        Int32 Green { get; }
        Int32 Blue { get; }

        // higher-level values
        Int32 Argb { get; }
        UInt64 Value { get; set; }

        // color methods
        Color GetColor();
        void SetColor(Color color);
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
