namespace XmlUtility
{
    public interface ISimpleType
    {
        string ToXmlText();

        object BaseValue { get; }
    }

    public interface ISimpleType<T> : ISimpleType
    {
        new T BaseValue { get; }
    }
}