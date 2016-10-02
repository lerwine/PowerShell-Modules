using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

namespace IOUtilityCLR
{
    /// <summary>
    /// Represents an asynchronous wait for results from a PowerShell pipeline invocation.
    /// </summary>
    public abstract class PSAsyncInvocationResult<TResult> : IDisposable
        where TResult : PSInvocationResult
    {
        private object _syncRoot = new object();
        private Hashtable _synchronizedData;
        private IAsyncResult _asyncResult;
        private Runspace _runspace;
        private PowerShell _powershell;
        private string[] _variableNames;
        private bool _isCompleted = false;
        private bool _stopInvoked = false;
        private TResult _result = null;

        /// <summary>
        /// True if the PowerShell pipeline invocation has completed; otherwise, false.
        /// </summary>
        public bool IsCompleted
        {
            get
            {
                CheckEndInvoke(false);
                return _isCompleted;
            }
        }

        /// <summary>
        /// Waits for the invocation to complete, and returns the result.
        /// </summary>
        /// <returns>Object which represents the results of the invocation.</returns>
        public TResult GetResult()
        {
            CheckEndInvoke(true);
            return _result;
        }

        private void CheckEndInvoke(bool force)
        {
            lock (_syncRoot)
            {
                if (!_isCompleted && (force || _asyncResult.IsCompleted))
                {
                    _isCompleted = true;
                    try
                    {
                        _result = CreateResult(new Collection<PSObject>(_powershell.EndInvoke(_asyncResult)), _runspace, _powershell.Streams, _variableNames, _stopInvoked, _synchronizedData);
                    }
                    catch
                    {
                        throw;
                    }
                    finally
                    {
                        _powershell.Dispose();
                        _powershell = null;
                        _runspace.Dispose();
                        _runspace = null;
                    }
                }
            }
        }

        protected abstract TResult CreateResult(Collection<PSObject> collection, Runspace runspace, PSDataStreams streams, string[] variableNames, bool stopInvoked, Hashtable synchronizedData);

        /// <summary>
        /// Create a new object to represent an asynchronous wait for results from a PowerShell pipeline invocation.
        /// </summary>
        /// <param name="asyncResult">Object representing background invocation.</param>
        /// <param name="runspace">Runspace of background invocation.</param>
        /// <param name="powershell">Powershell containing pipeline being invoked.</param>
        /// <param name="variableNames">Name of variables to check when invocation is completed.</param>
        /// <param name="synchronizedData">Data that is being syncrhonized with the background pipeline invocation.</param>
        public PSAsyncInvocationResult(IAsyncResult asyncResult, Runspace runspace, PowerShell powershell, string[] variableNames, Hashtable synchronizedData)
        {
            _asyncResult = asyncResult;
            _runspace = runspace;
            _powershell = powershell;
            _variableNames = variableNames;
            _synchronizedData = synchronizedData;
        }

        /// <summary>
        /// Gets any errors that have been produced by the executing PowerShell pipeline.
        /// </summary>
        /// <returns>Error records, if any, that have been produced.</returns>
        public Collection<ErrorRecord> GetErrors()
        {
            lock (_syncRoot)
            {
                if (!_isCompleted)
                    return new Collection<ErrorRecord>(_powershell.Streams.Error.ReadAll());
            }

            return new Collection<ErrorRecord>();
        }

        /// <summary>
        /// Gets any warnings that have been produced by the executing PowerShell pipeline.
        /// </summary>
        /// <returns>Warning records, if any, that have been produced.</returns>
        public Collection<WarningRecord> GetWarnings()
        {
            lock (_syncRoot)
            {
                if (!_isCompleted)
                    return new Collection<WarningRecord>(_powershell.Streams.Warning.ReadAll());
            }

            return new Collection<WarningRecord>();
        }

        /// <summary>
        /// Gets any progress notifications that have been produced by the executing PowerShell pipeline.
        /// </summary>
        /// <returns>Progress records, if any, that have been produced.</returns>
        public Collection<ProgressRecord> GetProgress()
        {
            lock (_syncRoot)
            {
                if (!_isCompleted)
                    return new Collection<ProgressRecord>(_powershell.Streams.Progress.ReadAll());
            }

            return new Collection<ProgressRecord>();
        }

        /// <summary>
        /// Gets any informational messages that have been produced by the executing PowerShell pipeline.
        /// </summary>
        /// <returns>Informational messages, if any, that have been produced.</returns>
        public Collection<InformationRecord> GetInformationMessages()
        {
            lock (_syncRoot)
            {
                if (!_isCompleted)
                    return new Collection<InformationRecord>(_powershell.Streams.Information.ReadAll());
            }

            return new Collection<InformationRecord>();
        }

        /// <summary>
        /// Gets any verbose messages that have been produced by the executing PowerShell pipeline.
        /// </summary>
        /// <returns>Verbose messages, if any, that have been produced.</returns>
        public Collection<VerboseRecord> GetVerboseMessages()
        {
            lock (_syncRoot)
            {
                if (!_isCompleted)
                    return new Collection<VerboseRecord>(_powershell.Streams.Verbose.ReadAll());
            }

            return new Collection<VerboseRecord>();
        }

        /// <summary>
        /// Gets any debug messages that have been produced by the executing PowerShell pipeline.
        /// </summary>
        /// <returns>Debug messages, if any, that have been produced.</returns>
        public Collection<DebugRecord> GetDebugMessages()
        {
            lock (_syncRoot)
            {
                if (!_isCompleted)
                    return new Collection<DebugRecord>(_powershell.Streams.Debug.ReadAll());
            }

            return new Collection<DebugRecord>();
        }

        #region IDisposable Support

        /// <summary>
        /// Disposes all PowerShell objects used.
        /// </summary>
        /// <param name="disposing">True if being disposed through the <see cref="Dispose()"/> command.</param>
        protected virtual void Dispose(bool disposing)
        {
            object syncRoot = _syncRoot;
            _syncRoot = null;
            if (syncRoot == null || !disposing)
                return;
            lock (syncRoot)
            {
                if (_powershell != null)
                {
                    try { _powershell.Stop(); } catch { }
                    try { _powershell.Dispose(); } catch { }
                }
                if (_runspace != null)
                    try { _runspace.Dispose(); } catch { }
            }
        }

        /// <summary>
        /// Disposes all PowerShell objects used.
        /// </summary>
        public void Dispose() { Dispose(true); }

        #endregion
    }

