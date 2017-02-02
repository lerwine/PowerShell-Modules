using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Host;
using System.Management.Automation.Runspaces;
using System.Text;
using System.Threading;

namespace IOUtilityCLR
{
    /// <summary>
    /// A specialized context for invoking a <seealso cref="ScriptBlock"/> which will handle an event.
    /// </summary>
    public class PSEventScriptHandler<TEventArgs> : IPSEventScriptHandler
        where TEventArgs : EventArgs
    {
        PSInvocationContext _parentContext;
        private PSHost _host = null;
        private string _initialLocation = "";
        private bool? _useLocalScope = null;
        private ApartmentState? _apartmentState = null;
        private PSThreadOptions? _threadOptions = null;
        private PSObject _this = new PSObject();
        private RunspaceConfiguration _configuration = null;

        private event EventHandler<PSInvocationEventHandlerInvokedArgs> _eventHandlerInvoked;

        event EventHandler<PSInvocationEventHandlerInvokedArgs> IPSEventScriptHandler.EventHandlerInvoked
        {
            add { _eventHandlerInvoked += value; }
            remove { _eventHandlerInvoked -= value; }
        }

        public event EventHandler<PSInvocationEventHandlerInvokedArgs<TEventArgs>> EventHandlerInvoked;

        /// <summary>
        /// ScriptBlock which gets invoked when the event is raised.
        /// </summary>
        public ScriptBlock HandlerScript { get; private set; }

        public string Name { get; private set; }

        public PSHost Host
        {
            get
            {
                PSHost host = _host;
                if (host == null)
                    host = _parentContext.Host;
                return host;
            }
            set { _host = value; }
        }

        /// <summary>
        /// Path representing initial location to be represented by the current Context.
        /// </summary>
        /// <remarks>If an invoked script changes the location, this variable will not be updated.</remarks>
        public string InitialLocation
        {
            get
            {
                string s = _initialLocation;
                if (s.Length == 0)
                    s = _parentContext.InitialLocation;
                return s;
            }
            set { _initialLocation = (value == null) ? "" : value; }
        }

        public bool? UseLocalScope
        {
            get
            {
                bool? b = _useLocalScope;
                if (b.HasValue)
                    return b;

                return _parentContext.UseLocalScope;
            }
            set { _useLocalScope = value; }
        }

        public ApartmentState? ApartmentState
        {
            get
            {
                ApartmentState? value = _apartmentState;
                if (value.HasValue)
                    return value;

                return _parentContext.ApartmentState;
            }
            set { _apartmentState = value; }
        }

        public PSThreadOptions? ThreadOptions
        {
            get
            {
                PSThreadOptions? value = _threadOptions;
                if (value.HasValue)
                    return value;

                return _parentContext.ThreadOptions;
            }
            set { _threadOptions = value; }
        }

        public RunspaceConfiguration Configuration
        {
            get
            {
                RunspaceConfiguration value = _configuration;
                if (value != null)
                    return value;

                return _parentContext.Configuration;
            }
            set { _configuration = value; }
        }

        /// <summary>
        /// Other variables to define when invoking a script.
        /// </summary>
        public Hashtable Variables { get; private set; }

        /// <summary>
        /// Data which is synchronized with the parent context.
        /// </summary>
        public Hashtable SynchronizedData { get { return _parentContext.SynchronizedData; } }

        /// <summary>
        /// The object which will serve as the "this" variable during script execution.
        /// </summary>
        public PSObject This
        {
            get { return _this; }
            set { _this = (value == null) ? new PSObject() : value; }
        }

        public PSEventScriptHandler(string name, ScriptBlock handlerScript, PSInvocationContext parentContext)
        {
            if (handlerScript == null)
                throw new ArgumentNullException("handlerScript");

            if (parentContext == null)
                throw new ArgumentNullException("parentContext");

            Name = name;
            HandlerScript = handlerScript;
            _parentContext = parentContext;
            Configuration = RunspaceConfiguration.Create();
            Variables = new Hashtable();
        }

        public void EventHandler(object sender, TEventArgs e)
        {
            object[] variableKeys = Variables.Keys.Cast<object>().ToArray();
            Dictionary<object, object> variables = new Dictionary<object, object>();
            IDictionaryEnumerator enumerator = Variables.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                    variables.Add(enumerator.Key, enumerator.Value);
            }
            finally
            {
                if (enumerator is IDisposable)
                    (enumerator as IDisposable).Dispose();
            }
            PSHost host = Host;
            RunspaceConfiguration configuration = Configuration;
            ApartmentState? apartmentState = ApartmentState;
            PSThreadOptions? threadOptions = ThreadOptions;
            string initialLocation = InitialLocation;

            using (Runspace runspace = (host == null) ?
                ((configuration == null) ? RunspaceFactory.CreateRunspace() : RunspaceFactory.CreateRunspace(host)) :
                ((configuration == null) ? RunspaceFactory.CreateRunspace(host) : RunspaceFactory.CreateRunspace(host, configuration)))
            {
                if (apartmentState.HasValue)
                    runspace.ApartmentState = apartmentState.Value;
                if (threadOptions.HasValue)
                    runspace.ThreadOptions = threadOptions.Value;
                runspace.Open();

                foreach (object key in variables.Keys)
                {
                    string s = (key is string) ? key as string : key.ToString();
                    if (Variables[key] != null && String.Compare(s, "this", true) != 0 && String.Compare(s, "SynchronizedData", true) != 0)
                        runspace.SessionStateProxy.SetVariable(s, Variables[key]);
                }

                runspace.SessionStateProxy.SetVariable("this", This);
                runspace.SessionStateProxy.SetVariable("SynchronizedData", SynchronizedData);

                if (initialLocation.Length > 0)
                    runspace.SessionStateProxy.Path.SetLocation(initialLocation);

                using (PowerShell powerShell = PowerShell.Create())
                {
                    powerShell.Runspace = runspace;
                    RaiseEventHandlerInvoked(sender, e, new PSInvocationResult(HandlerScript.ToString(), this, powerShell, variableKeys, sender, e));
                }
            }
        }

        protected void RaiseEventHandlerInvoked(object sender, TEventArgs args, PSInvocationResult invocationResult)
        {
            PSInvocationEventHandlerInvokedArgs<TEventArgs> e = new PSInvocationEventHandlerInvokedArgs<TEventArgs>(sender, args, invocationResult);
            try { OnEventHandlerInvoked(e); }
            finally
            {
                try
                {
                    EventHandler<PSInvocationEventHandlerInvokedArgs<TEventArgs>> eventHandlerInvoked = EventHandlerInvoked;
                    if (eventHandlerInvoked != null)
                        eventHandlerInvoked(this, e);
                }
                finally
                {
                    EventHandler<PSInvocationEventHandlerInvokedArgs> eventHandlerInvoked = _eventHandlerInvoked;
                    if (eventHandlerInvoked != null)
                        eventHandlerInvoked(this, e);
                }
            }
        }

        protected virtual void OnEventHandlerInvoked(PSInvocationEventHandlerInvokedArgs<TEventArgs> e) { }
    }
}
