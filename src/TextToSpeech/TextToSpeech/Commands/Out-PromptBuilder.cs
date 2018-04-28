using System;
using System.Management.Automation;
using System.Speech.Synthesis;
using System.Xml;

namespace Erwine.Leonard.T.TextToSpeech.Commands
{
    [Cmdlet(VerbsData.Out, "PromptBuilder", DefaultParameterSetName = PARAMETERSETNAME_TEXT)]
    [OutputType(typeof(PromptBuilder))]
    public class Out_PromptBuilder : PSCmdlet
    {
        public const string PARAMETERSETNAME_INTEXT = "InText";
        public const string PARAMETERSETNAME_TEXT = "Text";
        public const string PARAMETERSETNAME_TEXTRATE = "TextRate";
        public const string PARAMETERSETNAME_TEXTVOLUME = "TextVolume";
        public const string PARAMETERSETNAME_TEXTALIAS = "TextAlias";
        public const string PARAMETERSETNAME_TEXTSAYAS = "TextSayAs";
        public const string PARAMETERSETNAME_TEXTHINT = "TextHint";
        public const string PARAMETERSETNAME_TEXTPRONOUNCED = "TextPronounced";
        public const string PARAMETERSETNAME_AUDIO = "Audio";
        public const string PARAMETERSETNAME_BREAK = "Break";
        public const string PARAMETERSETNAME_PAUSE = "Pause";
        public const string PARAMETERSETNAME_SSML = "SSML";
        public const string PARAMETERSETNAME_SSMLREADER = "SsmlReader";
        public const string PARAMETERSETNAME_MARKUP = "Markup";
        public const string PARAMETERSETNAME_STARTSTYLE = "StartStyle";
        public const string PARAMETERSETNAME_STARTVOICE = "StartVoice";
        public const string PARAMETERSETNAME_STARTGENDER = "StartGender";
        public const string PARAMETERSETNAME_STARTPARAGRAPH = "StartParagraph";
        public const string PARAMETERSETNAME_ENDPARAGRAPH = "EndParagraph";
        public const string PARAMETERSETNAME_STARTSENTENCE = "StartSentence";
        public const string PARAMETERSETNAME_ENDSENTENCE = "EndSentence";
        public const string PARAMETERSETNAME_ENDSTYLE = "EndStyle";
        public const string PARAMETERSETNAME_ENDVOICE = "EndVoice";
        public const string ERRORID_INVALIDPATH = "InvalidPath";
        public const string ERRORID_UNEXPECTEDERROR = "UnexpectedError";

        [Parameter(Mandatory = true, ValueFromPipeline = true, ParameterSetName = PARAMETERSETNAME_INTEXT)]
        public string[] InputText { get; set; }

        [Parameter(Mandatory = true, Position = 0, ParameterSetName = PARAMETERSETNAME_TEXT)]
        [Parameter(Mandatory = true, Position = 0, ParameterSetName = PARAMETERSETNAME_TEXTRATE)]
        [Parameter(Mandatory = true, Position = 0, ParameterSetName = PARAMETERSETNAME_TEXTVOLUME)]
        [Parameter(Mandatory = true, Position = 0, ParameterSetName = PARAMETERSETNAME_TEXTALIAS)]
        [Parameter(Mandatory = true, Position = 0, ParameterSetName = PARAMETERSETNAME_TEXTSAYAS)]
        [Parameter(Mandatory = true, Position = 0, ParameterSetName = PARAMETERSETNAME_TEXTHINT)]
        [Parameter(Mandatory = true, Position = 0, ParameterSetName = PARAMETERSETNAME_TEXTPRONOUNCED)]
        public string Text { get; set; }
        
        [Parameter(Position = 1, ParameterSetName = PARAMETERSETNAME_TEXT)]
        public PromptEmphasis Emphasis { get; set; }
        
        [Parameter(Mandatory = true, Position = 1, ParameterSetName = PARAMETERSETNAME_TEXTRATE)]
        public PromptRate Rate { get; set; }

        [Parameter(Mandatory = true, Position = 1, ParameterSetName = PARAMETERSETNAME_TEXTVOLUME)]
        public PromptVolume Volume { get; set; }

        [Parameter(Mandatory = true, Position = 1, ParameterSetName = PARAMETERSETNAME_TEXTALIAS)]
        public string Alias { get; set; }

