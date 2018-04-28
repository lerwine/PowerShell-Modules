using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace LteDev.XsdTypeGen.command
{
#pragma warning disable 1591 // Missing XML comment for publicly visible type or member

    [Serializable()]
    [XmlRoot("terminatingErrors", Namespace = Constants.Xmlns_command)]
    public class terminatingErrors
    {
        // TODO: Reference from command/command.cs: command:terminatingErrors
    }
    
#pragma warning restore 1591 // Missing XML comment for publicly visible type or member
}