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
    public class MethodListItemVM : MethodBaseListItemVM
    {
        #region GenericTypeArguments Property Members

        /// <summary>
        /// Defines the name for the <see cref="GenericTypeArguments"/> dependency property.
        /// </summary>
        public const string PropertyName_GenericTypeArguments = "GenericTypeArguments";

        private static readonly DependencyPropertyKey GenericTypeArgumentsPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_GenericTypeArguments,
            typeof(string), typeof(MethodListItemVM), new PropertyMetadata(""));

        /// <summary>
        /// Identifies the <see cref="GenericTypeArguments"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty GenericTypeArgumentsProperty = GenericTypeArgumentsPropertyKey.DependencyProperty;

        /// <summary>
        /// Value of the model property <seealso cref="Type.GenericTypeArguments"/>.
        /// </summary>
        public string GenericTypeArguments
        {
            get { return (string)(GetValue(GenericTypeArgumentsProperty)); }
            private set { SetValue(GenericTypeArgumentsPropertyKey, value); }
        }

        #endregion
        
        #region GenericType Property Members

        /// <summary>
        /// Defines the name for the <see cref="GenericType"/> dependency property.
        /// </summary>
        public const string PropertyName_GenericType = "GenericType";

        private static readonly DependencyPropertyKey GenericTypePropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_GenericType,
            typeof(string), typeof(MethodListItemVM), new PropertyMetadata(""));

        /// <summary>
        /// Identifies the <see cref="GenericType"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty GenericTypeProperty = GenericTypePropertyKey.DependencyProperty;

        /// <summary>
        /// Whether it is a generic type definition or a constructed generic type.
        /// </summary>
        public string GenericType
        {
            get { return (string)(GetValue(GenericTypeProperty)); }
            private set { SetValue(GenericTypePropertyKey, value); }
        }

        #endregion
        
        #region Name Property Members

        /// <summary>
        /// Defines the name for the <see cref="Name"/> dependency property.
        /// </summary>
        public const string PropertyName_Name = "Name";

        private static readonly DependencyPropertyKey NamePropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_Name,
            typeof(string), typeof(MethodListItemVM), new PropertyMetadata(""));

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
        
        #region IsStatic Property Members

        /// <summary>
        /// Defines the name for the <see cref="IsStatic"/> dependency property.
        /// </summary>
        public const string PropertyName_IsStatic = "IsStatic";

        private static readonly DependencyPropertyKey IsStaticPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_IsStatic,
            typeof(bool), typeof(MethodListItemVM), new PropertyMetadata(false));

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
        
        #region Modifier Property Members

        /// <summary>
        /// Defines the name for the <see cref="Modifier"/> dependency property.
        /// </summary>
        public const string PropertyName_Modifier = "Modifier";

        private static readonly DependencyPropertyKey ModifierPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_Modifier,
            typeof(string), typeof(MethodListItemVM), new PropertyMetadata(""));

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
        
        private MethodInfo _model = null;

        public MethodListItemVM() { }

        public MethodListItemVM(MethodInfo model)
            : base(model)
        {
            if (model == null)
                return;

            _model = model;
            Name = TypeListItemVM.TypeNameOnlyRegex.Match(model.Name).Value;
            GenericType = (model.IsGenericMethodDefinition) ? "Definition" : "Generic";
            if (model.IsGenericMethod)
                GenericTypeArguments = "<" + String.Join(",", model.GetGenericArguments().Select(t => TypeListItemVM.ToCSharpTypeName(t.FullName)).ToArray()) + ">";
            IsStatic = model.IsStatic;
            Modifier = (model.IsAbstract) ? "abstract" : ((model.IsFinal) ? "sealed" : ((model.IsVirtual) ? "virtual" : ""));
        }

        public MethodListItemVM(MethodListItemVM vm)
            : base(vm)
        {
            if (vm == null)
                return;
            _model = vm._model;
            Name = vm.Name;
            GenericType = vm.GenericType;
            GenericTypeArguments = vm.GenericTypeArguments;
            IsStatic = vm.IsStatic;
            Modifier = vm.Modifier;
        }
    }
}