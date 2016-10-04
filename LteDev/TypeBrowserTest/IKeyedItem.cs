namespace Erwine.Leonard.T.TypeBrowserTest
{
    public interface IKeyedItem
    {
        object Key { get; }
    }

    public interface IKeyedItem<TKey> : IKeyedItem
    {
        new TKey Key { get; }
    }
}