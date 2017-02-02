using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IOUtilityCLR.PSInvocation
{
    public class PSInvocationEventResult
    {
        public string Name { get; private set; }
        public PSInvocationEventHandlerInvokedArgs Args { get; private set; }
        public PSInvocationEventResult(string name, PSInvocationEventHandlerInvokedArgs args)
        {
            Name = name;
            Args = args;
        }
    }
}
