using System.Diagnostics;
using Markdig.Renderers.Html;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace HtmlUtility;

public static partial class MarkdownExtensionMethods
{
    /// <summary>
    /// Gets the nested descendants of a <see cref="ContainerBlock"/> within an inclusive depth range.
    /// </summary>
    /// <param name="parent">The parent <see cref="ContainerBlock"/>.</param>
    /// <param name="minDepth">The minimum depth of the recursive nested descendants to return. This must not be less than 1.</param>
    /// <param name="maxDepth">The maximum depth of the recursive descendants to return. This must greater than <paramref name="minDepth"/>.</param>
    /// <returns>Nested desendants not less than <paramref name="minDepth"/> and not greater than <paramref name="maxDepth"/> levels deep, not including <see cref="HtmlAttributes"/>.</returns>
    internal static IEnumerable<MarkdownObject> GetDescendantsInDepthRange(this ContainerBlock parent, int minDepth, int maxDepth)
    {
        Debug.Assert(parent is not null);
        Debug.Assert(minDepth > 1);
        if (parent.HasDirectDescendant(out IEnumerable<MarkdownObject>? descendants))
        {
            maxDepth -= minDepth;
            Debug.Assert(maxDepth > 0);
            if (minDepth == 1)
                foreach (var item in descendants)
                {
                    yield return item;
                    if (item is ContainerBlock containerBlock)
                        foreach (var obj in containerBlock.GetDescendantsToDepth(maxDepth))
                            yield return obj;
                    else if (item is LeafBlock leafBlock)
                        foreach (var obj in leafBlock.GetDescendantsToDepth(maxDepth))
                            yield return obj;
                }
            else
            {
                Debug.Assert(maxDepth > minDepth);
                minDepth--;
                maxDepth--;
                foreach (var item in descendants)
                {
                    if (item is ContainerBlock containerBlock)
                        foreach (var obj in containerBlock.GetDescendantsInDepthRange(minDepth, maxDepth))
                            yield return obj;
                    else if (item is LeafBlock leafBlock)
                        foreach (var obj in leafBlock.GetDescendantsInDepthRange(minDepth, maxDepth))
                            yield return obj;
                }
            }
        }
    }

    /// <summary>
    /// Gets the nested descendants of a <see cref="ContainerInline"/> within an inclusive depth range.
    /// </summary>
    /// <param name="parent">The parent <see cref="ContainerInline"/>.</param>
    /// <param name="minDepth">The minimum depth of the recursive nested descendants to return. This must not be less than 1.</param>
    /// <param name="maxDepth">The maximum depth of the recursive descendants to return. This must greater than <paramref name="minDepth"/>.</param>
    /// <returns>Nested desendants not less than <paramref name="minDepth"/> and not greater than <paramref name="maxDepth"/> levels deep, not including <see cref="HtmlAttributes"/>.</returns>
    internal static IEnumerable<MarkdownObject> GetDescendantsInDepthRange(this ContainerInline parent, int minDepth, int maxDepth)
    {
        Debug.Assert(parent is not null);
        Debug.Assert(minDepth > 1);
        maxDepth -= minDepth;
        if (parent.HasDirectDescendant(out IEnumerable<MarkdownObject>? descendants))
        {
            Debug.Assert(maxDepth > 0);
            if (minDepth == 1)
                foreach (var item in descendants)
                {
                    yield return item;
                    if (item is ContainerInline containerInline)
                        foreach (var obj in containerInline.GetDescendantsToDepth(maxDepth))
                            yield return obj;
                }
            else
            {
                Debug.Assert(maxDepth > minDepth);
                minDepth--;
                maxDepth--;
                foreach (var item in descendants)
                {
                    if (item is ContainerInline containerInline)
                        foreach (var obj in containerInline.GetDescendantsInDepthRange(minDepth, maxDepth))
                            yield return obj;
                }
            }
        }
    }

