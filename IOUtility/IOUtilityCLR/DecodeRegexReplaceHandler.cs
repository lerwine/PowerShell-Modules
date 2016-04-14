using System;
using System.Text.RegularExpressions;

namespace IOUtilityCLR
{
    public class DecodeRegexReplaceHandler : RegexReplaceHandler
    {
        public DecodeRegexReplaceHandler() : base(RegularExpressions.EncodedName) { }

        protected override string Evaluator(Match match)
        {
            int value = 0;
            foreach (char c in match.Groups["hex"].Value)
                value = (value << 4) | Uri.FromHex(c);
            return new string((char)value, 1);
        }
    }
}
