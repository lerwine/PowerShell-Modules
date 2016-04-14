using System;
using System.Text;
using System.Text.RegularExpressions;

namespace IOUtilityCLR
{
    public class EncodeRegexReplaceHandler : RegexReplaceHandler
    {
        public EncodeRegexReplaceHandler(Regex regex) : base(regex) { }

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
