using System.Diagnostics;
using Markdig.Renderers.Html;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace HtmlUtility;

public static partial class MarkdownExtensionMethods
{
    /// <summary>
    /// Gets the nested descendants of a <see cref="ContainerBlock"/>.
    /// </summary>
    /// <param name="parent">The parent <see cref="ContainerBlock"/>.</param>
    /// <returns>Recursive nested descendants of <paramref name="parent"/>, not including <see cref="HtmlAttributes"/>.</returns>
    internal static IEnumerable<MarkdownObject> GetNestedDescendants(this ContainerBlock parent)
    {
        Debug.Assert(parent is not null);
        if (parent.HasDirectDescendants(out IEnumerable<MarkdownObject>? descendants))
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
    }

    /// <summary>
    /// Gets the nested descendants of a <see cref="ContainerInline"/>.
    /// </summary>
    /// <param name="parent">The parent <see cref="ContainerInline"/>.</param>
    /// <returns>Recursive nested descendants of <paramref name="parent"/>, not including <see cref="HtmlAttributes"/>.</returns>
    internal static IEnumerable<MarkdownObject> GetNestedDescendants(this ContainerInline parent)
    {
        Debug.Assert(parent is not null);
        if (parent.HasDirectDescendants(out IEnumerable<MarkdownObject>? descendants))
        {
            foreach (var item in descendants)
            {
                yield return item;
                if (item is ContainerInline containerInline)
                    foreach (var obj in containerInline.GetNestedDescendants())
                        yield return obj;
            }
        }
    }

    /// <summary>
    /// Gets the nested descendants of a <see cref="LeafBlock"/>.
    /// </summary>
    /// <param name="parent">The parent <see cref="LeafBlock"/>.</param>
    /// <returns>Recursive nested descendants of <paramref name="parent"/>, not including <see cref="HtmlAttributes"/>.</returns>
    internal static IEnumerable<MarkdownObject> GetNestedDescendants(this LeafBlock parent)
    {
        Debug.Assert(parent is not null);
        return (parent.Inline is null) ? [] : parent.Inline.GetNestedDescendants();
    }

