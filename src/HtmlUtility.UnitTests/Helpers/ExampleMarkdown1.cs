using System.ComponentModel;
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

    static readonly Tuple<Type, SourceSpan>[] ReturnsEmpty = [];

    static Tuple<Type, SourceSpan> ToReturnsTuple(MarkdownObject obj)
    {
        return new Tuple<Type, SourceSpan>(obj.GetType(), obj.Span);
    }

    /// <summary>
    /// Test cases for <see cref="MarkdownExtensionMethods.GetChildObjects(MarkdownObject?, bool)"/>.
    /// </summary>
    /// <returns></returns>
    public static System.Collections.IEnumerable GetGetChildObjectsTestData()
    {
        MarkdownDocument document = GetMarkdownDocument();
        // TestHelper.AddMarkdownJsonTestAttachment(document, SourceFileName, JsonTestOutputFileName);

        var elements = new MarkdownElements(document);
        var expected = ((IEnumerable<MarkdownObject>)[elements.Element0, elements.Element1, elements.Element2, elements.Element3, elements.Element4, elements.Element5,
            elements.Element6, elements.Element7, elements.Element8, elements.Element9, elements.Element10, elements.Element11, elements.Element12, elements.Element13,
            elements.Element14, elements.Element15, elements.Element16, elements.Element17, elements.Element18, elements.Element19, elements.Element20,
            elements.Element21, elements.Element22, elements.Element23, elements.Element24, elements.Element25, elements.Element26, elements.Element27,
            elements.Element28]).Select(ToReturnsTuple).ToArray();
        yield return new TestCaseData(document, null).Returns(expected).SetArgDisplayNames("Document", "null");
        yield return new TestCaseData(document, false).Returns(expected).SetArgDisplayNames("Document", "false");
        yield return new TestCaseData(document, true).Returns(((IEnumerable<MarkdownObject>)[elements.Element0, elements.Element0_Attributes, elements.Element1,
            elements.Element2, elements.Element3, elements.Element3_Attributes, elements.Element4, elements.Element4_Attributes, elements.Element5,
            elements.Element5_Attributes, elements.Element6, elements.Element7, elements.Element7_Attributes, elements.Element8, elements.Element9, elements.Element10,
            elements.Element11, elements.Element11_Attributes, elements.Element12, elements.Element13, elements.Element14, elements.Element15,
            elements.Element15_Attributes, elements.Element16, elements.Element17, elements.Element17_Attributes, elements.Element18, elements.Element19,
            elements.Element19_Attributes, elements.Element20, elements.Element20_Attributes, elements.Element21, elements.Element21_Attributes, elements.Element22,
            elements.Element23, elements.Element23_Attributes, elements.Element24, elements.Element25, elements.Element25_Attributes, elements.Element26,
            elements.Element27, elements.Element28]).Select(ToReturnsTuple).ToArray()).SetArgDisplayNames("Document", "true");

        expected = [ToReturnsTuple(elements.Element0_0)];
        yield return new TestCaseData(elements.Element0, null).Returns(expected).SetArgDisplayNames("(HeadingBlock)Document[0]", "null");
        yield return new TestCaseData(elements.Element0, false).Returns(expected).SetArgDisplayNames("(HeadingBlock)Document[0]", "false");
        yield return new TestCaseData(elements.Element0, true)
            .Returns(((IEnumerable<MarkdownObject>)[elements.Element0_Attributes, elements.Element0_0]).Select(ToReturnsTuple).ToArray())
            .SetArgDisplayNames("(HeadingBlock)Document[0]", "true");

        yield return new TestCaseData(elements.Element0_0, null).Returns(ReturnsEmpty).SetArgDisplayNames("(LiteralInline)Document[0][0]", "null");
        yield return new TestCaseData(elements.Element0_0, false).Returns(ReturnsEmpty).SetArgDisplayNames("(LiteralInline)Document[0][0]", "false");
        yield return new TestCaseData(elements.Element0_0, true).Returns(ReturnsEmpty).SetArgDisplayNames("(LiteralInline)Document[0][0]", "true");

        expected = [ToReturnsTuple(elements.Element1_0)];
        yield return new TestCaseData(elements.Element1, null).Returns(expected).SetArgDisplayNames("(ParagraphBlock)Document[1]", "null");
        yield return new TestCaseData(elements.Element1, false).Returns(expected).SetArgDisplayNames("(ParagraphBlock)Document[1]", "false");
        yield return new TestCaseData(elements.Element1, true).Returns(expected).SetArgDisplayNames("(ParagraphBlock)Document[1]", "true");

        expected = [ToReturnsTuple(elements.Element1_0_0)];
        yield return new TestCaseData(elements.Element1_0, null).Returns(expected).SetArgDisplayNames("(LinkInline)Document[1]", "null");
        yield return new TestCaseData(elements.Element1_0, false).Returns(expected).SetArgDisplayNames("(LinkInline)Document[1][0]", "false");
        yield return new TestCaseData(elements.Element1_0, true).Returns(expected).SetArgDisplayNames("(LinkInline)Document[1][0]", "true");

        expected = ((IEnumerable<MarkdownObject>)[elements.Element2_0, elements.Element2_1, elements.Element2_2]).Select(ToReturnsTuple).ToArray();
        yield return new TestCaseData(elements.Element2, null).Returns(expected).SetArgDisplayNames("(ParagraphBlock)Document[2]", "null");
        yield return new TestCaseData(elements.Element2, false).Returns(expected).SetArgDisplayNames("(ParagraphBlock)Document[2]", "false");
        yield return new TestCaseData(elements.Element2, true).Returns(expected).SetArgDisplayNames("(ParagraphBlock)Document[2]", "true");

        expected = ((IEnumerable<MarkdownObject>)[elements.Element3_0, elements.Element3_1, elements.Element3_2, elements.Element3_3]).Select(ToReturnsTuple).ToArray();
        yield return new TestCaseData(elements.Element3, null).Returns(expected).SetArgDisplayNames("(ListBlock)Document[3]", "null");
        yield return new TestCaseData(elements.Element3, false).Returns(expected).SetArgDisplayNames("(ListBlock)Document[3]", "false");
        yield return new TestCaseData(elements.Element3, true)
            .Returns(((IEnumerable<MarkdownObject>)[elements.Element3_Attributes, elements.Element3_0, elements.Element3_1, elements.Element3_2, elements.Element3_3]).Select(ToReturnsTuple).ToArray())
            .SetArgDisplayNames("(ListBlock)Document[3]", "true");

        expected = ((IEnumerable<MarkdownObject>)[elements.Element3_0_0]).Select(ToReturnsTuple).ToArray();
        yield return new TestCaseData(elements.Element3_0, null).Returns(expected).SetArgDisplayNames("(ListItemBlock)Document[3][0]", "null");
        yield return new TestCaseData(elements.Element3_0, false).Returns(expected).SetArgDisplayNames("(ListItemBlock)Document[3][0]", "false");
        yield return new TestCaseData(elements.Element3_0, true)
            .Returns(((IEnumerable<MarkdownObject>)[elements.Element3_0_Attributes, elements.Element3_0_0]).Select(ToReturnsTuple).ToArray())
            .SetArgDisplayNames("(ListItemBlock)Document[3][0]", "true");

        expected = ((IEnumerable<MarkdownObject>)[elements.Element3_0_0_0, elements.Element3_0_0_1]).Select(ToReturnsTuple).ToArray();
        yield return new TestCaseData(elements.Element3_0_0, null).Returns(expected).SetArgDisplayNames("(ParagraphBlock)Document[3][0][0]", "null");
        yield return new TestCaseData(elements.Element3_0_0, false).Returns(expected).SetArgDisplayNames("(ParagraphBlock)Document[3][0][0]", "false");
        yield return new TestCaseData(elements.Element3_0_0, true).Returns(expected).SetArgDisplayNames("(ParagraphBlock)Document[3][0][0]", "true");

        expected = ((IEnumerable<MarkdownObject>)[elements.Element8_0_0]).Select(ToReturnsTuple).ToArray();
        yield return new TestCaseData(elements.Element8_0, null).Returns(expected).SetArgDisplayNames("(LinkInline)Document[8][0]", "null");
        yield return new TestCaseData(elements.Element8_0, false).Returns(expected).SetArgDisplayNames("(LinkInline)Document[8][0]", "false");
        yield return new TestCaseData(elements.Element8_0, true).Returns(expected).SetArgDisplayNames("(LinkInline)Document[8][0]", "true");

        expected = ((IEnumerable<MarkdownObject>)[elements.Element9_0_0, elements.Element9_0_1]).Select(ToReturnsTuple).ToArray();
        yield return new TestCaseData(elements.Element9_0, null).Returns(expected).SetArgDisplayNames("(LinkInline)Document[9][0]", "null");
        yield return new TestCaseData(elements.Element9_0, false).Returns(expected).SetArgDisplayNames("(LinkInline)Document[9][0]", "false");
        yield return new TestCaseData(elements.Element9_0, true).Returns(expected).SetArgDisplayNames("(LinkInline)Document[9][0]", "true");

        expected = ((IEnumerable<MarkdownObject>)[elements.Element9_0_1_0]).Select(ToReturnsTuple).ToArray();
        yield return new TestCaseData(elements.Element9_0_1, null).Returns(expected).SetArgDisplayNames("(EmphasisInline)Document[9][0][1]", "null");
        yield return new TestCaseData(elements.Element9_0_1, false).Returns(expected).SetArgDisplayNames("(EmphasisInline)Document[9][0][1]", "false");
        yield return new TestCaseData(elements.Element9_0_1, true).Returns(expected).SetArgDisplayNames("(EmphasisInline)Document[9][0][1]", "true");

        expected = ((IEnumerable<MarkdownObject>)[elements.Element10_1, elements.Element10_2, elements.Element10_3, elements.Element10_4, elements.Element10_5, elements.Element10_6, elements.Element10_7,
            elements.Element10_8, elements.Element10_9, elements.Element10_10, elements.Element10_11, elements.Element10_12]).Select(ToReturnsTuple).ToArray();
        yield return new TestCaseData(elements.Element10, null).Returns(expected).SetArgDisplayNames("(LinkReferenceDefinition)Document[10]", "null");
        yield return new TestCaseData(elements.Element10, false).Returns(expected).SetArgDisplayNames("(LinkReferenceDefinition)Document[10]", "false");
        yield return new TestCaseData(elements.Element10, true).Returns(expected).SetArgDisplayNames("(LinkReferenceDefinition)Document[10]", "true");

        expected = ((IEnumerable<MarkdownObject>)[elements.Element16_0, elements.Element16_1]).Select(ToReturnsTuple).ToArray();
        yield return new TestCaseData(elements.Element16, null).Returns(expected).SetArgDisplayNames("(DefinitionList)Document[16]", "null");
        yield return new TestCaseData(elements.Element16, false).Returns(expected).SetArgDisplayNames("(DefinitionList)Document[16]", "false");
        yield return new TestCaseData(elements.Element16, true).Returns(expected).SetArgDisplayNames("(DefinitionList)Document[16]", "true");

        expected = ((IEnumerable<MarkdownObject>)[elements.Element16_0_0, elements.Element16_0_1]).Select(ToReturnsTuple).ToArray();
        yield return new TestCaseData(elements.Element16_0, null).Returns(expected).SetArgDisplayNames("(DefinitionItem)Document[16][0]", "null");
        yield return new TestCaseData(elements.Element16_0, false).Returns(expected).SetArgDisplayNames("(DefinitionItem)Document[16][0]", "false");
        yield return new TestCaseData(elements.Element16_0, true).Returns(expected).SetArgDisplayNames("(DefinitionItem)Document[16][0]", "true");

        expected = ((IEnumerable<MarkdownObject>)[elements.Element28_0, elements.Element28_1]).Select(ToReturnsTuple).ToArray();
        yield return new TestCaseData(elements.Element28, null).Returns(expected).SetArgDisplayNames("(FootnoteGroup)Document[16][0]", "null");
        yield return new TestCaseData(elements.Element28, false).Returns(expected).SetArgDisplayNames("(FootnoteGroup)Document[16][0]", "false");
        yield return new TestCaseData(elements.Element28, true).Returns(expected).SetArgDisplayNames("(FootnoteGroup)Document[16][0]", "true");

        yield return new TestCaseData(elements.Element2_0, null).Returns(ReturnsEmpty).SetArgDisplayNames("(LiteralInline)Document[2][0]", "null");
        yield return new TestCaseData(elements.Element2_0, false).Returns(ReturnsEmpty).SetArgDisplayNames("(LiteralInline)Document[2][0]", "false");
        yield return new TestCaseData(elements.Element2_0, true).Returns(ReturnsEmpty).SetArgDisplayNames("(LiteralInline)Document[2][0]", "true");

        yield return new TestCaseData(elements.Element10_1, null).Returns(ReturnsEmpty).SetArgDisplayNames("(FootnoteLinkReferenceDefinition)Document[10][1]", "null");
        yield return new TestCaseData(elements.Element10_1, false).Returns(ReturnsEmpty).SetArgDisplayNames("(FootnoteLinkReferenceDefinition)Document[10][1]", "false");
        yield return new TestCaseData(elements.Element10_1, true).Returns(ReturnsEmpty).SetArgDisplayNames("(FootnoteLinkReferenceDefinition)Document[10][1]", "true");

        yield return new TestCaseData(elements.Element10_3, null).Returns(ReturnsEmpty).SetArgDisplayNames("(HeadingLinkReferenceDefinition)Document[10][1]", "null");
        yield return new TestCaseData(elements.Element10_3, false).Returns(ReturnsEmpty).SetArgDisplayNames("(HeadingLinkReferenceDefinition)Document[10][1]", "false");
        yield return new TestCaseData(elements.Element10_3, true).Returns(ReturnsEmpty).SetArgDisplayNames("(HeadingLinkReferenceDefinition)Document[10][1]", "true");

        yield return new TestCaseData(elements.Element12_3, null).Returns(ReturnsEmpty).SetArgDisplayNames("(MathInline)Document[12][3]", "null");
        yield return new TestCaseData(elements.Element12_3, false).Returns(ReturnsEmpty).SetArgDisplayNames("(MathInline)Document[12][3]", "false");
        yield return new TestCaseData(elements.Element12_3, true)
            .Returns(((IEnumerable<MarkdownObject>)[elements.Element12_3_Attributes]).Select(ToReturnsTuple).ToArray())
            .SetArgDisplayNames("(MathInline)Document[12][3]", "true");

        yield return new TestCaseData(elements.Element18_1, null).Returns(ReturnsEmpty).SetArgDisplayNames("(CodeInline)Document[18][1]", "null");
        yield return new TestCaseData(elements.Element18_1, false).Returns(ReturnsEmpty).SetArgDisplayNames("(CodeInline)Document[18][1]", "false");
        yield return new TestCaseData(elements.Element18_1, true).Returns(ReturnsEmpty).SetArgDisplayNames("(CodeInline)Document[18][1]", "true");

        yield return new TestCaseData(elements.Element18_1, null).Returns(ReturnsEmpty).SetArgDisplayNames("(FootnoteLink)Document[24][1]", "null");
        yield return new TestCaseData(elements.Element18_1, false).Returns(ReturnsEmpty).SetArgDisplayNames("(FootnoteLink)Document[24][1]", "false");
        yield return new TestCaseData(elements.Element18_1, true).Returns(ReturnsEmpty).SetArgDisplayNames("(FootnoteLink)Document[24][1]", "true");

        yield return new TestCaseData(elements.Element28_0_0_1, null).Returns(ReturnsEmpty).SetArgDisplayNames("(FootnoteLink)Document[28][0][0][1]", "null");
        yield return new TestCaseData(elements.Element28_0_0_1, false).Returns(ReturnsEmpty).SetArgDisplayNames("(FootnoteLink)Document[28][0][0][1]", "false");
        yield return new TestCaseData(elements.Element28_0_0_1, true).Returns(ReturnsEmpty).SetArgDisplayNames("(FootnoteLink)Document[28][0][0][1]", "true");
    }

    /// <summary>
    /// Test cases for <see cref="MarkdownExtensionMethods.GetAllDescendants(MarkdownObject?, bool)"/>.
    /// </summary>
    /// <returns></returns>
    public static System.Collections.IEnumerable GetGetAllDescendantsTestData()
    {
        MarkdownDocument document = GetMarkdownDocument();
        var elements = new MarkdownElements(document);
        IEnumerable<MarkdownObject> expected = [elements.Element0_0];
        IEnumerable<MarkdownObject> withAttrReturns = [elements.Element0_Attributes, elements.Element0_0];
        yield return new TestCaseData(elements.Element0, null).Returns(expected.Select(ToReturnsTuple).ToArray()).SetArgDisplayNames("(HeadingBlock)Document[0]", "null");
        yield return new TestCaseData(elements.Element0, false).Returns(expected.Select(ToReturnsTuple).ToArray()).SetArgDisplayNames("(HeadingBlock)Document[0]", "false");
        yield return new TestCaseData(elements.Element0, true).Returns(withAttrReturns.Select(ToReturnsTuple).ToArray()).SetArgDisplayNames("(HeadingBlock)Document[0]", "true");

        expected = [elements.Element2_0, elements.Element2_1, elements.Element2_2];
        yield return new TestCaseData(elements.Element2, null).Returns(expected.Select(ToReturnsTuple).ToArray()).SetArgDisplayNames("(ParagraphBlock)Document[2]", "null");
        yield return new TestCaseData(elements.Element2, false).Returns(expected.Select(ToReturnsTuple).ToArray()).SetArgDisplayNames("(ParagraphBlock)Document[2]", "false");
        yield return new TestCaseData(elements.Element2, true).Returns(expected.Select(ToReturnsTuple).ToArray()).SetArgDisplayNames("(ParagraphBlock)Document[2]", "true");

        expected = [elements.Element3_0, elements.Element3_0_0, elements.Element3_0_0_0, elements.Element3_0_0_1, elements.Element3_1, elements.Element3_1_0, elements.Element3_1_0_0, elements.Element3_1_0_1,
            elements.Element3_2, elements.Element3_2_0, elements.Element3_2_0_0, elements.Element3_3, elements.Element3_3_0, elements.Element3_3_0_0];
        withAttrReturns = [elements.Element3_Attributes, elements.Element3_0, elements.Element3_0_Attributes, elements.Element3_0_0, elements.Element3_0_0_0, elements.Element3_0_0_1, elements.Element3_1,
            elements.Element3_1_Attributes, elements.Element3_1_0, elements.Element3_1_0_0, elements.Element3_1_0_1, elements.Element3_2, elements.Element3_2_0, elements.Element3_2_0_0, elements.Element3_3,
            elements.Element3_3_0, elements.Element3_3_0_0];
        yield return new TestCaseData(elements.Element3, null).Returns(expected.Select(ToReturnsTuple).ToArray()).SetArgDisplayNames("(ListBlock)Document[3]", "null");
        yield return new TestCaseData(elements.Element3, false).Returns(expected.Select(ToReturnsTuple).ToArray()).SetArgDisplayNames("(ListBlock)Document[3]", "false");
        yield return new TestCaseData(elements.Element3, true).Returns(withAttrReturns.Select(ToReturnsTuple).ToArray()).SetArgDisplayNames("(ListBlock)Document[3]", "true");

        expected = [elements.Element3_0_0, elements.Element3_0_0_0, elements.Element3_0_0_1];
        withAttrReturns = [elements.Element3_0_Attributes, elements.Element3_0_0, elements.Element3_0_0_0, elements.Element3_0_0_1];
        yield return new TestCaseData(elements.Element3_0, null).Returns(expected.Select(ToReturnsTuple).ToArray()).SetArgDisplayNames("(ListItemBlock)Document[3][0]", "null");
        yield return new TestCaseData(elements.Element3_0, false).Returns(expected.Select(ToReturnsTuple).ToArray()).SetArgDisplayNames("(ListItemBlock)Document[3][0]", "false");
        yield return new TestCaseData(elements.Element3_0, true).Returns(withAttrReturns.Select(ToReturnsTuple).ToArray()).SetArgDisplayNames("(ListItemBlock)Document[3][0]", "true");

        expected = [elements.Element6_0, elements.Element6_0_0];
        yield return new TestCaseData(elements.Element6, null).Returns(expected.Select(ToReturnsTuple).ToArray()).SetArgDisplayNames("(ParagraphBlock)Document[6]", "null");
        yield return new TestCaseData(elements.Element6, false).Returns(expected.Select(ToReturnsTuple).ToArray()).SetArgDisplayNames("(ParagraphBlock)Document[6]", "false");
        yield return new TestCaseData(elements.Element6, true).Returns(expected.Select(ToReturnsTuple).ToArray()).SetArgDisplayNames("(ParagraphBlock)Document[6]", "true");

        expected = [elements.Element6_0_0];
        yield return new TestCaseData(elements.Element6_0, null).Returns(expected.Select(ToReturnsTuple).ToArray()).SetArgDisplayNames("(LinkInline)Document[6][0]", "null");
        yield return new TestCaseData(elements.Element6_0, false).Returns(expected.Select(ToReturnsTuple).ToArray()).SetArgDisplayNames("(LinkInline)Document[6][0]", "false");
        yield return new TestCaseData(elements.Element6_0, true).Returns(expected.Select(ToReturnsTuple).ToArray()).SetArgDisplayNames("(LinkInline)Document[6][0]", "true");

        expected = [elements.Element9_0, elements.Element9_0_0, elements.Element9_0_1, elements.Element9_0_1_0];
        yield return new TestCaseData(elements.Element9, null).Returns(expected.Select(ToReturnsTuple).ToArray()).SetArgDisplayNames("(ParagraphBlock)Document[9]", "null");
        yield return new TestCaseData(elements.Element9, false).Returns(expected.Select(ToReturnsTuple).ToArray()).SetArgDisplayNames("(ParagraphBlock)Document[9]", "false");
        yield return new TestCaseData(elements.Element9, true).Returns(expected.Select(ToReturnsTuple).ToArray()).SetArgDisplayNames("(ParagraphBlock)Document[9]", "true");

        expected = [elements.Element9_0_0, elements.Element9_0_1, elements.Element9_0_1_0];
        yield return new TestCaseData(elements.Element9_0, null).Returns(expected.Select(ToReturnsTuple).ToArray()).SetArgDisplayNames("(LinkInline)Document[9][0]", "null");
        yield return new TestCaseData(elements.Element9_0, false).Returns(expected.Select(ToReturnsTuple).ToArray()).SetArgDisplayNames("(LinkInline)Document[9][0]", "false");
        yield return new TestCaseData(elements.Element9_0, true).Returns(expected.Select(ToReturnsTuple).ToArray()).SetArgDisplayNames("(LinkInline)Document[9][0]", "true");

        expected = [elements.Element10_0, elements.Element10_1, elements.Element10_2, elements.Element10_3, elements.Element10_4, elements.Element10_5, elements.Element10_6, elements.Element10_7,
            elements.Element10_8, elements.Element10_9, elements.Element10_10, elements.Element10_11, elements.Element10_12];
        yield return new TestCaseData(elements.Element10, null).Returns(expected.Select(ToReturnsTuple).ToArray()).SetArgDisplayNames("(LinkReferenceDefinitionGroup)Document[10]", "null");
        yield return new TestCaseData(elements.Element10, false).Returns(expected.Select(ToReturnsTuple).ToArray()).SetArgDisplayNames("(LinkReferenceDefinitionGroup)Document[10]", "false");
        yield return new TestCaseData(elements.Element10, true).Returns(expected.Select(ToReturnsTuple).ToArray()).SetArgDisplayNames("(LinkReferenceDefinitionGroup)Document[10]", "true");

        expected = [elements.Element12_0, elements.Element12_1, elements.Element12_2, elements.Element12_3];
        withAttrReturns = [elements.Element12_0, elements.Element12_1, elements.Element12_2, elements.Element12_3, elements.Element12_3_Attributes];
        yield return new TestCaseData(elements.Element12, null).Returns(expected.Select(ToReturnsTuple).ToArray()).SetArgDisplayNames("(ParagraphBlock)Document[12]", "null");
        yield return new TestCaseData(elements.Element12, false).Returns(expected.Select(ToReturnsTuple).ToArray()).SetArgDisplayNames("(ParagraphBlock)Document[12]", "false");
        yield return new TestCaseData(elements.Element12, true).Returns(withAttrReturns.Select(ToReturnsTuple).ToArray()).SetArgDisplayNames("(ParagraphBlock)Document[12]", "true");

        expected = [elements.Element16_0, elements.Element16_0_0, elements.Element16_0_0_0, elements.Element16_0_1, elements.Element16_0_1_0, elements.Element16_0_1_1, elements.Element16_0_1_2, elements.Element16_1,
            elements.Element16_1_0, elements.Element16_1_0_0, elements.Element16_1_1, elements.Element16_1_1_0];
        yield return new TestCaseData(elements.Element16, null).Returns(expected.Select(ToReturnsTuple).ToArray()).SetArgDisplayNames("(DefinitionList)Document[16]", "null");
        yield return new TestCaseData(elements.Element16, false).Returns(expected.Select(ToReturnsTuple).ToArray()).SetArgDisplayNames("(DefinitionList)Document[16]", "false");
        yield return new TestCaseData(elements.Element16, true).Returns(expected.Select(ToReturnsTuple).ToArray()).SetArgDisplayNames("(DefinitionList)Document[16]", "true");

        expected = [elements.Element16_0_0, elements.Element16_0_0_0, elements.Element16_0_1, elements.Element16_0_1_0, elements.Element16_0_1_1, elements.Element16_0_1_2];
        yield return new TestCaseData(elements.Element16_0, null).Returns(expected.Select(ToReturnsTuple).ToArray()).SetArgDisplayNames("(DefinitionItem)Document[16][0]", "null");
        yield return new TestCaseData(elements.Element16_0, false).Returns(expected.Select(ToReturnsTuple).ToArray()).SetArgDisplayNames("(DefinitionItem)Document[16][0]", "false");
        yield return new TestCaseData(elements.Element16_0, true).Returns(expected.Select(ToReturnsTuple).ToArray()).SetArgDisplayNames("(DefinitionItem)Document[16][0]", "true");

        expected = [elements.Element16_0_0_0];
        yield return new TestCaseData(elements.Element16_0_0, null).Returns(expected.Select(ToReturnsTuple).ToArray()).SetArgDisplayNames("(DefinitionTerm)Document[16][0][0]", "null");
        yield return new TestCaseData(elements.Element16_0_0, false).Returns(expected.Select(ToReturnsTuple).ToArray()).SetArgDisplayNames("(DefinitionTerm)Document[16][0][0]", "false");
        yield return new TestCaseData(elements.Element16_0_0, true).Returns(expected.Select(ToReturnsTuple).ToArray()).SetArgDisplayNames("(DefinitionTerm)Document[16][0][0]", "true");

        expected = [elements.Element16_0_1_0, elements.Element16_0_1_1, elements.Element16_0_1_2];
        yield return new TestCaseData(elements.Element16_0_1, null).Returns(expected.Select(ToReturnsTuple).ToArray()).SetArgDisplayNames("(ParagraphBlock)Document[16][0][1]", "null");
        yield return new TestCaseData(elements.Element16_0_1, false).Returns(expected.Select(ToReturnsTuple).ToArray()).SetArgDisplayNames("(ParagraphBlock)Document[16][0][1]", "false");
        yield return new TestCaseData(elements.Element16_0_1, true).Returns(expected.Select(ToReturnsTuple).ToArray()).SetArgDisplayNames("(ParagraphBlock)Document[16][0][1]", "true");

        expected = [elements.Element16_1_0, elements.Element16_1_0_0, elements.Element16_1_1, elements.Element16_1_1_0];
        yield return new TestCaseData(elements.Element16_1, null).Returns(expected.Select(ToReturnsTuple).ToArray()).SetArgDisplayNames("(DefinitionItem)Document[16][1]", "null");
        yield return new TestCaseData(elements.Element16_1, false).Returns(expected.Select(ToReturnsTuple).ToArray()).SetArgDisplayNames("(DefinitionItem)Document[16][1]", "false");
        yield return new TestCaseData(elements.Element16_1, true).Returns(expected.Select(ToReturnsTuple).ToArray()).SetArgDisplayNames("(DefinitionItem)Document[16][1]", "true");

        expected = [elements.Element18_0, elements.Element18_1, elements.Element18_2];
        yield return new TestCaseData(elements.Element18, null).Returns(expected.Select(ToReturnsTuple).ToArray()).SetArgDisplayNames("(ParagraphBlock)Document[18]", "null");
        yield return new TestCaseData(elements.Element18, false).Returns(expected.Select(ToReturnsTuple).ToArray()).SetArgDisplayNames("(ParagraphBlock)Document[18]", "false");
        yield return new TestCaseData(elements.Element18, true).Returns(expected.Select(ToReturnsTuple).ToArray()).SetArgDisplayNames("(ParagraphBlock)Document[18]", "true");

        expected = [elements.Element22_0, elements.Element22_1];
        withAttrReturns = [elements.Element22_0, elements.Element22_1, elements.Element22_1_Attributes];
        yield return new TestCaseData(elements.Element22, null).Returns(expected.Select(ToReturnsTuple).ToArray()).SetArgDisplayNames("(ParagraphBlock)Document[22]", "null");
        yield return new TestCaseData(elements.Element22, false).Returns(expected.Select(ToReturnsTuple).ToArray()).SetArgDisplayNames("(ParagraphBlock)Document[22]", "false");
        yield return new TestCaseData(elements.Element22, true).Returns(withAttrReturns.Select(ToReturnsTuple).ToArray()).SetArgDisplayNames("(ParagraphBlock)Document[22]", "true");

        expected = [elements.Element24_0, elements.Element24_1, elements.Element24_2, elements.Element24_3, elements.Element24_4];
        yield return new TestCaseData(elements.Element24, null).Returns(expected.Select(ToReturnsTuple).ToArray()).SetArgDisplayNames("(ParagraphBlock)Document[24]", "null");
        yield return new TestCaseData(elements.Element24, false).Returns(expected.Select(ToReturnsTuple).ToArray()).SetArgDisplayNames("(ParagraphBlock)Document[24]", "false");
        yield return new TestCaseData(elements.Element24, true).Returns(expected.Select(ToReturnsTuple).ToArray()).SetArgDisplayNames("(ParagraphBlock)Document[24]", "true");

        expected = [elements.Element28_0, elements.Element28_0_0, elements.Element28_0_0_0, elements.Element28_0_0_1,
                elements.Element28_1, elements.Element28_1_0, elements.Element28_1_0_0, elements.Element28_1_0_1];
        yield return new TestCaseData(elements.Element28, null).Returns(expected.Select(ToReturnsTuple).ToArray()).SetArgDisplayNames("(FootnoteGroup)Document[28]", "null");
        yield return new TestCaseData(elements.Element28, false).Returns(expected.Select(ToReturnsTuple).ToArray()).SetArgDisplayNames("(FootnoteGroup)Document[28]", "false");
        yield return new TestCaseData(elements.Element28, true).Returns(expected.Select(ToReturnsTuple).ToArray()).SetArgDisplayNames("(FootnoteGroup)Document[28]", "true");

        expected = [elements.Element28_1_0, elements.Element28_1_0_0, elements.Element28_1_0_1];
        yield return new TestCaseData(elements.Element28_1, null).Returns(expected.Select(ToReturnsTuple).ToArray()).SetArgDisplayNames("(Footnote)Document[28][1]", "null");
        yield return new TestCaseData(elements.Element28_1, false).Returns(expected.Select(ToReturnsTuple).ToArray()).SetArgDisplayNames("(Footnote)Document[28][1]", "false");
        yield return new TestCaseData(elements.Element28_1, true).Returns(expected.Select(ToReturnsTuple).ToArray()).SetArgDisplayNames("(Footnote)Document[28][1]", "true");

        expected = [elements.Element0, elements.Element0_0, elements.Element1, elements.Element1_0_0, elements.Element2, elements.Element2_0, elements.Element2_1, elements.Element2_2, elements.Element3,
            elements.Element3_0, elements.Element3_0_0, elements.Element3_0_0_0, elements.Element3_0_0_1, elements.Element3_1, elements.Element3_1_0, elements.Element3_1_0_0, elements.Element3_1_0_1,
            elements.Element3_2, elements.Element3_2_0, elements.Element3_2_0_0, elements.Element3_3, elements.Element3_3_0, elements.Element3_3_0_0, elements.Element4, elements.Element4_0, elements.Element5,
            elements.Element5_0, elements.Element6, elements.Element6_0, elements.Element6_0_0, elements.Element7, elements.Element7_0, elements.Element8, elements.Element8_0, elements.Element8_0_0,
            elements.Element9, elements.Element9_0, elements.Element9_0_0, elements.Element9_0_1, elements.Element9_0_1_0, elements.Element10, elements.Element10_0, elements.Element10_1, elements.Element10_2,
            elements.Element10_3, elements.Element10_4, elements.Element10_5, elements.Element10_6, elements.Element10_7, elements.Element10_8, elements.Element10_9, elements.Element10_10, elements.Element10_11,
            elements.Element10_12, elements.Element11, elements.Element11_0, elements.Element12, elements.Element12_0, elements.Element12_1, elements.Element12_2, elements.Element12_3, elements.Element13,
            elements.Element13_0, elements.Element13_1, elements.Element13_2, elements.Element13_3, elements.Element14, elements.Element14_0, elements.Element14_0_0, elements.Element14_1, elements.Element14_2,
            elements.Element15, elements.Element15_0, elements.Element16, elements.Element16_0, elements.Element16_0_0, elements.Element16_0_0_0, elements.Element16_0_1, elements.Element16_0_1_0,
            elements.Element16_0_1_1, elements.Element16_0_1_2, elements.Element16_1, elements.Element16_1_0, elements.Element16_1_0_0, elements.Element16_1_1, elements.Element16_1_1_0, elements.Element17,
            elements.Element17_0, elements.Element18, elements.Element18_0, elements.Element18_1, elements.Element18_2, elements.Element19, elements.Element20, elements.Element21, elements.Element21_0,
            elements.Element22, elements.Element22_0, elements.Element22_1, elements.Element23, elements.Element23_0, elements.Element24, elements.Element24_0, elements.Element24_1, elements.Element24_2,
            elements.Element24_3, elements.Element24_4, elements.Element25, elements.Element25_0, elements.Element26, elements.Element26_0, elements.Element27, elements.Element27_0, elements.Element28,
            elements.Element28_0, elements.Element28_0_0, elements.Element28_0_0_0, elements.Element28_0_0_1, elements.Element28_1, elements.Element28_1_0, elements.Element28_1_0_0, elements.Element28_1_0_1];
        withAttrReturns = [elements.Element0, elements.Element0_Attributes, elements.Element0_0, elements.Element1, elements.Element1_0_0, elements.Element2, elements.Element2_0, elements.Element2_1,
            elements.Element2_2, elements.Element3, elements.Element3_Attributes, elements.Element3_0, elements.Element3_0_Attributes, elements.Element3_0_0, elements.Element3_0_0_0, elements.Element3_0_0_1,
            elements.Element3_1, elements.Element3_1_Attributes, elements.Element3_1_0, elements.Element3_1_0_0, elements.Element3_1_0_1, elements.Element3_2, elements.Element3_2_0, elements.Element3_2_0_0,
            elements.Element3_3, elements.Element3_3_0, elements.Element3_3_0_0, elements.Element4, elements.Element4_Attributes, elements.Element4_0, elements.Element5, elements.Element5_Attributes,
            elements.Element5_0, elements.Element6, elements.Element6_0, elements.Element6_0_0, elements.Element7, elements.Element7_Attributes, elements.Element7_0, elements.Element8, elements.Element8_0,
            elements.Element8_0_0, elements.Element9, elements.Element9_0, elements.Element9_0_0, elements.Element9_0_1, elements.Element9_0_1_0, elements.Element10, elements.Element10_0, elements.Element10_1,
            elements.Element10_2, elements.Element10_3, elements.Element10_4, elements.Element10_5, elements.Element10_6, elements.Element10_7, elements.Element10_8, elements.Element10_9, elements.Element10_10,
            elements.Element10_11, elements.Element10_12, elements.Element11, elements.Element11_Attributes, elements.Element11_0, elements.Element12, elements.Element12_0, elements.Element12_1,
            elements.Element12_2, elements.Element12_3, elements.Element12_3_Attributes, elements.Element13, elements.Element13_0, elements.Element13_1, elements.Element13_1_Attributes, elements.Element13_2,
            elements.Element13_3, elements.Element13_3_Attributes, elements.Element14, elements.Element14_0, elements.Element14_0_0, elements.Element14_1, elements.Element14_2, elements.Element14_2_Attributes,
            elements.Element15, elements.Element15_Attributes, elements.Element15_0, elements.Element16, elements.Element16_0, elements.Element16_0_0, elements.Element16_0_0_0, elements.Element16_0_1,
            elements.Element16_0_1_0, elements.Element16_0_1_1, elements.Element16_0_1_2, elements.Element16_1, elements.Element16_1_0, elements.Element16_1_0_0, elements.Element16_1_1, elements.Element16_1_1_0,
            elements.Element17, elements.Element17_Attributes, elements.Element17_0, elements.Element18, elements.Element18_0, elements.Element18_1, elements.Element18_2, elements.Element19,
            elements.Element19_Attributes, elements.Element20, elements.Element21, elements.Element21_Attributes, elements.Element21_0, elements.Element22, elements.Element22_0, elements.Element22_1,
            elements.Element22_1_Attributes, elements.Element23, elements.Element23_Attributes, elements.Element23_0, elements.Element24, elements.Element24_0, elements.Element24_1, elements.Element24_2,
            elements.Element24_3, elements.Element24_4, elements.Element25, elements.Element25_Attributes, elements.Element25_0, elements.Element26, elements.Element26_0, elements.Element27, elements.Element27_0,
            elements.Element28, elements.Element28_0, elements.Element28_0_0, elements.Element28_0_0_0, elements.Element28_0_0_1, elements.Element28_1, elements.Element28_1_0, elements.Element28_1_0_0,
            elements.Element28_1_0_1];
        yield return new TestCaseData(document, null).Returns(expected.Select(ToReturnsTuple).ToArray()).SetArgDisplayNames("Document", "null");
        yield return new TestCaseData(document, false).Returns(expected.Select(ToReturnsTuple).ToArray()).SetArgDisplayNames("Document", "false");
        yield return new TestCaseData(document, true).Returns(withAttrReturns.Select(ToReturnsTuple).ToArray()).SetArgDisplayNames("Document", "true");

        yield return new TestCaseData(elements.Element0_0, null).Returns(ReturnsEmpty).SetArgDisplayNames("(LiteralInline)Document[0][0]", "null");
        yield return new TestCaseData(elements.Element0_0, false).Returns(ReturnsEmpty).SetArgDisplayNames("(LiteralInline)Document[0][0]", "false");
        yield return new TestCaseData(elements.Element0_0, true).Returns(ReturnsEmpty).SetArgDisplayNames("(LiteralInline)Document[0][0]", "true");

        withAttrReturns = [elements.Element12_3_Attributes];
        yield return new TestCaseData(elements.Element12_3, null).Returns(ReturnsEmpty).SetArgDisplayNames("(MathInline)Document[12][3]", "null");
        yield return new TestCaseData(elements.Element12_3, false).Returns(ReturnsEmpty).SetArgDisplayNames("(MathInline)Document[12][3]", "false");
        yield return new TestCaseData(elements.Element12_3, true).Returns(withAttrReturns.Select(ToReturnsTuple).ToArray()).SetArgDisplayNames("(MathInline)Document[12][3]", "true");

        yield return new TestCaseData(elements.Element18_1, null).Returns(ReturnsEmpty).SetArgDisplayNames("(CodeInline)Document[18][1]", "null");
        yield return new TestCaseData(elements.Element18_1, false).Returns(ReturnsEmpty).SetArgDisplayNames("(CodeInline)Document[18][1]", "false");
        yield return new TestCaseData(elements.Element18_1, true).Returns(ReturnsEmpty).SetArgDisplayNames("(CodeInline)Document[18][1]", "true");

        withAttrReturns = [elements.Element20_Attributes];
        yield return new TestCaseData(elements.Element20, null).Returns(ReturnsEmpty).SetArgDisplayNames("(FencedCodeBlock)Document[20]", "null");
        yield return new TestCaseData(elements.Element20, false).Returns(ReturnsEmpty).SetArgDisplayNames("(FencedCodeBlock)Document[20]", "false");
        yield return new TestCaseData(elements.Element20, true).Returns(withAttrReturns.Select(ToReturnsTuple).ToArray()).SetArgDisplayNames("(FencedCodeBlock)Document[20]", "true");

        withAttrReturns = [elements.Element22_1_Attributes];
        yield return new TestCaseData(elements.Element22_1, null).Returns(ReturnsEmpty).SetArgDisplayNames("(LineBreakInline)Document[22][1]", "null");
        yield return new TestCaseData(elements.Element22_1, false).Returns(ReturnsEmpty).SetArgDisplayNames("(LineBreakInline)Document[22][1]", "false");
        yield return new TestCaseData(elements.Element22_1, true).Returns(withAttrReturns.Select(ToReturnsTuple).ToArray()).SetArgDisplayNames("(LineBreakInline)Document[22][1]", "true");

        yield return new TestCaseData(elements.Element24_1, null).Returns(ReturnsEmpty).SetArgDisplayNames("(FootnoteLink)Document[24][1]", "null");
        yield return new TestCaseData(elements.Element24_1, false).Returns(ReturnsEmpty).SetArgDisplayNames("(FootnoteLink)Document[24][1]", "false");
        yield return new TestCaseData(elements.Element24_1, true).Returns(expected.Select(ToReturnsTuple).ToArray()).SetArgDisplayNames("(FootnoteLink)Document[24][1]", "true");
    }

    /// <summary>
    /// Test cases for <see cref="MarkdownExtensionMethods.GetDescendantBranchesMatchingType(MarkdownObject?, Type)"/>.
    /// </summary>
    /// <returns></returns>
    public static System.Collections.IEnumerable GetGetDescendantBranchesMatchingType1TestData()
    {
        MarkdownDocument document = GetMarkdownDocument();
        var elements = new MarkdownElements(document);
        IEnumerable<MarkdownObject> expected = [elements.Element0, elements.Element0_0, elements.Element1, elements.Element1_0_0, elements.Element2, elements.Element2_0, elements.Element2_1, elements.Element2_2, elements.Element3,
            elements.Element3_0, elements.Element3_0_0, elements.Element3_0_0_0, elements.Element3_0_0_1, elements.Element3_1, elements.Element3_1_0, elements.Element3_1_0_0, elements.Element3_1_0_1,
            elements.Element3_2, elements.Element3_2_0, elements.Element3_2_0_0, elements.Element3_3, elements.Element3_3_0, elements.Element3_3_0_0, elements.Element4, elements.Element4_0, elements.Element5,
            elements.Element5_0, elements.Element6, elements.Element6_0, elements.Element6_0_0, elements.Element7, elements.Element7_0, elements.Element8, elements.Element8_0, elements.Element8_0_0,
            elements.Element9, elements.Element9_0, elements.Element9_0_0, elements.Element9_0_1, elements.Element9_0_1_0, elements.Element10, elements.Element10_0, elements.Element10_1, elements.Element10_2,
            elements.Element10_3, elements.Element10_4, elements.Element10_5, elements.Element10_6, elements.Element10_7, elements.Element10_8, elements.Element10_9, elements.Element10_10, elements.Element10_11,
            elements.Element10_12, elements.Element11, elements.Element11_0, elements.Element12, elements.Element12_0, elements.Element12_1, elements.Element12_2, elements.Element12_3, elements.Element13,
            elements.Element13_0, elements.Element13_1, elements.Element13_2, elements.Element13_3, elements.Element14, elements.Element14_0, elements.Element14_0_0, elements.Element14_1, elements.Element14_2,
            elements.Element15, elements.Element15_0, elements.Element16, elements.Element16_0, elements.Element16_0_0, elements.Element16_0_0_0, elements.Element16_0_1, elements.Element16_0_1_0,
            elements.Element16_0_1_1, elements.Element16_0_1_2, elements.Element16_1, elements.Element16_1_0, elements.Element16_1_0_0, elements.Element16_1_1, elements.Element16_1_1_0, elements.Element17,
            elements.Element17_0, elements.Element18, elements.Element18_0, elements.Element18_1, elements.Element18_2, elements.Element19, elements.Element20, elements.Element21, elements.Element21_0,
            elements.Element22, elements.Element22_0, elements.Element22_1, elements.Element23, elements.Element23_0, elements.Element24, elements.Element24_0, elements.Element24_1, elements.Element24_2,
            elements.Element24_3, elements.Element24_4, elements.Element25, elements.Element25_0, elements.Element26, elements.Element26_0, elements.Element27, elements.Element27_0, elements.Element28,
            elements.Element28_0, elements.Element28_0_0, elements.Element28_0_0_0, elements.Element28_0_0_1, elements.Element28_1, elements.Element28_1_0, elements.Element28_1_0_0, elements.Element28_1_0_1];
        yield return new TestCaseData(document, typeof(MarkdownObject)).Returns(expected.Select(ToReturnsTuple).ToArray()).SetArgDisplayNames("Document", "MarkdownObject");

        expected = [elements.Element0, elements.Element1, elements.Element2, elements.Element3_0_0, elements.Element3_1_0, elements.Element3_2_0, elements.Element3_3_0, elements.Element4, elements.Element5,
            elements.Element6, elements.Element7, elements.Element8, elements.Element9, elements.Element10, elements.Element11, elements.Element12, elements.Element13, elements.Element14, elements.Element15,
            elements.Element16_0_0, elements.Element16_0_1, elements.Element16_1_0, elements.Element16_1_1, elements.Element17, elements.Element18, elements.Element19, elements.Element20, elements.Element21,
            elements.Element22, elements.Element23, elements.Element24, elements.Element25, elements.Element26, elements.Element27, elements.Element28_0_0, elements.Element28_1_0];
        yield return new TestCaseData(document, typeof(LeafBlock)).Returns(expected.Select(ToReturnsTuple).ToArray()).SetArgDisplayNames("Document", "LeafBlock");

        expected = [elements.Element1, elements.Element2, elements.Element3_0_0, elements.Element3_1_0, elements.Element3_2_0, elements.Element3_3_0, elements.Element6, elements.Element8, elements.Element9,
            elements.Element12, elements.Element13, elements.Element14, elements.Element16_0_1, elements.Element16_1_1, elements.Element18,  elements.Element22, elements.Element24,elements.Element26,
            elements.Element27, elements.Element28_0_0, elements.Element28_1_0];
        yield return new TestCaseData(document, typeof(ParagraphBlock)).Returns(expected.Select(ToReturnsTuple).ToArray()).SetArgDisplayNames("Document", "ParagraphBlock");

        expected = [elements.Element3, elements.Element10, elements.Element16, elements.Element24_1, elements.Element24_3, elements.Element28];
        yield return new TestCaseData(document, typeof(ContainerBlock)).Returns(expected.Select(ToReturnsTuple).ToArray()).SetArgDisplayNames("Document", "ContainerBlock");

        expected = [elements.Element19, elements.Element20, elements.Element16, elements.Element24_1, elements.Element24_3, elements.Element28];
        yield return new TestCaseData(document, typeof(CodeBlock)).Returns(expected.Select(ToReturnsTuple).ToArray()).SetArgDisplayNames("Document", "ContainerBlock");

        expected = [elements.Element1_0, elements.Element6_0, elements.Element8_0, elements.Element9_0, elements.Element14_0];
        yield return new TestCaseData(document, typeof(ContainerInline)).Returns(expected.Select(ToReturnsTuple).ToArray()).SetArgDisplayNames("Document", "ContainerBlock");

        expected = [elements.Element9_0_1];
        yield return new TestCaseData(elements.Element9_0, typeof(ContainerInline)).Returns(expected.Select(ToReturnsTuple).ToArray()).SetArgDisplayNames("(LinkInline)Document[9][0]", "ContainerBlock");

        expected = [elements.Element0_0, elements.Element1_0_0, elements.Element2_0, elements.Element2_1, elements.Element2_2, elements.Element3_0_0_0, elements.Element3_0_0_1, elements.Element3_1_0_0,
            elements.Element3_1_0_1, elements.Element3_2_0_0, elements.Element3_3_0_0, elements.Element4_0, elements.Element5_0, elements.Element6_0_0, elements.Element7_0,  elements.Element8_0_0,
            elements.Element9_0_0, elements.Element9_0_1_0, elements.Element11_0, elements.Element12_0, elements.Element12_1, elements.Element12_2, elements.Element12_3, elements.Element13_0, elements.Element13_2,
            elements.Element13_3, elements.Element14_0_0, elements.Element14_1, elements.Element14_2, elements.Element15_0, elements.Element16_0_0_0, elements.Element16_0_1_0, elements.Element16_0_1_1,
            elements.Element16_0_1_2, elements.Element16_1_0_0, elements.Element16_1_1_0, elements.Element17_0, elements.Element18_0, elements.Element18_1, elements.Element18_2, elements.Element21_0,
            elements.Element22_0, elements.Element22_1, elements.Element23_0, elements.Element24_0, elements.Element24_2, elements.Element24_4, elements.Element25_0, elements.Element26_0, elements.Element27_0,
            elements.Element28_0_0_0, elements.Element28_1_0_0];
        yield return new TestCaseData(document, typeof(LeafInline)).Returns(expected.Select(ToReturnsTuple).ToArray()).SetArgDisplayNames("Document", "LeafInline");

        expected = [elements.Element0_0, elements.Element1_0_0, elements.Element2_0, elements.Element2_2, elements.Element3_0_0_1, elements.Element3_1_0_1, elements.Element3_2_0_0, elements.Element3_3_0_0,
            elements.Element4_0, elements.Element5_0, elements.Element6_0_0, elements.Element7_0,  elements.Element8_0_0, elements.Element9_0_0, elements.Element9_0_1_0, elements.Element11_0, elements.Element12_0,
            elements.Element12_2, elements.Element13_0, elements.Element13_2, elements.Element14_0_0, elements.Element15_0, elements.Element16_0_0_0, elements.Element16_0_1_0, elements.Element16_0_1_2,
            elements.Element16_1_0_0, elements.Element16_1_1_0, elements.Element17_0, elements.Element18_0, elements.Element18_2, elements.Element21_0, elements.Element22_0, elements.Element23_0, elements.Element24_0,
            elements.Element24_2, elements.Element24_4, elements.Element25_0, elements.Element26_0, elements.Element27_0, elements.Element28_0_0_0, elements.Element28_1_0_0];
        yield return new TestCaseData(document, typeof(LiteralInline)).Returns(expected.Select(ToReturnsTuple).ToArray()).SetArgDisplayNames("Document", "LiteralInline");

        expected = [elements.Element1_0_0];
        yield return new TestCaseData(elements.Element1_0, typeof(LeafInline)).Returns(expected.Select(ToReturnsTuple).ToArray()).SetArgDisplayNames("(LinkInline)Document[1][0]", "LeafInline");

        yield return new TestCaseData(elements.Element12_2, typeof(LiteralInline)).Returns(ReturnsEmpty).SetArgDisplayNames("(LiteralInline)Document[12][2]", "LiteralInline");

        yield return new TestCaseData(elements.Element2_1, typeof(LeafInline)).Returns(ReturnsEmpty).SetArgDisplayNames("(LineBreakInline)Document[2][1]", "LeafInline");
    }

    /// <summary>
    /// Test cases for <see cref="MarkdownExtensionMethods.GetDescendantBranchesMatchingType(MarkdownObject?, IEnumerable{Type})"/>.
    /// </summary>
    /// <returns></returns>
    public static System.Collections.IEnumerable GetGetDescendantBranchesMatchingType2TestData()
    {
        MarkdownDocument document = GetMarkdownDocument();
        var elements = new MarkdownElements(document);
        IEnumerable<Type> types = [typeof(ContainerInline), typeof(Container)];
        IEnumerable<MarkdownObject> expected = [elements.Element1_0, elements.Element3, elements.Element6_0, elements.Element8_0, elements.Element9_0, elements.Element10, elements.Element14_0, elements.Element16,
            elements.Element24_1, elements.Element24_3, elements.Element28];
        yield return new TestCaseData(document, types).Returns(expected.Select(ToReturnsTuple).ToArray()).SetArgDisplayNames("Document", $"[{string.Join(", ", types.Select(t => t.Name))}]");

        types = [typeof(CodeInline), typeof(FencedCodeBlock), typeof(LineBreakInline)];
        expected = [elements.Element2_1, elements.Element12_1, elements.Element14_1, elements.Element16_0_1_1, elements.Element18_1, elements.Element19, elements.Element20, elements.Element22_1];
        yield return new TestCaseData(document, types).Returns(expected.Select(ToReturnsTuple).ToArray()).SetArgDisplayNames("Document", $"[{string.Join(", ", types.Select(t => t.Name))}]");

        types = [typeof(ListBlock), typeof(ParagraphBlock)];
        expected = [elements.Element1, elements.Element2, elements.Element3, elements.Element6, elements.Element8, elements.Element9, elements.Element12, elements.Element13, elements.Element14,
            elements.Element16_0_1, elements.Element16_1_1, elements.Element18, elements.Element22, elements.Element24, elements.Element26, elements.Element27, elements.Element28_0_0, elements.Element28_1_0];
        yield return new TestCaseData(document, types).Returns(expected.Select(ToReturnsTuple).ToArray()).SetArgDisplayNames("Document", $"[{string.Join(", ", types.Select(t => t.Name))}]");
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