using System.Diagnostics;
using Markdig.Renderers.Html;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace HtmlUtility;

public static partial class MarkdownExtensionMethods
{
    /// <summary>
    /// Gets the nested descendants of a <see cref="ContainerBlock"/> at a specified depth.
    /// </summary>
    /// <param name="parent">The parent <see cref="ContainerBlock"/>.</param>
    /// <param name="depth">The depth at which to return nested descendants.</param>
    /// <returns>Nested desendants at <paramref name="depth"/> levels deep, not including <see cref="HtmlAttributes"/>.</returns>
    // TODO: Rename to DescendantsAtDepth
    internal static IEnumerable<MarkdownObject> GetNestedDescendantsAtDepth(this ContainerBlock parent, int depth)
    {
        Debug.Assert(parent is not null);
        Debug.Assert(depth > 0);
        if (parent.HasDirectDescendants(out IEnumerable<MarkdownObject>? descendants))
        {
            if (depth == 1)
                foreach (var item in descendants)
                    yield return item;
            else
            {
                depth--;
                foreach (var item in descendants)
                {
                    if (item is ContainerBlock containerBlock)
                        foreach (var obj in containerBlock.GetNestedDescendantsAtDepth(depth))
                            yield return obj;
                    else if (item is LeafBlock leafBlock)
                        foreach (var obj in leafBlock.GetNestedDescendantsAtDepth(depth))
                            yield return obj;
                }
            }
        }
    }

    /// <summary>
    /// Gets the nested descendants of a <see cref="ContainerInline"/> at a specified depth.
    /// </summary>
    /// <param name="parent">The parent <see cref="ContainerInline"/>.</param>
    /// <param name="depth">The depth at which to return nested descendants.</param>
    /// <returns>Nested desendants at <paramref name="depth"/> levels deep, not including <see cref="HtmlAttributes"/>.</returns>
    // TODO: Rename to DescendantsAtDepth
    internal static IEnumerable<MarkdownObject> GetNestedDescendantsAtDepth(this ContainerInline parent, int depth)
    {
        Debug.Assert(parent is not null);
        Debug.Assert(depth > 0);
        if (parent.HasDirectDescendants(out IEnumerable<MarkdownObject>? descendants))
        {
            if (depth == 1)
                foreach (var item in descendants)
                    yield return item;
            else
            {
                depth--;
                foreach (var item in descendants)
                {
                    if (item is ContainerInline containerInline)
                        foreach (var obj in containerInline.GetNestedDescendantsAtDepth(depth))
                            yield return obj;
                }
            }
        }
    }

    /// <summary>
    /// Gets the nested descendants of a <see cref="LeafBlock"/> at a specified depth.
    /// </summary>
    /// <param name="parent">The parent <see cref="LeafBlock"/>.</param>
    /// <param name="depth">The depth at which to return nested descendants.</param>
    /// <returns>Nested desendants at <paramref name="depth"/> levels deep, not including <see cref="HtmlAttributes"/>.</returns>
    // TODO: Rename to DescendantsAtDepth
    internal static IEnumerable<MarkdownObject> GetNestedDescendantsAtDepth(this LeafBlock parent, int depth)
    {
        Debug.Assert(parent is not null);
        return (parent.Inline is null) ? [] : parent.Inline.GetNestedDescendantsAtDepth(depth);
    }

    /// <summary>
    /// Gets specific nested descendants of a <see cref="ContainerBlock"/> at a specified depth.
    /// </summary>
    /// <param name="parent">The parent <see cref="ContainerBlock"/>.</param>
    /// <param name="depth">The depth at which to return nested descendants.</param>
    /// <param name="predicate">Function that specifies which descendant object to return.</param>
    /// <returns>Nested desendants at <paramref name="depth"/> levels deep where <paramref name="predicate"/> returns true, not including <see cref="HtmlAttributes"/>.</returns>
    /// <remarks>No descendants of yeilded items will be returned.</remarks>
    // TODO: Rename to DescendantsAtDepth
    internal static IEnumerable<MarkdownObject> GetNestedDescendantsAtDepth(this ContainerBlock parent, int depth, Func<MarkdownObject, bool> predicate)
    {
        Debug.Assert(parent is not null);
        Debug.Assert(predicate is not null);
        Debug.Assert(depth > 0);
        if (parent.HasDirectDescendants(out IEnumerable<MarkdownObject>? descendants))
        {
            if (depth == 1)
                foreach (var item in descendants.Where(predicate))
                    yield return item;
            else
            {
                depth--;
                foreach (var item in descendants)
                {
                    if (item is ContainerBlock containerBlock)
                        foreach (var obj in containerBlock.GetNestedDescendantsAtDepth(depth, predicate))
                            yield return obj;
                    else if (item is LeafBlock leafBlock)
                        foreach (var obj in leafBlock.GetNestedDescendantsAtDepth(depth, predicate))
                            yield return obj;
                }
            }
        }
    }

    /// <summary>
    /// Gets specific nested descendants of a <see cref="ContainerInline"/> at a specified depth.
    /// </summary>
    /// <param name="parent">The parent <see cref="ContainerInline"/>.</param>
    /// <param name="depth">The depth at which to return nested descendants.</param>
    /// <param name="predicate">Function that specifies which descendant object to return.</param>
    /// <returns>Nested desendants at <paramref name="depth"/> levels deep where <paramref name="predicate"/> returns true, not including <see cref="HtmlAttributes"/>.</returns>
    /// <remarks>No descendants of yeilded items will be returned.</remarks>
    // TODO: Rename to DescendantsAtDepth
    internal static IEnumerable<MarkdownObject> GetNestedDescendantsAtDepth(this ContainerInline parent, int depth, Func<MarkdownObject, bool> predicate)
    {
        Debug.Assert(parent is not null);
        Debug.Assert(predicate is not null);
        Debug.Assert(depth > 0);
        if (parent.HasDirectDescendants(out IEnumerable<MarkdownObject>? descendants))
        {
            if (depth == 1)
                foreach (var item in descendants.Where(predicate))
                    yield return item;
            else
            {
                depth--;
                foreach (var item in descendants)
                {
                    if (item is ContainerInline containerInline)
                        foreach (var obj in containerInline.GetNestedDescendantsAtDepth(depth, predicate))
                            yield return obj;
                }
            }
        }
    }

    /// <summary>
    /// Gets specific nested descendants of a <see cref="LeafBlock"/> at a specified depth.
    /// </summary>
    /// <param name="parent">The parent <see cref="LeafBlock"/>.</param>
    /// <param name="depth">The depth at which to return nested descendants.</param>
    /// <param name="predicate">Function that specifies which descendant object to return.</param>
    /// <returns>Nested desendants at <paramref name="depth"/> levels deep where <paramref name="predicate"/> returns true, not including <see cref="HtmlAttributes"/>.</returns>
    /// <remarks>No descendants of yeilded items will be returned.</remarks>
    // TODO: Rename to DescendantsAtDepth
    internal static IEnumerable<MarkdownObject> GetNestedDescendantsAtDepth(this LeafBlock parent, int depth, Func<MarkdownObject, bool> predicate)
    {
        Debug.Assert(parent is not null);
        Debug.Assert(predicate is not null);
        return (parent.Inline is null) ? [] : parent.Inline.GetNestedDescendantsAtDepth(depth, predicate);
    }
}
