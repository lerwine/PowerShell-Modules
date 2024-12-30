using System.Diagnostics;
using Markdig.Renderers.Html;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace HtmlUtility;

public static partial class MarkdownExtensionMethods
{
    /// <summary>
    /// Gets the nested descendants of a <see cref="ContainerBlock"/> up to a specified depth.
    /// <param name="parent">The parent <see cref="ContainerBlock"/>.</param>
    /// <param name="maxDepth">The maximum depth of the recursive descendants to return. This must not be less than 1.</param>
    /// <returns>Nested desendants at or less than <paramref name="maxDepth"/> levels deep, not including <see cref="HtmlAttributes"/>.</returns>
    internal static IEnumerable<MarkdownObject> GetDescendantsToDepth(this ContainerBlock parent, int maxDepth)
    {
        Debug.Assert(parent is not null);
        Debug.Assert(maxDepth > 0);
        if (parent.HasDirectDescendant(out IEnumerable<MarkdownObject>? descendants))
        {
            if (maxDepth == 1)
                foreach (var item in descendants)
                    yield return item;
            else
            {
                maxDepth--;
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
            }
        }
    }

    /// <summary>
    /// Gets the nested descendants of a <see cref="ContainerInline"/> up to a specified depth.
    /// </summary>
    /// <param name="parent">The parent <see cref="ContainerInline"/>.</param>
    /// <param name="maxDepth">The maximum depth of the recursive descendants to return. This must not be less than 1.</param>
    /// <returns>Nested desendants at or less than <paramref name="maxDepth"/> levels deep, not including <see cref="HtmlAttributes"/>.</returns>
    internal static IEnumerable<MarkdownObject> GetDescendantsToDepth(this ContainerInline parent, int maxDepth)
    {
        Debug.Assert(parent is not null);
        Debug.Assert(maxDepth > 0);
        if (parent.HasDirectDescendant(out IEnumerable<MarkdownObject>? descendants))
        {
            if (maxDepth == 1)
                foreach (var item in descendants)
                    yield return item;
            else
            {
                maxDepth--;
                foreach (var item in descendants)
                {
                    yield return item;
                    if (item is ContainerInline containerInline)
                        foreach (var obj in containerInline.GetDescendantsToDepth(maxDepth))
                            yield return obj;
                }
            }
        }
    }

    /// <summary>
    /// Gets the nested descendants of a <see cref="LeafBlock"/> up to a specified depth.
    /// </summary>
    /// <param name="parent">The parent <see cref="LeafBlock"/>.</param>
    /// <param name="maxDepth">The maximum depth of the recursive descendants to return. This must not be less than 1.</param>
    /// <returns>Nested desendants at or less than <paramref name="maxDepth"/> levels deep, not including <see cref="HtmlAttributes"/>.</returns>
    internal static IEnumerable<MarkdownObject> GetDescendantsToDepth(this LeafBlock parent, int maxDepth)
    {
        Debug.Assert(parent is not null);
        return (parent.Inline is null) ? [] : parent.Inline.GetDescendantsToDepth(maxDepth);
    }

    /// <summary>
    /// Gets the nested descendants and attributes of a <see cref="ContainerBlock"/> up to a specified depth.
    /// </summary>
    /// <param name="parent">The parent <see cref="ContainerBlock"/>.</param>
    /// <param name="maxDepth">The maximum depth of the recursive descendants to return. This must not be less than 1.</param>
    /// <returns>Nested desendants at or less than <paramref name="maxDepth"/> levels deep, including <see cref="HtmlAttributes"/>.</returns>
    internal static IEnumerable<MarkdownObject> GetDescendantsAndAttributesToDepth(this ContainerBlock parent, int maxDepth)
    {
        Debug.Assert(parent is not null);
        Debug.Assert(maxDepth > 0);
        var attribute = parent.TryGetAttributes();
        if (attribute is not null)
            yield return attribute;
        if (parent.HasDirectDescendant(out IEnumerable<MarkdownObject>? descendants))
        {
            if (maxDepth == 1)
                foreach (var item in descendants)
                    yield return item;
            else
            {
                maxDepth--;
                if (maxDepth == 1)
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
                else
                    foreach (var item in descendants)
                    {
                        yield return item;
                        if (item is ContainerBlock containerBlock)
                            foreach (var obj in containerBlock.GetDescendantsAndAttributesToDepth(maxDepth))
                                yield return obj;
                        else if (item is LeafBlock leafBlock)
                            foreach (var obj in leafBlock.GetDescendantsAndAttributesToDepth(maxDepth))
                                yield return obj;
                    }
            }
        }
    }

