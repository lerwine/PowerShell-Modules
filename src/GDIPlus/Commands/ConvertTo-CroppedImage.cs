using System.Drawing.Imaging;
using System.Management.Automation;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Erwine.Leonard.T.GDIPlus.Commands
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    /// <summary>
    /// New-Image
    /// </summary>
    [Cmdlet(VerbsData.ConvertTo, "CroppedImage", RemotingCapability = RemotingCapability.None)]
    [OutputType(typeof(Bitmap))]
    public class ConvertTo_CroppedImage : PSCmdlet
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        [ValidateNotNullOrEmpty()]        
        public Image[] Source { get; set; }
        
        [Parameter(Mandatory = true)]
        [ValidateRange(0, 65535)]
        public int Left { get; set; }
        
        [Parameter(Mandatory = true)]
        [ValidateRange(1, 65535)]
        public int Width { get; set; }
        
        [Parameter(Mandatory = true)]
        [ValidateRange(0, 65535)]
        public int Top { get; set; }
        
        [Parameter(Mandatory = true)]
        [ValidateRange(1, 65535)]
        public int Height { get; set; }

        [Parameter()]
        public PixelFormat Format { get; set; }

        protected override void ProcessRecord()
        {
            foreach (Image image in Source)
            {
                try
                {
                    if (image.Width == Width && image.Height == Height)
                    {
                        if (!MyInvocation.BoundParameters.ContainsKey("Format") || image.PixelFormat == Format)
                        {
                            WriteObject(image);
                            continue;
                        }
                    }
                    Bitmap target = new(Width, Height, MyInvocation.BoundParameters.ContainsKey("Format") ? Format : image.PixelFormat);
                    if (Left < image.Width && Top < image.Height)
                    {
                        int right = Left + Width;
                        if (right > image.Width) right = image.Width;
                        int bottom = Top + Height;
                        if (bottom > image.Height) bottom = image.Height;
                        int width = right - Left;
                        int height = bottom - Top;
                        using Graphics g = Graphics.FromImage(target);
                        Rectangle dest = new(0, 0, width, height);
                        g.DrawImage(image, dest, new Rectangle(Left, Top, width, height), GraphicsUnit.Pixel);
                    }
                    WriteObject(target);
                }
                catch (Exception e)
                {
                    WriteError(new ErrorRecord(e, "New_Image.FromImage", ErrorCategory.InvalidData, image));
                }
            }
        }
    }
}