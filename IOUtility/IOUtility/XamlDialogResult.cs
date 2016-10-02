using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Xml;

namespace IOUtilityCLR
{
    public class XamlDialogResult : PSInvocationResult
    {
#if USEXLINQ
        public XamlDialogResult(Collection<PSObject> collection, Runspace runspace, PSDataStreams streams, string[] variableNames, bool wasStopped, Hashtable synchronizedData)
#else
        public XamlDialogResult(Collection<PSObject> collection, Runspace runspace, PSDataStreams streams, string[] variableNames, bool wasStopped, Hashtable synchronizedData)
#endif
            : base(collection, runspace, streams, variableNames, wasStopped, synchronizedData)

        {
        }

#if USEXLINQ
        private static XamlDialogResult Create(Func<Runspace> createRunspace, Func<Runspace, PowerShell> createPowerShell, IEnumerable<string> variableNames, Func<PowerShell, Collection<PSObject>> invoke, Hashtable synchronizedData)
#else
        private static XamlDialogResult Create(Func<Runspace> createRunspace, Func<Runspace, PowerShell> createPowerShell, IEnumerable<string> variableNames, Func<PowerShell, Collection<PSObject>> invoke, Hashtable synchronizedData)
#endif
        {
            string[] names = variableNames.Where(s => !String.IsNullOrEmpty(s)).ToArray();
            using (Runspace runspace = createRunspace())
            {
                using (PowerShell powershell = createPowerShell(runspace))
                    return new XamlDialogResult(invoke(powershell), runspace, powershell.Streams, names, false, synchronizedData);
            }
        }

#if USEXLINQ
        internal static new XamlDialogResult Create(Func<Runspace> createRunspace, Func<Runspace, PowerShell> createPowerShell, IEnumerable<string> variableNames, Hashtable synchronizedData)
#else
        internal static new XamlDialogResult Create(Func<Runspace> createRunspace, Func<Runspace, PowerShell> createPowerShell, IEnumerable<string> variableNames, Hashtable synchronizedData)
#endif
        {
            return Create(createRunspace, createPowerShell, variableNames, ps => ps.Invoke(), synchronizedData);
        }

#if USEXLINQ
        internal static new XamlDialogResult Create(Func<Runspace> createRunspace, Func<Runspace, PowerShell> createPowerShell, IEnumerable input, IEnumerable<string> variableNames, Hashtable synchronizedData)
#else
        internal static new XamlDialogResult Create(Func<Runspace> createRunspace, Func<Runspace, PowerShell> createPowerShell, IEnumerable input, IEnumerable<string> variableNames, Hashtable synchronizedData)
#endif
        {
            return Create(createRunspace, createPowerShell, variableNames, ps => ps.Invoke(input), synchronizedData);
        }

#if USEXLINQ
        internal static new XamlDialogResult Create(Func<Runspace> createRunspace, Func<Runspace, PowerShell> createPowerShell, IEnumerable input, PSInvocationSettings settings, IEnumerable<string> variableNames, Hashtable synchronizedData)
#else
        internal static new XamlDialogResult Create(Func<Runspace> createRunspace, Func<Runspace, PowerShell> createPowerShell, IEnumerable input, PSInvocationSettings settings, IEnumerable<string> variableNames, Hashtable synchronizedData)
#endif
        {
            return Create(createRunspace, createPowerShell, variableNames, ps => ps.Invoke(input, settings), synchronizedData);
        }
    }
}