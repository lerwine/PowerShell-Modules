using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace LteDev.ModuleGeneration
{
    [Serializable]
    [XmlRoot(ElementName_Command)]
#pragma warning disable 1591 // Missing XML comment for publicly visible type or member
    public class CommandDefinition
    {
        public const string ElementName_Command = "Command";
    }
#pragma warning restore 1591 // Missing XML comment for publicly visible type or member
}