using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;

namespace LteDev.TypeBrowser
{
    public class FieldListItemVM : MemberListItemVM
    {
        private FieldInfo _model = null;

        #region Access Property Members

        /// <summary>
        /// Defines the name for the <see cref="Access"/> dependency property.
        /// </summary>
        public const string PropertyName_Access = "Access";

        private static readonly DependencyPropertyKey AccessPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_Access,
            typeof(string), typeof(FieldListItemVM), new PropertyMetadata(""));

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
        
        #region IsInitOnly Property Members

        /// <summary>
        /// Defines the name for the <see cref="IsInitOnly"/> dependency property.
        /// </summary>
        public const string PropertyName_IsInitOnly = "IsInitOnly";

        private static readonly DependencyPropertyKey IsInitOnlyPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_IsInitOnly,
            typeof(bool), typeof(FieldListItemVM), new PropertyMetadata(false));

        /// <summary>
        /// Identifies the <see cref="IsInitOnly"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsInitOnlyProperty = IsInitOnlyPropertyKey.DependencyProperty;

        /// <summary>
        /// Value of the model property <seealso cref="Type.IsInitOnly"/>.
        /// </summary>
        public bool IsInitOnly
        {
            get { return (bool)(GetValue(IsInitOnlyProperty)); }
            private set { SetValue(IsInitOnlyPropertyKey, value); }
        }

        #endregion
        
        #region IsLiteral Property Members

        /// <summary>
        /// Defines the name for the <see cref="IsLiteral"/> dependency property.
        /// </summary>
        public const string PropertyName_IsLiteral = "IsLiteral";

        private static readonly DependencyPropertyKey IsLiteralPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_IsLiteral,
            typeof(bool), typeof(FieldListItemVM), new PropertyMetadata(false));

        /// <summary>
        /// Identifies the <see cref="IsLiteral"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsLiteralProperty = IsLiteralPropertyKey.DependencyProperty;

        /// <summary>
        /// Value of the model property <seealso cref="Type.IsLiteral"/>.
        /// </summary>
        public bool IsLiteral
        {
            get { return (bool)(GetValue(IsLiteralProperty)); }
            private set { SetValue(IsLiteralPropertyKey, value); }
        }

        #endregion
        
        public FieldListItemVM() : base() { }
        
        public FieldListItemVM(FieldListItemVM vm) : base(vm)
        {
            if (vm == null)
                return;
            _model = vm._model;
            Access = vm.Access;
            IsInitOnly = vm.IsInitOnly;
            IsLiteral = vm.IsLiteral;
        }

        public FieldListItemVM(FieldInfo model)
            : base(model)
        {
            _model = model;
            if (model == null)
                return;
            if (model.IsPrivate)
                Access = "private";
            else if (model.IsAssembly)
                Access = "internal";
            else if (model.IsFamily)
                Access = "protected";
            else if (model.IsFamilyAndAssembly)
                Access = "protected internal";
            else
                Access = "public";
            IsInitOnly = model.IsInitOnly;
            IsLiteral = model.IsLiteral;
        }
    }
}