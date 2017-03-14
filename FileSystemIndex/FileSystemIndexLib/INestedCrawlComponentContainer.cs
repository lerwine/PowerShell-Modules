using System;
using System.ComponentModel;

namespace FileSystemIndexLib
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public interface INestedCrawlComponentContainer<TKey> : ICrawlComponentContainer<TKey>, ICrawledComponent<TKey>
        where TKey : IComparable
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        new INestedCrawlComponentContainer<TKey> Clone(ICrawlComponentContainer<TKey> directory);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        new INestedCrawlComponentContainer<TKey> Clone();
    }
}
