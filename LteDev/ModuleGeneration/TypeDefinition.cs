using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace LteDev.ModuleGeneration
{
    [Serializable]
    [XmlRoot(ElementName_Type)]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class TypeDefinition
    {
        public const string ElementName_Type = "Type";
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}