        [Parameter(Mandatory = true, Position = 1, ParameterSetName = PARAMETERSETNAME_TEXTSAYAS)]
        public SayAs SayAs { get; set; }

        [Parameter(Mandatory = true, Position = 1, ParameterSetName = PARAMETERSETNAME_TEXTHINT)]
        public string Hint { get; set; }

        [Parameter(Mandatory = true, Position = 1, ParameterSetName = PARAMETERSETNAME_TEXTPRONOUNCED)]
        public string Pronounced { get; set; }

        [Parameter(Mandatory = true, Position = 0, ParameterSetName = PARAMETERSETNAME_AUDIO)]
        public string Audio { get; set; }
        
        [Parameter(Position = 1, ParameterSetName = PARAMETERSETNAME_AUDIO)]
        public string AlternateText { get; set; }

        [Parameter()]
        public string Bookmark { get; set; }

        [Parameter(Mandatory = true, Position = 0, ParameterSetName = PARAMETERSETNAME_BREAK)]
        public PromptBreak Break { get; set; }

        [Parameter(Mandatory = true, Position = 0, ParameterSetName = PARAMETERSETNAME_PAUSE)]
        public TimeSpan Pause { get; set; }

        [Parameter(Mandatory = true, Position = 0, ParameterSetName = PARAMETERSETNAME_SSML)]
        public string SSML { get; set; }

        [Parameter(Mandatory = true, Position = 0, ParameterSetName = PARAMETERSETNAME_SSMLREADER)]
        public XmlReader SsmlReader { get; set; }

        [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true, ParameterSetName = PARAMETERSETNAME_MARKUP)]
        public XmlDocument[] Markup { get; set; }

        [Parameter(Mandatory = true, Position = 0, ParameterSetName = PARAMETERSETNAME_STARTSTYLE)]
        public PromptStyle StartStyle { get; set; }

        [Parameter(Mandatory = true, Position = 0, ParameterSetName = PARAMETERSETNAME_STARTVOICE)]
        public string StartVoice { get; set; }

        [Parameter(Mandatory = true, Position = 0, ParameterSetName = PARAMETERSETNAME_STARTGENDER)]
        public VoiceGender StartGender { get; set; }

