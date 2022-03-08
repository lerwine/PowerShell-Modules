namespace LteDev.RegexParsing
{
    public interface IRegexParserSettings
    {
        ITokenParser PatternStartParser { get; }

        ITokenParser CharacterClassParser { get; }
        
        ITokenParser GroupTypeParser { get; }
        
        ITokenParser QuantifierParser { get; }
    }
}
