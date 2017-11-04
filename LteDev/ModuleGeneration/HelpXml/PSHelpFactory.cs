using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LteDev.HelpXml
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class PSHelpFactory
    {
        public const string HelpXml_FileName_Append = "-Help.ps1xml";
        public const string HelpInfo_FileName_Append = "_HelpInfo.xml";
        public const string XmlNsUri_HelpInfo = "http://schemas.microsoft.com/powershell/help/2010/05";
        public const string XmlNsUri_msh = "http://msh";
        public const string XmlNsUri_MSHelp = "http://msdn.microsoft.com/mshelp";
        public const string XmlNsUri_managed = "http://schemas.microsoft.com/maml/dev/managed/2004/10";
        public const string helpItems = "helpItems";

        public static readonly XNamespace XmlNs_msh = XNamespace.Get(XmlNsUri_msh);
        public static readonly XNamespace XmlNs_managed = XNamespace.Get(XmlNsUri_managed);
        public static readonly XNamespace XmlNs_MSHelp = XNamespace.Get(XmlNsUri_MSHelp);

        public CommandHelpFactory Command { get; private set; }

        public MamlFactory Maml { get; private set; }

        public DevHelpFactory Dev { get; private set; }

        public PSHelpFactory()
        {
            Command = new CommandHelpFactory(this);
            Maml = new MamlFactory(this);
            Dev = new DevHelpFactory(this);
        }
        
        public XDocument CreateHelpItemsDocument(PSModuleInfo moduleInfo)
        {
            if (moduleInfo == null)
                throw new ArgumentNullException("moduleInfo");

            PSHelpFactoryContext context = new PSHelpFactoryContext(moduleInfo);
            
            return new XDocument(context.GetHelpItems(Command.GetCommandHelp(context)));
        }

        internal XElement GetSummaryElement(XElement element)
        {
            if (element == null || element.IsEmpty)
                return null;

            return element.Elements("summary").FirstOrDefault();
        }

        internal XElement GetDescriptionElement(XElement element)
        {
            if (element == null || element.IsEmpty)
                return null;

            return element.Elements("description").FirstOrDefault();
        }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
