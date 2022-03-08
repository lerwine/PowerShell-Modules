using System;

namespace LteDev.RegexParsing
{
    public interface ITokenParser : IObservable<IRegexPatternToken>
    {
        int Parse(CharacterStream stream);
    }
}
