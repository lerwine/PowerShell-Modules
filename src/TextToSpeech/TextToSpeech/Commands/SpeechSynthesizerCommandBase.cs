using System;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Speech.Synthesis;
using System.Xml;

namespace Erwine.Leonard.T.TextToSpeech.Commands
{
    public abstract class SpeechSynthesizerCommandBase<TResult> : PSCmdlet
        where TResult : class
    {
        public const string PARAMETERSETNAME_PROMPT = "Prompt";
        public const string PARAMETERSETNAME_PROMPTBUILDER = "PromptBuilder";
        public const string PARAMETERSETNAME_TEXT = "Text";
        public const string PARAMETERSETNAME_SSML = "SSML";
        public const string ERRORID_INVALIDSSML = "InvalidSSML";
        public const string ERRORID_UNEXPECTEDERROR = "UnexpectedError";

        [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true, ParameterSetName = PARAMETERSETNAME_TEXT)]
        public string[] Text { get; set; }

        [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true, ParameterSetName = PARAMETERSETNAME_PROMPT)]
        public Prompt[] Prompt { get; set; }

        [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true, ParameterSetName = PARAMETERSETNAME_PROMPTBUILDER)]
        public PromptBuilder[] PromptBuilder { get; set; }

        [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true, ParameterSetName = PARAMETERSETNAME_SSML)]
        public XmlDocument[] SSML { get; set; }

        [Parameter()]
        public ScriptBlock OnSpeakStarted { get; set; }

        [Parameter()]
        public ScriptBlock OnPromptStarted { get; set; }

        [Parameter()]
        public ScriptBlock OnStateChanged { get; set; }

        [Parameter()]
        public ScriptBlock OnSpeakProgress { get; set; }

        [Parameter()]
        public ScriptBlock OnBookmarkReached { get; set; }

        [Parameter()]
        public ScriptBlock OnVoiceChange { get; set; }

        [Parameter()]
        public ScriptBlock OnVisemeReached { get; set; }

        [Parameter()]
        public ScriptBlock OnPhonemeReached { get; set; }

        [Parameter()]
        public ScriptBlock OnPromptCompleted { get; set; }

        [Parameter()]
        public ScriptBlock OnSpeakCompleted { get; set; }

        private Collection<Prompt> _prompts = null;

        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            _prompts = new Collection<Prompt>();
        }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            switch (ParameterSetName)
            {
                case PARAMETERSETNAME_PROMPT:
                    foreach (Prompt prompt in Prompt)
                    {
                        try { _prompts.Add(prompt); }
                        catch (Exception exception)
                        {
                            WriteError(new ErrorRecord(exception, ERRORID_UNEXPECTEDERROR, ErrorCategory.InvalidArgument, prompt));
                        }
                    }
                    break;
                case PARAMETERSETNAME_PROMPTBUILDER:
                    foreach (PromptBuilder promptBuilder in PromptBuilder)
                    {
                        try { _prompts.Add(new Prompt(promptBuilder)); }
                        catch (Exception exception)
                        {
                            WriteError(new ErrorRecord(exception, ERRORID_UNEXPECTEDERROR, ErrorCategory.InvalidArgument, promptBuilder));
                        }
                    }
                    break;
                case PARAMETERSETNAME_SSML:
                    foreach (XmlDocument ssml in SSML)
                    {
                        try { _prompts.Add(new Prompt(ssml.OuterXml, SynthesisTextFormat.Ssml)); }
                        catch (Exception exception)
                        {
                            WriteError(new ErrorRecord(exception, ERRORID_INVALIDSSML, ErrorCategory.InvalidArgument, ssml));
                        }
                    }
                    break;
            }
        }

        protected override void EndProcessing()
        {
            base.EndProcessing();
            bool enumerateCollection;
            TResult result = CreateResult(_prompts, out enumerateCollection);
            if (result != null)
                WriteObject(result, enumerateCollection);
        }

        protected abstract TResult CreateResult(Collection<Prompt> prompts, out bool enumerateCollection);
    }
}