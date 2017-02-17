using System;
using System.Management.Automation;

namespace PSModuleInstallUtil.Module
{
    public class StatementTerminatorNode : TokenNode
    {
        internal StatementTerminatorNode(ScriptNode parent, PSToken token) : base(parent, token) { }

        public override ScriptNodeType NodeType
        {
            get
            {
                switch (TokenType)
                {
                    case PSTokenType.NewLine:
                        return ScriptNodeType.NewLine;
                    case PSTokenType.StatementSeparator:
                        return ScriptNodeType.StatementSeparator;
                    default:
                        return ScriptNodeType.Unknown;
                }
            }
        }
    }
}