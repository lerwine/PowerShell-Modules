using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
#if !PSLEGACY
using System.Linq;
#endif
using System.Management.Automation;
using System.Management.Automation.Host;
using System.Management.Automation.Runspaces;

namespace IOUtilityCLR
{
    /// <summary>
    /// Represents the invocation of a background PowerShell pipeline.
    /// </summary>
    public class BackgroundPipelineInvocation : IDisposable
    {
        private object _syncRoot = new object();
        PSHost _host;
        Runspace _runspace;
        PowerShell _powerShell;
        IAsyncResult _asyncResult;
        private bool _isCompleted = false;

        /// <summary>
        /// True if the invocation has finished; otherwise, false.
        /// </summary>
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

        /// <summary>
        /// User state object associated with invocation.
        /// </summary>
        public object State { get; private set; }

        /// <summary>
        /// Values to be shared between host process and the invoked powershell instance.
        /// </summary>
        public Hashtable SynchronizedData { get; private set; }

        /// <summary>
        /// True if the background PowerShell instance was stopped before completion; otherwise, false.
        /// </summary>
        public bool StopInvoked { get; private set; }

        /// <summary>
        /// Output objects returned as a result of the invocation.
        /// </summary>
        public Collection<PSObject> Output { get; private set; }

        /// <summary>
        /// Error records which were generated during the invocation.
        /// </summary>
        public Collection<ErrorRecord> Errors { get; private set; }

        /// <summary>
        /// Warning records which were generated during the invocation.
        /// </summary>
        public Collection<WarningRecord> Warnings { get; private set; }
        
        /// <summary>
        /// Verbose records which were generated during the invocation.
        /// </summary>
        public Collection<VerboseRecord> Verbose { get; private set; }

        /// <summary>
        /// Debug records which were generated during the invocation.
        /// </summary>
        public Collection<DebugRecord> Debug { get; private set; }
        
        /// <summary>
        /// Invoke PowerShell scripts in the background.
        /// </summary>
        /// <param name="host">PowerShell host to use.</param>
        /// <param name="parameters">Parameters which represent the behavior and scripts for the background process.</param>
        public BackgroundPipelineInvocation(PSHost host, BackgroundPipelineParameters parameters) : this(host, parameters, null) { }

