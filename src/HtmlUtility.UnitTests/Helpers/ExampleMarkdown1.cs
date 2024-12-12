using Markdig;
using Markdig.Renderers.Html;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace HtmlUtility.UnitTests.Helpers;

public static class ExampleMarkdown1
{
    internal const string SourceFileName = "Example1.md";
    internal const string JsonTestOutputFileName = "Example1.json";

    internal static string GetSourcePath() => Path.Combine(TestHelper.GetResourcesDirectoryPath(), SourceFileName);

    internal static string GetMarkdownSourceText() => File.ReadAllText(GetSourcePath());

    internal static MarkdownDocument GetMarkdownDocument() => Markdown.Parse(GetMarkdownSourceText(), new MarkdownPipelineBuilder().UseAdvancedExtensions().Build());

    /// <summary>
    /// Test cases for <see cref="MarkdownExtensionMethods.GetChildObjects(MarkdownObject?, bool)"/>.
    /// </summary>
    /// <returns></returns>
    public static System.Collections.IEnumerable GetGetChildObjectsTestData(bool includeAttributes)
    {
        MarkdownDocument document = GetMarkdownDocument();
        // TestHelper.AddMarkdownJsonTestAttachment(document, SourceFileName, JsonTestOutputFileName);

        static Tuple<Type, SourceSpan> toReturnsTuple(MarkdownObject obj)
        {
            return new Tuple<Type, SourceSpan>(obj.GetType(), obj.Span);
        }

        static TestCaseData containerBlockToTestCaseData(ContainerBlock cb, bool? inclAttr)
        {
            if (inclAttr.HasValue && inclAttr.Value)
            {
                var attr = cb.TryGetAttributes();
                if (attr is not null)
                    return new TestCaseData(cb, inclAttr).Returns(new MarkdownObject[] { attr }.Concat(cb).Select(toReturnsTuple).ToArray())
                        .SetArgDisplayNames($"{cb.GetType().Name}: Line {cb.Line}");
            }
            return new TestCaseData(cb, inclAttr).Returns(cb.Select(toReturnsTuple).ToArray())
                .SetArgDisplayNames($"{cb.GetType().Name}: Line {cb.Line}");
        }

        // static TestCaseData containerInlineToTestCaseData(ContainerInline ci)
        // {
        //     var attr = ci.TryGetAttributes();
        //     if (attr is null)
        //         return new TestCaseData(ci).Returns(ci.Select(toReturnsTuple).ToArray())
        //             .SetArgDisplayNames($"{ci.GetType().Name}: Line {ci.Line}, Column {ci.Column}");
        //     return new TestCaseData(ci).Returns(new MarkdownObject[] { attr }.Concat(ci).Select(toReturnsTuple).ToArray())
        //         .SetArgDisplayNames($"{ci.GetType().Name}: Line {ci.Line}, Column {ci.Column}");
        // }

        static TestCaseData leafBlockToTestCaseData(LeafBlock lb, bool? inclAttr)
        {
            var leaf = lb.Inline;
            if (inclAttr.HasValue && inclAttr.Value)
            {
                var attr = lb.TryGetAttributes();
                if (attr is not null)
                {
                    if (leaf is null)
                        return new TestCaseData(lb, inclAttr).Returns(new Tuple<Type, SourceSpan>[] { toReturnsTuple(attr) })
                            .SetArgDisplayNames($"{lb.GetType().Name}: Line {lb.Line}");

                    return new TestCaseData(lb, inclAttr).Returns(new MarkdownObject[] { attr }.Concat(leaf).Select(toReturnsTuple).ToArray())
                        .SetArgDisplayNames($"{lb.GetType().Name}: Line {lb.Line}");
                }
            }
            if (leaf is null)
                return new TestCaseData(lb, inclAttr).Returns(Array.Empty<Tuple<Type, SourceSpan>>())
                    .SetArgDisplayNames($"{lb.GetType().Name}: Line {lb.Line}");
            return new TestCaseData(lb, inclAttr).Returns(leaf.Select(toReturnsTuple).ToArray())
                .SetArgDisplayNames($"{lb.GetType().Name}: Line {lb.Line}");
        }

        static TestCaseData nonContainerTestCaseData(MarkdownObject lb, bool? inclAttr)
        {
            if (inclAttr.HasValue && inclAttr.Value)
            {
                var attr = lb.TryGetAttributes();
                if (attr is not null)
                {
                    if (lb is Inline)
                        return new TestCaseData(lb, inclAttr).Returns(new Tuple<Type, SourceSpan>[] { toReturnsTuple(attr) })
                            .SetArgDisplayNames($"{lb.GetType().Name}: Line {lb.Line}, Column: {lb.Column}");

                    return new TestCaseData(lb, inclAttr).Returns(new Tuple<Type, SourceSpan>[] { toReturnsTuple(attr) })
                        .SetArgDisplayNames($"{lb.GetType().Name}: Line {lb.Line}");
                }
            }
            if (lb is Inline)
                return new TestCaseData(lb, inclAttr).Returns(Array.Empty<Tuple<Type, SourceSpan>>())
                    .SetArgDisplayNames($"{lb.GetType().Name}: Line {lb.Line}, Column: {lb.Column}");
            return new TestCaseData(lb, inclAttr).Returns(Array.Empty<Tuple<Type, SourceSpan>>())
                .SetArgDisplayNames($"{lb.GetType().Name}: Line {lb.Line}");
        }


        if (includeAttributes)
            yield return containerBlockToTestCaseData(document, true);
        else
        {
            yield return containerBlockToTestCaseData(document, null);
            yield return containerBlockToTestCaseData(document, false);
        }

        // # Example Markdown Document
        HeadingBlock headingBlock = (HeadingBlock)document[0];
        yield return leafBlockToTestCaseData(headingBlock, includeAttributes); // Has attribute
        yield return nonContainerTestCaseData(headingBlock.Inline!.FirstChild!, includeAttributes);

        // [CommonMark Spec](https://spec.commonmark.org/0.31.2/)
        ParagraphBlock paragraphBlock = (ParagraphBlock)document[1];
        yield return leafBlockToTestCaseData(paragraphBlock, includeAttributes);
        ContainerInline containerInline = (ContainerInline)paragraphBlock.Inline!.FirstChild!;
        yield return nonContainerTestCaseData(containerInline.FirstChild!, includeAttributes);
        // Hard line break\
        // here
        paragraphBlock = (ParagraphBlock)document[2];
        yield return leafBlockToTestCaseData(paragraphBlock, includeAttributes);
        containerInline = paragraphBlock.Inline!;
        Inline inline = containerInline.FirstChild!;
        yield return nonContainerTestCaseData(inline, includeAttributes);
        yield return nonContainerTestCaseData(inline.NextSibling!, includeAttributes);
        yield return nonContainerTestCaseData(containerInline.LastChild!, includeAttributes);

        ContainerBlock containerBlock = (ContainerBlock)document[3];
        yield return containerBlockToTestCaseData(containerBlock, includeAttributes); // Has attribute

        // - [X] Task
        ContainerBlock innerContainerBlock = (ContainerBlock)containerBlock[0];
        yield return containerBlockToTestCaseData(innerContainerBlock, includeAttributes); // Has attribute
        paragraphBlock = (ParagraphBlock)innerContainerBlock[0];
        yield return leafBlockToTestCaseData(paragraphBlock, includeAttributes);
        containerInline = paragraphBlock.Inline!;
        yield return nonContainerTestCaseData(containerInline.FirstChild!, includeAttributes);
        yield return nonContainerTestCaseData(containerInline.LastChild!, includeAttributes);

        // - [ ] List Item
        innerContainerBlock = (ContainerBlock)containerBlock[1];
        yield return containerBlockToTestCaseData(innerContainerBlock, includeAttributes); // Has attribute
        paragraphBlock = (ParagraphBlock)innerContainerBlock[0];
        yield return leafBlockToTestCaseData(paragraphBlock, includeAttributes);
        containerInline = paragraphBlock.Inline!;
        yield return nonContainerTestCaseData(containerInline.FirstChild!, includeAttributes);
        yield return nonContainerTestCaseData(containerInline.LastChild!, includeAttributes);

        // - Normal List
        innerContainerBlock = (ContainerBlock)containerBlock[2];
        yield return containerBlockToTestCaseData(innerContainerBlock, includeAttributes);
        paragraphBlock = (ParagraphBlock)innerContainerBlock[0];
        yield return leafBlockToTestCaseData(paragraphBlock, includeAttributes);
        containerInline = paragraphBlock.Inline!;
        yield return nonContainerTestCaseData(containerInline.FirstChild!, includeAttributes);

        // - Item
        innerContainerBlock = (ContainerBlock)containerBlock[3];
        yield return containerBlockToTestCaseData(innerContainerBlock, includeAttributes);
        paragraphBlock = (ParagraphBlock)innerContainerBlock[0];
        yield return leafBlockToTestCaseData(paragraphBlock, includeAttributes);
        containerInline = paragraphBlock.Inline!;
        yield return nonContainerTestCaseData(containerInline.FirstChild!, includeAttributes);

        // ## Abbreviations {#custom-id}

        // *[HTML]: Hyper Text Markup Language
        // *[W3C]:  World Wide Web Consortium

        headingBlock = (HeadingBlock)document[4];
        yield return leafBlockToTestCaseData(headingBlock, includeAttributes); // Has attribute
        yield return nonContainerTestCaseData(headingBlock.Inline!.FirstChild!, includeAttributes);

        // ## Link
        headingBlock = (HeadingBlock)document[5];
        yield return leafBlockToTestCaseData(headingBlock, includeAttributes); // Has attribute
        yield return nonContainerTestCaseData(headingBlock.Inline!.FirstChild!, includeAttributes);

        // [Abbreviations Link](#custom-id)
        paragraphBlock = (ParagraphBlock)document[6];
        yield return leafBlockToTestCaseData(paragraphBlock, includeAttributes);
        containerInline = (ContainerInline)paragraphBlock.Inline!.FirstChild!;
        yield return nonContainerTestCaseData(containerInline.FirstChild!, includeAttributes);

        // ## Image with Alt and Title
        headingBlock = (HeadingBlock)document[7];
        yield return leafBlockToTestCaseData(headingBlock, includeAttributes); // Has attribute
        yield return nonContainerTestCaseData(headingBlock.Inline!.FirstChild!, includeAttributes);

        // ![alt attribute goes here](./sn-logo.jpg "This is a Title" )
        paragraphBlock = (ParagraphBlock)document[8];
        yield return leafBlockToTestCaseData(paragraphBlock, includeAttributes);
        containerInline = (ContainerInline)paragraphBlock.Inline!.FirstChild!;
        yield return nonContainerTestCaseData(containerInline.FirstChild!, includeAttributes);

        // ![foo *bar*]
        // [foo *bar*]: ./sn-logo.jpg "train & tracks"
        paragraphBlock = (ParagraphBlock)document[9];
        yield return leafBlockToTestCaseData(paragraphBlock, includeAttributes);
        containerInline = (ContainerInline)paragraphBlock.Inline!.FirstChild!;
        inline = containerInline.FirstChild!;
        yield return nonContainerTestCaseData(inline, includeAttributes);
        containerInline = (ContainerInline)inline.NextSibling!;
        yield return nonContainerTestCaseData(containerInline.FirstChild!, includeAttributes);

        containerBlock = (ContainerBlock)document[10];
        yield return containerBlockToTestCaseData(containerBlock, includeAttributes);
        for (var i = 0; i < containerBlock.Count; i++)
            yield return leafBlockToTestCaseData((LeafBlock)containerBlock[0], includeAttributes);

        // ## Math
        headingBlock = (HeadingBlock)document[11];
        yield return leafBlockToTestCaseData(headingBlock, includeAttributes); // Has attribute
        yield return nonContainerTestCaseData(headingBlock.Inline!.FirstChild!, includeAttributes);

        // This sentence uses `$` delimiters to show math inline: $\sqrt{3x-1}+(1+x)^2$
        paragraphBlock = (ParagraphBlock)document[12];
        yield return leafBlockToTestCaseData(paragraphBlock, includeAttributes);
        inline = paragraphBlock.Inline!.FirstChild!;
        yield return nonContainerTestCaseData(inline, includeAttributes);
        inline = inline.NextSibling!;
        yield return nonContainerTestCaseData(inline, includeAttributes);
        inline = inline.NextSibling!;
        yield return nonContainerTestCaseData(inline, includeAttributes);
        yield return nonContainerTestCaseData(inline.NextSibling!, includeAttributes); // Has attribute

        // This sentence uses $\` and \`$ delimiters to show math inline: $`\sqrt{3x-1}+(1+x)^2`$
        paragraphBlock = (ParagraphBlock)document[13];
        yield return leafBlockToTestCaseData(paragraphBlock, includeAttributes);
        inline = paragraphBlock.Inline!.FirstChild!;
        yield return nonContainerTestCaseData(inline, includeAttributes);
        inline = inline.NextSibling!;
        yield return nonContainerTestCaseData(inline, includeAttributes); // Has attribute
        inline = inline.NextSibling!;
        yield return nonContainerTestCaseData(inline, includeAttributes);
        yield return nonContainerTestCaseData(inline.NextSibling!, includeAttributes); // Has attribute

        // **The Cauchy-Schwarz Inequality**
        // $$\left( \sum_{k=1}^n a_k b_k \right)^2 \leq \left( \sum_{k=1}^n a_k^2 \right) \left( \sum_{k=1}^n b_k^2 \right)$$
        paragraphBlock = (ParagraphBlock)document[14];
        yield return leafBlockToTestCaseData(paragraphBlock, includeAttributes);
        containerInline = paragraphBlock.Inline!;
        containerInline = (ContainerInline)containerInline.FirstChild!;
        yield return nonContainerTestCaseData(containerInline.FirstChild!, includeAttributes);
        inline = containerInline.NextSibling!;
        yield return nonContainerTestCaseData(inline, includeAttributes);
        yield return nonContainerTestCaseData(inline.NextSibling!, includeAttributes); // Has attribute

        // ## Definition List
        headingBlock = (HeadingBlock)document[15];
        yield return leafBlockToTestCaseData(headingBlock, includeAttributes); // Has attribute
        yield return nonContainerTestCaseData(headingBlock.Inline!.FirstChild!, includeAttributes);

        containerBlock = (ContainerBlock)document[16];
        yield return containerBlockToTestCaseData(containerBlock, includeAttributes);
        innerContainerBlock = (ContainerBlock)containerBlock[0];
        yield return containerBlockToTestCaseData(innerContainerBlock, includeAttributes);
        // Apple
        LeafBlock leafBlock = (LeafBlock)innerContainerBlock[0];
        yield return leafBlockToTestCaseData(leafBlock, includeAttributes);
        yield return nonContainerTestCaseData(leafBlock.Inline!.FirstChild!, includeAttributes);
        // :   Pomaceous fruit of plants of the genus Malus in
        //     the family Rosaceae.
        leafBlock = (LeafBlock)innerContainerBlock[1];
        yield return leafBlockToTestCaseData(leafBlock, includeAttributes);
        inline = leafBlock.Inline!.FirstChild!;
        yield return nonContainerTestCaseData(inline, includeAttributes);
        inline = inline.NextSibling!;
        yield return nonContainerTestCaseData(inline, includeAttributes);
        yield return nonContainerTestCaseData(inline.NextSibling!, includeAttributes);

        innerContainerBlock = (ContainerBlock)containerBlock[1];
        yield return containerBlockToTestCaseData(innerContainerBlock, includeAttributes);
        // Orange
        leafBlock = (LeafBlock)innerContainerBlock[0];
        yield return leafBlockToTestCaseData(leafBlock, includeAttributes);
        yield return nonContainerTestCaseData(leafBlock.Inline!.FirstChild!, includeAttributes);
        // :   The fruit of an evergreen tree of the genus Citrus.
        leafBlock = (LeafBlock)innerContainerBlock[1];
        yield return leafBlockToTestCaseData(leafBlock, includeAttributes);
        yield return nonContainerTestCaseData(leafBlock.Inline!.FirstChild!, includeAttributes);

        // ## Code
        headingBlock = (HeadingBlock)document[17];
        yield return leafBlockToTestCaseData(headingBlock, includeAttributes); // Has attribute
        yield return nonContainerTestCaseData(headingBlock.Inline!.FirstChild!, includeAttributes);

        // This is `inline code`.
        paragraphBlock = (ParagraphBlock)document[18];
        yield return leafBlockToTestCaseData(paragraphBlock, includeAttributes);
        inline = paragraphBlock.Inline!.FirstChild!;
        yield return nonContainerTestCaseData(inline, includeAttributes);
        inline = inline.NextSibling!;
        yield return nonContainerTestCaseData(inline, includeAttributes);
        yield return nonContainerTestCaseData(inline.NextSibling!, includeAttributes);

        // ```log
        // This is a fenced code block
        // ```
        yield return leafBlockToTestCaseData((LeafBlock)document[19], includeAttributes); // Has attribute

        // ``` { .html #codeId style="color: #333; background: #f8f8f8;" }
        // This is a fenced code block
        // with an ID and style
        // ```
        yield return leafBlockToTestCaseData((LeafBlock)document[20], includeAttributes); // Has attribute
        // ((LeafBlock)document[20]).Inline

        // ## Attribute lists
        headingBlock = (HeadingBlock)document[21];
        yield return leafBlockToTestCaseData(headingBlock, includeAttributes); // Has attribute
        yield return nonContainerTestCaseData(headingBlock.Inline!.FirstChild!, includeAttributes);

        // This is red paragraph.
        // {: style="color: #333; color: #ff0000;" }
        paragraphBlock = (ParagraphBlock)document[22];
        yield return leafBlockToTestCaseData(paragraphBlock, includeAttributes);
        inline = paragraphBlock.Inline!.FirstChild!;
        yield return nonContainerTestCaseData(inline, includeAttributes);
        yield return nonContainerTestCaseData(inline.NextSibling!, includeAttributes); // Has attribute

        // ## Footnotes
        headingBlock = (HeadingBlock)document[23];
        yield return leafBlockToTestCaseData(headingBlock, includeAttributes); // Has attribute
        yield return nonContainerTestCaseData(headingBlock.Inline!.FirstChild!, includeAttributes);

        paragraphBlock = (ParagraphBlock)document[24];
        yield return leafBlockToTestCaseData(paragraphBlock, includeAttributes);
        inline = paragraphBlock.Inline!.FirstChild!;
        yield return nonContainerTestCaseData(inline, includeAttributes);
        inline = inline.NextSibling!;
        yield return nonContainerTestCaseData(inline, includeAttributes);
        inline = inline.NextSibling!;
        yield return nonContainerTestCaseData(inline, includeAttributes);
        inline = inline.NextSibling!;
        yield return nonContainerTestCaseData(inline, includeAttributes);
        yield return nonContainerTestCaseData(inline.NextSibling!, includeAttributes);

        // ### Smarties
        headingBlock = (HeadingBlock)document[25];
        yield return leafBlockToTestCaseData(headingBlock, includeAttributes); // Has attribute
        yield return nonContainerTestCaseData(headingBlock.Inline!.FirstChild!, includeAttributes);

        // << angle quotes >>
        paragraphBlock = (ParagraphBlock)document[26];
        yield return leafBlockToTestCaseData(paragraphBlock, includeAttributes);
        yield return nonContainerTestCaseData(paragraphBlock.Inline!.FirstChild!, includeAttributes);

        // Ellipsis...
        paragraphBlock = (ParagraphBlock)document[27];
        yield return leafBlockToTestCaseData(paragraphBlock, includeAttributes);
        yield return nonContainerTestCaseData(paragraphBlock.Inline!.FirstChild!, includeAttributes);

        containerBlock = (ContainerBlock)document[28];
        yield return containerBlockToTestCaseData(containerBlock, includeAttributes);
        innerContainerBlock = (ContainerBlock)containerBlock[0];
        yield return containerBlockToTestCaseData(innerContainerBlock, includeAttributes);
        paragraphBlock = (ParagraphBlock)innerContainerBlock[0];
        yield return leafBlockToTestCaseData(paragraphBlock, includeAttributes);
        inline = paragraphBlock.Inline!.FirstChild!;
        yield return nonContainerTestCaseData(inline, includeAttributes);
        yield return nonContainerTestCaseData(inline.NextSibling!, includeAttributes);
        innerContainerBlock = (ContainerBlock)containerBlock[1];
        yield return containerBlockToTestCaseData(innerContainerBlock, includeAttributes);
        paragraphBlock = (ParagraphBlock)innerContainerBlock[0];
        yield return leafBlockToTestCaseData(paragraphBlock, includeAttributes);
        inline = paragraphBlock.Inline!.FirstChild!;
        yield return nonContainerTestCaseData(inline, includeAttributes);
        yield return nonContainerTestCaseData(inline.NextSibling!, includeAttributes);
    }

