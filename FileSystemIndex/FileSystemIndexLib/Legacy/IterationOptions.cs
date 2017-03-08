using System;
using System.Xml.Serialization;

namespace FileSystemIndexLib
{
    [Serializable]
    [XmlRoot(ElementName = ElementName_IterationOptions, Namespace = XmlSerialization_NamespaceUri)]
    public class IterationOptions
    {
        /// <summary>
        /// XML Namespace URI used for XML serialization.
        /// </summary>
        public const string XmlSerialization_NamespaceUri = "urn:Erwine.Leonard.T:PowerShellModules/FileSystemIndex/IterationOptions.xsd";

        /// <summary>
        /// Root XML element name when serializing the root <see cref="IterationState"/> object.
        /// </summary>
        public const string ElementName_IterationOptions = "IterationOptions";

        public IterationItemType ItemType { get; set; }

        public int MaxDepth { get; set; }

        public string FileFilter { get; set; }

        public string SubdirectoryFilter { get; set; }

        public bool IncludeHiddenFiles { get; set; }

        public bool IncludeHiddenDirectories { get; set; }

        public bool IncludeSystemFiles { get; set; }

        public bool IncludeSystemDirectories { get; set; }

        public bool ExcludeNonArchiveFiles { get; set; }

        public bool ExcludeNonArchiveDirectories { get; set; }
    }
}