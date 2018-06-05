using System;
using System.Management.Automation;
using System.Management.Automation.Host;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.Build;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System.Diagnostics;

namespace LteDev
{
    public static class XmlHelper
    {
        public static XmlElement SetInnerText(this XmlElement element, string text)
        {
            if (text == null)
                element.IsEmpty = true;
            else
            {
                element.InnerText = text;
                if (text.Length > 12 && element.InnerXml.Length > (text.Length + 12))
                {
                    element.IsEmpty = true;
                    element.AppendChild(element.OwnerDocument.CreateCDataSection(text));
                }
            }
            return element;
        }
        public static XmlElement CreateElement<T>(this XmlNode node, T name)
            where T : struct, IComparable, IFormattable, IConvertible
        {
            return ((node is XmlDocument) ? (XmlDocument)node : node.OwnerDocument).CreateElement(Enum.GetName(name.GetType(), name));
        }
        
        public static XmlElement AppendElement<T>(this XmlElement parentElement, T name)
            where T : struct, IComparable, IFormattable, IConvertible
        {
            return (XmlElement)(parentElement.AppendChild(parentElement.OwnerDocument.CreateElement<T>(name)));
        }
        
        public static XmlElement ApplyAttribute<TName, TValue>(this XmlElement element, TName name, TValue value)
            where TName : struct, IComparable, IFormattable, IConvertible
            where TValue : struct, IComparable, IFormattable, IConvertible
        {
            return element.ApplyAttribute<TName>(name, Enum.GetName(value.GetType(), name));
        }
        
        public static XmlElement ApplyAttribute<T>(this XmlElement element, T name, string value)
            where T : struct, IComparable, IFormattable, IConvertible
        {
            element.SetAttribute(Enum.GetName(name.GetType(), name), value);
            return element;
        }
        
        public static XmlElement ApplyAttribute<T>(this XmlElement element, string name, string value)
            where T : struct, IComparable, IFormattable, IConvertible
        {
            element.SetAttribute(name, value);
            return element;
        }
        
        public static string GetAttributeValue(this XmlElement element, string name, string defaultValue = null)
        {
            XmlAttribute attribute = element.Attributes[name];
            return (attribute == null) ? defaultValue : attribute.Value;
        }
        
        public static string GetAttributeValue<T>(this XmlElement element, T name, string defaultValue = null)
            where T : struct, IComparable, IFormattable, IConvertible
        {
            XmlAttribute attribute = element.Attributes[Enum.GetName(name.GetType(), name)];
            return (attribute == null) ? defaultValue : attribute.Value;
        }
        
        public static TValue? GetAttributeValue<TName, TValue>(this XmlElement element, TName name, TValue? defaultValue = null)
            where TName : struct, IComparable, IFormattable, IConvertible
            where TValue : struct, IComparable, IFormattable, IConvertible
        {
            XmlAttribute attribute = element.Attributes[Enum.GetName(name.GetType(), name)];
            TValue value;
            return (attribute != null && Enum.TryParse(attribute.Value.Trim(), true, out value)) ? value : defaultValue;
        }
        
        public static TValue GetAttributeValueD<TName, TValue>(this XmlElement element, TName name, TValue defaultValue)
            where TName : struct, IComparable, IFormattable, IConvertible
            where TValue : struct, IComparable, IFormattable, IConvertible
        {
            XmlAttribute attribute = element.Attributes[Enum.GetName(name.GetType(), name)];
            TValue value;
            return (attribute != null && Enum.TryParse(attribute.Value.Trim(), true, out value)) ? value : defaultValue;
        }
        
        public static IEnumerable<XmlElement> GetChildElements(this XmlElement element)
        {
            return (element == null || element.IsEmpty || !element.HasChildNodes) ? new XmlElement[0] : element.ChildNodes.OfType<XmlElement>();
        }
        
        public static IEnumerable<XmlElement> GetChildElements(this IEnumerable<XmlElement> elements)
        {
            return (elements == null) ? new XmlElement[0] : elements.SelectMany(e => e.GetChildElements());
        }
        
        public static IEnumerable<XmlAttribute> GetChildAttributes(this XmlElement element)
        {
            return (element == null || !element.HasAttributes) ? new XmlAttribute[0] : element.Attributes.OfType<XmlAttribute>();
        }
        
        public static IEnumerable<XmlAttribute> GetChildAttributes(this IEnumerable<XmlElement> elements)
        {
            return (elements == null) ? new XmlAttribute[0] : elements.SelectMany(e => e.GetChildAttributes());
        }
        
        public static IEnumerable<XmlElement> GetChildElements<T>(this XmlElement element, T name)
            where T : struct, IComparable, IFormattable, IConvertible
        {
            string localName = Enum.GetName(name.GetType(), name);
            return element.GetChildElements().Where(e => e.LocalName == localName && e.NamespaceURI.Length == 0);
        }
        
        public static IEnumerable<XmlElement> GetChildElements(this IEnumerable<XmlElement> elements, string name)
        {
            return (elements == null || name == null) ? new XmlElement[0] : elements.GetChildElements().Where(e => e.LocalName == name && e.NamespaceURI.Length == 0);
        }
        
        public static IEnumerable<XmlElement> GetChildElements<T>(this IEnumerable<XmlElement> elements, T name)
            where T : struct, IComparable, IFormattable, IConvertible
        {
            string localName = Enum.GetName(name.GetType(), name);
            return elements.GetChildElements().Where(e => e.LocalName == localName && e.NamespaceURI.Length == 0);
        }
        
        public static IEnumerable<XmlElement> GetElementsByLocalName(this IEnumerable<XmlElement> elements, string name)
        {
            return (elements == null || name == null) ? new XmlElement[0] : elements.Where(e => e.LocalName == name && e.NamespaceURI.Length == 0);
        }
        
        public static IEnumerable<XmlElement> GetElementsByLocalName<T>(this IEnumerable<XmlElement> elements, T name)
            where T : struct, IComparable, IFormattable, IConvertible
        {
            string localName = Enum.GetName(name.GetType(), name);
            return (elements == null) ? new XmlElement[0] : elements.Where(e => e.LocalName == localName && e.NamespaceURI.Length == 0);
        }
        
        public static IEnumerable<XmlElement> WhereAttributeMatches(this IEnumerable<XmlElement> elements, string attributeName, bool ignoreCase, string value, params string[] otherValues)
        {
            if (elements == null || String.IsNullOrEmpty(attributeName))
                return new XmlElement[0];
            StringComparer comparer = (ignoreCase) ? StringComparer.InvariantCultureIgnoreCase : StringComparer.InvariantCulture;
            if (otherValues == null)
                otherValues = new string[0];
            else if (otherValues.Length > 0)
            {
                if (otherValues.Any(v => v == null))
                {
                    if (value != null)
                        otherValues = (new string[] { value }).Concat(otherValues.Where(v => v != null)).Distinct(comparer).ToArray();
                    else
                        otherValues = otherValues.Where(v => v != null).Distinct(comparer).ToArray();
                    value = null;
                }
                else if (value != null)
                    otherValues = otherValues.Where(v => !comparer.Equals(v, value)).Distinct(comparer).ToArray();
                else
                    otherValues = otherValues.Distinct(comparer).ToArray();
            }
            if (otherValues.Length == 0)
            {
                if (value == null)
                    return elements.Where(e => e.GetAttributeValue(attributeName) == null);
                return elements.Where(e => {
                    string s = e.GetAttributeValue(attributeName);
                    return s != null && comparer.Equals(value, s);
                });
            }
            if (value == null)
                return elements.Where(e => {
                    string s = e.GetAttributeValue(attributeName);
                    return s == null || otherValues.Any(o => comparer.Equals(o, s));
                });
            return elements.Where(e => {
                string s = e.GetAttributeValue(attributeName);
                return s != null && (comparer.Equals(value, s) ||  otherValues.Any(o => comparer.Equals(o, s)));
            });
        }
        
