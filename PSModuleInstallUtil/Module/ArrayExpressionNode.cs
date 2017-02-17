using System;
using System.Management.Automation;

namespace PSModuleInstallUtil.Module
{
    public class ArrayExpressionNode : GroupingNode
    {
        internal ArrayExpressionNode(ScriptNode parentNode, PSToken token) : base(parentNode, token) { }

        public override ScriptNodeType NodeType { get { return ScriptNodeType.Array; } }
    }
}