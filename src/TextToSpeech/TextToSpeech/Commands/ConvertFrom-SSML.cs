using System;
using System.Management.Automation;
using System.Speech.Synthesis;
using System.Xml;

namespace Erwine.Leonard.T.TextToSpeech.Commands
{
    [Cmdlet(VerbsData.ConvertFrom, "SSML")]
    [OutputType(typeof(PromptBuilder))]
    public class ConvertFrom_SSML : PSCmdlet
    {
        public const string ERRORID_UNEXPECTEDERROR = "UnexpectedError";

        [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true)]
        public XmlDocument[] SSML { get; set; }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            foreach (XmlDocument ssml in SSML)
            {
                try
                {
                    PromptBuilder promptBuilder = new PromptBuilder();
                    promptBuilder.AppendSsmlMarkup(ssml.OuterXml);
                }
                catch (Exception exception)
                {
                    WriteError(new ErrorRecord(exception, ERRORID_UNEXPECTEDERROR, ErrorCategory.ReadError, ssml));
                    return;
                }
            }
        }
    }
}
