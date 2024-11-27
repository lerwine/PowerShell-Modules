using System.Diagnostics.CodeAnalysis;

namespace HtmlUtility.UnitTests;

public partial class MarkdownExtensionsTests
{
    [SetUp]
    public void Setup()
    {
    }

    [TestCaseSource(typeof(TestData), nameof(TestData.GetToReflectionTypeTestData))]
    public Type ToReflectionTypeTest(MarkdownTokenType type)
    {
        return MarkdownExtensions.ToReflectionType(type);
    }

    [TestCaseSource(typeof(TestData), nameof(TestData.GetToReflectionTypesTestData))]
    public Type[]? ToReflectionTypesTest(IList<MarkdownTokenType>? types)
    {
        return MarkdownExtensions.ToReflectionTypes(types)?.ToArray();
    }

    [TestCaseSource(typeof(TestData), nameof(TestData.GetGetChildObjectsTestData))]
    public Markdig.Syntax.SourceSpan[] GetChildObjectsTest(Markdig.Syntax.MarkdownObject source, Type[] expectedTypes)
    {
        Markdig.Syntax.MarkdownObject[] result = MarkdownExtensions.GetChildObjects(source).ToArray();
        Assert.That(result, Has.Length.EqualTo(expectedTypes.Length));
        for (int i = 0; i < expectedTypes.Length; i++)
            Assert.That(result[i], Is.InstanceOf(expectedTypes[i]), "Type Index ", i);
        return MarkdownExtensions.GetChildObjects(source).Select(obj => obj.Span).ToArray();
    }

    [Test]
    public void GetAllDescendantsTest()
    {
        // Markdig.Syntax.MarkdownObject source;
        // IEnumerable<Markdig.Syntax.MarkdownObject> result = MarkdownExtensions.GetAllDescendants(source);
        Assert.Inconclusive();
    }

    [Test]
    public void GetDescendantBranchesMatchingType1Test()
    {
        // Markdig.Syntax.MarkdownObject source;
        // Type type;
        // IEnumerable<Markdig.Syntax.MarkdownObject> result = MarkdownExtensions.GetDescendantBranchesMatchingType(source, type);
        Assert.Inconclusive();
    }

    [Test]
    public void GetDescendantBranchesMatchingType2Test()
    {
        // Markdig.Syntax.MarkdownObject source;
        // ICollection<Type> types;
        // IEnumerable<Markdig.Syntax.MarkdownObject> result = MarkdownExtensions.GetDescendantBranchesMatchingType(source, types);
        Assert.Inconclusive();
    }

    [Test]
    public void GetDescendantBranchesMatchingType3Test()
    {
        // Markdig.Syntax.MarkdownObject source;
        // Type type;
        // int maximumDepth;
        // IEnumerable<Markdig.Syntax.MarkdownObject> result = MarkdownExtensions.GetDescendantBranchesMatchingType(source, type, maximumDepth);
        Assert.Inconclusive();
    }

    [Test]
    public void GetDescendantBranchesMatchingType4Test()
    {
        // Markdig.Syntax.MarkdownObject source;
        // ICollection<Type> types;
        // int maximumDepth;
        // IEnumerable<Markdig.Syntax.MarkdownObject> result = MarkdownExtensions.GetDescendantBranchesMatchingType(source, types, maximumDepth);
        Assert.Inconclusive();
    }

    [Test]
    public void GetDescendantsAtDepthTest()
    {
        // Markdig.Syntax.MarkdownObject source;
        // int depth;
        // IEnumerable<Markdig.Syntax.MarkdownObject> result = MarkdownExtensions.GetDescendantsAtDepth(source, depth);
        Assert.Inconclusive();
    }

    [Test]
    public void GetDescendantsFromDepthTest()
    {
        // Markdig.Syntax.MarkdownObject source;
        // int minimumDepth;
        // IEnumerable<Markdig.Syntax.MarkdownObject> result = MarkdownExtensions.GetDescendantsFromDepth(source, minimumDepth);
        Assert.Inconclusive();
    }

    [Test]
    public void GetDescendantsUpToDepthTest()
    {
        // Markdig.Syntax.MarkdownObject source;
        // int maximumDepth;
        // IEnumerable<Markdig.Syntax.MarkdownObject> result = MarkdownExtensions.GetDescendantsUpToDepth(source, maximumDepth);
        Assert.Inconclusive();
    }

    [Test]
    public void GetDescendantsInDepthRangeTest()
    {
        // Markdig.Syntax.MarkdownObject source;
        // int minimumDepth;
        // int maximumDepth;
        // IEnumerable<Markdig.Syntax.MarkdownObject> result = MarkdownExtensions.GetDescendantsInDepthRange(source, minimumDepth, maximumDepth);
        Assert.Inconclusive();
    }

    [Test]
    public void GetDescendantBranchesMatchingType5Test()
    {
        // Markdig.Syntax.MarkdownObject source;
        // Type type;
        // int minimumDepth;
        // int maximumDepth;
        // IEnumerable<Markdig.Syntax.MarkdownObject> result = MarkdownExtensions.GetDescendantBranchesMatchingType(source, type, minimumDepth, maximumDepth);
        Assert.Inconclusive();
    }

    [Test]
    public void GetDescendantBranchesMatchingType6Test()
    {
        // Markdig.Syntax.MarkdownObject source;
        // ICollection<Type> types;
        // int minimumDepth;
        // int maximumDepth;
        // IEnumerable<Markdig.Syntax.MarkdownObject> result = MarkdownExtensions.GetDescendantBranchesMatchingType(source, types, minimumDepth, maximumDepth);
        Assert.Inconclusive();
    }
}