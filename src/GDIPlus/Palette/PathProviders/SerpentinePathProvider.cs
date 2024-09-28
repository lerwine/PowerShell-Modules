#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Erwine.Leonard.T.GDIPlus.Palette.PathProviders
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    public class SerpentinePathProvider : IPathProvider
    {
        public IList<Point> GetPointPath(int width, int height)
        {
            bool leftToRight = true;
            List<Point> result = new(width * height);

            for (int y = 0; y < height; y++)
            {
                for (int x = leftToRight ? 0 : width - 1; leftToRight ? x < width : x >= 0; x += leftToRight ? 1 : -1)
                {
                    Point point = new(x, y);
                    result.Add(point);
                }

                leftToRight = !leftToRight;
            }

            return result;
        }
    }
}
