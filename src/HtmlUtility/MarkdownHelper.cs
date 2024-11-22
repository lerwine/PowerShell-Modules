using System.Collections.ObjectModel;
using System.Reflection;
using Markdig.Renderers.Html;

namespace HtmlUtility;

public static class MarkdownHelper
{
    private static readonly ReadOnlyDictionary<MarkdownTokenType, Type> _markdownTokenTypeMap;

    static MarkdownHelper()
    {
        Type t = typeof(MarkdownTokenType);
        _markdownTokenTypeMap = new(Enum.GetValues<MarkdownTokenType>().ToDictionary(k => k, v => t.GetField(v.ToString("F"))!.GetCustomAttribute<ReflectionTypeAttribute>()!.Type));
    }
    public static Type ToReflectionType(this MarkdownTokenType type) => _markdownTokenTypeMap[type];
    
    public static List<Type>? ToReflectionTypes(this MarkdownTokenType[]? Type)
    {
        if (Type is null || Type.Length == 0) return null;
        var types = Type.Distinct().Select(ToReflectionType).ToList();
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

    internal static IEnumerable<Markdig.Syntax.MarkdownObject> GetChildObjects(this Markdig.Syntax.MarkdownObject source)
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