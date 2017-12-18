using System.Xml.Serialization;

namespace LteDev.ModuleGeneration
{
#pragma warning disable 1591 // Missing XML comment for publicly visible type or member
    public class ParameterSetSpec
    {
        [XmlAttribute]
        public string Name { get; set; }

        [XmlAttribute]
        public bool Mandatory { get; set; }

        [XmlAttribute]
        public bool ValueFromPipeline { get; set; }

        [XmlAttribute]
        public bool ValueFromPipelineByPropertyName { get; set; }

        [XmlAttribute]
        public int Position { get { return _position; } set { _position = (value < 0) ? -1 : value; } }

        private int _position = -1;
    }
#pragma warning restore 1591 // Missing XML comment for publicly visible type or member
}