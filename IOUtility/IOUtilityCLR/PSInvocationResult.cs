using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
#if !PSLEGACY
using System.Linq;
#endif
using System.Management.Automation;
using System.Management.Automation.Runspaces;

namespace IOUtilityCLR
{
    /// <summary>
    /// Represents the results of invoking a PowerShell pipeline.
    /// </summary>
    public class PSInvocationResult
    {
        /// <summary>
        /// Output from pipeline invocation.
        /// </summary>
        public Collection<PSObject> Output { get; set; }
        /// <summary>
        /// Values of variables after pipeline invocation.
        /// </summary>
        public Dictionary<string, object> Variables { get; set; }
        /// <summary>
        /// Error records that were produced during the pipeline invocation.
        /// </summary>
        public Collection<ErrorRecord> Errors { get; private set; }
        /// <summary>
        /// Warning records that were produced during the pipeline invocation.
        /// </summary>
        public Collection<WarningRecord> Warnings { get; private set; }
        /// <summary>
        /// Verbose messages that were produced during the pipeline invocation.
        /// </summary>
        public Collection<VerboseRecord> VerboseMessages { get; private set; }
        /// <summary>
        /// Debug messages that were produced during the pipeline invocation.
        /// </summary>
        public Collection<DebugRecord> DebugMessages { get; private set; }
        /// <summary>
        /// True if the pipeline invocation was stopped before completion; otherwise false.
        /// </summary>
        public bool WasStopped { get; private set; }
        /// <summary>
        /// Synchronized data that was shared between the calling process and the pipeline being executed.
        /// </summary>
        public Hashtable SynchronizedData { get; private set; }

        public PSInvocationResult(Collection<PSObject> collection, Runspace runspace, PSDataStreams streams, string[] variableNames, bool wasStopped, Hashtable synchronizedData)
        {
            Output = collection;
            Variables = new Dictionary<string, object>();
            foreach (string name in variableNames)
                Variables.Add(name, runspace.SessionStateProxy.GetVariable(name));
            Errors = new Collection<ErrorRecord>(streams.Error.ReadAll());
            Warnings = new Collection<WarningRecord>(streams.Warning.ReadAll());
            VerboseMessages = new Collection<VerboseRecord>(streams.Verbose.ReadAll());
            DebugMessages = new Collection<DebugRecord>(streams.Debug.ReadAll());
            WasStopped = wasStopped;
            SynchronizedData = synchronizedData;
        }
        
        private static PSInvocationResult Create(Func<Runspace> createRunspace, Func<Runspace, PowerShell> createPowerShell, IEnumerable<string> variableNames, Func<PowerShell, Collection<PSObject>> invoke, Hashtable synchronizedData)
        {
#if PSLEGACY
            string[] names = LinqEmul.ToArray<string>(LinqEmul.Where(variableNames, LinqEmul.StringNotNullOrEmpty));
#else
            string[] names = variableNames.Where(s => !String.IsNullOrEmpty(s)).ToArray();
#endif
            using (Runspace runspace = createRunspace())
            {
                using (PowerShell powershell = createPowerShell(runspace))
                    return new PSInvocationResult(invoke(powershell), runspace, powershell.Streams, names, false, synchronizedData);
            }
        }

        internal static PSInvocationResult Create(Func<Runspace> createRunspace, Func<Runspace, PowerShell> createPowerShell, IEnumerable<string> variableNames, Hashtable synchronizedData)
        {
            return Create(createRunspace, createPowerShell, variableNames, ps => ps.Invoke(), synchronizedData);
        }

        internal static PSInvocationResult Create(Func<Runspace> createRunspace, Func<Runspace, PowerShell> createPowerShell, IEnumerable input, IEnumerable<string> variableNames, Hashtable synchronizedData)
        {
            return Create(createRunspace, createPowerShell, variableNames, ps => ps.Invoke(input), synchronizedData);
        }

        internal static PSInvocationResult Create(Func<Runspace> createRunspace, Func<Runspace, PowerShell> createPowerShell, IEnumerable input, PSInvocationSettings settings, IEnumerable<string> variableNames, Hashtable synchronizedData)
        {
            return Create(createRunspace, createPowerShell, variableNames, ps => ps.Invoke(input, settings), synchronizedData);
        }
    }
}