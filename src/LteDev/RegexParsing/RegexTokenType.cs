namespace LteDev.RegexParsing
{
    public enum RegexTokenType
    {
        /// <summary>
        /// Literal pattern characters.
        /// </summary>
        LiteralCharacter,
        /// <summary>
        /// Escaped pattern characters other than those must be escaped.
        /// </summary>
        EscapedLiteral,
        /// <summary>
        /// Escaped pattern characters.
        /// </summary>
        CharacterEscape,
        /// <summary>
        /// Anchors with pattern <c>^</c>, <c>^</c>, <c>\G</c>, <c>\A</c>, <c>\Z</c>, or <c>\z</c>.
        /// </summary>
        Anchor,
        /// <summary>
        /// The <c>(</c> character.
        /// </summary>
        GroupOpen,
        /// <summary>
        /// The <c>)</c> character.
        /// </summary>
        GroupClose,
        /// <summary>
        /// The <c>[</c> character.
        /// </summary>
        CharacterClassOpen,
        /// <summary>
        /// The <c>]</c> character.
        /// </summary>
        CharacterClassClose,
        /// <summary>
        /// Alternation characters (<c>|</c>).
        /// </summary>
        Alternation,
        /// <summary>
        /// &quot;Any&quot; characters (<c>|</c>).
        /// </summary>
        AnyCharacters,
        /// <summary>
        /// Backreference with pattern <c>\number</c>, <c>\k&lt;number&gt;</c>, <c>\k&lt;name&gt;</c> or <c>\k'name'</c>.
        /// </summary>
        Backreference,
        ModeModifier,
        ControlCharacterEscape,
        HexidecimalEscape,
        MetaCharacterEscape,
        NullEscape,
        OctalEscape,
        NonCapturingGroupOpen,
        NamedGroupOpen,
        /// <summary>
        /// Quantifier with pattern <c>?</c>.
        /// </summary>
        GreedyOptionalQuantifier,
        /// <summary>
        /// Quantifier with pattern <c>*</c>.
        /// </summary>
        GreedyOptionalRepeatQuantifier,
        /// <summary>
        /// Quantifier with pattern <c>+</c>.
        /// </summary>
        GreedyMultipleQuantifier,
        /// <summary>
        /// Quantifier with pattern <c>{N}</c>.
        /// </summary>
        GreedyFixedQuantifier,
        /// <summary>
        /// Quantifier with pattern <c>{N,}</c>.
        /// </summary>
        GreedyMinRepeatQuantifier,
        /// <summary>
        /// Quantifier with pattern <c>{N,N}</c>.
        /// </summary>
        GreedyLimitedQuantifier,
        /// <summary>
        /// Quantifier with pattern <c>??</c>.
        /// </summary>
        LazyOptionalQuantifier,
        /// <summary>
        /// Quantifier with pattern <c>*?</c>.
        /// </summary>
        LazyOptionalRepeatQuantifier,
        /// <summary>
        /// Quantifier with pattern <c>+?</c>.
        /// </summary>
        LazyMultipleQuantifier,
        /// <summary>
        /// Quantifier with pattern <c>{N}?</c>.
        /// </summary>
        LazyFixedQuantifier,
        /// <summary>
        /// Quantifier with pattern <c>{N,N}?</c>.
        /// </summary>
        LazyLimitedQuantifier,
        /// <summary>
        /// Quantifier with pattern <c>{N,}?</c>.
        /// </summary>
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
