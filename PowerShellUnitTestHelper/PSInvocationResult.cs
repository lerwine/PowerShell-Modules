using System;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

namespace PowerShellUnitTestHelper
{
    public class PSInvocationResult
    {
        public static Exception Error { get; private set; }
        public Collection<DebugRecord> DebugStream { get; private set; }
        public Collection<ErrorRecord> ErrorStream { get; private set; }
        public bool HadErrors { get; private set; }
        public string HistoryString { get; private set; }
        public Collection<InformationRecord> InformationStream { get; private set; }
        public Collection<PSObject> Output { get; private set; }
        public Collection<ProgressRecord> ProgressStream { get; private set; }
        public Collection<VerboseRecord> VerboseStream { get; private set; }
        public Collection<WarningRecord> WarningStream { get; private set; }

        internal static PSInvocationResult Create(Runspace runspace, PowerShell powershell)
        {
            PSInvocationResult result = new PSInvocationResult();
            try
            {
                powershell.Runspace = runspace;
                result.Output = powershell.Invoke();
                result.HadErrors = powershell.HadErrors;
                result.HistoryString = powershell.HistoryString;
                result.DebugStream = powershell.Streams.Debug.ReadAll();
                result.ErrorStream = powershell.Streams.Error.ReadAll();
                result.InformationStream = powershell.Streams.Information.ReadAll();
                result.ProgressStream = powershell.Streams.Progress.ReadAll();
                result.VerboseStream = powershell.Streams.Verbose.ReadAll();
                result.WarningStream = powershell.Streams.Warning.ReadAll();
            }
            catch (Exception exception)
            {
                Error = exception;
            }

            if (result.DebugStream == null)
                result.DebugStream = new Collection<DebugRecord>();
            if (result.ErrorStream == null)
                result.ErrorStream = new Collection<ErrorRecord>();
            if (result.InformationStream == null)
                result.InformationStream = new Collection<InformationRecord>();
            if (result.ProgressStream == null)
                result.ProgressStream = new Collection<ProgressRecord>();
            if (result.VerboseStream == null)
                result.VerboseStream = new Collection<VerboseRecord>();
            if (result.WarningStream == null)
                result.WarningStream = new Collection<WarningRecord>();
            return result;
        }
    }
}