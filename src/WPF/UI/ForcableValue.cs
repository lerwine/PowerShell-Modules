using System;
using System.Windows;

namespace Erwine.Leonard.T.WPF.UI
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class ForcableValue<T> : DependencyObject
    {
        #region SpecifiedValue Dependency Property Members

        public T SpecifiedValue
        {
            get { return (T)GetValue(SpecifiedValueProperty); }
            set { SetValue(SpecifiedValueProperty, value); }
        }

        public static readonly DependencyProperty SpecifiedValueProperty = DependencyProperty.Register("SpecifiedValue", typeof(T), typeof(ForcableValue<T>),
            new PropertyMetadata(default(T), (d, e) => ((ForcableValue<T>)d).SpecifiedValue_PropertyChanged((T)(e.NewValue), (T)(e.OldValue), e.Property)));

        private void SpecifiedValue_PropertyChanged(T newValue, T oldValue, DependencyProperty property)
        {
            if (NotOverridden)
                EffectiveValue = newValue;
        }

        #endregion

        #region OverriddenValue Dependency Property Members

        public T OverriddenValue
        {
            get { return (T)GetValue(OverriddenValueProperty); }
            private set { SetValue(OverriddenValuePropertyKey, value); }
        }

        private static readonly DependencyPropertyKey OverriddenValuePropertyKey = DependencyProperty.RegisterReadOnly("OverriddenValue", typeof(T), typeof(ForcableValue<T>),
            new PropertyMetadata(default(T)));

        public static readonly DependencyProperty OverriddenValueProperty = OverriddenValuePropertyKey.DependencyProperty;

        #endregion

        #region IsOverridden Dependency Property Members

        public bool IsOverridden
        {
            get { return (bool)GetValue(IsOverriddenProperty); }
            private set { SetValue(IsOverriddenPropertyKey, value); }
        }

        private static readonly DependencyPropertyKey IsOverriddenPropertyKey = DependencyProperty.RegisterReadOnly("IsOverridden", typeof(bool), typeof(ForcableValue<T>),
            new PropertyMetadata(false));

        public static readonly DependencyProperty IsOverriddenProperty = IsOverriddenPropertyKey.DependencyProperty;

        #endregion

        #region IsOverridden Dependency Property Members

        public bool NotOverridden
        {
            get { return (bool)GetValue(NotOverriddenProperty); }
            private set { SetValue(NotOverriddenPropertyKey, value); }
        }

        private static readonly DependencyPropertyKey NotOverriddenPropertyKey = DependencyProperty.RegisterReadOnly("NotOverridden", typeof(bool), typeof(ForcableValue<T>),
            new PropertyMetadata(false));

        public static readonly DependencyProperty NotOverriddenProperty = NotOverriddenPropertyKey.DependencyProperty;

        #endregion

        #region EffectiveValue Dependency Property Members

        public T EffectiveValue
        {
            get { return (T)GetValue(EffectiveValueProperty); }
            private set { SetValue(EffectiveValuePropertyKey, value); }
        }

        private static readonly DependencyPropertyKey EffectiveValuePropertyKey = DependencyProperty.RegisterReadOnly("EffectiveValue", typeof(T), typeof(ForcableValue<T>),
            new PropertyMetadata(default(T), (d, e) => ((ForcableValue<T>)d).EffectiveValue_PropertyChanged((T)(e.NewValue), (T)(e.OldValue), e.Property), (d, baseValue) =>
            {
                ForcableValue<T> vm = (ForcableValue<T>)d;
                if (vm.IsOverridden)
                    return vm.OverriddenValue;
                return vm.SpecifiedValue;
            }));

        public static readonly DependencyProperty EffectiveValueProperty = EffectiveValuePropertyKey.DependencyProperty;

        private void EffectiveValue_PropertyChanged(T newValue, T oldValue, DependencyProperty property)
        {
            EventHandler<ValueChangedEventArgs<T>> effectiveValueChanged = EffectiveValueChanged;
            if (effectiveValueChanged != null)
                effectiveValueChanged(this, new ValueChangedEventArgs<T>(oldValue, newValue));
        }

        public event EventHandler<ValueChangedEventArgs<T>> EffectiveValueChanged;
        #endregion

        public void SetOverrideValue(T value)
        {
            OverriddenValue = value;
            IsOverridden = true;
            NotOverridden = false;
            EffectiveValue = value;
        }

        public void UndoOverride()
        {
            OverriddenValue = SpecifiedValue;
            IsOverridden = false;
            NotOverridden = true;
            EffectiveValue = SpecifiedValue;
        }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}