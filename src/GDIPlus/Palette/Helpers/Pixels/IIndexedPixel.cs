#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Erwine.Leonard.T.GDIPlus.Palette.Helpers.Pixels
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    public interface IIndexedPixel
    {
        // index methods
        byte GetIndex(int offset);
        void SetIndex(int offset, byte value);
    }
}
