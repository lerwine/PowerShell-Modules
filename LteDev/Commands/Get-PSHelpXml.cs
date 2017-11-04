using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace LteDev.Commands
{
    /// <summary>
    /// Generates PowerShell help file from module information.
    /// </summary>
    /// <description>
    /// Gets information from the PowerShell module, including the DLL files, if the module is binary.
    /// </description>
    [Cmdlet(VerbsCommon.Get, "PSHelpXml")]
    public class Get_PSHelpXml : PSCmdlet
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public const string ParameterSetName_ModuleInfo = "ModuleInfo";
        public const string ParameterSetName_LiteralPath = "LiteralPath";
        public const string ParameterSetName_WildCardPath = "WildCardPath";
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// Information about the module, which is used to generate parameter help.
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipeline = true, ParameterSetName = ParameterSetName_ModuleInfo)]
        public PSModuleInfo[] ModuleInfo { get; set; }

        /// <summary>
        /// Path of module to load.
        /// </summary>
        /// <remarks>Using this parameter results in the module being loaded, if it not already loaded.</remarks>
        [Parameter(Mandatory = true, ValueFromPipeline = true, ParameterSetName = ParameterSetName_WildCardPath)]
        public string[] Path { get; set; }

        /// <summary>
        /// Literal path of module to load.
        /// </summary>
        /// <remarks>Using this parameter results in the module being loaded, if it not already loaded.</remarks>
        [Parameter(Mandatory = true, ParameterSetName = ParameterSetName_LiteralPath)]
        public string[] LiteralPath { get; set; }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        protected override void ProcessRecord()
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            IEnumerable<PSModuleInfo> modules;
            switch (ParameterSetName)
            {
                case ParameterSetName_ModuleInfo:
                    modules = ModuleInfo;
                    break;
                case ParameterSetName_LiteralPath:
                    throw new NotImplementedException();
                case ParameterSetName_WildCardPath:
                    throw new NotImplementedException();
            }
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static PSModuleInfo GetPSModule()
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            throw new NotImplementedException();
        }
    }
}
