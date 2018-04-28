namespace Erwine.Leonard.T.GDIPlus.Palette.Helpers
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class PixelTransform
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {
        /// <summary>
        /// Gets the source pixel.
        /// </summary>
        public Pixel SourcePixel { get; private set; }

        /// <summary>
        /// Gets the target pixel.
        /// </summary>
        public Pixel TargetPixel { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PixelTransform"/> class.
        /// </summary>
        public PixelTransform(Pixel sourcePixel, Pixel targetPixel)
        {
            SourcePixel = sourcePixel;
            TargetPixel = targetPixel;
        }
    }
}
