namespace Erwine.Leonard.T.WPF.UI
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class ItemValueChangedEventArgs<T> : DisplayValueChangedEventArgs<T>
    {
        public ItemValueChangedEventArgs(SelectableItemVM<T> item, T oldValue, T newValue, string displayText)
            : base(oldValue, newValue, displayText)
        {
            Item = item;
        }

        public SelectableItemVM<T> Item { get; private set; }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}