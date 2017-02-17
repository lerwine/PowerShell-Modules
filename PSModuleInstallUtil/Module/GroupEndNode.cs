using System;
using System.Management.Automation;

namespace PSModuleInstallUtil.Module
{
    public class GroupEndNode : TokenNode
    {
        public GroupEndNode(ScriptNode parent, PSToken token) : base(parent, token) { }

        public override ScriptNodeType NodeType { get { return ScriptNodeType.GroupEnd; } }
    }
}