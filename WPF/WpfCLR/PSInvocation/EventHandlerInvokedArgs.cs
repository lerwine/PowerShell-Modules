using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Management.Automation;

namespace WpfCLR.PSInvocation
{
    public abstract class EventHandlerInvokedArgs : EventArgs
    {
        protected EventHandlerInvokedArgs(object sender, EventArgs eventArgs, InvocationResult invocationResult)
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
    public class EventHandlerInvokedArgs<TEventArgs> : EventHandlerInvokedArgs
        where TEventArgs : EventArgs
    {
        public EventHandlerInvokedArgs(object sender, TEventArgs eventArgs, InvocationResult invocationResult)
            : base(sender, eventArgs, invocationResult) { }
        public new TEventArgs EventArgs { get; private set; }
    }
}