using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Reflection;
using System.Text;

namespace UnitTests.ModuleConformance
{
    public class ModuleValidator
    {
        public static void AssertPSModule(TestContext testContext, PSModuleInfo moduleInfo)
        {
            Assert.IsNotNull(moduleInfo);
            _AssertPSModule(testContext, moduleInfo);

            switch (moduleInfo.ModuleType)
            {
                case ModuleType.Binary:
                    _AssertBinaryPSModule(moduleInfo);
                    break;
                case ModuleType.Script:
                    _AssertScriptPSModule(moduleInfo);
                    break;
            }
        }

        public static void AssertScriptPSModule(TestContext testContext, PSModuleInfo moduleInfo)
        {
            Assert.IsNotNull(moduleInfo);
            Assert.AreEqual(ModuleType.Script, moduleInfo.ModuleType, "Module is not a script module.");
            _AssertPSModule(testContext, moduleInfo);
            _AssertScriptPSModule(moduleInfo);
        }

        private static void _AssertScriptPSModule(PSModuleInfo moduleInfo)
        {
            Assert.IsNotNull(moduleInfo.ClrVersion, "CLR version must be defined");
        }

        public static void AssertBinaryModule(TestContext testContext, PSModuleInfo moduleInfo)
        {
            Assert.IsNotNull(moduleInfo);
            Assert.AreEqual(ModuleType.Binary, moduleInfo.ModuleType, "Module is not a binary module.");
            _AssertPSModule(testContext, moduleInfo);
            _AssertBinaryPSModule(moduleInfo);
        }

        private static void _AssertBinaryPSModule(PSModuleInfo moduleInfo)
        {
            Assembly assembly = Assembly.LoadFrom(moduleInfo.Path);
            Assert.IsNotNull(assembly, "Failed to load module assembly.");
            AssemblyCmdlet[] cmdlets = AssemblyCmdlet.GetCmdlets(assembly).ToArray();
            foreach (AssemblyCmdlet a in cmdlets)
            {
                CmdletInfo cmdlet = moduleInfo.ExportedCmdlets.Select(e => e.Value).FirstOrDefault(e => e.ImplementingType.AssemblyQualifiedName == a.Type.AssemblyQualifiedName);
                Assert.IsNotNull(cmdlet, String.Format("Cmdlet \"{0}-{1}\" is not exported.", a.CmdletAttribute.VerbName, a.CmdletAttribute.NounName));
            }

            Version expectionVersion = assembly.GetCustomAttributes<AssemblyFileVersionAttribute>().Where(a => !String.IsNullOrWhiteSpace(a.Version)).Select(a =>
            {
                Version v;
                if (Version.TryParse(a.Version, out v))
                    return v;
                return null;
            }).Where(v => v != null).DefaultIfEmpty(assembly.GetName().Version).First();
            Assert.AreEqual(expectionVersion.ToString(2), moduleInfo.Version.ToString(2), "Unexpected module version");
            string s = assembly.ImageRuntimeVersion;
            if (s.StartsWith("v"))
                s = s.Substring(1);
            Assert.AreEqual(new Version(s).ToString(2), moduleInfo.ClrVersion.ToString(2), "Unexpected CLR version");
            string path = moduleInfo.Path + "-Help.xml";
            if (File.Exists(path))
            {
                // TODO: Generate help
            }
        }

