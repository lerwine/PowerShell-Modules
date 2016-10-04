using System;
using System.Collections.Generic;
using System.IO;
#if !PSLEGACY
using System.Linq;
#endif
using System.Text;
#if !PSLEGACY
using System.Threading.Tasks;
#endif
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
            writer.WriteStartElement(RootElementName, NamespaceURI);
            try
            {
                writer.WriteAttributeString(AttributeName, AttributeValue);
#if PSLEGACY
                foreach (Command command in Commands.ToArray())
                {
                    if (command != null)
#else
                foreach (Command command in Commands.Where(c => c != null).ToArray())
#endif
                    command.WriteXml(writer);
#if PSLEGACY
                }
#endif
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
