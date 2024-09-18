using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Reflection;

namespace LteDev.Commands
{
#pragma warning disable 1591 // Missing XML comment for publicly visible type or member
    [Cmdlet(VerbsData.Group, "TypeInfo", DefaultParameterSetName = ParameterSetName_ByNsFromTypes)]
    [OutputType(typeof(IDictionary<string, TypeInfo>))]
    public class Group_TypeInfo : Cmdlet
    {
        public const string ParameterSetName_ByAssemblyFromTypes = "ByAssemblyFromTypes";
        public const string ParameterSetName_ByAssemblyFromAssembly = "ByAssemblyFromAssembly";
        public const string ParameterSetName_ByNsFromTypes = "ByNsFromTypes";
        public const string ParameterSetName_ByNsFromAssembly = "ByNsFromAssembly";

        #region Properties

        [Parameter(Mandatory = true, ValueFromPipeline = true, ParameterSetName = ParameterSetName_ByAssemblyFromTypes)]
        [Parameter(Mandatory = true, ValueFromPipeline = true, ParameterSetName = ParameterSetName_ByNsFromTypes)]
        public Type[] Type { get; set; } = null!;

        [Parameter(Mandatory = true, ValueFromPipeline = true, ParameterSetName = ParameterSetName_ByAssemblyFromAssembly)]
        [Parameter(Mandatory = true, ValueFromPipeline = true, ParameterSetName = ParameterSetName_ByNsFromAssembly)]
        public Assembly[] Assembly { get; set; } = null!;

        [Parameter(ParameterSetName = ParameterSetName_ByNsFromTypes)]
        [Parameter(ValueFromPipeline = true, ParameterSetName = ParameterSetName_ByNsFromAssembly)]
        public SwitchParameter ByNs { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = ParameterSetName_ByAssemblyFromTypes)]
        [Parameter(Mandatory = true, ParameterSetName = ParameterSetName_ByAssemblyFromAssembly)]
        public SwitchParameter ByAssembly { get; set; }

        #endregion

        #region Overrides

        protected override void ProcessRecord()
        {
        }

        #endregion
    }
#pragma warning restore 1591 // Missing XML comment for publicly visible type or member
}
