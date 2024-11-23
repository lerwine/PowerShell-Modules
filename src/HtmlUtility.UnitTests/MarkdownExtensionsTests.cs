using System.Diagnostics.CodeAnalysis;

namespace HtmlUtility.UnitTests;

public class MarkdownExtensionsTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void ToReflectionTypeTest()
    {
        // MarkdownTokenType type;
        // Type actual = MarkdownExtensions.ToReflectionType(type);
        Assert.Inconclusive();
    }

    [Test]
    public void ToReflectionTypesTest()
    {
        // MarkdownTokenType[] types;
        // List<Type>? actual = MarkdownExtensions.ToReflectionTypes(types);
        Assert.Inconclusive();
    }

    [Test]
    public void GetChildObjectsTest()
    {
        // Markdig.Syntax.MarkdownObject source;
        // IEnumerable<Markdig.Syntax.MarkdownObject> result = MarkdownExtensions.GetChildObjects(source);
        Assert.Inconclusive();
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