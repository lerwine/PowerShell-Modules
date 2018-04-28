using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;

namespace PSModuleInstallUtil
{
    public class ModuleStatusDataSource
    {
        private DirectoryInfo _basePath;
        private List<ModuleStatus> _moduleDirectories = null;
    }
}
