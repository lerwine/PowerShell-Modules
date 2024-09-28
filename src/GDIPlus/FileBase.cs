using System.IO;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Erwine.Leonard.T.GDIPlus
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    public abstract class FileBase
    {
        private string _name = "";
        private string _extension = "";
        private FileType? _fileType;
        public string Name { get => _name; set => _name = (value == null) ? "" : value; }
        public string Extension
        {
            get => _extension;
            set
            {
                string s = (value == null) ? "" : value;
                if (s.Length == 0 || s.StartsWith("."))
                    _extension = s;
                else
                    _extension = "." + s;
            }
        }
        public FileType FileType
        {
            get
            {
                if (!_fileType.HasValue)
                    _fileType = ExtensionToFileType(_extension);
                return _fileType.Value;
            }
            set => _fileType = value;
        }

        public static FileType ExtensionToFileType(string extension)
        {
            if (string.IsNullOrEmpty(extension))
                return FileType.Unknown;

            if (!extension.StartsWith("."))
                extension = "." + extension;
            Type t = typeof(FileType);
            return Enum.GetValues(t).OfType<FileType>().SelectMany(e => t.GetField(Enum.GetName(t, e)).GetCustomAttributes(typeof(FileExtensionMapAttribute), false).OfType<FileExtensionMapAttribute>().Select(a => new { E = e, A = a.Extension }))
                .Where(a => string.Compare(a.A, extension, true) == 0).Select(a => a.E).DefaultIfEmpty(FileType.Unknown).First();
        }

        private void SetName(string name, string extension)
        {
            if (string.IsNullOrEmpty(name))
            {
                if (string.IsNullOrEmpty(extension))
                {
                    _name = "";
                    _extension = "";
                    return;
                }

                name = extension;
            }
            else if (!string.IsNullOrEmpty(extension))
            {
                if (extension.StartsWith("."))
                    name += name.EndsWith(".") ? extension.Substring(1) : extension;
                else
                    name += name.EndsWith(".") ? extension : ("." + extension);
            }

            _extension = name.EndsWith(".") ? "." : Path.GetExtension(name);
            _name = Path.GetFileNameWithoutExtension(name);
        }

        public abstract string GetFullName();

        protected FileBase() { }

        protected FileBase(FileInfo file)
        {
            ArgumentNullException.ThrowIfNull(file);
            SetName(file.Name, null);
        }

        protected FileBase(FileBase item)
        {
            ArgumentNullException.ThrowIfNull(item);

            _fileType = item._fileType;
            _extension = item._extension;
            _name = item._name;
        }
    }
}