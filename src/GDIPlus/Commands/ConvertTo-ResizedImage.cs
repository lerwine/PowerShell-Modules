using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Management.Automation;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Erwine.Leonard.T.GDIPlus.Commands
#pragma warning restore IDE0130 // Namespace does not match folder structure
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

        [Parameter()]
        public Color BackgroundColor { get; set; }

        [Parameter()]
        public Brush Background{ get; set; }

        [Parameter()]
        public SwitchParameter KeepAspectRatio { get; set; }

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
                    using (Graphics g = Graphics.FromImage(target))
                    {
                        if (MyInvocation.BoundParameters.ContainsKey("BackgroundColor"))
                            g.FillRectangle(new SolidBrush(BackgroundColor), new Rectangle(0, 0, Width, Height));
                        else if (MyInvocation.BoundParameters.ContainsKey("Background"))
                            g.FillRectangle(Background, new Rectangle(0, 0, Width, Height));
                        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        if (KeepAspectRatio.IsPresent)
                        {
                            float sw = Convert.ToSingle(Width);
                            float sh = Convert.ToSingle(Height);
                            float tw = Convert.ToSingle(target.Width);
                            float th = Convert.ToSingle(target.Height);
                            float rh = th / sh;
                            float rw = tw / sw;
                            RectangleF dest;
                            if (rw < rh)
                                dest = new RectangleF(0.0f, (th - sh) / 2.0f, sw * rw, sh * rw);
                            else
                                dest = new RectangleF((tw - sw) / 2.0f, 0.0f, sw * rh, sh * rh);
                            g.DrawImage(image, dest, new RectangleF(0.0f, 0.0f, Convert.ToSingle(image.Width), 
                                Convert.ToSingle(image.Height)), GraphicsUnit.Pixel);
                        }
                        else
                            g.DrawImage(image, new Rectangle(0, 0, image.Width, image.Height),
                                new Rectangle(0, 0, image.Width, image.Height), GraphicsUnit.Pixel);
                        g.Flush();
                    }
                    WriteObject(image);
                }
                catch (Exception e)
                {
                    WriteError(new ErrorRecord(e, "New_Image.FromImage", ErrorCategory.InvalidData, image));
                }
            }
        }
    }
}