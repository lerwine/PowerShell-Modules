using System;
using Erwine.Leonard.T.GDIPlus.Palette.Helpers;
using Erwine.Leonard.T.GDIPlus.Palette.PathProviders;
using Erwine.Leonard.T.GDIPlus.Palette.Quantizers;

namespace Erwine.Leonard.T.GDIPlus.Palette.Ditherers
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public interface IColorDitherer : IPathProvider
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {
        /// <summary>
        /// Gets a value indicating whether this ditherer uses only actually process pixel.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this ditherer is inplace; otherwise, <c>false</c>.
        /// </value>
        Boolean IsInplace { get; }

        /// <summary>
        /// Prepares this instance.
        /// </summary>
        void Prepare(IColorQuantizer quantizer, Int32 colorCount, ImageBuffer sourceBuffer, ImageBuffer targetBuffer);

        /// <summary>
        /// Processes the specified buffer.
        /// </summary>
        Boolean ProcessPixel(Pixel sourcePixel, Pixel targetPixel);

        /// <summary>
        /// Finishes this instance.
        /// </summary>
        void Finish();
    }
}
