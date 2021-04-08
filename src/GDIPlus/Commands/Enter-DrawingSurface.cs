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
    /// New-DrawingSurface
    /// </summary>
    [Cmdlet(VerbsCommon.Enter, "DrawingSurface", RemotingCapability = RemotingCapability.None)]
    [OutputType(typeof(Graphics))]
    public class Enter_DrawingSurface : PSCmdlet
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        [ValidateNotNullOrEmpty()]
        public Image[] Image { get; set; }
        
        protected override void ProcessRecord()
        {
            if (Image == null)
                return;
            foreach (Image i in Image)
            {
                if (i == null)
                    continue;
                try { WriteObject(Graphics.FromImage(i)); }
                catch (Exception e)
                {
                    WriteError(new ErrorRecord(e, "Enter_DrawingSurface", ErrorCategory.InvalidData, i));
                }
            }
        }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }
}