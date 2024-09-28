using System;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Provider;
using System.Xml;
using System.Xml.Schema;
using Microsoft.PowerShell.Commands;

namespace XmlUtility.Commands
{
    [Cmdlet(VerbsCommon.Get, "ModuleSchemaFiles")]
    public class Get_ModuleSchemaFiles : PSCmdlet
    {
        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            if (Stopping)
                return;
            
            PSModuleInfo module = SessionState.Module;
            if (module == null)
                this.Host.UI.WriteLine(ConsoleColor.Cyan, ConsoleColor.Black, "SessionState.Module is null");
            module = MyInvocation.MyCommand.Module;
            string path = SessionState.Path.Combine(module.ModuleBase, "Schemas");
            System.Diagnostics.Trace.WriteLine("Path is " + path);
            if (module.ModuleBase == null)
                this.Host.UI.WriteLine(ConsoleColor.Cyan, ConsoleColor.Black, "ModuleBase is null");
            else
                this.Host.UI.WriteLine(ConsoleColor.Cyan, ConsoleColor.Black, "ModuleBase is '" + module.ModuleBase + "'");
            if (module.Name == null)
                this.Host.UI.WriteLine(ConsoleColor.Cyan, ConsoleColor.Black, "Name is null");
            else
                this.Host.UI.WriteLine(ConsoleColor.Cyan, ConsoleColor.Black, "Name is '" + module.Name + "'");
            if (module.Path == null)
                this.Host.UI.WriteLine(ConsoleColor.Cyan, ConsoleColor.Black, "Path is null");
            else
                this.Host.UI.WriteLine(ConsoleColor.Cyan, ConsoleColor.Black, "Path is '" + module.Path + "'");
                
            foreach (PSObject obj in this.InvokeProvider.ChildItem.Get(path, true))
            {
                if (Stopping)
                    return;
                WriteObject(obj);
            }
        }
    }
}
