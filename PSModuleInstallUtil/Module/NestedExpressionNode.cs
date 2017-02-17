using System.Management.Automation;

namespace PSModuleInstallUtil.Module
{
    public class NestedExpressionNode : GroupingNode
    {
        internal NestedExpressionNode(ScriptNode parent, PSToken token) : base(parent, token) { }

        public override ScriptNodeType NodeType { get { return ScriptNodeType.NestedExpression; } }
    }
}