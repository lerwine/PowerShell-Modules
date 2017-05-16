using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Host;
using System.Management.Automation.Runspaces;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Threading;
using System.Xml;
using System.Collections;

namespace Erwine.Leonard.T.WPF
{
    public class WpfWindow
    {
        private XmlDocument _window = null;
        private Hashtable _variables = new Hashtable();

        public XmlDocument Window
        {
            get
            {
                XmlDocument windowXaml = _window;
                if (windowXaml == null)
                    windowXaml = new XmlDocument();
                else if (windowXaml.DocumentElement != null)
                    return windowXaml;

                windowXaml.AppendChild(windowXaml.CreateElement(XamlUtility.XmlElementName_Window, XamlUtility.XmlNamespaceURI_Presentation));
                windowXaml.Attributes.Append(windowXaml.CreateAttribute("xmlns", "x", XamlUtility.XmlNamespaceURI_Xml)).Value = XamlUtility.XmlNamespaceURI_Xaml;
                windowXaml.Attributes.Append(windowXaml.CreateAttribute("xmlns", "d", XamlUtility.XmlNamespaceURI_Xml)).Value = XamlUtility.XmlNamespaceURI_Blend;
                windowXaml.Attributes.Append(windowXaml.CreateAttribute("xmlns", "mc", XamlUtility.XmlNamespaceURI_Xml)).Value = XamlUtility.XmlNamespaceURI_MarkupCompatibility;
                windowXaml.Attributes.Append(windowXaml.CreateAttribute("Ignorable", XamlUtility.XmlNamespaceURI_Blend)).Value = "d";
                windowXaml.Attributes.Append(windowXaml.CreateAttribute(XamlUtility.XmlAttributeName_Width)).Value = "800";
                windowXaml.Attributes.Append(windowXaml.CreateAttribute(XamlUtility.XmlAttributeName_Height)).Value = "600";
                _window = windowXaml;
                return windowXaml;
            }
            set
            {
                if (value != null)
                {
                    if (_window != null && ReferenceEquals(value, _window))
                        return;

                    if (value.DocumentElement == null)
                    {
                        _window = value;
                        return;
                    }
                }
                else
                {
                    if (_window != null)
                        _window.NodeRemoving -= WindowXaml_NodeRemoving;
                    return;
                }

                if (value.DocumentElement.NamespaceURI != XamlUtility.XmlNamespaceURI_Presentation || value.DocumentElement.LocalName != XamlUtility.XmlElementName_Window)
                    throw new ArgumentException("Invalid root element name.");

                if (_window != null)
                    _window.NodeRemoving -= WindowXaml_NodeRemoving;
                value.NodeRemoving += WindowXaml_NodeRemoving;
                _window = value;
            }
        }

        public class VariablesHashtable : Hashtable
        {
            public const string VariableName_Window = "Window";

            private static StringComparer _comparer = StringComparer.InvariantCultureIgnoreCase;

            public VariablesHashtable() : base(_comparer) { }

            public VariablesHashtable(XmlDocument windowDocument) : this(windowDocument, null) { }

            public VariablesHashtable(IDictionary source) : this(XamlUtility.CreateXamlWindowMarkup(), source) { }

            public VariablesHashtable(XmlDocument windowDocument, IDictionary source) : base(_comparer) { }

            private static string _AsName(object key)
            {
                if (key != null && key is PSObject)
                    key = (key as PSObject).BaseObject;
                return (key == null || key is string) ? key as string : key.ToString();
            }
            public override void Add(object key, object value)
            {
                base.Add(key, value);
            }

            public override void Clear()
            {
                base.Clear();
            }

            public override object Clone()
            {
                return base.Clone();
            }

            public override bool Contains(object key)
            {
                return base.Contains(_AsName(key));
            }

            public override bool ContainsKey(object key)
            {
                return base.ContainsKey(_AsName(key));
            }

            protected override int GetHash(object key)
            {
                return base.GetHash(_AsName(key));
            }

            protected override bool KeyEquals(object item, object key)
            {
                return base.KeyEquals(item, _AsName(key));
            }

            public override void Remove(object key)
            {
                string name = _AsName(key);
                if (_comparer.Equals(name, VariableName_Window))
                    throw new NotSupportedException();

                base.Remove(key);
            }

            public override object this[object key]
            {
                get
                {
                    return base[_AsName(key)];
                }
                set
                {
                    base[key] = value;
                }
            }
        }
        public Hashtable Variables { get { return _variables; } set { _variables = value ?? new Hashtable(); } }

        private void WindowXaml_NodeRemoving(object sender, XmlNodeChangedEventArgs e)
        {
            if (ReferenceEquals(e.Node, _window.DocumentElement))
                throw new InvalidOperationException("Cannot remove root node.");
        }

        public int Width
        {
            get { return GetAttributeAsInt32(Window.DocumentElement, XamlUtility.XmlAttributeName_Width, 0); }
            set
            {
                if (value < 1)
                    throw new ArgumentOutOfRangeException();

                SetAttributeValue(Window.DocumentElement, XamlUtility.XmlAttributeName_Width, value);
            }
        }

