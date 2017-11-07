using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Erwine.Leonard.T.WPF.UI
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class SelectableItemVM<T> : DependencyObject
    {
        public event EventHandler<DisplayValueChangedEventArgs<T>> ValueChanged;
        public event EventHandler<ValueChangedEventArgs<bool>> SelectionChanged;
        public event EventHandler<CoerceValueEventArgs<T>> GetCoercedValue;

        #region DisplayText Dependency Property Members

        public string DisplayText
        {
            get { return (string)GetValue(DisplayTextProperty); }
            set { SetValue(DisplayTextProperty, value); }
        }

        public static readonly DependencyProperty DisplayTextProperty = DependencyProperty.Register("DisplayText", typeof(string), typeof(SelectableItemVM<T>),
            new PropertyMetadata("", null, (d, baseValue) =>
            {
                string s = baseValue as string;
                return (String.IsNullOrEmpty(s)) ? "" : s.Trim();
            }));

        #endregion

        #region Value Dependency Property Members

        public T Value
        {
            get { return (T)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(T), typeof(SelectableItemVM<T>),
            new PropertyMetadata(default(T), (d, e) =>
            {
                string displayText = (e.NewValue == null || e.NewValue is string) ? e.NewValue as string : e.NewValue.ToString();
                if (displayText == null)
                    displayText = "";
                SelectableItemVM<T> vm = (SelectableItemVM<T>)d;
                EventHandler<DisplayValueChangedEventArgs<T>> valueChanged = vm.ValueChanged;
                if (valueChanged == null)
                    vm.DisplayText = displayText;
                else
                {
                    DisplayValueChangedEventArgs<T> args = new DisplayValueChangedEventArgs<T>((T)(e.OldValue), (T)(e.NewValue), displayText);
                    valueChanged(vm, args);
                    vm.DisplayText = args.Displaytext;
                }
            }, (d, baseValue) =>
            {
                SelectableItemVM<T> vm = (SelectableItemVM<T>)d;
                EventHandler<CoerceValueEventArgs<T>> getCoercedValue = vm.GetCoercedValue;
                if (getCoercedValue == null)
                    return (baseValue != null && baseValue is T) ? (T)baseValue : default(T);
                CoerceValueEventArgs<T> args = new CoerceValueEventArgs<T>(baseValue);
                getCoercedValue(vm, args);
                return args.CoercedValue;
            }));

        #endregion

        #region Parents Dependency Property Members

        public SingleSelectVM<T> Parent
        {
            get { return (SingleSelectVM<T>)GetValue(ParentsProperty); }
            private set { SetValue(ParentsProperty, value); }
        }

        private static readonly DependencyPropertyKey ParentsPropertyKey = DependencyProperty.RegisterReadOnly("Parents", typeof(SingleSelectVM<T>), typeof(SelectableItemVM<T>),
            new PropertyMetadata(null));
        
        public static readonly DependencyProperty ParentsProperty = ParentsPropertyKey.DependencyProperty;

        public void AddToParent(SingleSelectVM<T> parent)
        {
            if (!CheckAccess())
            {
                Dispatcher.Invoke(() => AddToParent(parent));
                return;
            }

            if (parent == null)
                throw new ArgumentNullException("parent");

            if (Parent != null)
            {
                if (ReferenceEquals(Parent, parent))
                    return;
                throw new InvalidOperationException("Item already belongs to another collection");
            }
            if (!parent.Options.Any(i => ReferenceEquals(i, this)))
            {
                parent.Add(this);
                if (!parent.Options.Any(i => ReferenceEquals(i, this)))
                {
                    if (Parent != null && ReferenceEquals(Parent, parent))
                        Parent = null;
                    return;
                }
                if (Parent != null)
                    return;
            }

            Parent = parent;
        }

        public void RemoveFromParent(SingleSelectVM<T> parent)
        {
            if (!CheckAccess())
            {
                Dispatcher.Invoke(() => RemoveFromParent(parent));
                return;
            }

            if (parent == null)
                throw new ArgumentNullException("parent");

            if (Parent == null || !ReferenceEquals(parent, Parent))
                return;

            if (parent.Options.Any(i => ReferenceEquals(i, this)))
            {
                parent.Remove(this);
                if (parent.Options.Any(i => ReferenceEquals(i, this)))
                {
                    if (Parent == null)
                        Parent = parent;
                    return;
                }
                else if (Parent == null)
                    return;
            }

            Parent = null;
        }

        #endregion

        #region IsSelected Dependency Property Members

        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register("IsSelected", typeof(bool), typeof(SelectableItemVM<T>),
            new PropertyMetadata("", (d, e) => ((SelectableItemVM<T>)d).IsSelected_PropertyChanged((bool)(e.NewValue), (bool)(e.OldValue), e.Property)));

        private void IsSelected_PropertyChanged(bool newValue, bool oldValue, DependencyProperty property)
        {
            EventHandler<ValueChangedEventArgs<bool>> selectionChanged = SelectionChanged;
            if (selectionChanged != null)
                selectionChanged(this, new ValueChangedEventArgs<bool>(oldValue, newValue));
        }

        #endregion

        public SelectableItemVM(T value, string displayText)
        {
            Value = value;
            DisplayText = displayText;
        }

        public SelectableItemVM() { }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}