using System;
using System.Collections.Generic;

namespace LteDev.RegexParsing
{
    public class NamedToken : IRegexPatternToken
    {
        private readonly IList<char> _precedingValues;
        private readonly string _name;
        private readonly IList<char> _followingValues;
        private readonly RegexTokenType _tokenType;

        public RegexTokenType TokenType { get { return _tokenType; } }

        public string Name { get { return _name; } }

        public NamedToken(RegexTokenType tokenType, IList<char> precedingValues, string name, params char[] followingValues)
        {
            if (precedingValues == null || precedingValues.Count < 1)
                throw new ArgumentException("At least one preceding character must be provided", "precedingValues");
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("Name cannot be empty", "name");
            if (followingValues == null || followingValues.Length < 1)
                throw new ArgumentException("At least one following character must be provided", "followingValues");
            _tokenType = tokenType;
            _precedingValues = precedingValues;
            _name = name;
            _followingValues = followingValues;
        }

        public IEnumerable<char> GetPrecedingPattern()
        {
            foreach (char c in _precedingValues)
                yield return c;
        }

        public IEnumerable<char> GetFollowingPattern()
        {
            foreach (char c in _followingValues)
                yield return c;
        }

        public IEnumerable<char> GetPattern()
        {
            foreach (char c in _precedingValues)
                yield return c;
            foreach (char c in _name)
                yield return c;
            foreach (char c in _followingValues)
                yield return c;
        }
    }
}
