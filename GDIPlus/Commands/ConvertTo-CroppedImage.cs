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
    /// New-Image
    /// </summary>
    [Cmdlet(VerbsData.ConvertTo, "CroppedImage", RemotingCapability = RemotingCapability.None)]
    [OutputType(typeof(Bitmap))]
    public class ConvertTo_CroppedImage : PSCmdlet
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
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
                    Bitmap target = new Bitmap(Width, Height, (MyInvocation.BoundParameters.ContainsKey("Format")) ? Format : image.PixelFormat);
                    using (Graphics g = Graphics.FromImage(target))
                    {
                            
                    }
                }
                catch (Exception e)
                {
                    WriteError(new ErrorRecord(e, "New_Image.FromImage", ErrorCategory.InvalidData, image));
                }
            }
        }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }
}