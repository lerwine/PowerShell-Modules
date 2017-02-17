using System.Collections.ObjectModel;
using System.Management.Automation;

namespace PSModuleInstallUtil.Module
{
    public class ScriptBlockNode : GroupingNode
    {
        public override ScriptNodeType NodeType { get { return ScriptNodeType.ScriptBlock; } }
        internal ScriptBlockNode(ScriptNode parentNode, PSToken token) : base(parentNode, token) { }
    }
}