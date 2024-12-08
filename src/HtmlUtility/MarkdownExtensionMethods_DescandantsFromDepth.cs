using System.Diagnostics;
using Markdig.Renderers.Html;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace HtmlUtility;

public static partial class MarkdownExtensionMethods
{
    /// <summary>
    /// Gets the nested descendants of a <see cref="ContainerBlock"/> starting from a specified depth.
    /// </summary>
    /// <param name="parent">The parent <see cref="ContainerBlock"/>.</param>
    /// <param name="minDepth">The minimum depth of the recursive nested descendants to return. This must not be less than 1.</param>
    /// <returns>Nested desendants at or greater than <paramref name="minDepth"/> levels deep, not including <see cref="HtmlAttributes"/>.</returns>
    internal static IEnumerable<MarkdownObject> GetNestedDescendantsFromDepth(this ContainerBlock parent, int minDepth)
    {
        Debug.Assert(parent is not null);
        Debug.Assert(minDepth > 0);
        if (parent.HasDirectDescendants(out IEnumerable<MarkdownObject>? descendants))
        {
            if (minDepth == 1)
                foreach (var item in descendants)
                {
                    yield return item;
                    if (item is ContainerBlock containerBlock)
                        foreach (var obj in containerBlock.GetNestedDescendants())
                            yield return obj;
                    else if (item is LeafBlock leafBlock)
                        foreach (var obj in leafBlock.GetNestedDescendants())
                            yield return obj;
                }
            else
            {
                minDepth--;
                foreach (var item in descendants)
                {
                    if (item is ContainerBlock containerBlock)
                        foreach (var obj in containerBlock.GetNestedDescendantsFromDepth(minDepth))
                            yield return obj;
                    else if (item is LeafBlock leafBlock)
                        foreach (var obj in leafBlock.GetNestedDescendantsFromDepth(minDepth))
                            yield return obj;
                }
            }
        }
    }

    /// <summary>
    /// Gets the nested descendants of a <see cref="ContainerInline"/> starting from a specified depth.
    /// </summary>
    /// <param name="parent">The parent <see cref="ContainerInline"/>.</param>
    /// <param name="minDepth">The minimum depth of the recursive nested descendants to return. This must not be less than 1.</param>
    /// <returns>Nested desendants at or greater than <paramref name="minDepth"/> levels deep, not including <see cref="HtmlAttributes"/>.</returns>
    internal static IEnumerable<MarkdownObject> GetNestedDescendantsFromDepth(this ContainerInline parent, int minDepth)
    {
        Debug.Assert(parent is not null);
        Debug.Assert(minDepth > 0);
        if (parent.HasDirectDescendants(out IEnumerable<MarkdownObject>? descendants))
        {
            if (minDepth == 1)
                foreach (var item in descendants)
                {
                    yield return item;
                    if (item is ContainerInline containerInline)
                        foreach (var obj in containerInline.GetNestedDescendants())
                            yield return obj;
                }
            else
            {
                minDepth--;
                foreach (var item in descendants)
                {
                    if (item is ContainerInline containerInline)
                        foreach (var obj in containerInline.GetNestedDescendantsFromDepth(minDepth))
                            yield return obj;
                }
            }
        }
    }

    /// <summary>
    /// Gets the nested descendants of a <see cref="LeafBlock"/> starting from a specified depth.
    /// </summary>
    /// <param name="parent">The parent <see cref="LeafBlock"/>.</param>
    /// <param name="minDepth">The minimum depth of the recursive nested descendants to return. This must not be less than 1.</param>
    /// <returns>Nested desendants at or greater than <paramref name="minDepth"/> levels deep, not including <see cref="HtmlAttributes"/>.</returns>
    internal static IEnumerable<MarkdownObject> GetNestedDescendantsFromDepth(this LeafBlock parent, int minDepth)
    {
        Debug.Assert(parent is not null);
        return (parent.Inline is null) ? [] : parent.Inline.GetNestedDescendantsFromDepth(minDepth);
    }

