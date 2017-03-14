using System;
using System.ComponentModel;

namespace FileSystemIndexLib
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public interface ICrawlComponentContainer<TKey> : /*IEquatable<ICrawlComponentContainer<TKey>>, IComparable<ICrawlComponentContainer<TKey>>, */INotifyPropertyChanging, INotifyPropertyChanged
        where TKey : IComparable
    {

        /// <summary>
        /// 
        /// </summary>
        CrawlComponentCollection<TKey> ItemCollection { get; }
    }
}
