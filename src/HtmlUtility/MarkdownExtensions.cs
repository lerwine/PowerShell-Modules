using System.Collections.ObjectModel;
using System.Reflection;
using Markdig.Renderers.Html;

namespace HtmlUtility;

public static class MarkdownExtensionMethods
{
    private static readonly ReadOnlyDictionary<MarkdownTokenType, Type> _markdownTokenTypeMap;

    static MarkdownExtensionMethods()
    {
        Type t = typeof(MarkdownTokenType);
        _markdownTokenTypeMap = new(Enum.GetValues<MarkdownTokenType>().ToDictionary(k => k, v => t.GetField(v.ToString("F"))!.GetCustomAttribute<ReflectionTypeAttribute>()!.Type));
    }
    
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
    /// <returns>The <see cref="Markdig.Syntax.MarkdownObject"/>s that are direct child objects of the <paramref name="source"/> object.</returns>
    public static IEnumerable<Markdig.Syntax.MarkdownObject> GetChildObjects(this Markdig.Syntax.MarkdownObject? source)
    {
        if (source is not null)
        {
            var attributes = source.TryGetAttributes();
            if (attributes is not null)
                yield return attributes;
            if (source is Markdig.Syntax.ContainerBlock block)
                foreach (var item in block) yield return item;
            else if (source is Markdig.Syntax.Inlines.ContainerInline inline)
                foreach (var item in inline) yield return item;
            else if (source is Markdig.Syntax.LeafBlock leaf && leaf.Inline is not null)
                yield return leaf.Inline;
        }
    }

    /// <summary>
    /// Gets all descendant markdown objects.
    /// </summary>
    /// <param name="source">The prospective parent markdown object.</param>
    /// <returns>The <see cref="Markdig.Syntax.MarkdownObject"/>s that descend from the <paramref name="source"/> object.</returns>
    public static IEnumerable<Markdig.Syntax.MarkdownObject> GetAllDescendants(this Markdig.Syntax.MarkdownObject? source)
    {
        if (source is not null)
        {
            foreach (var item in GetChildObjects(source))
            {
                yield return item;
                foreach (var c in GetAllDescendants(item))
                    yield return c;
            }
        }
    }

    /// <summary>
    /// Searches for descendants that match a specified type, ignoring nested descendants of matching markdown objects.
    /// </summary>
    /// <param name="source">The prospective parent markdown object.</param>
    /// <param name="type">The object type to search for.</param>
    /// <returns>The <see cref="Markdig.Syntax.MarkdownObject"/>s that descend from the <paramref name="source"/> object that is an instance of the specified <paramref name="type"/>,
    /// except for any that have an ancestor that has already been yielded.</returns>
    public static IEnumerable<Markdig.Syntax.MarkdownObject> GetDescendantBranchesMatchingType(this Markdig.Syntax.MarkdownObject? source, Type type)
    {
        if (source is not null && type is not null)
        {
            foreach (var item in GetChildObjects(source))
            {
                if (type.IsInstanceOfType(item))
                    yield return item;
                else
                    foreach (var c in GetDescendantBranchesMatchingType(item, type))
                        yield return c;
            }
        }
    }

    /// <summary>
    /// Searches for descendants tht match any of the specified types, ignoring nested descendants of matching markdown objects.
    /// </summary>
    /// <param name="source">The prospective parent markdown object.</param>
    /// <param name="types">The object types to search for.</param>
    /// <returns>The <see cref="Markdig.Syntax.MarkdownObject"/>s that descend from the <paramref name="source"/> object that is an instance of any of the specified <paramref name="types"/>,
    /// except for any that have an ancestor that has already been yielded.</returns>
    public static IEnumerable<Markdig.Syntax.MarkdownObject> GetDescendantBranchesMatchingType(this Markdig.Syntax.MarkdownObject? source, ICollection<Type> types)
    {
        if (source is not null && types is not null && types.Count > 0)
        {
            foreach (var item in GetChildObjects(source))
            {
                if (types.Any(t => t.IsInstanceOfType(item)))
                    yield return item;
                else
                    foreach (var c in GetDescendantBranchesMatchingType(item, types))
                        yield return c;
            }
        }
    }

