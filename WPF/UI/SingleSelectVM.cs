using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Erwine.Leonard.T.WPF.UI
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class SingleSelectVM<T> : DependencyObject
    {
        public event EventHandler<ItemValueChangedEventArgs<T>> ItemValueChanged;
        public event EventHandler<CoerceItemValueEventArgs<T>> GetItemCoercedValue;
        public event EventHandler<ValueChangedEventArgs<T>> SelectedValueChanged;

        #region SelectedIndex Dependency Property Members

        public int SelectedIndex
        {
            get { return (int)GetValue(SelectedIndexProperty); }
            set { SetValue(SelectedIndexProperty, value); }
        }

        public static readonly DependencyProperty SelectedIndexProperty = DependencyProperty.Register("SelectedIndex", typeof(int), typeof(SingleSelectVM<T>),
            new PropertyMetadata(-1, (d, e) => ((SingleSelectVM<T>)d).SelectedIndex_PropertyChanged((int)(e.NewValue), (int)(e.OldValue), e.Property),
                (d, baseValue) => ((SingleSelectVM<T>)d).SelectedIndex_CoerceValue(baseValue)), v => SelectedIndex_IsValid((int)v));

        private void SelectedIndex_PropertyChanged(int newValue, int oldValue, DependencyProperty property)
        {
            SelectableItemVM<T> item = (newValue < 0 || newValue >= _innerOptions.Count) ? null : _innerOptions[newValue];
            if ((item == null) ? SelectedItem != null : (SelectedItem == null || !ReferenceEquals(SelectedItem, item)))
                SelectedItem = item;
        }

        private object SelectedIndex_CoerceValue(object baseValue)
        {
            int index;
            if (!(baseValue != null && baseValue is int))
            {
                index = _innerOptions.TakeWhile(i => !i.IsSelected).Count();
                if (index > _innerOptions.Count - 1)
                    return -1;

                index++;
                return (_innerOptions[index].IsSelected) ? index: -1;
            }

            if ((index = (int)baseValue) < 0 || index >= _currentOptions.Count)
                return -1;
            return index;
        }

        private static bool SelectedIndex_IsValid(int value) { return value > -2; }

        #endregion

        #region SelectedItem Dependency Property Members

        public SelectableItemVM<T> SelectedItem
        {
            get { return (SelectableItemVM<T>)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register("SelectedItem", typeof(SelectableItemVM<T>), typeof(SingleSelectVM<T>),
            new PropertyMetadata(null, (d, e) => ((SingleSelectVM<T>)d).SelectedItem_PropertyChanged((SelectableItemVM<T>)(e.NewValue), (SelectableItemVM<T>)(e.OldValue), e.Property),
                (d, baseValue) => ((SingleSelectVM<T>)d).SelectedItem_CoerceValue(baseValue)));

        private void SelectedItem_PropertyChanged(SelectableItemVM<T> newValue, SelectableItemVM<T> oldValue, DependencyProperty property)
        {
            if (oldValue != null && oldValue.IsSelected)
                oldValue.IsSelected = false;

            int selectedIndex = (newValue == null) ? -1 : _innerOptions.IndexOf(newValue);
            SelectedIndex = selectedIndex;
            if (!(selectedIndex < 0 || newValue.IsSelected))
                newValue.IsSelected = true;
        }

        private object SelectedItem_CoerceValue(object baseValue)
        {
            SingleSelectVM<T> item = baseValue as SingleSelectVM<T>;
            if (item == null || _innerOptions.Any(i => ReferenceEquals(i, baseValue)))
                return item;
            return null;
        }

        #endregion

        #region SelectedValue Dependency Property Members

        public T SelectedValue
        {
            get { return (T)GetValue(SelectedValueProperty); }
            private set { SetValue(SelectedValueProperty, value); }
        }

        private static readonly DependencyPropertyKey SelectedValuePropertyKey = DependencyProperty.RegisterReadOnly("SelectedValue", typeof(T), typeof(SingleSelectVM<T>),
            new PropertyMetadata(default(T), (d, e) => ((SingleSelectVM<T>)d).SelectedValue_PropertyChanged((T)(e.NewValue), (T)(e.OldValue), e.Property),
                (d, baseValue) => ((SingleSelectVM<T>)d).SelectedValue_CoerceValue(baseValue)));

        public static readonly DependencyProperty SelectedValueProperty = SelectedValuePropertyKey.DependencyProperty;

        private void SelectedValue_PropertyChanged(T newValue, T oldValue, DependencyProperty property)
        {
            EventHandler<ValueChangedEventArgs<T>> selectedValueChanged = SelectedValueChanged;
            if (selectedValueChanged != null)
                selectedValueChanged(this, new ValueChangedEventArgs<T>(oldValue, newValue));
        }

        private object SelectedValue_CoerceValue(object baseValue)
        {
            SelectableItemVM<T> item = SelectedItem;
            if (item == null)
                return UnselectedValue;
            return item.Value;
        }
        
        #endregion

        #region UnselectedValue Dependency Property Members

        public T UnselectedValue
        {
            get { return (T)GetValue(UnselectedValueProperty); }
            set { SetValue(UnselectedValueProperty, value); }
        }

        public static readonly DependencyProperty UnselectedValueProperty = DependencyProperty.Register("UnselectedValue", typeof(T), typeof(SingleSelectVM<T>),
            new PropertyMetadata(default(T), (d, e) => ((SingleSelectVM<T>)d).UnselectedValue_PropertyChanged((T)(e.NewValue), (T)(e.OldValue), e.Property)));

        private void UnselectedValue_PropertyChanged(T newValue, T oldValue, DependencyProperty property)
        {
            CoerceValue(SelectedItemProperty);
        }

        #endregion

        #region Options Dependency Property Members

        private ObservableCollection<SelectableItemVM<T>> _innerOptions = new ObservableCollection<SelectableItemVM<T>>();
        private ReadOnlyObservableCollection<SelectableItemVM<T>> _currentOptions = null;
        
        public ReadOnlyObservableCollection<SelectableItemVM<T>> Options
        {
            get { return (ReadOnlyObservableCollection<SelectableItemVM<T>>)GetValue(OptionsProperty); }
            private set { SetValue(OptionsProperty, value); }
        }

        private static readonly DependencyPropertyKey OptionsPropertyKey = DependencyProperty.RegisterReadOnly("Options", typeof(ReadOnlyObservableCollection<SelectableItemVM<T>>), typeof(SingleSelectVM<T>),
            new PropertyMetadata(null, null, (d, baseValue) =>
            {
                SingleSelectVM<T> vm = (SingleSelectVM<T>)d;
                if (vm._currentOptions == null)
                    vm._currentOptions = new ReadOnlyObservableCollection<SelectableItemVM<T>>(vm._innerOptions);
                return vm._currentOptions;
            }));

        public static readonly DependencyProperty OptionsProperty = OptionsPropertyKey.DependencyProperty;

        #endregion

        public void Add(params SelectableItemVM<T>[] item) { Add(item as IEnumerable<SelectableItemVM<T>>); }

        public void Add(IEnumerable<SelectableItemVM<T>> item)
        {
            if (item == null)
                return;

            foreach (SelectableItemVM<T> i in item)
            {
                if (i == null || _innerOptions.Any(o => ReferenceEquals(i, o)))
                    continue;

                _innerOptions.Add(i);
                i.AddToParent(this);
                if (i.Parent != null && ReferenceEquals(i.Parent, this))
                {
                    i.SelectionChanged += Item_SelectionChanged;
                    i.ValueChanged += Item_ValueChanged;
                    i.GetCoercedValue += Item_GetCoercedValue;
                    if (i.IsSelected)
                        Item_SelectionChanged(i, new ValueChangedEventArgs<bool>(false, true));
                }
                else
                    _innerOptions.Remove(i);
            }
        }

        public void Remove(params SelectableItemVM<T>[] item) { Remove(item as IEnumerable<SelectableItemVM<T>>); }

        public void Remove(IEnumerable<SelectableItemVM<T>> item)
        {
            if (item == null)
                return;

            foreach (SelectableItemVM<T> i in item)
            {
                if (i == null || !_innerOptions.Any(o => ReferenceEquals(i, o)))
                    continue;

                if (i.IsSelected && SelectedItem != null && ReferenceEquals(i, SelectedItem))
                    SelectedItem = null;
                _innerOptions.Remove(i);
                i.RemoveFromParent(this);
                if (i.Parent != null && ReferenceEquals(i.Parent, this))
                    _innerOptions.Add(i);
                else
                {
                    i.SelectionChanged -= Item_SelectionChanged;
                    i.ValueChanged -= Item_ValueChanged;
                    i.GetCoercedValue -= Item_GetCoercedValue;
                }
            }
        }

        private void Item_SelectionChanged(object sender, ValueChangedEventArgs<bool> e)
        {
            SelectableItemVM<T> item = sender as SelectableItemVM<T>;
            if (item == null || !_innerOptions.Any(i => ReferenceEquals(i, item)))
                return;

            if (e.OldValue)
            {
                if (SelectedItem != null && ReferenceEquals(item, SelectedItem))
                    SelectedItem = _innerOptions.FirstOrDefault(i => i.IsSelected);
                return;
            }
            SelectableItemVM<T> previousItem = SelectedItem;
            if (ReferenceEquals(previousItem, item))
                return;
            SelectedItem = item;
            if (previousItem.IsSelected)
                previousItem.IsSelected = false;
        }

        private void Item_ValueChanged(object sender, DisplayValueChangedEventArgs<T> e)
        {
            EventHandler<ItemValueChangedEventArgs<T>> itemValueChanged = ItemValueChanged;
            if (itemValueChanged == null)
                return;

            ItemValueChangedEventArgs<T> args = new ItemValueChangedEventArgs<T>((SelectableItemVM<T>)sender, e.OldValue, e.NewValue, e.Displaytext);
            itemValueChanged(this, args);
            e.Displaytext = args.Displaytext;
        }

        private void Item_GetCoercedValue(object sender, CoerceValueEventArgs<T> e)
        {
            EventHandler<CoerceItemValueEventArgs<T>> getItemCoercedValue = GetItemCoercedValue;
            if (getItemCoercedValue == null)
                return;

            CoerceItemValueEventArgs<T> args = new CoerceItemValueEventArgs<T>((SelectableItemVM<T>)sender, e);
            getItemCoercedValue(this, args);
            e.CoercedValue = args.CoercedValue;
        }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}