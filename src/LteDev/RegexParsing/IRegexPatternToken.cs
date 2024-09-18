using System.Collections.Generic;
using System.Web.UI;

namespace LteDev.RegexParsing
{
    public interface IRegexPatternToken// : IEquatable<IRegexPatternToken>
    {
        RegexTokenType TokenType { get; }

        IEnumerable<char> GetPattern();

        void WriteTo(HtmlTextWriter writer, List<string> classNames, ICssClassMapper classMapper);
    }
}
