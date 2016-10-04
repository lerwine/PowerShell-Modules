using System;
using System.Collections;
using System.Collections.Generic;
#if !PSLEGACY
using System.Linq;
#endif
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Text;
using System.Threading;
#if !PSLEGACY
using System.Threading.Tasks;
#endif
using System.Xml;
#if !PSLEGACY
using System.Xml.Linq;
#endif

namespace IOUtilityCLR
{
    /// <summary>
    /// This represents the WPF window to be displayed.
    /// </summary>
    public class XamlWindowBuilder : PSInvocationBuilderBase<XamlDialogResult, XamlDialogAsyncResult>
    {
#region Constants

        /// <summary>
        /// Default XML namespace.
        /// </summary>
        public const string XML_XMLNAMESPACE = "http://www.w3.org/2000/xmlns/";

        /// <summary>
        /// XML namespace for XAML markup.
        /// </summary>
        public const string XAML_XMLNAMESPACE = "http://schemas.microsoft.com/winfx/2006/xaml";

        /// <summary>
        /// XML namespace for XAML Presentation node markup.
        /// </summary>
        public const string PRESENTATION_XMLNAMESPACE = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";

        /// <summary>
        /// XML namespace for XAML compatibility node markup.
        /// </summary>
        public const string MARKUP_COMPATIBILITY_XMLNAMESPACE = "http://schemas.openxmlformats.org/markup-compatibility/2006";

        /// <summary>
        /// Element name for XAML window markup.
        /// </summary>
        public const string WINDOW_ELEMENTNAME = "Window";

#endregion

#region Fields

        private PSInvocationBuilder _invocation = new PSInvocationBuilder();
        private object _syncRoot = new object();
        private bool _xmlChanged = false;
        private List<PipelineItem> _beforeWindowCreated = new List<PSInvocationBuilderBase<XamlDialogResult, XamlDialogAsyncResult>.PipelineItem>();
        private List<PipelineItem> _afterDialogClosed = new List<PSInvocationBuilderBase<XamlDialogResult, XamlDialogAsyncResult>.PipelineItem>();
#if USEXLINQ
        private XDocument _xaml;
#else
        private XmlDocument _xaml;
        private XmlNamespaceManager _namespaceManager = null;
#endif
#endregion

#region Properties
        
        /// <summary>
        /// Specifies pipeline which is to be invoked before the WPF window is created.
        /// </summary>
        public List<PipelineItem> BeforeWindowCreated { get { return _beforeWindowCreated; } }

        /// <summary>
        /// Specifies pipeline which is to be invoked after the WPF window has been created, and before it is shown.
        /// </summary>
        public List<PipelineItem> BeforeShowDialog { get { return Pipeline; } }

        /// <summary>
        /// Specifies pipeline which gets called after the WPF window has been closed.
        /// </summary>
        public List<PipelineItem> AfterDialogClosed { get { return _afterDialogClosed; } }

        /// <summary>
        /// Title for the displayed window.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Initial width of the displayed window.
        /// </summary>
        public double? Width { get; set; }

        /// <summary>
        /// Initial height of the displayed window.
        /// </summary>
        public double? Height { get; set; }

        /// <summary>
        /// Specifies the apartment state to use for the thread which invokes the pipeline.
        /// </summary>
        public override ApartmentState ApartmentState
        {
            get { return base.ApartmentState; }
            set
            {
                if (value != ApartmentState.STA)
                    throw new ArgumentException("Only single-Threaded Apartment state is supported.", "value");
                base.ApartmentState = value;
            }
        }

