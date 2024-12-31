using System.Reflection;
using Markdig.Renderers.Html;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace HtmlUtility;

public static partial class MarkdownExtensionMethods
{
    internal static IEnumerable<MarkdownObject> EnumerateMarkdownObjectIfNotNull(this MarkdownObject? markdownObject)
    {
        if (markdownObject is not null)
            yield return markdownObject;
    }

    internal static IEnumerable<MarkdownObject> EnumerateMarkdownObjects(this MarkdownObject markdownObject1, MarkdownObject? markdownObject2)
    {
        yield return markdownObject1;
        if (markdownObject2 is not null)
            yield return markdownObject2;
    }

    internal static IEnumerable<MarkdownObject> PrependToMarkdownObjects(this MarkdownObject markdownObject, IEnumerable<MarkdownObject> following)
    {
        yield return markdownObject;
        foreach (var item in following)
            yield return item;
    }

    internal static IEnumerable<MarkdownObject> PrependToMarkdownObjectsIfNotNull(this MarkdownObject? markdownObject, IEnumerable<MarkdownObject> following)
    {
        if (markdownObject is not null)
            yield return markdownObject;
        foreach (var item in following)
            yield return item;
    }

    internal static IEnumerable<MarkdownObject> PrependToMarkdownObjects(this MarkdownObject markdownObject1, MarkdownObject? markdownObject2, IEnumerable<MarkdownObject> following)
    {
        yield return markdownObject1;
        if (markdownObject2 is not null)
            yield return markdownObject2;
        foreach (var item in following)
            yield return item;
    }

    public static Type ToReflectionType(this MarkdownTokenType type) => _markdownTokenTypeMap[type];

    public static List<Type> ToReflectionTypes(this IEnumerable<MarkdownTokenType> source)
    {
        if (source is null) return [];
        var types = source.Distinct().Select(ToReflectionType).ToList();
        for (int end = 1; end < types.Count; end++)
        {
            var x = types[end];
            for (int n = 0; n < end; n++)
            {
                var y = types[n];
                if (x.IsAssignableFrom(y))
                {
                    types.RemoveAt(n);
                    end--;
                    break;
                }
                if (y.IsAssignableFrom(x))
                {
                    types.RemoveAt(end);
                    end--;
                    break;
                }
            }
        }
        return types;
    }

    public static IEnumerable<(MarkdownObject Parent, HtmlAttributes Attribute)> AllAttributes(this MarkdownObject? source)
    {
        if (source is not null)
        {
            foreach (MarkdownObject parent in source.Descendants())
            {
                var attr = parent.TryGetAttributes();
                if (attr is not null)
                    yield return (parent, attr);
            }
        }
    }

    /// <summary>
    /// Searches for descendants that match a specified type, ignoring nested descendants of matching markdown objects.
    /// </summary>
    /// <param name="source">The prospective parent markdown object.</param>
    /// <param name="type">The object type to search for.</param>
    /// <param name="emitAttributesofUnmatched">Whether to emit <see cref="HtmlAttributes"/> of unmatched tokens.</param>
    /// <returns>The <see cref="MarkdownObject"/>s that descend from the <paramref name="source"/> object that is an instance of the specified <paramref name="type"/>,
    /// except for any that have an ancestor that has already been yielded.</returns>
    public static IEnumerable<MarkdownObject> DescendantBranchesMatchingType(this MarkdownObject? source, Type type)
    {
        ArgumentNullException.ThrowIfNull(type);
        if (source is null || !type.IsNonAttributeMarkdownObjectType(out bool isBaseType)) return [];
        return isBaseType ? source.GetDirectDescendants() : GetBranches(source, type.IsInstanceOfType);
    }

