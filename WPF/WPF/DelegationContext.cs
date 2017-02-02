using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Host;
using System.Management.Automation.Runspaces;
using System.Threading;
using System.Xml;

namespace WpfCLR
{
    /// <summary>
    /// Base class for PowerShell script delegation.
    /// </summary>
    public abstract class DelegationContext
    {
        #region Fields

        /// <summary>
        /// Occurs when the associated delegate <seealso cref="ScriptBlock"/> is invoked.
        /// </summary>
        public event EventHandler<DelegateInvokedEventArgs> DelegateInvoked;

        private ScriptBlock _handler;
        private object _thisObject = null;
        PSHost _host;
        private Collection<PSAttachedEventInstance> _innerAttachedEvents = new Collection<PSAttachedEventInstance>();
        private ReadOnlyCollection<PSAttachedEventInstance> _attachedEvents;
        private ApartmentState _apartmentState;
        private PSThreadOptions _threadOptions;

        #endregion

        #region Properties

        /// <summary>
        /// List of attached event delegates.
        /// </summary>
        public ReadOnlyCollection<PSAttachedEventInstance> AttachedEvents { get { return _attachedEvents; } }

        /// <summary>
        /// Apartment state to use when invoking the delegate <seealso cref="ScriptBlock"/>.
        /// </summary>
        public ApartmentState ApartmentState { get { return _apartmentState; } }

        /// <summary>
        /// Thread options to use when invoking the delegate <seealso cref="ScriptBlock"/>.
        /// </summary>
        public PSThreadOptions ThreadOptions { get { return _threadOptions; } }

        /// <summary>
        /// Object which becomes the &quot;$this&quot; variable when the delegate <seealso cref="ScriptBlock"/> is invoked.
        /// </summary>
        public object ThisObject { get { return _thisObject; } }

        #endregion

        #region Constructors

        /// <summary>
        /// Initialize new <see cref="DelegationContext"/> object.
        /// </summary>
        /// <param name="handler"><seealso cref="ScriptBlock"/> which will handle the delegate invocation.</param>
        /// <param name="thisObject">Object which becomes the &quot;$this&quot; variable when the <paramref name="handler"/> is invoked.</param>
        /// <param name="host">PowerShell host to use.</param>
        /// <param name="apartmentState">Apartment state to use when invoking <paramref name="handler"/>.</param>
        /// <param name="threadOptions">Thread options to use when invoking <paramref name="handler"/>.</param>
        protected DelegationContext(ScriptBlock handler, object thisObject, PSHost host, ApartmentState apartmentState, PSThreadOptions threadOptions)
        {
            if (handler == null)
                throw new ArgumentNullException("handler");
            _handler = handler;
            _thisObject = thisObject;
            _host = host;
            _attachedEvents = new ReadOnlyCollection<PSAttachedEventInstance>(_innerAttachedEvents);
            _apartmentState = apartmentState;
            _threadOptions = threadOptions;
        }

        /// <summary>
        /// Initialize new <see cref="DelegationContext"/> object.
        /// </summary>
        /// <param name="handler"><seealso cref="ScriptBlock"/> which will handle the delegate invocation.</param>
        /// <param name="thisObject">Object which becomes the &quot;$this&quot; variable when the <paramref name="handler"/> is invoked.</param>
        /// <param name="host">PowerShell host to use.</param>
        protected DelegationContext(ScriptBlock handler, object thisObject, PSHost host) : this(handler, thisObject, host, ApartmentState.STA, PSThreadOptions.ReuseThread) { }

        /// <summary>
        /// Initialize new <see cref="DelegationContext"/> object.
        /// </summary>
        /// <param name="handler"><seealso cref="ScriptBlock"/> which will handle the delegate invocation.</param>
        /// <param name="thisObject">Object which becomes the &quot;$this&quot; variable when the <paramref name="handler"/> is invoked.</param>
        /// <param name="apartmentState">Apartment state to use when invoking <paramref name="handler"/>.</param>
        /// <param name="threadOptions">Thread options to use when invoking <paramref name="handler"/>.</param>
        protected DelegationContext(ScriptBlock handler, object thisObject, ApartmentState apartmentState, PSThreadOptions threadOptions) : this(handler, thisObject, null, apartmentState, threadOptions) { }

