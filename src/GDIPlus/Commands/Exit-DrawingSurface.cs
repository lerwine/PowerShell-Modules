using System.Management.Automation;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Erwine.Leonard.T.GDIPlus.Commands
#pragma warning restore IDE0130 // Namespace does not match folder structure
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
            for (int i = 0; i < Surface.Length; i++)
            {
                if (Surface[i] == null)
                    continue;
                try
                {
                    Surface[i].Flush();
                    Surface[i].Dispose();
                }
                catch (Exception e)
                {
                    WriteError(new ErrorRecord(e, "Exit_DrawingSurface", ErrorCategory.InvalidData, i));
                }
            }
        }
    }
}