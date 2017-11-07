using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Erwine.Leonard.T.WPF.UI
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class DependencyPropertyBuilderVM : DependencyObject
    {
        #region OwnerTypeName Dependency Property Members

        public string OwnerTypeName
        {
            get { return (string)GetValue(OwnerTypeNameProperty); }
            set { SetValue(OwnerTypeNameProperty, value); }
        }

        public static readonly DependencyProperty OwnerTypeNameProperty = DependencyProperty.Register("OwnerTypeName", typeof(string), typeof(DependencyPropertyBuilderVM),
            new PropertyMetadata("", (d, e) => ((DependencyPropertyBuilderVM)d).OwnerTypeName_PropertyChanged((string)(e.NewValue), (string)(e.OldValue), e.Property),
                (d, baseValue) => ((DependencyPropertyBuilderVM)d).OwnerTypeName_CoerceValue(baseValue)));

        private void OwnerTypeName_PropertyChanged(string newValue, string oldValue, DependencyProperty property)
        {
        }

        private object OwnerTypeName_CoerceValue(object baseValue)
        {
            string s = baseValue as string;
            return (s == null) ? "" : s;
        }

        #endregion

        #region PropertyName Dependency Property Members

        public string PropertyName
        {
            get { return (string)GetValue(PropertyNameProperty); }
            set { SetValue(PropertyNameProperty, value); }
        }

        public static readonly DependencyProperty PropertyNameProperty = DependencyProperty.Register("PropertyName", typeof(string), typeof(DependencyPropertyBuilderVM),
            new PropertyMetadata("", (d, e) => ((DependencyPropertyBuilderVM)d).PropertyName_PropertyChanged((string)(e.NewValue), (string)(e.OldValue), e.Property),
                (d, baseValue) => ((DependencyPropertyBuilderVM)d).PropertyName_CoerceValue(baseValue)));

        private void PropertyName_PropertyChanged(string newValue, string oldValue, DependencyProperty property)
        {
        }

        private object PropertyName_CoerceValue(object baseValue)
        {
            string s = baseValue as string;
            return (s == null) ? "" : s;
        }
        
        #endregion

        #region PropertyTypeName Dependency Property Members

        public string PropertyTypeName
        {
            get { return (string)GetValue(PropertyTypeNameProperty); }
            set { SetValue(PropertyTypeNameProperty, value); }
        }

        public static readonly DependencyProperty PropertyTypeNameProperty = DependencyProperty.Register("PropertyTypeName", typeof(string), typeof(DependencyPropertyBuilderVM),
            new PropertyMetadata("", (d, e) => ((DependencyPropertyBuilderVM)d).PropertyTypeName_PropertyChanged((string)(e.NewValue), (string)(e.OldValue), e.Property),
                (d, baseValue) => ((DependencyPropertyBuilderVM)d).PropertyTypeName_CoerceValue(baseValue)));

        private void PropertyTypeName_PropertyChanged(string newValue, string oldValue, DependencyProperty property)
        {
        }

        private object PropertyTypeName_CoerceValue(object baseValue)
        {
            string s = baseValue as string;
            return (s == null) ? "" : s;
        }

        #endregion

        #region DefaultValueLiteral Dependency Property Members

        public string DefaultValueLiteral
        {
            get { return (string)GetValue(DefaultValueLiteralProperty); }
            set { SetValue(DefaultValueLiteralProperty, value); }
        }

        public static readonly DependencyProperty DefaultValueLiteralProperty = DependencyProperty.Register("DefaultValueLiteral", typeof(string), typeof(DependencyPropertyBuilderVM),
            new PropertyMetadata("", (d, e) => ((DependencyPropertyBuilderVM)d).DefaultValueLiteral_PropertyChanged((string)(e.NewValue), (string)(e.OldValue), e.Property)));

        private void DefaultValueLiteral_PropertyChanged(string newValue, string oldValue, DependencyProperty property)
        {
            if (DefaultValueLiteralEnabled)
                Last_DefaultValueLiteral = newValue;
        }

        #endregion

        #region DefaultValueLiteralEnabled Dependency Property Members

        public bool DefaultValueLiteralEnabled
        {
            get { return (bool)GetValue(DefaultValueLiteralEnabledProperty); }
            private set { SetValue(DefaultValueLiteralEnabledPropertyKey, value); }
        }

        public static readonly DependencyPropertyKey DefaultValueLiteralEnabledPropertyKey = DependencyProperty.RegisterReadOnly("DefaultValueLiteralEnabled", typeof(bool), typeof(DependencyPropertyBuilderVM),
            new PropertyMetadata(true, (d, e) => ((DependencyPropertyBuilderVM)d).DefaultValueLiteralEnabled_PropertyChanged((bool)(e.NewValue), (bool)(e.OldValue), e.Property)));

        public static readonly DependencyProperty DefaultValueLiteralEnabledProperty = DefaultValueLiteralEnabledPropertyKey.DependencyProperty;

        private void DefaultValueLiteralEnabled_PropertyChanged(bool newValue, bool oldValue, DependencyProperty property)
        {
            if (oldValue)
                DefaultValueLiteral = Last_DefaultValueLiteral;
        }

        #endregion

        #region Last_DefaultValueLiteral Dependency Property Members

        public string Last_DefaultValueLiteral
        {
            get { return (string)GetValue(Last_DefaultValueLiteralProperty); }
            private set { SetValue(Last_DefaultValueLiteralPropertyKey, value); }
        }

        public static readonly DependencyPropertyKey Last_DefaultValueLiteralPropertyKey = DependencyProperty.RegisterReadOnly("Last_DefaultValueLiteral", typeof(string), typeof(DependencyPropertyBuilderVM),
            new PropertyMetadata(""));

        public static readonly DependencyProperty Last_DefaultValueLiteralProperty = Last_DefaultValueLiteralPropertyKey.DependencyProperty;

        #endregion

        #region Attached Dependency Property Members

        public bool Attached
        {
            get { return (bool)GetValue(AttachedProperty); }
            set { SetValue(AttachedProperty, value); }
        }

        public static readonly DependencyProperty AttachedProperty = DependencyProperty.Register("Attached", typeof(bool), typeof(DependencyPropertyBuilderVM),
            new PropertyMetadata(false, (d, e) => ((DependencyPropertyBuilderVM)d).Attached_PropertyChanged((bool)(e.NewValue), (bool)(e.OldValue), e.Property)));

        private void Attached_PropertyChanged(bool newValue, bool oldValue, DependencyProperty property)
        {
        }

        #endregion

        #region ReadOnly Dependency Property Members

        public bool ReadOnly
        {
            get { return (bool)GetValue(ReadOnlyProperty); }
            set { SetValue(ReadOnlyProperty, value); }
        }

        public static readonly DependencyProperty ReadOnlyProperty = DependencyProperty.Register("ReadOnly", typeof(bool), typeof(DependencyPropertyBuilderVM),
            new PropertyMetadata(false, (d, e) => ((DependencyPropertyBuilderVM)d).ReadOnly_PropertyChanged((bool)(e.NewValue), (bool)(e.OldValue), e.Property)));

        private void ReadOnly_PropertyChanged(bool newValue, bool oldValue, DependencyProperty property)
        {
            SetterAccessibilityEnabled = newValue && !ReadOnlyCollection;
            if (ReadOnlyEnabled)
                Last_ReadOnly = newValue;
        }

        #endregion

        #region ReadOnlyEnabled Dependency Property Members

        public bool ReadOnlyEnabled
        {
            get { return (bool)GetValue(ReadOnlyEnabledProperty); }
            private set { SetValue(ReadOnlyEnabledPropertyKey, value); }
        }

        public static readonly DependencyPropertyKey ReadOnlyEnabledPropertyKey = DependencyProperty.RegisterReadOnly("ReadOnlyEnabled", typeof(bool), typeof(DependencyPropertyBuilderVM),
            new PropertyMetadata(true, (d, e) => ((DependencyPropertyBuilderVM)d).ReadOnlyEnabled_PropertyChanged((bool)(e.NewValue), (bool)(e.OldValue), e.Property)));

        public static readonly DependencyProperty ReadOnlyEnabledProperty = ReadOnlyEnabledPropertyKey.DependencyProperty;

        private void ReadOnlyEnabled_PropertyChanged(bool newValue, bool oldValue, DependencyProperty property)
        {
            if (oldValue)
                ReadOnly = Last_ReadOnly;
        }

        #endregion

        #region Last_ReadOnly Dependency Property Members

        public bool Last_ReadOnly
        {
            get { return (bool)GetValue(Last_ReadOnlyProperty); }
            private set { SetValue(Last_ReadOnlyPropertyKey, value); }
        }

        public static readonly DependencyPropertyKey Last_ReadOnlyPropertyKey = DependencyProperty.RegisterReadOnly("Last_ReadOnly", typeof(bool), typeof(DependencyPropertyBuilderVM),
            new PropertyMetadata(false));

        public static readonly DependencyProperty Last_ReadOnlyProperty = Last_ReadOnlyPropertyKey.DependencyProperty;

        #endregion

        #region Collection Dependency Property Members

        public bool Collection
        {
            get { return (bool)GetValue(CollectionProperty); }
            set { SetValue(CollectionProperty, value); }
        }

        public static readonly DependencyProperty CollectionProperty = DependencyProperty.Register("Collection", typeof(bool), typeof(DependencyPropertyBuilderVM),
            new PropertyMetadata(false, (d, e) => ((DependencyPropertyBuilderVM)d).Collection_PropertyChanged((bool)(e.NewValue), (bool)(e.OldValue), e.Property)));

        private void Collection_PropertyChanged(bool newValue, bool oldValue, DependencyProperty property)
        {
            AllowNullEnabled = newValue;
            HandleCollectionChangedEnabled = newValue;
            DefaultValueLiteralEnabled = oldValue;
            if (CollectionEnabled)
                Last_Collection = newValue;
            if (newValue)
            {
                AllowNullEnabled = true;
                DefaultValueLiteral = "null";
            }
        }

        #endregion

        #region Last_Collection Dependency Property Members

        public bool Last_Collection
        {
            get { return (bool)GetValue(Last_CollectionProperty); }
            private set { SetValue(Last_CollectionPropertyKey, value); }
        }

        public static readonly DependencyPropertyKey Last_CollectionPropertyKey = DependencyProperty.RegisterReadOnly("Last_Collection", typeof(bool), typeof(DependencyPropertyBuilderVM),
            new PropertyMetadata(false));

        public static readonly DependencyProperty Last_CollectionProperty = Last_CollectionPropertyKey.DependencyProperty;

        #endregion

        #region CollectionEnabled Dependency Property Members

        public bool CollectionEnabled
        {
            get { return (bool)GetValue(CollectionEnabledProperty); }
            private set { SetValue(CollectionEnabledPropertyKey, value); }
        }

        public static readonly DependencyPropertyKey CollectionEnabledPropertyKey = DependencyProperty.RegisterReadOnly("CollectionEnabled", typeof(bool), typeof(DependencyPropertyBuilderVM),
            new PropertyMetadata(true, (d, e) => ((DependencyPropertyBuilderVM)d).CollectionEnabled_PropertyChanged((bool)(e.NewValue), (bool)(e.OldValue), e.Property)));

        public static readonly DependencyProperty CollectionEnabledProperty = CollectionEnabledPropertyKey.DependencyProperty;

        private void CollectionEnabled_PropertyChanged(bool newValue, bool oldValue, DependencyProperty property)
        {
            if (oldValue)
                Collection = Last_Collection;
        }

        #endregion

        #region AllowNull Dependency Property Members

        public bool AllowNull
        {
            get { return (bool)GetValue(AllowNullProperty); }
            set { SetValue(AllowNullProperty, value); }
        }

        public static readonly DependencyProperty AllowNullProperty = DependencyProperty.Register("AllowNull", typeof(bool), typeof(DependencyPropertyBuilderVM),
            new PropertyMetadata(false, (d, e) => ((DependencyPropertyBuilderVM)d).AllowNull_PropertyChanged((bool)(e.NewValue), (bool)(e.OldValue), e.Property)));

        private void AllowNull_PropertyChanged(bool newValue, bool oldValue, DependencyProperty property)
        {
            if (AllowNullEnabled)
                Last_AllowNull = newValue;
            HandleCoerceValueEnabled = newValue;
            if (oldValue)
                HandleCoerceValue = true;
        }

        #endregion

        #region Last_AllowNull Dependency Property Members

        public bool Last_AllowNull
        {
            get { return (bool)GetValue(Last_AllowNullProperty); }
            private set { SetValue(Last_AllowNullPropertyKey, value); }
        }

        public static readonly DependencyPropertyKey Last_AllowNullPropertyKey = DependencyProperty.RegisterReadOnly("Last_AllowNull", typeof(bool), typeof(DependencyPropertyBuilderVM),
            new PropertyMetadata(false));

        public static readonly DependencyProperty Last_AllowNullProperty = Last_AllowNullPropertyKey.DependencyProperty;

        #endregion

        #region AllowNullEnabled Dependency Property Members

        public bool AllowNullEnabled
        {
            get { return (bool)GetValue(AllowNullEnabledProperty); }
            private set { SetValue(AllowNullEnabledPropertyKey, value); }
        }

        public static readonly DependencyPropertyKey AllowNullEnabledPropertyKey = DependencyProperty.RegisterReadOnly("AllowNullEnabled", typeof(bool), typeof(DependencyPropertyBuilderVM),
            new PropertyMetadata(false, (d, e) => ((DependencyPropertyBuilderVM)d).AllowNullEnabled_PropertyChanged((bool)(e.NewValue), (bool)(e.OldValue), e.Property)));

        public static readonly DependencyProperty AllowNullEnabledProperty = AllowNullEnabledPropertyKey.DependencyProperty;

        private void AllowNullEnabled_PropertyChanged(bool newValue, bool oldValue, DependencyProperty property)
        {
            if (oldValue)
                AllowNull = Last_AllowNull;
        }

        #endregion

        #region HandlePropertyChanged Dependency Property Members

        public bool HandlePropertyChanged
        {
            get { return (bool)GetValue(HandlePropertyChangedProperty); }
            set { SetValue(HandlePropertyChangedProperty, value); }
        }

        public static readonly DependencyProperty HandlePropertyChangedProperty = DependencyProperty.Register("HandlePropertyChanged", typeof(bool), typeof(DependencyPropertyBuilderVM),
            new PropertyMetadata(false, (d, e) => ((DependencyPropertyBuilderVM)d).HandlePropertyChanged_PropertyChanged((bool)(e.NewValue), (bool)(e.OldValue), e.Property)));

        private void HandlePropertyChanged_PropertyChanged(bool newValue, bool oldValue, DependencyProperty property)
        {
            if (HandlePropertyChangedEnabled)
                Last_HandlePropertyChanged = newValue;
        }

        #endregion

        #region Last_HandlePropertyChanged Dependency Property Members

        public bool Last_HandlePropertyChanged
        {
            get { return (bool)GetValue(Last_HandlePropertyChangedProperty); }
            private set { SetValue(Last_HandlePropertyChangedPropertyKey, value); }
        }

        public static readonly DependencyPropertyKey Last_HandlePropertyChangedPropertyKey = DependencyProperty.RegisterReadOnly("Last_HandlePropertyChanged", typeof(bool), typeof(DependencyPropertyBuilderVM),
            new PropertyMetadata(false));

        public static readonly DependencyProperty Last_HandlePropertyChangedProperty = Last_HandlePropertyChangedPropertyKey.DependencyProperty;

        #endregion

        #region HandlePropertyChangedEnabled Dependency Property Members

        public bool HandlePropertyChangedEnabled
        {
            get { return (bool)GetValue(HandlePropertyChangedEnabledProperty); }
            private set { SetValue(HandlePropertyChangedEnabledPropertyKey, value); }
        }

        public static readonly DependencyPropertyKey HandlePropertyChangedEnabledPropertyKey = DependencyProperty.RegisterReadOnly("HandlePropertyChangedEnabled", typeof(bool), typeof(DependencyPropertyBuilderVM),
            new PropertyMetadata(true, (d, e) => ((DependencyPropertyBuilderVM)d).HandlePropertyChangedEnabled_PropertyChanged((bool)(e.NewValue), (bool)(e.OldValue), e.Property)));

        public static readonly DependencyProperty HandlePropertyChangedEnabledProperty = HandlePropertyChangedEnabledPropertyKey.DependencyProperty;

        private void HandlePropertyChangedEnabled_PropertyChanged(bool newValue, bool oldValue, DependencyProperty property)
        {
            if (oldValue)
                HandlePropertyChanged = Last_HandlePropertyChanged;
        }

        #endregion

        #region HandleCoerceValue Dependency Property Members

        public bool HandleCoerceValue
        {
            get { return (bool)GetValue(HandleCoerceValueProperty); }
            set { SetValue(HandleCoerceValueProperty, value); }
        }

        public static readonly DependencyProperty HandleCoerceValueProperty = DependencyProperty.Register("HandleCoerceValue", typeof(bool), typeof(DependencyPropertyBuilderVM),
            new PropertyMetadata(false, (d, e) => ((DependencyPropertyBuilderVM)d).HandleCoerceValue_PropertyChanged((bool)(e.NewValue), (bool)(e.OldValue), e.Property)));

        private void HandleCoerceValue_PropertyChanged(bool newValue, bool oldValue, DependencyProperty property)
        {
            if (HandleCoerceValueEnabled)
                Last_HandleCoerceValue = newValue;
        }

        #endregion

        #region Last_HandleCoerceValue Dependency Property Members

        public bool Last_HandleCoerceValue
        {
            get { return (bool)GetValue(Last_HandleCoerceValueProperty); }
            private set { SetValue(Last_HandleCoerceValuePropertyKey, value); }
        }

        public static readonly DependencyPropertyKey Last_HandleCoerceValuePropertyKey = DependencyProperty.RegisterReadOnly("Last_HandleCoerceValue", typeof(bool), typeof(DependencyPropertyBuilderVM),
            new PropertyMetadata(false));

        public static readonly DependencyProperty Last_HandleCoerceValueProperty = Last_HandleCoerceValuePropertyKey.DependencyProperty;

        #endregion

        #region HandleCoerceValueEnabled Dependency Property Members

        public bool HandleCoerceValueEnabled
        {
            get { return (bool)GetValue(HandleCoerceValueEnabledProperty); }
            private set { SetValue(HandleCoerceValueEnabledPropertyKey, value); }
        }

        public static readonly DependencyPropertyKey HandleCoerceValueEnabledPropertyKey = DependencyProperty.RegisterReadOnly("HandleCoerceValueEnabled", typeof(bool), typeof(DependencyPropertyBuilderVM),
            new PropertyMetadata(true, (d, e) => ((DependencyPropertyBuilderVM)d).HandleCoerceValueEnabled_PropertyChanged((bool)(e.NewValue), (bool)(e.OldValue), e.Property)));

        public static readonly DependencyProperty HandleCoerceValueEnabledProperty = HandleCoerceValueEnabledPropertyKey.DependencyProperty;

        private void HandleCoerceValueEnabled_PropertyChanged(bool newValue, bool oldValue, DependencyProperty property)
        {
            if (oldValue)
                HandleCoerceValue = Last_HandleCoerceValue;
        }

        #endregion

        #region HandleCollectionChanged Dependency Property Members

        public bool HandleCollectionChanged
        {
            get { return (bool)GetValue(HandleCollectionChangedProperty); }
            set { SetValue(HandleCollectionChangedProperty, value); }
        }

        public static readonly DependencyProperty HandleCollectionChangedProperty = DependencyProperty.Register("HandleCollectionChanged", typeof(bool), typeof(DependencyPropertyBuilderVM),
            new PropertyMetadata(false, (d, e) => ((DependencyPropertyBuilderVM)d).HandleCollectionChanged_PropertyChanged((bool)(e.NewValue), (bool)(e.OldValue), e.Property)));

        private void HandleCollectionChanged_PropertyChanged(bool newValue, bool oldValue, DependencyProperty property)
        {
            if (HandleCollectionChangedEnabled)
                Last_HandleCollectionChanged = newValue;
            HandlePropertyChangedEnabled = oldValue;
            if (newValue)
                HandlePropertyChanged = true;
        }

        #endregion

        #region Last_HandleCollectionChanged Dependency Property Members

        public bool Last_HandleCollectionChanged
        {
            get { return (bool)GetValue(Last_HandleCollectionChangedProperty); }
            private set { SetValue(Last_HandleCollectionChangedPropertyKey, value); }
        }

        public static readonly DependencyPropertyKey Last_HandleCollectionChangedPropertyKey = DependencyProperty.RegisterReadOnly("Last_HandleCollectionChanged", typeof(bool), typeof(DependencyPropertyBuilderVM),
            new PropertyMetadata(false));

        public static readonly DependencyProperty Last_HandleCollectionChangedProperty = Last_HandleCollectionChangedPropertyKey.DependencyProperty;

        #endregion

        #region HandleCollectionChangedEnabled Dependency Property Members

        public bool HandleCollectionChangedEnabled
        {
            get { return (bool)GetValue(HandleCollectionChangedEnabledProperty); }
            private set { SetValue(HandleCollectionChangedEnabledPropertyKey, value); }
        }

        public static readonly DependencyPropertyKey HandleCollectionChangedEnabledPropertyKey = DependencyProperty.RegisterReadOnly("HandleCollectionChangedEnabled", typeof(bool), typeof(DependencyPropertyBuilderVM),
            new PropertyMetadata(false, (d, e) => ((DependencyPropertyBuilderVM)d).HandleCollectionChangedEnabled_PropertyChanged((bool)(e.NewValue), (bool)(e.OldValue), e.Property)));

        public static readonly DependencyProperty HandleCollectionChangedEnabledProperty = HandleCollectionChangedEnabledPropertyKey.DependencyProperty;

        private void HandleCollectionChangedEnabled_PropertyChanged(bool newValue, bool oldValue, DependencyProperty property)
        {
            if (oldValue)
                HandleCollectionChanged = Last_HandleCollectionChanged;
        }

        #endregion

        #region HandleValidateValue Dependency Property Members

        public bool HandleValidateValue
        {
            get { return (bool)GetValue(HandleValidateValueProperty); }
            set { SetValue(HandleValidateValueProperty, value); }
        }

        public static readonly DependencyProperty HandleValidateValueProperty = DependencyProperty.Register("HandleValidateValue", typeof(bool), typeof(DependencyPropertyBuilderVM),
            new PropertyMetadata(false, (d, e) => ((DependencyPropertyBuilderVM)d).HandleValidateValue_PropertyChanged((bool)(e.NewValue), (bool)(e.OldValue), e.Property)));

        private void HandleValidateValue_PropertyChanged(bool newValue, bool oldValue, DependencyProperty property)
        {
        }

        #endregion
        
        #region ReadOnlyCollection Dependency Property Members

        public bool ReadOnlyCollection
        {
            get { return (bool)GetValue(ReadOnlyCollectionProperty); }
            set { SetValue(ReadOnlyCollectionProperty, value); }
        }

        public static readonly DependencyProperty ReadOnlyCollectionProperty = DependencyProperty.Register("ReadOnlyCollection", typeof(bool), typeof(DependencyPropertyBuilderVM),
            new PropertyMetadata(false, (d, e) => ((DependencyPropertyBuilderVM)d).ReadOnlyCollection_PropertyChanged((bool)(e.NewValue), (bool)(e.OldValue), e.Property)));

        private void ReadOnlyCollection_PropertyChanged(bool newValue, bool oldValue, DependencyProperty property)
        {
            SetterAccessibilityEnabled = oldValue && ReadOnly;
            ReadOnlyEnabled = oldValue;
            CollectionEnabled = oldValue;
            AllowNullEnabled = oldValue;
            if (newValue)
            {
                ReadOnly = true;
                Collection = true;
                AllowNullEnabled = false;
                AllowNull = false;
                PrivateSetter = true;
            }
        }

        #endregion

        #region SetterAccessibilityEnabled Dependency Property Members

        public bool SetterAccessibilityEnabled
        {
            get { return (bool)GetValue(SetterAccessibilityEnabledProperty); }
            private set { SetValue(SetterAccessibilityEnabledPropertyKey, value); }
        }

        public static readonly DependencyPropertyKey SetterAccessibilityEnabledPropertyKey = DependencyProperty.RegisterReadOnly("SetterAccessibilityEnabled", typeof(bool), typeof(DependencyPropertyBuilderVM),
            new PropertyMetadata(false));

        public static readonly DependencyProperty SetterAccessibilityEnabledProperty = SetterAccessibilityEnabledPropertyKey.DependencyProperty;

        #endregion

        #region PrivateSetter Dependency Property Members

        public bool PrivateSetter
        {
            get { return (bool)GetValue(PrivateSetterProperty); }
            set { SetValue(PrivateSetterProperty, value); }
        }

        public static readonly DependencyProperty PrivateSetterProperty = DependencyProperty.Register("PrivateSetter", typeof(bool), typeof(DependencyPropertyBuilderVM),
            new PropertyMetadata(false, (d, e) => ((DependencyPropertyBuilderVM)d).PrivateSetter_PropertyChanged((bool)(e.NewValue), (bool)(e.OldValue), e.Property)));

        private void PrivateSetter_PropertyChanged(bool newValue, bool oldValue, DependencyProperty property)
        {
            if (newValue)
            {
                ProtectedInternalSetter = false;
                InternalSetter = false;
                ProtectedSetter = false;
            }
            else if (ReadOnly && !(ReadOnlyCollection || ProtectedInternalSetter || InternalSetter || ProtectedSetter))
                ProtectedSetter = true;
        }

        #endregion

        #region ProtectedSetter Dependency Property Members

        public bool ProtectedSetter
        {
            get { return (bool)GetValue(ProtectedSetterProperty); }
            set { SetValue(ProtectedSetterProperty, value); }
        }

        public static readonly DependencyProperty ProtectedSetterProperty = DependencyProperty.Register("ProtectedSetter", typeof(bool), typeof(DependencyPropertyBuilderVM),
            new PropertyMetadata(false, (d, e) => ((DependencyPropertyBuilderVM)d).ProtectedSetter_PropertyChanged((bool)(e.NewValue), (bool)(e.OldValue), e.Property)));

        private void ProtectedSetter_PropertyChanged(bool newValue, bool oldValue, DependencyProperty property)
        {
            if (newValue)
            {
                ProtectedInternalSetter = false;
                InternalSetter = false;
                PrivateSetter = false;
            }
            else if (ReadOnly && !(ReadOnlyCollection || ProtectedInternalSetter || InternalSetter || PrivateSetter))
                PrivateSetter = true;
        }

        #endregion

        #region InternalSetter Dependency Property Members

        public bool InternalSetter
        {
            get { return (bool)GetValue(InternalSetterProperty); }
            set { SetValue(InternalSetterProperty, value); }
        }

        public static readonly DependencyProperty InternalSetterProperty = DependencyProperty.Register("InternalSetter", typeof(bool), typeof(DependencyPropertyBuilderVM),
            new PropertyMetadata(false, (d, e) => ((DependencyPropertyBuilderVM)d).InternalSetter_PropertyChanged((bool)(e.NewValue), (bool)(e.OldValue), e.Property)));

        private void InternalSetter_PropertyChanged(bool newValue, bool oldValue, DependencyProperty property)
        {
            if (newValue)
            {
                ProtectedInternalSetter = false;
                ProtectedSetter = false;
                PrivateSetter = false;
            }
            else if (ReadOnly && !(ReadOnlyCollection || ProtectedInternalSetter || ProtectedSetter || PrivateSetter))
                PrivateSetter = true;
        }

        #endregion

        #region ProtectedInternalSetter Dependency Property Members

        public bool ProtectedInternalSetter
        {
            get { return (bool)GetValue(ProtectedInternalSetterProperty); }
            set { SetValue(ProtectedInternalSetterProperty, value); }
        }

        public static readonly DependencyProperty ProtectedInternalSetterProperty = DependencyProperty.Register("ProtectedInternalSetter", typeof(bool), typeof(DependencyPropertyBuilderVM),
            new PropertyMetadata(false, (d, e) => ((DependencyPropertyBuilderVM)d).ProtectedInternalSetter_PropertyChanged((bool)(e.NewValue), (bool)(e.OldValue), e.Property)));

        private void ProtectedInternalSetter_PropertyChanged(bool newValue, bool oldValue, DependencyProperty property)
        {
            if (newValue)
            {
                InternalSetter = false;
                ProtectedSetter = false;
                PrivateSetter = false;
            }
            else if (ReadOnly && !(ReadOnlyCollection || InternalSetter || ProtectedSetter || PrivateSetter))
                PrivateSetter = true;
        }

        #endregion
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
