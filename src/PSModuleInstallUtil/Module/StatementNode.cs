using System;
using System.Collections.ObjectModel;
using System.Management.Automation;

namespace PSModuleInstallUtil.Module
{
    public class StatementNode : TokenNode
    {
        public StatementNode(ScriptNode parent, PSToken token) : base(parent, token) { }

        internal static int Load(ScriptNode parent, Collection<PSToken> collection, int index)
        {
            while (index < collection.Count)
            {
                int start = (index == 0) ? 0 : collection[index - 1].Start + collection[index - 1].Length;
                if (start < collection[index].Start)
                    new InertTextNode(parent, start, collection[index].Start - start);
                switch (collection[index].Type)
                {
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
                    case PSTokenType.NewLine:
                        bool foundComma = false;
                        for (int i=index; i>=0; i--)
                        {
                            if (collection[i].Type == PSTokenType.Operator)
                            {
                                foundComma = collection[i].Content == ",";
                                break;
                            }
                            if (collection[i].Type != PSTokenType.Comment && collection[i].Type != PSTokenType.LineContinuation)
                                break;
                        }
                        if (foundComma)
                        {
                            new SimpleTokenNode(parent, collection[index]);
                            index++;
                            break;
                        }
                        new StatementTerminatorNode(parent, collection[index]);
                        return index + 1;
                    case PSTokenType.Operator:
                        if (collection[index].Content == "[")
                        {
                            IndexerOperatorNode indexerOperatorNode = new IndexerOperatorNode(parent, collection[index]);
                            index = GroupingNode.Load(indexerOperatorNode, collection, index + 1);
                            break;
                        }
                        else
                        {
                            new SimpleTokenNode(parent, collection[index]);
                            index++;
                        }
                        break;
                    case PSTokenType.StatementSeparator:
                        new StatementTerminatorNode(parent, collection[index]);
                        return index + 1;
                    case PSTokenType.GroupEnd:
                        return index;
                    default:
                        new SimpleTokenNode(parent, collection[index]);
                        index++;
                        break;
                }
            }

            return index;
        }

        public override ScriptNodeType NodeType
        {
            get
            {
                switch (TokenType)
                {
                    case PSTokenType.CommandArgument:
                    case PSTokenType.CommandParameter:
                    case PSTokenType.Command:
                        return ScriptNodeType.Command;
                    case PSTokenType.Attribute:
                        return ScriptNodeType.Attribute;
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
                }

                return ScriptNodeType.Unknown;
            }
        }
    }
}