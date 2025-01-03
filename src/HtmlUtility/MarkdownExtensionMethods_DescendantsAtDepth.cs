using System.Diagnostics;
using Markdig.Renderers.Html;
using Markdig.Syntax;

namespace HtmlUtility;

public static partial class MarkdownExtensionMethods
{
    /// <summary>
    /// Gets the nested descendants of a <see cref="MarkdownObject"/> at a specified depth.
    /// </summary>
    /// <param name="parent">The parent <see cref="MarkdownObject"/>.</param>
    /// <param name="depth">The depth at which to return nested descendants.</param>
    /// <returns>Nested desendants at <paramref name="depth"/> levels deep, not including <see cref="HtmlAttributes"/>.</returns>
    internal static IEnumerable<MarkdownObject> GetDescendantsAtDepth(this MarkdownObject parent, int depth)
    {
        Debug.Assert(parent is not null);
        Debug.Assert(depth > 0);
        if (parent.HasDirectDescendant(out IEnumerable<MarkdownObject>? childObjects))
        {
            if (depth > 1)
            {
                depth--;
                return childObjects.SelectMany(item => item.GetDescendantsAtDepth(depth));
            }
            return childObjects;
        }
        return [];
    }

    /// <summary>
    /// Gets the nested descendants of a <see cref="MarkdownObject"/> at a specified depth, including <see cref="HtmlAttributes"/>.
    /// </summary>
    /// <param name="parent">The parent <see cref="MarkdownObject"/>.</param>
    /// <param name="depth">The depth at which to return nested descendants.</param>
    /// <returns>Nested desendants at <paramref name="depth"/> levels deep, including <see cref="HtmlAttributes"/>.</returns>
    internal static IEnumerable<MarkdownObject> GetDescendantsAtDepthIncludingAttributes(this MarkdownObject parent, int depth)
    {
        Debug.Assert(parent is not null);
        Debug.Assert(depth > 0);
        if (parent.HasDirectDescendant(out IEnumerable<MarkdownObject>? childObjects))
        {
            if (depth > 1)
            {
                depth--;
                return childObjects.SelectMany(item => item.GetDescendantsAtDepthIncludingAttributes(depth));
            }
            return parent.TryGetAttributes().PrependToMarkdownObjectsIfNotNull(childObjects);
        }
        return (depth > 1) ? [] : parent.TryGetAttributes().EnumerateMarkdownObjectIfNotNull();
    }

    /// <summary>
    /// Gets specific nested descendants of a <see cref="MarkdownObject"/> at a specified depth.
    /// </summary>
    /// <param name="parent">The parent <see cref="MarkdownObject"/>.</param>
    /// <param name="depth">The depth at which to return nested descendants.</param>
    /// <param name="predicate">Function that specifies which descendant object to return.</param>
    /// <returns>Nested desendants at <paramref name="depth"/> levels deep where <paramref name="predicate"/> returns true, not including <see cref="HtmlAttributes"/>.</returns>
    /// <remarks>No descendants of yeilded items will be returned.</remarks>
    internal static IEnumerable<MarkdownObject> GetDescendantsAtDepth(this MarkdownObject parent, int depth, Func<MarkdownObject, bool> predicate)
    {
        Debug.Assert(parent is not null);
        Debug.Assert(predicate is not null);
        Debug.Assert(depth > 0);
        if (parent.HasDirectDescendant(out IEnumerable<MarkdownObject>? childObjects))
        {
            if (depth > 1)
            {
                depth--;
                return childObjects.SelectMany(item => item.GetDescendantsAtDepth(depth, predicate));
            }
            return childObjects.Where(predicate);
        }
        return [];
    }

    /// <summary>
    /// Gets specific nested descendants of a <see cref="MarkdownObject"/> at a specified depth, including <see cref="HtmlAttributes"/>.
    /// </summary>
    /// <param name="parent">The parent <see cref="MarkdownObject"/>.</param>
    /// <param name="depth">The depth at which to return nested descendants.</param>
    /// <param name="predicate">Function that specifies which descendant object to return.</param>
    /// <returns>Nested desendants at <paramref name="depth"/> levels deep where <paramref name="predicate"/> returns true, including <see cref="HtmlAttributes"/> at the specified depth.</returns>
    /// <remarks>No descendants of yeilded items will be returned.</remarks>
    internal static IEnumerable<MarkdownObject> GetDescendantsAtDepthIncludingAttributes(this MarkdownObject parent, int depth, Func<MarkdownObject, bool> predicate)
    {
        Debug.Assert(parent is not null);
        Debug.Assert(predicate is not null);
        Debug.Assert(depth > 0);
        if (parent.HasDirectDescendant(out IEnumerable<MarkdownObject>? childObjects))
        {
            if (depth > 1)
            {
                depth--;
                return childObjects.SelectMany(item => item.GetDescendantsAtDepthIncludingAttributes(depth, predicate));
            }
            return parent.TryGetAttributes().PrependToMarkdownObjectsIfNotNull(childObjects.Where(predicate));
        }

        return (depth > 1) ? [] : parent.TryGetAttributes().EnumerateMarkdownObjectIfNotNull();
    }
}
