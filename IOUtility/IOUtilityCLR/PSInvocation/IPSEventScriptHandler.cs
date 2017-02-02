using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;

namespace IOUtilityCLR.PSInvocation
{
    public interface IPSEventScriptHandler : IPSInvocationContext
    {
        event EventHandler<PSInvocationEventHandlerInvokedArgs> EventHandlerInvoked;
        ScriptBlock HandlerScript { get; }
        string Name { get; }
    }
}