    /// <summary>
    /// Searches for descendants tht match any of the specified types, ignoring nested descendants of matching markdown objects.
    /// </summary>
    /// <param name="source">The prospective parent markdown object.</param>
    /// <param name="types">The object types to search for.</param>
    /// <param name="emitAttributesofUnmatched">Whether to emit <see cref="HtmlAttributes"/> of unmatched tokens.</param>
    /// <returns>The <see cref="MarkdownObject"/>s that descend from the <paramref name="source"/> object that is an instance of any of the specified <paramref name="types"/>,
    /// except for any that have an ancestor that has already been yielded.</returns>
    public static IEnumerable<MarkdownObject> DescendantBranchesMatchingType(this MarkdownObject? source, IEnumerable<Type> types)
    {
        ArgumentNullException.ThrowIfNull(types);
        if (source is null) return [];
        types = types.Where(IsNonAttributeMarkdownObjectType).CollapseTypes(MarkdownObjectType, out int? typeCount);
        if (!typeCount.HasValue)
            return source.GetDirectDescendants();
        if (typeCount.Value == 1)
            return GetBranches(source, types.First().IsInstanceOfType);
        return (typeCount.Value > 0) ? GetBranches(source, obj => types.Any(t => t.IsInstanceOfType(obj))) : [];
    }

    /// <summary>
    /// Searches for descendants, up to a specified recursion depth, which match the specifie type, ignoring nested descendants of matching markdown objects.
    /// </summary>
    /// <param name="source">The prospective parent markdown object.</param>
    /// <param name="type">The object type to search for.</param>
    /// <param name="maximumDepth">The maximum number of times to recurse into nested child objects.</param>
    /// <param name="emitAttributesofUnmatched">Whether to emit <see cref="HtmlAttributes"/> of unmatched tokens.</param>
    /// <returns>The <see cref="MarkdownObject"/>s that descend from the <paramref name="source"/> object that is an instance of the specified <paramref name="type"/>,
    /// except for any that have an ancestor that has already been yielded or are beyond the specified <paramref name="maximumDepth"/>.</returns>
    /// <remarks>If <paramref name="maximumDepth"/> is less than <c>1</c>, nothing will be yielded.</remarks>
    public static IEnumerable<MarkdownObject> DescendantBranchesMatchingType(this MarkdownObject? source, Type type, int maximumDepth)
    {
        ArgumentNullException.ThrowIfNull(type);
        if (source is null || maximumDepth < 1 || !type.IsNonAttributeMarkdownObjectType(out bool isBaseType)) return [];
        return isBaseType ? source.GetDirectDescendants(type.IsInstanceOfType) : GetBranchesToDepth(source, maximumDepth, type.IsInstanceOfType);
    }

    /// <summary>
    /// Searches for descendants, up to a specified recursion depth, which match any of the specified types, ignoring nested descendants of matching markdown objects.
    /// </summary>
    /// <param name="source">The prospective parent markdown object.</param>
    /// <param name="types">The object types to search for.</param>
    /// <param name="maximumDepth">The maximum number of times to recurse into nested child objects.</param>
    /// <param name="emitAttributesofUnmatched">Whether to emit <see cref="HtmlAttributes"/> of unmatched tokens.</param>
    /// <returns>The <see cref="MarkdownObject"/>s that descend from the <paramref name="source"/> object that is an instance of any of the specified <paramref name="types"/>,
    /// except for any that have an ancestor that has already been yielded or are beyond the specifed <paramref name="maximumDepth"/>.</returns>
    /// <remarks>If <paramref name="maximumDepth"/> is less than <c>1</c>, nothing will be yielded.</remarks>
    public static IEnumerable<MarkdownObject> DescendantBranchesMatchingType(this MarkdownObject? source, IEnumerable<Type> types, int maximumDepth)
    {
        ArgumentNullException.ThrowIfNull(types);
        if (source is null || maximumDepth < 1) return [];
        types = types.Where(IsNonAttributeMarkdownObjectType).CollapseTypes(MarkdownObjectType, out int? typeCount);
        if (!typeCount.HasValue)
            return source.GetDirectDescendants(obj => types.Any(t => t.IsInstanceOfType(obj)));
        if (typeCount.Value == 1)
            return GetBranchesToDepth(source, maximumDepth, types.First().IsInstanceOfType);
        return (typeCount.Value > 0) ? GetBranchesToDepth(source, maximumDepth, obj => types.Any(t => t.IsInstanceOfType(obj))) : [];
    }