    /// <summary>
    /// Gets the nested descendants of a <see cref="LeafBlock"/> within an inclusive depth range.
    /// </summary>
    /// <param name="parent">The parent <see cref="LeafBlock"/>.</param>
    /// <param name="minDepth">The minimum depth of the recursive nested descendants to return. This must not be less than 1.</param>
    /// <param name="maxDepth">The maximum depth of the recursive descendants to return. This must greater than <paramref name="minDepth"/>.</param>
    /// <returns>Nested desendants not less than <paramref name="minDepth"/> and not greater than <paramref name="maxDepth"/> levels deep, not including <see cref="HtmlAttributes"/>.</returns>
    internal static IEnumerable<MarkdownObject> GetDescendantsInDepthRange(this LeafBlock parent, int minDepth, int maxDepth)
    {
        Debug.Assert(parent is not null);
        return (parent.Inline is null) ? [] : parent.Inline.GetDescendantsInDepthRange(minDepth, maxDepth);
    }

    /// <summary>
    /// Gets the nested descendants and attributes of a <see cref="ContainerBlock"/> within an inclusive depth range.
    /// </summary>
    /// <param name="parent">The parent <see cref="ContainerBlock"/>.</param>
    /// <param name="minDepth">The minimum depth of the recursive nested descendants to return. This must not be less than 1.</param>
    /// <param name="maxDepth">The maximum depth of the recursive descendants to return. This must greater than <paramref name="minDepth"/>.</param>
    /// <returns>Nested desendants not less than <paramref name="minDepth"/> and not greater than <paramref name="maxDepth"/> levels deep, including <see cref="HtmlAttributes"/>.</returns>
    internal static IEnumerable<MarkdownObject> GetDescendantsAndAttributesInDepthRange(this ContainerBlock parent, int minDepth, int maxDepth)
    {
        Debug.Assert(parent is not null);
        Debug.Assert(minDepth > 1);
        if (parent.HasDirectDescendant(out IEnumerable<MarkdownObject>? descendants))
        {
            maxDepth -= minDepth;
            Debug.Assert(maxDepth > 0);
            if (minDepth == 1)
            {
                var attribute = parent.TryGetAttributes();
                if (attribute is not null)
                    yield return attribute;
                foreach (var item in descendants)
                {
                    yield return item;
                    if (item is ContainerBlock containerBlock)
                        foreach (var obj in containerBlock.GetDescendantsAndAttributesToDepth(maxDepth))
                            yield return obj;
                    else if (item is LeafBlock leafBlock)
                        foreach (var obj in leafBlock.GetDescendantsAndAttributesToDepth(maxDepth))
                            yield return obj;
                    else if ((attribute = item.TryGetAttributes()) is not null)
                        yield return attribute;
                }

            }
            else
            {
                Debug.Assert(maxDepth > minDepth);
                minDepth--;
                maxDepth--;
                if (minDepth == 1)
                    foreach (var item in descendants)
                    {
                        if (item is ContainerBlock containerBlock)
                            foreach (var obj in containerBlock.GetDescendantsAndAttributesInDepthRange(minDepth, maxDepth))
                                yield return obj;
                        else if (item is LeafBlock leafBlock)
                            foreach (var obj in leafBlock.GetDescendantsAndAttributesInDepthRange(minDepth, maxDepth))
                                yield return obj;
                        else
                        {
                            var attribute = item.TryGetAttributes();
                            if (attribute is not null)
                                yield return attribute;
                        }
                    }
                else
                    foreach (var item in descendants)
                    {
                        if (item is ContainerBlock containerBlock)
                            foreach (var obj in containerBlock.GetDescendantsAndAttributesInDepthRange(minDepth, maxDepth))
                                yield return obj;
                        else if (item is LeafBlock leafBlock)
                            foreach (var obj in leafBlock.GetDescendantsAndAttributesInDepthRange(minDepth, maxDepth))
                                yield return obj;
                    }
            }
        }
        else if (minDepth == 1)
        {
            var attribute = parent.TryGetAttributes();
            if (attribute is not null)
                yield return attribute;
        }
    }

