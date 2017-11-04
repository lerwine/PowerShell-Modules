using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Host;
using System.Management.Automation.Runspaces;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace WpfCLR
{
    public class PSInvocationResult
    {
        private Collection<PSObject> _output;
        private Collection<ErrorRecord> _errors;
        private Collection<WarningRecord> _warnings;
        private Collection<VerboseRecord> _verbose;
        private Collection<DebugRecord> _debug;
        private bool _hadErrors, _completed = false;
        private Hashtable _variables;

        public static Regex SupportedVarNameRegex = new Regex(@"^[_a-zA-Z][_a-zA-Z\d]*$", RegexOptions.Compiled);

        private PSInvocationResult(Hashtable variables, PSObject thisObject)
        {
            _variables = Hashtable.Synchronized(new Hashtable());
            if (variables != null)
            {
                foreach (var key in _variables.Keys)
                {
                    string name = (key is string) ? key as string : key.ToString();
                    if (!SupportedVarNameRegex.IsMatch(name))
                        throw new ArgumentException(String.Format("{0} is an invalid variable name.", name), "variables");
                    if (String.Compare(name, "this", true) != 0 || thisObject == null)
                        _variables[name] = variables[key];
                }
            }
            if (thisObject != null)
                _variables.Add("this", thisObject);
        }

        public Collection<PSObject> Output { get { return _output; } }
        public Collection<ErrorRecord> Errors { get { return _errors; } }
        public Collection<WarningRecord> Warnings { get { return _warnings; } }
        public Collection<VerboseRecord> Verbose { get { return _verbose; } }
        public Collection<DebugRecord> Debug { get { return _debug; } }
        public bool HadErrors { get { return _hadErrors; } }
        public bool Completed { get { return _completed; } }
        public Hashtable Variables { get { return _variables; } }
        public static PSInvocationResult GetResult(string script, PSInvocationParams invocationParams, params object[] args)
        {
            if (invocationParams == null)
                invocationParams = new PSInvocationParams();
            PSInvocationResult result = new WpfCLR.PSInvocationResult(invocationParams.Variables, invocationParams.This);
            using (Runspace runspace = (invocationParams.Host == null) ?
                ((invocationParams.Configuration == null) ? RunspaceFactory.CreateRunspace() : RunspaceFactory.CreateRunspace(invocationParams.Host)) :
                ((invocationParams.Configuration == null) ? RunspaceFactory.CreateRunspace(invocationParams.Host) : RunspaceFactory.CreateRunspace(invocationParams.Host, invocationParams.Configuration)))
            {
                if (invocationParams.ApartmentState.HasValue)
                    runspace.ApartmentState = invocationParams.ApartmentState.Value;
                if (invocationParams.ThreadOptions.HasValue)
                    runspace.ThreadOptions = invocationParams.ThreadOptions.Value;
                runspace.Open();

                foreach (string key in result._variables.Keys)
                {
                    if (result._variables[key] != null)
                        runspace.SessionStateProxy.SetVariable(key, result._variables[key]);
                }

                if (!String.IsNullOrEmpty(invocationParams.Location))
                    runspace.SessionStateProxy.Path.SetLocation(invocationParams.Location);

                using (PowerShell powerShell = PowerShell.Create())
                {
                    powerShell.Runspace = runspace;

                    ErrorRecord error = null;
                    try
                    {
                        if (invocationParams.UseLocalScope.HasValue)
                            powerShell.AddScript(script, invocationParams.UseLocalScope.Value);
                        else
                            powerShell.AddScript(script);
                        result._output = powerShell.Invoke();
                        result._hadErrors = powerShell.HadErrors;
                        result._completed = true;
                    }
                    catch (RuntimeException exception)
                    {
                        error = exception.ErrorRecord;
                        result._output = new Collection<PSObject>();
                        result._hadErrors = true;
                    }
                    catch (ArgumentException exception)
                    {
                        error = new ErrorRecord(exception, "UnexpectedInvalidArgument", ErrorCategory.InvalidArgument, script);
                        result._output = new Collection<PSObject>();
                        result._hadErrors = true;
                    }
                    catch (Exception exception)
                    {
                        error = new ErrorRecord(exception, "UnexpectedError", ErrorCategory.NotSpecified, script);
                        result._output = new Collection<PSObject>();
                        result._hadErrors = true;
                    }
                    result._errors = powerShell.Streams.Error.ReadAll();
                    if (error != null)
                        result._errors.Add(error);
                    result._warnings = powerShell.Streams.Warning.ReadAll();
                    result._verbose = powerShell.Streams.Verbose.ReadAll();
                    result._debug = powerShell.Streams.Debug.ReadAll();
                }
                foreach (string key in result._variables.Keys)
                    result._variables[key] = runspace.SessionStateProxy.GetVariable(key);

                if (invocationParams.This != null)
                    result._variables.Remove("this");
                return result;
            }
        }

        public static PSInvocationResult GetResult(ScriptBlock scriptBlock, PSInvocationParams invocationParams, params object[] args)
        {
            if (scriptBlock == null)
                throw new ArgumentNullException("scriptBlock");
            return GetResult(scriptBlock.ToString(), invocationParams, args);
        }
    }
}