        private static void _AssertPSModule(TestContext testContext, PSModuleInfo moduleInfo)
        {
            Assert.IsNotNull(moduleInfo.Author, "Author must be defined.");
            Assert.AreNotEqual(0, moduleInfo.Author.Trim().Length, "Author must be defined.");
            Assert.AreNotEqual("unknown", moduleInfo.Author.ToLower().Trim(), "Author must be defined.");
            Assert.AreNotEqual("unknown.", moduleInfo.Author.ToLower().Trim(), "Author must be defined.");

            Assert.IsNotNull(moduleInfo.CompanyName, "Company Name must be defined.");
            Assert.AreNotEqual(0, moduleInfo.CompanyName.Trim().Length, "Company Name must be defined.");
            Assert.AreNotEqual("unknown", moduleInfo.CompanyName.ToLower().Trim(), "Company Name must be defined.");
            Assert.AreNotEqual("unknown.", moduleInfo.CompanyName.ToLower().Trim(), "Company Name must be defined.");

            Assert.IsNotNull(moduleInfo.Description, "Description must be defined.");
            Assert.AreNotEqual(0, moduleInfo.Description.Trim().Length, "Description must be defined.");

            Assert.IsNotNull(moduleInfo.RootModule, "Root Module must be defined.");
            Assert.AreNotEqual(0, moduleInfo.RootModule.Trim().Length, "Root Module must be defined.");

            Assert.IsNotNull(moduleInfo.Version, "Module Version must be specified.");

            Assert.IsNotNull(moduleInfo.ClrVersion, "CLR version must be defined");
            Assert.AreEqual(PowerShellHelper.GetCLRVersion(testContext).ToString(2), moduleInfo.ClrVersion.ToString(2));

            Assert.IsNotNull(moduleInfo.PowerShellVersion, "Minimum PowerShell Version must be defined");
            Assert.AreEqual(PowerShellHelper.GetPSVersion(testContext).ToString(2), moduleInfo.PowerShellVersion.ToString(2));
            
            // TODO: Validate nested modules
            string[] knownFiles = GetAssumedFiles(moduleInfo).Concat(new string[]
            {
                Path.Combine(moduleInfo.ModuleBase, "Install.ps1"),
                Path.Combine(moduleInfo.ModuleBase, "Install.bat"),
                Path.Combine(moduleInfo.ModuleBase, "Setup.bat"),
                Path.Combine(moduleInfo.ModuleBase, "Setup.ps1"),
                Path.Combine(moduleInfo.ModuleBase, "Test.bat"),
                Path.Combine(moduleInfo.ModuleBase, "Test.ps1"),
                Path.Combine(moduleInfo.ModuleBase, "Uninstall.bat"),
                Path.Combine(moduleInfo.ModuleBase, "Uninstall.ps1"),
                Path.Combine(moduleInfo.ModuleBase, "README.md"),
                Path.Combine(moduleInfo.ModuleBase, "README.txt"),
                Path.Combine(moduleInfo.ModuleBase, "about_" + moduleInfo.Name + ".txt")
            }).ToArray();
            foreach (string path in GetModuleFiles(moduleInfo.ModuleBase))
                Assert.IsTrue(knownFiles.Any(f => String.Equals(f, path, StringComparison.InvariantCultureIgnoreCase)), String.Format("{0} is not included in the module.", path));
        }

        public static IEnumerable<string> GetModuleFiles(string path)
        {
            IEnumerable<string> files = Directory.GetFiles(path);
            foreach (string p in Directory.GetDirectories(path))
                files = files.Concat(GetModuleFiles(p));
            return files;
        }

        public static IEnumerable<string> GetAssumedFiles(PSModuleInfo moduleInfo)
        {
            IEnumerable<string> allFiles = new string[]
            {
                moduleInfo.Path,
                Path.Combine(moduleInfo.ModuleBase, Path.GetFileNameWithoutExtension(moduleInfo.Path) + ".psd1"),
                moduleInfo.Path + "-Help.ps1xml"
            };

            if (moduleInfo.ModuleType == ModuleType.Binary)
                allFiles = allFiles.Concat(new string[]
                {
                    Path.Combine(moduleInfo.ModuleBase, Path.GetFileNameWithoutExtension(moduleInfo.Path) + ".pdb")
                });

            if (moduleInfo.ExportedFormatFiles != null)
                allFiles = allFiles.Concat(moduleInfo.ExportedFormatFiles);

            if (moduleInfo.ExportedTypeFiles != null)
                allFiles = allFiles.Concat(moduleInfo.ExportedTypeFiles);

            if (moduleInfo.Scripts != null)
                allFiles = allFiles.Concat(moduleInfo.Scripts);

            if (moduleInfo.FileList != null)
                allFiles = allFiles.Concat(moduleInfo.FileList);

            // TODO: Append moduleInfo.ModuleList;
            if (moduleInfo.NestedModules != null)
            {
                foreach (PSModuleInfo m in moduleInfo.NestedModules)
                    allFiles = allFiles.Concat(GetAssumedFiles(m));
            }
            return allFiles;
        }
    }
}