        public int Height
        {
            get { return GetAttributeAsInt32(Window.DocumentElement, XamlUtility.XmlAttributeName_Height, 0); }
            set
            {
                if (value < 1)
                    throw new ArgumentOutOfRangeException();

                SetAttributeValue(Window.DocumentElement, XamlUtility.XmlAttributeName_Height, value);
            }
        }

        public static int? GetAttributeAsInt32(XmlElement element, string attributeName)
        {
            if (element == null || String.IsNullOrEmpty(attributeName))
                return null;

            XmlAttribute xmlAttribute = element.SelectSingleNode("@" + XmlConvert.EncodeLocalName(attributeName)) as XmlAttribute;
            string s;
            if (xmlAttribute == null || (s = xmlAttribute.Value.Trim()).Length == 0)
                return null;
            try { return XmlConvert.ToInt32(s); } catch { return null; }
        }

        public static int GetAttributeAsInt32(XmlElement element, string attributeName, int defaultValue)
        {
            int? result = GetAttributeAsInt32(element, attributeName);
            return (result.HasValue) ? result.Value : defaultValue;
        }

        public static void SetAttributeValue(XmlElement element, string attributeName, int value)
        {
            XmlAttribute xmlAttribute = element.SelectSingleNode("@" + XmlConvert.EncodeLocalName(attributeName)) as XmlAttribute;
            if (xmlAttribute == null)
                element.Attributes.Append(element.OwnerDocument.CreateAttribute(attributeName)).Value = XmlConvert.ToString(value);
            else
                xmlAttribute.Value = XmlConvert.ToString(value);
        }

        public WpfWindow()
        {
        }
    }

    /// <summary>
    /// Proxy object for a WPF window.
    /// </summary>
    public class WpfWindow_obsolete
    {
        #region Fields
        
        private WindowProcessInternal _openWindowProcess = null;
        private string _xml = "";
        private XmlDocument _windowXaml;
        private System.Collections.Hashtable _synchronizedData = System.Collections.Hashtable.Synchronized(new System.Collections.Hashtable());
        private ScriptBlock _beforeWindowCreated = null;
        private ScriptBlock _beforeWindowShown = null;
        private ScriptBlock _afterWindowClosed = null;
        private Collection<PSObject> _output = new Collection<PSObject>();
        private Collection<ErrorRecord> _errorRecords = new Collection<ErrorRecord>();
        private Collection<WarningRecord> _warningRecords = new Collection<WarningRecord>();
        private Collection<VerboseRecord> _verboseRecords = new Collection<VerboseRecord>();
        private Collection<DebugRecord> _debugRecords = new Collection<DebugRecord>();
        private bool? _dialogResult;
        
        #endregion
        
        #region Properties
        
        /// <summary>
        /// Data which is provided to the <see cref="BeforeWindowCreated" />, <see cref="BeforeWindowShown" /> and <see cref="AfterWindowClosed" /> <seealso cref="ScriptBlock" /> parameters.
        /// </summary>
        public System.Collections.Hashtable SynchronizedData { get { return _synchronizedData; } }
        
        /// <summary>
        /// A value that specifies whether the activity was accepted (true) or canceled (false) after the last time the window was shown.
        /// </summary>
        public bool? DialogResult { get { return _dialogResult; } set { _dialogResult = value; } }
        
        /// <summary>
        /// Text which contains the XAML markup that is used to create the WPF window.
        /// </summary>
        public string WindowXaml
        {
            get { return _xml; }
            set
            {
                if (_openWindowProcess != null)
                    throw new InvalidOperationException("Cannot change markup while window is open.");
                
                if (String.IsNullOrEmpty(value))
                {
                    _xml = "";
                    _windowXaml = null;
                    return;
                }

                string xml = value.Trim();
                
                if (xml.Length == 0)
                    throw new ArgumentException("XML Markup is empty.", "value");
                
                XmlDocument xmlDocument = new XmlDocument();
                
                try { xmlDocument.LoadXml(xml); }
                catch (Exception exception) { throw new ArgumentException("Invalid XML markup", "value", exception); }
                
                if (xmlDocument.DocumentElement == null)
                    throw new ArgumentException("XML document is empty", "value");
                
                if (xmlDocument.DocumentElement.NamespaceURI != XamlUtility.XmlNamespaceURI_Presentation)
                    throw new ArgumentException("XML markup does not represent a WPF document.", "value");
                
                if (xmlDocument.DocumentElement.LocalName != "Window")
                    throw new ArgumentException("XAML markup does not represent a window.", "value");
                
                _xml = value;
                _windowXaml = xmlDocument;
            }
        }
        
        #region Output Collections
        
        /// <summary>
        /// Collection of <seealso cref="PSObject" /> values which represent the output from the <see cref="BeforeWindowCreated" />, <see cref="BeforeWindowShown" /> and
        /// <see cref="AfterWindowClosed" /> <seealso cref="ScriptBlock" /> parameters.
        /// </summary>
        public Collection<PSObject> Output { get { return _output; } }
        