    /// <summary>
    /// Gets the nested descendants and attributes of a <see cref="ContainerBlock"/> starting from a specified depth.
    /// </summary>
    /// <param name="parent">The parent <see cref="ContainerBlock"/>.</param>
    /// <param name="minDepth">The minimum depth of the recursive nested descendants to return. This must not be less than 1.</param>
    /// <returns>Nested desendants at or greater than <paramref name="minDepth"/> levels deep, including <see cref="HtmlAttributes"/>.</returns>
    internal static IEnumerable<MarkdownObject> GetNestedDescendantsAndAttributesFromDepth(this ContainerBlock parent, int minDepth)
    {
        Debug.Assert(parent is not null);
        Debug.Assert(minDepth > 0);
        if (parent.HasDirectDescendants(out IEnumerable<MarkdownObject>? descendants))
        {
            if (minDepth == 1)
            {
                var attribute = parent.TryGetAttributes();
                if (attribute is not null)
                    yield return attribute;
                foreach (var item in descendants)
                {
                    yield return item;
                    if (item is ContainerBlock containerBlock)
                        foreach (var obj in containerBlock.GetNestedDescendantsAndAttributes())
                            yield return obj;
                    else if (item is LeafBlock leafBlock)
                        foreach (var obj in leafBlock.GetNestedDescendantsAndAttributes())
                            yield return obj;
                    else if ((attribute = item.TryGetAttributes()) is not null)
                        yield return attribute;
                }
            }
            else
            {
                minDepth--;
                if (minDepth == 1)
                    foreach (var item in descendants)
                    {
                        if (item is ContainerBlock containerBlock)
                            foreach (var obj in containerBlock.GetNestedDescendantsAndAttributesFromDepth(minDepth))
                                yield return obj;
                        else if (item is LeafBlock leafBlock)
                            foreach (var obj in leafBlock.GetNestedDescendantsAndAttributesFromDepth(minDepth))
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
                            foreach (var obj in containerBlock.GetNestedDescendantsAndAttributesFromDepth(minDepth))
                                yield return obj;
                        else if (item is LeafBlock leafBlock)
                            foreach (var obj in leafBlock.GetNestedDescendantsAndAttributesFromDepth(minDepth))
                                yield return obj;
                    }
            }
        }
    }

    /// <summary>
    /// Gets the nested descendants and attributes of a <see cref="ContainerInline"/> starting from a specified depth.
    /// </summary>
    /// <param name="parent">The parent <see cref="ContainerInline"/>.</param>
    /// <param name="minDepth">The minimum depth of the recursive nested descendants to return. This must not be less than 1.</param>
    /// <returns>Nested desendants at or greater than <paramref name="minDepth"/> levels deep, including <see cref="HtmlAttributes"/>.</returns>
    internal static IEnumerable<MarkdownObject> GetNestedDescendantsAndAttributesFromDepth(this ContainerInline parent, int minDepth)
    {
        Debug.Assert(parent is not null);
        Debug.Assert(minDepth > 0);
        if (parent.HasDirectDescendants(out IEnumerable<MarkdownObject>? descendants))
        {
            if (minDepth == 1)
            {
                var attribute = parent.TryGetAttributes();
                if (attribute is not null)
                    yield return attribute;
                foreach (var item in descendants)
                {
                    yield return item;
                    if (item is ContainerInline containerInline)
                        foreach (var obj in containerInline.GetNestedDescendantsAndAttributes())
                            yield return obj;
                    else if ((attribute = item.TryGetAttributes()) is not null)
                        yield return attribute;
                }
            }
            else
            {
                minDepth--;
                if (minDepth == 1)
                    foreach (var item in descendants)
                    {
                        if (item is ContainerInline containerInline)
                            foreach (var obj in containerInline.GetNestedDescendantsAndAttributesFromDepth(minDepth))
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
                            foreach (var obj in containerInline.GetNestedDescendantsAndAttributesFromDepth(minDepth))
                                yield return obj;
                    }
            }
        }
    }

