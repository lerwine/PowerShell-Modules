using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Erwine.Leonard.T.GDIPlus.Collections
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    public interface IGenericAccessObservableCollection<TBaseType> : IList, ICollection, IEnumerable, INotifyCollectionChanged, INotifyPropertyChanged
    {
        ReadOnlyObservableCollection<TBaseType> ItemCollection { get; }
    }

    public interface IGenericAccessObservableCollection<TItem, TBaseType> : IGenericAccessObservableCollection<TBaseType>, IList<TItem>, ICollection<TItem>, IEnumerable<TItem>
        where TItem : TBaseType
    {
        new TItem this[int index] { get; }
    }
}
