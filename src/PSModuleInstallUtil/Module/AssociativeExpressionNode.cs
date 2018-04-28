using System.Management.Automation;

namespace PSModuleInstallUtil.Module
{
    public class AssociativeExpressionNode : GroupingNode
    {
        internal AssociativeExpressionNode(ScriptNode parentNode, PSToken token) : base(parentNode, token) { }

        public override ScriptNodeType NodeType { get { return ScriptNodeType.Associative; } }
    }
}