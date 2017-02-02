using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Text;

namespace IOUtilityCLR.PSInvocation
{
    public class PSInvocationResult
    {
        public PSInvocationResult(string script, IPSInvocationContext context, PowerShell powerShell, object[] variableKeys) : this(script, context, powerShell, variableKeys, new object[0]) { }
        public PSInvocationResult(string script, IPSInvocationContext context, PowerShell powerShell, object[] variableKeys, params object[] arguments)
        {
            ErrorRecord error = null;
            try
            {
                if (context.UseLocalScope.HasValue)
                    powerShell.AddScript(script, context.UseLocalScope.Value);
                else
                    powerShell.AddScript(script);
                Output = powerShell.Invoke();
                HadErrors = powerShell.HadErrors;
                RanToCompletion = true;
            }
            catch (RuntimeException exception)
            {
                error = exception.ErrorRecord;
                HadErrors = true;
            }
            catch (ArgumentException exception)
            {
                error = new ErrorRecord(exception, "UnexpectedInvalidArgument", ErrorCategory.InvalidArgument, script);
                HadErrors = true;
            }
            catch (Exception exception)
            {
                error = new ErrorRecord(exception, "UnexpectedError", ErrorCategory.NotSpecified, script);
                HadErrors = true;
            }
            Errors = powerShell.Streams.Error.ReadAll();
            if (error != null)
                Errors.Add(error);
            Warnings = powerShell.Streams.Warning.ReadAll();
            Verbose = powerShell.Streams.Verbose.ReadAll();
            Debug = powerShell.Streams.Debug.ReadAll();
            if (Output == null)
                Output = new Collection<PSObject>();
            Variables = new Hashtable();
            foreach (object key in variableKeys)
                Variables[key] = powerShell.Runspace.SessionStateProxy.GetVariable((key is string) ? key as string : key.ToString());
        }
        public Hashtable Variables { get; private set; }
        public Collection<DebugRecord> Debug { get; private set; }
        public Collection<ErrorRecord> Errors { get; private set; }
        public bool HadErrors { get; private set; }
        public Collection<PSObject> Output { get; private set; }
        public bool RanToCompletion { get; private set; }
        public System.Collections.ObjectModel.Collection<VerboseRecord> Verbose { get; private set; }
        public Collection<WarningRecord> Warnings { get; private set; }
    }
}
