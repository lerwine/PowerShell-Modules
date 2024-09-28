#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Erwine.Leonard.T.GDIPlus.Palette.PathProviders
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    public class ReversedPathProvider : IPathProvider
    {
        public IList<Point> GetPointPath(int width, int height)
        {
            List<Point> result = new(width * height);

            for (int y = height - 1; y >= 0; y--)
            for (int x = width - 1; x >= 0; x--)
            {
                Point point = new(x, y);
                result.Add(point);
            }

            return result;
        }
    }
}
