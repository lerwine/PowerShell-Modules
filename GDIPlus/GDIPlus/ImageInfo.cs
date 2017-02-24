using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Erwine.Leonard.T.WinForms
{
    public class ImageInfo
    {
        private string _name = "";
        private string _extension = "";
        private string _directoryName;
        private bool _exists = false;
        private bool _isReadOnly = true;
        private long _length = 0L;
        private DateTime _creationTime;
        private DateTime _lastWriteTime;
        private FileAttributes _attributes;
		private int _width = 0;
		private int _height = 0;
		private float _horizontalResolution = 0.0f;
		private float _verticalResolution = 0.0f;
		private PixelFormat _pixelFormat;
		private ImageType _imageType;
		private int _flags = 0;
		
        public string Name { get { return _name; } set { _name = (value == null) ? "" : value; } }
        public string Extension
		{
			get { return _extension; }
			set
			{
				string s = (value == null) ? "" : value;
				if (s.Length == 0 || s.StartsWith("."))
					_extension = s;
				else
					_extension = "." + s;
			}
		}
        public string DirectoryName { get { return _directoryName; } set { _directoryName = (value == null) ? "" : value; } }
        public bool Exists { get { return _exists; } set { _exists = value; } }
        public bool IsReadOnly { get { return _isReadOnly; } set { _isReadOnly = value; } }
        public long Length { get { return _length; } set { _length = (value < 0L) ? 0L : value; } }
        public DateTime CreationTime { get { return _creationTime; } set { _creationTime = value; } }
        public DateTime LastWriteTime { get { return _lastWriteTime; } set { _lastWriteTime = value; } }
        public FileAttributes Attributes { get { return _attributes; } set { _attributes = value; } }
		public int Width { get { return _width; } set { _width = (value < 0) ? 0 : value; } }
		public int Height { get { return _height; } set { _height = (value < 0) ? 0 : value; } }
		public float HorizontalResolution { get { return _horizontalResolution; } set { _horizontalResolution = (value < 0.0f) ? 0.0f : value; } }
		public float VerticalResolution { get { return _verticalResolution; } set { _verticalResolution = (value < 0.0f) ? 0.0f : value; } }
		public PixelFormat PixelFormat { get { return _pixelFormat; } set { _pixelFormat = value; } }
		public ImageType ImageType { get { return _imageType; } set { _imageType = value; } }
		public int Flags { get { return _flags; } set { _flags = value; } }

		public static ImageType RawFormatToImageType(ImageFormat rawFormat)
		{
			if (rawFormat == null)
				return ImageType.Unknown;
			
			if (rawFormat.Guid.Equals(ImageFormat.Bmp.Guid))
				return ImageType.Bmp;
			
			if (rawFormat.Guid.Equals(ImageFormat.Emf.Guid))
				return ImageType.Emf;
			
			if (rawFormat.Guid.Equals(ImageFormat.Wmf.Guid))
				return ImageType.Wmf;
			
			if (rawFormat.Guid.Equals(ImageFormat.Gif.Guid))
				return ImageType.Gif;
			
			if (rawFormat.Guid.Equals(ImageFormat.Jpeg.Guid))
				return ImageType.Jpeg;
			
			if (rawFormat.Guid.Equals(ImageFormat.Png.Guid))
				return ImageType.Png;
			
			if (rawFormat.Guid.Equals(ImageFormat.Tiff.Guid))
				return ImageType.Tiff;
			
			if (rawFormat.Guid.Equals(ImageFormat.Exif.Guid))
				return ImageType.Exif;
			
			if (rawFormat.Guid.Equals(ImageFormat.Icon.Guid))
				return ImageType.Icon;
			
			return ImageType.Unknown;
		}
		
        public ImageInfo() { }
		
        public ImageInfo(FileInfo file, Bitmap bitmap)
		{
			if (file == null)
				throw new ArgumentNullException("file");
			
			SetName(file.Name, null);
			_directoryName = file.DirectoryName;
			_exists = file.Exists;
			if (_exists)
			{
				_length = file.Length;
				_isReadOnly = file.IsReadOnly;
				_creationTime = file.CreationTime;
				_lastWriteTime = file.LastWriteTime;
				_attributes = file.Attributes;
			}
			
			if (bitmap == null)
				return;
			
			_width = bitmap.Width;
			_height = bitmap.Height;
			_horizontalResolution = bitmap.HorizontalResolution;
			_verticalResolution = bitmap.VerticalResolution;
			_pixelFormat = bitmap.PixelFormat;
			_imageType = RawFormatToImageType(bitmap.RawFormat);
			_flags = bitmap.Flags;
		}
		
		private void SetName(string name, string extension)
		{
			if (String.IsNullOrEmpty(name))
			{
				if (String.IsNullOrEmpty(extension))
				{
					_name = "";
					_extension = "";
					return;
				}
				
				name = extension;
			}
			else if (!String.IsNullOrEmpty(extension))
			{
				if (extension.StartsWith("."))
					name += (name.EndsWith(".")) ? extension.Substring(1) : extension;
				else
					name += (name.EndsWith(".")) ? extension : ("." + extension);
			}
			
			_extension = (name.EndsWith(".")) ? "." : Path.GetExtension(name);
			_name = Path.GetFileNameWithoutExtension(name);
		}
    }
}
