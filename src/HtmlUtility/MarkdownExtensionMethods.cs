using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Markdig.Renderers.Html;
using Markdig.Syntax;

namespace HtmlUtility;

public static partial class MarkdownExtensionMethods
{
    internal static readonly Type HtmlAttributesType = typeof(HtmlAttributes);

    private static readonly ReadOnlyDictionary<MarkdownTokenType, Type> _markdownTokenTypeMap;

    static MarkdownExtensionMethods()
    {
        Type t = typeof(MarkdownTokenType);
        _markdownTokenTypeMap = new(Enum.GetValues<MarkdownTokenType>().ToDictionary(k => k, v => t.GetField(v.ToString("F"))!.GetCustomAttribute<ReflectionTypeAttribute>()!.Type));
    }

    internal static IEnumerable<Type> CollapseTypes(this IEnumerable<Type> source, Type baseType, out int? count)
    {
        ArgumentNullException.ThrowIfNull(baseType);
        if (source is null)
        {
            count = 0;
            return [];
        }
        using IEnumerator<Type> enumerator = source.GetEnumerator();
        if (!enumerator.MoveNext())
        {
            count = 0;
            return [];
        }
        var t = enumerator.Current;
        while (t is null)
        {
            if (!enumerator.MoveNext())
            {
                count = 0;
                return [];
            }
            t = enumerator.Current;
        }
        if (t == baseType)
        {
            count = null;
            return [t];
        }
        Collection<Type> types = [t];
        count = 1;
        while (enumerator.MoveNext())
        {
            if ((t = enumerator.Current) is null)
                continue;
            if (t == baseType)
            {
                count = null;
                return [t];
            }
            if (!baseType.IsAssignableFrom(t))
                continue;
            bool shouldAppend = true;
            for (int i = 0; i < count; i++)
            {
                Type c = types[i];
                if (c.IsAssignableFrom(t))
                {
                    shouldAppend = false;
                    break;
                }
                if (t.IsAssignableFrom(c))
                {
                    shouldAppend = false;
                    types[i] = t;
                    int n = i + 1;
                    while (n < count)
                    {
                        if (t.IsAssignableFrom(types[n]))
                        {
                            types.RemoveAt(n);
                            count--;
                        }
                        else
                            n++;
                    }
                    break;
                }
            }
            if (shouldAppend)
            {
                types.Add(t);
                count++;
            }
        }
        return types;
    }

