Add-Type -AssemblyName 'Microsoft.Build', 'Microsoft.Build.Framework', 'Microsoft.Build.Utilities.v4.0' -ErrorAction Stop;
Add-Type -TypeDefinition @'
namespace CompileHelper
{
    using System;
    using System.Management.Automation;
    using System.Management.Automation.Host;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Build;
    using Microsoft.Build.Evaluation;
    using Microsoft.Build.Framework;

    public class Logger : IList<PSObject>, INodeLogger, IList
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
'@ -ReferencedAssemblies 'System', 'Microsoft.Build', 'Microsoft.Build.Framework', 'Microsoft.Build.Utilities.v4.0', 'System.Management.Automation' -ErrorAction Stop;

if ($Project -ne $null) { $Project.ProjectCollection.UnloadAllProjects() }
$Project = [Microsoft.Build.Evaluation.Project]::new(($PSScriptRoot | Join-Path -ChildPath 'GDIPlus.csproj'));
if ($Project -eq $null) { return }
$Logger = [CompileHelper.Logger]::new();
$Logger.StartBuild($Project);
$Logger.Verbosity = [Microsoft.Build.Framework.LoggerVerbosity]::Normal;
$Output = @();
while (-not $Logger.WaitBuild(1000)) {
    $v = $Logger.Read();
    if ($v.Count -gt 0) { $v | Out-String }
}
$v = $Logger.Read();
if ($v.Count -gt 0) { $v | Out-String }
$Logger.EndBuild();