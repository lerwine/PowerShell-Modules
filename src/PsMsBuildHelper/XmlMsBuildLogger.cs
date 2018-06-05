using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace PsMsBuildHelper
{
    public class XmlMsBuildLogger : Logger, INodeLogger
    {
        public const string RoundTrimDateTimeFormat = "yyyy-MM-ddTHH:mm:ss.fffffffzzzzzz";
        private XmlWriter _writer = null;
        private object _syncRoot = new object();
        private IEventSource _eventSource = null;
        private int _cpuCount = -1;
        private string _outputPath = "";
        private StopwatchDictionary _stopwatches = null;
        static class ParameterParseHelper
        {
            public static readonly Regex _boolRegex = new Regex(@"^((?<false>f(alse)?|no?|-|[+\-]?0+(\.0+)?))|(?<true>)t(rue)?|y(es)?|\+|[+\-]?(0*[1-9]\d*(\.\d+)?|0+\.0*[1-9]\d*))$", RegexOptions.IgnoreCase);
            internal static bool ParseBoolean(string parameterName, string text)
            {
                Match m = _boolRegex.Match(text.Trim());
                if (!m.Success)
                    throw new Exception("'" + text + "' is not recognized yes/no value for the '" + parameterName + "' parameter.");
                return m.Groups["true"].Success;
            }
            internal static TEnum ParseEnum<TEnum>(string parameterName, string text)
                where TEnum : struct
            {
                TEnum result;
                if (!Enum.TryParse<TEnum>(text.Trim(), out result))
                    throw new Exception("'" + text + "' is not recognized option for the '" + parameterName + "' parameter.");
                return result;
            }
        }
        
        private void _Initialize(IEventSource eventSource, Int32 nodeCount)
        {
            if (eventSource == null)
                throw new ArgumentNullException("eventSource");
            Monitor.Enter(_syncRoot);
            try
            {
                if (_eventSource != null)
                {
                    if (!ReferenceEquals(_eventSource, eventSource))
                        throw new ArgumentException("Only one event source can be logged at a time", "eventSource");
                    _cpuCount = nodeCount;
                    return;
                }
                XmlWriterSettings settings = new XmlWriterSettings
                {
                    Async = true,
                    CheckCharacters = false,
                    Indent = true,
                    WriteEndDocumentOnClose = true
                };
                
                bool emitIdentifier = false;
                bool bigEndian = false;
                bool allowOpt = false;
                foreach (var g in ((Parameters == null) ? "" : Parameters.Trim()).Split(Path.PathSeparator).Select(p =>
                {
                    string[] kvp = p.Split(new char[] { '=' }, 2);
                    if (kvp.Length == 1)
                        return new { Key = "Path", Value = kvp[0].Trim() };
                    return new { Key = kvp[0].Trim(), Value = kvp[1].Trim() };
                }).GroupBy(kvp => kvp.Key, StringComparer.InvariantCultureIgnoreCase))
                {
                    if (g.Count() > 0)
                        throw new Exception("Parameter '" + g.Key + "' cannot be defined more than once.");
                    string text = g.First().Value;
                    switch (g.Key.ToLower())
                    {
                        case "path":
                            if (text.Length == 0)
                                throw new Exception("Parameter '" + g.Key + "' cannot be a zero-length string.");
                            _outputPath = text.Trim();
                            break;
                        case "writeenddocumentonclose":
                            settings.WriteEndDocumentOnClose = ParameterParseHelper.ParseBoolean(g.Key, text);
                            break;
                        case "namespacehandling":
                            settings.NamespaceHandling = ParameterParseHelper.ParseEnum<NamespaceHandling>(g.Key, text);
                            break;
                        case "checkcharacters":
                            settings.CheckCharacters = ParameterParseHelper.ParseBoolean(g.Key, text);
                            break;
                        case "conformancelevel":
                            settings.ConformanceLevel = ParameterParseHelper.ParseEnum<ConformanceLevel>(g.Key, text);
                            break;
                        case "newlineonattributes":
                            settings.NewLineOnAttributes = ParameterParseHelper.ParseBoolean(g.Key, text);
                            break;
                        case "indentchars":
                            if (text.Length == 0)
                                throw new Exception("Parameter '" + g.Key + "' cannot be a zero-length string.");
                            settings.IndentChars = text;
                            break;
                        case "indent":
                            settings.Indent = ParameterParseHelper.ParseBoolean(g.Key, text);
                            break;
                        case "newlinehandling":
                            settings.NewLineHandling = ParameterParseHelper.ParseEnum<NewLineHandling>(g.Key, text);
                            break;
                        case "omitxmldeclaration":
                            settings.OmitXmlDeclaration = ParameterParseHelper.ParseBoolean(g.Key, text);
                            break;
                        case "donotescapeuriattributes":
                            settings.DoNotEscapeUriAttributes = ParameterParseHelper.ParseBoolean(g.Key, text);
                            break;
                        case "emitidentifier":
                            emitIdentifier = ParameterParseHelper.ParseBoolean(g.Key, text);
                            break;
                        case "bigendian":
                            bigEndian = ParameterParseHelper.ParseBoolean(g.Key, text);
                            break;
                        case "allowopt":
                            allowOpt = ParameterParseHelper.ParseBoolean(g.Key, text);
                            break;
                        case "encoding":
                            if ((text = text.Trim()).Length == 0)
                                throw new Exception("Parameter '" + g.Key + "' cannot be a zero-length string.");
                            Encoding encoding;
                            try { encoding = Encoding.GetEncoding(text.Trim()); }
                            catch { encoding = null; }
                            if (encoding == null)
                                throw new Exception("'" + text + "' is not recognized yes/no value for the '" + g.Key + "' parameter.");
                            break;
                    }
                }
                if (settings.Encoding is UTF8Encoding)
                    settings.Encoding = new UTF8Encoding(emitIdentifier);
                else if (settings.Encoding is UnicodeEncoding)
                    settings.Encoding = new UnicodeEncoding(bigEndian, emitIdentifier);
                else if (settings.Encoding is UTF32Encoding)
                    settings.Encoding = new UTF32Encoding(bigEndian, emitIdentifier);
                else if (settings.Encoding is UTF7Encoding)
                    settings.Encoding = new UTF7Encoding(allowOpt);
                else if (String.IsNullOrEmpty(_outputPath))
                    throw new Exception("Path parameter not provided");
                _writer = XmlWriter.Create(_outputPath, settings);
                _writer.WriteStartElement("BuildResult");
                _writer.WriteAttributeString("Started", XmlConvert.ToString(DateTime.Now, RoundTrimDateTimeFormat));
                _writer.WriteAttributeString("CpuCount", XmlConvert.ToString(nodeCount));
                _eventSource = eventSource;
                _cpuCount = nodeCount;
                eventSource.MessageRaised += OnMessageRaised;
                eventSource.ErrorRaised += OnErrorRaised;
                eventSource.WarningRaised += OnWarningRaised;
                eventSource.BuildStarted += OnBuildStarted;
                eventSource.BuildFinished += OnBuildFinished;
                eventSource.ProjectStarted += OnProjectStarted;
                eventSource.ProjectFinished += OnProjectFinished;
                eventSource.TargetStarted += OnTargetStarted;
                eventSource.TargetFinished += OnTargetFinished;
                eventSource.TaskStarted += OnTaskStarted;
                eventSource.TaskFinished += OnTaskFinished;
                eventSource.CustomEventRaised += OnCustomEventRaised;
            }
            catch (Exception exception)
            {
                throw new LoggerException((String.IsNullOrEmpty(exception.Message)) ? "Unexpected " + exception.GetType().Name : exception.Message, exception);
            }
            finally { Monitor.Exit(_syncRoot); }
        }

        public void Initialize(IEventSource eventSource, Int32 nodeCount)
        {
            Initialize(eventSource, nodeCount);
        }
        public override void Initialize(IEventSource eventSource)
        {
            Initialize(eventSource, -1);
        }
        public override void Shutdown()
        {
            Monitor.Enter(_syncRoot);
            try
            {
                if (_eventSource != null)
                {
                    if (_writer != null)
                    {
                        try
                        {
                            _writer.WriteStartElement("Completed");
                            try
                            {
                                _writer.WriteAttributeString("TotalDuration", XmlConvert.ToString(_stopwatches.Stop()));
                            }
                            finally { _writer.WriteEndElement(); }
                        }
                        finally
                        {
                            try
                            {
                                _writer.WriteEndElement();
                                _writer.Flush();
                            }
                            finally
                            {
                                try { _writer.Close(); }
                                finally { _writer = null; }
                            }
                        }
                    }
                    _eventSource.MessageRaised -= OnMessageRaised;
                    _eventSource.ErrorRaised -= OnErrorRaised;
                    _eventSource.WarningRaised -= OnWarningRaised;
                    _eventSource.BuildStarted -= OnBuildStarted;
                    _eventSource.BuildFinished -= OnBuildFinished;
                    _eventSource.ProjectStarted -= OnProjectStarted;
                    _eventSource.ProjectFinished -= OnProjectFinished;
                    _eventSource.TargetStarted -= OnTargetStarted;
                    _eventSource.TargetFinished -= OnTargetFinished;
                    _eventSource.TaskStarted -= OnTaskStarted;
                    _eventSource.TaskFinished -= OnTaskFinished;
                    _eventSource.CustomEventRaised -= OnCustomEventRaised;
                    _eventSource = null;
                }
            }
            finally { Monitor.Exit(_syncRoot); }
        }
        public static readonly Regex RequiresCDataRegex = new Regex(@"^(\s+|[^\p{C}<>&""]*([\r\n\t]+[^\p{C}<>&""]+)*[^\p{C}<>&""]|\S+(\s+\S+)*\s)", RegexOptions.Compiled);
        private void WriteBuildEventContext(BuildEventContext context, string elementName = "Context")
        {
            if (context == null)
                return;
            _writer.WriteStartElement(elementName);
            try
            {
                _writer.WriteAttributeString("BuildRequest", XmlConvert.ToString(context.BuildRequestId));
                if (context.SubmissionId != BuildEventContext.InvalidSubmissionId)
                    _writer.WriteAttributeString("Submission", XmlConvert.ToString(context.SubmissionId));
                if (context.ProjectContextId != BuildEventContext.InvalidProjectContextId)
                    _writer.WriteAttributeString("ProjectContext", XmlConvert.ToString(context.ProjectContextId));
                if (context.ProjectInstanceId != BuildEventContext.InvalidProjectInstanceId)
                    _writer.WriteAttributeString("ProjectInstance", XmlConvert.ToString(context.ProjectInstanceId));
                if (context.TargetId != BuildEventContext.InvalidTargetId)
                    _writer.WriteAttributeString("Target", XmlConvert.ToString(context.TargetId));
                if (context.TaskId != BuildEventContext.InvalidTaskId)
                    _writer.WriteAttributeString("Task", XmlConvert.ToString(context.TaskId));
            }
            finally { _writer.WriteEndElement(); }
        }
        private void WriteEventArgsProperties(BuildEventArgs e, bool doNotWriteBuildEventContext = false)
        {
            _writer.WriteAttributeString("ThreadId", XmlConvert.ToString(e.ThreadId));
            _writer.WriteAttributeString("Timestamp", XmlConvert.ToString(e.Timestamp, RoundTrimDateTimeFormat));
            if (e.HelpKeyword != null)
                _writer.WriteAttributeString("HelpKeyword", e.HelpKeyword);
            if (e.HelpKeyword != null)
                _writer.WriteAttributeString("SenderName", e.SenderName);
            if (e.Message != null)
            {
                _writer.WriteStartElement("Message");
                try
                {
                    if (e.Message.Length > 0 && RequiresCDataRegex.IsMatch(e.Message))
                        _writer.WriteCData(e.Message);
                    else
                        _writer.WriteString(e.Message);
                }
                finally { _writer.WriteEndElement(); }
            }
            if (!doNotWriteBuildEventContext)
                WriteBuildEventContext(e.BuildEventContext);
        }
        private void OnBuildStarted(object sender, BuildStartedEventArgs e)
        {
            Monitor.Enter(_syncRoot);
            try
            {
                _stopwatches.StartNewBuild(e.BuildEventContext);
                _writer.WriteStartElement("BuildStarted");
                try
                {
                    
                    WriteEventArgsProperties(e);
                    if (e.BuildEnvironment != null && e.BuildEnvironment.Count > 0)
                    {
                        _writer.WriteStartElement("Environment");
                        try
                        {
                            foreach (string key in e.BuildEnvironment.Keys)
                            {
                                _writer.WriteStartElement("Property");
                                try
                                {
                                    _writer.WriteAttributeString("Name", key);
                                    string value = e.BuildEnvironment[key];
                                    if (e.BuildEnvironment[key] != null)
                                    {
                                        if (value.Length > 0 && RequiresCDataRegex.IsMatch(value))
                                            _writer.WriteCData(value);
                                        else
                                            _writer.WriteString(value);
                                    }
                                }
                                finally { _writer.WriteEndElement(); }
                            }
                        }
                        finally { _writer.WriteEndElement(); }
                    }
                }
                finally { _writer.WriteEndElement(); }
            }
            finally { Monitor.Exit(_syncRoot); }
        }

        private void OnBuildFinished(object sender, BuildFinishedEventArgs e)
        {
            Monitor.Enter(_syncRoot);
            try
            {
                _writer.WriteStartElement("BuildFinished");
                try
                {
                    _writer.WriteAttributeString("Succeeded", XmlConvert.ToString(e.Succeeded));
                    _writer.WriteAttributeString("Duration", XmlConvert.ToString(_stopwatches.StopBuild(e.BuildEventContext)));
                    WriteEventArgsProperties(e);
                }
                finally { _writer.WriteEndElement(); }
            }
            finally { Monitor.Exit(_syncRoot); }
        }
        private void WriteTypeInfo(Type type, string elementName = "Type", int maxDepth = 12)
        {
            if (type == null)
                return;
            _writer.WriteStartElement(elementName);
            try
            {
                _writer.WriteAttributeString("Name", type.Name);
                if (type.Namespace != null)
                    _writer.WriteAttributeString("Namespace", type.Namespace);
                if (type.IsClass && type.FullName != "System.String")
                    _writer.WriteAttributeString("Assembly", type.Assembly.FullName);
                if (type.IsGenericType)
                {
                    foreach (Type t in type.GetGenericArguments())
                        WriteTypeInfo(t, "GenericArgument", maxDepth - 1);
                }
                if (type.IsArray)
                    WriteTypeInfo(type.GetElementType(), "ElementType", maxDepth);
                else if (maxDepth > 0 && !type.IsPrimitive && type.FullName != "System.String")
                {
                    foreach (Type t in type.GetInterfaces())
                        WriteTypeInfo(t, "Interface", maxDepth - 1);
                    if (type.IsClass)
                        WriteTypeInfo(type.BaseType, "BaseType", maxDepth - 1);
                }
            }
            finally { _writer.WriteEndElement(); }
        }
        private void OnProjectStarted(object sender, ProjectStartedEventArgs e)
        {
            Monitor.Enter(_syncRoot);
            try
            {
                _stopwatches.StartNewProject(e.BuildEventContext);
                _writer.WriteStartElement("ProjectStarted");
                try
                {
                    _writer.WriteAttributeString("Id", XmlConvert.ToString(e.ProjectId));
                    if (e.ProjectFile != null)
                        _writer.WriteAttributeString("File", e.ProjectFile);
                    if (e.ToolsVersion != null)
                        _writer.WriteAttributeString("ToolsVersion", e.ToolsVersion);
                    WriteEventArgsProperties(e, true);
                    if (e.TargetNames != null && e.TargetNames.Length > 0)
                    {
                        _writer.WriteStartElement("TargetNames");
                        try
                        {
                            if (e.Message.Length > 0 && RequiresCDataRegex.IsMatch(e.TargetNames))
                                _writer.WriteCData(e.TargetNames);
                            else
                                _writer.WriteString(e.TargetNames);
                        }
                        finally { _writer.WriteEndElement(); }
                    }
                    WriteBuildEventContext(e.BuildEventContext);
                    if (e.ParentProjectBuildEventContext != null)
                        WriteBuildEventContext(e.ParentProjectBuildEventContext, "Parent");
                    if (e.Items != null)
                    {
                        _writer.WriteStartElement("Items");
                        try
                        {
                            WriteTypeInfo(e.Items.GetType());
                            foreach (object obj in e.Items)
                            {
                                if (obj != null)
                                    WriteTypeInfo(obj.GetType(), "Item");
                            }
                        }
                        finally { _writer.WriteEndElement(); }
                    }
                    if (e.Properties != null)
                    {
                        _writer.WriteStartElement("Properties");
                        try
                        {
                            _writer.WriteStartElement("Type");
                            try { WriteTypeInfo(e.Properties.GetType());}
                            finally { _writer.WriteEndElement(); }
                            foreach (object obj in e.Properties)
                            {
                                _writer.WriteStartElement("Property");
                                try
                                {
                                    if (obj != null)
                                        WriteTypeInfo(obj.GetType());
                                }
                                finally { _writer.WriteEndElement(); }
                            }
                        }
                        finally { _writer.WriteEndElement(); }
                    }
                    if (e.GlobalProperties != null && e.GlobalProperties.Count > 0)
                    {
                        _writer.WriteStartElement("GlobalProperties");
                        try
                        {
                            foreach (string key in e.GlobalProperties.Keys)
                            {
                                _writer.WriteStartElement("Property");
                                try
                                {
                                    _writer.WriteAttributeString("Name", key);
                                    string value = e.GlobalProperties[key];
                                    if (e.GlobalProperties[key] != null)
                                    {
                                        if (value.Length > 0 && RequiresCDataRegex.IsMatch(value))
                                            _writer.WriteCData(value);
                                        else
                                            _writer.WriteString(value);
                                    }
                                }
                                finally { _writer.WriteEndElement(); }
                            }
                        }
                        finally { _writer.WriteEndElement(); }
                    }
                }
                finally { _writer.WriteEndElement(); }
            }
            finally { Monitor.Exit(_syncRoot); }
        }

        private void OnProjectFinished(object sender, ProjectFinishedEventArgs e)
        {
            Monitor.Enter(_syncRoot);
            try
            {
                _writer.WriteStartElement("ProjectFinished");
                try
                {
                    _writer.WriteAttributeString("Succeeded", XmlConvert.ToString(e.Succeeded));
                    _writer.WriteAttributeString("Duration", XmlConvert.ToString(_stopwatches.StopProject(e.BuildEventContext)));
                    if (e.ProjectFile != null)
                        _writer.WriteAttributeString("File", e.ProjectFile);
                    WriteEventArgsProperties(e);
                }
                finally { _writer.WriteEndElement(); }
            }
            finally { Monitor.Exit(_syncRoot); }
        }
        private void OnTargetStarted(object sender, TargetStartedEventArgs e)
        {
            Monitor.Enter(_syncRoot);
            try
            {
                _stopwatches.StartNewTarget(e.BuildEventContext);
                _writer.WriteStartElement("TargetStarted");
                try
                {
                    if (e.TargetName != null)
                        _writer.WriteAttributeString("Name", e.TargetName);
                    if (e.TargetFile != null)
                        _writer.WriteAttributeString("File", e.TargetFile);
                    if (e.ProjectFile != null)
                        _writer.WriteAttributeString("ProjectFile", e.ProjectFile);
                    if (e.ParentTarget != null)
                        _writer.WriteAttributeString("Parent", e.ParentTarget);
                    WriteEventArgsProperties(e);
                }
                finally { _writer.WriteEndElement(); }
            }
            finally { Monitor.Exit(_syncRoot); }
        }
        private void OnTargetFinished(object sender, TargetFinishedEventArgs e)
        {
            Monitor.Enter(_syncRoot);
            try
            {
                _writer.WriteStartElement("TargetFinished");
                try
                {
                    _writer.WriteAttributeString("Succeeded", XmlConvert.ToString(e.Succeeded));
                    if (e.TargetName != null)
                        _writer.WriteAttributeString("Name", e.TargetName);
                    if (e.TargetFile != null)
                        _writer.WriteAttributeString("File", e.TargetFile);
                    if (e.ProjectFile != null)
                        _writer.WriteAttributeString("ProjectFile", e.ProjectFile);
                    _writer.WriteAttributeString("Duration", XmlConvert.ToString(_stopwatches.StopTarget(e.BuildEventContext)));
                    if (e.ProjectFile != null)
                        _writer.WriteAttributeString("File", e.ProjectFile);
                    WriteEventArgsProperties(e);
                }
                finally { _writer.WriteEndElement(); }
            }
            finally { Monitor.Exit(_syncRoot); }
        }
        private void OnTaskStarted(object sender, TaskStartedEventArgs e)
        {
            Monitor.Enter(_syncRoot);
            try
            {
                _stopwatches.StartNewTask(e.BuildEventContext);
                _writer.WriteStartElement("TaskStarted");
                try
                {
                    if (e.TaskName != null)
                        _writer.WriteAttributeString("Name", e.TaskName);
                    if (e.TaskFile != null)
                        _writer.WriteAttributeString("File", e.TaskFile);
                    if (e.ProjectFile != null)
                        _writer.WriteAttributeString("ProjectFile", e.ProjectFile);
                    WriteEventArgsProperties(e);
                }
                finally { _writer.WriteEndElement(); }
            }
            finally { Monitor.Exit(_syncRoot); }
        }
        private void OnTaskFinished(object sender, TaskFinishedEventArgs e)
        {
            Monitor.Enter(_syncRoot);
            try
            {
                _writer.WriteStartElement("TaskFinished");
                try
                {
                    _writer.WriteAttributeString("Succeeded", XmlConvert.ToString(e.Succeeded));
                    _writer.WriteAttributeString("Duration", XmlConvert.ToString(_stopwatches.StopTask(e.BuildEventContext)));
                    if (e.TaskName != null)
                        _writer.WriteAttributeString("Name", e.TaskName);
                    if (e.TaskFile != null)
                        _writer.WriteAttributeString("File", e.TaskFile);
                    if (e.ProjectFile != null)
                        _writer.WriteAttributeString("ProjectFile", e.ProjectFile);
                    WriteEventArgsProperties(e);
                }
                finally { _writer.WriteEndElement(); }
            }
            finally { Monitor.Exit(_syncRoot); }
        }
        private void OnMessageRaised(object sender, BuildMessageEventArgs e)
        {
            Monitor.Enter(_syncRoot);
            try
            {
                _writer.WriteStartElement("Message");
                try
                {
                    _writer.WriteAttributeString("Importance", Enum.GetName(e.Importance.GetType(), e.Importance));
                    if (e.LineNumber > 0)
                        _writer.WriteAttributeString("Line", XmlConvert.ToString(e.LineNumber));
                    if (e.ColumnNumber > 0)
                        _writer.WriteAttributeString("Column", XmlConvert.ToString(e.ColumnNumber));
                    if (e.EndLineNumber > 0 && e.EndLineNumber != e.LineNumber)
                        _writer.WriteAttributeString("EndLine", XmlConvert.ToString(e.EndLineNumber));
                    if (e.EndColumnNumber > 0 && e.EndColumnNumber != e.ColumnNumber)
                        _writer.WriteAttributeString("EndColumn", XmlConvert.ToString(e.EndColumnNumber));
                    if (e.Code != null)
                        _writer.WriteAttributeString("Code", e.Code);
                    if (e.Subcategory != null)
                        _writer.WriteAttributeString("Subcategory", e.Subcategory);
                    if (e.File != null)
                        _writer.WriteAttributeString("File", e.File);
                    if (e.ProjectFile != null)
                        _writer.WriteAttributeString("ProjectFile", e.ProjectFile);
                    WriteEventArgsProperties(e);
                }
                finally { _writer.WriteEndElement(); }
            }
            finally { Monitor.Exit(_syncRoot); }
        }
        private void OnErrorRaised(object sender, BuildErrorEventArgs e)
        {
            Monitor.Enter(_syncRoot);
            try
            {
                _writer.WriteStartElement("Error");
                try
                {
                    if (e.LineNumber > 0)
                        _writer.WriteAttributeString("Line", XmlConvert.ToString(e.LineNumber));
                    if (e.ColumnNumber > 0)
                        _writer.WriteAttributeString("Column", XmlConvert.ToString(e.ColumnNumber));
                    if (e.EndLineNumber > 0 && e.EndLineNumber != e.LineNumber)
                        _writer.WriteAttributeString("EndLine", XmlConvert.ToString(e.EndLineNumber));
                    if (e.EndColumnNumber > 0 && e.EndColumnNumber != e.ColumnNumber)
                        _writer.WriteAttributeString("EndColumn", XmlConvert.ToString(e.EndColumnNumber));
                    if (e.Code != null)
                        _writer.WriteAttributeString("Code", e.Code);
                    if (e.Subcategory != null)
                        _writer.WriteAttributeString("Subcategory", e.Subcategory);
                    if (e.File != null)
                        _writer.WriteAttributeString("File", e.File);
                    if (e.ProjectFile != null)
                        _writer.WriteAttributeString("ProjectFile", e.ProjectFile);
                    WriteEventArgsProperties(e);
                }
                finally { _writer.WriteEndElement(); }
            }
            finally { Monitor.Exit(_syncRoot); }
        }
        private void OnWarningRaised(object sender, BuildWarningEventArgs e)
        {
            Monitor.Enter(_syncRoot);
            try
            {
                _writer.WriteStartElement("Warning");
                try
                {
                    if (e.LineNumber > 0)
                        _writer.WriteAttributeString("Line", XmlConvert.ToString(e.LineNumber));
                    if (e.ColumnNumber > 0)
                        _writer.WriteAttributeString("Column", XmlConvert.ToString(e.ColumnNumber));
                    if (e.EndLineNumber > 0 && e.EndLineNumber != e.LineNumber)
                        _writer.WriteAttributeString("EndLine", XmlConvert.ToString(e.EndLineNumber));
                    if (e.EndColumnNumber > 0 && e.EndColumnNumber != e.ColumnNumber)
                        _writer.WriteAttributeString("EndColumn", XmlConvert.ToString(e.EndColumnNumber));
                    if (e.Code != null)
                        _writer.WriteAttributeString("Code", e.Code);
                    if (e.Subcategory != null)
                        _writer.WriteAttributeString("Subcategory", e.Subcategory);
                    if (e.File != null)
                        _writer.WriteAttributeString("File", e.File);
                    if (e.ProjectFile != null)
                        _writer.WriteAttributeString("ProjectFile", e.ProjectFile);
                    WriteEventArgsProperties(e);
                }
                finally { _writer.WriteEndElement(); }
            }
            finally { Monitor.Exit(_syncRoot); }
        }
        private void OnCustomEventRaised(object sender, CustomBuildEventArgs e)
        {
            Monitor.Enter(_syncRoot);
            try
            {
                _writer.WriteStartElement("CustomEvent");
                try
                {
                    WriteEventArgsProperties(e);
                }
                finally { _writer.WriteEndElement(); }
            }
            finally { Monitor.Exit(_syncRoot); }
        }
    }
}
