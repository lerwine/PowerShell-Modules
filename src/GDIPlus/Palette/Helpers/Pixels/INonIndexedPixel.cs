#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Erwine.Leonard.T.GDIPlus.Palette.Helpers.Pixels
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    public interface INonIndexedPixel
    {
        // components
        int Alpha { get; }
        int Red { get; }
        int Green { get; }
        int Blue { get; }

        // higher-level values
        int Argb { get; }
        ulong Value { get; set; }

        // color methods
        Color GetColor();
        void SetColor(Color color);
    }
}
