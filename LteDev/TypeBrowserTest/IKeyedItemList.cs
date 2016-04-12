using System.Collections;
using System.Collections.Generic;

namespace Erwine.Leonard.T.TypeBrowserTest
{
    public interface IKeyedItemList : IList<IKeyedItem>, IList
    {
    }

    public interface IKeyedItemList<TKey> : IList<IKeyedItem<TKey>>, IKeyedItemList
    {
    }
}