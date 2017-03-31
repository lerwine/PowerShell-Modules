using System;
using System.Collections;
using System.Collections.Generic;
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
    /// A specialized context for invoking a <seealso cref="ScriptBlock"/> which will handle an event.
    /// </summary>
    public class PSEventScriptHandler<TEventArgs> : IPSEventScriptHandler
        where TEventArgs : EventArgs
    {
        PSInvocationContext _parentContext;
		private ScriptBlock _handlerScript;
		private string _name;
        private PSHost _host = null;
        private string _initialLocation = "";
        private bool? _useLocalScope = null;
        private ApartmentState? _apartmentState = null;
        private PSThreadOptions? _threadOptions = null;
        private PSObject _this = new PSObject();
        private RunspaceConfiguration _configuration = null;
		private Hashtable _variables = new Hashtable();

        private event EventHandler<PSInvocationEventHandlerInvokedArgs> _eventHandlerInvoked;

        event EventHandler<PSInvocationEventHandlerInvokedArgs> IPSEventScriptHandler.EventHandlerInvoked
        {
            add { _eventHandlerInvoked += value; }
            remove { _eventHandlerInvoked -= value; }
        }

        /// <summary>
        /// This gets raised after <see cref="EventHandler(object, TEventArgs)"/> is invoked and <see cref="HandlerScript"/> handles the event.
        /// </summary>
        public event EventHandler<PSInvocationEventHandlerInvokedArgs<TEventArgs>> EventHandlerInvoked;

        /// <summary>
        /// <seealso cref="ScriptBlock"/> which gets invoked when <see cref="EventHandler(object, TEventArgs)"/> is invoked.
        /// </summary>
        public ScriptBlock HandlerScript { get { return _handlerScript; } }

        /// <summary>
        /// Arbitrary name to associate with events handled by this object.
        /// </summary>
        public string Name { get { return _name; } }

        /// <summary>
        /// PowerShell host to use when invoking <see cref="HandlerScript"/>.
        /// </summary>
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

        /// <summary>
        /// Whether or not to use the local scope when invoking <see cref="HandlerScript"/>.
        /// </summary>
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

        /// <summary>
        /// Specifies the apartment state to use when invoking <see cref="HandlerScript"/>.
        /// </summary>
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

        /// <summary>
        /// PowerShell threading options for invoking <see cref="HandlerScript"/>.
        /// </summary>
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

        /// <summary>
        /// Runspace configuration to use when invoking <see cref="HandlerScript"/>.
        /// </summary>
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
        public Hashtable Variables { get { return _variables; } }

        /// <summary>
        /// Data which is synchronized with the parent context.
        /// </summary>
        public Hashtable SynchronizedData { get { return _parentContext.SynchronizedData; } }

        /// <summary>
        /// The object which will serve as the &quot;$this&quot; variable during script execution.
        /// </summary>
        public PSObject This
        {
            get { return _this; }
            set { _this = (value == null) ? new PSObject() : value; }
        }

        /// <summary>
        /// Initialize new <see cref="PSEventScriptHandler{TEventArgs}"/> object for handling <seealso cref="EventHandler{TEventArgs}"/> events.
        /// </summary>
        /// <param name="name">Arbitrary name to associate with events handled by this object.</param>
        /// <param name="handlerScript"><seealso cref="ScriptBlock"/> which handles <seealso cref="EventHandler{TEventArgs}"/> events.</param>
        /// <param name="parentContext">Parent <seealso cref="PSInvocationContext"/> object.</param>
        public PSEventScriptHandler(string name, ScriptBlock handlerScript, PSInvocationContext parentContext)
        {
            if (handlerScript == null)
                throw new ArgumentNullException("handlerScript");

            if (parentContext == null)
                throw new ArgumentNullException("parentContext");

            _name = name;
            _handlerScript = handlerScript;
            _parentContext = parentContext;
            Configuration = RunspaceConfiguration.Create();
            _variables = new Hashtable();
        }

        /// <summary>
        /// Method which is intended for handling source <seealso cref="EventHandler{TEventArgs}"/> events.
        /// </summary>
        /// <param name="sender">Object which raised the event.</param>
        /// <param name="e">Information about the event.</param>
        public void EventHandler(object sender, TEventArgs e)
        {
#if PSLEGACY
			object[] variableKeys = LinqEmul.ToArray<object>(LinqEmul.Cast<object>(Variables.Keys));
#else
            object[] variableKeys = Variables.Keys.Cast<object>().ToArray();
#endif
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
                ((configuration == null) ? RunspaceFactory.CreateRunspace() : RunspaceFactory.CreateRunspace(configuration)) :
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

        /// <summary>
        /// Raises the <see cref="EventHandlerInvoked"/> event.
        /// </summary>
        /// <param name="sender">Object which is the source of the original event.</param>
        /// <param name="args">Information about the original event.</param>
        /// <param name="invocationResult">Results of handling the event.</param>
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

        /// <summary>
        /// This gets invoked after <see cref="HandlerScript"/> has handled the event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnEventHandlerInvoked(PSInvocationEventHandlerInvokedArgs<TEventArgs> e) { }
    }
}
