using System.IO;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Erwine.Leonard.T.GDIPlus
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    public class FileItem : FileBase
    {
        private string _directoryName;
        private long _length = 0L;

        public string DirectoryName { get => _directoryName; set => _directoryName = (value == null) ? "" : value; }

        public long Length { get => _length; set => _length = (value < 0L) ? 0L : value; }

        public FileItem() { }

        public FileItem(FileInfo file) : base(file)
        {
            ArgumentNullException.ThrowIfNull(file);

            _directoryName = file.DirectoryName;
            if (file.Exists)
                _length = file.Length;
        }

        public FileItem(FileItem item)
            : base(item)
        {
            ArgumentNullException.ThrowIfNull(item);

            _directoryName = item._directoryName;
            _length = item._length;
        }

        public override string GetFullName()
        {
            return Path.Combine(DirectoryName, Name + Extension);
        }
    }
}