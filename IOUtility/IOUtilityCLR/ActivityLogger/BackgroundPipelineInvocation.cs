using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Host;
using System.Management.Automation.Runspaces;

namespace ActivityLogger
{
    public class BackgroundPipelineInvocation : IDisposable
    {
        private bool _isDisposed = false;
        private object _syncRoot = new object();
        private PSHost _host;
        private Runspace _runspace;
        private PowerShell _powerShell;
        private IAsyncResult _asyncResult;
        private bool _isCompleted = false;
#if PSLEGACY
        private object _state = null;
        private Hashtable _synchronizedData = null;
        private bool _stopInvoked = false;
        private Collection<PSObject> _output = null;
        private Collection<ErrorRecord> _errors = null;
        private Collection<WarningRecord> _warnings = null;
        private Collection<VerboseRecord> _verbose = null;
        private Collection<DebugRecord> _debug = null;
#endif

        public bool IsCompleted
        {
            get
            {
                lock (_syncRoot)
                {
                    if (!_isCompleted && _asyncResult.IsCompleted)
                        EndInvoke();
                }

                return _isCompleted;
            }
        }

#if PSLEGACY
        public object State { get { return _state; } private set { _state = value; } }
#else
        public object State { get; private set; }
#endif

#if PSLEGACY
        public Hashtable SynchronizedData { get { return _synchronizedData; } private set { _synchronizedData = value; } }
#else
        public Hashtable SynchronizedData { get; private set; }
#endif

#if PSLEGACY
        public bool StopInvoked { get { return _stopInvoked; } private set { _stopInvoked = value; } }
#else
        public bool StopInvoked { get; private set; }
#endif

#if PSLEGACY
        public Collection<PSObject> Output { get { return _output; } private set { _output = value; } }
#else
        public Collection<PSObject> Output { get; private set; }
#endif

#if PSLEGACY
        public Collection<ErrorRecord> Errors { get { return _errors; } private set { _errors = value; } }
#else
        public Collection<ErrorRecord> Errors { get; private set; }
#endif

#if PSLEGACY
        public Collection<WarningRecord> Warnings { get { return _warnings; } private set { _warnings = value; } }
#else
        public Collection<WarningRecord> Warnings { get; private set; }
#endif

#if PSLEGACY
        public Collection<VerboseRecord> Verbose { get { return _verbose; } private set { _verbose = value; } }
#else
        public Collection<VerboseRecord> Verbose { get; private set; }
#endif

#if PSLEGACY
        public Collection<DebugRecord> Debug { get { return _debug; } private set { _debug = value; } }
#else
        public Collection<DebugRecord> Debug { get; private set; }
#endif

        public BackgroundPipelineInvocation(PSHost host, BackgroundPipelineParameters parameters) : this(host, parameters, null) { }
        public BackgroundPipelineInvocation(PSHost host, BackgroundPipelineParameters parameters, object state)
        {
            if (host == null)
                throw new ArgumentNullException("host");
            if (parameters == null)
                throw new ArgumentNullException("parameters");
#if PSLEGACY2
            List<ScriptBlock> scripts = new List<ScriptBlock>();
            foreach (ScriptBlock sb in parameters.PipelineScripts)
            {
                if (sb != null)
                    scripts.Add(sb);
            }
#else
            List<ScriptBlock> scripts = parameters.PipelineScripts.Where(s => s != null).ToList();
#endif
            if (scripts.Count == 0)
                throw new ArgumentException("No scripts to execute.");
            SynchronizedData = parameters.SynchronizedData;
            State = state;
            Output = new Collection<PSObject>();
            Errors = new Collection<ErrorRecord>();
            Warnings = new Collection<WarningRecord>();
            Verbose = new Collection<VerboseRecord>();
            Debug = new Collection<DebugRecord>();
            _host = host;
            _runspace = RunspaceFactory.CreateRunspace();
            try
            {
                BeforeOpenRunspace(host, _runspace, parameters);
                _runspace.Open();
                foreach (string key in parameters.Variables.Keys)
                    _runspace.SessionStateProxy.SetVariable(key, parameters.Variables[key]);
                _runspace.SessionStateProxy.SetVariable("SynchronizedData", SynchronizedData);
#if PSLEGACY2
                _powerShell.Streams.Progress.DataAdded += Progress_DataAdded;
#else
                _powerShell.Streams.Progress.DataAdding += Progress_DataAdding;
#endif
                BeforeAddPipelineScripts(host, _runspace, scripts);
                _powerShell = scripts[0].GetPowerShell();
                try
                {
                    _powerShell.Runspace = _runspace;
                    for (int i = 1; i < scripts.Count; i++)
                        _powerShell.AddScript(scripts[i].ToString());
                    _asyncResult = _powerShell.BeginInvoke();
                    AfterScriptsAdded(host, _runspace, _powerShell);
                }
                catch
                {
                    _powerShell.Dispose();
                    throw;
                }
            }
            catch
            {
                _runspace.Dispose();
                throw;
            }
        }

