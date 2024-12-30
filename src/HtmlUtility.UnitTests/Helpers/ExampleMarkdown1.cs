using Markdig;
using Markdig.Renderers.Html;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace HtmlUtility.UnitTests.Helpers;

public static partial class ExampleMarkdown1
{
    internal const string SourceFileName = "Example1.md";
    internal const string JsonTestOutputFileName = "Example1.json";

    internal static string GetSourcePath() => Path.Combine(TestHelper.GetResourcesDirectoryPath(), SourceFileName);

    internal static string GetMarkdownSourceText() => File.ReadAllText(GetSourcePath());

    internal static MarkdownDocument GetMarkdownDocument() => Markdown.Parse(GetMarkdownSourceText(), new MarkdownPipelineBuilder().UseAdvancedExtensions().Build());

    // static Tuple<Type, SourceSpan> ToReturnsTuple(MarkdownObject obj)
    // {
    //     return new Tuple<Type, SourceSpan>(obj.GetType(), obj.Span);
    // }

    /// <summary>
    /// Test cases for <see cref="MarkdownExtensionMethods.GetDescendantBranchesMatchingType(MarkdownObject?, Type)"/>.
    /// </summary>
    /// <returns></returns>
    public static System.Collections.IEnumerable GetGetDescendantBranchesMatchingType1TestData()
    {
        MarkdownDocument document = GetMarkdownDocument();
        var elements = new MarkdownElements(document);
        IEnumerable<MarkdownObject> expected = [elements.Element0, elements.Element1, elements.Element2, elements.Element3, elements.Element4, elements.Element5, elements.Element6, elements.Element7, elements.Element8, elements.Element9,
            elements.Element10, elements.Element11, elements.Element12, elements.Element13, elements.Element14, elements.Element15, elements.Element16, elements.Element17, elements.Element18, elements.Element19, elements.Element20, elements.Element21,
            elements.Element22, elements.Element23, elements.Element24, elements.Element25, elements.Element26, elements.Element27, elements.Element28, elements.Element29, elements.Element30, elements.Element31, elements.Element32, elements.Element33,
            elements.Element34, elements.Element35, elements.Element36, elements.Element37];
        yield return new TestCaseData(document, typeof(MarkdownObject)).Returns(expected).SetArgDisplayNames("Document", "MarkdownObject");
    }

    /// <summary>
    /// Test cases for <see cref="MarkdownExtensionMethods.GetDescendantBranchesMatchingType(MarkdownObject?, IEnumerable{Type})"/>.
    /// </summary>
    /// <returns></returns>
    public static System.Collections.IEnumerable GetGetDescendantBranchesMatchingType2TestData()
    {
        MarkdownDocument document = GetMarkdownDocument();
        var elements = new MarkdownElements(document);
        IEnumerable<Type> types = [typeof(ContainerInline), typeof(ContainerBlock)];
        IEnumerable<MarkdownObject> expected = [elements.Element3_0, elements.Element5_5, elements.Element8_0, elements.Element9_1, elements.Element10_4, elements.Element13, elements.Element14, elements.Element15, elements.Element16, elements.Element17_1,
            elements.Element18, elements.Element19, elements.Element21, elements.Element22, elements.Element23_0, elements.Element24_1, elements.Element24_3, elements.Element26_0, elements.Element29_0, elements.Element29_2, elements.Element34,
            elements.Element35, elements.Element36, elements.Element37];
        yield return new TestCaseData(document, types).Returns(expected).SetArgDisplayNames("Document", $"[{string.Join(", ", types.Select(t => t.Name))}]");
    }
/// 
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
        // TODO: This is not valid
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
        // TODO: This is not valid
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
        // TODO: This is not valid
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
            // Markdig.Extensions.DefinitionLists.DefinitionTerm
            definitionItem0[0],
            // ParagraphBlock
            definitionItem0[1],
            // Markdig.Extensions.DefinitionLists.DefinitionTerm
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
            // Markdig.Extensions.DefinitionLists.DefinitionTerm
            definitionItem0[0],
            // ParagraphBlock
            definitionItem0[1],
            // Markdig.Extensions.DefinitionLists.DefinitionTerm
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
        // TODO: This is not valid
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