using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Host;
using System.Management.Automation.Runspaces;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WpfCLR.PSInvocation
{
    public class Context
    {
        private PSHost _host = null;
        private string _initialLocation = "";
        private bool? _useLocalScope = null;
        private ApartmentState? _apartmentState = null;
        private PSThreadOptions? _threadOptions = null;
        private object[] _variableKeys = null;
        private PSObject _this = new PSObject();

        public PSHost Host
        {
            get { return _host; }
            set
            {
                if (_variableKeys != null)
                    throw new InvalidOperationException();
                _host = value;
            }
        }

        public string InitialLocation
        {
            get { return _initialLocation; }
            set
            {
                if (_variableKeys != null)
                    throw new InvalidOperationException();
                _initialLocation = (value == null) ? "" : value;
            }
        }

        public bool? UseLocalScope
        {
            get { return _useLocalScope; }
            set
            {
                if (_variableKeys != null)
                    throw new InvalidOperationException();
                _useLocalScope = value;
            }
        }

        public ApartmentState? ApartmentState
        {
            get { return _apartmentState; }
            set
            {
                if (_variableKeys != null)
                    throw new InvalidOperationException();
                _apartmentState = value;
            }
        }

        public PSThreadOptions? ThreadOptions
        {
            get { return _threadOptions; }
            set
            {
                if (_variableKeys != null)
                    throw new InvalidOperationException();
                _threadOptions = value;
            }
        }

        public RunspaceConfiguration Configuration { get; private set; }
        public Hashtable Variables { get; private set; }
        public PSObject This
        {
            get { return _this; }
            set
            {
                if (_variableKeys != null)
                    throw new InvalidOperationException();
                _this = (value == null) ? new PSObject() : value;
            }
        }

        public Context()
        {
            Configuration = RunspaceConfiguration.Create();
            Variables = new Hashtable();
        }

        public InvocationResult GetResult(string script)
        {
            if (script == null)
                throw new ArgumentNullException("script");

            if (_variableKeys != null)
                throw new InvalidOperationException();

            _variableKeys = Variables.Keys.Cast<object>().ToArray();
            try
            {
                using (Runspace runspace = (Host == null) ?
                    ((Configuration == null) ? RunspaceFactory.CreateRunspace() : RunspaceFactory.CreateRunspace(Host)) :
                    ((Configuration == null) ? RunspaceFactory.CreateRunspace(Host) : RunspaceFactory.CreateRunspace(Host, Configuration)))
                {
                    if (ApartmentState.HasValue)
                        runspace.ApartmentState = ApartmentState.Value;
                    if (ThreadOptions.HasValue)
                        runspace.ThreadOptions = ThreadOptions.Value;
                    runspace.Open();

                    foreach (string key in _variableKeys)
                    {
                        if (Variables[key] != null && (!(key is string) || String.Compare(key as string, "this", true) != 0))
                            runspace.SessionStateProxy.SetVariable(key, Variables[key]);
                    }
                    runspace.SessionStateProxy.SetVariable("this", This);

                    if (InitialLocation.Length > 0)
                        runspace.SessionStateProxy.Path.SetLocation(InitialLocation);
                    
                    using (PowerShell powerShell = PowerShell.Create())
                    {
                        powerShell.Runspace = runspace;
                        return new InvocationResult(script, this, powerShell, _variableKeys);
                    }
                }
            }
            finally { _variableKeys = null; }
        }

        public InvocationResult GetResult(ScriptBlock script)
        {
            if (script == null)
                throw new ArgumentNullException("script");

            return GetResult(script.ToString());
        }
    }
}
