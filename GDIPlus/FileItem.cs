using System;
using System.IO;

namespace Erwine.Leonard.T.GDIPlus
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class FileItem : FileBase
    {
        private string _directoryName;
        private long _length = 0L;

        public string DirectoryName { get { return _directoryName; } set { _directoryName = (value == null) ? "" : value; } }

        public long Length { get { return _length; } set { _length = (value < 0L) ? 0L : value; } }

        public FileItem() { }

        public FileItem(FileInfo file) : base(file)
        {
            if (file == null)
                throw new ArgumentNullException("file");

            _directoryName = file.DirectoryName;
            if (file.Exists)
                _length = file.Length;
        }

        public FileItem(FileItem item)
            : base(item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            _directoryName = item._directoryName;
            _length = item._length;
        }

        public override string GetFullName()
        {
            return Path.Combine(DirectoryName, (Name + Extension));
        }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}