using System;
using System.Text;
using System.Text.RegularExpressions;

namespace WinIOUtility
{
    /// <summary>
    /// Encodes text according to a specific pattern.
    /// </summary>
    public class EncodeRegexReplaceHandler : RegexReplaceHandler
    {
        /// <summary>
        /// Initialize new <see cref="EncodeRegexReplaceHandler" />.
        /// </summary>
        /// <param name="pattern">Regular Expression pattern of text to encode.</param>
        /// <param name="options">Regular expression pattern options.</param>
        public EncodeRegexReplaceHandler(string pattern, RegexOptions options) : base(pattern, options) { }

        /// <summary>
        /// Initialize new <see cref="EncodeRegexReplaceHandler" />.
        /// </summary>
        /// <param name="pattern">Regular Expression pattern of text to encode.</param>
        public EncodeRegexReplaceHandler(string pattern) : base(pattern) { }

        /// <summary>
        /// Initialize new <see cref="EncodeRegexReplaceHandler" />.
        /// </summary>
        /// <param name="regex">Regular Expression object which matches text to encode.</param>
        public EncodeRegexReplaceHandler(Regex regex) : base(regex) { }

        /// <summary>
        /// Returns encoded text.
        /// </summary>
        /// <param name="match">Current regular expression match to be replaced.</param>
        /// <returns>Text with matches encoded.</returns>
        protected override string Evaluator(Match match)
        {
            if (match.Length == 1)
                return String.Format("_0x{0:X4}_", (int)(match.Value[0]));

            StringBuilder result = new StringBuilder();
            foreach (char c in match.Value)
                result.AppendFormat("_0x{0:X4}_", (int)c);
            return result.ToString();
        }
    }
}