    /// <summary>
    /// Gets descendents which exist at a specified recursion depth, not including <see cref="HtmlAttributes"/>.
    /// </summary>
    /// <param name="source">The prospective parent markdown object.</param>
    /// <param name="depth">The number of times to recurse into child markdown objects.</param>
    /// <returns>The <see cref="MarkdownObject"/>s that exist at the specified recursion <paramref name="depth"/>.</returns>
    /// <remarks>If <paramref name="depth"/> is less than <c>1</c>, nothing will be yielded.</remarks>
    public static IEnumerable<MarkdownObject> DescendantsAtDepth(this MarkdownObject? source, int depth)
    {
        return (source is null || depth < 1) ? [] : source.GetDescendantsAtDepth(depth);
    }

    /// <summary>
    /// Gets descendents which exist at a specified recursion depth, including <see cref="HtmlAttributes"/>.
    /// </summary>
    /// <param name="source">The prospective parent markdown object.</param>
    /// <param name="depth">The number of times to recurse into child markdown objects.</param>
    /// <returns>The <see cref="MarkdownObject"/>s that exist at the specified recursion <paramref name="depth"/>.</returns>
    /// <remarks>If <paramref name="depth"/> is less than <c>1</c>, nothing will be yielded.</remarks>
    public static IEnumerable<MarkdownObject> DescendantsAtDepthIncludingHtmlAttributes(this MarkdownObject? source, int depth)
    {
        return (source is null || depth < 1) ? [] : source.GetDescendantsAtDepthIncludingAttributes(depth);
    }

    /// <summary>
    /// Gets descendents which exists at or beyond a specified recursion depth.
    /// </summary>
    /// <param name="source">The prospective parent markdown object.</param>
    /// <param name="minimumDepth">The number of to recurse into child objects before yielding descendant markdown objects.</param>
    /// <returns>The <see cref="MarkdownObject"/>s that exist at or beyond the specified recursion <paramref name="minimumDepth"/>.</returns>
    /// <remarks>If <paramref name="minimumDepth"/> is less than <c>1</c>, nothing will be yielded.</remarks>
    public static IEnumerable<MarkdownObject> DescendantsFromDepth(this MarkdownObject? source, int minimumDepth)
    {
        if (source is null) return [];
        return (minimumDepth < 2) ? source.Descendants() : source.GetDescendantsFromDepth(minimumDepth);
    }

    /// <summary>
    /// Gets descendents which exists at or beyond a specified recursion depth.
    /// </summary>
    /// <param name="source">The prospective parent markdown object.</param>
    /// <param name="minimumDepth">The number of to recurse into child objects before yielding descendant markdown objects.</param>
    /// <returns>The <see cref="MarkdownObject"/>s that exist at or beyond the specified recursion <paramref name="minimumDepth"/>.</returns>
    /// <remarks>If <paramref name="minimumDepth"/> is less than <c>1</c>, nothing will be yielded.</remarks>
    public static IEnumerable<MarkdownObject> DescendantsIncludingAttributesFromDepth(this MarkdownObject? source, int minimumDepth)
    {
        if (source is null) return [];
        return (minimumDepth < 2) ? source.GetDescendantsAndAttributes() : source.GetDescendantsAndAttributesFromDepth(minimumDepth);
    }

