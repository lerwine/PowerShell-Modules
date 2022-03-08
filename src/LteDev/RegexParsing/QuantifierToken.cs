using System.Collections.Generic;
using System.Web.UI;

namespace LteDev.RegexParsing
{
    public abstract class QuantifierToken : IRegexPatternToken
    {
        public QuantifierType Type { get; }

        public bool IsLazy { get; }

        public RegexTokenType TokenType { get; }

        protected QuantifierToken(QuantifierType type, bool isLazy)
        {
            Type = type;
            IsLazy = isLazy;
            switch (type)
            {
                case QuantifierType.Optional:
                    TokenType = IsLazy ? RegexTokenType.LazyOptionalQuantifier : RegexTokenType.GreedyOptionalQuantifier;
                    break;
                case QuantifierType.OptionalRepeat:
                    TokenType = IsLazy ? RegexTokenType.LazyOptionalRepeatQuantifier : RegexTokenType.GreedyOptionalRepeatQuantifier;
                    break;
                case QuantifierType.Multiple:
                    TokenType = IsLazy ? RegexTokenType.LazyMultipleQuantifier : RegexTokenType.GreedyMultipleQuantifier;
                    break;
                case QuantifierType.Fixed:
                    TokenType = IsLazy ? RegexTokenType.LazyFixedQuantifier : RegexTokenType.GreedyFixedQuantifier;
                    break;
                case QuantifierType.MinRepeat:
                    TokenType = IsLazy ? RegexTokenType.LazyMinRepeatQuantifier : RegexTokenType.GreedyMinRepeatQuantifier;
                    break;
                default:
                    TokenType = IsLazy ? RegexTokenType.LazyLimitedQuantifier : RegexTokenType.GreedyLimitedQuantifier;
                    break;
            }
        }

        public abstract IEnumerable<char> GetPattern();

        public abstract void WriteTo(Html32TextWriter writer, List<string> classNames, ICssClassMapper classMapper);
    }
}
