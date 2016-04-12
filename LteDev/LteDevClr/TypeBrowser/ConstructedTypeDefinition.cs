using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Text;

namespace LteDevClr.TypeBrowser
{
    public class ConstructedTypeDefinition : TypeInfoDataItem
    {
        private object _syncRoot = new object();
        private string _baseName = null;
        private string _fullName = null;
        private TypeInfoDataItem _elementType = null;
        private Collection<TypeInfoDataItem> _genericArguments = null;

        public TypeInfoDataItem ElementType
        {
            get
            {
                if (this._elementType == null && this.RepresentedMember.HasElementType)
                        this._elementType = TypeInfoDataItem.Get(this.RepresentedMember.GetElementType());

                return this._elementType;
            }
        }

        public Collection<TypeInfoDataItem> GenericArguments
        {
            get
            {
                lock (this._syncRoot)
                {
                    if (this._genericArguments != null)
                        return this._genericArguments;
                    this._genericArguments = new Collection<TypeInfoDataItem>();
                }

                if (!this.RepresentedMember.HasElementType)
                {
                    foreach (Type type in this.RepresentedMember.GetGenericArguments())
                        this._genericArguments.Add(TypeInfoDataItem.Get(type));
                }
                    
                return this._genericArguments;
            }
        }

        public override string BaseName
        {
            get
            {
                if (this._baseName == null)
                {
                    if (this.RepresentedMember.HasElementType)
                        this._baseName = this.ElementType.BaseName;
                    else
                        this._baseName = TypeInfoDataItem.GenericParamCountRegex.Replace(this.RepresentedMember.Name, "");
                }
                
                return this.BaseName;
            }
        }

        public override string FullName
        {
            get
            {
                if (this._fullName != null)
                    return this._fullName;

                if (this.RepresentedMember.HasElementType)
                    this._fullName = this.ElementType.FullName + this.RepresentedMember.Name.Substring(this.ElementType.RepresentedMember.Name.Length);
                else
                {
                    StringBuilder sb = new StringBuilder(this.GenericArguments[0].FullName);
                    for (int i = 1; i < this.GenericArguments.Count; i++)
                        sb.AppendFormat(",{0}", this.GenericArguments[i].FullName);
                    if (this.RepresentedMember.IsNested)
                        this._fullName = String.Format("{0}+{1}[{2}]", TypeInfoDataItem.Get(this.RepresentedMember.DeclaringType).FullName, this.BaseName, sb.ToString());
                    else if (this.RepresentedMember.Namespace == null)
                        this._fullName = String.Format("{0}[{1}]", this.BaseName, sb.ToString());
                    else
                        this._fullName = String.Format("{0}.{1}[{2}]", this.RepresentedMember.Namespace, this.BaseName, sb.ToString());
                }

                return this._fullName;
            }
        }

        protected override bool CanHaveNestedTypes { get { return true; } }

        public ConstructedTypeDefinition(Type representedMember)
            : base(representedMember)
        {
            if (representedMember.HasElementType)
                return;

            if (!representedMember.IsGenericType || representedMember.IsGenericTypeDefinition)
                throw new ArgumentException("Type must have an Element Type or be an instantiated Generic Type.", "representedMember");
        }
    }
}
