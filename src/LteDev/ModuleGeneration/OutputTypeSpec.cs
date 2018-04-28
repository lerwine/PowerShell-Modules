using System;
using System.Xml;
using System.Xml.Serialization;

namespace LteDev.ModuleGeneration
{
    [Serializable]
#pragma warning disable 1591 // Missing XML comment for publicly visible type or member
    public class OutputTypeSpec
    {
        [XmlAttribute]
        public string TypeName { get; set; }

        [XmlAttribute]
        public string ParameterSetName { get; set; }

        internal void WriteReturnValue(XmlWriter writer)
        {
#warning Not implemented
            throw new NotImplementedException();
        }
    }
#pragma warning restore 1591 // Missing XML comment for publicly visible type or member
}