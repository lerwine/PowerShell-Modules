using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Management.Automation;

namespace IOUtilityCLR
{
    /// <summary>
    /// Base abstract event information class for accessing base <seealso cref="EventArgs"/> object.
    /// </summary>
    public abstract class PSInvocationEventHandlerInvokedArgs : EventArgs
    {
        private object _sender;
        private EventArgs _eventArgs;
        private bool _hadErrors;
        private bool _ranToCompletion;
        private Collection<PSObject> _output;
        private Hashtable _variables;
        private Collection<ErrorRecord> _errors;
        private Collection<WarningRecord> _warnings;
        private Collection<VerboseRecord> _verbose;
        private Collection<DebugRecord> _debug;
		
        /// <summary>
        /// Initialize new <see cref="PSInvocationEventHandlerInvokedArgs"/> object.
        /// </summary>
        /// <param name="sender">Object which was the source of the event.</param>
        /// <param name="eventArgs">Information about the event.</param>
        /// <param name="invocationResult">Result from the event handler.</param>
        protected PSInvocationEventHandlerInvokedArgs(object sender, EventArgs eventArgs, PSInvocationResult invocationResult)
        {
            _sender = sender;
            _eventArgs = eventArgs;
            _ranToCompletion = invocationResult.RanToCompletion;
            _hadErrors = invocationResult.HadErrors;
            _output = invocationResult.Output;
            _variables = invocationResult.Variables;
            _errors = invocationResult.Errors;
            _warnings = invocationResult.Warnings;
            _verbose = invocationResult.Verbose;
            _debug = invocationResult.Debug;
        }
		
        /// <summary>
        /// Object which raised the original event.
        /// </summary>
        public object Sender { get { return _sender; } }

        /// <summary>
        /// Information about the source event which was handled by the event handler script.
        /// </summary>
        public EventArgs EventArgs { get { return _eventArgs; } }

        /// <summary>
        /// True if errors were encountered while handling the event.
        /// </summary>
        public bool HadErrors { get { return _hadErrors; } }

        /// <summary>
        /// True if event handling script ran to completion.
        /// </summary>
        public bool RanToCompletion { get { return _ranToCompletion; } } 

        /// <summary>
        /// Output returned from event handler script.
        /// </summary>}
        public Collection<PSObject> Output { get { return _output; } }

        /// <summary>
        /// Values of variables after event handler script completion.
        /// </summary>
        public Hashtable Variables { get { return _variables; } }

        /// <summary>
        /// Errors which occurred during event handler script execution.
        /// </summary>
        public Collection<ErrorRecord> Errors { get { return _errors; } }

        /// <summary>
        /// Warnings which occurred during event handler script execution.
        /// </summary>
        public Collection<WarningRecord> Warnings { get { return _warnings; } }

        /// <summary>
        /// Verbose messages generated during event handler script execution.
        /// </summary>
        public Collection<VerboseRecord> Verbose { get { return _verbose; } }

        /// <summary>
        /// Debug messages generated during event handler script execution.
        /// </summary>
        public Collection<DebugRecord> Debug { get { return _debug; } }
    }

    /// <summary>
    /// Contains information about the result of a script-handled event.
    /// </summary>
    /// <typeparam name="TEventArgs">Type of arguments passed from the source event, to the event handler script.</typeparam>
    public class PSInvocationEventHandlerInvokedArgs<TEventArgs> : PSInvocationEventHandlerInvokedArgs
        where TEventArgs : EventArgs
    {
        /// <summary>
        /// Initialize new <see cref="PSInvocationEventHandlerInvokedArgs{TEventArgs}"/> object.
        /// </summary>
        /// <param name="sender">Object which was the source of the event.</param>
        /// <param name="eventArgs">Information about the event.</param>
        /// <param name="invocationResult">Result from the event handler.</param>
        public PSInvocationEventHandlerInvokedArgs(object sender, TEventArgs eventArgs, PSInvocationResult invocationResult)
            : base(sender, eventArgs, invocationResult) { }

        /// <summary>
        /// Information about the source event which was handled by the event handler script.
        /// </summary>
        public new TEventArgs EventArgs { get { return base.EventArgs as TEventArgs; } }
    }
}