        /// <summary>
        /// Indicates how threads are handled when the pipeline is invoked.
        /// </summary>
        public override PSThreadOptions ThreadOptions
        {
            get { return base.ThreadOptions; }
            set
            {
                if (value != PSThreadOptions.ReuseThread)
                    throw new ArgumentException("Only Reuse-thread option is supported.", "value");

                base.ThreadOptions = value;
            }
        }

#if USEXLINQ
        /// <summary>
        /// XAML markup which represents the window that is to be displayed.
        /// </summary>
        public XDocument Xaml
        {
            get { return _xaml; }
            set
            {
                lock (_syncRoot)
                {
                    XDocument xaml = value ?? new XDocument();\
                    if (_xaml != null && ReferenceEquals(xaml, _xaml))
                        return;
                    if (xaml.Root == null)
                        xaml.Add(CreateWindowXaml());
                    else
                    {
                        if (xaml.Root.Name.LocalName != WINDOW_ELEMENTNAME || xaml.Root.Name.NamespaceName != PRESENTATION_XMLNAMESPACE)
                        {
                            XElement content = xaml.Root;
                            content.Remove();
                            XElement element = CreateWindowXaml();
                            xaml.Add(element);
                            element.Add(content);
                        }
                    }
                    _xmlChanged = true;
                    xaml.Changed += Xaml_Changed;
                    if (_xaml != null)
                        xaml.Changed += Xaml_Changed;
                    _xaml = xaml;
                }
            }
        }

        /// <summary>
        /// Window XAML element.
        /// </summary>
        public XElement WindowElement { get { return _xaml.Root; } }
#else
        /// <summary>
        /// Namespace manager used to look up XML nodes.
        /// </summary>
        public XmlNamespaceManager NamespaceManager
        {
            get
            {
                lock (_syncRoot)
                {
                    if (_namespaceManager != null)
                        return _namespaceManager;
                    _namespaceManager = new XmlNamespaceManager(_xaml.NameTable);
                    _namespaceManager.AddNamespace(EnsureRootNamespace(_xaml, XAML_XMLNAMESPACE, "x"), XAML_XMLNAMESPACE);
                    _namespaceManager.AddNamespace(EnsureRootNamespace(_xaml, MARKUP_COMPATIBILITY_XMLNAMESPACE, "mc"), MARKUP_COMPATIBILITY_XMLNAMESPACE);
                    string prefix = "p";
                    int i = -1;
                    while (_xaml.GetNamespaceOfPrefix(prefix) != null)
                    {
                        i++;
                        prefix = "p" + i.ToString();
                    }
                    _namespaceManager.AddNamespace(prefix, PRESENTATION_XMLNAMESPACE);
                }
                return _namespaceManager;
            }
        }
        
        /// <summary>
        /// XAML markup which represents the window that is to be displayed.
        /// </summary>
        public XmlDocument Xaml
        {
            get { return _xaml; }
            set
            {
                lock (_syncRoot)
                {
                    XmlDocument xaml = value ?? new XmlDocument();
                    if (_xaml != null && ReferenceEquals(xaml, _xaml))
                        return;
                    if (xaml.DocumentElement == null)
                        xaml.AppendChild(CreateWindowXaml(xaml));
                    else
                    {
                        if (xaml.DocumentElement.LocalName != WINDOW_ELEMENTNAME || xaml.DocumentElement.NamespaceURI != PRESENTATION_XMLNAMESPACE)
                        {
                            XmlElement content = xaml.DocumentElement;
                            xaml.RemoveChild(content);
                            xaml.AppendChild(CreateWindowXaml(xaml)).AppendChild(content);
                        }
                    }
                    _namespaceManager = null;
                    _xmlChanged = true;
                    xaml.NodeInserted += Xaml_DocumentChanged;
                    xaml.NodeRemoved += Xaml_DocumentChanged;
                    xaml.NodeChanged += Xaml_DocumentChanged;
                    if (_xaml != null)
                    {
                        _xaml.NodeInserted -= Xaml_DocumentChanged;
                        _xaml.NodeRemoved -= Xaml_DocumentChanged;
                        _xaml.NodeChanged -= Xaml_DocumentChanged;
                    }
                    _xaml = xaml;
                }
            }
        }

