namespace Erwine.Leonard.T.WPF.UI
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class CoerceItemValueEventArgs<T> : CoerceValueEventArgs<T>
    {
        public CoerceItemValueEventArgs(SelectableItemVM<T> item, CoerceValueEventArgs<T> args)
            : base(args)
        {
            Item = item;
        }

        public SelectableItemVM<T> Item { get; private set; }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}