using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Management.Automation;
using System.Text;

namespace Erwine.Leonard.T.GDIPlus.Commands
{
    /// <summary>
    /// Exit-DrawingSurface
    /// </summary>
    [Cmdlet(VerbsCommon.Exit, "DrawingSurface", RemotingCapability = RemotingCapability.None)]
    public class Exit_DrawingSurface : PSCmdlet
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        [ValidateNotNullOrEmpty()]
        [Alias("Graphics", "DrawingSurface")]
        public Graphics[] Surface { get; set; }
        
        protected override void ProcessRecord()
        {
            if (Surface == null)
                return;
            foreach (Graphics g in Surface)
            {
                if (g == null)
                    continue;
                try
                {
                    g.Flush();
                    g.Dispose();
                }
                catch (Exception e)
                {
                    WriteError(new ErrorRecord(e, "Sync_DrawingSurface", ErrorCategory.InvalidData, i));
                }
            }
        }
    }
}