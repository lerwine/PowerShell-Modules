using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace UnitTests.ModuleConformance
{
    public class CommandNode
    {
        private XmlElement _element;

        public string Name { get { return _element.GetNonEmptyText("details", ExtensionMethods.Xmlns_command, "command", ExtensionMethods.Xmlns_command).FirstOrDefault(); } }

        private CommandNode(XmlElement element) { _element = element; }

        public static IEnumerable<CommandNode> GetCommandNodes(XmlDocument xmlHelp)
        {
            XmlElement helpItems = xmlHelp.GetHelpItemsElement();
            if (helpItems == null)
                return new CommandNode[0];

            XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlHelp.NameTable);
            nsmgr.AddNamespace("command", ExtensionMethods.Xmlns_command);
            return helpItems.SelectNodes("command:command[not(count(command:details/command:name.text())=0)]").OfType<XmlElement>().Select(e => new CommandNode(e));
        }
    }
}