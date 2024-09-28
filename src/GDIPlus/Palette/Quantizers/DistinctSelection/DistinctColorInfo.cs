#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Erwine.Leonard.T.GDIPlus.Palette.Quantizers.DistinctSelection
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    /// <summary>
    /// Stores all the informations about single color only once, to be used later.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="DistinctColorInfo"/> struct.
    /// </remarks>
    public class DistinctColorInfo(Color color)
    {
        private const int Factor = 5000000;

        /// <summary>
        /// The original color.
        /// </summary>
        public int Color { get; private set; } = color.ToArgb();

        /// <summary>
        /// The pixel presence count in the image.
        /// </summary>
        public int Count { get; private set; } = 1;

        /// <summary>
        /// A hue component of the color.
        /// </summary>
        public int Hue { get; private set; } = Convert.ToInt32(color.GetHue() * Factor);

        /// <summary>
        /// A saturation component of the color.
        /// </summary>
        public int Saturation { get; private set; } = Convert.ToInt32(color.GetSaturation() * Factor);

        /// <summary>
        /// A brightness component of the color.
        /// </summary>
        public int Brightness { get; private set; } = Convert.ToInt32(color.GetBrightness() * Factor);

        /// <summary>
        /// Increases the count of pixels of this color.
        /// </summary>
        public DistinctColorInfo IncreaseCount()
        {
            Count++;
            return this;
        }
    }
}
