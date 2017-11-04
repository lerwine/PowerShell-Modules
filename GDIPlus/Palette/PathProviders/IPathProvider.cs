using System;
using System.Collections.Generic;
using System.Drawing;

namespace Erwine.Leonard.T.GDIPlus.Palette.PathProviders
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public interface IPathProvider
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {
        /// <summary>
        /// Retrieves the path throughout the image to determine the order in which pixels will be scanned.
        /// </summary>
        IList<Point> GetPointPath(Int32 width, Int32 height);
    }
}