    /// <summary>
    /// Gets the nested descendants and attributes of a <see cref="LeafBlock"/> starting from a specified depth.
    /// </summary>
    /// <param name="parent">The parent <see cref="MarkdownObject"/>.</param>
    /// <param name="minDepth">The minimum depth of the recursive nested descendants to return. This must not be less than 1.</param>
    /// <returns>Nested desendants at or greater than <paramref name="minDepth"/> levels deep, including <see cref="HtmlAttributes"/>.</returns>
    internal static IEnumerable<MarkdownObject> GetNestedDescendantsAndAttributesFromDepth(this LeafBlock parent, int minDepth)
    {
        Debug.Assert(parent is not null);
        Debug.Assert(minDepth > 0);
        if (parent.Inline is null)
        {
            if (minDepth == 1)
            {
                var attribute = parent.TryGetAttributes();
                if (attribute is not null)
                    yield return attribute;
            }
        }
        else if (parent.Inline.HasDirectDescendants(out IEnumerable<MarkdownObject>? descendants))
        {
            if (minDepth == 1)
            {
                var attribute = parent.TryGetAttributes();
                if (attribute is not null)
                    yield return attribute;
                foreach (var item in descendants)
                {
                    yield return item;
                    if (item is ContainerInline containerInline)
                        foreach (var obj in containerInline.GetNestedDescendantsAndAttributes())
                            yield return obj;
                    else if ((attribute = item.TryGetAttributes()) is not null)
                        yield return attribute;
                }
            }
            else
            {
                minDepth--;
                if (minDepth == 1)
                    foreach (var item in descendants)
                    {
                        if (item is ContainerInline containerInline)
                            foreach (var obj in containerInline.GetNestedDescendantsAndAttributesFromDepth(minDepth))
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
                            foreach (var obj in containerInline.GetNestedDescendantsAndAttributesFromDepth(minDepth))
                                yield return obj;
                    }
            }
        }
    }

    /// <summary>
    /// Gets specific nested descendants of a <see cref="ContainerBlock"/> starting from a specified depth.
    /// </summary>
    /// <param name="parent">The parent <see cref="ContainerBlock"/>.</param>
    /// <param name="minDepth">The minimum depth of the recursive nested descendants to return. This must not be less than 1.</param>
    /// <param name="predicate">Function that specifies which descendant object to return.</param>
    /// <returns>Nested desendants at or greater than <paramref name="minDepth"/> levels deep where <paramref name="predicate"/> returns true, not including <see cref="HtmlAttributes"/>.</returns>
    /// <remarks>No descendants of yeilded items will be returned.</remarks>
    internal static IEnumerable<MarkdownObject> GetNestedDescendantsFromDepth(this ContainerBlock parent, int minDepth, Func<MarkdownObject, bool> predicate)
    {
        Debug.Assert(parent is not null);
        Debug.Assert(predicate is not null);
        Debug.Assert(minDepth > 0);
        if (parent.HasDirectDescendants(out IEnumerable<MarkdownObject>? descendants))
        {
            if (minDepth == 1)
                foreach (var item in descendants)
                {
                    if (predicate(item))
                        yield return item;
                    else if (item is ContainerBlock containerBlock)
                        foreach (var obj in containerBlock.GetNestedDescendants(predicate))
                            yield return obj;
                    else if (item is LeafBlock leafBlock)
                        foreach (var obj in leafBlock.GetNestedDescendants(predicate))
                            yield return obj;
                }
            else
            {
                minDepth--;
                foreach (var item in descendants)
                {
                    if (item is ContainerBlock containerBlock)
                        foreach (var obj in containerBlock.GetNestedDescendantsFromDepth(minDepth, predicate))
                            yield return obj;
                    else if (item is LeafBlock leafBlock)
                        foreach (var obj in leafBlock.GetNestedDescendantsFromDepth(minDepth, predicate))
                            yield return obj;
                }
            }
        }
    }

    /// <summary>
    /// Gets specific nested descendants of a <see cref="ContainerInline"/> starting from a specified depth.
    /// </summary>
    /// <param name="parent">The parent <see cref="ContainerInline"/>.</param>
    /// <param name="minDepth">The minimum depth of the recursive nested descendants to return. This must not be less than 1.</param>
    /// <param name="predicate">Function that specifies which descendant object to return.</param>
    /// <returns>Nested desendants at or greater than <paramref name="minDepth"/> levels deep where <paramref name="predicate"/> returns true, not including <see cref="HtmlAttributes"/>.</returns>
    /// <remarks>No descendants of yeilded items will be returned.</remarks>
    internal static IEnumerable<MarkdownObject> GetNestedDescendantsFromDepth(this ContainerInline parent, int minDepth, Func<MarkdownObject, bool> predicate)
    {
        Debug.Assert(parent is not null);
        Debug.Assert(predicate is not null);
        Debug.Assert(minDepth > 0);
        if (parent.HasDirectDescendants(out IEnumerable<MarkdownObject>? descendants))
        {
            if (minDepth == 1)
                foreach (var item in descendants)
                {
                    if (predicate(item))
                        yield return item;
                    else if (item is ContainerInline containerInline)
                        foreach (var obj in containerInline.GetNestedDescendants(predicate))
                            yield return obj;
                }
            else
            {
                minDepth--;
                foreach (var item in descendants)
                {
                    if (item is ContainerInline containerInline)
                        foreach (var obj in containerInline.GetNestedDescendantsFromDepth(minDepth, predicate))
                            yield return obj;
                }
            }
        }
    }