        /// <summary>
        /// Collection of <seealso cref="ErrorRecord" /> objects which represent the errors encountered while showing the window as well as executing the <see cref="BeforeWindowCreated" />,
        /// <see cref="BeforeWindowShown" /> and <see cref="AfterWindowClosed" /> <seealso cref="ScriptBlock" /> parameters.
        /// </summary>
        public Collection<ErrorRecord> ErrorRecords { get { return _errorRecords; } }
        
        /// <summary>
        /// Collection of <seealso cref="WarningRecord" /> objects which represent the warning messages emitted while showing the window as well as executing the <see cref="BeforeWindowCreated" />,
        /// <see cref="BeforeWindowShown" /> and <see cref="AfterWindowClosed" /> <seealso cref="ScriptBlock" /> parameters.
        /// </summary>
        public Collection<WarningRecord> WarningRecords { get { return _warningRecords; } }
        
        /// <summary>
        /// Collection of <seealso cref="VerboseRecord" /> objects which represent the verbose messages emitted while showing the window as well as executing the <see cref="BeforeWindowCreated" />,
        /// <see cref="BeforeWindowShown" /> and <see cref="AfterWindowClosed" /> <seealso cref="ScriptBlock" /> parameters.
        /// </summary>
        public Collection<VerboseRecord> VerboseRecords { get { return _verboseRecords; } }
        
        /// <summary>
        /// Collection of <seealso cref="DebugRecord" /> objects which represent the debug messages emitted while showing the window as well as executing the <see cref="BeforeWindowCreated" />,
        /// <see cref="BeforeWindowShown" /> and <see cref="AfterWindowClosed" /> <seealso cref="ScriptBlock" /> parameters.
        /// </summary>
        public Collection<DebugRecord> DebugRecords { get { return _debugRecords; } }
        
        #endregion
        
        #region Handler Script Blocks
        
        /// <summary>
        /// Script which gets executed before the XAML window is created.
        /// </summary>
        /// <remarks>
        /// When this script is invoked, the &quot;$this&quot; variable will be an instance of the <see cref="ThisObj" /> object.
        /// </remarks>
        public ScriptBlock BeforeWindowCreated
        {
            get { return _beforeWindowCreated; }
            set
            {
                if (_openWindowProcess != null)
                    throw new InvalidOperationException("Cannot change event handler script blocks while window is open.");
                
                _beforeWindowCreated = value;
            }
        }
        
        /// <summary>
        /// Script which gets executed after the XAML window is created, but before it is shown.
        /// </summary>
        /// <remarks>
        /// When this script is invoked, the &quot;$this&quot; variable will be an instance of the <see cref="ThisObj" /> object.
        /// If <code>$this.Window</code> is null, then that means there was an error while trying to create the window.
        /// </remarks>
        public ScriptBlock BeforeWindowShown
        {
            get { return _beforeWindowShown; }
            set
            {
                if (_openWindowProcess != null)
                    throw new InvalidOperationException("Cannot change event handler script blocks while window is open.");
                
                _beforeWindowShown = value;
            }
        }
        
        /// <summary>
        /// Script which gets executed after the XAML window has been displayed and has been closed.
        /// </summary>
        /// <remarks>
        /// When this script is invoked, the &quot;$this&quot; variable will be an instance of the <see cref="ThisObj" /> object.
        /// If <code>$this.Window</code> is null, then that means there was an error while trying to create the window, also indicating that no window was actually displayed.
        /// </remarks>
        public ScriptBlock AfterWindowClosed
        {
            get { return _afterWindowClosed; }
            set
            {
                if (_openWindowProcess != null)
                    throw new InvalidOperationException("Cannot change event handler script blocks while window is open.");
                
                _afterWindowClosed = value;
            }
        }
        
        #endregion
        
        #endregion
        
        #region Methods
        
        #region AddOutput Overrides
        
        /// <summary>
        /// Add output from a <seealso cref="PSDataStreams" /> object to the <see cref="ErrorRecords" />, <see cref="WarningRecords" />, <see cref="VerboseRecords" /> and
        /// <see cref="DebugRecords" /> collections.
        /// </summary>
        /// <param name="dataStreams">Data streams object to be added.</param>
        public void AddOutput(PSDataStreams dataStreams)
        {
            foreach (ErrorRecord obj in dataStreams.Error.ReadAll())
                ErrorRecords.Add(obj);
            foreach (WarningRecord obj in dataStreams.Warning.ReadAll())
                WarningRecords.Add(obj);
            foreach (VerboseRecord obj in dataStreams.Verbose.ReadAll())
                VerboseRecords.Add(obj);
            foreach (DebugRecord obj in dataStreams.Debug.ReadAll())
                DebugRecords.Add(obj);
            
        }
        
        /// <summary>
        /// Add async output to the <see cref="Output" /> collections.
        /// </summary>
        /// <param name="asyncOutput">Output object to be added.</param>
        public void AddOutput(PSDataCollection<PSObject> asyncOutput) { if (asyncOutput != null) { AddOutput(asyncOutput.ReadAll()); } }
        
        /// <summary>
        /// Add output to the <see cref="Output" /> collections.
        /// </summary>
        /// <param name="output">Output object to be added.</param>
        public void AddOutput(IEnumerable<PSObject> output)
        {
            if (output == null)
                return;
            
            foreach (PSObject obj in output)
                Output.Add(obj);
        }
        
