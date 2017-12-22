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
    public class MethodBaseListItemVM : DependencyObject
    {
        #region CustomAttributes Property Members

        /// <summary>
        /// Defines the name for the <see cref="CustomAttributes"/> dependency property.
        /// </summary>
        public const string PropertyName_CustomAttributes = "CustomAttributes";

        private static readonly DependencyPropertyKey CustomAttributesPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_CustomAttributes,
            typeof(ReadOnlyObservableCollection<CustomAttributeVM>), typeof(MethodBaseListItemVM),
            new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="CustomAttributes"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CustomAttributesProperty = CustomAttributesPropertyKey.DependencyProperty;

        /// <summary>
        /// Value of the model property <seealso cref="Assembly.CustomAttributes"/>.
        /// </summary>
        public ReadOnlyObservableCollection<CustomAttributeVM> CustomAttributes
        {
            get { return (ReadOnlyObservableCollection<CustomAttributeVM>)(GetValue(CustomAttributesProperty)); }
            private set { SetValue(CustomAttributesPropertyKey, value); }
        }

        #endregion
        
        #region Parameters Property Members

        /// <summary>
        /// Defines the name for the <see cref="Parameters"/> dependency property.
        /// </summary>
        public const string PropertyName_Parameters = "Parameters";

        private static readonly DependencyPropertyKey ParametersPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_Parameters,
            typeof(ReadOnlyObservableCollection<ParameterListItemVM>), typeof(MethodBaseListItemVM),
            new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="Parameters"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ParametersProperty = ParametersPropertyKey.DependencyProperty;

        /// <summary>
        /// Value of the model property <seealso cref="Assembly.Parameters"/>.
        /// </summary>
        public ReadOnlyObservableCollection<ParameterListItemVM> Parameters
        {
            get { return (ReadOnlyObservableCollection<ParameterListItemVM>)(GetValue(ParametersProperty)); }
            private set { SetValue(ParametersPropertyKey, value); }
        }

        #endregion
        
        #region ParameterText Property Members

        /// <summary>
        /// Defines the name for the <see cref="ParameterText"/> dependency property.
        /// </summary>
        public const string PropertyName_ParameterText = "ParameterText";

        private static readonly DependencyPropertyKey ParameterTextPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_ParameterText,
            typeof(string), typeof(MethodBaseListItemVM), new PropertyMetadata(""));

        /// <summary>
        /// Identifies the <see cref="ParameterText"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ParameterTextProperty = ParameterTextPropertyKey.DependencyProperty;

        /// <summary>
        /// Value of the model property <seealso cref="Type.ParameterText"/>.
        /// </summary>
        public string ParameterText
        {
            get { return (string)(GetValue(ParameterTextProperty)); }
            private set { SetValue(ParameterTextPropertyKey, value); }
        }

        #endregion
        
        #region DeclaringType Property Members

        /// <summary>
        /// Defines the name for the <see cref="DeclaringType"/> dependency property.
        /// </summary>
        public const string PropertyName_DeclaringType = "DeclaringType";

        private static readonly DependencyPropertyKey DeclaringTypePropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_DeclaringType,
            typeof(string), typeof(MethodBaseListItemVM), new PropertyMetadata(""));

        /// <summary>
        /// Identifies the <see cref="DeclaringType"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DeclaringTypeProperty = DeclaringTypePropertyKey.DependencyProperty;

        /// <summary>
        /// Value of the model property <seealso cref="Type.DeclaringType"/>.
        /// </summary>
        public string DeclaringType
        {
            get { return (string)(GetValue(DeclaringTypeProperty)); }
            private set { SetValue(DeclaringTypePropertyKey, value); }
        }

        #endregion
        
        #region ReflectedType Property Members

        /// <summary>
        /// Defines the name for the <see cref="ReflectedType"/> dependency property.
        /// </summary>
        public const string PropertyName_ReflectedType = "ReflectedType";

        private static readonly DependencyPropertyKey ReflectedTypePropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_ReflectedType,
            typeof(string), typeof(MethodBaseListItemVM), new PropertyMetadata(""));

        /// <summary>
        /// Identifies the <see cref="ReflectedType"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ReflectedTypeProperty = ReflectedTypePropertyKey.DependencyProperty;

        /// <summary>
        /// Value of the model property <seealso cref="Type.ReflectedType"/>.
        /// </summary>
        public string ReflectedType
        {
            get { return (string)(GetValue(ReflectedTypeProperty)); }
            private set { SetValue(ReflectedTypePropertyKey, value); }
        }

        #endregion
        
        #region Access Property Members

        /// <summary>
        /// Defines the name for the <see cref="Access"/> dependency property.
        /// </summary>
        public const string PropertyName_Access = "Access";

        private static readonly DependencyPropertyKey AccessPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_Access,
            typeof(string), typeof(MethodBaseListItemVM), new PropertyMetadata(""));

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
        
        public MethodBaseListItemVM()
        {
            CustomAttributes = new ReadOnlyObservableCollection<CustomAttributeVM>(new ObservableCollection<CustomAttributeVM>());
            Parameters = new ReadOnlyObservableCollection<ParameterListItemVM>(new ObservableCollection<ParameterListItemVM>());
        }

        private MethodBase _model = null;

        public MethodBaseListItemVM(MethodBase model)
        {
            ObservableCollection<CustomAttributeVM> customAttributes = new ObservableCollection<CustomAttributeVM>();
            CustomAttributes = new ReadOnlyObservableCollection<CustomAttributeVM>(customAttributes);
            ObservableCollection<ParameterListItemVM> parameters = new ObservableCollection<ParameterListItemVM>();
            Parameters = new ReadOnlyObservableCollection<ParameterListItemVM>(parameters);

            _model = model;
            if (model == null)
                return;

            if (model == null)
                return;
            foreach (Attribute attribute in model.GetCustomAttributes())
                customAttributes.Add(new CustomAttributeVM(attribute));
            foreach (ParameterInfo p in model.GetParameters())
                parameters.Add(new ParameterListItemVM(p));
            if (parameters.Count > 0)
                ParameterText = String.Join(", ", parameters.Select(p => p.GetModel().ToString()).ToArray());
            DeclaringType = (model.DeclaringType == null) ? "" : TypeListItemVM.ToCSharpTypeName(model.DeclaringType.FullName);
            ReflectedType = (model.ReflectedType == null) ? "" : TypeListItemVM.ToCSharpTypeName(model.ReflectedType.FullName);
            Access = (model.IsPublic) ? "public" : ((model.IsFamilyAndAssembly) ? "protected internal" : ((model.IsAssembly) ? "internal" : ((model.IsFamily) ? "protected" : ((model.IsPrivate) ? "private" : ""))));
        }

        public MethodBaseListItemVM(MethodBaseListItemVM vm)
        {
            if (vm == null)
            {
                CustomAttributes = new ReadOnlyObservableCollection<CustomAttributeVM>(new ObservableCollection<CustomAttributeVM>());
                Parameters = new ReadOnlyObservableCollection<ParameterListItemVM>(new ObservableCollection<ParameterListItemVM>());
                return;
            }

            _model = vm._model;
            Parameters = vm.Parameters;
            CustomAttributes = vm.CustomAttributes;
            ParameterText = vm.ParameterText;
            DeclaringType = vm.DeclaringType;
            ReflectedType = vm.ReflectedType;
            Access = vm.Access;
        }
    }
}