        /// <summary>
        /// Initialize new <see cref="DelegationContext"/> object.
        /// </summary>
        /// <param name="handler"><seealso cref="ScriptBlock"/> which will handle the delegate invocation.</param>
        /// <param name="thisObject">Object which becomes the &quot;$this&quot; variable when the <paramref name="handler"/> is invoked.</param>
        protected DelegationContext(ScriptBlock handler, object thisObject) : this(handler, thisObject, ApartmentState.STA, PSThreadOptions.ReuseThread) { }

        #endregion

        #region Methods

        /// <summary>
        /// Raises the <see cref="DelegateInvoked"/> event.
        /// </summary>
        /// <param name="args">Arguments containing results of the <see cref="ScriptBlock"/> that was invoked as a delegate.</param>
        protected void RaiseDelegateInvoked(DelegateInvokedEventArgs args)
        {
            EventHandler<DelegateInvokedEventArgs> handler = DelegateInvoked;
            if (handler != null)
                handler(this, args);
        }

        /// <summary>
        /// Attaches a object instance event.
        /// </summary>
        /// <param name="eventSource">Object which is the source of the event.</param>
        /// <param name="eventName">Name of the event.</param>
        public void AttachInstanceEvent(object eventSource, string eventName)
        {
            AttachInstanceEvent(new PSAttachedEventInstance(CreateHandlerDelegate(), eventSource, eventName));
        }

        /// <summary>
        /// Attaches a static event.
        /// </summary>
        /// <param name="sourceType">Type of object which is the source of the event.</param>
        /// <param name="eventName">Name of the event.</param>
        public void AttachStaticEvent(Type sourceType, string eventName)
        {
            AttachInstanceEvent(new PSAttachedEventInstance(CreateHandlerDelegate(), eventName, sourceType));
        }
        
        private void AttachInstanceEvent(PSAttachedEventInstance instance)
        {
            instance.OnDetached += Instance_OnDetached;
            _innerAttachedEvents.Add(instance);
        }
        private void Instance_OnDetached(object sender, EventArgs e)
        {
            _innerAttachedEvents.Remove(sender as PSAttachedEventInstance);
        }

        /// <summary>
        /// Create delegate method to handle event.
        /// </summary>
        /// <returns><seealso cref="Delegate"/> which will handle the event.</returns>
        protected abstract Delegate CreateHandlerDelegate();

        /// <summary>
        /// Invokes the script block.
        /// </summary>
        /// <param name="args">Arguments to pass to the scriptblock.</param>
        /// <returns>Output results from <seealso cref="ScriptBlock"/> invocation.</returns>
        protected DelegateInvokedEventArgs InvokeScriptBlock(params object[] args)
        {
            using (Runspace runspace = ((_host == null) ? RunspaceFactory.CreateRunspace() : RunspaceFactory.CreateRunspace(_host)))
            {
                runspace.ApartmentState = _apartmentState;
                runspace.ThreadOptions = _threadOptions;
                runspace.Open();
                using (PowerShell powerShell = PowerShell.Create())
                {
                    powerShell.Runspace = runspace;
                    runspace.SessionStateProxy.SetVariable("this", _thisObject);
                    powerShell.AddCommand("Add-Type").AddParameter("Path", new string[]
                    {
                        (typeof(XmlDocument)).Assembly.Location,
                        (typeof(System.Windows.Window)).Assembly.Location,
                        (typeof(System.Windows.UIElement)).Assembly.Location,
                        (typeof(System.Windows.DependencyObject)).Assembly.Location,
                        this.GetType().Assembly.Location
                    });
                    PowerShell command = powerShell.AddScript(_handler.ToString());
                    if (args != null)
                    {
                        foreach (object a in args)
                            command.AddArgument(a);
                    }
                    Collection<PSObject> result = powerShell.Invoke();
                    DelegateInvokedEventArgs e = new DelegateInvokedEventArgs(powerShell.Streams);
                    e.AppendOutput(result);
                    return e;
                }
            }
        }

