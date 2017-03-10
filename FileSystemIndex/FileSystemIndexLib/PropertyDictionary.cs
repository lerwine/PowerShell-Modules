using System;
using System.Collections.Generic;

namespace Erwine.Leonard.T.GDIPlus
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable()]
    public class PropertyDictionary : NamedDictionaryBase<object>
    {
        /// <summary>
        /// 
        /// </summary>
        public PropertyDictionary() : base() { }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="caseSensitive"></param>
        public PropertyDictionary(bool caseSensitive) : base(caseSensitive) { }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="nameComparer"></param>
        public PropertyDictionary(IEqualityComparer<string> nameComparer) : base(nameComparer) { }
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    [Serializable()]
    public class PropertyDictionary<TValue> : NamedDictionaryBase<TValue>
    {
        /// <summary>
        /// 
        /// </summary>
        public PropertyDictionary() : base() { }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="caseSensitive"></param>
        public PropertyDictionary(bool caseSensitive) : base(caseSensitive) { }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="nameComparer"></param>
        public PropertyDictionary(IEqualityComparer<string> nameComparer) : base(nameComparer) { }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void Add(string name, TValue value) { BaseAdd(name, value); }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetValue(string name, out TValue[] value) { return BaseTryGet(name, out value); }
    }
}