        #endregion
        
        #region AddError Overrides
        
        private ErrorRecord _AddError(string message, string recommendedAction, Exception exception, string errorId, ErrorCategory errorCategory, string activity, string reason, object targetObject)
        {
            if (exception == null)
                throw new ArgumentNullException("exception");
            
            if (errorId == null)
                throw new ArgumentNullException("errorId");
            
            if (errorId.Trim().Length == 0)
                throw new ArgumentException("Error ID cannot be empty.", "errorId");
            
            ErrorRecord errorRecord = new ErrorRecord(exception, errorId, errorCategory, targetObject);
            ErrorRecords.Add(errorRecord);
            if (!String.IsNullOrEmpty(message))
            {
                errorRecord.ErrorDetails = new ErrorDetails(message);
                if (recommendedAction != null)
                    errorRecord.ErrorDetails.RecommendedAction = recommendedAction;
            }
            if (!String.IsNullOrEmpty(activity))
                errorRecord.CategoryInfo.Activity = activity;
            if (!String.IsNullOrEmpty(reason))
                errorRecord.CategoryInfo.Reason = reason;
            return errorRecord;
        }

        /// <summary>
        /// Adds an error record to the <see cref="ErrorRecords"/> collection.
        /// </summary>
        /// <param name="message">The replacement error message for the exception. This cannot be null, empty or entirely whitespace.</param>
        /// <param name="recommendedAction">Recommended action that should be taken as a result of this error.</param>
        /// <param name="exception">The exception that is associated with this record. This cannot be null.</param>
        /// <param name="errorId">A developer-defined identifier of the error. This identifier must be a non-localized string for a specific error type and
        /// cannot be null, empty or entirely whitespace.</param>
        /// <param name="errorCategory">An <seealso cref="ErrorCategory"/> constant that defines the category of the error (for display purposes).</param>
        /// <param name="activity">Description of the activity that caused the error.</param>
        /// <param name="reason">Description of the reason for the error.</param>
        /// <param name="targetObject">The object that was being operated on when the error occurred.</param>
        /// <returns><seealso cref="ErrorRecord"/> object that was added.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="message"/>, <paramref name="exception"/> or <paramref name="errorId"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="message"/> or <paramref name="errorId"/> is empty.</exception>
        public ErrorRecord AddError(string message, string recommendedAction, Exception exception, string errorId, ErrorCategory errorCategory, string activity, string reason, object targetObject)
        {
            if (message == null)
                throw new ArgumentNullException("message");
            
            if (message.Trim().Length == 0)
                throw new ArgumentException("Message cannot be empty.", "message");
            
            return _AddError(message, recommendedAction, exception, errorId, errorCategory, activity, reason, targetObject);
        }

        /// <summary>
        /// Adds an error record to the <see cref="ErrorRecords"/> collection.
        /// </summary>
        /// <param name="message">The replacement error message for the exception. This cannot be null, empty or entirely whitespace.</param>
        /// <param name="recommendedAction">Recommended action that should be taken as a result of this error.</param>
        /// <param name="exception">The exception that is associated with this record. This cannot be null.</param>
        /// <param name="errorId">A developer-defined identifier of the error. This identifier must be a non-localized string for a specific error type and
        /// cannot be null, empty or entirely whitespace.</param>
        /// <param name="errorCategory">An <seealso cref="ErrorCategory"/> constant that defines the category of the error (for display purposes).</param>
        /// <param name="activity">Description of the activity that caused the error.</param>
        /// <param name="targetObject">The object that was being operated on when the error occurred.</param>
        /// <returns><seealso cref="ErrorRecord"/> object that was added.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="message"/>, <paramref name="exception"/> or <paramref name="errorId"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="message"/> or <paramref name="errorId"/> is empty.</exception>
        public ErrorRecord AddError(string message, string recommendedAction, Exception exception, string errorId, ErrorCategory errorCategory, string activity, object targetObject)
        {
            return AddError(message, recommendedAction, exception, errorId, errorCategory, activity, null, targetObject);
        }

        /// <summary>
        /// Adds an error record to the <see cref="ErrorRecords"/> collection.
        /// </summary>
        /// <param name="message">The replacement error message for the exception.</param>
        /// <param name="exception">The exception that is associated with this record. This cannot be null.</param>
        /// <param name="errorId">A developer-defined identifier of the error. This identifier must be a non-localized string for a specific error type and
        /// cannot be null, empty or entirely whitespace.</param>
        /// <param name="errorCategory">An <seealso cref="ErrorCategory"/> constant that defines the category of the error (for display purposes).</param>
        /// <param name="activity">Description of the activity that caused the error.</param>
        /// <param name="reason">Description of the reason for the error.</param>
        /// <param name="targetObject">The object that was being operated on when the error occurred.</param>
        /// <returns><seealso cref="ErrorRecord"/> object that was added.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="exception"/> or <paramref name="errorId"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="errorId"/> is empty.</exception>
        public ErrorRecord AddError(string message, Exception exception, string errorId, ErrorCategory errorCategory, string activity, string reason, object targetObject)
        {
            return _AddError(message, null, exception, errorId, errorCategory, activity, reason, targetObject);
        }

