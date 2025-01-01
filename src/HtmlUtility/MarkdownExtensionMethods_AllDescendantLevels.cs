using System.Diagnostics;
using Markdig.Renderers.Html;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace HtmlUtility;

public static partial class MarkdownExtensionMethods
{
    internal static IEnumerable<MarkdownObject> GetDescendantsIncludingSelf(this MarkdownObject parent) =>
        parent.HasDirectDescendant(out IEnumerable<MarkdownObject>? descendants) ? ((IEnumerable<MarkdownObject>)[parent]).Concat(descendants) : [parent];

    internal static IEnumerable<MarkdownObject> GetDescendantsIncludingSelfAndAtributes(this MarkdownObject parent) =>
        parent.HasDirectDescendantIncludingAttributes(out IEnumerable<MarkdownObject>? descendants) ? ((IEnumerable<MarkdownObject>)[parent]).Concat(descendants) : [parent];

    /// <summary>
    /// Gets the descendants and attributes of a <see cref="MarkdownObject"/>.
    /// </summary>
    /// <param name="parent">The parent <see cref="MarkdownObject"/>.</param>
    /// <returns>Recursive descendants of <paramref name="parent"/>, including <see cref="HtmlAttributes"/>.</returns>
    internal static IEnumerable<MarkdownObject> GetDescendantsAndAttributes(this MarkdownObject parent)
    {
        Debug.Assert(parent is not null);
        var attribute = parent.TryGetAttributes();
        if (attribute is not null)
            yield return attribute;
        if (parent.HasDirectDescendantIncludingAttributes(out IEnumerable<MarkdownObject>? descendants))
            foreach (var item in descendants)
            {
                yield return item;
                if (item is ContainerBlock containerBlock)
                    foreach (var obj in containerBlock.GetDescendantsAndAttributes())
                        yield return obj;
                else if (item is LeafBlock leafBlock)
                    foreach (var obj in leafBlock.GetDescendantsAndAttributes())
                        yield return obj;
                else if ((attribute = item.TryGetAttributes()) is not null)
                    yield return attribute;
            }
    }

    /// <summary>
    /// Gets specific nested descendants of a <see cref="MarkdownObject"/>.
    /// </summary>
    /// <param name="parent">The parent <see cref="MarkdownObject"/>.</param>
    /// <param name="predicate">Function that specifies which descendant object to return.</param>
    /// <returns>Nested recursive descendants of <paramref name="parent"/> where <paramref name="predicate"/> returns true.</returns>
    /// <remarks>No descendants of yeilded items will be returned.</remarks>
    internal static IEnumerable<MarkdownObject> GetBranches(this MarkdownObject parent, Func<MarkdownObject, bool> predicate)
    {
        Debug.Assert(parent is not null);
        Debug.Assert(predicate is not null);
        if (parent.HasDirectDescendant(out IEnumerable<MarkdownObject>? descendants))
            foreach (var item in descendants)
            {
                if (predicate(item))
                    yield return item;
                else
                    foreach (var obj in item.GetBranches(predicate))
                        yield return obj;
            }
    }

    /// <summary>
    /// Gets specific nested descendants and attributes of a <see cref="MarkdownObject"/>.
    /// </summary>
    /// <param name="parent">The parent <see cref="MarkdownObject"/>.</param>
    /// <param name="predicate">Function that specifies which descendant object to return.</param>
    /// <returns>Nested recursive descendants where <paramref name="predicate"/> returns true, including <see cref="HtmlAttributes"/> where <paramref name="predicate"/> returns false.</returns>
    /// <remarks>No attributes or descendants of yeilded items will be returned.</remarks>
    internal static IEnumerable<MarkdownObject> GetBranchesIncludingAttributes(this MarkdownObject parent, Func<MarkdownObject, bool> predicate)
    {
        Debug.Assert(parent is not null);
        Debug.Assert(predicate is not null);
        var attribute = parent.TryGetAttributes();
        if (attribute is not null)
            yield return attribute;
        if (parent.HasDirectDescendant(out IEnumerable<MarkdownObject>? descendants))
            foreach (var item in descendants)
            {
                if (predicate(item))
                    yield return item;
                else
                    foreach (var obj in item.GetBranchesIncludingAttributes(predicate))
                        yield return obj;
            }
    }
}
