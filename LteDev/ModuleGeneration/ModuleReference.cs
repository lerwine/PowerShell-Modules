using System;
using System.Text;
using System.Xml.Serialization;

namespace LteDev.ModuleGeneration
{
    [Serializable]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class ModuleReference
    {
        [XmlAttribute]
        public string ModuleName { get; set; }

        [XmlAttribute]
        public string ModuleVersion { get; set; }

        [XmlAttribute]
        public string Guid { get; set; }

        internal void WriteManifestHashValue(StringBuilder generationEnvironment)
        {
            throw new NotImplementedException();
        }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}