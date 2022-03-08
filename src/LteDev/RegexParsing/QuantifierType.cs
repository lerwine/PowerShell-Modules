namespace LteDev.RegexParsing
{
    public enum QuantifierType
    {
        /// <summary>
        /// Quantifier with pattern <c>?</c>.
        /// </summary>
        Optional,
        /// <summary>
        /// Quantifier with pattern <c>*</c>.
        /// </summary>
        OptionalRepeat,
        /// <summary>
        /// Quantifier with pattern <c>+</c>.
        /// </summary>
        Multiple,
        /// <summary>
        /// Quantifier with pattern <c>{N}</c>.
        /// </summary>
        Fixed,
        /// <summary>
        /// Quantifier with pattern <c>{N,}</c>.
        /// </summary>
        MinRepeat,
        /// <summary>
        /// Quantifier with pattern <c>{N,N}</c>.
        /// </summary>
        Limited,
    }
}
