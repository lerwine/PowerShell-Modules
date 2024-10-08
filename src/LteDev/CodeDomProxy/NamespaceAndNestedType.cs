using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LteDev.CodeDomProxy
{
#pragma warning disable 1591 // Missing XML comment for publicly visible type or member
    public class NamespaceAndNestedType : INamespaceAndType
    {
        public INamespaceAndType Parent { get; private set; }
        
        public CodeTypeDeclaration Type { get; private set; }

        CodeNamespace INamespaceAndType.Namespace { get { return Parent.Namespace; } }

        public NamespaceAndNestedType(INamespaceAndType parent, string name, params string[] genericTypeNames)
        {
            ArgumentNullException.ThrowIfNull(parent);
            ArgumentNullException.ThrowIfNull(name);
            if (!ValidateCsNameAttribute.NameRegex.IsMatch(name))
                throw new ArgumentException("Invalid type name", nameof(name));
            
            Parent = parent;
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
            parent.Type.Members.Add(Type);
        }

        public NamespaceAndNestedType(INamespaceAndType parent, CodeTypeDeclaration type)
        {
            ArgumentNullException.ThrowIfNull(parent);
            ArgumentNullException.ThrowIfNull(type);
            Parent = parent;
            Type = type;
            if (!parent.Type.Members.OfType<CodeTypeDeclaration>().Any(t => ReferenceEquals(t, type)))
                parent.Type.Members.Add(type);
        }

        public CodeTypeReference ToTypeReference()
        {
            CodeTypeReference[] parameters = Type.TypeParameters.OfType<CodeTypeParameter>().Select(p => p.Name).Where(n => !string.IsNullOrEmpty(n)).Select(n => new CodeTypeReference(n)).ToArray();
            if (parameters.Length == 0)
                return new CodeTypeReference(Parent.ToString() + "." + this.Type.Name);

            return new CodeTypeReference(Parent.ToString() + "." + this.Type.Name, parameters);
        }

        public override string ToString()
        {
            string[] parameters = Type.TypeParameters.OfType<CodeTypeParameter>().Select(p => p.Name).Where(n => !string.IsNullOrEmpty(n)).ToArray();
            if (parameters.Length == 0)
                return Parent.ToString() + "." + this.Type.Name;

            return Parent.ToString() + "." + this.Type.Name + "[" + string.Join(",", parameters) + "]";
        }
    }
#pragma warning restore 1591 // Missing XML comment for publicly visible type or member
}
