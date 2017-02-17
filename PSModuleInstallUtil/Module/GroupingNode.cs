using System.Collections.ObjectModel;
using System.Management.Automation;

namespace PSModuleInstallUtil.Module
{
    public abstract class GroupingNode : TokenNode
    {
        internal GroupingNode(ScriptNode parentNode, PSToken token) : base(parentNode, token) { }
        internal static int Load(ScriptNode parent, Collection<PSToken> collection, int index)
        {
            while (index < collection.Count)
            {
                int start = (index == 0) ? 0 : collection[index - 1].Start + collection[index - 1].Length;
                if (start < collection[index].Start)
                    new InertTextNode(parent, start, collection[index].Start - start);
                switch (collection[index].Type)
                {
                    case PSTokenType.GroupEnd:
                        new GroupEndNode(parent, collection[index]);
                        return index + 1;
                    case PSTokenType.GroupStart:
                        switch (collection[index].Content)
                        {
                            case "@{":
                                AssociativeExpressionNode associativeExpressionNode = new AssociativeExpressionNode(parent, collection[index]);
                                index = GroupingNode.Load(associativeExpressionNode, collection, index + 1);
                                break;
                            case "@(":
                                ArrayExpressionNode arrayExpressionNode = new ArrayExpressionNode(parent, collection[index]);
                                index = GroupingNode.Load(arrayExpressionNode, collection, index + 1);
                                break;
                            case "$(":
                                SubExpressionNode subExpressionNode = new SubExpressionNode(parent, collection[index]);
                                index = GroupingNode.Load(subExpressionNode, collection, index + 1);
                                break;
                            case "{":
                                ScriptBlockNode scriptBlockNode = new ScriptBlockNode(parent, collection[index]);
                                index = GroupingNode.Load(scriptBlockNode, collection, index + 1);
                                break;
                            default:
                                NestedExpressionNode nestedExpressionNode = new NestedExpressionNode(parent, collection[index]);
                                index = GroupingNode.Load(nestedExpressionNode, collection, index + 1);
                                break;
                        }
                        break;
                    case PSTokenType.Comment:
                    case PSTokenType.LineContinuation:
                    case PSTokenType.LoopLabel:
                    case PSTokenType.NewLine:
                    case PSTokenType.StatementSeparator:
                    case PSTokenType.Unknown:
                        new SimpleTokenNode(parent, collection[index]);
                        index++;
                        break;
                    case PSTokenType.Operator:
                        if (collection[index].Content == "[")
                        {
                            IndexerOperatorNode indexerOperatorNode = new IndexerOperatorNode(parent, collection[index]);
                            index = GroupingNode.Load(indexerOperatorNode, collection, index + 1);
                            break;
                        }
                        else
                        {
                            if (collection[index].Content == "]")
                            {
                                new GroupEndNode(parent, collection[index]);
                                return index + 1;
                            }
                            new SimpleTokenNode(parent, collection[index]);
                            index++;
                        }
                        break;
                    default:
                        StatementNode statementNode = new StatementNode(parent, collection[index]);
                        index = StatementNode.Load(statementNode, collection, index + 1);
                        break;
                }
            }

            return index;
        }
    }
}