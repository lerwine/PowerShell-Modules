using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Markdig.Renderers.Html;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace HtmlUtility;

public static partial class MarkdownExtensionMethods
{
    internal static readonly Type MarkdownObjectType = typeof(MarkdownObject);

    internal static readonly Type HtmlAttributesType = typeof(HtmlAttributes);

    private static readonly ReadOnlyDictionary<MarkdownTokenType, Type> _markdownTokenTypeMap;

    static MarkdownExtensionMethods()
    {
        Type t = typeof(MarkdownTokenType);
        _markdownTokenTypeMap = new(Enum.GetValues<MarkdownTokenType>().ToDictionary(k => k, v => t.GetField(v.ToString("F"))!.GetCustomAttribute<ReflectionTypeAttribute>()!.Type));
    }

    internal static bool IsNonAttributeMarkdownObjectType(this Type type) => MarkdownObjectType.IsAssignableFrom(type) && !HtmlAttributesType.IsAssignableFrom(type);

    internal static IEnumerable<Type> CollapseTypes(this IEnumerable<Type> source, out int count)
    {
        if (source is null)
        {
            count = 0;
            return [];
        }
        using IEnumerator<Type> enumerator = source.GetEnumerator();
        while (enumerator.MoveNext())
        {
            var t = enumerator.Current;
            if (t is not null)
            {
                Collection<Type> types = [t];
                count = 1;
                while (enumerator.MoveNext())
                {
                    if ((t = enumerator.Current) is not null)
                    {
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
                }
                return types;
            }
        }
        count = 0;
        return [];
    }

    internal static bool IsMarkdownObjectEnumerable(this MarkdownObject? parent, [NotNullWhen(true)] out IEnumerable<MarkdownObject>? result)
    {
        if (parent is not null)
        {
            if (parent is ContainerBlock || parent is ContainerInline)
            {
                result = (IEnumerable<MarkdownObject>)parent;
                return true;
            }
            if (parent is LeafBlock leafBlock && leafBlock.Inline is not null)
            {
                result = leafBlock.Inline.AsEnumerable();
                return true;
            }
        }
        result = null;
        return false;
    }

    internal static bool HasDirectDescendants(this ContainerBlock parent, [NotNullWhen(true)] out IEnumerable<MarkdownObject>? result)
    {
        if (parent.Count > 0)
        {
            result = parent.AsEnumerable();
            return true;
        }
        result = null;
        return false;
    }

    internal static bool HasDirectDescendants(this ContainerInline parent, [NotNullWhen(true)] out IEnumerable<MarkdownObject>? result)
    {
        if (parent.FirstChild is not null)
        {
            result = parent.AsEnumerable();
            return true;
        }
        result = null;
        return false;
    }
}
