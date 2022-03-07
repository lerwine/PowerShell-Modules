namespace LteDev.RegexParsing
{
    public enum RegexTokenType
    {
        /// <summary>
        /// Literal pattern character
        /// </summary>
        LiteralCharacter,
        /// <summary>
        /// Escaped pattern character other than those must be escaped.
        /// </summary>
        EscapedLiteral,
        /// <summary>
        /// Escaped pattern character.
        /// </summary>
        CharacterEscape,
        Anchor,
        GroupOpen,
        GroupClose,
        CharacterClassOpen,
        CharacterClassClose,
        Alternation,
        AnyCharacters,
        Backreference,
        ModeModifier,
        ControlCharacterEscape,
        HexidecimalEscape,
        MetaCharacterEscape,
        NullEscape,
        OctalEscape,
        NonCapturingGroupOpen,
        NamedGroupOpen,
        FixedQuantifier,
        GreedyOptionalQuantifier,
        GreedyOptionalRepeatQuantifier,
        GreedyMultipleQuantifier,
        GreedyLimitedQuantifier,
        GreedyMinRepeatQuantifier,
        LazyOptionalQuantifier,
        LazyOptionalRepeatQuantifier,
        LazyMultipleQuantifier,
        LazyLimitedQuantifier,
        LazyMinRepeatQuantifier,
        AtomicGroupOpen,
        CommentType,
        Conditional,
        LookbehindOpen,
        LookaheadOpen,
        CodePoint,
        UnicodeEscape,
        UnicodeCategory,
        Unknown
    }
}
