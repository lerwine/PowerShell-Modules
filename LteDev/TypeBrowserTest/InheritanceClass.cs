using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Erwine.Leonard.T.TypeBrowserTest
{
    public abstract class InheritanceClass
    {
    }

    public class InheritanceClass<TValue> : InheritanceClass
        where TValue : struct
    {
    }

    public class InheritanceClass<TKey, TValue> : InheritanceClass<TValue>
        where TValue : struct
    {

    }
}
