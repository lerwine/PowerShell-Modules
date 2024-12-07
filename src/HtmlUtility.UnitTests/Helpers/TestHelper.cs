using System.Text.Json;
using System.Text.Json.Nodes;
using Markdig;
using Markdig.Renderers.Html;
using Markdig.Syntax;

namespace HtmlUtility.UnitTests.Helpers;

public static class TestHelper
{
    internal static string GetResourcesDirectoryPath() => Path.Combine(TestContext.CurrentContext.TestDirectory, "Resources");

    internal static void AddMarkdownJsonTestAttachment(MarkdownDocument document, string sourceFileName, string jsonFileName)
    {
        static JsonObject createJsonObject(MarkdownObject obj)
        {
            JsonObject result = new()
            {
                { "Name", JsonValue.Create(obj.GetType().Name) }
            };
            JsonArray arr = new();
            if (obj is HtmlAttributes a)
            {
                if (!string.IsNullOrEmpty(a.Id))
                    result.Add("Id", JsonValue.Create(a.Id));
                if (a.Classes is not null && a.Classes.Count > 0)
                {
                    JsonArray ca = new();
                    foreach (var c in a.Classes!)
                    {
                        if (!string.IsNullOrWhiteSpace(c))
                            ca.Add(JsonValue.Create(c));
                    }
                    if (ca.Count > 0)
                        result.Add("Classes", ca);
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
                    result.Add("Properties", ca);
                }
            }
            else
            {
                if ((a = obj.TryGetAttributes()!) is not null)
                    arr.Add(createJsonObject(a));
                if (obj is Markdig.Extensions.Footnotes.FootnoteLink fnl)
                {
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
                else if (obj is LinkReferenceDefinition lrd)
                {
                    if (lrd.Inline is not null)
                        arr.Add(createJsonObject(lrd.Inline));
                    if (obj is Markdig.Extensions.AutoIdentifiers.HeadingLinkReferenceDefinition hlr && hlr.Heading is not null)
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
                else
                {
                    if (obj is ContainerBlock cb)
                    {
                        JsonObject pos = new()
                        {
                            { "Line", JsonValue.Create(obj.Line) },
                            { "Column", JsonValue.Create(obj.Column) }
                        };
                        result.Add("Position", pos);
                        pos = new()
                        {
                            { "Start", JsonValue.Create(obj.Span.Start) },
                            { "End", JsonValue.Create(obj.Span.End) }
                        };
                        result.Add("Span", pos);
                        foreach (var o in cb)
                            arr.Add(createJsonObject(o));
                    }
                    else if (obj is Markdig.Syntax.Inlines.ContainerInline ci)
                    {
                        foreach (var o in ci)
                            arr.Add(createJsonObject(o));
                        if (obj is Markdig.Syntax.Inlines.LinkInline li)
                        {
                            if (!string.IsNullOrEmpty(li.Title))
                                result.Add("Title", JsonValue.Create(li.Title));
                            if (!string.IsNullOrEmpty(li.Label))
                                result.Add("Label", JsonValue.Create(li.Label));
                            if (!string.IsNullOrEmpty(li.Url))
                                result.Add("Url", JsonValue.Create(li.Url));
                            if (!string.IsNullOrEmpty(li.LinkRefDefLabel))
                                result.Add("LinkRefDefLabel", JsonValue.Create(li.LinkRefDefLabel));
                            if (li.IsImage)
                                result.Add("IsImage", JsonValue.Create(true));
                            if (li.IsShortcut)
                                result.Add("IsShortcut", JsonValue.Create(true));
                            if (li.IsAutoLink)
                                result.Add("IsAutoLink", JsonValue.Create(true));
                        }
                    }
                    else
                    {
                        JsonObject pos = new()
                        {
                            { "Line", JsonValue.Create(obj.Line) },
                            { "Column", JsonValue.Create(obj.Column) }
                        };
                        result.Add("Position", pos);
                        pos = new()
                        {
                            { "Start", JsonValue.Create(obj.Span.Start) },
                            { "End", JsonValue.Create(obj.Span.End) }
                        };
                        result.Add("Span", pos);
                        if (obj is LeafBlock blk && blk.Inline is not null)
                            arr.Add(createJsonObject(blk.Inline));
                        else if (obj is Markdig.Syntax.Inlines.Inline)
                            result.Add("Text", obj.ToString());
                    }
                }
                if (arr.Count > 0)
                    result.Add("Tokens", arr);
            }
            return result;
        }
        JsonObject rootObject = createJsonObject(document);
        string path = Path.Combine(TestContext.CurrentContext.TestDirectory, "Resources", jsonFileName);
        using FileStream stream = File.OpenWrite(path);
        using (Utf8JsonWriter writer = new(stream))
        {
            rootObject.WriteTo(writer);
            writer.Flush();
            stream.Flush();
        }
        stream.Close();
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