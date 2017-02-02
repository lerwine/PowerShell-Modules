using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
#if !PSLEGACY2
using System.Linq;
#endif
using System.Management.Automation;
using System.Management.Automation.Host;
using System.Management.Automation.Runspaces;
using System.Text;
using System.Threading;

namespace IOUtilityCLR
{
    /// <summary>
    /// Context under which ScriptBlock will be executed.
    /// </summary>
    public class PSInvocationContext : IPSInvocationContext
    {
        private PSHost _host = null;
        private string _initialLocation = "";
        private bool? _useLocalScope = null;
        private ApartmentState? _apartmentState = null;
        private PSThreadOptions? _threadOptions = null;
        private object[] _variableKeys = null;
        private PSObject _this = new PSObject();
        private Collection<IPSEventScriptHandler> _eventHandlers = new Collection<IPSEventScriptHandler>();
        private Collection<PSInvocationEventResult> _eventHandlerResults = new Collection<PSInvocationEventResult>();
        private object _syncRoot = new object();
		private Hashtable _variables = new Hashtable();
        private RunspaceConfiguration _configuration = null;
		private Hashtable _synchronizedData;
		
        public void AddEventHandler(IPSEventScriptHandler handler)
        {
            lock (_syncRoot)
            {
                if (!_eventHandlers.Contains(handler))
                {
                    _eventHandlers.Add(handler);
                    handler.EventHandlerInvoked += Handler_EventHandlerInvoked;
                }
            }
        }

        public void RemoveEventHandler(IPSEventScriptHandler handler)
        {
            lock (_syncRoot)
            {
                if (_eventHandlers.Contains(handler))
                {
                    handler.EventHandlerInvoked -= Handler_EventHandlerInvoked;
                    _eventHandlers.Remove(handler);
                }
            }
        }

        private void Handler_EventHandlerInvoked(object sender, PSInvocationEventHandlerInvokedArgs e)
        {
            IPSEventScriptHandler handler = sender as IPSEventScriptHandler;

            lock (_syncRoot)
                _eventHandlerResults.Add(new PSInvocationEventResult((handler == null) ? null : handler.Name, e));
        }

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

        /// <summary>
        /// Path representing initial location to be represented by the current Context.
        /// </summary>
        /// <remarks>If an invoked script changes the location, this variable will not be updated.</remarks>
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

        public RunspaceConfiguration Configuration
		{
			get { return _configuration; }
			set
            {
                if (_variableKeys != null)
                    throw new InvalidOperationException();
                _configuration = value;
            }
		}

        /// <summary>
        /// Other variables to define when invoking a script.
        /// </summary>
        /// <remarks>After a script is executed, the values in this property will not be updated. Instead, the values of these variables wil be represnted in the resulting InvocationResult object.</remarks>
        public Hashtable Variables { get { return _variables; } }

        /// <summary>
        /// Data which is synchronized with all invocations and event handlers.
        /// </summary>
        public Hashtable SynchronizedData { get { return _synchronizedData; } }

        /// <summary>
        /// The object which will serve as the "this" variable during script execution.
        /// </summary>
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

        public Collection<PSInvocationEventResult> EventHandlerResults { get { return _eventHandlerResults; } }
		
        /// <summary>
        /// Initialize new Context object.
        /// </summary>
        public PSInvocationContext()
        {
            _synchronizedData = Hashtable.Synchronized(new Hashtable());
        }

        /// <summary>
        /// Asynchronously executes a script using the current Context.
        /// </summary>
        /// <param name="script">Script to execute.</param>
        /// <returns>An InvocationResult object representing the results of the execution.</returns>
        public PSInvocationResult GetResult(string script)
        {
            if (script == null)
                throw new ArgumentNullException("script");

            if (_variableKeys != null)
                throw new InvalidOperationException();

#if PSLEGACY2
			_variableKeys = LinqEmul.ToArray<object>(LinqEmul.Cast<object>(Variables.Keys));
#else
            _variableKeys = Variables.Keys.Cast<object>().ToArray();
#endif
            try
            {
                using (Runspace runspace = (Host == null) ?
                    ((_configuration == null) ? RunspaceFactory.CreateRunspace() : RunspaceFactory.CreateRunspace(_configuration)) :
                    ((_configuration == null) ? RunspaceFactory.CreateRunspace(Host) : RunspaceFactory.CreateRunspace(Host, _configuration)))
                {
                    if (ApartmentState.HasValue)
                        runspace.ApartmentState = ApartmentState.Value;
                    if (ThreadOptions.HasValue)
                        runspace.ThreadOptions = ThreadOptions.Value;
                    runspace.Open();

                    foreach (object key in _variableKeys)
                    {
                        string s = (key is string) ? key as string : key.ToString();
                        if (Variables[key] != null && String.Compare(s, "this", true) != 0 && String.Compare(s, "SynchronizedData", true) != 0)
                            runspace.SessionStateProxy.SetVariable(s, Variables[key]);
                    }
                    runspace.SessionStateProxy.SetVariable("this", This);
                    runspace.SessionStateProxy.SetVariable("SynchronizedData", SynchronizedData);
                    if (InitialLocation.Length > 0)
                        runspace.SessionStateProxy.Path.SetLocation(InitialLocation);

                    using (PowerShell powerShell = PowerShell.Create())
                    {
                        powerShell.Runspace = runspace;
                        return new PSInvocationResult(script, this, powerShell, _variableKeys);
                    }
                }
            }
            finally { _variableKeys = null; }
        }

        /// <summary>
        /// Asynchronously invokes a ScriptBlock using the current Context.
        /// </summary>
        /// <param name="script">ScriptBlock to execute.</param>
        /// <returns>An InvocationResult object representing the results of the execution.</returns>
        public PSInvocationResult GetResult(ScriptBlock script)
        {
            if (script == null)
                throw new ArgumentNullException("script");

            return GetResult(script.ToString());
        }
    }
}
