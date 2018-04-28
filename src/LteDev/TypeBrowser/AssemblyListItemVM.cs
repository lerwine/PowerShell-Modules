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
    public class AssemblyListItemVM : AssemblyNameListItemVM
    {
        #region IsFullyTrusted Property Members

        /// <summary>
        /// Defines the name for the <see cref="IsFullyTrusted"/> dependency property.
        /// </summary>
        public const string PropertyName_IsFullyTrusted = "IsFullyTrusted";

        private static readonly DependencyPropertyKey IsFullyTrustedPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_IsFullyTrusted,
            typeof(bool), typeof(AssemblyListItemVM), new PropertyMetadata(false));

        /// <summary>
        /// Identifies the <see cref="IsFullyTrusted"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsFullyTrustedProperty = IsFullyTrustedPropertyKey.DependencyProperty;

        /// <summary>
        /// Value of the model property <seealso cref="Assembly.IsFullyTrusted"/>.
        /// </summary>
        public bool IsFullyTrusted
        {
            get { return (bool)(GetValue(IsFullyTrustedProperty)); }
            private set { SetValue(IsFullyTrustedPropertyKey, value); }
        }

        #endregion
        
        #region Location Property Members

        /// <summary>
        /// Defines the name for the <see cref="Location"/> dependency property.
        /// </summary>
        public const string PropertyName_Location = "Location";

        private static readonly DependencyPropertyKey LocationPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_Location,
            typeof(string), typeof(AssemblyListItemVM), new PropertyMetadata(""));

        /// <summary>
        /// Identifies the <see cref="Location"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LocationProperty = LocationPropertyKey.DependencyProperty;

        /// <summary>
        /// Value of the model property <seealso cref="Assembly.Location"/>.
        /// </summary>
        public string Location
        {
            get { return (string)(GetValue(LocationProperty)); }
            private set { SetValue(LocationPropertyKey, value); }
        }

        #endregion
        
        #region ImageRuntimeVersion Property Members

        /// <summary>
        /// Defines the name for the <see cref="ImageRuntimeVersion"/> dependency property.
        /// </summary>
        public const string PropertyName_ImageRuntimeVersion = "ImageRuntimeVersion";

        private static readonly DependencyPropertyKey ImageRuntimeVersionPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_ImageRuntimeVersion,
            typeof(string), typeof(AssemblyListItemVM), new PropertyMetadata(""));

        /// <summary>
        /// Identifies the <see cref="ImageRuntimeVersion"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ImageRuntimeVersionProperty = ImageRuntimeVersionPropertyKey.DependencyProperty;

        /// <summary>
        /// Value of the model property <seealso cref="Assembly.ImageRuntimeVersion"/>.
        /// </summary>
        public string ImageRuntimeVersion
        {
            get { return (string)(GetValue(ImageRuntimeVersionProperty)); }
            private set { SetValue(ImageRuntimeVersionPropertyKey, value); }
        }

        #endregion

        #region GlobalAssemblyCache Property Members

        /// <summary>
        /// Defines the name for the <see cref="GlobalAssemblyCache"/> dependency property.
        /// </summary>
        public const string PropertyName_GlobalAssemblyCache = "GlobalAssemblyCache";

        private static readonly DependencyPropertyKey GlobalAssemblyCachePropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_GlobalAssemblyCache,
            typeof(bool), typeof(AssemblyListItemVM), new PropertyMetadata(false));

        /// <summary>
        /// Identifies the <see cref="GlobalAssemblyCache"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty GlobalAssemblyCacheProperty = GlobalAssemblyCachePropertyKey.DependencyProperty;

        /// <summary>
        /// Value of the model property <seealso cref="Assembly.GlobalAssemblyCache"/>.
        /// </summary>
        public bool GlobalAssemblyCache
        {
            get { return (bool)(GetValue(GlobalAssemblyCacheProperty)); }
            private set { SetValue(GlobalAssemblyCachePropertyKey, value); }
        }

        #endregion

        private Assembly _model = null;

        /// <summary>
        /// Initializes a new <see cref="AssemblyListItemVM" />.
        /// </summary>
        public AssemblyListItemVM() : base() { }

        /// <summary>
        /// Initializes a new <see cref="AssemblyListItemVM" />.
        /// </summary>
        /// <param name="model">The <seealso cref="Assembly" /> that the view model is to represent.</param>
        public AssemblyListItemVM(Assembly model)
            : base((model == null) ? null : model.GetName())
        {
            _model = model;
            if (model == null)
                return;
            IsFullyTrusted = model.IsFullyTrusted;
            Location = model.Location ?? "";
            ImageRuntimeVersion = model.ImageRuntimeVersion ?? "";
            GlobalAssemblyCache = model.GlobalAssemblyCache;
        }

        /// <summary>
        /// Initializes a new <see cref="AssemblyListItemVM" />.
        /// </summary>
        /// <param name="model">The <seealso cref="Assembly" /> that the view model is to represent.</param>
        public AssemblyListItemVM(AssemblyListItemVM vm)
        {
            if (vm == null)
                return;
            IsFullyTrusted = vm.IsFullyTrusted;
            Location = vm.Location;
            ImageRuntimeVersion = vm.ImageRuntimeVersion;
            GlobalAssemblyCache = vm.GlobalAssemblyCache;
        }

        /// <summary>
        /// Gets the <seealso cref="Assembly" /> that this view model represents.
        /// </summary>
        /// <returns>The <seealso cref="Assembly" /> that this view model represents.</returns>
        public Assembly GetModel() { return _model; }
    }
}