    /// <summary>
    /// Gets the nested descendants and attributes of a <see cref="ContainerInline"/> within an inclusive depth range.
    /// </summary>
    /// <param name="parent">The parent <see cref="ContainerInline"/>.</param>
    /// <param name="minDepth">The minimum depth of the recursive nested descendants to return. This must not be less than 1.</param>
    /// <param name="maxDepth">The maximum depth of the recursive descendants to return. This must greater than <paramref name="minDepth"/>.</param>
    /// <returns>Nested desendants not less than <paramref name="minDepth"/> and not greater than <paramref name="maxDepth"/> levels deep, including <see cref="HtmlAttributes"/>.</returns>
    internal static IEnumerable<MarkdownObject> GetDescendantsAndAttributesInDepthRange(this ContainerInline parent, int minDepth, int maxDepth)
    {
        Debug.Assert(parent is not null);
        Debug.Assert(minDepth > 1);
        maxDepth -= minDepth;
        if (parent.HasDirectDescendant(out IEnumerable<MarkdownObject>? descendants))
        {
            Debug.Assert(maxDepth > 0);
            if (minDepth == 1)
            {
                var attribute = parent.TryGetAttributes();
                if (attribute is not null)
                    yield return attribute;
                foreach (var item in descendants)
                {
                    yield return item;
                    if (item is ContainerInline containerInline)
                        foreach (var obj in containerInline.GetDescendantsAndAttributesToDepth(maxDepth))
                            yield return obj;
                    else if ((attribute = item.TryGetAttributes()) is not null)
                        yield return attribute;
                }
            }
            else
            {
                Debug.Assert(maxDepth > minDepth);
                minDepth--;
                maxDepth--;
                if (minDepth == 1)
                    foreach (var item in descendants)
                    {
                        if (item is ContainerInline containerInline)
                            foreach (var obj in containerInline.GetDescendantsAndAttributesInDepthRange(minDepth, maxDepth))
                                yield return obj;
                        else
                        {
                            var attribute = item.TryGetAttributes();
                            if (attribute is not null)
                                yield return attribute;
                        }
                    }
                else
                    foreach (var item in descendants)
                    {
                        if (item is ContainerInline containerInline)
                            foreach (var obj in containerInline.GetDescendantsAndAttributesInDepthRange(minDepth, maxDepth))
                                yield return obj;
                    }
            }
        }
        else if (minDepth == 1)
        {
            var attribute = parent.TryGetAttributes();
            if (attribute is not null)
                yield return attribute;
        }
    }

    /// <summary>
    /// Gets the nested descendants and attributes of a <see cref="LeafBlock"/> within an inclusive depth range.
    /// </summary>
    /// <param name="parent">The parent <see cref="LeafBlock"/>.</param>
    /// <param name="minDepth">The minimum depth of the recursive nested descendants to return. This must not be less than 1.</param>
    /// <param name="maxDepth">The maximum depth of the recursive descendants to return. This must greater than <paramref name="minDepth"/>.</param>
    /// <returns>Nested desendants not less than <paramref name="minDepth"/> and not greater than <paramref name="maxDepth"/> levels deep, including <see cref="HtmlAttributes"/>.</returns>
    internal static IEnumerable<MarkdownObject> GetDescendantsAndAttributesInDepthRange(this LeafBlock parent, int minDepth, int maxDepth)
    {
        Debug.Assert(parent is not null);
        Debug.Assert(minDepth > 1);
        maxDepth -= minDepth;
        if (parent.Inline is not null && parent.Inline.HasDirectDescendant(out IEnumerable<MarkdownObject>? descendants))
        {
            Debug.Assert(maxDepth > 0);
            if (minDepth == 1)
            {
                var attribute = parent.TryGetAttributes();
                if (attribute is not null)
                    yield return attribute;
                foreach (var item in descendants)
                {
                    yield return item;
                    if (item is ContainerInline containerInline)
                        foreach (var obj in containerInline.GetDescendantsAndAttributesToDepth(maxDepth))
                            yield return obj;
                    else if ((attribute = item.TryGetAttributes()) is not null)
                        yield return attribute;
                }
            }
            else
            {
                Debug.Assert(maxDepth > minDepth);
                minDepth--;
                maxDepth--;
                if (minDepth == 1)
                    foreach (var item in descendants)
                    {
                        if (item is ContainerInline containerInline)
                            foreach (var obj in containerInline.GetDescendantsAndAttributesInDepthRange(minDepth, maxDepth))
                                yield return obj;
                        else
                        {
                            var attribute = item.TryGetAttributes();
                            if (attribute is not null)
                                yield return attribute;
                        }
                    }
                else
                    foreach (var item in descendants)
                    {
                        if (item is ContainerInline containerInline)
                            foreach (var obj in containerInline.GetDescendantsAndAttributesInDepthRange(minDepth, maxDepth))
                                yield return obj;
                    }
            }
        }
        else if (minDepth == 1)
        {
            var attribute = parent.TryGetAttributes();
            if (attribute is not null)
                yield return attribute;
        }
    }

