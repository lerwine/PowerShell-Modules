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
    /// Represents a <seealso cref="PropertyInfo" /> or <seealso cref="FieldInfo" />.
    /// </summary>
    public class MemberListItemVM : DependencyObject
    {
        #region Name Property Members

        /// <summary>
        /// Defines the name for the <see cref="Name"/> dependency property.
        /// </summary>
        public const string PropertyName_Name = "Name";

        private static readonly DependencyPropertyKey NamePropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_Name,
            typeof(string), typeof(MemberListItemVM), new PropertyMetadata(""));

        /// <summary>
        /// Identifies the <see cref="Name"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty NameProperty = NamePropertyKey.DependencyProperty;

        /// <summary>
        /// Value of the model property <seealso cref="Type.Name"/>.
        /// </summary>
        public string Name
        {
            get { return (string)(GetValue(NameProperty)); }
            private set { SetValue(NamePropertyKey, value); }
        }

        #endregion
        
        #region MemberType Property Members

        /// <summary>
        /// Defines the name for the <see cref="MemberType"/> dependency property.
        /// </summary>
        public const string PropertyName_MemberType = "MemberType";

        private static readonly DependencyPropertyKey MemberTypePropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_MemberType,
            typeof(string), typeof(MemberListItemVM), new PropertyMetadata(""));

        /// <summary>
        /// Identifies the <see cref="MemberType"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MemberTypeProperty = MemberTypePropertyKey.DependencyProperty;

        /// <summary>
        /// Value of the model property <seealso cref="Type.Type"/>.
        /// </summary>
        public string MemberType
        {
            get { return (string)(GetValue(MemberTypeProperty)); }
            private set { SetValue(MemberTypePropertyKey, value); }
        }

        #endregion
        
        #region IsPublic Property Members

        /// <summary>
        /// Defines the name for the <see cref="IsPublic"/> dependency property.
        /// </summary>
        public const string PropertyName_IsPublic = "IsPublic";

        private static readonly DependencyPropertyKey IsPublicPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_IsPublic,
            typeof(bool), typeof(MemberListItemVM), new PropertyMetadata(false));

        /// <summary>
        /// Identifies the <see cref="IsPublic"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsPublicProperty = IsPublicPropertyKey.DependencyProperty;

        /// <summary>
        /// Value of the model property <seealso cref="Type.IsPublic"/>.
        /// </summary>
        public bool IsPublic
        {
            get { return (bool)(GetValue(IsPublicProperty)); }
            private set { SetValue(IsPublicPropertyKey, value); }
        }

        #endregion
       
        #region IsReadOnly Property Members

        /// <summary>
        /// Defines the name for the <see cref="IsReadOnly"/> dependency property.
        /// </summary>
        public const string PropertyName_IsReadOnly = "IsReadOnly";

        private static readonly DependencyPropertyKey IsReadOnlyPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_IsReadOnly,
            typeof(bool), typeof(MemberListItemVM), new PropertyMetadata(false));

        /// <summary>
        /// Identifies the <see cref="IsReadOnly"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsReadOnlyProperty = IsReadOnlyPropertyKey.DependencyProperty;

        /// <summary>
        /// Value of the model property <seealso cref="Type.IsReadOnly"/>.
        /// </summary>
        public bool IsReadOnly
        {
            get { return (bool)(GetValue(IsReadOnlyProperty)); }
            private set { SetValue(IsReadOnlyPropertyKey, value); }
        }

        #endregion
       
        #region IsStatic Property Members

        /// <summary>
        /// Defines the name for the <see cref="IsStatic"/> dependency property.
        /// </summary>
        public const string PropertyName_IsStatic = "IsStatic";

        private static readonly DependencyPropertyKey IsStaticPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_IsStatic,
            typeof(bool), typeof(MemberListItemVM), new PropertyMetadata(false));

        /// <summary>
        /// Identifies the <see cref="IsStatic"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsStaticProperty = IsStaticPropertyKey.DependencyProperty;

        /// <summary>
        /// Value of the model property <seealso cref="Type.IsStatic"/>.
        /// </summary>
        public bool IsStatic
        {
            get { return (bool)(GetValue(IsStaticProperty)); }
            private set { SetValue(IsStaticPropertyKey, value); }
        }

        #endregion
       
        private MemberInfo _memberInfo = null;

        public MemberListItemVM() { }

        public MemberListItemVM(PropertyInfo property)
        {
            if (property == null)
                return;
            
            _memberInfo = property;
            Name = property.Name;
            MemberType = TypeListItemVM.ToCSharpTypeName(property.PropertyType.FullName);
            MethodInfo method = property.GetSetMethod();
            
            if (method == null)
            {
                IsReadOnly = true;
                method = property.GetGetMethod();
            }
            else
            {
                IsPublic = method.IsPublic;
                IsReadOnly = !IsPublic;
            }
            IsStatic = method != null && method.IsStatic;
            if (method != null && method.IsPublic)
                IsPublic = true;
        }

        public MemberListItemVM(FieldInfo field)
        {
            if (field == null)
                return;
            
            _memberInfo = field;
            Name = field.Name;
            MemberType = TypeListItemVM.ToCSharpTypeName(field.FieldType.FullName);
            IsReadOnly = field.IsLiteral || field.IsInitOnly;
            IsStatic = field.IsStatic;
            IsPublic = field.IsPublic;
        }

        public MemberListItemVM(MemberListItemVM vm)
        {
            if (vm == null)
                return;
            
            _memberInfo = vm._memberInfo;
            Name = vm.Name;
            MemberType = vm.MemberType;
            IsReadOnly = vm.IsReadOnly;
            IsStatic = vm.IsStatic;
            IsPublic = vm.IsPublic;
        }

        public MemberInfo GetMemberInfo() { return _memberInfo; }
    }
}