#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Erwine.Leonard.T.GDIPlus.Palette.Helpers
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PixelTransform"/> class.
    /// </summary>
    public class PixelTransform(Pixel sourcePixel, Pixel targetPixel)
    {
        /// <summary>
        /// Gets the source pixel.
        /// </summary>
        public Pixel SourcePixel { get; private set; } = sourcePixel;

        /// <summary>
        /// Gets the target pixel.
        /// </summary>
        public Pixel TargetPixel { get; private set; } = targetPixel;
    }
}
