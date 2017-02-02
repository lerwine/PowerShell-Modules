using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;

namespace IOUtilityCLR
{
    public interface IPSEventScriptHandler : IPSInvocationContext
    {
        event EventHandler<PSInvocationEventHandlerInvokedArgs> EventHandlerInvoked;
        ScriptBlock HandlerScript { get; }
        string Name { get; }
    }
}