    /// <summary>
    /// Gets the nested descendants and attributes of a <see cref="ContainerInline"/> up to a specified depth.
    /// </summary>
    /// <param name="parent">The parent <see cref="ContainerInline"/>.</param>
    /// <param name="maxDepth">The maximum depth of the recursive descendants to return. This must not be less than 1.</param>
    /// <returns>Nested desendants at or less than <paramref name="maxDepth"/> levels deep, including <see cref="HtmlAttributes"/>.</returns>
    internal static IEnumerable<MarkdownObject> GetDescendantsAndAttributesToDepth(this ContainerInline parent, int maxDepth)
    {
        Debug.Assert(parent is not null);
        Debug.Assert(maxDepth > 0);
        var attribute = parent.TryGetAttributes();
        if (attribute is not null)
            yield return attribute;
        if (parent.HasDirectDescendant(out IEnumerable<MarkdownObject>? descendants))
        {
            if (maxDepth == 1)
                foreach (var item in descendants)
                    yield return item;
            else
            {
                maxDepth--;
                if (maxDepth == 1)
                    foreach (var item in descendants)
                    {
                        yield return item;
                        if (item is ContainerInline containerInline)
                            foreach (var obj in containerInline.GetDescendantsAndAttributesToDepth(maxDepth))
                                yield return obj;
                        else if ((attribute = item.TryGetAttributes()) is not null)
                            yield return attribute;
                    }
                else
                    foreach (var item in descendants)
                    {
                        yield return item;
                        if (item is ContainerInline containerInline)
                            foreach (var obj in containerInline.GetDescendantsAndAttributesToDepth(maxDepth))
                                yield return obj;
                    }
            }
        }
    }

    /// <summary>
    /// Gets the nested descendants and attributes of a <see cref="LeafBlock"/> up to a specified depth.
    /// </summary>
    /// <param name="parent">The parent <see cref="MarkdownObject"/>.</param>
    /// <param name="maxDepth">The maximum depth of the recursive descendants to return. This must not be less than 1.</param>
    /// <returns>Nested desendants at or less than <paramref name="maxDepth"/> levels deep, including <see cref="HtmlAttributes"/>.</returns>
    internal static IEnumerable<MarkdownObject> GetDescendantsAndAttributesToDepth(this LeafBlock parent, int maxDepth)
    {
        Debug.Assert(parent is not null);
        Debug.Assert(maxDepth > 0);
        var attribute = parent.TryGetAttributes();
        if (attribute is not null)
            yield return attribute;
        if (parent.Inline is not null && parent.Inline.HasDirectDescendant(out IEnumerable<MarkdownObject>? descendants))
        {
            if (maxDepth == 1)
                foreach (var item in descendants)
                    yield return item;
            else
            {
                maxDepth--;
                if (maxDepth == 1)
                    foreach (var item in descendants)
                    {
                        yield return item;
                        if (item is ContainerInline containerInline)
                            foreach (var obj in containerInline.GetDescendantsAndAttributesToDepth(maxDepth))
                                yield return obj;
                        else if ((attribute = item.TryGetAttributes()) is not null)
                            yield return attribute;
                    }
                else
                    foreach (var item in descendants)
                    {
                        yield return item;
                        if (item is ContainerInline containerInline)
                            foreach (var obj in containerInline.GetDescendantsAndAttributesToDepth(maxDepth))
                                yield return obj;
                    }
            }
        }
    }

