using System;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Task;

namespace Erwine.Leonard.T.GDIPlus
{
	[Serializable()]
    public class ComponentPropertyDictionary<TKey> : PropertyDictionary
		where TKey : IComparable
    {
		private CrawledComponent<TKey> _owner;
		private PropertyDescriptorCollection _properties;
        public ComponentPropertyDictionary() : this(null) { }
		public ComponentPropertyDictionary(CrawledComponent<TKey> owner) : base(false)
		{
			_owner = owner;
			Initialize();
		}
		
		public override bool ContainsKey(TKey name)
		{
			if (base.ContainsKey(name))
				return true;
			
			CrawledComponent<TKey> owner = _owner;
			PropertyDescriptorCollection properties = _properties;
			return owner != null && properties == null && properties.Find(name, true) != null;
		}
		
        internal void Initialize(CrawledComponent<TKey> owner)
        {
            if (owner == null)
                throw new ArgumentNullException("owner");
			if (!ReferenceEquals(owner.ItemCollection, this))
				throw new InvalidOperationException("Component container does not own this collection.");
			
			if (_owner != null && !ReferenceEquals(_owner, owner))
				throw new InvalidOperationException("Dictionary owner cannot be changed once it is initialized.");
			_owner = owner;
			Initialize();
		}
		
        private void Initialize()
        {
			_properties = (_owner == null) ? null : TypeDescriptor.GetProperties(_owner);
		}
		
		protected bool TryGetFromPropertyDescriptor(string name, out object value)
		{
			CrawledComponent<TKey> owner = _owner;
			PropertyDescriptorCollection properties = _properties;
			PropertyDescriptor propertyDescriptor;
			if (owner == null || properties == null || ((propertyDescriptor = properties.Find(name, false)) == null && (propertyDescriptor = properties.Find(name, true)) == null))
			{
				value = null;
				return false;
			}
			
			value = propertyDescriptor.GetValue(owner);
			return true;
		}
		
		protected override bool BaseTryGet(string name, out object value)
		{
			if (name == null)
			{
				value = null;
				return false;
			}
			
			if (base.BaseTryGet(name, out value))
				return true;
			return TryGetFromPropertyDescriptor(name, out value);
		}
		
		protected virtual bool BaseTryGet(string name, int subIndex, out object value)
		{
			if (name == null || subIndex < 0)
			{
				value = null;
				return false;
			}
			
			if (base.BaseTryGet(name, subIndex, out value))
				return true;
			
			if (subIndex == 0)
				return TryGetFromPropertyDescriptor(name, out value);
			value = null;
			return false;
		}
		
		protected virtual object BaseGet(string name)
		{
			object value;
			if (!BaseTryGet(name, out value))
				TryGetFromPropertyDescriptor(name, out value);
			return value;
		}
		
		protected virtual object BaseGet(string name, int subIndex)
		{
			object value;
			if (!BaseTryGet(name, subIndex, out value) && subIndex == 0)
				TryGetFromPropertyDescriptor(name, out value);
			return value;
		}
		
	}
}