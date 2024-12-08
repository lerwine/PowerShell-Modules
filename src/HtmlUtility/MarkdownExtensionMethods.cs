using System.Reflection;
using Markdig.Renderers.Html;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace HtmlUtility;

public static partial class MarkdownExtensionMethods
{
    public static Type ToReflectionType(this MarkdownTokenType type) => _markdownTokenTypeMap[type];

    public static List<Type>? ToReflectionTypes(this IList<MarkdownTokenType>? source)
    {
        if (source is null || source.Count == 0) return null;
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

    /// <summary>
    /// Gets all direct child markdown objects.
    /// </summary>
    /// <param name="source">The prospective parent markdown object.</param>
    /// <param name="includeAttributes">Whether to include <see cref="HtmlAttributes"/>.</param>
    /// <returns>The <see cref="MarkdownObject"/>s that are direct child objects of the <paramref name="source"/> object.</returns>
    public static IEnumerable<MarkdownObject> GetChildObjects(this MarkdownObject? source, bool includeAttributes = false)
    {
        if (source is null) return [];
        if (includeAttributes)
        {
            var attributes = source.TryGetAttributes();
            if (attributes is not null)
            {
                if (source.IsMarkdownObjectEnumerable(out IEnumerable<MarkdownObject>? enumerable))
                    return ((MarkdownObject[])[attributes]).Concat(enumerable);
                return [attributes];
            }
        }
        return source.GetDirectDescendants();
    }

    /// <summary>
    /// Gets all descendant markdown objects.
    /// </summary>
    /// <param name="source">The prospective parent markdown object.</param>
    /// <param name="includeAttributes">Whether to include <see cref="HtmlAttributes"/>.</param>
    /// <returns>The <see cref="MarkdownObject"/>s that descend from the <paramref name="source"/> object.</returns>
    public static IEnumerable<MarkdownObject> GetAllDescendants(this MarkdownObject? source, bool includeAttributes = false)
    {
        if (source is null) return [];
        if (includeAttributes)
        {
            if (source is ContainerBlock cb)
                return GetNestedDescendantsAndAttributes(cb);
            if (source is ContainerInline ci)
                return GetNestedDescendantsAndAttributes(ci);
            if (source is LeafBlock lb)
                return GetNestedDescendantsAndAttributes(lb);
            var attributes = source.TryGetAttributes();
            return (attributes is null) ? [] : [attributes];
        }
        if (source is ContainerBlock containerBlock)
            return GetNestedDescendants(containerBlock);
        if (source is ContainerInline containerInline)
            return GetNestedDescendants(containerInline);
        return (source is LeafBlock leafBlock) ? GetNestedDescendants(leafBlock) : [];
    }

    /// <summary>
    /// Searches for descendants that match a specified type, ignoring nested descendants of matching markdown objects.
    /// </summary>
    /// <param name="source">The prospective parent markdown object.</param>
    /// <param name="type">The object type to search for.</param>
    /// <param name="emitAttributesofUnmatched">Whether to emit <see cref="HtmlAttributes"/> of unmatched tokens.</param>
    /// <returns>The <see cref="MarkdownObject"/>s that descend from the <paramref name="source"/> object that is an instance of the specified <paramref name="type"/>,
    /// except for any that have an ancestor that has already been yielded.</returns>
    public static IEnumerable<MarkdownObject> GetDescendantBranchesMatchingType(this MarkdownObject? source, Type type, bool emitAttributesofUnmatched = false)
    {
        ArgumentNullException.ThrowIfNull(type);
        if (source is null) return [];
        if (type == MarkdownObjectType)
            return GetChildObjects(source, emitAttributesofUnmatched);
        if (type.IsNonAttributeMarkdownObjectType())
        {
            if (source is ContainerBlock containerBlock)
            {
                if (emitAttributesofUnmatched)
                    return GetNestedDescendantsAndAttributes(containerBlock, type.IsInstanceOfType);
                return GetNestedDescendants(containerBlock, type.IsInstanceOfType);
            }
            if (source is ContainerInline containerInline)
            {
                if (emitAttributesofUnmatched)
                    return GetNestedDescendantsAndAttributes(containerInline, type.IsInstanceOfType);
                return GetNestedDescendants(containerInline, type.IsInstanceOfType);
            }
            if (source is LeafBlock leafBlock)
            {
                if (emitAttributesofUnmatched)
                    return GetNestedDescendantsAndAttributes(leafBlock, type.IsInstanceOfType);
                return (leafBlock.Inline is null) ? [] : GetNestedDescendants(leafBlock.Inline, type.IsInstanceOfType);
            }
        }
        
        if (emitAttributesofUnmatched)
        {
            var attributes = source.TryGetAttributes();
            if (attributes is not null)
                return [attributes];
        }
        return [];
    }

    /// <summary>
    /// Searches for descendants tht match any of the specified types, ignoring nested descendants of matching markdown objects.
    /// </summary>
    /// <param name="source">The prospective parent markdown object.</param>
    /// <param name="types">The object types to search for.</param>
    /// <param name="emitAttributesofUnmatched">Whether to emit <see cref="HtmlAttributes"/> of unmatched tokens.</param>
    /// <returns>The <see cref="MarkdownObject"/>s that descend from the <paramref name="source"/> object that is an instance of any of the specified <paramref name="types"/>,
    /// except for any that have an ancestor that has already been yielded.</returns>
    public static IEnumerable<MarkdownObject> GetDescendantBranchesMatchingType(this MarkdownObject? source, IEnumerable<Type> types, bool emitAttributesofUnmatched = false)
    {
        ArgumentNullException.ThrowIfNull(types);
        if (source is null) return [];
        types = types.Where(IsNonAttributeMarkdownObjectType).CollapseTypes(out int typeCount);
        if (typeCount == 1)
        {
            Type singleType = types.First();
            if (singleType == MarkdownObjectType)
                return GetChildObjects(source, emitAttributesofUnmatched);
            if (source is ContainerBlock containerBlock)
            {
                if (emitAttributesofUnmatched)
                    return GetNestedDescendantsAndAttributes(containerBlock, singleType.IsInstanceOfType);
                return GetNestedDescendants(containerBlock, singleType.IsInstanceOfType);
            }
            if (source is ContainerInline containerInline)
            {
                if (emitAttributesofUnmatched)
                    return GetNestedDescendantsAndAttributes(containerInline, singleType.IsInstanceOfType);
                return GetNestedDescendants(containerInline, singleType.IsInstanceOfType);
            }
            if (source is LeafBlock leafBlock)
            {
                if (emitAttributesofUnmatched)
                    return GetNestedDescendantsAndAttributes(leafBlock, singleType.IsInstanceOfType);
                return (leafBlock.Inline is null) ? [] : GetNestedDescendants(leafBlock.Inline, singleType.IsInstanceOfType);
            }
        }
        else if (typeCount > 0)
        {
            if (source is ContainerBlock containerBlock)
            {
                if (emitAttributesofUnmatched)
                    return GetNestedDescendantsAndAttributes(containerBlock, obj => types.Any(t => t.IsInstanceOfType(obj)));
                return GetNestedDescendants(containerBlock, obj => types.Any(t => t.IsInstanceOfType(obj)));
            }
            if (source is ContainerInline containerInline)
            {
                if (emitAttributesofUnmatched)
                    return GetNestedDescendantsAndAttributes(containerInline, obj => types.Any(t => t.IsInstanceOfType(obj)));
                return GetNestedDescendants(containerInline, obj => types.Any(t => t.IsInstanceOfType(obj)));
            }
            if (source is LeafBlock leafBlock)
            {
                if (emitAttributesofUnmatched)
                    return GetNestedDescendantsAndAttributes(leafBlock, obj => types.Any(t => t.IsInstanceOfType(obj)));
                return (leafBlock.Inline is null) ? [] : GetNestedDescendants(leafBlock.Inline, obj => types.Any(t => t.IsInstanceOfType(obj)));
            }
        }

        if (emitAttributesofUnmatched)
        {
            var attributes = source.TryGetAttributes();
            if (attributes is not null)
                return [attributes];
        }
        return [];
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
    public static IEnumerable<MarkdownObject> GetDescendantBranchesMatchingType(this MarkdownObject? source, Type type, int maximumDepth, bool emitAttributesofUnmatched = false)
    {
        ArgumentNullException.ThrowIfNull(type);
        if (source is null || maximumDepth < 1) return [];
        if (type == MarkdownObjectType)
            return GetChildObjects(source, emitAttributesofUnmatched);
        if (type.IsNonAttributeMarkdownObjectType())
        {
            if (source is ContainerBlock containerBlock)
            {
                if (emitAttributesofUnmatched)
                    return GetNestedDescendantsAndAttributesToDepth(containerBlock, maximumDepth, type.IsInstanceOfType);
                return GetNestedDescendantsToDepth(containerBlock, maximumDepth, type.IsInstanceOfType);
            }
            if (source is ContainerInline containerInline)
            {
                if (emitAttributesofUnmatched)
                    return GetNestedDescendantsAndAttributesToDepth(containerInline, maximumDepth, type.IsInstanceOfType);
                return GetNestedDescendantsToDepth(containerInline, maximumDepth, type.IsInstanceOfType);
            }
            if (source is LeafBlock leafBlock)
            {
                if (emitAttributesofUnmatched)
                    return GetNestedDescendantsAndAttributesToDepth(leafBlock, maximumDepth, type.IsInstanceOfType);
                return GetNestedDescendantsToDepth(leafBlock, maximumDepth, type.IsInstanceOfType);
            }
        }

        if (emitAttributesofUnmatched)
        {
            var attributes = source.TryGetAttributes();
            if (attributes is not null)
                return [attributes];
        }
        return [];
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
    public static IEnumerable<MarkdownObject> GetDescendantBranchesMatchingType(this MarkdownObject? source, IEnumerable<Type> types, int maximumDepth, bool emitAttributesofUnmatched = false)
    {
        ArgumentNullException.ThrowIfNull(types);
        if (source is null || maximumDepth < 1) return [];
        types = types.Where(IsNonAttributeMarkdownObjectType).CollapseTypes(out int typeCount);
        if (typeCount == 0) return [];
        if (typeCount == 1)
        {
            Type type = types.First();
            if (type == MarkdownObjectType)
                return GetChildObjects(source, emitAttributesofUnmatched);
            if (source is ContainerBlock containerBlock)
            {
                if (emitAttributesofUnmatched)
                    return GetNestedDescendantsAndAttributesToDepth(containerBlock, maximumDepth, type.IsInstanceOfType);
                return GetNestedDescendantsToDepth(containerBlock, maximumDepth, type.IsInstanceOfType);
            }
            if (source is ContainerInline containerInline)
            {
                if (emitAttributesofUnmatched)
                    return GetNestedDescendantsAndAttributesToDepth(containerInline, maximumDepth, type.IsInstanceOfType);
                return GetNestedDescendantsToDepth(containerInline, maximumDepth, type.IsInstanceOfType);
            }
            if (source is LeafBlock leafBlock)
            {
                if (emitAttributesofUnmatched)
                    return GetNestedDescendantsAndAttributesToDepth(leafBlock, maximumDepth, type.IsInstanceOfType);
                return GetNestedDescendantsToDepth(leafBlock, maximumDepth, type.IsInstanceOfType);
            }
        }
        else
        {
            if (source is ContainerBlock containerBlock)
            {
                if (emitAttributesofUnmatched)
                    return GetNestedDescendantsAndAttributesToDepth(containerBlock, maximumDepth, obj => types.Any(t => t.IsInstanceOfType(obj)));
                return GetNestedDescendantsToDepth(containerBlock, maximumDepth, obj => types.Any(t => t.IsInstanceOfType(obj)));
            }
            if (source is ContainerInline containerInline)
            {
                if (emitAttributesofUnmatched)
                    return GetNestedDescendantsAndAttributesToDepth(containerInline, maximumDepth, obj => types.Any(t => t.IsInstanceOfType(obj)));
                return GetNestedDescendantsToDepth(containerInline, maximumDepth, obj => types.Any(t => t.IsInstanceOfType(obj)));
            }
            if (source is LeafBlock leafBlock)
            {
                if (emitAttributesofUnmatched)
                    return GetNestedDescendantsAndAttributesToDepth(leafBlock, maximumDepth, obj => types.Any(t => t.IsInstanceOfType(obj)));
                return GetNestedDescendantsToDepth(leafBlock, maximumDepth, obj => types.Any(t => t.IsInstanceOfType(obj)));
            }
        }

        if (emitAttributesofUnmatched)
        {
            var attributes = source.TryGetAttributes();
            if (attributes is not null)
                return [attributes];
        }
        return [];
    }

    /// <summary>
    /// Gets descendents which exist at a specified recursion depth.
    /// </summary>
    /// <param name="source">The prospective parent markdown object.</param>
    /// <param name="depth">The number of times to recurse into child markdown objects.</param>
    /// <returns>The <see cref="MarkdownObject"/>s that exist at the specified recursion <paramref name="depth"/>.</returns>
    /// <param name="includeAttributes">Whether to include <see cref="HtmlAttributes"/>.</param>
    /// <remarks>If <paramref name="depth"/> is less than <c>1</c>, nothing will be yielded.</remarks>
    public static IEnumerable<MarkdownObject> GetDescendantsAtDepth(this MarkdownObject? source, int depth, bool includeAttributes = false)
    {
        if (source is null || depth < 1) return [];
        throw new NotImplementedException();
    }

    /// <summary>
    /// Gets descendents which exists at or beyond a specified recursion depth.
    /// </summary>
    /// <param name="source">The prospective parent markdown object.</param>
    /// <param name="minimumDepth">The number of to recurse into child objects before yielding descendant markdown objects.</param>
    /// <param name="includeAttributes">Whether to include <see cref="HtmlAttributes"/>.</param>
    /// <returns>The <see cref="MarkdownObject"/>s that exist at or beyond the specified recursion <paramref name="minimumDepth"/>.</returns>
    /// <remarks>If <paramref name="minimumDepth"/> is less than <c>1</c>, nothing will be yielded.</remarks>
    public static IEnumerable<MarkdownObject> GetDescendantsFromDepth(this MarkdownObject? source, int minimumDepth, bool includeAttributes = false)
    {
        if (source is null) return [];
        throw new NotImplementedException();
    }

    /// <summary>
    /// Gets descendants that exist at or below the specified recursion depth.
    /// </summary>
    /// <param name="source">The prospective parent markdown object.</param>
    /// <param name="maximumDepth">The maximum number of times to recurse into nested child objects.</param>
    /// <returns>The <see cref="MarkdownObject"/>s that exist at or below the specified recursion <paramref name="maximumDepth"/>.</returns>
    /// <param name="includeAttributes">Whether to include <see cref="HtmlAttributes"/>.</param>
    /// <remarks>If <paramref name="minimumDepth"/> is less than <c>1</c>, nothing will be yielded.</remarks>
    public static IEnumerable<MarkdownObject> GetDescendantsUpToDepth(this MarkdownObject? source, int maximumDepth, bool includeAttributes = false)
    {
        if (source is null || maximumDepth < 1) return [];
        throw new NotImplementedException();
    }

    public static IEnumerable<MarkdownObject> GetDescendantsInDepthRange(this MarkdownObject? source, int minimumDepth, int maximumDepth, bool includeAttributes = false)
    {
        if (source is null || maximumDepth < 1 || maximumDepth < minimumDepth) return [];
        throw new NotImplementedException();
    }

    public static IEnumerable<MarkdownObject> GetDescendantBranchesMatchingType(this MarkdownObject? source, Type type, int minimumDepth, int maximumDepth, bool emitAttributesofUnmatched = false)
    {
        ArgumentNullException.ThrowIfNull(type);
        if (source is null || maximumDepth < 1 || maximumDepth < minimumDepth) return [];
        throw new NotImplementedException();
    }

    public static IEnumerable<MarkdownObject> GetDescendantBranchesMatchingType(this MarkdownObject? source, IEnumerable<Type> types, int minimumDepth, int maximumDepth, bool emitAttributesofUnmatched = false)
    {
        ArgumentNullException.ThrowIfNull(types);
        if (source is null || minimumDepth < 1 || maximumDepth < minimumDepth) return [];
        types = types.Where(IsNonAttributeMarkdownObjectType).CollapseTypes(out int typeCount);
        if (typeCount == 0) return [];
        throw new NotImplementedException();
    }
}