    /// <summary>
    /// Gets specific nested descendants of a <see cref="ContainerBlock"/> within a specified range.
    /// </summary>
    /// <param name="parent">The parent <see cref="ContainerBlock"/>.</param>
    /// <param name="minDepth">The minimum depth of the recursive nested descendants to return. This must not be less than 1.</param>
    /// <param name="maxDepth">The maximum depth of the recursive descendants to return. This must greater than <paramref name="minDepth"/>.</param>
    /// <param name="predicate">Function that specifies which descendant object to return.</param>
    /// <returns></returns>
    /// <remarks>No descendants of yeilded items will be returned.</remarks>
    internal static IEnumerable<MarkdownObject> GetBranchesInDepthRange(this ContainerBlock parent, int minDepth, int maxDepth, Func<MarkdownObject, bool> predicate)
    {
        Debug.Assert(parent is not null);
        Debug.Assert(predicate is not null);
        Debug.Assert(minDepth > 1);
        if (parent.HasDirectDescendant(out IEnumerable<MarkdownObject>? descendants))
        {
            maxDepth -= minDepth;
            Debug.Assert(maxDepth > 0);
            if (minDepth == 1)
                foreach (var item in descendants)
                {
                    if (predicate(item))
                        yield return item;
                    else if (item is ContainerBlock containerBlock)
                        foreach (var obj in containerBlock.GetBranchesToDepth(maxDepth, predicate))
                            yield return obj;
                    else if (item is LeafBlock leafBlock)
                        foreach (var obj in leafBlock.GetBranchesToDepth(maxDepth, predicate))
                            yield return obj;
                }
            else
            {
                Debug.Assert(maxDepth > minDepth);
                minDepth--;
                maxDepth--;
                foreach (var item in descendants)
                {
                    if (item is ContainerBlock containerBlock)
                        foreach (var obj in containerBlock.GetBranchesInDepthRange(minDepth, maxDepth, predicate))
                            yield return obj;
                    else if (item is LeafBlock leafBlock)
                        foreach (var obj in leafBlock.GetBranchesInDepthRange(minDepth, maxDepth, predicate))
                            yield return obj;
                }
            }
        }
    }

