using System.Collections.ObjectModel;
using System.Management.Automation;

namespace PSModuleInstallUtil.Module
{
    public class ScriptDocument : ScriptNode
    {
        private string _text;
        private ReadOnlyCollection<PSParseError> _errors;

        public override string Text { get { return _text; } }

        public override string Value { get { return _text; } }

        public override int Start { get { return 0; } }

        public override int Length { get { return _text.Length; } }

        public ReadOnlyCollection<PSParseError> Errors { get { return _errors; } }

        public override ScriptNodeType NodeType { get { return ScriptNodeType.Document; } }

        public ScriptDocument(string script)
            : base(null)
        {
            _text = script;
            Collection<PSParseError> errors;
            Collection<PSToken> tokens = PSParser.Tokenize(script, out errors);
            if (errors == null)
                errors = new Collection<PSParseError>();
            if (tokens == null)
                tokens = new Collection<PSToken>();
            _errors = new ReadOnlyCollection<PSParseError>(errors);
            if (tokens.Count == 0)
                return;

            int index = 0;
            while (index < tokens.Count)
                index = GroupingNode.Load(this, tokens, index);
        }
    }
}