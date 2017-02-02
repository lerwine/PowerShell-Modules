using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Host;
using System.Management.Automation.Runspaces;
using System.Text;
using System.Threading;

namespace IOUtilityCLR.PSInvocation
{
    public interface IPSInvocationContext
    {
        PSHost Host { get; }
        string InitialLocation { get; }
        bool? UseLocalScope { get; }
        ApartmentState? ApartmentState { get; }
        PSThreadOptions? ThreadOptions { get; }
        RunspaceConfiguration Configuration { get; }
        Hashtable Variables { get; }
        Hashtable SynchronizedData { get; }
        PSObject This { get; }
    }
}
