using System.Collections.Generic;

namespace LteDev.RegexParsing
{
    public class CloseGroupToken : IRegexPatternToken
    {
        private readonly int _depth;
        private readonly RegexTokenType _tokenType;

        public RegexTokenType TokenType { get { return _tokenType; } }

        public int Depth { get { return _depth; } }

        public CloseGroupToken(RegexTokenType tokenType, int depth)
        {
            _tokenType = tokenType;
            _depth = depth;
        }

        public IEnumerable<char> GetPattern()
        {
            yield return ')';
        }
    }
}
