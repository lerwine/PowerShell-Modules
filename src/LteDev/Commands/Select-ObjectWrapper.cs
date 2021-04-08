using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Reflection;
using Microsoft.PowerShell.Commands;

namespace LteDev.Commands
{
#pragma warning disable 1591 // Missing XML comment for publicly visible type or member
    [Cmdlet(VerbsCommon.Get, "TypeNameInfo", DefaultParameterSetName = ParameterSetName_CurrentDomain)]
    [OutputType(typeof(PSTypeName[]))]
    public class Get_TypeNameInfo : Cmdlet
    {
        public const string ParameterSetName_CurrentDomain = "CurrentDomain";
        public const string ParameterSetName_FromType = "FromType";
        public const string ParameterSetName_FromAssembly = "FromAssembly";
        public const string ParameterSetName_ = "";

        #region Properties

        [Parameter(Mandatory = true, ValueFromPipeline = true, ParameterSetName = ParameterSetName_FromType)]
        public Type[] Type { get; set; }

        [Parameter(Mandatory = true, ValueFromPipeline = true, ParameterSetName = ParameterSetName_FromAssembly)]
        public Assembly[] Assembly { get; set; }

        [Parameter(ParameterSetName = ParameterSetName_CurrentDomain)]
        [Parameter(ParameterSetName = ParameterSetName_FromAssembly)]
        [ValidateNotNull()]
        [AllowEmptyString()]
        public string Namespace { get; set; }

        [Parameter(ParameterSetName = ParameterSetName_CurrentDomain)]
        public SwitchParameter CurrentDomain { get; set; }

        [Parameter(ParameterSetName = ParameterSetName_CurrentDomain)]
        [Parameter(ParameterSetName = ParameterSetName_FromAssembly)]
        public SwitchParameter GetNamespaces { get; set; }

        public SwitchParameter Recursive { get; set; }

        #endregion

        #region Overrides

        protected override void ProcessRecord()
        {
        }

        public enum TypeSelectProperties
        {
            GenericTypeParameters,
            IsVisible,
            IsNotPublic,
            IsPublic,
            IsNestedPublic,
            IsNestedAssembly,
            IsNestedFamily,
            Attributes,
            IsNestedFamANDAssem,
            IsNestedFamORAssem,
            IsAutoLayout,
            IsNestedPrivate,
            IsNested,
            Namespace,
            BaseType,
            AssemblyQualifiedName,
            IsLayoutSequential,
            FullName,
            Assembly,
            DeclaringMethod,
            DeclaringType,
            IsExplicitLayout,
            IsValueType,
            IsInterface,
            IsSecuritySafeCritical,
            IsSecurityCritical,
            IsMarshalByRef,
            IsContextful,
            HasElementType,
            IsCOMObject,
            IsPrimitive,
            IsPointer,
            IsByRef,
            ContainsGenericParameters,
            GenericParameterPosition,
            IsGenericParameter,
            IsConstructedGenericType,
            IsGenericTypeDefinition,
            IsGenericType,
            IsArray,
            IsAutoClass,
            IsUnicodeClass,
            IsAnsiClass,
            IsSerializable,
            IsImport,
            IsSpecialName,
            IsEnum,
            IsSealed,
            IsAbstract,
            MemberType,
            IsClass,
            IsSecurityTransparent,
            UnderlyingSystemType
        }
        public class TypeSelectionInfo
        {
            private TypeInfo _type;

            internal TypeSelectionInfo(Type type)
            {
                _type = (type is TypeInfo) ? (TypeInfo)type : type.GetTypeInfo();
            }
        }

        private static IEnumerable<PSTypeName> AsPSTypeName(IEnumerable<Type> types)
        {
            return types.Select(t => new PSTypeName((t is TypeInfo) ? (TypeInfo)t : t.GetTypeInfo()));
        }

        private IEnumerable<PSTypeName> GetNestedTypes(Type type)
        {
            IEnumerable<PSTypeName> nestedTypes = AsPSTypeName(type.GetNestedTypes());
            if (!Recursive.IsPresent)
                return nestedTypes;
            return nestedTypes.SelectMany(t => (new PSTypeName[] { t }).Concat(GetNestedTypes(t.Type)));
        }

        private IEnumerable<PSTypeName> FromAssembly(Assembly[] assembly)
        {
            IEnumerable<PSTypeName> types = AsPSTypeName(Assembly.Where(a => a != null).SelectMany(a => a.ExportedTypes));
            if (Namespace != null)
            {
                if (Namespace.Length > 0)
                    types = (Recursive.IsPresent) ? types.Where(t => t.Type.Namespace != null && t.Type.Namespace.StartsWith(Namespace)) :
                        types.Where(t => t.Type.Namespace != null && t.Type.Namespace == Namespace);
                else
                    types = (Recursive.IsPresent) ? types : types.Where(t => String.IsNullOrEmpty(t.Type.Namespace));
            }
            if (Recursive.IsPresent)
                return types.SelectMany(t => (new PSTypeName[] { t }).Concat(GetNestedTypes(t.Type)));
            return types;
        }

        #endregion
    }
#pragma warning restore 1591 // Missing XML comment for publicly visible type or member
}
