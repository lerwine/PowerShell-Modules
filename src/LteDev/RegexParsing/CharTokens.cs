using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;

namespace LteDev.RegexParsing
{
    public class CharTokens : IRegexPatternToken, IReadOnlyList<char>
    {
        private readonly IList<char> _values;
        private readonly RegexTokenType _tokenType;

        public RegexTokenType TokenType { get { return _tokenType; } }

        public int Count { get { return _values.Count; } }

        public char this[int index] { get { return _values[index]; } }

        public CharTokens(RegexTokenType tokenType, params char[] values)
        {
            if (values is null || values.Length < 1)
                throw new ArgumentException("At least one character must be provided", nameof(values));
            _tokenType = tokenType;
            _values = values;
        }

        public CharTokens(RegexTokenType tokenType, string value)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentException("Value cannot be empty", nameof(value));
            _tokenType = tokenType;
            _values = value.ToCharArray();
        }

        public IEnumerator<char> GetEnumerator() { return _values.GetEnumerator(); }

        IEnumerator IEnumerable.GetEnumerator() { return ((IEnumerable)_values).GetEnumerator(); }

        IEnumerable<char> IRegexPatternToken.GetPattern() { return this; }

        public void WriteTo(HtmlTextWriter writer, List<string> classNames, ICssClassMapper classMapper) { writer.WriteEncodedText(new string(_values.ToArray())); }
    }
}
