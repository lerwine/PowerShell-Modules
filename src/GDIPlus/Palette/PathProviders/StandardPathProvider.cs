#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Erwine.Leonard.T.GDIPlus.Palette.PathProviders
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    public class StandardPathProvider : IPathProvider
    {
        public IList<Point> GetPointPath(int width, int height)
        {
            List<Point> result = new(width * height);

            for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
            {
                Point point = new(x, y);
                result.Add(point);
            }

            return result;
        }
    }
}
