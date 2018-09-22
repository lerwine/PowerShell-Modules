using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace UnitTests.PsHelp
{
    public static class helpItems
    {
        public const string XmlnsURI_msh = "http://msh";
        public const string Xmlns_RootElementName = "helpItems";
        public const string AttributeName_schema = "schema";
        public const string XmlnsPrefix_MSHelp = "MSHelp";
        public const string XmlnsURI_MSHelp = "http://msdn.microsoft.com/mshelp";
        public const string HelpFileNameAppend = "-Help.xml";
        public static XmlDocument CreateModuleHelp(TestContext testContext, PSModuleInfo moduleInfo, params string[] relativeModulePaths)
        {
            string path = moduleInfo.Path + HelpFileNameAppend;
            XmlDocument helpItemsDocument = new XmlDocument();
            if (File.Exists(path))
                try { helpItemsDocument.Load(path); } catch { }
            XmlElement helpItemsElement = GetRootElement(helpItemsDocument);
            if (helpItemsElement == null)
            {
                if (helpItemsDocument.DocumentElement != null)
                    helpItemsDocument.RemoveChild(helpItemsDocument.DocumentElement);
                helpItemsElement = helpItemsDocument.AppendChild(helpItemsDocument.CreateElement(Xmlns_RootElementName, XmlnsURI_msh)) as XmlElement;
            }

            helpItemsElement.SetAttributeValue(AttributeName_schema, "maml");
            helpItemsElement.AddXmlnsAttribute(maml.Xmlns_Prefix, maml.Xmlns_URI);
            helpItemsElement.AddXmlnsAttribute(command.Xmlns_Prefix, command.Xmlns_URI);
            helpItemsElement.AddXmlnsAttribute(dev.Xmlns_Prefix, dev.Xmlns_URI);
            helpItemsElement.AddXmlnsAttribute(XmlnsPrefix_MSHelp, XmlnsURI_MSHelp);

            XmlElement rootElement = GetRootElement(helpItemsDocument);
            command.UpdateHelp(testContext, helpItemsElement, moduleInfo, relativeModulePaths);

            return helpItemsDocument;
        }

        public static XmlElement GetRootElement(XmlDocument document)
        {
            return document.GetChildElements(Xmlns_RootElementName, XmlnsURI_msh).FirstOrDefault();
        }
    }
}
