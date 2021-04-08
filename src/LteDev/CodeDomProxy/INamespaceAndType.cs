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
    public interface INamespaceAndType
    {
        CodeNamespace Namespace { get; }
        
        CodeTypeDeclaration Type { get; }

        INamespaceAndType Parent { get; }

        CodeTypeReference ToTypeReference();
    }
#pragma warning restore 1591 // Missing XML comment for publicly visible type or member
}
