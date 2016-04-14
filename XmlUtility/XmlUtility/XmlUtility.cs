using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml;

namespace XmlUtilityCLR
{
    public static class XmlUtility
    {
        public static readonly Regex WhitespaceRegex = new Regex(@"\s+", RegexOptions.Compiled);
        public static readonly Regex WhitespaceEncodeRegex = new Regex(@"(_(?=0x[\dA-F]{4}_)|\s)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        public static readonly Regex EncodedRegex = new Regex(@"_0x(?<hex>[\dA-F]{4})_", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static string EncodeSpace(string input) { return XmlUtility.WhitespaceEncodeRegex.Replace(input, XmlUtility.WhitespaceEncodeEvaluator); }

        public static string EncodeSpace(string input, int count) { return XmlUtility.WhitespaceEncodeRegex.Replace(input, XmlUtility.WhitespaceEncodeEvaluator, count); }

        public static string EncodeSpace(string input, int count, int startat) { return XmlUtility.WhitespaceEncodeRegex.Replace(input, XmlUtility.WhitespaceEncodeEvaluator, count, startat); }
        
        public static string Decode(string input) { return XmlUtility.EncodedRegex.Replace(input, XmlUtility.DecodeEvaluator); }
        
        public static string Decode(string input, int count) { return XmlUtility.EncodedRegex.Replace(input, XmlUtility.DecodeEvaluator, count); }
        
        public static string Decode(string input, int count, int startat) { return XmlUtility.EncodedRegex.Replace(input, XmlUtility.DecodeEvaluator, count, startat); }
        
        private static string Evaluator(Match match)
        {
            if (match.Length == 1)
                return String.Format("_0x{0:X4}_", (int)(match.Value[0]));

            StringBuilder result = new StringBuilder();
            foreach (char c in match.Value)
                result.AppendFormat("_0x{0:X4}_", (int)c);
                
            return result.ToString();
        }

        private static string DecodeEvaluator(Match match)
        {
            int value = 0;
            foreach (char c in match.Groups["hex"].Value)
                value = (value << 4) | Uri.FromHex(c);
            return new string((char)value, 1);
        }
    }
}
