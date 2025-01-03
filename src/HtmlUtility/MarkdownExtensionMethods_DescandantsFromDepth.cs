using System.Diagnostics;
using Markdig.Renderers.Html;
using Markdig.Syntax;

namespace HtmlUtility;

public static partial class MarkdownExtensionMethods
{
    /// <summary>
    /// Gets the nested descendants of a <see cref="MarkdownObject"/> starting from a specified depth.
    /// </summary>
    /// <param name="parent">The parent <see cref="MarkdownObject"/>.</param>
    /// <param name="minDepth">The minimum depth of the recursive nested descendants to return. This must not be less than 1.</param>
    /// <returns>Nested desendants at or greater than <paramref name="minDepth"/> levels deep, not including <see cref="HtmlAttributes"/>.</returns>
    internal static IEnumerable<MarkdownObject> GetDescendantsFromDepth(this MarkdownObject parent, int minDepth)
    {
        Debug.Assert(parent is not null);
        Debug.Assert(minDepth > 0);
        if (parent.HasDirectDescendant(out IEnumerable<MarkdownObject>? descendants))
        {
            if (minDepth > 1)
            {
                minDepth--;
                return descendants.SelectMany(item => item.GetDescendantsFromDepth(minDepth));
            }
            return descendants.SelectMany(GetDescendantsIncludingSelf);
        }
        return [];
    }

    /// <summary>
    /// Gets the nested descendants and attributes of a <see cref="MarkdownObject"/> starting from a specified depth.
    /// </summary>
    /// <param name="parent">The parent <see cref="MarkdownObject"/>.</param>
    /// <param name="minDepth">The minimum depth of the recursive nested descendants to return. This must not be less than 1.</param>
    /// <returns>Nested desendants at or greater than <paramref name="minDepth"/> levels deep, including <see cref="HtmlAttributes"/>.</returns>
    internal static IEnumerable<MarkdownObject> GetDescendantsAndAttributesFromDepth(this MarkdownObject parent, int minDepth)
    {
        Debug.Assert(parent is not null);
        Debug.Assert(minDepth > 0);
        if (parent.HasDirectDescendant(out IEnumerable<MarkdownObject>? descendants))
        {
            if (minDepth > 1)
            {
                minDepth--;
                return descendants.SelectMany(item => item.GetDescendantsAndAttributesFromDepth(minDepth));
            }
            var attributes = parent.TryGetAttributes();
            return parent.TryGetAttributes().PrependToMarkdownObjectsIfNotNull(descendants.SelectMany(GetDescendantsIncludingSelf));
        }
        return (minDepth > 1) ? [] : parent.TryGetAttributes().EnumerateMarkdownObjectIfNotNull();
    }

    /// <summary>
    /// Gets specific nested descendants of a <see cref="MarkdownObject"/> starting from a specified depth.
    /// </summary>
    /// <param name="parent">The parent <see cref="ContainMarkdownObjecterBlock"/>.</param>
    /// <param name="minDepth">The minimum depth of the recursive nested descendants to return. This must not be less than 1.</param>
    /// <param name="predicate">Function that specifies which descendant object to return.</param>
    /// <returns>Nested desendants at or greater than <paramref name="minDepth"/> levels deep where <paramref name="predicate"/> returns true, not including <see cref="HtmlAttributes"/>.</returns>
    /// <remarks>No descendants of yeilded items will be returned.</remarks>
    internal static IEnumerable<MarkdownObject> GetBranchesFromDepth(this MarkdownObject parent, int minDepth, Func<MarkdownObject, bool> predicate)
    {
        Debug.Assert(parent is not null);
        Debug.Assert(predicate is not null);
        Debug.Assert(minDepth > 0);

        if (parent.HasDirectDescendant(out IEnumerable<MarkdownObject>? descendants))
        {
            if (minDepth > 1)
            {
                minDepth--;
                return descendants.SelectMany(item => item.GetBranchesFromDepth(minDepth, predicate));
            }
            return descendants.Where(predicate);
        }
        return [];
    }

    /// <summary>
    /// Gets specific nested descendants and attributes of a <see cref="MarkdownObject"/> starting from a specified depth.
    /// </summary>
    /// <param name="parent">The parent <see cref="MarkdownObject"/>.</param>
    /// <param name="minDepth">The minimum depth of the recursive nested descendants to return. This must not be less than 1.</param>
    /// <param name="predicate">Function that specifies which descendant object to return.</param>
    /// <returns>Nested desendants at or greater than <paramref name="minDepth"/> levels deep where <paramref name="predicate"/> returns true,
    /// including <see cref="HtmlAttributes"/> of the <paramref name="parent"/> and where <paramref name="predicate"/> returns false.</returns>
    /// <remarks>No attributes or descendants of yeilded items will be returned.</remarks>
    internal static IEnumerable<MarkdownObject> GetBranchesIncludingAttributesFromDepth(this MarkdownObject parent, int minDepth, Func<MarkdownObject, bool> predicate)
    {
        Debug.Assert(parent is not null);
        Debug.Assert(predicate is not null);
        Debug.Assert(minDepth > 0);
        if (parent.HasDirectDescendant(out IEnumerable<MarkdownObject>? descendants))
        {
            if (minDepth > 1)
            {
                minDepth--;
                return descendants.SelectMany(item => item.GetBranchesIncludingAttributesFromDepth(minDepth, predicate));
            }
            return parent.TryGetAttributes().PrependToMarkdownObjectsIfNotNull(descendants.SelectMany(GetDescendantsIncludingSelf));
        }
        return (minDepth > 1) ? [] : parent.TryGetAttributes().EnumerateMarkdownObjectIfNotNull();
    }
}
