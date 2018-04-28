using System;

namespace Erwine.Leonard.T.WPF.UI
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class CoerceValueEventArgs<T>
    {
        protected CoerceValueEventArgs(CoerceValueEventArgs<T> args)
        {
            BaseValue = args.BaseValue;
            CoercedValue = args.CoercedValue;
        }

        public CoerceValueEventArgs(object baseValue)
        {
            BaseValue = baseValue;
            CoercedValue = (baseValue != null && baseValue is T) ? (T)baseValue : default(T);
        }

        public object BaseValue { get; private set; }
        public T CoercedValue { get; set; }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}