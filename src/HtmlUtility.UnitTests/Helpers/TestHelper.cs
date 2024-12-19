using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using Json.Schema;
using Markdig;
using Markdig.Renderers.Html;
using Markdig.Syntax;

namespace HtmlUtility.UnitTests.Helpers;

public static class TestHelper
{
    internal static string GetResourcesDirectoryPath() => Path.Combine(TestContext.CurrentContext.TestDirectory, "Resources");

    internal static void AddMarkdownJsonTestAttachment(MarkdownDocument document, string sourceFileName, string jsonFileName)
    {
        static JsonObject createJsonObject(MarkdownObject obj, int level, int index)
        {
            JsonObject position = new()
            {
                { "Level", JsonValue.Create(level) },
                { "Index", JsonValue.Create(index) }
            };
            JsonObject result = new()
            {
                { "Name", JsonValue.Create(obj.GetType().Name) },
                { "Position", position }
            };
            level++;
            index = -1;

            JsonArray arr = new();
            var a = obj.TryGetAttributes();
            if (a is not null)
            {
                var aPos = new JsonObject()
                {
                    { "Level", JsonValue.Create(level) }
                };
                if (!a.Span.IsEmpty)
                {
                    aPos.Add("Line", JsonValue.Create(a.Line));
                    aPos.Add("Column", JsonValue.Create(a.Column));
                    aPos.Add("Start", JsonValue.Create(a.Span.Start));
                    aPos.Add("End", JsonValue.Create(a.Span.End));
                }
                JsonObject attr = new()
                {
                    { "Name", JsonValue.Create(a.GetType().Name) },
                    { "Position", aPos }
                };
                if (!string.IsNullOrEmpty(a.Id))
                    attr.Add("Id", JsonValue.Create(a.Id));
                if (a.Classes is not null && a.Classes.Count > 0)
                {
                    JsonArray ca = new();
                    foreach (var c in a.Classes!)
                    {
                        if (!string.IsNullOrWhiteSpace(c))
                            ca.Add(JsonValue.Create(c));
                    }
                    if (ca.Count > 0)
                        attr.Add("Classes", ca);
                }
                if (a.Properties is not null && a.Properties.Count > 0)
                {
                    JsonArray ca = new();
                    foreach (var kvp in a.Properties)
                    {
                        JsonObject ko = new()
                        {
                            { "Key", JsonValue.Create(kvp.Key) },
                            { "Value", JsonValue.Create(kvp.Value) }
                        };
                        ca.Add(ko);
                    }
                    attr.Add("Properties", ca);
                }
                arr.Add(attr);
            }
            if (obj is ContainerBlock containerBlock)
            {
                position.Add("Line", JsonValue.Create(obj.Line));
                position.Add("Column", JsonValue.Create(obj.Column));
                position.Add("Start", JsonValue.Create(obj.Span.Start));
                position.Add("End", JsonValue.Create(obj.Span.End));
                // if (containerBlock is LinkReferenceDefinitionGroup linkReferenceDefinitionGroup)
                if (containerBlock is ListBlock listBlock)
                {
                    result.Add("IsOrdered", JsonValue.Create(listBlock.IsOrdered));
                    result.Add("BulletType", JsonValue.Create(listBlock.BulletType));
                    if (listBlock.OrderedStart is not null)
                        result.Add("OrderedStart", JsonValue.Create(listBlock.OrderedStart));
                }
                else if (containerBlock is Markdig.Extensions.Tables.TableCell tableCell)
                {
                    result.Add("ColumnIndex", JsonValue.Create(tableCell.ColumnIndex));
                    if (tableCell.ColumnSpan > 1)
                        result.Add("ColumnSpan", JsonValue.Create(tableCell.ColumnSpan));
                    if (tableCell.RowSpan > 1)
                        result.Add("ColumnSpan", JsonValue.Create(tableCell.RowSpan));
                }
                else if (containerBlock is Markdig.Extensions.Tables.TableRow tableRow)
                    result.Add("IsHeader", JsonValue.Create(tableRow.IsHeader));
                else if (containerBlock is Markdig.Extensions.Footnotes.Footnote footnote)
                {
                    if (footnote.Order >= 0)
                        result.Add("Order", JsonValue.Create(footnote.Order));
                    if (footnote.Label is not null)
                        result.Add("Label", JsonValue.Create(footnote.Label));
                }
                else if (containerBlock is Markdig.Extensions.CustomContainers.CustomContainer customContainer)
                {
                    if (customContainer.Arguments is not null)
                        result.Add("Arguments", JsonValue.Create(customContainer.Arguments));
                    if (customContainer.Info is not null)
                        result.Add("Info", JsonValue.Create(customContainer.Info));
                }
                foreach (var o in containerBlock)
                    arr.Add(createJsonObject(o, level, ++index));
            }
            else if (obj is LeafBlock leafBlock)
            {
                if (!obj.Span.IsEmpty)
                {
                    position.Add("Line", JsonValue.Create(obj.Line));
                    position.Add("Column", JsonValue.Create(obj.Column));
                    position.Add("Start", JsonValue.Create(obj.Span.Start));
                    position.Add("End", JsonValue.Create(obj.Span.End));
                }
                if (obj is CodeBlock codeBlock)
                {
                    if (obj is FencedCodeBlock fencedCodeBlock)
                    {
                        result.Add("IndentCount", JsonValue.Create(fencedCodeBlock.IndentCount));
                        if (fencedCodeBlock.Arguments is not null)
                            result.Add("Arguments", JsonValue.Create(fencedCodeBlock.Arguments));
                        if (fencedCodeBlock.Info is not null)
                            result.Add("Info", JsonValue.Create(fencedCodeBlock.Info));
                    }
                }
                else if (obj is ThematicBreakBlock thematicBreakBlock)
                    result.Add("ThematicCharCount", JsonValue.Create(thematicBreakBlock.ThematicCharCount));
                else if (obj is HeadingBlock headingBlock)
                    result.Add("Level", JsonValue.Create(headingBlock.Level));
                else if (obj is Markdig.Extensions.Abbreviations.Abbreviation abbreviation)
                {
                    if (abbreviation.Label is not null)
                        result.Add("Label", JsonValue.Create(abbreviation.Label));
                }
                else if (obj is LinkReferenceDefinition lrd)
                {
                    if (obj is Markdig.Extensions.Footnotes.FootnoteLinkReferenceDefinition footnoteLink)
                    {
                        if (footnoteLink.Footnote.Order < 0)
                        {
                            if (footnoteLink.Footnote.Label is not null)
                                result.Add("Footnote", new JsonObject() { { "Label", JsonValue.Create(footnoteLink.Footnote.Label) } });
                        }
                        else if (footnoteLink.Footnote.Label is null)
                            result.Add("Order", new JsonObject() { { "Order", JsonValue.Create(footnoteLink.Footnote.Order) } });
                        else
                            result.Add("Order", new JsonObject()
                        {
                            { "Order", JsonValue.Create(footnoteLink.Footnote.Order) },
                            { "Label", JsonValue.Create(footnoteLink.Footnote.Label) }
                        });
                    }
                    else if (obj is Markdig.Extensions.AutoIdentifiers.HeadingLinkReferenceDefinition hlr && hlr.Heading is not null)
                    {
                        JsonObject fno = new()
                        {
                            { "Start", hlr.Heading.Span.Start },
                            { "End", hlr.Heading.Span.Start }
                        };
                        result.Add("Heading", fno);
                    }
                    if (!string.IsNullOrEmpty(lrd.Title))
                        result.Add("Title", JsonValue.Create(lrd.Title));
                    if (!string.IsNullOrEmpty(lrd.Label))
                        result.Add("Label", JsonValue.Create(lrd.Label));
                    if (!string.IsNullOrEmpty(lrd.Url))
                        result.Add("Url", JsonValue.Create(lrd.Url));
                }
                if (leafBlock.Inline is not null)
                    foreach (var o in leafBlock.Inline)
                        arr.Add(createJsonObject(o, level, ++index));
            }
            else
            {
                if (obj is Markdig.Extensions.Footnotes.FootnoteLink fnl)
                {
                    position.Add("Line", JsonValue.Create(obj.Line));
                    position.Add("Column", JsonValue.Create(obj.Column));
                    if (!obj.Span.IsEmpty)
                    {
                        position.Add("Start", JsonValue.Create(obj.Span.Start));
                        position.Add("End", JsonValue.Create(obj.Span.End));
                    }
                    if (fnl.Footnote is not null)
                    {
                        JsonObject fno = new()
                        {
                            { "Start", fnl.Footnote.Span.Start },
                            { "End", fnl.Footnote.Span.Start }
                        };
                        result.Add("Footnote", fno);
                    }
                    result.Add("Index", JsonValue.Create(fnl.Index));
                    if (fnl.IsBackLink)
                        result.Add("IsBackLink", JsonValue.Create(true));
                }
                else
                {
                    position.Add("Line", JsonValue.Create(obj.Line));
                    position.Add("Column", JsonValue.Create(obj.Column));
                    position.Add("Start", JsonValue.Create(obj.Span.Start));
                    position.Add("End", JsonValue.Create(obj.Span.End));
                    if (obj is Markdig.Syntax.Inlines.ContainerInline containerInline)
                    {
                        foreach (var o in containerInline)
                            arr.Add(createJsonObject(o, level, ++index));
                        if (containerInline is Markdig.Syntax.Inlines.DelimiterInline delimiterInline)
                        {
                            result.Add("Type", JsonValue.Create(delimiterInline.Type.ToString("F")));
                            if (delimiterInline is Markdig.Syntax.Inlines.EmphasisDelimiterInline emphasisDelimiterInline)
                                result.Add("DelimiterCount", JsonValue.Create(emphasisDelimiterInline.DelimiterCount));
                            else if (delimiterInline is Markdig.Syntax.Inlines.LinkDelimiterInline linkDelimiterInline)
                            {
                                result.Add("DelimiterCount", JsonValue.Create(linkDelimiterInline.IsImage));
                                if (linkDelimiterInline.Label is not null)
                                    result.Add("Label", JsonValue.Create(linkDelimiterInline.Label));
                            }
                            else if (delimiterInline is Markdig.Extensions.Tables.PipeTableDelimiterInline pipeTableDelimiterInline)
                                result.Add("LocalLineIndex", JsonValue.Create(pipeTableDelimiterInline.LocalLineIndex));

                        }
                        else if (containerInline is Markdig.Syntax.Inlines.EmphasisInline emphasisInline)
                        {
                            result.Add("DelimiterChar", JsonValue.Create(emphasisInline.DelimiterChar));
                            result.Add("DelimiterCount", JsonValue.Create(emphasisInline.DelimiterCount));
                        }
                        else if (containerInline is Markdig.Syntax.Inlines.LinkInline linkInline)
                        {
                            if (!string.IsNullOrEmpty(linkInline.Title))
                                result.Add("Title", JsonValue.Create(linkInline.Title));
                            if (!string.IsNullOrEmpty(linkInline.Label))
                                result.Add("Label", JsonValue.Create(linkInline.Label));
                            if (!string.IsNullOrEmpty(linkInline.Url))
                                result.Add("Url", JsonValue.Create(linkInline.Url));
                            if (!string.IsNullOrEmpty(linkInline.LinkRefDefLabel))
                                result.Add("LinkRefDefLabel", JsonValue.Create(linkInline.LinkRefDefLabel));
                            if (linkInline.IsImage)
                                result.Add("IsImage", JsonValue.Create(true));
                            if (linkInline.IsShortcut)
                                result.Add("IsShortcut", JsonValue.Create(true));
                            if (linkInline.IsAutoLink)
                                result.Add("IsAutoLink", JsonValue.Create(true));
                            if (obj is Markdig.Extensions.JiraLinks.JiraLink jiraLink)
                            {
                                result.Add("ProjectKey", JsonValue.Create(jiraLink.ProjectKey.Text));
                                result.Add("ProjectKey", JsonValue.Create(jiraLink.Issue.Text));
                            }
                        }
                    }
                    else if (obj is Markdig.Syntax.Inlines.AutolinkInline autolinkInline)
                    {
                        result.Add("IsEmail", JsonValue.Create(autolinkInline.IsEmail));
                        result.Add("Url", JsonValue.Create(autolinkInline.Url));
                    }
                    else if (obj is Markdig.Syntax.Inlines.CodeInline codeInline)
                        result.Add("Content", JsonValue.Create(codeInline.Content));
                    else if (obj is Markdig.Syntax.Inlines.HtmlEntityInline htmlEntityInline)
                        result.Add("Content", JsonValue.Create(htmlEntityInline.Original.ToString()));
                    else if (obj is Markdig.Syntax.Inlines.LineBreakInline lineBreakInline)
                    {
                        result.Add("IsHard", JsonValue.Create(lineBreakInline.IsHard));
                        result.Add("IsBackslash", JsonValue.Create(lineBreakInline.IsBackslash));
                    }
                    else if (obj is Markdig.Syntax.Inlines.LiteralInline literalInline)
                    {
                        result.Add("Content", JsonValue.Create(literalInline.Content.ToString()));
                        if (obj is Markdig.Extensions.Emoji.EmojiInline emojiInline && emojiInline.Match is not null)
                            result.Add("Match", JsonValue.Create(emojiInline.Match));
                    }
                    else if (obj is Markdig.Extensions.TaskLists.TaskList taskList)
                        result.Add("Checked", JsonValue.Create(taskList.Checked));
                    else if (obj is Markdig.Extensions.SmartyPants.SmartyPant smartyPant)
                        result.Add("Type", JsonValue.Create(smartyPant.Type.ToString("F")));
                    else if (obj is Markdig.Extensions.Mathematics.MathInline mathInline)
                        result.Add("Content", JsonValue.Create(mathInline.Content.ToString()));
                    else if (obj is Markdig.Extensions.Abbreviations.AbbreviationInline abbreviationInline)
                    {
                        JsonObject abbreviation = new() { {"Text", JsonValue.Create(abbreviationInline.Abbreviation.Text.ToString()) } };
                        if (abbreviationInline.Abbreviation.Label is not null)
                            result.Add("Label", JsonValue.Create(abbreviationInline.Abbreviation.Label));
                    }
                }
            }

            if (arr.Count > 0)
                result.Add("Tokens", arr);
            return result;
        }
        JsonObject rootObject = createJsonObject(document, 0, 0);
        string path = Path.Combine(TestContext.CurrentContext.TestDirectory, "Resources", jsonFileName);
        using FileStream stream = new(path, FileMode.Create, FileAccess.Write);
        using (Utf8JsonWriter writer = new(stream))
        {
            rootObject.WriteTo(writer);
            writer.Flush();
            stream.Flush();
        }
        stream.Close();

        // "Position":\s+\{[\r\n\s]+"Level":\s+(\d+),[\r\n\s]+"Index":\s+(\d+),[\r\n\s]+"Line":\s+(\d+),[\r\n\s]+"Column":\s+(\d+),[\r\n\s]+"Start":\s+(\d+),[\r\n\s]+"End":\s+(\d+)[\r\n\s]+\}
        // "Position": { "Level": $1, "Index": $2, "Line": $3, "Column": $4, "Start": $5, "End": $6 }

        // "(\w+)":\s+\{[\r\n\s]+"(\w+)":\s+(\d+),[\r\n\s]+"(\w+)":\s+(\d+)[\r\n\s]+\}
        // "$1": { "$2": $3, "$4": $5 }

        // "(\w+)":\s+\{[\r\n\s]+"(\w+)":\s+(\d+|"[^"]*")[\r\n\s]+\}
        // "$1": { "$2": $3 }

        // "(\w+)":\s+\[[\r\n\s]+("[^"]*")[\r\n\s]+\]
        // "$1": [ $2 ]

        // \\u0026
        // &

        // \\u002B
        // +

        // \\u0060
        // `

        // \\u0027
        // '

        // \\u0022
        // \\"

        TestContext.AddTestAttachment(path, $"JSON node information for contents of {sourceFileName}");

    }

