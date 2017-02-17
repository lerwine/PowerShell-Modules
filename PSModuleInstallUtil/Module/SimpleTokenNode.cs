using System.Management.Automation;

namespace PSModuleInstallUtil.Module
{
    public class SimpleTokenNode : TokenNode
    {
        public SimpleTokenNode(ScriptNode parent, PSToken token) : base(parent, token) { }

        public override ScriptNodeType NodeType
        {
            get
            {
                switch (TokenType)
                {
                    case PSTokenType.Comment:
                        return ScriptNodeType.Comment;
                    case PSTokenType.LineContinuation:
                        return ScriptNodeType.LineContinuation;
                    case PSTokenType.LoopLabel:
                        return ScriptNodeType.LoopLabel;
                    case PSTokenType.NewLine:
                        return ScriptNodeType.NewLine;
                    case PSTokenType.StatementSeparator:
                        return ScriptNodeType.StatementSeparator;
                    case PSTokenType.Attribute:
                        return ScriptNodeType.Attribute;
                    case PSTokenType.CommandArgument:
                        return ScriptNodeType.CommandArgument;
                    case PSTokenType.CommandParameter:
                        return ScriptNodeType.CommandParameter;
                    case PSTokenType.Keyword:
                        return ScriptNodeType.Keyword;
                    case PSTokenType.Member:
                        return ScriptNodeType.Member;
                    case PSTokenType.Number:
                        return ScriptNodeType.Number;
                    case PSTokenType.Operator:
                        return ScriptNodeType.Operator;
                    case PSTokenType.Position:
                        return ScriptNodeType.Position;
                    case PSTokenType.String:
                        return ScriptNodeType.String;
                    case PSTokenType.Type:
                        return ScriptNodeType.Type;
                    case PSTokenType.Variable:
                        return ScriptNodeType.Variable;
                    default:
                        return ScriptNodeType.Unknown;
                }
            }
        }
    }
}