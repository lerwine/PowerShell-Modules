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

        // /// <summary>
        // /// Occurs when the value of <see cref="OwnerClass"/> has changed.
        // /// </summary>
        // public event EventHandler OwnerClassPropertyChanged;

        /// <summary>
        /// Defines the name for the <see cref="OwnerClass"/> dependency property.
        /// </summary>
        public const string DependencyPropertyName_OwnerClass = "OwnerClass";

        /// <summary>
        /// Identifies the <see cref="OwnerClass"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty OwnerClassProperty = DependencyProperty.Register(DependencyPropertyName_OwnerClass, typeof(string), typeof(DependencyPropertyVM),
                new PropertyMetadata("", (d, e) => (d as DependencyPropertyVM).OwnerClass_PropertyChanged(e.OldValue as string, e.NewValue as string),
                    (d, b) => (d as DependencyPropertyVM).OwnerClass_CoerceValue(b)));

        // TODO: Add summary documentation to the OwnerClass Property.
        /// <summary>
        /// 
        /// </summary>
        public string OwnerClass
        {
            get { return GetValue(OwnerClassProperty) as string; }
            set { SetValue(OwnerClassProperty, value); }
        }

        /// <summary>
        /// This gets called after the value associated with the <see cref="OwnerClass"/> dependency property has changed.
        /// </summary>
        /// <param name="oldValue">The <seealso cref="string"/> value before the <see cref="OwnerClass"/> property was changed.</param>
        /// <param name="newValue">The <seealso cref="string"/> value after the <see cref="OwnerClass"/> property was changed.</param>
        protected virtual void OwnerClass_PropertyChanged(string oldValue, string newValue)
        {
            // TODO: Implement DependencyPropertyVM.OwnerClass_PropertyChanged(string, string)
            // OwnerClassPropertyChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// This gets called whenever <see cref="OwnerClass"/> is being re-evaluated, or coercion is specifically requested.
        /// </summary>
        /// <param name="baseValue">The new value of the property, prior to any coercion attempt.</param>
        /// <returns>The coerced value.</returns>
        public virtual string OwnerClass_CoerceValue(object baseValue)
        {
            // TODO: Implement DependencyPropertyVM.OwnerClass_CoerceValue(DependencyObject, object)
            return (baseValue as string) ?? "";
        }

        #endregion

        #region PropertyName Property Members

        // /// <summary>
        // /// Occurs when the value of <see cref="PropertyName"/> has changed.
        // /// </summary>
        // public event EventHandler PropertyNamePropertyChanged;

        /// <summary>
        /// Defines the name for the <see cref="PropertyName"/> dependency property.
        /// </summary>
        public const string DependencyPropertyName_PropertyName = "PropertyName";

        /// <summary>
        /// Identifies the <see cref="PropertyName"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PropertyNameProperty = DependencyProperty.Register(DependencyPropertyName_PropertyName, typeof(string), typeof(DependencyPropertyVM),
                new PropertyMetadata("", (d, e) => (d as DependencyPropertyVM).PropertyName_PropertyChanged(e.OldValue as string, e.NewValue as string),
                    (d, b) => (d as DependencyPropertyVM).PropertyName_CoerceValue(b)));

        // TODO: Add summary documentation to the PropertyName Property.
        /// <summary>
        /// 
        /// </summary>
        public string PropertyName
        {
            get { return GetValue(PropertyNameProperty) as string; }
            set { SetValue(PropertyNameProperty, value); }
        }

        /// <summary>
        /// This gets called after the value associated with the <see cref="PropertyName"/> dependency property has changed.
        /// </summary>
        /// <param name="oldValue">The <seealso cref="string"/> value before the <see cref="PropertyName"/> property was changed.</param>
        /// <param name="newValue">The <seealso cref="string"/> value after the <see cref="PropertyName"/> property was changed.</param>
        protected virtual void PropertyName_PropertyChanged(string oldValue, string newValue)
        {
            // TODO: Implement DependencyPropertyVM.PropertyName_PropertyChanged(string, string)
            // PropertyNamePropertyChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// This gets called whenever <see cref="PropertyName"/> is being re-evaluated, or coercion is specifically requested.
        /// </summary>
        /// <param name="baseValue">The new value of the property, prior to any coercion attempt.</param>
        /// <returns>The coerced value.</returns>
        public virtual string PropertyName_CoerceValue(object baseValue)
        {
            // TODO: Implement DependencyPropertyVM.PropertyName_CoerceValue(DependencyObject, object)
            return (baseValue as string) ?? "";
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

        // /// <summary>
        // /// Occurs when the value of <see cref="PropertyTypeName"/> has changed.
        // /// </summary>
        // public event EventHandler PropertyTypeNamePropertyChanged;

        /// <summary>
        /// Defines the name for the <see cref="PropertyTypeName"/> dependency property.
        /// </summary>
        public const string DependencyPropertyName_PropertyTypeName = "PropertyTypeName";

        /// <summary>
        /// Identifies the <see cref="PropertyTypeName"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PropertyTypeNameProperty = DependencyProperty.Register(DependencyPropertyName_PropertyTypeName, typeof(string), typeof(DependencyPropertyVM),
                new PropertyMetadata("", (d, e) => (d as DependencyPropertyVM).PropertyTypeName_PropertyChanged(e.OldValue as string, e.NewValue as string),
                    (d, b) => (d as DependencyPropertyVM).PropertyTypeName_CoerceValue(b)));

        // TODO: Add summary documentation to the PropertyTypeName Property.
        /// <summary>
        /// 
        /// </summary>
        public string PropertyTypeName
        {
            get { return GetValue(PropertyTypeNameProperty) as string; }
            set { SetValue(PropertyTypeNameProperty, value); }
        }

        /// <summary>
        /// This gets called after the value associated with the <see cref="PropertyTypeName"/> dependency property has changed.
        /// </summary>
        /// <param name="oldValue">The <seealso cref="string"/> value before the <see cref="PropertyTypeName"/> property was changed.</param>
        /// <param name="newValue">The <seealso cref="string"/> value after the <see cref="PropertyTypeName"/> property was changed.</param>
        protected virtual void PropertyTypeName_PropertyChanged(string oldValue, string newValue)
        {
            // TODO: Implement DependencyPropertyVM.PropertyTypeName_PropertyChanged(string, string)
            // PropertyTypeNamePropertyChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// This gets called whenever <see cref="PropertyTypeName"/> is being re-evaluated, or coercion is specifically requested.
        /// </summary>
        /// <param name="baseValue">The new value of the property, prior to any coercion attempt.</param>
        /// <returns>The coerced value.</returns>
        public virtual string PropertyTypeName_CoerceValue(object baseValue)
        {
            // TODO: Implement DependencyPropertyVM.PropertyTypeName_CoerceValue(DependencyObject, object)
            return (baseValue as string) ?? "";
        }

        #endregion

        // Set to "null" if IsCollection
        #region DefaultValueLiteral Property Members

        // /// <summary>
        // /// Occurs when the value of <see cref="DefaultValueLiteral"/> has changed.
        // /// </summary>
        // public event EventHandler DefaultValueLiteralPropertyChanged;

        /// <summary>
        /// Defines the name for the <see cref="DefaultValueLiteral"/> dependency property.
        /// </summary>
        public const string DependencyPropertyName_DefaultValueLiteral = "DefaultValueLiteral";

        /// <summary>
        /// Identifies the <see cref="DefaultValueLiteral"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DefaultValueLiteralProperty = DependencyProperty.Register(DependencyPropertyName_DefaultValueLiteral, typeof(string), typeof(DependencyPropertyVM),
                new PropertyMetadata("", (d, e) => (d as DependencyPropertyVM).DefaultValueLiteral_PropertyChanged(e.OldValue as string, e.NewValue as string),
                    (d, b) => (d as DependencyPropertyVM).DefaultValueLiteral_CoerceValue(b)));

        // TODO: Add summary documentation to the DefaultValueLiteral Property.
        /// <summary>
        /// 
        /// </summary>
        public string DefaultValueLiteral
        {
            get { return GetValue(DefaultValueLiteralProperty) as string; }
            set { SetValue(DefaultValueLiteralProperty, value); }
        }

        /// <summary>
        /// This gets called after the value associated with the <see cref="DefaultValueLiteral"/> dependency property has changed.
        /// </summary>
        /// <param name="oldValue">The <seealso cref="string"/> value before the <see cref="DefaultValueLiteral"/> property was changed.</param>
        /// <param name="newValue">The <seealso cref="string"/> value after the <see cref="DefaultValueLiteral"/> property was changed.</param>
        protected virtual void DefaultValueLiteral_PropertyChanged(string oldValue, string newValue)
        {
            // TODO: Implement DependencyPropertyVM.DefaultValueLiteral_PropertyChanged(string, string)
            // DefaultValueLiteralPropertyChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// This gets called whenever <see cref="DefaultValueLiteral"/> is being re-evaluated, or coercion is specifically requested.
        /// </summary>
        /// <param name="baseValue">The new value of the property, prior to any coercion attempt.</param>
        /// <returns>The coerced value.</returns>
        public virtual string DefaultValueLiteral_CoerceValue(object baseValue)
        {
            // TODO: Implement DependencyPropertyVM.DefaultValueLiteral_CoerceValue(DependencyObject, object)
            return (baseValue as string) ?? "";
        }

        #endregion

        #region IsAttached Property Members

        // /// <summary>
        // /// Occurs when the value of <see cref="IsAttached"/> has changed.
        // /// </summary>
        // public event EventHandler IsAttachedPropertyChanged;

        /// <summary>
        /// Defines the name for the <see cref="IsAttached"/> dependency property.
        /// </summary>
        public const string DependencyPropertyName_IsAttached = "IsAttached";

        /// <summary>
        /// Identifies the <see cref="IsAttached"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsAttachedProperty = DependencyProperty.Register(DependencyPropertyName_IsAttached, typeof(bool), typeof(DependencyPropertyVM),
                new PropertyMetadata(false, (d, e) => (d as DependencyPropertyVM).IsAttached_PropertyChanged((bool)(e.OldValue), (bool)(e.NewValue))/*,
            (d, b) => (d as DependencyPropertyVM).IsAttached_CoerceValue(b)*/));

        // TODO: Add summary documentation to the IsAttached Property.
        /// <summary>
        /// 
        /// </summary>
        public bool IsAttached
        {
            get { return (bool)(GetValue(IsAttachedProperty)); }
            set { SetValue(IsAttachedProperty, value); }
        }

        /// <summary>
        /// This gets called after the value associated with the <see cref="IsAttached"/> dependency property has changed.
        /// </summary>
        /// <param name="oldValue">The <seealso cref="bool"/> value before the <seealso cref="IsAttached"/> property was changed.</param>
        /// <param name="newValue">The <seealso cref="bool"/> value after the <seealso cref="IsAttached"/> property was changed.</param>
        protected virtual void IsAttached_PropertyChanged(bool oldValue, bool newValue)
        {
            // TODO: Implement DependencyPropertyVM.IsAttached_PropertyChanged(bool, bool)
            // IsAttachedPropertyChanged?.Invoke(this, EventArgs.Empty);
        }

        // /// <summary>
        // /// This gets called whenever <see cref="IsAttached"/> is being re-evaluated, or coercion is specifically requested.
        // /// </summary>
        // /// <param name="baseValue">The new value of the property, prior to any coercion attempt.</param>
        // /// <returns>The coerced value.</returns>
        // public virtual bool IsAttached_CoerceValue(object baseValue)
        // {
        //     throw new NotImplementedException();
        // }

        #endregion
        
        #region IsReadOnly Property Members

        // /// <summary>
        // /// Occurs when the value of <see cref="IsReadOnly"/> has changed.
        // /// </summary>
        // public event EventHandler IsReadOnlyPropertyChanged;

        /// <summary>
        /// Defines the name for the <see cref="IsReadOnly"/> dependency property.
        /// </summary>
        public const string DependencyPropertyName_IsReadOnly = "IsReadOnly";

        /// <summary>
        /// Identifies the <see cref="IsReadOnly"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register(DependencyPropertyName_IsReadOnly, typeof(bool), typeof(DependencyPropertyVM),
                new PropertyMetadata(false, (d, e) => (d as DependencyPropertyVM).IsReadOnly_PropertyChanged((bool)(e.OldValue), (bool)(e.NewValue))/*,
            (d, b) => (d as DependencyPropertyVM).IsReadOnly_CoerceValue(b)*/));

        // TODO: Add summary documentation to the IsReadOnly Property.
        /// <summary>
        /// 
        /// </summary>
        public bool IsReadOnly
        {
            get { return (bool)(GetValue(IsReadOnlyProperty)); }
            set { SetValue(IsReadOnlyProperty, value); }
        }

        /// <summary>
        /// This gets called after the value associated with the <see cref="IsReadOnly"/> dependency property has changed.
        /// </summary>
        /// <param name="oldValue">The <seealso cref="bool"/> value before the <seealso cref="IsReadOnly"/> property was changed.</param>
        /// <param name="newValue">The <seealso cref="bool"/> value after the <seealso cref="IsReadOnly"/> property was changed.</param>
        protected virtual void IsReadOnly_PropertyChanged(bool oldValue, bool newValue)
        {
            // TODO: Implement DependencyPropertyVM.IsReadOnly_PropertyChanged(bool, bool)
            // IsReadOnlyPropertyChanged?.Invoke(this, EventArgs.Empty);
        }

        // /// <summary>
        // /// This gets called whenever <see cref="IsReadOnly"/> is being re-evaluated, or coercion is specifically requested.
        // /// </summary>
        // /// <param name="baseValue">The new value of the property, prior to any coercion attempt.</param>
        // /// <returns>The coerced value.</returns>
        // public virtual bool IsReadOnly_CoerceValue(object baseValue)
        // {
        //     throw new NotImplementedException();
        // }

        #endregion

        #region AllowNull Property Members

        // /// <summary>
        // /// Occurs when the value of <see cref="AllowNull"/> has changed.
        // /// </summary>
        // public event EventHandler AllowNullPropertyChanged;

        /// <summary>
        /// Defines the name for the <see cref="AllowNull"/> dependency property.
        /// </summary>
        public const string DependencyPropertyName_AllowNull = "AllowNull";

        /// <summary>
        /// Identifies the <see cref="AllowNull"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AllowNullProperty = DependencyProperty.Register(DependencyPropertyName_AllowNull, typeof(bool), typeof(DependencyPropertyVM),
                new PropertyMetadata(true, (d, e) => (d as DependencyPropertyVM).AllowNull_PropertyChanged((bool)(e.OldValue), (bool)(e.NewValue))/*,
            (d, b) => (d as DependencyPropertyVM).AllowNull_CoerceValue(b)*/));

        // TODO: Add summary documentation to the AllowNull Property.
        /// <summary>
        /// 
        /// </summary>
        public bool AllowNull
        {
            get { return (bool)(GetValue(AllowNullProperty)); }
            set { SetValue(AllowNullProperty, value); }
        }

        /// <summary>
        /// This gets called after the value associated with the <see cref="AllowNull"/> dependency property has changed.
        /// </summary>
        /// <param name="oldValue">The <seealso cref="bool"/> value before the <seealso cref="AllowNull"/> property was changed.</param>
        /// <param name="newValue">The <seealso cref="bool"/> value after the <seealso cref="AllowNull"/> property was changed.</param>
        protected virtual void AllowNull_PropertyChanged(bool oldValue, bool newValue)
        {
            // TODO: Implement DependencyPropertyVM.AllowNull_PropertyChanged(bool, bool)
            // AllowNullPropertyChanged?.Invoke(this, EventArgs.Empty);
        }

        // /// <summary>
        // /// This gets called whenever <see cref="AllowNull"/> is being re-evaluated, or coercion is specifically requested.
        // /// </summary>
        // /// <param name="baseValue">The new value of the property, prior to any coercion attempt.</param>
        // /// <returns>The coerced value.</returns>
        // public virtual bool AllowNull_CoerceValue(object baseValue)
        // {
        //     throw new NotImplementedException();
        // }

        #endregion
        
        // Set to true if IsReadOnlyCollection
        // SetDefaultValueLiteral to "null" if true
        #region IsCollection Property Members
            
        /// <summary>
        /// Defines the name for the <see cref="IsCollection"/> dependency property.
        /// </summary>
        public const string DependencyPropertyName_IsCollection = "IsCollection";

        /// <summary>
        /// Identifies the <see cref="IsCollection"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsCollectionProperty = DependencyProperty.Register(DependencyPropertyName_IsCollection, typeof(bool), typeof(DependencyPropertyVM),
                new PropertyMetadata(false, (d, e) => (d as DependencyPropertyVM).IsCollection_PropertyChanged((bool)(e.OldValue), (bool)(e.NewValue))/*,
            (d, b) => (d as DependencyPropertyVM).IsCollection_CoerceValue(b)*/));

        /// <summary>
        /// Indicates whether property is a collection
        /// </summary>
        public bool IsCollection
        {
            get { return (bool)(GetValue(IsCollectionProperty)); }
            set { SetValue(IsCollectionProperty, value); }
        }

        /// <summary>
        /// This gets called after the value associated with the <see cref="IsCollection"/> dependency property has changed.
        /// </summary>
        /// <param name="oldValue">The <seealso cref="bool"/> value before the <seealso cref="IsCollection"/> property was changed.</param>
        /// <param name="newValue">The <seealso cref="bool"/> value after the <seealso cref="IsCollection"/> property was changed.</param>
        protected virtual void IsCollection_PropertyChanged(bool oldValue, bool newValue)
        {
            PropertyTypeLabel = (newValue) ? DefaulValue_PropertyTypeLabel_Element : DefaulValue_PropertyTypeLabel_Property;
        }

        // /// <summary>
        // /// This gets called whenever <see cref="IsCollection"/> is being re-evaluated, or coercion is specifically requested.
        // /// </summary>
        // /// <param name="baseValue">The new value of the property, prior to any coercion attempt.</param>
        // /// <returns>The coerced value.</returns>
        // public virtual bool IsCollection_CoerceValue(object baseValue)
        // {
        //     throw new NotImplementedException();
        // }

        #endregion
        
        #region IsReadOnlyCollection Property Members

        // /// <summary>
        // /// Occurs when the value of <see cref="IsReadOnlyCollection"/> has changed.
        // /// </summary>
        // public event EventHandler IsReadOnlyCollectionPropertyChanged;

        /// <summary>
        /// Defines the name for the <see cref="IsReadOnlyCollection"/> dependency property.
        /// </summary>
        public const string DependencyPropertyName_IsReadOnlyCollection = "IsReadOnlyCollection";

        /// <summary>
        /// Identifies the <see cref="IsReadOnlyCollection"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsReadOnlyCollectionProperty = DependencyProperty.Register(DependencyPropertyName_IsReadOnlyCollection, typeof(bool), typeof(DependencyPropertyVM),
                new PropertyMetadata(false, (d, e) => (d as DependencyPropertyVM).IsReadOnlyCollection_PropertyChanged((bool)(e.OldValue), (bool)(e.NewValue))/*,
            (d, b) => (d as DependencyPropertyVM).IsReadOnlyCollection_CoerceValue(b)*/));

        // TODO: Add summary documentation to the IsReadOnlyCollection Property.
        /// <summary>
        /// 
        /// </summary>
        public bool IsReadOnlyCollection
        {
            get { return (bool)(GetValue(IsReadOnlyCollectionProperty)); }
            set { SetValue(IsReadOnlyCollectionProperty, value); }
        }

        /// <summary>
        /// This gets called after the value associated with the <see cref="IsReadOnlyCollection"/> dependency property has changed.
        /// </summary>
        /// <param name="oldValue">The <seealso cref="bool"/> value before the <seealso cref="IsReadOnlyCollection"/> property was changed.</param>
        /// <param name="newValue">The <seealso cref="bool"/> value after the <seealso cref="IsReadOnlyCollection"/> property was changed.</param>
        protected virtual void IsReadOnlyCollection_PropertyChanged(bool oldValue, bool newValue)
        {
            // TODO: Implement DependencyPropertyVM.IsReadOnlyCollection_PropertyChanged(bool, bool)
            // IsReadOnlyCollectionPropertyChanged?.Invoke(this, EventArgs.Empty);
        }

        // /// <summary>
        // /// This gets called whenever <see cref="IsReadOnlyCollection"/> is being re-evaluated, or coercion is specifically requested.
        // /// </summary>
        // /// <param name="baseValue">The new value of the property, prior to any coercion attempt.</param>
        // /// <returns>The coerced value.</returns>
        // public virtual bool IsReadOnlyCollection_CoerceValue(object baseValue)
        // {
        //     throw new NotImplementedException();
        // }

        #endregion

        #region HandlePropertyChanged Property Members

        // /// <summary>
        // /// Occurs when the value of <see cref="HandlePropertyChanged"/> has changed.
        // /// </summary>
        // public event EventHandler HandlePropertyChangedPropertyChanged;

        /// <summary>
        /// Defines the name for the <see cref="HandlePropertyChanged"/> dependency property.
        /// </summary>
        public const string DependencyPropertyName_HandlePropertyChanged = "HandlePropertyChanged";

        /// <summary>
        /// Identifies the <see cref="HandlePropertyChanged"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HandlePropertyChangedProperty = DependencyProperty.Register(DependencyPropertyName_HandlePropertyChanged, typeof(bool), typeof(DependencyPropertyVM),
                new PropertyMetadata(false, (d, e) => (d as DependencyPropertyVM).HandlePropertyChanged_PropertyChanged((bool)(e.OldValue), (bool)(e.NewValue))/*,
            (d, b) => (d as DependencyPropertyVM).HandlePropertyChanged_CoerceValue(b)*/));

        // TODO: Add summary documentation to the HandlePropertyChanged Property.
        /// <summary>
        /// 
        /// </summary>
        public bool HandlePropertyChanged
        {
            get { return (bool)(GetValue(HandlePropertyChangedProperty)); }
            set { SetValue(HandlePropertyChangedProperty, value); }
        }

        /// <summary>
        /// This gets called after the value associated with the <see cref="HandlePropertyChanged"/> dependency property has changed.
        /// </summary>
        /// <param name="oldValue">The <seealso cref="bool"/> value before the <seealso cref="HandlePropertyChanged"/> property was changed.</param>
        /// <param name="newValue">The <seealso cref="bool"/> value after the <seealso cref="HandlePropertyChanged"/> property was changed.</param>
        protected virtual void HandlePropertyChanged_PropertyChanged(bool oldValue, bool newValue)
        {
            // TODO: Implement DependencyPropertyVM.HandlePropertyChanged_PropertyChanged(bool, bool)
            // HandlePropertyChangedPropertyChanged?.Invoke(this, EventArgs.Empty);
        }

        // /// <summary>
        // /// This gets called whenever <see cref="HandlePropertyChanged"/> is being re-evaluated, or coercion is specifically requested.
        // /// </summary>
        // /// <param name="baseValue">The new value of the property, prior to any coercion attempt.</param>
        // /// <returns>The coerced value.</returns>
        // public virtual bool HandlePropertyChanged_CoerceValue(object baseValue)
        // {
        //     throw new NotImplementedException();
        // }

        #endregion

        #region HandleCoerceValue Property Members

        // /// <summary>
        // /// Occurs when the value of <see cref="HandleCoerceValue"/> has changed.
        // /// </summary>
        // public event EventHandler HandleCoerceValuePropertyChanged;

        /// <summary>
        /// Defines the name for the <see cref="HandleCoerceValue"/> dependency property.
        /// </summary>
        public const string DependencyPropertyName_HandleCoerceValue = "HandleCoerceValue";

        /// <summary>
        /// Identifies the <see cref="HandleCoerceValue"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HandleCoerceValueProperty = DependencyProperty.Register(DependencyPropertyName_HandleCoerceValue, typeof(bool), typeof(DependencyPropertyVM),
                new PropertyMetadata(false, (d, e) => (d as DependencyPropertyVM).HandleCoerceValue_PropertyChanged((bool)(e.OldValue), (bool)(e.NewValue))/*,
            (d, b) => (d as DependencyPropertyVM).HandleCoerceValue_CoerceValue(b)*/));

        // TODO: Add summary documentation to the HandleCoerceValue Property.
        /// <summary>
        /// 
        /// </summary>
        public bool HandleCoerceValue
        {
            get { return (bool)(GetValue(HandleCoerceValueProperty)); }
            set { SetValue(HandleCoerceValueProperty, value); }
        }

        /// <summary>
        /// This gets called after the value associated with the <see cref="HandleCoerceValue"/> dependency property has changed.
        /// </summary>
        /// <param name="oldValue">The <seealso cref="bool"/> value before the <seealso cref="HandleCoerceValue"/> property was changed.</param>
        /// <param name="newValue">The <seealso cref="bool"/> value after the <seealso cref="HandleCoerceValue"/> property was changed.</param>
        protected virtual void HandleCoerceValue_PropertyChanged(bool oldValue, bool newValue)
        {
            // TODO: Implement DependencyPropertyVM.HandleCoerceValue_PropertyChanged(bool, bool)
            // HandleCoerceValuePropertyChanged?.Invoke(this, EventArgs.Empty);
        }

        // /// <summary>
        // /// This gets called whenever <see cref="HandleCoerceValue"/> is being re-evaluated, or coercion is specifically requested.
        // /// </summary>
        // /// <param name="baseValue">The new value of the property, prior to any coercion attempt.</param>
        // /// <returns>The coerced value.</returns>
        // public virtual bool HandleCoerceValue_CoerceValue(object baseValue)
        // {
        //     throw new NotImplementedException();
        // }

        #endregion

        #region HandleCollectionChanged Property Members

        // /// <summary>
        // /// Occurs when the value of <see cref="HandleCollectionChanged"/> has changed.
        // /// </summary>
        // public event EventHandler HandleCollectionChangedPropertyChanged;

        /// <summary>
        /// Defines the name for the <see cref="HandleCollectionChanged"/> dependency property.
        /// </summary>
        public const string DependencyPropertyName_HandleCollectionChanged = "HandleCollectionChanged";

        /// <summary>
        /// Identifies the <see cref="HandleCollectionChanged"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HandleCollectionChangedProperty = DependencyProperty.Register(DependencyPropertyName_HandleCollectionChanged, typeof(bool), typeof(DependencyPropertyVM),
                new PropertyMetadata(false, (d, e) => (d as DependencyPropertyVM).HandleCollectionChanged_PropertyChanged((bool)(e.OldValue), (bool)(e.NewValue))/*,
            (d, b) => (d as DependencyPropertyVM).HandleCollectionChanged_CoerceValue(b)*/));

        // TODO: Add summary documentation to the HandleCollectionChanged Property.
        /// <summary>
        /// 
        /// </summary>
        public bool HandleCollectionChanged
        {
            get { return (bool)(GetValue(HandleCollectionChangedProperty)); }
            set { SetValue(HandleCollectionChangedProperty, value); }
        }

        /// <summary>
        /// This gets called after the value associated with the <see cref="HandleCollectionChanged"/> dependency property has changed.
        /// </summary>
        /// <param name="oldValue">The <seealso cref="bool"/> value before the <seealso cref="HandleCollectionChanged"/> property was changed.</param>
        /// <param name="newValue">The <seealso cref="bool"/> value after the <seealso cref="HandleCollectionChanged"/> property was changed.</param>
        protected virtual void HandleCollectionChanged_PropertyChanged(bool oldValue, bool newValue)
        {
            // TODO: Implement DependencyPropertyVM.HandleCollectionChanged_PropertyChanged(bool, bool)
            // HandleCollectionChangedPropertyChanged?.Invoke(this, EventArgs.Empty);
        }

        // /// <summary>
        // /// This gets called whenever <see cref="HandleCollectionChanged"/> is being re-evaluated, or coercion is specifically requested.
        // /// </summary>
        // /// <param name="baseValue">The new value of the property, prior to any coercion attempt.</param>
        // /// <returns>The coerced value.</returns>
        // public virtual bool HandleCollectionChanged_CoerceValue(object baseValue)
        // {
        //     throw new NotImplementedException();
        // }

        #endregion

        #region SetterIsProtected Property Members

        // /// <summary>
        // /// Occurs when the value of <see cref="SetterIsProtected"/> has changed.
        // /// </summary>
        // public event EventHandler SetterIsProtectedPropertyChanged;

        /// <summary>
        /// Defines the name for the <see cref="SetterIsProtected"/> dependency property.
        /// </summary>
        public const string DependencyPropertyName_SetterIsProtected = "SetterIsProtected";

        /// <summary>
        /// Identifies the <see cref="SetterIsProtected"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SetterIsProtectedProperty = DependencyProperty.Register(DependencyPropertyName_SetterIsProtected, typeof(bool), typeof(DependencyPropertyVM),
                new PropertyMetadata(false, (d, e) => (d as DependencyPropertyVM).SetterIsProtected_PropertyChanged((bool)(e.OldValue), (bool)(e.NewValue))/*,
            (d, b) => (d as DependencyPropertyVM).SetterIsProtected_CoerceValue(b)*/));

        // TODO: Add summary documentation to the SetterIsProtected Property.
        /// <summary>
        /// 
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
            // TODO: Implement DependencyPropertyVM.SetterIsProtected_PropertyChanged(bool, bool)
            // SetterIsProtectedPropertyChanged?.Invoke(this, EventArgs.Empty);
        }

        // /// <summary>
        // /// This gets called whenever <see cref="SetterIsProtected"/> is being re-evaluated, or coercion is specifically requested.
        // /// </summary>
        // /// <param name="baseValue">The new value of the property, prior to any coercion attempt.</param>
        // /// <returns>The coerced value.</returns>
        // public virtual bool SetterIsProtected_CoerceValue(object baseValue)
        // {
        //     throw new NotImplementedException();
        // }

        #endregion

        #region SetterIsPrivate Property Members

        // /// <summary>
        // /// Occurs when the value of <see cref="SetterIsPrivate"/> has changed.
        // /// </summary>
        // public event EventHandler SetterIsPrivatePropertyChanged;

        /// <summary>
        /// Defines the name for the <see cref="SetterIsPrivate"/> dependency property.
        /// </summary>
        public const string DependencyPropertyName_SetterIsPrivate = "SetterIsPrivate";

        /// <summary>
        /// Identifies the <see cref="SetterIsPrivate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SetterIsPrivateProperty = DependencyProperty.Register(DependencyPropertyName_SetterIsPrivate, typeof(bool), typeof(DependencyPropertyVM),
                new PropertyMetadata(false, (d, e) => (d as DependencyPropertyVM).SetterIsPrivate_PropertyChanged((bool)(e.OldValue), (bool)(e.NewValue))/*,
            (d, b) => (d as DependencyPropertyVM).SetterIsPrivate_CoerceValue(b)*/));

        // TODO: Add summary documentation to the SetterIsPrivate Property.
        /// <summary>
        /// 
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
            // TODO: Implement DependencyPropertyVM.SetterIsPrivate_PropertyChanged(bool, bool)
            // SetterIsPrivatePropertyChanged?.Invoke(this, EventArgs.Empty);
        }

        // /// <summary>
        // /// This gets called whenever <see cref="SetterIsPrivate"/> is being re-evaluated, or coercion is specifically requested.
        // /// </summary>
        // /// <param name="baseValue">The new value of the property, prior to any coercion attempt.</param>
        // /// <returns>The coerced value.</returns>
        // public virtual bool SetterIsPrivate_CoerceValue(object baseValue)
        // {
        //     throw new NotImplementedException();
        // }

        #endregion

        #region SetterIsInternal Property Members

        // /// <summary>
        // /// Occurs when the value of <see cref="SetterIsInternal"/> has changed.
        // /// </summary>
        // public event EventHandler SetterIsInternalPropertyChanged;

        /// <summary>
        /// Defines the name for the <see cref="SetterIsInternal"/> dependency property.
        /// </summary>
        public const string DependencyPropertyName_SetterIsInternal = "SetterIsInternal";

        /// <summary>
        /// Identifies the <see cref="SetterIsInternal"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SetterIsInternalProperty = DependencyProperty.Register(DependencyPropertyName_SetterIsInternal, typeof(bool), typeof(DependencyPropertyVM),
                new PropertyMetadata(false, (d, e) => (d as DependencyPropertyVM).SetterIsInternal_PropertyChanged((bool)(e.OldValue), (bool)(e.NewValue))/*,
            (d, b) => (d as DependencyPropertyVM).SetterIsInternal_CoerceValue(b)*/));

        // TODO: Add summary documentation to the SetterIsInternal Property.
        /// <summary>
        /// 
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
            // TODO: Implement DependencyPropertyVM.SetterIsInternal_PropertyChanged(bool, bool)
            // SetterIsInternalPropertyChanged?.Invoke(this, EventArgs.Empty);
        }

        // /// <summary>
        // /// This gets called whenever <see cref="SetterIsInternal"/> is being re-evaluated, or coercion is specifically requested.
        // /// </summary>
        // /// <param name="baseValue">The new value of the property, prior to any coercion attempt.</param>
        // /// <returns>The coerced value.</returns>
        // public virtual bool SetterIsInternal_CoerceValue(object baseValue)
        // {
        //     throw new NotImplementedException();
        // }

        #endregion

        #region SetterIsProtectedInternal Property Members

        // /// <summary>
        // /// Occurs when the value of <see cref="SetterIsProtectedInternal"/> has changed.
        // /// </summary>
        // public event EventHandler SetterIsProtectedInternalPropertyChanged;

        /// <summary>
        /// Defines the name for the <see cref="SetterIsProtectedInternal"/> dependency property.
        /// </summary>
        public const string DependencyPropertyName_SetterIsProtectedInternal = "SetterIsProtectedInternal";

        /// <summary>
        /// Identifies the <see cref="SetterIsProtectedInternal"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SetterIsProtectedInternalProperty = DependencyProperty.Register(DependencyPropertyName_SetterIsProtectedInternal, typeof(bool), typeof(DependencyPropertyVM),
                new PropertyMetadata(false, (d, e) => (d as DependencyPropertyVM).SetterIsProtectedInternal_PropertyChanged((bool)(e.OldValue), (bool)(e.NewValue))/*,
            (d, b) => (d as DependencyPropertyVM).SetterIsProtectedInternal_CoerceValue(b)*/));

        // TODO: Add summary documentation to the SetterIsProtectedInternal Property.
        /// <summary>
        /// 
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
            // TODO: Implement DependencyPropertyVM.SetterIsProtectedInternal_PropertyChanged(bool, bool)
            // SetterIsProtectedInternalPropertyChanged?.Invoke(this, EventArgs.Empty);
        }

        // /// <summary>
        // /// This gets called whenever <see cref="SetterIsProtectedInternal"/> is being re-evaluated, or coercion is specifically requested.
        // /// </summary>
        // /// <param name="baseValue">The new value of the property, prior to any coercion attempt.</param>
        // /// <returns>The coerced value.</returns>
        // public virtual bool SetterIsProtectedInternal_CoerceValue(object baseValue)
        // {
        //     throw new NotImplementedException();
        // }

        #endregion
    }
}