        [Parameter(Position = 1, ParameterSetName = PARAMETERSETNAME_STARTGENDER)]
        public VoiceAge Age { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = PARAMETERSETNAME_STARTPARAGRAPH)]
        public SwitchParameter StartParagraph { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = PARAMETERSETNAME_ENDPARAGRAPH)]
        public SwitchParameter EndParagraph { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = PARAMETERSETNAME_STARTSENTENCE)]
        public SwitchParameter StartSentence { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = PARAMETERSETNAME_ENDSENTENCE)]
        public SwitchParameter EndSentence { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = PARAMETERSETNAME_ENDSTYLE)]
        public SwitchParameter EndStyle { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = PARAMETERSETNAME_ENDVOICE)]
        [Alias("EndGender")]
        public SwitchParameter EndVoice{ get; set; }

        [Parameter()]
        public PromptBuilder PromptBuilder { get; set; }

        private PromptBuilder _promptBuilder = null;
        private bool _writeBuilder = false;
        
        protected override void BeginProcessing()
        {
            base.BeginProcessing();

            _writeBuilder = MyInvocation.BoundParameters.ContainsKey("PromptBuilder");
            _promptBuilder = (_writeBuilder) ? PromptBuilder : new PromptBuilder();
        }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            object targetObject = null;
            try
            {
                switch (ParameterSetName)
                {
                    case PARAMETERSETNAME_INTEXT:
                        targetObject = InputText;
                        foreach (string t in InputText)
                            _promptBuilder.AppendText(t);
                        break;
                    case PARAMETERSETNAME_TEXTRATE:
                        targetObject = Text;
                        _promptBuilder.AppendText(Text, Rate);
                        break;
                    case PARAMETERSETNAME_TEXTVOLUME:
                        targetObject = Text;
                        _promptBuilder.AppendText(Text, Volume);
                        break;
                    case PARAMETERSETNAME_TEXTALIAS:
                        targetObject = Text;
                        _promptBuilder.AppendTextWithAlias(Text, Alias);
                        break;
                    case PARAMETERSETNAME_TEXTSAYAS:
                        targetObject = Text;
                        _promptBuilder.AppendTextWithHint(Text, SayAs);
                        break;
                    case PARAMETERSETNAME_TEXTHINT:
                        targetObject = Text;
                        _promptBuilder.AppendTextWithHint(Text, Hint);
                        break;
                    case PARAMETERSETNAME_TEXTPRONOUNCED:
                        targetObject = Text;
                        _promptBuilder.AppendTextWithPronunciation(Text, Pronounced);
                        break;
                    case PARAMETERSETNAME_AUDIO:
                        targetObject = Audio;
                        if (MyInvocation.BoundParameters.ContainsKey("AlternateText"))
                        {
                            Uri uri;
                            if (!Uri.TryCreate(Audio, UriKind.Absolute, out uri))
                            {
                                try
                                {
                                    if (!Uri.TryCreate(System.IO.Path.GetFullPath(Audio), UriKind.Absolute, out uri))
                                        throw new PSArgumentException("Invalid Audio path", "Audio");
                                }
                                catch (Exception exception)
                                {
                                    WriteError(new ErrorRecord(exception, ERRORID_INVALIDPATH, ErrorCategory.InvalidArgument, targetObject));
                                    return;
                                }
                            }
                            _promptBuilder.AppendAudio(uri, AlternateText);
                        }
                        else
                            _promptBuilder.AppendText(Audio);
                        break;
                    case PARAMETERSETNAME_BREAK:
                        if (Break == PromptBreak.None)
                            _promptBuilder.AppendBreak();
                        else
                            _promptBuilder.AppendBreak(Break);
                        targetObject = Break;
                        break;
                    case PARAMETERSETNAME_PAUSE:
                        targetObject = Pause;
                        _promptBuilder.AppendBreak(Pause);
                        break;
                    case PARAMETERSETNAME_SSML:
                        targetObject = SSML;
                        _promptBuilder.AppendSsml(SSML);
                        break;
                    case PARAMETERSETNAME_SSMLREADER:
                        targetObject = SsmlReader;
                        _promptBuilder.AppendSsml(SsmlReader);
                        break;
                    case PARAMETERSETNAME_MARKUP:
                        targetObject = Markup;
                        foreach (XmlDocument markup in Markup)
                            _promptBuilder.AppendSsmlMarkup(markup.OuterXml);
                        break;
                    case PARAMETERSETNAME_STARTSTYLE:
                        targetObject = StartStyle;
                        _promptBuilder.StartStyle(StartStyle);
                        break;
                    case PARAMETERSETNAME_STARTVOICE:
                        targetObject = StartVoice;
                        _promptBuilder.StartVoice(StartVoice);
                        break;
                    case PARAMETERSETNAME_STARTGENDER:
                        targetObject = StartGender;
                        _promptBuilder.StartVoice(StartGender);
                        break;
                    case PARAMETERSETNAME_STARTPARAGRAPH:
                        targetObject = null;
                        _promptBuilder.StartParagraph();
                        break;
                    case PARAMETERSETNAME_ENDPARAGRAPH:
                        targetObject = null;
                        _promptBuilder.EndParagraph();
                        break;
                    case PARAMETERSETNAME_STARTSENTENCE:
                        targetObject = null;
                        _promptBuilder.StartSentence();
                        break;
                    case PARAMETERSETNAME_ENDSENTENCE:
                        targetObject = null;
                        _promptBuilder.EndSentence();
                        break;
                    case PARAMETERSETNAME_ENDSTYLE:
                        targetObject = null;
                        _promptBuilder.EndStyle();
                        break;
                    case PARAMETERSETNAME_ENDVOICE:
                        targetObject = null;
                        _promptBuilder.EndVoice();
                        break;
                    default:
                        targetObject = Text;
                        if (MyInvocation.BoundParameters.ContainsKey("Emphasis"))
                            _promptBuilder.AppendText(Text, Emphasis);
                        else
                            _promptBuilder.AppendText(Text);
                        break;
                }
                _writeBuilder = true;
            }
            catch (Exception exception)
            {
                WriteError(new ErrorRecord(exception, ERRORID_UNEXPECTEDERROR, ErrorCategory.OpenError, targetObject));
                return;
            }
        }

        protected override void EndProcessing()
        {
            base.EndProcessing();
            if (_writeBuilder)
                WriteObject(_promptBuilder);
        }
    }
}
