using System.Management.Automation;
using Markdig.Renderers.Html;
using Markdig.Syntax;

namespace HtmlUtility.UnitTests;

public partial class MarkdownExtensionsTests
{
    [SetUp]
    public void Setup()
    {
    }

    /// <summary>
    /// Unit test for <see cref="MarkdownExtensionMethods.ToReflectionType(MarkdownTokenType)"/>
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [TestCaseSource(typeof(Helpers.TestHelper), nameof(Helpers.TestHelper.GetToReflectionTypeTestData))]
    public Type ToReflectionTypeTest(MarkdownTokenType type)
    {
        return MarkdownExtensionMethods.ToReflectionType(type);
    }

    /// <summary>
    /// Unit test for <see cref="MarkdownExtensionMethods.ToReflectionTypes(IList{MarkdownTokenType}?)"/>.
    /// </summary>
    /// <param name="types"></param>
    /// <returns></returns>
    [TestCaseSource(typeof(Helpers.TestHelper), nameof(Helpers.TestHelper.GetToReflectionTypesTestData))]
    public Type[] ToReflectionTypesTest(IList<MarkdownTokenType> types)
    {
        return [.. MarkdownExtensionMethods.ToReflectionTypes(types)];
    }

    /// <summary>
    /// Unit test for <see cref="MarkdownExtensionMethods.GetDescendantBranchesMatchingType(MarkdownObject?, Type, bool)"/>.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    [TestCaseSource(typeof(Helpers.ExampleMarkdown1), nameof(Helpers.ExampleMarkdown1.GetDescendantBranchesMatchingType1TestData))]
    public IEnumerable<MarkdownObject> DescendantBranchesMatchingType1Test(MarkdownObject source, Type type)
    {
        return MarkdownExtensionMethods.DescendantBranchesMatchingType(source, type);
    }

    /// <summary>
    /// Unit test for <see cref="MarkdownExtensionMethods.GetDescendantBranchesMatchingType(MarkdownObject?, IEnumerable{Type}, bool)"/>.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="types"></param>
    /// <returns></returns>
    [TestCaseSource(typeof(Helpers.ExampleMarkdown1), nameof(Helpers.ExampleMarkdown1.GetDescendantBranchesMatchingType2TestData))]
    public IEnumerable<MarkdownObject> DescendantBranchesMatchingType2Test(MarkdownObject source, IEnumerable<Type> types)
    {
        return MarkdownExtensionMethods.DescendantBranchesMatchingType(source, types);
    }

    /// <summary>
    /// Unit test for <see cref="MarkdownExtensionMethods.GetDescendantBranchesMatchingType(MarkdownObject?, Type, int, bool)"/>.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="type"></param>
    /// <param name="maximumDepth"></param>
    /// <param name="emitAttributesofUnmatched"></param>
    /// <returns></returns>
    [TestCaseSource(typeof(Helpers.ExampleMarkdown1), nameof(Helpers.ExampleMarkdown1.GetDescendantBranchesMatchingType3TestData))]
    public IEnumerable<MarkdownObject> DescendantBranchesMatchingType3Test(MarkdownObject? source, Type type, int maximumDepth)
    {
        return MarkdownExtensionMethods.DescendantBranchesMatchingType(source, type, maximumDepth);
    }

    /// <summary>
    /// Unit test for <see cref="MarkdownExtensionMethods.GetDescendantBranchesMatchingType(MarkdownObject?, IEnumerable{Type}, int, bool)"/>.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="types"></param>
    /// <param name="maximumDepth"></param>
    /// <param name="emitAttributesofUnmatched"></param>
    /// <returns></returns>
    [TestCaseSource(typeof(Helpers.ExampleMarkdown1), nameof(Helpers.ExampleMarkdown1.GetDescendantBranchesMatchingType4TestData))]
    public IEnumerable<MarkdownObject> DescendantBranchesMatchingType4Test(MarkdownObject? source, IEnumerable<Type> types, int maximumDepth)
    {
        return MarkdownExtensionMethods.DescendantBranchesMatchingType(source, types, maximumDepth);
    }

    /// <summary>
    /// Unit test for <see cref="MarkdownExtensionMethods.DescendantsAtDepth(MarkdownObject?, int, bool)"/>.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="depth"></param>
    /// <param name="includeAttributes"></param>
    /// <returns></returns>
    [TestCaseSource(typeof(Helpers.ExampleMarkdown1), nameof(Helpers.ExampleMarkdown1.GetDescendantsAtDepthTestData))]
    public IEnumerable<MarkdownObject> DescendantsAtDepthTest(MarkdownObject? source, int depth, bool includeAttributes)
    {
        if (includeAttributes)
            return MarkdownExtensionMethods.DescendantsAtDepthIncludingHtmlAttributes(source, depth);
        return MarkdownExtensionMethods.DescendantsAtDepth(source, depth);
    }

    /// <summary>
    /// Unit test for <see cref="MarkdownExtensionMethods.DescendantsFromDepth(MarkdownObject?, int, bool)"/>.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="minimumDepth"></param>
    /// <param name="includeAttributes"></param>
    /// <returns></returns>
    [TestCaseSource(typeof(Helpers.ExampleMarkdown1), nameof(Helpers.ExampleMarkdown1.GetDescendantsFromDepthTestData))]
    public IEnumerable<MarkdownObject> DescendantsFromDepthTest(MarkdownObject? source, int minimumDepth, bool includeAttributes)
    {
        if (includeAttributes)
            return MarkdownExtensionMethods.DescendantsIncludingAttributesFromDepth(source, minimumDepth);
        return MarkdownExtensionMethods.DescendantsFromDepth(source, minimumDepth);
    }
}