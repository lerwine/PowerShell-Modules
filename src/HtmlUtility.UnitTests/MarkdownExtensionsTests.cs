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
        return MarkdownExtensionMethods.ToReflectionType(type);
    }

    [TestCaseSource(typeof(TestData), nameof(TestData.GetToReflectionTypesTestData))]
    public Type[]? ToReflectionTypesTest(IList<MarkdownTokenType>? types)
    {
        return MarkdownExtensionMethods.ToReflectionTypes(types)?.ToArray();
    }

    [TestCaseSource(typeof(TestData), nameof(TestData.GetGetChildObjectsTestData))]
    public Tuple<Type, Markdig.Syntax.SourceSpan>[] GetChildObjectsTest(Markdig.Syntax.MarkdownObject source)
    {
        return MarkdownExtensionMethods.GetChildObjects(source).Select(obj => new Tuple<Type, Markdig.Syntax.SourceSpan>(obj.GetType(), obj.Span)).ToArray();
    }

    [TestCaseSource(typeof(TestData), nameof(TestData.GetGetAllDescendantsTestData))]
    public Tuple<Type, Markdig.Syntax.SourceSpan>[] GetAllDescendantsTest(Markdig.Syntax.MarkdownObject source)
    {
        return MarkdownExtensionMethods.GetAllDescendants(source).Select(obj => new Tuple<Type, Markdig.Syntax.SourceSpan>(obj.GetType(), obj.Span)).ToArray();
    }

    [TestCaseSource(typeof(TestData), nameof(TestData.GetGetDescendantBranchesMatchingType1TestData))]
    public Tuple<Type, Markdig.Syntax.SourceSpan, int, int>[] GetDescendantBranchesMatchingType1Test(Markdig.Syntax.MarkdownObject source, Type type)
    {
        return MarkdownExtensionMethods.GetDescendantBranchesMatchingType(source, type)
            .Select(r => new Tuple<Type, Markdig.Syntax.SourceSpan, int, int>(r.GetType(), r.Span, r.Line, r.Column)).ToArray();
    }

    [TestCaseSource(typeof(TestData), nameof(TestData.GetGetDescendantBranchesMatchingType2TestData))]
    public Tuple<Type, Markdig.Syntax.SourceSpan, int, int>[] GetDescendantBranchesMatchingType2Test(Markdig.Syntax.MarkdownObject source, ICollection<Type> types)
    {
        return MarkdownExtensionMethods.GetDescendantBranchesMatchingType(source, types)
            .Select(r => new Tuple<Type, Markdig.Syntax.SourceSpan, int, int>(r.GetType(), r.Span, r.Line, r.Column)).ToArray();
    }

    [Test]
    public void GetDescendantBranchesMatchingType3Test()
    {
        // Markdig.Syntax.MarkdownObject source;
        // Type type;
        // int maximumDepth;
        // IEnumerable<Markdig.Syntax.MarkdownObject> result = MarkdownExtensionMethods.GetDescendantBranchesMatchingType(source, type, maximumDepth);
        Assert.Inconclusive();
    }

    [Test]
    public void GetDescendantBranchesMatchingType4Test()
    {
        // Markdig.Syntax.MarkdownObject source;
        // ICollection<Type> types;
        // int maximumDepth;
        // IEnumerable<Markdig.Syntax.MarkdownObject> result = MarkdownExtensionMethods.GetDescendantBranchesMatchingType(source, types, maximumDepth);
        Assert.Inconclusive();
    }

    [Test]
    public void GetDescendantsAtDepthTest()
    {
        // Markdig.Syntax.MarkdownObject source;
        // int depth;
        // IEnumerable<Markdig.Syntax.MarkdownObject> result = MarkdownExtensionMethods.GetDescendantsAtDepth(source, depth);
        Assert.Inconclusive();
    }

    [Test]
    public void GetDescendantsFromDepthTest()
    {
        // Markdig.Syntax.MarkdownObject source;
        // int minimumDepth;
        // IEnumerable<Markdig.Syntax.MarkdownObject> result = MarkdownExtensionMethods.GetDescendantsFromDepth(source, minimumDepth);
        Assert.Inconclusive();
    }

    [Test]
    public void GetDescendantsUpToDepthTest()
    {
        // Markdig.Syntax.MarkdownObject source;
        // int maximumDepth;
        // IEnumerable<Markdig.Syntax.MarkdownObject> result = MarkdownExtensionMethods.GetDescendantsUpToDepth(source, maximumDepth);
        Assert.Inconclusive();
    }

    [Test]
    public void GetDescendantsInDepthRangeTest()
    {
        // Markdig.Syntax.MarkdownObject source;
        // int minimumDepth;
        // int maximumDepth;
        // IEnumerable<Markdig.Syntax.MarkdownObject> result = MarkdownExtensionMethods.GetDescendantsInDepthRange(source, minimumDepth, maximumDepth);
        Assert.Inconclusive();
    }

    [Test]
    public void GetDescendantBranchesMatchingType5Test()
    {
        // Markdig.Syntax.MarkdownObject source;
        // Type type;
        // int minimumDepth;
        // int maximumDepth;
        // IEnumerable<Markdig.Syntax.MarkdownObject> result = MarkdownExtensionMethods.GetDescendantBranchesMatchingType(source, type, minimumDepth, maximumDepth);
        Assert.Inconclusive();
    }

    [Test]
    public void GetDescendantBranchesMatchingType6Test()
    {
        // Markdig.Syntax.MarkdownObject source;
        // ICollection<Type> types;
        // int minimumDepth;
        // int maximumDepth;
        // IEnumerable<Markdig.Syntax.MarkdownObject> result = MarkdownExtensionMethods.GetDescendantBranchesMatchingType(source, types, minimumDepth, maximumDepth);
        Assert.Inconclusive();
    }
}