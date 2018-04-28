using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;

namespace LteDev.TypeBrowser
{
    /// <summary>
    /// Represents an <seealso cref="Attribute" />.
    /// </summary>
    public class MemberValueVM : MemberListItemVM
    {
        #region Value Property Members

        /// <summary>
        /// Defines the name for the <see cref="Value"/> dependency property.
        /// </summary>
        public const string PropertyName_Value = "Value";

        private static readonly DependencyPropertyKey ValuePropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_Value,
            typeof(string), typeof(MemberValueVM), new PropertyMetadata(""));

        /// <summary>
        /// Identifies the <see cref="Value"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ValueProperty = ValuePropertyKey.DependencyProperty;

        /// <summary>
        /// Value of the model property <seealso cref="Type.Value"/>.
        /// </summary>
        public string Value
        {
            get { return (string)(GetValue(ValueProperty)); }
            private set { SetValue(ValuePropertyKey, value); }
        }

        #endregion
        
        #region ActualType Property Members

        /// <summary>
        /// Defines the name for the <see cref="ActualType"/> dependency property.
        /// </summary>
        public const string PropertyName_ActualType = "ActualType";

        private static readonly DependencyPropertyKey ActualTypePropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_ActualType,
            typeof(string), typeof(MemberValueVM), new PropertyMetadata(""));

        /// <summary>
        /// Identifies the <see cref="ActualType"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ActualTypeProperty = ActualTypePropertyKey.DependencyProperty;

        /// <summary>
        /// Value of the model property <seealso cref="Type.ActualType"/>.
        /// </summary>
        public string ActualType
        {
            get { return (string)(GetValue(ActualTypeProperty)); }
            private set { SetValue(ActualTypePropertyKey, value); }
        }

        #endregion
        
        public MemberValueVM() { }

        public MemberValueVM(PropertyInfo property, object obj)
            : base(property)
        {
            if (property == null)
                return;
            object value = property.GetValue(obj);
            if (value == null)
                return;
            Value = (obj is string) ? (string)obj : obj.ToString();
            ActualType = TypeListItemVM.ToCSharpTypeName(obj.GetType().FullName);
        }

        public MemberValueVM(FieldInfo field, object obj)
            : base(field)
        {
            if (field == null)
                return;
            object value = field.GetValue(obj);
            if (value == null)
                return;
            Value = (obj is string) ? (string)obj : obj.ToString();
            ActualType = TypeListItemVM.ToCSharpTypeName(obj.GetType().FullName);
        }
    }
}