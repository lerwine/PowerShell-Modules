using System;
using System.ComponentModel;

namespace Erwine.Leonard.T.GDIPlus
{
    public interface ICrawledComponent<TKey> : /*IEquatable<CrawledComponent<TKey>>, IEquatable<TKey>, IEquatable<IComparable<TKey>>, IComparable<TKey>, */INotifyPropertyChanging, INotifyPropertyChanged, ICloneable
		where TKey : IComparable
    {
        TKey Key { get; set; }
        ICrawlComponentContainer<TKey> Parent { get; set; }
        ICrawledComponent<TKey> Clone(ICrawlComponentContainer<TKey> parent);
		ComponentPropertyDictionary Properties { get; }
        new ICrawledComponent<TKey> Clone();
    }
}
