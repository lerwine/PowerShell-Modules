using System.Collections.Generic;
using System.Management.Automation;
using System.Speech.Synthesis;
using System.Xml;

namespace Speech.Commands
{
    [Cmdlet(VerbsData.ConvertTo, "Ssml", RemotingCapability = RemotingCapability.None)]
    [OutputType(typeof(XmlDocument))]
    public class ConvertTo_Ssml : TextToSpeechCmdlet
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        [ValidateNotNull()]
        [AllowEmptyString()]
        public string[] InputText { get; set; }

        List<string[]> _allParagraphs = new List<string[]>();

        protected override void BeginProcessing()
        {
            _allParagraphs.Clear();
            base.BeginProcessing();
        }

        protected override void ProcessRecord()
        {
            foreach (string s in InputText)
                _allParagraphs.AddRange(SplitParagraphsAndSentences(s));
            base.ProcessRecord();
        }

        protected override void EndProcessing()
        {
            PromptBuilder promptBuilder = new PromptBuilder();
            Convert(promptBuilder, _allParagraphs);

            base.EndProcessing();
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(promptBuilder.ToXml());
            WriteObject(xmlDocument);
        }
    }
}
