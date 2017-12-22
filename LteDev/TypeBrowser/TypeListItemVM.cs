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
    public class TypeListItemVM : DependencyObject
    {
        private Type _model = null;

        #region To*TypeName

        public static readonly Regex TypeNameParseRegex = new Regex(@"(^|\G)(?<b>\[)?(?<fn>((?<ns><[\w-]+>|[\w-]+(\.[\w-]+(?=\.))*)\.)?((?<dt>[\w-]+)\+)?(?<name>(<[\w-]+>)+|(<>)?[\w?$@-]+(<(\w+::)+\w+(\s*(\^|\\\*))?>|\{\d+\})?))(`(?<gCount>\d+))?(\[(?<gArgs>(?=[^,\]])([^\[\],]*(?>(?<g>\[)[^\[\]]*)*(?>(?<-g>\])[^\[\]]*)*(?(g)(?!))(\[[,\s]*\]|\*|&)*(\+|,\s*)?)*)\])?(?<indexer>(\[[,\s]*\]|\*|&)+)?(,\s*[\w-]+(\.[\w-]+)*(,\s*\w+=[\w.]+)+)?(?<-b>\])?(?(b)(?!))((?<j>\+|,\s*)|$)", RegexOptions.Compiled);

        public static readonly Regex TypeNameOnlyRegex = new Regex(@"^[^\[\]`<>()]*", RegexOptions.Compiled);

        /// <summary>
        /// Converts a type name to c# code.
        /// </summary>
        /// <param cref="typeName">The name of <seealso cref="Type" /> (can be full name or assembly qualified name).</param>
        /// <returns>Text which can be used as a type specification in c#.</returns>
        public static string ToCSharpTypeName(string typeName, params string[] usings)
        {
            if (usings == null)
                usings = new string[0];
            else
                usings = usings.Where(s => s != null).Select(s => s.Trim()).Where(s => s.Length > 0).ToArray();
            if (typeName == null || (typeName = typeName.Trim()).Length == 0)
                return "";
            
            return _ToCSharpTypeName(typeName, usings);
        }

        private static string _ToCSharpTypeName(string typeName, params string[] usings)
        {
            MatchCollection matches = TypeNameParseRegex.Matches(typeName);
            if (matches.Count == 0)
                return typeName;
            StringBuilder result = new StringBuilder();
            foreach (Match m in matches)
            {
                switch (m.Groups["fn"].Value)
                {
                    case "System.String":
                        result.Append("string");
                        break;
                    case "System.Boolean":
                        result.Append("bool");
                        break;
                    case "System.Decimal":
                        result.Append("decimal");
                        break;
                    case "System.Double":
                        result.Append("double");
                        break;
                    case "System.Single":
                        result.Append("float");
                        break;
                    case "System.Char":
                        result.Append("char");
                        break;
                    case "System.Byte":
                        result.Append("byte");
                        break;
                    case "System.SByte":
                        result.Append("sbyte");
                        break;
                    case "System.Int16":
                        result.Append("short");
                        break;
                    case "System.Int32":
                        result.Append("int");
                        break;
                    case "System.Int64":
                        result.Append("long");
                        break;
                    case "System.UInt16":
                        result.Append("ushort");
                        break;
                    case "System.UInt32":
                        result.Append("uint");
                        break;
                    case "System.UInt64":
                        result.Append("ulong");
                        break;
                    default:
                        if (m.Groups["ns"].Success)
                        {
                            string ns = m.Groups["ns"].Value;
                            if (!usings.Any(s => s == ns))
                                result.Append(ns).Append(".");
                        }
                        if (m.Groups["dt"].Success)
                            result.Append(m.Groups["dt"].Value).Append('.');
                        result.Append(m.Groups["name"].Value);
                        int c;
                        if (m.Groups["gArgs"].Success)
                            result.Append('<').Append(_ToCSharpTypeName(m.Groups["gArgs"].Value, usings)).Append('>');
                        else if (m.Groups["gCount"].Success && int.TryParse(m.Groups["gCount"].Value, out c))
                        {
                            result.Append('<');
                            if (c == 1)
                                result.Append(',');
                            else if (c > 1)
                                result.Append(',', c - 1);
                            result.Append('>');
                        }
                        break;
                }
                if (m.Groups["indexer"].Success)
                    result.Append(m.Groups["indexer"].Value);
                if (m.Groups["j"].Success)
                    result.Append(m.Groups["j"].Value);
            }
            Group grp = matches[matches.Count - 1];
            int idx = grp.Index + grp.Length;
            if (idx < typeName.Length)
                result.Append(typeName.Substring(idx));
            return result.ToString();
        }

        /// <summary>
        /// Converts a type name to PowerShell code.
        /// </summary>
        /// <param cref="typeName">The name of <seealso cref="Type" /> (can be full name or assembly qualified name).</param>
        /// <returns>Text which can be used as a type specification in PowerShell.</returns>
        public static string ToPSTypeName(string typeName)
        {
            if (typeName == null || (typeName = typeName.Trim()).Length == 0)
                return "";
            
            string s = LanguagePrimitives.ConvertTypeNameToPSTypeName(_ToPSTypeName(typeName));
            return s.Substring(1, s.Length - 2);
        }

        private static string _ToPSTypeName(string typeName)
        {   
            MatchCollection matches = TypeNameParseRegex.Matches(typeName);
            if (matches.Count == 0)
                return typeName;
            StringBuilder result = new StringBuilder();
            foreach (Match m in matches)
            {
                if (m.Groups["ns"].Success)
                    result.Append(m.Groups["ns"].Value).Append(".");
                if (m.Groups["dt"].Success)
                    result.Append(m.Groups["dt"].Value).Append('+');
                result.Append(m.Groups["name"].Value);
                if (m.Groups["gArgs"].Success)
                    result.Append('[').Append(_ToPSTypeName(m.Groups["gArgs"].Value)).Append(']');
                else if (m.Groups["gCount"].Success)
                    result.Append('`').Append(m.Groups["gCount"].Value);
                if (m.Groups["indexer"].Success)
                    result.Append(m.Groups["indexer"].Value);
                if (m.Groups["j"].Success)
                    result.Append(m.Groups["j"].Value);
            }
            Group grp = matches[matches.Count - 1];
            int idx = grp.Index + grp.Length;
            if (idx < typeName.Length)
                result.Append(typeName.Substring(idx));
            return result.ToString();
        }

        #endregion
        
        #region DeclaringType Property Members

        /// <summary>
        /// Defines the name for the <see cref="DeclaringType"/> dependency property.
        /// </summary>
        public const string PropertyName_DeclaringType = "DeclaringType";

        private static readonly DependencyPropertyKey DeclaringTypePropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_DeclaringType,
            typeof(string), typeof(TypeListItemVM), new PropertyMetadata(""));

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
        
        #region DeclaringMethod Property Members

        /// <summary>
        /// Defines the name for the <see cref="DeclaringMethod"/> dependency property.
        /// </summary>
        public const string PropertyName_DeclaringMethod = "DeclaringMethod";

        private static readonly DependencyPropertyKey DeclaringMethodPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_DeclaringMethod,
            typeof(string), typeof(TypeListItemVM), new PropertyMetadata(""));

        /// <summary>
        /// Identifies the <see cref="DeclaringMethod"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DeclaringMethodProperty = DeclaringMethodPropertyKey.DependencyProperty;

        /// <summary>
        /// Value of the model property <seealso cref="Type.DeclaringMethod"/>.
        /// </summary>
        public string DeclaringMethod
        {
            get { return (string)(GetValue(DeclaringMethodProperty)); }
            private set { SetValue(DeclaringMethodPropertyKey, value); }
        }

        #endregion
        
        #region Module Property Members

        /// <summary>
        /// Defines the name for the <see cref="Module"/> dependency property.
        /// </summary>
        public const string PropertyName_Module = "Module";

        private static readonly DependencyPropertyKey ModulePropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_Module,
            typeof(string), typeof(TypeListItemVM), new PropertyMetadata(""));

        /// <summary>
        /// Identifies the <see cref="Module"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ModuleProperty = ModulePropertyKey.DependencyProperty;

        /// <summary>
        /// Value of the model property <seealso cref="Type.Module"/>.
        /// </summary>
        public string Module
        {
            get { return (string)(GetValue(ModuleProperty)); }
            private set { SetValue(ModulePropertyKey, value); }
        }

        #endregion
        
        #region Assembly Property Members

        /// <summary>
        /// Defines the name for the <see cref="Assembly"/> dependency property.
        /// </summary>
        public const string PropertyName_Assembly = "Assembly";

        private static readonly DependencyPropertyKey AssemblyPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_Assembly,
            typeof(string), typeof(TypeListItemVM), new PropertyMetadata(""));

        /// <summary>
        /// Identifies the <see cref="Assembly"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AssemblyProperty = AssemblyPropertyKey.DependencyProperty;

        /// <summary>
        /// Value of the model property <seealso cref="Type.Assembly"/>.
        /// </summary>
        public string Assembly
        {
            get { return (string)(GetValue(AssemblyProperty)); }
            private set { SetValue(AssemblyPropertyKey, value); }
        }

        #endregion
        
        #region FullName Property Members

        /// <summary>
        /// Defines the name for the <see cref="FullName"/> dependency property.
        /// </summary>
        public const string PropertyName_FullName = "FullName";

        private static readonly DependencyPropertyKey FullNamePropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_FullName,
            typeof(string), typeof(TypeListItemVM), new PropertyMetadata(""));

        /// <summary>
        /// Identifies the <see cref="FullName"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FullNameProperty = FullNamePropertyKey.DependencyProperty;

        /// <summary>
        /// Value of the model property <seealso cref="Type.FullName"/>.
        /// </summary>
        public string FullName
        {
            get { return (string)(GetValue(FullNameProperty)); }
            private set { SetValue(FullNamePropertyKey, value); }
        }

        #endregion
        
        #region Namespace Property Members

        /// <summary>
        /// Defines the name for the <see cref="Namespace"/> dependency property.
        /// </summary>
        public const string PropertyName_Namespace = "Namespace";

        private static readonly DependencyPropertyKey NamespacePropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_Namespace,
            typeof(string), typeof(TypeListItemVM), new PropertyMetadata(""));

        /// <summary>
        /// Identifies the <see cref="Namespace"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty NamespaceProperty = NamespacePropertyKey.DependencyProperty;

        /// <summary>
        /// Value of the model property <seealso cref="Type.Namespace"/>.
        /// </summary>
        public string Namespace
        {
            get { return (string)(GetValue(NamespaceProperty)); }
            private set { SetValue(NamespacePropertyKey, value); }
        }

        #endregion
        
        #region AssemblyQualifiedName Property Members

        /// <summary>
        /// Defines the name for the <see cref="AssemblyQualifiedName"/> dependency property.
        /// </summary>
        public const string PropertyName_AssemblyQualifiedName = "AssemblyQualifiedName";

        private static readonly DependencyPropertyKey AssemblyQualifiedNamePropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_AssemblyQualifiedName,
            typeof(string), typeof(TypeListItemVM), new PropertyMetadata(""));

        /// <summary>
        /// Identifies the <see cref="AssemblyQualifiedName"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AssemblyQualifiedNameProperty = AssemblyQualifiedNamePropertyKey.DependencyProperty;

        /// <summary>
        /// Value of the model property <seealso cref="Type.AssemblyQualifiedName"/>.
        /// </summary>
        public string AssemblyQualifiedName
        {
            get { return (string)(GetValue(AssemblyQualifiedNameProperty)); }
            private set { SetValue(AssemblyQualifiedNamePropertyKey, value); }
        }

        #endregion
        
        #region BaseType Property Members

        /// <summary>
        /// Defines the name for the <see cref="BaseType"/> dependency property.
        /// </summary>
        public const string PropertyName_BaseType = "BaseType";

        private static readonly DependencyPropertyKey BaseTypePropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_BaseType,
            typeof(string), typeof(TypeListItemVM), new PropertyMetadata(""));

        /// <summary>
        /// Identifies the <see cref="BaseType"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BaseTypeProperty = BaseTypePropertyKey.DependencyProperty;

        /// <summary>
        /// Value of the model property <seealso cref="Type.BaseType"/>.
        /// </summary>
        public string BaseType
        {
            get { return (string)(GetValue(BaseTypeProperty)); }
            private set { SetValue(BaseTypePropertyKey, value); }
        }

        #endregion
        
        #region IsNested Property Members

        /// <summary>
        /// Defines the name for the <see cref="IsNested"/> dependency property.
        /// </summary>
        public const string PropertyName_IsNested = "IsNested";

        private static readonly DependencyPropertyKey IsNestedPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_IsNested,
            typeof(bool), typeof(TypeListItemVM), new PropertyMetadata(false));

        /// <summary>
        /// Identifies the <see cref="IsNested"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsNestedProperty = IsNestedPropertyKey.DependencyProperty;

        /// <summary>
        /// Value of the model property <seealso cref="Type.IsNested"/>.
        /// </summary>
        public bool IsNested
        {
            get { return (bool)(GetValue(IsNestedProperty)); }
            private set { SetValue(IsNestedPropertyKey, value); }
        }

        #endregion
       
        #region IsVisible Property Members

        /// <summary>
        /// Defines the name for the <see cref="IsVisible"/> dependency property.
        /// </summary>
        public const string PropertyName_IsVisible = "IsVisible";

        private static readonly DependencyPropertyKey IsVisiblePropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_IsVisible,
            typeof(bool), typeof(TypeListItemVM), new PropertyMetadatafalse(false));

        /// <summary>
        /// Identifies the <see cref="IsVisible"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsVisibleProperty = IsVisiblePropertyKey.DependencyProperty;

        /// <summary>
        /// Value of the model property <seealso cref="Type.IsVisible"/>.
        /// </summary>
        public bool IsVisible
        {
            get { return (bool)(GetValue(IsVisibleProperty)); }
            private set { SetValue(IsVisiblePropertyKey, value); }
        }

        #endregion
        
        #region Access Property Members

        /// <summary>
        /// Defines the name for the <see cref="Access"/> dependency property.
        /// </summary>
        public const string PropertyName_Access = "Access";

        private static readonly DependencyPropertyKey AccessPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_Access,
            typeof(string), typeof(TypeListItemVM), new PropertyMetadata(""));

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
        
        #region StructLayout Property Members

        /// <summary>
        /// Defines the name for the <see cref="StructLayout"/> dependency property.
        /// </summary>
        public const string PropertyName_StructLayout = "StructLayout";

        private static readonly DependencyPropertyKey StructLayoutPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_StructLayout,
            typeof(LayoutKind), typeof(TypeListItemVM), new PropertyMetadata(LayoutKind.Auto));

        /// <summary>
        /// Identifies the <see cref="StructLayout"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty StructLayoutProperty = StructLayoutPropertyKey.DependencyProperty;

        /// <summary>
        /// Value of the model property <seealso cref="Type.StructLayout"/>.
        /// </summary>
        public LayoutKind StructLayout
        {
            get { return (LayoutKind)(GetValue(StructLayoutProperty)); }
            private set { SetValue(StructLayoutPropertyKey, value); }
        }

        #endregion
        
        #region Kind Property Members

        /// <summary>
        /// Defines the name for the <see cref="Kind"/> dependency property.
        /// </summary>
        public const string PropertyName_Kind = "Kind";

        private static readonly DependencyPropertyKey KindPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_Kind,
            typeof(string), typeof(TypeListItemVM), new PropertyMetadata(""));

        /// <summary>
        /// Identifies the <see cref="Kind"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty KindProperty = KindPropertyKey.DependencyProperty;

        /// <summary>
        /// Whether type is a struct, class or interface
        /// </summary>
        public string Kind
        {
            get { return (string)(GetValue(KindProperty)); }
            private set { SetValue(KindPropertyKey, value); }
        }

        #endregion
        
        #region Modifier Property Members

        /// <summary>
        /// Defines the name for the <see cref="Modifier"/> dependency property.
        /// </summary>
        public const string PropertyName_Modifier = "Modifier";

        private static readonly DependencyPropertyKey ModifierPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_Modifier,
            typeof(string), typeof(TypeListItemVM), new PropertyMetadata(""));

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
        
        #region IsSpecialName Property Members

        /// <summary>
        /// Defines the name for the <see cref="IsSpecialName"/> dependency property.
        /// </summary>
        public const string PropertyName_IsSpecialName = "IsSpecialName";

        private static readonly DependencyPropertyKey IsSpecialNamePropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_IsSpecialName,
            typeof(bool), typeof(TypeListItemVM), new PropertyMetadata());

        /// <summary>
        /// Identifies the <see cref="IsSpecialName"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsSpecialNameProperty = IsSpecialNamePropertyKey.DependencyProperty;

        /// <summary>
        /// Value of the model property <seealso cref="Type.IsSpecialName"/>.
        /// </summary>
        public bool IsSpecialName
        {
            get { return (bool)(GetValue(IsSpecialNameProperty)); }
            private set { SetValue(IsSpecialNamePropertyKey, value); }
        }

        #endregion
        
        #region IsImport Property Members

        /// <summary>
        /// Defines the name for the <see cref="IsImport"/> dependency property.
        /// </summary>
        public const string PropertyName_IsImport = "IsImport";

        private static readonly DependencyPropertyKey IsImportPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_IsImport,
            typeof(bool), typeof(TypeListItemVM), new PropertyMetadata(false));

        /// <summary>
        /// Identifies the <see cref="IsImport"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsImportProperty = IsImportPropertyKey.DependencyProperty;

        /// <summary>
        /// Value of the model property <seealso cref="Type.IsImport"/>.
        /// </summary>
        public bool IsImport
        {
            get { return (bool)(GetValue(IsImportProperty)); }
            private set { SetValue(IsImportPropertyKey, value); }
        }

        #endregion
        
        #region IsSerializable Property Members

        /// <summary>
        /// Defines the name for the <see cref="IsSerializable"/> dependency property.
        /// </summary>
        public const string PropertyName_IsSerializable = "IsSerializable";

        private static readonly DependencyPropertyKey IsSerializablePropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_IsSerializable,
            typeof(bool), typeof(TypeListItemVM), new PropertyMetadata(false));

        /// <summary>
        /// Identifies the <see cref="IsSerializable"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsSerializableProperty = IsSerializablePropertyKey.DependencyProperty;

        /// <summary>
        /// Value of the model property <seealso cref="Type.IsSerializable"/>.
        /// </summary>
        public bool IsSerializable
        {
            get { return (bool)(GetValue(IsSerializableProperty)); }
            private set { SetValue(IsSerializablePropertyKey, value); }
        }

        #endregion
        
        #region ClassSpec Property Members

        /// <summary>
        /// Defines the name for the <see cref="ClassSpec"/> dependency property.
        /// </summary>
        public const string PropertyName_ClassSpec = "ClassSpec";

        private static readonly DependencyPropertyKey ClassSpecPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_ClassSpec,
            typeof(string), typeof(TypeListItemVM), new PropertyMetadata(""));

        /// <summary>
        /// Identifies the <see cref="ClassSpec"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ClassSpecProperty = ClassSpecPropertyKey.DependencyProperty;

        /// <summary>
        /// Value of the model property <seealso cref="Type.ClassSpec"/>.
        /// </summary>
        public string ClassSpec
        {
            get { return (string)(GetValue(ClassSpecProperty)); }
            private set { SetValue(ClassSpecPropertyKey, value); }
        }

        #endregion
        
        #region ElementRef Property Members

        /// <summary>
        /// Defines the name for the <see cref="ElementRef"/> dependency property.
        /// </summary>
        public const string PropertyName_ElementRef = "ElementRef";

        private static readonly DependencyPropertyKey ElementRefPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_ElementRef,
            typeof(string), typeof(TypeListItemVM), new PropertyMetadata(""));

        /// <summary>
        /// Identifies the <see cref="ElementRef"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ElementRefProperty = ElementRefPropertyKey.DependencyProperty;

        /// <summary>
        /// Value of the model property <seealso cref="Type.ElementRef"/>.
        /// </summary>
        public string ElementRef
        {
            get { return (string)(GetValue(ElementRefProperty)); }
            private set { SetValue(ElementRefPropertyKey, value); }
        }

        #endregion
        
        #region GenericType Property Members

        /// <summary>
        /// Defines the name for the <see cref="GenericType"/> dependency property.
        /// </summary>
        public const string PropertyName_GenericType = "GenericType";

        private static readonly DependencyPropertyKey GenericTypePropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_GenericType,
            typeof(string), typeof(TypeListItemVM), new PropertyMetadata(""));

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
        
        #region IsGenericParameter Property Members

        /// <summary>
        /// Defines the name for the <see cref="IsGenericParameter"/> dependency property.
        /// </summary>
        public const string PropertyName_IsGenericParameter = "IsGenericParameter";

        private static readonly DependencyPropertyKey IsGenericParameterPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_IsGenericParameter,
            typeof(bool), typeof(TypeListItemVM), new PropertyMetadata(false));

        /// <summary>
        /// Identifies the <see cref="IsGenericParameter"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsGenericParameterProperty = IsGenericParameterPropertyKey.DependencyProperty;

        /// <summary>
        /// Value of the model property <seealso cref="Type.IsGenericParameter"/>.
        /// </summary>
        public bool IsGenericParameter
        {
            get { return (bool)(GetValue(IsGenericParameterProperty)); }
            private set { SetValue(IsGenericParameterPropertyKey, value); }
        }

        #endregion
        
        #region GenericParameterPosition Property Members

        /// <summary>
        /// Defines the name for the <see cref="GenericParameterPosition"/> dependency property.
        /// </summary>
        public const string PropertyName_GenericParameterPosition = "GenericParameterPosition";

        private static readonly DependencyPropertyKey GenericParameterPositionPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_GenericParameterPosition,
            typeof(int), typeof(TypeListItemVM), new PropertyMetadata(-1));

        /// <summary>
        /// Identifies the <see cref="GenericParameterPosition"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty GenericParameterPositionProperty = GenericParameterPositionPropertyKey.DependencyProperty;

        /// <summary>
        /// Value of the model property <seealso cref="Type.GenericParameterPosition"/>.
        /// </summary>
        public int GenericParameterPosition
        {
            get { return (int)(GetValue(GenericParameterPositionProperty)); }
            private set { SetValue(GenericParameterPositionPropertyKey, value); }
        }

        #endregion
        
        #region IsPrimitive Property Members

        /// <summary>
        /// Defines the name for the <see cref="IsPrimitive"/> dependency property.
        /// </summary>
        public const string PropertyName_IsPrimitive = "IsPrimitive";

        private static readonly DependencyPropertyKey IsPrimitivePropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_IsPrimitive,
            typeof(bool), typeof(TypeListItemVM), new PropertyMetadata(false));

        /// <summary>
        /// Identifies the <see cref="IsPrimitive"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsPrimitiveProperty = IsPrimitivePropertyKey.DependencyProperty;

        /// <summary>
        /// Value of the model property <seealso cref="Type.IsPrimitive"/>.
        /// </summary>
        public bool IsPrimitive
        {
            get { return (bool)(GetValue(IsPrimitiveProperty)); }
            private set { SetValue(IsPrimitivePropertyKey, value); }
        }

        #endregion
        
        #region IsCOMObject Property Members

        /// <summary>
        /// Defines the name for the <see cref="IsCOMObject"/> dependency property.
        /// </summary>
        public const string PropertyName_IsCOMObject = "IsCOMObject";

        private static readonly DependencyPropertyKey IsCOMObjectPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_IsCOMObject,
            typeof(bool), typeof(TypeListItemVM), new PropertyMetadata(false));

        /// <summary>
        /// Identifies the <see cref="IsCOMObject"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsCOMObjectProperty = IsCOMObjectPropertyKey.DependencyProperty;

        /// <summary>
        /// Value of the model property <seealso cref="Type.IsCOMObject"/>.
        /// </summary>
        public bool IsCOMObject
        {
            get { return (bool)(GetValue(IsCOMObjectProperty)); }
            private set { SetValue(IsCOMObjectPropertyKey, value); }
        }

        #endregion
        
        #region IsContextful Property Members

        /// <summary>
        /// Defines the name for the <see cref="IsContextful"/> dependency property.
        /// </summary>
        public const string PropertyName_IsContextful = "IsContextful";

        private static readonly DependencyPropertyKey IsContextfulPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_IsContextful,
            typeof(bool), typeof(TypeListItemVM), new PropertyMetadata(false));

        /// <summary>
        /// Identifies the <see cref="IsContextful"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsContextfulProperty = IsContextfulPropertyKey.DependencyProperty;

        /// <summary>
        /// Value of the model property <seealso cref="Type.IsContextful"/>.
        /// </summary>
        public bool IsContextful
        {
            get { return (bool)(GetValue(IsContextfulProperty)); }
            private set { SetValue(IsContextfulPropertyKey, value); }
        }

        #endregion
        
        #region IsMarshalByRef Property Members

        /// <summary>
        /// Defines the name for the <see cref="IsMarshalByRef"/> dependency property.
        /// </summary>
        public const string PropertyName_IsMarshalByRef = "IsMarshalByRef";

        private static readonly DependencyPropertyKey IsMarshalByRefPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_IsMarshalByRef,
            typeof(bool), typeof(TypeListItemVM), new PropertyMetadata(false));

        /// <summary>
        /// Identifies the <see cref="IsMarshalByRef"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsMarshalByRefProperty = IsMarshalByRefPropertyKey.DependencyProperty;

        /// <summary>
        /// Value of the model property <seealso cref="Type.IsMarshalByRef"/>.
        /// </summary>
        public bool IsMarshalByRef
        {
            get { return (bool)(GetValue(IsMarshalByRefProperty)); }
            private set { SetValue(IsMarshalByRefPropertyKey, value); }
        }

        #endregion
        
        #region GenericTypeArguments Property Members

        /// <summary>
        /// Defines the name for the <see cref="GenericTypeArguments"/> dependency property.
        /// </summary>
        public const string PropertyName_GenericTypeArguments = "GenericTypeArguments";

        private static readonly DependencyPropertyKey GenericTypeArgumentsPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_GenericTypeArguments,
            typeof(string), typeof(TypeListItemVM), new PropertyMetadata(""));

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
        
        #region IsSecurityCritical Property Members

        /// <summary>
        /// Defines the name for the <see cref="IsSecurityCritical"/> dependency property.
        /// </summary>
        public const string PropertyName_IsSecurityCritical = "IsSecurityCritical";

        private static readonly DependencyPropertyKey IsSecurityCriticalPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_IsSecurityCritical,
            typeof(bool), typeof(TypeListItemVM), new PropertyMetadata(false));

        /// <summary>
        /// Identifies the <see cref="IsSecurityCritical"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsSecurityCriticalProperty = IsSecurityCriticalPropertyKey.DependencyProperty;

        /// <summary>
        /// Value of the model property <seealso cref="Type.IsSecurityCritical"/>.
        /// </summary>
        public bool IsSecurityCritical
        {
            get { return (bool)(GetValue(IsSecurityCriticalProperty)); }
            private set { SetValue(IsSecurityCriticalPropertyKey, value); }
        }

        #endregion
        
        #region IsSecuritySafeCritical Property Members

        /// <summary>
        /// Defines the name for the <see cref="IsSecuritySafeCritical"/> dependency property.
        /// </summary>
        public const string PropertyName_IsSecuritySafeCritical = "IsSecuritySafeCritical";

        private static readonly DependencyPropertyKey IsSecuritySafeCriticalPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_IsSecuritySafeCritical,
            typeof(bool), typeof(TypeListItemVM), new PropertyMetadata(false));

        /// <summary>
        /// Identifies the <see cref="IsSecuritySafeCritical"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsSecuritySafeCriticalProperty = IsSecuritySafeCriticalPropertyKey.DependencyProperty;

        /// <summary>
        /// Value of the model property <seealso cref="Type.IsSecuritySafeCritical"/>.
        /// </summary>
        public bool IsSecuritySafeCritical
        {
            get { return (bool)(GetValue(IsSecuritySafeCriticalProperty)); }
            private set { SetValue(IsSecuritySafeCriticalPropertyKey, value); }
        }

        #endregion
        
        #region IsSecurityTransparent Property Members

        /// <summary>
        /// Defines the name for the <see cref="IsSecurityTransparent"/> dependency property.
        /// </summary>
        public const string PropertyName_IsSecurityTransparent = "IsSecurityTransparent";

        private static readonly DependencyPropertyKey IsSecurityTransparentPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_IsSecurityTransparent,
            typeof(bool), typeof(TypeListItemVM), new PropertyMetadata(false));

        /// <summary>
        /// Identifies the <see cref="IsSecurityTransparent"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsSecurityTransparentProperty = IsSecurityTransparentPropertyKey.DependencyProperty;

        /// <summary>
        /// Value of the model property <seealso cref="Type.IsSecurityTransparent"/>.
        /// </summary>
        public bool IsSecurityTransparent
        {
            get { return (bool)(GetValue(IsSecurityTransparentProperty)); }
            private set { SetValue(IsSecurityTransparentPropertyKey, value); }
        }

        #endregion
        
        #region UnderlyingType Property Members

        /// <summary>
        /// Defines the name for the <see cref="UnderlyingType"/> dependency property.
        /// </summary>
        public const string PropertyName_UnderlyingType = "UnderlyingType";

        private static readonly DependencyPropertyKey UnderlyingTypePropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_UnderlyingType,
            typeof(string), typeof(TypeListItemVM), new PropertyMetadata(""));

        /// <summary>
        /// Identifies the <see cref="UnderlyingType"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty UnderlyingTypeProperty = UnderlyingTypePropertyKey.DependencyProperty;

        /// <summary>
        /// Value of the model property <seealso cref="Type.ElementType"/> or <seealso cref="Type.UnderlyingSystemType"/>.
        /// </summary>
        public string UnderlyingType
        {
            get { return (string)(GetValue(UnderlyingTypeProperty)); }
            private set { SetValue(UnderlyingTypePropertyKey, value); }
        }

        #endregion
        
        #region Name Property Members

        /// <summary>
        /// Defines the name for the <see cref="Name"/> dependency property.
        /// </summary>
        public const string PropertyName_Name = "Name";

        private static readonly DependencyPropertyKey NamePropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_Name,
            typeof(string), typeof(TypeListItemVM), new PropertyMetadata(false));

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
        
        public TypeListItemVM() { }

        public TypeListItemVM(Type model)
        {
            if (model == null)
                return;

            _model = model;
            Name = (model.IsGenericParameter) ? model.Name : TypeNameOnlyRegex.Match(model.Name).Value;
            Module = model.Module.Name;
            Assembly = model.Assembly.FullName;
            FullName = model.FullName;
            Namespace = model.Namespace;
            AssemblyQualifiedName = model.AssemblyQualifiedName;
            BaseType = (model.BaseType == null) ? "" : ToCSharpTypeName(model.BaseType.FullName);
            DeclaringType = (model.DeclaringType == null) ? "" : ToCSharpTypeName(model.DeclaringType.FullName);
            DeclaringMethod = (model.DeclaringMethod == null) ? "" : model.DeclaringMethod.ToString();
            IsNested = model.IsNested;
            IsVisible = model.IsVisible;
            Access = (model.IsPublic || model.IsNestedPublic) ? "public" : ((model.IsNestedFamANDAssem) ? "protected internal" : ((model.IsNestedAssembly) ? "internal" : ((model.IsNestedFamily) ? "protected" : ((model.IsNotPublic) ? "private" : ""))));
            StructLayout = (model.IsLayoutSequential) ? LayoutKind.Sequential : ((model.IsExplicitLayout) ? LayoutKind.Explicit : LayoutKind.Auto);
            Kind = (model.IsClass) ? "class" : ((model.IsEnum) ? "enum" : ((model.IsValueType) ? "struct" : ((model.IsInterface) ? "interface" : "")));
            Modifier = (model.IsAbstract) ? "abstract" : ((model.IsSealed) ? "sealed" : "");
            IsSpecialName = model.IsSpecialName;
            IsImport = model.IsImport;
            IsSerializable = model.IsSerializable;
            ClassSpec = (model.IsAnsiClass) ? "ANSI" : ((model.IsUnicodeClass) ? "Unicode" : ((model.IsAutoClass) ? "Auto" : ""));
            Match m = TypeNameParseRegex.Match(model.FullName);
            ElementRef = (m.Success && m.Groups["indexer"].Success) ? m.Groups["indexer"].Value : "";
            GenericType = (model.IsGenericTypeDefinition) ? "Definition" : ((model.IsConstructedGenericType) ? "Constructed" : ((model.IsGenericType) ? "Generic" : ""));
            if (model.IsGenericType)
                GenericTypeArguments = "<" + String.Join(",", model.GetGenericArguments().Select(t => ToCSharpTypeName(t.FullName)).ToArray()) + ">";
            IsMarshalByRef = model.IsMarshalByRef;
            IsContextful = model.IsContextful;
            IsCOMObject = model.IsCOMObject;
            IsPrimitive = model.IsPrimitive;
            GenericParameterPosition = model.GenericParameterPosition;
            IsGenericParameter = model.IsGenericParameter;
            UnderlyingType = (model.IsArray || model.IsPointer || model.IsByRef) ? model.GetElementType().FullName : ((model.UnderlyingSystemType == null) ? "" : model.UnderlyingSystemType.FullName);
            IsSecurityTransparent = model.IsSecurityTransparent;
            IsSecuritySafeCritical = model.IsSecuritySafeCritical;
            IsSecurityCritical = model.IsSecurityCritical;
        }

        public TypeListItemVM(TypeListItemVM vm)
        {
            if (vm == null)
                return;

            _model = vm._model;
            Name = vm.Name;
            Module = vm.Module;
            Assembly = vm.Assembly;
            FullName = vm.FullName;
            Namespace = vm.Namespace;
            AssemblyQualifiedName = vm.AssemblyQualifiedName;
            BaseType = vm.BaseType;
            DeclaringType = vm.DeclaringType;
            DeclaringMethod = vm.DeclaringMethod;
            IsNested = vm.IsNested;
            IsVisible = vm.IsVisible;
            Access = vm.Access;
            StructLayout = vm.StructLayout;
            Kind = vm.Kind;
            Modifier = vm.Modifier;
            IsSpecialName = vm.IsSpecialName;
            IsImport = vm.IsImport;
            IsSerializable = vm.IsSerializable;
            ClassSpec = vm.ClassSpec;
            ElementRef = vm.ElementRef;
            GenericType = vm.GenericType;
            GenericTypeArguments = vm.GenericTypeArguments;
            IsMarshalByRef = vm.IsMarshalByRef;
            IsContextful = vm.IsContextful;
            IsCOMObject = vm.IsCOMObject;
            IsPrimitive = vm.IsPrimitive;
            GenericParameterPosition = vm.GenericParameterPosition;
            IsGenericParameter = vm.IsGenericParameter;
            UnderlyingType = vm.UnderlyingType;
            IsSecurityTransparent = vm.IsSecurityTransparent;
            IsSecuritySafeCritical = vm.IsSecuritySafeCritical;
            IsSecurityCritical = vm.IsSecurityCritical;
        }

        public Type GetModel() { return _model; }
    }
}
