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
    [Cmdlet(VerbsCommon.Get, "CrytoDataFolder")]
    [OutputType(typeof(string))]
    public class Get_CrytoDataFolder : PSCmdlet
    {
        protected override void ProcessRecord()
        {
            Guid guid = new Guid(Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(GuidAttribute), false).OfType<GuidAttribute>().First().Value);
            this.WriteObject(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), guid.ToString("B")));
        }
    }
}