        /// <summary>
        /// Adds an error record to the <see cref="ErrorRecords"/> collection.
        /// </summary>
        /// <param name="message">The replacement error message for the exception.</param>
        /// <param name="exception">The exception that is associated with this record. This cannot be null.</param>
        /// <param name="errorId">A developer-defined identifier of the error. This identifier must be a non-localized string for a specific error type and
        /// cannot be null, empty or entirely whitespace.</param>
        /// <param name="errorCategory">An <seealso cref="ErrorCategory"/> constant that defines the category of the error (for display purposes).</param>
        /// <param name="activity">Description of the activity that caused the error.</param>
        /// <param name="targetObject">The object that was being operated on when the error occurred.</param>
        /// <returns><seealso cref="ErrorRecord"/> object that was added.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="exception"/> or <paramref name="errorId"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="errorId"/> is empty.</exception>
        public ErrorRecord AddError(string message, Exception exception, string errorId, ErrorCategory errorCategory, string activity, object targetObject)
        {
            return _AddError(message, null, exception, errorId, errorCategory, activity, null, targetObject);
        }

        /// <summary>
        /// Adds an error record to the <see cref="ErrorRecords"/> collection.
        /// </summary>
        /// <param name="message">The replacement error message for the exception. This cannot be null, empty or entirely whitespace.</param>
        /// <param name="recommendedAction">Recommended action that should be taken as a result of this error.</param>
        /// <param name="exception">The exception that is associated with this record. This cannot be null.</param>
        /// <param name="errorId">A developer-defined identifier of the error. This identifier must be a non-localized string for a specific error type and
        /// cannot be null, empty or entirely whitespace.</param>
        /// <param name="errorCategory">An <seealso cref="ErrorCategory"/> constant that defines the category of the error (for display purposes).</param>
        /// <param name="targetObject">The object that was being operated on when the error occurred.</param>
        /// <returns><seealso cref="ErrorRecord"/> object that was added.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="message"/>, <paramref name="exception"/> or <paramref name="errorId"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="message"/> or <paramref name="errorId"/> is empty.</exception>
        public ErrorRecord AddError(string message, string recommendedAction, Exception exception, string errorId, ErrorCategory errorCategory, object targetObject)
        {
            return AddError(message, recommendedAction, exception, errorId, errorCategory, null, null, targetObject);
        }
        
        /// <summary>
        /// Adds an error record to the <see cref="ErrorRecords"/> collection.
        /// </summary>
        /// <param name="message">The replacement error message for the exception. This cannot be null, empty or entirely whitespace.</param>
        /// <param name="recommendedAction">Recommended action that should be taken as a result of this error.</param>
        /// <param name="exception">The exception that is associated with this record. This cannot be null.</param>
        /// <param name="errorId">A developer-defined identifier of the error. This identifier must be a non-localized string for a specific error type and
        /// cannot be null, empty or entirely whitespace.</param>
        /// <param name="errorCategory">An <seealso cref="ErrorCategory"/> constant that defines the category of the error (for display purposes).</param>
        /// <returns><seealso cref="ErrorRecord"/> object that was added.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="message"/>, <paramref name="exception"/> or <paramref name="errorId"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="message"/> or <paramref name="errorId"/> is empty.</exception>
        public ErrorRecord AddError(string message, string recommendedAction, Exception exception, string errorId, ErrorCategory errorCategory)
        {
            return AddError(message, recommendedAction, exception, errorId, errorCategory, null, null, null);
        }
        
        /// <summary>
        /// Adds an error record to the <see cref="ErrorRecords"/> collection.
        /// </summary>
        /// <param name="message">The replacement error message for the exception. This cannot be null, empty or entirely whitespace.</param>
        /// <param name="recommendedAction">Recommended action that should be taken as a result of this error.</param>
        /// <param name="exception">The exception that is associated with this record. This cannot be null.</param>
        /// <param name="errorId">A developer-defined identifier of the error. This identifier must be a non-localized string for a specific error type and
        /// cannot be null, empty or entirely whitespace.</param>
        /// <returns><seealso cref="ErrorRecord"/> object that was added.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="message"/>, <paramref name="exception"/> or <paramref name="errorId"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="message"/> or <paramref name="errorId"/> is empty.</exception>
        public ErrorRecord AddError(string message, string recommendedAction, Exception exception, string errorId)
        {
            return AddError(message, recommendedAction, exception, errorId, ErrorCategoryFromExceptionType(exception));
        }
        
        /// <summary>
        /// Adds an error record to the <see cref="ErrorRecords"/> collection.
        /// </summary>
        /// <param name="message">The replacement error message for the exception.</param>
        /// <param name="exception">The exception that is associated with this record. This cannot be null.</param>
        /// <param name="errorId">A developer-defined identifier of the error. This identifier must be a non-localized string for a specific error type and
        /// cannot be null, empty or entirely whitespace.</param>
        /// <param name="errorCategory">An <seealso cref="ErrorCategory"/> constant that defines the category of the error (for display purposes).</param>
        /// <param name="targetObject">The object that was being operated on when the error occurred.</param>
        /// <returns><seealso cref="ErrorRecord"/> object that was added.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="exception"/> or <paramref name="errorId"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="errorId"/> is empty.</exception>
        public ErrorRecord AddError(string message, Exception exception, string errorId, ErrorCategory errorCategory, object targetObject)
        {
            return _AddError(message, null, exception, errorId, errorCategory, null, null, targetObject);
        }
        
