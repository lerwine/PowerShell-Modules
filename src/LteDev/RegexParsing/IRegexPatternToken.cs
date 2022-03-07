using System.Collections.Generic;

namespace LteDev.RegexParsing
{
    //public enum RegexElementType
    //{
    //    Anchor,
    //    Alternation,
    //    AnyCharacters,
    //    Backreference,
    //    CapturingGroup,
    //    NonCapturingGroup,
    //    CharacterClass,
    //    Range,
    //    ModeModifier,
    //    CharacterEscape,
    //    ControlCharacterEscape,
    //    HexidecimalEscape,
    //    LiteralCharacters,
    //    MetaCharacterEscape,
    //    NullEscape,
    //    OctalEscape,
    //    AtomicGroup,
    //    Comment,
    //    Conditional,
    //    Lookaround,
    //    CodePoint,
    //    UnicodeCategory
    //}

    //public enum QuantifierType
    //{
    //    Greedy,
    //    Lazy,
    //    Possessive
    //}

    public interface IRegexPatternToken// : IEquatable<IRegexPatternToken>
    {
        RegexTokenType TokenType { get; }

        IEnumerable<char> GetPattern();
    }
}
