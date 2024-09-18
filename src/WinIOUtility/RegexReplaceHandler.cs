using System;
using System.Text.RegularExpressions;

namespace WinIOUtility
{
    /// <summary>
    /// Base class for replacing text based upon regular expression matches.
    /// </summary>
    public abstract class RegexReplaceHandler
    {
        private Regex _regex;

        /// <summary>
        /// Regular Expression object which matches text to encode.
        /// </summary>
        public Regex Regex
        {
            get { return _regex; }
            private set
            {
                ArgumentNullException.ThrowIfNull(value);
                _regex = value;
            }
        }

        /// <summary>
        /// Initialize new <see cref="RegexReplaceHandler" />.
        /// </summary>
        /// <param name="pattern">Regular Expression pattern of text to encode.</param>
        /// <param name="options">Regular expression pattern options.</param>
        protected RegexReplaceHandler(string pattern, RegexOptions options)
        {
            if (string.IsNullOrWhiteSpace(pattern))
                throw new ArgumentException($"'{nameof(pattern)}' cannot be null or whitespace.", nameof(pattern));
            _regex = new Regex(pattern, options);
        }

        /// <summary>
        /// Initialize new <see cref="RegexReplaceHandler" />.
        /// </summary>
        /// <param name="pattern">Regular Expression pattern of text to encode.</param>
        protected RegexReplaceHandler(string pattern)
        {
            if (string.IsNullOrWhiteSpace(pattern))
                throw new ArgumentException($"'{nameof(pattern)}' cannot be null or whitespace.", nameof(pattern));
            _regex = new Regex(pattern);
        }

        /// <summary>
        /// Initialize new <see cref="RegexReplaceHandler" />.
        /// </summary>
        /// <param name="regex">Regular Expression object which matches text to encode.</param>
        protected RegexReplaceHandler(Regex regex)
        {
            _regex = regex ?? throw new ArgumentNullException(nameof(regex));
        }

        /// <summary>
        /// Returns replaced text.
        /// </summary>
        /// <param name="match">Current regular expression match to be replaced.</param>
        /// <returns>Text which has been replaced according to match results.</returns>
        protected abstract string Evaluator(Match match);

        /// <summary>
        /// Replaces text.
        /// </summary>
        /// <param name="input">Text to be searched and replaced.</param>
        /// <returns>Text which has been replaced according to match results.</returns>
        public string Replace(string input) { return this.Regex.Replace(input, Evaluator); }

        /// <summary>
        /// Replaces text.
        /// </summary>
        /// <param name="input">Text to be searched and replaced.</param>
        /// <param name="count">Maximum number of matches to be replaced.</param>
        /// <returns>Text which has been replaced according to match results.</returns>
        public string Replace(string input, int count) { return this.Regex.Replace(input, this.Evaluator, count); }

        /// <summary>
        /// Replaces text.
        /// </summary>
        /// <param name="input">Text to be searched and replaced.</param>
        /// <param name="count">Maximum number of matches to be replaced.</param>
        /// <param name="startat">Index at which replacement starts.</param>
        /// <returns>Text which has been replaced according to match results.</returns>
        public string Replace(string input, int count, int startat) { return this.Regex.Replace(input, this.Evaluator, count, startat); }
    }
}
