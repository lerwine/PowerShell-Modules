using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Erwine.Leonard.T.GDIPlus.Collections
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public interface IGenericAccessObservableCollection<TBaseType> : IList, ICollection, IEnumerable, INotifyCollectionChanged, INotifyPropertyChanged
    {
        ReadOnlyObservableCollection<TBaseType> ItemCollection { get; }
    }

    public interface IGenericAccessObservableCollection<TItem, TBaseType> : IGenericAccessObservableCollection<TBaseType>, IList<TItem>, ICollection<TItem>, IEnumerable<TItem>
        where TItem : TBaseType
    {
        new TItem this[int index] { get; }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