    /// <summary>
    /// Test cases for <see cref="MarkdownExtensionMethods.GetAllDescendants(MarkdownObject?, bool)"/>.
    /// </summary>
    /// <returns></returns>
    public static System.Collections.IEnumerable GetGetAllDescendantsTestData(bool withAttributes)
    {
        TestCaseData getTestCaseData(MarkdownObject src, bool? includeAttributes, params MarkdownObject[] expected)
        {
            return new TestCaseData(src).Returns(expected.Select(obj => new Tuple<Type, SourceSpan>(obj.GetType(), obj.Span)).ToArray())
                .SetArgDisplayNames($"{src.GetType().Name}: Line {src.Line} Column: {src.Column}", includeAttributes?.ToString() ?? "null");
        }
        TestCaseData getTestCaseData2(MarkdownObject src, bool? includeAttributes, IEnumerable<MarkdownObject> expected)
        {
            return new TestCaseData(src).Returns(expected.Select(obj => new Tuple<Type, SourceSpan>(obj.GetType(), obj.Span)).ToArray())
                .SetArgDisplayNames($"{src.GetType().Name}: Line {src.Line} Column: {src.Column}", includeAttributes?.ToString() ?? "null");
        }
        MarkdownDocument document = GetMarkdownDocument();

        // HeadingBlock
        LeafBlock leafBlock0 = (LeafBlock)document[0];
        List<MarkdownObject> documentTokens;
        // LiteralInline: Example Markdown Document
        Inline inline0 = leafBlock0.Inline!.FirstChild!;
        if (withAttributes)
        {
            // id="example-markdown-document"
            HtmlAttributes attributes = leafBlock0.GetAttributes();
            yield return getTestCaseData(attributes, true);
            yield return getTestCaseData(inline0, true);
            yield return getTestCaseData(leafBlock0, true, attributes, inline0);
            documentTokens = [leafBlock0, attributes, inline0];
        }
        else
        {
            yield return getTestCaseData(inline0, null);
            yield return getTestCaseData(inline0, false);
            yield return getTestCaseData(leafBlock0, null, inline0);
            yield return getTestCaseData(leafBlock0, false, inline0);
            documentTokens = [leafBlock0, inline0];
        }

        // ParagraphBlock
        leafBlock0 = (LeafBlock)document[1];
        // LinkInline
        ContainerInline containerInline0 = (ContainerInline)leafBlock0.Inline!.FirstChild!;
        // LiteralInline: CommonMark Spec
        inline0 = containerInline0.FirstChild!;
        if (withAttributes)
        {
            yield return getTestCaseData(inline0, true);
            yield return getTestCaseData(containerInline0, true, inline0);
            yield return getTestCaseData(leafBlock0, true, containerInline0, inline0);
        }
        else
        {
            yield return getTestCaseData(inline0, null);
            yield return getTestCaseData(inline0, false);
            yield return getTestCaseData(containerInline0, null, inline0);
            yield return getTestCaseData(containerInline0, false, inline0);
            yield return getTestCaseData(leafBlock0, null, containerInline0, inline0);
            yield return getTestCaseData(leafBlock0, false, containerInline0, inline0);
        }
        documentTokens.AddRange([leafBlock0, containerInline0, inline0]);

        // ParagraphBlock
        leafBlock0 = (LeafBlock)document[2];
        // LiteralInline: Hard line break
        inline0 = leafBlock0.Inline!.FirstChild!;
        // LineBreakInline
        Inline inline1 = inline0.NextSibling!;
        // LiteralInline: here
        Inline inline2 = inline1.NextSibling!;
        if (withAttributes)
        {
            yield return getTestCaseData(inline0, true);
            yield return getTestCaseData(inline1, true);
            yield return getTestCaseData(inline2, true);
            yield return getTestCaseData(containerInline0, true, inline0, inline1, inline2);
            yield return getTestCaseData(leafBlock0, true, containerInline0, inline0, inline1, inline2);
        }
        else
        {
            yield return getTestCaseData(inline0, null);
            yield return getTestCaseData(inline0, false);
            yield return getTestCaseData(inline1, null);
            yield return getTestCaseData(inline1, false);
            yield return getTestCaseData(inline2, null);
            yield return getTestCaseData(inline2, false);
            yield return getTestCaseData(leafBlock0, null, inline0, inline1, inline2);
            yield return getTestCaseData(leafBlock0, false, inline0, inline1, inline2);
        }
        documentTokens.AddRange([leafBlock0, inline0, inline1, inline2]);
        ContainerBlock containerBlock0 = (ContainerBlock)document[3];
        List<MarkdownObject> containerTokens;
        // ListItemBlock
        ContainerBlock containerBlock1 = (ContainerBlock)containerBlock0[0];
        // ParagraphBlock
        leafBlock0 = (LeafBlock)containerBlock1[0];
        // TaskList
        inline0 = leafBlock0.Inline!.FirstChild!;
        // LiteralInline: Task
        inline1 = inline0.NextSibling!;
        if (withAttributes)
        {
            // class="contains-task-list"
            HtmlAttributes attributes0 = containerBlock0.GetAttributes();
            yield return getTestCaseData(attributes0, true);
            HtmlAttributes attributes1 = containerBlock1.GetAttributes();
            yield return getTestCaseData(attributes1, true);
            yield return getTestCaseData(inline0, true);
            yield return getTestCaseData(inline1, true);
            yield return getTestCaseData(containerBlock1, true, attributes1, inline0, inline1);
            containerTokens = [attributes0, containerBlock1, attributes1, inline0, inline1];
        }
        else
        {
            yield return getTestCaseData(inline0, true);
            yield return getTestCaseData(inline1, true);
            yield return getTestCaseData(containerBlock1, true, inline0, inline1);
            containerTokens = [containerBlock1, inline0, inline1];
        }

        containerBlock1 = (ContainerBlock)containerBlock0[1];
        // ParagraphBlock
        leafBlock0 = (LeafBlock)containerBlock1[0];
        // TaskList
        inline0 = leafBlock0.Inline!.FirstChild!;
        // LiteralInline: List Item
        inline1 = inline0.NextSibling!;
        if (withAttributes)
        {
            // class="task-list-item"
            HtmlAttributes attributes0 = containerBlock0.GetAttributes();
            yield return getTestCaseData(attributes0, true);
            HtmlAttributes attributes1 = containerBlock1.GetAttributes();
            yield return getTestCaseData(attributes1, true);
            yield return getTestCaseData(inline0, true);
            yield return getTestCaseData(inline1, true);
            yield return getTestCaseData(containerBlock1, true, attributes1, inline0, inline1);
            containerTokens.AddRange([attributes0, containerBlock1, attributes1, inline0, inline1]);
        }
        else
        {
            yield return getTestCaseData(inline0, null);
            yield return getTestCaseData(inline0, false);
            yield return getTestCaseData(inline1, null);
            yield return getTestCaseData(inline1, false);
            yield return getTestCaseData(containerBlock1, null, inline0, inline1);
            yield return getTestCaseData(containerBlock1, false, inline0, inline1);
            containerTokens.AddRange([containerBlock1, inline0, inline1]);
        }
        // ListItemBlock
        containerBlock1 = (ContainerBlock)containerBlock0[2];
        // ParagraphBlock
        leafBlock0 = (LeafBlock)containerBlock1[0];
        // LiteralInline: Normal List
        inline0 = leafBlock0.Inline!.FirstChild!;
        containerTokens.AddRange([containerBlock1, inline0, inline1]);
        if (withAttributes)
        {
            yield return getTestCaseData(inline0, true);
            yield return getTestCaseData(inline1, true);
            yield return getTestCaseData(containerBlock1, true, inline0, inline1);
        }
        else
        {
            yield return getTestCaseData(inline0, null);
            yield return getTestCaseData(inline0, false);
            yield return getTestCaseData(inline1, null);
            yield return getTestCaseData(inline1, false);
            yield return getTestCaseData(containerBlock1, null, inline0, inline1);
            yield return getTestCaseData(containerBlock1, false, inline0, inline1);
        }
        // ListItemBlock
        containerBlock1 = (ContainerBlock)containerBlock0[3];
        // ParagraphBlock
        leafBlock0 = (LeafBlock)containerBlock1[0];
        // LiteralInline: Item
        inline0 = leafBlock0.Inline!.FirstChild!;
        containerTokens.AddRange([containerBlock1, inline0, inline1]);
        documentTokens.Add(containerBlock0);
        documentTokens.AddRange(containerTokens);
        if (withAttributes)
        {
            yield return getTestCaseData(inline0, true);
            yield return getTestCaseData(inline1, true);
            yield return getTestCaseData(containerBlock1, true, inline0, inline1);
            yield return getTestCaseData2(containerBlock0, true, containerTokens);
        }
        else
        {
            yield return getTestCaseData(inline0, null);
            yield return getTestCaseData(inline0, false);
            yield return getTestCaseData(inline1, null);
            yield return getTestCaseData(inline1, false);
            yield return getTestCaseData(containerBlock1, null, inline0, inline1);
            yield return getTestCaseData(containerBlock1, false, inline0, inline1);
            yield return getTestCaseData2(containerBlock0, null, containerTokens);
            yield return getTestCaseData2(containerBlock0, false, containerTokens);
        }

        // HeadingBlock
        leafBlock0 = (LeafBlock)document[4];
        // LiteralInline: Abbreviations
        inline0 = leafBlock0.Inline!.FirstChild!;
        if (withAttributes)
        {
            // id="custom-id"
            HtmlAttributes attributes = leafBlock0.GetAttributes();
            yield return getTestCaseData(attributes, true);
            yield return getTestCaseData(inline0, true);
            yield return getTestCaseData(leafBlock0, true, attributes, inline0);
            documentTokens.AddRange([leafBlock0, attributes, inline0]);
        }
        else
        {
            yield return getTestCaseData(inline0, null);
            yield return getTestCaseData(inline0, false);
            yield return getTestCaseData(leafBlock0, null, inline0);
            yield return getTestCaseData(leafBlock0, false, inline0);
            documentTokens.AddRange([leafBlock0, inline0]);
        }

        // HeadingBlock
        leafBlock0 = (LeafBlock)document[4];
        // LiteralInline: Link
        inline0 = leafBlock0.Inline!.FirstChild!;
        if (withAttributes)
        {
            // id="link"
            HtmlAttributes attributes = leafBlock0.GetAttributes();
            yield return getTestCaseData(attributes, true);
            yield return getTestCaseData(inline0, true);
            yield return getTestCaseData(leafBlock0, true, attributes, inline0);
            documentTokens.AddRange([leafBlock0, attributes, inline0]);
        }
        else
        {
            yield return getTestCaseData(inline0, null);
            yield return getTestCaseData(inline0, false);
            yield return getTestCaseData(leafBlock0, null, inline0);
            yield return getTestCaseData(leafBlock0, false, inline0);
            documentTokens.AddRange([leafBlock0, inline0]);
        }

        // ParagraphBlock
        leafBlock0 = (LeafBlock)document[6];
        // LinkInline
        containerInline0 = (ContainerInline)leafBlock0.Inline!.FirstChild!;
        // LiteralInline: Abbreviations Link
        inline0 = containerInline0.FirstChild!;
        documentTokens.AddRange([leafBlock0, containerInline0, inline0]);
        if (withAttributes)
        {
            yield return getTestCaseData(inline0, true);
            yield return getTestCaseData(containerInline0, true, inline0);
            yield return getTestCaseData(leafBlock0, true, containerInline0, inline0);
        }
        else
        {
            yield return getTestCaseData(inline0, null);
            yield return getTestCaseData(inline0, false);
            yield return getTestCaseData(containerInline0, null, inline0);
            yield return getTestCaseData(containerInline0, false, inline0);
            yield return getTestCaseData(leafBlock0, null, containerInline0, inline0);
            yield return getTestCaseData(leafBlock0, false, containerInline0, inline0);
        }

        // HeadingBlock
        leafBlock0 = (LeafBlock)document[7];
        // LiteralInline: Image with Alt and Title
        inline0 = leafBlock0.Inline!.FirstChild!;
        if (withAttributes)
        {
            // id="image-with-alt-and-title"
            HtmlAttributes attributes = leafBlock0.GetAttributes();
            yield return getTestCaseData(attributes, true);
            yield return getTestCaseData(inline0, true);
            yield return getTestCaseData(leafBlock0, true, attributes, inline0);
            documentTokens.AddRange([leafBlock0, attributes, inline0]);
        }
        else
        {
            yield return getTestCaseData(inline0, null);
            yield return getTestCaseData(inline0, false);
            yield return getTestCaseData(leafBlock0, null, inline0);
            yield return getTestCaseData(leafBlock0, false, inline0);
            documentTokens.AddRange([leafBlock0, inline0]);
        }

        // ParagraphBlock
        leafBlock0 = (LeafBlock)document[6];
        // LinkInline
        containerInline0 = (ContainerInline)leafBlock0.Inline!.FirstChild!;
        // LiteralInline - Url: ./sn-logo.jpg
        inline0 = leafBlock0.Inline!.FirstChild!;
        documentTokens.AddRange([leafBlock0, containerInline0, inline0]);
        if (withAttributes)
        {
            yield return getTestCaseData(inline0, true);
            yield return getTestCaseData(containerInline0, true, inline0);
            yield return getTestCaseData(leafBlock0, true, containerInline0, inline0);
        }
        else
        {
            yield return getTestCaseData(inline0, null);
            yield return getTestCaseData(inline0, false);
            yield return getTestCaseData(containerInline0, null, inline0);
            yield return getTestCaseData(containerInline0, false, inline0);
            yield return getTestCaseData(leafBlock0, null, containerInline0, inline0);
            yield return getTestCaseData(leafBlock0, false, containerInline0, inline0);
        }

        // ParagraphBlock
        leafBlock0 = (LeafBlock)document[9];
        // LinkInline
        containerInline0 = (ContainerInline)leafBlock0.Inline!.FirstChild!;
        // LiteralInline - Url: ./sn-logo.jpg
        inline0 = leafBlock0.Inline!.FirstChild!;
        // EmphasisInline
        ContainerInline containerInline1 = (ContainerInline)inline0.NextSibling!;
        // LiteralInline: bar
        inline1 = containerInline1.FirstChild!;
        documentTokens.AddRange([leafBlock0, containerInline0, inline0, containerInline1, inline1]);
        if (withAttributes)
        {
            yield return getTestCaseData(inline1, true);
            yield return getTestCaseData(containerInline1, true, inline1);
            yield return getTestCaseData(inline0, true);
            yield return getTestCaseData(containerInline0, true, inline0, containerInline1, inline1);
            yield return getTestCaseData(leafBlock0, true, containerInline0, inline0, containerInline1, inline1);
        }
        else
        {
            yield return getTestCaseData(inline1, null);
            yield return getTestCaseData(inline1, false);
            yield return getTestCaseData(containerInline1, null, inline1);
            yield return getTestCaseData(containerInline1, false, inline1);
            yield return getTestCaseData(inline0, null);
            yield return getTestCaseData(inline0, false);
            yield return getTestCaseData(containerInline0, null, inline0, containerInline1, inline1);
            yield return getTestCaseData(containerInline0, false, inline0, containerInline1, inline1);
            yield return getTestCaseData(leafBlock0, null, containerInline0, inline0, containerInline1, inline1);
            yield return getTestCaseData(leafBlock0, false, containerInline0, inline0, containerInline1, inline1);
        }

        // LinkReferenceDefinitionGroup
        containerBlock0 = (ContainerBlock)document[10];
        containerTokens = containerBlock0.Cast<MarkdownObject>().ToList();
        documentTokens.Add(containerBlock0);
        documentTokens.AddRange(containerTokens);
        if (withAttributes)
        {
            foreach (var token in containerTokens)
                yield return getTestCaseData(token, true);
            yield return getTestCaseData2(containerBlock0, true, containerTokens);
        }
        else
        {
            foreach (var token in containerTokens)
            {
                yield return getTestCaseData(token, null);
                yield return getTestCaseData(token, false);
            }
            yield return getTestCaseData2(containerBlock0, null, containerTokens);
            yield return getTestCaseData2(containerBlock0, false, containerTokens);
        }

        // HeadingBlock
        leafBlock0 = (LeafBlock)document[11];
        // LiteralInline: Math
        inline0 = leafBlock0.Inline!.FirstChild!;
        if (withAttributes)
        {
            // id="math"
            HtmlAttributes attributes = leafBlock0.GetAttributes();
            yield return getTestCaseData(attributes, true);
            yield return getTestCaseData(inline0, true);
            yield return getTestCaseData(leafBlock0, true, attributes, inline0);
            documentTokens.AddRange([leafBlock0, attributes, inline0]);
        }
        else
        {
            yield return getTestCaseData(inline0, null);
            yield return getTestCaseData(inline0, false);
            yield return getTestCaseData(leafBlock0, null, inline0);
            yield return getTestCaseData(leafBlock0, false, inline0);
            documentTokens.AddRange([leafBlock0, inline0]);
        }

        // ParagraphBlock
        leafBlock0 = (LeafBlock)document[12];
        // LiteralInline: This sentence uses
        inline0 = leafBlock0.Inline!.FirstChild!;
        // CodeInline
        inline1 = inline0.NextSibling!;
        // LiteralInline: delimiters to show math inline:
        inline2 = inline1.NextSibling!;
        // MathInline
        Inline inline3 = inline2.NextSibling!;
        if (withAttributes)
        {
            // class="math"
            yield return getTestCaseData(inline0, true);
            yield return getTestCaseData(inline1, true);
            yield return getTestCaseData(inline2, true);
            // class="math"
            HtmlAttributes attributes = inline3.GetAttributes();
            yield return getTestCaseData(attributes, true);
            yield return getTestCaseData(inline3, true, attributes);
            yield return getTestCaseData(leafBlock0, true, inline0, inline1, inline2, inline3, attributes);
            documentTokens.AddRange([leafBlock0, inline0, inline1, inline2, inline3, attributes]);
        }
        else
        {
            yield return getTestCaseData(inline0, true);
            yield return getTestCaseData(inline1, true);
            yield return getTestCaseData(inline2, true);
            yield return getTestCaseData(leafBlock0, true, inline0, inline1, inline2, inline3);
            documentTokens.AddRange([leafBlock0, inline0, inline1, inline2, inline3]);
        }

        // ParagraphBlock
        leafBlock0 = (LeafBlock)document[13];
        // LiteralInline: This sentence uses
        inline0 = leafBlock0.Inline!.FirstChild!;
        // MathInline
        inline1 = inline0.NextSibling!;
        // LiteralInline: delimiters to show math inline:
        inline2 = inline1.NextSibling!;
        // MathInline
        inline3 = inline2.NextSibling!;
        if (withAttributes)
        {
            yield return getTestCaseData(inline0, true);
            // class="math"
            HtmlAttributes attributes0 = inline3.GetAttributes();
            yield return getTestCaseData(attributes0, true);
            yield return getTestCaseData(inline1, true, attributes0);
            yield return getTestCaseData(inline2, true);
            // class="math"
            HtmlAttributes attributes1 = inline3.GetAttributes();
            yield return getTestCaseData(attributes1, true);
            yield return getTestCaseData(inline3, true, attributes1);
            yield return getTestCaseData(leafBlock0, true, inline0, inline1, attributes0, inline2, inline3, attributes1);
            documentTokens.AddRange([leafBlock0, inline0, inline1, attributes0, inline2, inline3, attributes1]);
        }
        else
        {
            yield return getTestCaseData(inline0, true);
            yield return getTestCaseData(inline1, true);
            yield return getTestCaseData(inline2, true);
            yield return getTestCaseData(leafBlock0, true, inline0, inline1, inline2, inline3);
            documentTokens.AddRange([leafBlock0, inline0, inline1, inline2, inline3]);
        }

        // ParagraphBlock
        leafBlock0 = (LeafBlock)document[14];
        // EmphasisInline
        containerInline0 = (ContainerInline)leafBlock0.Inline!.FirstChild!;
        // LiteralInline: The Cauchy-Schwarz Inequality
        inline0 = leafBlock0.Inline!.FirstChild!;
        // LineBreakInline
        inline1 = inline0.NextSibling!;
        // MathInline
        inline2 = inline1.NextSibling!;
        if (withAttributes)
        {
            yield return getTestCaseData(inline0, true);
            // class="math"
            yield return getTestCaseData(inline1, true);
            HtmlAttributes attributes = inline3.GetAttributes();
            yield return getTestCaseData(attributes, true);
            yield return getTestCaseData(inline2, true, attributes);
            yield return getTestCaseData(leafBlock0, true, inline0, inline1, inline2, attributes);
            documentTokens.AddRange([leafBlock0, inline0, inline1, inline2, attributes]);
        }
        else
        {
            yield return getTestCaseData(inline0, true);
            yield return getTestCaseData(inline1, true);
            yield return getTestCaseData(inline2, true);
            yield return getTestCaseData(leafBlock0, true, inline0, inline1, inline2);
            documentTokens.AddRange([leafBlock0, inline0, inline1, inline2]);
        }

        // HeadingBlock
        leafBlock0 = (LeafBlock)document[15];
        // LiteralInline: Definition List
        inline0 = leafBlock0.Inline!.FirstChild!;
        if (withAttributes)
        {
            // id="definition-list"
            HtmlAttributes attributes = leafBlock0.GetAttributes();
            yield return getTestCaseData(attributes, true);
            yield return getTestCaseData(inline0, true);
            yield return getTestCaseData(leafBlock0, true, attributes, inline0);
            documentTokens.AddRange([leafBlock0, attributes, inline0]);
        }
        else
        {
            yield return getTestCaseData(inline0, null);
            yield return getTestCaseData(inline0, false);
            yield return getTestCaseData(leafBlock0, null, inline0);
            yield return getTestCaseData(leafBlock0, false, inline0);
            documentTokens.AddRange([leafBlock0, inline0]);
        }

        // DefinitionList
        containerBlock0 = (ContainerBlock)document[16];

        // DefinitionItem
        containerBlock1 = (ContainerBlock)containerBlock0[0];
        // DefinitionTerm
        leafBlock0 = (LeafBlock)containerBlock1[0];
        // LiteralInline: Apple
        inline0 = leafBlock0.Inline!.FirstChild!;
        // ParagraphBlock
        LeafBlock leafBlock1 = (LeafBlock)containerBlock1[1];
        // LiteralInline: Pomaceous fruit of plants of the genus Malus in
        inline1 = leafBlock1.Inline!.FirstChild!;
        // LineBreakInline
        inline2 = inline1.NextSibling!;
        // LiteralInline: the family Rosaceae.
        inline3 = inline2.NextSibling!;
        containerTokens = [containerBlock1, leafBlock0, inline0, leafBlock1, inline1, inline2, inline3];
        if (withAttributes)
        {
            yield return getTestCaseData(inline0, true);
            yield return getTestCaseData(leafBlock0, true, inline0);
            yield return getTestCaseData(inline1, true);
            yield return getTestCaseData(inline2, true);
            yield return getTestCaseData(inline3, true);
            yield return getTestCaseData(leafBlock1, true, inline1, inline2, inline3);
            yield return getTestCaseData(containerBlock1, true, leafBlock0, inline0, leafBlock1, inline1, inline2, inline3);
        }
        else
        {
            yield return getTestCaseData(inline0, null);
            yield return getTestCaseData(inline0, false);
            yield return getTestCaseData(leafBlock0, null, inline0);
            yield return getTestCaseData(leafBlock0, false, inline0);
            yield return getTestCaseData(inline1, null);
            yield return getTestCaseData(inline1, false);
            yield return getTestCaseData(inline2, null);
            yield return getTestCaseData(inline2, false);
            yield return getTestCaseData(inline3, null);
            yield return getTestCaseData(inline3, false);
            yield return getTestCaseData(leafBlock1, null, inline1, inline2, inline3);
            yield return getTestCaseData(leafBlock1, false, inline1, inline2, inline3);
            yield return getTestCaseData(containerBlock1, null, leafBlock0, inline0, leafBlock1, inline1, inline2, inline3);
            yield return getTestCaseData(containerBlock1, false, leafBlock0, inline0, leafBlock1, inline1, inline2, inline3);
        }

        // DefinitionItem
        containerBlock1 = (ContainerBlock)containerBlock0[1];
        // DefinitionTerm
        leafBlock0 = (LeafBlock)containerBlock1[0];
        // LiteralInline: Orange
        inline0 = leafBlock0.Inline!.FirstChild!;
        // ParagraphBlock
        leafBlock1 = (LeafBlock)containerBlock1[1];
        // LiteralInline: The fruit of an evergreen tree of the genus Citrus.
        inline1 = leafBlock1.Inline!.FirstChild!;
        containerTokens.AddRange([containerBlock1, leafBlock0, inline0, leafBlock1, inline1]);
        documentTokens.Add(containerBlock0);
        documentTokens.AddRange(containerTokens);
        if (withAttributes)
        {
            yield return getTestCaseData(inline0, true);
            yield return getTestCaseData(leafBlock0, true, inline0);
            yield return getTestCaseData(inline1, true);
            yield return getTestCaseData(leafBlock1, true, inline1);
            yield return getTestCaseData(containerBlock1, true, leafBlock0, inline0, leafBlock1, inline1);
        }
        else
        {
            yield return getTestCaseData(inline0, null);
            yield return getTestCaseData(inline0, false);
            yield return getTestCaseData(leafBlock0, null, inline0);
            yield return getTestCaseData(leafBlock0, false, inline0);
            yield return getTestCaseData(inline1, null);
            yield return getTestCaseData(inline1, false);
            yield return getTestCaseData(leafBlock1, null, inline1);
            yield return getTestCaseData(leafBlock1, false, inline1);
            yield return getTestCaseData(containerBlock1, null, leafBlock0, inline0, leafBlock1, inline1);
            yield return getTestCaseData(containerBlock1, false, leafBlock0, inline0, leafBlock1, inline1);
        }

        // HeadingBlock
        leafBlock0 = (LeafBlock)document[17];
        // LiteralInline: Code
        inline0 = leafBlock0.Inline!.FirstChild!;
        if (withAttributes)
        {
            // id="code"
            HtmlAttributes attributes = leafBlock0.GetAttributes();
            yield return getTestCaseData(attributes, true);
            yield return getTestCaseData(inline0, true);
            yield return getTestCaseData(leafBlock0, true, attributes, inline0);
            documentTokens.AddRange([leafBlock0, attributes, inline0]);
        }
        else
        {
            yield return getTestCaseData(inline0, null);
            yield return getTestCaseData(inline0, false);
            yield return getTestCaseData(leafBlock0, null, inline0);
            yield return getTestCaseData(leafBlock0, false, inline0);
            documentTokens.AddRange([leafBlock0, inline0]);
        }

        // ParagraphBlock
        leafBlock0 = (LeafBlock)document[18];
        // LiteralInline: This is
        inline0 = leafBlock0.Inline!.FirstChild!;
        // CodeInline
        inline1 = inline0.NextSibling!;
        // LiteralInline: .
        inline2 = inline1.NextSibling!;
        documentTokens.AddRange([leafBlock0, inline0, inline1, inline2]);
        if (withAttributes)
        {
            yield return getTestCaseData(inline0, true);
            yield return getTestCaseData(inline1, true);
            yield return getTestCaseData(inline2, true);
            yield return getTestCaseData(leafBlock0, true, inline0, inline1, inline2);
        }
        else
        {
            yield return getTestCaseData(inline0, null);
            yield return getTestCaseData(inline0, false);
            yield return getTestCaseData(inline1, null);
            yield return getTestCaseData(inline1, false);
            yield return getTestCaseData(inline2, null);
            yield return getTestCaseData(inline2, false);
            yield return getTestCaseData(leafBlock1, null, inline0, inline1, inline2);
            yield return getTestCaseData(leafBlock1, false, inline0, inline1, inline2);
        }

        // FencedCodeBlock
        leafBlock0 = (LeafBlock)document[19];
        if (withAttributes)
        {
            // class="language-log"
            HtmlAttributes attributes = leafBlock0.GetAttributes();
            yield return getTestCaseData(attributes, true);
            yield return getTestCaseData(leafBlock0, true, attributes);
            documentTokens.AddRange([leafBlock0, attributes]);
        }
        else
        {
            yield return getTestCaseData(leafBlock0, null);
            yield return getTestCaseData(leafBlock0, false);
            documentTokens.Add(leafBlock0);
        }

        // FencedCodeBlock
        leafBlock0 = (LeafBlock)document[20];
        if (withAttributes)
        {
            // class="html" id="codeId" style="color: #333; background: #f8f8f8;"
            HtmlAttributes attributes = leafBlock0.GetAttributes();
            yield return getTestCaseData(attributes, true);
            yield return getTestCaseData(leafBlock0, true, attributes);
            documentTokens.AddRange([leafBlock0, attributes]);
        }
        else
        {
            yield return getTestCaseData(leafBlock0, null);
            yield return getTestCaseData(leafBlock0, false);
            documentTokens.Add(leafBlock0);
        }

        // HeadingBlock
        leafBlock0 = (LeafBlock)document[21];
        // LiteralInline: Attribute lists
        inline0 = leafBlock0.Inline!.FirstChild!;
        if (withAttributes)
        {
            // id="attribute-lists"
            HtmlAttributes attributes = leafBlock0.GetAttributes();
            yield return getTestCaseData(attributes, true);
            yield return getTestCaseData(inline0, true);
            yield return getTestCaseData(leafBlock0, true, attributes, inline0);
            documentTokens.AddRange([leafBlock0, attributes, inline0]);
        }
        else
        {
            yield return getTestCaseData(inline0, null);
            yield return getTestCaseData(inline0, false);
            yield return getTestCaseData(leafBlock0, null, inline0);
            yield return getTestCaseData(leafBlock0, false, inline0);
            documentTokens.AddRange([leafBlock0, inline0]);
        }

        // ParagraphBlock
        leafBlock0 = (LeafBlock)document[22];
        // LiteralInline: This is red paragraph.
        inline0 = leafBlock0.Inline!.FirstChild!;
        // LineBreakInline
        inline1 = inline0.NextSibling!;
        if (withAttributes)
        {
            // style="color: #333; color: #ff0000;"
            yield return getTestCaseData(inline0, true);
            HtmlAttributes attributes = leafBlock0.GetAttributes();
            yield return getTestCaseData(attributes, true);
            yield return getTestCaseData(inline1, true, attributes);
            yield return getTestCaseData(leafBlock0, true, inline0, inline1, attributes);
            documentTokens.AddRange([leafBlock0, inline0, inline1, attributes]);
        }
        else
        {
            yield return getTestCaseData(inline0, null);
            yield return getTestCaseData(inline0, false);
            yield return getTestCaseData(inline1, null);
            yield return getTestCaseData(inline1, false);
            yield return getTestCaseData(leafBlock0, null, inline0, inline1);
            yield return getTestCaseData(leafBlock0, false, inline0, inline1);
            documentTokens.AddRange([leafBlock0, inline0, inline1]);
        }

        // HeadingBlock
        leafBlock0 = (LeafBlock)document[23];
        // LiteralInline: Footnotes
        inline0 = leafBlock0.Inline!.FirstChild!;
        if (withAttributes)
        {
            // id="footnotes"
            HtmlAttributes attributes = leafBlock0.GetAttributes();
            yield return getTestCaseData(attributes, true);
            yield return getTestCaseData(inline0, true);
            yield return getTestCaseData(leafBlock0, true, attributes, inline0);
            documentTokens.AddRange([leafBlock0, attributes, inline0]);
        }
        else
        {
            yield return getTestCaseData(inline0, null);
            yield return getTestCaseData(inline0, false);
            yield return getTestCaseData(leafBlock0, null, inline0);
            yield return getTestCaseData(leafBlock0, false, inline0);
            documentTokens.AddRange([leafBlock0, inline0]);
        }

        // ParagraphBlock
        leafBlock0 = (LeafBlock)document[24];
        // LiteralInline: Footnotes.
        inline0 = leafBlock0.Inline!.FirstChild!;
        // FootnoteLink
        inline1 = inline0.NextSibling!;
        // LiteralInline: have a label
        inline2 = inline1.NextSibling!;
        // FootnoteLink
        inline3 = inline2.NextSibling!;
        // LiteralInline:  and the footnote's content.
        Inline inline4 = inline3.NextSibling!;
        documentTokens.AddRange([leafBlock0, inline0, inline1, inline2, inline3, inline4]);
        if (withAttributes)
        {
            yield return getTestCaseData(inline0, true);
            yield return getTestCaseData(inline1, true);
            yield return getTestCaseData(inline3, true);
            yield return getTestCaseData(inline4, true);
            yield return getTestCaseData(leafBlock0, true, inline0, inline1, inline2, inline3, inline4);
        }
        else
        {
            yield return getTestCaseData(inline0, null);
            yield return getTestCaseData(inline0, false);
            yield return getTestCaseData(inline1, null);
            yield return getTestCaseData(inline1, false);
            yield return getTestCaseData(inline3, null);
            yield return getTestCaseData(inline3, false);
            yield return getTestCaseData(inline4, null);
            yield return getTestCaseData(inline4, false);
            yield return getTestCaseData(leafBlock0, null, inline0, inline1, inline2, inline3, inline4);
            yield return getTestCaseData(leafBlock0, false, inline0, inline1, inline2, inline3, inline4);
        }

        // HeadingBlock
        leafBlock0 = (LeafBlock)document[25];
        // LiteralInline: Smarties
        inline0 = leafBlock0.Inline!.FirstChild!;
        if (withAttributes)
        {
            // id="smarties"
            HtmlAttributes attributes = leafBlock0.GetAttributes();
            yield return getTestCaseData(attributes, true);
            yield return getTestCaseData(inline0, true);
            yield return getTestCaseData(leafBlock0, true, attributes, inline0);
            documentTokens.AddRange([leafBlock0, attributes, inline0]);
        }
        else
        {
            yield return getTestCaseData(inline0, null);
            yield return getTestCaseData(inline0, false);
            yield return getTestCaseData(leafBlock0, null, inline0);
            yield return getTestCaseData(leafBlock0, false, inline0);
            documentTokens.AddRange([leafBlock0, inline0]);
        }

        // ParagraphBlock
        leafBlock0 = (LeafBlock)document[26];
        // LiteralInline: Footnotes
        inline0 = leafBlock0.Inline!.FirstChild!;
        documentTokens.AddRange([leafBlock0, inline0]);
        if (withAttributes)
        {
            yield return getTestCaseData(inline0, true);
            yield return getTestCaseData(leafBlock0, true, inline0);
        }
        else
        {
            yield return getTestCaseData(inline0, null);
            yield return getTestCaseData(leafBlock1, null, inline0);
            yield return getTestCaseData(leafBlock1, false, inline0);
        }

        // ParagraphBlock
        leafBlock0 = (LeafBlock)document[27];
        // LiteralInline: Ellipsis...
        inline0 = leafBlock0.Inline!.FirstChild!;
        documentTokens.AddRange([leafBlock0, inline0]);
        if (withAttributes)
        {
            yield return getTestCaseData(inline0, true);
            yield return getTestCaseData(leafBlock0, true, inline0);
        }
        else
        {
            yield return getTestCaseData(inline0, null);
            yield return getTestCaseData(inline0, false);
            yield return getTestCaseData(leafBlock1, null, inline0);
            yield return getTestCaseData(leafBlock1, false, inline0);
        }

        // FootnoteGroup
        containerBlock0 = (ContainerBlock)document[28];

        // Footnote
        containerBlock1 = (ContainerBlock)containerBlock0[1];
        // ParagraphBlock
        leafBlock0 = (LeafBlock)containerBlock1[0];
        // LiteralInline: This is a footnote content.
        inline0 = leafBlock0.Inline!.FirstChild!;
        // FootnoteLink
        inline1 = inline0.NextSibling!;
        containerTokens = [containerBlock1, leafBlock0, inline0, inline1];
        if (withAttributes)
        {
            yield return getTestCaseData(inline0, true);
            yield return getTestCaseData(inline1, true);
            yield return getTestCaseData(leafBlock0, true, inline0, inline1);
            yield return getTestCaseData(containerBlock1, true, leafBlock0, inline0, inline1);
        }
        else
        {
            yield return getTestCaseData(inline0, null);
            yield return getTestCaseData(inline0, false);
            yield return getTestCaseData(inline1, null);
            yield return getTestCaseData(inline1, false);
            yield return getTestCaseData(leafBlock0, null, inline0, inline1);
            yield return getTestCaseData(leafBlock0, false, inline0, inline1);
            yield return getTestCaseData(containerBlock1, null, leafBlock0, inline0, inline1);
            yield return getTestCaseData(containerBlock1, false, leafBlock0, inline0, inline1);
        }

        // Footnote
        containerBlock1 = (ContainerBlock)containerBlock0[1];
        // ParagraphBlock
        leafBlock0 = (LeafBlock)containerBlock1[0];
        // LiteralInline: A footnote on the label: \u0022@#$%\u0022.
        inline0 = leafBlock0.Inline!.FirstChild!;
        // FootnoteLink
        inline1 = inline0.NextSibling!;
        containerTokens.AddRange([containerBlock1, leafBlock0, inline0, inline1]);
        documentTokens.Add(containerBlock0);
        documentTokens.AddRange(containerTokens);
        if (withAttributes)
        {
            yield return getTestCaseData(inline0, true);
            yield return getTestCaseData(inline1, true);
            yield return getTestCaseData(leafBlock0, true, inline0, inline1);
            yield return getTestCaseData(containerBlock1, true, leafBlock0, inline0, inline1);
            yield return getTestCaseData2(containerBlock0, true, containerTokens);
        }
        else
        {
            yield return getTestCaseData(inline0, null);
            yield return getTestCaseData(inline0, false);
            yield return getTestCaseData(inline1, null);
            yield return getTestCaseData(inline1, false);
            yield return getTestCaseData(leafBlock0, null, inline0, inline1);
            yield return getTestCaseData(leafBlock0, false, inline0, inline1);
            yield return getTestCaseData(containerBlock1, null, leafBlock0, inline0, inline1);
            yield return getTestCaseData(containerBlock1, false, leafBlock0, inline0, inline1);
            yield return getTestCaseData2(containerBlock0, null, containerTokens);
            yield return getTestCaseData2(containerBlock0, false, containerTokens);
        }
    }

