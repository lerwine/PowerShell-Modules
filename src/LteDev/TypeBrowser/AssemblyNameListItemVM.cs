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
    /// Represents an <seealso cref="Assembly" />.
    /// </summary>
    public class AssemblyNameListItemVM : DependencyObject
    {
        #region CodeBase Property Members

        /// <summary>
        /// Defines the name for the <see cref="CodeBase"/> dependency property.
        /// </summary>
        public const string PropertyName_CodeBase = "CodeBase";

        private static readonly DependencyPropertyKey CodeBasePropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_CodeBase,
            typeof(string), typeof(AssemblyNameListItemVM), new PropertyMetadata(""));

        /// <summary>
        /// Identifies the <see cref="CodeBase"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CodeBaseProperty = CodeBasePropertyKey.DependencyProperty;

        /// <summary>
        /// Value of the model property <seealso cref="Assembly.CodeBase"/>.
        /// </summary>
        public string CodeBase
        {
            get { return (string)(GetValue(CodeBaseProperty)); }
            private set { SetValue(CodeBasePropertyKey, value); }
        }

        #endregion
        
        #region FullName Property Members

        /// <summary>
        /// Defines the name for the <see cref="FullName"/> dependency property.
        /// </summary>
        public const string PropertyName_FullName = "FullName";

        private static readonly DependencyPropertyKey FullNamePropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_FullName,
            typeof(string), typeof(AssemblyNameListItemVM), new PropertyMetadata(""));

        /// <summary>
        /// Identifies the <see cref="FullName"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FullNameProperty = FullNamePropertyKey.DependencyProperty;

        /// <summary>
        /// Value of the model property <seealso cref="Assembly.FullName"/>.
        /// </summary>
        public string FullName
        {
            get { return (string)(GetValue(FullNameProperty)); }
            private set { SetValue(FullNamePropertyKey, value); }
        }

        #endregion
        
        #region Name Property Members

        /// <summary>
        /// Defines the name for the <see cref="Name"/> dependency property.
        /// </summary>
        public const string PropertyName_Name = "Name";

        private static readonly DependencyPropertyKey NamePropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_Name,
            typeof(string), typeof(AssemblyNameListItemVM), new PropertyMetadata(""));

        /// <summary>
        /// Identifies the <see cref="Name"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty NameProperty = NamePropertyKey.DependencyProperty;

        /// <summary>
        /// Value of the <seealso cref="AssemblyName.Name"/> property taken from <seealso cref="Assembly.GetName()"/>.
        /// </summary>
        public string Name
        {
            get { return (string)(GetValue(NameProperty)); }
            private set { SetValue(NamePropertyKey, value); }
        }

        #endregion

        #region Version Property Members

        /// <summary>
        /// Defines the name for the <see cref="Version"/> dependency property.
        /// </summary>
        public const string PropertyName_Version = "Version";

        private static readonly DependencyPropertyKey VersionPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_Version,
            typeof(string), typeof(AssemblyNameListItemVM), new PropertyMetadata(""));

        /// <summary>
        /// Identifies the <see cref="Version"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty VersionProperty = VersionPropertyKey.DependencyProperty;

        /// <summary>
        /// Value of the <seealso cref="AssemblyName.Version"/> property taken from <seealso cref="Assembly.GetName()"/>.
        /// </summary>
        public string Version
        {
            get { return (string)(GetValue(VersionProperty)); }
            private set { SetValue(VersionPropertyKey, value); }
        }

        #endregion

        #region Culture Property Members

        /// <summary>
        /// Defines the name for the <see cref="Culture"/> dependency property.
        /// </summary>
        public const string PropertyName_Culture = "Culture";

        private static readonly DependencyPropertyKey CulturePropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_Culture,
            typeof(string), typeof(AssemblyNameListItemVM), new PropertyMetadata(""));

        /// <summary>
        /// Identifies the <see cref="Culture"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CultureProperty = CulturePropertyKey.DependencyProperty;

        /// <summary>
        /// Value of the <seealso cref="AssemblyName.CultureName"/> property taken from <seealso cref="Assembly.GetName()"/>.
        /// </summary>
        public string Culture
        {
            get { return (string)(GetValue(CultureProperty)); }
            private set { SetValue(CulturePropertyKey, value); }
        }

        #endregion

        #region ProcessorArchitecture Property Members

        /// <summary>
        /// Defines the name for the <see cref="ProcessorArchitecture"/> dependency property.
        /// </summary>
        public const string PropertyName_ProcessorArchitecture = "ProcessorArchitecture";

        private static readonly DependencyPropertyKey ProcessorArchitecturePropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_ProcessorArchitecture,
            typeof(ProcessorArchitecture), typeof(AssemblyNameListItemVM),
            new PropertyMetadata(ProcessorArchitecture.None));

        /// <summary>
        /// Identifies the <see cref="ProcessorArchitecture"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ProcessorArchitectureProperty = ProcessorArchitecturePropertyKey.DependencyProperty;

        /// <summary>
        /// Value of the <seealso cref="AssemblyName.ProcessorArchitecture"/> property taken from <seealso cref="Assembly.GetName()"/>.
        /// </summary>
        public ProcessorArchitecture ProcessorArchitecture
        {
            get { return (ProcessorArchitecture)(GetValue(ProcessorArchitectureProperty)); }
            private set { SetValue(ProcessorArchitecturePropertyKey, value); }
        }

        #endregion

        private AssemblyName _model = null;

        /// <summary>
        /// Initializes a new <see cref="AssemblyNameListItemVM" />.
        /// </summary>
        public AssemblyNameListItemVM() { }

        /// <summary>
        /// Initializes a new <see cref="AssemblyNameListItemVM" />.
        /// </summary>
        /// <param name="model">The <seealso cref="Assembly" /> that the view model is to represent.</param>
        public AssemblyNameListItemVM(AssemblyName model)
        {
            _model = model;
            if (model == null)
                return;
            CodeBase = model.CodeBase ?? "";
            FullName = model.FullName ?? "";
            Name = model.Name ?? "";
            Version = (model.Version == null) ? "" : model.Version.ToString();
            Culture = model.CultureName ?? "";
            ProcessorArchitecture = model.ProcessorArchitecture;
        }

        /// <summary>
        /// Initializes a new <see cref="AssemblyNameListItemVM" />.
        /// </summary>
        /// <param name="model">The <seealso cref="Assembly" /> that the view model is to represent.</param>
        public AssemblyNameListItemVM(AssemblyNameListItemVM vm)
        {
            if (vm == null)
                return;
            _model = vm._model;
            if (_model == null)
                return;
            CodeBase = vm.CodeBase;
            FullName = vm.FullName;
            Name = vm.Name;
            Version = vm.Version;
            Culture = vm.Culture;
            ProcessorArchitecture = vm.ProcessorArchitecture;
        }

        /// <summary>
        /// Gets the <seealso cref="Assembly" /> that this view model represents.
        /// </summary>
        /// <returns>The <seealso cref="Assembly" /> that this view model represents.</returns>
        public AssemblyName GetName() { return _model; }
    }
}
