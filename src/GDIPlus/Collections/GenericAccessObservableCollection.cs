using System.Collections.ObjectModel;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Erwine.Leonard.T.GDIPlus.Collections
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    public class GenericAccessObservableCollection<TItem, TBaseType> : ObservableCollection<TItem>, IGenericAccessObservableCollection<TItem, TBaseType>
        where TItem : TBaseType
    {
        ObservableCollection<TBaseType> _innerItemCollection;
        ReadOnlyObservableCollection<TBaseType> _itemCollection;

        public ReadOnlyObservableCollection<TBaseType> ItemCollection => _itemCollection;

        public GenericAccessObservableCollection()
            : base()
        {
            OnInitializeItemCollection(null);
        }

        public GenericAccessObservableCollection(List<TItem> list)
            : base(list)
        {
            OnInitializeItemCollection(list);
        }

        public GenericAccessObservableCollection(IEnumerable<TItem> collection)
            : base(collection)
        {
            OnInitializeItemCollection(collection);
        }

        protected virtual void OnInitializeItemCollection(IEnumerable<TItem> collection)
        {
            _innerItemCollection = [];
            _itemCollection = new ReadOnlyObservableCollection<TBaseType>(_innerItemCollection);
            if (collection == null)
                return;

            foreach (TItem item in collection)
                _innerItemCollection.Add(item);
        }

        protected override void ClearItems()
        {
            base.ClearItems();
            _innerItemCollection.Clear();
        }

        protected override void InsertItem(int index, TItem item)
        {
            base.InsertItem(index, item);
            _innerItemCollection.Insert(index, item);
        }

        protected override void RemoveItem(int index)
        {
            base.RemoveItem(index);
            _innerItemCollection.RemoveAt(index);
        }

        protected override void SetItem(int index, TItem item)
        {
            base.SetItem(index, item);
            _innerItemCollection[index] = item;
        }
    }
}