    /// <summary>
    /// Test cases for <see cref="MarkdownExtensionMethods.GetDescendantBranchesMatchingType(MarkdownObject?, Type)"/>.
    /// </summary>
    /// <returns></returns>
    public static System.Collections.IEnumerable GetGetDescendantBranchesMatchingType1TestData()
    {
        TestCaseData createTestCaseData(MarkdownObject source, Type type, params MarkdownObject[] expected)
        {
            return new TestCaseData(source, type)
                .Returns(expected.Select(o => new Tuple<Type, SourceSpan, int, int>(o.GetType(), o.Span, o.Line, o.Column)).ToArray())
                .SetArgDisplayNames($"{source.GetType().Name} {{ {source.ToPositionText()}}}", type.FullName!);
        }

        /*
CodeBlock
FencedCodeBlock

"((Fenced)?Code|Heading|Html|Leaf|Empty|Paragraph|ThematicBreak|YamlFrontMatter|Math)Block|(Footnote|Heading)?LinkReferenceDefinition|FigureCaption|DefinitionTerm|Abbreviation"

Markdig.Extensions.Yaml.YamlFrontMatterBlock
Markdig.Extensions.Mathematics.MathBlock
Markdig.Extensions.Footnotes.FootnoteLinkReferenceDefinition
Markdig.Extensions.Figures.FigureCaption
Markdig.Extensions.DefinitionLists.DefinitionTerm
Markdig.Extensions.AutoIdentifiers.HeadingLinkReferenceDefinition
Markdig.Extensions.Abbreviations.Abbreviation
        */
        MarkdownDocument document = GetMarkdownDocument();
        yield return createTestCaseData(document, typeof(MarkdownDocument), document);
        yield return createTestCaseData(document, typeof(HeadingBlock), document.OfType<HeadingBlock>().ToArray());
        IEnumerable<MarkdownObject> expected = document.Take(3);
        expected = expected.Concat(((ListBlock)document[3]).Cast<ListItemBlock>().Select(lib => lib[0]));
        expected = expected.Concat(document.Skip(4).Take(6));
        expected = expected.Concat((ContainerBlock)document[10]);
        expected = expected.Concat(document.Skip(11).Take(5));
        expected = expected.Concat(((Markdig.Extensions.DefinitionLists.DefinitionList)document[16]).SelectMany(b => ((Markdig.Extensions.DefinitionLists.DefinitionItem)b).AsEnumerable()));
        expected = expected.Concat(document.Skip(17).Take(11));
        expected = expected.Concat(((Markdig.Extensions.Footnotes.FootnoteGroup)document[28]).Cast<Markdig.Extensions.Footnotes.Footnote>().Select(f => f[0]));
        yield return createTestCaseData(document, typeof(LeafBlock), expected.ToArray());

        LeafBlock leafBlock = (LeafBlock)document[24];
        var inline0 = leafBlock.Inline!.FirstChild!;
        yield return createTestCaseData(leafBlock, typeof(LiteralInline), [inline0, inline0.NextSibling!.NextSibling!, leafBlock.Inline!.LastChild!]);

        yield return createTestCaseData(leafBlock, typeof(ParagraphBlock), []);
    }

