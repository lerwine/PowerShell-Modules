using System;

namespace Erwine.Leonard.T.WPF.UI
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class DisplayValueChangedEventArgs<T> : ValueChangedEventArgs<T>
    {
        public string Displaytext { get; set; }
        public DisplayValueChangedEventArgs(T oldValue, T newValue, string displayText)
            : base(oldValue, newValue)
        {
            Displaytext = displayText;
        }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}