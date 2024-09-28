using System.Management.Automation;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Erwine.Leonard.T.GDIPlus.Commands
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    /// <summary>
    /// New-DrawingSurface
    /// </summary>
    [Cmdlet(VerbsCommon.Enter, "DrawingSurface", RemotingCapability = RemotingCapability.None)]
    [OutputType(typeof(Graphics))]
    public class Enter_DrawingSurface : PSCmdlet
    {
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
    }
}