using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Provider;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace LteDev.ModuleBuilder
{
    /// <summary>
    /// Contextual information for a CLR property.
    /// </summary>
    public class CLRPropertyInfo : InformationAggregator, IEquatable<CLRPropertyInfo>, IEquatable<PropertyInfo>
    {
        private XElement _xmlDoc = null;

        /// <summary>
        /// Contextual information for the reflected CLR type.
        /// </summary>
        public CLRTypeInfo TypeInfo { get; private set; }

        /// <summary>
        /// CLR property information.
        /// </summary>
        public PropertyInfo PropertyInfo { get; private set; }

        /// <summary>
        /// Member element for associated property.
        /// </summary>
        public XElement XmlDoc
        {
            get
            {
                string key = AssemblyInfo.XmlDocPrefix_Property + PropertyInfo.ReflectedType.FullName + "." + PropertyInfo.Name;
                XElement e = _xmlDoc;
                if (e == null)
                {
                    if ((e = TypeInfo.AssemblyInfo.DocMembers.Elements(AssemblyInfo.ElementName_member).Attributes(AssemblyInfo.AttributeName_name)
                        .Where(a => a.Value == key).Select(a => a.Parent).FirstOrDefault()) == null)
                    {
                        if (PropertyInfo.ReflectedType.AssemblyQualifiedName == PropertyInfo.DeclaringType.AssemblyQualifiedName)
                        {
                            e = new XElement(AssemblyInfo.ElementName_member, new XAttribute(AssemblyInfo.AttributeName_name, key));
                            TypeInfo.AssemblyInfo.DocMembers.Add(e);
                        }
                        else
                            e = TypeInfo.AssemblyInfo.ContextFactory.GetAssemblyTypeContext(PropertyInfo.DeclaringType).GetPropertyContext(PropertyInfo).XmlDoc;
                    }
                    _xmlDoc = e;
                }
                return e;
            }
        }

        /// <summary>
        /// Initialize new <see cref="CLRPropertyInfo"/> object.
        /// </summary>
        /// <param name="propertyInfo">Property information.</param>
        /// <param name="typeContext">Contextual information for Reflected CLR type.</param>
        public CLRPropertyInfo(PropertyInfo propertyInfo, CLRTypeInfo typeContext)
        {
            if (propertyInfo == null)
                throw new ArgumentNullException("propertyInfo");

            if (typeContext == null)
                throw new ArgumentNullException("typeContext");

            if (!typeContext.Equals(propertyInfo.DeclaringType))
                throw new ArgumentException("Property belongs to another type.");

            PropertyInfo = propertyInfo;
            TypeInfo = typeContext;
        }

        /// <summary>
        /// Determines if another <see cref="CLRPropertyInfo"/> is the same instance as the current.
        /// </summary>
        /// <param name="other"><see cref="CLRPropertyInfo"/> object to compare.</param>
        /// <returns>true if <paramref name="other"/> is the same instance as the current <see cref="CLRPropertyInfo"/>; otherwise, false.</returns>
        public bool Equals(CLRPropertyInfo other)
        {
            return other != null && ReferenceEquals(this, other);
        }

        /// <summary>
        /// Determines if the current <see cref="CLRPropertyInfo"/> represents a <seealso cref="System.Reflection.PropertyInfo"/> object.
        /// </summary>
        /// <param name="other"><seealso cref="System.Reflection.PropertyInfo"/> object to compare.</param>
        /// <returns>True if the current <see cref="CLRPropertyInfo"/> represents the specified <seealso cref="System.Reflection.PropertyInfo"/> object; otherwise, false.</returns>
        public bool Equals(PropertyInfo other)
        {
            return other != null && (ReferenceEquals(PropertyInfo, other) || (TypeInfo.Equals(other.ReflectedType) && PropertyInfo.Name == other.Name));
        }

        public override bool Equals(object obj)
        {
            if (obj != null && obj is PropertyInfo)
                return Equals(obj as PropertyInfo);

            return Equals(obj as CLRPropertyInfo);
        }

        public override int GetHashCode() { return PropertyInfo.Name.GetHashCode(); }

        public override string ToString() { return PropertyInfo.Name; }
    }
}