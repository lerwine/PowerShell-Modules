using System;
using System.Collections;
using System.Collections.Generic;

namespace LteDev.RegexParsing
{
    //public interface IValidatableRegexItem : INotifyPropertyChanged, INotifyDataErrorInfo, IDataErrorInfo
    //{
    //    bool IsPropertyValid(string propertyName);

    //    string GetErrorMessage(string propertyName);
    //}

    //public interface IRegexQuantifier : IRegexPatternToken//, IValidatableRegexItem
    //{
    //    QuantifierType Type { get; }
    //}

    //public interface IRegexMinQualifier : IRegexQuantifier
    //{
    //    int MinimumCount { get; }
    //}

    //public interface IRegexMaxQualifier : IRegexQuantifier
    //{
    //    int MaximumCount { get; }
    //}

    //public interface IRegexPatternElement //: IValidatableRegexItem, IEquatable<IRegexPatternElement>
    //{
    //    RegexElementType Type { get; }

    //    IEnumerable<IRegexPatternToken> AllTokens { get; }
    //}

    //public interface IRegexPatternContainer : IRegexPatternElement
    //{
    //    IRegexPatternToken OpenToken { get; }

    //    IRegexPatternToken CloseToken { get; }
    //}

    //public interface IRegexQualtifiable : IRegexPatternElement
    //{
    //    IRegexQuantifier Quantifier { get; }
    //}

    public class CharTokens : IRegexPatternToken, IReadOnlyList<char>
    {
        private readonly IList<char> _values;
        private readonly RegexTokenType _tokenType;

        public RegexTokenType TokenType { get { return _tokenType; } }

        public int Count { get { return _values.Count; } }

        public char this[int index] { get { return _values[index]; } }

        public CharTokens(RegexTokenType tokenType, params char[] values)
        {
            if (values == null || values.Length < 1)
                throw new ArgumentException("At least one character must be provided", "values");
            _tokenType = tokenType;
            _values = values;
        }

        public CharTokens(RegexTokenType tokenType, string value)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentException("Value cannot be empty", "value");
            _tokenType = tokenType;
            _values = value.ToCharArray();
        }

        public IEnumerator<char> GetEnumerator() { return _values.GetEnumerator(); }

        IEnumerator IEnumerable.GetEnumerator() { return ((IEnumerable)_values).GetEnumerator(); }

        IEnumerable<char> IRegexPatternToken.GetPattern() { return this; }
    }
}
