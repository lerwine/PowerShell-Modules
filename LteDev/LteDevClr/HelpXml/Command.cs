using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace LteDevClr.HelpXml
{
    [XmlRoot(Command.RootElementName, IsNullable = false, Namespace = Command.NamespaceURI_Command)]
    [Serializable]
    public class Command : IXmlSerializable
    {
        public const string RootElementName = "command";
        public const string DefaultPrefix_Command = "command";
        public const string NamespaceURI_Command = "http://schemas.microsoft.com/maml/dev/command/2004/10";
        public const string DefaultPrefix_Maml = "maml";
        public const string NamespaceURI_Maml = "http://schemas.microsoft.com/maml/2004/10";
        public const string DefaultPrefix_Dev = "dev";
        public const string NamespaceURI_Dev = "http://schemas.microsoft.com/maml/dev/2004/10";
        public const string DefaultPrefix_MSHelp = "MSHelp";
        public const string NamespaceURI_MSHelp = "http://msdn.microsoft.com/mshelp";

        public XmlSchema GetSchema()
        {
            using (StringReader reader = new StringReader(XmlSchemas.developerCommand))
                return XmlSchema.Read(reader, null);
        }

        public void ReadXml(XmlReader reader)
        {
            if (reader.LocalName != Command.RootElementName || reader.NamespaceURI != Command.NamespaceURI_Command)
                throw new XmlSchemaValidationException("Invalid element name.");
            throw new NotImplementedException();
        }

        public void WriteXml(XmlWriter writer)
        {
            string commandPrefix = writer.LookupPrefix(NamespaceURI_Command);
            if (String.IsNullOrEmpty(commandPrefix))
                commandPrefix = DefaultPrefix_Command;
            writer.WriteStartElement(commandPrefix, RootElementName, NamespaceURI_Command);
            try
            {
                string mamlPrefix = writer.LookupPrefix(NamespaceURI_Maml);
                if (mamlPrefix == null)
                {
                    mamlPrefix = DefaultPrefix_Maml;
                    writer.WriteAttributeString("xmlns", mamlPrefix, "http://www.w3.org/2000/xmlns/", NamespaceURI_Maml);
                }
                string devPrefix = writer.LookupPrefix(NamespaceURI_Dev);
                if (devPrefix == null)
                {
                    devPrefix = DefaultPrefix_Dev;
                    writer.WriteAttributeString("xmlns", devPrefix, "http://www.w3.org/2000/xmlns/", NamespaceURI_Dev);
                }
                string msHelpPrefix = writer.LookupPrefix(NamespaceURI_MSHelp);
                if (msHelpPrefix == null)
                {
                    msHelpPrefix = DefaultPrefix_MSHelp;
                    writer.WriteAttributeString("xmlns", msHelpPrefix, "http://www.w3.org/2000/xmlns/", NamespaceURI_MSHelp);
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                writer.WriteEndElement();
            }
        }
    }
}