    /// <summary>
    /// Gets specific nested descendants of a <see cref="ContainerInline"/> within a specified range.
    /// </summary>
    /// <param name="parent">The parent <see cref="ContainerInline"/>.</param>
    /// <param name="minDepth">The minimum depth of the recursive nested descendants to return. This must not be less than 1.</param>
    /// <param name="maxDepth">The maximum depth of the recursive descendants to return. This must greater than <paramref name="minDepth"/>.</param>
    /// <param name="predicate">Function that specifies which descendant object to return.</param>
    /// <returns>Nested desendants not less than <paramref name="minDepth"/> and not greater than <paramref name="maxDepth"/> levels deep where <paramref name="predicate"/> returns true,
    /// not including <see cref="HtmlAttributes"/>.</returns>
    /// <remarks>No descendants of yeilded items will be returned.</remarks>
    internal static IEnumerable<MarkdownObject> GetBranchesInDepthRange(this ContainerInline parent, int minDepth, int maxDepth, Func<MarkdownObject, bool> predicate)
    {
        Debug.Assert(parent is not null);
        Debug.Assert(predicate is not null);
        Debug.Assert(minDepth > 1);
        maxDepth -= minDepth;
        if (parent.HasDirectDescendant(out IEnumerable<MarkdownObject>? descendants))
        {
            Debug.Assert(maxDepth > 0);
            if (minDepth == 1)
                foreach (var item in descendants)
                {
                    if (predicate(item))
                        yield return item;
                    else if (item is ContainerInline containerInline)
                        foreach (var obj in containerInline.GetBranchesToDepth(maxDepth, predicate))
                            yield return obj;
                }
            else
            {
                Debug.Assert(maxDepth > minDepth);
                minDepth--;
                maxDepth--;
                foreach (var item in descendants)
                {
                    if (item is ContainerInline containerInline)
                        foreach (var obj in containerInline.GetBranchesInDepthRange(minDepth, maxDepth, predicate))
                            yield return obj;
                }
            }
        }
    }

    /// <summary>
    /// Gets specific nested descendants of a <see cref="LeafBlock"/> within a specified range.
    /// </summary>
    /// <param name="parent">The parent <see cref="LeafBlock"/>.</param>
    /// <param name="minDepth">The minimum depth of the recursive nested descendants to return. This must not be less than 1.</param>
    /// <param name="maxDepth">The maximum depth of the recursive descendants to return. This must greater than <paramref name="minDepth"/>.</param>
    /// <param name="predicate">Function that specifies which descendant object to return.</param>
    /// <returns>Nested desendants not less than <paramref name="minDepth"/> and not greater than <paramref name="maxDepth"/> levels deep where <paramref name="predicate"/> returns true,
    /// not including <see cref="HtmlAttributes"/>.</returns>
    /// <remarks>No descendants of yeilded items will be returned.</remarks>
    internal static IEnumerable<MarkdownObject> GetBranchesInDepthRange(this LeafBlock parent, int minDepth, int maxDepth, Func<MarkdownObject, bool> predicate)
    {
        Debug.Assert(parent is not null);
        Debug.Assert(predicate is not null);
        return (parent.Inline is null) ? [] : parent.Inline.GetBranchesInDepthRange(minDepth, maxDepth, predicate);
    }

