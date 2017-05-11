using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace LteDevClr.ModuleGeneration
{
    [Serializable]
    [XmlRoot(ElementName_Command)]
    public class CommandDefinition
    {
        public const string ElementName_Command = "Command";
    }
}