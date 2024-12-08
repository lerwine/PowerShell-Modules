using Markdig.Syntax;

namespace HtmlUtility;

public interface IMarkdownDescendantEnumerator : IEnumerator<MarkdownObject>
{
    int CurrentDepth { get; }
    
    bool MoveNextSkippingDescendants();
}
