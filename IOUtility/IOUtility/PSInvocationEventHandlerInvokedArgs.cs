using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Management.Automation;

namespace IOUtilityCLR
{
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
		
        public object Sender { get { return _sender; } }
        public EventArgs EventArgs { get { return _eventArgs; } }
        public bool HadErrors { get { return _hadErrors; } }
        public bool RanToCompletion { get { return _ranToCompletion; } }
        public Collection<PSObject> Output { get { return _output; } }
        public Hashtable Variables { get { return _variables; } }
        public Collection<ErrorRecord> Errors { get { return _errors; } }
        public Collection<WarningRecord> Warnings { get { return _warnings; } }
        public Collection<VerboseRecord> Verbose { get { return _verbose; } }
        public Collection<DebugRecord> Debug { get { return _debug; } }
    }
    public class PSInvocationEventHandlerInvokedArgs<TEventArgs> : PSInvocationEventHandlerInvokedArgs
        where TEventArgs : EventArgs
    {
        public PSInvocationEventHandlerInvokedArgs(object sender, TEventArgs eventArgs, PSInvocationResult invocationResult)
            : base(sender, eventArgs, invocationResult) { }
        public new TEventArgs EventArgs { get { return base.EventArgs as TEventArgs; } }
    }
}