    /// <summary>
    /// Represents an asynchronous wait for results from a PowerShell pipeline invocation.
    /// </summary>
    public class PSAsyncInvocationResult : PSAsyncInvocationResult<PSInvocationResult>
    {
        public PSAsyncInvocationResult(IAsyncResult asyncResult, Runspace runspace, PowerShell powershell, string[] variableNames, Hashtable synchronizedData) 
            : base(asyncResult, runspace, powershell, variableNames, synchronizedData)
        {
        }

        private static PSAsyncInvocationResult Create(Func<Runspace> createRunspace, Func<Runspace, PowerShell> createPowerShell, IEnumerable<string> variableNames, Func<PowerShell, IAsyncResult> invoke, Hashtable synchronizedData)
        {
            string[] names = variableNames.Where(s => !String.IsNullOrEmpty(s)).ToArray();
            Runspace runspace = createRunspace();
            try
            {
                PowerShell powershell = createPowerShell(runspace);
                try { return new PSAsyncInvocationResult(invoke(powershell), runspace, powershell, names, synchronizedData); }
                catch
                {
                    powershell.Dispose();
                    throw;
                }
            }
            catch
            {
                runspace.Dispose();
                throw;
            }
        }

        internal static PSAsyncInvocationResult Create(Func<Runspace> createRunspace, Func<Runspace, PowerShell> createPowerShell, IEnumerable<string> variableNames, Hashtable synchronizedData)
        {
            return Create(createRunspace, createPowerShell, variableNames, ps => ps.BeginInvoke(), synchronizedData);
        }

        internal static PSAsyncInvocationResult Create<T>(Func<Runspace> createRunspace, Func<Runspace, PowerShell> createPowerShell, PSDataCollection<T> input, PSInvocationSettings settings, IEnumerable<string> variableNames, Hashtable synchronizedData)
        {
            return Create(createRunspace, createPowerShell, variableNames, ps => ps.BeginInvoke<T>(input, settings, null, null), synchronizedData);
        }

        internal static PSAsyncInvocationResult Create<T>(Func<Runspace> createRunspace, Func<Runspace, PowerShell> createPowerShell, PSDataCollection<T> input, IEnumerable<string> variableNames, Hashtable synchronizedData)
        {
            return Create(createRunspace, createPowerShell, variableNames, ps => ps.BeginInvoke<T>(input), synchronizedData);
        }

        protected override PSInvocationResult CreateResult(Collection<PSObject> collection, Runspace runspace, PSDataStreams streams, string[] variableNames, bool stopInvoked, Hashtable synchronizedData)
        {
            return new PSInvocationResult(collection, runspace, streams, variableNames, stopInvoked, synchronizedData);
        }
    }
}