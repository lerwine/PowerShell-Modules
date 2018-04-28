using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LteDev.WPF
{
    public class DependencyPropertyVM : DependencyObject
    {
        #region OwnerClass Property Members

        /// <summary>
        /// Defines the name for the <see cref="OwnerClass"/> dependency property.
        /// </summary>
        public const string DependencyPropertyName_OwnerClass = "OwnerClass";

        /// <summary>
        /// Identifies the <see cref="OwnerClass"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty OwnerClassProperty = DependencyProperty.Register(DependencyPropertyName_OwnerClass, typeof(string), typeof(DependencyPropertyVM),
                new PropertyMetadata("", null, CoerceNonNullString));

        /// <summary>
        /// Type name of owner class
        /// </summary>
        public string OwnerClass
        {
            get { return GetValue(OwnerClassProperty) as string; }
            set { SetValue(OwnerClassProperty, value); }
        }

        #endregion

        #region PropertyName Property Members

        /// <summary>
        /// Defines the name for the <see cref="PropertyName"/> dependency property.
        /// </summary>
        public const string DependencyPropertyName_PropertyName = "PropertyName";

        /// <summary>
        /// Identifies the <see cref="PropertyName"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PropertyNameProperty = DependencyProperty.Register(DependencyPropertyName_PropertyName, typeof(string), typeof(DependencyPropertyVM),
                new PropertyMetadata("", null, CoerceNonNullString));

        /// <summary>
        /// Name of property
        /// </summary>
        public string PropertyName
        {
            get { return GetValue(PropertyNameProperty) as string; }
            set { SetValue(PropertyNameProperty, value); }
        }

        #endregion

        #region PropertyTypeLabel Property Members

        public const string DefaulValue_PropertyTypeLabel_Property = "Property Type:";
        
        public const string DefaulValue_PropertyTypeLabel_Element = "Element Type:";
        
        /// <summary>
        /// Defines the name for the <see cref="PropertyTypeLabel"/> dependency property.
        /// </summary>
        public const string PropertyName_PropertyTypeLabel = "PropertyTypeLabel";

        private static readonly DependencyPropertyKey PropertyTypeLabelPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_PropertyTypeLabel, typeof(string), typeof(DependencyPropertyVM),
            new PropertyMetadata(DefaulValue_PropertyTypeLabel_Property));

        /// <summary>
        /// Identifies the <see cref="PropertyTypeLabel"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PropertyTypeLabelProperty = PropertyTypeLabelPropertyKey.DependencyProperty;

        /// <summary>
        /// Label text for property type
        /// </summary>
        public string PropertyTypeLabel
        {
            get { return GetValue(PropertyTypeLabelProperty) as string; }
            private set { SetValue(PropertyTypeLabelPropertyKey, value); }
        }

        #endregion

        #region PropertyTypeName Property Members

        /// <summary>
        /// Defines the name for the <see cref="PropertyTypeName"/> dependency property.
        /// </summary>
        public const string DependencyPropertyName_PropertyTypeName = "PropertyTypeName";

        /// <summary>
        /// Identifies the <see cref="PropertyTypeName"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PropertyTypeNameProperty = DependencyProperty.Register(DependencyPropertyName_PropertyTypeName, typeof(string), typeof(DependencyPropertyVM),
                new PropertyMetadata("", null, CoerceNonNullString));

        /// <summary>
        /// Type name of property
        /// </summary>
        public string PropertyTypeName
        {
            get { return GetValue(PropertyTypeNameProperty) as string; }
            set { SetValue(PropertyTypeNameProperty, value); }
        }

        #endregion

        #region DefaultValueLiteral Property Members

        /// <summary>
        /// Defines the name for the <see cref="DefaultValueLiteral"/> dependency property.
        /// </summary>
        public const string PropertyName_DefaultValueLiteral = "DefaultValueLiteral";

        private static readonly DependencyPropertyKey DefaultValueLiteralPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_DefaultValueLiteral, typeof(ImpliedAndExplicitVM<string>), typeof(DependencyPropertyVM),
            new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="DefaultValueLiteral"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DefaultValueLiteralProperty = DefaultValueLiteralPropertyKey.DependencyProperty;

        /// <summary>
        /// Label text for property type
        /// </summary>
        public ImpliedAndExplicitVM<string> DefaultValueLiteral
        {
            get { return GetValue(DefaultValueLiteralProperty) as ImpliedAndExplicitVM<string>; }
            private set { SetValue(DefaultValueLiteralPropertyKey, value); }
        }

        #endregion

        #region IsAttached Property Members

        /// <summary>
        /// Defines the name for the <see cref="IsAttached"/> dependency property.
        /// </summary>
        public const string DependencyPropertyName_IsAttached = "IsAttached";

        /// <summary>
        /// Identifies the <see cref="IsAttached"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsAttachedProperty = DependencyProperty.Register(DependencyPropertyName_IsAttached, typeof(bool), typeof(DependencyPropertyVM),
                new PropertyMetadata(false));

        /// <summary>
        /// Indicates that the property is attached.
        /// </summary>
        public bool IsAttached
        {
            get { return (bool)(GetValue(IsAttachedProperty)); }
            set { SetValue(IsAttachedProperty, value); }
        }

        #endregion
        
        #region IsReadOnly Property Members

        /// <summary>
        /// Defines the name for the <see cref="IsReadOnly"/> dependency property.
        /// </summary>
        public const string PropertyName_IsReadOnly = "IsReadOnly";

        private static readonly DependencyPropertyKey IsReadOnlyPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_IsReadOnly, typeof(ImpliedAndExplicitVM<bool>), typeof(DependencyPropertyVM),
            new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="IsReadOnly"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsReadOnlyProperty = IsReadOnlyPropertyKey.DependencyProperty;

        /// <summary>
        /// Label text for property type
        /// </summary>
        public ImpliedAndExplicitVM<bool> IsReadOnly
        {
            get { return GetValue(IsReadOnlyProperty) as ImpliedAndExplicitVM<bool>; }
            private set { SetValue(IsReadOnlyPropertyKey, value); }
        }

        #endregion

        #region CoerceNonNull Property Members

        /// <summary>
        /// Defines the name for the <see cref="CoerceNonNull"/> dependency property.
        /// </summary>
        public const string PropertyName_CoerceNonNull = "CoerceNonNull";

        private static readonly DependencyPropertyKey CoerceNonNullPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_CoerceNonNull, typeof(ImpliedAndExplicitVM<bool>), typeof(DependencyPropertyVM),
            new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="CoerceNonNull"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CoerceNonNullProperty = CoerceNonNullPropertyKey.DependencyProperty;

        /// <summary>
        /// Label text for property type
        /// </summary>
        public ImpliedAndExplicitVM<bool> CoerceNonNull
        {
            get { return GetValue(CoerceNonNullProperty) as ImpliedAndExplicitVM<bool>; }
            private set { SetValue(CoerceNonNullPropertyKey, value); }
        }

        #endregion

        #region IsCollection Property Members

        /// <summary>
        /// Defines the name for the <see cref="IsCollection"/> dependency property.
        /// </summary>
        public const string PropertyName_IsCollection = "IsCollection";

        private static readonly DependencyPropertyKey IsCollectionPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_IsCollection, typeof(ImpliedAndExplicitVM<bool>), typeof(DependencyPropertyVM),
            new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="IsCollection"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsCollectionProperty = IsCollectionPropertyKey.DependencyProperty;

        /// <summary>
        /// Label text for property type
        /// </summary>
        public ImpliedAndExplicitVM<bool> IsCollection
        {
            get { return GetValue(IsCollectionProperty) as ImpliedAndExplicitVM<bool>; }
            private set { SetValue(IsCollectionPropertyKey, value); }
        }

        #endregion

        #region IsReadOnlyCollection Property Members

        /// <summary>
        /// Defines the name for the <see cref="IsReadOnlyCollection"/> dependency property.
        /// </summary>
        public const string DependencyPropertyName_IsReadOnlyCollection = "IsReadOnlyCollection";

        /// <summary>
        /// Identifies the <see cref="IsReadOnlyCollection"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsReadOnlyCollectionProperty = DependencyProperty.Register(DependencyPropertyName_IsReadOnlyCollection, typeof(bool), typeof(DependencyPropertyVM),
                new PropertyMetadata(false, (d, e) => (d as DependencyPropertyVM).IsReadOnlyCollection_PropertyChanged((bool)(e.OldValue), (bool)(e.NewValue))));

        /// <summary>
        /// Indicates that the property is a read-only collection
        /// </summary>
        public bool IsReadOnlyCollection
        {
            get { return (bool)(GetValue(IsReadOnlyCollectionProperty)); }
            set { SetValue(IsReadOnlyCollectionProperty, value); }
        }

        #endregion

        #region HandlePropertyChanged Property Members

        /// <summary>
        /// Defines the name for the <see cref="HandlePropertyChanged"/> dependency property.
        /// </summary>
        public const string PropertyName_HandlePropertyChanged = "HandlePropertyChanged";

        private static readonly DependencyPropertyKey HandlePropertyChangedPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_HandlePropertyChanged, typeof(ImpliedAndExplicitVM<bool>), typeof(DependencyPropertyVM),
            new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="HandlePropertyChanged"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HandlePropertyChangedProperty = HandlePropertyChangedPropertyKey.DependencyProperty;

        /// <summary>
        /// Label text for property type
        /// </summary>
        public ImpliedAndExplicitVM<bool> HandlePropertyChanged
        {
            get { return GetValue(HandlePropertyChangedProperty) as ImpliedAndExplicitVM<bool>; }
            private set { SetValue(HandlePropertyChangedPropertyKey, value); }
        }

        #endregion

        #region HandleCoerceValue Property Members

        /// <summary>
        /// Defines the name for the <see cref="HandleCoerceValue"/> dependency property.
        /// </summary>
        public const string PropertyName_HandleCoerceValue = "HandleCoerceValue";

        private static readonly DependencyPropertyKey HandleCoerceValuePropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_HandleCoerceValue, typeof(ImpliedAndExplicitVM<bool>), typeof(DependencyPropertyVM),
            new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="HandleCoerceValue"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HandleCoerceValueProperty = HandleCoerceValuePropertyKey.DependencyProperty;

        /// <summary>
        /// Label text for property type
        /// </summary>
        public ImpliedAndExplicitVM<bool> HandleCoerceValue
        {
            get { return GetValue(HandleCoerceValueProperty) as ImpliedAndExplicitVM<bool>; }
            private set { SetValue(HandleCoerceValuePropertyKey, value); }
        }

        #endregion

        #region HandleCollectionChanged Property Members

        /// <summary>
        /// Defines the name for the <see cref="HandleCollectionChanged"/> dependency property.
        /// </summary>
        public const string PropertyName_HandleCollectionChanged = "HandleCollectionChanged";

        private static readonly DependencyPropertyKey HandleCollectionChangedPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_HandleCollectionChanged, typeof(ImpliedAndExplicitVM<bool>), typeof(DependencyPropertyVM),
            new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="HandleCollectionChanged"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HandleCollectionChangedProperty = HandleCollectionChangedPropertyKey.DependencyProperty;

        /// <summary>
        /// Label text for property type
        /// </summary>
        public ImpliedAndExplicitVM<bool> HandleCollectionChanged
        {
            get { return GetValue(HandleCollectionChangedProperty) as ImpliedAndExplicitVM<bool>; }
            private set { SetValue(HandleCollectionChangedPropertyKey, value); }
        }

        #endregion

        #region SetterControlsEnabled Property Members

        /// <summary>
        /// Defines the name for the <see cref="SetterControlsEnabled"/> dependency property.
        /// </summary>
        public const string PropertyName_SetterControlsEnabled = "SetterControlsEnabled";

        private static readonly DependencyPropertyKey SetterControlsEnabledPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_SetterControlsEnabled, typeof(bool), typeof(DependencyPropertyVM),
            new PropertyMetadata(false));

        /// <summary>
        /// Identifies the <see cref="SetterControlsEnabled"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SetterControlsEnabledProperty = SetterControlsEnabledPropertyKey.DependencyProperty;

        /// <summary>
        /// Whether setter controls are enabled.
        /// </summary>
        public bool SetterControlsEnabled
        {
            get { return (bool)(GetValue(SetterControlsEnabledProperty)); }
            private set { SetValue(SetterControlsEnabledPropertyKey, value); }
        }

        #endregion

        #region SetterIsProtected Property Members

        /// <summary>
        /// Defines the name for the <see cref="SetterIsProtected"/> dependency property.
        /// </summary>
        public const string DependencyPropertyName_SetterIsProtected = "SetterIsProtected";

        /// <summary>
        /// Identifies the <see cref="SetterIsProtected"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SetterIsProtectedProperty = DependencyProperty.Register(DependencyPropertyName_SetterIsProtected, typeof(bool), typeof(DependencyPropertyVM),
                new PropertyMetadata(false, (d, e) => (d as DependencyPropertyVM).SetterIsProtected_PropertyChanged((bool)(e.OldValue), (bool)(e.NewValue))));

        /// <summary>
        /// Property setter accessibility is protected.
        /// </summary>
        public bool SetterIsProtected
        {
            get { return (bool)(GetValue(SetterIsProtectedProperty)); }
            set { SetValue(SetterIsProtectedProperty, value); }
        }

        /// <summary>
        /// This gets called after the value associated with the <see cref="SetterIsProtected"/> dependency property has changed.
        /// </summary>
        /// <param name="oldValue">The <seealso cref="bool"/> value before the <seealso cref="SetterIsProtected"/> property was changed.</param>
        /// <param name="newValue">The <seealso cref="bool"/> value after the <seealso cref="SetterIsProtected"/> property was changed.</param>
        protected virtual void SetterIsProtected_PropertyChanged(bool oldValue, bool newValue)
        {
            if (oldValue)
                return;
            SetterIsPrivate = false;
            SetterIsInternal = false;
            SetterIsProtectedInternal = false;
        }

        #endregion

        #region SetterIsPrivate Property Members

        /// <summary>
        /// Defines the name for the <see cref="SetterIsPrivate"/> dependency property.
        /// </summary>
        public const string DependencyPropertyName_SetterIsPrivate = "SetterIsPrivate";

        /// <summary>
        /// Identifies the <see cref="SetterIsPrivate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SetterIsPrivateProperty = DependencyProperty.Register(DependencyPropertyName_SetterIsPrivate, typeof(bool), typeof(DependencyPropertyVM),
                new PropertyMetadata(false, (d, e) => (d as DependencyPropertyVM).SetterIsPrivate_PropertyChanged((bool)(e.OldValue), (bool)(e.NewValue))));

        /// <summary>
        /// Property setter accessibility is private.
        /// </summary>
        public bool SetterIsPrivate
        {
            get { return (bool)(GetValue(SetterIsPrivateProperty)); }
            set { SetValue(SetterIsPrivateProperty, value); }
        }

        /// <summary>
        /// This gets called after the value associated with the <see cref="SetterIsPrivate"/> dependency property has changed.
        /// </summary>
        /// <param name="oldValue">The <seealso cref="bool"/> value before the <seealso cref="SetterIsPrivate"/> property was changed.</param>
        /// <param name="newValue">The <seealso cref="bool"/> value after the <seealso cref="SetterIsPrivate"/> property was changed.</param>
        protected virtual void SetterIsPrivate_PropertyChanged(bool oldValue, bool newValue)
        {
            if (oldValue)
                return;
            SetterIsProtected = false;
            SetterIsInternal = false;
            SetterIsProtectedInternal = false;
        }

        #endregion

        #region SetterIsInternal Property Members

        /// <summary>
        /// Defines the name for the <see cref="SetterIsInternal"/> dependency property.
        /// </summary>
        public const string DependencyPropertyName_SetterIsInternal = "SetterIsInternal";

        /// <summary>
        /// Identifies the <see cref="SetterIsInternal"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SetterIsInternalProperty = DependencyProperty.Register(DependencyPropertyName_SetterIsInternal, typeof(bool), typeof(DependencyPropertyVM),
                new PropertyMetadata(false, (d, e) => (d as DependencyPropertyVM).SetterIsInternal_PropertyChanged((bool)(e.OldValue), (bool)(e.NewValue))));

        /// <summary>
        /// Property setter accessibility is internal.
        /// </summary>
        public bool SetterIsInternal
        {
            get { return (bool)(GetValue(SetterIsInternalProperty)); }
            set { SetValue(SetterIsInternalProperty, value); }
        }

        /// <summary>
        /// This gets called after the value associated with the <see cref="SetterIsInternal"/> dependency property has changed.
        /// </summary>
        /// <param name="oldValue">The <seealso cref="bool"/> value before the <seealso cref="SetterIsInternal"/> property was changed.</param>
        /// <param name="newValue">The <seealso cref="bool"/> value after the <seealso cref="SetterIsInternal"/> property was changed.</param>
        protected virtual void SetterIsInternal_PropertyChanged(bool oldValue, bool newValue)
        {
            if (oldValue)
                return;
            SetterIsProtected = false;
            SetterIsPrivate = false;
            SetterIsProtectedInternal = false;
        }

        #endregion

        #region SetterIsProtectedInternal Property Members

        /// <summary>
        /// Defines the name for the <see cref="SetterIsProtectedInternal"/> dependency property.
        /// </summary>
        public const string DependencyPropertyName_SetterIsProtectedInternal = "SetterIsProtectedInternal";

        /// <summary>
        /// Identifies the <see cref="SetterIsProtectedInternal"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SetterIsProtectedInternalProperty = DependencyProperty.Register(DependencyPropertyName_SetterIsProtectedInternal, typeof(bool), typeof(DependencyPropertyVM),
                new PropertyMetadata(false, (d, e) => (d as DependencyPropertyVM).SetterIsProtectedInternal_PropertyChanged((bool)(e.OldValue), (bool)(e.NewValue))));

        /// <summary>
        /// Property setter accessibility is protected internal.
        /// </summary>
        public bool SetterIsProtectedInternal
        {
            get { return (bool)(GetValue(SetterIsProtectedInternalProperty)); }
            set { SetValue(SetterIsProtectedInternalProperty, value); }
        }

        /// <summary>
        /// This gets called after the value associated with the <see cref="SetterIsProtectedInternal"/> dependency property has changed.
        /// </summary>
        /// <param name="oldValue">The <seealso cref="bool"/> value before the <seealso cref="SetterIsProtectedInternal"/> property was changed.</param>
        /// <param name="newValue">The <seealso cref="bool"/> value after the <seealso cref="SetterIsProtectedInternal"/> property was changed.</param>
        protected virtual void SetterIsProtectedInternal_PropertyChanged(bool oldValue, bool newValue)
        {
            if (oldValue)
                return;
            SetterIsProtected = false;
            SetterIsPrivate = false;
            SetterIsInternal = false;
        }

        #endregion

        public DependencyPropertyVM()
        {
            DefaultValueLiteral = new ImpliedAndExplicitVM<string>("");
            IsReadOnly = new ImpliedAndExplicitVM<bool>(false);
            CoerceNonNull = new ImpliedAndExplicitVM<bool>(false);
            IsCollection = new ImpliedAndExplicitVM<bool>(false);
            HandlePropertyChanged = new ImpliedAndExplicitVM<bool>(false);
            HandleCoerceValue = new ImpliedAndExplicitVM<bool>(false);
            HandleCollectionChanged = new ImpliedAndExplicitVM<bool>(false);
            DefaultValueLiteral.OnCoerceValue += CoerceNonNullString;
            IsCollection.ActualValueChanged += IsCollection_ActualValueChanged;
            IsReadOnly.ActualValueChanged += IsReadOnly_ActualValueChanged;
            CoerceNonNull.ActualValueChanged += CoerceNonNull_ActualValueChanged;
            HandleCollectionChanged.ActualValueChanged += HandleCollectionChanged_ActualValueChanged;
        }

        private void IsCollection_ActualValueChanged(object sender, EventArgs e)
        {
            HandleCollectionChanged.UseExplicit = IsCollection.ActualValue;
            DefaultValueLiteral.UseImplied = IsCollection.ActualValue;
            if (IsCollection.ActualValue)
                DefaultValueLiteral.ImpliedValue = "null";
            else
                HandleCollectionChanged.ImpliedValue = false;
        }

        private void IsReadOnly_ActualValueChanged(object sender, EventArgs e)
        {
            SetterControlsEnabled = IsReadOnly.ActualValue && !IsReadOnlyCollection;
        }

        private void CoerceNonNull_ActualValueChanged(object sender, EventArgs e)
        {
            HandleCoerceValue.UseImplied = CoerceNonNull.ActualValue;
            if (CoerceNonNull.ActualValue)
                HandleCoerceValue.ImpliedValue = true;
        }

        protected virtual void IsReadOnlyCollection_PropertyChanged(bool oldValue, bool newValue)
        {
            IsReadOnly.UseExplicit = oldValue;
            SetterControlsEnabled = oldValue && IsReadOnly.ActualValue;
            IsCollection.UseExplicit = oldValue;
            CoerceNonNull.UseExplicit = oldValue;
            if (oldValue)
                SetterIsProtected = true;
            else
            {
                IsReadOnly.ImpliedValue = true;
                IsCollection.ImpliedValue = true;
                CoerceNonNull.ImpliedValue = true;
                SetterIsPrivate = true;
            }
        }

        private void HandleCollectionChanged_ActualValueChanged(object sender, EventArgs e)
        {
            HandlePropertyChanged.UseImplied = HandleCollectionChanged.ActualValue;
            if (HandleCollectionChanged.ActualValue)
                HandlePropertyChanged.ImpliedValue = true;
        }

        internal static string CoerceNonNullString(DependencyObject d, object baseValue) { return (baseValue as string) ?? ""; }
    }
}
