namespace XmlUtility
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public interface ISimpleType
    {
        string ToXmlText();

        object BaseValue { get; }
    }

    public interface ISimpleType<T> : ISimpleType
    {
        new T BaseValue { get; }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}