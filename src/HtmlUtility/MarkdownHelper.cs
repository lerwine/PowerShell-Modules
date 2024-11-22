namespace HtmlUtility;

public static class MarkdownHelper
{
    public static Type ToReflectionType(this MarkdownTokenType type)
    {
        throw new NotImplementedException();
    }
    
    public static List<Type>? ToUniqueReflectionTypes(this MarkdownTokenType[]? Type)
    {
        if (Type is null || Type.Length == 0) return null;
            var types = Type.Distinct().Select(ToReflectionType).ToList();
            if (types.Count > 0)
            {
                for (int end = 1; end < types.Count; end++)
                {
                    var c = types[end];
                    for (int n = 0; n < end; n++)
                    {
                        if (types[n].IsAssignableFrom(c))
                        {
                            types.RemoveAt(n);
                            end--;
                            break;
                        }
                    }
                }
            }
            return types;
    }
}