    internal static bool HasDirectDescendant(this MarkdownObject parent, [NotNullWhen(true)] out IEnumerable<MarkdownObject>? directDescendants)
    {
        if (parent is Markdig.Syntax.Inlines.ContainerInline containerInline)
        {
            if (containerInline.FirstChild is not null)
            {
                directDescendants = containerInline.AsEnumerable();
                return true;
            }
        }
        else if (parent is ContainerBlock containerBlock)
        {
            if (containerBlock.Count > 0)
            {
                directDescendants = containerBlock.AsEnumerable();
                return true;
            }
        }
        else if (parent is LeafBlock leafBlock && leafBlock.Inline is not null && leafBlock.Inline.FirstChild is not null)
        {
            directDescendants = leafBlock.Inline.AsEnumerable();
            return true;
        }
        directDescendants = null;
        return false;
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

    /// <summary>
    /// Iterates over the descendant elements of a given markdown element where the descendant matches the specified predicate.
    /// </summary>
    /// <param name="parentObject">The parent markdown object to enumerate.</param>
    /// <param name="filter">The predicate that determines which markdown objects to include in the result <see cref="IEnumerable{T}"/>.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> that contains all descendant elements of <paramref name="parentObject"/>
    /// where <paramref name="filter"/> returned <see langword="true"/>, given each descendant.</returns>
    /// <remarks><see cref="HtmlAttributes"> are not included in the result <see cref="IEnumerable{T}"/>.</remarks>
    internal static IEnumerable<MarkdownObject> GetDescendants(this MarkdownObject parentObject, Func<MarkdownObject, bool> filter) =>
        parentObject.HasDirectDescendant(out var directDescendants) ? directDescendants : [];

    /// <summary>
    /// Iterates over the descendant elements of a given markdown element where the descendant matches the specified predicate.
    /// </summary>
    /// <param name="parentObject">The parent markdown object to enumerate.</param>
    /// <param name="filter">The predicate that determines which markdown objects to include in the result <see cref="IEnumerable{T}"/>.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> that contains all descendant <see cref="MarkdownObject"/> values of <paramref name="parentObject"/>
    /// where <paramref name="filter"/> returned <see langword="true"/>, given each descendant.</returns>
    /// <remarks><see cref="HtmlAttributes"/> are not included in the result <see cref="IEnumerable{T}"/>.</remarks>
    public static IEnumerable<MarkdownObject> Descendants(this MarkdownObject parentObject, Func<MarkdownObject, bool> filter)
    {
        ArgumentNullException.ThrowIfNull(parentObject);
        ArgumentNullException.ThrowIfNull(filter);
        return parentObject.Descendants().Where(filter);
    }

    internal static IEnumerable<MarkdownObject> PrependMarkdownObject(this IEnumerable<MarkdownObject> following, MarkdownObject preceding) =>
        ((IEnumerable<MarkdownObject>)[preceding]).Concat(following);

    internal static IEnumerable<MarkdownObject> PrependMarkdownObjectWithAttributes(this IEnumerable<MarkdownObject> following, MarkdownObject preceding)
    {
        var attributes = preceding.TryGetAttributes();
        return ((IEnumerable<MarkdownObject>)((attributes is null) ? [preceding] : [preceding, attributes])).Concat(following);
    }

    internal static IEnumerable<MarkdownObject> PrependMarkdownObjectWithAttributes(this IEnumerable<MarkdownObject> following, MarkdownObject preceding, Func<MarkdownObject, bool> predicate)
    {
        var attributes = preceding.TryGetAttributes();
        return predicate(preceding) ? ((IEnumerable<MarkdownObject>)((attributes is null) ? [preceding] : [preceding, attributes])).Concat(following) :
            (attributes is null) ? following : ((IEnumerable<MarkdownObject>)[attributes]).Concat(following);
    }

    internal static IEnumerable<MarkdownObject> PrependAttributes(this IEnumerable<MarkdownObject> following, MarkdownObject preceding)
    {
        var attributes = preceding.TryGetAttributes();
        return (attributes is null) ? following : ((IEnumerable<MarkdownObject>)[attributes]).Concat(following);
    }

    /// <summary>
    /// Iterates over the descendant elements of a given markdown element, with the given markdown element itself as the first value.
    /// </summary>
    /// <param name="parentObject">The parent markdown object to enumerate.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> that contains <paramref name="parentObject"/>, followed by its all descendant elements.</returns>
    /// <remarks><see cref="HtmlAttributes"> are not included in the result <see cref="IEnumerable{T}"/>.</remarks>
    internal static IEnumerable<MarkdownObject> GetCurrentAndDescendants(this MarkdownObject parentObject) =>
         parentObject.HasDirectDescendant(out var directDescendants) ? ((IEnumerable<MarkdownObject>)[parentObject]).Concat(directDescendants) : [parentObject];

    /// <summary>
    /// Filters the given markdown object and all its descendant elements, iterating the values that match the specified predicate.
    /// </summary>
    /// <param name="parentObject">The parent markdown object to enumerate.</param>
    /// <param name="filter">The predicate that determines which markdown objects to include in the result <see cref="IEnumerable{T}"/>.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> that contains either of the following:
    /// <list type="table">
    /// <listheader>
    ///     <term>Condition</term>
    ///     <description>Result</description>
    /// </listheader>
    /// <item>
    ///     <term><paramref name="parentObject"/> is <see cref="HtmlAttributes"/></term>
    ///     <description><i>(empty)</i></description>
    /// </item>
    /// <item>
    ///     <term><paramref name="filter"/> evaluates as <see langword="true"/> for <paramref name="parentObject"/></term>
    ///     <description><paramref name="parentObject"/> followed by all descendant elements of <paramref name="parentObject"/>
    ///         where <paramref name="filter"/> returned <see langword="true"/>, given each descendant.</description>
    /// </item>
    /// <item>
    ///     <term>Otherwise</term>
    ///     <description>All descendant elements of <paramref name="parentObject"/>
    ///         where <paramref name="filter"/> returned <see langword="true"/>, given each descendant</description>
    /// </item></returns>
    internal static IEnumerable<MarkdownObject> GetCurrentAndDescendants(this MarkdownObject parentObject, Func<MarkdownObject, bool> filter) =>
        parentObject.HasDirectDescendant(out var directDescendants) ? (filter(parentObject) ? directDescendants.PrependMarkdownObject(parentObject) : directDescendants) :
        (parentObject is not HtmlAttributes && filter(parentObject)) ? [parentObject] : [];

#pragma warning disable IDE1006 // Naming Styles
    private static IEnumerable<MarkdownObject> _CurrentOrTopMostDescendants(this MarkdownObject parentObject, Func<MarkdownObject, bool> filter) =>
        filter(parentObject) ? [parentObject] : parentObject.HasDirectDescendant(out var directDescendants) ? directDescendants.SelectMany(d => _CurrentOrTopMostDescendants(d, filter)) : [];
#pragma warning restore IDE1006 // Naming Styles

    /// <summary>
    /// Iterates over the top-most elements of a given markdown element where the descendant matches the specified predicate.
    /// </summary>
    /// <param name="parentObject">The parent markdown object to enumerate.</param>
    /// <param name="filter">The predicate that determines which markdown objects to include in the result <see cref="IEnumerable{T}"/>.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> that contains all top-most <see cref="MarkdownObject"/> values of <paramref name="parentObject"/>
    /// where <paramref name="filter"/> returned <see langword="true"/>, given each descendant.</returns>
    /// <remarks>Yielded descendants are not recursed into.
    /// <para><see cref="HtmlAttributes"> are not included in the result <see cref="IEnumerable{T}"/>.</para></remarks>
    internal static IEnumerable<MarkdownObject> GetTopMostDescendants(this MarkdownObject parentObject, Func<MarkdownObject, bool> filter) =>
        parentObject.HasDirectDescendant(out var directDescendants) ? directDescendants.SelectMany(d => _CurrentOrTopMostDescendants(d, filter)) : [];

    /// <summary>
    /// Iterates over the top-most elements of a given markdown element where the descendant matches the specified predicate.
    /// </summary>
    /// <param name="parentObject">The parent markdown object to enumerate.</param>
    /// <param name="filter">The predicate that determines which markdown objects to include in the result <see cref="IEnumerable{T}"/>.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> that contains all top-most <see cref="MarkdownObject"/> values of <paramref name="parentObject"/>
    /// where <paramref name="filter"/> returned <see langword="true"/>, given each descendant.</returns>
    /// <remarks>Yielded descendants are not recursed into.
    /// <para><see cref="HtmlAttributes"/> are not included in the result <see cref="IEnumerable{T}"/>.</para></remarks>
    public static IEnumerable<MarkdownObject> TopMostDescendants(this MarkdownObject parentObject, Func<MarkdownObject, bool> filter)
    {
        ArgumentNullException.ThrowIfNull(parentObject);
        ArgumentNullException.ThrowIfNull(filter);
        return GetTopMostDescendants(parentObject, filter);
    }

    /// <summary>
    /// Filters the given markdown object and all its descendant elements, iterating the top-most values that match the specified predicate.
    /// </summary>
    /// <param name="parentObject"></param>
    /// <param name="filter"></param>
    /// <returns>An <see cref="IEnumerable{T}"/> that contains either of the following:
    /// <list type="table">
    /// <listheader>
    ///     <term>Condition</term>
    ///     <description>Result</description>
    /// </listheader>
    /// <item>
    ///     <term><paramref name="parentObject"/> is <see cref="HtmlAttributes"/></term>
    ///     <description><i>(empty)</i></description>
    /// </item>
    /// <item>
    ///     <term><paramref name="filter"/> evaluates as <see langword="true"/> for <paramref name="parentObject"/></term>
    ///     <description><paramref name="parentObject"/> only</description>
    /// </item>
    /// <item>
    ///     <term>Otherwise</term>
    ///     <description>All descendant elements of <paramref name="parentObject"/>
    ///         where <paramref name="filter"/> returned <see langword="true"/>, given each descendant.</description>
    /// </item></returns>
    internal static IEnumerable<MarkdownObject> GetCurrentOrTopMostDescendants(this MarkdownObject parentObject, Func<MarkdownObject, bool> filter) =>
        (parentObject is HtmlAttributes) ? [] : _CurrentOrTopMostDescendants(parentObject, filter);

    /// <summary>
    /// Iterates over the descendant elements of a given markdown element, including <see cref="HtmlAttributes"/>.
    /// </summary>
    /// <param name="parentObject">The parent markdown object to enumerate.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> that contains all descendant <see cref="MarkdownObject"/> values of <paramref name="parentObject"/>,
    /// including those that are <see cref="HtmlAttributes"/>.</returns>
    internal static IEnumerable<MarkdownObject> GetAttributesAndDescendants(this MarkdownObject parentObject) =>
        parentObject.HasDirectDescendant(out var directDescendants) ? directDescendants.SelectMany(GetAttributesCurrentAndDescendants).PrependAttributes(parentObject) : parentObject.GetDirectAttributes();

    /// <summary>
    /// Iterates over the descendant elements of a given markdown element, including <see cref="HtmlAttributes"/>.
    /// </summary>
    /// <param name="parentObject">The parent markdown object to enumerate.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> that contains all descendant <see cref="MarkdownObject"/> values of <paramref name="parentObject"/>,
    /// including those that are <see cref="HtmlAttributes"/>.</returns>
    public static IEnumerable<MarkdownObject> AttributesAndDescendants(this MarkdownObject parentObject)
    {
        ArgumentNullException.ThrowIfNull(parentObject);
        return GetAttributesAndDescendants(parentObject);
    }

    /// <summary>
    /// Iterates over the descendant elements of a given markdown element, including <see cref="HtmlAttributes"/>, with the given markdown element itself as the first value.
    /// </summary>
    /// <param name="parentObject">The parent markdown object to enumerate.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> that contains <paramref name="parentObject"/>, followed by all all descendant <see cref="MarkdownObject"/> values of <paramref name="parentObject"/>,
    /// including those that are <see cref="HtmlAttributes"/>.</returns>
    internal static IEnumerable<MarkdownObject> GetAttributesCurrentAndDescendants(this MarkdownObject parentObject) =>
        parentObject.HasDirectDescendant(out var directDescendants) ? directDescendants.SelectMany(GetAttributesCurrentAndDescendants).PrependMarkdownObjectWithAttributes(parentObject) :
        parentObject.GetCurrentAndDirectAttributes();

#pragma warning disable IDE1006 // Naming Styles
    private static IEnumerable<MarkdownObject> _AttributesCurrentAndDescendants(MarkdownObject parentObject, Func<MarkdownObject, bool> filter) => parentObject.HasDirectDescendant(out var directDescendants) ?
            directDescendants.SelectMany(d => _AttributesCurrentAndDescendants(d, filter)).PrependMarkdownObjectWithAttributes(parentObject, filter) : parentObject.GetCurrentAndDirectAttributes(filter);
#pragma warning restore IDE1006 // Naming Styles

    /// <summary>
    /// Iterates over the descendant elements of a given markdown element where the descendant is <see cref="HtmlAttributes"/> or it matches the specified predicate.
    /// </summary>
    /// <param name="parentObject">The parent markdown object to enumerate.</param>
    /// <param name="filter">The predicate that determines which non-attribute markdown objects to include in the result <see cref="IEnumerable{T}"/>.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> that contains all descendant <see cref="MarkdownObject"/> values of <paramref name="parentObject"/> that are <see cref="HtmlAttributes"/>
    /// or where <paramref name="filter"/> returned <see langword="true"/>, given each descendant.</returns>
    internal static IEnumerable<MarkdownObject> GetAttributesAndDescendants(this MarkdownObject parentObject, Func<MarkdownObject, bool> filter)
    {
        if (parentObject is HtmlAttributes) return [];
        return parentObject.HasDirectDescendant(out var directDescendants) ? directDescendants.SelectMany(d => _AttributesCurrentAndDescendants(d, filter)).PrependAttributes(parentObject) :
            parentObject.GetDirectAttributes();
    }

    /// <summary>
    /// Iterates over the descendant elements of a given markdown element where the descendant is <see cref="HtmlAttributes"/> or it matches the specified predicate.
    /// </summary>
    /// <param name="parentObject">The parent markdown object to enumerate.</param>
    /// <param name="filter">The predicate that determines which non-attribute markdown objects to include in the result <see cref="IEnumerable{T}"/>.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> that contains all descendant <see cref="MarkdownObject"/> values of <paramref name="parentObject"/> that are <see cref="HtmlAttributes"/>
    /// or where <paramref name="filter"/> returned <see langword="true"/>, given each descendant.</returns>
    public static IEnumerable<MarkdownObject> AttributesAndDescendants(this MarkdownObject parentObject, Func<MarkdownObject, bool> filter)
    {
        ArgumentNullException.ThrowIfNull(parentObject);
        ArgumentNullException.ThrowIfNull(filter);
        return GetAttributesAndDescendants(parentObject, filter);
    }

    /// <summary>
    /// Filters the given markdown object and all its descendant elements, iterating the values that match the specified predicate.
    /// </summary>
    /// <param name="parentObject">The parent markdown object to enumerate.</param>
    /// <param name="filter">The predicate that determines which non-attribute markdown objects to include in the result <see cref="IEnumerable{T}"/>.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> that contains either of the following:
    /// <list type="table">
    /// <listheader>
    ///     <term>Condition</term>
    ///     <description>Result</description>
    /// </listheader>
    /// <item>
    ///     <term><paramref name="parentObject"/> is <see cref="HtmlAttributes"/></term>
    ///     <description><paramref name="parentObject"/> only</description>
    /// </item>
    /// <item>
    ///     <term><paramref name="filter"/> evaluates as <see langword="true"/> for <paramref name="parentObject"/></term>
    ///     <description><paramref name="parentObject"/> followed by all its descendant elements that are <see cref="HtmlAttributes"/>
    ///     or where <paramref name="filter"/> returned <see langword="true"/>, given each descendant.</description>
    /// </item>
    /// <item>
    ///     <term>Otherwise</term>
    ///     <description>All descendant elements of <paramref name="parentObject"/> that are <see cref="HtmlAttributes"/>
    ///         or where <paramref name="filter"/> returned <see langword="true"/>, given each descendant</description>
    /// </item></returns>
    internal static IEnumerable<MarkdownObject> GetAttributesCurrentAndDescendants(this MarkdownObject parentObject, Func<MarkdownObject, bool> filter) =>
        (parentObject is HtmlAttributes) ? [parentObject] : _AttributesCurrentAndDescendants(parentObject, filter);

#pragma warning disable IDE1006 // Naming Styles
    private static IEnumerable<MarkdownObject> _AttributesAndCurrentOrTopMostDescendants(this MarkdownObject parentObject, Func<MarkdownObject, bool> filter) =>
        parentObject.HasDirectDescendant(out var directDescendants) ?
            directDescendants.SelectMany(d => _AttributesAndCurrentOrTopMostDescendants(d, filter)).PrependMarkdownObjectWithAttributes(parentObject, filter) :
            parentObject.GetCurrentAndDirectAttributes(filter);
#pragma warning restore IDE1006 // Naming Styles

    /// <summary>
    /// Iterates over the top-most elements of a given markdown element where the descendant is <see cref="HtmlAttributes"/> or it matches the specified predicate.
    /// </summary>
    /// <param name="parentObject">The parent markdown object to enumerate.</param>
    /// <param name="filter">The predicate that determines which non-attribute markdown objects to include in the result <see cref="IEnumerable{T}"/>.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> that contains all top-most <see cref="MarkdownObject"/> values of <paramref name="parentObject"/> that are <see cref="HtmlAttributes"/>
    /// or where <paramref name="filter"/> returned <see langword="true"/>, given each descendant.</returns>
    /// <remarks>Yielded descendants are not recursed into.</remarks>
    internal static IEnumerable<MarkdownObject> GetAttributesAndTopMostDescendants(this MarkdownObject parentObject, Func<MarkdownObject, bool> filter) => (parentObject is HtmlAttributes) ? [] :
        parentObject.HasDirectDescendant(out var directDescendants) ?
            directDescendants.SelectMany(d => _AttributesAndCurrentOrTopMostDescendants(d, filter)).PrependAttributes(parentObject) : parentObject.GetDirectAttributes();

    /// <summary>
    /// Iterates over the top-most elements of a given markdown element where the descendant is <see cref="HtmlAttributes"/> or it matches the specified predicate.
    /// </summary>
    /// <param name="parentObject">The parent markdown object to enumerate.</param>
    /// <param name="filter">The predicate that determines which non-attribute markdown objects to include in the result <see cref="IEnumerable{T}"/>.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> that contains all top-most <see cref="MarkdownObject"/> values of <paramref name="parentObject"/> that are <see cref="HtmlAttributes"/>
    /// or where <paramref name="filter"/> returned <see langword="true"/>, given each descendant.</returns>
    /// <remarks>Yielded descendants are not recursed into.</remarks>
    public static IEnumerable<MarkdownObject> AttributesAndTopMostDescendants(this MarkdownObject parentObject, Func<MarkdownObject, bool> filter)
    {
        ArgumentNullException.ThrowIfNull(parentObject);
        ArgumentNullException.ThrowIfNull(filter);
        return GetAttributesAndTopMostDescendants(parentObject, filter);
    }

    /// <summary>
    /// Filters the given markdown object and all its descendant elements, iterating the top-most values that are <see cref="HtmlAttributes"/> or match the specified predicate.
    /// </summary>
    /// <param name="parentObject">The parent markdown object to enumerate.</param>
    /// <param name="filter">The predicate that determines which non-attribute markdown objects to include in the result <see cref="IEnumerable{T}"/>.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> that contains either of the following:
    /// <list type="table">
    /// <listheader>
    ///     <term>Condition</term>
    ///     <description>Result</description>
    /// </listheader>
    /// <item>
    ///     <term><paramref name="parentObject"/> is <see cref="HtmlAttributes"/> or <paramref name="filter"/> evaluates as <see langword="true"/> for <paramref name="parentObject"/></term>
    ///     <description><paramref name="parentObject"/> only</description>
    /// </item>
    /// <item>
    ///     <term>Otherwise</term>
    ///     <description>All descendant elements of <paramref name="parentObject"/> that are <see cref="HtmlAttributes"/>
    ///         or the top-most descendants where <paramref name="filter"/> returned <see langword="true"/>, given that descendant</description>
    /// </item></re turns>
    internal static IEnumerable<MarkdownObject> GetAttributesAndCurrentOrTopMostDescendants(this MarkdownObject parentObject, Func<MarkdownObject, bool> filter) =>
        (parentObject is HtmlAttributes) ? [parentObject] : _AttributesAndCurrentOrTopMostDescendants(parentObject, filter);

    /// <summary>
    /// Iterates over all descendant <see cref="HtmlAttributes"> of a given markdown element.
    /// </summary>
    /// <param name="parentObject">The parent markdown object to enumerate.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> that contains all recursive descendant <see cref="HtmlAttributes"> values of <paramref name="parentObject"/>.</returns>
    internal static IEnumerable<MarkdownObject> GetDescendantAttributes(this MarkdownObject parentObject)
    {
        var attributes = parentObject.TryGetAttributes();
        if (attributes is not null)
            yield return attributes;
        foreach (var item in parentObject.Descendants())
            if ((attributes = item.TryGetAttributes()) is not null)
                yield return attributes;
    }

    /// <summary>
    /// Iterates over all descendant <see cref="HtmlAttributes"/> of a given markdown element.
    /// </summary>
    /// <param name="parentObject">The parent markdown object to enumerate.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> that contains all recursive descendant <see cref="HtmlAttributes"/> values of <paramref name="parentObject"/>.</returns>
    public static IEnumerable<MarkdownObject> DescendantAttributes(this MarkdownObject parentObject)
    {
        ArgumentNullException.ThrowIfNull(parentObject);
        return GetDescendantAttributes(parentObject);
    }

    internal static IEnumerable<MarkdownObject> GetDescendantAttributesIncludingCurrent(this MarkdownObject parentObject)
    {
        if (parentObject is HtmlAttributes)
            yield return parentObject;
        else
        {
            var attributes = parentObject.TryGetAttributes();
            if (attributes is not null)
                yield return attributes;
            foreach (var item in parentObject.Descendants())
                if ((attributes = item.TryGetAttributes()) is not null)
                    yield return attributes;
        }
    }

    internal static IEnumerable<MarkdownObject> GetCurrentAndDirectDescendants(this MarkdownObject parentObject) =>
        parentObject.HasDirectDescendant(out var directDescendants) ? directDescendants.PrependMarkdownObject(parentObject) : [parentObject];

    /// <summary>
    /// Iterates over the direct descendants of a given markdown element.
    /// </summary>
    /// <param name="parentObject">The parent markdown object to enumerate.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> that contains the direct descendants of <paramref name="parentObject"/> (depth of <c>1</c>).</returns>
    /// <remarks><see cref="HtmlAttributes"> are not included in the result <see cref="IEnumerable{T}"/>.</remarks>
    internal static IEnumerable<MarkdownObject> GetDirectDescendants(this MarkdownObject parentObject) => parentObject.HasDirectDescendant(out var directDescendants) ? directDescendants : [];

    /// <summary>
    /// Iterates over the direct descendants of a given markdown element.
    /// </summary>
    /// <param name="parentObject">The parent markdown object to enumerate.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> that contains the direct descendants of <paramref name="parentObject"/> (depth of <c>1</c>).</returns>
    /// <remarks><see cref="HtmlAttributes"/> are not included in the result <see cref="IEnumerable{T}"/>.</remarks>
    public static IEnumerable<MarkdownObject> DirectDescendants(this MarkdownObject parentObject)
    {
        ArgumentNullException.ThrowIfNull(parentObject);
        return GetDirectDescendants(parentObject);
    }

    /// <summary>
    /// Iterates over the direct descendants of a given markdown element where the descendant matches the specified predicate.
    /// </summary>
    /// <param name="parentObject">The parent markdown object to enumerate.</param>
    /// <param name="filter">The predicate that determines which markdown objects to include in the result <see cref="IEnumerable{T}"/>.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> that contains the direct descendants of <paramref name="parentObject"/> (depth of <c>1</c>)
    /// where <paramref name="filter"/> returned <see langword="true"/>, given each descendant.</returns>
    /// <remarks><see cref="HtmlAttributes"> are not included in the result <see cref="IEnumerable{T}"/>.</remarks>
    internal static IEnumerable<MarkdownObject> GetDirectDescendants(this MarkdownObject parentObject, Func<MarkdownObject, bool> filter) =>
        parentObject.HasDirectDescendant(out var directDescendants) ? directDescendants.Where(filter) : [];

    /// <summary>
    /// Iterates over the direct descendants of a given markdown element where the descendant matches the specified predicate.
    /// </summary>
    /// <param name="parentObject">The parent markdown object to enumerate.</param>
    /// <param name="filter">The predicate that determines which markdown objects to include in the result <see cref="IEnumerable{T}"/>.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> that contains the direct descendants of <paramref name="parentObject"/> (depth of <c>1</c>)
    /// where <paramref name="filter"/> returned <see langword="true"/>, given each descendant.</returns>
    /// <remarks><see cref="HtmlAttributes"/> are not included in the result <see cref="IEnumerable{T}"/>.</remarks>
    public static IEnumerable<MarkdownObject> DirectDescendants(this MarkdownObject parentObject, Func<MarkdownObject, bool> filter)
    {
        ArgumentNullException.ThrowIfNull(parentObject);
        ArgumentNullException.ThrowIfNull(filter);
        return GetDirectDescendants(parentObject, filter);
    }

    internal static IEnumerable<MarkdownObject> GetCurrentAndDirectDescendants(this MarkdownObject parentObject, Func<MarkdownObject, bool> filter) =>
        (parentObject is HtmlAttributes) ? [] : parentObject.HasDirectDescendant(out var directDescendants) ?
        (filter(parentObject) ? directDescendants.PrependMarkdownObject(parentObject) : directDescendants) :
        filter(parentObject) ? [parentObject] : [];

    internal static IEnumerable<MarkdownObject> GetCurrentOrDirectDescendants(this MarkdownObject parentObject, Func<MarkdownObject, bool> filter) =>
        (parentObject is HtmlAttributes) ? [] : filter(parentObject) ? [parentObject] : parentObject.HasDirectDescendant(out var directDescendants) ? directDescendants : [];

    /// <summary>
    /// Iterates over the direct descendants of a given markdown element, including <see cref="HtmlAttributes">.
    /// </summary>
    /// <param name="parentObject">The parent markdown object to enumerate.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> that contains the direct <see cref="HtmlAttributes"/> of <paramref name="parentObject"/> if present,
    /// plus the direct descendants of <paramref name="parentObject"/> (depth of <c>1</c>).</returns>
    internal static IEnumerable<MarkdownObject> GetAttributesAndDirectDescendants(this MarkdownObject parentObject) => parentObject.HasDirectDescendant(out var directDescendants) ?
        directDescendants.PrependAttributes(parentObject) : parentObject.GetDirectAttributes();

    internal static IEnumerable<MarkdownObject> GetAttributesCurrentAndDirectDescendants(this MarkdownObject parentObject) =>
        parentObject.HasDirectDescendant(out var directDescendants) ?
            directDescendants.PrependMarkdownObjectWithAttributes(parentObject) :
            parentObject.GetCurrentAndDirectAttributes();

    /// <summary>
    /// Iterates over the direct descendants of a given markdown element, including <see cref="HtmlAttributes"/>.
    /// </summary>
    /// <param name="parentObject">The parent markdown object to enumerate.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> that contains the direct <see cref="HtmlAttributes"/> of <paramref name="parentObject"/> if present,
    /// plus the direct descendants of <paramref name="parentObject"/> (depth of <c>1</c>).</returns>
    public static IEnumerable<MarkdownObject> AttributesAndDirectDescendants(this MarkdownObject parentObject)
    {
        ArgumentNullException.ThrowIfNull(parentObject);
        return GetAttributesAndDirectDescendants(parentObject);
    }

    /// <summary>
    /// Iterates over the direct descendants of a given markdown element where the descendant is <see cref="HtmlAttributes"> or it matches the specified predicate.
    /// </summary>
    /// <param name="parentObject">The parent markdown object to enumerate.</param>
    /// <param name="filter">The predicate that determines which non-attribute markdown objects to include in the result <see cref="IEnumerable{T}"/>.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> that contains the direct <see cref="HtmlAttributes"/> of <paramref name="parentObject"/> if present,
    /// plus the direct descendants of <paramref name="parentObject"/> (depth of <c>1</c>) where <paramref name="filter"/> returned <see langword="true"/>, given each descendant.</returns>
    internal static IEnumerable<MarkdownObject> GetAttributesAndDirectDescendants(this MarkdownObject parentObject, Func<MarkdownObject, bool> filter) =>
        parentObject.HasDirectDescendant(out var directDescendants) ?
            directDescendants.Where(filter).PrependAttributes(parentObject) :
            parentObject.GetDirectAttributes();

    /// <summary>
    /// Iterates over the direct descendants of a given markdown element where the descendant is <see cref="HtmlAttributes"/> or it matches the specified predicate.
    /// </summary>
    /// <param name="parentObject">The parent markdown object to enumerate.</param>
    /// <param name="filter">The predicate that determines which non-attribute markdown objects to include in the result <see cref="IEnumerable{T}"/>.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> that contains the direct <see cref="HtmlAttributes"/> of <paramref name="parentObject"/> if present,
    /// plus the direct descendants of <paramref name="parentObject"/> (depth of <c>1</c>) where <paramref name="filter"/> returned <see langword="true"/>, given each descendant.</returns>
    public static IEnumerable<MarkdownObject> AttributesAndDirectDescendants(this MarkdownObject parentObject, Func<MarkdownObject, bool> filter)
    {
        ArgumentNullException.ThrowIfNull(parentObject);
        ArgumentNullException.ThrowIfNull(filter);
        return GetAttributesAndDirectDescendants(parentObject, filter);
    }

    internal static IEnumerable<MarkdownObject> GetAttributesCurrentAndDirectDescendants(this MarkdownObject parentObject, Func<MarkdownObject, bool> filter) =>
        parentObject.HasDirectDescendant(out var directDescendants) ?
            directDescendants.Where(filter).PrependMarkdownObjectWithAttributes(parentObject, filter) :
            parentObject.GetCurrentAndDirectAttributes(filter);

    internal static IEnumerable<MarkdownObject> GetAttributesCurrentOrDirectDescendants(this MarkdownObject parentObject, Func<MarkdownObject, bool> filter) =>
        (parentObject is HtmlAttributes || filter(parentObject)) ? [parentObject] :
        parentObject.HasDirectDescendant(out var directDescendants) ? ((IEnumerable<MarkdownObject>)[parentObject]).Concat(directDescendants) : [parentObject];

    internal static IEnumerable<MarkdownObject> GetDirectAttributes(this MarkdownObject parentObject)
    {
        var attributes = parentObject.TryGetAttributes();
        if (attributes is not null)
            yield return attributes;
    }

    internal static IEnumerable<MarkdownObject> GetCurrentAndDirectAttributes(this MarkdownObject parentObject)
    {
        yield return parentObject;
        var attributes = parentObject.TryGetAttributes();
        if (attributes is not null)
            yield return attributes;
    }

    internal static IEnumerable<MarkdownObject> GetCurrentAndDirectAttributes(this MarkdownObject parentObject, Func<MarkdownObject, bool> filter)
    {
        if (filter(parentObject))
            yield return parentObject;
        var attributes = parentObject.TryGetAttributes();
        if (attributes is not null)
            yield return attributes;
    }

#pragma warning disable IDE1006 // Naming Styles
    private static IEnumerable<MarkdownObject> _CurrentAndDescendantsToDepth(this MarkdownObject parentObject, int maxDepth)
#pragma warning restore IDE1006 // Naming Styles
    {
        if (parentObject.HasDirectDescendant(out var directDescendants))
        {
            if (maxDepth < 2)
                return directDescendants.PrependMarkdownObject(parentObject);
            maxDepth--;
            return directDescendants.SelectMany(d => _CurrentAndDescendantsToDepth(d, maxDepth)).PrependMarkdownObject(parentObject);
        }
        return [parentObject];
    }

    /// <summary>
    /// Iterates over the descendant elements of a given markdown element up to a specified depth.
    /// </summary>
    /// <param name="parentObject">The parent markdown object to enumerate.</param>
    /// <param name="maxDepth">The inclusive maximum depth of descendants to include, relative to the <paramref name="parentObject"/>, whereby a value of <c>1</c> is for direct descendants.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> that contains all recursive descendantements of <paramref name="parentObject"/>, up to and including the specified <paramref name="maxDepth"/>.</returns>
    /// <remarks><see cref="HtmlAttributes"> are not included in the result <see cref="IEnumerable{T}"/>.</remarks>
    internal static IEnumerable<MarkdownObject> GetDescendantsToDepth(this MarkdownObject parentObject, int maxDepth)
    {
        Debug.Assert(maxDepth > 1);
        if (parentObject.HasDirectDescendant(out var directDescendants))
        {
            maxDepth--;
            return directDescendants.SelectMany(d => _CurrentAndDescendantsToDepth(d, maxDepth));
        }
        return [];
    }

    /// <summary>
    /// Iterates over the descendant elements of a given markdown element up to a specified depth.
    /// </summary>
    /// <param name="parentObject">The parent markdown object to enumerate.</param>
    /// <param name="maxDepth">The inclusive maximum depth of descendants to include, relative to the <paramref name="parentObject"/>, whereby a value of <c>1</c> is for direct descendants.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> that contains all recursive descendantements of <paramref name="parentObject"/>, up to and including the specified <paramref name="maxDepth"/>.</returns>
    /// <remarks><see cref="HtmlAttributes"/> are not included in the result <see cref="IEnumerable{T}"/>.</remarks>
    public static IEnumerable<MarkdownObject> DescendantsToDepth(this MarkdownObject parentObject, int maxDepth)
    {
        ArgumentNullException.ThrowIfNull(parentObject);
        return (maxDepth < 1) ? [] : (maxDepth > 1) ? GetDescendantsToDepth(parentObject, maxDepth) :
            GetDirectDescendants(parentObject);
    }

    internal static IEnumerable<MarkdownObject> GetCurrentAndDescendantsToDepth(this MarkdownObject parentObject, int maxDepth)
    {
        Debug.Assert(maxDepth > 1);
        return (parentObject is HtmlAttributes) ? [] : _CurrentAndDescendantsToDepth(parentObject, maxDepth);
    }

#pragma warning disable IDE1006 // Naming Styles
    private static IEnumerable<MarkdownObject> _CurrentAndDescendantsToDepth(this MarkdownObject parentObject, int maxDepth, Func<MarkdownObject, bool> filter)
#pragma warning restore IDE1006 // Naming Styles
    {
        if (parentObject.HasDirectDescendant(out var descendants))
        {
            if (maxDepth < 2)
                return filter(parentObject) ? descendants.Where(filter).PrependMarkdownObject(parentObject) : descendants.Where(filter);
            maxDepth--;
            descendants = descendants.SelectMany(d => _CurrentAndDescendantsToDepth(d, maxDepth, filter));
            return filter(parentObject) ? descendants.PrependMarkdownObject(parentObject) : descendants;
        }
        return filter(parentObject) ? [parentObject] : [];
    }

    /// <summary>
    /// Iterates over the descendant elements of a given markdown element, up to a specified depth, where the descendant matches the specified predicate.
    /// </summary>
    /// <param name="parentObject">The parent markdown object to enumerate.</param>
    /// <param name="maxDepth">The inclusive maximum depth of descendants to include, relative to the <paramref name="parentObject"/>, whereby a value of <c>1</c> is for direct descendants.</param>
    /// <param name="filter">The predicate that determines which markdown objects to include in the result <see cref="IEnumerable{T}"/>.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> that contains all recursive descendant elements of <paramref name="parentObject"/>, up to and including the specified <paramref name="maxDepth"/>,
    /// where <paramref name="filter"/> returned <see langword="true"/>, given each descendant.</returns>
    /// <remarks><see cref="HtmlAttributes"> are not included in the result <see cref="IEnumerable{T}"/>.</remarks>
    internal static IEnumerable<MarkdownObject> GetDescendantsToDepth(this MarkdownObject parentObject, int maxDepth, Func<MarkdownObject, bool> filter)
    {
        Debug.Assert(maxDepth > 1);
        if (parentObject.HasDirectDescendant(out var directDescendants))
        {
            maxDepth--;
            return directDescendants.SelectMany(d => _CurrentAndDescendantsToDepth(d, maxDepth, filter));
        }
        return [];
    }

    /// <summary>
    /// Iterates over the descendant elements of a given markdown element, up to a specified depth, where the descendant matches the specified predicate.
    /// </summary>
    /// <param name="parentObject">The parent markdown object to enumerate.</param>
    /// <param name="maxDepth">The inclusive maximum depth of descendants to include, relative to the <paramref name="parentObject"/>, whereby a value of <c>1</c> is for direct descendants.</param>
    /// <param name="filter">The predicate that determines which markdown objects to include in the result <see cref="IEnumerable{T}"/>.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> that contains all recursive descendant elements of <paramref name="parentObject"/>, up to and including the specified <paramref name="maxDepth"/>,
    /// where <paramref name="filter"/> returned <see langword="true"/>, given each descendant.</returns>
    /// <remarks><see cref="HtmlAttributes"/> are not included in the result <see cref="IEnumerable{T}"/>.</remarks>
    public static IEnumerable<MarkdownObject> DescendantsToDepth(this MarkdownObject parentObject, int maxDepth, Func<MarkdownObject, bool> filter)
    {
        ArgumentNullException.ThrowIfNull(parentObject);
        ArgumentNullException.ThrowIfNull(filter);
        return (maxDepth < 1) ? [] : (maxDepth > 1) ? GetDescendantsToDepth(parentObject, maxDepth, filter) :
            GetDirectDescendants(parentObject, filter);
    }

    internal static IEnumerable<MarkdownObject> GetCurrentAndDescendantsToDepth(this MarkdownObject parentObject, int maxDepth, Func<MarkdownObject, bool> filter)
    {
        Debug.Assert(maxDepth > 1);
        return (parentObject is HtmlAttributes) ? [] : _CurrentAndDescendantsToDepth(parentObject, maxDepth, filter);
    }

#pragma warning disable IDE1006 // Naming Styles
    private static IEnumerable<MarkdownObject> _CurrentOrTopMostDescendantsToDepth(this MarkdownObject parentObject, int maxDepth, Func<MarkdownObject, bool> filter)
#pragma warning restore IDE1006 // Naming Styles
    {
        if (filter(parentObject)) return [parentObject];
        if (parentObject.HasDirectDescendant(out var descendants))
        {
            if (maxDepth < 2)
                return descendants.Where(filter);
            maxDepth--;
            return descendants.SelectMany(d => _CurrentOrTopMostDescendantsToDepth(d, maxDepth, filter));
        }
        return filter(parentObject) ? [parentObject] : [];
    }

    /// <summary>
    /// Iterates over the top-most elements of a given markdown element, up to a specified depth, where the descendant matches the specified predicate.
    /// </summary>
    /// <param name="parentObject">The parent markdown object to enumerate.</param>
    /// <param name="maxDepth">The inclusive maximum depth of descendants to include, relative to the <paramref name="parentObject"/>, whereby a value of <c>1</c> is for direct descendants.</param>
    /// <param name="filter">The predicate that determines which markdown objects to include in the result <see cref="IEnumerable{T}"/>.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> that contains all top-most descendant elements of <paramref name="parentObject"/>, up to and including the specified <paramref name="maxDepth"/>,
    /// where <paramref name="filter"/> returned <see langword="true"/>, given each descendant.</returns>
    /// <remarks>Yielded descendants are not recursed into.
    /// <para><see cref="HtmlAttributes"> are not included in the result <see cref="IEnumerable{T}"/>.</para></remarks>
    internal static IEnumerable<MarkdownObject> GetTopMostDescendantsToDepth(this MarkdownObject parentObject, int maxDepth, Func<MarkdownObject, bool> filter)
    {
        Debug.Assert(maxDepth > 1);
        if (parentObject.HasDirectDescendant(out var directDescendants))
        {
            maxDepth--;
            return directDescendants.SelectMany(d => _CurrentOrTopMostDescendantsToDepth(d, maxDepth, filter));
        }
        return [];
    }

    /// <summary>
    /// Iterates over the top-most elements of a given markdown element, up to a specified depth, where the descendant matches the specified predicate.
    /// </summary>
    /// <param name="parentObject">The parent markdown object to enumerate.</param>
    /// <param name="maxDepth">The inclusive maximum depth of descendants to include, relative to the <paramref name="parentObject"/>, whereby a value of <c>1</c> is for direct descendants.</param>
    /// <param name="filter">The predicate that determines which markdown objects to include in the result <see cref="IEnumerable{T}"/>.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> that contains all top-most descendant elements of <paramref name="parentObject"/>, up to and including the specified <paramref name="maxDepth"/>,
    /// where <paramref name="filter"/> returned <see langword="true"/>, given each descendant.</returns>
    /// <remarks>Yielded descendants are not recursed into.
    /// <para><see cref="HtmlAttributes"/> are not included in the result <see cref="IEnumerable{T}"/>.</para></remarks>
    public static IEnumerable<MarkdownObject> TopMostDescendantsToDepth(this MarkdownObject parentObject, int maxDepth, Func<MarkdownObject, bool> filter)
    {
        ArgumentNullException.ThrowIfNull(parentObject);
        ArgumentNullException.ThrowIfNull(filter);
        return (maxDepth < 1) ? [] : (maxDepth > 1) ? GetTopMostDescendantsToDepth(parentObject, maxDepth, filter) :
            GetDirectDescendants(parentObject, filter);
    }

    internal static IEnumerable<MarkdownObject> GetCurrentOrTopMostDescendantsToDepth(this MarkdownObject parentObject, int maxDepth, Func<MarkdownObject, bool> filter)
    {
        Debug.Assert(maxDepth > 1);
        return (parentObject is HtmlAttributes) ? [] : _CurrentOrTopMostDescendantsToDepth(parentObject, maxDepth, filter);
    }

#pragma warning disable IDE1006 // Naming Styles
    private static IEnumerable<MarkdownObject> _AttributesCurrentAndDescendantsToDepth(this MarkdownObject parentObject, int maxDepth)
#pragma warning restore IDE1006 // Naming Styles
    {
        if (parentObject.HasDirectDescendant(out var directDescendants))
        {
            if (maxDepth < 2)
                return directDescendants.PrependMarkdownObjectWithAttributes(parentObject);
            maxDepth--;
            return directDescendants.SelectMany(d => _AttributesCurrentAndDescendantsToDepth(d, maxDepth)).PrependMarkdownObjectWithAttributes(parentObject);
        }
        return parentObject.GetCurrentAndDirectAttributes();
    }

    /// <summary>
    /// Iterates over the descendant elements of a given markdown element, including <see cref="HtmlAttributes">, up to a specified depth.
    /// </summary>
    /// <param name="parentObject">The parent markdown object to enumerate.</param>
    /// <param name="maxDepth">The inclusive maximum depth of descendants to include, relative to the <paramref name="parentObject"/>, whereby a value of <c>1</c> is for direct descendants.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> that contains all top-most descendant elements of <paramref name="parentObject"/>, up to and including the specified <paramref name="maxDepth"/>,
    /// including <see cref="HtmlAttributes">.</returns>
    internal static IEnumerable<MarkdownObject> GetAttributesAndDescendantsToDepth(this MarkdownObject parentObject, int maxDepth)
    {
        Debug.Assert(maxDepth > 1);
        if (parentObject.HasDirectDescendant(out var directDescendants))
        {
            maxDepth--;
            return directDescendants.SelectMany(d => _AttributesCurrentAndDescendantsToDepth(d, maxDepth)).PrependAttributes(parentObject);
        }
        return parentObject.GetDirectAttributes();
    }

    /// <summary>
    /// Iterates over the descendant elements of a given markdown element, including <see cref="HtmlAttributes"/>, up to a specified depth.
    /// </summary>
    /// <param name="parentObject">The parent markdown object to enumerate.</param>
    /// <param name="maxDepth">The inclusive maximum depth of descendants to include, relative to the <paramref name="parentObject"/>, whereby a value of <c>1</c> is for direct descendants.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> that contains all descendant elements of <paramref name="parentObject"/>, up to and including the specified <paramref name="maxDepth"/>,
    /// including <see cref="HtmlAttributes"/>.</returns>
    public static IEnumerable<MarkdownObject> AttributesAndDescendantsToDepth(this MarkdownObject parentObject, int maxDepth)
    {
        ArgumentNullException.ThrowIfNull(parentObject);
        return (maxDepth < 1) ? [] : (maxDepth > 1) ? GetAttributesAndDescendantsToDepth(parentObject, maxDepth) :
            GetAttributesAndDirectDescendants(parentObject);
    }

    internal static IEnumerable<MarkdownObject> GetAttributesCurrentAndDescendantsToDepth(this MarkdownObject parentObject, int maxDepth)
    {
        Debug.Assert(maxDepth > 1);
        return (parentObject is HtmlAttributes) ? [parentObject] : _AttributesCurrentAndDescendantsToDepth(parentObject, maxDepth);
    }

#pragma warning disable IDE1006 // Naming Styles
    private static IEnumerable<MarkdownObject> _AttributesCurrentAndDescendantsToDepth(this MarkdownObject parentObject, int maxDepth, Func<MarkdownObject, bool> filter)
#pragma warning restore IDE1006 // Naming Styles
    {
        if (parentObject.HasDirectDescendant(out var descendants))
        {
            if (maxDepth < 2)
                return descendants.Where(filter).PrependMarkdownObjectWithAttributes(parentObject, filter);
            maxDepth--;
            return descendants.SelectMany(d => _CurrentAndDescendantsToDepth(d, maxDepth, filter)).PrependMarkdownObjectWithAttributes(parentObject, filter);
        }
        return filter(parentObject) ? parentObject.GetCurrentAndDirectAttributes() : parentObject.GetDirectAttributes();
    }

    /// <summary>
    /// Iterates over the descendant elements of a given markdown element, including <see cref="HtmlAttributes">, up to a specified depth, where the descendant matches the specified predicate.
    /// </summary>
    /// <param name="parentObject">The parent markdown object to enumerate.</param>
    /// <param name="maxDepth">The inclusive maximum depth of descendants to include, relative to the <paramref name="parentObject"/>, whereby a value of <c>1</c> is for direct descendants.</param>
    /// <param name="filter">The predicate that determines which non-attribute markdown objects to include in the result <see cref="IEnumerable{T}"/>.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> that contains all top-most descendant elements of <paramref name="parentObject"/>, up to and including the specified <paramref name="maxDepth"/>,
    /// that are <see cref="HtmlAttributes"> or where <paramref name="filter"/> returned <see langword="true"/>, given each descendant.</returns>
    internal static IEnumerable<MarkdownObject> GetAttributesAndDescendantsToDepth(this MarkdownObject parentObject, int maxDepth, Func<MarkdownObject, bool> filter)
    {
        Debug.Assert(maxDepth > 1);
        if (parentObject.HasDirectDescendant(out var directDescendants))
        {
            maxDepth--;
            return directDescendants.SelectMany(d => _AttributesCurrentAndDescendantsToDepth(d, maxDepth, filter)).PrependAttributes(parentObject);
        }
        return parentObject.GetDirectAttributes();
    }

    /// <summary>
    /// Iterates over the descendant elements of a given markdown element, including <see cref="HtmlAttributes"/>, up to a specified depth, where the descendant matches the specified predicate.
    /// </summary>
    /// <param name="parentObject">The parent markdown object to enumerate.</param>
    /// <param name="maxDepth">The inclusive maximum depth of descendants to include, relative to the <paramref name="parentObject"/>, whereby a value of <c>1</c> is for direct descendants.</param>
    /// <param name="filter">The predicate that determines which non-attribute markdown objects to include in the result <see cref="IEnumerable{T}"/>.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> that contains all descendant elements of <paramref name="parentObject"/>, up to and including the specified <paramref name="maxDepth"/>,
    /// that are <see cref="HtmlAttributes"/> or where <paramref name="filter"/> returned <see langword="true"/>, given each descendant.</returns>
    public static IEnumerable<MarkdownObject> AttributesAndDescendantsToDepth(this MarkdownObject parentObject, int maxDepth, Func<MarkdownObject, bool> filter)
    {
        ArgumentNullException.ThrowIfNull(parentObject);
        ArgumentNullException.ThrowIfNull(filter);
        return (maxDepth < 1) ? [] : (maxDepth > 1) ? GetAttributesAndDescendantsToDepth(parentObject, maxDepth, filter) :
            GetAttributesAndDirectDescendants(parentObject, filter);
    }

    internal static IEnumerable<MarkdownObject> GetAttributesCurrentAndDescendantsToDepth(this MarkdownObject parentObject, int maxDepth, Func<MarkdownObject, bool> filter)
    {
        Debug.Assert(maxDepth > 1);
        return (parentObject is HtmlAttributes) ? [parentObject] : _AttributesCurrentAndDescendantsToDepth(parentObject, maxDepth, filter);
    }

#pragma warning disable IDE1006 // Naming Styles
    private static IEnumerable<MarkdownObject> _AttributesAndCurrentOrTopMostDescendantsToDepth(this MarkdownObject parentObject, int maxDepth, Func<MarkdownObject, bool> filter)
#pragma warning restore IDE1006 // Naming Styles
    {
        if (filter(parentObject)) return [parentObject];
        if (parentObject.HasDirectDescendant(out var descendants))
        {
            if (maxDepth < 2)
                return descendants.Where(filter).PrependAttributes(parentObject);
            maxDepth--;
            return descendants.SelectMany(d => _CurrentAndDescendantsToDepth(d, maxDepth, filter)).PrependAttributes(parentObject);
        }
        return parentObject.GetDirectAttributes();
    }

    internal static IEnumerable<MarkdownObject> GetAttributesAndTopMostDescendantsToDepth(this MarkdownObject parentObject, int maxDepth, Func<MarkdownObject, bool> filter)
    {
        Debug.Assert(maxDepth > 1);
        if (parentObject.HasDirectDescendant(out var directDescendants))
        {
            maxDepth--;
            return directDescendants.SelectMany(d => _AttributesAndCurrentOrTopMostDescendantsToDepth(d, maxDepth, filter)).PrependAttributes(parentObject);
        }
        return parentObject.GetDirectAttributes();
    }

    /// <summary>
    /// Iterates over the top-most elements of a given markdown element, including <see cref="HtmlAttributes"/>, up to a specified depth, where the descendant matches the specified predicate.
    /// </summary>
    /// <param name="parentObject">The parent markdown object to enumerate.</param>
    /// <param name="maxDepth">The inclusive maximum depth of descendants to include, relative to the <paramref name="parentObject"/>, whereby a value of <c>1</c> is for direct descendants.</param>
    /// <param name="filter">The predicate that determines which non-attribute markdown objects to include in the result <see cref="IEnumerable{T}"/>.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> that contains all top-most descendant elements of <paramref name="parentObject"/>, up to and including the specified <paramref name="maxDepth"/>,
    /// that are <see cref="HtmlAttributes"/> or where <paramref name="filter"/> returned <see langword="true"/>, given each descendant.</returns>
    /// <remarks>Yielded descendants are not recursed into.
    /// <para><see cref="HtmlAttributes"/> are not included in the result <see cref="IEnumerable{T}"/>.</para></remarks>
    public static IEnumerable<MarkdownObject> AttributesAndTopMostDescendantsToDepth(this MarkdownObject parentObject, int maxDepth, Func<MarkdownObject, bool> filter)
    {
        ArgumentNullException.ThrowIfNull(parentObject);
        ArgumentNullException.ThrowIfNull(filter);
        return (maxDepth < 1) ? [] : (maxDepth > 1) ? GetAttributesAndTopMostDescendantsToDepth(parentObject, maxDepth, filter) :
            GetAttributesAndDirectDescendants(parentObject, filter);
    }

    internal static IEnumerable<MarkdownObject> GetAttributesAndCurrentOrTopMostDescendantsToDepth(this MarkdownObject parentObject, int maxDepth, Func<MarkdownObject, bool> filter)
    {
        Debug.Assert(maxDepth > 1);
        return (parentObject is HtmlAttributes) ? [parentObject] : _AttributesAndCurrentOrTopMostDescendantsToDepth(parentObject, maxDepth, filter);
    }

    internal static IEnumerable<MarkdownObject> GetAttributesToDepth(this MarkdownObject parentObject, int maxDepth)
    {
        Debug.Assert(maxDepth > 1);
        var attributes = parentObject.TryGetAttributes();
        if (attributes is not null)
            yield return attributes;
        foreach (var item in (maxDepth > 2) ? parentObject.GetDescendantsToDepth(maxDepth - 1) : parentObject.GetDirectDescendants())
            if ((attributes = item.TryGetAttributes()) is not null)
                yield return attributes;
    }

    /// <summary>
    /// Iterates over all descendant <see cref="HtmlAttributes"/> up to a specified depth.
    /// </summary>
    /// <param name="parentObject">The parent markdown object to enumerate.</param>
    /// <param name="maxDepth">The inclusive maximum depth of descendants to include, relative to the <paramref name="parentObject"/>, whereby a value of <c>1</c> is for direct descendants.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> that contains all descendant <see cref="HtmlAttributes"/> of <paramref name="parentObject"/>,
    /// up to and including the specified <paramref name="maxDepth"/>.</returns>
    public static IEnumerable<MarkdownObject> AttributesToDepth(this MarkdownObject parentObject, int maxDepth)
    {
        ArgumentNullException.ThrowIfNull(parentObject);
        if (maxDepth < 1) return [];
        if (maxDepth > 1) return GetAttributesToDepth(parentObject, maxDepth);
        var attributes = parentObject.TryGetAttributes();
        return (attributes is null) ? [] : [attributes];
    }

    internal static IEnumerable<MarkdownObject> GetAttributesToDepthIncludingCurrent(this MarkdownObject parentObject, int maxDepth)
    {
        Debug.Assert(maxDepth > 1);
        if (parentObject is HtmlAttributes)
            yield return parentObject;
        else
        {
            var attributes = parentObject.TryGetAttributes();
            if (attributes is not null)
                yield return attributes;
            foreach (var item in (maxDepth > 2) ? parentObject.GetDescendantsToDepth(maxDepth - 1) : parentObject.GetDirectDescendants())
                if ((attributes = item.TryGetAttributes()) is not null)
                    yield return attributes;
        }
    }

    /// <summary>
    /// Iterates over the descendant elements of a given markdown element that exist at a specified depth.
    /// </summary>
    /// <param name="parentObject">The parent markdown object to enumerate.</param>
    /// <param name="depth">The depth at which to include descendants, relative to the <paramref name="parentObject"/>, whereby a value of <c>1</c> is for direct descendants.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> that contains descendant elements of <paramref name="parentObject"/> that exist at the specified <paramref name="depth"/>.</returns>
    /// <remarks><see cref="HtmlAttributes"> are not included in the result <see cref="IEnumerable{T}"/>.</remarks>
    internal static IEnumerable<MarkdownObject> GetDescendantsAtDepth(this MarkdownObject parentObject, int depth)
    {
        Debug.Assert(depth > 1);
        if (parentObject.HasDirectDescendant(out var directDescendants))
        {
            if (depth > 2)
            {
                depth--;
                return directDescendants.SelectMany(d => GetDescendantsAtDepth(d, depth));
            }
            return directDescendants.SelectMany(d => d.GetDirectDescendants());
        }
        return [];
    }

    /// <summary>
    /// Iterates over the descendant elements of a given markdown element that exist at a specified depth.
    /// </summary>
    /// <param name="parentObject">The parent markdown object to enumerate.</param>
    /// <param name="depth">The depth at which to include descendants, relative to the <paramref name="parentObject"/>, whereby a value of <c>1</c> is for direct descendants.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> that contains descendant elements of <paramref name="parentObject"/> that exist at the specified <paramref name="depth"/>.</returns>
    /// <remarks><see cref="HtmlAttributes"/> are not included in the result <see cref="IEnumerable{T}"/>.</remarks>
    public static IEnumerable<MarkdownObject> DescendantsAtDepth(this MarkdownObject parentObject, int depth)
    {
        ArgumentNullException.ThrowIfNull(parentObject);
        return (depth < 1) ? [] : (depth > 1) ? GetDescendantsAtDepth(parentObject, depth) : GetDirectDescendants(parentObject);
    }

    /// <summary>
    /// Iterates over the descendant elements of a given markdown element that exist at a specified depth, where the descendant matches the specified predicate.
    /// </summary>
    /// <param name="parentObject">The parent markdown object to enumerate.</param>
    /// <param name="depth">The depth at which to include descendants, relative to the <paramref name="parentObject"/>, whereby a value of <c>1</c> is for direct descendants.</param>
    /// <param name="filter">The predicate that determines which markdown objects to include in the result <see cref="IEnumerable{T}"/>.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> that contains descendant elements of <paramref name="parentObject"/> that exist at the specified <paramref name="depth"/>,
    /// where <paramref name="filter"/> returned <see langword="true"/>, given each descendant.</returns>
    /// <remarks><see cref="HtmlAttributes"> are not included in the result <see cref="IEnumerable{T}"/>.</remarks>
    internal static IEnumerable<MarkdownObject> GetDescendantsAtDepth(this MarkdownObject parentObject, int depth, Func<MarkdownObject, bool> filter)
    {
        Debug.Assert(depth > 1);
        if (parentObject.HasDirectDescendant(out var directDescendants))
        {
            if (depth > 2)
            {
                depth--;
                return directDescendants.SelectMany(d => GetDescendantsAtDepth(d, depth, filter));
            }
            return directDescendants.SelectMany(d => d.GetDirectDescendants().Where(filter));
        }
        return [];
    }

    /// <summary>
    /// Iterates over the descendant elements of a given markdown element that exist at a specified depth, where the descendant matches the specified predicate.
    /// </summary>
    /// <param name="parentObject">The parent markdown object to enumerate.</param>
    /// <param name="depth">The depth at which to include descendants, relative to the <paramref name="parentObject"/>, whereby a value of <c>1</c> is for direct descendants.</param>
    /// <param name="filter">The predicate that determines which markdown objects to include in the result <see cref="IEnumerable{T}"/>.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> that contains descendant elements of <paramref name="parentObject"/> that exist at the specified <paramref name="depth"/>,
    /// where <paramref name="filter"/> returned <see langword="true"/>, given each descendant.</returns>
    /// <remarks><see cref="HtmlAttributes"/> are not included in the result <see cref="IEnumerable{T}"/>.</remarks>
    public static IEnumerable<MarkdownObject> DescendantsAtDepth(this MarkdownObject parentObject, int depth, Func<MarkdownObject, bool> filter)
    {
        ArgumentNullException.ThrowIfNull(parentObject);
        ArgumentNullException.ThrowIfNull(filter);
        return (depth < 1) ? [] : (depth > 1) ? GetDescendantsAtDepth(parentObject, depth, filter) :
            GetDirectDescendants(parentObject, filter);
    }

    /// <summary>
    /// Iterates over the descendant elements of a given markdown element that exist at a specified depth, including <see cref="HtmlAttributes">.
    /// </summary>
    /// <param name="parentObject">The parent markdown object to enumerate.</param>
    /// <param name="depth">The depth at which to include descendants, relative to the <paramref name="parentObject"/>, whereby a value of <c>1</c> is for direct descendants.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> that contains descendant elements of <paramref name="parentObject"/> that exist at the specified <paramref name="depth"/>,
    /// including <see cref="HtmlAttributes">.</returns>
    internal static IEnumerable<MarkdownObject> GetAttributesAndDescendantsAtDepth(this MarkdownObject parentObject, int depth)
    {
        Debug.Assert(depth > 1);
        if (parentObject.HasDirectDescendant(out var directDescendants))
        {
            if (depth > 2)
            {
                depth--;
                return directDescendants.SelectMany(d => GetAttributesAndDescendantsAtDepth(d, depth));
            }
            return directDescendants.SelectMany(d => d.GetDirectDescendants().PrependAttributes(d));
        }
        return [];
    }

    /// <summary>
    /// Iterates over the descendant elements of a given markdown element that exist at a specified depth, including <see cref="HtmlAttributes"/>.
    /// </summary>
    /// <param name="parentObject">The parent markdown object to enumerate.</param>
    /// <param name="depth">The depth at which to include descendants, relative to the <paramref name="parentObject"/>, whereby a value of <c>1</c> is for direct descendants.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> that contains descendant elements of <paramref name="parentObject"/> that exist at the specified <paramref name="depth"/>,
    /// including <see cref="HtmlAttributes"/>.</returns>
    public static IEnumerable<MarkdownObject> AttributesAndDescendantsAtDepth(this MarkdownObject parentObject, int depth)
    {
        ArgumentNullException.ThrowIfNull(parentObject);
        return (depth < 1) ? [] : (depth > 1) ? GetAttributesAndDescendantsAtDepth(parentObject, depth) :
            GetAttributesAndDirectDescendants(parentObject);
    }

    /// <summary>
    /// Iterates over the descendant elements of a given markdown element that exist at a specified depth, where the descendant is <see cref="HtmlAttributes"> or matches the specified predicate.
    /// </summary>
    /// <param name="parentObject">The parent markdown object to enumerate.</param>
    /// <param name="depth">The depth at which to include descendants, relative to the <paramref name="parentObject"/>, whereby a value of <c>1</c> is for direct descendants.</param>
    /// <param name="filter">The predicate that determines which non-attribute markdown objects to include in the result <see cref="IEnumerable{T}"/>.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> that contains descendant elements of <paramref name="parentObject"/> that exist at the specified <paramref name="depth"/>,
    /// that are <see cref="HtmlAttributes"> or where <paramref name="filter"/> returned <see langword="true"/>, given each descendant.</returns>
    internal static IEnumerable<MarkdownObject> GetAttributesAndDescendantsAtDepth(this MarkdownObject parentObject, int depth, Func<MarkdownObject, bool> filter)
    {
        Debug.Assert(depth > 1);
        if (parentObject.HasDirectDescendant(out var directDescendants))
        {
            if (depth > 2)
            {
                depth--;
                return directDescendants.SelectMany(d => GetAttributesAndDescendantsAtDepth(d, depth, filter));
            }
            return directDescendants.SelectMany(d => d.GetDirectDescendants().Where(filter).PrependAttributes(d));
        }
        return [];
    }

    /// <summary>
    /// Iterates over the descendant elements of a given markdown element that exist at a specified depth, where the descendant is <see cref="HtmlAttributes"/> or matches the specified predicate.
    /// </summary>
    /// <param name="parentObject">The parent markdown object to enumerate.</param>
    /// <param name="depth">The depth at which to include descendants, relative to the <paramref name="parentObject"/>, whereby a value of <c>1</c> is for direct descendants.</param>
    /// <param name="filter">The predicate that determines which non-attribute markdown objects to include in the result <see cref="IEnumerable{T}"/>.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> that contains descendant elements of <paramref name="parentObject"/> that exist at the specified <paramref name="depth"/>,
    /// that are <see cref="HtmlAttributes"/> or where <paramref name="filter"/> returned <see langword="true"/>, given each descendant.</returns>
    public static IEnumerable<MarkdownObject> AttributesAndDescendantsAtDepth(this MarkdownObject parentObject, int depth, Func<MarkdownObject, bool> filter)
    {
        ArgumentNullException.ThrowIfNull(parentObject);
        return (depth < 1) ? [] : (depth > 1) ? GetAttributesAndDescendantsAtDepth(parentObject, depth, filter) :
            GetAttributesAndDirectDescendants(parentObject, filter);
    }

    /// <summary>
    /// Iterates over the descendant <see cref="HtmlAttributes"> of a given markdown element that exist at a specified depth.
    /// </summary>
    /// <param name="parentObject">The parent markdown object to enumerate.</param>
    /// <param name="depth">The depth at which to include descendants, relative to the <paramref name="parentObject"/>, whereby a value of <c>1</c> is for direct descendants.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> that contains descendant <see cref="HtmlAttributes"> of <paramref name="parentObject"/> that exist at the specified <paramref name="depth"/>.</returns>
    internal static IEnumerable<MarkdownObject> GetAttributesAtDepth(this MarkdownObject parentObject, int depth)
    {
        Debug.Assert(depth > 1);
        foreach (var item in (depth > 2) ? parentObject.GetDescendantsAtDepth(depth - 1) : parentObject.GetDirectDescendants())
        {
            var attributes = item.TryGetAttributes();
            if (attributes is not null)
                yield return attributes;
        }
    }

    /// <summary>
    /// Iterates over the descendant <see cref="HtmlAttributes"/> of a given markdown element that exist at a specified depth.
    /// </summary>
    /// <param name="parentObject">The parent markdown object to enumerate.</param>
    /// <param name="depth">The depth at which to include descendants, relative to the <paramref name="parentObject"/>, whereby a value of <c>1</c> is for direct descendants.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> that contains descendant <see cref="HtmlAttributes"/> of <paramref name="parentObject"/> that exist at the specified <paramref name="depth"/>.</returns>
    public static IEnumerable<MarkdownObject> AttributesAtDepth(this MarkdownObject parentObject, int depth)
    {
        ArgumentNullException.ThrowIfNull(parentObject);
        if (depth < 1) return [];
        if (depth > 1) return GetAttributesAtDepth(parentObject, depth);
        var attributes = parentObject.TryGetAttributes();
        return (attributes is null) ? [] : [attributes];
    }

    /// <summary>
    /// Iterates over the descendant elements of a given markdown element, starting from a specified depth.
    /// </summary>
    /// <param name="parentObject">The parent markdown object to enumerate.</param>
    /// <param name="minDepth">The inclusive minimum depth of descendants to include, relative to the <paramref name="parentObject"/>, whereby a value of <c>1</c> is for direct descendants.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> that contains descendant elements of <paramref name="parentObject"/> that exist at or beyond the specified <paramref name="minDepth"/>.</returns>
    /// <remarks><see cref="HtmlAttributes"> are not included in the result <see cref="IEnumerable{T}"/>.</remarks>
    internal static IEnumerable<MarkdownObject> GetDescendantsFromDepth(this MarkdownObject parentObject, int minDepth)
    {
        Debug.Assert(minDepth > 1);
        return parentObject.GetDescendantsAtDepth(minDepth).SelectMany(GetCurrentAndDescendants);
    }

    /// <summary>
    /// Iterates over the descendant elements of a given markdown element, starting from a specified depth.
    /// </summary>
    /// <param name="parentObject">The parent markdown object to enumerate.</param>
    /// <param name="minDepth">The inclusive minimum depth of descendants to include, relative to the <paramref name="parentObject"/>, whereby a value of <c>1</c> is for direct descendants.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> that contains descendant elements of <paramref name="parentObject"/> that exist at or beyond the specified <paramref name="minDepth"/>.</returns>
    /// <remarks><see cref="HtmlAttributes"/> are not included in the result <see cref="IEnumerable{T}"/>.</remarks>
    public static IEnumerable<MarkdownObject> DescendantsFromDepth(this MarkdownObject parentObject, int minDepth)
    {
        ArgumentNullException.ThrowIfNull(parentObject);
        return (minDepth < 2) ? parentObject.Descendants() : GetDescendantsFromDepth(parentObject, minDepth);
    }

    /// <summary>
    /// Iterates over the descendant elements of a given markdown element, starting from a specified depth, where the descendant matches the specified predicate.
    /// </summary>
    /// <param name="parentObject">The parent markdown object to enumerate.</param>
    /// <param name="minDepth">The inclusive minimum depth of descendants to include, relative to the <paramref name="parentObject"/>, whereby a value of <c>1</c> is for direct descendants.</param>
    /// <param name="filter">The predicate that determines which markdown objects to include in the result <see cref="IEnumerable{T}"/>.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> that contains descendant elements of <paramref name="parentObject"/> that exist at or beyond the specified <paramref name="minDepth"/>,
    /// where <paramref name="filter"/> returned <see langword="true"/>, given each descendant.</returns>
    /// <remarks><see cref="HtmlAttributes"> are not included in the result <see cref="IEnumerable{T}"/>.</remarks>
    internal static IEnumerable<MarkdownObject> GetDescendantsFromDepth(this MarkdownObject parentObject, int minDepth, Func<MarkdownObject, bool> filter)
    {
        Debug.Assert(minDepth > 1);
        return parentObject.GetDescendantsAtDepth(minDepth).SelectMany(d => d.GetCurrentAndDescendants(filter));
    }

    /// <summary>
    /// Iterates over the descendant elements of a given markdown element, starting from a specified depth, where the descendant matches the specified predicate.
    /// </summary>
    /// <param name="parentObject">The parent markdown object to enumerate.</param>
    /// <param name="minDepth">The inclusive minimum depth of descendants to include, relative to the <paramref name="parentObject"/>, whereby a value of <c>1</c> is for direct descendants.</param>
    /// <param name="filter">The predicate that determines which markdown objects to include in the result <see cref="IEnumerable{T}"/>.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> that contains descendant elements of <paramref name="parentObject"/> that exist at or beyond the specified <paramref name="minDepth"/>,
    /// where <paramref name="filter"/> returned <see langword="true"/>, given each descendant.</returns>
    /// <remarks><see cref="HtmlAttributes"/> are not included in the result <see cref="IEnumerable{T}"/>.</remarks>
    public static IEnumerable<MarkdownObject> DescendantsFromDepth(this MarkdownObject parentObject, int minDepth, Func<MarkdownObject, bool> filter)
    {
        ArgumentNullException.ThrowIfNull(parentObject);
        ArgumentNullException.ThrowIfNull(filter);
        return (minDepth < 2) ? parentObject.Descendants().Where(filter) : GetDescendantsFromDepth(parentObject, minDepth, filter);
    }

    /// <summary>
    /// Iterates over the top-most elements of a given markdown element, starting from a specified depth, where the descendant matches the specified predicate.
    /// </summary>
    /// <param name="parentObject">The parent markdown object to enumerate.</param>
    /// <param name="minDepth">The inclusive minimum depth of descendants to include, relative to the <paramref name="parentObject"/>, whereby a value of <c>1</c> is for direct descendants.</param>
    /// <param name="filter">The predicate that determines which markdown objects to include in the result <see cref="IEnumerable{T}"/>.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> that contains top-most descendants of <paramref name="parentObject"/> that exist at or beyond the specified <paramref name="minDepth"/>,
    /// where <paramref name="filter"/> returned <see langword="true"/>, given each descendant.</returns>
    /// <remarks>Yielded descendants are not recursed into.
    /// <para><see cref="HtmlAttributes"> are not included in the result <see cref="IEnumerable{T}"/>.</para></remarks>
    internal static IEnumerable<MarkdownObject> GetTopMostDescendantsFromDepth(this MarkdownObject parentObject, int minDepth, Func<MarkdownObject, bool> filter)
    {
        Debug.Assert(minDepth > 1);
        return parentObject.GetDescendantsAtDepth(minDepth).SelectMany(d => d.GetCurrentOrTopMostDescendants(filter));
    }

    /// <summary>
    /// Iterates over the top-most elements of a given markdown element, starting from a specified depth, where the descendant matches the specified predicate.
    /// </summary>
    /// <param name="parentObject">The parent markdown object to enumerate.</param>
    /// <param name="minDepth">The inclusive minimum depth of descendants to include, relative to the <paramref name="parentObject"/>, whereby a value of <c>1</c> is for direct descendants.</param>
    /// <param name="filter">The predicate that determines which markdown objects to include in the result <see cref="IEnumerable{T}"/>.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> that contains top-most descendants of <paramref name="parentObject"/> that exist at or beyond the specified <paramref name="minDepth"/>,
    /// where <paramref name="filter"/> returned <see langword="true"/>, given each descendant.</returns>
    /// <remarks>Yielded descendants are not recursed into.
    /// <para><see cref="HtmlAttributes"/> are not included in the result <see cref="IEnumerable{T}"/>.</para></remarks>
    public static IEnumerable<MarkdownObject> TopMostDescendantsFromDepth(this MarkdownObject parentObject, int minDepth, Func<MarkdownObject, bool> filter)
    {
        ArgumentNullException.ThrowIfNull(parentObject);
        ArgumentNullException.ThrowIfNull(filter);
        return (minDepth < 2) ? GetTopMostDescendants(parentObject, filter) :
            GetTopMostDescendantsFromDepth(parentObject, minDepth, filter);
    }

    /// <summary>
    /// Iterates over the descendant elements of a given markdown element, starting from a specified depth, including <see cref="HtmlAttributes"/>.
    /// </summary>
    /// <param name="parentObject">The parent markdown object to enumerate.</param>
    /// <param name="minDepth">The inclusive minimum depth of descendants to include, relative to the <paramref name="parentObject"/>, whereby a value of <c>1</c> is for direct descendants.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> that contains descendant elements of <paramref name="parentObject"/> that exist at or beyond the specified <paramref name="minDepth"/>,
    /// including <see cref="HtmlAttributes"/>.</returns>
    internal static IEnumerable<MarkdownObject> GetAttributesAndDescendantsFromDepth(this MarkdownObject parentObject, int minDepth)
    {
        Debug.Assert(minDepth > 1);
        return parentObject.GetDescendantsAtDepth(minDepth).SelectMany(GetAttributesCurrentAndDescendants);
    }

    /// <summary>
    /// Iterates over the descendant elements of a given markdown element, starting from a specified depth, including <see cref="HtmlAttributes"/>.
    /// </summary>
    /// <param name="parentObject">The parent markdown object to enumerate.</param>
    /// <param name="minDepth">The inclusive minimum depth of descendants to include, relative to the <paramref name="parentObject"/>, whereby a value of <c>1</c> is for direct descendants.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> that contains descendant elements of <paramref name="parentObject"/> that exist at or beyond the specified <paramref name="minDepth"/>,
    /// including <see cref="HtmlAttributes"/>.</returns>
    public static IEnumerable<MarkdownObject> AttributesAndDescendantsFromDepth(this MarkdownObject parentObject, int minDepth)
    {
        ArgumentNullException.ThrowIfNull(parentObject);
        return (minDepth < 2) ? GetAttributesAndDescendants(parentObject) :
            GetAttributesAndDescendantsFromDepth(parentObject, minDepth);
    }

    /// <summary>
    /// Iterates over the descendant elements of a given markdown element, starting from a specified depth, where the descendant is <see cref="HtmlAttributes"/> or matches the specified predicate.
    /// </summary>
    /// <param name="parentObject">The parent markdown object to enumerate.</param>
    /// <param name="minDepth">The inclusive minimum depth of descendants to include, relative to the <paramref name="parentObject"/>, whereby a value of <c>1</c> is for direct descendants.</param>
    /// <param name="filter">The predicate that determines which non-attribute markdown objects to include in the result <see cref="IEnumerable{T}"/>.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> that contains descendant elements of <paramref name="parentObject"/> that exist at or beyond the specified <paramref name="minDepth"/>,
    /// where the descendant is <see cref="HtmlAttributes"/> or <paramref name="filter"/> returned <see langword="true"/>, given each descendant.</returns>
    internal static IEnumerable<MarkdownObject> GetAttributesAndDescendantsFromDepth(this MarkdownObject parentObject, int minDepth, Func<MarkdownObject, bool> filter)
    {
        Debug.Assert(minDepth > 1);
        return parentObject.GetDescendantsAtDepth(minDepth).SelectMany(d => _AttributesCurrentAndDescendants(d, filter));
    }

    /// <summary>
    /// Iterates over the descendant elements of a given markdown element, starting from a specified depth, where the descendant is <see cref="HtmlAttributes"/> or matches the specified predicate.
    /// </summary>
    /// <param name="parentObject">The parent markdown object to enumerate.</param>
    /// <param name="minDepth">The inclusive minimum depth of descendants to include, relative to the <paramref name="parentObject"/>, whereby a value of <c>1</c> is for direct descendants.</param>
    /// <param name="filter">The predicate that determines which non-attribute markdown objects to include in the result <see cref="IEnumerable{T}"/>.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> that contains descendant elements of <paramref name="parentObject"/> that exist at or beyond the specified <paramref name="minDepth"/>,
    /// where the descendant is <see cref="HtmlAttributes"/> or <paramref name="filter"/> returned <see langword="true"/>, given each descendant.</returns>
    public static IEnumerable<MarkdownObject> AttributesAndDescendantsFromDepth(this MarkdownObject parentObject, int minDepth, Func<MarkdownObject, bool> filter)
    {
        ArgumentNullException.ThrowIfNull(parentObject);
        ArgumentNullException.ThrowIfNull(filter);
        return (minDepth < 2) ? GetAttributesAndDescendants(parentObject, filter) :
            GetAttributesAndDescendantsFromDepth(parentObject, minDepth, filter);
    }

    /// <summary>
    /// Iterates over the top-most elements of a given markdown element, starting from a specified depth, where the descendant is <see cref="HtmlAttributes"/> or matches the specified predicate.
    /// </summary>
    /// <param name="parentObject">The parent markdown object to enumerate.</param>
    /// <param name="minDepth">The inclusive minimum depth of descendants to include, relative to the <paramref name="parentObject"/>, whereby a value of <c>1</c> is for direct descendants.</param>
    /// <param name="filter">The predicate that determines which non-attribute markdown objects to include in the result <see cref="IEnumerable{T}"/>.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> that contains top-most descendants of <paramref name="parentObject"/> that exist at or beyond the specified <paramref name="minDepth"/>,
    /// where the descendant is <see cref="HtmlAttributes"/> or <paramref name="filter"/> returned <see langword="true"/>, given each descendant.</returns>
    /// <remarks>Yielded descendants are not recursed into.</remarks>
    internal static IEnumerable<MarkdownObject> GetAttributesAndTopMostDescendantsFromDepth(this MarkdownObject parentObject, int minDepth, Func<MarkdownObject, bool> filter)
    {
        Debug.Assert(minDepth > 1);
        return parentObject.GetDescendantsAtDepth(minDepth).SelectMany(d => _AttributesAndCurrentOrTopMostDescendants(d, filter));
    }

    /// <summary>
    /// Iterates over the top-most elements of a given markdown element, starting from a specified depth, where the descendant is <see cref="HtmlAttributes"/> or matches the specified predicate.
    /// </summary>
    /// <param name="parentObject">The parent markdown object to enumerate.</param>
    /// <param name="minDepth">The inclusive minimum depth of descendants to include, relative to the <paramref name="parentObject"/>, whereby a value of <c>1</c> is for direct descendants.</param>
    /// <param name="filter">The predicate that determines which non-attribute markdown objects to include in the result <see cref="IEnumerable{T}"/>.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> that contains top-most descendants of <paramref name="parentObject"/> that exist at or beyond the specified <paramref name="minDepth"/>,
    /// where the descendant is <see cref="HtmlAttributes"/> or <paramref name="filter"/> returned <see langword="true"/>, given each descendant.</returns>
    /// <remarks>Yielded descendants are not recursed into.</remarks>
    public static IEnumerable<MarkdownObject> AttributesAndTopMostDescendantsFromDepth(this MarkdownObject parentObject, int minDepth, Func<MarkdownObject, bool> filter)
    {
        ArgumentNullException.ThrowIfNull(parentObject);
        ArgumentNullException.ThrowIfNull(filter);
        return (minDepth < 2) ? GetAttributesAndTopMostDescendants(parentObject, filter) :
            GetAttributesAndTopMostDescendantsFromDepth(parentObject, minDepth, filter);
    }

    /// <summary>
    /// Iterates over the descendant <see cref="HtmlAttributes"/> of a given markdown element, starting from a specified depth.
    /// </summary>
    /// <param name="parentObject">The parent markdown object to enumerate.</param>
    /// <param name="minDepth">The inclusive minimum depth of descendants to include, relative to the <paramref name="parentObject"/>, whereby a value of <c>1</c> is for direct descendants.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> that contains descendant <see cref="HtmlAttributes"/> of <paramref name="parentObject"/>
    /// that exist at or beyond the specified <paramref name="minDepth"/>.</returns>
    internal static IEnumerable<MarkdownObject> GetAttributesFromDepth(this MarkdownObject parentObject, int minDepth)
    {
        Debug.Assert(minDepth > 1);
        return ((minDepth > 2) ? parentObject.GetDescendantsAtDepth(minDepth) : parentObject.GetDirectDescendants()).SelectMany(GetDescendantAttributes);
    }

    /// <summary>
    /// Iterates over the descendant <see cref="HtmlAttributes"/> of a given markdown element, starting from a specified depth.
    /// </summary>
    /// <param name="parentObject">The parent markdown object to enumerate.</param>
    /// <param name="minDepth">The inclusive minimum depth of descendants to include, relative to the <paramref name="parentObject"/>, whereby a value of <c>1</c> is for direct descendants.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> that contains descendant <see cref="HtmlAttributes"/> of <paramref name="parentObject"/>
    /// that exist at or beyond the specified <paramref name="minDepth"/>.</returns>
    public static IEnumerable<MarkdownObject> AttributesFromDepth(this MarkdownObject parentObject, int minDepth)
    {
        ArgumentNullException.ThrowIfNull(parentObject);
        return (minDepth < 2) ? GetDescendantAttributes(parentObject) : GetAttributesFromDepth(parentObject, minDepth);
    }

    /// <summary>
    /// Iterates over the descendant elements of a given markdown element that exist with a specified depth range.
    /// </summary>
    /// <param name="parentObject">The parent markdown object to enumerate.</param>
    /// <param name="minDepth">The inclusive minimum depth of descendants to include, relative to the <paramref name="parentObject"/>, whereby a value of <c>1</c> is for direct descendants.</param>
    /// <param name="maxDepth">The inclusive maximum depth of descendants to include, relative to the <paramref name="parentObject"/>.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> that contains descendant elements of <paramref name="parentObject"/> that exist at or within
    /// the specified <paramref name="minDepth"/> and <paramref name="maxDepth"/>.</returns>
    /// <remarks><see cref="HtmlAttributes"/> are not included in the result <see cref="IEnumerable{T}"/>.</remarks>
    internal static IEnumerable<MarkdownObject> GetDescendantsInDepthRange(this MarkdownObject parentObject, int minDepth, int maxDepth)
    {
        Debug.Assert(minDepth > 1);
        Debug.Assert(maxDepth > minDepth);
        maxDepth -= minDepth;
        return (maxDepth > 1) ? parentObject.GetDescendantsAtDepth(minDepth).SelectMany(d => _CurrentAndDescendantsToDepth(d, maxDepth)) :
            parentObject.GetDescendantsAtDepth(minDepth).SelectMany(GetCurrentAndDirectDescendants);
    }

    /// <summary>
    /// Iterates over the descendant elements of a given markdown element that exist with a specified depth range.
    /// </summary>
    /// <param name="parentObject">The parent markdown object to enumerate.</param>
    /// <param name="minDepth">The inclusive minimum depth of descendants to include, relative to the <paramref name="parentObject"/>, whereby a value of <c>1</c> is for direct descendants.</param>
    /// <param name="maxDepth">The inclusive maximum depth of descendants to include, relative to the <paramref name="parentObject"/>.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> that contains descendant elements of <paramref name="parentObject"/> that exist at or within
    /// the specified <paramref name="minDepth"/> and <paramref name="maxDepth"/>.</returns>
    /// <remarks><see cref="HtmlAttributes"/> are not included in the result <see cref="IEnumerable{T}"/>.</remarks>
    public static IEnumerable<MarkdownObject> DescendantsInDepthRange(this MarkdownObject parentObject, int minDepth, int maxDepth)
    {
        ArgumentNullException.ThrowIfNull(parentObject);
        return (maxDepth < 1 || maxDepth < minDepth) ? [] : (maxDepth == 1) ? GetDirectDescendants(parentObject) :
            (minDepth == maxDepth) ? GetDescendantsAtDepth(parentObject, minDepth) :
                (minDepth < 2) ? GetDescendantsToDepth(parentObject, maxDepth) :
                    GetDescendantsInDepthRange(parentObject, minDepth, maxDepth);
    }

    /// <summary>
    /// Iterates over the descendant elements of a given markdown element that exist with a specified depth range, where the descendant matches the specified predicate.
    /// </summary>
    /// <param name="parentObject">The parent markdown object to enumerate.</param>
    /// <param name="minDepth">The inclusive minimum depth of descendants to include, relative to the <paramref name="parentObject"/>, whereby a value of <c>1</c> is for direct descendants.</param>
    /// <param name="maxDepth">The inclusive maximum depth of descendants to include, relative to the <paramref name="parentObject"/>.</param>
    /// <param name="filter">The predicate that determines which markdown objects to include in the result <see cref="IEnumerable{T}"/>.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> that contains descendant elements of <paramref name="parentObject"/> that exist at or within
    /// the specified <paramref name="minDepth"/> and <paramref name="maxDepth"/>, where <paramref name="filter"/> returned <see langword="true"/>, given each descendant.</returns>
    /// <remarks><see cref="HtmlAttributes"/> are not included in the result <see cref="IEnumerable{T}"/>.</remarks>
    internal static IEnumerable<MarkdownObject> GetDescendantsInDepthRange(this MarkdownObject parentObject, int minDepth, int maxDepth, Func<MarkdownObject, bool> filter)
    {
        Debug.Assert(minDepth > 1);
        Debug.Assert(maxDepth > minDepth);
        maxDepth -= minDepth;
        return (maxDepth > 1) ? parentObject.GetDescendantsAtDepth(minDepth).SelectMany(d => _CurrentAndDescendantsToDepth(d, maxDepth, filter)) :
            parentObject.GetDescendantsAtDepth(minDepth).SelectMany(d => GetCurrentAndDirectDescendants(d, filter));
    }

    /// <summary>
    /// Iterates over the descendant elements of a given markdown element that exist with a specified depth range, where the descendant matches the specified predicate.
    /// </summary>
    /// <param name="parentObject">The parent markdown object to enumerate.</param>
    /// <param name="minDepth">The inclusive minimum depth of descendants to include, relative to the <paramref name="parentObject"/>, whereby a value of <c>1</c> is for direct descendants.</param>
    /// <param name="maxDepth">The inclusive maximum depth of descendants to include, relative to the <paramref name="parentObject"/>.</param>
    /// <param name="filter">The predicate that determines which markdown objects to include in the result <see cref="IEnumerable{T}"/>.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> that contains descendant elements of <paramref name="parentObject"/> that exist at or within
    /// the specified <paramref name="minDepth"/> and <paramref name="maxDepth"/>, where <paramref name="filter"/> returned <see langword="true"/>, given each descendant.</returns>
    /// <remarks><see cref="HtmlAttributes"/> are not included in the result <see cref="IEnumerable{T}"/>.</remarks>
    public static IEnumerable<MarkdownObject> DescendantsInDepthRange(this MarkdownObject parentObject, int minDepth, int maxDepth, Func<MarkdownObject, bool> filter)
    {
        ArgumentNullException.ThrowIfNull(parentObject);
        ArgumentNullException.ThrowIfNull(filter);
        return (maxDepth < 1 || maxDepth < minDepth) ? [] : (maxDepth == 1) ? GetDirectDescendants(parentObject, filter) :
            (minDepth == maxDepth) ? GetDescendantsAtDepth(parentObject, minDepth, filter) :
                (minDepth < 2) ? GetDescendantsToDepth(parentObject, maxDepth, filter) :
                    GetDescendantsInDepthRange(parentObject, minDepth, maxDepth, filter);
    }

    /// <summary>
    /// Iterates over the top-most descendants of a given markdown element that exist with a specified depth range, where the descendant matches the specified predicate.
    /// </summary>
    /// <param name="parentObject">The parent markdown object to enumerate.</param>
    /// <param name="minDepth">The inclusive minimum depth of descendants to include, relative to the <paramref name="parentObject"/>, whereby a value of <c>1</c> is for direct descendants.</param>
    /// <param name="maxDepth">The inclusive maximum depth of descendants to include, relative to the <paramref name="parentObject"/>.</param>
    /// <param name="filter">The predicate that determines which markdown objects to include in the result <see cref="IEnumerable{T}"/>.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> that contains the top-most descendant elements of <paramref name="parentObject"/> that exist at or within
    /// the specified <paramref name="minDepth"/> and <paramref name="maxDepth"/>, where <paramref name="filter"/> returned <see langword="true"/>, given each descendant.</returns>
    /// <remarks>Yielded descendants are not recursed into.
    /// <para><see cref="HtmlAttributes"/> are not included in the result <see cref="IEnumerable{T}"/>.</para></remarks>
    internal static IEnumerable<MarkdownObject> GetTopMostDescendantsInDepthRange(this MarkdownObject parentObject, int minDepth, int maxDepth, Func<MarkdownObject, bool> filter)
    {
        Debug.Assert(minDepth > 1);
        Debug.Assert(maxDepth > minDepth);
        maxDepth -= minDepth;
        return (maxDepth > 1) ? parentObject.GetDescendantsAtDepth(minDepth).SelectMany(d => _CurrentOrTopMostDescendantsToDepth(d, maxDepth, filter)) :
            parentObject.GetDescendantsAtDepth(minDepth).SelectMany(d => GetCurrentOrDirectDescendants(d, filter));
    }

    /// <summary>
    /// Iterates over the top-most descendants of a given markdown element that exist with a specified depth range, where the descendant matches the specified predicate.
    /// </summary>
    /// <param name="parentObject">The parent markdown object to enumerate.</param>
    /// <param name="minDepth">The inclusive minimum depth of descendants to include, relative to the <paramref name="parentObject"/>, whereby a value of <c>1</c> is for direct descendants.</param>
    /// <param name="maxDepth">The inclusive maximum depth of descendants to include, relative to the <paramref name="parentObject"/>.</param>
    /// <param name="filter">The predicate that determines which markdown objects to include in the result <see cref="IEnumerable{T}"/>.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> that contains the top-most descendant elements of <paramref name="parentObject"/> that exist at or within
    /// the specified <paramref name="minDepth"/> and <paramref name="maxDepth"/>, where <paramref name="filter"/> returned <see langword="true"/>, given each descendant.</returns>
    /// <remarks>Yielded descendants are not recursed into.
    /// <para><see cref="HtmlAttributes"/> are not included in the result <see cref="IEnumerable{T}"/>.</para></remarks>
    public static IEnumerable<MarkdownObject> TopMostDescendantsInDepthRange(this MarkdownObject parentObject, int minDepth, int maxDepth, Func<MarkdownObject, bool> filter)
    {
        ArgumentNullException.ThrowIfNull(parentObject);
        ArgumentNullException.ThrowIfNull(filter);
        return (maxDepth < 1 || maxDepth < minDepth) ? [] : (maxDepth == 1) ? GetDirectDescendants(parentObject, filter) :
            (minDepth == maxDepth) ? GetDescendantsAtDepth(parentObject, minDepth, filter) :
                (minDepth < 2) ? GetTopMostDescendantsToDepth(parentObject, maxDepth, filter) :
                    GetTopMostDescendantsInDepthRange(parentObject, minDepth, maxDepth, filter);
    }

    /// <summary>
    /// Iterates over the descendant elements of a given markdown element that exist with a specified depth range, including <see cref="HtmlAttributes"/>.
    /// </summary>
    /// <param name="parentObject">The parent markdown object to enumerate.</param>
    /// <param name="minDepth">The inclusive minimum depth of descendants to include, relative to the <paramref name="parentObject"/>, whereby a value of <c>1</c> is for direct descendants.</param>
    /// <param name="maxDepth">The inclusive maximum depth of descendants to include, relative to the <paramref name="parentObject"/>.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> that contains descendant elements of <paramref name="parentObject"/> that exist at or within
    /// the specified <paramref name="minDepth"/> and <paramref name="maxDepth"/>, including <see cref="HtmlAttributes"/>.</returns>
    internal static IEnumerable<MarkdownObject> GetAttributesAndDescendantsInDepthRange(this MarkdownObject parentObject, int minDepth, int maxDepth)
    {
        Debug.Assert(minDepth > 1);
        Debug.Assert(maxDepth > minDepth);
        maxDepth -= minDepth;
        return (maxDepth > 1) ? parentObject.GetDescendantsAtDepth(minDepth).SelectMany(d => _AttributesCurrentAndDescendantsToDepth(d, maxDepth)) :
            parentObject.GetDescendantsAtDepth(minDepth).SelectMany(GetAttributesCurrentAndDirectDescendants);
    }

    /// <summary>
    /// Iterates over the descendant elements of a given markdown element that exist with a specified depth range, including <see cref="HtmlAttributes"/>.
    /// </summary>
    /// <param name="parentObject">The parent markdown object to enumerate.</param>
    /// <param name="minDepth">The inclusive minimum depth of descendants to include, relative to the <paramref name="parentObject"/>, whereby a value of <c>1</c> is for direct descendants.</param>
    /// <param name="maxDepth">The inclusive maximum depth of descendants to include, relative to the <paramref name="parentObject"/>.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> that contains descendant elements of <paramref name="parentObject"/> that exist at or within
    /// the specified <paramref name="minDepth"/> and <paramref name="maxDepth"/>, including <see cref="HtmlAttributes"/>.</returns>
    public static IEnumerable<MarkdownObject> AttributesAndDescendantsInDepthRange(this MarkdownObject parentObject, int minDepth, int maxDepth)
    {
        ArgumentNullException.ThrowIfNull(parentObject);
        return (maxDepth < 1 || maxDepth < minDepth) ? [] : (maxDepth == 1) ? GetAttributesAndDirectDescendants(parentObject) :
            (minDepth == maxDepth) ? GetAttributesAndDescendantsAtDepth(parentObject, minDepth) :
                (minDepth < 2) ? GetAttributesAndDescendantsToDepth(parentObject, maxDepth) :
                    GetAttributesAndDescendantsInDepthRange(parentObject, minDepth, maxDepth);
    }

    /// <summary>
    /// Iterates over the descendant elements of a given markdown element that exist with a specified depth range, where the descendant is <see cref="HtmlAttributes"/> or matches the specified predicate.
    /// </summary>
    /// <param name="parentObject">The parent markdown object to enumerate.</param>
    /// <param name="minDepth">The inclusive minimum depth of descendants to include, relative to the <paramref name="parentObject"/>, whereby a value of <c>1</c> is for direct descendants.</param>
    /// <param name="maxDepth">The inclusive maximum depth of descendants to include, relative to the <paramref name="parentObject"/>.</param>
    /// <param name="filter">The predicate that determines which non-attribute markdown objects to include in the result <see cref="IEnumerable{T}"/>.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> that contains descendant elements of <paramref name="parentObject"/> that exist at or within
    /// the specified <paramref name="minDepth"/> and <paramref name="maxDepth"/>, where the descendant is <see cref="HtmlAttributes"/> or <paramref name="filter"/> returned <see langword="true"/>,
    /// given each descendant.</returns>
    internal static IEnumerable<MarkdownObject> GetAttributesAndDescendantsInDepthRange(this MarkdownObject parentObject, int minDepth, int maxDepth, Func<MarkdownObject, bool> filter)
    {
        Debug.Assert(minDepth > 1);
        Debug.Assert(maxDepth > minDepth);
        maxDepth -= minDepth;
        return (maxDepth > 1) ? parentObject.GetDescendantsAtDepth(minDepth).SelectMany(d => _AttributesCurrentAndDescendantsToDepth(d, maxDepth, filter)) :
            parentObject.GetDescendantsAtDepth(minDepth).SelectMany(d => GetAttributesCurrentAndDirectDescendants(d, filter));
    }

    /// <summary>
    /// Iterates over the descendant elements of a given markdown element that exist with a specified depth range, where the descendant is <see cref="HtmlAttributes"/> or matches the specified predicate.
    /// </summary>
    /// <param name="parentObject">The parent markdown object to enumerate.</param>
    /// <param name="minDepth">The inclusive minimum depth of descendants to include, relative to the <paramref name="parentObject"/>, whereby a value of <c>1</c> is for direct descendants.</param>
    /// <param name="maxDepth">The inclusive maximum depth of descendants to include, relative to the <paramref name="parentObject"/>.</param>
    /// <param name="filter">The predicate that determines which non-attribute markdown objects to include in the result <see cref="IEnumerable{T}"/>.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> that contains descendant elements of <paramref name="parentObject"/> that exist at or within
    /// the specified <paramref name="minDepth"/> and <paramref name="maxDepth"/>, where the descendant is <see cref="HtmlAttributes"/> or <paramref name="filter"/> returned <see langword="true"/>,
    /// given each descendant.</returns>
    public static IEnumerable<MarkdownObject> AttributesAndDescendantsInDepthRange(this MarkdownObject parentObject, int minDepth, int maxDepth, Func<MarkdownObject, bool> filter)
    {
        ArgumentNullException.ThrowIfNull(parentObject);
        ArgumentNullException.ThrowIfNull(filter);
        return (maxDepth < 1 || maxDepth < minDepth) ? [] : (maxDepth == 1) ? GetAttributesAndDirectDescendants(parentObject, filter) :
            (minDepth == maxDepth) ? GetAttributesAndDescendantsAtDepth(parentObject, minDepth, filter) :
                (minDepth < 2) ? GetAttributesAndDescendantsToDepth(parentObject, maxDepth, filter) :
                    GetAttributesAndDescendantsInDepthRange(parentObject, minDepth, maxDepth, filter);
    }

    /// <summary>
    /// Iterates over the descendant elements of a given markdown element that exist with a specified depth range, where the descendant is <see cref="HtmlAttributes"/> or matches the specified predicate.
    /// </summary>
    /// <param name="parentObject">The parent markdown object to enumerate.</param>
    /// <param name="minDepth">The inclusive minimum depth of descendants to include, relative to the <paramref name="parentObject"/>, whereby a value of <c>1</c> is for direct descendants.</param>
    /// <param name="maxDepth">The inclusive maximum depth of descendants to include, relative to the <paramref name="parentObject"/>.</param>
    /// <param name="filter">The predicate that determines which non-attribute markdown objects to include in the result <see cref="IEnumerable{T}"/>.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> that contains descendant elements of <paramref name="parentObject"/> that exist at or within
    /// the specified <paramref name="minDepth"/> and <paramref name="maxDepth"/>, where the descendant is <see cref="HtmlAttributes"/> or <paramref name="filter"/> returned <see langword="true"/>,
    /// given each descendant.</returns>
    internal static IEnumerable<MarkdownObject> GetAttributesAndTopMostDescendantsInDepthRange(this MarkdownObject parentObject, int minDepth, int maxDepth, Func<MarkdownObject, bool> filter)
    {
        Debug.Assert(minDepth > 1);
        Debug.Assert(maxDepth > minDepth);
        maxDepth -= minDepth;
        return (maxDepth > 1) ? parentObject.GetDescendantsAtDepth(minDepth).SelectMany(d => _AttributesAndCurrentOrTopMostDescendantsToDepth(d, maxDepth, filter)) :
            parentObject.GetDescendantsAtDepth(minDepth).SelectMany(d => GetAttributesCurrentOrDirectDescendants(d, filter));
    }

    /// <summary>
    /// Iterates over the descendant <see cref="HtmlAttributes"/> of a given markdown element that exist with a specified depth range.
    /// </summary>
    /// <param name="parentObject">The parent markdown object to enumerate.</param>
    /// <param name="minDepth">The inclusive minimum depth of descendants to include, relative to the <paramref name="parentObject"/>, whereby a value of <c>1</c> is for direct descendants.</param>
    /// <param name="maxDepth">The inclusive maximum depth of descendants to include, relative to the <paramref name="parentObject"/>.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> that contains descendant <see cref="HtmlAttributes"/> of <paramref name="parentObject"/> that exist at or within
    /// the specified <paramref name="minDepth"/> and <paramref name="maxDepth"/>.</returns>
    internal static IEnumerable<MarkdownObject> GetAttributesInDepthRange(this MarkdownObject parentObject, int minDepth, int maxDepth)
    {
        Debug.Assert(minDepth > 1);
        Debug.Assert(maxDepth > minDepth);
        maxDepth -= minDepth;
        var parentsForDepth = (minDepth > 2) ? parentObject.GetDescendantsAtDepth(minDepth - 1) : parentObject.GetDirectDescendants();
        return (maxDepth > 1) ? parentsForDepth.SelectMany(d => GetAttributesToDepth(d, maxDepth)) : parentsForDepth.SelectMany(GetDirectAttributes);
    }

    /// <summary>
    /// Iterates over the descendant <see cref="HtmlAttributes"/> of a given markdown element that exist with a specified depth range.
    /// </summary>
    /// <param name="parentObject">The parent markdown object to enumerate.</param>
    /// <param name="minDepth">The inclusive minimum depth of descendants to include, relative to the <paramref name="parentObject"/>, whereby a value of <c>1</c> is for direct descendants.</param>
    /// <param name="maxDepth">The inclusive maximum depth of descendants to include, relative to the <paramref name="parentObject"/>.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> that contains descendant <see cref="HtmlAttributes"/> of <paramref name="parentObject"/> that exist at or within
    /// the specified <paramref name="minDepth"/> and <paramref name="maxDepth"/>.</returns>
    public static IEnumerable<MarkdownObject> AttributesInDepthRange(this MarkdownObject parentObject, int minDepth, int maxDepth)
    {
        ArgumentNullException.ThrowIfNull(parentObject);
        if (maxDepth < 1 || maxDepth < minDepth) return [];
        if (maxDepth > 1) return (minDepth < 2) ? GetAttributesToDepth(parentObject, maxDepth) :
            GetAttributesInDepthRange(parentObject, minDepth, maxDepth);
        var attributes = parentObject.TryGetAttributes();
        return (attributes is null) ? [] : [attributes];
    }
}