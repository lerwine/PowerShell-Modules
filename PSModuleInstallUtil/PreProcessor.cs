using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Text.RegularExpressions;

namespace PSModuleInstallUtil
{
    public class PreProcessor
    {
        private string _source;
        private ReadOnlyCollection<PSParseError> _errors;
        private ReadOnlyCollection<PSToken> _tokens;
        private Collection<string> _innerDefinedSymbols = new Collection<string>();
        private ReadOnlyCollection<string> _definedSymbols;

        public ReadOnlyCollection<string> DefinedSymbols { get { return _definedSymbols; } }

        public PreProcessor(string source)
        {
            _definedSymbols = new ReadOnlyCollection<string>(_innerDefinedSymbols);
            _source = (source == null) ? "" : source;
            Collection<PSParseError> errors;
            Collection<PSToken> tokens;
            if (_source.Length == 0)
            {
                errors = new Collection<PSParseError>();
                tokens = new Collection<PSToken>();
            }
            else
            {
                if ((tokens = PSParser.Tokenize(_source, out errors)) == null)
                    tokens = new Collection<PSToken>();
                if (errors == null)
                    errors = new Collection<PSParseError>();
            }
            _tokens = new ReadOnlyCollection<PSToken>(tokens);
            _errors = new ReadOnlyCollection<PSParseError>(errors);
        }

        public PreProcessor(string source, IEnumerable<string> definedSymbols)
            : this(source)
        {
            if (definedSymbols == null)
                return;

            foreach (string s in definedSymbols)
            {
                if (!String.IsNullOrEmpty(s))
                    Define(s);
            }
        }

        public void Define(string symbol)
        {
            if (symbol == null)
                throw new ArgumentNullException("symbol");

            if (!PreProcesorCommentDefine.IsMatch(symbol))
                throw new ArgumentOutOfRangeException("symbol", "Invalid symbol name.");

            symbol = symbol.ToUpper();
            if (!_innerDefinedSymbols.Contains(symbol))
                _innerDefinedSymbols.Add(symbol);
        }

        public void Undefine(string symbol)
        {
            if (symbol == null)
                throw new ArgumentNullException("symbol");

            if (!PreProcesorCommentDefine.IsMatch(symbol))
                throw new ArgumentOutOfRangeException("symbol", "Invalid symbol name.");

            symbol = symbol.ToUpper();
            if (!_innerDefinedSymbols.Contains(symbol))
                _innerDefinedSymbols.Remove(symbol);
        }
        
        public static readonly Regex PreProcesorCommentRegex = new Regex(@"^<#:(((?<c>if)\s+(?<e>[^#]+)|(?<c>endif|else)\s*|(?<c>=)(?<t>[^:]+(:([^#]|.[^>])*)):#>$", RegexOptions.Compiled);
        public static readonly Regex PreProcesorConditionalSyntax1 = new Regex(@"^[^()]*(?>(?>(?<open>\()[^()]*)+(?>(?<-open>\))[^()]*)+)+(?(open)(?!))$", RegexOptions.Compiled);
        public static readonly Regex PreProcesorConditionalSyntax2 = new Regex(@"^(((^|\(|\||&)\s*(?=[(!a-z]))!?[a-z][a-z\d_]+\)*\s*(?=\||&|$))+$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        public static readonly Regex PreProcesorCommentDefine = new Regex(@"^[a-z][a-z\d_]+$", RegexOptions.Compiled);
        public static readonly Regex ConditionalReplaceRegex = new Regex(@"\b\s*((?<e>\||&|!|[a-z][a-z\d_]+)\s*", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public class PreProcessorData
        {
            private int _startIndex;
            private string _code;
            private string _content;

            public int StartIndex { get { return _startIndex; } }
            public string Code { get { return _code; } }
            public string Content { get { return _content; } }

            public PreProcessorData(string code, string content, int startIndex)
            {
                this._code = code;
                this._content = content;
                this._startIndex = startIndex;
            }
        }

        private static string ConditionalReplaceHandler(Match match)
        {
            switch (match.Groups["e"].Value)
            {
                case "|":
                    return " -or ";
                case "&":
                    return " -and ";
                case "!":
                    return " -not ";
            }

            return "$" + match.Groups["e"].Value;
        }

        public Collection<PreProcessorData> GetPreProcessorData()
        {
            Collection<PreProcessorData> preProcessorData = new Collection<PreProcessorData>();
            if (_tokens.Count == 0)
                return preProcessorData;

            int startIndex = 0;
            foreach (PSToken token in _tokens)
            {
                Match m;
                if (token.Type == PSTokenType.Comment && ((m = PreProcesorCommentRegex.Match(token.Content)).Success))
                {
                    if (startIndex < token.Start)
                        preProcessorData.Add(new PreProcessorData("", _source.Substring(startIndex, token.Start - token.Length), startIndex));
                    startIndex = token.Start + token.Length;
                    switch (m.Groups["c"].Value)
                    {
                        case "endif":
                            preProcessorData.Add(new PreProcessorData("}", "", token.Start));
                            break;
                        case "else":
                            preProcessorData.Add(new PreProcessorData("} else {", "", token.Start));
                            break;
                        case "=":
                            preProcessorData.Add(new PreProcessorData("", m.Groups["t"].Value, token.Start));
                            break;
                        default:
                            string code = m.Groups["e"].Value.Trim();
                            if (!PreProcesorConditionalSyntax1.IsMatch(code))
                                throw new FormatException(String.Format("Parenthesis match failed in \"if\" pre-processor conditional, after line {0}, column {1}", token.StartLine, token.StartColumn));
                            if (!PreProcesorConditionalSyntax2.IsMatch(code))
                                throw new FormatException(String.Format("Syntax error in \"if\" pre-processor conditional, after line {0}, column {1}", token.StartLine, token.StartColumn));
                            preProcessorData.Add(new PreProcessorData("if (" + ConditionalReplaceRegex.Replace(m.Groups["e"].Value.Trim(), new MatchEvaluator(ConditionalReplaceHandler)) + ") {", "", token.Start));
                            break;
                    }
                }
            }

            if (startIndex < _source.Length)
                preProcessorData.Add(new PreProcessorData("", _source.Substring(startIndex), startIndex));
            if (preProcessorData[0].Code.Length == 0 && preProcessorData[0].Content.Length == 0)
                preProcessorData.RemoveAt(0);
            return preProcessorData;
        }
    }
}
