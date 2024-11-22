namespace HtmlUtility;

[AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = true)]
sealed class ReflectionTypeAttribute : Attribute
{
    // This is a positional argument
    public ReflectionTypeAttribute(Type type) => Type = type ?? throw new ArgumentNullException(nameof(type));
    
    public Type Type { get; }
}