    /// <summary>
    /// Gets specific nested descendants of a <see cref="LeafBlock"/> starting from a specified depth.
    /// </summary>
    /// <param name="parent">The parent <see cref="LeafBlock"/>.</param>
    /// <param name="minDepth">The minimum depth of the recursive nested descendants to return. This must not be less than 1.</param>
    /// <param name="predicate">Function that specifies which descendant object to return.</param>
    /// <returns>Nested desendants at or greater than <paramref name="minDepth"/> levels deep where <paramref name="predicate"/> returns true, not including <see cref="HtmlAttributes"/>.</returns>
    /// <remarks>No descendants of yeilded items will be returned.</remarks>
    internal static IEnumerable<MarkdownObject> GetNestedDescendantsFromDepth(this LeafBlock parent, int minDepth, Func<MarkdownObject, bool> predicate)
    {
        Debug.Assert(parent is not null);
        Debug.Assert(predicate is not null);
        return (parent.Inline is null) ? [] : parent.Inline.GetNestedDescendantsFromDepth(minDepth, predicate);
    }

    /// <summary>
    /// Gets specific nested descendants and attributes of a <see cref="ContainerBlock"/> starting from a specified depth.
    /// </summary>
    /// <param name="parent">The parent <see cref="ContainerBlock"/>.</param>
    /// <param name="minDepth">The minimum depth of the recursive nested descendants to return. This must not be less than 1.</param>
    /// <param name="predicate">Function that specifies which descendant object to return.</param>
    /// <returns>Nested desendants at or greater than <paramref name="minDepth"/> levels deep where <paramref name="predicate"/> returns true, including <see cref="HtmlAttributes"/>.</returns>
    /// <remarks>No attributes or descendants of yeilded items will be returned.</remarks>
    internal static IEnumerable<MarkdownObject> GetNestedDescendantsAndAttributesFromDepth(this ContainerBlock parent, int minDepth, Func<MarkdownObject, bool> predicate)
    {
        Debug.Assert(parent is not null);
        Debug.Assert(predicate is not null);
        Debug.Assert(minDepth > 0);
        if (parent.HasDirectDescendants(out IEnumerable<MarkdownObject>? descendants))
        {
            if (minDepth == 1)
            {
                var attribute = parent.TryGetAttributes();
                if (attribute is not null)
                    yield return attribute;
                foreach (var item in descendants)
                {
                    if (predicate(item))
                        yield return item;
                    else if (item is ContainerBlock containerBlock)
                        foreach (var obj in containerBlock.GetNestedDescendantsAndAttributes(predicate))
                            yield return obj;
                    else if (item is LeafBlock leafBlock)
                        foreach (var obj in leafBlock.GetNestedDescendantsAndAttributes(predicate))
                            yield return obj;
                    else if ((attribute = item.TryGetAttributes()) is not null)
                        yield return attribute;
                }
            }
            else
            {
                minDepth--;
                if (minDepth == 1)
                    foreach (var item in descendants)
                    {
                        if (item is ContainerBlock containerBlock)
                            foreach (var obj in containerBlock.GetNestedDescendantsAndAttributesFromDepth(minDepth, predicate))
                                yield return obj;
                        else if (item is LeafBlock leafBlock)
                            foreach (var obj in leafBlock.GetNestedDescendantsAndAttributesFromDepth(minDepth, predicate))
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
                            foreach (var obj in containerBlock.GetNestedDescendantsAndAttributesFromDepth(minDepth, predicate))
                                yield return obj;
                        else if (item is LeafBlock leafBlock)
                            foreach (var obj in leafBlock.GetNestedDescendantsAndAttributesFromDepth(minDepth, predicate))
                                yield return obj;
                    }
            }
        }
    }

