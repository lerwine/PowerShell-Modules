#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Erwine.Leonard.T.GDIPlus.Palette.ColorCaches
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    public interface IColorCache
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
        void GetColorPaletteIndex(Color color, out int paletteIndex);
    }
}
