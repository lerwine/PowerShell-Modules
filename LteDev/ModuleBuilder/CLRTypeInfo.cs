using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Provider;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace LteDev.ModuleBuilder
{
    public class CLRTypeInfo : InformationAggregator, IEquatable<CLRTypeInfo>, IEquatable<Type>
    {
        private List<CLRPropertyInfo> _propertyContexts = new List<CLRPropertyInfo>();
        private XElement _xmlDoc = null;

        public AssemblyInfo AssemblyInfo { get; private set; }

        public PSTypeName Type { get; private set; }

        public virtual string Name { get { return Type.Name; } }

        public XElement XmlDoc
        {
            get
            {
                string key = AssemblyInfo.XmlDocPrefix_Type + Type.Type.FullName;
                XElement e = _xmlDoc;
                if (e == null)
                {
                    if ((e = AssemblyInfo.DocMembers.Elements(AssemblyInfo.ElementName_member).Attributes(AssemblyInfo.AttributeName_name)
                        .Where(a => a.Value == key).Select(a => a.Parent).FirstOrDefault()) == null)
                    {
                        e = new XElement(AssemblyInfo.ElementName_member, new XAttribute(AssemblyInfo.AttributeName_name, key));
                        AssemblyInfo.DocMembers.Add(e);
                    }
                    _xmlDoc = e;
                }
                return e;
            }
        }
        
        public virtual XElement GetPSMamlDevType()
        {
            return new XElement(dev.type,
                new XElement(maml.name, Name),
                new XElement(maml.uri)
            );
        }
        
        public CLRTypeInfo(PSTypeName typeName, AssemblyInfo assemblyInfo)
        {
            if (typeName == null)
                throw new ArgumentNullException("typeName");

            if (assemblyInfo == null)
                throw new ArgumentNullException("assemblyContext");

            if (!assemblyInfo.Equals(typeName.Type.Assembly))
                throw new ArgumentException("Type belongs to another assembly.");

            Type = typeName;
            AssemblyInfo = assemblyInfo;
        }

        public CLRTypeInfo(Type type, AssemblyInfo assemblyInfo)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            if (assemblyInfo == null)
                throw new ArgumentNullException("assemblyContext");

            if (!assemblyInfo.Equals(type.Assembly))
                throw new ArgumentException("Type belongs to another assembly.");

            Type = new PSTypeName(type);
            AssemblyInfo = assemblyInfo;
        }

        public CLRPropertyInfo GetPropertyContext(PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
                throw new ArgumentNullException("propertyInfo");

            if (!Equals(propertyInfo.ReflectedType))
                throw new ArgumentException("Property belongs to another type.");

            CLRPropertyInfo context;
            Monitor.Enter(_propertyContexts);
            try
            {
                context = _propertyContexts.FirstOrDefault(c => c.Equals(propertyInfo));
                if (context == null)
                {
                    context = new CLRPropertyInfo(propertyInfo, this);
                    _propertyContexts.Add(context);
                }
            }
            finally { Monitor.Exit(_propertyContexts); }
            return context;
        }

        public bool Equals(CLRTypeInfo other)
        {
            return other != null && ReferenceEquals(this, other);
        }

        public bool Equals(Type other)
        {
            return other != null && (ReferenceEquals(Type, other) || (AssemblyInfo.Equals(other.Assembly) && Name == other.Name && Type.Type.AssemblyQualifiedName == other.AssemblyQualifiedName));
        }

        public override bool Equals(object obj)
        {
            if (obj != null && obj is Type)
                return Equals(obj as Type);

            return Equals(obj as CLRTypeInfo);
        }

        public override int GetHashCode() { return Type.GetHashCode(); }

        public override string ToString() { return Type.ToString(); }
    }
}