    /// <summary>
    /// Test cases for <see cref="MarkdownExtensionMethods.GetDescendantBranchesMatchingType(MarkdownObject?, IEnumerable{Type})"/>.
    /// </summary>
    /// <returns></returns>
    public static System.Collections.IEnumerable GetGetDescendantBranchesMatchingType2TestData()
    {
        TestCaseData createTestCaseData(MarkdownObject? source, IEnumerable<Type> types, params MarkdownObject[] expected)
        {
            return new TestCaseData(source, types)
                .Returns(expected.Select(o => new Tuple<Type, SourceSpan, int, int>(o.GetType(), o.Span, o.Line, o.Column)).ToArray())
                .SetArgDisplayNames((source is null) ? "null" : $"{source.GetType().Name} {{ {source.ToPositionText()}}}", $"[{string.Join(", ", types.Select(t => t.FullName))}]");
        }

        MarkdownDocument document = GetMarkdownDocument();
        yield return createTestCaseData(document, [typeof(ParagraphBlock), typeof(LeafBlock)], document.Take(3).Concat(((ListBlock)document[3]).Cast<ListItemBlock>().Select(lib => lib[0]))
            .Concat(document.Skip(4).Take(6)).Concat((ContainerBlock)document[10]).Concat(document.Skip(11).Take(5)).Concat(((ContainerBlock)document[16]).SelectMany(b => ((ContainerBlock)b).AsEnumerable()))
            .Concat(document.Skip(17).Take(11)).Concat(((Markdig.Extensions.Footnotes.FootnoteGroup)document[28]).Cast<Markdig.Extensions.Footnotes.Footnote>().Select(f => f[0])).ToArray());

        yield return createTestCaseData(document, [typeof(CodeInline), typeof(FencedCodeBlock), typeof(LineBreakInline)], [((LeafBlock)document[2]).Inline!.FirstChild!.NextSibling!,
            ((LeafBlock)document[12]).Inline!.FirstChild!.NextSibling!, ((LeafBlock)document[14]).Inline!.FirstChild!.NextSibling!,
            ((LeafBlock)((ContainerBlock)((ContainerBlock)document[16])[0])[1]).Inline!.FirstChild!.NextSibling!, ((LeafBlock)document[18]).Inline!.FirstChild!.NextSibling!, document[19], document[20],
            ((LeafBlock)document[22]).Inline!.FirstChild!.NextSibling!]);

        yield return createTestCaseData(document, [typeof(ListBlock), typeof(ParagraphBlock)], document.Skip(1).Take(3).Concat([document[6], document[8], document[9]]).Concat(document.Skip(12).Take(3))
            .Concat(((ContainerBlock)document[16]).Select(b => ((ContainerBlock)b)[1])).Concat([document[18], document[22], document[24]]).Concat(document.Skip(26).Take(3)).ToArray());

        yield return createTestCaseData(((ContainerBlock)((ContainerBlock)document[16])[0])[1], [typeof(ParagraphBlock), typeof(ContainerBlock)], []);
    }

