using System;

namespace Erwine.Leonard.T.GDIPlus
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
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
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