    /// <summary>
    /// Searches for descendants, up to a specified recursion depth, which match the specifie type, ignoring nested descendants of matching markdown objects.
    /// </summary>
    /// <param name="source">The prospective parent markdown object.</param>
    /// <param name="type">The object type to search for.</param>
    /// <param name="maximumDepth">The maximum number of times to recurse into nested child objects.</param>
    /// <returns>The <see cref="Markdig.Syntax.MarkdownObject"/>s that descend from the <paramref name="source"/> object that is an instance of the specified <paramref name="type"/>,
    /// except for any that have an ancestor that has already been yielded or are beyond the specified <paramref name="maximumDepth"/>.</returns>
    /// <remarks>If <paramref name="maximumDepth"/> is less than <c>1</c>, nothing will be yielded.</remarks>
    public static IEnumerable<Markdig.Syntax.MarkdownObject> GetDescendantBranchesMatchingType(this Markdig.Syntax.MarkdownObject? source, Type type, int maximumDepth)
    {
        if (source is not null && maximumDepth > 0 && type is not null)
        {
            if (maximumDepth == 1)
            {
                foreach (var item in GetChildObjects(source).Where(type.IsInstanceOfType))
                    yield return item;
            }
            else
            {
                maximumDepth--;
                foreach (var item in GetChildObjects(source))
                {
                    if (type.IsInstanceOfType(item))
                        yield return item;
                    else
                        foreach (var c in GetDescendantBranchesMatchingType(item, type, maximumDepth))
                            yield return c;

                }
            }
        }
    }

    /// <summary>
    /// Searches for descendants, up to a specified recursion depth, which match any of the specified types, ignoring nested descendants of matching markdown objects.
    /// </summary>
    /// <param name="source">The prospective parent markdown object.</param>
    /// <param name="types">The object types to search for.</param>
    /// <param name="maximumDepth">The maximum number of times to recurse into nested child objects.</param>
    /// <returns>The <see cref="Markdig.Syntax.MarkdownObject"/>s that descend from the <paramref name="source"/> object that is an instance of any of the specified <paramref name="types"/>,
    /// except for any that have an ancestor that has already been yielded or are beyond the specifed <paramref name="maximumDepth"/>.</returns>
    /// <remarks>If <paramref name="maximumDepth"/> is less than <c>1</c>, nothing will be yielded.</remarks>
    public static IEnumerable<Markdig.Syntax.MarkdownObject> GetDescendantBranchesMatchingType(this Markdig.Syntax.MarkdownObject? source, ICollection<Type> types, int maximumDepth)
    {
        ArgumentNullException.ThrowIfNull(types);
        if (source is not null && types is not null && maximumDepth > 0 && types.Count > 0)
        {
            if (maximumDepth == 1)
            {
                foreach (var item in GetChildObjects(source).Where(obj => types.Any(t => t.IsInstanceOfType(obj))))
                    yield return item;
            }
            else
            {
                maximumDepth--;
                foreach (var item in GetChildObjects(source))
                {
                    if (types.Any(t => t.IsInstanceOfType(item)))
                        yield return item;
                    else
                        foreach (var c in GetDescendantBranchesMatchingType(item, types, maximumDepth))
                            yield return c;

                }
            }
        }
    }

    /// <summary>
    /// Gets descendents which exist at a specified recursion depth.
    /// </summary>
    /// <param name="source">The prospective parent markdown object.</param>
    /// <param name="depth">The number of times to recurse into child markdown objects.</param>
    /// <returns>The <see cref="Markdig.Syntax.MarkdownObject"/>s that exist at the specified recursion <paramref name="depth"/>.</returns>
    /// <remarks>If <paramref name="depth"/> is less than <c>1</c>, nothing will be yielded.</remarks>
    public static IEnumerable<Markdig.Syntax.MarkdownObject> GetDescendantsAtDepth(this Markdig.Syntax.MarkdownObject? source, int depth)
    {
        if (source is not null && depth > 0)
        {
            if (depth == 1)
            {
                foreach (var item in source.GetChildObjects())
                    yield return item;
            }
            else
            {
                depth--;
                foreach (var parent in source.GetChildObjects())
                {
                    foreach (var item in GetDescendantsAtDepth(parent, depth))
                    yield return item;
                }
            }
        }
    }

