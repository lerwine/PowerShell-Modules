using System;
using System.Collections.Generic;
using System.Drawing;

namespace Erwine.Leonard.T.GDIPlus.Palette.ColorCaches.LocalitySensitiveHash
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class BucketInfo
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {
        private readonly SortedDictionary<Int32, Color> colors;

        /// <summary>
        /// Gets the colors.
        /// </summary>
        /// <value>The colors.</value>
        public IDictionary<Int32, Color> Colors
        {
            get { return colors; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BucketInfo"/> class.
        /// </summary>
        public BucketInfo()
        {
            colors = new SortedDictionary<Int32, Color>();
        }

        /// <summary>
        /// Adds the color to the bucket informations.
        /// </summary>
        /// <param name="paletteIndex">Index of the palette.</param>
        /// <param name="color">The color.</param>
        public void AddColor(Int32 paletteIndex, Color color)
        {
            colors.Add(paletteIndex, color);
        }
    }
}
