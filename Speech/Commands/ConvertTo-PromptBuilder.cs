using System.Collections.Generic;
using System.Management.Automation;
using System.Speech.Synthesis;

namespace Speech.Commands
{
    [Cmdlet(VerbsData.ConvertTo, "PromptBuilder", DefaultParameterSetName = ParameterSetName_NewPromptBuilder, RemotingCapability = RemotingCapability.None)]
    [OutputType(typeof(PromptBuilder))]
    public class ConvertTo_PromptBuilder : TextToSpeechCmdlet
    {
        public const string ParameterSetName_NewPromptBuilder = "NewPromptBuilder";
        public const string ParameterSetName_ExistingPromptBuilder = "ExistingPromptBuilder";

        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        [ValidateNotNull()]
        [AllowEmptyString()]
        public string[] InputText { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = ParameterSetName_ExistingPromptBuilder)]
        [ValidateNotNull()]
        public PromptBuilder PromptBuilder { get; set; }

        [Parameter(ParameterSetName = ParameterSetName_ExistingPromptBuilder)]
        public SwitchParameter PassThru { get; set; }

        private PromptBuilder _promptBuilder = null;

        List<string[]> _allParagraphs = new List<string[]>();

        protected override void BeginProcessing()
        {
            _promptBuilder = (ParameterSetName == ParameterSetName_ExistingPromptBuilder) ? PromptBuilder : new PromptBuilder();
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
            Convert(_promptBuilder, _allParagraphs);

            base.EndProcessing();

            if (ParameterSetName == ParameterSetName_NewPromptBuilder || PassThru.IsPresent)
                WriteObject(_promptBuilder);
        }
    }
}
