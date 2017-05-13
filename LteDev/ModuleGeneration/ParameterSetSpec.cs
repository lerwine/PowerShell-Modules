using System.Xml.Serialization;

namespace LteDev.ModuleGeneration
{
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

        public int _position = -1;
    }
}