        /// <summary>
        /// Window XAML element.
        /// </summary>
        public XmlElement WindowElement { get { return _xaml.DocumentElement; } }

        /// <summary>
        /// Inner XML for the Window XAML.
        /// </summary>
        public string InnerXml
        {
            get
            {
                lock (_syncRoot)
                    return (_xaml.DocumentElement.IsEmpty) ? null : _xaml.DocumentElement.InnerXml;
            }
            set
            {
                lock (_syncRoot)
                {
                    if (value == null)
                    {
                        if (!_xaml.DocumentElement.IsEmpty)
                            _xaml.DocumentElement.IsEmpty = true;
                    }
                    else
                        _xaml.DocumentElement.InnerXml = value;
                }
            }
        }
#endif

#endregion

#region Methods

#if USEXLINQ
        private void Xaml_Changed(object sender, XObjectChangeEventArgs e) { _xmlChanged = true; }

        public static XDocument CreateWindowXamlDocument()
        {
            XDocument xaml = new XDocument();
            xaml.Add(CreateWindowXaml());
            return xaml;
        }

        public static XElement CreateWindowXaml()
        {
            return new XElement(XName.Get(WINDOW_ELEMENTNAME, PRESENTATION_XMLNAMESPACE),
                new XAttribute(XNamespace.Xmlns.GetName("x"), XAML_XMLNAMESPACE),
                new XAttribute(XNamespace.Xmlns.GetName("mc"), MARKUP_COMPATIBILITY_XMLNAMESPACE),
                new XAttribute("Title", "{Binding Path=WindowTitle, Mode=TwoWay}"),
                new XAttribute("Width", "{Binding Path=WindowWidth, Mode=TwoWay}"),
                new XAttribute("Height", "{Binding Path=WindowHeight, Mode=TwoWay}"));
        }
#else
        /// <summary>
        /// Ensures a namespace and prefix is defined at the root level.
        /// </summary>
        /// <param name="node">Node in document.</param>
        /// <param name="namespaceUri">URI of namespace.</param>
        /// <param name="defaultPrefix">Default prefix to use for element names.</param>
        /// <returns></returns>
        public static string EnsureRootNamespace(XmlNode node, string namespaceUri, string defaultPrefix)
        {
            XmlDocument xmlDocument = (node is XmlDocument) ? node as XmlDocument : node.OwnerDocument;
            string prefix = xmlDocument.GetPrefixOfNamespace(namespaceUri);
            if (prefix != null)
                return prefix;
            prefix = defaultPrefix;
            int i = -1;
            while (xmlDocument.GetNamespaceOfPrefix(prefix) != null)
            {
                i++;
                prefix = defaultPrefix + i.ToString();
            }
            xmlDocument.DocumentElement.Attributes.Append(xmlDocument.CreateAttribute("xmlns", prefix, XML_XMLNAMESPACE)).Value = namespaceUri;
            return prefix;
        }

        /// <summary>
        /// This gets called whenever the XAML document has changed.
        /// </summary>
        /// <param name="sender">Object which initiated the event.</param>
        /// <param name="e">Information about the change.</param>
        private void Xaml_DocumentChanged(object sender, XmlNodeChangedEventArgs e) { _xmlChanged = true; }
        
        /// <summary>
        /// Create a new <see cref="XmlDocument"/> with a XAML window as teh content.
        /// </summary>
        /// <returns></returns>
        public static XmlDocument CreateWindowXaml()
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.AppendChild(CreateWindowXaml(xmlDocument));
            return xmlDocument;
        }

