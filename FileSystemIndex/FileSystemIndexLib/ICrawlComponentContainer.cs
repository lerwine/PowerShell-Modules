using System;
using System.ComponentModel;

namespace Erwine.Leonard.T.GDIPlus
{
    public interface ICrawlComponentContainer<TKey> : /*IEquatable<ICrawlComponentContainer<TKey>>, IComparable<ICrawlComponentContainer<TKey>>, */INotifyPropertyChanging, INotifyPropertyChanged
		where TKey : IComparable
    {
        CrawlComponentCollection<TKey> ItemCollection { get; }
    }
}
