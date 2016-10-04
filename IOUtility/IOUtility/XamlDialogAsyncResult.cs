using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

namespace IOUtilityCLR
{
    public class XamlDialogAsyncResult : PSAsyncInvocationResult<XamlDialogResult>
    {
        public XamlDialogAsyncResult(IAsyncResult asyncResult, Runspace runspace, PowerShell powershell, string[] variableNames, Hashtable synchronizedData) : base(asyncResult, runspace, powershell, variableNames, synchronizedData)
        {
        }

        protected override XamlDialogResult CreateResult(Collection<PSObject> collection, Runspace runspace, PSDataStreams streams, string[] variableNames, bool stopInvoked, Hashtable synchronizedData)
        {
            throw new NotImplementedException();
        }

        internal static XamlDialogAsyncResult Create(Func<Runspace> createRunspace, Func<Runspace, PowerShell> createPowerShell, Dictionary<string, object>.KeyCollection keys, Hashtable synchronizedData)
        {
            throw new NotImplementedException();
        }
    }
}