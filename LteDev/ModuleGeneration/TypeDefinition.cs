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
    public class TypeDefinition
    {
        public const string ElementName_Type = "Type";
    }
}