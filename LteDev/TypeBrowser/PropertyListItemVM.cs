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
    /// Represents a <seealso cref="PropertyInfo" />.
    /// </summary>
    public class PropertyListItemVM : MemberListItemVM
    {
        private PropertyInfo _model = null;
        
        #region GetAccess Property Members

        /// <summary>
        /// Defines the name for the <see cref="GetAccess"/> dependency property.
        /// </summary>
        public const string PropertyName_GetAccess = "GetAccess";

        private static readonly DependencyPropertyKey GetAccessPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_GetAccess,
            typeof(string), typeof(PropertyListItemVM), new PropertyMetadata(""));

        /// <summary>
        /// Identifies the <see cref="GetAccess"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty GetAccessProperty = GetAccessPropertyKey.DependencyProperty;

        /// <summary>
        /// Value of the model property <seealso cref="Type.GetAccess"/>.
        /// </summary>
        public string GetAccess
        {
            get { return (string)(GetValue(GetAccessProperty)); }
            private set { SetValue(GetAccessPropertyKey, value); }
        }

        #endregion
        
        #region SetAccess Property Members

        /// <summary>
        /// Defines the name for the <see cref="SetAccess"/> dependency property.
        /// </summary>
        public const string PropertyName_SetAccess = "SetAccess";

        private static readonly DependencyPropertyKey SetAccessPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_SetAccess,
            typeof(string), typeof(PropertyListItemVM), new PropertyMetadata(""));

        /// <summary>
        /// Identifies the <see cref="SetAccess"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SetAccessProperty = SetAccessPropertyKey.DependencyProperty;

        /// <summary>
        /// Value of the model property <seealso cref="Type.SetAccess"/>.
        /// </summary>
        public string SetAccess
        {
            get { return (string)(GetValue(SetAccessProperty)); }
            private set { SetValue(SetAccessPropertyKey, value); }
        }

        #endregion
        
        #region Access Property Members

        /// <summary>
        /// Defines the name for the <see cref="Access"/> dependency property.
        /// </summary>
        public const string PropertyName_Access = "Access";

        private static readonly DependencyPropertyKey AccessPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_Access,
            typeof(string), typeof(PropertyListItemVM), new PropertyMetadata(""));

        /// <summary>
        /// Identifies the <see cref="Access"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AccessProperty = AccessPropertyKey.DependencyProperty;

        /// <summary>
        /// Value of the model property <seealso cref="Type.Access"/>.
        /// </summary>
        public string Access
        {
            get { return (string)(GetValue(AccessProperty)); }
            private set { SetValue(AccessPropertyKey, value); }
        }

        #endregion
        
        #region CanRead Property Members

        /// <summary>
        /// Defines the name for the <see cref="CanRead"/> dependency property.
        /// </summary>
        public const string PropertyName_CanRead = "CanRead";

        private static readonly DependencyPropertyKey CanReadPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_CanRead,
            typeof(bool), typeof(PropertyListItemVM), new PropertyMetadata(false));

        /// <summary>
        /// Identifies the <see cref="CanRead"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CanReadProperty = CanReadPropertyKey.DependencyProperty;

        /// <summary>
        /// Value of the model property <seealso cref="Type.CanRead"/>.
        /// </summary>
        public bool CanRead
        {
            get { return (bool)(GetValue(CanReadProperty)); }
            private set { SetValue(CanReadPropertyKey, value); }
        }

        #endregion
        
        #region CanWrite Property Members

        /// <summary>
        /// Defines the name for the <see cref="CanWrite"/> dependency property.
        /// </summary>
        public const string PropertyName_CanWrite = "CanWrite";

        private static readonly DependencyPropertyKey CanWritePropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_CanWrite,
            typeof(bool), typeof(PropertyListItemVM), new PropertyMetadata(false));

        /// <summary>
        /// Identifies the <see cref="CanWrite"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CanWriteProperty = CanWritePropertyKey.DependencyProperty;

        /// <summary>
        /// Value of the model property <seealso cref="Type.CanWrite"/>.
        /// </summary>
        public bool CanWrite
        {
            get { return (bool)(GetValue(CanWriteProperty)); }
            private set { SetValue(CanWritePropertyKey, value); }
        }

        #endregion
        
        #region Modifier Property Members

        /// <summary>
        /// Defines the name for the <see cref="Modifier"/> dependency property.
        /// </summary>
        public const string PropertyName_Modifier = "Modifier";

        private static readonly DependencyPropertyKey ModifierPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_Modifier,
            typeof(string), typeof(PropertyListItemVM), new PropertyMetadata(""));

        /// <summary>
        /// Identifies the <see cref="Modifier"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ModifierProperty = ModifierPropertyKey.DependencyProperty;

        /// <summary>
        /// Value of the model properties <seealso cref="Type.IsSealed"/> or <seealso cref="Type.IsAbstract"/>.
        /// </summary>
        public string Modifier
        {
            get { return (string)(GetValue(ModifierProperty)); }
            private set { SetValue(ModifierPropertyKey, value); }
        }

        #endregion
        
        public PropertyListItemVM() : base() { }

        public PropertyListItemVM(PropertyListItemVM vm) : base(vm)
        {
            if (vm == null)
                return;
            _model = vm._model;
            CanRead = vm.CanRead;
            CanWrite = vm.CanWrite;
            GetAccess = vm.GetAccess;
            SetAccess = vm.SetAccess;
            Access = vm.Access;
            Modifier = vm.Modifier;
        }

        public PropertyListItemVM(PropertyInfo model)
            : base(model)
        {
            _model = model;
            if (model == null)
                return;
            MethodInfo getMethod = model.GetGetMethod();
            MethodInfo setMethod = model.GetSetMethod();
            CanRead = model.CanRead && getMethod != null && getMethod.IsPublic;
            CanWrite = model.CanWrite && setMethod != null && setMethod.IsPublic;
            if (getMethod == null || getMethod.IsPrivate)
                GetAccess = "private";
            else if (getMethod.IsAssembly)
                GetAccess = "internal";
            else if (getMethod.IsFamily)
                GetAccess = "protected";
            else if (getMethod.IsFamilyAndAssembly)
                GetAccess = "protected internal";
            else
                GetAccess = "public";
            if (setMethod == null || setMethod.IsPrivate)
                SetAccess = "private";
            else if (setMethod.IsAssembly)
                SetAccess = "internal";
            else if (setMethod.IsFamily)
                SetAccess = "protected";
            else if (setMethod.IsFamilyAndAssembly)
                SetAccess = "protected internal";
            else
                SetAccess = "public";
            if (GetAccess == "public" || SetAccess == "public")
                Access = "public";
            else if (GetAccess == "protected internal" || SetAccess == "protected internal")
                Access = "protected internal";
            else if (GetAccess == "protected" || SetAccess == "protected")
                Access = "protected";
            else if (GetAccess == "internal" || SetAccess == "internal")
                Access = "internal";
            else
                Access = "private";
            if (getMethod != null)
                Modifier = (getMethod.IsAbstract) ? "abstract" : ((getMethod.IsFinal) ? "sealed" : ((getMethod.IsVirtual) ? "virtual" : ""));
            else if (setMethod != null)
                Modifier = (setMethod.IsAbstract) ? "abstract" : ((setMethod.IsFinal) ? "sealed" : ((setMethod.IsVirtual) ? "virtual" : ""));
        }
    }
}