    /// <summary>
    /// Test cases for <see cref="MarkdownExtensionMethods.GetDescendantBranchesMatchingType(MarkdownObject?, Type, int)"/>.
    /// </summary>
    /// <returns></returns>
    public static System.Collections.IEnumerable GetGetDescendantBranchesMatchingType3TestData()
    {
        TestCaseData createTestCaseData(MarkdownObject? source, Type type, int maximumDepth, params MarkdownObject[] expected)
        {
            return new TestCaseData(source, type, maximumDepth)
                .Returns(expected.Select(o => new Tuple<Type, SourceSpan, int, int>(o.GetType(), o.Span, o.Line, o.Column)).ToArray())
                .SetArgDisplayNames((source is null) ? "null" : $"{source.GetType().Name} {{ {source.ToPositionText()}}}", type.FullName!, maximumDepth.ToString());
        }
        MarkdownDocument document = GetMarkdownDocument();
        IEnumerable<MarkdownObject> expected = [((LeafBlock)document[0]).Inline!.FirstChild!, ((ContainerInline)((LeafBlock)document[1]).Inline!.FirstChild!).FirstChild!];
        var containerInline = ((LeafBlock)document[2]).Inline!;
        expected = expected.Concat([containerInline.FirstChild!, containerInline.LastChild!, ((LeafBlock)document[4]).Inline!.FirstChild!, ((LeafBlock)document[5]).Inline!.FirstChild!,
            ((ContainerInline)((LeafBlock)document[6]).Inline!.FirstChild!).FirstChild!, ((LeafBlock)document[7]).Inline!.FirstChild!,
            ((ContainerInline)((LeafBlock)document[8]).Inline!.FirstChild!).FirstChild!]);
        containerInline = (ContainerInline)((LeafBlock)document[9]).Inline!.FirstChild!;
        expected = expected.Concat([containerInline.FirstChild!, ((ContainerInline)containerInline.LastChild!).FirstChild!, ((LeafBlock)document[11]).Inline!.FirstChild!]);
        var inline = ((LeafBlock)document[12]).Inline!.FirstChild!;
        expected = expected.Concat([inline, inline.NextSibling!.NextSibling!]);
        inline = ((LeafBlock)document[13]).Inline!.FirstChild!;
        expected = expected.Concat([inline, inline.NextSibling!.NextSibling!, ((ContainerInline)((LeafBlock)document[14]).Inline!.FirstChild!).FirstChild!,
            ((LeafBlock)document[15]).Inline!.FirstChild!, ((LeafBlock)document[17]).Inline!.FirstChild!]);
        containerInline = ((LeafBlock)document[18]).Inline!;
        expected = expected.Concat([containerInline.FirstChild!, containerInline.LastChild!]);
        expected = expected.Concat(document.Skip(21).Take(3).Cast<LeafBlock>().Select(lb => lb.Inline!.FirstChild!));
        containerInline = ((LeafBlock)document[24]).Inline!;
        inline = containerInline.FirstChild!;
        expected = expected.Concat([inline, inline.NextSibling!.NextSibling!, containerInline.LastChild!]);
        expected = expected.Concat(document.Skip(25).Take(3).Cast<LeafBlock>().Select(lb => lb.Inline!.FirstChild!));
        yield return createTestCaseData(document, typeof(LiteralInline), 3, expected.ToArray());
    }

