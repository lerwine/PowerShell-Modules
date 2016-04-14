using System;
using System.Text.RegularExpressions;

namespace IOUtilityCLR
{
    public abstract class RegexReplaceHandler
    {
        private Regex _regex;

        protected Regex Regex { get { return this._regex; } private set { this._regex = value; } }

        protected RegexReplaceHandler(Regex regex)
        {
            if (regex == null)
                throw new ArgumentNullException("regex");

            this.Regex = regex;
        }

        protected abstract string Evaluator(Match match);

        public string Replace(string input) { return this.Regex.Replace(input, this.Evaluator); }

        public string Replace(string input, int count) { return this.Regex.Replace(input, this.Evaluator, count); }

        public string Replace(string input, int count, int startat) { return this.Regex.Replace(input, this.Evaluator, count, startat); }
    }
}
