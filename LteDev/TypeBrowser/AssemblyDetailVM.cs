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
    public class AssemblyDetailVM : AssemblyListItemVM
    {
        #region ExportedTypes Property Members

        /// <summary>
        /// Defines the name for the <see cref="ExportedTypes"/> dependency property.
        /// </summary>
        public const string PropertyName_ExportedTypes = "ExportedTypes";

        private static readonly DependencyPropertyKey ExportedTypesPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_ExportedTypes,
            typeof(ReadOnlyObservableCollection<TypeListItemVM>), typeof(AssemblyDetailVM),
            new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="ExportedTypes"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ExportedTypesProperty = ExportedTypesPropertyKey.DependencyProperty;

        /// <summary>
        /// Value of the model property <seealso cref="Assembly.ExportedTypes"/>.
        /// </summary>
        public ReadOnlyObservableCollection<TypeListItemVM> ExportedTypes
        {
            get { return (ReadOnlyObservableCollection<TypeListItemVM>)(GetValue(ExportedTypesProperty)); }
            private set { SetValue(ExportedTypesPropertyKey, value); }
        }

        #endregion
        
        #region CustomAttributes Property Members

        /// <summary>
        /// Defines the name for the <see cref="CustomAttributes"/> dependency property.
        /// </summary>
        public const string PropertyName_CustomAttributes = "CustomAttributes";

        private static readonly DependencyPropertyKey CustomAttributesPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_CustomAttributes,
            typeof(ReadOnlyObservableCollection<CustomAttributeVM>), typeof(AssemblyDetailVM),
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
        
        #region ReferencedAssemblies Property Members

        /// <summary>
        /// Defines the name for the <see cref="ReferencedAssemblies"/> dependency property.
        /// </summary>
        public const string PropertyName_ReferencedAssemblies = "ReferencedAssemblies";

        private static readonly DependencyPropertyKey ReferencedAssembliesPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_ReferencedAssemblies,
            typeof(ReadOnlyObservableCollection<AssemblyNameListItemVM>), typeof(AssemblyDetailVM),
            new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="ReferencedAssemblies"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ReferencedAssembliesProperty = ReferencedAssembliesPropertyKey.DependencyProperty;

        /// <summary>
        /// Value of the model property <seealso cref="Type.ElementType"/> or <seealso cref="Type.UnderlyingSystemType"/>.
        /// </summary>
        public ReadOnlyObservableCollection<AssemblyNameListItemVM> ReferencedAssemblies
        {
            get { return (ReadOnlyObservableCollection<AssemblyNameListItemVM>)(GetValue(ReferencedAssembliesProperty)); }
            private set { SetValue(ReferencedAssembliesPropertyKey, value); }
        }

        #endregion
        
        public AssemblyDetailVM() : base()
        {
            ExportedTypes = new ReadOnlyObservableCollection<TypeListItemVM>(new ObservableCollection<TypeListItemVM>());
            CustomAttributes = new ReadOnlyObservableCollection<CustomAttributeVM>(new ObservableCollection<CustomAttributeVM>());
            ReferencedAssemblies = new ReadOnlyObservableCollection<AssemblyNameListItemVM>(new ObservableCollection<AssemblyNameListItemVM>());
        }

        public AssemblyDetailVM(AssemblyListItemVM vm) : base(vm)
        {
            Assembly model = GetModel();
            ObservableCollection<TypeListItemVM> exportedTypes = new ObservableCollection<TypeListItemVM>();
            ExportedTypes = new ReadOnlyObservableCollection<TypeListItemVM>(exportedTypes);
            ObservableCollection<CustomAttributeVM> customAttributes = new ObservableCollection<CustomAttributeVM>();
            CustomAttributes = new ReadOnlyObservableCollection<CustomAttributeVM>(customAttributes);
            ObservableCollection<AssemblyNameListItemVM> referencedAssemblies = new ObservableCollection<AssemblyNameListItemVM>();
            ReferencedAssemblies = new ReadOnlyObservableCollection<AssemblyNameListItemVM>(referencedAssemblies);
            if (model == null)
                return;
            foreach (Type type in model.GetExportedTypes())
                exportedTypes.Add(new TypeListItemVM(type));
            foreach (Attribute attribute in model.GetCustomAttributes())
                customAttributes.Add(new CustomAttributeVM(attribute));
            foreach (AssemblyName name in model.GetReferencedAssemblies())
                referencedAssemblies.Add(new AssemblyNameListItemVM(name));
        }
    }
}