    /// <summary>
    /// Test cases for <see cref="MarkdownExtensionMethods.GetDescendantBranchesMatchingType(MarkdownObject?, IEnumerable{Type}, int)"/>.
    /// </summary>
    /// <returns></returns>
    public static System.Collections.IEnumerable GetGetDescendantBranchesMatchingType4TestData()
    {
        TestCaseData createTestCaseData(MarkdownObject? source, IEnumerable<Type> types, int maximumDepth, params MarkdownObject[] expected)
        {
            return new TestCaseData(source, types, maximumDepth)
                .Returns(expected.Select(o => new Tuple<Type, SourceSpan, int, int>(o.GetType(), o.Span, o.Line, o.Column)).ToArray())
                .SetArgDisplayNames((source is null) ? "null" : $"{source.GetType().Name} {{ {source.ToPositionText()}}}", $"[{string.Join(", ", types.Select(t => t.FullName))}]", maximumDepth.ToString());
        }
        MarkdownDocument document = GetMarkdownDocument();
        IEnumerable<MarkdownObject> expected = [((LeafBlock)document[0]).Inline!.FirstChild!, ((ContainerInline)((LeafBlock)document[1]).Inline!.FirstChild!).FirstChild!];
        var containerInline = ((LeafBlock)document[2]).Inline!;
        expected = expected.Concat([containerInline.FirstChild!, containerInline.LastChild!, ((LeafBlock)document[4]).Inline!.FirstChild!, ((LeafBlock)document[5]).Inline!.FirstChild!,
            ((LeafBlock)document[7]).Inline!.FirstChild!, ((LeafBlock)document[11]).Inline!.FirstChild!]);
        var inline = ((LeafBlock)document[12]).Inline!.FirstChild!;
        expected = expected.Concat([inline, inline.NextSibling!.NextSibling!]);
        inline = ((LeafBlock)document[13]).Inline!.FirstChild!;
        expected = expected.Concat([inline, inline.NextSibling!.NextSibling!, ((LeafBlock)document[14]).Inline!.FirstChild!, ((LeafBlock)document[15]).Inline!.FirstChild!,
            ((LeafBlock)document[17]).Inline!.FirstChild!]);
        containerInline = ((LeafBlock)document[18]).Inline!;
        expected = expected.Concat([containerInline.FirstChild!, containerInline.LastChild!]);
        expected = expected.Concat(document.Skip(21).Take(3).Cast<LeafBlock>().Select(lb => lb.Inline!.FirstChild!));
        containerInline = ((LeafBlock)document[24]).Inline!;
        inline = containerInline.FirstChild!;
        expected = expected.Concat([inline, inline.NextSibling!.NextSibling!, containerInline.LastChild!]);
        expected = expected.Concat(document.Skip(25).Take(3).Cast<LeafBlock>().Select(lb => lb.Inline!.FirstChild!));
        yield return createTestCaseData(document, [typeof(LiteralInline), typeof(EmphasisInline)], 2, expected.ToArray());
    }