        /// <summary>
        /// Adds an error record to the <see cref="ErrorRecords"/> collection.
        /// </summary>
        /// <param name="message">The replacement error message for the exception.</param>
        /// <param name="exception">The exception that is associated with this record. This cannot be null.</param>
        /// <param name="errorId">A developer-defined identifier of the error. This identifier must be a non-localized string for a specific error type and
        /// cannot be null, empty or entirely whitespace.</param>
        /// <param name="errorCategory">An <seealso cref="ErrorCategory"/> constant that defines the category of the error (for display purposes).</param>
        /// <returns><seealso cref="ErrorRecord"/> object that was added.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="exception"/> or <paramref name="errorId"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="errorId"/> is empty.</exception>
        public ErrorRecord AddError(string message, Exception exception, string errorId, ErrorCategory errorCategory)
        {
            return _AddError(message, null, exception, errorId, errorCategory, null, null, null);
        }
        
        /// <summary>
        /// Adds an error record to the <see cref="ErrorRecords"/> collection.
        /// </summary>
        /// <param name="message">The replacement error message for the exception.</param>
        /// <param name="exception">The exception that is associated with this record. This cannot be null.</param>
        /// <param name="errorId">A developer-defined identifier of the error. This identifier must be a non-localized string for a specific error type and
        /// cannot be null, empty or entirely whitespace.</param>
        /// <returns><seealso cref="ErrorRecord"/> object that was added.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="exception"/> or <paramref name="errorId"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="errorId"/> is empty.</exception>
        public ErrorRecord AddError(string message, Exception exception, string errorId)
        {
            return AddError(message, exception, errorId, ErrorCategoryFromExceptionType(exception));
        }
        
        /// <summary>
        /// Adds an error record to the <see cref="ErrorRecords"/> collection.
        /// </summary>
        /// <param name="exception">The exception that is associated with this record. This cannot be null.</param>
        /// <param name="errorId">A developer-defined identifier of the error. This identifier must be a non-localized string for a specific error type and
        /// cannot be null, empty or entirely whitespace.</param>
        /// <param name="errorCategory">An <seealso cref="ErrorCategory"/> constant that defines the category of the error (for display purposes).</param>
        /// <param name="activity">Description of the activity that caused the error.</param>
        /// <param name="reason">Description of the reason for the error.</param>
        /// <param name="targetObject">The object that was being operated on when the error occurred.</param>
        /// <returns><seealso cref="ErrorRecord"/> object that was added.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="exception"/> or <paramref name="errorId"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="errorId"/> is empty.</exception>
        public ErrorRecord AddError(Exception exception, string errorId, ErrorCategory errorCategory, string activity, string reason, object targetObject)
        {
            return _AddError(null, null, exception, errorId, errorCategory, activity, reason, targetObject);
        }

        /// <summary>
        /// Adds an error record to the <see cref="ErrorRecords"/> collection.
        /// </summary>
        /// <param name="exception">The exception that is associated with this record. This cannot be null.</param>
        /// <param name="errorId">A developer-defined identifier of the error. This identifier must be a non-localized string for a specific error type and
        /// cannot be null, empty or entirely whitespace.</param>
        /// <param name="errorCategory">An <seealso cref="ErrorCategory"/> constant that defines the category of the error (for display purposes).</param>
        /// <param name="activity">Description of the activity that caused the error.</param>
        /// <param name="targetObject">The object that was being operated on when the error occurred.</param>
        /// <returns><seealso cref="ErrorRecord"/> object that was added.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="exception"/> or <paramref name="errorId"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="errorId"/> is empty.</exception>
        public ErrorRecord AddError(Exception exception, string errorId, ErrorCategory errorCategory, string activity, object targetObject)
        {
            return _AddError(null, null, exception, errorId, errorCategory, activity, null, targetObject);
        }

        /// <summary>
        /// Adds an error record to the <see cref="ErrorRecords"/> collection.
        /// </summary>
        /// <param name="exception">The exception that is associated with this record. This cannot be null.</param>
        /// <param name="errorId">A developer-defined identifier of the error. This identifier must be a non-localized string for a specific error type and
        /// cannot be null, empty or entirely whitespace.</param>
        /// <param name="errorCategory">An <seealso cref="ErrorCategory"/> constant that defines the category of the error (for display purposes).</param>
        /// <param name="targetObject">The object that was being operated on when the error occurred.</param>
        /// <returns><seealso cref="ErrorRecord"/> object that was added.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="exception"/> or <paramref name="errorId"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="errorId"/> is empty.</exception>
        public ErrorRecord AddError(Exception exception, string errorId, ErrorCategory errorCategory, object targetObject)
        {
            return _AddError(null, null, exception, errorId, errorCategory, null, null, targetObject);
        }
        
