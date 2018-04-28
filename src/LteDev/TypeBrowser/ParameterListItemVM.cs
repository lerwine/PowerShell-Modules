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
    /// Represents a <seealso cref="ParameterInfo" />.
    /// </summary>
    public class ParameterListItemVM : DependencyObject
    {
        private ParameterInfo _model = null;

        #region Name Property Members

        /// <summary>
        /// Defines the name for the <see cref="Name"/> dependency property.
        /// </summary>
        public const string PropertyName_Name = "Name";

        private static readonly DependencyPropertyKey NamePropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_Name,
            typeof(string), typeof(ParameterListItemVM), new PropertyMetadata(""));

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
        
        #region DefaultValue Property Members

        /// <summary>
        /// Defines the name for the <see cref="DefaultValue"/> dependency property.
        /// </summary>
        public const string PropertyName_DefaultValue = "DefaultValue";

        private static readonly DependencyPropertyKey DefaultValuePropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_DefaultValue,
            typeof(string), typeof(ParameterListItemVM), new PropertyMetadata(""));

        /// <summary>
        /// Identifies the <see cref="DefaultValue"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DefaultValueProperty = DefaultValuePropertyKey.DependencyProperty;

        /// <summary>
        /// Value of the model property <seealso cref="Type.ElementType"/> or <seealso cref="Type.UnderlyingSystemType"/>.
        /// </summary>
        public string DefaultValue
        {
            get { return (string)(GetValue(DefaultValueProperty)); }
            private set { SetValue(DefaultValuePropertyKey, value); }
        }

        #endregion
        
        #region ParameterType Property Members

        /// <summary>
        /// Defines the name for the <see cref="ParameterType"/> dependency property.
        /// </summary>
        public const string PropertyName_ParameterType = "ParameterType";

        private static readonly DependencyPropertyKey ParameterTypePropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_ParameterType,
            typeof(string), typeof(ParameterListItemVM), new PropertyMetadata(""));

        /// <summary>
        /// Identifies the <see cref="ParameterType"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ParameterTypeProperty = ParameterTypePropertyKey.DependencyProperty;

        /// <summary>
        /// Value of the model property <seealso cref="Type.ElementType"/> or <seealso cref="Type.UnderlyingSystemType"/>.
        /// </summary>
        public string ParameterType
        {
            get { return (string)(GetValue(ParameterTypeProperty)); }
            private set { SetValue(ParameterTypePropertyKey, value); }
        }

        #endregion
        
        #region IsIn Property Members

        /// <summary>
        /// Defines the name for the <see cref="IsIn"/> dependency property.
        /// </summary>
        public const string PropertyName_IsIn = "IsIn";

        private static readonly DependencyPropertyKey IsInPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_IsIn,
            typeof(bool), typeof(ParameterListItemVM), new PropertyMetadata(false));

        /// <summary>
        /// Identifies the <see cref="IsIn"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsInProperty = IsInPropertyKey.DependencyProperty;

        /// <summary>
        /// Value of the model property <seealso cref="Type.IsIn"/>.
        /// </summary>
        public bool IsIn
        {
            get { return (bool)(GetValue(IsInProperty)); }
            private set { SetValue(IsInPropertyKey, value); }
        }

        #endregion
        
        #region IsOut Property Members

        /// <summary>
        /// Defines the name for the <see cref="IsOut"/> dependency property.
        /// </summary>
        public const string PropertyName_IsOut = "IsOut";

        private static readonly DependencyPropertyKey IsOutPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_IsOut,
            typeof(bool), typeof(ParameterListItemVM), new PropertyMetadata(false));

        /// <summary>
        /// Identifies the <see cref="IsOut"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsOutProperty = IsOutPropertyKey.DependencyProperty;

        /// <summary>
        /// Value of the model property <seealso cref="Type.IsOut"/>.
        /// </summary>
        public bool IsOut
        {
            get { return (bool)(GetValue(IsOutProperty)); }
            private set { SetValue(IsOutPropertyKey, value); }
        }

        #endregion
        
        #region IsRetval Property Members

        /// <summary>
        /// Defines the name for the <see cref="IsRetval"/> dependency property.
        /// </summary>
        public const string PropertyName_IsRetval = "IsRetval";

        private static readonly DependencyPropertyKey IsRetvalPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_IsRetval,
            typeof(bool), typeof(ParameterListItemVM), new PropertyMetadata(false));

        /// <summary>
        /// Identifies the <see cref="IsRetval"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsRetvalProperty = IsRetvalPropertyKey.DependencyProperty;

        /// <summary>
        /// Value of the model property <seealso cref="Type.IsRetval"/>.
        /// </summary>
        public bool IsRetval
        {
            get { return (bool)(GetValue(IsRetvalProperty)); }
            private set { SetValue(IsRetvalPropertyKey, value); }
        }

        #endregion
        
        #region IsOptional Property Members

        /// <summary>
        /// Defines the name for the <see cref="IsOptional"/> dependency property.
        /// </summary>
        public const string PropertyName_IsOptional = "IsOptional";

        private static readonly DependencyPropertyKey IsOptionalPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_IsOptional,
            typeof(bool), typeof(ParameterListItemVM), new PropertyMetadata(false));

        /// <summary>
        /// Identifies the <see cref="IsOptional"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsOptionalProperty = IsOptionalPropertyKey.DependencyProperty;

        /// <summary>
        /// Value of the model property <seealso cref="Type.IsOptional"/>.
        /// </summary>
        public bool IsOptional
        {
            get { return (bool)(GetValue(IsOptionalProperty)); }
            private set { SetValue(IsOptionalPropertyKey, value); }
        }

        #endregion
        
        #region Position Property Members

        /// <summary>
        /// Defines the name for the <see cref="Position"/> dependency property.
        /// </summary>
        public const string PropertyName_Position = "Position";

        private static readonly DependencyPropertyKey PositionPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_Position,
            typeof(int), typeof(ParameterListItemVM), new PropertyMetadata(-1));

        /// <summary>
        /// Identifies the <see cref="Position"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PositionProperty = PositionPropertyKey.DependencyProperty;

        /// <summary>
        /// Value of the model property <seealso cref="Type.Position"/>.
        /// </summary>
        public int Position
        {
            get { return (int)(GetValue(PositionProperty)); }
            private set { SetValue(PositionPropertyKey, value); }
        }

        #endregion
        
