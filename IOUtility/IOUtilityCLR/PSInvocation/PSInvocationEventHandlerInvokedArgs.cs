using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Text;

namespace IOUtilityCLR.PSInvocation
{
    public abstract class PSInvocationEventHandlerInvokedArgs : EventArgs
    {
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
        public object Sender { get; private set; }
        public EventArgs EventArgs { get; private set; }
        public bool HadErrors { get; private set; }
        public bool RanToCompletion { get; private set; }
        public Collection<PSObject> Output { get; private set; }
        public Hashtable Variables { get; private set; }
        public Collection<ErrorRecord> Errors { get; private set; }
        public Collection<WarningRecord> Warnings { get; private set; }
        public Collection<VerboseRecord> Verbose { get; private set; }
        public Collection<DebugRecord> Debug { get; private set; }
    }
    public class PSInvocationEventHandlerInvokedArgs<TEventArgs> : PSInvocationEventHandlerInvokedArgs
        where TEventArgs : EventArgs
    {
        public PSInvocationEventHandlerInvokedArgs(object sender, TEventArgs eventArgs, PSInvocationResult invocationResult)
            : base(sender, eventArgs, invocationResult) { }
        public new TEventArgs EventArgs { get; private set; }
    }
}
