using System.Drawing.Imaging;
using System.Management.Automation;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Erwine.Leonard.T.GDIPlus.Commands
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    /// <summary>
    /// New-Image
    /// </summary>
    [Cmdlet(VerbsCommon.New, "Image", DefaultParameterSetName = ParameterSetName_New, RemotingCapability = RemotingCapability.None)]
    [OutputType(typeof(Bitmap))]
    public class New_Image : PSCmdlet
    {
        public const string ParameterSetName_New = "New";
        public const string ParameterSetName_FromImage = "FromImage";

        [Parameter(Mandatory = true, ValueFromPipeline = true, ParameterSetName = ParameterSetName_FromImage)]
        [ValidateNotNull()]        
        public Image[] Original { get; set; }
        
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, ParameterSetName = ParameterSetName_New)]
        [ValidateRange(1, 65535)]
        public int Width { get; set; }
        
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, ParameterSetName = ParameterSetName_New)]
        [ValidateRange(1, 65535)]
        public int Height { get; set; }

        [Parameter(ParameterSetName = ParameterSetName_New)]
        public PixelFormat Format { get; set; }

        protected override void ProcessRecord()
        {
            if (ParameterSetName == ParameterSetName_FromImage)
            {
                if (Original == null)
                    return;
                foreach (Image i in Original)
                {
                    try { WriteObject(new Bitmap(i)); }
                    catch (Exception e)
                    {
                        WriteError(new ErrorRecord(e, "New_Image.FromImage", ErrorCategory.InvalidData, i));
                    }
                }
                return;
            }
            try
            {
                if (MyInvocation.BoundParameters.ContainsKey("Format"))
                    WriteObject(new Bitmap(Width, Height, Format));
                else
                    WriteObject(new Bitmap(Width, Height));
            }
            catch (Exception e)
            {
                WriteError(new ErrorRecord(e, "New_Image.New", ErrorCategory.InvalidArgument, new Size(Width, Height)));    
            }
        }
    }
}