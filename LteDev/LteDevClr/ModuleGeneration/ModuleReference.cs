using System;
using System.Text;
using System.Xml.Serialization;

namespace LteDevClr.ModuleGeneration
{
    [Serializable]
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
}