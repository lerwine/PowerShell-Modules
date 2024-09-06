using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;

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

        private static readonly char[] first = ['?'];

        public OpenGroupToken(RegexTokenType tokenType, int depth, string? name, params char[] followingValues)
        {
            _tokenType = tokenType;
            _depth = depth;
            _precedingValues = Array.Empty<char>();
            _name = name ?? string.Empty;
            _followingValues = followingValues ?? ([]);
        }

        public OpenGroupToken(RegexTokenType tokenType, int depth, char precedingChar, string? name, params char[] followingValues)
        {
            _tokenType = tokenType;
            _depth = depth;
            _precedingValues = [precedingChar];
            _name = name ?? string.Empty;
            _followingValues = followingValues ?? ([]);
        }

        public OpenGroupToken(RegexTokenType tokenType, int depth, string precedingChars, string? name, params char[] followingValues)
        {
            _tokenType = tokenType;
            _depth = depth;
            _precedingValues = [.. first, .. precedingChars];
            _name = name ?? string.Empty;
            _followingValues = followingValues ?? ([]);
        }

        public OpenGroupToken(RegexTokenType tokenType, int depth, char precedingChar1, char precedingChar2, string? name, params char[] followingValues)
        {
            _tokenType = tokenType;
            _depth = depth;
            _precedingValues = [precedingChar1, precedingChar2];
            _name = name ?? string.Empty;
            _followingValues = followingValues ?? [];
        }

        public OpenGroupToken(RegexTokenType tokenType, int depth, char precedingChar1, char precedingChar2, char precedingChar3, string? name, params char[] followingValues)
        {
            _tokenType = tokenType;
            _depth = depth;
            _precedingValues = [precedingChar1, precedingChar2, precedingChar3];
            _name = name ?? string.Empty;
            _followingValues = followingValues ?? [];
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

        public void WriteTo(HtmlTextWriter writer, List<string> classNames, ICssClassMapper classMapper)
        {
            foreach (char c in GetPrecedingPattern())
                writer.Write(c);
            RegexParser.WriteSpanned(_name, writer, classNames, classMapper.TryGetNameClassNames(out string[]? spanClassNames) ? spanClassNames : null);
            foreach (char c in GetFollowingPattern())
                writer.Write(c);
        }
    }
}
