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
    /// Test cases for <see cref="MarkdownExtensionMethods.GetChildObjects(MarkdownObject?, bool)"/>.
    /// </summary>
    /// <returns></returns>
    public static System.Collections.IEnumerable GetGetChildObjectsTestData()
    {
        MarkdownDocument document = GetMarkdownDocument();

        var elements = new MarkdownElements(document);
        IEnumerable<MarkdownObject> expected = [elements.Element0, elements.Element1, elements.Element2, elements.Element3, elements.Element4, elements.Element5, elements.Element6, elements.Element7, elements.Element8, elements.Element9,
            elements.Element10, elements.Element11, elements.Element12, elements.Element13, elements.Element14, elements.Element15, elements.Element16, elements.Element17, elements.Element18, elements.Element19, elements.Element20, elements.Element21,
            elements.Element22, elements.Element23, elements.Element24, elements.Element25, elements.Element26, elements.Element27, elements.Element28, elements.Element29, elements.Element30, elements.Element31, elements.Element32, elements.Element33,
            elements.Element34, elements.Element35, elements.Element36, elements.Element37];
        yield return new TestCaseData(document, null).Returns(expected).SetArgDisplayNames("Document", "null");
        yield return new TestCaseData(document, false).Returns(expected).SetArgDisplayNames("Document", "false");
        yield return new TestCaseData(document, true).Returns(expected).SetArgDisplayNames("Document", "true");

        expected = [elements.Element0_0];
        yield return new TestCaseData(elements.Element0, null).Returns(expected).SetArgDisplayNames("(HeadingBlock)Document[0]", "null");
        yield return new TestCaseData(elements.Element0, false).Returns(expected).SetArgDisplayNames("(HeadingBlock)Document[0]", "false");
        yield return new TestCaseData(elements.Element0, true)
            .Returns((IEnumerable<MarkdownObject>)[elements.Element0_Attributes, elements.Element0_0])
            .SetArgDisplayNames("(HeadingBlock)Document[0]", "true");

        expected = [];
        yield return new TestCaseData(elements.Element0_0, null).Returns(expected).SetArgDisplayNames("(LiteralInline)Document[0][0]", "null");
        yield return new TestCaseData(elements.Element0_0, false).Returns(expected).SetArgDisplayNames("(LiteralInline)Document[0][0]", "false");
        yield return new TestCaseData(elements.Element0_0, true).Returns(expected).SetArgDisplayNames("(LiteralInline)Document[0][0]", "true");

        yield return new TestCaseData(elements.Element1, null).Returns(expected).SetArgDisplayNames("(HtmlBlock)Document[1]", "null");
        yield return new TestCaseData(elements.Element1, false).Returns(expected).SetArgDisplayNames("(HtmlBlock)Document[1]", "false");
        yield return new TestCaseData(elements.Element1, true).Returns(expected).SetArgDisplayNames("(HtmlBlock)Document[1]", "true");

        expected = [elements.Element2_0];
        yield return new TestCaseData(elements.Element2, null).Returns(expected).SetArgDisplayNames("(ParagraphBlock)Document[2]", "null");
        yield return new TestCaseData(elements.Element2, false).Returns(expected).SetArgDisplayNames("(ParagraphBlock)Document[2]", "false");
        yield return new TestCaseData(elements.Element2, true).Returns(expected).SetArgDisplayNames("(ParagraphBlock)Document[2]", "true");

        expected = [elements.Element3_0_0];
        yield return new TestCaseData(elements.Element3_0, null).Returns(expected).SetArgDisplayNames("(LinkInline)Document[3][0]", "null");
        yield return new TestCaseData(elements.Element3_0, false).Returns(expected).SetArgDisplayNames("(LinkInline)Document[3][0]", "false");
        yield return new TestCaseData(elements.Element3_0, true).Returns(expected).SetArgDisplayNames("(LinkInline)Document[3][0]", "true");

        expected = [elements.Element4_0, elements.Element4_1, elements.Element4_2];
        yield return new TestCaseData(elements.Element4, null).Returns(expected).SetArgDisplayNames("(ParagraphBlock)Document[4]", "null");
        yield return new TestCaseData(elements.Element4, false).Returns(expected).SetArgDisplayNames("(ParagraphBlock)Document[4]", "false");
        yield return new TestCaseData(elements.Element4, true).Returns(expected).SetArgDisplayNames("(ParagraphBlock)Document[4]", "true");

        expected = [];
        yield return new TestCaseData(elements.Element4_2, null).Returns(expected).SetArgDisplayNames("(LineBreakInline)Document[4][2]", "null");
        yield return new TestCaseData(elements.Element4_2, false).Returns(expected).SetArgDisplayNames("(LineBreakInline)Document[4][2]", "false");
        yield return new TestCaseData(elements.Element4_2, true)
            .Returns((IEnumerable<MarkdownObject>)[elements.Element4_2_Attributes]).SetArgDisplayNames("(LineBreakInline)Document[4][2]", "true");

        expected = [elements.Element5_0, elements.Element5_1, elements.Element5_2, elements.Element5_3, elements.Element5_4, elements.Element5_5, elements.Element5_6];
        yield return new TestCaseData(elements.Element5, null).Returns(expected).SetArgDisplayNames("(ParagraphBlock)Document[5]", "null");
        yield return new TestCaseData(elements.Element5, false).Returns(expected).SetArgDisplayNames("(ParagraphBlock)Document[5]", "false");
        yield return new TestCaseData(elements.Element5, true).Returns(expected).SetArgDisplayNames("(ParagraphBlock)Document[5]", "true");

        expected = [elements.Element5_5_0];
        yield return new TestCaseData(elements.Element5_5, null).Returns(expected).SetArgDisplayNames("(LinkInline)Document[5][5]", "null");
        yield return new TestCaseData(elements.Element5_5, false).Returns(expected).SetArgDisplayNames("(LinkInline)Document[5][5]", "false");
        yield return new TestCaseData(elements.Element5_5, true).Returns(expected).SetArgDisplayNames("(LinkInline)Document[5][5]", "true");

        expected = [elements.Element13_0, elements.Element13_1, elements.Element13_2, elements.Element13_3];
        yield return new TestCaseData(elements.Element13, null).Returns(expected).SetArgDisplayNames("(ListBlock)Document[13]", "null");
        yield return new TestCaseData(elements.Element13, false).Returns(expected).SetArgDisplayNames("(ListBlock)Document[13]", "false");
        yield return new TestCaseData(elements.Element13, true)
            .Returns((IEnumerable<MarkdownObject>)[elements.Element13_Attributes, elements.Element13_0, elements.Element13_1, elements.Element13_2, elements.Element13_3])
            .SetArgDisplayNames("(ListBlock)Document[13]", "true");

        expected = [elements.Element13_1_0];
        yield return new TestCaseData(elements.Element13_1, null).Returns(expected).SetArgDisplayNames("(ListItemBlock)Document[13][1]", "null");
        yield return new TestCaseData(elements.Element13_1, false).Returns(expected).SetArgDisplayNames("(ListItemBlock)Document[13][1]", "false");
        yield return new TestCaseData(elements.Element13_1, true)
            .Returns((IEnumerable<MarkdownObject>)[elements.Element13_1_Attributes, elements.Element13_1_0])
            .SetArgDisplayNames("(ListItemBlock)Document[13][1]", "true");

        expected = [elements.Element16_0, elements.Element16_1];
        yield return new TestCaseData(elements.Element16, null).Returns(expected).SetArgDisplayNames("(QuoteBlock)Document[16]", "null");
        yield return new TestCaseData(elements.Element16, false).Returns(expected).SetArgDisplayNames("(QuoteBlock)Document[16]", "false");
        yield return new TestCaseData(elements.Element16, true).Returns(expected).SetArgDisplayNames("(QuoteBlock)Document[16]", "true");

        expected = [elements.Element17_1_0, elements.Element17_1_1, elements.Element17_1_2, elements.Element17_1_3];
        yield return new TestCaseData(elements.Element17_1, null).Returns(expected).SetArgDisplayNames("(EmphasisInline)Document[17][1]", "null");
        yield return new TestCaseData(elements.Element17_1, false).Returns(expected).SetArgDisplayNames("(EmphasisInline)Document[17][1]", "false");
        yield return new TestCaseData(elements.Element17_1, true).Returns(expected).SetArgDisplayNames("(EmphasisInline)Document[17][1]", "true");

        expected = [elements.Element19_0, elements.Element19_1, elements.Element19_2];
        yield return new TestCaseData(elements.Element19, null).Returns(expected).SetArgDisplayNames("(Table)Document[21]", "null");
        yield return new TestCaseData(elements.Element19, false).Returns(expected).SetArgDisplayNames("(Table)Document[21]", "false");
        yield return new TestCaseData(elements.Element19, true).Returns(expected).SetArgDisplayNames("(Table)Document[21]", "true");

        expected = [elements.Element21_2_0, elements.Element21_2_1, elements.Element21_2_2];
        yield return new TestCaseData(elements.Element21_2, null).Returns(expected).SetArgDisplayNames("(TableRow)Document[21][2]", "null");
        yield return new TestCaseData(elements.Element21_2, false).Returns(expected).SetArgDisplayNames("(TableRow)Document[21][2]", "false");
        yield return new TestCaseData(elements.Element21_2, true).Returns(expected).SetArgDisplayNames("(TableRow)Document[21][2]", "true");

        expected = [elements.Element22_0, elements.Element22_1];
        yield return new TestCaseData(elements.Element22, null).Returns(expected).SetArgDisplayNames("(Figure)Document[22]", "null");
        yield return new TestCaseData(elements.Element22, false).Returns(expected).SetArgDisplayNames("(Figure)Document[22]", "false");
        yield return new TestCaseData(elements.Element22, true).Returns(expected).SetArgDisplayNames("(Figure)Document[22]", "true");

        expected = [];
        yield return new TestCaseData(elements.Element26_2, null).Returns(expected).SetArgDisplayNames("(MathInline)Document[26][2]", "null");
        yield return new TestCaseData(elements.Element26_2, false).Returns(expected).SetArgDisplayNames("(MathInline)Document[26][2]", "false");
        yield return new TestCaseData(elements.Element26_2, true)
            .Returns((IEnumerable<MarkdownObject>)[elements.Element26_2_Attributes]).SetArgDisplayNames("(MathInline)Document[26][2]", "true");

        yield return new TestCaseData(elements.Element28, null).Returns(expected).SetArgDisplayNames("(ThematicBreakBlock)Document[28]", "null");
        yield return new TestCaseData(elements.Element28, false).Returns(expected).SetArgDisplayNames("(ThematicBreakBlock)Document[28]", "false");
        yield return new TestCaseData(elements.Element28, true).Returns(expected).SetArgDisplayNames("(ThematicBreakBlock)Document[28]", "true");

        expected = [elements.Element34_0, elements.Element34_1];
        yield return new TestCaseData(elements.Element34, null).Returns(expected).SetArgDisplayNames("(DefinitionList)Document[34]", "null");
        yield return new TestCaseData(elements.Element34, false).Returns(expected).SetArgDisplayNames("(DefinitionList)Document[34]", "false");
        yield return new TestCaseData(elements.Element34, true).Returns(expected).SetArgDisplayNames("(DefinitionList)Document[34]", "true");

        expected = [elements.Element34_1_0, elements.Element34_1_1];
        yield return new TestCaseData(elements.Element34_1, null).Returns(expected).SetArgDisplayNames("(DefinitionItem)Document[34][1]", "null");
        yield return new TestCaseData(elements.Element34_1, false).Returns(expected).SetArgDisplayNames("(DefinitionItem)Document[34][1]", "false");
        yield return new TestCaseData(elements.Element34_1, true).Returns(expected).SetArgDisplayNames("(DefinitionItem)Document[34][1]", "true");

        expected = [elements.Element36_0, elements.Element36_1, elements.Element36_2, elements.Element36_3, elements.Element36_4, elements.Element36_5];
        yield return new TestCaseData(elements.Element36, null).Returns(expected).SetArgDisplayNames("(LinkReferenceDefinitionGroup)Document[36]", "null");
        yield return new TestCaseData(elements.Element36, false).Returns(expected).SetArgDisplayNames("(LinkReferenceDefinitionGroup)Document[36]", "false");
        yield return new TestCaseData(elements.Element36, true).Returns(expected).SetArgDisplayNames("(LinkReferenceDefinitionGroup)Document[36]", "true");

        expected = [elements.Element37_0, elements.Element37_1];
        yield return new TestCaseData(elements.Element37, null).Returns(expected).SetArgDisplayNames("(FootnoteGroup)Document[37]", "null");
        yield return new TestCaseData(elements.Element37, false).Returns(expected).SetArgDisplayNames("(FootnoteGroup)Document[37]", "false");
        yield return new TestCaseData(elements.Element37, true).Returns(expected).SetArgDisplayNames("(FootnoteGroup)Document[37]", "true");
    }

    /// <summary>
    /// Test cases for <see cref="MarkdownExtensionMethods.GetAllDescendants(MarkdownObject?, bool)"/>.
    /// </summary>
    /// <returns></returns>
    public static System.Collections.IEnumerable GetGetAllDescendantsTestData()
    {
        MarkdownDocument document = GetMarkdownDocument();
        var elements = new MarkdownElements(document);
        IEnumerable<MarkdownObject> expected = [elements.Element0, elements.Element0_0, elements.Element1, elements.Element2, elements.Element2_0, elements.Element3, elements.Element3_0, elements.Element3_0_0, elements.Element4, elements.Element4_0,
            elements.Element4_1, elements.Element4_2, elements.Element5, elements.Element5_0, elements.Element5_1, elements.Element5_2, elements.Element5_3, elements.Element5_4, elements.Element5_5, elements.Element5_5_0, elements.Element5_6, elements.Element6,
            elements.Element6_0, elements.Element7, elements.Element7_0, elements.Element8, elements.Element8_0, elements.Element8_0_0, elements.Element8_1, elements.Element8_2, elements.Element9, elements.Element9_0, elements.Element9_1, elements.Element9_1_0,
            elements.Element9_2, elements.Element10, elements.Element10_0, elements.Element10_1, elements.Element10_2, elements.Element10_3, elements.Element10_4, elements.Element10_4_0, elements.Element11, elements.Element11_0, elements.Element11_1,
            elements.Element11_2, elements.Element12, elements.Element12_0, elements.Element12_1, elements.Element12_2, elements.Element13, elements.Element13_0, elements.Element13_0_0, elements.Element13_0_0_0, elements.Element13_0_0_1, elements.Element13_1,
            elements.Element13_1_0, elements.Element13_1_0_0, elements.Element13_1_0_1, elements.Element13_2, elements.Element13_2_0, elements.Element13_2_0_0, elements.Element13_3, elements.Element13_3_0, elements.Element13_3_0_0, elements.Element14,
            elements.Element14_0, elements.Element14_0_0, elements.Element14_0_0_0, elements.Element14_1, elements.Element14_1_0, elements.Element14_1_0_0, elements.Element15, elements.Element15_0, elements.Element15_0_0, elements.Element15_0_1,
            elements.Element15_0_2, elements.Element15_0_3, elements.Element16, elements.Element16_0, elements.Element16_0_0, elements.Element16_0_1, elements.Element16_0_2, elements.Element16_0_2_0, elements.Element16_1, elements.Element16_1_0,
            elements.Element17, elements.Element17_0, elements.Element17_1, elements.Element17_1_0, elements.Element17_1_1, elements.Element17_1_2, elements.Element17_1_3, elements.Element17_2, elements.Element17_3, elements.Element17_4, elements.Element17_5,
            elements.Element17_6, elements.Element18, elements.Element18_0, elements.Element18_0_0, elements.Element18_0_1, elements.Element18_0_2, elements.Element18_0_3, elements.Element18_1, elements.Element18_1_0, elements.Element18_2, elements.Element18_3,
            elements.Element18_3_0, elements.Element19, elements.Element19_0, elements.Element19_0_0, elements.Element19_0_0_0, elements.Element19_0_0_0_0, elements.Element19_0_1, elements.Element19_0_1_0, elements.Element19_0_1_0_0, elements.Element19_1,
            elements.Element19_1_0, elements.Element19_1_0_0, elements.Element19_1_0_0_0, elements.Element19_1_1, elements.Element19_1_1_0, elements.Element19_1_1_0_0, elements.Element19_2, elements.Element19_2_0, elements.Element19_2_0_0,
            elements.Element19_2_0_0_0, elements.Element19_2_1, elements.Element19_2_1_0, elements.Element19_2_1_0_0, elements.Element20, elements.Element20_0, elements.Element21, elements.Element21_0, elements.Element21_0_0, elements.Element21_0_0_0,
            elements.Element21_0_0_0_0, elements.Element21_0_1, elements.Element21_0_1_0, elements.Element21_0_1_0_0, elements.Element21_0_2, elements.Element21_0_2_0, elements.Element21_0_2_0_0, elements.Element21_1, elements.Element21_1_0,
            elements.Element21_1_0_0, elements.Element21_1_0_0_0, elements.Element21_1_1, elements.Element21_1_1_0, elements.Element21_1_1_0_0, elements.Element21_1_2, elements.Element21_1_2_0, elements.Element21_1_2_0_0, elements.Element21_2,
            elements.Element21_2_0, elements.Element21_2_0_0, elements.Element21_2_0_0_0, elements.Element21_2_1, elements.Element21_2_1_0, elements.Element21_2_1_0_0, elements.Element21_2_2, elements.Element21_2_2_0, elements.Element21_2_2_0_0,
            elements.Element22, elements.Element22_0, elements.Element22_0_0, elements.Element22_0_0_0, elements.Element22_1, elements.Element22_1_0, elements.Element23, elements.Element23_0, elements.Element23_0_0, elements.Element24,
            elements.Element24_0, elements.Element24_1, elements.Element24_1_0, elements.Element24_2, elements.Element24_3, elements.Element24_3_0, elements.Element24_4, elements.Element24_5, elements.Element25, elements.Element25_0, elements.Element25_1,
            elements.Element25_2, elements.Element25_3, elements.Element25_4, elements.Element26, elements.Element26_0, elements.Element26_0_0, elements.Element26_1, elements.Element26_2, elements.Element27, elements.Element27_0, elements.Element27_1,
            elements.Element27_2, elements.Element28, elements.Element29, elements.Element29_0, elements.Element29_0_0, elements.Element29_1, elements.Element29_2, elements.Element29_2_0, elements.Element29_3, elements.Element30, elements.Element31,
            elements.Element32, elements.Element33, elements.Element34, elements.Element34_0, elements.Element34_0_0, elements.Element34_0_0_0, elements.Element34_0_1, elements.Element34_0_1_0, elements.Element34_0_1_1, elements.Element34_0_1_2,
            elements.Element34_1, elements.Element34_1_0, elements.Element34_1_0_0, elements.Element34_1_1, elements.Element34_1_1_0, elements.Element35, elements.Element35_0, elements.Element35_0_0, elements.Element35_0_1, elements.Element35_0_2,
            elements.Element36, elements.Element36_0, elements.Element36_1, elements.Element36_2, elements.Element36_3, elements.Element36_4, elements.Element36_5, elements.Element37, elements.Element37_0, elements.Element37_0_0, elements.Element37_0_0_0,
            elements.Element37_0_0_1, elements.Element37_1, elements.Element37_1_0, elements.Element37_1_0_0, elements.Element37_1_0_1];
        IEnumerable<MarkdownObject> withAttrReturns = [elements.Element0, elements.Element0_Attributes, elements.Element0_0, elements.Element1, elements.Element2, elements.Element2_0, elements.Element3, elements.Element3_0, elements.Element3_0_0,
            elements.Element4, elements.Element4_0, elements.Element4_1, elements.Element4_2, elements.Element4_2_Attributes, elements.Element5, elements.Element5_0, elements.Element5_1, elements.Element5_2, elements.Element5_3, elements.Element5_4,
            elements.Element5_5, elements.Element5_5_0, elements.Element5_6, elements.Element6, elements.Element6_Attributes, elements.Element6_0, elements.Element7, elements.Element7_Attributes, elements.Element7_0, elements.Element8, elements.Element8_0,
            elements.Element8_0_Attributes, elements.Element8_0_0, elements.Element8_1, elements.Element8_2, elements.Element9, elements.Element9_0, elements.Element9_1, elements.Element9_1_0, elements.Element9_2, elements.Element10, elements.Element10_0,
            elements.Element10_1, elements.Element10_2, elements.Element10_3, elements.Element10_4, elements.Element10_4_0, elements.Element11, elements.Element11_0, elements.Element11_1, elements.Element11_2, elements.Element12, elements.Element12_0,
            elements.Element12_1, elements.Element12_2, elements.Element13, elements.Element13_Attributes, elements.Element13_0, elements.Element13_0_Attributes, elements.Element13_0_0, elements.Element13_0_0_0, elements.Element13_0_0_1, elements.Element13_1,
            elements.Element13_1_Attributes, elements.Element13_1_0, elements.Element13_1_0_0, elements.Element13_1_0_1, elements.Element13_2, elements.Element13_2_0, elements.Element13_2_0_0, elements.Element13_3, elements.Element13_3_0,
            elements.Element13_3_0_0, elements.Element14, elements.Element14_0, elements.Element14_0_0,  elements.Element14_0_0_0, elements.Element14_1, elements.Element14_1_0, elements.Element14_1_0_0, elements.Element15, elements.Element15_0,
            elements.Element15_0_0, elements.Element15_0_1, elements.Element15_0_2, elements.Element15_0_3, elements.Element16, elements.Element16_0, elements.Element16_0_0, elements.Element16_0_1, elements.Element16_0_2, elements.Element16_0_2_0,
            elements.Element16_1, elements.Element16_1_0, elements.Element17, elements.Element17_0, elements.Element17_1, elements.Element17_1_0, elements.Element17_1_1, elements.Element17_1_2, elements.Element17_1_3, elements.Element17_2, elements.Element17_3,
            elements.Element17_4, elements.Element17_5, elements.Element17_6, elements.Element18, elements.Element18_0, elements.Element18_0_0, elements.Element18_0_1, elements.Element18_0_2, elements.Element18_0_3, elements.Element18_1, elements.Element18_1_0,
            elements.Element18_2, elements.Element18_2_Attributes, elements.Element18_3, elements.Element18_3_0, elements.Element19, elements.Element19_0, elements.Element19_0_0, elements.Element19_0_0_0, elements.Element19_0_0_0_0, elements.Element19_0_1,
            elements.Element19_0_1_0, elements.Element19_0_1_0_0, elements.Element19_1, elements.Element19_1_0, elements.Element19_1_0_0, elements.Element19_1_0_0_0, elements.Element19_1_1, elements.Element19_1_1_0, elements.Element19_1_1_0_0,
            elements.Element19_2, elements.Element19_2_0, elements.Element19_2_0_0, elements.Element19_2_0_0_0, elements.Element19_2_1, elements.Element19_2_1_0, elements.Element19_2_1_0_0, elements.Element20, elements.Element20_Attributes, elements.Element20_0,
            elements.Element21, elements.Element21_0, elements.Element21_0_0, elements.Element21_0_0_0, elements.Element21_0_0_0_0, elements.Element21_0_1, elements.Element21_0_1_0, elements.Element21_0_1_0_0, elements.Element21_0_2, elements.Element21_0_2_0,
            elements.Element21_0_2_0_0, elements.Element21_1, elements.Element21_1_0, elements.Element21_1_0_0, elements.Element21_1_0_0_0, elements.Element21_1_1, elements.Element21_1_1_0, elements.Element21_1_1_0_0, elements.Element21_1_2,
            elements.Element21_1_2_0, elements.Element21_1_2_0_0, elements.Element21_2, elements.Element21_2_0, elements.Element21_2_0_0, elements.Element21_2_0_0_0, elements.Element21_2_1, elements.Element21_2_1_0, elements.Element21_2_1_0_0,
            elements.Element21_2_2, elements.Element21_2_2_0, elements.Element21_2_2_0_0, elements.Element22, elements.Element22_0, elements.Element22_0_0, elements.Element22_0_0_0, elements.Element22_1, elements.Element22_1_0, elements.Element23,
            elements.Element23_0, elements.Element23_0_0, elements.Element24, elements.Element24_0, elements.Element24_1, elements.Element24_1_0, elements.Element24_2, elements.Element24_3, elements.Element24_3_0, elements.Element24_4, elements.Element24_5,
            elements.Element24_5_Attributes, elements.Element25, elements.Element25_0, elements.Element25_1, elements.Element25_1_Attributes, elements.Element25_2, elements.Element25_3, elements.Element25_3_Attributes, elements.Element25_4, elements.Element26,
            elements.Element26_0, elements.Element26_0_0, elements.Element26_1, elements.Element26_2, elements.Element26_2_Attributes, elements.Element27, elements.Element27_0,  elements.Element27_1, elements.Element27_2, elements.Element28, elements.Element29,
            elements.Element29_0, elements.Element29_0_0, elements.Element29_1, elements.Element29_2, elements.Element29_2_0, elements.Element29_3, elements.Element30, elements.Element30_Attributes, elements.Element31, elements.Element31_Attributes,
            elements.Element32, elements.Element32_Attributes, elements.Element33, elements.Element33_Attributes, elements.Element34, elements.Element34_0, elements.Element34_0_0, elements.Element34_0_0_0, elements.Element34_0_1, elements.Element34_0_1_0,
            elements.Element34_0_1_1, elements.Element34_0_1_2, elements.Element34_1, elements.Element34_1_0, elements.Element34_1_0_0, elements.Element34_1_1, elements.Element34_1_1_0, elements.Element35, elements.Element35_0, elements.Element35_0_0,
            elements.Element35_0_1, elements.Element35_0_2, elements.Element36, elements.Element36_0, elements.Element36_1, elements.Element36_2, elements.Element36_3, elements.Element36_4, elements.Element36_5, elements.Element37, elements.Element37_0,
            elements.Element37_0_0, elements.Element37_0_0_0, elements.Element37_0_0_1, elements.Element37_1, elements.Element37_1_0, elements.Element37_1_0_0, elements.Element37_1_0_1];
        yield return new TestCaseData(document, null).Returns(expected).SetArgDisplayNames("(HeadingBlock)Document", "null");
        yield return new TestCaseData(document, false).Returns(expected).SetArgDisplayNames("(HeadingBlock)Document", "false");
        yield return new TestCaseData(document, true).Returns(withAttrReturns).SetArgDisplayNames("(HeadingBlock)Document", "true");

        expected = [elements.Element13, elements.Element13_0, elements.Element13_0_0, elements.Element13_0_0_0, elements.Element13_0_0_1, elements.Element13_1, elements.Element13_1_0, elements.Element13_1_0_0, elements.Element13_1_0_1, elements.Element13_2,
        elements.Element13_2_0, elements.Element13_2_0_0, elements.Element13_3, elements.Element13_3_0, elements.Element13_3_0_0];
        withAttrReturns = [elements.Element13, elements.Element13_Attributes, elements.Element13_0, elements.Element13_0_Attributes, elements.Element13_0_0, elements.Element13_0_0_0, elements.Element13_0_0_1, elements.Element13_1,
            elements.Element13_1_Attributes, elements.Element13_1_0, elements.Element13_1_0_0, elements.Element13_1_0_1, elements.Element13_2, elements.Element13_2_0, elements.Element13_2_0_0, elements.Element13_3, elements.Element13_3_0,
            elements.Element13_3_0_0];
        yield return new TestCaseData(elements.Element13, null).Returns(expected).SetArgDisplayNames("(ListBlock)Document[13]", "null");
        yield return new TestCaseData(elements.Element13, false).Returns(expected).SetArgDisplayNames("(ListBlock)Document[13]", "false");
        yield return new TestCaseData(elements.Element13, true).Returns(withAttrReturns).SetArgDisplayNames("(ListBlock)Document[13]", "true");


        expected = [elements.Element19_0, elements.Element19_0_0, elements.Element19_0_0_0, elements.Element19_0_0_0_0, elements.Element19_0_1, elements.Element19_0_1_0, elements.Element19_0_1_0_0, elements.Element19_1,
            elements.Element19_1_0, elements.Element19_1_0_0, elements.Element19_1_0_0_0, elements.Element19_1_1, elements.Element19_1_1_0, elements.Element19_1_1_0_0, elements.Element19_2, elements.Element19_2_0, elements.Element19_2_0_0,
            elements.Element19_2_0_0_0, elements.Element19_2_1, elements.Element19_2_1_0, elements.Element19_2_1_0_0];
        yield return new TestCaseData(elements.Element19, null).Returns(expected).SetArgDisplayNames("(Table)Document[19]", "null");
        yield return new TestCaseData(elements.Element19, false).Returns(expected).SetArgDisplayNames("(Table)Document[19]", "false");
        yield return new TestCaseData(elements.Element19, true).Returns(expected).SetArgDisplayNames("(Table)Document[19]", "true");

        expected = [elements.Element20, elements.Element20_0];
        withAttrReturns = [elements.Element20, elements.Element20_Attributes, elements.Element20_0];
        yield return new TestCaseData(document, null).Returns(expected).SetArgDisplayNames("(HeadingBlock)Document[20]", "null");
        yield return new TestCaseData(document, false).Returns(expected).SetArgDisplayNames("(HeadingBlock)Document[20]", "false");
        yield return new TestCaseData(document, true).Returns(withAttrReturns).SetArgDisplayNames("(HeadingBlock)Document[20]", "true");

        expected = [elements.Element25, elements.Element25_0, elements.Element25_1, elements.Element25_2, elements.Element25_3, elements.Element25_4];
        withAttrReturns = [elements.Element25, elements.Element25_0, elements.Element25_1, elements.Element25_1_Attributes, elements.Element25_2, elements.Element25_3, elements.Element25_3_Attributes, elements.Element25_4];
        yield return new TestCaseData(document, null).Returns(expected).SetArgDisplayNames("(ParagraphBlock)Document[25]", "null");
        yield return new TestCaseData(document, false).Returns(expected).SetArgDisplayNames("(ParagraphBlock)Document[25]", "false");
        yield return new TestCaseData(document, true).Returns(withAttrReturns).SetArgDisplayNames("(ParagraphBlock)Document[25]", "true");
    }

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