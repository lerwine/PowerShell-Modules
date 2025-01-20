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
    /// Unit test for <see cref="MarkdownExtensionMethods.DescendantsAtDepth(MarkdownObject?, int, bool)"/>.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="depth"></param>
    /// <param name="includeAttributes"></param>
    /// <returns></returns>
    [TestCaseSource(typeof(Helpers.ExampleMarkdown1), nameof(Helpers.ExampleMarkdown1.GetDescendantsAtDepthTestData))]
    public IEnumerable<MarkdownObject> DescendantsAtDepthTest(MarkdownObject source, int depth, bool includeAttributes)
    {
        if (includeAttributes)
            return MarkdownExtensionMethods.AttributesAndDescendantsAtDepth(source, depth);
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
    public IEnumerable<MarkdownObject> DescendantsFromDepthTest(MarkdownObject source, int minimumDepth, bool includeAttributes)
    {
        if (includeAttributes)
            return MarkdownExtensionMethods.AttributesAndDescendantsFromDepth(source, minimumDepth);
        return MarkdownExtensionMethods.DescendantsFromDepth(source, minimumDepth);
    }
}