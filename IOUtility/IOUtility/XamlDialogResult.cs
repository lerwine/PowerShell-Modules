using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
#if !PSLEGACY
using System.Linq;
#endif
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Xml;

namespace IOUtilityCLR
{
    public class XamlDialogResult : PSInvocationResult
    {
        public XamlDialogResult(Collection<PSObject> collection, Runspace runspace, PSDataStreams streams, string[] variableNames, bool wasStopped, Hashtable synchronizedData)
            : base(collection, runspace, streams, variableNames, wasStopped, synchronizedData)

        {
        }

        private static XamlDialogResult Create(Func<Runspace> createRunspace, Func<Runspace, PowerShell> createPowerShell, IEnumerable<string> variableNames, Func<PowerShell, Collection<PSObject>> invoke, Hashtable synchronizedData)
        {
#if PSLEGACY
            string[] names = LinqEmul.ToArray<string>(LinqEmul.Where(variableNames, LinqEmul.StringNotNullOrEmpty));
#else
            string[] names = variableNames.Where(s => !String.IsNullOrEmpty(s)).ToArray();
#endif
            using (Runspace runspace = createRunspace())
            {
                using (PowerShell powershell = createPowerShell(runspace))
                    return new XamlDialogResult(invoke(powershell), runspace, powershell.Streams, names, false, synchronizedData);
            }
        }

        internal static new XamlDialogResult Create(Func<Runspace> createRunspace, Func<Runspace, PowerShell> createPowerShell, IEnumerable<string> variableNames, Hashtable synchronizedData)
        {
            return Create(createRunspace, createPowerShell, variableNames, ps => ps.Invoke(), synchronizedData);
        }

        internal static new XamlDialogResult Create(Func<Runspace> createRunspace, Func<Runspace, PowerShell> createPowerShell, IEnumerable input, IEnumerable<string> variableNames, Hashtable synchronizedData)
        {
            return Create(createRunspace, createPowerShell, variableNames, ps => ps.Invoke(input), synchronizedData);
        }

        internal static new XamlDialogResult Create(Func<Runspace> createRunspace, Func<Runspace, PowerShell> createPowerShell, IEnumerable input, PSInvocationSettings settings, IEnumerable<string> variableNames, Hashtable synchronizedData)
        {
            return Create(createRunspace, createPowerShell, variableNames, ps => ps.Invoke(input, settings), synchronizedData);
        }
    }
}