#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Erwine.Leonard.T.GDIPlus
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    public enum FileType
    {
        Unknown,
        [FileExtensionMap(".bmp")]
        Bmp,
        Emf,
        Wmf,
        [FileExtensionMap(".gif")]
        Gif,
        [FileExtensionMap(".jpg",IsPrimary = true)]
        [FileExtensionMap(".jpeg")]
        Jpeg,
        [FileExtensionMap(".png")]
        Png,
        [FileExtensionMap(".tif", IsPrimary = true)]
        [FileExtensionMap(".tiff")]
        Tiff,
        Exif,
        [FileExtensionMap(".icos")]
        Icon
    }
}
