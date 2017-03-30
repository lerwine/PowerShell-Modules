using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Speech.Synthesis;
using System.Text.RegularExpressions;

namespace Speech.Commands
{
    public abstract class TextToSpeechCmdlet : PSCmdlet
    {
        public const string FileExtension_Xml = ".xml";
        public const string FileExtension_Ssml = ".ssml";
        public const string FileExtension_Sml = ".sml";
        public const string FileExtension_Txt = ".txt";
        public static readonly Regex ParagraphSeparatorRegex = new Regex(@"\p{Zp}|(\p{Zl}|\r\n?|\n)(\p{Zl}|\r\n?|\n)+", RegexOptions.Compiled);
        public static readonly Regex SentenceSeparatorRegex = new Regex(@"\p{Zl}|(?<=[^!.:;?\s]\s*[!.:;?])\s+", RegexOptions.Compiled);
        public static readonly Regex OtherSeparatorRegex = new Regex(@"\p{Z}", RegexOptions.Compiled);

        public static string[] SplitParagraphs(string text)
        {
            if (text == null)
                return new string[0];

            return ParagraphSeparatorRegex.Split(text);
        }

        protected static void Convert(PromptBuilder promptBuilder, List<string[]> allParagraphs)
        {
            if (allParagraphs.Count == 1)
            {
                if (allParagraphs[0].Length == 1)
                {
                    promptBuilder.AppendText(allParagraphs[0][0]);
                    return;
                }
                foreach (string s in allParagraphs[0])
                {
                    promptBuilder.StartSentence();
                    promptBuilder.AppendText(s);
                    promptBuilder.EndSentence();
                }
                return;
            }

            foreach (string[] p in allParagraphs)
            {
                promptBuilder.StartParagraph();
                if (p.Length == 1)
                    promptBuilder.AppendText(p[0]);
                else
                {
                    foreach (string s in p)
                    {
                        promptBuilder.StartSentence();
                        promptBuilder.AppendText(s);
                        promptBuilder.EndSentence();
                    }
                }
                promptBuilder.EndParagraph();
            }
        }

        public static string[] SplitSentences(string text)
        {
            if (text == null)
                return new string[0];

            return SentenceSeparatorRegex.Split(text);
        }

        public static IEnumerable<string[]> SplitParagraphsAndSentences(string text)
        {
            return SplitParagraphs(text).Select(p => p.Trim()).Where(p => p.Length > 0)
                .Select(p => SplitSentences(p).Select(s => s.Trim()).Where(s => s.Length > 0).ToArray()).Where(p => p.Length > 0);
        }
    }
}