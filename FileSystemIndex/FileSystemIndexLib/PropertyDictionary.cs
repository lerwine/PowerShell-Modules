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
    public class PropertyDictionary : NamedDictionaryBase<object>
    {
		public PropertyDictionary() : base() { }
		
		public PropertyDictionary(bool caseSensitive) : base(caseSensitive) { }
		
		public PropertyDictionary(IEqualityComparer<string> nameComparer) : base(nameComparer) { }
	}
	
	[Serializable()]
    public class PropertyDictionary<TValue> : NamedDictionaryBase<string, TValue>
    {
		public PropertyDictionary() : base() { }
		
		public PropertyDictionary(bool caseSensitive) : base(caseSensitive) { }
		
		public PropertyDictionary(IEqualityComparer<string> nameComparer) : base(nameComparer) { }
		
		public TValue this[string name]
		{
			get { return BaseGet(name); }
			set { BaseSet(name, value); }
		}
		
		public void Add(string name, TValue value) { BaseAdd(name, value); }
		
		public bool TryGetValue(string name, out TValue value) { return BaseTryGet(name, out value); }
	}
}