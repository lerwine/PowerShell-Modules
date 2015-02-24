using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace Erwine.Leonard.T.CryptoShare.Commands
{
    [Cmdlet(VerbsCommon.Get, "AssemblyDataFolder")]
    [OutputType(typeof(string))]
    public class Get_AssemblyDataFolder : PSCmdlet
    {
        [Parameter(Mandatory = false, ParameterSetName = "Roaming")]
        public SwitchParameter Roaming { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = "Local")]
        public SwitchParameter Local { get; set; }

        protected override void ProcessRecord()
        {
            Guid guid = new Guid(Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(GuidAttribute), false).OfType<GuidAttribute>().First().Value);
            Environment.SpecialFolder folder = (this.Local.ToBool()) ? Environment.SpecialFolder.LocalApplicationData : Environment.SpecialFolder.ApplicationData;
            this.WriteObject(Path.Combine(Environment.GetFolderPath(folder), guid.ToString("B")));
        }
    }
}
