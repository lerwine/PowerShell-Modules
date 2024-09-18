using System.Collections.Generic;

namespace LteDev.RegexParsing
{
    public interface ICssClassMapper : IReadOnlyDictionary<RegexTokenType, string[]>
    {
        bool TryGetNumberClassNames(out string[]? classNames);
        bool TryGetNameClassNames(out string[]? classNames);
    }
}