    /// <summary>
    /// Test cases for <see cref="MarkdownExtensionMethods.GetDescendantsAtDepth(MarkdownObject?, int, bool)"/>.
    /// </summary>
    /// <returns></returns>
    public static System.Collections.IEnumerable GetGetDescendantsAtDepthTestData()
    {
        TestCaseData createTestCaseData(MarkdownObject? source, int minimumDepth, bool? inclAttr, params MarkdownObject[] expected)
        {
            return new TestCaseData(source, minimumDepth, inclAttr)
                .Returns(expected.Select(o => new Tuple<Type, SourceSpan, int, int>(o.GetType(), o.Span, o.Line, o.Column)).ToArray())
                .SetArgDisplayNames((source is null) ? "null" : $"{source.GetType().Name} {{ {source.ToPositionText()}}}", minimumDepth.ToString(), inclAttr?.ToString() ?? "null");
        }

        MarkdownDocument document = GetMarkdownDocument();
        ContainerBlock listBlock = (ContainerBlock)document[3];
        ContainerBlock listItemBlock0 = (ContainerBlock)listBlock[0];
        ContainerBlock listItemBlock1 = (ContainerBlock)listBlock[1];
        ContainerBlock listItemBlock2 = (ContainerBlock)listBlock[2];
        ContainerBlock listItemBlock3 = (ContainerBlock)listBlock[3];
        Inline inline0 = ((LinkInline)((ParagraphBlock)document[9]).Inline!.FirstChild!).FirstChild!;
        Inline mathInine = ((ParagraphBlock)document[13]).Inline!.FirstChild!.NextSibling!;
        ContainerInline emphasisInline = (ContainerInline)((LeafBlock)document[14]).Inline!.FirstChild!;
        ContainerBlock definitionList = (ContainerBlock)document[16];
        ContainerBlock definitionItem0 = (ContainerBlock)definitionList[0];
        ContainerBlock definitionItem1 = (ContainerBlock)definitionList[1];
        ContainerBlock footNoteGroup = (ContainerBlock)document[28];

        MarkdownObject[] expected = [
            // LiteralInline: CommonMark Spec
            ((ContainerInline)((LeafBlock)document[1]).Inline!.FirstChild!).FirstChild!,
            // ParagraphBlock: Task
            listItemBlock0[0],
            // ParagraphBlock: List Item
            listItemBlock1[0],
            // ParagraphBlock: Normal List
            listItemBlock2[0],
            // ParagraphBlock: Item
            listItemBlock3[0],
            // LiteralInline: Abbreviations Link
            ((ContainerInline)((LeafBlock)document[6]).Inline!.FirstChild!).FirstChild!,
            // LiteralInline: alt attribute goes here
            ((ContainerInline)((LeafBlock)document[8]).Inline!.FirstChild!).FirstChild!,
            // LiteralInline: foo
            inline0,
            // EmphasisInline: bar
            inline0.NextSibling!,
            // LiteralInline: The Cauchy-Schwarz Inequality
            emphasisInline.FirstChild!,
            // DefinitionTerm
            definitionItem0[0],
            // ParagraphBlock
            definitionItem0[1],
            // DefinitionTerm
            definitionItem1[0],
            // ParagraphBlock
            definitionItem1[1],
            ((ContainerBlock)footNoteGroup[0])[0],
            ((ContainerBlock)footNoteGroup[1])[0]
        ];
        yield return createTestCaseData(document, 3, null, expected);
        yield return createTestCaseData(document, 3, false, expected);
        yield return createTestCaseData(document, 3, true, [
            // LiteralInline: CommonMark Spec
            ((ContainerInline)((LeafBlock)document[1]).Inline!.FirstChild!).FirstChild!,
            // HtmlAttributes: class="task-list-item"
            listItemBlock0.GetAttributes(),
            // ParagraphBlock: Task
            listItemBlock0[0],
            // HtmlAttributes: class="task-list-item"
            listItemBlock1.GetAttributes(),
            // ParagraphBlock: List Item
            listItemBlock1[0],
            // HtmlAttributes: class="task-list-item"
            listItemBlock2.GetAttributes(),
            // ParagraphBlock: Normal List
            listItemBlock2[0],
            // HtmlAttributes: class="task-list-item"
            listItemBlock3.GetAttributes(),
            // ParagraphBlock: Item
            listItemBlock3[0],
            // LiteralInline: Abbreviations Link
            ((ContainerInline)((LeafBlock)document[6]).Inline!.FirstChild!).FirstChild!,
            // LiteralInline: alt attribute goes here
            ((ContainerInline)((LeafBlock)document[8]).Inline!.FirstChild!).FirstChild!,
            // LiteralInline: foo
            inline0,
            // EmphasisInline: bar
            inline0.NextSibling!,
            // class="math"
            ((ParagraphBlock)document[9]).Inline!.LastChild!.GetAttributes(),
            // class="math"
            mathInine.GetAttributes(),
            mathInine.NextSibling!.NextSibling!.GetAttributes(),
            // LiteralInline: The Cauchy-Schwarz Inequality
            emphasisInline.FirstChild!,
            // class="math"
            emphasisInline.NextSibling!.NextSibling!.GetAttributes(),
            // DefinitionTerm
            definitionItem0[0],
            // ParagraphBlock
            definitionItem0[1],
            // DefinitionTerm
            definitionItem1[0],
            // ParagraphBlock
            definitionItem1[1],
            // style="color: #333; color: #ff0000;"
            ((ParagraphBlock)document[22]).Inline!.LastChild!.GetAttributes(),
            ((ContainerBlock)footNoteGroup[0])[0],
            ((ContainerBlock)footNoteGroup[1])[0]
        ]);
    }