    /// <summary>
    /// Gets specific nested descendants and attributes of a <see cref="ContainerBlock"/> within a specified range.
    /// </summary>
    /// <param name="parent">The parent <see cref="ContainerBlock"/>.</param>
    /// <param name="minDepth">The minimum depth of the recursive nested descendants to return. This must not be less than 1.</param>
    /// <param name="maxDepth">The maximum depth of the recursive descendants to return. This must greater than <paramref name="minDepth"/>.</param>
    /// <param name="predicate">Function that specifies which descendant object to return.</param>
    /// <returns>Nested desendants not less than <paramref name="minDepth"/> and not greater than <paramref name="maxDepth"/> levels deep where <paramref name="predicate"/> returns true,
    /// including <see cref="HtmlAttributes"/>.</returns>
    /// <remarks>No attributes or descendants of yeilded items will be returned.</remarks>
    internal static IEnumerable<MarkdownObject> GetBranchesIncludingAttributesInDepthRange(this ContainerBlock parent, int minDepth, int maxDepth, Func<MarkdownObject, bool> predicate)
    {
        Debug.Assert(parent is not null);
        Debug.Assert(predicate is not null);
        Debug.Assert(minDepth > 1);
        if (parent.HasDirectDescendant(out IEnumerable<MarkdownObject>? descendants))
        {
            maxDepth -= minDepth;
            Debug.Assert(maxDepth > 0);
            if (minDepth == 1)
            {
                var attribute = parent.TryGetAttributes();
                if (attribute is not null && predicate(attribute))
                    yield return attribute;
                foreach (var item in descendants)
                {
                    if (predicate(item))
                        yield return item;
                    else if (item is ContainerBlock containerBlock)
                        foreach (var obj in containerBlock.GetBranchesIncludingAttributesToDepth(maxDepth, predicate))
                            yield return obj;
                    else if (item is LeafBlock leafBlock)
                        foreach (var obj in leafBlock.GetBranchesIncludingAttributesToDepth(maxDepth, predicate))
                            yield return obj;
                    else if ((attribute = item.TryGetAttributes()) is not null && predicate(attribute))
                        yield return attribute;
                }

            }
            else
            {
                Debug.Assert(maxDepth > minDepth);
                minDepth--;
                maxDepth--;
                if (minDepth == 1)
                    foreach (var item in descendants)
                    {
                        if (item is ContainerBlock containerBlock)
                            foreach (var obj in containerBlock.GetBranchesIncludingAttributesInDepthRange(minDepth, maxDepth, predicate))
                                yield return obj;
                        else if (item is LeafBlock leafBlock)
                            foreach (var obj in leafBlock.GetBranchesIncludingAttributesInDepthRange(minDepth, maxDepth, predicate))
                                yield return obj;
                        else
                        {
                            var attribute = item.TryGetAttributes();
                            if (attribute is not null && predicate(attribute))
                                yield return attribute;
                        }
                    }
                else
                    foreach (var item in descendants)
                    {
                        if (item is ContainerBlock containerBlock)
                            foreach (var obj in containerBlock.GetBranchesIncludingAttributesInDepthRange(minDepth, maxDepth, predicate))
                                yield return obj;
                        else if (item is LeafBlock leafBlock)
                            foreach (var obj in leafBlock.GetBranchesIncludingAttributesInDepthRange(minDepth, maxDepth, predicate))
                                yield return obj;
                    }
            }
        }
        else if (minDepth == 1)
        {
            var attribute = parent.TryGetAttributes();
            if (attribute is not null && predicate(attribute))
                yield return attribute;
        }
    }

    /// <summary>
    /// Gets specific nested descendants and attributes of a <see cref="ContainerInline"/> within a specified range.
    /// </summary>
    /// <param name="parent">The parent <see cref="ContainerInline"/>.</param>
    /// <param name="minDepth">The minimum depth of the recursive nested descendants to return. This must not be less than 1.</param>
    /// <param name="maxDepth">The maximum depth of the recursive descendants to return. This must greater than <paramref name="minDepth"/>.</param>
    /// <param name="predicate">Function that specifies which descendant object to return.</param>
    /// <returns>Nested desendants not less than <paramref name="minDepth"/> and not greater than <paramref name="maxDepth"/> levels deep where <paramref name="predicate"/> returns true,
    /// including <see cref="HtmlAttributes"/>.</returns>
    /// <remarks>No attributes or descendants of yeilded items will be returned.</remarks>
    internal static IEnumerable<MarkdownObject> GetBranchesIncludingAttributesInDepthRange(this ContainerInline parent, int minDepth, int maxDepth, Func<MarkdownObject, bool> predicate)
    {
        Debug.Assert(parent is not null);
        Debug.Assert(predicate is not null);
        Debug.Assert(minDepth > 1);
        maxDepth -= minDepth;
        if (parent.HasDirectDescendant(out IEnumerable<MarkdownObject>? descendants))
        {
            Debug.Assert(maxDepth > 0);
            if (minDepth == 1)
            {
                var attribute = parent.TryGetAttributes();
                if (attribute is not null && predicate(attribute))
                    yield return attribute;
                foreach (var item in descendants)
                {
                    if (predicate(item))
                        yield return item;
                    else if (item is ContainerInline containerInline)
                        foreach (var obj in containerInline.GetBranchesIncludingAttributesToDepth(maxDepth, predicate))
                            yield return obj;
                    else if ((attribute = item.TryGetAttributes()) is not null && predicate(attribute))
                        yield return attribute;
                }
            }
            else
            {
                Debug.Assert(maxDepth > minDepth);
                minDepth--;
                maxDepth--;
                if (minDepth == 1)
                    foreach (var item in descendants)
                    {
                        if (item is ContainerInline containerInline)
                            foreach (var obj in containerInline.GetBranchesIncludingAttributesInDepthRange(minDepth, maxDepth, predicate))
                                yield return obj;
                        else
                        {
                            var attribute = item.TryGetAttributes();
                            if (attribute is not null && predicate(attribute))
                                yield return attribute;
                        }
                    }
                else
                    foreach (var item in descendants)
                    {
                        if (item is ContainerInline containerInline)
                            foreach (var obj in containerInline.GetBranchesIncludingAttributesInDepthRange(minDepth, maxDepth, predicate))
                                yield return obj;
                    }
            }
        }
        else if (minDepth == 1)
        {
            var attribute = parent.TryGetAttributes();
            if (attribute is not null && predicate(attribute))
                yield return attribute;
        }
    }

