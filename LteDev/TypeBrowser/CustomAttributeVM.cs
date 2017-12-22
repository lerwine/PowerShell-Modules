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
    public class CustomAttributeVM : DependencyObject
    {
        #region TypeName Property Members

        /// <summary>
        /// Defines the name for the <see cref="TypeName"/> dependency property.
        /// </summary>
        public const string PropertyName_TypeName = "TypeName";

        private static readonly DependencyPropertyKey TypeNamePropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_TypeName,
            typeof(string), typeof(CustomAttributeVM), new PropertyMetadata(""));

        /// <summary>
        /// Identifies the <see cref="TypeName"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TypeNameProperty = TypeNamePropertyKey.DependencyProperty;

        /// <summary>
        /// Value of the model property <seealso cref="TypeName.TypeName"/>.
        /// </summary>
        public string TypeName
        {
            get { return (string)(GetValue(TypeNameProperty)); }
            private set { SetValue(TypeNamePropertyKey, value); }
        }

        #endregion
        
        #region Values Property Members

        /// <summary>
        /// Defines the name for the <see cref="Values"/> dependency property.
        /// </summary>
        public const string PropertyName_Values = "Values";

        private static readonly DependencyPropertyKey ValuesPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_Values,
            typeof(string), typeof(CustomAttributeVM), new PropertyMetadata(""));

        /// <summary>
        /// Identifies the <see cref="Values"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ValuesProperty = ValuesPropertyKey.DependencyProperty;

        /// <summary>
        /// Value of the model property <seealso cref="Type.Values"/>.
        /// </summary>
        public string Values
        {
            get { return (string)(GetValue(ValuesProperty)); }
            private set { SetValue(ValuesPropertyKey, value); }
        }

        #endregion
        
        #region Properties Property Members

        /// <summary>
        /// Defines the name for the <see cref="Properties"/> dependency property.
        /// </summary>
        public const string PropertyName_Properties = "Properties";

        private static readonly DependencyPropertyKey PropertiesPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_Properties,
            typeof(ReadOnlyObservableCollection<MemberValueVM>), typeof(CustomAttributeVM),
            new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="Properties"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PropertiesProperty = PropertiesPropertyKey.DependencyProperty;

        /// <summary>
        /// Value of the model property <seealso cref="Assembly.Properties"/>.
        /// </summary>
        public ReadOnlyObservableCollection<MemberValueVM> Properties
        {
            get { return (ReadOnlyObservableCollection<MemberValueVM>)(GetValue(PropertiesProperty)); }
            private set { SetValue(PropertiesPropertyKey, value); }
        }

        #endregion
        
        public CustomAttributeVM()
        {
            Properties = new ReadOnlyObservableCollection<MemberValueVM>(new ObservableCollection<MemberValueVM>());
        }

        public CustomAttributeVM(Attribute attribute)
        {
            ObservableCollection<MemberValueVM> properties = new ObservableCollection<MemberValueVM>();
            Properties = new ReadOnlyObservableCollection<MemberValueVM>(properties);
            if (attribute == null)
                return;
            Type t = attribute.GetType();
            TypeName = TypeListItemVM.ToCSharpTypeName(t.FullName);
            foreach (MemberValueVM i in t.GetProperties().Select(p => new MemberValueVM(p, attribute))
                    .Concat(t.GetFields().Select(f => new MemberValueVM(f, attribute))).Where(m => m.IsPublic && !m.IsStatic))
                properties.Add(i);
            if (properties.Count == 0)
                return;
            Values = String.Join(", ", properties.Select(p => p.Name + " = " + ((p.ActualType.Length > 0) ? ((p.MemberType == "string") ? "\"" + p.Value.Replace("\\", "\\\\").Replace("\"", "\\\"") + "\"" : p.Value) : "null")).ToArray());
        }
    }
}