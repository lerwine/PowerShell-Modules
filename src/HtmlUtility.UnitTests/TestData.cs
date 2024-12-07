using System.Text.Json;
using System.Text.Json.Nodes;
using Markdig;
using Markdig.Renderers.Html;
using Markdig.Syntax;

namespace HtmlUtility.UnitTests;

public static class TestData
{
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

    public static string GetExampleMarkdownText() => File.ReadAllText(Path.Combine(TestContext.CurrentContext.TestDirectory, @"TestData\Example.md"));
    
    public static MarkdownDocument GetExampleMarkdownDocument() => Markdown.Parse(GetExampleMarkdownText(), new MarkdownPipelineBuilder().UseAdvancedExtensions().Build());

    public static System.Collections.IEnumerable GetGetChildObjectsTestData()
    {
        MarkdownDocument document = GetExampleMarkdownDocument();

        static Tuple<Type, SourceSpan> toReturnsTuple(MarkdownObject obj)
        {
            return new Tuple<Type, SourceSpan>(obj.GetType(), obj.Span);
        }

        static TestCaseData containerBlockToTestCaseData(ContainerBlock cb)
        {
            var attr = cb.TryGetAttributes();
            if (attr is null)
                return new TestCaseData(cb).Returns(cb.Select(toReturnsTuple).ToArray())
                    .SetArgDisplayNames($"{cb.GetType().Name}: Line {cb.Line}");
            return new TestCaseData(cb).Returns(new MarkdownObject[] { attr }.Concat(cb).Select(toReturnsTuple).ToArray())
                .SetArgDisplayNames($"{cb.GetType().Name}: Line {cb.Line}");
        }
        static TestCaseData containerInlineToTestCaseData(Markdig.Syntax.Inlines.ContainerInline ci)
        {
            var attr = ci.TryGetAttributes();
            if (attr is null)
                return new TestCaseData(ci).Returns(ci.Select(toReturnsTuple).ToArray())
                    .SetArgDisplayNames($"{ci.GetType().Name}: Line {ci.Line}, Column {ci.Column}");
            return new TestCaseData(ci).Returns(new MarkdownObject[] { attr }.Concat(ci).Select(toReturnsTuple).ToArray())
                .SetArgDisplayNames($"{ci.GetType().Name}: Line {ci.Line}, Column {ci.Column}");
        }

        static TestCaseData leafBlockToTestCaseData(LeafBlock lb)
        {
            var attr = lb.TryGetAttributes();
            var leaf = lb.Inline;
            if (attr is null)
            {
                if (leaf is null)
                    return new TestCaseData(lb).Returns(Array.Empty<Tuple<Type, SourceSpan>>())
                        .SetArgDisplayNames($"{lb.GetType().Name}: Line {lb.Line}");
                return new TestCaseData(lb).Returns(new Tuple<Type, SourceSpan>[] { toReturnsTuple(leaf) })
                    .SetArgDisplayNames($"{lb.GetType().Name}: Line {lb.Line}");
            }
            if (leaf is null)
                return new TestCaseData(lb).Returns(new Tuple<Type, SourceSpan>[] { toReturnsTuple(attr) })
                    .SetArgDisplayNames($"{lb.GetType().Name}: Line {lb.Line}");

            return new TestCaseData(lb).Returns(new Tuple<Type, SourceSpan>[] { toReturnsTuple(attr), toReturnsTuple(leaf) })
                .SetArgDisplayNames($"{lb.GetType().Name}: Line {lb.Line}");
        }
        
        static TestCaseData nonContainerTestCaseData(MarkdownObject lb)
        {
            var attr = lb.TryGetAttributes();
            if (attr is null)
            {
                if (lb is Markdig.Syntax.Inlines.Inline)
                    return new TestCaseData(lb).Returns(Array.Empty<Tuple<Type, SourceSpan>>())
                        .SetArgDisplayNames($"{lb.GetType().Name}: Line {lb.Line}, Column: {lb.Column}");
                return new TestCaseData(lb).Returns(Array.Empty<Tuple<Type, SourceSpan>>())
                    .SetArgDisplayNames($"{lb.GetType().Name}: Line {lb.Line}");
            }
            if (lb is Markdig.Syntax.Inlines.Inline)
                return new TestCaseData(lb).Returns(new Tuple<Type, SourceSpan>[] { toReturnsTuple(attr) })
                    .SetArgDisplayNames($"{lb.GetType().Name}: Line {lb.Line}, Column: {lb.Column}");
                
            return new TestCaseData(lb).Returns(new Tuple<Type, SourceSpan>[] { toReturnsTuple(attr) })
                .SetArgDisplayNames($"{lb.GetType().Name}: Line {lb.Line}");
        }
        

        yield return containerBlockToTestCaseData(document);

        // # Example Markdown Document
        HeadingBlock headingBlock = (HeadingBlock)document[0];
        yield return leafBlockToTestCaseData(headingBlock); // Has attribute
        yield return containerInlineToTestCaseData(headingBlock.Inline!);
        yield return nonContainerTestCaseData(headingBlock.Inline!.FirstChild!);

        // [CommonMark Spec](https://spec.commonmark.org/0.31.2/)
        ParagraphBlock paragraphBlock = (ParagraphBlock)document[1];
        yield return leafBlockToTestCaseData(paragraphBlock);
        yield return containerInlineToTestCaseData(paragraphBlock.Inline!);
        Markdig.Syntax.Inlines.ContainerInline containerInline = (Markdig.Syntax.Inlines.ContainerInline)paragraphBlock.Inline!.FirstChild!;
        yield return containerInlineToTestCaseData(containerInline);
        yield return nonContainerTestCaseData(containerInline.FirstChild!);
        // Hard line break\
        // here
        paragraphBlock = (ParagraphBlock)document[2];
        yield return leafBlockToTestCaseData(paragraphBlock);
        containerInline = paragraphBlock.Inline!;
        yield return containerInlineToTestCaseData(containerInline);
        Markdig.Syntax.Inlines.Inline inline = containerInline.FirstChild!;
        yield return nonContainerTestCaseData(inline);
        yield return nonContainerTestCaseData(inline.NextSibling!);
        yield return nonContainerTestCaseData(containerInline.LastChild!);

        ContainerBlock containerBlock = (ContainerBlock)document[3];
        yield return containerBlockToTestCaseData(containerBlock); // Has attribute

        // - [X] Task
        ContainerBlock innerContainerBlock = (ContainerBlock)containerBlock[0];
        yield return containerBlockToTestCaseData(innerContainerBlock); // Has attribute
        paragraphBlock = (ParagraphBlock)innerContainerBlock[0];
        yield return leafBlockToTestCaseData(paragraphBlock);
        containerInline = paragraphBlock.Inline!;
        yield return containerInlineToTestCaseData(containerInline);
        yield return nonContainerTestCaseData(containerInline.FirstChild!);
        yield return nonContainerTestCaseData(containerInline.LastChild!);

        // - [ ] List Item
        innerContainerBlock = (ContainerBlock)containerBlock[1];
        yield return containerBlockToTestCaseData(innerContainerBlock); // Has attribute
        paragraphBlock = (ParagraphBlock)innerContainerBlock[0];
        yield return leafBlockToTestCaseData(paragraphBlock);
        containerInline = paragraphBlock.Inline!;
        yield return containerInlineToTestCaseData(containerInline);
        yield return nonContainerTestCaseData(containerInline.FirstChild!);
        yield return nonContainerTestCaseData(containerInline.LastChild!);

        // - Normal List
        innerContainerBlock = (ContainerBlock)containerBlock[2];
        yield return containerBlockToTestCaseData(innerContainerBlock);
        paragraphBlock = (ParagraphBlock)innerContainerBlock[0];
        yield return leafBlockToTestCaseData(paragraphBlock);
        containerInline = paragraphBlock.Inline!;
        yield return containerInlineToTestCaseData(containerInline);
        yield return nonContainerTestCaseData(containerInline.FirstChild!);

        // - Item
        innerContainerBlock = (ContainerBlock)containerBlock[3];
        yield return containerBlockToTestCaseData(innerContainerBlock);
        paragraphBlock = (ParagraphBlock)innerContainerBlock[0];
        yield return leafBlockToTestCaseData(paragraphBlock);
        containerInline = paragraphBlock.Inline!;
        yield return containerInlineToTestCaseData(containerInline);
        yield return nonContainerTestCaseData(containerInline.FirstChild!);

        // ## Abbreviations {#custom-id}

        // *[HTML]: Hyper Text Markup Language
        // *[W3C]:  World Wide Web Consortium

        headingBlock = (HeadingBlock)document[4];
        yield return leafBlockToTestCaseData(headingBlock); // Has attribute
        yield return containerInlineToTestCaseData(headingBlock.Inline!);
        yield return nonContainerTestCaseData(headingBlock.Inline!.FirstChild!);

        // ## Link
        headingBlock = (HeadingBlock)document[5];
        yield return leafBlockToTestCaseData(headingBlock); // Has attribute
        yield return containerInlineToTestCaseData(headingBlock.Inline!);
        yield return nonContainerTestCaseData(headingBlock.Inline!.FirstChild!);

        // [Abbreviations Link](#custom-id)
        paragraphBlock = (ParagraphBlock)document[6];
        yield return leafBlockToTestCaseData(paragraphBlock);
        yield return containerInlineToTestCaseData(paragraphBlock.Inline!);
        containerInline = (Markdig.Syntax.Inlines.ContainerInline)paragraphBlock.Inline!.FirstChild!;
        yield return containerInlineToTestCaseData(containerInline);
        yield return nonContainerTestCaseData(containerInline.FirstChild!);

        // ## Image with Alt and Title
        headingBlock = (HeadingBlock)document[7];
        yield return leafBlockToTestCaseData(headingBlock); // Has attribute
        yield return containerInlineToTestCaseData(headingBlock.Inline!);
        yield return nonContainerTestCaseData(headingBlock.Inline!.FirstChild!);

        // ![alt attribute goes here](./sn-logo.jpg "This is a Title" )
        paragraphBlock = (ParagraphBlock)document[8];
        yield return leafBlockToTestCaseData(paragraphBlock);
        yield return containerInlineToTestCaseData(paragraphBlock.Inline!);
        containerInline = (Markdig.Syntax.Inlines.ContainerInline)paragraphBlock.Inline!.FirstChild!;
        yield return containerInlineToTestCaseData(containerInline);
        yield return nonContainerTestCaseData(containerInline.FirstChild!);

        // ![foo *bar*]
        // [foo *bar*]: ./sn-logo.jpg "train & tracks"
        paragraphBlock = (ParagraphBlock)document[9];
        yield return leafBlockToTestCaseData(paragraphBlock);
        yield return containerInlineToTestCaseData(paragraphBlock.Inline!);
        containerInline = (Markdig.Syntax.Inlines.ContainerInline)paragraphBlock.Inline!.FirstChild!;
        yield return containerInlineToTestCaseData(containerInline);
        inline = containerInline.FirstChild!;
        yield return nonContainerTestCaseData(inline);
        containerInline = (Markdig.Syntax.Inlines.ContainerInline)inline.NextSibling!;
        yield return containerInlineToTestCaseData(containerInline);
        yield return nonContainerTestCaseData(containerInline.FirstChild!);

        containerBlock = (ContainerBlock)document[10];
        yield return containerBlockToTestCaseData(containerBlock);
        for (var i = 0; i < containerBlock.Count; i++)
            yield return leafBlockToTestCaseData((LeafBlock)containerBlock[0]);

        // ## Math
        headingBlock = (HeadingBlock)document[11];
        yield return leafBlockToTestCaseData(headingBlock); // Has attribute
        yield return containerInlineToTestCaseData(headingBlock.Inline!);
        yield return nonContainerTestCaseData(headingBlock.Inline!.FirstChild!);

        // This sentence uses `$` delimiters to show math inline: $\sqrt{3x-1}+(1+x)^2$
        paragraphBlock = (ParagraphBlock)document[12];
        yield return leafBlockToTestCaseData(paragraphBlock);
        yield return containerInlineToTestCaseData(paragraphBlock.Inline!);
        inline = paragraphBlock.Inline!.FirstChild!;
        yield return nonContainerTestCaseData(inline);
        inline = inline.NextSibling!;
        yield return nonContainerTestCaseData(inline);
        inline = inline.NextSibling!;
        yield return nonContainerTestCaseData(inline);
        yield return nonContainerTestCaseData(inline.NextSibling!); // Has attribute

        // This sentence uses $\` and \`$ delimiters to show math inline: $`\sqrt{3x-1}+(1+x)^2`$
        paragraphBlock = (ParagraphBlock)document[13];
        yield return leafBlockToTestCaseData(paragraphBlock);
        yield return containerInlineToTestCaseData(paragraphBlock.Inline!);
        inline = paragraphBlock.Inline!.FirstChild!;
        yield return nonContainerTestCaseData(inline);
        inline = inline.NextSibling!;
        yield return nonContainerTestCaseData(inline); // Has attribute
        inline = inline.NextSibling!;
        yield return nonContainerTestCaseData(inline);
        yield return nonContainerTestCaseData(inline.NextSibling!); // Has attribute

        // **The Cauchy-Schwarz Inequality**
        // $$\left( \sum_{k=1}^n a_k b_k \right)^2 \leq \left( \sum_{k=1}^n a_k^2 \right) \left( \sum_{k=1}^n b_k^2 \right)$$
        paragraphBlock = (ParagraphBlock)document[14];
        yield return leafBlockToTestCaseData(paragraphBlock);
        containerInline = paragraphBlock.Inline!;
        yield return containerInlineToTestCaseData(containerInline);
        containerInline = (Markdig.Syntax.Inlines.ContainerInline)containerInline.FirstChild!;
        yield return containerInlineToTestCaseData(containerInline);
        yield return nonContainerTestCaseData(containerInline.FirstChild!);
        inline = containerInline.NextSibling!;
        yield return nonContainerTestCaseData(inline);
        yield return nonContainerTestCaseData(inline.NextSibling!); // Has attribute

        // ## Definition List
        headingBlock = (HeadingBlock)document[15];
        yield return leafBlockToTestCaseData(headingBlock); // Has attribute
        yield return containerInlineToTestCaseData(headingBlock.Inline!);
        yield return nonContainerTestCaseData(headingBlock.Inline!.FirstChild!);

        containerBlock = (ContainerBlock)document[16];
        yield return containerBlockToTestCaseData(containerBlock);
        innerContainerBlock = (ContainerBlock)containerBlock[0];
        yield return containerBlockToTestCaseData(innerContainerBlock);
        // Apple
        LeafBlock leafBlock = (LeafBlock)innerContainerBlock[0];
        yield return leafBlockToTestCaseData(leafBlock);
        yield return containerInlineToTestCaseData(leafBlock.Inline!);
        yield return nonContainerTestCaseData(leafBlock.Inline!.FirstChild!);
        // :   Pomaceous fruit of plants of the genus Malus in
        //     the family Rosaceae.
        leafBlock = (LeafBlock)innerContainerBlock[1];
        yield return leafBlockToTestCaseData(leafBlock);
        yield return containerInlineToTestCaseData(leafBlock.Inline!);
        inline = leafBlock.Inline!.FirstChild!;
        yield return nonContainerTestCaseData(inline);
        inline = inline.NextSibling!;
        yield return nonContainerTestCaseData(inline);
        yield return nonContainerTestCaseData(inline.NextSibling!);

        innerContainerBlock = (ContainerBlock)containerBlock[1];
        yield return containerBlockToTestCaseData(innerContainerBlock);
        // Orange
        leafBlock = (LeafBlock)innerContainerBlock[0];
        yield return leafBlockToTestCaseData(leafBlock);
        yield return containerInlineToTestCaseData(leafBlock.Inline!);
        yield return nonContainerTestCaseData(leafBlock.Inline!.FirstChild!);
        // :   The fruit of an evergreen tree of the genus Citrus.
        leafBlock = (LeafBlock)innerContainerBlock[1];
        yield return leafBlockToTestCaseData(leafBlock);
        yield return containerInlineToTestCaseData(leafBlock.Inline!);
        yield return nonContainerTestCaseData(leafBlock.Inline!.FirstChild!);

        // ## Code
        headingBlock = (HeadingBlock)document[17];
        yield return leafBlockToTestCaseData(headingBlock); // Has attribute
        yield return containerInlineToTestCaseData(headingBlock.Inline!);
        yield return nonContainerTestCaseData(headingBlock.Inline!.FirstChild!);

        // This is `inline code`.
        paragraphBlock = (ParagraphBlock)document[18];
        yield return leafBlockToTestCaseData(paragraphBlock);
        yield return containerInlineToTestCaseData(paragraphBlock.Inline!);
        inline = paragraphBlock.Inline!.FirstChild!;
        yield return nonContainerTestCaseData(inline);
        inline = inline.NextSibling!;
        yield return nonContainerTestCaseData(inline);
        yield return nonContainerTestCaseData(inline.NextSibling!);

        // ```log
        // This is a fenced code block
        // ```
        yield return leafBlockToTestCaseData((LeafBlock)document[19]); // Has attribute

        // ``` { .html #codeId style="color: #333; background: #f8f8f8;" }
        // This is a fenced code block
        // with an ID and style
        // ```
        yield return leafBlockToTestCaseData((LeafBlock)document[20]); // Has attribute
        // ((LeafBlock)document[20]).Inline

        // ## Attribute lists
        headingBlock = (HeadingBlock)document[21];
        yield return leafBlockToTestCaseData(headingBlock); // Has attribute
        yield return containerInlineToTestCaseData(headingBlock.Inline!);
        yield return nonContainerTestCaseData(headingBlock.Inline!.FirstChild!);

        // This is red paragraph.
        // {: style="color: #333; color: #ff0000;" }
        paragraphBlock = (ParagraphBlock)document[22];
        yield return leafBlockToTestCaseData(paragraphBlock);
        yield return containerInlineToTestCaseData(paragraphBlock.Inline!);
        inline = paragraphBlock.Inline!.FirstChild!;
        yield return nonContainerTestCaseData(inline);
        yield return nonContainerTestCaseData(inline.NextSibling!); // Has attribute

        // ## Footnotes
        headingBlock = (HeadingBlock)document[23];
        yield return leafBlockToTestCaseData(headingBlock); // Has attribute
        yield return containerInlineToTestCaseData(headingBlock.Inline!);
        yield return nonContainerTestCaseData(headingBlock.Inline!.FirstChild!);

        paragraphBlock = (ParagraphBlock)document[24];
        yield return leafBlockToTestCaseData(paragraphBlock);
        yield return containerInlineToTestCaseData(paragraphBlock.Inline!);
        inline = paragraphBlock.Inline!.FirstChild!;
        yield return nonContainerTestCaseData(inline);
        inline = inline.NextSibling!;
        yield return nonContainerTestCaseData(inline);
        inline = inline.NextSibling!;
        yield return nonContainerTestCaseData(inline);
        inline = inline.NextSibling!;
        yield return nonContainerTestCaseData(inline);
        yield return nonContainerTestCaseData(inline.NextSibling!);

        // ### Smarties
        headingBlock = (HeadingBlock)document[25];
        yield return leafBlockToTestCaseData(headingBlock); // Has attribute
        yield return containerInlineToTestCaseData(headingBlock.Inline!);
        yield return nonContainerTestCaseData(headingBlock.Inline!.FirstChild!);

        // << angle quotes >>
        paragraphBlock = (ParagraphBlock)document[26];
        yield return leafBlockToTestCaseData(paragraphBlock);
        yield return containerInlineToTestCaseData(paragraphBlock.Inline!);
        yield return nonContainerTestCaseData(paragraphBlock.Inline!.FirstChild!);

        // Ellipsis...
        paragraphBlock = (ParagraphBlock)document[27];
        yield return leafBlockToTestCaseData(paragraphBlock);
        yield return containerInlineToTestCaseData(paragraphBlock.Inline!);
        yield return nonContainerTestCaseData(paragraphBlock.Inline!.FirstChild!);

        containerBlock = (ContainerBlock)document[28];
        yield return containerBlockToTestCaseData(containerBlock);
        innerContainerBlock = (ContainerBlock)containerBlock[0];
        yield return containerBlockToTestCaseData(innerContainerBlock);
        paragraphBlock = (ParagraphBlock)innerContainerBlock[0];
        yield return leafBlockToTestCaseData(paragraphBlock);
        yield return containerInlineToTestCaseData(paragraphBlock.Inline!);
        inline = paragraphBlock.Inline!.FirstChild!;
        yield return nonContainerTestCaseData(inline);
        yield return nonContainerTestCaseData(inline.NextSibling!);
        innerContainerBlock = (ContainerBlock)containerBlock[1];
        yield return containerBlockToTestCaseData(innerContainerBlock);
        paragraphBlock = (ParagraphBlock)innerContainerBlock[0];
        yield return leafBlockToTestCaseData(paragraphBlock);
        yield return containerInlineToTestCaseData(paragraphBlock.Inline!);
        inline = paragraphBlock.Inline!.FirstChild!;
        yield return nonContainerTestCaseData(inline);
        yield return nonContainerTestCaseData(inline.NextSibling!);
    }

