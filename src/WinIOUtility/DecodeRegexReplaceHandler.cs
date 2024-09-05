using System;
using System.Text.RegularExpressions;

namespace WinIOUtility
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
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
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
