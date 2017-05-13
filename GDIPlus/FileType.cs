using System;

namespace Erwine.Leonard.T.GDIPlus
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