    public static System.Collections.IEnumerable GetToReflectionTypeTestData()
    {
        yield return new TestCaseData(MarkdownTokenType.Block).Returns(typeof(Block));
        yield return new TestCaseData(MarkdownTokenType.BlankLineBlock).Returns(typeof(BlankLineBlock));
        yield return new TestCaseData(MarkdownTokenType.ContainerBlock).Returns(typeof(ContainerBlock));
        yield return new TestCaseData(MarkdownTokenType.CustomContainer).Returns(typeof(Markdig.Extensions.CustomContainers.CustomContainer));
        yield return new TestCaseData(MarkdownTokenType.DefinitionItem).Returns(typeof(Markdig.Extensions.DefinitionLists.DefinitionItem));
        yield return new TestCaseData(MarkdownTokenType.DefinitionList).Returns(typeof(Markdig.Extensions.DefinitionLists.DefinitionList));
        yield return new TestCaseData(MarkdownTokenType.Figure).Returns(typeof(Markdig.Extensions.Figures.Figure));
        yield return new TestCaseData(MarkdownTokenType.FooterBlock).Returns(typeof(Markdig.Extensions.Footers.FooterBlock));
        yield return new TestCaseData(MarkdownTokenType.Footnote).Returns(typeof(Markdig.Extensions.Footnotes.Footnote));
        yield return new TestCaseData(MarkdownTokenType.FootnoteGroup).Returns(typeof(Markdig.Extensions.Footnotes.FootnoteGroup));
        yield return new TestCaseData(MarkdownTokenType.LinkReferenceDefinitionGroup).Returns(typeof(LinkReferenceDefinitionGroup));
        yield return new TestCaseData(MarkdownTokenType.ListBlock).Returns(typeof(ListBlock));
        yield return new TestCaseData(MarkdownTokenType.ListItemBlock).Returns(typeof(ListItemBlock));
        yield return new TestCaseData(MarkdownTokenType.MarkdownDocument).Returns(typeof(MarkdownDocument));
        yield return new TestCaseData(MarkdownTokenType.QuoteBlock).Returns(typeof(QuoteBlock));
        yield return new TestCaseData(MarkdownTokenType.Table).Returns(typeof(Markdig.Extensions.Tables.Table));
        yield return new TestCaseData(MarkdownTokenType.TableCell).Returns(typeof(Markdig.Extensions.Tables.TableCell));
        yield return new TestCaseData(MarkdownTokenType.TableRow).Returns(typeof(Markdig.Extensions.Tables.TableRow));
        yield return new TestCaseData(MarkdownTokenType.LeafBlock).Returns(typeof(LeafBlock));
        yield return new TestCaseData(MarkdownTokenType.Abbreviation).Returns(typeof(Markdig.Extensions.Abbreviations.Abbreviation));
        yield return new TestCaseData(MarkdownTokenType.CodeBlock).Returns(typeof(CodeBlock));
        yield return new TestCaseData(MarkdownTokenType.FencedCodeBlock).Returns(typeof(FencedCodeBlock));
        yield return new TestCaseData(MarkdownTokenType.MathBlock).Returns(typeof(Markdig.Extensions.Mathematics.MathBlock));
        yield return new TestCaseData(MarkdownTokenType.YamlFrontMatterBlock).Returns(typeof(Markdig.Extensions.Yaml.YamlFrontMatterBlock));
        yield return new TestCaseData(MarkdownTokenType.DefinitionTerm).Returns(typeof(Markdig.Extensions.DefinitionLists.DefinitionTerm));
        yield return new TestCaseData(MarkdownTokenType.EmptyBlock).Returns(typeof(EmptyBlock));
        yield return new TestCaseData(MarkdownTokenType.FigureCaption).Returns(typeof(Markdig.Extensions.Figures.FigureCaption));
        yield return new TestCaseData(MarkdownTokenType.HeadingBlock).Returns(typeof(HeadingBlock));
        yield return new TestCaseData(MarkdownTokenType.HtmlBlock).Returns(typeof(HtmlBlock));
        yield return new TestCaseData(MarkdownTokenType.LinkReferenceDefinition).Returns(typeof(LinkReferenceDefinition));
        yield return new TestCaseData(MarkdownTokenType.FootnoteLinkReferenceDefinition).Returns(typeof(Markdig.Extensions.Footnotes.FootnoteLinkReferenceDefinition));
        yield return new TestCaseData(MarkdownTokenType.HeadingLinkReferenceDefinition).Returns(typeof(Markdig.Extensions.AutoIdentifiers.HeadingLinkReferenceDefinition));
        yield return new TestCaseData(MarkdownTokenType.ParagraphBlock).Returns(typeof(ParagraphBlock));
        yield return new TestCaseData(MarkdownTokenType.ThematicBreakBlock).Returns(typeof(ThematicBreakBlock));
        yield return new TestCaseData(MarkdownTokenType.Inline).Returns(typeof(Markdig.Syntax.Inlines.Inline));
        yield return new TestCaseData(MarkdownTokenType.ContainerInline).Returns(typeof(Markdig.Syntax.Inlines.ContainerInline));
        yield return new TestCaseData(MarkdownTokenType.FootnoteLink).Returns(typeof(Markdig.Extensions.Footnotes.FootnoteLink));
        yield return new TestCaseData(MarkdownTokenType.LeafInline).Returns(typeof(Markdig.Syntax.Inlines.LeafInline));
        yield return new TestCaseData(MarkdownTokenType.AbbreviationInline).Returns(typeof(Markdig.Extensions.Abbreviations.AbbreviationInline));
        yield return new TestCaseData(MarkdownTokenType.AutolinkInline).Returns(typeof(Markdig.Syntax.Inlines.AutolinkInline));
        yield return new TestCaseData(MarkdownTokenType.CodeInline).Returns(typeof(Markdig.Syntax.Inlines.CodeInline));
        yield return new TestCaseData(MarkdownTokenType.HtmlEntityInline).Returns(typeof(Markdig.Syntax.Inlines.HtmlEntityInline));
        yield return new TestCaseData(MarkdownTokenType.HtmlInline).Returns(typeof(Markdig.Syntax.Inlines.HtmlInline));
        yield return new TestCaseData(MarkdownTokenType.LineBreakInline).Returns(typeof(Markdig.Syntax.Inlines.LineBreakInline));
        yield return new TestCaseData(MarkdownTokenType.LiteralInline).Returns(typeof(Markdig.Syntax.Inlines.LiteralInline));
        yield return new TestCaseData(MarkdownTokenType.EmojiInline).Returns(typeof(Markdig.Extensions.Emoji.EmojiInline));
        yield return new TestCaseData(MarkdownTokenType.MathInline).Returns(typeof(Markdig.Extensions.Mathematics.MathInline));
        yield return new TestCaseData(MarkdownTokenType.SmartyPant).Returns(typeof(Markdig.Extensions.SmartyPants.SmartyPant));
        yield return new TestCaseData(MarkdownTokenType.TaskList).Returns(typeof(Markdig.Extensions.TaskLists.TaskList));
        yield return new TestCaseData(MarkdownTokenType.DelimiterInline).Returns(typeof(Markdig.Syntax.Inlines.DelimiterInline));
        yield return new TestCaseData(MarkdownTokenType.EmphasisDelimiterInline).Returns(typeof(Markdig.Syntax.Inlines.EmphasisDelimiterInline));
        yield return new TestCaseData(MarkdownTokenType.LinkDelimiterInline).Returns(typeof(Markdig.Syntax.Inlines.LinkDelimiterInline));
        yield return new TestCaseData(MarkdownTokenType.PipeTableDelimiterInline).Returns(typeof(Markdig.Extensions.Tables.PipeTableDelimiterInline));
        yield return new TestCaseData(MarkdownTokenType.EmphasisInline).Returns(typeof(Markdig.Syntax.Inlines.EmphasisInline));
        yield return new TestCaseData(MarkdownTokenType.CustomContainerInline).Returns(typeof(Markdig.Extensions.CustomContainers.CustomContainerInline));
        yield return new TestCaseData(MarkdownTokenType.HtmlAttributes).Returns(typeof(Markdig.Renderers.Html.HtmlAttributes));
        yield return new TestCaseData(MarkdownTokenType.LinkInline).Returns(typeof(Markdig.Syntax.Inlines.LinkInline));
        yield return new TestCaseData(MarkdownTokenType.JiraLink).Returns(typeof(Markdig.Extensions.JiraLinks.JiraLink));
    }