        public static IEnumerable<XmlElement> WhereAttributeMatches<T>(this IEnumerable<XmlElement> elements, T attributeName, bool ignoreCase, string value, params string[] otherValues)
            where T : struct, IComparable, IFormattable, IConvertible
        {
            string localName = Enum.GetName(attributeName.GetType(), attributeName);
            if (elements == null)
                return new XmlElement[0];
            StringComparer comparer = (ignoreCase) ? StringComparer.InvariantCultureIgnoreCase : StringComparer.InvariantCulture;
            if (otherValues == null)
                otherValues = new string[0];
            else if (otherValues.Length > 0)
            {
                if (otherValues.Any(v => v == null))
                {
                    if (value != null)
                        otherValues = (new string[] { value }).Concat(otherValues.Where(v => v != null)).Distinct(comparer).ToArray();
                    else
                        otherValues = otherValues.Where(v => v != null).Distinct(comparer).ToArray();
                    value = null;
                }
                else if (value != null)
                    otherValues = otherValues.Where(v => !comparer.Equals(v, value)).Distinct(comparer).ToArray();
                else
                    otherValues = otherValues.Distinct(comparer).ToArray();
            }
            if (otherValues.Length == 0)
            {
                if (value == null)
                    return elements.Where(e => e.GetAttributeValue(localName) == null);
                return elements.Where(e => {
                    string s = e.GetAttributeValue(localName);
                    return s != null && comparer.Equals(value, s);
                });
            }
            if (value == null)
                return elements.Where(e => {
                    string s = e.GetAttributeValue(localName);
                    return s == null || otherValues.Any(o => comparer.Equals(o, s));
                });
            return elements.Where(e => {
                string s = e.GetAttributeValue(localName);
                return s != null && (comparer.Equals(value, s) ||  otherValues.Any(o => comparer.Equals(o, s)));
            });
        }
        
        public static IEnumerable<XmlElement> WhereAttributeMatches<TName, TValue>(this IEnumerable<XmlElement> elements, TName attributeName, bool ignoreCase, bool includeNull,
                bool includeEmpty, TValue value, params TValue[] otherValues)
            where TName : struct, IComparable, IFormattable, IConvertible
            where TValue : struct, IComparable, IFormattable, IConvertible
        {
            Type t = value.GetType();
            string sValue = Enum.GetName(t, value);
            string[] otherS = (otherValues == null || otherValues.Length == 0) ? new string[0] : otherValues.Where(o => o.CompareTo(value) != 0).Select(o => Enum.GetName(t, o)).ToArray();
            if (includeNull)
            {
                if (includeEmpty)
                {
                    if (otherValues == null || otherValues.Length == 0)
                        return elements.WhereAttributeMatches<TName>(attributeName, ignoreCase, null, "", sValue);
                    return elements.WhereAttributeMatches<TName>(attributeName, ignoreCase, null, (new string[] { "", sValue }).Concat(otherValues.Where(o => o.CompareTo(value) != 0).Select(o => Enum.GetName(t, o))).ToArray());
                }
                if (otherValues == null || otherValues.Length == 0)
                    return elements.WhereAttributeMatches<TName>(attributeName, ignoreCase, null, sValue);
                return elements.WhereAttributeMatches<TName>(attributeName, ignoreCase, null, (new string[] { sValue }).Concat(otherValues.Where(o => o.CompareTo(value) != 0).Select(o => Enum.GetName(t, o))).ToArray());
            }
            if (includeEmpty)
            {
                if (otherValues == null || otherValues.Length == 0)
                    return elements.WhereAttributeMatches<TName>(attributeName, ignoreCase, sValue, "");
                return elements.WhereAttributeMatches<TName>(attributeName, ignoreCase, sValue, (new string[] { "" }).Concat(otherValues.Where(o => o.CompareTo(value) != 0).Select(o => Enum.GetName(t, o))).ToArray());
            }
            if (otherValues == null || otherValues.Length == 0)
                return elements.WhereAttributeMatches<TName>(attributeName, ignoreCase, sValue);
            return elements.WhereAttributeMatches<TName>(attributeName, ignoreCase, sValue, otherValues.Where(o => o.CompareTo(value) != 0).Select(o => Enum.GetName(t, o)).ToArray());
        }

        public static IEnumerable<XmlElement> WhereAttributeMatches(this IEnumerable<XmlElement> elements, string attributeName, string value, params string[] otherValues)
        {
            return elements.WhereAttributeMatches(attributeName, false, value, otherValues);
        }

