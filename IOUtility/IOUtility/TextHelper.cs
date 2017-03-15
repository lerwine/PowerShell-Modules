using System;
using System.Collections.Generic;
using System.Globalization;
#if !PSLEGACY2
using System.Linq;
#endif
using System.Text.RegularExpressions;

namespace IOUtilityCLR
{
    public class TextHelper
    {
        internal static string GetRegexPattern(UnicodeCategory? categories, IEnumerable<char> otherChars, bool notMatch, bool noBrackets)
        {
            List<string> patterns = new List<string>();

            if (categories.HasValue)
            {
                foreach (UnicodeCategory uc in Enum.GetValues(typeof(UnicodeCategory)))
                {
                    string pattern;

                    switch (uc)
                    {
                        case UnicodeCategory.UppercaseLetter:
                            pattern = @"\p{Lu}";
                            break;
                        case UnicodeCategory.LowercaseLetter:
                            pattern = @"\p{Ll}";
                            break;
                        case UnicodeCategory.TitlecaseLetter:
                            pattern = @"\p{Lt}";
                            break;
                        case UnicodeCategory.ModifierLetter:
                            pattern = @"\p{Lm}";
                            break;
                        case UnicodeCategory.OtherLetter:
                            pattern = @"\p{Lo}";
                            break;
                        case UnicodeCategory.NonSpacingMark:
                            pattern = @"\p{Mn}";
                            break;
                        case UnicodeCategory.SpacingCombiningMark:
                            pattern = @"\p{Mc}";
                            break;
                        case UnicodeCategory.EnclosingMark:
                            pattern = @"\p{Me}";
                            break;
                        case UnicodeCategory.DecimalDigitNumber:
                            pattern = @"\p{Nd}";
                            break;
                        case UnicodeCategory.LetterNumber:
                            pattern = @"\p{Nl}";
                            break;
                        case UnicodeCategory.OtherNumber:
                            pattern = @"\p{No}";
                            break;
                        case UnicodeCategory.SpaceSeparator:
                            pattern = @"\p{Zs}";
                            break;
                        case UnicodeCategory.LineSeparator:
                            pattern = @"\p{Zl}";
                            break;
                        case UnicodeCategory.ParagraphSeparator:
                            pattern = @"\p{Zp}";
                            break;
                        case UnicodeCategory.Control:
                            pattern = @"\p{Cc}";
                            break;
                        case UnicodeCategory.Format:
                            pattern = @"\p{Cf}";
                            break;
                        case UnicodeCategory.Surrogate:
                            pattern = @"\p{Cs}";
                            break;
                        case UnicodeCategory.PrivateUse:
                            pattern = @"\p{Co}";
                            break;
                        case UnicodeCategory.ConnectorPunctuation:
                            pattern = @"\p{Pc}";
                            break;
                        case UnicodeCategory.DashPunctuation:
                            pattern = @"\p{Pd}";
                            break;
                        case UnicodeCategory.OpenPunctuation:
                            pattern = @"\p{Ps}";
                            break;
                        case UnicodeCategory.ClosePunctuation:
                            pattern = @"\p{Pe}";
                            break;
                        case UnicodeCategory.InitialQuotePunctuation:
                            pattern = @"\p{Pi}";
                            break;
                        case UnicodeCategory.FinalQuotePunctuation:
                            pattern = @"\p{Pf}";
                            break;
                        case UnicodeCategory.OtherPunctuation:
                            pattern = @"\p{Po}";
                            break;
                        case UnicodeCategory.MathSymbol:
                            pattern = @"\p{Sm}";
                            break;
                        case UnicodeCategory.CurrencySymbol:
                            pattern = @"\p{Sc}";
                            break;
                        case UnicodeCategory.ModifierSymbol:
                            pattern = @"\p{Sk}";
                            break;
                        case UnicodeCategory.OtherSymbol:
                            pattern = @"\p{So}";
                            break;
                        default:
                            pattern = @"\p{Cn}";
                            break;
                    }
                    if (!patterns.Contains(pattern))
                        patterns.Add(pattern);
                }
            }

#if PSLEGACY2
            if (otherChars != null && LinqEmul.Any<char>(otherChars))
#else
            if (otherChars != null && otherChars.Any())
#endif
            {
                Regex regex;
                if (patterns.Count == 1)
                    regex = new Regex(patterns[0], RegexOptions.Compiled);
                else if (patterns.Count > 1)
                    regex = new Regex(String.Format("[{0}]", String.Join("", patterns.ToArray())), RegexOptions.Compiled);
                else
                    regex = null;
                if (regex == null)
                {
                    foreach (char c in otherChars)
                    {
                        int i = (int)c;
                        string pattern = (i > 127) ? String.Format(@"\u{0:X4}", (int)c) : Regex.Escape(new String(c, 1));
                        if (!patterns.Contains(pattern))
                            patterns.Add(pattern);
                    }
                }
                else
                {
                    foreach (char c in otherChars)
                    {
                        if (regex.IsMatch(c.ToString()))
                            continue;

                        int i = (int)c;
                        string pattern = (i > 127) ? String.Format(@"\u{0:X4}", (int)c) : Regex.Escape(new String(c, 1));
                        if (!patterns.Contains(pattern))
                            patterns.Add(pattern);
                    }
                }
            }

            if (notMatch)
            {
                if (noBrackets)
                    return String.Format("^{0}", String.Join("", patterns.ToArray()));

                return String.Format("[^{0}]", String.Join("", patterns.ToArray()));
            }

            if (patterns.Count == 0)
                return "";

            if (patterns.Count == 1)
                return patterns[0];

            if (noBrackets)
                return String.Join("", patterns.ToArray());

            return String.Format("[{0}]", String.Join("", patterns.ToArray()));
        }

        public static string GetRegexPattern(char ignoreChar, params char[] otherChars)
        {
            return TextHelper.GetRegexPattern(false, ignoreChar, otherChars);
        }

        public static string GetRegexPattern(bool notMatch, char ignoreChar, params char[] otherChars)
        {
            return TextHelper.GetRegexPattern(notMatch, false, ignoreChar, otherChars);
        }

        public static string GetRegexPattern(bool notMatch, bool noBrackets, char ignoreChar, params char[] otherChars)
        {
            if (otherChars == null || otherChars.Length == 0)
            return TextHelper.GetRegexPattern(null, new char[] { ignoreChar }, notMatch, noBrackets);
            List<char> allChars = new List<char>(otherChars);
            allChars.Insert(0, ignoreChar);
            return TextHelper.GetRegexPattern(null, allChars, notMatch, noBrackets);
        }

        public static string GetRegexPattern(UnicodeCategory categories, params char[] otherChars)
        {
            return TextHelper.GetRegexPattern(categories, false, otherChars);
        }

        public static string GetRegexPattern(UnicodeCategory categories, bool notMatch, params char[] otherChars)
        {
            return TextHelper.GetRegexPattern(categories, notMatch, false, otherChars);
        }

        public static string GetRegexPattern(UnicodeCategory categories, bool notMatch, bool noBrackets, params char[] otherChars)
        {
            return TextHelper.GetRegexPattern(categories, otherChars, notMatch, noBrackets);
        }
    }
}
