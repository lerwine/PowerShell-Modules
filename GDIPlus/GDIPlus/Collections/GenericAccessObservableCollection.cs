using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erwine.Leonard.T.GDIPlus.Collections
{
    public class GenericAccessObservableCollection<TItem, TBaseType> : ObservableCollection<TItem>, IGenericAccessObservableCollection<TItem, TBaseType>
        where TItem : TBaseType
    {
        ObservableCollection<TBaseType> _innerItemCollection;
        ReadOnlyObservableCollection<TBaseType> _itemCollection;

        public ReadOnlyObservableCollection<TBaseType> ItemCollection { get { return this._itemCollection; } }

        public GenericAccessObservableCollection()
            : base()
        {
            this.OnInitializeItemCollection(null);
        }

        public GenericAccessObservableCollection(List<TItem> list)
            : base(list)
        {
            this.OnInitializeItemCollection(list);
        }

        public GenericAccessObservableCollection(IEnumerable<TItem> collection)
            : base(collection)
        {
            this.OnInitializeItemCollection(collection);
        }

        protected virtual void OnInitializeItemCollection(IEnumerable<TItem> collection)
        {
            this._innerItemCollection = new ObservableCollection<TBaseType>();
            this._itemCollection = new ReadOnlyObservableCollection<TBaseType>(this._innerItemCollection);
            if (collection == null)
                return;

            foreach (TItem item in collection)
                this._innerItemCollection.Add(item);
        }

        protected override void ClearItems()
        {
            base.ClearItems();
            this._innerItemCollection.Clear();
        }

        protected override void InsertItem(int index, TItem item)
        {
            base.InsertItem(index, item);
            this._innerItemCollection.Insert(index, item);
        }

        protected override void RemoveItem(int index)
        {
            base.RemoveItem(index);
            this._innerItemCollection.RemoveAt(index);
        }

        protected override void SetItem(int index, TItem item)
        {
            base.SetItem(index, item);
            this._innerItemCollection[index] = item;
        }
    }
}
