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
        public static void AssertPSModule(PSModuleInfo moduleInfo)
        {
            Assert.IsNotNull(moduleInfo);
            _AssertPSModule(moduleInfo);

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

        public static void AssertScriptPSModule(PSModuleInfo moduleInfo)
        {
            Assert.IsNotNull(moduleInfo);
            Assert.AreEqual(ModuleType.Script, moduleInfo.ModuleType, "Module is not a script module.");
            _AssertPSModule(moduleInfo);
            _AssertScriptPSModule(moduleInfo);
        }

        private static void _AssertScriptPSModule(PSModuleInfo moduleInfo)
        {
            throw new NotImplementedException();
        }

        public static void AssertBinaryModule(PSModuleInfo moduleInfo)
        {
            Assert.IsNotNull(moduleInfo);
            Assert.AreEqual(ModuleType.Binary, moduleInfo.ModuleType, "Module is not a binary module.");
            _AssertPSModule(moduleInfo);
            _AssertBinaryPSModule(moduleInfo);
        }

        private static void _AssertBinaryPSModule(PSModuleInfo moduleInfo)
        {
            Assembly assembly = Assembly.LoadFrom(moduleInfo.Path);
            Assert.IsNotNull(assembly, "Failed to load module assembly.");
            AssemblyCmdlet[] cmdlets = AssemblyCmdlet.GetCmdlets(assembly).ToArray();
            foreach (AssemblyCmdlet a in cmdlets)
            {
                CmdletInfo cmdlet = moduleInfo.ExportedCmdlets.Select(e => e.Value).FirstOrDefault(e => e.ImplementingType.Equals(a.Type));
                Assert.IsNotNull(cmdlet, String.Format("Cmdlet \"{0}-{1}\" is not exported.", a.CmdletAttribute.VerbName, a.CmdletAttribute.NounName));
            }

            Version expectionVersion = assembly.GetCustomAttributes<AssemblyFileVersionAttribute>().Where(a => !String.IsNullOrWhiteSpace(a.Version)).Select(a =>
            {
                Version v;
                if (Version.TryParse(a.Version, out v))
                    return v;
                return null;
            }).Where(v => v != null).DefaultIfEmpty(assembly.GetName().Version).First();
            Assert.AreEqual(expectionVersion, moduleInfo.Version, "Unexpected module version");
            Assert.IsNotNull(moduleInfo.ClrVersion, "CLR version must be defined");
            Assert.AreEqual(assembly.ImageRuntimeVersion, moduleInfo.ClrVersion, "Unexpected CLR version");
            string path = moduleInfo.Path + "-Help.xml";
            if (File.Exists(path))
            {

            }
        }

        private static void _AssertPSModule(PSModuleInfo moduleInfo)
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
        }
    }
}
