using System;
using System.Xml;
using System.Xml.Serialization;

namespace LteDevClr.ModuleGeneration
{
    [Serializable]
    public class OutputTypeSpec
    {
        [XmlAttribute]
        public string TypeName { get; set; }

        [XmlAttribute]
        public string ParameterSetName { get; set; }

        internal void WriteReturnValue(XmlWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}