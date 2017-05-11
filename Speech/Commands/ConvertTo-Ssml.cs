using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Speech.Synthesis;
using System.Xml;

namespace Speech.Commands
{
    /// <summary>
    /// Converts plain text to SSML Markup.
    /// </summary>
    [Cmdlet(VerbsData.ConvertTo, "Ssml", RemotingCapability = RemotingCapability.None)]
    [OutputType(typeof(XmlDocument))]
    public class ConvertTo_Ssml : TextToSpeechCmdlet
    {
        /// <summary>
        /// Plain text to be converted.
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        [ValidateNotNull()]
        [AllowEmptyString()]
        public string[] InputText { get; set; }

        List<string[]> _allParagraphs = new List<string[]>();
        
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        protected override void BeginProcessing()
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            _allParagraphs.Clear();
            base.BeginProcessing();
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        protected override void ProcessRecord()
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            foreach (string s in InputText)
                _allParagraphs.AddRange(SplitParagraphsAndSentences(s).Select(p => p.ToArray()));
            base.ProcessRecord();
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        protected override void EndProcessing()
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            PromptBuilder promptBuilder = new PromptBuilder();
            AppendParagraphs(promptBuilder, _allParagraphs);

            base.EndProcessing();
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(promptBuilder.ToXml());
            WriteObject(xmlDocument);
        }
    }
}