#pragma warning disable 1591 // Missing XML comment for publicly visible type or member
        public ParameterListItemVM() { }

        public ParameterListItemVM(ParameterListItemVM vm)
        {
            if (vm == null)
                return;
            _model = vm._model;
            Name = vm.Name;
            DefaultValue = vm.DefaultValue;
            ParameterType = vm.ParameterType;
            IsIn = vm.IsIn;
            IsOut = vm.IsOut;
            IsOptional = vm.IsOptional;
            IsRetval = vm.IsRetval;
            Position = vm.Position;
        }

        public ParameterListItemVM(ParameterInfo model)
        {
            _model = model;
            if (model == null)
                return;
            Name = model.Name;
            DefaultValue = (model.HasDefaultValue) ? 
                ((model.DefaultValue == null) ? "null" :
                ((model.DefaultValue is string) ? "\"" +
                    ((string)(model.DefaultValue)).Replace("\\", "\\\\").Replace("\"", "\\\"") + "\"" :
                    model.DefaultValue.ToString())) : "";
            ParameterType = TypeListItemVM.ToCSharpTypeName(model.ParameterType.FullName);
            IsIn = model.IsIn;
            IsOut = model.IsOut;
            IsOptional = model.IsOptional;
            IsRetval = model.IsRetval;
            Position = model.Position;
        }

        public ParameterInfo GetModel() { return _model; }
#pragma warning restore 1591 // Missing XML comment for publicly visible type or member
    }
}