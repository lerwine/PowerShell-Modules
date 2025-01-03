using System.Diagnostics;
using Markdig.Renderers.Html;
using Markdig.Syntax;

namespace HtmlUtility;

public static partial class MarkdownExtensionMethods
{
    /// <summary>
    /// Gets the nested descendants of a <see cref="MarkdownObject"/> up to a specified depth.
    /// <param name="parent">The parent <see cref="MarkdownObject"/>.</param>
    /// <param name="maxDepth">The maximum depth of the recursive descendants to return. This must not be less than 1.</param>
    /// <returns>Nested desendants at or less than <paramref name="maxDepth"/> levels deep, not including <see cref="HtmlAttributes"/>.</returns>
    internal static IEnumerable<MarkdownObject> GetDescendantsToDepth(this MarkdownObject parent, int maxDepth)
    {
        Debug.Assert(parent is not null);
        Debug.Assert(maxDepth > 0);
        if (parent.HasDirectDescendant(out IEnumerable<MarkdownObject>? descendants))
        {
            if (maxDepth == 1) return descendants;
            maxDepth--;
            return descendants.SelectMany(item => item.GetDescendantsToDepthIncludingSelf(maxDepth));
        }
        return [];
    }

    internal static IEnumerable<MarkdownObject> GetDescendantsToDepthIncludingSelf(this MarkdownObject parent, int maxDepth)
    {
        Debug.Assert(parent is not null);
        Debug.Assert(maxDepth > 0);
        if (parent.HasDirectDescendant(out IEnumerable<MarkdownObject>? descendants))
        {
            if (maxDepth > 1)
            {
                maxDepth--;
                descendants = descendants.SelectMany(item => item.GetDescendantsToDepthIncludingSelf(maxDepth));
            }
            return parent.PrependToMarkdownObjects(descendants);
        }
        return [parent];
    }

    /// <summary>
    /// Gets the nested descendants and attributes of a <see cref="MarkdownObject"/> up to a specified depth.
    /// </summary>
    /// <param name="parent">The parent <see cref="MarkdownObject"/>.</param>
    /// <param name="maxDepth">The maximum depth of the recursive descendants to return. This must not be less than 1.</param>
    /// <returns>Nested desendants at or less than <paramref name="maxDepth"/> levels deep, including <see cref="HtmlAttributes"/>.</returns>
    internal static IEnumerable<MarkdownObject> GetDescendantsAndAttributesToDepth(this MarkdownObject parent, int maxDepth)
    {
        Debug.Assert(parent is not null);
        Debug.Assert(maxDepth > 0);
        var attributes = parent.TryGetAttributes();
        if (parent.HasDirectDescendant(out IEnumerable<MarkdownObject>? descendants))
        {
            if (maxDepth > 1)
            {
                maxDepth--;
                return attributes.PrependToMarkdownObjectsIfNotNull(descendants.SelectMany(item => item.GetDescendantsAndAttributesToDepthIncludingSelf(maxDepth)));
            }
            return attributes.PrependToMarkdownObjectsIfNotNull(descendants);
        }
        return attributes.EnumerateMarkdownObjectIfNotNull();
    }

    internal static IEnumerable<MarkdownObject> GetDescendantsAndAttributesToDepthIncludingSelf(this MarkdownObject parent, int maxDepth)
    {
        Debug.Assert(parent is not null);
        Debug.Assert(maxDepth > 0);
        var self = parent.EnumerateMarkdownObjects(parent.TryGetAttributes());
        if (parent.HasDirectDescendant(out IEnumerable<MarkdownObject>? descendants))
        {
            if (maxDepth > 1)
            {
                maxDepth--;
                return self.Concat(descendants.SelectMany(item => item.GetDescendantsAndAttributesToDepthIncludingSelf(maxDepth)));
            }
            return self.Concat(descendants);
        }
        return self;
    }

    /// <summary>
    /// Gets specific nested descendants of a <see cref="MarkdownObject"/> starting up to a specified depth.
    /// </summary>
    /// <param name="parent">The parent <see cref="MarkdownObject"/>.</param>
    /// <param name="maxDepth">The maximum depth of the recursive descendants to return. This must not be less than 1.</param>
    /// <param name="predicate">Function that specifies which descendant object to return.</param>
    /// <returns>Nested desendants at or less than <paramref name="maxDepth"/> levels deep where <paramref name="predicate"/> returns true, not including <see cref="HtmlAttributes"/>.</returns>
    /// <remarks>No descendants of yeilded items will be returned.</remarks>
    internal static IEnumerable<MarkdownObject> GetBranchesToDepth(this MarkdownObject parent, int maxDepth, Func<MarkdownObject, bool> predicate)
    {
        Debug.Assert(parent is not null);
        Debug.Assert(maxDepth > 0);
        Debug.Assert(predicate is not null);
        if (parent.HasDirectDescendant(out IEnumerable<MarkdownObject>? descendants))
        {
            if (maxDepth > 1)
            {
                maxDepth--;
                foreach (var item in descendants)
                {
                    if (predicate(item))
                        yield return item;
                    else
                        foreach (var obj in item.GetBranchesToDepth(maxDepth, predicate))
                            yield return obj;
                }
            }
            else
                foreach (var item in descendants.Where(predicate))
                    yield return item;
        }
    }

    /// <summary>
    /// Gets specific nested descendants and attributes of a <see cref="MarkdownObject"/> starting up to a specified depth.
    /// </summary>
    /// <param name="parent">The parent <see cref="MarkdownObject"/>.</param>
    /// <param name="maxDepth">The maximum depth of the recursive descendants to return. This must not be less than 1.</param>
    /// <param name="predicate">Function that specifies which descendant object to return.</param>
    /// <returns>Nested desendants at or less than <paramref name="maxDepth"/> levels deep where <paramref name="predicate"/> returns true,
    /// including <see cref="HtmlAttributes"/> of the <paramref name="parent"/> or where <paramref name="predicate"/> returns false.</returns>
    /// <remarks>No attributes or descendants of yeilded items will be returned.
    /// <para>The <see cref="HtmlAttributes"/> of <paramref name="parent"/> will always be returned, if present.</para></remarks>
    internal static IEnumerable<MarkdownObject> GetBranchesIncludingAttributesToDepth(this MarkdownObject parent, int maxDepth, Func<MarkdownObject, bool> predicate)
    {
        Debug.Assert(parent is not null);
        Debug.Assert(maxDepth > 0);
        Debug.Assert(predicate is not null);
        var attributes = parent.TryGetAttributes();
        if (attributes is not null)
            yield return attributes;
        if (parent.HasDirectDescendant(out IEnumerable<MarkdownObject>? descendants))
        {
            if (maxDepth > 1)
            {
                maxDepth--;
                foreach (var item in descendants)
                {
                    if (predicate(item))
                        yield return item;
                    else
                        foreach (var obj in item.GetBranchesIncludingAttributesToDepth(maxDepth, predicate))
                            yield return obj;
                }
            }
            else
                foreach (var item in descendants.Where(predicate))
                    yield return item;
        }
    }
}