        protected virtual void BeforeOpenRunspace(PSHost host, Runspace runspace, BackgroundPipelineParameters parameters)
        {
            runspace.ApartmentState = parameters.ApartmentState;
            runspace.ThreadOptions = parameters.ThreadOptions;
        }

        protected virtual void BeforeAddPipelineScripts(PSHost host, Runspace runspace, List<ScriptBlock> scripts) { }

        protected virtual void AfterScriptsAdded(PSHost host, Runspace runspace, PowerShell powerShell) { }

#if PSLEGACY2
        private void Progress_DataAdded(object sender, DataAddedEventArgs e)
        {
            ProgressRecord progress = _powerShell.Streams.Progress[e.Index];
#else
        private void Progress_DataAdding(object sender, DataAddingEventArgs e)
        {
            ProgressRecord progress = e.ItemAdded as ProgressRecord;
#endif
            if (progress != null)
                OnProgressChanged(progress);
        }

        protected virtual void OnProgressChanged(ProgressRecord progress) { }

        public Collection<PSObject> GetResult()
        {
            lock (_syncRoot)
            {
                if (!_isCompleted)
                    EndInvoke();
            }

            return Output;
        }

        public void Stop()
        {
            lock (_syncRoot)
            {
                if (_isCompleted)
                    return;
                if (!_asyncResult.IsCompleted)
                {
                    StopInvoked = true;
                    _powerShell.Stop();
                }
                EndInvoke();
            }
        }

        private void EndInvoke()
        {
            _isCompleted = true;
            ReadAll<PSObject>(Output, _powerShell.EndInvoke(_asyncResult));
            ReadAll<ErrorRecord>(Errors, _powerShell.Streams.Error);
            ReadAll<WarningRecord>(Warnings, _powerShell.Streams.Warning);
            ReadAll<VerboseRecord>(Verbose, _powerShell.Streams.Verbose);
            ReadAll<DebugRecord>(Debug, _powerShell.Streams.Debug);
            OnEndInvoked();
        }

        protected virtual void OnEndInvoked() { }

        public Collection<ProgressRecord> ReadProgress() { return _powerShell.Streams.Progress.ReadAll(); }

        public Collection<ErrorRecord> ReadErrors()
        {
            lock (_syncRoot)
            {
                if (_isCompleted)
                    return new Collection<ErrorRecord>();

                return ReadAll<ErrorRecord>(Errors, _powerShell.Streams.Error);
            }
        }

        public Collection<WarningRecord> ReadWarnings()
        {
            lock (_syncRoot)
            {
                if (_isCompleted)
                    return new Collection<WarningRecord>();

                return ReadAll<WarningRecord>(Warnings, _powerShell.Streams.Warning);
            }
        }
        
        public Collection<VerboseRecord> ReadVerbose()
        {
            lock (_syncRoot)
            {
                if (_isCompleted)
                    return new Collection<VerboseRecord>();

                return ReadAll<VerboseRecord>(Verbose, _powerShell.Streams.Verbose);
            }
        }

        public Collection<DebugRecord> ReadDebug()
        {
            lock (_syncRoot)
            {
                if (_isCompleted)
                    return new Collection<DebugRecord>();

                return ReadAll<DebugRecord>(Debug, _powerShell.Streams.Debug);
            }
        }

        private static Collection<T> ReadAll<T>(Collection<T> target, PSDataCollection<T> source)
        {
            if (source == null)
                return new Collection<T>();
            Collection<T> items = source.ReadAll();
            foreach (T i in items)
                target.Add(i);
            return items;
        }

#region IDisposable Support

        public void Dispose() { Dispose(true); }

        protected virtual void Dispose(bool disposing)
        {
            object syncRoot = _syncRoot;
            if (syncRoot == null)
            {
                if (disposing)
                    throw new ObjectDisposedException(GetType().FullName);
                return;
            }
            lock (syncRoot)
            {
                if (_syncRoot != null)
                {
                    _syncRoot = null;
                    if (_powerShell != null)
                    {
                        try
                        {
                            if (_asyncResult != null && !_asyncResult.IsCompleted)
                                _powerShell.Stop();
                        }
                        catch { }
                        try { _powerShell.Dispose(); } catch { }
                    }
                    if (_runspace != null)
                        try { _runspace.Dispose(); } catch { }
                }
            }
        }

#endregion
    }
}
