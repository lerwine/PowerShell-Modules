using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LteDev.HelpXml
{
#pragma warning disable 1591 // Missing XML comment for publicly visible type or member
    public abstract class CommandFactoryContext<T> : CommandFactoryContext
        where T : CommandInfo
    {
        public new T CommandInfo { get { return (T)(base.CommandInfo); } }

        public CommandFactoryContext(PSHelpFactoryContext context, T commandInfo) : base(context, commandInfo) { }
    }

    public abstract class CommandFactoryContext : PSHelpFactoryContext
    {
        public abstract string Verb { get; }

        public abstract string Noun { get; }

        public CommandInfo CommandInfo { get; private set; }

        private XElement _synopsisElement = null;
        private XElement _copyrightElement = null;
        private XElement _versionElement = null;
        private XElement _vendorElement = null;
        private XElement _commandDescriptionElement = null;
        private Collection<string> _aliases = new Collection<string>();

        public Collection<string> Aliases { get { return _aliases; } }

        public XElement SynopsisElement
        {
            get
            {
                if (_synopsisElement == null)
                    _synopsisElement = new XElement(MamlFactory.XmlNs_maml.GetName(MamlFactory.description));
                return _synopsisElement;
            }
        }

        public XElement CopyrightElement
        {
            get
            {
                if (_copyrightElement == null)
                    _copyrightElement = new XElement(MamlFactory.XmlNs_maml.GetName(MamlFactory.copyright));
                return _copyrightElement;
            }
        }

        public XElement VersionElement
        {
            get
            {
                if (_versionElement == null)
                    _versionElement = new XElement(DevHelpFactory.XmlNs_dev.GetName(DevHelpFactory.version));
                return _versionElement;
            }
        }

        public XElement VendorElement
        {
            get
            {
                if (_vendorElement == null)
                    _vendorElement = new XElement(CommandHelpFactory.XmlNs_command.GetName(CommandHelpFactory.vendor));
                return _vendorElement;
            }
        }

        public XElement CommandDescriptionElement
        {
            get
            {
                if (_commandDescriptionElement == null)
                    _commandDescriptionElement = new XElement(MamlFactory.XmlNs_maml.GetName(MamlFactory.description));
                return _commandDescriptionElement;
            }
        }

        public abstract AssemblyContext AssemblyContext { get; }

        public abstract XElement AssemblyClassHelp { get; }

        public abstract XElement GetAssemblyPropertyHelp(string propertyName);

        protected CommandFactoryContext(PSHelpFactoryContext context, CommandInfo commandInfo)
            : base(context)
        {
            CommandInfo = commandInfo;
        }
        protected CommandFactoryContext(CommandFactoryContext context)
            : base(context)
        {
            CommandInfo = context.CommandInfo;
            _aliases = context.Aliases;
            _commandDescriptionElement = context.CommandDescriptionElement;
            _copyrightElement = context.CopyrightElement;
            _synopsisElement = context.SynopsisElement;
            _vendorElement = context.VendorElement;
            _versionElement = context.VersionElement;
        }
    }
#pragma warning restore 1591 // Missing XML comment for publicly visible type or member
}