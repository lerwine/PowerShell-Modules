using System;
using System.Collections.Generic;
using System.Linq;

namespace LteDev.RegexParsing
{
    public class OpenGroupToken : IRegexPatternToken
    {
        private readonly IList<char> _precedingValues;
        private readonly string _name;
        private readonly IList<char> _followingValues;
        private readonly RegexTokenType _tokenType;
        private readonly int _depth;

        public RegexTokenType TokenType { get { return _tokenType; } }

        public int Depth { get { return _depth; } }

        public string Name { get { return _name; } }

        public OpenGroupToken(RegexTokenType tokenType, int depth, string name, params char[] followingValues)
        {
            _tokenType = tokenType;
            _depth = depth;
            _precedingValues = Array.Empty<char>();
            _name = (name == null) ? string.Empty : name;
            _followingValues = (followingValues == null) ? Array.Empty<char>() : followingValues;
        }

        public OpenGroupToken(RegexTokenType tokenType, int depth, char precedingChar, string name, params char[] followingValues)
        {
            _tokenType = tokenType;
            _depth = depth;
            _precedingValues = new char[] { precedingChar };
            _name = (name == null) ? string.Empty : name;
            _followingValues = (followingValues == null) ? Array.Empty<char>() : followingValues;
        }

        public OpenGroupToken(RegexTokenType tokenType, int depth, string precedingChars, string name, params char[] followingValues)
        {
            _tokenType = tokenType;
            _depth = depth;
            _precedingValues = new char[] { '?' }.Concat(precedingChars).ToArray();
            _name = (name == null) ? string.Empty : name;
            _followingValues = (followingValues == null) ? Array.Empty<char>() : followingValues;
        }

        public OpenGroupToken(RegexTokenType tokenType, int depth, char precedingChar1, char precedingChar2, string name, params char[] followingValues)
        {
            _tokenType = tokenType;
            _depth = depth;
            _precedingValues = new char[] { precedingChar1, precedingChar2 };
            _name = (name == null) ? string.Empty : name;
            _followingValues = (followingValues == null) ? Array.Empty<char>() : followingValues;
        }

        public OpenGroupToken(RegexTokenType tokenType, int depth, char precedingChar1, char precedingChar2, char precedingChar3, string name, params char[] followingValues)
        {
            _tokenType = tokenType;
            _depth = depth;
            _precedingValues = new char[] { precedingChar1, precedingChar2, precedingChar3 };
            _name = (name == null) ? string.Empty : name;
            _followingValues = (followingValues == null) ? Array.Empty<char>() : followingValues;
        }

        public IEnumerable<char> GetPrecedingPattern()
        {
            yield return '(';
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
            yield return '(';
            foreach (char c in _precedingValues)
                yield return c;
            foreach (char c in _name)
                yield return c;
            foreach (char c in _followingValues)
                yield return c;
        }
    }
}