        public static IEnumerable<XmlElement> WhereAttributeMatches<T>(this IEnumerable<XmlElement> elements, T attributeName, string value, params string[] otherValues)
            where T : struct, IComparable, IFormattable, IConvertible
        {
            return elements.WhereAttributeMatches<T>(attributeName, false, value, otherValues);
        }
        public static IEnumerable<XmlElement> WhereAttributeMatches<TName, TValue>(this IEnumerable<XmlElement> elements, TName attributeName, bool ignoreCase, bool includeNull,
                TValue value, params TValue[] otherValues)
            where TName : struct, IComparable, IFormattable, IConvertible
            where TValue : struct, IComparable, IFormattable, IConvertible
        {
            return elements.WhereAttributeMatches<TName, TValue>(attributeName, ignoreCase, includeNull, false, value, otherValues);
        }
        public static IEnumerable<XmlElement> WhereAttributeMatches<TName, TValue>(this IEnumerable<XmlElement> elements, TName attributeName, bool ignoreCase, TValue value,
                params TValue[] otherValues)
            where TName : struct, IComparable, IFormattable, IConvertible
            where TValue : struct, IComparable, IFormattable, IConvertible
        {
            return elements.WhereAttributeMatches<TName, TValue>(attributeName, ignoreCase, false, value, otherValues);
        }
        public static IEnumerable<XmlElement> WhereAttributeMatches<TName, TValue>(this IEnumerable<XmlElement> elements, TName attributeName, TValue value,
                params TValue[] otherValues)
            where TName : struct, IComparable, IFormattable, IConvertible
            where TValue : struct, IComparable, IFormattable, IConvertible
        {
            return elements.WhereAttributeMatches<TName, TValue>(attributeName, false, value, otherValues);
        }
    }
    public class PsMsBuildLogger : Logger
    {
        public const string RoundTrimDateTimeFormat = "yyyy-MM-ddTHH:mm:ss.fffffffzzzzzz";
        public enum XmlLocalNames
        {
            BuildResult,
            Project,
            Target,
            Task,
            Message,
            Started,
            Duration,
            CpuCount,
            ID,
            SubmissionId,
            Environment,
            Name,
            BuildRequest,
            Build,
            Succeeded,
            Property,
            Parent,
            File,
            ToolsVersion,
            Timestamp,
            Importance,
            Code,
            Subcategory,
            Line,
            EndLine,
            Column,
            EndColumn,
            Error,
            Warning,
            CustomEvent,
            SenderName
        }
        private XmlDocument _buildResultDocument = null;
        private object _syncRoot = new object();
        private IEventSource _eventSource = null;
        private int _cpuCount = -1;
        private string _outputPath = "";
        private bool _consoleEcho = false;
        private StringComparer _pathComparer = StringComparer.InvariantCultureIgnoreCase;
        private StopwatchDictionary _stopwatches = null;
        public interface IStopwatchItem
        {
            TimeSpan Elapsed { get; }
            bool IsRunning { get; }
            TimeSpan Stop();
            void Start();
        }
        public interface IStopwatchItem<T> : IStopwatchItem
            where T : struct, IComparable, IFormattable, IConvertible, IComparable<T>, IEquatable<T>
        {
            T ID { get; }
        }
        public abstract class StopwatchItemDictionary<TKey, TItem> : KeyedCollection<TKey, TItem>, IStopwatchItem
            where TKey : struct, IComparable, IFormattable, IConvertible, IComparable<TKey>, IEquatable<TKey>
            where TItem : class, IStopwatchItem<TKey>
        {
            private List<TItem> _currencyList = new List<TItem>();
            public TItem Current
            {
                get
                {
                    Monitor.Enter(_currencyList);
                    try
                    {
                        if (_currencyList.Count == 0)
                            return null;
                        return _currencyList[0];
                    }
                    finally { Monitor.Exit(_currencyList); }
                }
            }
            public TimeSpan Elapsed { get { return _stopwatch.Elapsed; } }
            public bool IsRunning { get { return _stopwatch.IsRunning; } }
            private Stopwatch _stopwatch = Stopwatch.StartNew();
            public Stopwatch GetStopwatch() { return _stopwatch; }
            public StopwatchItemDictionary() { }
            public TimeSpan Stop()
            {
                Monitor.Enter(_currencyList);
                try
                {
                    if (_stopwatch.IsRunning)
                        _stopwatch.Stop();
                    foreach (TItem item in base.Items)
                        item.Stop();
                    return _stopwatch.Elapsed;
                }
                finally { Monitor.Exit(_currencyList); }
            }
            public void Start()
            {
                Monitor.Enter(_currencyList);
                try
                {
                    if (!_stopwatch.IsRunning)
                        _stopwatch.Start();
                    foreach (TItem item in base.Items)
                        item.Start();
                }
                finally { Monitor.Exit(_currencyList); }
            }
            protected override TKey GetKeyForItem(TItem item)
            {
                if (item == null)
                    throw new ArgumentNullException("item");
                return item.ID;
            }
            protected override void ClearItems()
            {
                Monitor.Enter(_currencyList);
                try
                {
                    foreach (TItem item in base.Items)
                        item.Stop();
                    base.ClearItems();
                    _currencyList.Clear();
                }
                finally { Monitor.Exit(_currencyList); }
            }
            protected override void InsertItem(int index, TItem item)
            {
                if (item == null)
                    throw new ArgumentNullException("item");
                Monitor.Enter(_currencyList);
                try
                {
                    if (Contains(item.ID))
                        throw new ArgumentOutOfRangeException("Item key already exists");
                    base.InsertItem(index, item);
                    item.Start();
                    if (!_currencyList.Contains(item))
                        _currencyList.Add(item);
                }
                finally { Monitor.Exit(_currencyList); }
            }
            protected override void RemoveItem(int index)
            {
                Monitor.Enter(_currencyList);
                try
                {
                    TItem item = base.Items[index];
                    item.Stop();
                    base.RemoveItem(index);
                    _currencyList.Remove(item);
                }
                finally { Monitor.Exit(_currencyList); }
            }
            protected override void SetItem(int index, TItem item)
            {
                if (item == null)
                    throw new ArgumentNullException("item");
                Monitor.Enter(_currencyList);
                try
                {
                    TItem oldItem = base.Items[index];
                    if (Contains(item.ID) && !oldItem.ID.Equals(item.ID))
                        throw new ArgumentOutOfRangeException("Item key already exists");
                    base.SetItem(index, item);
                    if (ReferenceEquals(item, oldItem))
                        return;
                    oldItem.Stop();
                    item.Start();
                    if (!_currencyList.Contains(item))
                        _currencyList.Add(item);
                    _currencyList.Remove(oldItem);
                }
                finally { Monitor.Exit(_currencyList); }
            }
            public TimeSpan StopAndRemove(TKey key)
            {
                Monitor.Enter(_currencyList);
                try
                {
                    if (!Contains(key))
                        return TimeSpan.Zero;
                    TItem item = this[key];
                    TimeSpan result = item.Stop();
                    Remove(key);
                    return result;
                }
                finally { Monitor.Exit(_currencyList); }
            }
            protected abstract TItem CreateNew(TKey id);
            public TItem StartItem(TKey key, bool doNotSetCurrent = false)
            {
                TItem item;
                Monitor.Enter(_currencyList);
                try
                {
                    if (Contains(key))
                    {
                        item = this[key];
                        item.Start();
                        if (doNotSetCurrent)
                            return item;
                        _currencyList.Remove(item);
                        _currencyList.Insert(0, item);
                    }
                    else
                    {
                        item = CreateNew(key);
                        if (!doNotSetCurrent)
                            _currencyList.Insert(0, item);
                        item.Start();
                        this.Add(item);
                    }
                }
                finally { Monitor.Exit(_currencyList); }
                return item;
            }
            public TResult GetValue<TResult>(TKey key, Func<TItem, TResult> getValue, Func<TResult> defaultValue)
            {
                Monitor.Enter(_currencyList);
                try
                {
                    if (Contains(key))
                        return getValue(this[key]);
                }
                finally { Monitor.Exit(_currencyList); }
                return defaultValue();
            }
        }
        public abstract class KeyedStopwatchItem<TCurrentKey, TChildKey, TItem> : StopwatchItemDictionary<TChildKey, TItem>, IStopwatchItem<TCurrentKey>, IEquatable<KeyedStopwatchItem<TCurrentKey, TChildKey, TItem>>
            where TChildKey : struct, IComparable, IFormattable, IConvertible, IComparable<TChildKey>, IEquatable<TChildKey>
            where TCurrentKey : struct, IComparable, IFormattable, IConvertible, IComparable<TCurrentKey>, IEquatable<TCurrentKey>
            where TItem : class, IStopwatchItem<TChildKey>
        {
            public TCurrentKey ID { get; private set; }
            public KeyedStopwatchItem(TCurrentKey id) { ID = id; }
            public bool Equals(KeyedStopwatchItem<TCurrentKey, TChildKey, TItem> other) { return (other != null && ReferenceEquals(this, other)); }
            public override bool Equals(object obj) { return (obj != null && obj is KeyedStopwatchItem<TCurrentKey, TChildKey, TItem> && ReferenceEquals(this, obj)); }
            public abstract override int GetHashCode();
            public override string ToString() { return ID.ToString(); }
        }
        public class TaskStopwatch : IStopwatchItem<int>, IEquatable<TaskStopwatch>
        {
            public int ID { get; private set; }
            public TimeSpan Elapsed { get { return _stopwatch.Elapsed; } }
            public bool IsRunning { get { return _stopwatch.IsRunning; } }
            private Stopwatch _stopwatch = Stopwatch.StartNew();
            public TimeSpan Stop()
            {
                if (_stopwatch.IsRunning)
                    _stopwatch.Stop();
                return _stopwatch.Elapsed;
            }
            public void Start()
            {
                if (!_stopwatch.IsRunning)
                    _stopwatch.Start();
            }
            public bool Equals(TaskStopwatch other) { return (other != null && ReferenceEquals(this, other)); }
            public override bool Equals(object obj) { return (obj != null && obj is TaskStopwatch && ReferenceEquals(this, obj)); }
            public override int GetHashCode() { return ID; }
            public override string ToString() { return ID.ToString(); }
            public TaskStopwatch(int id) { ID = id;}
        }
        public class TargetStopwatch : KeyedStopwatchItem<int, int, TaskStopwatch>
        {
            public TaskStopwatch CurrentTask { get { return Current; } }
            public TargetStopwatch(int id) : base(id) { }
            protected override TaskStopwatch CreateNew(int id) { return new TaskStopwatch(id); }
            public TaskStopwatch StartNewTask(BuildEventContext context, bool doNotSetCurrent = false) { return StartItem(context.TaskId, doNotSetCurrent); }
            public TimeSpan StopTask(BuildEventContext context) { return StopAndRemove(context.TaskId); }

            public override int GetHashCode() { return ID; }
        }
        public class ProjectStopwatch : KeyedStopwatchItem<int, int, TargetStopwatch>
        {
            public TargetStopwatch CurrentTarget { get { return Current; } }
            public TaskStopwatch CurrentTask
            {
                get
                {
                    TargetStopwatch current = Current;
                    if (current == null)
                        return null;
                    return current.CurrentTask;
                }
            }
            public ProjectStopwatch(int id) : base(id) { }
            protected override TargetStopwatch CreateNew(int id) { return new TargetStopwatch(id); }
            public TargetStopwatch StartNewTarget(BuildEventContext context, bool doNotSetCurrent = false) { return StartItem(context.TargetId, doNotSetCurrent); }
            public TaskStopwatch StartNewTask(BuildEventContext context, bool doNotSetCurrent = false)
            {
                return StartNewTarget(context, doNotSetCurrent).StartItem(context.TaskId, doNotSetCurrent);
            }
            public TimeSpan StopTarget(BuildEventContext context) { return StopAndRemove(context.TargetId); }
            public TimeSpan StopTask(BuildEventContext context) { return GetValue(context.TargetId, i => i.StopAndRemove(context.TaskId), () => TimeSpan.Zero); }

