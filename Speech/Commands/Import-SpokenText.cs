using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Speech.Synthesis;
using System.Xml;

namespace Speech.Commands
{
    [Cmdlet(VerbsData.Import, "SpokenText", DefaultParameterSetName = ParameterSetName_Text, RemotingCapability = RemotingCapability.None)]
    [OutputType(typeof(PromptBuilder))]
    public class Import_SpokenText : TextToSpeechCmdlet
    {
        public const string ParameterSetName_Text = "Text";
        public const string ParameterSetName_Path = "Path";
        public const string ParameterSetName_LiteralPath = "LiteralPath";
        public const string ParameterSetName_Ssml = "Ssml";

        [Parameter(Mandatory = true, ValueFromPipeline = true, ParameterSetName = ParameterSetName_Text)]
        [ValidateNotNull()]
        public string[] Text { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = ParameterSetName_Path)]
        [ValidateNotNullOrEmpty()]
        [SupportsWildcards()]
        public string[] Path { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = ParameterSetName_LiteralPath)]
        [ValidateNotNullOrEmpty()]
        public string LiteralPath { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = ParameterSetName_Ssml)]
        [ValidateNotNull()]
        public XmlNode Ssml { get; set; }

        PromptBuilder _promptBuilder = null;

        protected override void BeginProcessing()
        {
            _promptBuilder = new PromptBuilder();
            base.BeginProcessing();
        }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            if (ParameterSetName == ParameterSetName_Text)
            {
                try
                {
                    foreach (string[] paragraph in Text.Where(s => s != null).SelectMany(s => ConvertTo_PromptBuilder.SplitParagraphsAndSentences(s)))
                    {
                        _promptBuilder.StartParagraph();
                        if (paragraph.Length == 1)
                            _promptBuilder.AppendText(paragraph[0]);
                        else
                        {
                            foreach (string s in paragraph)
                            {
                                _promptBuilder.StartSentence();
                                _promptBuilder.AppendText(s);
                                _promptBuilder.EndSentence();
                            }
                        }
                        _promptBuilder.EndParagraph();
                    }
                }
                catch (Exception exception)
                {
                    WriteError(new ErrorRecord(exception, "Import_SpokenText(Text)", ErrorCategory.SyntaxError, Text));
                }
                return;
            }

            if (ParameterSetName == ParameterSetName_Ssml)
            {
                try { _promptBuilder.AppendSsmlMarkup(Ssml.OuterXml); }
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
                    pathCollection = Path.SelectMany(p => GetResolvedProviderPathFromPSPath(p, out provider));
                }
                catch (Exception exception)
                {
                    WriteError(new ErrorRecord(exception, "Import_SpokenText(Path)", ErrorCategory.OpenError, Path));
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
                    try { text = File.ReadAllText(path); }
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
                        try { text = File.ReadAllText(path); }
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
                    try { _promptBuilder.AppendSsmlMarkup(xml.OuterXml); }
                    catch (Exception exception) { WriteError(new ErrorRecord(exception, "Import_SpokenText.OuterXml", ErrorCategory.SyntaxError, path)); }
                    continue;
                }

                try
                {
                    foreach (string[] paragraph in ConvertTo_PromptBuilder.SplitParagraphsAndSentences(text))
                    {
                        _promptBuilder.StartParagraph();
                        if (paragraph.Length == 1)
                            _promptBuilder.AppendText(paragraph[0]);
                        else
                        {
                            foreach (string s in paragraph)
                            {
                                _promptBuilder.StartSentence();
                                _promptBuilder.AppendText(s);
                                _promptBuilder.EndSentence();
                            }
                        }
                        _promptBuilder.EndParagraph();
                    }
                }
                catch (Exception exception)
                {
                    WriteError(new ErrorRecord(exception, "Import_SpokenText.Text", ErrorCategory.SyntaxError, Text));
                }
            }
        }
    }
}
