using System;
using System.Management.Automation;

namespace WpfCLR.PSInvocation
{
    public interface IEventScriptHandler : IContext
    {
        event EventHandler<EventHandlerInvokedArgs> EventHandlerInvoked;
        ScriptBlock HandlerScript { get; }
        string Name { get; }
    }
}