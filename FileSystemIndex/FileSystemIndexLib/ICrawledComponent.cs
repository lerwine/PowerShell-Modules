using System;
using System.ComponentModel;

namespace Erwine.Leonard.T.GDIPlus
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public interface ICrawledComponent<TKey> : /*IEquatable<CrawledComponent<TKey>>, IEquatable<TKey>, IEquatable<IComparable<TKey>>, IComparable<TKey>, */INotifyPropertyChanging, INotifyPropertyChanged, ICloneable
        where TKey : IComparable
    {
        /// <summary>
        /// 
        /// </summary>
        TKey Key { get; set; }

        /// <summary>
        /// 
        /// </summary>
        ICrawlComponentContainer<TKey> Parent { get; set; }

        /// <summary>
        /// 
        /// </summary>
        ComponentPropertyDictionary<TKey> Properties { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        ICrawledComponent<TKey> Clone(ICrawlComponentContainer<TKey> parent);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        new ICrawledComponent<TKey> Clone();
    }
}