    /// <summary>
    /// Gets specific nested descendants and attributes of a <see cref="LeafBlock"/> within a specified range.
    /// </summary>
    /// <param name="parent">The parent <see cref="LeafBlock"/>.</param>
    /// <param name="minDepth">The minimum depth of the recursive nested descendants to return. This must not be less than 1.</param>
    /// <param name="maxDepth">The maximum depth of the recursive descendants to return. This must greater than <paramref name="minDepth"/>.</param>
    /// <param name="predicate">Function that specifies which descendant object to return.</param>
    /// <returns>Nested desendants not less than <paramref name="minDepth"/> and not greater than <paramref name="maxDepth"/> levels deep where <paramref name="predicate"/> returns true,
    /// including <see cref="HtmlAttributes"/>.</returns>
    /// <remarks>No attributes or descendants of yeilded items will be returned.</remarks>
    internal static IEnumerable<MarkdownObject> GetBranchesIncludingAttributesInDepthRange(this LeafBlock parent, int minDepth, int maxDepth, Func<MarkdownObject, bool> predicate)
    {
        Debug.Assert(parent is not null);
        Debug.Assert(predicate is not null);
        Debug.Assert(minDepth > 1);
        maxDepth -= minDepth;
        if (parent.Inline is not null && parent.Inline.HasDirectDescendant(out IEnumerable<MarkdownObject>? descendants))
        {
            Debug.Assert(maxDepth > 0);
            if (minDepth == 1)
            {
                var attribute = parent.TryGetAttributes();
                if (attribute is not null && predicate(attribute))
                    yield return attribute;
                foreach (var item in descendants)
                {
                    if (predicate(item))
                        yield return item;
                    else if (item is ContainerInline containerInline)
                        foreach (var obj in containerInline.GetBranchesIncludingAttributesToDepth(maxDepth, predicate))
                            yield return obj;
                    else if ((attribute = item.TryGetAttributes()) is not null && predicate(attribute))
                        yield return attribute;
                }
            }
            else
            {
                Debug.Assert(maxDepth > minDepth);
                minDepth--;
                maxDepth--;
                if (minDepth == 1)
                    foreach (var item in descendants)
                    {
                        if (item is ContainerInline containerInline)
                            foreach (var obj in containerInline.GetBranchesIncludingAttributesInDepthRange(minDepth, maxDepth, predicate))
                                yield return obj;
                        else
                        {
                            var attribute = item.TryGetAttributes();
                            if (attribute is not null && predicate(attribute))
                                yield return attribute;
                        }
                    }
                else
                    foreach (var item in descendants)
                    {
                        if (item is ContainerInline containerInline)
                            foreach (var obj in containerInline.GetBranchesIncludingAttributesInDepthRange(minDepth, maxDepth, predicate))
                                yield return obj;
                    }
            }
        }
        else if (minDepth == 1)
        {
            var attribute = parent.TryGetAttributes();
            if (attribute is not null && predicate(attribute))
                yield return attribute;
        }
    }
}