            public override int GetHashCode() { return ID; }
        }
        public class BuildStopwatch : KeyedStopwatchItem<long, int, ProjectStopwatch>
        {
            public ProjectStopwatch CurrentProject { get { return Current; } }
            public TargetStopwatch CurrentTarget
            {
                get
                {
                    ProjectStopwatch current = Current;
                    if (current == null)
                        return null;
                    return current.CurrentTarget;
                }
            }
            public TaskStopwatch CurrentTask
            {
                get
                {
                    ProjectStopwatch current = Current;
                    if (current == null)
                        return null;
                    return current.CurrentTask;
                }
            }
            public BuildStopwatch(long id) : base(id) { }
            protected override ProjectStopwatch CreateNew(int id) { return new ProjectStopwatch(id); }
            public ProjectStopwatch StartNewProject(BuildEventContext context, bool doNotSetCurrent = false) { return StartItem(context.ProjectContextId, doNotSetCurrent); }
            public TargetStopwatch StartNewTarget(BuildEventContext context, bool doNotSetCurrent = false)
            {
                return StartNewProject(context, doNotSetCurrent).StartNewTarget(context, doNotSetCurrent);
            }
            public TaskStopwatch StartNewTask(BuildEventContext context, bool doNotSetCurrent = false)
            {
                return StartNewProject(context, doNotSetCurrent).StartNewTask(context, doNotSetCurrent);
            }
            public TimeSpan StopProject(BuildEventContext context) { return StopAndRemove(context.ProjectContextId); }
            public TimeSpan StopTarget(BuildEventContext context) { return GetValue(context.ProjectContextId, i => i.StopAndRemove(context.TargetId), () => TimeSpan.Zero); }
            public TimeSpan StopTask(BuildEventContext context) { return GetValue(context.ProjectContextId, i => i.StopTask(context), () => TimeSpan.Zero); }

            public override int GetHashCode() { return ID.GetHashCode(); }
        }
        public class StopwatchDictionary : StopwatchItemDictionary<long, BuildStopwatch>
        {
            public BuildStopwatch CurrentBuild { get { return Current; } }
            public ProjectStopwatch CurrentProject
            {
                get
                {
                    BuildStopwatch current = Current;
                    if (current == null)
                        return null;
                    return current.CurrentProject;
                }
            }
            public TargetStopwatch CurrentTarget
            {
                get
                {
                    BuildStopwatch current = Current;
                    if (current == null)
                        return null;
                    return current.CurrentTarget;
                }
            }
            public TaskStopwatch CurrentTask
            {
                get
                {
                    BuildStopwatch current = Current;
                    if (current == null)
                        return null;
                    return current.CurrentTask;
                }
            }
            protected override BuildStopwatch CreateNew(long id) { return new BuildStopwatch(id); }
            public BuildStopwatch StartNewBuild(BuildEventContext context, bool doNotSetCurrent = false) { return StartItem(context.ProjectContextId, doNotSetCurrent); }
            public ProjectStopwatch StartNewProject(BuildEventContext context, bool doNotSetCurrent = false)
            {
                return StartNewBuild(context, doNotSetCurrent).StartNewProject(context, doNotSetCurrent);
            }
            public TargetStopwatch StartNewTarget(BuildEventContext context, bool doNotSetCurrent = false)
            {
                return StartNewBuild(context, doNotSetCurrent).StartNewTarget(context, doNotSetCurrent);
            }
            public TaskStopwatch StartNewTask(BuildEventContext context, bool doNotSetCurrent = false)
            {
                return StartNewBuild(context, doNotSetCurrent).StartNewTask(context, doNotSetCurrent);
            }
            public TimeSpan StopBuild(BuildEventContext context) { return StopAndRemove(context.BuildRequestId); }
            public TimeSpan StopProject(BuildEventContext context) { return GetValue(context.ProjectContextId, i => i.StopAndRemove(context.ProjectContextId), () => TimeSpan.Zero); }
            public TimeSpan StopTarget(BuildEventContext context) { return GetValue(context.ProjectContextId, i => i.StopTarget(context), () => TimeSpan.Zero); }
            public TimeSpan StopTask(BuildEventContext context) { return GetValue(context.ProjectContextId, i => i.StopTask(context), () => TimeSpan.Zero); }
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
                _buildResultDocument = new XmlDocument();
                _buildResultDocument.AppendChild(_buildResultDocument.CreateElement(XmlLocalNames.BuildResult));
                _buildResultDocument.DocumentElement.ApplyAttribute(XmlLocalNames.Started, XmlConvert.ToString(DateTime.Now, RoundTrimDateTimeFormat));
                if (nodeCount > 0)
                    _buildResultDocument.DocumentElement.ApplyAttribute(XmlLocalNames.CpuCount, XmlConvert.ToString(nodeCount));