    public static System.Collections.IEnumerable GetToReflectionTypesTestData()
    {
        static TestCaseData createTestCaseData(MarkdownTokenType[] tokenTypes, Type[] returns)
        {
            return new TestCaseData(tokenTypes).Returns(returns).SetArgDisplayNames(tokenTypes.Select(t => t.ToString("F")).ToArray());
        }
        yield return createTestCaseData([MarkdownTokenType.Block], [typeof(Block)]);
        yield return createTestCaseData([MarkdownTokenType.BlankLineBlock], [typeof(BlankLineBlock)]);
        yield return createTestCaseData([MarkdownTokenType.EmphasisInline, MarkdownTokenType.TableRow, MarkdownTokenType.ContainerBlock],
            [typeof(Markdig.Syntax.Inlines.EmphasisInline), typeof(ContainerBlock)]);
        yield return createTestCaseData([MarkdownTokenType.CustomContainer, MarkdownTokenType.Block], [typeof(Block)]);
        yield return createTestCaseData([MarkdownTokenType.DefinitionList, MarkdownTokenType.DefinitionItem],
            [typeof(Markdig.Extensions.DefinitionLists.DefinitionList), typeof(Markdig.Extensions.DefinitionLists.DefinitionItem)]);
        yield return createTestCaseData([MarkdownTokenType.SmartyPant, MarkdownTokenType.DefinitionList, MarkdownTokenType.LiteralInline],
            [typeof(Markdig.Extensions.SmartyPants.SmartyPant), typeof(Markdig.Extensions.DefinitionLists.DefinitionList),
                typeof(Markdig.Syntax.Inlines.LiteralInline)]);
        yield return createTestCaseData([MarkdownTokenType.Figure, MarkdownTokenType.AutolinkInline],
            [typeof(Markdig.Extensions.Figures.Figure), typeof(Markdig.Syntax.Inlines.AutolinkInline)]);
        yield return createTestCaseData([MarkdownTokenType.BlankLineBlock, MarkdownTokenType.FooterBlock, MarkdownTokenType.EmojiInline],
            [typeof(BlankLineBlock), typeof(Markdig.Extensions.Footers.FooterBlock), typeof(Markdig.Extensions.Emoji.EmojiInline)]);
        yield return createTestCaseData([MarkdownTokenType.Footnote], [typeof(Markdig.Extensions.Footnotes.Footnote)]);
        yield return createTestCaseData([MarkdownTokenType.FootnoteGroup], [typeof(Markdig.Extensions.Footnotes.FootnoteGroup)]);
        yield return createTestCaseData([MarkdownTokenType.LinkReferenceDefinitionGroup, MarkdownTokenType.CustomContainerInline, MarkdownTokenType.LeafBlock],
            [typeof(LinkReferenceDefinitionGroup), typeof(Markdig.Extensions.CustomContainers.CustomContainerInline), typeof(LeafBlock)]);
        yield return createTestCaseData([MarkdownTokenType.ListBlock, MarkdownTokenType.AutolinkInline, MarkdownTokenType.ContainerInline],
            [typeof(ListBlock), typeof(Markdig.Syntax.Inlines.AutolinkInline), typeof(Markdig.Syntax.Inlines.ContainerInline)]);
        yield return createTestCaseData([MarkdownTokenType.ListItemBlock, MarkdownTokenType.LinkInline],
            [typeof(ListItemBlock), typeof(Markdig.Syntax.Inlines.LinkInline)]);
        yield return createTestCaseData([MarkdownTokenType.MarkdownDocument, MarkdownTokenType.Figure],
            [typeof(MarkdownDocument), typeof(Markdig.Extensions.Figures.Figure)]);
        yield return createTestCaseData([MarkdownTokenType.ParagraphBlock, MarkdownTokenType.QuoteBlock, MarkdownTokenType.CustomContainerInline],
            [typeof(ParagraphBlock), typeof(QuoteBlock), typeof(Markdig.Extensions.CustomContainers.CustomContainerInline)]);
        yield return createTestCaseData([MarkdownTokenType.Table, MarkdownTokenType.ParagraphBlock],
            [typeof(Markdig.Extensions.Tables.Table), typeof(ParagraphBlock)]);
        yield return createTestCaseData([MarkdownTokenType.TableCell, MarkdownTokenType.MathInline],
            [typeof(Markdig.Extensions.Tables.TableCell), typeof(Markdig.Extensions.Mathematics.MathInline)]);
        yield return createTestCaseData([MarkdownTokenType.TableRow], [typeof(Markdig.Extensions.Tables.TableRow)]);
        yield return createTestCaseData([MarkdownTokenType.Abbreviation, MarkdownTokenType.LeafBlock], [typeof(LeafBlock)]);
        yield return createTestCaseData([MarkdownTokenType.SmartyPant, MarkdownTokenType.Abbreviation],
            [typeof(Markdig.Extensions.SmartyPants.SmartyPant), typeof(Markdig.Extensions.Abbreviations.Abbreviation)]);
        yield return createTestCaseData([MarkdownTokenType.PipeTableDelimiterInline, MarkdownTokenType.CodeBlock, MarkdownTokenType.HtmlInline],
            [typeof(Markdig.Extensions.Tables.PipeTableDelimiterInline), typeof(CodeBlock), typeof(Markdig.Syntax.Inlines.HtmlInline)]);
        yield return createTestCaseData([MarkdownTokenType.FencedCodeBlock, MarkdownTokenType.AbbreviationInline, MarkdownTokenType.FigureCaption],
            [typeof(FencedCodeBlock), typeof(Markdig.Extensions.Abbreviations.AbbreviationInline), typeof(Markdig.Extensions.Figures.FigureCaption)]);
        yield return createTestCaseData([MarkdownTokenType.MathBlock], [typeof(Markdig.Extensions.Mathematics.MathBlock)]);
        yield return createTestCaseData([MarkdownTokenType.YamlFrontMatterBlock], [typeof(Markdig.Extensions.Yaml.YamlFrontMatterBlock)]);
        yield return createTestCaseData([MarkdownTokenType.DefinitionTerm, MarkdownTokenType.QuoteBlock],
            [typeof(Markdig.Extensions.DefinitionLists.DefinitionTerm), typeof(QuoteBlock)]);
        yield return createTestCaseData([MarkdownTokenType.EmptyBlock, MarkdownTokenType.HeadingBlock, MarkdownTokenType.FootnoteLinkReferenceDefinition],
            [typeof(EmptyBlock), typeof(HeadingBlock), typeof(Markdig.Extensions.Footnotes.FootnoteLinkReferenceDefinition)]);
        yield return createTestCaseData([MarkdownTokenType.HtmlAttributes, MarkdownTokenType.FigureCaption],
            [typeof(HtmlAttributes), typeof(Markdig.Extensions.Figures.FigureCaption)]);
        yield return createTestCaseData([MarkdownTokenType.HeadingBlock, MarkdownTokenType.Inline],
            [typeof(HeadingBlock), typeof(Markdig.Syntax.Inlines.Inline)]);
        yield return createTestCaseData([MarkdownTokenType.HtmlBlock], [typeof(HtmlBlock)]);
        yield return createTestCaseData([MarkdownTokenType.LiteralInline, MarkdownTokenType.LinkReferenceDefinition, MarkdownTokenType.HtmlAttributes],
            [typeof(Markdig.Syntax.Inlines.LiteralInline), typeof(LinkReferenceDefinition), typeof(HtmlAttributes)]);
        yield return createTestCaseData([MarkdownTokenType.HtmlInline, MarkdownTokenType.MathBlock, MarkdownTokenType.FootnoteLinkReferenceDefinition],
            [typeof(Markdig.Syntax.Inlines.HtmlInline), typeof(Markdig.Extensions.Mathematics.MathBlock),
                typeof(Markdig.Extensions.Footnotes.FootnoteLinkReferenceDefinition)]);
        yield return createTestCaseData([MarkdownTokenType.HeadingLinkReferenceDefinition], [typeof(Markdig.Extensions.AutoIdentifiers.HeadingLinkReferenceDefinition)]);
        yield return createTestCaseData([MarkdownTokenType.ParagraphBlock], [typeof(ParagraphBlock)]);
        yield return createTestCaseData([MarkdownTokenType.LinkInline, MarkdownTokenType.ThematicBreakBlock],
            [typeof(Markdig.Syntax.Inlines.LinkInline), typeof(ThematicBreakBlock)]);
        yield return createTestCaseData([MarkdownTokenType.AutolinkInline, MarkdownTokenType.Inline], [typeof(Markdig.Syntax.Inlines.Inline)]);
        yield return createTestCaseData([MarkdownTokenType.LiteralInline, MarkdownTokenType.ContainerInline],
            [typeof(Markdig.Syntax.Inlines.LiteralInline), typeof(Markdig.Syntax.Inlines.ContainerInline)]);
        yield return createTestCaseData([MarkdownTokenType.FootnoteLink, MarkdownTokenType.Block],
            [typeof(Markdig.Extensions.Footnotes.FootnoteLink), typeof(Block)]);
        yield return createTestCaseData([MarkdownTokenType.FencedCodeBlock, MarkdownTokenType.LeafInline, MarkdownTokenType.LiteralInline],
            [typeof(FencedCodeBlock), typeof(Markdig.Syntax.Inlines.LeafInline)]);
        yield return createTestCaseData([MarkdownTokenType.LinkInline, MarkdownTokenType.AbbreviationInline],
            [typeof(Markdig.Syntax.Inlines.LinkInline), typeof(Markdig.Extensions.Abbreviations.AbbreviationInline)]);
        yield return createTestCaseData([MarkdownTokenType.AutolinkInline], [typeof(Markdig.Syntax.Inlines.AutolinkInline)]);
        yield return createTestCaseData([MarkdownTokenType.CodeInline, MarkdownTokenType.EmphasisInline, MarkdownTokenType.HtmlInline],
            [typeof(Markdig.Syntax.Inlines.CodeInline), typeof(Markdig.Syntax.Inlines.EmphasisInline), typeof(Markdig.Syntax.Inlines.HtmlInline)]);
        yield return createTestCaseData([MarkdownTokenType.HtmlEntityInline], [typeof(Markdig.Syntax.Inlines.HtmlEntityInline)]);
        yield return createTestCaseData([MarkdownTokenType.HtmlInline, MarkdownTokenType.Table],
            [typeof(Markdig.Syntax.Inlines.HtmlInline), typeof(Markdig.Extensions.Tables.Table)]);
        yield return createTestCaseData([MarkdownTokenType.Footnote, MarkdownTokenType.LineBreakInline],
            [typeof(Markdig.Extensions.Footnotes.Footnote), typeof(Markdig.Syntax.Inlines.LineBreakInline)]);
        yield return createTestCaseData([MarkdownTokenType.LiteralInline, MarkdownTokenType.FooterBlock],
            [typeof(Markdig.Syntax.Inlines.LiteralInline), typeof(Markdig.Extensions.Footers.FooterBlock)]);
        yield return createTestCaseData([MarkdownTokenType.EmojiInline], [typeof(Markdig.Extensions.Emoji.EmojiInline)]);
        yield return createTestCaseData([MarkdownTokenType.HeadingLinkReferenceDefinition, MarkdownTokenType.LeafBlock, MarkdownTokenType.MathInline],
            [typeof(LeafBlock), typeof(Markdig.Extensions.Mathematics.MathInline)]);
        yield return createTestCaseData([MarkdownTokenType.SmartyPant, MarkdownTokenType.BlankLineBlock, MarkdownTokenType.DefinitionTerm],
            [typeof(Markdig.Extensions.SmartyPants.SmartyPant), typeof(BlankLineBlock), typeof(Markdig.Extensions.DefinitionLists.DefinitionTerm)]);
        yield return createTestCaseData([MarkdownTokenType.TaskList, MarkdownTokenType.TaskList], [typeof(Markdig.Extensions.TaskLists.TaskList)]);
        yield return createTestCaseData([MarkdownTokenType.Figure, MarkdownTokenType.DelimiterInline],
            [typeof(Markdig.Extensions.Figures.Figure), typeof(Markdig.Syntax.Inlines.DelimiterInline)]);
        yield return createTestCaseData([MarkdownTokenType.EmphasisDelimiterInline, MarkdownTokenType.EmptyBlock],
            [typeof(Markdig.Syntax.Inlines.EmphasisDelimiterInline), typeof(EmptyBlock)]);
        yield return createTestCaseData([MarkdownTokenType.LinkDelimiterInline, MarkdownTokenType.HtmlInline],
            [typeof(Markdig.Syntax.Inlines.LinkDelimiterInline), typeof(Markdig.Syntax.Inlines.HtmlInline)]);
        yield return createTestCaseData([MarkdownTokenType.LinkReferenceDefinition, MarkdownTokenType.PipeTableDelimiterInline, MarkdownTokenType.LinkReferenceDefinitionGroup],
            [typeof(LinkReferenceDefinition), typeof(Markdig.Extensions.Tables.PipeTableDelimiterInline), typeof(LinkReferenceDefinitionGroup)]);
        yield return createTestCaseData([MarkdownTokenType.LeafInline, MarkdownTokenType.EmphasisInline],
            [typeof(Markdig.Syntax.Inlines.LeafInline), typeof(Markdig.Syntax.Inlines.EmphasisInline)]);
        yield return createTestCaseData([MarkdownTokenType.CustomContainerInline, MarkdownTokenType.MathInline, MarkdownTokenType.EmojiInline],
            [typeof(Markdig.Extensions.CustomContainers.CustomContainerInline), typeof(Markdig.Extensions.Mathematics.MathInline), typeof(Markdig.Extensions.Emoji.EmojiInline)]);
        yield return createTestCaseData([MarkdownTokenType.EmphasisDelimiterInline, MarkdownTokenType.YamlFrontMatterBlock, MarkdownTokenType.HtmlAttributes],
            [typeof(Markdig.Syntax.Inlines.EmphasisDelimiterInline), typeof(Markdig.Extensions.Yaml.YamlFrontMatterBlock), typeof(Markdig.Renderers.Html.HtmlAttributes)]);
        yield return createTestCaseData([MarkdownTokenType.LinkInline, MarkdownTokenType.HeadingLinkReferenceDefinition],
            [typeof(Markdig.Syntax.Inlines.LinkInline), typeof(Markdig.Extensions.AutoIdentifiers.HeadingLinkReferenceDefinition)]);
        yield return createTestCaseData([MarkdownTokenType.JiraLink], [typeof(Markdig.Extensions.JiraLinks.JiraLink)]);
        yield return new TestCaseData(Array.Empty<MarkdownTokenType>()).Returns(null);
        yield return new TestCaseData(null).Returns(null);
    }
}