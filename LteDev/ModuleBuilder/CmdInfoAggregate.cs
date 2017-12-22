using Microsoft.PowerShell.Commands;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace LteDev.ModuleBuilder
{
#pragma warning disable 1591 // Missing XML comment for publicly visible type or member
    /// <summary>
    /// Aggregates information about a PowerShell command
    /// </summary>
    public abstract class CmdInfoAggregate : InformationAggregator
    {
        private object _syncRoot = new object();
        private Collection<CmdParamInfoAggregate> _innerParameters = null;
        private ReadOnlyCollection<CmdParamInfoAggregate> _parameters = null;
        private Collection<ParameterSetAggregate> _innerParameterSets = null;
        private ReadOnlyCollection<ParameterSetAggregate> _parameterSets = null;
        private Collection<string> _innerAliases = null;
        private ReadOnlyCollection<string> _aliases = null;
        private Collection<CLRTypeInfo> _innerReturnValues = null;
        private ReadOnlyCollection<CLRTypeInfo> _returnValues = null;
        private PSObject _mamlCommandHelpInfo = null;
        /// <summary>
        /// Aggregate information about the module to which the command belongs.
        /// </summary>
        public ModuleInfoAggregate Module { get; private set; }

        /// <summary>
        /// The PowerShell command being represented by this object.
        /// </summary>
        public CommandInfo Command { get; private set; }

        /// <summary>
        /// Represents all parameters of the associated command.
        /// </summary>
        public ReadOnlyCollection<CmdParamInfoAggregate> Parameters
        {
            get
            {
                Monitor.Enter(_syncRoot);
                try
                {
                    if (_parameters == null)
                    {
                        _innerParameters = new Collection<CmdParamInfoAggregate>();
                        foreach (string key in Command.Parameters.Keys)
                            _innerParameters.Add(CreateCmdParamInfoAggregate(Command.Parameters[key]));
                        _parameters = new ReadOnlyCollection<CmdParamInfoAggregate>(_innerParameters);
                    }
                }
                finally { Monitor.Exit(_syncRoot); }
                return _parameters;
            }
        }

        /// <summary>
        /// Parameter sets of the current command
        /// </summary>
        public ReadOnlyCollection<ParameterSetAggregate> ParameterSets
        {
            get
            {
                Monitor.Enter(_syncRoot);
                try
                {
                    if (_parameterSets == null)
                    {
                        _innerParameterSets = new Collection<ParameterSetAggregate>();
                        foreach (CommandParameterSetInfo parameterSet in Command.ParameterSets)
                            _innerParameterSets.Add(CreateParameterSetAggregate(parameterSet));
                        _parameterSets = new ReadOnlyCollection<ParameterSetAggregate>(_innerParameterSets);
                    }
                }
                finally { Monitor.Exit(_syncRoot); }
                return _parameterSets;
            }
        }

        /// <summary>
        /// Exported aliases of the current command
        /// </summary>
        public ReadOnlyCollection<string> Aliases
        {
            get
            {
                Monitor.Enter(_syncRoot);
                try
                {
                    if (_aliases == null)
                    {
                        _innerAliases = new Collection<string>();
                        foreach (AliasInfo alias in Module.ModuleInfo.ExportedAliases.Keys.Select(k => Module.ModuleInfo.ExportedAliases[k]).Where(a => a.ReferencedCommand != null && a.ReferencedCommand.Name == Command.Name))
                            _innerAliases.Add(alias.Name);
                        _aliases = new ReadOnlyCollection<string>(_innerAliases);
                    }
                }
                finally { Monitor.Exit(_syncRoot); }
                return _aliases;
            }
        }

        /// <summary>
        /// Return values for the current command
        /// </summary>
        public ReadOnlyCollection<CLRTypeInfo> ReturnValues
        {
            get
            {
                Monitor.Enter(_syncRoot);
                try
                {
                    if (_returnValues == null)
                    {
                        _innerReturnValues = new Collection<CLRTypeInfo>();
                        foreach (PSTypeName typeName in Command.OutputType)
                            _innerReturnValues.Add(new CLRTypeInfo(typeName, (typeName.Type == null) ? null : Module.AggregateFactory.GetAssemblyContext(typeName.Type.Assembly)));
                        _returnValues = new ReadOnlyCollection<CLRTypeInfo>(_innerReturnValues);
                    }
                }
                finally { Monitor.Exit(_syncRoot); }
                return _returnValues;
            }
        }

        public abstract string Verb { get; }

        public abstract string Noun { get; }

        /// <summary>
        /// Command help object
        /// </summary>
        /// <remarks>Source code which generates this object can be found at https://github.com/PowerShell/PowerShell/blob/master/src/System.Management.Automation/help/HelpInfo.cs.</remarks>
        public PSObject MamlCommandHelpInfo
        {
            get
            {
                if (_mamlCommandHelpInfo == null)
                {
                    GetHelpCommand command = new GetHelpCommand();
                    command.Name = Command.Name;
                    command.Full = SwitchParameter.Present;
                    IEnumerable result;
                    try { result = command.Invoke(); } catch { result = null; }
                    object obj = null;
                    if (result != null)
                    {
                        IEnumerator e = result.GetEnumerator();
                        while (obj == null && e.MoveNext())
                        {
                            if (e.Current != null)
                            {
                                obj = e.Current;
                                break;
                            }
                        }
                    }
                    _mamlCommandHelpInfo = (obj == null) ? new PSObject() : (obj is PSObject) ? obj as PSObject : PSObject.AsPSObject(obj);
                }
                return _mamlCommandHelpInfo;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CmdInfoAggregate" /> object.
        /// </summary>
        /// <param name="command">The PowerShell command being represented by this object.</param>
        /// <param name="module">Aggregate information about the module to which the command belongs.</param>
        protected CmdInfoAggregate(CommandInfo command, ModuleInfoAggregate module)
        {
            if (command == null)
                throw new ArgumentNullException("command");
            
            if (module == null)
                throw new ArgumentNullException("module");

            if (!module.ModuleInfo.ExportedCommands.Any(c => ReferenceEquals(c, command)))
                throw new ArgumentException("Command belongs to another module.");

            Module = module;
            Command = command;
        }

        /// <summary>
        /// This gets invoked to create an object which contains aggregate information about a command parameter.
        /// </summary>
        /// <remarks></remarks>
        /// <param name="parameterMetadata">Metadata object which describes the parameter.</param>
        /// <returns>An object which contains aggregate information about a command parameter.</returns>
        protected virtual CmdParamInfoAggregate CreateCmdParamInfoAggregate(ParameterMetadata parameterMetadata)
        {
            return new CmdParamInfoAggregate(parameterMetadata, this);
        }

        /// <summary>
        /// This gets invoked to create an object which contains aggregate information about a parameter set.
        /// </summary>
        /// <param name="parameterSet">Parameter set to be represented.</param>
        /// <returns>An object which contains aggregate information about a parameter set.</returns>
        protected virtual ParameterSetAggregate CreateParameterSetAggregate(CommandParameterSetInfo parameterSet)
        {
            return new ParameterSetAggregate(parameterSet, this);
        }

        /// <summary>
        /// This gets invoked to create an object which contains aggregate information about a parameter in a particular parameter set.
        /// </summary>
        /// <param name="parameter">Parameter to be represented.</param>
        /// <param name="parameterSet">Aggregated parameter set information.</param>
        /// <returns>An object which contains aggregate information about a parameter in a particular parameter set.</returns>
        protected internal virtual ParameterSetParamInfoAggregate CreateParameterSetParamInfoAggregate(CommandParameterInfo parameter, ParameterSetAggregate parameterSet)
        {
            return new ParameterSetParamInfoAggregate(parameter, parameterSet);
        }

        /// <summary>
        /// Creates help command element.
        /// </summary>
        /// <returns>A command PSMAML element</returns>
        public virtual XElement CreateCommandHelp()
        {
            IEnumerable<XElement> description = GetCommandHelpDescription();
            if (description == null ||  !description.Any())
                description = new XElement[] { new XElement(maml.para, new XComment("Long Description goes here.")) };

            XElement commandElement = new XElement(NS_command.GetName("command"),
                new XAttribute(XNamespace.Xmlns.GetName("command"), NS_command.NamespaceName),
                new XAttribute(XNamespace.Xmlns.GetName("maml"), maml.NS.NamespaceName),
                new XAttribute(XNamespace.Xmlns.GetName("dev"), dev.NS.NamespaceName),
                new XAttribute(XNamespace.Xmlns.GetName("MSHelp"), NS_MSHelp.NamespaceName),
                CreateCommandHelpDetails(),
                new XElement(maml.description, description.ToArray()),
                new XElement(NS_command.GetName("syntax"), ParameterSets.Select(p => p.GetSyntaxItemHelp()).ToArray()),
                new XElement(NS_command.GetName("parameters"), Parameters.Select(p => p.GetParameterHelp()).ToArray()),
                new XElement(NS_command.GetName("inputTypes"), ParameterSets.SelectMany(p => p.Parameters)
                    .Where(p => p.Parameter.ValueFromPipeline || p.Parameter.ValueFromPipelineByPropertyName).GroupBy(p => p.ParameterType.Name)
                    .Select(g => g.First().GetParameterTypeHelp()).ToArray()),
                new XElement(NS_command.GetName("returnValues"), GetReturnValues().ToArray()),
                new XElement(NS_command.GetName("terminatingErrors"), GetTerminatingErrors().ToArray()),
                new XElement(NS_command.GetName("nonTerminatingErrors"), GetNonTerminatingErrors().ToArray())
            );

            IEnumerable<XElement> elements = GetNotes();
            if (elements.Any())
                commandElement.Add(elements.ToArray());

            elements = GetExamples();
            if (elements.Any())
                commandElement.Add(new XElement(NS_command.GetName("examples"), elements.ToArray()));
            commandElement.Add(new XElement(maml.relatedLinks, GetRelatedNavigationLinks().ToArray()));

            return commandElement;
        }

        protected virtual IEnumerable<XElement> GetReturnValues()
        {
            var returnValues = MamlCommandHelpInfo.ValueOf("returnValues").ValueOf("returnValue").PSObjectValues().Where(o => o != null).Select(o => new
            {
                description = o.ValueOf("description").PSObjectValues().SelectMany(v => v.PSObjectValues("Text"))
                        .SelectMany(v => v.AsStringValues()).Where(s => !String.IsNullOrWhiteSpace(s)).ToArray(),
                type = o.ValueOf("type").PSObjectValues().Where(t => t != null).Select(t => new
                {
                    description = t.ValueOf("description").PSObjectValues().SelectMany(v => v.PSObjectValues("Text"))
                        .SelectMany(v => v.AsStringValues()).Where(s => !String.IsNullOrWhiteSpace(s)).ToArray(),
                    name = t.ValueOf("name").PSObjectValues().SelectMany(s => s.AsStringValues()).Where(s => s != null)
                        .SelectMany(v => v.Split(',')).Select(s => s.Trim()).Where(s => s.Length > 0)
                        .Select(s => LanguagePrimitives.ConvertTypeNameToPSTypeName(s)).LastOrDefault(),
                    uri = t.ValueOf("uri").AsStringValues().FirstOrDefault()
                }).FirstOrDefault()
            });
            // TODO: fill in gaps from returnValues
            return ReturnValues.Select(r => new XElement(NS_command.GetName("returnValue"), r.GetPSMamlDevType()));
        }

        protected virtual XElement CreateCommandHelpDetails()
        {
            IEnumerable<XElement> commandSummary = GetCommandHelpSynopsis();
            if (commandSummary == null || !commandSummary.Any())
                commandSummary = new XElement[] { new XElement(maml.para, new XComment("Short Description goes here.")) };

            IEnumerable<XElement> copyright = GetCopyright();
            if (copyright == null || !copyright.Any())
                copyright = new XElement[] { new XElement(maml.para, new XComment("Copyright goes here.")) };

            IEnumerable<XElement> version = GetVersion();
            if (version == null || !version.Any())
                version = new XElement[] { new XElement(maml.para, new XComment("Version goes here.")) };

            XElement detailsElement = new XElement(NS_command.GetName("details"),
                new XElement(NS_command.GetName("name"), Command.Name),
                new XElement(maml.description, commandSummary.ToArray())
            );

            if (Aliases.Count > 0)
                detailsElement.Add(new XElement(NS_command.GetName("synonyms"), Aliases.Select(s => new XElement(NS_command.GetName("synonym"), s)).ToArray()));
            detailsElement.Add(new XElement(maml.copyright, copyright.ToArray()), new XElement(NS_command.GetName("verb"), Verb),
                new XElement(NS_command.GetName("noun"), Noun), new XElement(dev.version, version.ToArray()));

            return detailsElement;
        }

        protected virtual IEnumerable<XElement> GetTerminatingErrors()
        {

            return new XElement[0];
        }

        protected virtual IEnumerable<XElement> GetNonTerminatingErrors()
        {

            return new XElement[0];
        }

        /// <summary>
        /// Gets notes.
        /// </summary>
        /// <returns><see cref="InformationAggregator.maml.alertSet"/> elements for current command.</returns>
        protected virtual IEnumerable<XElement> GetNotes()
        {
            return MamlCommandHelpInfo.PSObjectValues("alertSet").Select(alertSet => alertSet.PSObjectValues("alert").Where(a => !a.IsNullOrEmpty()))
                .Where(alertSet => alertSet.Any()).Select(a => new XElement(maml.alertSet, maml.FromMamlParaTextItems(a).ToArray()));
        }

        /// <summary>
        /// Gets examples for current command.
        /// </summary>
        /// <returns>example elements for current command</returns>
        protected virtual IEnumerable<XElement> GetExamples()
        {
            return MamlCommandHelpInfo.PSObjectValues("examples").SelectMany(o => o.PSObjectValues("example")).Where(o => !o.IsNullOrEmpty()).Select(ex =>
                {
                    XElement element = new XElement(NS_command.GetName("example"));
                    IEnumerable<string> sArr = ex.ValueOf("title").ToStringValues().Where(s => !String.IsNullOrWhiteSpace(s));
                    if (sArr.Any())
                        element.Add(new XElement(maml.title, (String.Join(" ", sArr.ToArray()).Trim())));
                    IEnumerable<PSObject> objArr = ex.ValueOf("introduction").PSObjectValues().Where(a => !a.IsNullOrEmpty());
                    if (objArr.Any())
                        element.Add(new XAttribute(maml.introduction, maml.FromMamlParaTextItems(objArr).ToArray()));
                    sArr = ex.ValueOf("code").ToStringValues().Where(s => !String.IsNullOrWhiteSpace(s));
                    if (sArr.Any())
                        element.Add(new XElement(dev.code, (String.Join(" ", sArr.ToArray()).Trim())));
                    sArr = ex.ValueOf("codeReference").ToStringValues().Where(s => !String.IsNullOrWhiteSpace(s));
                    if (sArr.Any())
                        element.Add(new XElement(dev.codeReference, (String.Join(" ", sArr.ToArray()).Trim())));
                    objArr = ex.ValueOf("security").PSObjectValues().Where(a => !a.IsNullOrEmpty());
                    if (objArr.Any())
                        element.Add(new XAttribute(dev.security, maml.FromMamlParaTextItems(objArr).ToArray()));
                    objArr = ex.ValueOf("remarks").PSObjectValues().Where(a => !a.IsNullOrEmpty());
                    if (objArr.Any())
                        element.Add(new XAttribute(dev.remarks, maml.FromMamlParaTextItems(objArr).ToArray()));
                    // TODO: Need to handle commandLines
                    return element;
                });
        }

        /// <summary>
        /// Gets related links.
        /// </summary>
        /// <returns><see cref="InformationAggregator.maml.navigationLink"/> elements for current command.</returns>
        protected virtual IEnumerable<XElement> GetRelatedNavigationLinks()
        {
            // TODO: MamlCommandHelpInfo#relatedLinks relatedLinks=@{navigationLink=System.Management.Automation.PSObject[]}
            return new XElement[0];
        }

        protected virtual IEnumerable<XElement> GetCommandHelpSynopsis()
        {
            PSObject synopsis = MamlCommandHelpInfo.ValueOf("Synopsis");
            if (synopsis.IsNullOrEmpty())
            {
                synopsis = MamlCommandHelpInfo.ValueOf("details");
                if (synopsis.IsNullOrEmpty())
                    return new XElement[0];
                    synopsis = synopsis.ValueOf("description");
            }

            if (!synopsis.IsSingleValue())
                return maml.FromMamlParaTextItems(synopsis.PSObjectValues());

            return new XElement[] { new XElement(maml.para, String.Join(Environment.NewLine, synopsis.AsStringValues().Where(s => !String.IsNullOrWhiteSpace(s)).ToArray()).Trim()) };
        }

        protected virtual IEnumerable<XElement> GetCommandHelpDescription()
        {
            PSObject description = MamlCommandHelpInfo.ValueOf("description");
            if (description.IsNullOrEmpty())
                return new XElement[0];
            return maml.FromMamlParaTextItems(description.PSObjectValues());
        }

        protected virtual IEnumerable<XElement> GetVersion()
        {
            if (Module.ModuleInfo.Version != null)
                yield return new XElement(maml.para, Module.ModuleInfo.Version.ToString((Module.ModuleInfo.Version.Revision > 0) ? 4 : ((Module.ModuleInfo.Version.Build > 0) ? 3 : 2)));
        }

        protected virtual IEnumerable<XElement> GetCopyright()
        {
            if (!String.IsNullOrWhiteSpace(Module.ModuleInfo.Copyright))
                yield return new XElement(maml.para, Module.ModuleInfo.Copyright);
        }
    }

    public abstract class CmdInfoAggregate<T> : CmdInfoAggregate
        where T : CommandInfo
    {
        public new T Command { get { return (T)(base.Command); } }

        protected CmdInfoAggregate(T command, ModuleInfoAggregate module) : base(command, module) { }
    }
#pragma warning restore 1591 // Missing XML comment for publicly visible type or member
}