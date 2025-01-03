using System.Diagnostics;
using Markdig.Renderers.Html;
using Markdig.Syntax;

namespace HtmlUtility;

public static partial class MarkdownExtensionMethods
{
    /// <summary>
    /// Gets the direct descendants of a <see cref="MarkdownObject"/>.
    /// </summary>
    /// <param name="parent">The parent <see cref="MarkdownObject"/>.</param>
    /// <returns>Direct descendants of <paramref name="parent"/>, not including <see cref="HtmlAttributes"/>.</returns>
    internal static IEnumerable<MarkdownObject> GetDirectDescendants(this MarkdownObject parent)
    {
        Debug.Assert(parent is not null);
        return parent.HasDirectDescendant(out IEnumerable<MarkdownObject>? descendants) ? descendants : [];
    }

    /// <summary>
    /// Gets the direct descendants and attributes of a <see cref="MarkdownObject"/>.
    /// </summary>
    /// <param name="parent">The parent <see cref="MarkdownObject"/>.</param>
    /// <returns>Direct descendants of <paramref name="parent"/>, including <see cref="HtmlAttributes"/> of <paramref name="parent"/>, if present.</returns>
    internal static IEnumerable<MarkdownObject> GetDirectDescendantsAndAttributes(this MarkdownObject parent)
    {
        Debug.Assert(parent is not null);
        return parent.HasDirectDescendantIncludingAttributes(out IEnumerable<MarkdownObject>? descendants) ? descendants : [];
    }

    /// <summary>
    /// Gets specific direct descendants of a <see cref="MarkdownObject"/>.
    /// </summary>
    /// <param name="parent">The parent <see cref="MarkdownObject"/>.</param>
    /// <param name="predicate">Function that specifies which descendant object to return.</param>
    /// <returns>Direct descendants of <paramref name="parent"/> where <paramref name="predicate"/> returns true.</returns>
    internal static IEnumerable<MarkdownObject> GetDirectDescendants(this MarkdownObject parent, Func<MarkdownObject, bool> predicate)
    {
        Debug.Assert(parent is not null);
        Debug.Assert(predicate is not null);
        return parent.HasDirectDescendant(out IEnumerable<MarkdownObject>? descendants) ? descendants.Where(predicate) : [];
    }

    /// <summary>
    /// Gets specific direct descendants and attributes of a <see cref="MarkdownObject"/>.
    /// </summary>
    /// <param name="parent">The parent <see cref="MarkdownObject"/>.</param>
    /// <param name="predicate">Function that specifies which descendant object to return.</param>
    /// <returns>Nested recursive descendants where <paramref name="predicate"/> returns true.</returns>
    /// <remarks>No attributes or descendants of yeilded items will be returned.
    /// <para>The <see cref="HtmlAttributes"/> of <paramref name="parent"/> will always be returned, if present.</para></remarks>
    internal static IEnumerable<MarkdownObject> GetDirectDescendantsAndAttributes(this MarkdownObject parent, Func<MarkdownObject, bool> predicate)
    {
        Debug.Assert(parent is not null);
        Debug.Assert(predicate is not null);
        var attributes = parent.TryGetAttributes();
        return parent.HasDirectDescendant(out IEnumerable<MarkdownObject>? descendants) ? attributes.PrependToMarkdownObjectsIfNotNull(descendants) : attributes.EnumerateMarkdownObjectIfNotNull();
    }
}
