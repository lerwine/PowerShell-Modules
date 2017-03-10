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
    /// <typeparam name="TValue"></typeparam>
    [Serializable()]
    public class NamedDictionaryBase<TValue> : ListDictionaryBase<string, TValue>
    {
        /// <summary>
        /// 
        /// </summary>
        public NamedDictionaryBase() : base() { }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="caseSensitive"></param>
        public NamedDictionaryBase(bool caseSensitive) : this((caseSensitive) ? StringComparer.InvariantCulture : StringComparer.InvariantCultureIgnoreCase) { }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="nameComparer"></param>
        public NamedDictionaryBase(IEqualityComparer<string> nameComparer) : base(nameComparer) { }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected override int BaseAdd(string name, TValue value)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            
            return base.BaseAdd(name, value);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        protected override void BaseInsert(int index, string name, TValue value)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            
            base.BaseInsert(index, name, value);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected override bool BaseRemove(string name, TValue value)
        {
            if (name == null)
                return false;
            
            return base.BaseRemove(name, value);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected override bool BaseRemove(string name)
        {
            if (name == null)
                return false;
            
            return base.BaseRemove(name);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="subIndex"></param>
        /// <returns></returns>
        protected override bool BaseRemoveAt(string name, int subIndex)
        {
            if (name == null || subIndex < 0)
                return false;
            
            return base.BaseRemoveAt(name, subIndex);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected override bool BaseTryGet(string name, out TValue[] value)
        {
            if (name != null)
                return base.BaseTryGet(name, out value);

            value = new TValue[0];
            return false;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="subIndex"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected override bool BaseTryGet(string name, int subIndex, out TValue value)
        {
            if (name != null && subIndex > -1)
                return base.BaseTryGet(name, subIndex, out value);

            value = default(TValue);
            return false;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected override TValue[] BaseGet(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            
            return base.BaseGet(name);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="subIndex"></param>
        /// <returns></returns>
        protected override TValue BaseGet(string name, int subIndex)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            
            return base.BaseGet(name, subIndex);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected override int BaseSet(string name, TValue value)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            
            return base.BaseSet(name, value);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="subIndex"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected override int BaseSet(string name, int subIndex, TValue value)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            
            return base.BaseSet(name, subIndex, value);
        }
    }
}