    /// <summary>
    /// Gets descendants that exist at or below the specified recursion depth.
    /// </summary>
    /// <param name="source">The prospective parent markdown object.</param>
    /// <param name="maximumDepth">The maximum number of times to recurse into nested child objects.</param>
    /// <returns>The <see cref="MarkdownObject"/>s that exist at or below the specified recursion <paramref name="maximumDepth"/>.</returns>
    /// <remarks>If <paramref name="minimumDepth"/> is less than <c>1</c>, nothing will be yielded.</remarks>
    public static IEnumerable<MarkdownObject> DescendantsUpToDepth(this MarkdownObject? source, int maximumDepth) => (source is null || maximumDepth < 1) ? [] : source.GetDescendantsToDepth(maximumDepth);

    /// <summary>
    /// Gets descendants that exist at or below the specified recursion depth.
    /// </summary>
    /// <param name="source">The prospective parent markdown object.</param>
    /// <param name="maximumDepth">The maximum number of times to recurse into nested child objects.</param>
    /// <returns>The <see cref="MarkdownObject"/>s that exist at or below the specified recursion <paramref name="maximumDepth"/>.</returns>
    /// <remarks>If <paramref name="minimumDepth"/> is less than <c>1</c>, nothing will be yielded.</remarks>
    public static IEnumerable<MarkdownObject> DescendantsIncludingAttribuetsUpToDepth(this MarkdownObject? source, int maximumDepth) =>
        (source is null || maximumDepth < 1) ? [] : source.GetDescendantsAndAttributesToDepth(maximumDepth);

    public static IEnumerable<MarkdownObject> DescendantsInDepthRange(this MarkdownObject? source, int minimumDepth, int maximumDepth)
    {
        if (source is null || maximumDepth < 1 || maximumDepth < minimumDepth) return [];
        return (minimumDepth < 2) ? source.GetDescendantsToDepth(maximumDepth) : (minimumDepth < maximumDepth) ? source.GetDescendantsInDepthRange(minimumDepth, maximumDepth) : source.GetDescendantsAtDepth(minimumDepth);
    }

    public static IEnumerable<MarkdownObject> DescendantsIncludingAttributesInDepthRange(this MarkdownObject? source, int minimumDepth, int maximumDepth)
    {
        if (source is null || maximumDepth < 1 || maximumDepth < minimumDepth) return [];
        return (minimumDepth < 2) ? source.GetDescendantsAndAttributesToDepth(maximumDepth) : (minimumDepth < maximumDepth) ? source.GetDescendantsAndAttributesInDepthRange(minimumDepth, maximumDepth) :
            source.GetDescendantsAtDepthIncludingAttributes(minimumDepth);
    }

    public static IEnumerable<MarkdownObject> DescendantBranchesMatchingType(this MarkdownObject? source, Type type, int minimumDepth, int maximumDepth)
    {
        ArgumentNullException.ThrowIfNull(type);
        if (source is null || maximumDepth < 1 || maximumDepth < minimumDepth || !type.IsNonAttributeMarkdownObjectType(out bool isBaseType)) return [];
        if (isBaseType)
            return (minimumDepth < 2) ? source.GetDirectDescendants(type.IsInstanceOfType) : source.GetDescendantsAtDepth(minimumDepth, type.IsInstanceOfType);
        return (minimumDepth < 2) ? source.GetBranchesToDepth(maximumDepth, type.IsInstanceOfType) : (minimumDepth < maximumDepth) ? source.GetBranchesInDepthRange(minimumDepth, maximumDepth, type.IsInstanceOfType) :
            source.GetDescendantsAtDepth(minimumDepth, type.IsInstanceOfType);
    }

