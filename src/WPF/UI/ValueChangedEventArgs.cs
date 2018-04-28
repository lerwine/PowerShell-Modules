using System;

namespace Erwine.Leonard.T.WPF.UI
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class ValueChangedEventArgs<T>
    {
        public T OldValue { get; private set; }
        public T NewValue { get; private set; }
        public ValueChangedEventArgs(T oldValue, T newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}