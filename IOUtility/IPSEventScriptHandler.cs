using System;
using System.Management.Automation;

namespace IOUtility
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public interface IPSEventScriptHandler : IPSInvocationContext
    {
        event EventHandler<PSInvocationEventHandlerInvokedArgs> EventHandlerInvoked;
        ScriptBlock HandlerScript { get; }
        string Name { get; }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
