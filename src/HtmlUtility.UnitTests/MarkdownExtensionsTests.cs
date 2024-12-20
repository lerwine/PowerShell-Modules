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
    public Type[]? ToReflectionTypesTest(IList<MarkdownTokenType>? types)
    {
        return MarkdownExtensionMethods.ToReflectionTypes(types)?.ToArray();
    }

    /// <summary>
    /// Unit test for <see cref="MarkdownExtensionMethods.GetChildObjects(MarkdownObject?, bool)"/>.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    [TestCaseSource(typeof(Helpers.ExampleMarkdown1), nameof(Helpers.ExampleMarkdown1.GetGetChildObjectsTestData))]
    public IEnumerable<MarkdownObject> GetChildObjectsTest(MarkdownObject source, bool? includeAttributes)
    {
        if (includeAttributes.HasValue)
            return MarkdownExtensionMethods.GetChildObjects(source, includeAttributes.Value);
        return MarkdownExtensionMethods.GetChildObjects(source);
    }
    
    /// <summary>
    /// Unit test for <see cref="MarkdownExtensionMethods.GetAllDescendants(MarkdownObject?, bool)"/>.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    [TestCaseSource(typeof(Helpers.ExampleMarkdown1), nameof(Helpers.ExampleMarkdown1.GetGetAllDescendantsTestData))]
    public IEnumerable<MarkdownObject> GetAllDescendantsTest(MarkdownObject source, bool? includeAttributes)
    {
        if (includeAttributes.HasValue)
            return MarkdownExtensionMethods.GetAllDescendants(source, includeAttributes.Value);
        return MarkdownExtensionMethods.GetAllDescendants(source);
    }

    /// <summary>
    /// Unit test for <see cref="MarkdownExtensionMethods.GetDescendantBranchesMatchingType(MarkdownObject?, Type, bool)"/>.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    [TestCaseSource(typeof(Helpers.ExampleMarkdown1), nameof(Helpers.ExampleMarkdown1.GetGetDescendantBranchesMatchingType1TestData))]
    public IEnumerable<MarkdownObject> GetDescendantBranchesMatchingType1Test(MarkdownObject source, Type type)
    {
        return MarkdownExtensionMethods.GetDescendantBranchesMatchingType(source, type);
    }

    /// <summary>
    /// Unit test for <see cref="MarkdownExtensionMethods.GetDescendantBranchesMatchingType(MarkdownObject?, IEnumerable{Type}, bool)"/>.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="types"></param>
    /// <returns></returns>
    [TestCaseSource(typeof(Helpers.ExampleMarkdown1), nameof(Helpers.ExampleMarkdown1.GetGetDescendantBranchesMatchingType2TestData))]
    public IEnumerable<MarkdownObject> GetDescendantBranchesMatchingType2Test(MarkdownObject source, IEnumerable<Type> types)
    {
        return MarkdownExtensionMethods.GetDescendantBranchesMatchingType(source, types);
    }

    /// <summary>
    /// Unit test for <see cref="MarkdownExtensionMethods.GetDescendantBranchesMatchingType(MarkdownObject?, Type, int, bool)"/>.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="type"></param>
    /// <param name="maximumDepth"></param>
    /// <param name="emitAttributesofUnmatched"></param>
    /// <returns></returns>
    [TestCaseSource(typeof(Helpers.ExampleMarkdown1), nameof(Helpers.ExampleMarkdown1.GetGetDescendantBranchesMatchingType3TestData))]
    public IEnumerable<MarkdownObject> GetDescendantBranchesMatchingType3Test(MarkdownObject? source, Type type, int maximumDepth)
    {
        return MarkdownExtensionMethods.GetDescendantBranchesMatchingType(source, type, maximumDepth);
    }

    /// <summary>
    /// Unit test for <see cref="MarkdownExtensionMethods.GetDescendantBranchesMatchingType(MarkdownObject?, IEnumerable{Type}, int, bool)"/>.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="types"></param>
    /// <param name="maximumDepth"></param>
    /// <param name="emitAttributesofUnmatched"></param>
    /// <returns></returns>
    [TestCaseSource(typeof(Helpers.ExampleMarkdown1), nameof(Helpers.ExampleMarkdown1.GetGetDescendantBranchesMatchingType4TestData))]
    public IEnumerable<MarkdownObject> GetDescendantBranchesMatchingType4Test(MarkdownObject? source, IEnumerable<Type> types, int maximumDepth)
    {
        return MarkdownExtensionMethods.GetDescendantBranchesMatchingType(source, types, maximumDepth);
    }

    /// <summary>
    /// Unit test for <see cref="MarkdownExtensionMethods.GetDescendantsAtDepth(MarkdownObject?, int, bool)"/>.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="depth"></param>
    /// <param name="includeAttributes"></param>
    /// <returns></returns>
    [TestCaseSource(typeof(Helpers.ExampleMarkdown1), nameof(Helpers.ExampleMarkdown1.GetGetDescendantsAtDepthTestData))]
    public IEnumerable<MarkdownObject> GetDescendantsAtDepthTest(MarkdownObject? source, int depth, bool? includeAttributes)
    {
        if (includeAttributes.HasValue)
            return MarkdownExtensionMethods.GetDescendantsAtDepth(source, depth, includeAttributes.Value);
        return MarkdownExtensionMethods.GetDescendantsAtDepth(source, depth);
    }

    /// <summary>
    /// Unit test for <see cref="MarkdownExtensionMethods.GetDescendantsFromDepth(MarkdownObject?, int, bool)"/>.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="minimumDepth"></param>
    /// <param name="includeAttributes"></param>
    /// <returns></returns>
    [TestCaseSource(typeof(Helpers.ExampleMarkdown1), nameof(Helpers.ExampleMarkdown1.GetGetDescendantsFromDepthTestData))]
    public IEnumerable<MarkdownObject> GetDescendantsFromDepthTest(MarkdownObject? source, int minimumDepth, bool? includeAttributes)
    {
        if (includeAttributes.HasValue)
            return MarkdownExtensionMethods.GetDescendantsFromDepth(source, minimumDepth, includeAttributes.Value);
        return MarkdownExtensionMethods.GetDescendantsFromDepth(source, minimumDepth);
    }

    // /// <summary>
    // /// Unit test for <see cref="MarkdownExtensionMethods.GetDescendantsUpToDepth(MarkdownObject?, int, bool)"/>.
    // /// </summary>
    // /// <param name="source"></param>
    // /// <param name="maximumDepth"></param>
    // /// <param name="includeAttributes"></param>
    // /// <returns></returns>
    // [TestCaseSource(typeof(Helpers.ExampleMarkdown1), nameof(Helpers.ExampleMarkdown1.GetGetDescendantsUpToDepthTestData))]
    // public Tuple<Type, SourceSpan, int, int>[] GetDescendantsUpToDepthTest(MarkdownObject? source, int maximumDepth, bool? includeAttributes)
    // {
    //     if (includeAttributes.HasValue)
    //         return MarkdownExtensionMethods.GetDescendantsUpToDepth(source, maximumDepth, includeAttributes.Value)
    //             .Select(r => new Tuple<Type, SourceSpan, int, int>(r.GetType(), r.Span, r.Line, r.Column)).ToArray();
    //     return MarkdownExtensionMethods.GetDescendantsUpToDepth(source, maximumDepth)
    //         .Select(r => new Tuple<Type, SourceSpan, int, int>(r.GetType(), r.Span, r.Line, r.Column)).ToArray();
    // }

    // /// <summary>
    // /// Unit test for <see cref="MarkdownExtensionMethods.GetDescendantsInDepthRange(MarkdownObject?, int, int, bool)"/>.
    // /// </summary>
    // /// <param name="source"></param>
    // /// <param name="minimumDepth"></param>
    // /// <param name="maximumDepth"></param>
    // /// <param name="includeAttributes"></param>
    // /// <returns></returns>
    // [TestCaseSource(typeof(Helpers.ExampleMarkdown1), nameof(Helpers.ExampleMarkdown1.GetGetDescendantsInDepthRangeTestData))]
    // public Tuple<Type, SourceSpan, int, int>[] GetDescendantsInDepthRangeTest(MarkdownObject? source, int minimumDepth, int maximumDepth, bool? includeAttributes)
    // {
    //     if (includeAttributes.HasValue)
    //         return MarkdownExtensionMethods.GetDescendantsInDepthRange(source, minimumDepth, maximumDepth, includeAttributes.Value)
    //             .Select(r => new Tuple<Type, SourceSpan, int, int>(r.GetType(), r.Span, r.Line, r.Column)).ToArray();
    //     return MarkdownExtensionMethods.GetDescendantsInDepthRange(source, minimumDepth, maximumDepth)
    //         .Select(r => new Tuple<Type, SourceSpan, int, int>(r.GetType(), r.Span, r.Line, r.Column)).ToArray();
    // }

    // /// <summary>
    // /// Unit test for <see cref="MarkdownExtensionMethods.GetDescendantBranchesMatchingType(MarkdownObject?, Type, int, int, bool)"/>.
    // /// </summary>
    // /// <param name="source"></param>
    // /// <param name="type"></param>
    // /// <param name="minimumDepth"></param>
    // /// <param name="maximumDepth"></param>
    // /// <param name="emitAttributesofUnmatched"></param>
    // /// <returns></returns>
    // [TestCaseSource(typeof(Helpers.ExampleMarkdown1), nameof(Helpers.ExampleMarkdown1.GetGetDescendantBranchesMatchingType5TestData))]
    // public Tuple<Type, SourceSpan, int, int>[] GetDescendantBranchesMatchingType5Test(MarkdownObject? source, Type type, int minimumDepth, int maximumDepth)
    // {
    //     return MarkdownExtensionMethods.GetDescendantBranchesMatchingType(source, type, minimumDepth, maximumDepth)
    //         .Select(r => new Tuple<Type, SourceSpan, int, int>(r.GetType(), r.Span, r.Line, r.Column)).ToArray();
    // }

    // /// <summary>
    // /// Unit test for <see cref="MarkdownExtensionMethods.GetDescendantBranchesMatchingType(MarkdownObject?, IEnumerable{Type}, int, int, bool)"/>.
    // /// </summary>
    // /// <param name="source"></param>
    // /// <param name="types"></param>
    // /// <param name="minimumDepth"></param>
    // /// <param name="maximumDepth"></param>
    // /// <param name="emitAttributesofUnmatched"></param>
    // /// <returns></returns>
    // [TestCaseSource(typeof(Helpers.ExampleMarkdown1), nameof(Helpers.ExampleMarkdown1.GetGetDescendantBranchesMatchingType6TestData))]
    // public Tuple<Type, SourceSpan, int, int>[] GetDescendantBranchesMatchingType6Test(MarkdownObject? source, IEnumerable<Type> types, int minimumDepth, int maximumDepth)
    // {
    //     return MarkdownExtensionMethods.GetDescendantBranchesMatchingType(source, types, minimumDepth, maximumDepth)
    //         .Select(r => new Tuple<Type, SourceSpan, int, int>(r.GetType(), r.Span, r.Line, r.Column)).ToArray();
    // }
}