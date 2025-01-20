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

    /// <summary>
    /// Test cases for <see cref="MarkdownExtensionMethods.DescendantsAtDepth(MarkdownObject?, int, bool)"/>.
    /// </summary>
    public static System.Collections.IEnumerable GetDescendantsAtDepthTestData()
    {
        MarkdownDocument document = GetMarkdownDocument();
        var elements = new MarkdownElements(document);

        IEnumerable<MarkdownObject> expected = [];
        for (int depth = -1; depth < 2; depth++)
        {
            yield return new TestCaseData(document, depth, false).Returns(expected).SetArgDisplayNames("MarkdownDocument", depth.ToString(), "false");
            yield return new TestCaseData(document, depth, true).Returns(expected).SetArgDisplayNames("MarkdownDocument", depth.ToString(), "true");
        }

        expected = [elements.Element0, elements.Element1, elements.Element2, elements.Element3, elements.Element4, elements.Element5, elements.Element6, elements.Element7, elements.Element8, elements.Element9,
            elements.Element10, elements.Element11, elements.Element12, elements.Element13, elements.Element14, elements.Element15, elements.Element16, elements.Element17, elements.Element18, elements.Element19,
            elements.Element20, elements.Element21, elements.Element22, elements.Element23, elements.Element24, elements.Element25, elements.Element26, elements.Element27, elements.Element28, elements.Element29,
            elements.Element30, elements.Element31, elements.Element32, elements.Element33, elements.Element34, elements.Element35, elements.Element36, elements.Element37];
        yield return new TestCaseData(document, 1, false).Returns(expected).SetArgDisplayNames("MarkdownDocument", "1", "false");
        yield return new TestCaseData(document, 1, true).Returns(expected).SetArgDisplayNames("MarkdownDocument", "1", "true");

        expected = [elements.Element3_0_0, elements.Element5_5_0, elements.Element8_0_0, elements.Element9_1_0, elements.Element10_4_0, elements.Element13_0_0, elements.Element13_1_0, elements.Element13_2_0,
            elements.Element13_3_0, elements.Element14_0_0, elements.Element14_1_0, elements.Element15_0_0, elements.Element15_0_1, elements.Element15_0_2, elements.Element15_0_3, elements.Element16_0_0,
            elements.Element16_0_1, elements.Element16_0_2, elements.Element16_1_0, elements.Element17_1_0, elements.Element17_1_1, elements.Element17_1_2, elements.Element17_1_3, elements.Element18_0_0,
            elements.Element18_0_1, elements.Element18_0_2, elements.Element18_0_3, elements.Element18_1_0, elements.Element18_3_0, elements.Element19_0_0, elements.Element19_0_1, elements.Element19_1_0,
            elements.Element19_1_1, elements.Element19_2_0, elements.Element19_2_1, elements.Element21_0_0, elements.Element21_0_1, elements.Element21_0_2, elements.Element21_1_0, elements.Element21_1_1,
            elements.Element21_1_2, elements.Element21_2_0, elements.Element21_2_1, elements.Element21_2_2, elements.Element22_0_0, elements.Element22_1_0, elements.Element23_0_0, elements.Element24_1_0,
            elements.Element24_3_0, elements.Element26_0_0, elements.Element29_0_0, elements.Element29_2_0, elements.Element34_0_0, elements.Element34_0_1, elements.Element34_1_0, elements.Element34_1_1,
            elements.Element35_0_0, elements.Element35_0_1, elements.Element35_0_2, elements.Element37_0_0, elements.Element37_1_0_0];
        yield return new TestCaseData(document, 3, false).Returns(expected).SetArgDisplayNames("MarkdownDocument", "3", "false");

        expected = [elements.Element3_0_0, elements.Element4_2_Attributes, elements.Element5_5_0, elements.Element8_0_Attributes, elements.Element8_0_0, elements.Element9_1_0, elements.Element10_4_0,
            elements.Element13_0_Attributes, elements.Element13_0_0, elements.Element13_1_Attributes, elements.Element13_1_0, elements.Element13_2_0, elements.Element13_3_0, elements.Element14_0_0,
            elements.Element14_1_0, elements.Element15_0_0, elements.Element15_0_1, elements.Element15_0_2, elements.Element15_0_3, elements.Element16_0_0, elements.Element16_0_1, elements.Element16_0_2,
            elements.Element16_1_0, elements.Element17_1_0, elements.Element17_1_1, elements.Element17_1_2, elements.Element17_1_3, elements.Element18_0_0, elements.Element18_0_1, elements.Element18_0_2,
            elements.Element18_0_3, elements.Element18_1_0, elements.Element18_2_Attributes, elements.Element18_3_0, elements.Element19_0_0, elements.Element19_0_1, elements.Element19_1_0, elements.Element19_1_1,
            elements.Element19_2_0, elements.Element19_2_1, elements.Element21_0_0, elements.Element21_0_1, elements.Element21_0_2, elements.Element21_1_0, elements.Element21_1_1, elements.Element21_1_2,
            elements.Element21_2_0, elements.Element21_2_1, elements.Element21_2_2, elements.Element22_0_0, elements.Element22_1_0, elements.Element23_0_0, elements.Element24_1_0, elements.Element24_3_0,
            elements.Element24_5_Attributes, elements.Element25_1_Attributes, elements.Element25_3_Attributes, elements.Element26_0_0, elements.Element26_2_Attributes, elements.Element29_0_0, elements.Element29_2_0,
            elements.Element34_0_0, elements.Element34_0_1, elements.Element34_1_0, elements.Element34_1_1, elements.Element35_0_0, elements.Element35_0_1, elements.Element35_0_2, elements.Element37_0_0,
            elements.Element37_1_0_0];
        yield return new TestCaseData(document, 3, true).Returns(expected).SetArgDisplayNames("MarkdownDocument", "3", "true");
    }

    /// <summary>
    /// Test cases for <see cref="MarkdownExtensionMethods.DescendantsFromDepth(MarkdownObject?, int, bool)"/>.
    /// </summary>
    public static System.Collections.IEnumerable GetDescendantsFromDepthTestData()
    {
        MarkdownDocument document = GetMarkdownDocument();
        var elements = new MarkdownElements(document);

        IEnumerable<MarkdownObject> expected = [elements.Element0, elements.Element0_0, elements.Element1, elements.Element2, elements.Element2_0, elements.Element3, elements.Element3_0, elements.Element3_0_0, elements.Element4,
            elements.Element4_0, elements.Element4_1, elements.Element4_2, elements.Element5, elements.Element5_0, elements.Element5_1, elements.Element5_2, elements.Element5_3, elements.Element5_4, elements.Element5_5,
            elements.Element5_5_0, elements.Element5_6, elements.Element6, elements.Element6_0, elements.Element7, elements.Element7_0, elements.Element8, elements.Element8_0, elements.Element8_0_0, elements.Element8_1,
            elements.Element8_2, elements.Element9, elements.Element9_0, elements.Element9_1, elements.Element9_1_0, elements.Element9_2, elements.Element10, elements.Element10_0, elements.Element10_1, elements.Element10_2,
            elements.Element10_3, elements.Element10_4, elements.Element10_4_0, elements.Element11, elements.Element11_0, elements.Element11_1, elements.Element11_2, elements.Element12, elements.Element12_0, elements.Element12_1,
            elements.Element12_2, elements.Element13, elements.Element13_0, elements.Element13_0_0, elements.Element13_0_0_0, elements.Element13_0_0_1, elements.Element13_1, elements.Element13_1_0, elements.Element13_1_0_0,
            elements.Element13_1_0_1, elements.Element13_2, elements.Element13_2_0, elements.Element13_2_0_0, elements.Element13_3, elements.Element13_3_0, elements.Element13_3_0_0, elements.Element14, elements.Element14_0,
            elements.Element14_0_0, elements.Element14_0_0_0, elements.Element14_1, elements.Element14_1_0, elements.Element14_1_0_0, elements.Element15, elements.Element15_0, elements.Element15_0_0, elements.Element15_0_1,
            elements.Element15_0_2, elements.Element15_0_3, elements.Element16, elements.Element16_0, elements.Element16_0_0, elements.Element16_0_1, elements.Element16_0_2, elements.Element16_0_2_0, elements.Element16_1,
            elements.Element16_1_0, elements.Element17, elements.Element17_0, elements.Element17_1, elements.Element17_1_0, elements.Element17_1_1, elements.Element17_1_2, elements.Element17_1_3, elements.Element17_2, elements.Element17_3,
            elements.Element17_4, elements.Element17_5, elements.Element17_6, elements.Element18, elements.Element18_0, elements.Element18_0_0, elements.Element18_0_1, elements.Element18_0_2, elements.Element18_0_3, elements.Element18_1,
            elements.Element18_1_0, elements.Element18_2, elements.Element18_3, elements.Element18_3_0, elements.Element19, elements.Element19_0, elements.Element19_0_0, elements.Element19_0_0_0, elements.Element19_0_0_0_0,
            elements.Element19_0_1, elements.Element19_0_1_0, elements.Element19_0_1_0_0, elements.Element19_1, elements.Element19_1_0, elements.Element19_1_0_0, elements.Element19_1_0_0_0, elements.Element19_1_1, elements.Element19_1_1_0,
            elements.Element19_1_1_0_0, elements.Element19_2, elements.Element19_2_0, elements.Element19_2_0_0, elements.Element19_2_0_0_0, elements.Element19_2_1, elements.Element19_2_1_0, elements.Element19_2_1_0_0, elements.Element20,
            elements.Element20_0, elements.Element21, elements.Element21_0, elements.Element21_0_0, elements.Element21_0_0_0, elements.Element21_0_0_0_0, elements.Element21_0_1, elements.Element21_0_1_0, elements.Element21_0_1_0_0,
            elements.Element21_0_2, elements.Element21_0_2_0, elements.Element21_0_2_0_0, elements.Element21_1, elements.Element21_1_0, elements.Element21_1_0_0, elements.Element21_1_0_0_0, elements.Element21_1_1, elements.Element21_1_1_0,
            elements.Element21_1_1_0_0, elements.Element21_1_2, elements.Element21_1_2_0, elements.Element21_1_2_0_0, elements.Element21_2, elements.Element21_2_0, elements.Element21_2_0_0, elements.Element21_2_0_0_0,
            elements.Element21_2_1, elements.Element21_2_1_0, elements.Element21_2_1_0_0, elements.Element21_2_2, elements.Element21_2_2_0, elements.Element21_2_2_0_0, elements.Element22, elements.Element22_0, elements.Element22_0_0,
            elements.Element22_0_0_0, elements.Element22_1, elements.Element22_1_0, elements.Element23, elements.Element23_0, elements.Element23_0_0, elements.Element24, elements.Element24_0, elements.Element24_1, elements.Element24_1_0,
            elements.Element24_2, elements.Element24_3, elements.Element24_3_0, elements.Element24_4, elements.Element24_5, elements.Element25, elements.Element25_0, elements.Element25_1, elements.Element25_2, elements.Element25_3,
            elements.Element25_4, elements.Element26, elements.Element26_0, elements.Element26_0_0, elements.Element26_1, elements.Element26_2, elements.Element27, elements.Element27_0, elements.Element27_1, elements.Element27_2,
            elements.Element28, elements.Element29, elements.Element29_0, elements.Element29_0_0, elements.Element29_1, elements.Element29_2, elements.Element29_2_0, elements.Element29_3, elements.Element30, elements.Element31,
            elements.Element32, elements.Element33, elements.Element34, elements.Element34_0, elements.Element34_0_0, elements.Element34_0_0_0, elements.Element34_0_1, elements.Element34_0_1_0, elements.Element34_0_1_1,
            elements.Element34_0_1_2, elements.Element34_1, elements.Element34_1_0, elements.Element34_1_0_0, elements.Element34_1_1, elements.Element34_1_1_0, elements.Element35, elements.Element35_0, elements.Element35_0_0,
            elements.Element35_0_1, elements.Element35_0_2, elements.Element36, elements.Element36_0, elements.Element36_1, elements.Element36_2, elements.Element36_3, elements.Element36_4, elements.Element36_5, elements.Element37,
            elements.Element37_0, elements.Element37_0_0, elements.Element37_0_0_0, elements.Element37_0_0_1, elements.Element37_1, elements.Element37_1_0, elements.Element37_1_0_0, elements.Element37_1_0_1];
        for (int depth = -1; depth < 2; depth++)
            yield return new TestCaseData(document, depth, false).Returns(expected).SetArgDisplayNames("MarkdownDocument", depth.ToString(), "false");
        expected = [elements.Element0, elements.Element0_Attributes, elements.Element0_0, elements.Element1, elements.Element2, elements.Element2_0, elements.Element3, elements.Element3_0, elements.Element3_0_0, elements.Element4,
            elements.Element4_0, elements.Element4_1, elements.Element4_2, elements.Element4_2_Attributes, elements.Element5, elements.Element5_0, elements.Element5_1, elements.Element5_2, elements.Element5_3, elements.Element5_4,
            elements.Element5_5, elements.Element5_5_0, elements.Element5_6, elements.Element6, elements.Element6_Attributes, elements.Element6_0, elements.Element7, elements.Element7_Attributes, elements.Element7_0, elements.Element8,
            elements.Element8_0, elements.Element8_0_Attributes, elements.Element8_0_0, elements.Element8_1, elements.Element8_2, elements.Element9, elements.Element9_0, elements.Element9_1, elements.Element9_1_0, elements.Element9_2,
            elements.Element10, elements.Element10_0, elements.Element10_1, elements.Element10_2, elements.Element10_3, elements.Element10_4, elements.Element10_4_0, elements.Element11, elements.Element11_0, elements.Element11_1,
            elements.Element11_2, elements.Element12, elements.Element12_0, elements.Element12_1, elements.Element12_2, elements.Element13, elements.Element13_Attributes, elements.Element13_0, elements.Element13_0_Attributes,
            elements.Element13_0_0, elements.Element13_0_0_0, elements.Element13_0_0_1, elements.Element13_1, elements.Element13_1_Attributes, elements.Element13_1_0, elements.Element13_1_0_0, elements.Element13_1_0_1,
            elements.Element13_2, elements.Element13_2_0, elements.Element13_2_0_0, elements.Element13_3, elements.Element13_3_0, elements.Element13_3_0_0, elements.Element14, elements.Element14_0, elements.Element14_0_0,
            elements.Element14_0_0_0, elements.Element14_1, elements.Element14_1_0, elements.Element14_1_0_0, elements.Element15, elements.Element15_0, elements.Element15_0_0, elements.Element15_0_1, elements.Element15_0_2,
            elements.Element15_0_3, elements.Element16, elements.Element16_0, elements.Element16_0_0, elements.Element16_0_1, elements.Element16_0_2, elements.Element16_0_2_0, elements.Element16_1, elements.Element16_1_0,
            elements.Element17, elements.Element17_0, elements.Element17_1, elements.Element17_1_0, elements.Element17_1_1, elements.Element17_1_2, elements.Element17_1_3, elements.Element17_2, elements.Element17_3, elements.Element17_4,
            elements.Element17_5, elements.Element17_6, elements.Element18, elements.Element18_0, elements.Element18_0_0, elements.Element18_0_1, elements.Element18_0_2, elements.Element18_0_3, elements.Element18_1, elements.Element18_1_0,
            elements.Element18_2, elements.Element18_2_Attributes, elements.Element18_3, elements.Element18_3_0, elements.Element19, elements.Element19_0, elements.Element19_0_0, elements.Element19_0_0_0, elements.Element19_0_0_0_0,
            elements.Element19_0_1, elements.Element19_0_1_0, elements.Element19_0_1_0_0, elements.Element19_1, elements.Element19_1_0, elements.Element19_1_0_0, elements.Element19_1_0_0_0, elements.Element19_1_1, elements.Element19_1_1_0,
            elements.Element19_1_1_0_0, elements.Element19_2, elements.Element19_2_0, elements.Element19_2_0_0, elements.Element19_2_0_0_0, elements.Element19_2_1, elements.Element19_2_1_0, elements.Element19_2_1_0_0, elements.Element20,
            elements.Element20_Attributes, elements.Element20_0, elements.Element21, elements.Element21_0, elements.Element21_0_0, elements.Element21_0_0_0, elements.Element21_0_0_0_0, elements.Element21_0_1, elements.Element21_0_1_0,
            elements.Element21_0_1_0_0, elements.Element21_0_2, elements.Element21_0_2_0, elements.Element21_0_2_0_0, elements.Element21_1, elements.Element21_1_0, elements.Element21_1_0_0, elements.Element21_1_0_0_0,
            elements.Element21_1_1, elements.Element21_1_1_0, elements.Element21_1_1_0_0, elements.Element21_1_2, elements.Element21_1_2_0, elements.Element21_1_2_0_0, elements.Element21_2, elements.Element21_2_0, elements.Element21_2_0_0,
            elements.Element21_2_0_0_0, elements.Element21_2_1, elements.Element21_2_1_0, elements.Element21_2_1_0_0, elements.Element21_2_2, elements.Element21_2_2_0, elements.Element21_2_2_0_0, elements.Element22, elements.Element22_0,
            elements.Element22_0_0, elements.Element22_0_0_0, elements.Element22_1, elements.Element22_1_0, elements.Element23, elements.Element23_0, elements.Element23_0_0, elements.Element24, elements.Element24_0, elements.Element24_1,
            elements.Element24_1_0, elements.Element24_2, elements.Element24_3, elements.Element24_3_0, elements.Element24_4, elements.Element24_5, elements.Element24_5_Attributes, elements.Element25, elements.Element25_0,
            elements.Element25_1, elements.Element25_1_Attributes, elements.Element25_2, elements.Element25_3, elements.Element25_3_Attributes, elements.Element25_4, elements.Element26, elements.Element26_0, elements.Element26_0_0,
            elements.Element26_1, elements.Element26_2, elements.Element26_2_Attributes, elements.Element27, elements.Element27_0, elements.Element27_1, elements.Element27_2, elements.Element28, elements.Element29, elements.Element29_0,
            elements.Element29_0_0, elements.Element29_1, elements.Element29_2, elements.Element29_2_0, elements.Element29_3, elements.Element30, elements.Element30_Attributes, elements.Element31, elements.Element31_Attributes,
            elements.Element32, elements.Element32_Attributes, elements.Element33, elements.Element33_Attributes, elements.Element34, elements.Element34_0, elements.Element34_0_0, elements.Element34_0_0_0, elements.Element34_0_1,
            elements.Element34_0_1_0, elements.Element34_0_1_1, elements.Element34_0_1_2, elements.Element34_1, elements.Element34_1_0, elements.Element34_1_0_0, elements.Element34_1_1, elements.Element34_1_1_0, elements.Element35,
            elements.Element35_0, elements.Element35_0_0, elements.Element35_0_1, elements.Element35_0_2, elements.Element36, elements.Element36_0, elements.Element36_1, elements.Element36_2, elements.Element36_3, elements.Element36_4,
            elements.Element36_5, elements.Element37, elements.Element37_0, elements.Element37_0_0, elements.Element37_0_0_0, elements.Element37_0_0_1, elements.Element37_1, elements.Element37_1_0, elements.Element37_1_0_0,
            elements.Element37_1_0_1];
        for (int depth = -1; depth < 2; depth++)
            yield return new TestCaseData(document, depth, true).Returns(expected)
                .SetArgDisplayNames("MarkdownDocument", depth.ToString(), "true");
    }
}