        /// <summary>
        /// Adds an error record to the <see cref="ErrorRecords"/> collection.
        /// </summary>
        /// <param name="exception">The exception that is associated with this record. This cannot be null.</param>
        /// <param name="errorId">A developer-defined identifier of the error. This identifier must be a non-localized string for a specific error type and
        /// cannot be null, empty or entirely whitespace.</param>
        /// <param name="errorCategory">An <seealso cref="ErrorCategory"/> constant that defines the category of the error (for display purposes).</param>
        /// <returns><seealso cref="ErrorRecord"/> object that was added.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="exception"/> or <paramref name="errorId"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="errorId"/> is empty.</exception>
        public ErrorRecord AddError(Exception exception, string errorId, ErrorCategory errorCategory)
        {
            return _AddError(null, null, exception, errorId, errorCategory, null, null, null);
        }
        
        /// <summary>
        /// Adds an error record to the <see cref="ErrorRecords"/> collection.
        /// </summary>
        /// <param name="exception">The exception that is associated with this record. This cannot be null.</param>
        /// <param name="errorId">A developer-defined identifier of the error. This identifier must be a non-localized string for a specific error type and
        /// cannot be null, empty or entirely whitespace.</param>
        /// <returns><seealso cref="ErrorRecord"/> object that was added.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="exception"/> or <paramref name="errorId"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="errorId"/> is empty.</exception>
        public ErrorRecord AddError(Exception exception, string errorId)
        {
            return AddError(exception, errorId, ErrorCategoryFromExceptionType(exception));
        }
        
        public static ErrorCategory ErrorCategoryFromExceptionType(Exception exception)
        {
            if (exception == null)
                return ErrorCategory.NotSpecified;
            
            if (exception is ArgumentException || exception is IndexOutOfRangeException || exception is System.IO.PathTooLongException || exception is ParameterBindingException ||
                    exception is RankException)
                return ErrorCategory.InvalidArgument;
            
            if (exception is FormatException || exception is System.Security.XmlSyntaxException)
                return ErrorCategory.SyntaxError;
            
            if (exception is InvalidOperationException || exception is ArithmeticException || exception is System.IO.InternalBufferOverflowException ||
                    exception is ScriptRequiresException || exception is StackOverflowException)
                return ErrorCategory.InvalidOperation;
            
            if (exception is DataMisalignedException || exception is System.IO.InvalidDataException)
                return ErrorCategory.InvalidData;
            
            if (exception is ParseException || exception is System.Xml.XmlException || exception is System.Xml.Schema.XmlSchemaException)
                return ErrorCategory.ParserError;
            
            if (exception is System.Runtime.Serialization.SerializationException)
                return ErrorCategory.WriteError;
            
            if (exception is System.Reflection.AmbiguousMatchException)
                return ErrorCategory.InvalidResult;
            
            if (exception is System.IO.EndOfStreamException)
                return ErrorCategory.ReadError;
            
            if (exception is MemberAccessException || exception is OutOfMemoryException)
                return ErrorCategory.ResourceUnavailable;
            
            if (exception is NotImplementedException || exception is NotSupportedException || exception is MulticastNotSupportedException)	
                return ErrorCategory.NotImplemented;
            
            if (exception is MetadataException)
                return ErrorCategory.MetadataError;
            
            if (exception is System.Configuration.SettingsPropertyNotFoundException || exception is Microsoft.PowerShell.Commands.HelpNotFoundException ||
                    exception is KeyNotFoundException || exception is System.IO.DirectoryNotFoundException || exception is System.IO.DriveNotFoundException ||
                    exception is System.IO.FileNotFoundException || exception is CommandNotFoundException || exception is DriveNotFoundException ||
                    exception is ItemNotFoundException || exception is ProviderNotFoundException || exception is System.Resources.MissingManifestResourceException ||
                    exception is System.Resources.MissingSatelliteAssemblyException || exception is DllNotFoundException)
                return ErrorCategory.ObjectNotFound;
            
            if (exception is ArrayTypeMismatchException || exception is System.Configuration.SettingsPropertyWrongTypeException || exception is InvalidCastException ||
                    exception is ExtendedTypeSystemException || exception is NullReferenceException || exception is TypeLoadException)
                return ErrorCategory.InvalidType;
            
            if (exception is System.Threading.WaitHandleCannotBeOpenedException || exception is System.IO.FileLoadException)
                return ErrorCategory.OpenError;
            
            if (exception is PipelineClosedException || exception is PipelineStoppedException || exception is OperationCanceledException || exception is System.Threading.ThreadAbortException || 
                    exception is System.Threading.ThreadInterruptedException)
                return ErrorCategory.OperationStopped;
            
            if (exception is TimeoutException)
                return ErrorCategory.OperationTimeout;
            
            if (exception is System.Configuration.SettingsPropertyIsReadOnlyException || exception is AccessViolationException || exception is System.ComponentModel.LicenseException ||
                    exception is SessionStateUnauthorizedAccessException || exception is UnauthorizedAccessException)
                return ErrorCategory.PermissionDenied;
            
            if (exception is PSSecurityException || exception is System.Security.Authentication.AuthenticationException || exception is System.Security.Cryptography.CryptographicException ||
                    exception is System.Security.HostProtectionException || exception is System.Security.Policy.PolicyException ||
                    exception is System.Security.Principal.IdentityNotMappedException || exception is System.Security.SecurityException || exception is System.Security.VerificationException)
                return ErrorCategory.SecurityError;
            
            if (exception is System.IO.IsolatedStorage.IsolatedStorageException)
                return ErrorCategory.DeviceError;
            
            if (exception is ScriptBlockToPowerShellNotSupportedException)
                return ErrorCategory.NotInstalled;
            
            return ErrorCategory.NotSpecified;
        }
        
