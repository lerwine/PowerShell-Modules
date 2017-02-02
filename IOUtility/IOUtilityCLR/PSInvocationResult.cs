using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
#if !PSLEGACY2
using System.Linq;
#endif
using System.Management.Automation;
using System.Text;

namespace IOUtilityCLR
{
    public class PSInvocationResult
    {
        private Hashtable _variables;
        private Collection<DebugRecord> _debug;
        private Collection<ErrorRecord> _errors;
        private bool _hadErrors;
        private Collection<PSObject> _output;
        private bool _ranToCompletion;
        private Collection<VerboseRecord> _verbose;
        private Collection<WarningRecord> _warnings;
		
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
                if (arguments != null && arguments.Length > 0)
                {
                    foreach (object a in arguments)
                        powerShell.AddArgument(a);
                }
                _output = powerShell.Invoke();
#if !PSLEGACY2
                _hadErrors = powerShell.HadErrors;
#endif
                _ranToCompletion = true;
            }
            catch (RuntimeException exception)
            {
                error = exception.ErrorRecord;
                _hadErrors = true;
            }
            catch (ArgumentException exception)
            {
                error = new ErrorRecord(exception, "UnexpectedInvalidArgument", ErrorCategory.InvalidArgument, script);
                _hadErrors = true;
            }
            catch (Exception exception)
            {
                error = new ErrorRecord(exception, "UnexpectedError", ErrorCategory.NotSpecified, script);
                _hadErrors = true;
            }
            _errors = powerShell.Streams.Error.ReadAll();
            if (error != null)
                _errors.Add(error);
#if PSLEGACY2
			if (_errors.Count > 0)
				_hadErrors = true;
#endif
            _warnings = powerShell.Streams.Warning.ReadAll();
            _verbose = powerShell.Streams.Verbose.ReadAll();
            _debug = powerShell.Streams.Debug.ReadAll();
            if (_output == null)
                _output = new Collection<PSObject>();
            _variables = new Hashtable();
            foreach (object key in variableKeys)
                _variables[key] = powerShell.Runspace.SessionStateProxy.GetVariable((key is string) ? key as string : key.ToString());
        }
		
        public Hashtable Variables { get { return _variables; } }
        public Collection<DebugRecord> Debug { get { return _debug; } }
        public Collection<ErrorRecord> Errors { get { return _errors; } }
        public bool HadErrors { get { return _hadErrors; } }
        public Collection<PSObject> Output { get { return _output; } }
        public bool RanToCompletion { get { return _ranToCompletion; } }
        public Collection<VerboseRecord> Verbose { get { return _verbose; } }
        public Collection<WarningRecord> Warnings { get { return _warnings; } }
    }
}
