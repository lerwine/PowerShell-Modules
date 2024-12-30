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
    /// Test cases for <see cref="MarkdownExtensionMethods.DescendantBranchesMatchingType(MarkdownObject?, Type)"/>.
    /// </summary>
    /// <returns></returns>
    public static System.Collections.IEnumerable GetDescendantBranchesMatchingType1TestData()
    {
        MarkdownDocument document = GetMarkdownDocument();
        var elements = new MarkdownElements(document);
        IEnumerable<MarkdownObject> expected = [elements.Element0, elements.Element1, elements.Element2, elements.Element3, elements.Element4, elements.Element5, elements.Element6, elements.Element7, elements.Element8, elements.Element9,
            elements.Element10, elements.Element11, elements.Element12, elements.Element13, elements.Element14, elements.Element15, elements.Element16, elements.Element17, elements.Element18, elements.Element19, elements.Element20,
            elements.Element21, elements.Element22, elements.Element23, elements.Element24, elements.Element25, elements.Element26, elements.Element27, elements.Element28, elements.Element29, elements.Element30, elements.Element31,
            elements.Element32, elements.Element33, elements.Element34, elements.Element35, elements.Element36, elements.Element37];
        yield return new TestCaseData(document, typeof(MarkdownObject)).Returns(expected).SetArgDisplayNames("Document", "MarkdownObject");
    }

    /// <summary>
    /// Test cases for <see cref="MarkdownExtensionMethods.DescendantBranchesMatchingType(MarkdownObject?, IEnumerable{Type})"/>.
    /// </summary>
    /// <returns></returns>
    public static System.Collections.IEnumerable GetDescendantBranchesMatchingType2TestData()
    {
        MarkdownDocument document = GetMarkdownDocument();
        var elements = new MarkdownElements(document);
        IEnumerable<Type> types = [typeof(ContainerInline), typeof(ContainerBlock)];
        IEnumerable<MarkdownObject> expected = [elements.Element3_0, elements.Element5_5, elements.Element8_0, elements.Element9_1, elements.Element10_4, elements.Element13, elements.Element14, elements.Element15, elements.Element16,
            elements.Element17_1, elements.Element18, elements.Element19, elements.Element21, elements.Element22, elements.Element23_0, elements.Element24_1, elements.Element24_3, elements.Element26_0, elements.Element29_0,
            elements.Element29_2, elements.Element34, elements.Element35, elements.Element36, elements.Element37];
        yield return new TestCaseData(document, types).Returns(expected).SetArgDisplayNames("Document", $"[{string.Join(", ", types.Select(t => t.Name))}]");
    }
