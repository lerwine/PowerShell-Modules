using System.Collections;
using System.Collections.ObjectModel;
using System.Management.Automation;

namespace WpfCLR.PSInvocation
{
    public interface IInvocationResult
    {
        bool HadErrors { get; }
        bool RanToCompletion { get; }
        Collection<PSObject> Output { get; }
        Hashtable Variables { get; }
        Collection<ErrorRecord> Errors { get; }
        Collection<WarningRecord> Warnings { get; }
        Collection<VerboseRecord> Verbose { get; }
        Collection<DebugRecord> Debug { get; }
    }
}