    /// <summary>
    /// Gets descendents which exists at or beyond a specified recursion depth.
    /// </summary>
    /// <param name="source">The prospective parent markdown object.</param>
    /// <param name="minimumDepth">The number of to recurse into child objects before yielding descendant markdown objects.</param>
    /// <returns>The <see cref="Markdig.Syntax.MarkdownObject"/>s that exist at or beyond the specified recursion <paramref name="minimumDepth"/>.</returns>
    /// <remarks>If <paramref name="minimumDepth"/> is less than <c>1</c>, nothing will be yielded.</remarks>
    public static IEnumerable<Markdig.Syntax.MarkdownObject> GetDescendantsFromDepth(this Markdig.Syntax.MarkdownObject? source, int minimumDepth)
    {
        foreach (var parent in GetDescendantsAtDepth(source, minimumDepth))
        {
            yield return parent;
            foreach (var item in GetAllDescendants(parent))
                yield return item;
        }
    }

    /// <summary>
    /// Gets descendants that exist at or below the specified recursion depth.
    /// </summary>
    /// <param name="source">The prospective parent markdown object.</param>
    /// <param name="maximumDepth">The maximum number of times to recurse into nested child objects.</param>
    /// <returns>The <see cref="Markdig.Syntax.MarkdownObject"/>s that exist at or below the specified recursion <paramref name="maximumDepth"/>.</returns>
    /// <remarks>If <paramref name="minimumDepth"/> is less than <c>1</c>, nothing will be yielded.</remarks>
    public static IEnumerable<Markdig.Syntax.MarkdownObject> GetDescendantsUpToDepth(this Markdig.Syntax.MarkdownObject? source, int maximumDepth)
    {
        if (source is not null && maximumDepth > 0)
        {
            if (maximumDepth == 1)
            {
                foreach (var item in source.GetChildObjects())
                    yield return item;
            }
            else
            {
                maximumDepth--;
                foreach (var parent in source.GetChildObjects())
                {
                    yield return parent;
                    foreach (var item in GetDescendantsUpToDepth(source, maximumDepth))
                        yield return item;
                }
            }
        }
    }

    public static IEnumerable<Markdig.Syntax.MarkdownObject> GetDescendantsInDepthRange(this Markdig.Syntax.MarkdownObject? source, int minimumDepth, int maximumDepth)
    {
        if (source is not null && minimumDepth > 0 && (maximumDepth -= minimumDepth) >= 0)
        {
            if (maximumDepth == 0)
                foreach (var item in GetDescendantsAtDepth(source, minimumDepth))
                    yield return item;
            else
                foreach (var item in GetDescendantsAtDepth(source, minimumDepth))
                {
                    yield return item;
                    foreach (var c in item.GetDescendantsUpToDepth(maximumDepth))
                        yield return c;
                }
        }
    }

    public static IEnumerable<Markdig.Syntax.MarkdownObject> GetDescendantBranchesMatchingType(this Markdig.Syntax.MarkdownObject? source, Type type, int minimumDepth, int maximumDepth)
    {
        if (source is not null && minimumDepth > 0 && (maximumDepth -= minimumDepth) >= 0)
        {
            if (maximumDepth == 0)
                foreach (var item in GetDescendantsAtDepth(source, minimumDepth).Where(type.IsInstanceOfType))
                    yield return item;
            else
                foreach (var item in GetDescendantsAtDepth(source, minimumDepth))
                {
                    if (type.IsInstanceOfType(item))
                        yield return item;
                    else
                        foreach (var c in item.GetDescendantBranchesMatchingType(type, maximumDepth))
                            yield return c;
                }
        }
    }

    public static IEnumerable<Markdig.Syntax.MarkdownObject> GetDescendantBranchesMatchingType(this Markdig.Syntax.MarkdownObject? source, ICollection<Type> types, int minimumDepth, int maximumDepth)
    {
        if (source is not null && minimumDepth > 0 && (maximumDepth -= minimumDepth) >= 0)
        {
            if (maximumDepth == 0)
                foreach (var item in GetDescendantsAtDepth(source, minimumDepth).Where(obj => types.Any(t => t.IsInstanceOfType(obj))))
                    yield return item;
            else
                foreach (var item in GetDescendantsAtDepth(source, minimumDepth))
                {
                    if (types.Any(t => t.IsInstanceOfType(item)))
                        yield return item;
                    else
                        foreach (var c in item.GetDescendantBranchesMatchingType(types, maximumDepth))
                            yield return c;
                }
        }
    }
}