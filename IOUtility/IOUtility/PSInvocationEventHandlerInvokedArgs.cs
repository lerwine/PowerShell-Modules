using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Text;

namespace IOUtilityCLR
{
    /// <summary>
    /// Base abstract event information class for accessing base <seealso cref="EventArgs"/> object.
    /// </summary>
    public abstract class PSInvocationEventHandlerInvokedArgs : EventArgs
    {
        /// <summary>
        /// Initialize new <see cref="PSInvocationEventHandlerInvokedArgs"/> object.
        /// </summary>
        /// <param name="sender">Object which was the source of the event.</param>
        /// <param name="eventArgs">Information about the event.</param>
        /// <param name="invocationResult">Result from the event handler.</param>
        protected PSInvocationEventHandlerInvokedArgs(object sender, EventArgs eventArgs, PSInvocationResult invocationResult)
        {
            Sender = sender;
            EventArgs = eventArgs;
            RanToCompletion = invocationResult.RanToCompletion;
            HadErrors = invocationResult.HadErrors;
            Output = invocationResult.Output;
            Variables = invocationResult.Variables;
            Errors = invocationResult.Errors;
            Warnings = invocationResult.Warnings;
            Verbose = invocationResult.Verbose;
            Debug = invocationResult.Debug;
        }

        /// <summary>
        /// Object which raised the original event.
        /// </summary>
        public object Sender { get; private set; }

        /// <summary>
        /// Information about the source event which was handled by the event handler script.
        /// </summary>
        public EventArgs EventArgs { get; private set; }

        /// <summary>
        /// True if errors were encountered while handling the event.
        /// </summary>
        public bool HadErrors { get; private set; }

        /// <summary>
        /// True if event handling script ran to completion.
        /// </summary>
        public bool RanToCompletion { get; private set; }

        /// <summary>
        /// Output returned from event handler script.
        /// </summary>
        public Collection<PSObject> Output { get; private set; }

        /// <summary>
        /// Values of variables after event handler script completion.
        /// </summary>
        public Hashtable Variables { get; private set; }

        /// <summary>
        /// Errors which occurred during event handler script execution.
        /// </summary>
        public Collection<ErrorRecord> Errors { get; private set; }

        /// <summary>
        /// Warnings which occurred during event handler script execution.
        /// </summary>
        public Collection<WarningRecord> Warnings { get; private set; }

        /// <summary>
        /// Verbose messages generated during event handler script execution.
        /// </summary>
        public Collection<VerboseRecord> Verbose { get; private set; }

        /// <summary>
        /// Debug messages generated during event handler script execution.
        /// </summary>
        public Collection<DebugRecord> Debug { get; private set; }
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
        public new TEventArgs EventArgs { get; private set; }
    }
}
