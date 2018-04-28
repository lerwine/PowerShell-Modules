namespace PSModuleInstallUtil.Module
{
    public class InertTextNode : ScriptNode
    {
        private int _start, _length;

        public override int Start { get { return _start; } }

        public override int Length { get { return _length; } }

        public override string Text { get { return OwnerDocument.Text.Substring(_start, _length); } }

        public override string Value { get { return Text; } }

        public override ScriptNodeType NodeType { get { return ScriptNodeType.Inert; } }
        internal InertTextNode(ScriptNode parentNode, int start, int length) : base(parentNode)
        {
            _start = start;
            _length = length;
        }
    }
}
