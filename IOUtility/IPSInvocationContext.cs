using System;
using System.Collections;
using System.Management.Automation;
using System.Management.Automation.Host;
using System.Management.Automation.Runspaces;
using System.Threading;

namespace IOUtility
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
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
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
