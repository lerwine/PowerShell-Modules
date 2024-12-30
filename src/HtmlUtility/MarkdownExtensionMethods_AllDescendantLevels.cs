using System.Diagnostics;
using Markdig.Renderers.Html;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace HtmlUtility;

public static partial class MarkdownExtensionMethods
{
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
    /// Gets specific nested descendants of a <see cref="ContainerBlock"/>.
    /// </summary>
    /// <param name="parent">The parent <see cref="ContainerBlock"/>.</param>
    /// <param name="predicate">Function that specifies which descendant object to return.</param>
    /// <returns>Nested recursive descendants of <paramref name="parent"/> where <paramref name="predicate"/> returns true.</returns>
    /// <remarks>No descendants of yeilded items will be returned.</remarks>
    internal static IEnumerable<MarkdownObject> GetBranches(this ContainerBlock parent, Func<MarkdownObject, bool> predicate)
    {
        Debug.Assert(parent is not null);
        Debug.Assert(predicate is not null);
        if (parent.HasDirectDescendant(out IEnumerable<MarkdownObject>? descendants))
            foreach (var item in descendants)
            {
                if (predicate(item))
                    yield return item;
                else if (item is ContainerBlock containerBlock)
                    foreach (var obj in containerBlock.GetBranches(predicate))
                        yield return obj;
                else if (item is LeafBlock leafBlock)
                    foreach (var obj in leafBlock.GetBranches(predicate))
                        yield return obj;
            }
    }

    /// <summary>
    /// Gets specific nested descendants of a <see cref="ContainerInline"/>.
    /// </summary>
    /// <param name="parent">The parent <see cref="ContainerInline"/>.</param>
    /// <param name="predicate">Function that specifies which descendant object to return.</param>
    /// <returns>Direct descendants of <paramref name="parent"/> where <paramref name="predicate"/> returns true.</returns>
    /// <remarks>No descendants of yeilded items will be returned.</remarks>
    internal static IEnumerable<MarkdownObject> GetBranches(this ContainerInline parent, Func<MarkdownObject, bool> predicate)
    {
        Debug.Assert(parent is not null);
        Debug.Assert(predicate is not null);
        if (parent.HasDirectDescendant(out IEnumerable<MarkdownObject>? descendants))
        {
            foreach (var item in descendants)
            {
                if (predicate(item))
                    yield return item;
                else if (item is ContainerInline containerInline)
                    foreach (var obj in containerInline.GetBranches(predicate))
                        yield return obj;
            }
        }
    }

    /// <summary>
    /// Gets specific nested descendants of a <see cref="LeafBlock"/>.
    /// </summary>
    /// <param name="parent">The parent <see cref="LeafBlock"/>.</param>
    /// <param name="predicate">Function that specifies which descendant object to return.</param>
    /// <returns>Nested recursive descendants where <paramref name="predicate"/> returns true.</returns>
    /// <remarks>No descendants of yeilded items will be returned.</remarks>
    internal static IEnumerable<MarkdownObject> GetBranches(this LeafBlock parent, Func<MarkdownObject, bool> predicate)
    {
        Debug.Assert(parent is not null);
        Debug.Assert(predicate is not null);
        return (parent.Inline is null) ? [] : parent.Inline.GetBranches(predicate);
    }

    /// <summary>
    /// Gets specific nested descendants and attributes of a <see cref="ContainerBlock"/>.
    /// </summary>
    /// <param name="parent">The parent <see cref="ContainerBlock"/>.</param>
    /// <param name="predicate">Function that specifies which descendant object to return.</param>
    /// <returns>Nested recursive descendants, including <see cref="HtmlAttributes"/>, where <paramref name="predicate"/> returns true.</returns>
    /// <remarks>No attributes or descendants of yeilded items will be returned.</remarks>
    internal static IEnumerable<MarkdownObject> GetBranchesIncludingAttributes(this ContainerBlock parent, Func<MarkdownObject, bool> predicate)
    {
        Debug.Assert(parent is not null);
        Debug.Assert(predicate is not null);
        var attribute = parent.TryGetAttributes();
        if (attribute is not null && predicate(attribute))
            yield return attribute;
        if (parent.HasDirectDescendant(out IEnumerable<MarkdownObject>? descendants))
            foreach (var item in descendants)
            {
                if (predicate(item))
                    yield return item;
                else if (item is ContainerBlock containerBlock)
                    foreach (var obj in containerBlock.GetBranchesIncludingAttributes(predicate))
                        yield return obj;
                else if (item is LeafBlock leafBlock)
                    foreach (var obj in leafBlock.GetBranchesIncludingAttributes(predicate))
                        yield return obj;
                else if ((attribute = item.TryGetAttributes()) is not null && predicate(attribute))
                    yield return attribute;
            }
    }

    /// <summary>
    /// Gets specific nested descendants and attributes of a <see cref="ContainerInline"/>.
    /// </summary>
    /// <param name="parent">The parent <see cref="ContainerInline"/>.</param>
    /// <param name="predicate">Function that specifies which descendant object to return.</param>
    /// <returns>Nested recursive descendants, including <see cref="HtmlAttributes"/>, where <paramref name="predicate"/> returns true.</returns>
    /// <remarks>No attributes or descendants of yeilded items will be returned.</remarks>
    internal static IEnumerable<MarkdownObject> GetBranchesIncludingAttributes(this ContainerInline parent, Func<MarkdownObject, bool> predicate)
    {
        Debug.Assert(parent is not null);
        Debug.Assert(predicate is not null);
        var attribute = parent.TryGetAttributes();
        if (attribute is not null && predicate(attribute))
            yield return attribute;
        if (parent.HasDirectDescendant(out IEnumerable<MarkdownObject>? descendants))
            foreach (var item in descendants)
            {
                if (predicate(item))
                    yield return item;
                else if (item is ContainerInline containerInline)
                    foreach (var obj in containerInline.GetBranchesIncludingAttributes(predicate))
                        yield return obj;
                else if ((attribute = item.TryGetAttributes()) is not null && predicate(attribute))
                    yield return attribute;
            }
    }

    /// <summary>
    /// Gets specific nested descendants and attributes of a <see cref="LeafBlock"/>.
    /// </summary>
    /// <param name="parent">The parent <see cref="LeafBlock"/>.</param>
    /// <param name="predicate">Function that specifies which descendant object to return.</param>
    /// <returns>Nested recursive descendants, including <see cref="HtmlAttributes"/>, where <paramref name="predicate"/> returns true.</returns>
    /// <remarks>No attributes or descendants of yeilded items will be returned.</remarks>
    internal static IEnumerable<MarkdownObject> GetBranchesIncludingAttributes(this LeafBlock parent, Func<MarkdownObject, bool> predicate)
    {
        Debug.Assert(parent is not null);
        Debug.Assert(predicate is not null);
        var attribute = parent.TryGetAttributes();
        if (attribute is not null && predicate(attribute))
            yield return attribute;
        if (parent.Inline is not null)
            foreach (var item in parent.Inline.GetBranchesIncludingAttributes(predicate))
                yield return item;
    }
}
