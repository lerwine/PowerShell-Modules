using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UnitTests
{
    public static class PowerShellHelper
    {
        public static Dictionary<string, object> GetDefaultVariables()
        {
            throw new NotImplementedException();
        }

        public static PSInvocationResult InvokeRunspace(TestContext testContext, InitialSessionState iss, ApartmentState apartmentState, PSThreadOptions threadOptions, Dictionary<string, object> variables, ScriptBlock initialScript, params ScriptBlock[] additionalScripts)
        {
            if (iss == null)
                iss = InitialSessionState.CreateDefault();
            // this.TestContext;
            using (Runspace runspace = RunspaceFactory.CreateRunspace(iss))
            {
                runspace.ApartmentState = apartmentState;
                runspace.ThreadOptions = threadOptions;
                runspace.Open();
                if (variables == null)
                    variables = GetDefaultVariables();
                foreach (string name in variables.Keys)
                    runspace.SessionStateProxy.SetVariable(name, variables[name]);

                using (PowerShell powershell = initialScript.GetPowerShell())
                {
                    powershell.Runspace = runspace;
                    if (additionalScripts != null)
                    {
                        foreach (ScriptBlock sb in additionalScripts)
                        {
                            if (sb != null)
                                powershell.AddScript(sb.ToString());
                        }
                    }

                    return PSInvocationResult.Create(runspace, powershell);
                }
            }
        }

        public static PSInvocationResult InvokeRunspace(TestContext testContext, InitialSessionState iss, ApartmentState apartmentState, PSThreadOptions threadOptions, ScriptBlock initialScript, params ScriptBlock[] additionalScripts)
        {
            return InvokeRunspace(testContext, iss, apartmentState, threadOptions, null, initialScript, additionalScripts);
        }
        public static PSInvocationResult InvokeRunspace(TestContext testContext, InitialSessionState iss, Dictionary<string, object> variables, ScriptBlock initialScript, params ScriptBlock[] additionalScripts)
        {
            return InvokeRunspace(testContext, iss, ApartmentState.STA, PSThreadOptions.ReuseThread, variables, initialScript, additionalScripts);
        }
        public static PSInvocationResult InvokeRunspace(TestContext testContext, InitialSessionState iss, ScriptBlock initialScript, params ScriptBlock[] additionalScripts)
        {
            return InvokeRunspace(testContext, iss, null, initialScript, additionalScripts);
        }
        public static PSInvocationResult InvokeRunspace(TestContext testContext, ApartmentState apartmentState, PSThreadOptions threadOptions, Dictionary<string, object> variables, ScriptBlock initialScript, params ScriptBlock[] additionalScripts)
        {
            return InvokeRunspace(testContext, null, apartmentState, threadOptions, variables, initialScript, additionalScripts);
        }
        public static PSInvocationResult InvokeRunspace(TestContext testContext, ApartmentState apartmentState, PSThreadOptions threadOptions, ScriptBlock initialScript, params ScriptBlock[] additionalScripts)
        {
            return InvokeRunspace(testContext, apartmentState, threadOptions, null, initialScript, additionalScripts);
        }
        public static PSInvocationResult InvokeRunspace(TestContext testContext, Dictionary<string, object> variables, ScriptBlock initialScript, params ScriptBlock[] additionalScripts)
        {
            return InvokeRunspace(testContext, null, variables, initialScript, additionalScripts);
        }
        public static PSInvocationResult InvokeRunspace(TestContext testContext, ScriptBlock initialScript, params ScriptBlock[] additionalScripts)
        {
            return InvokeRunspace(testContext, null, null, initialScript, additionalScripts);
        }

        private static object _psVersion = null;
        private static object _clrVersion = null;
        private static object _buildVersion = null;
        private static object _psCompatibleVersions = null;
        private static object _psHome = null;
        private static object _psModulePath = null;
        private static object _hostName = null;
        private static object _hostVersion = null;

        public static Version AsMajorMinor(Version version)
        {
            if (version == null || (version.Build == 0 && version.Revision == 0))
                return version;

            return new Version(version.Major, version.Minor);

        }
        public static Version GetPSVersion(TestContext testContext)
        {
            if (_psVersion == null)
                _GetHostPsInfo(testContext);
            return _psVersion as Version;
        }
        public static Version GetCLRVersion(TestContext testContext)
        {
            if (_clrVersion == null)
                _GetHostPsInfo(testContext);
            return _clrVersion as Version;
        }

        public static Version GetBuildVersion(TestContext testContext)
        {
            if (_buildVersion == null)
                _GetHostPsInfo(testContext);
            return _buildVersion as Version;
        }

        public static Version[] GetPSCompatibleVersions(TestContext testContext)
        {
            if (_psCompatibleVersions == null)
                _GetHostPsInfo(testContext);
            return _psCompatibleVersions as Version[];
        }

        public static string GetPSHome(TestContext testContext)
        {
            if (_psHome == null)
                _GetHostPsInfo(testContext);
            return _psHome as string;
        }

        public static string[] GetPSModulePath(TestContext testContext)
        {
            if (_psModulePath == null)
                _GetHostPsInfo(testContext);
            return _psModulePath as string[];
        }

        public static string GetHostName(TestContext testContext)
        {
            if (_hostName == null)
                _GetHostPsInfo(testContext);
            return _hostName as string;
        }

        public static Version GetHostVersion(TestContext testContext)
        {
            if (_hostVersion == null)
                _GetHostPsInfo(testContext);
            return _hostVersion as Version;
        }

        private static void _GetHostPsInfo(TestContext testContext)
        {
            try
            {
                InitialSessionState iss = InitialSessionState.CreateDefault();
                // this.TestContext;
                using (Runspace runspace = RunspaceFactory.CreateRunspace(iss))
                {
                    runspace.Open();
                    using (PowerShell powershell = PowerShell.Create(RunspaceMode.NewRunspace))
                    {
                        powershell.AddScript("@{ PSHome = $PSHOME; PSVersionTable = $PSVersionTable; PSModulePath = $env:PSModulePath; HostName = $Host.Name; HostVersion = $Host.Version }");
                        Collection<PSObject> output = powershell.Invoke();
                        if (powershell.HadErrors)
                        {
                            foreach (ErrorRecord errorRecord in powershell.Streams.Error)
                                WriteErrorRecord(testContext, errorRecord);
                            if (powershell.HadErrors)
                                Assert.Inconclusive("Failed to get baseline host info: Multiple errors encountered: See test output for details.");
                        }
                        if (output.Count != 1)
                            Assert.Inconclusive("Failed to get baseline host info: There was not exactly one item returned.");
                        if (output[0] == null || output[0].BaseObject == null)
                            Assert.Inconclusive("Failed to get baseline host info: Return value is null");
                        if (!(typeof(Hashtable)).IsInstanceOfType(output[0].BaseObject))
                            Assert.Inconclusive("Failed to get baseline host info: Return value is not a PSModuleInfo object");
                        Hashtable result = output[0].BaseObject as Hashtable;
                        if ((_psHome = result["PSHome"] as string) == null)
                            _psHome = new object();
                        if ((_hostName = result["HostName"] as string) == null)
                            _hostName = new object();
                        if ((_hostVersion = result["HostVersion"] as Version) == null)
                            _hostVersion = new object();
                        string psModulePath;
                        if ((psModulePath = result["PSHome"] as string) == null)
                            _psModulePath = new object();
                        else if (psModulePath.Length == 0)
                            _psModulePath = new string[0];
                        else
                            _psModulePath = psModulePath.Split(Path.DirectorySeparatorChar);
                        Hashtable psVersionTable = result["PSVersionTable"] as Hashtable;
                        if (psVersionTable == null)
                        {
                            _psVersion = new object();
                            _clrVersion = new object();
                            _buildVersion = new object();
                            _psCompatibleVersions = new object();
                        }
                        else
                        {
                            if ((_psVersion = psVersionTable["PSVersion"] as Version) == null)
                                _psVersion = new object();
                            if ((_clrVersion = psVersionTable["CLRVersion"] as Version) == null)
                                _clrVersion = new object();
                            else
                            if ((_buildVersion = psVersionTable["BuildVersion"] as Version) == null)
                                _buildVersion = new object();
                            if ((_psCompatibleVersions = psVersionTable["PSCompatibleVersions"] as Version[]) == null)
                                _psCompatibleVersions = new object();
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                WriteErrorRecord(testContext, new ErrorRecord(exception, (typeof(PowerShellHelper)).FullName + ".GetHostPsInfo", ErrorCategory.ResourceUnavailable, null));
            }
        }

        public static PSModuleInfo LoadPSModuleFromDeploymentDir(TestContext testContext, string relativePath, params string[] additionalModulePaths)
        {
            string path = GetDeploymentRelativePath(testContext, relativePath);
            Assert.IsTrue(File.Exists(path));

            InitialSessionState iss = InitialSessionState.CreateDefault();
            // this.TestContext;
            using (Runspace runspace = RunspaceFactory.CreateRunspace(iss))
            {
                runspace.Open();
                using (PowerShell powershell = PowerShell.Create(RunspaceMode.NewRunspace))
                {
                    string modulePath = Path.GetFullPath(path);
                    testContext.WriteLine("Path: {0}", modulePath);
                    powershell.Runspace = runspace;

                    PowerShell ps = powershell.AddScript(@"
for ($i = 1; $i -lt $args.Count; $i++) {
    Import-Module $args[$i];
}
Import-Module $args[0] -PassThru;
");
                    PowerShell ps2 = ps.AddArgument(modulePath);
                    if (additionalModulePaths != null && additionalModulePaths.Length > 0)
                    {
                        foreach (string m in additionalModulePaths)
                            powershell.AddArgument(GetDeploymentRelativePath(testContext, m));
                    }

                    Collection<PSObject> result = powershell.Invoke();
                    if (powershell.HadErrors)
                    {
                        foreach (ErrorRecord errorRecord in powershell.Streams.Error)
                            WriteErrorRecord(testContext, errorRecord);
                        Assert.IsFalse(powershell.HadErrors, "Multiple errors encountered: See test output for details.");
                    }
                    Assert.AreEqual(1, result.Count, "There was not exactly one item returned.");
                    Assert.IsNotNull(result[0], "Return value is null");
                    Assert.IsNotNull(result[0].BaseObject, "Return value is null");
                    Assert.IsInstanceOfType(result[0].BaseObject, typeof(PSModuleInfo), "Return value is not a PSModuleInfo object");
                    return result[0].BaseObject as PSModuleInfo;
                }
            }
        }

        public static string GetDeploymentRelativePath(TestContext testContext, string relativePath)
        {
            return Path.GetFullPath(Path.Combine(Path.Combine(testContext.DeploymentDirectory, @"..\..\..\Deployment"), relativePath));
        }

        public static PSModuleInfo LoadPSModuleInfo(TestContext testContext, string path, params string[] additionalModules)
        {
            InitialSessionState iss = InitialSessionState.CreateDefault();
            // this.TestContext;
            using (Runspace runspace = RunspaceFactory.CreateRunspace(iss))
            {
                runspace.Open();
                using (PowerShell powershell = PowerShell.Create(RunspaceMode.NewRunspace))
                {
                    string modulePath = Path.GetFullPath(path);
                    testContext.WriteLine("Path: {0}", modulePath);
                    powershell.Runspace = runspace;

                    PowerShell ps = powershell.AddScript(@"
for ($i = 1; $i -lt $args.Count; $i++) {
    Import-Module $args[$i];
}
Import-Module $args[0] -PassThru;
");
                    PowerShell ps2 = ps.AddArgument(modulePath);
                    if (additionalModules != null && additionalModules.Length > 0)
                    {
                        foreach (string m in additionalModules)
                            powershell.AddArgument(m);
                    }

                    Collection<PSObject> result = powershell.Invoke();
                    if (powershell.HadErrors)
                    {
                        foreach (ErrorRecord errorRecord in powershell.Streams.Error)
                            WriteErrorRecord(testContext, errorRecord);
                        Assert.IsFalse(powershell.HadErrors, "Multiple errors encountered: See test output for details.");
                    }
                    Assert.AreEqual(1, result.Count, "There was not exactly one item returned.");
                    Assert.IsNotNull(result[0], "Return value is null");
                    Assert.IsNotNull(result[0].BaseObject, "Return value is null");
                    Assert.IsInstanceOfType(result[0].BaseObject, typeof(PSModuleInfo), "Return value is not a PSModuleInfo object");
                    return result[0].BaseObject as PSModuleInfo;
                }
            }
        }

        public static void TestLoadModule(TestContext testContext, string moduleName, string relativeModulePath, string moduleExtension, params string[] additionalModules)
        {
            InitialSessionState iss = InitialSessionState.CreateDefault();
            // this.TestContext;
            using (Runspace runspace = RunspaceFactory.CreateRunspace(iss))
            {
                runspace.Open();
                using (PowerShell powershell = PowerShell.Create(RunspaceMode.NewRunspace))
                {
                    string modulePath = Path.GetFullPath(Path.Combine(@"..\..\..", relativeModulePath, moduleName + ".psd1"));
                    testContext.WriteLine("Path: {0}", modulePath);
                    powershell.Runspace = runspace;
                    
                    PowerShell ps = powershell.AddScript(@"
for ($i = 1; $i -lt $args.Count; $i++) {
    Import-Module $args[$i];
}
Import-Module $args[0] -PassThru;
");
                    PowerShell ps2 = ps.AddArgument(modulePath);
                    if (additionalModules != null && additionalModules.Length > 0)
                    {
                        foreach (string m in additionalModules)
                            powershell.AddArgument(m);
                    }

                    Collection<PSObject> result = powershell.Invoke();
                    if (powershell.HadErrors)
                    {
                        foreach (ErrorRecord errorRecord in powershell.Streams.Error)
                            WriteErrorRecord(testContext, errorRecord);
                        Assert.IsFalse(powershell.HadErrors, "Multiple errors encountered: See test output for details.");
                    }
                    Assert.AreEqual(1, result.Count, "There was not exactly one item returned.");
                    Assert.IsNotNull(result[0], "Return value is null");
                    Assert.IsNotNull(result[0].BaseObject, "Return value is null");
                    Assert.IsInstanceOfType(result[0].BaseObject, typeof(PSModuleInfo), "Return value is not a PSModuleInfo object");
                    PSModuleInfo psModuleInfo = result[0].BaseObject as PSModuleInfo;
                    Assert.AreEqual(moduleName, psModuleInfo.Name, "Mismatched module name");
                    Assert.AreEqual(moduleName + moduleExtension, psModuleInfo.RootModule, "Mismatched RootModule");
                    Version version = GetPSVersion(testContext);
                    if (version != null)
                    {
                        Assert.IsNotNull(psModuleInfo.PowerShellVersion, "PowerShell version not specified");
                        Assert.AreEqual(AsMajorMinor(version), AsMajorMinor(psModuleInfo.PowerShellVersion), "Invalid PowerShell version");
                    }
                    if ((version = GetCLRVersion(testContext)) != null)
                    {
                        Assert.IsNotNull(psModuleInfo.ClrVersion, "CLR version not specified");
                        Assert.AreEqual(AsMajorMinor(version), AsMajorMinor(psModuleInfo.ClrVersion), "Invalid CLR version");
                        if (psModuleInfo.DotNetFrameworkVersion != null)
                            Assert.AreEqual(AsMajorMinor(version), AsMajorMinor(psModuleInfo.DotNetFrameworkVersion), "Invalid .NET Framework version");
                    }
                }
            }
        }

        private static void WriteErrorRecord(TestContext testContext, ErrorRecord errorRecord)
        {
            if (errorRecord.InvocationInfo != null)
            {
                if (!String.IsNullOrEmpty(errorRecord.InvocationInfo.ScriptName))
                    testContext.WriteLine("Error in \"{0}\", line {1}, positon {2}:", errorRecord.InvocationInfo.ScriptName, errorRecord.InvocationInfo.ScriptLineNumber, errorRecord.InvocationInfo.OffsetInLine);
                else
                    testContext.WriteLine("Error on line {0}, positon {1}:", errorRecord.InvocationInfo.ScriptLineNumber, errorRecord.InvocationInfo.OffsetInLine);
                if (!String.IsNullOrEmpty(errorRecord.InvocationInfo.Line))
                    testContext.WriteLine("\tLine: \"{0}\"", errorRecord.InvocationInfo.Line);
            }
            else
                testContext.WriteLine("Error:");
            testContext.WriteLine("\tFullyQualifiedErrorId: {0}", errorRecord.FullyQualifiedErrorId);

            if (errorRecord.ErrorDetails != null && !String.IsNullOrEmpty(errorRecord.ErrorDetails.Message))
                testContext.WriteLine("\tDetails: {0}", errorRecord.ErrorDetails.Message);

            if (errorRecord.Exception != null)
                testContext.WriteLine("\tException: {0}", errorRecord.Exception.ToString());
            testContext.WriteLine("");
        }

        public class PSInvocationResult
        {
            public static Collection<PSObject> Output { get; private set; }

            internal static PSInvocationResult Create(Runspace runspace, PowerShell powershell)
            {
                Output = powershell.Invoke();
                throw new NotImplementedException();
            }
        }
    }
}