    public static System.Collections.IEnumerable GetGetAllDescendantsTestData()
    {
        TestCaseData getTestCaseData(MarkdownObject src, params MarkdownObject[] objs)
        {
            return new TestCaseData(src).Returns(objs.Select(obj => new Tuple<Type, SourceSpan>(obj.GetType(), obj.Span)).ToArray())
                .SetArgDisplayNames($"{src.GetType().Name}: Line {src.Line} Column: {src.Column}");
        }
        TestCaseData getTestCaseData2(MarkdownObject src, IEnumerable<MarkdownObject> objs)
        {
            return new TestCaseData(src).Returns(objs.Select(obj => new Tuple<Type, SourceSpan>(obj.GetType(), obj.Span)).ToArray())
                .SetArgDisplayNames($"{src.GetType().Name}: Line {src.Line} Column: {src.Column}");
        }
        MarkdownDocument document = GetExampleMarkdownDocument();

        // # Example Markdown Document
        var headingBlock = (HeadingBlock)document[0];
        var containerInline = headingBlock.Inline!;
        Markdig.Syntax.Inlines.Inline inline0 = containerInline.FirstChild!;
        yield return getTestCaseData(inline0);
        yield return getTestCaseData(containerInline, inline0);
        var attr0 = headingBlock.TryGetAttributes()!;
        yield return getTestCaseData(headingBlock, attr0, containerInline, inline0);
        IEnumerable<MarkdownObject> documentDescendants = [headingBlock, attr0, containerInline, inline0];

        // [CommonMark Spec](https://spec.commonmark.org/0.31.2/)
        ParagraphBlock paragraphBlock = (ParagraphBlock)document[1];
        containerInline = paragraphBlock.Inline!;
        Markdig.Syntax.Inlines.ContainerInline innerContainerInline0 = (Markdig.Syntax.Inlines.ContainerInline)containerInline.FirstChild!;
        inline0 = innerContainerInline0.FirstChild!;
        yield return getTestCaseData(inline0);
        yield return getTestCaseData(innerContainerInline0, inline0);
        yield return getTestCaseData(containerInline, innerContainerInline0, inline0);
        yield return getTestCaseData(paragraphBlock, containerInline, innerContainerInline0, inline0);
        documentDescendants = documentDescendants.Concat([paragraphBlock, containerInline, innerContainerInline0, inline0]);
        
        // Hard line break\
        // here
        paragraphBlock = (ParagraphBlock)document[2];
        // yield return leafBlockToTestCaseData(paragraphBlock);
        containerInline = paragraphBlock.Inline!;
        // yield return containerInlineToTestCaseData(containerInline);
        inline0 = containerInline.FirstChild!;
        yield return getTestCaseData(inline0);
        Markdig.Syntax.Inlines.Inline inline1 = inline0.NextSibling!;
        yield return getTestCaseData(inline1);
        Markdig.Syntax.Inlines.Inline inline2 = containerInline.LastChild!;
        yield return getTestCaseData(inline2);
        yield return getTestCaseData(containerInline, inline0, inline1, inline2);
        yield return getTestCaseData(paragraphBlock, containerInline, inline0, inline1, inline2);
        documentDescendants = documentDescendants.Concat([paragraphBlock, containerInline, inline0, inline1, inline2]);

        ContainerBlock containerBlock = (ContainerBlock)document[3];
        // yield return containerBlockToTestCaseData(containerBlock); // Has attribute

        // - [X] Task
        ContainerBlock innerContainerBlock0 = (ContainerBlock)containerBlock[0];
        // yield return containerBlockToTestCaseData(innerContainerBlock); // Has attribute
        paragraphBlock = (ParagraphBlock)innerContainerBlock0[0];
        // yield return leafBlockToTestCaseData(paragraphBlock);
        containerInline = paragraphBlock.Inline!;
        inline0 = containerInline.FirstChild!;
        yield return getTestCaseData(inline0);
        inline1 = containerInline.LastChild!;
        yield return getTestCaseData(inline1);
        yield return getTestCaseData(containerInline, inline0, inline1);
        yield return getTestCaseData(paragraphBlock, containerInline, inline0, inline1);
        attr0 = innerContainerBlock0.TryGetAttributes()!;
        yield return getTestCaseData(innerContainerBlock0, attr0, paragraphBlock, containerInline, inline0, inline1);
        IEnumerable<MarkdownObject> containerDescendants = [containerBlock.TryGetAttributes()!, innerContainerBlock0, attr0, paragraphBlock, containerInline, inline0, inline1];

        // - [ ] List Item
        innerContainerBlock0 = (ContainerBlock)containerBlock[1];
        paragraphBlock = (ParagraphBlock)innerContainerBlock0[0];
        containerInline = paragraphBlock.Inline!;
        containerInline = paragraphBlock.Inline!;
        inline0 = containerInline.FirstChild!;
        yield return getTestCaseData(inline0);
        inline1 = containerInline.LastChild!;
        yield return getTestCaseData(inline1);
        yield return getTestCaseData(containerInline, inline0, inline1);
        yield return getTestCaseData(paragraphBlock, containerInline, inline0, inline1);
        attr0 = innerContainerBlock0.TryGetAttributes()!;
        yield return getTestCaseData(innerContainerBlock0, attr0, paragraphBlock, containerInline, inline0, inline1);
        containerDescendants = containerDescendants.Concat([innerContainerBlock0, attr0, paragraphBlock, containerInline, inline0, inline1]);

        // - Normal List
        innerContainerBlock0 = (ContainerBlock)containerBlock[2];
        paragraphBlock = (ParagraphBlock)innerContainerBlock0[0];
        containerInline = paragraphBlock.Inline!;
        inline0 = containerInline.FirstChild!;
        yield return getTestCaseData(inline0);
        yield return getTestCaseData(containerInline, inline0);
        yield return getTestCaseData(paragraphBlock, containerInline, inline0);
        yield return getTestCaseData(innerContainerBlock0, paragraphBlock, containerInline, inline0);
        containerDescendants = containerDescendants.Concat([innerContainerBlock0, paragraphBlock, containerInline, inline0]);

        // - Item
        innerContainerBlock0 = (ContainerBlock)containerBlock[3];
        paragraphBlock = (ParagraphBlock)innerContainerBlock0[0];
        containerInline = paragraphBlock.Inline!;
        inline0 = containerInline.FirstChild!;
        yield return getTestCaseData(inline0);
        yield return getTestCaseData(containerInline, inline0);
        yield return getTestCaseData(paragraphBlock, containerInline, inline0);
        yield return getTestCaseData(innerContainerBlock0, paragraphBlock, containerInline, inline0);
        containerDescendants = containerDescendants.Concat([innerContainerBlock0, paragraphBlock, containerInline, inline0]);
        yield return getTestCaseData2(containerBlock, containerDescendants);
        documentDescendants = documentDescendants.Concat([containerBlock]).Concat(containerDescendants);

        // ## Abbreviations {#custom-id}
        headingBlock = (HeadingBlock)document[4];
        containerInline = headingBlock.Inline!;
        inline0 = containerInline.FirstChild!;
        yield return getTestCaseData(inline0);
        yield return getTestCaseData(containerInline, inline0);
        attr0 = headingBlock.TryGetAttributes()!;
        yield return getTestCaseData(headingBlock, attr0, containerInline, inline0);
        documentDescendants = documentDescendants.Concat([headingBlock, attr0, containerInline, inline0]);

        // ## Link
        headingBlock = (HeadingBlock)document[5];
        containerInline = headingBlock.Inline!;
        inline0 = containerInline.FirstChild!;
        yield return getTestCaseData(inline0);
        yield return getTestCaseData(containerInline, inline0);
        attr0 = headingBlock.TryGetAttributes()!;
        yield return getTestCaseData(headingBlock, attr0, containerInline, inline0);
        documentDescendants = documentDescendants.Concat([headingBlock, attr0, containerInline, inline0]);

        // [Abbreviations Link](#custom-id)
        paragraphBlock = (ParagraphBlock)document[6];
        containerInline = paragraphBlock.Inline!;
        innerContainerInline0 = (Markdig.Syntax.Inlines.ContainerInline)containerInline.FirstChild!;
        inline0 = innerContainerInline0.FirstChild!;
        yield return getTestCaseData(inline0);
        yield return getTestCaseData(innerContainerInline0, inline0);
        yield return getTestCaseData(containerInline, innerContainerInline0, inline0);
        yield return getTestCaseData(paragraphBlock, containerInline, innerContainerInline0, inline0);
        documentDescendants = documentDescendants.Concat([paragraphBlock, containerInline, innerContainerInline0, inline0]);

        // ## Image with Alt and Title
        headingBlock = (HeadingBlock)document[7];
        containerInline = headingBlock.Inline!;
        inline0 = containerInline.FirstChild!;
        yield return getTestCaseData(inline0);
        yield return getTestCaseData(containerInline, inline0);
        attr0 = headingBlock.TryGetAttributes()!;
        yield return getTestCaseData(headingBlock, attr0, containerInline, inline0);
        documentDescendants = documentDescendants.Concat([headingBlock, attr0, containerInline, inline0]);

        // ![alt attribute goes here](./sn-logo.jpg "This is a Title" )
        paragraphBlock = (ParagraphBlock)document[8];
        containerInline = paragraphBlock.Inline!;
        innerContainerInline0 = (Markdig.Syntax.Inlines.ContainerInline)containerInline.FirstChild!;
        inline0 = innerContainerInline0.FirstChild!;
        yield return getTestCaseData(inline0);
        yield return getTestCaseData(innerContainerInline0, inline0);
        yield return getTestCaseData(containerInline, innerContainerInline0, inline0);
        yield return getTestCaseData(paragraphBlock, containerInline, innerContainerInline0, inline0);
        documentDescendants = documentDescendants.Concat([paragraphBlock, containerInline, innerContainerInline0, inline0]);

        // ![foo *bar*]
        // [foo *bar*]: ./sn-logo.jpg "train & tracks"
        paragraphBlock = (ParagraphBlock)document[9];
        containerInline = paragraphBlock.Inline!;
        innerContainerInline0 = (Markdig.Syntax.Inlines.ContainerInline)containerInline.FirstChild!;
        inline0 = innerContainerInline0.FirstChild!;
        yield return getTestCaseData(inline0);
        var innerContainerInline1 = (Markdig.Syntax.Inlines.ContainerInline)inline0.NextSibling!;
        inline1 = innerContainerInline1.FirstChild!;
        yield return getTestCaseData(inline1);
        yield return getTestCaseData(innerContainerInline1, inline1);
        yield return getTestCaseData(innerContainerInline0, inline0, innerContainerInline1, inline1);
        yield return getTestCaseData(containerInline, innerContainerInline0, inline0, innerContainerInline1, inline1);
        yield return getTestCaseData(paragraphBlock, containerInline, innerContainerInline0, inline0, innerContainerInline1, inline1);
        documentDescendants = documentDescendants.Concat([paragraphBlock, containerInline, innerContainerInline0, inline0, innerContainerInline1, inline1]);

        containerBlock = (ContainerBlock)document[10];
        for (var i = 0; i < containerBlock.Count; i++)
            yield return getTestCaseData(containerBlock[i]);
        getTestCaseData2(containerBlock, containerBlock);

        // ## Math
        headingBlock = (HeadingBlock)document[11];
        containerInline = headingBlock.Inline!;
        inline0 = containerInline.FirstChild!;
        yield return getTestCaseData(inline0);
        yield return getTestCaseData(containerInline, inline0);
        attr0 = headingBlock.TryGetAttributes()!;
        yield return getTestCaseData(headingBlock, attr0, containerInline, inline0);
        documentDescendants = documentDescendants.Concat([headingBlock, attr0, containerInline, inline0]);

        // This sentence uses `$` delimiters to show math inline: $\sqrt{3x-1}+(1+x)^2$
        paragraphBlock = (ParagraphBlock)document[12];
        containerInline = paragraphBlock.Inline!;
        inline0 = containerInline.FirstChild!;
        yield return getTestCaseData(inline0);
        inline1 = inline0.NextSibling!;
        yield return getTestCaseData(inline1);
        inline2 = inline1.NextSibling!;
        yield return getTestCaseData(inline2);
        var inline3 = inline2.NextSibling!;
        attr0 = inline3.TryGetAttributes()!;
        yield return getTestCaseData(inline3, attr0);
        yield return getTestCaseData(containerInline, inline0, inline1, inline2, inline3, attr0);
        yield return getTestCaseData(paragraphBlock, containerInline, inline0, inline1, inline2, inline3, attr0);
        documentDescendants = documentDescendants.Concat([paragraphBlock, containerInline, inline0, inline1, inline2, inline3, attr0]);

        // This sentence uses $\` and \`$ delimiters to show math inline: $`\sqrt{3x-1}+(1+x)^2`$
        paragraphBlock = (ParagraphBlock)document[13];
        containerInline = paragraphBlock.Inline!;
        inline0 = containerInline.FirstChild!;
        yield return getTestCaseData(inline0);
        inline1 = inline0.NextSibling!;
        attr0 = inline1.TryGetAttributes()!;
        yield return getTestCaseData(inline1, attr0);
        inline2 = inline1.NextSibling!;
        yield return getTestCaseData(inline2);
        inline3 = inline2.NextSibling!;
        var attr1 = inline3.TryGetAttributes()!;
        yield return getTestCaseData(inline3, attr1);
        yield return getTestCaseData(containerInline, inline0, inline1, attr0, inline2, inline3, attr1);
        yield return getTestCaseData(paragraphBlock, containerInline, inline0, inline1, attr0, inline2, inline3, attr1);
        documentDescendants = documentDescendants.Concat([paragraphBlock, containerInline, inline0, inline1, attr0, inline2, inline3, attr1]);
        
        // **The Cauchy-Schwarz Inequality**
        // $$\left( \sum_{k=1}^n a_k b_k \right)^2 \leq \left( \sum_{k=1}^n a_k^2 \right) \left( \sum_{k=1}^n b_k^2 \right)$$
        paragraphBlock = (ParagraphBlock)document[14];
        containerInline = paragraphBlock.Inline!;
        innerContainerInline0 = (Markdig.Syntax.Inlines.ContainerInline)containerInline.FirstChild!;
        inline0 = innerContainerInline0.FirstChild!;
        yield return getTestCaseData(inline0);
        yield return getTestCaseData(innerContainerInline0, inline0);
        inline1 = innerContainerInline0.NextSibling!;
        yield return getTestCaseData(inline1);
        inline2 = inline1.NextSibling!;
        attr0 = inline2.TryGetAttributes()!;
        yield return getTestCaseData(inline2, attr0);
        yield return getTestCaseData(containerInline, innerContainerInline0, inline0, inline1, inline2, attr0);
        yield return getTestCaseData(paragraphBlock, containerInline, innerContainerInline0, inline0, inline1, inline2, attr0);
        documentDescendants = documentDescendants.Concat([paragraphBlock, containerInline, innerContainerInline0, inline0, inline1, inline2, attr0]);

        // ## Definition List
        headingBlock = (HeadingBlock)document[15];
        containerInline = headingBlock.Inline!;
        inline0 = containerInline.FirstChild!;
        yield return getTestCaseData(inline0);
        yield return getTestCaseData(containerInline, inline0);
        attr0 = headingBlock.TryGetAttributes()!;
        yield return getTestCaseData(headingBlock, attr0, containerInline, inline0);
        documentDescendants = documentDescendants.Concat([headingBlock, attr0, containerInline, inline0]);

        containerBlock = (ContainerBlock)document[16];
        innerContainerBlock0 = (ContainerBlock)containerBlock[0];
        // Apple
        LeafBlock leafBlock = (LeafBlock)innerContainerBlock0[0];
        innerContainerInline0 = leafBlock.Inline!;
        inline0 = innerContainerInline0.FirstChild!;
        yield return getTestCaseData(inline0);
        yield return getTestCaseData(innerContainerInline0, inline0);
        yield return getTestCaseData(leafBlock, innerContainerInline0, inline0);
        containerDescendants = [leafBlock, innerContainerInline0, inline0];
        // :   Pomaceous fruit of plants of the genus Malus in
        //     the family Rosaceae.
        leafBlock = (LeafBlock)innerContainerBlock0[1];
        innerContainerInline0 = leafBlock.Inline!;
        inline0 = innerContainerInline0.FirstChild!;
        yield return getTestCaseData(inline0);
        inline1 = inline0.NextSibling!;
        yield return getTestCaseData(inline1);
        inline2 = inline1.NextSibling!;
        yield return getTestCaseData(inline2);
        yield return getTestCaseData(innerContainerInline0, inline0, inline1, inline2);
        yield return getTestCaseData(leafBlock, innerContainerInline0, inline0, inline1, inline2);
        containerDescendants = containerDescendants.Concat([leafBlock, innerContainerInline0, inline0, inline1, inline2]);
        yield return getTestCaseData2(innerContainerBlock0, containerDescendants);
        containerDescendants = ((IEnumerable<MarkdownObject>)[innerContainerBlock0]).Concat(containerDescendants);

        innerContainerBlock0 = (ContainerBlock)containerBlock[1];
        // Orange
        leafBlock = (LeafBlock)innerContainerBlock0[0];
        innerContainerInline0 = leafBlock.Inline!;
        inline0 = innerContainerInline0.FirstChild!;
        yield return getTestCaseData(inline0);
        yield return getTestCaseData(innerContainerInline0, inline0);
        yield return getTestCaseData(leafBlock, innerContainerInline0, inline0);
        IEnumerable<MarkdownObject> innerDescendants = [leafBlock, innerContainerInline0, inline0];
        // :   The fruit of an evergreen tree of the genus Citrus.
        leafBlock = (LeafBlock)innerContainerBlock0[1];
        innerContainerInline0 = leafBlock.Inline!;
        inline0 = innerContainerInline0.FirstChild!;
        yield return getTestCaseData(inline0);
        yield return getTestCaseData(innerContainerInline0, inline0);
        yield return getTestCaseData(leafBlock, innerContainerInline0, inline0);
        innerDescendants = innerDescendants.Concat([leafBlock, innerContainerInline0, inline0]);
        yield return getTestCaseData2(innerContainerBlock0, innerDescendants);
        containerDescendants = containerDescendants.Concat([innerContainerBlock0]).Concat(innerDescendants);
        yield return getTestCaseData2(containerBlock, containerDescendants);
        documentDescendants = documentDescendants.Concat([containerBlock]).Concat(containerDescendants);

        // ## Code
        headingBlock = (HeadingBlock)document[17];
        containerInline = headingBlock.Inline!;
        inline0 = containerInline.FirstChild!;
        yield return getTestCaseData(inline0);
        yield return getTestCaseData(containerInline, inline0);
        attr0 = headingBlock.TryGetAttributes()!;
        yield return getTestCaseData(headingBlock, attr0, containerInline, inline0);
        documentDescendants = documentDescendants.Concat([headingBlock, attr0, containerInline, inline0]);

        // This is `inline code`.
        paragraphBlock = (ParagraphBlock)document[18];
        containerInline = paragraphBlock.Inline!;
        inline0 = containerInline.FirstChild!;
        yield return getTestCaseData(inline0);
        inline1 = inline0.NextSibling!;
        yield return getTestCaseData(inline1);
        inline2 = inline1.NextSibling!;
        yield return getTestCaseData(inline2);
        yield return getTestCaseData(containerInline, inline0, inline1, inline2);
        yield return getTestCaseData(paragraphBlock, containerInline, inline0, inline1, inline2);
        documentDescendants = documentDescendants.Concat([paragraphBlock, containerInline, inline0, inline1, inline2]);

        // ```log
        // This is a fenced code block
        // ```
        leafBlock = (LeafBlock)document[19];
        attr0 = leafBlock.TryGetAttributes()!;
        yield return getTestCaseData(leafBlock, attr0);
        documentDescendants = documentDescendants.Concat([leafBlock, attr0]);

        // ``` { .html #codeId style="color: #333; background: #f8f8f8;" }
        // This is a fenced code block
        // with an ID and style
        // ```
        leafBlock = (LeafBlock)document[20];
        attr0 = leafBlock.TryGetAttributes()!;
        yield return getTestCaseData(leafBlock, attr0);
        documentDescendants = documentDescendants.Concat([leafBlock, attr0]);

        // ## Attribute lists
        headingBlock = (HeadingBlock)document[21];
        containerInline = headingBlock.Inline!;
        inline0 = containerInline.FirstChild!;
        yield return getTestCaseData(inline0);
        yield return getTestCaseData(containerInline, inline0);
        attr0 = headingBlock.TryGetAttributes()!;
        yield return getTestCaseData(headingBlock, attr0, containerInline, inline0);
        documentDescendants = documentDescendants.Concat([headingBlock, attr0, containerInline, inline0]);

        // This is red paragraph.
        // {: style="color: #333; color: #ff0000;" }
        paragraphBlock = (ParagraphBlock)document[22];
        // yield return leafBlockToTestCaseData(paragraphBlock);
        containerInline = paragraphBlock.Inline!;
        // yield return containerInlineToTestCaseData(paragraphBlock.Inline!);
        inline0 = containerInline.FirstChild!;
        yield return getTestCaseData(inline0);
        // inline = paragraphBlock.Inline!.FirstChild!;
        // yield return nonContainerTestCaseData(inline);
        inline1 = inline0.NextSibling!;
        attr0 = inline1.TryGetAttributes()!;
        yield return getTestCaseData(inline1, attr0);
        // yield return nonContainerTestCaseData(inline.NextSibling!); // Has attribute
        yield return getTestCaseData(containerInline, inline0, inline1, attr0);
        yield return getTestCaseData(paragraphBlock, containerInline, inline0, inline1, attr0);
        documentDescendants = documentDescendants.Concat([paragraphBlock, containerInline, inline0, inline1, attr0]);

        // ## Footnotes
        headingBlock = (HeadingBlock)document[23];
        containerInline = headingBlock.Inline!;
        inline0 = containerInline.FirstChild!;
        yield return getTestCaseData(inline0);
        yield return getTestCaseData(containerInline, inline0);
        attr0 = headingBlock.TryGetAttributes()!;
        yield return getTestCaseData(headingBlock, attr0, containerInline, inline0);
        documentDescendants = documentDescendants.Concat([headingBlock, attr0, containerInline, inline0]);

        paragraphBlock = (ParagraphBlock)document[24];
        containerInline = paragraphBlock.Inline!;
        inline0 = containerInline.FirstChild!;
        yield return getTestCaseData(inline0);
        inline1 = inline0.NextSibling!;
        yield return getTestCaseData(inline1);
        inline2 = inline1.NextSibling!;
        yield return getTestCaseData(inline2);
        inline3 = inline2.NextSibling!;
        yield return getTestCaseData(inline3);
        var inline4 = inline3.NextSibling!;
        yield return getTestCaseData(inline4);
        yield return getTestCaseData(containerInline, inline0, inline1, inline2, inline3, inline4);
        yield return getTestCaseData(paragraphBlock, containerInline, inline0, inline1, inline2, inline3, inline4);
        documentDescendants = documentDescendants.Concat([paragraphBlock, containerInline, inline0, inline1, inline2, inline3, inline4]);

        // ### Smarties
        headingBlock = (HeadingBlock)document[25];
        containerInline = headingBlock.Inline!;
        inline0 = containerInline.FirstChild!;
        yield return getTestCaseData(inline0);
        yield return getTestCaseData(containerInline, inline0);
        attr0 = headingBlock.TryGetAttributes()!;
        yield return getTestCaseData(headingBlock, attr0, containerInline, inline0);
        documentDescendants = documentDescendants.Concat([headingBlock, attr0, containerInline, inline0]);

        // << angle quotes >>
        paragraphBlock = (ParagraphBlock)document[26];
        containerInline = paragraphBlock.Inline!;
        inline0 = containerInline.FirstChild!;
        yield return getTestCaseData(inline0);
        yield return getTestCaseData(containerInline, inline0);
        yield return getTestCaseData(paragraphBlock, containerInline, inline0);
        documentDescendants = documentDescendants.Concat([paragraphBlock, containerInline, inline0]);

        // Ellipsis...
        paragraphBlock = (ParagraphBlock)document[27];
        containerInline = paragraphBlock.Inline!;
        inline0 = containerInline.FirstChild!;
        yield return getTestCaseData(inline0);
        yield return getTestCaseData(containerInline, inline0);
        yield return getTestCaseData(paragraphBlock, containerInline, inline0);
        documentDescendants = documentDescendants.Concat([paragraphBlock, containerInline, inline0]);

        containerBlock = (ContainerBlock)document[28];
        innerContainerBlock0 = (ContainerBlock)containerBlock[0];
        paragraphBlock = (ParagraphBlock)innerContainerBlock0[0];
        containerInline = paragraphBlock.Inline!;
        inline0 = containerInline.FirstChild!;
        yield return getTestCaseData(inline0);
        inline1 = inline0.NextSibling!;
        yield return getTestCaseData(inline1);
        yield return getTestCaseData(containerInline, inline0, inline1);
        yield return getTestCaseData(paragraphBlock, containerInline, inline0, inline1);
        yield return getTestCaseData(innerContainerBlock0, paragraphBlock, containerInline, inline0, inline1);
        containerDescendants = [innerContainerBlock0, paragraphBlock, containerInline, inline0, inline1];
        innerContainerBlock0 = (ContainerBlock)containerBlock[1];
        paragraphBlock = (ParagraphBlock)innerContainerBlock0[0];
        containerInline = paragraphBlock.Inline!;
        inline0 = containerInline.FirstChild!;
        yield return getTestCaseData(inline0);
        inline1 = inline0.NextSibling!;
        yield return getTestCaseData(inline1);
        yield return getTestCaseData(containerInline, inline0, inline1);
        yield return getTestCaseData(paragraphBlock, containerInline, inline0, inline1);
        yield return getTestCaseData(innerContainerBlock0, paragraphBlock, containerInline, inline0, inline1);
        containerDescendants = containerDescendants.Concat([innerContainerBlock0, paragraphBlock, containerInline, inline0, inline1]);
        getTestCaseData2(containerBlock, containerDescendants);
        documentDescendants = documentDescendants.Concat([containerBlock]).Concat(containerDescendants);
        getTestCaseData2(document, documentDescendants);

        JsonObject createJsonObject(MarkdownObject obj)
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
        using FileStream stream = File.OpenWrite(@"C:\Users\Lenny\Source\Repositories\PowerShell-Modules\src\HtmlUtility.UnitTests\TestData\ExampleMd.json");
        using Utf8JsonWriter writer = new Utf8JsonWriter(stream);
        rootObject.WriteTo(writer);
        writer.Flush();
        stream.Flush();
    }

