using System;
using System.Collections.Generic;
using System.Drawing;

namespace Erwine.Leonard.T.GDIPlus.Palette.ColorCaches
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public interface IColorCache
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {
        /// <summary>
        /// Prepares color cache for next use.
        /// </summary>
        void Prepare();

        /// <summary>
        /// Called when a palette is about to be cached, or precached.
        /// </summary>
        /// <param name="palette">The palette.</param>
        void CachePalette(IList<Color> palette);

        /// <summary>
        /// Called when palette index is about to be retrieve for a given color.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <param name="paletteIndex">Index of the palette.</param>
        void GetColorPaletteIndex(Color color, out Int32 paletteIndex);
    }
}
