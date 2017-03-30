using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Speech.AudioFormat;
using System.Speech.Synthesis;
using System.Xml;

namespace Speech.Commands
{
    [Cmdlet(VerbsLifecycle.Start, "TextToSpeech", RemotingCapability = RemotingCapability.None)]
    [OutputType(typeof(TextToSpeechJob))]
    public class Start_TextToSpeech : TextToSpeechCmdlet
    {
        public const string ParameterSetName_Text = "Text";
        public const string ParameterSetName_Path = "Path";
        public const string ParameterSetName_LiteralPath = "LiteralPath";
        public const string ParameterSetName_Ssml = "Ssml";
        public const string ParameterSetName_PromptBuilder = "PromptBuilder";
        public const string ParameterSetName_Prompt = "Prompt";

        [Parameter(Mandatory = true, ValueFromPipeline = true, ParameterSetName = ParameterSetName_Text)]
        [ValidateNotNull()]
        public string[] Text { get; set; }

        [Parameter(Mandatory = true, ValueFromPipeline = true, ParameterSetName = ParameterSetName_PromptBuilder)]
        [ValidateNotNull()]
        public PromptBuilder[] PromptBuilder { get; set; }

        [Parameter(Mandatory = true, ValueFromPipeline = true, ParameterSetName = ParameterSetName_Prompt)]
        [ValidateNotNull()]
        public Prompt[] Prompt { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = ParameterSetName_Path)]
        [ValidateNotNullOrEmpty()]
        [SupportsWildcards()]
        public string[] InputPath { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = ParameterSetName_LiteralPath)]
        [ValidateNotNullOrEmpty()]
        public string LiteralPath { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = ParameterSetName_Ssml)]
        [ValidateNotNull()]
        public XmlNode Ssml { get; set; }

        [Parameter(HelpMessage = "")]
        [ValidateNotNullOrEmpty()]
        public string OutputPath { get; set; }

        [Parameter(HelpMessage = "")]
        public SpeechAudioFormatInfo AudioFormat { get; set; }

        private int? _volume = null;
        private int? _rate = null;

        [Parameter()]
        [ValidateRange(0, 100)]
        public int Volume { get { return (_volume.HasValue) ? _volume.Value : 50; } set { _volume = value; } }

        [Parameter()]
        [ValidateRange(-10, 10)]
        public int Rate { get { return (_rate.HasValue) ? _rate.Value : 0; } set { _rate = value; } }

        [Parameter()]
        [ValidateNotNullOrEmpty()]
        public string VoiceName { get; set; }

        [Parameter()]
        [AllowNull()]
        public object AsyncState { get; set; }

        [Parameter()]
        public SwitchParameter AsPaused { get; set; }

        Collection<Prompt> _prompts = new Collection<Prompt>();

        protected override void BeginProcessing()
        {
            _prompts.Clear();
            base.BeginProcessing();
        }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            if (ParameterSetName == ParameterSetName_Prompt)
            {
                foreach (Prompt p in Prompt)
                    _prompts.Add(p);
                return;
            }

            if (ParameterSetName == ParameterSetName_PromptBuilder)
            {
                foreach (PromptBuilder p in PromptBuilder)
                    _prompts.Add(new Prompt(p));
                return;

            }

            if (ParameterSetName == ParameterSetName_Text)
            {
                try
                {
                    PromptBuilder promptBuilder = new PromptBuilder();
                    foreach (string[] paragraph in Text.Where(s => s != null).SelectMany(s => ConvertTo_PromptBuilder.SplitParagraphsAndSentences(s)))
                    {
                        promptBuilder.StartParagraph();
                        if (paragraph.Length == 1)
                            promptBuilder.AppendText(paragraph[0]);
                        else
                        {
                            foreach (string s in paragraph)
                            {
                                promptBuilder.StartSentence();
                                promptBuilder.AppendText(s);
                                promptBuilder.EndSentence();
                            }
                        }
                        promptBuilder.EndParagraph();
                    }
                    _prompts.Add(new Prompt(promptBuilder));
                }
                catch (Exception exception)
                {
                    WriteError(new ErrorRecord(exception, "Import_SpokenText(Text)", ErrorCategory.SyntaxError, Text));
                }
                return;
            }

            if (ParameterSetName == ParameterSetName_Ssml)
            {
                try
                {
                    PromptBuilder promptBuilder = new PromptBuilder();
                    promptBuilder.AppendSsmlMarkup(Ssml.OuterXml);
                    _prompts.Add(new Prompt(promptBuilder));
                }
                catch (Exception exception)
                {
                    WriteError(new ErrorRecord(exception, "Import_SpokenText(Ssml)", ErrorCategory.SyntaxError, Ssml));
                }
                return;
            }

            IEnumerable<string> pathCollection;
            if (ParameterSetName == ParameterSetName_Path)
            {
                try
                {
                    ProviderInfo provider;
                    pathCollection = InputPath.SelectMany(p => GetResolvedProviderPathFromPSPath(p, out provider));
                }
                catch (Exception exception)
                {
                    WriteError(new ErrorRecord(exception, "Import_SpokenText(Path)", ErrorCategory.OpenError, InputPath));
                    return;
                }
            }
            else
            {
                try { pathCollection = new string[] { GetUnresolvedProviderPathFromPSPath(LiteralPath) }; }
                catch (Exception exception)
                {
                    WriteError(new ErrorRecord(exception, "Import_SpokenText(LiteralPath)", ErrorCategory.OpenError, LiteralPath));
                    return;
                }
            }

            foreach (string path in pathCollection)
            {
                if (String.IsNullOrWhiteSpace(path))
                    continue;
                string extension;
                try { extension = System.IO.Path.GetExtension(path); }
                catch (Exception exception)
                {
                    WriteError(new ErrorRecord(exception, "Import_SpokenText.GetExtension", ErrorCategory.InvalidArgument, path));
                    continue;
                }
                string text;
                XmlDocument xml;
                if (String.Equals(extension, FileExtension_Txt, StringComparison.InvariantCultureIgnoreCase))
                {
                    try { text = System.IO.File.ReadAllText(path); }
                    catch (Exception exception)
                    {
                        WriteError(new ErrorRecord(exception, "Import_SpokenText.ReadText", ErrorCategory.ReadError, path));
                        continue;
                    }
                    xml = null;
                }
                else if (String.Equals(extension, FileExtension_Xml, StringComparison.InvariantCultureIgnoreCase) || String.Equals(extension, FileExtension_Ssml, StringComparison.InvariantCultureIgnoreCase) || String.Equals(extension, FileExtension_Sml, StringComparison.InvariantCultureIgnoreCase))
                {
                    xml = new XmlDocument();
                    try { xml.Load(path); }
                    catch (Exception exception)
                    {
                        WriteError(new ErrorRecord(exception, "Import_SpokenText.ReadXml", ErrorCategory.ReadError, path));
                        continue;
                    }
                    text = null;
                }
                else
                {
                    try
                    {
                        xml = new XmlDocument();
                        xml.Load(path);
                        text = null;
                    }
                    catch
                    {
                        try { text = System.IO.File.ReadAllText(path); }
                        catch (Exception exception)
                        {
                            WriteError(new ErrorRecord(exception, "Import_SpokenText.ReadFile", ErrorCategory.ReadError, path));
                            continue;
                        }
                        xml = null;
                    }
                }

                if (text == null)
                {
                    try
                    {
                        PromptBuilder promptBuilder = new PromptBuilder();
                        promptBuilder.AppendSsmlMarkup(xml.OuterXml);
                        _prompts.Add(new Prompt(promptBuilder));
                    }
                    catch (Exception exception) { WriteError(new ErrorRecord(exception, "Import_SpokenText.OuterXml", ErrorCategory.SyntaxError, path)); }
                    continue;
                }

                try
                {
                    PromptBuilder promptBuilder = new PromptBuilder();
                    foreach (string[] paragraph in ConvertTo_PromptBuilder.SplitParagraphsAndSentences(text))
                    {
                        promptBuilder.StartParagraph();
                        if (paragraph.Length == 1)
                            promptBuilder.AppendText(paragraph[0]);
                        else
                        {
                            foreach (string s in paragraph)
                            {
                                promptBuilder.StartSentence();
                                promptBuilder.AppendText(s);
                                promptBuilder.EndSentence();
                            }
                        }
                        promptBuilder.EndParagraph();
                    }
                    _prompts.Add(new Prompt(promptBuilder));
                }
                catch (Exception exception)
                {
                    WriteError(new ErrorRecord(exception, "Import_SpokenText.Text", ErrorCategory.SyntaxError, Text));
                }
            }
        }

        protected override void EndProcessing()
        {
            WriteObject(new TextToSpeechJob(_prompts, AsPaused.IsPresent, _rate, _volume, VoiceName, OutputPath, AudioFormat, AsyncState));
            base.EndProcessing();
        }
    }
}
