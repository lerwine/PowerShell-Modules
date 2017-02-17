using System;
using System.Management.Automation;

namespace PSModuleInstallUtil.Module
{
    public class SubExpressionNode : GroupingNode
    {
        internal SubExpressionNode(ScriptNode parent, PSToken token) : base(parent, token) { }

        public override ScriptNodeType NodeType { get { return ScriptNodeType.SubExpression; } }
    }
}