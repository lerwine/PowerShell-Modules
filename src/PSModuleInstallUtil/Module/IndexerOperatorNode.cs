using System.Management.Automation;

namespace PSModuleInstallUtil.Module
{
    public class IndexerOperatorNode : GroupingNode
    {
        internal IndexerOperatorNode(ScriptNode parent, PSToken token) : base(parent, token) { }

        public override ScriptNodeType NodeType { get { return ScriptNodeType.Operator; } }
    }
}