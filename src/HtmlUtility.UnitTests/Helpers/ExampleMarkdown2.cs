using Markdig;
using Markdig.Syntax;

namespace HtmlUtility.UnitTests.Helpers;

public static partial class ExampleMarkdown2
{
    internal const string SourceFileName = "Example2.md";
    internal const string JsonTestOutputFileName = "Example2.json";

    internal static string GetSourcePath() => Path.Combine(TestHelper.GetResourcesDirectoryPath(), SourceFileName);

    internal static string GetMarkdownSourceText() => File.ReadAllText(GetSourcePath());

    internal static MarkdownDocument GetMarkdownDocument() => Markdown.Parse(GetMarkdownSourceText(), new MarkdownPipelineBuilder().UseAdvancedExtensions().Build());
}