        #endregion
        
        #region Validation Overrides
        
        /// <summary>
        /// Asserts that a string value contains valid XML markup that represents a WPF window.
        /// </summary>
        /// <param name="value">The XML to be parsed.</param>
        /// <param name="xmlDocument">The parsed XML document object.</param>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="value"/> is empty or contains no markup.</exception>
        /// <exception cref="XmlException"><paramref name="value"/> does not contain valid XML.</exception>
        /// <exception cref="KeyNotFoundException">The root element of <paramref name="value"/> is not a WPF window.</exception>
        public static void AssertValidXaml(string value, out XmlDocument xmlDocument)
        {
            if (value == null)
                throw new ArgumentNullException("value");
            
            string xml = value.Trim();
            
            if (xml.Length == 0)
                throw new ArgumentException("XML Markup is empty.", "value");
            
            XmlDocument doc = new XmlDocument();
            
            try { doc.LoadXml(xml); }
            catch (Exception exception) { throw new ArgumentException("Invalid XML markup", "value", exception); }
            
            if (doc.DocumentElement == null)
                throw new ArgumentException("XML document is empty", "value");
            
            if (doc.DocumentElement.NamespaceURI != XamlUtility.XmlNamespaceURI_Presentation)
                throw new ArgumentException("XML markup does not represent a WPF document.", "value");
            
            if (doc.DocumentElement.LocalName != "Window")
                throw new ArgumentException("XAML markup does not represent a window.", "value");
            
            xmlDocument = doc;
        }
        
        /// <summary>
        /// Attempts to validate that a string value contains valid XML markup which represents a WPF window.
        /// </summary>
        /// <param name="value">The XML to be parsed.</param>
        /// <param name="exception">Exception that was thrown while trying to validate the document.</param>
        /// <param name="xmlDocument">The parsed XML document object or null if an exception occurred.</param>
        /// <returns>True if the XML is valid; otherwise false. This invokes <seealso cref="AssertValidXaml(string, out XmlDocument)"/> to validate the XML.</returns>
        public static bool TryValidateXaml(string value, out Exception exception, out XmlDocument xmlDocument)
        {
            try 
            {
                AssertValidXaml(value, out xmlDocument);
                exception = null;
                return true;
            }
            catch (Exception exc)
            {
                xmlDocument = null;
                exception = exc;
                return false;
            }
        }
        
        /// <summary>
        /// Attempts to validate that a string value contains valid XML markup which represents a WPF window.
        /// </summary>
        /// <param name="value">The XML to be parsed.</param>
        /// <param name="message">Message which represents the result of validation.</param>
        /// <returns>True if the XML is valid; otherwise false.</returns>
        public static bool TryValidateXaml(string value, out string message)
        {
            Exception exception;
            XmlDocument xmlDocument;
            if (TryValidateXaml(value, out exception, out xmlDocument))
            {
                message = "Markup is valid.";
                return true;
            }
            
            message = exception.Message;
            return false;
        }
        
        #endregion
        
        /// <summary>
        /// Displays WPF window as a dialog (modal).
        /// </summary>
        /// <param name="host">The PowerShell host to be used.</param>
        /// <returns>A value that specifies whether the activity was accepted (true) or canceled (false). The return value is the value of the DialogResult property before the window closes.</returns>
        public bool? ShowDialog(PSHost host)
        {
            if (_openWindowProcess != null)
                throw new InvalidOperationException("Window is already open.");
            
            if (_windowXaml == null)
                throw new InvalidOperationException("No window XAML has been defined.");
            
            using (WindowProcessInternal openWindowProcess  = new WindowProcessInternal(this, _windowXaml, true, host))
            {
                _openWindowProcess = openWindowProcess;
                try { return openWindowProcess.ShowDialog(); }
                catch { throw; }
                finally { _openWindowProcess = null; }
            }
        }
        
        /// <summary>
        /// Displays WPF window, permitting child windows to be created.
        /// </summary>
        /// <param name="host">The PowerShell host to be used.</param>
        /// <returns>A value that specifies whether the activity was accepted (true) or canceled (false). The return value is the value of the DialogResult property before the window closes.</returns>
        public bool? Show(PSHost host)
        {
            if (_openWindowProcess != null)
                throw new InvalidOperationException("Window is already open.");
            
            if (_windowXaml == null)
                throw new InvalidOperationException("No window XAML has been defined.");
            
            using (WindowProcessInternal openWindowProcess  = new WindowProcessInternal(this, _windowXaml, false, host))
            {
                _openWindowProcess = openWindowProcess;
                try { return openWindowProcess.Show(); }
                catch { throw; }
                finally { _openWindowProcess = null; }
            }
        }
        
        #endregion
    }
}