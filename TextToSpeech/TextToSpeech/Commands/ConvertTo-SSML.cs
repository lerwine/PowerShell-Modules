using System;
using System.Management.Automation;
using System.Speech.Synthesis;
using System.Xml;

namespace Erwine.Leonard.T.TextToSpeech.Commands
{
    [Cmdlet(VerbsData.ConvertTo, "SSML")]
    [OutputType(typeof(XmlDocument))]
    public class ConvertTo_SSML : PSCmdlet
    {
        public const string ERRORID_UNEXPECTEDERROR = "UnexpectedError";

        [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true)]
        public PromptBuilder[] PromptBuilder { get; set; }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            foreach (PromptBuilder p in PromptBuilder)
            {
                try
                {
                    XmlDocument ssml = new XmlDocument();
                    ssml.LoadXml(p.ToXml().Trim());
                    WriteObject(ssml, false);
                }
                catch (Exception exception)
                {
                    WriteError(new ErrorRecord(exception, ERRORID_UNEXPECTEDERROR, ErrorCategory.WriteError, p));
                    return;
                }
            }
        }
    }
}