    /// <summary>
    /// Test cases for <see cref="MarkdownExtensionMethods.GetDescendantsUpToDepth(MarkdownObject?, int, bool)"/>.
    /// </summary>
    /// <returns></returns>
    public static System.Collections.IEnumerable GetGetDescendantsFromDepthTestData()
    {
        TestCaseData createTestCaseData(MarkdownObject? source, int minimumDepth, bool? includeAttributes, params MarkdownObject[] expected)
        {
            return new TestCaseData(source, minimumDepth, includeAttributes)
                .Returns(expected.Select(o => new Tuple<Type, SourceSpan, int, int>(o.GetType(), o.Span, o.Line, o.Column)).ToArray())
                .SetArgDisplayNames((source is null) ? "null" : $"{source.GetType().Name} {{ {source.ToPositionText()}}}", minimumDepth.ToString(), includeAttributes?.ToString() ?? "null");
        }
        MarkdownDocument document = GetMarkdownDocument();
        var inline0 = ((ContainerInline)((LeafBlock)document[1]).Inline!.FirstChild!).FirstChild!;
        var listBlock0 = (ContainerBlock)document[3];
        var containerBlock0 = (ContainerBlock)listBlock0[0];
        var leafBlock0 = (LeafBlock)containerBlock0[0];
        var containerBlock1 = (ContainerBlock)listBlock0[1];
        var leafBlock1 = (LeafBlock)containerBlock1[0];
        var containerBlock2 = (ContainerBlock)listBlock0[2];
        var leafBlock2 = (LeafBlock)containerBlock2[0];
        var containerBlock3 = (ContainerBlock)listBlock0[3];
        var leafBlock3 = (LeafBlock)containerBlock2[0];
        var inline1 = ((ContainerInline)((LeafBlock)document[6]).Inline!.FirstChild!).FirstChild!;
        var inline2 = ((ContainerInline)((LeafBlock)document[8]).Inline!.FirstChild!).FirstChild!;
        var inline3 = ((ContainerInline)((LeafBlock)document[9]).Inline!.FirstChild!).FirstChild!;
        var inline4 = inline3!.NextSibling!;
        var inline5 = ((ContainerInline)inline4).FirstChild!;
        var paragraph0 = ((LeafBlock)document[13]).Inline!;
        var paragraph1 = ((LeafBlock)document[14]).Inline!;
        var inline6 = ((ContainerInline)paragraph1.FirstChild!).FirstChild!;
        var leafBlocks0 = ((ContainerBlock)document[16]).Cast<ContainerBlock>().SelectMany(cb => cb.Cast<LeafBlock>());
        var leafBlocks1 = ((ContainerBlock)document[28]).Cast<ContainerBlock>().SelectMany(cb => cb.Cast<LeafBlock>());
        var expected = ((IEnumerable<MarkdownObject>)[inline0, leafBlock0, leafBlock0.Inline!.FirstChild!, leafBlock0.Inline!.LastChild!, leafBlock1, leafBlock1.Inline!.FirstChild!,
            leafBlock1.Inline!.LastChild!, leafBlock2, leafBlock2.Inline!.FirstChild!, leafBlock3, leafBlock3.Inline!.FirstChild!, inline1, inline2, inline3, inline4, inline5, inline6])
            .Concat(leafBlocks0.SelectMany(lb => ((IEnumerable<MarkdownObject>)[lb]).Concat(lb.Inline!))).Concat(leafBlocks1.SelectMany(lb => ((IEnumerable<MarkdownObject>)[lb]).Concat(lb.Inline!))).ToArray();
        yield return createTestCaseData(document, 3, null, expected);
        yield return createTestCaseData(document, 3, false, expected);
        yield return createTestCaseData(document, 3, true, ((IEnumerable<MarkdownObject>)[inline0, containerBlock0.GetAttributes(), leafBlock0, leafBlock0.Inline!.FirstChild!, leafBlock0.Inline!.LastChild!,
            containerBlock1.GetAttributes(), leafBlock1, leafBlock1.Inline!.FirstChild!, leafBlock1.Inline!.LastChild!, leafBlock2, leafBlock2.Inline!.FirstChild!, leafBlock3, leafBlock3.Inline!.FirstChild!,
            inline1, inline2, inline3, inline4, inline5, ((LeafBlock)document[12]).Inline!.LastChild!.GetAttributes(), paragraph0.FirstChild!.NextSibling!.GetAttributes(),
            paragraph0.LastChild!.GetAttributes(), inline6, paragraph1.LastChild!.GetAttributes()])
            .Concat(leafBlocks0.SelectMany(lb => ((IEnumerable<MarkdownObject>)[lb]).Concat(lb.Inline!)).Concat([((LeafBlock)document[22]).Inline!.LastChild!.GetAttributes()]))
            .Concat(leafBlocks1.SelectMany(lb => ((IEnumerable<MarkdownObject>)[lb]).Concat(lb.Inline!))).ToArray());
    }

    /// <summary>
    /// Test cases for <see cref="MarkdownExtensionMethods.GetDescendantsUpToDepth(MarkdownObject?, int, bool)"/>.
    /// </summary>
    /// <returns></returns>
    public static System.Collections.IEnumerable GetGetDescendantsUpToDepthTestData()
    {
        TestCaseData createTestCaseData(MarkdownObject? source, int maximumDepth, bool? includeAttributes, params MarkdownObject[] expected)
        {
            return new TestCaseData(source, maximumDepth, includeAttributes)
                .Returns(expected.Select(o => new Tuple<Type, SourceSpan, int, int>(o.GetType(), o.Span, o.Line, o.Column)).ToArray())
                .SetArgDisplayNames((source is null) ? "null" : $"{source.GetType().Name} {{ {source.ToPositionText()}}}", maximumDepth.ToString(), includeAttributes?.ToString() ?? "null");
        }
        throw new NotImplementedException();
    }

    /// <summary>
    /// Test cases for <see cref="MarkdownExtensionMethods.GetDescendantsInDepthRange(MarkdownObject?, int, int, bool)"/>.
    /// </summary>
    /// <returns></returns>
    public static System.Collections.IEnumerable GetGetDescendantsInDepthRangeTestData()
    {
        TestCaseData createTestCaseData(MarkdownObject? source, int minimumDepth, int maximumDepth, bool? includeAttributes, params MarkdownObject[] expected)
        {
            return new TestCaseData(source, minimumDepth, maximumDepth, includeAttributes)
                .Returns(expected.Select(o => new Tuple<Type, SourceSpan, int, int>(o.GetType(), o.Span, o.Line, o.Column)).ToArray())
                .SetArgDisplayNames((source is null) ? "null" : $"{source.GetType().Name} {{ {source.ToPositionText()}}}", minimumDepth.ToString(), maximumDepth.ToString(), includeAttributes?.ToString() ?? "null");
        }
        throw new NotImplementedException();
    }

    /// <summary>
    /// Test cases for <see cref="MarkdownExtensionMethods.GetDescendantBranchesMatchingType(MarkdownObject?, Type, int, int)"/>.
    /// </summary>
    /// <returns></returns>
    public static System.Collections.IEnumerable GetGetDescendantBranchesMatchingType5TestData()
    {
        TestCaseData createTestCaseData(MarkdownObject? source, Type type, int minimumDepth, int maximumDepth, params MarkdownObject[] expected)
        {
            return new TestCaseData(source, type, maximumDepth)
                .Returns(expected.Select(o => new Tuple<Type, SourceSpan, int, int>(o.GetType(), o.Span, o.Line, o.Column)).ToArray())
                .SetArgDisplayNames((source is null) ? "null" : $"{source.GetType().Name} {{ {source.ToPositionText()}}}", type.FullName!, maximumDepth.ToString());
        }
        throw new NotImplementedException();
    }

    /// <summary>
    /// Test cases for <see cref="MarkdownExtensionMethods.GetDescendantBranchesMatchingType(MarkdownObject?, IEnumerable{Type}, int, int)"/>.
    /// </summary>
    /// <returns></returns>
    public static System.Collections.IEnumerable GetGetDescendantBranchesMatchingType6TestData()
    {
        TestCaseData createTestCaseData(MarkdownObject? source, IEnumerable<Type> types, int minimumDepth, int maximumDepth, params MarkdownObject[] expected)
        {
            return new TestCaseData(source, types, maximumDepth)
                .Returns(expected.Select(o => new Tuple<Type, SourceSpan, int, int>(o.GetType(), o.Span, o.Line, o.Column)).ToArray())
                .SetArgDisplayNames((source is null) ? "null" : $"{source.GetType().Name} {{ {source.ToPositionText()}}}", $"[{string.Join(", ", types.Select(t => t.FullName))}]", maximumDepth.ToString());
        }
        throw new NotImplementedException();
    }
}