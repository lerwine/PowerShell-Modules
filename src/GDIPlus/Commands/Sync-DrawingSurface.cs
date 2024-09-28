using System.Management.Automation;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Erwine.Leonard.T.GDIPlus.Commands
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    /// <summary>
    /// Sync-DrawingSurface
    /// </summary>
    [Cmdlet(VerbsData.Sync, "DrawingSurface", RemotingCapability = RemotingCapability.None)]
    public class Sync_DrawingSurface : PSCmdlet
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
                try { Surface[i].Flush(); }
                catch (Exception e)
                {
                    WriteError(new ErrorRecord(e, "Sync_DrawingSurface", ErrorCategory.InvalidData, i));
                }
            }
        }
    }
}