        /// <summary>
        /// Invoke PowerShell scripts in the background.
        /// </summary>
        /// <param name="host">PowerShell host to use.</param>
        /// <param name="parameters">Parameters which represent the behavior and scripts for the background process.</param>
        /// <param name="state">User state to associate with the results.</param>
        public BackgroundPipelineInvocation(PSHost host, BackgroundPipelineParameters parameters, object state)
        {
            if (host == null)
                throw new ArgumentNullException("host");
            if (parameters == null)
                throw new ArgumentNullException("parameters");

#if PSLEGACY
            List<ScriptBlock> scripts = new List<ScriptBlock>(LinqEmul.Where<ScriptBlock>(parameters.PipelineScripts, new Func<ScriptBlock, bool>(LinqEmul.ObjectIsNotNullPredicate<ScriptBlock>)));
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
#if PSLEGACY
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


        /// <summary>
        /// This gets invoked after the runspace is created and before it is opened.
        /// </summary>
        /// <param name="host">PowerShell host associated with runspace.</param>
        /// <param name="runspace">Runspace about to be opened.</param>
        /// <param name="parameters">Parameters that define the behavior of the invocation</param>
        protected virtual void BeforeOpenRunspace(PSHost host, Runspace runspace, BackgroundPipelineParameters parameters)
        {
            runspace.ApartmentState = parameters.ApartmentState;
            runspace.ThreadOptions = parameters.ThreadOptions;
        }

        /// <summary>
        /// This gets invoked after the runspace is opened, and before any scripts are added to the pipeline.
        /// </summary>
        /// <param name="host">PowerShell host associated with runspace.</param>
        /// <param name="runspace">Runspace about to be opened.</param>
        /// <param name="scripts">List of scripts to be added in order.</param>
        protected virtual void BeforeAddPipelineScripts(PSHost host, Runspace runspace, List<ScriptBlock> scripts) { }

        /// <summary>
        /// This gets invoked after scripts have been added to the pipeline.
        /// </summary>
        /// <param name="host">PowerShell host associated with runspace.</param>
        /// <param name="runspace">Runspace about to be opened.</param>
        /// <param name="powerShell">PowerShell object to be invoked.</param>
        protected virtual void AfterScriptsAdded(PSHost host, Runspace runspace, PowerShell powerShell) { }

#if PSLEGACY
        private void Progress_DataAdded(object sender, DataAddedEventArgs e)
        {
            ProgressRecord progress = _powerShell.Streams.Progress[e.Index] as ProgressRecord;
            if (progress != null)
                OnProgressChanged(progress);
        }
#else
        private void Progress_DataAdding(object sender, DataAddingEventArgs e)
        {
            ProgressRecord progress = e.ItemAdded as ProgressRecord;
            if (progress != null)
                OnProgressChanged(progress);
        }
#endif

        /// <summary>
        /// This gets called when a script in the pipline writes to the progress stream.
        /// </summary>
        /// <param name="progress">Progress object which was written to the progress stream.</param>
        protected virtual void OnProgressChanged(ProgressRecord progress) { }

        /// <summary>
        /// Waits for invocation to complete and gets pipeline output.
        /// </summary>
        /// <returns>Collection of objects which represent the output of the executed pipeline.</returns>
        public Collection<PSObject> GetResult()
        {
            lock (_syncRoot)
            {
                if (!_isCompleted)
                    EndInvoke();
            }

            return Output;
        }

        /// <summary>
        /// Stops the execution of the pipeline.
        /// </summary>
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

        /// <summary>
        /// This gets called when the results of the pipeline execution are obtained.
        /// </summary>
        protected virtual void OnEndInvoked() { }

        /// <summary>
        /// Reads progress records from the progress stream.
        /// </summary>
        /// <returns>Progress records that were written since the last time this method was called.</returns>
        public Collection<ProgressRecord> ReadProgress() { return _powerShell.Streams.Progress.ReadAll(); }

        /// <summary>
        /// Reads error records from the error stream.
        /// </summary>
        /// <returns>Error records that were written since the last time this method was called.</returns>
        public Collection<ErrorRecord> ReadErrors()
        {
            lock (_syncRoot)
            {
                if (_isCompleted)
                    return new Collection<ErrorRecord>();

                return ReadAll<ErrorRecord>(Errors, _powerShell.Streams.Error);
            }
        }

        /// <summary>
        /// Reads warning records from the warnings stream.
        /// </summary>
        /// <returns>Warning records that were written since the last time this method was called.</returns>
        public Collection<WarningRecord> ReadWarnings()
        {
            lock (_syncRoot)
            {
                if (_isCompleted)
                    return new Collection<WarningRecord>();

                return ReadAll<WarningRecord>(Warnings, _powerShell.Streams.Warning);
            }
        }

        /// <summary>
        /// Reads verbose records from the information stream
        /// </summary>
        /// <returns>Verbose records that were written since the last time this method was called.</returns>
        public Collection<VerboseRecord> ReadVerbose()
        {
            lock (_syncRoot)
            {
                if (_isCompleted)
                    return new Collection<VerboseRecord>();

                return ReadAll<VerboseRecord>(Verbose, _powerShell.Streams.Verbose);
            }
        }

        /// <summary>
        /// Reads debug records from the information stream
        /// </summary>
        /// <returns>Debug records that were written since the last time this method was called.</returns>
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

        /// <summary>
        /// Disposes the current invocation object.
        /// </summary>
        public void Dispose() { Dispose(true); }

        /// <summary>
        /// This gets called when the invocation object is about to be disposed.
        /// </summary>
        /// <param name="disposing">True if being disposed through the <see cref="BackgroundPipelineInvocation.Dispose()"/> method; otherwise, false.</param>
        protected virtual void Dispose(bool disposing)
        {
            object syncRoot = _syncRoot;
            if (syncRoot == null)
                return;
            lock (syncRoot)
            {
                if (_syncRoot == null)
                    return;

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

#endregion
    }
}
