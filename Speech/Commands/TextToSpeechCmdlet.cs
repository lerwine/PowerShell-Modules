using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Speech.Synthesis;
using System.Text.RegularExpressions;

namespace Speech.Commands
{
    /// <summary>
    /// Base class for Text-To-Speech cmdlets.
    /// </summary>
    public abstract class TextToSpeechCmdlet : PSCmdlet
    {
        /// <summary>
        /// File extension for XML files.
        /// </summary>
        public const string FileExtension_Xml = ".xml";

        /// <summary>
        /// File extension for SSML markup Files.
        /// </summary>
        public const string FileExtension_Ssml = ".ssml";

        /// <summary>
        /// 3-letter file extension for SSML markup Files.
        /// </summary>
        public const string FileExtension_Sml = ".sml";

        /// <summary>
        /// File extension for plain text files.
        /// </summary>
        public const string FileExtension_Txt = ".txt";

        /// <summary>
        /// Matches text which indicates a paragraph separation.
        /// </summary>
        public static readonly Regex ParagraphSeparatorRegex = new Regex(@"\p{Zp}|(\p{Zl}|\r\n?|\n)(\p{Zl}|\r\n?|\n)+", RegexOptions.Compiled);

        /// <summary>
        /// Matches text which indicates a sentence separation.
        /// </summary>
        public static readonly Regex SentenceSeparatorRegex = new Regex(@"\p{Zl}|(?<=[^!.:;?\s]\s*[!.:;?])\s+", RegexOptions.Compiled);

        /// <summary>
        /// Matches text which indicates any separator.
        /// </summary>
        public static readonly Regex OtherSeparatorRegex = new Regex(@"\p{Z}", RegexOptions.Compiled);

        /// <summary>
        /// Appends paragraphs and sentences to a <seealso cref="PromptBuilder"/> object.
        /// </summary>
        /// <param name="promptBuilder"><seealso cref="PromptBuilder"/> object to append to.</param>
        /// <param name="paragraphs">Collection where each element is a collection of sentences.</param>
        protected static void AppendParagraphs(PromptBuilder promptBuilder, IEnumerable<IEnumerable<string>> paragraphs)
        {
            if (promptBuilder == null)
                throw new ArgumentNullException("promptBuilder");

            if (paragraphs == null)
                return;

            string[][] normalized = paragraphs.Where(p => p != null).Select(p => p.Where(s => s != null).Select(s => s.TrimEnd()).Where(s => s.Length > 0).ToArray())
                .Where(p => p.Length > 0).ToArray();

            if (normalized.Length == 1)
            {
                if (normalized[0].Length == 1)
                {
                    promptBuilder.AppendText(normalized[0][0]);
                    return;
                }
                foreach (string s in normalized[0])
                {
                    promptBuilder.StartSentence();
                    promptBuilder.AppendText(s);
                    promptBuilder.EndSentence();
                }
                return;
            }

            foreach (string[] p in normalized)
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

        /// <summary>
        /// Splits text at the end of each match.
        /// </summary>
        /// <param name="text">Text to be parsed.</param>
        /// <param name="regex">Expression used to split text.</param>
        /// <returns>Array of string values which are split at each match.</returns>
        public static IEnumerable<string> SplitAfterMatches(string text, Regex regex)
        {
            if (regex == null)
                throw new ArgumentNullException("regex");

            if (text == null)
                yield break;

            MatchCollection matches = regex.Matches(text);
            if (matches.Count == 0)
            {
                yield return text;
                yield break;
            }

            int index = 0;
            foreach (Match m in matches)
            {
                yield return text.Substring(index, (m.Index - index) + m.Length);
                index = m.Index + m.Length;
            }
            if (index < text.Length)
                yield return text.Substring(index);
        }

        /// <summary>
        /// Splits text along paragraph boundaries.
        /// </summary>
        /// <param name="text">Text to be parsed.</param>
        /// <returns>Text split by paragraph boundaries.</returns>
        public static IEnumerable<string> SplitParagraphs(string text)
        {
            return SplitAfterMatches(text, ParagraphSeparatorRegex);
        }

        /// <summary>
        /// Splits text
        /// </summary>
        /// <param name="text">Text to be parsed.</param>
        /// <returns>Text split by sentence boundaries.</returns>
        public static IEnumerable<string> SplitSentences(string text)
        {
            return SplitAfterMatches(text, SentenceSeparatorRegex);
        }

        /// <summary>
        /// Splits text into paragraphs and sentences
        /// </summary>
        /// <param name="text">Text to be parsed.</param>
        /// <returns>Text split into paragraphs and then split into sentences.</returns>
        public static IEnumerable<IEnumerable<string>> SplitParagraphsAndSentences(string text)
        {
            return SplitParagraphs(text).Select(p => SplitSentences(p));
        }
    }
}