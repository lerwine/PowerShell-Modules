﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests
{
    public static class PowerShellHelper
    {
        public static void TestLoadModule(TestContext testContext, string moduleName, string relativeModulePath, string moduleExtension, params string[] additionalModules)
        {
            InitialSessionState iss = InitialSessionState.CreateDefault();
            if (additionalModules != null && additionalModules.Length > 0)
                iss.ImportPSModule(additionalModules);
            iss.ExecutionPolicy = Microsoft.PowerShell.ExecutionPolicy.Bypass;
            // this.TestContext;
            using (Runspace runspace = RunspaceFactory.CreateRunspace(iss))
            {
                runspace.Open();
                using (PowerShell powershell = PowerShell.Create(RunspaceMode.NewRunspace))
                {
                    string modulePath = Path.GetFullPath(Path.Combine(@"..\..\..", relativeModulePath, moduleName + ".psd1"));
                    testContext.WriteLine("Path: {0}", modulePath);
                    powershell.Runspace = runspace;
                    PowerShell ps = powershell.AddCommand("Import-Module");
                    ps.AddParameter("Name", modulePath);
                    ps.AddParameter("PassThru");
                    Collection<PSObject> result = powershell.Invoke();
                    if (powershell.HadErrors)
                    {
                        foreach (ErrorRecord errorRecord in powershell.Streams.Error)
                            testContext.WriteLine("Error: {0}", errorRecord);
                        Assert.IsFalse(powershell.HadErrors, "Multiple errors encountered: See test output for details.");
                    }
                    Assert.AreEqual(1, result.Count, "There was not exactly one item returned.");
                    Assert.IsNotNull(result[0], "Return value is null");
                    Assert.IsNotNull(result[0].BaseObject, "Return value is null");
                    Assert.IsInstanceOfType(result[0].BaseObject, typeof(PSModuleInfo), "Return value is not a PSModuleInfo object");
                    PSModuleInfo psModuleInfo = result[0].BaseObject as PSModuleInfo;
                    Assert.AreEqual(moduleName, psModuleInfo.Name, "Mismatched module name");
                    Assert.AreEqual(moduleName + moduleExtension, psModuleInfo.RootModule, "Mismatched RootModule");
                    Assert.IsNotNull(psModuleInfo.PowerShellVersion, "PowerShell version not specified");
                    Assert.AreEqual(psModuleInfo.PowerShellVersion, new Version(4, 0), "Invalid PowerShell version");
                    Assert.IsNotNull(psModuleInfo.ClrVersion, "CLR version not specified");
                    Assert.AreEqual(psModuleInfo.ClrVersion, new Version(4, 0), "Invalid CLR version");
                    Assert.IsNotNull(psModuleInfo.DotNetFrameworkVersion, ".NET Framework version not specified");
                    Assert.AreEqual(psModuleInfo.DotNetFrameworkVersion, new Version(4, 0), "Invalid .NET Framework version");
                }
            }
        }
    }
}