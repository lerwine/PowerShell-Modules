using System;
using System.ComponentModel;

namespace Erwine.Leonard.T.GDIPlus
{
    public interface INestedCrawlComponentContainer<TKey> : ICrawlComponentContainer<TKey>, ICrawledComponent<TKey>
		where TKey : IComparable
    {
        new INestedCrawlComponentContainer<TKey> Clone(ICrawlComponentContainer<TKey> directory);
        new INestedCrawlComponentContainer<TKey> Clone();
    }
}