        public static XmlElement CreateWindowXaml(XmlDocument xmlDocument)
        {
            XmlElement xmlElement = xmlDocument.CreateElement(WINDOW_ELEMENTNAME, PRESENTATION_XMLNAMESPACE);
            xmlElement.Attributes.Append(xmlDocument.CreateAttribute("xmlns", "x", XML_XMLNAMESPACE)).Value = XAML_XMLNAMESPACE;
            xmlElement.Attributes.Append(xmlDocument.CreateAttribute("xmlns", "mc", XML_XMLNAMESPACE)).Value = MARKUP_COMPATIBILITY_XMLNAMESPACE;
            xmlElement.Attributes.Append(xmlDocument.CreateAttribute("Title")).Value = "{Binding Path=WindowTitle, Mode=TwoWay}";
            xmlElement.Attributes.Append(xmlDocument.CreateAttribute("Width")).Value = "{Binding Path=WindowWidth, Mode=TwoWay}";
            xmlElement.Attributes.Append(xmlDocument.CreateAttribute("Height")).Value = "{Binding Path=WindowHeight, Mode=TwoWay}";
            return xmlElement;
        }

#endif

        protected override void BeforeAddPipelineItems(PowerShell powershell, Runspace runspace)
        {
            runspace.SessionStateProxy.SetVariable("Xaml", _xaml);
            runspace.SessionStateProxy.SetVariable("WindowTitle", Title);
            runspace.SessionStateProxy.SetVariable("WindowWidth", Width);
            runspace.SessionStateProxy.SetVariable("WindowHeight", Height);

            base.BeforeAddPipelineItems(powershell, runspace);
        }

        protected override void AfterAddPipelineItems(PowerShell powershell, Runspace runspace)
        {
            base.AfterAddPipelineItems(powershell, runspace);
        }

        /// <summary>
        /// Creates and shows the window, waiting for the window to be closed.
        /// </summary>
        /// <returns>An object which represents the results of the invocation.</returns>
        public XamlDialogResult ShowDialog()
        {
            //Xaml.CreateReader();
            if (Settings == null)
            {
                if (Input == null)
                    return XamlDialogResult.Create(CreateRunspace, CreatePowerShell, Variables.Keys, SynchronizedData);
                return XamlDialogResult.Create(CreateRunspace, CreatePowerShell, Input, Variables.Keys, SynchronizedData);
            }

            if (Input == null)
                return XamlDialogResult.Create(CreateRunspace, CreatePowerShell, new object[0], Settings, Variables.Keys, SynchronizedData);
            return XamlDialogResult.Create(CreateRunspace, CreatePowerShell, Input, Settings, Variables.Keys, SynchronizedData);
        }

        /// <summary>
        /// Creates and shows the window.
        /// </summary>
        /// <returns>An object which represents the results of the invocation.</returns>
        public XamlDialogAsyncResult Show()
        {
            if (Settings != null || Input != null)
                throw new InvalidOperationException("Input type must be explicity defined.");
            return XamlDialogAsyncResult.Create(CreateRunspace, CreatePowerShell, Variables.Keys, SynchronizedData);
        }
        
#endregion

#region Constructors

#if USEXLINQ
        /// <summary>
        /// Create new object to represent a WPF window.
        /// </summary>
        public XamlWindowBuilder() : this(null as XDocument) { }

        /// <summary>
        /// Create new objectd to represent a WPF window with specified markup.
        /// </summary>
        /// <param name="xaml">Markup for window or window content.</param>
        public XamlWindowBuilder(XDocument xaml)
        {
            try { Xaml = xaml; }
            catch (ArgumentException exception) { throw new ArgumentException(exception.Message, "xaml"); }
        }
#else
        /// <summary>
        /// Create new object to represent a WPF window.
        /// </summary>
        public XamlWindowBuilder() : this(null as XmlDocument) { }

        /// <summary>
        /// Create new objectd to represent a WPF window with specified markup.
        /// </summary>
        /// <param name="xaml">Markup for window or window content.</param>
        public XamlWindowBuilder(XmlDocument xaml)
        {
            base.ApartmentState = ApartmentState.STA;
            base.ThreadOptions = PSThreadOptions.ReuseThread;
            try { Xaml = xaml; }
            catch (ArgumentException exception) { throw new ArgumentException(exception.Message, "xaml"); }
        }
#endif

#endregion
    }
}
