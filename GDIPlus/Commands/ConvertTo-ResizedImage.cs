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
    [Cmdlet(VerbsData.ConvertTo, "ResizedImage", RemotingCapability = RemotingCapability.None)]
    [OutputType(typeof(Bitmap))]
    public class ConvertTo_ResizedImage : PSCmdlet
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        [ValidateNotNullOrEmpty()]        
        public Image[] Source { get; set; }
        
        [Parameter(Mandatory = true)]
        [ValidateRange(1, 65535)]
        public int Width { get; set; }
        
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
                    WriteError(new ErrorRecord(e, "New_Image.FromImage", ErrorCategory.InvalidData, Original));
                }
            }
        }
    }
}