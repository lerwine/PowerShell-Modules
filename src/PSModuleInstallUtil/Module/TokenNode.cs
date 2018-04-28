using System.Management.Automation;

namespace PSModuleInstallUtil.Module
{
    public abstract class TokenNode : ScriptNode
    {
        PSToken _token;
        public override string Value { get { return _token.Content; } }
        public override string Text { get { return OwnerDocument.Text.Substring(_token.Start, _token.Length); } }
        public override int Start { get { return _token.Start; } }
        public override int Length { get { return _token.Length; } }
        public abstract override ScriptNodeType NodeType { get; }
        public PSTokenType TokenType { get { return _token.Type; } }
        protected TokenNode(ScriptNode parentNode, PSToken token) : base(parentNode) { _token = token; }
    }
}