    /// <summary>
    /// Gets specific nested descendants of a <see cref="ContainerBlock"/> starting up to a specified depth.
    /// </summary>
    /// <param name="parent">The parent <see cref="ContainerBlock"/>.</param>
    /// <param name="maxDepth">The maximum depth of the recursive descendants to return. This must not be less than 1.</param>
    /// <param name="predicate">Function that specifies which descendant object to return.</param>
    /// <returns>Nested desendants at or less than <paramref name="maxDepth"/> levels deep where <paramref name="predicate"/> returns true, not including <see cref="HtmlAttributes"/>.</returns>
    /// <remarks>No descendants of yeilded items will be returned.</remarks>
    internal static IEnumerable<MarkdownObject> GetBranchesToDepth(this ContainerBlock parent, int maxDepth, Func<MarkdownObject, bool> predicate)
    {
        Debug.Assert(parent is not null);
        Debug.Assert(predicate is not null);
        Debug.Assert(maxDepth > 0);
        if (parent.HasDirectDescendant(out IEnumerable<MarkdownObject>? descendants))
        {
            if (maxDepth == 1)
                foreach (var item in descendants.Where(predicate))
                    yield return item;
            else
            {
                maxDepth--;
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
            }
        }
    }

    /// <summary>
    /// Gets specific nested descendants of a <see cref="ContainerInline"/> starting up to a specified depth.
    /// </summary>
    /// <param name="parent">The parent <see cref="ContainerInline"/>.</param>
    /// <param name="maxDepth">The maximum depth of the recursive descendants to return. This must not be less than 1.</param>
    /// <param name="predicate">Function that specifies which descendant object to return.</param>
    /// <returns>Nested desendants at or less than <paramref name="maxDepth"/> levels deep where <paramref name="predicate"/> returns true, not including <see cref="HtmlAttributes"/>.</returns>
    /// <remarks>No descendants of yeilded items will be returned.</remarks>
    internal static IEnumerable<MarkdownObject> GetBranchesToDepth(this ContainerInline parent, int maxDepth, Func<MarkdownObject, bool> predicate)
    {
        Debug.Assert(parent is not null);
        Debug.Assert(predicate is not null);
        Debug.Assert(maxDepth > 0);
        if (parent.HasDirectDescendant(out IEnumerable<MarkdownObject>? descendants))
        {
            if (maxDepth == 1)
                foreach (var item in descendants.Where(predicate))
                    yield return item;
            else
            {
                maxDepth--;
                foreach (var item in descendants)
                {
                    yield return item;
                    if (item is ContainerInline containerInline)
                        foreach (var obj in containerInline.GetBranchesToDepth(maxDepth, predicate))
                            yield return obj;
                }
            }
        }
    }

    /// <summary>
    /// Gets specific nested descendants of a <see cref="LeafBlock"/> starting up to a specified depth.
    /// </summary>
    /// <param name="parent">The parent <see cref="LeafBlock"/>.</param>
    /// <param name="maxDepth">The maximum depth of the recursive descendants to return. This must not be less than 1.</param>
    /// <param name="predicate">Function that specifies which descendant object to return.</param>
    /// <returns>Nested desendants at or less than <paramref name="maxDepth"/> levels deep where <paramref name="predicate"/> returns true, not including <see cref="HtmlAttributes"/>.</returns>
    /// <remarks>No descendants of yeilded items will be returned.</remarks>
    internal static IEnumerable<MarkdownObject> GetBranchesToDepth(this LeafBlock parent, int maxDepth, Func<MarkdownObject, bool> predicate)
    {
        Debug.Assert(parent is not null);
        Debug.Assert(predicate is not null);
        return (parent.Inline is null) ? [] : parent.Inline.GetBranchesToDepth(maxDepth, predicate);
    }

