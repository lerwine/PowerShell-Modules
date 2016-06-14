using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace LteDevClr.HelpXml
{
    [XmlRoot(HelpItems.RootElementName, IsNullable = false, Namespace = HelpItems.NamespaceURI)]
    [Serializable]
    public class HelpItems : IXmlSerializable
    {
        public const string RootElementName = "helpItems";
        public const string NamespaceURI = "http://msh";
        public const string AttributeName = "schema";
        public const string AttributeValue = "maml";
        private List<Command> _commands = new List<Command>();

        public string Schema { get { return HelpItems.AttributeValue; } }

        public List<Command> Commands
        {
            get { return this._commands; }
            set { this._commands = value ?? new List<Command>(); }
        }

        public XmlSchema GetSchema()
        {
            using (StringReader reader = new StringReader(XmlSchemas.helpItems))
                return XmlSchema.Read(reader, null);
        }

        public void ReadXml(XmlReader reader)
        {
            if (reader.LocalName != HelpItems.RootElementName || reader.NamespaceURI != HelpItems.NamespaceURI)
                throw new XmlSchemaValidationException("Invalid element name.");

            List<Command> commands = new List<Command>();
            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Attribute:
                        if (reader.Name != HelpItems.AttributeName || reader.NamespaceURI != "" || reader.Value != HelpItems.AttributeValue)
                            throw new XmlSchemaValidationException("Invalid attribute.");
                        break;
                    case XmlNodeType.Element:
                        Command command = new Command();
                        command.ReadXml(reader);
                        commands.Add(command);
                        break;
                }
            }
            this._commands = commands;
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement(HelpItems.RootElementName, HelpItems.NamespaceURI);
            try
            {
                writer.WriteAttributeString(HelpItems.AttributeName, HelpItems.AttributeValue);
                foreach (Command command in this.Commands.Where(c => c != null).ToArray())
                    command.WriteXml(writer);
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
