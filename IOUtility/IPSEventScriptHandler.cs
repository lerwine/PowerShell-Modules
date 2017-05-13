using System;
using System.Management.Automation;

namespace IOUtility
{
    public interface IPSEventScriptHandler : IPSInvocationContext
    {
        event EventHandler<PSInvocationEventHandlerInvokedArgs> EventHandlerInvoked;
        ScriptBlock HandlerScript { get; }
        string Name { get; }
    }
}
