using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LteDev.CodeDomProxy
{
#pragma warning disable 1591 // Missing XML comment for publicly visible type or member
    public class NamespaceAndType : INamespaceAndType
    {   
        public CodeNamespace Namespace { get; private set; }
        
        public CodeTypeDeclaration Type { get; private set; }

        INamespaceAndType? INamespaceAndType.Parent { get { return null; } }

        public NamespaceAndType(CodeNamespace ns, string name, params string[] genericTypeNames)
        {
            ArgumentNullException.ThrowIfNull(ns);
            ArgumentNullException.ThrowIfNull(name);
            if (!ValidateCsNameAttribute.NameRegex.IsMatch(name))
                throw new ArgumentException("Invalid type name", nameof(name));
            
            Namespace = ns;
            Type = new CodeTypeDeclaration(name);
            if (genericTypeNames != null && genericTypeNames.Length > 0)
            {
                foreach (string n in genericTypeNames)
                {
                    if (n == null)
                        throw new ArgumentNullException(nameof(genericTypeNames));
                    if (!ValidateCsNameAttribute.NameRegex.IsMatch(n))
                        throw new ArgumentException("Invalid type name", nameof(genericTypeNames));
                    Type.TypeParameters.Add(new CodeTypeParameter(n));
                }
            }
            ns.Types.Add(Type);
        }

        public NamespaceAndType(CodeNamespace ns, CodeTypeDeclaration type)
        {
            ArgumentNullException.ThrowIfNull(ns);
            ArgumentNullException.ThrowIfNull(type);
            Namespace = ns;
            Type = type;
            if (!ns.Types.OfType<CodeTypeDeclaration>().Any(t => ReferenceEquals(t, type)))
                ns.Types.Add(type);
        }

        public CodeTypeReference ToTypeReference()
        {
            CodeTypeReference[] parameters = Type.TypeParameters.OfType<CodeTypeParameter>().Select(p => p.Name).Where(n => !string.IsNullOrEmpty(n)).Select(n => new CodeTypeReference(n)).ToArray();
            if (parameters.Length == 0)
                return new CodeTypeReference((string.IsNullOrEmpty(this.Namespace.Name)) ? this.Type.Name : this.Namespace.Name + "." + this.Type.Name);

            return new CodeTypeReference((string.IsNullOrEmpty(this.Namespace.Name)) ? this.Type.Name : this.Namespace.Name + "." + this.Type.Name, parameters);
        }

        public override string ToString()
        {
            string[] parameters = Type.TypeParameters.OfType<CodeTypeParameter>().Select(p => p.Name).Where(n => !string.IsNullOrEmpty(n)).ToArray();
            if (parameters.Length == 0)
                return (string.IsNullOrEmpty(this.Namespace.Name)) ? this.Type.Name : this.Namespace.Name + "." + this.Type.Name;

            return ((string.IsNullOrEmpty(this.Namespace.Name)) ? this.Type.Name : this.Namespace.Name + "." + this.Type.Name) + "[" + string.Join(",", parameters) + "]";
        }
    }
#pragma warning restore 1591 // Missing XML comment for publicly visible type or member
}
