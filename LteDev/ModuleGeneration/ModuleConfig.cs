using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace LteDev.ModuleGeneration
{
    [Serializable]
    public class ModuleConfig
    {
        private string _author = null;
        public const string AttributeName_Author = "Author";
        [XmlAttribute(AttributeName_Author)]
        public string Author
        {
            get { return _author; }
            set { _author = (value != null && value.Length > 0) ? value : null; }
        }
        private Version _powerShellVersion = null;
        [XmlIgnore()]
        public Version PowerShellVersion { get { return _powerShellVersion; } set { _powerShellVersion = value; } }
        public const string AttributeName_PowerShellVersion = "PowerShellVersion";
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string PowerShellVersionString
        {
            get { return (_powerShellVersion == null) ? null : ((_powerShellVersion.Revision == 0) ? _powerShellVersion.ToString((_powerShellVersion.Build == 0) ? 2 : 3) : _powerShellVersion.ToString()); }
            set
            {
                string s;
                if (value == null || (s = value.Trim()).Length == 0 || !Version.TryParse(s, out _powerShellVersion))
                    _powerShellVersion = null;
            }
        }
        private string _copyright = "";
        public const string ElementName_Copyright = "Copyright";
        [XmlElement(ElementName_Copyright)]
        public string Copyright
        {
            get { return _copyright; }
            set { _copyright = value ?? ""; }
        }

        private Collection<CommandDefinition> _commands = new Collection<CommandDefinition>();
        [XmlElement(CommandDefinition.ElementName_Command)]
        public Collection<CommandDefinition> Commands
        {
            get { return _commands; }
            set { _commands = value ?? new Collection<CommandDefinition>(); }
        }

        private Collection<TypeDefinition> _types = new Collection<TypeDefinition>();
        [XmlElement(TypeDefinition.ElementName_Type)]
        public Collection<TypeDefinition> Types
        {
            get { return _types; }
            set { _types = value ?? new Collection<TypeDefinition>(); }
        }
    }
}
