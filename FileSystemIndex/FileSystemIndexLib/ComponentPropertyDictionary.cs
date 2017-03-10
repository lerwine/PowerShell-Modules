using System;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Threading;

namespace Erwine.Leonard.T.GDIPlus
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    [Serializable()]
    public class ComponentPropertyDictionary<TKey> : PropertyDictionary
        where TKey : IComparable
    {
        private CrawledComponent<TKey> _owner;
        private PropertyDescriptorCollection _properties;

        /// <summary>
        /// 
        /// </summary>
        public ComponentPropertyDictionary() : this(null) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="owner"></param>
        public ComponentPropertyDictionary(CrawledComponent<TKey> owner) : base(false)
        {
            _owner = owner;
            Initialize();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected override object OnGetAssertedValue(string key)
        {
            CrawledComponent<TKey> owner = _owner;
            PropertyDescriptorCollection properties = _properties;
            object v;
            if (owner != null && properties == null && (v = properties.Find(key, true)) != null)
                return v;

            return base.OnGetAssertedValue(key);
        }
        
        internal void Initialize(CrawledComponent<TKey> owner)
        {
            if (owner == null)
                throw new ArgumentNullException("owner");
            if (!ReferenceEquals(owner.Properties, this))
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
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
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
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected override bool BaseTryGet(string name, out object[] value)
        {
            if (name == null)
            {
                value = null;
                return false;
            }
            
            if (base.BaseTryGet(name, out value))
                return true;
            object o;
            if (TryGetFromPropertyDescriptor(name, out o))
            {
                value = new object[] { o };
                return true;
            }

            value = new object[0];
            return false;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="subIndex"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected override bool BaseTryGet(string name, int subIndex, out object value)
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
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected override object[] BaseGet(string name)
        {
            object[] value;
            if (BaseTryGet(name, out value))
                return value;

            object o;
            if (TryGetFromPropertyDescriptor(name, out o))
                return new object[] { o };

            return new object[0];
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="subIndex"></param>
        /// <returns></returns>
        protected override object BaseGet(string name, int subIndex)
        {
            object value;
            if (!BaseTryGet(name, subIndex, out value) && subIndex == 0)
                return TryGetFromPropertyDescriptor(name, out value);
            return value;
        }
        
    }
}