    /// <summary>
    /// Gets the nested descendants and attributes of a <see cref="ContainerBlock"/>.
    /// </summary>
    /// <param name="parent">The parent <see cref="ContainerBlock"/>.</param>
    /// <returns>Recursive nested descendants of <paramref name="parent"/>, including <see cref="HtmlAttributes"/>.</returns>
    internal static IEnumerable<MarkdownObject> GetNestedDescendantsAndAttributes(this ContainerBlock parent)
    {
        Debug.Assert(parent is not null);
        var attribute = parent.TryGetAttributes();
        if (attribute is not null)
            yield return attribute;
        if (parent.HasDirectDescendants(out IEnumerable<MarkdownObject>? descendants))
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

    /// <summary>
    /// Gets the nested descendants and attributes of a <see cref="ContainerInline"/>.
    /// </summary>
    /// <param name="parent">The parent <see cref="ContainerInline"/>.</param>
    /// <returns>Recursive nested descendants of <paramref name="parent"/>, including <see cref="HtmlAttributes"/>.</returns>
    internal static IEnumerable<MarkdownObject> GetNestedDescendantsAndAttributes(this ContainerInline parent)
    {
        Debug.Assert(parent is not null);
        var attribute = parent.TryGetAttributes();
        if (attribute is not null)
            yield return attribute;
        if (parent.HasDirectDescendants(out IEnumerable<MarkdownObject>? descendants))
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

    /// <summary>
    /// Gets the nested descendants and attributes of a <see cref="LeafBlock"/>.
    /// </summary>
    /// <param name="parent">The parent <see cref="LeafBlock"/>.</param>
    /// <returns>Recursive nested descendants of <paramref name="parent"/>, including <see cref="HtmlAttributes"/>.</returns>
    internal static IEnumerable<MarkdownObject> GetNestedDescendantsAndAttributes(this LeafBlock parent)
    {
        Debug.Assert(parent is not null);
        var attribute = parent.TryGetAttributes();
        if (attribute is not null)
            yield return attribute;
        if (parent.Inline is not null)
            foreach (var item in parent.Inline.GetNestedDescendantsAndAttributes())
                yield return item;
    }

    /// <summary>
    /// Gets specific nested descendants of a <see cref="ContainerBlock"/>.
    /// </summary>
    /// <param name="parent">The parent <see cref="ContainerBlock"/>.</param>
    /// <param name="predicate">Function that specifies which descendant object to return.</param>
    /// <returns>Nested recursive descendants of <paramref name="parent"/> where <paramref name="predicate"/> returns true.</returns>
    /// <remarks>No descendants of yeilded items will be returned.</remarks>
    internal static IEnumerable<MarkdownObject> GetNestedDescendants(this ContainerBlock parent, Func<MarkdownObject, bool> predicate)
    {
        Debug.Assert(parent is not null);
        Debug.Assert(predicate is not null);
        if (parent.HasDirectDescendants(out IEnumerable<MarkdownObject>? descendants))
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
    }

    /// <summary>
    /// Gets specific nested descendants of a <see cref="ContainerInline"/>.
    /// </summary>
    /// <param name="parent">The parent <see cref="ContainerInline"/>.</param>
    /// <param name="predicate">Function that specifies which descendant object to return.</param>
    /// <returns>Direct descendants of <paramref name="parent"/> where <paramref name="predicate"/> returns true.</returns>
    /// <remarks>No descendants of yeilded items will be returned.</remarks>
    internal static IEnumerable<MarkdownObject> GetNestedDescendants(this ContainerInline parent, Func<MarkdownObject, bool> predicate)
    {
        Debug.Assert(parent is not null);
        Debug.Assert(predicate is not null);
        if (parent.HasDirectDescendants(out IEnumerable<MarkdownObject>? descendants))
        {
            foreach (var item in descendants)
            {
                if (predicate(item))
                    yield return item;
                else if (item is ContainerInline containerInline)
                    foreach (var obj in containerInline.GetNestedDescendants(predicate))
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
    internal static IEnumerable<MarkdownObject> GetNestedDescendants(this LeafBlock parent, Func<MarkdownObject, bool> predicate)
    {
        Debug.Assert(parent is not null);
        Debug.Assert(predicate is not null);
        return (parent.Inline is null) ? [] : parent.Inline.GetNestedDescendants(predicate);
    }

    /// <summary>
    /// Gets specific nested descendants and attributes of a <see cref="ContainerBlock"/>.
    /// </summary>
    /// <param name="parent">The parent <see cref="ContainerBlock"/>.</param>
    /// <param name="predicate">Function that specifies which descendant object to return.</param>
    /// <returns>Nested recursive descendants where <paramref name="predicate"/> returns true, as well as <see cref="HtmlAttributes"/>.</returns>
    /// <remarks>No attributes or descendants of yeilded items will be returned.
    /// <para>The <see cref="HtmlAttributes"/> of <paramref name="parent"/> will always be returned, if present.</para></remarks>
    internal static IEnumerable<MarkdownObject> GetNestedDescendantsAndAttributes(this ContainerBlock parent, Func<MarkdownObject, bool> predicate)
    {
        Debug.Assert(parent is not null);
        Debug.Assert(predicate is not null);
        var attribute = parent.TryGetAttributes();
        if (attribute is not null)
            yield return attribute;
        if (parent.HasDirectDescendants(out IEnumerable<MarkdownObject>? descendants))
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

    /// <summary>
    /// Gets specific nested descendants and attributes of a <see cref="ContainerInline"/>.
    /// </summary>
    /// <param name="parent">The parent <see cref="ContainerInline"/>.</param>
    /// <param name="predicate">Function that specifies which descendant object to return.</param>
    /// <returns>Nested recursive descendants where <paramref name="predicate"/> returns true, as well as <see cref="HtmlAttributes"/>.</returns>
    /// <remarks>No attributes or descendants of yeilded items will be returned.
    /// <para>The <see cref="HtmlAttributes"/> of <paramref name="parent"/> will always be returned, if present.</para></remarks>
    internal static IEnumerable<MarkdownObject> GetNestedDescendantsAndAttributes(this ContainerInline parent, Func<MarkdownObject, bool> predicate)
    {
        Debug.Assert(parent is not null);
        Debug.Assert(predicate is not null);
        var attribute = parent.TryGetAttributes();
        if (attribute is not null)
            yield return attribute;
        if (parent.HasDirectDescendants(out IEnumerable<MarkdownObject>? descendants))
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

    /// <summary>
    /// Gets specific nested descendants and attributes of a <see cref="LeafBlock"/>.
    /// </summary>
    /// <param name="parent">The parent <see cref="LeafBlock"/>.</param>
    /// <param name="predicate">Function that specifies which descendant object to return.</param>
    /// <returns>Nested recursive descendants where <paramref name="predicate"/> returns true, as well as <see cref="HtmlAttributes"/>.</returns>
    /// <remarks>No attributes or descendants of yeilded items will be returned.
    /// <para>The <see cref="HtmlAttributes"/> of <paramref name="parent"/> will always be returned, if present.</para></remarks>
    internal static IEnumerable<MarkdownObject> GetNestedDescendantsAndAttributes(this LeafBlock parent, Func<MarkdownObject, bool> predicate)
    {
        Debug.Assert(parent is not null);
        Debug.Assert(predicate is not null);
        var attribute = parent.TryGetAttributes();
        if (attribute is not null)
            yield return attribute;
        if (parent.Inline is not null)
            foreach (var item in parent.Inline.GetNestedDescendantsAndAttributes(predicate))
                yield return item;
    }
}