    public static System.Collections.IEnumerable GetGetDescendantBranchesMatchingType1TestData()
    {
        TestCaseData createTestCaseData(MarkdownObject source, Type type, params MarkdownObject[] obj)
        {
            return new TestCaseData(source, type)
                .Returns(obj.Select(o => new Tuple<Type, SourceSpan, int, int>(o.GetType(), o.Span, o.Line, o.Column)).ToArray())
                .SetArgDisplayNames($"{source.GetType().Name} {{ {source.ToPositionText() }}}", type.FullName!);
        }

        MarkdownDocument document = GetExampleMarkdownDocument();
        yield return createTestCaseData(document, typeof(MarkdownDocument), document);
        yield return createTestCaseData(document, typeof(HeadingBlock), document.OfType<HeadingBlock>().ToArray());
        IEnumerable<MarkdownObject> expected = document.Take(3);
        expected = expected.Concat(((ListBlock)document[3]).Cast<ListItemBlock>().Select(lib => lib[0]));
        expected = expected.Concat(document.Skip(3).Take(6));
        expected = expected.Concat((ContainerBlock)document[10]);
        expected = expected.Concat(document.Skip(10).Take(5));
        expected = expected.Concat(((Markdig.Extensions.DefinitionLists.DefinitionList)document[16]).SelectMany(b => ((Markdig.Extensions.DefinitionLists.DefinitionItem)b).AsEnumerable()));
        expected = expected.Concat(document.Skip(16).Take(11));
        expected = expected.Concat(((Markdig.Extensions.Footnotes.FootnoteGroup)document[28]).Cast<Markdig.Extensions.Footnotes.Footnote>().Select(f => f[0]));
        yield return createTestCaseData(document, typeof(LeafBlock), expected.ToArray());

    }

    public static System.Collections.IEnumerable GetGetDescendantBranchesMatchingType2TestData()
    {
        TestCaseData createTestCaseData2(MarkdownObject source, Type type1, Type type2, params MarkdownObject[] obj)
        {
            return new TestCaseData(source, type1, type2)
                .Returns(obj.Select(o => new Tuple<Type, SourceSpan, int, int>(o.GetType(), o.Span, o.Line, o.Column)).ToArray())
                .SetArgDisplayNames($"{source.GetType().Name} {{ {source.ToPositionText()}}}", type1.FullName!, type2.FullName!);
        }

        MarkdownDocument document = GetExampleMarkdownDocument();
        var expected = ((IEnumerable<MarkdownObject>)[document]).Concat(((LinkReferenceDefinitionGroup)document[10]).Cast<MarkdownObject>());
        yield return createTestCaseData2(document, typeof(LinkReferenceDefinition), typeof(MarkdownDocument), expected.ToArray());
    }

    // public static System.Collections.IEnumerable GetData()
    // {

    // }

    // public static System.Collections.IEnumerable GetData()
    // {

    // }
}