                foreach (var g in ((Parameters == null) ? "" : Parameters.Trim()).Split(Path.PathSeparator).Select(p =>
                {
                    string[] kvp = p.Split(new char[] { '=' }, 2);
                    if (kvp.Length == 1)
                        return new { Key = "Path", Value = kvp[0].Trim() };
                    return new { Key = kvp[0].Trim(), Value = kvp[1].Trim() };
                }).GroupBy(kvp => kvp.Key, _pathComparer))
                {
                    if (g.Count() > 0)
                        throw new Exception("Parameter '" + g.Key + "' cannot be defined more than once.");
                    if (_pathComparer.Equals("Path", g.Key))
                        _outputPath = g.First().Value;
                    else if (_pathComparer.Equals("Echo", g.Key))
                    {
                        string v = g.First().Value;
                        Match match = Regex.Match(v, @"^((?<false>f(alse)?|no?|-|[+\-]?0+(\.0+)?))|(?<true>)t(rue)?|y(es)?|\+|[+\-]?(0*[1-9]\d*(\.\d+)?|0+\.0*[1-9]\d*))$", RegexOptions.IgnoreCase);
                        if (!match.Success)
                            throw new Exception("'" + v + "' is not recognized yes/no value for the '" + g.Key + "' parameter.");
                        _consoleEcho = match.Groups["true"].Success;
                    }
                    else
                        throw new Exception("'" + g.Key + "' is not recognized parameter name.");
                }
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
                    TimeSpan duration = _stopwatches.Stop();
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
                    _buildResultDocument.DocumentElement.ApplyAttribute(XmlLocalNames.Duration, XmlConvert.ToString(duration));
                    if (!String.IsNullOrWhiteSpace(_outputPath))
                    {
                        using (XmlWriter writer = XmlWriter.Create(_outputPath, new XmlWriterSettings
                        {
                            Indent = true
                        }))
                        {
                            _buildResultDocument.WriteTo(writer);
                            writer.Flush();
                        }
                    }
                }
            }
            finally { Monitor.Exit(_syncRoot); }
        }
        private List<XmlElement> _contextElements = new List<XmlElement>();
        private XmlElement GetBuildElement(long buildRequestId, bool createNew, out bool isNew)
        {
            string id = XmlConvert.ToString(buildRequestId);
            isNew = createNew;
            if (!isNew)
            {
                XmlElement result = _contextElements.GetElementsByLocalName(XmlLocalNames.Build).WhereAttributeMatches(XmlLocalNames.ID, id).FirstOrDefault();
                if (result != null)
                    return result;
                result = _buildResultDocument.DocumentElement.GetChildElements(XmlLocalNames.Build).WhereAttributeMatches(XmlLocalNames.ID, id).LastOrDefault();
                if (result != null)
                    return result;
                isNew = true;
            }
            return _buildResultDocument.DocumentElement.AppendElement(XmlLocalNames.Build).ApplyAttribute(XmlLocalNames.ID, id);
        }
        private XmlElement GetBuildElement(long buildRequestId, bool createNew = false)
        {
            bool isNew;
            return GetBuildElement(buildRequestId, createNew, out isNew);
        }
        private XmlElement GetBuildElement(long buildRequestId, out bool isNew)
        {
            return GetBuildElement(buildRequestId, false, out isNew);
        }
        private XmlElement GetProjectElement(BuildEventContext buildEventContext, bool createNew, out bool isNew)
        {
            if (buildEventContext.ProjectContextId == BuildEventContext.InvalidProjectContextId)
            {
                isNew = false;
                return GetBuildElement(buildEventContext.BuildRequestId, false);
            }
            string id = XmlConvert.ToString(buildEventContext.ProjectContextId);
            isNew = createNew;
            if (!isNew)
            {
                XmlElement result = _contextElements.GetElementsByLocalName(XmlLocalNames.Project).WhereAttributeMatches(XmlLocalNames.ID, id).FirstOrDefault();
                if (result != null)
                    return result;
                result = GetBuildElement(buildEventContext.BuildRequestId).GetChildElements(XmlLocalNames.Project).WhereAttributeMatches(XmlLocalNames.ID, id).LastOrDefault();
                if (result != null)
                    return result;
                isNew = true;
            }
            return GetBuildElement(buildEventContext.BuildRequestId).AppendElement(XmlLocalNames.Project).ApplyAttribute(XmlLocalNames.ID, id);
        }
        private XmlElement GetProjectElement(BuildEventContext buildEventContext, bool createNew = false)
        {
            bool isNew;
            return GetProjectElement(buildEventContext, createNew, out isNew);
        }
        private XmlElement GetProjectElement(BuildEventContext buildEventContext, out bool isNew)
        {
            return GetProjectElement(buildEventContext, false, out isNew);
        }
        private XmlElement GetTargetElement(BuildEventContext buildEventContext, bool createNew, out bool isNew)
        {
            if (buildEventContext.TargetId == BuildEventContext.InvalidTargetId)
            {
                isNew = false;
                return GetProjectElement(buildEventContext, false);
            }
            string id = XmlConvert.ToString(buildEventContext.TargetId);
            isNew = createNew;
            if (!isNew)
            {
                XmlElement result = _contextElements.GetElementsByLocalName(XmlLocalNames.Target).WhereAttributeMatches(XmlLocalNames.ID, id).FirstOrDefault();
                if (result != null)
                    return result;
                result = GetProjectElement(buildEventContext).GetChildElements(XmlLocalNames.Target).WhereAttributeMatches(XmlLocalNames.ID, id).LastOrDefault();
                if (result != null)
                    return result;
                isNew = true;
            }
            return GetProjectElement(buildEventContext).AppendElement(XmlLocalNames.Target).ApplyAttribute(XmlLocalNames.ID, id);
        }
        private XmlElement GetTargetElement(BuildEventContext buildEventContext, bool createNew = false)
        {
            bool isNew;
            return GetTargetElement(buildEventContext, createNew, out isNew);
        }
        private XmlElement GetTargetElement(BuildEventContext buildEventContext, out bool isNew)
        {
            return GetTargetElement(buildEventContext, false, out isNew);
        }
        private XmlElement GetTaskElement(BuildEventContext buildEventContext, bool createNew, out bool isNew)
        {
            if (buildEventContext.TaskId == BuildEventContext.InvalidTaskId)
            {
                isNew = false;
                return GetTargetElement(buildEventContext, false);
            }
            string id = XmlConvert.ToString(buildEventContext.TaskId);
            isNew = createNew;
            if (!isNew)
            {
                XmlElement result = _contextElements.GetElementsByLocalName(XmlLocalNames.Task).WhereAttributeMatches(XmlLocalNames.ID, id).FirstOrDefault();
                if (result != null)
                    return result;
                result = GetTargetElement(buildEventContext).GetChildElements(XmlLocalNames.Task).WhereAttributeMatches(XmlLocalNames.ID, id).LastOrDefault();
                if (result != null)
                    return result;
                isNew = true;
            }
            return GetTargetElement(buildEventContext).AppendElement(XmlLocalNames.Task).ApplyAttribute(XmlLocalNames.ID, id);
        }
        private XmlElement GetTaskElement(BuildEventContext buildEventContext, out bool isNew)
        {
            return GetTaskElement(buildEventContext, false, out isNew);
        }
        private XmlElement GetTaskElement(BuildEventContext buildEventContext, bool createNew = false)
        {
            bool isNew;
            return GetTaskElement(buildEventContext, createNew, out isNew);
        }
        private void OnBuildStarted(object sender, BuildStartedEventArgs e)
        {
            Monitor.Enter(_syncRoot);
            try
            {
                _stopwatches.StartNewBuild(e.BuildEventContext);
                XmlElement element = GetBuildElement(e.BuildEventContext.BuildRequestId, true);
                _contextElements.Insert(0, element);
                element.ApplyAttribute(XmlLocalNames.ID, XmlConvert.ToString(e.BuildEventContext.BuildRequestId))
                    .ApplyAttribute(XmlLocalNames.SubmissionId, XmlConvert.ToString(e.BuildEventContext.SubmissionId))
                    .ApplyAttribute(XmlLocalNames.Started, XmlConvert.ToString(e.Timestamp, RoundTrimDateTimeFormat));
                if (!String.IsNullOrEmpty(e.Message))
                    element.AppendElement(XmlLocalNames.Message).SetInnerText(e.Message);
                if (e.BuildEnvironment != null && e.BuildEnvironment.Count > 0)
                {
                    element = element.AppendElement(XmlLocalNames.Environment);
                    foreach (string name in e.BuildEnvironment.Keys)
                    {
                        XmlElement env = element.AppendElement(XmlLocalNames.Property);
                        env.ApplyAttribute(XmlLocalNames.Name, name).SetInnerText(e.BuildEnvironment[name]);
                    }
                }
            }
            finally { Monitor.Exit(_syncRoot); }
        }

        private void OnBuildFinished(object sender, BuildFinishedEventArgs e)
        {
            Monitor.Enter(_syncRoot);
            try
            {
                TimeSpan duration = _stopwatches.StopBuild(e.BuildEventContext);
                XmlElement element = GetBuildElement(e.BuildEventContext.BuildRequestId);
                if (element == null)
                    return;
                _contextElements.Remove(element);
                element.ApplyAttribute(XmlLocalNames.Succeeded, XmlConvert.ToString(e.Succeeded))
                    .ApplyAttribute(XmlLocalNames.Duration, XmlConvert.ToString(duration));
                if (!String.IsNullOrEmpty(e.Message))
                    element.AppendElement(XmlLocalNames.Message).SetInnerText(e.Message);
            }
            finally { Monitor.Exit(_syncRoot); }
        }
        private void OnProjectStarted(object sender, ProjectStartedEventArgs e)
        {
            Monitor.Enter(_syncRoot);
            try
            {
                _stopwatches.StartNewProject(e.BuildEventContext);
                bool isNew;
                XmlElement element = GetProjectElement(e.BuildEventContext, true, out isNew);
                if (isNew)
                {
                    _contextElements.Insert(0, element);
                    element.ApplyAttribute(XmlLocalNames.ID, XmlConvert.ToString(e.BuildEventContext.ProjectContextId))
                        .ApplyAttribute(XmlLocalNames.File, e.ProjectFile).ApplyAttribute(XmlLocalNames.ToolsVersion, e.ToolsVersion);
                    if (e.ParentProjectBuildEventContext != null && e.ParentProjectBuildEventContext.ProjectContextId != BuildEventContext.InvalidProjectContextId)
                        element.ApplyAttribute(XmlLocalNames.Parent, XmlConvert.ToString(e.ParentProjectBuildEventContext.ProjectContextId));
                    element.ApplyAttribute(XmlLocalNames.Started, XmlConvert.ToString(e.Timestamp, RoundTrimDateTimeFormat));
                    if (!String.IsNullOrEmpty(e.Message))
                        element.AppendElement(XmlLocalNames.Message).SetInnerText(e.Message);
                }
                else
                    element.AppendElement(XmlLocalNames.Project).ApplyAttribute(XmlLocalNames.File, e.ProjectFile).ApplyAttribute(XmlLocalNames.Timestamp, XmlConvert.ToString(e.Timestamp, RoundTrimDateTimeFormat))
                        .SetInnerText(e.Message);
            }
            finally { Monitor.Exit(_syncRoot); }
        }
        private void OnProjectFinished(object sender, ProjectFinishedEventArgs e)
        {
            Monitor.Enter(_syncRoot);
            try
            {
                TimeSpan duration = _stopwatches.StopProject(e.BuildEventContext);
                XmlElement element = GetProjectElement(e.BuildEventContext);
                if (element == null)
                    return;
                _contextElements.Remove(element);
                element.ApplyAttribute(XmlLocalNames.Succeeded, XmlConvert.ToString(e.Succeeded))
                    .ApplyAttribute(XmlLocalNames.Duration, XmlConvert.ToString(duration));
                if (!String.IsNullOrEmpty(e.Message))
                    element.AppendElement(XmlLocalNames.Message).SetInnerText(e.Message);
            }
            finally { Monitor.Exit(_syncRoot); }
        }
        private void OnTargetStarted(object sender, TargetStartedEventArgs e)
        {
            Monitor.Enter(_syncRoot);
            try
            {
                _stopwatches.StartNewTarget(e.BuildEventContext);
                bool isNew;
                XmlElement element = GetTargetElement(e.BuildEventContext, true, out isNew);
                if (isNew)
                {
                    _contextElements.Insert(0, element);
                    element.ApplyAttribute(XmlLocalNames.ID, XmlConvert.ToString(e.BuildEventContext.TargetId))
                        .ApplyAttribute(XmlLocalNames.Name, e.TargetName).ApplyAttribute(XmlLocalNames.File, e.TargetFile)
                        .ApplyAttribute(XmlLocalNames.Parent, e.ParentTarget).ApplyAttribute(XmlLocalNames.Started, XmlConvert.ToString(e.Timestamp, RoundTrimDateTimeFormat));
                    if (!String.IsNullOrEmpty(e.ProjectFile) && !_pathComparer.Equals(GetProjectElement(e.BuildEventContext).GetAttributeValue(XmlLocalNames.Project, ""), e.ProjectFile))
                        element.ApplyAttribute(XmlLocalNames.Project, e.ProjectFile);
                    if (!String.IsNullOrEmpty(e.Message))
                        element.AppendElement(XmlLocalNames.Message).SetInnerText(e.Message);
                }
                else
                    element.AppendElement(XmlLocalNames.Target).ApplyAttribute(XmlLocalNames.Timestamp, XmlConvert.ToString(e.Timestamp, RoundTrimDateTimeFormat))
                        .SetInnerText(e.Message);
            }
            finally { Monitor.Exit(_syncRoot); }
        }
        private void OnTargetFinished(object sender, TargetFinishedEventArgs e)
        {
            Monitor.Enter(_syncRoot);
            try
            {
                TimeSpan duration = _stopwatches.StopTarget(e.BuildEventContext);
                XmlElement element = GetTargetElement(e.BuildEventContext);
                if (element == null)
                    return;
                _contextElements.Remove(element);
                element.ApplyAttribute(XmlLocalNames.Succeeded, XmlConvert.ToString(e.Succeeded))
                    .ApplyAttribute(XmlLocalNames.Duration, XmlConvert.ToString(duration));
                if (!String.IsNullOrEmpty(e.Message))
                    element.AppendElement(XmlLocalNames.Message).SetInnerText(e.Message);
            }
            finally { Monitor.Exit(_syncRoot); }
        }
        private void OnTaskStarted(object sender, TaskStartedEventArgs e)
        {
            Monitor.Enter(_syncRoot);
            try
            {
                _stopwatches.StartNewTask(e.BuildEventContext);
                bool isNew;
                XmlElement element = GetTaskElement(e.BuildEventContext, true, out isNew);
                if (isNew)
                {
                    _contextElements.Insert(0, element);
                    element.ApplyAttribute(XmlLocalNames.ID, XmlConvert.ToString(e.BuildEventContext.TargetId))
                        .ApplyAttribute(XmlLocalNames.Name, e.TaskName).ApplyAttribute(XmlLocalNames.File, e.TaskFile)
                        .ApplyAttribute(XmlLocalNames.Started, XmlConvert.ToString(e.Timestamp, RoundTrimDateTimeFormat));
                    if (!String.IsNullOrEmpty(e.ProjectFile) && !_pathComparer.Equals(GetProjectElement(e.BuildEventContext).GetAttributeValue(XmlLocalNames.Project, ""), e.ProjectFile))
                        element.ApplyAttribute(XmlLocalNames.Project, e.ProjectFile);
                    if (!String.IsNullOrEmpty(e.Message))
                        element.AppendElement(XmlLocalNames.Message).SetInnerText(e.Message);
                }
                else
                    element.AppendElement(XmlLocalNames.Task).ApplyAttribute(XmlLocalNames.Timestamp, XmlConvert.ToString(e.Timestamp, RoundTrimDateTimeFormat))
                        .SetInnerText(e.Message);
            }
            finally { Monitor.Exit(_syncRoot); }
        }
        private void OnTaskFinished(object sender, TaskFinishedEventArgs e)
        {
            Monitor.Enter(_syncRoot);
            try
            {
                TimeSpan duration = _stopwatches.StopTask(e.BuildEventContext);
                XmlElement element = GetTaskElement(e.BuildEventContext);
                if (element == null)
                    return;
                _contextElements.Remove(element);
                element.ApplyAttribute(XmlLocalNames.Succeeded, XmlConvert.ToString(e.Succeeded))
                    .ApplyAttribute(XmlLocalNames.Duration, XmlConvert.ToString(duration));
                if (!String.IsNullOrEmpty(e.Message))
                    element.AppendElement(XmlLocalNames.Message).SetInnerText(e.Message);
            }
            finally { Monitor.Exit(_syncRoot); }
        }
        private void OnMessageRaised(object sender, BuildMessageEventArgs e)
        {
            Monitor.Enter(_syncRoot);
            try
            {
                XmlElement element = GetTaskElement(e.BuildEventContext).AppendElement(XmlLocalNames.Message)
                    .ApplyAttribute(XmlLocalNames.Importance, e.Importance).ApplyAttribute(XmlLocalNames.Code, e.Code).ApplyAttribute(XmlLocalNames.Subcategory, e.Subcategory)
                    .ApplyAttribute(XmlLocalNames.Timestamp, XmlConvert.ToString(e.Timestamp, RoundTrimDateTimeFormat))
                    .SetInnerText(e.Message);
                if (!String.IsNullOrEmpty(e.ProjectFile) && !_pathComparer.Equals(GetProjectElement(e.BuildEventContext).GetAttributeValue(XmlLocalNames.Project, ""), e.ProjectFile))
                    element.ApplyAttribute(XmlLocalNames.Project, e.ProjectFile);
                if (!String.IsNullOrEmpty(e.File) || e.LineNumber > 0)
                {
                    element = element.AppendElement(XmlLocalNames.File).SetInnerText(e.File);
                    if (e.LineNumber > 0)
                    {
                        element.ApplyAttribute(XmlLocalNames.Line, XmlConvert.ToString(e.LineNumber));
                        if (e.EndLineNumber > e.LineNumber)
                            element.ApplyAttribute(XmlLocalNames.EndLine, XmlConvert.ToString(e.EndLineNumber));
                    }
                    if (e.ColumnNumber > 0)
                    {
                        element.ApplyAttribute(XmlLocalNames.Column, XmlConvert.ToString(e.ColumnNumber));
                        if (e.EndColumnNumber > e.ColumnNumber)
                            element.ApplyAttribute(XmlLocalNames.EndColumn, XmlConvert.ToString(e.EndColumnNumber));
                    }
                }
            }
            finally { Monitor.Exit(_syncRoot); }
        }
        private void OnErrorRaised(object sender, BuildErrorEventArgs e)
        {
            Monitor.Enter(_syncRoot);
            try
            {
                XmlElement element = GetTaskElement(e.BuildEventContext).AppendElement(XmlLocalNames.Error)
                    .ApplyAttribute(XmlLocalNames.Code, e.Code).ApplyAttribute(XmlLocalNames.Subcategory, e.Subcategory)
                    .ApplyAttribute(XmlLocalNames.Timestamp, XmlConvert.ToString(e.Timestamp, RoundTrimDateTimeFormat))
                    .SetInnerText(e.Message);
                if (!String.IsNullOrEmpty(e.ProjectFile) && !_pathComparer.Equals(GetProjectElement(e.BuildEventContext).GetAttributeValue(XmlLocalNames.Project, ""), e.ProjectFile))
                    element.ApplyAttribute(XmlLocalNames.Project, e.ProjectFile);
                if (!String.IsNullOrEmpty(e.File) || e.LineNumber > 0)
                {
                    element = element.AppendElement(XmlLocalNames.File).SetInnerText(e.File);
                    if (e.LineNumber > 0)
                    {
                        element.ApplyAttribute(XmlLocalNames.Line, XmlConvert.ToString(e.LineNumber));
                        if (e.EndLineNumber > e.LineNumber)
                            element.ApplyAttribute(XmlLocalNames.EndLine, XmlConvert.ToString(e.EndLineNumber));
                    }
                    if (e.ColumnNumber > 0)
                    {
                        element.ApplyAttribute(XmlLocalNames.Column, XmlConvert.ToString(e.ColumnNumber));
                        if (e.EndColumnNumber > e.ColumnNumber)
                            element.ApplyAttribute(XmlLocalNames.EndColumn, XmlConvert.ToString(e.EndColumnNumber));
                    }
                }
            }
            finally { Monitor.Exit(_syncRoot); }
        }
        private void OnWarningRaised(object sender, BuildWarningEventArgs e)
        {
            Monitor.Enter(_syncRoot);
            try
            {
                XmlElement element = GetTaskElement(e.BuildEventContext).AppendElement(XmlLocalNames.Warning)
                    .ApplyAttribute(XmlLocalNames.Code, e.Code).ApplyAttribute(XmlLocalNames.Subcategory, e.Subcategory)
                    .ApplyAttribute(XmlLocalNames.Timestamp, XmlConvert.ToString(e.Timestamp, RoundTrimDateTimeFormat))
                    .SetInnerText(e.Message);
                if (!String.IsNullOrEmpty(e.ProjectFile) && !_pathComparer.Equals(GetProjectElement(e.BuildEventContext).GetAttributeValue(XmlLocalNames.Project, ""), e.ProjectFile))
                    element.ApplyAttribute(XmlLocalNames.Project, e.ProjectFile);
                if (!String.IsNullOrEmpty(e.File) || e.LineNumber > 0)
                {
                    element = element.AppendElement(XmlLocalNames.File).SetInnerText(e.File);
                    if (e.LineNumber > 0)
                    {
                        element.ApplyAttribute(XmlLocalNames.Line, XmlConvert.ToString(e.LineNumber));
                        if (e.EndLineNumber > e.LineNumber)
                            element.ApplyAttribute(XmlLocalNames.EndLine, XmlConvert.ToString(e.EndLineNumber));
                    }
                    if (e.ColumnNumber > 0)
                    {
                        element.ApplyAttribute(XmlLocalNames.Column, XmlConvert.ToString(e.ColumnNumber));
                        if (e.EndColumnNumber > e.ColumnNumber)
                            element.ApplyAttribute(XmlLocalNames.EndColumn, XmlConvert.ToString(e.EndColumnNumber));
                    }
                }
            }
            finally { Monitor.Exit(_syncRoot); }
        }
        private void OnCustomEventRaised(object sender, CustomBuildEventArgs e)
        {
            Monitor.Enter(_syncRoot);
            try
            {
                XmlElement element = GetTaskElement(e.BuildEventContext).AppendElement(XmlLocalNames.CustomEvent)
                    .ApplyAttribute(XmlLocalNames.SenderName, e.SenderName)
                    .ApplyAttribute(XmlLocalNames.Timestamp, XmlConvert.ToString(e.Timestamp, RoundTrimDateTimeFormat))
                    .SetInnerText(e.Message);
            }
            finally { Monitor.Exit(_syncRoot); }
        }
    }

    public class MSBuildLogHelper : IList<PSObject>, INodeLogger, IList 
    {
        private Task<bool> _buildTask = null;
        private Project _currentProject = null;
        private string[] _targets = null;
        
        private CancellationTokenSource _tokenSource = null;
        
        private CancellationToken _cancellationToken = new CancellationToken(true);
        
        public bool IsFaulted
        {
            get
            {
                Monitor.Enter(_syncRoot);
                try { return _buildTask != null && _buildTask.IsCompleted && _buildTask.IsFaulted; }
                finally { Monitor.Exit(_syncRoot); }
            }
        }

        public void StartBuild(Project project) { StartBuild(project, new string[0]); }

        public void StartBuild(Project project, params string[] targets)
        {
            if (project == null)
                throw new ArgumentNullException("project");
            Task<bool> buildTask;
            Monitor.Enter(_syncRoot);
            try
            {
                if (!(_buildTask == null || _buildTask.IsCompleted))
                    throw new InvalidOperationException("A build task is already in progress");
                _currentProject = project;
                _tokenSource = new CancellationTokenSource();
                _cancellationToken = _tokenSource.Token;
                buildTask = Task<bool>.Factory.StartNew(BuildAsync);
                _buildTask = buildTask;
            }
            finally { Monitor.Exit(_syncRoot); }
            buildTask.ContinueWith(t =>
            {
                Monitor.Enter(_syncRoot);
                try
                {
                    if (_tokenSource != null && ReferenceEquals(_buildTask, t))
                    {
                        _cancellationToken = new CancellationToken(true);
                        _tokenSource.Dispose();
                        _tokenSource = null;
                    }
                }
                finally { Monitor.Exit(_syncRoot); }
            });
        }
        
        public void StartBuild(Project project, IEnumerable<string> targets) { StartBuild(project, (targets == null) ? null : targets.ToArray()); }
        
        public bool EndBuild()
        {
            Task<bool> task = _buildTask;
            if (task == null)
                return false;

            if (!task.IsCompleted)
                task.Wait();
            return task.Result;
        }

        public bool WaitBuild(int milliseconds)
        {
            Task<bool> task = _buildTask;
            if (task == null || task.IsCompleted)
                return true;

            return task.Wait(milliseconds);
        }

        public bool WaitBuild(TimeSpan duration)
        {
            Task<bool> task = _buildTask;
            if (task == null || task.IsCompleted)
                return true;

            return task.Wait(duration);
        }

        private bool BuildAsync()
        {
            Project currentProject;
            string[] targets;
            Monitor.Enter(_syncRoot);
            try
            {
                currentProject = _currentProject;
                targets = _targets;
            }
            finally { Monitor.Exit(_syncRoot); }

            if (currentProject == null)
                return false;

            try
            {
                if (targets == null || (targets = targets.Where(t => !String.IsNullOrEmpty(t)).ToArray()).Length == 0)
                    return currentProject.Build(this);

                if (targets.Length == 1)
                    return currentProject.Build(targets[0], new ILogger[] { this });
                
                return currentProject.Build(targets, new ILogger[] { this });
            }
            catch (Exception exc) { Add(exc); }

            return false;
        }
        
        public bool IsBuilding
        {
            get
            {
                Monitor.Enter(_syncRoot);
                try { return !(_buildTask == null || _buildTask.IsCompleted); }
                finally { Monitor.Exit(_syncRoot); }
            }
        }

        public bool IsActive
        {
            get
            {
                Monitor.Enter(_syncRoot);
                try { return _eventSource != null; }
                finally { Monitor.Exit(_syncRoot); }
            }
        }

        public Collection<PSObject> Read(int maxCount, bool doNotClear)
        {
            if (!doNotClear)
                return Read(maxCount);
            if (maxCount < 0)
                return Read(true);
            Collection<PSObject> result = new Collection<PSObject>();
            if (maxCount == 0)
                return result;
            Monitor.Enter(_syncRoot);
            try
            {
                foreach (PSObject obj in _innerList.Take(maxCount))
                    result.Add(obj);
            }
            finally { Monitor.Exit(_syncRoot); }
            return result;
        }
        
        public Collection<PSObject> Read(int maxCount)
        {
            if (maxCount < 0)
                return Read();
            Collection<PSObject> result = new Collection<PSObject>();
            if (maxCount == 0)
                return result;
            Monitor.Enter(_syncRoot);
            try
            {
                while (result.Count < maxCount && _innerList.Count > 0)
                    result.Add(_innerList.Dequeue());
            }
            finally { Monitor.Exit(_syncRoot); }
            return result;
        }
        
        public Collection<PSObject> Read(bool doNotClear)
        {
            if (!doNotClear)
                return Read();
            Collection<PSObject> result = new Collection<PSObject>();
            Monitor.Enter(_syncRoot);
            try
            {
                if (_innerList.Count > 0)
                {
                    foreach (PSObject obj in _innerList)
                        result.Add(obj);
                }
            }
            finally { Monitor.Exit(_syncRoot); }
            return result;
        }

        public Collection<PSObject> Read()
        {
            Collection<PSObject> result = new Collection<PSObject>();
            Monitor.Enter(_syncRoot);
            try
            {
                if (_innerList.Count > 0)
                {
                    foreach (PSObject obj in _innerList)
                        result.Add(obj);
                    _innerList.Clear();
                }
            }
            finally { Monitor.Exit(_syncRoot); }
            return result;
        }

        private object _syncRoot = new object();
        private LoggerVerbosity _verbosity;
        private string _parameters;
        private IEventSource _eventSource = null;
        private int _cpuCount;
        private Queue<PSObject> _innerList = new Queue<PSObject>();

        # region ILogger Property Implementation

        public LoggerVerbosity Verbosity { get { return _verbosity; } set { _verbosity = value; } }

        string ILogger.Parameters { get { return _parameters; } set { _parameters = value; } }

        # endregion

        # region IList<PSObject> Property Implementation
        

        PSObject IList<PSObject>.this[int index] { get { return (index < 0) ? null : _innerList.Skip(index).Take(1).FirstOrDefault(); } set { throw new NotSupportedException(); } }

        # endregion

        # region ICollection<PSObject> Property Implementation

        public int Count { get { return _innerList.Count; } }

        bool ICollection<PSObject>.IsReadOnly { get { return true; } }

        # endregion

        # region IList Property Implementation

        object IList.this[int index] { get { return (index < 0) ? null : _innerList.Skip(index).Take(1).FirstOrDefault(); } set { throw new NotSupportedException(); } }

        bool IList.IsReadOnly { get { return true; } }

        bool IList.IsFixedSize { get { return false; } }

        # endregion

        # region ICollection Property Implementation

        object ICollection.SyncRoot { get { return _syncRoot; } }

        bool ICollection.IsSynchronized { get { return true; } }

        # endregion

        # region INodeLogger Method Implementation

        void INodeLogger.Initialize(IEventSource eventSource, Int32 nodeCount)
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
                eventSource.StatusEventRaised += OnStatusEventRaised;
                eventSource.AnyEventRaised += OnAnyEventRaised;
            }
            finally { Monitor.Exit(_syncRoot); }
        }

        # endregion

        # region ILogger Method Implementation

        void ILogger.Initialize(IEventSource eventSource)
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
                    return;
                }
                _eventSource = eventSource;
                _cpuCount = -1;
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
                eventSource.StatusEventRaised += OnStatusEventRaised;
                eventSource.AnyEventRaised += OnAnyEventRaised;
            }
            finally { Monitor.Exit(_syncRoot); }
        }

        void ILogger.Shutdown()
        {
            Monitor.Enter(_syncRoot);
            try
            {
                if (_eventSource != null)
                {
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
                    _eventSource.StatusEventRaised -= OnStatusEventRaised;
                    _eventSource.AnyEventRaised -= OnAnyEventRaised;
                    _eventSource = null;
                }
            }
            finally { Monitor.Exit(_syncRoot); }
        }

        # endregion

        # region IList<PSObject> Method Implementation

        int IList<PSObject>.IndexOf(PSObject item) { return _innerList.ToList().IndexOf(item); }

        void IList<PSObject>.Insert(Int32 index, PSObject item) { throw new NotSupportedException(); }

        void IList<PSObject>.RemoveAt(Int32 index) { throw new NotSupportedException(); }

        # endregion

        # region ICollection<PSObject> Method Implementation

        void ICollection<PSObject>.Add(PSObject item) { Add(item); }

        public void Clear()
        {
            Monitor.Enter(_syncRoot);
            try { _innerList.Clear(); }
            finally { Monitor.Exit(_syncRoot); }
        }

        public bool Contains(PSObject item) { return _innerList.Contains(item); }

        public void CopyTo(PSObject[] array, Int32 arrayIndex) { _innerList.CopyTo(array, arrayIndex); }

        bool ICollection<PSObject>.Remove(PSObject item) { throw new NotSupportedException(); }

        # endregion

        # region IEnumerable<PSObject> Method Implementation

        public IEnumerator<PSObject> GetEnumerator() { return _innerList.GetEnumerator(); }

        # endregion

        # region IList Method Implementation
        
        public const string PropertyName_Verbosity = "Verbosity";

        public void Add(object item)
        {
            if (item == null)
                throw new ArgumentNullException("item");
            Monitor.Enter(_syncRoot);
            try
            {
                PSObject obj = (item is PSObject) ? (PSObject)item : PSObject.AsPSObject(item);
                if (!obj.Properties.Any(p => String.Equals(p.Name, PropertyName_Verbosity)))
                    obj.Properties.Add(new PSNoteProperty(PropertyName_Verbosity, Verbosity));
                _innerList.Enqueue(obj); }
            finally { Monitor.Exit(_syncRoot); }
        }

        int IList.Add(object value)
        {
            if (value == null)
                throw new ArgumentNullException("value");
            int index;
            Monitor.Enter(_syncRoot);
            try
            {
                index = _innerList.Count;
                Add(value);
            }
            finally { Monitor.Exit(_syncRoot); }
            return index;
        }

        bool IList.Contains(object value)
        {
            if (value == null || value is PSObject)
                return Contains((PSObject)value);
            return _innerList.Select(o => o.BaseObject).ToList().Contains(value);
        }

        int IList.IndexOf(object value)
        {
            if (value == null || value is PSObject)
                return _innerList.ToList().IndexOf((PSObject)value);
            return _innerList.Select(o => o.BaseObject).ToList().IndexOf(value);
        }

        void IList.Insert(Int32 index, object value) { throw new NotSupportedException(); }

        void IList.Remove(object value) { throw new NotSupportedException(); }

        void IList.RemoveAt(Int32 index) { throw new NotSupportedException(); }

        # endregion

        # region ICollection Method Implementation

        void ICollection.CopyTo(System.Array array, Int32 index) { _innerList.ToArray().CopyTo(array, index); }

        # endregion

        # region IEnumerable Method Implementation

        IEnumerator IEnumerable.GetEnumerator() { return ((IEnumerable)_innerList).GetEnumerator(); }

        # endregion
        
        #region Event Source Handler Methods
        
        
        private void AddBuildEventArgs(BuildEventArgs e, LoggerVerbosity level)
        {
            Monitor.Enter(_syncRoot);
            try
            {
                if (level <= _verbosity)
                    Add(e);
            }
            finally { Monitor.Exit(_syncRoot); }
        }

        private void OnMessageRaised(object sender, BuildMessageEventArgs e)
        {
            if (e.Importance == MessageImportance.High)
                AddBuildEventArgs(e, LoggerVerbosity.Normal);
            else if (e.Importance == MessageImportance.Low)
                AddBuildEventArgs(e, LoggerVerbosity.Detailed);
            else
                AddBuildEventArgs(e, LoggerVerbosity.Diagnostic);
        }
        private void OnErrorRaised(object sender, BuildErrorEventArgs e) { AddBuildEventArgs(e, LoggerVerbosity.Normal); }
        private void OnWarningRaised(object sender, BuildWarningEventArgs e) { AddBuildEventArgs(e, LoggerVerbosity.Normal); }
        private void OnBuildStarted(object sender, BuildStartedEventArgs e) { AddBuildEventArgs(e, LoggerVerbosity.Normal); }
        private void OnBuildFinished(object sender, BuildFinishedEventArgs e) { AddBuildEventArgs(e, LoggerVerbosity.Quiet); }
        private void OnProjectStarted(object sender, ProjectStartedEventArgs e) { AddBuildEventArgs(e, LoggerVerbosity.Detailed); }
        private void OnProjectFinished(object sender, ProjectFinishedEventArgs e) { AddBuildEventArgs(e, (e.Succeeded) ? LoggerVerbosity.Minimal : LoggerVerbosity.Normal); }
        private void OnTargetStarted(object sender, TargetStartedEventArgs e) { AddBuildEventArgs(e, LoggerVerbosity.Detailed); }
        private void OnTargetFinished(object sender, TargetFinishedEventArgs e) { AddBuildEventArgs(e, (e.Succeeded) ? LoggerVerbosity.Detailed : LoggerVerbosity.Normal); }
        private void OnTaskStarted(object sender, TaskStartedEventArgs e) { AddBuildEventArgs(e, LoggerVerbosity.Detailed); }
        private void OnTaskFinished(object sender, TaskFinishedEventArgs e) { AddBuildEventArgs(e, (e.Succeeded) ? LoggerVerbosity.Detailed : LoggerVerbosity.Normal); }
        private void OnCustomEventRaised(object sender, CustomBuildEventArgs e)
        {
            if (e is ExternalProjectFinishedEventArgs)
                AddBuildEventArgs(e, ((e as ExternalProjectFinishedEventArgs).Succeeded) ? LoggerVerbosity.Detailed : LoggerVerbosity.Normal); 
            else
                AddBuildEventArgs(e, LoggerVerbosity.Detailed);
        }
        private void OnStatusEventRaised(object sender, BuildStatusEventArgs e) { AddBuildEventArgs(e, LoggerVerbosity.Minimal); }
        private void OnAnyEventRaised(object sender, BuildEventArgs e) { AddBuildEventArgs(e, LoggerVerbosity.Detailed); }
        
        #endregion
    }
}