        /// <summary>
        /// Get attached events for a specific name and event source instance.
        /// </summary>
        /// <param name="eventName">Name of event.</param>
        /// <param name="eventSource">Source instance of event.</param>
        /// <returns>Collection of objects which represent attached events.</returns>
        public IEnumerable<PSAttachedEventInstance> GetInstanceEvents(string eventName, object eventSource)
        {
            foreach (PSAttachedEventInstance instance in _innerAttachedEvents)
            {
                if (instance.EventInfo.Name == eventName && instance.SourceEquals(eventSource))
                    yield return instance;
            }
        }

        /// <summary>
        /// Get attached static events of a specific name and source type.
        /// </summary>
        /// <param name="eventName">Name of event.</param>
        /// <param name="sourceType">Object type of event source.</param>
        /// <returns>Collection of objects which represent attached events.</returns>
        public IEnumerable<PSAttachedEventInstance> GetStaticEvents(string eventName, Type sourceType)
        {
            foreach (PSAttachedEventInstance instance in _innerAttachedEvents)
            {
                if (instance.EventInfo.Name == eventName && !instance.IsInstance && instance.SourceType.Equals(sourceType))
                    yield return instance;
            }
        }

        /// <summary>
        /// Get all events of a specific name and source type.
        /// </summary>
        /// <param name="eventName">Name of event.</param>
        /// <param name="sourceType">Object type of event source.</param>
        /// <returns>Collection of objects which represent attached events.</returns>
        public IEnumerable<PSAttachedEventInstance> GetEvents(string eventName, Type sourceType)
        {
            foreach (PSAttachedEventInstance instance in _innerAttachedEvents)
            {
                if (instance.EventInfo.Name == eventName && instance.SourceType.Equals(sourceType))
                    yield return instance;
            }
        }

        /// <summary>
        /// Get all events of a specific name.
        /// </summary>
        /// <param name="eventName">Name of event.</param>
        /// <returns>Collection of objects which represent attached events.</returns>
        public IEnumerable<PSAttachedEventInstance> GetEvents(string eventName)
        {
            foreach (PSAttachedEventInstance instance in _innerAttachedEvents)
            {
                if (instance.EventInfo.Name == eventName)
                    yield return instance;
            }
        }

        /// <summary>
        /// Detach events by name and source instance.
        /// </summary>
        /// <param name="eventName">Name of event.</param>
        /// <param name="eventSource">Source instance of event.</param>
        /// <returns>Collection of instances whose events were detached.</returns>
        public Collection<object> DetachInstanceEvents(string eventName, object eventSource) { return DetachEvents(GetInstanceEvents(eventName, eventSource)); }

        /// <summary>
        /// Detach static events by name and source type.
        /// </summary>
        /// <param name="eventName">Name of event.</param>
        /// <param name="sourceType">Object type of event source.</param>
        /// <returns>Collection of type names whose events were detached.</returns>
        public Collection<object> DetachStaticEvents(string eventName, Type sourceType) { return DetachEvents(GetStaticEvents(eventName, sourceType)); }

        /// <summary>
        /// Detach events by name and source type.
        /// </summary>
        /// <param name="eventName">Name of event.</param>
        /// <param name="sourceType">Object type of event source.</param>
        /// <returns>Collection of instances and type names whose events were detached.</returns>
        public Collection<object> DetachEvents(string eventName, Type sourceType) { return DetachEvents(GetEvents(eventName, sourceType)); }

        /// <summary>
        /// Detach events by name.
        /// </summary>
        /// <param name="eventName">Name of event.</param>
        /// <returns>Collection of instances and type names whose events were detached.</returns>
        public Collection<object> DetachEvents(string eventName) { return DetachEvents(GetEvents(eventName)); }

        private Collection<object> DetachEvents(IEnumerable<PSAttachedEventInstance> collection)
        {
            Collection<object> result = new Collection<object>();
            List<PSAttachedEventInstance> toDetach = new List<PSAttachedEventInstance>(collection);
            foreach (PSAttachedEventInstance instance in toDetach)
                result.Add(instance.Detach());
            return result;
        }

        #endregion
    }
}