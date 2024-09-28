#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Erwine.Leonard.T.GDIPlus.Palette.ColorCaches.LocalitySensitiveHash
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    public class BucketInfo
    {
        private readonly SortedDictionary<int, Color> colors;

        /// <summary>
        /// Gets the colors.
        /// </summary>
        /// <value>The colors.</value>
        public IDictionary<int, Color> Colors => colors;

        /// <summary>
        /// Initializes a new instance of the <see cref="BucketInfo"/> class.
        /// </summary>
        public BucketInfo()
        {
            colors = [];
        }

        /// <summary>
        /// Adds the color to the bucket informations.
        /// </summary>
        /// <param name="paletteIndex">Index of the palette.</param>
        /// <param name="color">The color.</param>
        public void AddColor(int paletteIndex, Color color) => colors.Add(paletteIndex, color);
    }
}
