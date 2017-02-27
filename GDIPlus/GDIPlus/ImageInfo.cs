using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace Erwine.Leonard.T.GDIPlus
{
    public class ImageInfo : FileItem
    {
        private bool _exists = false;
        private bool _isReadOnly = true;
        private DateTime _creationTime;
        private DateTime _lastWriteTime;
        private FileAttributes _attributes;
		private int _width = 0;
		private int _height = 0;
		private float _horizontalResolution = 0.0f;
		private float _verticalResolution = 0.0f;
		private PixelFormat _pixelFormat;
		private int _flags = 0;
        private bool _hasAlpha = false;
        private bool _isIndexed = false;
        private ColorSpaceType _colorSpace;

        public bool Exists { get { return _exists; } set { _exists = value; } }

        public bool IsReadOnly { get { return _isReadOnly; } set { _isReadOnly = value; } }

        public DateTime CreationTime { get { return _creationTime; } set { _creationTime = value; } }

        public DateTime LastWriteTime { get { return _lastWriteTime; } set { _lastWriteTime = value; } }

        public FileAttributes Attributes { get { return _attributes; } set { _attributes = value; } }

		public int Width { get { return _width; } set { _width = (value < 0) ? 0 : value; } }

		public int Height { get { return _height; } set { _height = (value < 0) ? 0 : value; } }

		public float HorizontalResolution { get { return _horizontalResolution; } set { _horizontalResolution = (value < 0.0f) ? 0.0f : value; } }

		public float VerticalResolution { get { return _verticalResolution; } set { _verticalResolution = (value < 0.0f) ? 0.0f : value; } }

		public PixelFormat PixelFormat { get { return _pixelFormat; } set { _pixelFormat = value; } }

		public int Flags { get { return _flags; } set { _flags = value; } }

        public bool HasAlpha { get { return _hasAlpha; } set { _hasAlpha = value; } }

        public bool IsIndexed { get { return _isIndexed; } set { _isIndexed = value; } }

        public ColorSpaceType ColorSpace { get { return _colorSpace; } set { _colorSpace = value; } }

        public static FileType RawFormatToImageType(ImageFormat rawFormat)
		{
			if (rawFormat == null)
				return FileType.Unknown;
			
			if (rawFormat.Guid.Equals(ImageFormat.Bmp.Guid))
				return FileType.Bmp;
			
			if (rawFormat.Guid.Equals(ImageFormat.Emf.Guid))
				return FileType.Emf;
			
			if (rawFormat.Guid.Equals(ImageFormat.Wmf.Guid))
				return FileType.Wmf;
			
			if (rawFormat.Guid.Equals(ImageFormat.Gif.Guid))
				return FileType.Gif;
			
			if (rawFormat.Guid.Equals(ImageFormat.Jpeg.Guid))
				return FileType.Jpeg;
			
			if (rawFormat.Guid.Equals(ImageFormat.Png.Guid))
				return FileType.Png;
			
			if (rawFormat.Guid.Equals(ImageFormat.Tiff.Guid))
				return FileType.Tiff;
			
			if (rawFormat.Guid.Equals(ImageFormat.Exif.Guid))
				return FileType.Exif;
			
			if (rawFormat.Guid.Equals(ImageFormat.Icon.Guid))
				return FileType.Icon;
			
			return FileType.Unknown;
		}

        public ImageInfo() { }

        public ImageInfo(FileInfo file, Bitmap bitmap)
            : base(file)
        {
            if (file == null)
                throw new ArgumentNullException("file");

            _exists = file.Exists;
            if (_exists)
            {
                _isReadOnly = file.IsReadOnly;
                _creationTime = file.CreationTime;
                _lastWriteTime = file.LastWriteTime;
                _attributes = file.Attributes;
            }

            if (bitmap == null)
            {
                FileType = ExtensionToFileType(Extension);
                return;
            }

            _width = bitmap.Width;
            _height = bitmap.Height;
            _horizontalResolution = bitmap.HorizontalResolution;
            _verticalResolution = bitmap.VerticalResolution;
            _pixelFormat = bitmap.PixelFormat;
            FileType = RawFormatToImageType(bitmap.RawFormat);
            _flags = bitmap.Flags;

            _hasAlpha = (bitmap.Flags & (int)(ImageFlags.HasAlpha)) != 0;
            _isIndexed = bitmap.PixelFormat.HasFlag(PixelFormat.Indexed);
            
            if ((bitmap.Flags & (int)(ImageFlags.ColorSpaceCmyk)) != 0)
                _colorSpace = ColorSpaceType.Cmyk;
            else if ((bitmap.Flags & (int)(ImageFlags.ColorSpaceYcbcr)) != 0)
                _colorSpace = ColorSpaceType.Ycbcr;
            else if ((bitmap.Flags & (int)(ImageFlags.ColorSpaceYcck)) != 0)
                _colorSpace = ColorSpaceType.Ycck;
            else if ((bitmap.Flags & (int)(ImageFlags.ColorSpaceRgb)) != 0)
                _colorSpace = ColorSpaceType.Rgb;
            else if ((bitmap.Flags & (int)(ImageFlags.ColorSpaceGray)) != 0)
                _colorSpace = ColorSpaceType.Gray;
            else
                _colorSpace = ColorSpaceType.Unknown;
        }

        public ImageInfo(ImageInfo item)
            : base(item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            _exists = item._exists;
            _isReadOnly = item._isReadOnly;
            _creationTime = item._creationTime;
            _lastWriteTime = item._lastWriteTime;
            _attributes = item._attributes;
            _width = item._width;
            _height = item._height;
            _horizontalResolution = item._horizontalResolution;
            _verticalResolution = item._verticalResolution;
            _pixelFormat = item._pixelFormat;
            _flags = item._flags;
            _hasAlpha = item._hasAlpha;
            _isIndexed = item._isIndexed;
            _colorSpace = item._colorSpace;
        }
    }
}
