using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Erwine.Leonard.T.WPF.UI
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class DependencyObjectBuilderVM : DependencyObject
    {
        #region ClassName Dependency Property Members

        public string ClassName
        {
            get { return (string)GetValue(ClassNameProperty); }
            set { SetValue(ClassNameProperty, value); }
        }

        public static readonly DependencyProperty ClassNameProperty = DependencyProperty.Register("ClassName", typeof(string), typeof(DependencyObjectBuilderVM),
            new PropertyMetadata("", (d, e) => ((DependencyObjectBuilderVM)d).ClassName_PropertyChanged((string)(e.NewValue), (string)(e.OldValue), e.Property),
                (d , baseValue) => ((DependencyObjectBuilderVM)d).ClassName_CoerceValue(baseValue)), v => ClassName_IsValid((string)v));

        private void ClassName_PropertyChanged(string newValue, string oldValue, DependencyProperty property)
        {
#warning Not implemented
            throw new NotImplementedException();
        }

        private object ClassName_CoerceValue(object baseValue)
        {
#warning Not implemented
            throw new NotImplementedException();
        }

        private static bool ClassName_IsValid(string value)
        {
#warning Not implemented
            throw new NotImplementedException();
        }

        #endregion

    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
