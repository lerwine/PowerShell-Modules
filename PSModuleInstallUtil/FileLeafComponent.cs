using System.ComponentModel;
using System.IO;

namespace PSModuleInstallUtil
{
    public class FileLeafComponent : FileSystemComponent
    {
        public static FileLeafComponent Create(FileInfo file)
        {
            if (file == null)
                return null;

            return new FileLeafComponent(file.Name, FileDirectoryComponent.Create(file.Directory));
        }

        public FileLeafComponent(string name, IContainer parent) : base(name, parent) { }

        public FileLeafComponent(string name) : base(name) { }

        public FileLeafComponent(IContainer parent) : base(parent) { }

        public FileLeafComponent() : base() { }

    }
}