    /// <summary>
    /// Gets specific nested descendants and attributes of a <see cref="ContainerBlock"/> starting up to a specified depth.
    /// </summary>
    /// <param name="parent">The parent <see cref="ContainerBlock"/>.</param>
    /// <param name="maxDepth">The maximum depth of the recursive descendants to return. This must not be less than 1.</param>
    /// <param name="predicate">Function that specifies which descendant object to return.</param>
    /// <returns>Nested desendants at or less than <paramref name="maxDepth"/> levels deep where <paramref name="predicate"/> returns true, including <see cref="HtmlAttributes"/>.</returns>
    /// <remarks>No attributes or descendants of yeilded items will be returned.
    /// <para>The <see cref="HtmlAttributes"/> of <paramref name="parent"/> will always be returned, if present.</para></remarks>
    internal static IEnumerable<MarkdownObject> GetBranchesIncludingAttributesToDepth(this ContainerBlock parent, int maxDepth, Func<MarkdownObject, bool> predicate)
    {
        Debug.Assert(parent is not null);
        Debug.Assert(predicate is not null);
        Debug.Assert(maxDepth > 0);
        var attribute = parent.TryGetAttributes();
        if (attribute is not null && predicate(attribute))
            yield return attribute;
        if (parent.HasDirectDescendant(out IEnumerable<MarkdownObject>? descendants))
        {
            if (maxDepth == 1)
            {
                foreach (var item in descendants)
                    if (predicate(item))
                        yield return item;
            }
            else
            {
                maxDepth--;
                if (maxDepth == 1)
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
                else
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
        }
    }

    /// <summary>
    /// Gets specific nested descendants and attributes of a <see cref="ContainerInline"/> starting up to a specified depth.
    /// </summary>
    /// <param name="parent">The parent <see cref="ContainerInline"/>.</param>
    /// <param name="maxDepth">The maximum depth of the recursive descendants to return. This must not be less than 1.</param>
    /// <param name="predicate">Function that specifies which descendant object to return.</param>
    /// <returns>Nested desendants at or less than <paramref name="maxDepth"/> levels deep where <paramref name="predicate"/> returns true, including <see cref="HtmlAttributes"/>.</returns>
    /// <remarks>No attributes or descendants of yeilded items will be returned.
    /// <para>The <see cref="HtmlAttributes"/> of <paramref name="parent"/> will always be returned, if present.</para></remarks>
    internal static IEnumerable<MarkdownObject> GetBranchesIncludingAttributesToDepth(this ContainerInline parent, int maxDepth, Func<MarkdownObject, bool> predicate)
    {
        Debug.Assert(parent is not null);
        Debug.Assert(predicate is not null);
        Debug.Assert(maxDepth > 0);
        var attribute = parent.TryGetAttributes();
        if (attribute is not null && predicate(attribute))
            yield return attribute;
        if (parent.HasDirectDescendant(out IEnumerable<MarkdownObject>? descendants))
        {
            if (maxDepth == 1)
            {
                foreach (var item in descendants)
                    if (predicate(item))
                        yield return item;
            }
            else
            {
                maxDepth--;
                if (maxDepth == 1)
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
                else
                    foreach (var item in descendants)
                    {
                        if (predicate(item))
                            yield return item;
                        else if (item is ContainerInline containerInline)
                            foreach (var obj in containerInline.GetBranchesIncludingAttributesToDepth(maxDepth, predicate))
                                yield return obj;
                    }
            }
        }
    }

    /// <summary>
    /// Gets specific nested descendants and attributes of a <see cref="LeafBlock"/> starting up to a specified depth.
    /// </summary>
    /// <param name="parent">The parent <see cref="LeafBlock"/>.</param>
    /// <param name="maxDepth">The maximum depth of the recursive descendants to return. This must not be less than 1.</param>
    /// <param name="predicate">Function that specifies which descendant object to return.</param>
    /// <returns>Nested desendants at or less than <paramref name="maxDepth"/> levels deep where <paramref name="predicate"/> returns true, including <see cref="HtmlAttributes"/>.</returns>
    /// <remarks>No attributes or descendants of yeilded items will be returned.
    /// <para>The <see cref="HtmlAttributes"/> of <paramref name="parent"/> will always be returned, if present.</para></remarks>
    internal static IEnumerable<MarkdownObject> GetBranchesIncludingAttributesToDepth(this LeafBlock parent, int maxDepth, Func<MarkdownObject, bool> predicate)
    {
        Debug.Assert(parent is not null);
        Debug.Assert(predicate is not null);
        Debug.Assert(maxDepth > 0);
        var attribute = parent.TryGetAttributes();
        if (attribute is not null && predicate(attribute))
            yield return attribute;
        if (parent.Inline is not null && parent.Inline.HasDirectDescendant(out IEnumerable<MarkdownObject>? descendants))
        {
            if (maxDepth == 1)
            {
                foreach (var item in descendants)
                    if (predicate(item))
                        yield return item;
            }
            else
            {
                maxDepth--;
                if (maxDepth == 1)
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
                else
                    foreach (var item in descendants)
                    {
                        if (predicate(item))
                            yield return item;
                        else if (item is ContainerInline containerInline)
                            foreach (var obj in containerInline.GetBranchesIncludingAttributesToDepth(maxDepth, predicate))
                                yield return obj;
                    }
            }
        }
    }
}
