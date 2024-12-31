using System.Diagnostics;
using Markdig.Renderers.Html;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace HtmlUtility;

public static partial class MarkdownExtensionMethods
{
    /// <summary>
    /// Gets the nested descendants of a <see cref="MarkdownObject"/> within an inclusive depth range.
    /// </summary>
    /// <param name="parent">The parent <see cref="MarkdownObject"/>.</param>
    /// <param name="minDepth">The minimum depth of the recursive nested descendants to return. This must not be less than 1.</param>
    /// <param name="maxDepth">The maximum depth of the recursive descendants to return. This must greater than <paramref name="minDepth"/>.</param>
    /// <returns>Nested desendants not less than <paramref name="minDepth"/> and not greater than <paramref name="maxDepth"/> levels deep, not including <see cref="HtmlAttributes"/>.</returns>
    internal static IEnumerable<MarkdownObject> GetDescendantsInDepthRange(this MarkdownObject parent, int minDepth, int maxDepth)
    {
        Debug.Assert(parent is not null);
        Debug.Assert(minDepth > 0);
        Debug.Assert(maxDepth > minDepth);
        if (parent.HasDirectDescendant(out IEnumerable<MarkdownObject>? descendants))
        {
            maxDepth -= minDepth;
            if (minDepth > 1)
            {
                minDepth--;
                descendants = descendants.SelectMany(item => item.GetDescendantsAtDepth(minDepth));
            }
            return descendants.SelectMany(item => item.GetDescendantsToDepthIncludingSelf(maxDepth));
        }
        return [];
    }

    /// <summary>
    /// Gets the nested descendants and attributes of a <see cref="MarkdownObject"/> within an inclusive depth range.
    /// </summary>
    /// <param name="parent">The parent <see cref="MarkdownObject"/>.</param>
    /// <param name="minDepth">The minimum depth of the recursive nested descendants to return. This must not be less than 1.</param>
    /// <param name="maxDepth">The maximum depth of the recursive descendants to return. This must greater than <paramref name="minDepth"/>.</param>
    /// <returns>Nested desendants not less than <paramref name="minDepth"/> and not greater than <paramref name="maxDepth"/> levels deep, including <see cref="HtmlAttributes"/>.</returns>
    internal static IEnumerable<MarkdownObject> GetDescendantsAndAttributesInDepthRange(this MarkdownObject parent, int minDepth, int maxDepth)
    {
        Debug.Assert(parent is not null);
        Debug.Assert(minDepth > 1);
        Debug.Assert(maxDepth > minDepth);
        if (parent.HasDirectDescendant(out IEnumerable<MarkdownObject>? descendants))
        {
            if (minDepth > 1)
            {
                minDepth--;
                maxDepth--;
                return descendants.SelectMany(item => item.GetDescendantsAndAttributesInDepthRange(minDepth, maxDepth));
            }
            maxDepth -= minDepth;
            return parent.TryGetAttributes().PrependToMarkdownObjectsIfNotNull(descendants.SelectMany(item => item.GetDescendantsAndAttributesToDepthIncludingSelf(maxDepth)));
        }
        return (minDepth > 1) ? [] : parent.TryGetAttributes().EnumerateMarkdownObjectIfNotNull();
    }

    /// <summary>
    /// Gets specific nested descendants of a <see cref="MarkdownObject"/> within a specified range.
    /// </summary>
    /// <param name="parent">The parent <see cref="MarkdownObject"/>.</param>
    /// <param name="minDepth">The minimum depth of the recursive nested descendants to return. This must not be less than 1.</param>
    /// <param name="maxDepth">The maximum depth of the recursive descendants to return. This must greater than <paramref name="minDepth"/>.</param>
    /// <param name="predicate">Function that specifies which descendant object to return.</param>
    /// <returns>Nested desendants not less than <paramref name="minDepth"/> and not greater than <paramref name="maxDepth"/> levels deep, not including <see cref="HtmlAttributes"/>.</returns>
    /// <remarks>No descendants of yeilded items will be returned.</remarks>
    internal static IEnumerable<MarkdownObject> GetBranchesInDepthRange(this MarkdownObject parent, int minDepth, int maxDepth, Func<MarkdownObject, bool> predicate)
    {
        Debug.Assert(parent is not null);
        Debug.Assert(predicate is not null);
        Debug.Assert(minDepth > 1);
        Debug.Assert(maxDepth > minDepth);
        if (parent.HasDirectDescendant(out IEnumerable<MarkdownObject>? descendants))
        {
            if (minDepth > 1)
            {
                minDepth--;
                maxDepth--;
                foreach (var item in descendants.SelectMany(item => item.GetBranchesInDepthRange(minDepth, maxDepth, predicate)))
                    yield return item;
            }
            else
            {
                maxDepth -= minDepth;
                foreach (var item in descendants)
                {
                    if (predicate(item))
                        yield return item;
                    else
                        foreach (var obj in item.GetBranchesToDepth(maxDepth, predicate))
                            yield return obj;
                }
            }
        }
    }

    /// <summary>
    /// Gets specific nested descendants and attributes of a <see cref="MarkdownObject"/> within a specified range.
    /// </summary>
    /// <param name="parent">The parent <see cref="MarkdownObject"/>.</param>
    /// <param name="minDepth">The minimum depth of the recursive nested descendants to return. This must not be less than 1.</param>
    /// <param name="maxDepth">The maximum depth of the recursive descendants to return. This must greater than <paramref name="minDepth"/>.</param>
    /// <param name="predicate">Function that specifies which descendant object to return.</param>
    /// <returns>Nested desendants not less than <paramref name="minDepth"/> and not greater than <paramref name="maxDepth"/> levels deep where <paramref name="predicate"/> returns true,
    /// including <see cref="HtmlAttributes"/> where <paramref name="predicate"/> returns false.</returns>
    /// <remarks>No attributes or descendants of yeilded items will be returned.</remarks>
    internal static IEnumerable<MarkdownObject> GetBranchesIncludingAttributesInDepthRange(this MarkdownObject parent, int minDepth, int maxDepth, Func<MarkdownObject, bool> predicate)
    {
        Debug.Assert(parent is not null);
        Debug.Assert(predicate is not null);
        Debug.Assert(minDepth > 1);
        Debug.Assert(maxDepth > minDepth);
        if (parent.HasDirectDescendant(out IEnumerable<MarkdownObject>? descendants))
        {
            if (minDepth > 1)
            {
                minDepth--;
                maxDepth--;
                foreach (var item in descendants.SelectMany(item => item.GetBranchesInDepthRange(minDepth, maxDepth, predicate)))
                    yield return item;
            }
            else
            {
                maxDepth -= minDepth;
                var attributes = parent.TryGetAttributes();
                if (attributes is not null)
                    yield return attributes;
                foreach (var item in descendants)
                {
                    if (predicate(item))
                        yield return item;
                    else
                        foreach (var obj in item.GetBranchesIncludingAttributesToDepth(maxDepth, predicate))
                            yield return obj;
                }
            }
        }
        else if (minDepth == 1)
        {
            var attributes = parent.TryGetAttributes();
            if (attributes is not null)
                yield return attributes;
        }
    }
}