/// 
    /// <summary>
    /// Test cases for <see cref="MarkdownExtensionMethods.DescendantBranchesMatchingType(MarkdownObject?, Type, int)"/>.
    /// </summary>
    /// <returns></returns>
    public static System.Collections.IEnumerable GetDescendantBranchesMatchingType3TestData()
    {
        MarkdownDocument document = GetMarkdownDocument();
        var elements = new MarkdownElements(document);
        
        IEnumerable<MarkdownObject> expected = [];
        Type type = typeof(MarkdownDocument);
        Type type2 = typeof(ContainerInline);
        for (int maximumDepth = -1; maximumDepth < 2; maximumDepth++)
        {
            yield return new TestCaseData(document, type, maximumDepth).Returns(expected) .SetArgDisplayNames("MarkdownDocument", $"typeof({type.FullName})", maximumDepth.ToString());
            yield return new TestCaseData(document, type2, maximumDepth).Returns(expected) .SetArgDisplayNames("MarkdownDocument", $"typeof({type2.FullName})", maximumDepth.ToString());
        }

        type = typeof(ParagraphBlock);
        expected = [elements.Element2, elements.Element3, elements.Element4, elements.Element5, elements.Element8, elements.Element9, elements.Element10, elements.Element11, elements.Element12,
            elements.Element23, elements.Element17, elements.Element24, elements.Element25, elements.Element26, elements.Element27, elements.Element29];
        yield return new TestCaseData(document, type, 1).Returns(expected).SetArgDisplayNames("MarkdownDocument", $"typeof({type.FullName})", "1");

        expected = [elements.Element2, elements.Element3, elements.Element4, elements.Element5, elements.Element8, elements.Element9, elements.Element10, elements.Element11, elements.Element12,
            elements.Element15_0, elements.Element16_0, elements.Element16_1, elements.Element17, elements.Element18_0, elements.Element18_1, elements.Element18_3, elements.Element22_0, elements.Element23,
            elements.Element24, elements.Element25, elements.Element26, elements.Element27, elements.Element29, elements.Element35_0];
        yield return new TestCaseData(document, type, 2).Returns(expected).SetArgDisplayNames("MarkdownDocument", $"typeof({type.FullName})", "2");

        type = typeof(ContainerInline);
        expected = [elements.Element3_0, elements.Element5_5, elements.Element8_0, elements.Element9_1, elements.Element10_4, elements.Element16_0_2, elements.Element17_1, elements.Element22_0_0, elements.Element23_0,
            elements.Element24_1, elements.Element24_3, elements.Element26_0, elements.Element29_0, elements.Element29_2];
        yield return new TestCaseData(document, type, 2).Returns(expected).SetArgDisplayNames("MarkdownDocument", $"typeof({type.FullName})", "2");

        type = typeof(LeafBlock);
        expected = [elements.Element0, elements.Element1, elements.Element2, elements.Element3, elements.Element4, elements.Element5, elements.Element6, elements.Element7, elements.Element8, elements.Element9,
            elements.Element10, elements.Element11, elements.Element12, elements.Element13_0_0, elements.Element13_1_0, elements.Element13_2_0, elements.Element13_3_0, elements.Element14_0_0, elements.Element14_1_0,
            elements.Element15_0, elements.Element16_0, elements.Element16_1, elements.Element17, elements.Element18_0, elements.Element18_1, elements.Element18_2, elements.Element18_3, elements.Element20,
            elements.Element22_0, elements.Element22_1, elements.Element23, elements.Element24, elements.Element25, elements.Element26, elements.Element27, elements.Element28, elements.Element29, elements.Element30,
            elements.Element31, elements.Element32, elements.Element33, elements.Element34_0_0, elements.Element34_0_1, elements.Element34_1_0, elements.Element34_1_1, elements.Element35_0, elements.Element36_0,
            elements.Element36_1, elements.Element36_2, elements.Element36_3, elements.Element36_4, elements.Element36_5, elements.Element37_0_0, elements.Element37_1_0];
        yield return new TestCaseData(document, type, 3).Returns(expected).SetArgDisplayNames("MarkdownDocument", $"typeof({type.FullName})", "3");

    }

    /// <summary>
    /// Test cases for <see cref="MarkdownExtensionMethods.DescendantBranchesMatchingType(MarkdownObject?, IEnumerable{Type}, int)"/>.
    /// </summary>
    /// <returns></returns>
    public static System.Collections.IEnumerable GetDescendantBranchesMatchingType4TestData()
    {
        MarkdownDocument document = GetMarkdownDocument();
        var elements = new MarkdownElements(document);
        
        IEnumerable<MarkdownObject> expected = [];
        Type[] types = [typeof(MarkdownDocument), typeof(ContainerInline)];
        string display = "[" + string.Join(", ", types.Select(t => $"typeof({t.Name})")) + "]";
        for (int maximumDepth = -1; maximumDepth < 2; maximumDepth++)
            yield return new TestCaseData(document, types, maximumDepth).Returns(expected)
                .SetArgDisplayNames("MarkdownDocument", display, maximumDepth.ToString());

        types = [typeof(ParagraphBlock), typeof(LiteralInline)];
        display = "[" + string.Join(", ", types.Select(t => $"typeof({t.Name})")) + "]";
        expected = [elements.Element2, elements.Element3, elements.Element4, elements.Element5, elements.Element8, elements.Element9, elements.Element10, elements.Element11, elements.Element12, elements.Element17,
            elements.Element23, elements.Element24, elements.Element25, elements.Element26, elements.Element27, elements.Element29];
        yield return new TestCaseData(document, types, 1).Returns(expected).SetArgDisplayNames("MarkdownDocument", display, "1");
        
        expected = [elements.Element0_0, elements.Element2, elements.Element4, elements.Element5, elements.Element6_0, elements.Element7_0, elements.Element8, elements.Element9, elements.Element10, elements.Element11,
            elements.Element12, elements.Element13_0_0, elements.Element13_1_0, elements.Element13_2_0, elements.Element13_3_0, elements.Element14_0_0, elements.Element14_1_0, elements.Element15_0, elements.Element16_0,
            elements.Element16_1, elements.Element17, elements.Element18_1, elements.Element18_3, elements.Element20_0, elements.Element22_0, elements.Element22_1_0, elements.Element23, elements.Element24, elements.Element25,
            elements.Element26, elements.Element27, elements.Element29, elements.Element34_0_1, elements.Element34_1_1, elements.Element35_0, elements.Element37_0_0, elements.Element37_1_0];
        yield return new TestCaseData(document, types, 3).Returns(expected) .SetArgDisplayNames("MarkdownDocument", display, "3");
        
        types = [typeof(ParagraphBlock), typeof(ContainerBlock), typeof(LiteralInline)];
        display = "[" + string.Join(", ", types.Select(t => $"typeof({t.Name})")) + "]";
        expected = [elements.Element0_0, elements.Element2_0, elements.Element3_0, elements.Element4_0, elements.Element5_0, elements.Element5_2, elements.Element5_4, elements.Element5_5, elements.Element5_6,
            elements.Element6_0, elements.Element7_0, elements.Element8_0, elements.Element9_0, elements.Element9_1, elements.Element9_2, elements.Element10_4, elements.Element11_0, elements.Element11_2, elements.Element12_0,
            elements.Element12_2, elements.Element13, elements.Element14, elements.Element15, elements.Element16, elements.Element17_0, elements.Element17_1_0, elements.Element17_1_1, elements.Element17_1_2,
            elements.Element17_1_3, elements.Element17_3, elements.Element17_5, elements.Element17_6, elements.Element18, elements.Element20_0, elements.Element22_0_0, elements.Element22_1_0, elements.Element23_0,
            elements.Element24_0, elements.Element24_1_0, elements.Element24_2, elements.Element24_3_0, elements.Element24_4, elements.Element25_0, elements.Element25_2, elements.Element25_4, elements.Element26_0_0,
            elements.Element27_0, elements.Element27_2, elements.Element29_0_0, elements.Element29_1, elements.Element29_2_0, elements.Element29_3, elements.Element35_0_0, elements.Element35_0_2, elements.Element36];
        yield return new TestCaseData(document, types, 3).Returns(expected) .SetArgDisplayNames("MarkdownDocument", display, "3");
    }

    /// <summary>
    /// Test cases for <see cref="MarkdownExtensionMethods.DescendantsAtDepth(MarkdownObject?, int, bool)"/>.
    /// </summary>
    /// <returns></returns>
    public static System.Collections.IEnumerable GetDescendantsAtDepthTestData()
    {
        MarkdownDocument document = GetMarkdownDocument();
        var elements = new MarkdownElements(document);
        
        IEnumerable<MarkdownObject> expected = [];
        for (int depth = -1; depth < 2; depth++)
        {
            yield return new TestCaseData(document, depth, null).Returns(expected)
                .SetArgDisplayNames("MarkdownDocument", depth.ToString(), "null");
            yield return new TestCaseData(document, depth, false).Returns(expected)
                .SetArgDisplayNames("MarkdownDocument", depth.ToString(), "false");
            yield return new TestCaseData(document, depth, true).Returns(expected)
                .SetArgDisplayNames("MarkdownDocument", depth.ToString(), "true");
        }

        expected = [elements.Element0, elements.Element1, elements.Element2, elements.Element3, elements.Element4, elements.Element5, elements.Element6, elements.Element7, elements.Element8, elements.Element9,
            elements.Element10, elements.Element11, elements.Element12, elements.Element13, elements.Element14, elements.Element15, elements.Element16, elements.Element17, elements.Element18, elements.Element19,
            elements.Element20, elements.Element21, elements.Element22, elements.Element23, elements.Element24, elements.Element25, elements.Element26, elements.Element27, elements.Element28, elements.Element29,
            elements.Element30, elements.Element31, elements.Element32, elements.Element33, elements.Element34, elements.Element35, elements.Element36, elements.Element37];
        yield return new TestCaseData(document, 1, null).Returns(expected) .SetArgDisplayNames("MarkdownDocument", "1", "null");
        yield return new TestCaseData(document, 1, false).Returns(expected) .SetArgDisplayNames("MarkdownDocument", "1", "false");
        yield return new TestCaseData(document, 1, true).Returns(expected) .SetArgDisplayNames("MarkdownDocument", "1", "true");

        expected = [elements.Element3_0_0, elements.Element5_5_0, elements.Element8_0_0, elements.Element9_1_0, elements.Element10_4_0, elements.Element13_0_0, elements.Element13_1_0, elements.Element13_2_0,
            elements.Element13_3_0, elements.Element14_0_0, elements.Element14_1_0, elements.Element15_0_0, elements.Element15_0_1, elements.Element15_0_2, elements.Element15_0_3, elements.Element16_0_0,
            elements.Element16_0_1, elements.Element16_0_2, elements.Element16_1_0, elements.Element17_1_0, elements.Element17_1_1, elements.Element17_1_2, elements.Element17_1_3, elements.Element18_0_0,
            elements.Element18_0_1, elements.Element18_0_2, elements.Element18_0_3, elements.Element18_1_0, elements.Element18_3_0, elements.Element19_0_0, elements.Element19_0_1, elements.Element19_1_0,
            elements.Element19_1_1, elements.Element19_2_0, elements.Element19_2_1, elements.Element21_0_0, elements.Element21_0_1, elements.Element21_0_2, elements.Element21_1_0, elements.Element21_1_1,
            elements.Element21_1_2, elements.Element21_2_0, elements.Element21_2_1, elements.Element21_2_2, elements.Element22_0_0, elements.Element22_1_0, elements.Element23_0_0, elements.Element24_1_0,
            elements.Element24_3_0, elements.Element26_0_0, elements.Element29_0_0, elements.Element29_2_0, elements.Element34_0_0, elements.Element34_0_1, elements.Element34_1_0, elements.Element34_1_1,
            elements.Element35_0_0, elements.Element35_0_1, elements.Element35_0_2, elements.Element37_0_0, elements.Element37_1_0_0];
        yield return new TestCaseData(document, 3, null).Returns(expected) .SetArgDisplayNames("MarkdownDocument", "3", "null");
        yield return new TestCaseData(document, 3, false).Returns(expected) .SetArgDisplayNames("MarkdownDocument", "3", "false");

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
        yield return new TestCaseData(document, 3, true).Returns(expected) .SetArgDisplayNames("MarkdownDocument", "3", "true");
    }

    /// <summary>
    /// Test cases for <see cref="MarkdownExtensionMethods.DescendantsFromDepth(MarkdownObject?, int, bool)"/>.
    /// </summary>
    /// <returns></returns>
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
        {
            yield return new TestCaseData(document, depth, null).Returns(expected)
                .SetArgDisplayNames("MarkdownDocument", depth.ToString(), "null");
            yield return new TestCaseData(document, depth, false).Returns(expected)
                .SetArgDisplayNames("MarkdownDocument", depth.ToString(), "false");
        }
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

    /// <summary>
    /// Test cases for <see cref="MarkdownExtensionMethods.DescendantsUpToDepth(MarkdownObject?, int, bool)"/>.
    /// </summary>
    /// <returns></returns>
    public static System.Collections.IEnumerable GetDescendantsUpToDepthTestData()
    {
        MarkdownDocument document = GetMarkdownDocument();
        var elements = new MarkdownElements(document);
        
        IEnumerable<MarkdownObject> expected = [];
        yield return new TestCaseData(document, -1, null).Returns(expected).SetArgDisplayNames("MarkdownDocument", "-1", "null");
        yield return new TestCaseData(document, -1, false).Returns(expected) .SetArgDisplayNames("MarkdownDocument", "-1", "false");
        yield return new TestCaseData(document, -1, true).Returns(expected).SetArgDisplayNames("MarkdownDocument", "-1", "true");
        yield return new TestCaseData(document, 0, null).Returns(expected).SetArgDisplayNames("MarkdownDocument", "0", "null");
        yield return new TestCaseData(document, 0, false).Returns(expected) .SetArgDisplayNames("MarkdownDocument", "0", "false");
        yield return new TestCaseData(document, 0, true).Returns(expected).SetArgDisplayNames("MarkdownDocument", "0", "true");

        expected = [elements.Element0, elements.Element1, elements.Element2, elements.Element3, elements.Element4, elements.Element5, elements.Element6, elements.Element7, elements.Element8, elements.Element9, elements.Element10,
            elements.Element11, elements.Element12, elements.Element13, elements.Element14, elements.Element15, elements.Element16, elements.Element17, elements.Element18, elements.Element19, elements.Element20, elements.Element21,
            elements.Element22, elements.Element23, elements.Element24, elements.Element25, elements.Element26, elements.Element27, elements.Element28, elements.Element29, elements.Element30, elements.Element31, elements.Element32,
            elements.Element33, elements.Element34, elements.Element35, elements.Element36, elements.Element37];
        yield return new TestCaseData(document, 1, null).Returns(expected).SetArgDisplayNames("MarkdownDocument", "1", "null");
        yield return new TestCaseData(document, 1, false).Returns(expected) .SetArgDisplayNames("MarkdownDocument", "1", "false");
        yield return new TestCaseData(document, 1, true).Returns(expected).SetArgDisplayNames("MarkdownDocument", "1", "true");
        expected = [elements.Element0, elements.Element0_0, elements.Element1, elements.Element2, elements.Element2_0, elements.Element3, elements.Element3_0, elements.Element4, elements.Element4_0, elements.Element4_1,
            elements.Element4_2, elements.Element5, elements.Element5_0, elements.Element5_1, elements.Element5_2, elements.Element5_3, elements.Element5_4, elements.Element5_5, elements.Element5_6, elements.Element6, elements.Element6_0,
            elements.Element7, elements.Element7_0, elements.Element8, elements.Element8_0, elements.Element8_1, elements.Element8_2, elements.Element9, elements.Element9_0, elements.Element9_1, elements.Element9_2, elements.Element10,
            elements.Element10_0, elements.Element10_1, elements.Element10_2, elements.Element10_3, elements.Element10_4, elements.Element11, elements.Element11_0, elements.Element11_1, elements.Element11_2, elements.Element12,
            elements.Element12_0, elements.Element12_1, elements.Element12_2, elements.Element13, elements.Element13_0, elements.Element13_1, elements.Element13_2, elements.Element13_3, elements.Element14, elements.Element14_0,
            elements.Element14_1, elements.Element15, elements.Element15_0, elements.Element16, elements.Element16_0, elements.Element16_1, elements.Element17, elements.Element17_0, elements.Element17_1, elements.Element17_2,
            elements.Element17_3, elements.Element17_4, elements.Element17_5, elements.Element17_6, elements.Element18, elements.Element18_0, elements.Element18_1, elements.Element18_2, elements.Element18_3, elements.Element19,
            elements.Element19_0, elements.Element19_1, elements.Element19_2, elements.Element20, elements.Element20_0, elements.Element21, elements.Element21_0, elements.Element21_1, elements.Element21_2, elements.Element22,
            elements.Element22_0, elements.Element22_1, elements.Element23, elements.Element23_0, elements.Element24, elements.Element24_0, elements.Element24_1, elements.Element24_2, elements.Element24_3, elements.Element24_4,
            elements.Element24_5, elements.Element25, elements.Element25_0, elements.Element25_1, elements.Element25_2, elements.Element25_3, elements.Element25_4, elements.Element26, elements.Element26_0, elements.Element26_1,
            elements.Element26_2, elements.Element27, elements.Element27_0, elements.Element27_1, elements.Element27_2, elements.Element28, elements.Element29, elements.Element29_0, elements.Element29_1, elements.Element29_2,
            elements.Element29_3, elements.Element30, elements.Element31, elements.Element32, elements.Element33, elements.Element34, elements.Element34_0, elements.Element34_1, elements.Element35, elements.Element35_0, elements.Element36,
            elements.Element36_0, elements.Element36_1, elements.Element36_2, elements.Element36_3, elements.Element36_4, elements.Element36_5, elements.Element37, elements.Element37_0, elements.Element37_1];
        yield return new TestCaseData(document, 2, null).Returns(expected).SetArgDisplayNames("MarkdownDocument", "2", "null");
        yield return new TestCaseData(document, 2, false).Returns(expected) .SetArgDisplayNames("MarkdownDocument", "2", "false");

        expected = [elements.Element0, elements.Element0_Attributes, elements.Element0_0, elements.Element1, elements.Element2, elements.Element2_0, elements.Element3, elements.Element3_0, elements.Element4, elements.Element4_0,
            elements.Element4_1, elements.Element4_2, elements.Element5, elements.Element5_0, elements.Element5_1, elements.Element5_2, elements.Element5_3, elements.Element5_4, elements.Element5_5, elements.Element5_6, elements.Element6,
            elements.Element6_Attributes, elements.Element6_0, elements.Element7, elements.Element7_Attributes, elements.Element7_0, elements.Element8, elements.Element8_0, elements.Element8_1, elements.Element8_2, elements.Element9,
            elements.Element9_0, elements.Element9_1, elements.Element9_2, elements.Element10, elements.Element10_0, elements.Element10_1, elements.Element10_2, elements.Element10_3, elements.Element10_4, elements.Element11,
            elements.Element11_0, elements.Element11_1, elements.Element11_2, elements.Element12, elements.Element12_0, elements.Element12_1, elements.Element12_2, elements.Element13, elements.Element13_Attributes, elements.Element13_0,
            elements.Element13_1, elements.Element13_2, elements.Element13_3, elements.Element14, elements.Element14_0, elements.Element14_1, elements.Element15, elements.Element15_0, elements.Element16, elements.Element16_0, 
            elements.Element16_1, elements.Element17, elements.Element17_0, elements.Element17_1, elements.Element17_2, elements.Element17_3, elements.Element17_4, elements.Element17_5, elements.Element17_6, elements.Element18,
            elements.Element18_0, elements.Element18_1, elements.Element18_2, elements.Element18_3, elements.Element19, elements.Element19_0, elements.Element19_1, elements.Element19_2, elements.Element20, elements.Element20_Attributes,
            elements.Element20_0, elements.Element21, elements.Element21_0, elements.Element21_1, elements.Element21_2, elements.Element22, elements.Element22_0, elements.Element22_1, elements.Element23, elements.Element23_0,
            elements.Element24, elements.Element24_0, elements.Element24_1, elements.Element24_2, elements.Element24_3, elements.Element24_4, elements.Element24_5, elements.Element25, elements.Element25_0, elements.Element25_1,
            elements.Element25_2, elements.Element25_3, elements.Element25_4, elements.Element26, elements.Element26_0, elements.Element26_1, elements.Element26_2, elements.Element27, elements.Element27_0, elements.Element27_1,
            elements.Element27_2, elements.Element28, elements.Element29, elements.Element29_0, elements.Element29_1, elements.Element29_2, elements.Element29_3, elements.Element30, elements.Element30_Attributes, elements.Element31,
            elements.Element31_Attributes, elements.Element32, elements.Element32_Attributes, elements.Element33, elements.Element33_Attributes, elements.Element34, elements.Element34_0, elements.Element34_1, elements.Element35,
            elements.Element35_0, elements.Element36, elements.Element36_0, elements.Element36_1, elements.Element36_2, elements.Element36_3, elements.Element36_4, elements.Element36_5, elements.Element37, elements.Element37_0,
            elements.Element37_1];
        yield return new TestCaseData(document, 2, true).Returns(expected).SetArgDisplayNames("MarkdownDocument", "2", "true");

        expected = [elements.Element0, elements.Element0_0, elements.Element1, elements.Element2, elements.Element2_0, elements.Element3, elements.Element3_0, elements.Element3_0_0, elements.Element4, elements.Element4_0,
            elements.Element4_1, elements.Element4_2, elements.Element5, elements.Element5_0, elements.Element5_1, elements.Element5_2, elements.Element5_3, elements.Element5_4, elements.Element5_5, elements.Element5_5_0,
            elements.Element5_6, elements.Element6, elements.Element6_0, elements.Element7, elements.Element7_0, elements.Element8, elements.Element8_0, elements.Element8_0_0, elements.Element8_1, elements.Element8_2, elements.Element9,
            elements.Element9_0, elements.Element9_1, elements.Element9_1_0, elements.Element9_2, elements.Element10, elements.Element10_0, elements.Element10_1, elements.Element10_2, elements.Element10_3, elements.Element10_4,
            elements.Element10_4_0, elements.Element11, elements.Element11_0, elements.Element11_1, elements.Element11_2, elements.Element12, elements.Element12_0, elements.Element12_1, elements.Element12_2, elements.Element13,
            elements.Element13_0, elements.Element13_0_0, elements.Element13_0_0_0, elements.Element13_0_0_1, elements.Element13_1, elements.Element13_1_0, elements.Element13_1_0_0, elements.Element13_1_0_1, elements.Element13_2,
            elements.Element13_2_0, elements.Element13_2_0_0, elements.Element13_3, elements.Element13_3_0, elements.Element13_3_0_0, elements.Element14, elements.Element14_0, elements.Element14_0_0, elements.Element14_0_0_0,
            elements.Element14_1, elements.Element14_1_0, elements.Element14_1_0_0, elements.Element15, elements.Element15_0, elements.Element15_0_0, elements.Element15_0_1, elements.Element15_0_2, elements.Element15_0_3,
            elements.Element16, elements.Element16_0, elements.Element16_0_0, elements.Element16_0_1, elements.Element16_0_2, elements.Element16_0_2_0, elements.Element16_1, elements.Element16_1_0,
            elements.Element17, elements.Element17_0, elements.Element17_1, elements.Element17_1_0, elements.Element17_1_1, elements.Element17_1_2, elements.Element17_1_3, elements.Element17_2, elements.Element17_3, elements.Element17_4,
            elements.Element17_5, elements.Element17_6, elements.Element18, elements.Element18_0, elements.Element18_0_0, elements.Element18_0_1, elements.Element18_0_2, elements.Element18_0_3, elements.Element18_1, elements.Element18_1_0,
            elements.Element18_2, elements.Element18_3, elements.Element18_3_0, elements.Element19, elements.Element19_0, elements.Element19_0_0, elements.Element19_0_0_0, elements.Element19_0_1, elements.Element19_0_1_0,
            elements.Element19_1, elements.Element19_1_0, elements.Element19_1_0_0, elements.Element19_1_1, elements.Element19_1_1_0, elements.Element19_2, elements.Element19_2_0, elements.Element19_2_0_0, elements.Element19_2_1,
            elements.Element19_2_1_0, elements.Element20, elements.Element20_0, elements.Element21, elements.Element21_0, elements.Element21_0_0, elements.Element21_0_0_0, elements.Element21_0_1, elements.Element21_0_1_0,
            elements.Element21_0_2, elements.Element21_0_2_0, elements.Element21_1, elements.Element21_1_0, elements.Element21_1_0_0, elements.Element21_1_1, elements.Element21_1_1_0, elements.Element21_1_2, elements.Element21_1_2_0,
            elements.Element21_2, elements.Element21_2_0, elements.Element21_2_0_0, elements.Element21_2_1, elements.Element21_2_1_0, elements.Element21_2_2, elements.Element21_2_2_0, elements.Element22, elements.Element22_0,
            elements.Element22_0_0, elements.Element22_0_0_0, elements.Element22_1, elements.Element22_1_0, elements.Element23, elements.Element23_0, elements.Element23_0_0, elements.Element24, elements.Element24_0, elements.Element24_1,
            elements.Element24_1_0, elements.Element24_2, elements.Element24_3, elements.Element24_3_0, elements.Element24_4, elements.Element24_5, elements.Element25, elements.Element25_0, elements.Element25_1, elements.Element25_2,
            elements.Element25_3, elements.Element25_4, elements.Element26, elements.Element26_0, elements.Element26_0_0, elements.Element26_1, elements.Element26_2, elements.Element27, elements.Element27_0, elements.Element27_1,
            elements.Element27_2, elements.Element28, elements.Element29, elements.Element29_0, elements.Element29_0_0, elements.Element29_1, elements.Element29_2, elements.Element29_2_0, elements.Element29_3, elements.Element30,
            elements.Element31, elements.Element32, elements.Element33, elements.Element34, elements.Element34_0, elements.Element34_0_0, elements.Element34_0_0_0, elements.Element34_0_1,
            elements.Element34_0_1_0, elements.Element34_0_1_1, elements.Element34_0_1_2, elements.Element34_1, elements.Element34_1_0, elements.Element34_1_0_0, elements.Element34_1_1, elements.Element34_1_1_0, elements.Element35,
            elements.Element35_0, elements.Element35_0_0, elements.Element35_0_1, elements.Element35_0_2, elements.Element36, elements.Element36_0, elements.Element36_1, elements.Element36_2, elements.Element36_3, elements.Element36_4,
            elements.Element36_5, elements.Element37, elements.Element37_0, elements.Element37_0_0, elements.Element37_0_0_0, elements.Element37_0_0_1, elements.Element37_1, elements.Element37_1_0, elements.Element37_1_0_0,
            elements.Element37_1_0_1];
        yield return new TestCaseData(document, 4, null).Returns(expected).SetArgDisplayNames("MarkdownDocument", "4", "null");
        yield return new TestCaseData(document, 4, false).Returns(expected) .SetArgDisplayNames("MarkdownDocument", "4", "false");
            
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
            elements.Element18_2, elements.Element18_2_Attributes, elements.Element18_3, elements.Element18_3_0, elements.Element19, elements.Element19_0, elements.Element19_0_0, elements.Element19_0_0_0, elements.Element19_0_1,
            elements.Element19_0_1_0, elements.Element19_1, elements.Element19_1_0, elements.Element19_1_0_0, elements.Element19_1_1, elements.Element19_1_1_0, elements.Element19_2, elements.Element19_2_0, elements.Element19_2_0_0,
            elements.Element19_2_1, elements.Element19_2_1_0, elements.Element20, elements.Element20_Attributes, elements.Element20_0, elements.Element21, elements.Element21_0, elements.Element21_0_0, elements.Element21_0_0_0,
            elements.Element21_0_1, elements.Element21_0_1_0, elements.Element21_0_2, elements.Element21_0_2_0, elements.Element21_1, elements.Element21_1_0, elements.Element21_1_0_0, elements.Element21_1_1, elements.Element21_1_1_0,
            elements.Element21_1_2, elements.Element21_1_2_0, elements.Element21_2, elements.Element21_2_0, elements.Element21_2_0_0, elements.Element21_2_1, elements.Element21_2_1_0, elements.Element21_2_2, elements.Element21_2_2_0,
            elements.Element22, elements.Element22_0, elements.Element22_0_0, elements.Element22_0_0_0, elements.Element22_1, elements.Element22_1_0, elements.Element23, elements.Element23_0, elements.Element23_0_0, elements.Element24,
            elements.Element24_0, elements.Element24_1, elements.Element24_1_0, elements.Element24_2, elements.Element24_3, elements.Element24_3_0, elements.Element24_4, elements.Element24_5, elements.Element24_5_Attributes,
            elements.Element25, elements.Element25_0, elements.Element25_1, elements.Element25_1_Attributes, elements.Element25_2, elements.Element25_3, elements.Element25_3_Attributes, elements.Element25_4, elements.Element26,
            elements.Element26_0, elements.Element26_0_0, elements.Element26_1, elements.Element26_2, elements.Element26_2_Attributes, elements.Element27, elements.Element27_0, elements.Element27_1, elements.Element27_2,
            elements.Element28, elements.Element29, elements.Element29_0, elements.Element29_0_0, elements.Element29_1, elements.Element29_2, elements.Element29_2_0, elements.Element29_3, elements.Element30, elements.Element30_Attributes,
            elements.Element31, elements.Element31_Attributes, elements.Element32, elements.Element32_Attributes, elements.Element33, elements.Element33_Attributes, elements.Element34, elements.Element34_0, elements.Element34_0_0,
            elements.Element34_0_0_0, elements.Element34_0_1, elements.Element34_0_1_0, elements.Element34_0_1_1, elements.Element34_0_1_2, elements.Element34_1, elements.Element34_1_0, elements.Element34_1_0_0, elements.Element34_1_1,
            elements.Element34_1_1_0, elements.Element35, elements.Element35_0, elements.Element35_0_0, elements.Element35_0_1, elements.Element35_0_2, elements.Element36, elements.Element36_0, elements.Element36_1, elements.Element36_2,
            elements.Element36_3, elements.Element36_4, elements.Element36_5, elements.Element37, elements.Element37_0, elements.Element37_0_0, elements.Element37_0_0_0, elements.Element37_0_0_1, elements.Element37_1,
            elements.Element37_1_0, elements.Element37_1_0_0, elements.Element37_1_0_1];
        yield return new TestCaseData(document, 4, true).Returns(expected).SetArgDisplayNames("MarkdownDocument", "4", "true");
    }

    /// <summary>
    /// Test cases for <see cref="MarkdownExtensionMethods.DescendantsInDepthRange(MarkdownObject?, int, int, bool)"/>.
    /// </summary>
    /// <returns></returns>
    public static System.Collections.IEnumerable GetDescendantsInDepthRangeTestData()
    {
        MarkdownDocument document = GetMarkdownDocument();
        var elements = new MarkdownElements(document);
        
        IEnumerable<MarkdownObject> expected = [];
        yield return new TestCaseData(document, -1, 0, null).Returns(expected).SetArgDisplayNames("MarkdownDocument", "-1", "0", "null");
        yield return new TestCaseData(document, -1, 0, false).Returns(expected) .SetArgDisplayNames("MarkdownDocument", "-1", "0", "false");
        yield return new TestCaseData(document, -1, 0, true).Returns(expected).SetArgDisplayNames("MarkdownDocument", "-1", "0", "true");
        yield return new TestCaseData(document, 0, 0, null).Returns(expected).SetArgDisplayNames("MarkdownDocument", "0", "0", "null");
        yield return new TestCaseData(document, 0, 0, false).Returns(expected) .SetArgDisplayNames("MarkdownDocument", "0", "0", "false");
        yield return new TestCaseData(document, 0, 0, true).Returns(expected).SetArgDisplayNames("MarkdownDocument", "0", "0", "true");

        expected = [elements.Element0, elements.Element1, elements.Element2, elements.Element3, elements.Element4, elements.Element5, elements.Element6, elements.Element7, elements.Element8, elements.Element9, elements.Element10,
            elements.Element11, elements.Element12, elements.Element13, elements.Element14, elements.Element15, elements.Element16, elements.Element17, elements.Element18, elements.Element19, elements.Element20, elements.Element21,
            elements.Element22, elements.Element23, elements.Element24, elements.Element25, elements.Element26, elements.Element27, elements.Element28, elements.Element29, elements.Element30, elements.Element31, elements.Element32,
            elements.Element33, elements.Element34, elements.Element35, elements.Element36, elements.Element37];
        for (int minDepth = -1; minDepth < 2; minDepth++)
        {
            yield return new TestCaseData(document, minDepth, 1, null).Returns(expected).SetArgDisplayNames("MarkdownDocument", minDepth.ToString(), "1", "null");
            yield return new TestCaseData(document, minDepth, 1, false).Returns(expected) .SetArgDisplayNames("MarkdownDocument", minDepth.ToString(), "1", "false");
            yield return new TestCaseData(document, minDepth, 1, false).Returns(expected) .SetArgDisplayNames("MarkdownDocument", minDepth.ToString(), "1", "true");
        }

        expected = [elements.Element0, elements.Element0_0, elements.Element1, elements.Element2, elements.Element2_0, elements.Element3, elements.Element3_0, elements.Element4, elements.Element4_0, elements.Element4_1,
            elements.Element4_2, elements.Element5, elements.Element5_0, elements.Element5_1, elements.Element5_2, elements.Element5_3, elements.Element5_4, elements.Element5_5, elements.Element5_6, elements.Element6, elements.Element6_0,
            elements.Element7, elements.Element7_0, elements.Element8, elements.Element8_0, elements.Element8_1, elements.Element8_2, elements.Element9, elements.Element9_0, elements.Element9_1, elements.Element9_2, elements.Element10,
            elements.Element10_0, elements.Element10_1, elements.Element10_2, elements.Element10_3, elements.Element10_4, elements.Element11, elements.Element11_0, elements.Element11_1, elements.Element11_2, elements.Element12,
            elements.Element12_0, elements.Element12_1, elements.Element12_2, elements.Element13, elements.Element13_0, elements.Element13_1,elements.Element13_2, elements.Element13_3, elements.Element14, elements.Element14_0,
            elements.Element14_1, elements.Element15, elements.Element15_0, elements.Element16, elements.Element16_0, elements.Element16_1,elements.Element17, elements.Element17_0, elements.Element17_1, elements.Element17_2,
            elements.Element17_3, elements.Element17_4, elements.Element17_5, elements.Element17_6, elements.Element18, elements.Element18_0, elements.Element18_1, elements.Element18_2, elements.Element18_3, elements.Element19,
            elements.Element19_0, elements.Element19_1, elements.Element19_2, elements.Element20, elements.Element20_0, elements.Element21, elements.Element21_0, elements.Element21_1, elements.Element21_2, elements.Element22,
            elements.Element22_0, elements.Element22_1, elements.Element23, elements.Element23_0, elements.Element24, elements.Element24_0, elements.Element24_1, elements.Element24_2, elements.Element24_3, elements.Element24_4,
            elements.Element24_5, elements.Element25, elements.Element25_0, elements.Element25_1, elements.Element25_2, elements.Element25_3, elements.Element25_4, elements.Element26, elements.Element26_0, elements.Element26_1,
            elements.Element26_2, elements.Element27, elements.Element27_0, elements.Element27_1, elements.Element27_2, elements.Element28, elements.Element29, elements.Element29_0, elements.Element29_1, elements.Element29_2,
            elements.Element29_3, elements.Element30, elements.Element31, elements.Element32, elements.Element33, elements.Element34, elements.Element34_0, elements.Element34_1, elements.Element35, elements.Element35_0, elements.Element36,
            elements.Element36_0, elements.Element36_1, elements.Element36_2, elements.Element36_3, elements.Element36_4, elements.Element36_5, elements.Element37, elements.Element37_0, elements.Element37_1];
        yield return new TestCaseData(document, 1, 2, null).Returns(expected).SetArgDisplayNames("MarkdownDocument", "1", "2", "null");
        yield return new TestCaseData(document, 1, 2, false).Returns(expected) .SetArgDisplayNames("MarkdownDocument", "1", "2", "false");
            
        expected = [elements.Element0, elements.Element0_Attributes, elements.Element0_0, elements.Element1, elements.Element2, elements.Element2_0, elements.Element3, elements.Element3_0, elements.Element4, elements.Element4_0,
            elements.Element4_1, elements.Element4_2, elements.Element5, elements.Element5_0, elements.Element5_1, elements.Element5_2, elements.Element5_3, elements.Element5_4, elements.Element5_5, elements.Element5_6, elements.Element6,
            elements.Element6_Attributes, elements.Element6_0, elements.Element7, elements.Element7_Attributes, elements.Element7_0, elements.Element8, elements.Element8_0, elements.Element8_1, elements.Element8_2, elements.Element9,
            elements.Element9_0, elements.Element9_1, elements.Element9_2, elements.Element10, elements.Element10_0, elements.Element10_1, elements.Element10_2, elements.Element10_3, elements.Element10_4, elements.Element11,
            elements.Element11_0, elements.Element11_1, elements.Element11_2, elements.Element12, elements.Element12_0, elements.Element12_1, elements.Element12_2, elements.Element13, elements.Element13_Attributes, elements.Element13_0,
            elements.Element13_1, elements.Element13_2, elements.Element13_3, elements.Element14, elements.Element14_0, elements.Element14_1, elements.Element15, elements.Element15_0, elements.Element16, elements.Element16_0,
            elements.Element16_1, elements.Element17, elements.Element17_0, elements.Element17_1, elements.Element17_2, elements.Element17_3, elements.Element17_4, elements.Element17_5, elements.Element17_6, elements.Element18,
            elements.Element18_0, elements.Element18_1, elements.Element18_2, elements.Element18_3, elements.Element19, elements.Element19_0, elements.Element19_1, elements.Element19_2, elements.Element20, elements.Element20_Attributes,
            elements.Element20_0, elements.Element21, elements.Element21_0, elements.Element21_1, elements.Element21_2, elements.Element22, elements.Element22_0, elements.Element22_1, elements.Element23, elements.Element23_0,
            elements.Element24, elements.Element24_0, elements.Element24_1, elements.Element24_2, elements.Element24_3, elements.Element24_4, elements.Element24_5, elements.Element25, elements.Element25_0, elements.Element25_1,
            elements.Element25_2, elements.Element25_3, elements.Element25_4, elements.Element26, elements.Element26_0, elements.Element26_1, elements.Element26_2, elements.Element27, elements.Element27_0, elements.Element27_1,
            elements.Element27_2, elements.Element28, elements.Element29, elements.Element29_0, elements.Element29_1, elements.Element29_2, elements.Element29_3, elements.Element30, elements.Element30_Attributes, elements.Element31,
            elements.Element31_Attributes, elements.Element32, elements.Element32_Attributes, elements.Element33, elements.Element33_Attributes, elements.Element34, elements.Element34_0, elements.Element34_1, elements.Element35,
            elements.Element35_0, elements.Element36, elements.Element36_0, elements.Element36_1, elements.Element36_2, elements.Element36_3, elements.Element36_4, elements.Element36_5, elements.Element37, elements.Element37_0,
            elements.Element37_1];
        yield return new TestCaseData(document, 1, 2, true).Returns(expected) .SetArgDisplayNames("MarkdownDocument", "1", "2", "true");
            
        expected = [elements.Element0_0, elements.Element2_0, elements.Element3_0, elements.Element3_0_0, elements.Element4_0, elements.Element4_1, elements.Element4_2, elements.Element5_0, elements.Element5_1, elements.Element5_2,
            elements.Element5_3, elements.Element5_4, elements.Element5_5, elements.Element5_5_0, elements.Element5_6, elements.Element6_0, elements.Element7_0, elements.Element8_0, elements.Element8_0_0, elements.Element8_1,
            elements.Element8_2, elements.Element9_0, elements.Element9_1, elements.Element9_1_0, elements.Element9_2, elements.Element10_0, elements.Element10_1, elements.Element10_2, elements.Element10_3, elements.Element10_4,
            elements.Element10_4_0, elements.Element11_0, elements.Element11_1, elements.Element11_2, elements.Element12_0, elements.Element12_1, elements.Element12_2, elements.Element13_0, elements.Element13_0_0, elements.Element13_0_0_0,
            elements.Element13_0_0_1, elements.Element13_1, elements.Element13_1_0, elements.Element13_1_0_0, elements.Element13_1_0_1, elements.Element13_2, elements.Element13_2_0, elements.Element13_2_0_0, elements.Element13_3,
            elements.Element13_3_0, elements.Element13_3_0_0, elements.Element14_0, elements.Element14_0_0, elements.Element14_0_0_0, elements.Element14_1, elements.Element14_1_0, elements.Element14_1_0_0, elements.Element15_0,
            elements.Element15_0_0, elements.Element15_0_1, elements.Element15_0_2, elements.Element15_0_3, elements.Element16_0, elements.Element16_0_0, elements.Element16_0_1, elements.Element16_0_2, elements.Element16_0_2_0,
            elements.Element16_1, elements.Element16_1_0, elements.Element17_0, elements.Element17_1, elements.Element17_1_0, elements.Element17_1_1, elements.Element17_1_2, elements.Element17_1_3, elements.Element17_2,
            elements.Element17_3, elements.Element17_4, elements.Element17_5, elements.Element17_6, elements.Element18_0, elements.Element18_0_0, elements.Element18_0_1, elements.Element18_0_2, elements.Element18_0_3, elements.Element18_1,
            elements.Element18_1_0, elements.Element18_2, elements.Element18_3, elements.Element18_3_0, elements.Element19_0, elements.Element19_0_0, elements.Element19_0_0_0, elements.Element19_0_1, elements.Element19_0_1_0,
            elements.Element19_1, elements.Element19_1_0, elements.Element19_1_0_0, elements.Element19_1_1, elements.Element19_1_1_0, elements.Element19_2, elements.Element19_2_0, elements.Element19_2_0_0, elements.Element19_2_1,
            elements.Element19_2_1_0, elements.Element20_0, elements.Element21_0, elements.Element21_0_0, elements.Element21_0_0_0, elements.Element21_0_1, elements.Element21_0_1_0, elements.Element21_0_2, elements.Element21_0_2_0,
            elements.Element21_1, elements.Element21_1_0, elements.Element21_1_0_0, elements.Element21_1_1, elements.Element21_1_1_0, elements.Element21_1_2, elements.Element21_1_2_0, elements.Element21_2, elements.Element21_2_0,
            elements.Element21_2_0_0, elements.Element21_2_1, elements.Element21_2_1_0, elements.Element21_2_2, elements.Element21_2_2_0, elements.Element22_0, elements.Element22_0_0, elements.Element22_0_0_0, elements.Element22_1,
            elements.Element22_1_0, elements.Element23_0, elements.Element23_0_0, elements.Element24_0, elements.Element24_1, elements.Element24_1_0, elements.Element24_2, elements.Element24_3, elements.Element24_3_0, elements.Element24_4,
            elements.Element24_5, elements.Element25_0, elements.Element25_1, elements.Element25_2, elements.Element25_3, elements.Element25_4, elements.Element26_0, elements.Element26_0_0, elements.Element26_1, elements.Element26_2,
            elements.Element27_0, elements.Element27_1, elements.Element27_2, elements.Element29_0, elements.Element29_0_0, elements.Element29_1, elements.Element29_2, elements.Element29_2_0, elements.Element29_3, elements.Element34_0,
            elements.Element34_0_0, elements.Element34_0_0_0, elements.Element34_0_1, elements.Element34_0_1_0, elements.Element34_0_1_1, elements.Element34_0_1_2, elements.Element34_1, elements.Element34_1_0, elements.Element34_1_0_0,
            elements.Element34_1_1, elements.Element34_1_1_0, elements.Element35_0, elements.Element35_0_0, elements.Element35_0_1, elements.Element35_0_2, elements.Element36_0, elements.Element36_1, elements.Element36_2,
            elements.Element36_3, elements.Element36_4, elements.Element36_5, elements.Element37_0, elements.Element37_0_0, elements.Element37_0_0_0, elements.Element37_0_0_1, elements.Element37_1, elements.Element37_1_0,
            elements.Element37_1_0_0, elements.Element37_1_0_1];
        yield return new TestCaseData(document, 2, 4, null).Returns(expected).SetArgDisplayNames("MarkdownDocument", "2", "4", "null");
        yield return new TestCaseData(document, 2, 4, false).Returns(expected) .SetArgDisplayNames("MarkdownDocument", "2", "4", "false");
            
        expected = [elements.Element0_Attributes, elements.Element0_0, elements.Element2_0, elements.Element3_0, elements.Element3_0_0, elements.Element4_0, elements.Element4_1, elements.Element4_2, elements.Element4_2_Attributes,
            elements.Element5_0, elements.Element5_1, elements.Element5_2, elements.Element5_3, elements.Element5_4, elements.Element5_5, elements.Element5_5_0, elements.Element5_6, elements.Element6_Attributes, elements.Element6_0,
            elements.Element7_Attributes, elements.Element7_0, elements.Element8_0, elements.Element8_0_Attributes, elements.Element8_0_0, elements.Element8_1, elements.Element8_2, elements.Element9_0, elements.Element9_1,
            elements.Element9_1_0, elements.Element9_2, elements.Element10_0, elements.Element10_1, elements.Element10_2, elements.Element10_3, elements.Element10_4, elements.Element10_4_0, elements.Element11_0, elements.Element11_1,
            elements.Element11_2, elements.Element12_0, elements.Element12_1, elements.Element12_2, elements.Element13_Attributes, elements.Element13_0, elements.Element13_0_Attributes, elements.Element13_0_0, elements.Element13_0_0_0,
            elements.Element13_0_0_1, elements.Element13_1, elements.Element13_1_Attributes, elements.Element13_1_0, elements.Element13_1_0_0, elements.Element13_1_0_1, elements.Element13_2, elements.Element13_2_0,
            elements.Element13_2_0_0, elements.Element13_3, elements.Element13_3_0, elements.Element13_3_0_0, elements.Element14_0, elements.Element14_0_0, elements.Element14_0_0_0, elements.Element14_1, elements.Element14_1_0,
            elements.Element14_1_0_0, elements.Element15_0, elements.Element15_0_0, elements.Element15_0_1, elements.Element15_0_2, elements.Element15_0_3, elements.Element16_0, elements.Element16_0_0, elements.Element16_0_1,
            elements.Element16_0_2, elements.Element16_0_2_0, elements.Element16_1, elements.Element16_1_0, elements.Element17_0, elements.Element17_1, elements.Element17_1_0, elements.Element17_1_1, elements.Element17_1_2,
            elements.Element17_1_3, elements.Element17_2, elements.Element17_3, elements.Element17_4, elements.Element17_5, elements.Element17_6, elements.Element18_0, elements.Element18_0_0, elements.Element18_0_1, elements.Element18_0_2,
            elements.Element18_0_3, elements.Element18_1, elements.Element18_1_0, elements.Element18_2, elements.Element18_2_Attributes, elements.Element18_3, elements.Element18_3_0, elements.Element19_0, elements.Element19_0_0,
            elements.Element19_0_0_0, elements.Element19_0_1, elements.Element19_0_1_0, elements.Element19_1, elements.Element19_1_0, elements.Element19_1_0_0, elements.Element19_1_1, elements.Element19_1_1_0, elements.Element19_2,
            elements.Element19_2_0, elements.Element19_2_0_0, elements.Element19_2_1, elements.Element19_2_1_0, elements.Element20_Attributes, elements.Element20_0, elements.Element21_0, elements.Element21_0_0, elements.Element21_0_0_0,
            elements.Element21_0_1, elements.Element21_0_1_0, elements.Element21_0_2, elements.Element21_0_2_0, elements.Element21_1, elements.Element21_1_0, elements.Element21_1_0_0, elements.Element21_1_1, elements.Element21_1_1_0,
            elements.Element21_1_2, elements.Element21_1_2_0, elements.Element21_2, elements.Element21_2_0, elements.Element21_2_0_0, elements.Element21_2_1, elements.Element21_2_1_0, elements.Element21_2_2, elements.Element21_2_2_0,
            elements.Element22_0, elements.Element22_0_0, elements.Element22_0_0_0, elements.Element22_1, elements.Element22_1_0, elements.Element23_0, elements.Element23_0_0, elements.Element24_0, elements.Element24_1,
            elements.Element24_1_0, elements.Element24_2, elements.Element24_3, elements.Element24_3_0, elements.Element24_4, elements.Element24_5, elements.Element24_5_Attributes, elements.Element25_0, elements.Element25_1,
            elements.Element25_1_Attributes, elements.Element25_2, elements.Element25_3, elements.Element25_3_Attributes, elements.Element25_4, elements.Element26_0, elements.Element26_0_0, elements.Element26_1, elements.Element26_2,
            elements.Element26_2_Attributes, elements.Element27_0, elements.Element27_1, elements.Element27_2, elements.Element29_0, elements.Element29_0_0, elements.Element29_1, elements.Element29_2, elements.Element29_2_0,
            elements.Element29_3, elements.Element30_Attributes, elements.Element31_Attributes, elements.Element32_Attributes, elements.Element33_Attributes, elements.Element34_0, elements.Element34_0_0, elements.Element34_0_0_0,
            elements.Element34_0_1, elements.Element34_0_1_0, elements.Element34_0_1_1, elements.Element34_0_1_2, elements.Element34_1, elements.Element34_1_0, elements.Element34_1_0_0, elements.Element34_1_1, elements.Element34_1_1_0,
            elements.Element35_0, elements.Element35_0_0, elements.Element35_0_1, elements.Element35_0_2, elements.Element36_0, elements.Element36_1, elements.Element36_2, elements.Element36_3, elements.Element36_4, elements.Element36_5,
            elements.Element37_0, elements.Element37_0_0, elements.Element37_0_0_0, elements.Element37_0_0_1, elements.Element37_1, elements.Element37_1_0, elements.Element37_1_0_0, elements.Element37_1_0_1];
        yield return new TestCaseData(document, 2, 4, true).Returns(expected) .SetArgDisplayNames("MarkdownDocument", "2", "4", "true");
    }

    /// <summary>
    /// Test cases for <see cref="MarkdownExtensionMethods.DescendantBranchesMatchingType(MarkdownObject?, Type, int, int)"/>.
    /// </summary>
    /// <returns></returns>
    public static System.Collections.IEnumerable GetDescendantBranchesMatchingType5TestData()
    {
        MarkdownDocument document = GetMarkdownDocument();
        var elements = new MarkdownElements(document);
        
        IEnumerable<MarkdownObject> expected = [];
        Type type = typeof(MarkdownDocument);
        for (int maxDepth = 0; maxDepth < 3; maxDepth++)
        {
            yield return new TestCaseData(document, type, -1, maxDepth).Returns(expected).SetArgDisplayNames("MarkdownDocument", type.Name, "-1", maxDepth.ToString());
            yield return new TestCaseData(document, type, 0, maxDepth).Returns(expected).SetArgDisplayNames("MarkdownDocument", type.Name, "0", maxDepth.ToString());
        }

        type = typeof(ParagraphBlock);
        expected = [elements.Element2, elements.Element3, elements.Element4, elements.Element5, elements.Element8, elements.Element9, elements.Element10, elements.Element11, elements.Element12, elements.Element17, elements.Element23,
            elements.Element24, elements.Element25, elements.Element26, elements.Element27, elements.Element29];
        for (int minDepth = -1; minDepth < 2; minDepth++)
            yield return new TestCaseData(document, type, minDepth, 1).Returns(expected).SetArgDisplayNames("MarkdownDocument", type.Name, minDepth.ToString(), "1");

        expected = [elements.Element2, elements.Element3, elements.Element4, elements.Element5, elements.Element6, elements.Element8, elements.Element9, elements.Element10, elements.Element11, elements.Element12, elements.Element15_0,
            elements.Element16_0, elements.Element16_1, elements.Element17, elements.Element17_0,  elements.Element18_0, elements.Element18_1, elements.Element18_2, elements.Element18_3, elements.Element22_0, elements.Element23,
            elements.Element25, elements.Element26, elements.Element27, elements.Element29, elements.Element35_0];
        yield return new TestCaseData(document, type, 1, 2).Returns(expected).SetArgDisplayNames("MarkdownDocument", type.Name, "1", "2");
            
        type = typeof(HtmlAttributes);
        expected = [elements.Element0_Attributes, elements.Element6_Attributes, elements.Element7_Attributes, elements.Element13_Attributes, elements.Element20_Attributes, elements.Element30_Attributes, elements.Element31_Attributes,
            elements.Element32_Attributes, elements.Element33_Attributes];
        yield return new TestCaseData(document, type, 1, 2).Returns(expected).SetArgDisplayNames("MarkdownDocument", type.Name, "1", "2");
            
        type = typeof(ParagraphBlock);
        expected = [elements.Element13_0_0, elements.Element13_1_0, elements.Element13_2_0, elements.Element14_0_0, elements.Element14_1_0, elements.Element15_0, elements.Element16_0, elements.Element18_0, elements.Element18_1,
            elements.Element18_3, elements.Element19_0_0_0, elements.Element19_0_1_0, elements.Element19_1_0_0, elements.Element19_1_1_0, elements.Element19_2_0_0, elements.Element19_2_1_0, elements.Element21_0_0_0,
            elements.Element21_0_1_0, elements.Element21_0_2_0, elements.Element21_1_0_0, elements.Element21_1_1_0, elements.Element21_1_2_0, elements.Element21_2_0_0, elements.Element21_2_1_0, elements.Element21_2_2_0,
            elements.Element22_0, elements.Element34_0_1, elements.Element34_1_1, elements.Element35_0, elements.Element37_0_0, elements.Element37_1_0];
        yield return new TestCaseData(document, type, 2, 4).Returns(expected).SetArgDisplayNames("MarkdownDocument", type.Name, "2", "4");
            
        type = typeof(HtmlAttributes);
        expected = [elements.Element0_Attributes, elements.Element4_2_Attributes, elements.Element6_Attributes, elements.Element7_Attributes, elements.Element8_0_Attributes, elements.Element13_Attributes, elements.Element13_0_Attributes,
            elements.Element13_1_Attributes, elements.Element18_2_Attributes, elements.Element20_Attributes, elements.Element24_5_Attributes, elements.Element25_1_Attributes, elements.Element25_3_Attributes, elements.Element26_2_Attributes,
            elements.Element30_Attributes, elements.Element31_Attributes, elements.Element32_Attributes, elements.Element33_Attributes];
        yield return new TestCaseData(document, type, 2, 4).Returns(expected).SetArgDisplayNames("MarkdownDocument", type.Name, "2", "4");
    }

    /// <summary>
    /// Test cases for <see cref="MarkdownExtensionMethods.DescendantBranchesMatchingType(MarkdownObject?, IEnumerable{Type}, int, int)"/>.
    /// </summary>
    /// <returns></returns>
    public static System.Collections.IEnumerable GetDescendantBranchesMatchingType6TestData()
    {
        MarkdownDocument document = GetMarkdownDocument();
        var elements = new MarkdownElements(document);
        
        IEnumerable<MarkdownObject> expected = [];
        IEnumerable<Type> types = [typeof(MarkdownDocument), typeof(ContainerInline)];
        string display = "[" + string.Join(", ", types.Select(t => $"typeof({t.Name})")) + "]";
        yield return new TestCaseData(document, types, -1, 0).Returns(expected).SetArgDisplayNames("MarkdownDocument", display, "-1", "0");
        yield return new TestCaseData(document, types, -1, 1).Returns(expected).SetArgDisplayNames("MarkdownDocument", display, "-1", "1");
        yield return new TestCaseData(document, types, 0, 1).Returns(expected).SetArgDisplayNames("MarkdownDocument", display, "0", "1");

        types = [typeof(ParagraphBlock), typeof(LeafInline)];
        display = "[" + string.Join(", ", types.Select(t => $"typeof({t.Name})")) + "]";
        expected = [elements.Element0_0, elements.Element2, elements.Element3, elements.Element4, elements.Element5, elements.Element6_0, elements.Element7_0, elements.Element8, elements.Element9, elements.Element10, elements.Element11,
            elements.Element12, elements.Element15_0, elements.Element16_0, elements.Element16_1, elements.Element17, elements.Element18_0, elements.Element18_1, elements.Element18_3, elements.Element20_0, elements.Element22_0,
            elements.Element23, elements.Element24, elements.Element25, elements.Element26, elements.Element27, elements.Element29, elements.Element35_0];
        yield return new TestCaseData(document, types, 1, 2).Returns(expected).SetArgDisplayNames("MarkdownDocument", display, "0", "1");
    }
}