    /// <summary>
    /// Gets specific nested descendants and attributes of a <see cref="ContainerInline"/> starting from a specified depth.
    /// </summary>
    /// <param name="parent">The parent <see cref="ContainerInline"/>.</param>
    /// <param name="minDepth">The minimum depth of the recursive nested descendants to return. This must not be less than 1.</param>
    /// <param name="predicate">Function that specifies which descendant object to return.</param>
    /// <returns>Nested desendants at or greater than <paramref name="minDepth"/> levels deep where <paramref name="predicate"/> returns true, including <see cref="HtmlAttributes"/>.</returns>
    /// <remarks>No attributes or descendants of yeilded items will be returned.</remarks>
    internal static IEnumerable<MarkdownObject> GetNestedDescendantsAndAttributesFromDepth(this ContainerInline parent, int minDepth, Func<MarkdownObject, bool> predicate)
    {
        Debug.Assert(parent is not null);
        Debug.Assert(predicate is not null);
        Debug.Assert(minDepth > 0);
        if (parent.HasDirectDescendants(out IEnumerable<MarkdownObject>? descendants))
        {
            if (minDepth == 1)
            {
                var attribute = parent.TryGetAttributes();
                if (attribute is not null)
                    yield return attribute;
                foreach (var item in descendants)
                {
                    if (predicate(item))
                        yield return item;
                    else if (item is ContainerInline containerInline)
                        foreach (var obj in containerInline.GetNestedDescendantsAndAttributes(predicate))
                            yield return obj;
                    else if ((attribute = item.TryGetAttributes()) is not null)
                        yield return attribute;
                }
            }
            else
            {
                minDepth--;
                if (minDepth == 1)
                    foreach (var item in descendants)
                    {
                        if (item is ContainerInline containerInline)
                            foreach (var obj in containerInline.GetNestedDescendantsAndAttributesFromDepth(minDepth, predicate))
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
                            foreach (var obj in containerInline.GetNestedDescendantsAndAttributesFromDepth(minDepth, predicate))
                                yield return obj;
                    }
            }
        }
    }

    /// <summary>
    /// Gets specific nested descendants and attributes of a <see cref="LeafBlock"/> starting from a specified depth.
    /// </summary>
    /// <param name="parent">The parent <see cref="LeafBlock"/>.</param>
    /// <param name="minDepth">The minimum depth of the recursive nested descendants to return. This must not be less than 1.</param>
    /// <param name="predicate">Function that specifies which descendant object to return.</param>
    /// <returns>Nested desendants at or greater than <paramref name="minDepth"/> levels deep where <paramref name="predicate"/> returns true, including <see cref="HtmlAttributes"/>.</returns>
    /// <remarks>No attributes or descendants of yeilded items will be returned.</remarks>
    internal static IEnumerable<MarkdownObject> GetNestedDescendantsAndAttributesFromDepth(this LeafBlock parent, int minDepth, Func<MarkdownObject, bool> predicate)
    {
        Debug.Assert(parent is not null);
        Debug.Assert(predicate is not null);
        Debug.Assert(minDepth > 0);
        if (parent.Inline is null)
        {
            if (minDepth == 1)
            {
                var attribute = parent.TryGetAttributes();
                if (attribute is not null)
                    yield return attribute;
            }
        }
        else if (parent.Inline.HasDirectDescendants(out IEnumerable<MarkdownObject>? descendants))
        {
            if (minDepth == 1)
            {
                var attribute = parent.TryGetAttributes();
                if (attribute is not null)
                    yield return attribute;
                foreach (var item in descendants)
                {
                    if (predicate(item))
                        yield return item;
                    else if (item is ContainerInline containerInline)
                        foreach (var obj in containerInline.GetNestedDescendantsAndAttributes(predicate))
                            yield return obj;
                    else if ((attribute = item.TryGetAttributes()) is not null)
                        yield return attribute;
                }
            }
            else
            {
                minDepth--;
                if (minDepth == 1)
                    foreach (var item in descendants)
                    {
                        if (item is ContainerInline containerInline)
                            foreach (var obj in containerInline.GetNestedDescendantsAndAttributesFromDepth(minDepth, predicate))
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
                            foreach (var obj in containerInline.GetNestedDescendantsAndAttributesFromDepth(minDepth, predicate))
                                yield return obj;
                    }
            }
        }
    }
}
