using System;
using System.Collections.Generic;
using System.Drawing;

namespace Erwine.Leonard.T.GDIPlus.Palette.PathProviders
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class ReversedPathProvider : IPathProvider
    {
        public IList<Point> GetPointPath(Int32 width, Int32 height)
        {
            List<Point> result = new List<Point>(width*height);

            for (Int32 y = height - 1; y >= 0; y--)
            for (Int32 x = width - 1; x >= 0; x--)
            {
                Point point = new Point(x, y);
                result.Add(point);
            }

            return result;
        }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
