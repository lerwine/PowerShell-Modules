using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Speech.Synthesis;

namespace Speech.Commands
{
    /// <summary>
    /// Converts plain text to a <seealso cref="System.Speech.Synthesis.PromptBuilder"/>.
    /// </summary>
    /// <remarks>
    /// <para type="description">Creates a new <seealso cref="System.Speech.Synthesis.PromptBuilder"/> from the provided plain text or appends text to a
    /// provided <seealso cref="System.Speech.Synthesis.PromptBuilder"/>.</para>
    /// </remarks>
    [Cmdlet(VerbsData.ConvertTo, "PromptBuilder", DefaultParameterSetName = ParameterSetName_NewPromptBuilder, RemotingCapability = RemotingCapability.None)]
    [OutputType(typeof(PromptBuilder))]
    public class ConvertTo_PromptBuilder : TextToSpeechCmdlet
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public const string ParameterSetName_NewPromptBuilder = "NewPromptBuilder";
        public const string ParameterSetName_ExistingPromptBuilder = "ExistingPromptBuilder";
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// Text to be imported into the resulting <seealso cref="System.Speech.Synthesis.PromptBuilder"/>.
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        [ValidateNotNull()]
        [AllowEmptyString()]
        public string[] InputText { get; set; }

        /// <summary>
        /// <seealso cref="System.Speech.Synthesis.PromptBuilder"/> object to append to.
        /// </summary>
        [Parameter(Mandatory = true, ParameterSetName = ParameterSetName_ExistingPromptBuilder)]
        [ValidateNotNull()]
        public PromptBuilder PromptBuilder { get; set; }

        /// <summary>
        /// Returns the <seealso cref="System.Speech.Synthesis.PromptBuilder"/> that was passed.
        /// </summary>
        [Parameter(ParameterSetName = ParameterSetName_ExistingPromptBuilder)]
        public SwitchParameter PassThru { get; set; }

        private PromptBuilder _promptBuilder = null;

        List<string[]> _allParagraphs = new List<string[]>();
        
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        protected override void BeginProcessing()
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            _promptBuilder = (ParameterSetName == ParameterSetName_ExistingPromptBuilder) ? PromptBuilder : new PromptBuilder();
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
            AppendParagraphs(_promptBuilder, _allParagraphs);

            base.EndProcessing();

            if (ParameterSetName == ParameterSetName_NewPromptBuilder || PassThru.IsPresent)
                WriteObject(_promptBuilder);
        }
    }
}
