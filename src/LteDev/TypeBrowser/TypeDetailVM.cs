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
    public class TypeDetailVM : TypeListItemVM
    {
        #region NestedTypes Property Members

        /// <summary>
        /// Defines the name for the <see cref="NestedTypes"/> dependency property.
        /// </summary>
        public const string PropertyName_NestedTypes = "NestedTypes";

        private static readonly DependencyPropertyKey NestedTypesPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_NestedTypes,
            typeof(ReadOnlyObservableCollection<TypeListItemVM>), typeof(TypeDetailVM),
            new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="NestedTypes"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty NestedTypesProperty = NestedTypesPropertyKey.DependencyProperty;

        /// <summary>
        /// Value of the model property <seealso cref="Assembly.NestedTypes"/>.
        /// </summary>
        public ReadOnlyObservableCollection<TypeListItemVM> NestedTypes
        {
            get { return (ReadOnlyObservableCollection<TypeListItemVM>)(GetValue(NestedTypesProperty)); }
            private set { SetValue(NestedTypesPropertyKey, value); }
        }

        #endregion
        
        #region CustomAttributes Property Members

        /// <summary>
        /// Defines the name for the <see cref="CustomAttributes"/> dependency property.
        /// </summary>
        public const string PropertyName_CustomAttributes = "CustomAttributes";

        private static readonly DependencyPropertyKey CustomAttributesPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_CustomAttributes,
            typeof(ReadOnlyObservableCollection<CustomAttributeVM>), typeof(TypeDetailVM),
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
        
        #region Interfaces Property Members

        /// <summary>
        /// Defines the name for the <see cref="Interfaces"/> dependency property.
        /// </summary>
        public const string PropertyName_Interfaces = "Interfaces";

        private static readonly DependencyPropertyKey InterfacesPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_Interfaces,
            typeof(ReadOnlyObservableCollection<TypeListItemVM>), typeof(TypeDetailVM),
            new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="Interfaces"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty InterfacesProperty = InterfacesPropertyKey.DependencyProperty;

        /// <summary>
        /// Value of the model property <seealso cref="Assembly.Interfaces"/>.
        /// </summary>
        public ReadOnlyObservableCollection<TypeListItemVM> Interfaces
        {
            get { return (ReadOnlyObservableCollection<TypeListItemVM>)(GetValue(InterfacesProperty)); }
            private set { SetValue(InterfacesPropertyKey, value); }
        }

        #endregion
        
        #region GenericArguments Property Members

        /// <summary>
        /// Defines the name for the <see cref="GenericArguments"/> dependency property.
        /// </summary>
        public const string PropertyName_GenericArguments = "GenericArguments";

        private static readonly DependencyPropertyKey GenericArgumentsPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_GenericArguments,
            typeof(ReadOnlyObservableCollection<TypeListItemVM>), typeof(TypeDetailVM),
            new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="GenericArguments"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty GenericArgumentsProperty = GenericArgumentsPropertyKey.DependencyProperty;

        /// <summary>
        /// Value of the model property <seealso cref="Assembly.GenericArguments"/>.
        /// </summary>
        public ReadOnlyObservableCollection<TypeListItemVM> GenericArguments
        {
            get { return (ReadOnlyObservableCollection<TypeListItemVM>)(GetValue(GenericArgumentsProperty)); }
            private set { SetValue(GenericArgumentsPropertyKey, value); }
        }

        #endregion
        
        #region GenericParameterConstraints Property Members

        /// <summary>
        /// Defines the name for the <see cref="GenericParameterConstraints"/> dependency property.
        /// </summary>
        public const string PropertyName_GenericParameterConstraints = "GenericParameterConstraints";

        private static readonly DependencyPropertyKey GenericParameterConstraintsPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_GenericParameterConstraints,
            typeof(ReadOnlyObservableCollection<TypeListItemVM>), typeof(TypeDetailVM),
            new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="GenericParameterConstraints"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty GenericParameterConstraintsProperty = GenericParameterConstraintsPropertyKey.DependencyProperty;

        /// <summary>
        /// Value of the model property <seealso cref="Assembly.GenericParameterConstraints"/>.
        /// </summary>
        public ReadOnlyObservableCollection<TypeListItemVM> GenericParameterConstraints
        {
            get { return (ReadOnlyObservableCollection<TypeListItemVM>)(GetValue(GenericParameterConstraintsProperty)); }
            private set { SetValue(GenericParameterConstraintsPropertyKey, value); }
        }

        #endregion
        
        #region Constructors Property Members

        /// <summary>
        /// Defines the name for the <see cref="Constructors"/> dependency property.
        /// </summary>
        public const string PropertyName_Constructors = "Constructors";

        private static readonly DependencyPropertyKey ConstructorsPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_Constructors,
            typeof(ReadOnlyObservableCollection<ConstructorListItemVM>), typeof(TypeDetailVM),
            new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="Constructors"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ConstructorsProperty = ConstructorsPropertyKey.DependencyProperty;

        /// <summary>
        /// Value of the model property <seealso cref="Assembly.Constructors"/>.
        /// </summary>
        public ReadOnlyObservableCollection<ConstructorListItemVM> Constructors
        {
            get { return (ReadOnlyObservableCollection<ConstructorListItemVM>)(GetValue(ConstructorsProperty)); }
            private set { SetValue(ConstructorsPropertyKey, value); }
        }

        #endregion
        
        #region Fields Property Members

        /// <summary>
        /// Defines the name for the <see cref="Fields"/> dependency property.
        /// </summary>
        public const string PropertyName_Fields = "Fields";

        private static readonly DependencyPropertyKey FieldsPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_Fields,
            typeof(ReadOnlyObservableCollection<FieldListItemVM>), typeof(TypeDetailVM),
            new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="Fields"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FieldsProperty = FieldsPropertyKey.DependencyProperty;

        /// <summary>
        /// Value of the model property <seealso cref="Assembly.Fields"/>.
        /// </summary>
        public ReadOnlyObservableCollection<FieldListItemVM> Fields
        {
            get { return (ReadOnlyObservableCollection<FieldListItemVM>)(GetValue(FieldsProperty)); }
            private set { SetValue(FieldsPropertyKey, value); }
        }

        #endregion
        
        #region Properties Property Members

        /// <summary>
        /// Defines the name for the <see cref="Properties"/> dependency property.
        /// </summary>
        public const string PropertyName_Properties = "Properties";

        private static readonly DependencyPropertyKey PropertiesPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_Properties,
            typeof(ReadOnlyObservableCollection<PropertyListItemVM>), typeof(TypeDetailVM),
            new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="Properties"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PropertiesProperty = PropertiesPropertyKey.DependencyProperty;

        /// <summary>
        /// Value of the model property <seealso cref="Assembly.Properties"/>.
        /// </summary>
        public ReadOnlyObservableCollection<PropertyListItemVM> Properties
        {
            get { return (ReadOnlyObservableCollection<PropertyListItemVM>)(GetValue(PropertiesProperty)); }
            private set { SetValue(PropertiesPropertyKey, value); }
        }

        #endregion
        
        #region Events Property Members

        /// <summary>
        /// Defines the name for the <see cref="Events"/> dependency property.
        /// </summary>
        public const string PropertyName_Events = "Events";

        private static readonly DependencyPropertyKey EventsPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_Events,
            typeof(ReadOnlyObservableCollection<EventListItemVM>), typeof(TypeDetailVM),
            new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="Events"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty EventsProperty = EventsPropertyKey.DependencyProperty;

        /// <summary>
        /// Value of the model property <seealso cref="Assembly.Events"/>.
        /// </summary>
        public ReadOnlyObservableCollection<EventListItemVM> Events
        {
            get { return (ReadOnlyObservableCollection<EventListItemVM>)(GetValue(EventsProperty)); }
            private set { SetValue(EventsPropertyKey, value); }
        }

        #endregion
        
        #region Methods Property Members

        /// <summary>
        /// Defines the name for the <see cref="Methods"/> dependency property.
        /// </summary>
        public const string PropertyName_Methods = "Methods";

        private static readonly DependencyPropertyKey MethodsPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_Methods,
            typeof(ReadOnlyObservableCollection<MethodListItemVM>), typeof(TypeDetailVM),
            new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="Methods"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MethodsProperty = MethodsPropertyKey.DependencyProperty;

        /// <summary>
        /// Value of the model property <seealso cref="Assembly.Methods"/>.
        /// </summary>
        public ReadOnlyObservableCollection<MethodListItemVM> Methods
        {
            get { return (ReadOnlyObservableCollection<MethodListItemVM>)(GetValue(MethodsProperty)); }
            private set { SetValue(MethodsPropertyKey, value); }
        }

        #endregion
        
        public TypeDetailVM() : base()
        {
            NestedTypes = new ReadOnlyObservableCollection<TypeListItemVM>(new ObservableCollection<TypeListItemVM>());
            Interfaces = new ReadOnlyObservableCollection<TypeListItemVM>(new ObservableCollection<TypeListItemVM>());
            GenericArguments = new ReadOnlyObservableCollection<TypeListItemVM>(new ObservableCollection<TypeListItemVM>());
            GenericParameterConstraints = new ReadOnlyObservableCollection<TypeListItemVM>(new ObservableCollection<TypeListItemVM>());
            Constructors = new ReadOnlyObservableCollection<ConstructorListItemVM>(new ObservableCollection<ConstructorListItemVM>());
            Fields = new ReadOnlyObservableCollection<FieldListItemVM>(new ObservableCollection<FieldListItemVM>());
            CustomAttributes = new ReadOnlyObservableCollection<CustomAttributeVM>(new ObservableCollection<CustomAttributeVM>());
            Properties = new ReadOnlyObservableCollection<PropertyListItemVM>(new ObservableCollection<PropertyListItemVM>());
            Events = new ReadOnlyObservableCollection<EventListItemVM>(new ObservableCollection<EventListItemVM>());
            Methods = new ReadOnlyObservableCollection<MethodListItemVM>(new ObservableCollection<MethodListItemVM>());
        }

        public TypeDetailVM(Type model)
            : base(model)
        {
            ObservableCollection<TypeListItemVM> nestedTypes = new ObservableCollection<TypeListItemVM>();
            NestedTypes = new ReadOnlyObservableCollection<TypeListItemVM>(nestedTypes);
            ObservableCollection<TypeListItemVM> interfaces = new ObservableCollection<TypeListItemVM>();
            Interfaces = new ReadOnlyObservableCollection<TypeListItemVM>(interfaces);
            ObservableCollection<TypeListItemVM> genericArguments = new ObservableCollection<TypeListItemVM>();
            GenericArguments = new ReadOnlyObservableCollection<TypeListItemVM>(genericArguments);
            ObservableCollection<TypeListItemVM> genericParameterConstraints = new ObservableCollection<TypeListItemVM>();
            GenericParameterConstraints = new ReadOnlyObservableCollection<TypeListItemVM>(genericParameterConstraints);
            ObservableCollection<CustomAttributeVM> customAttributes = new ObservableCollection<CustomAttributeVM>();
            CustomAttributes = new ReadOnlyObservableCollection<CustomAttributeVM>(customAttributes);
            ObservableCollection<ConstructorListItemVM> constructors = new ObservableCollection<ConstructorListItemVM>();
            Constructors = new ReadOnlyObservableCollection<ConstructorListItemVM>(constructors);
            ObservableCollection<FieldListItemVM> fields = new ObservableCollection<FieldListItemVM>();
            Fields = new ReadOnlyObservableCollection<FieldListItemVM>(fields);
            ObservableCollection<PropertyListItemVM> properties = new ObservableCollection<PropertyListItemVM>();
            Properties = new ReadOnlyObservableCollection<PropertyListItemVM>(properties);
            ObservableCollection<EventListItemVM> events = new ObservableCollection<EventListItemVM>();
            Events = new ReadOnlyObservableCollection<EventListItemVM>(events);
            ObservableCollection<MethodListItemVM> methods = new ObservableCollection<MethodListItemVM>();
            Methods = new ReadOnlyObservableCollection<MethodListItemVM>(methods);
            if (model == null)
                return;
            foreach (Type type in model.GetNestedTypes())
                nestedTypes.Add(new TypeListItemVM(type));
            foreach (Type type in model.GetInterfaces())
                interfaces.Add(new TypeListItemVM(type));
            foreach (Type type in model.GetGenericArguments())
                genericArguments.Add(new TypeListItemVM(type));
            foreach (Type type in model.GetGenericParameterConstraints())
                genericParameterConstraints.Add(new TypeListItemVM(type));
            foreach (Attribute attribute in model.GetCustomAttributes())
                customAttributes.Add(new CustomAttributeVM(attribute));
            foreach (ConstructorInfo constructor in model.GetConstructors())
                constructors.Add(new ConstructorListItemVM(constructor));
            foreach (FieldInfo field in model.GetFields())
                fields.Add(new FieldListItemVM(field));
            foreach (PropertyInfo property in model.GetProperties())
                properties.Add(new PropertyListItemVM(property));
            foreach (EventInfo e in model.GetEvents())
                events.Add(new EventListItemVM(e));
            foreach (MethodInfo e in model.GetMethods())
                methods.Add(new MethodListItemVM(e));
        }

        public TypeDetailVM(TypeListItemVM vm)
            : base(vm)
        {
            ObservableCollection<TypeListItemVM> nestedTypes = new ObservableCollection<TypeListItemVM>();
            NestedTypes = new ReadOnlyObservableCollection<TypeListItemVM>(nestedTypes);
            ObservableCollection<TypeListItemVM> interfaces = new ObservableCollection<TypeListItemVM>();
            Interfaces = new ReadOnlyObservableCollection<TypeListItemVM>(interfaces);
            ObservableCollection<TypeListItemVM> genericArguments = new ObservableCollection<TypeListItemVM>();
            GenericArguments = new ReadOnlyObservableCollection<TypeListItemVM>(genericArguments);
            ObservableCollection<TypeListItemVM> genericParameterConstraints = new ObservableCollection<TypeListItemVM>();
            GenericParameterConstraints = new ReadOnlyObservableCollection<TypeListItemVM>(genericParameterConstraints);
            ObservableCollection<CustomAttributeVM> customAttributes = new ObservableCollection<CustomAttributeVM>();
            CustomAttributes = new ReadOnlyObservableCollection<CustomAttributeVM>(customAttributes);
            ObservableCollection<ConstructorListItemVM> constructors = new ObservableCollection<ConstructorListItemVM>();
            Constructors = new ReadOnlyObservableCollection<ConstructorListItemVM>(constructors);
            ObservableCollection<FieldListItemVM> fields = new ObservableCollection<FieldListItemVM>();
            Fields = new ReadOnlyObservableCollection<FieldListItemVM>(fields);
            ObservableCollection<PropertyListItemVM> properties = new ObservableCollection<PropertyListItemVM>();
            Properties = new ReadOnlyObservableCollection<PropertyListItemVM>(properties);
            ObservableCollection<EventListItemVM> events = new ObservableCollection<EventListItemVM>();
            Events = new ReadOnlyObservableCollection<EventListItemVM>(events);
            ObservableCollection<MethodListItemVM> methods = new ObservableCollection<MethodListItemVM>();
            Methods = new ReadOnlyObservableCollection<MethodListItemVM>(methods);
            Type model = GetModel();
            if (model == null)
                return;
            foreach (Type type in model.GetNestedTypes())
                nestedTypes.Add(new TypeListItemVM(type));
            foreach (Type type in model.GetInterfaces())
                interfaces.Add(new TypeListItemVM(type));
            foreach (Type type in model.GetGenericArguments())
                genericArguments.Add(new TypeListItemVM(type));
            foreach (Type type in model.GetGenericParameterConstraints())
                genericParameterConstraints.Add(new TypeListItemVM(type));
            foreach (Attribute attribute in model.GetCustomAttributes())
                customAttributes.Add(new CustomAttributeVM(attribute));
            foreach (ConstructorInfo constructor in model.GetConstructors())
                constructors.Add(new ConstructorListItemVM(constructor));
            foreach (FieldInfo field in model.GetFields())
                fields.Add(new FieldListItemVM(field));
            foreach (PropertyInfo property in model.GetProperties())
                properties.Add(new PropertyListItemVM(property));
            foreach (EventInfo e in model.GetEvents())
                events.Add(new EventListItemVM(e));
            foreach (MethodInfo e in model.GetMethods())
                methods.Add(new MethodListItemVM(e));
        }
    }
}