    public static IEnumerable<MarkdownObject> DescendantBranchesMatchingTypeIncludingAttributes(this MarkdownObject? source, Type type, int minimumDepth, int maximumDepth)
    {
        ArgumentNullException.ThrowIfNull(type);
        if (source is null || maximumDepth < 1 || maximumDepth < minimumDepth || !type.IsNonAttributeMarkdownObjectType(out bool isBaseType)) return [];
        if (isBaseType)
            return (minimumDepth < 2) ? source.GetDirectDescendantsAndAttributes(type.IsInstanceOfType) : source.GetDescendantsAtDepthIncludingAttributes(minimumDepth, type.IsInstanceOfType);
        return (minimumDepth < 2) ? source.GetBranchesIncludingAttributesToDepth(maximumDepth, type.IsInstanceOfType) :
            (minimumDepth < maximumDepth) ? source.GetBranchesIncludingAttributesInDepthRange(minimumDepth, maximumDepth, type.IsInstanceOfType) :
                source.GetDescendantsAtDepthIncludingAttributes(minimumDepth, type.IsInstanceOfType);
    }

    public static IEnumerable<MarkdownObject> DescendantBranchesMatchingType(this MarkdownObject? source, IEnumerable<Type> types, int minimumDepth, int maximumDepth)
    {
        ArgumentNullException.ThrowIfNull(types);
        if (source is null || minimumDepth < 1 || maximumDepth < minimumDepth) return [];
        types = types.Where(IsNonAttributeMarkdownObjectType).CollapseTypes(MarkdownObjectType, out int? typeCount);
        if (!typeCount.HasValue)
            return (minimumDepth < 2) ? source.GetDirectDescendants() : source.GetDescendantsAtDepth(minimumDepth);
        if (typeCount == 1)
            return (minimumDepth < 2) ? source.GetBranchesToDepth(maximumDepth, types.First().IsInstanceOfType) :
                (minimumDepth < maximumDepth) ? source.GetBranchesInDepthRange(minimumDepth, maximumDepth, types.First().IsInstanceOfType) : source.GetDescendantsAtDepth(minimumDepth, types.First().IsInstanceOfType);
        
        return (minimumDepth < 2) ? source.GetBranchesToDepth(maximumDepth, obj => types.Any(t => t.IsInstanceOfType(obj))) :
            (minimumDepth < maximumDepth) ? source.GetBranchesInDepthRange(minimumDepth, maximumDepth, obj => types.Any(t => t.IsInstanceOfType(obj))) :
                source.GetDescendantsAtDepth(minimumDepth, obj => types.Any(t => t.IsInstanceOfType(obj)));
    }

    public static IEnumerable<MarkdownObject> DescendantBranchesMatchingTypeIncludingAttributes(this MarkdownObject? source, IEnumerable<Type> types, int minimumDepth, int maximumDepth)
    {
        ArgumentNullException.ThrowIfNull(types);
        if (source is null || minimumDepth < 1 || maximumDepth < minimumDepth) return [];
        types = types.Where(IsNonAttributeMarkdownObjectType).CollapseTypes(MarkdownObjectType, out int? typeCount);
        if (!typeCount.HasValue)
            return (minimumDepth < 2) ? source.GetDirectDescendantsAndAttributes() : source.GetDescendantsAtDepthIncludingAttributes(minimumDepth);
        if (typeCount == 1)
            return (minimumDepth < 2) ? source.GetBranchesIncludingAttributesToDepth(maximumDepth, types.First().IsInstanceOfType) :
                (minimumDepth < maximumDepth) ? source.GetBranchesIncludingAttributesInDepthRange(minimumDepth, maximumDepth, types.First().IsInstanceOfType) :
                    source.GetDescendantsAtDepthIncludingAttributes(minimumDepth, types.First().IsInstanceOfType);
        
        return (minimumDepth < 2) ? source.GetBranchesIncludingAttributesToDepth(maximumDepth, obj => types.Any(t => t.IsInstanceOfType(obj))) :
            (minimumDepth < maximumDepth) ? source.GetBranchesIncludingAttributesInDepthRange(minimumDepth, maximumDepth, obj => types.Any(t => t.IsInstanceOfType(obj))) :
                source.GetDescendantsAtDepthIncludingAttributes(minimumDepth, obj => types.Any(t => t.IsInstanceOfType(obj)));
    }
}