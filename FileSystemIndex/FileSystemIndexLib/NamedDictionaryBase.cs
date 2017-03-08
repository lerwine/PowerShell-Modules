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
    public class NamedDictionaryBase<string, TValue> : ListDictionaryBase<string, TValue>
    {
		public NamedDictionaryBase() : base() { }
		
		public NamedDictionaryBase(bool caseSensitive) : base(caseSensitive) { }
		
		public NamedDictionaryBase(IEqualityComparer<string> nameComparer) : base(nameComparer) { }
		
		protected override int BaseAdd(string name, TValue value)
		{
			if (name == null)
				throw new ArgumentNullException("name");
			
			return base.BaseAdd(name, value);
		}
		
		protected override void BaseInsert(int index, string name, TValue value)
		{
			if (name == null)
				throw new ArgumentNullException("name");
			
			return base.BaseInsert(index, name, value);
		}
		
		protected override bool BaseRemove(string name, TValue value)
		{
			if (name == null)
				return false;
			
			return base.BaseRemove(name, value);
		}
		
		protected override bool BaseRemove(string name)
		{
			if (name == null)
				return false;
			
			return base.BaseRemove(name);
		}
		
		protected override bool BaseRemoveAt(string name, int subIndex)
		{
			if (name == null || subIndex < 0)
				return false;
			
			return base.BaseRemoveAt(name, subIndex);
		}
		
		protected override bool BaseTryGet(string name, out TValue value)
		{
			if (name != null)
				return base.BaseTryGet(name, out value);

			value = null;
			return false;
		}
		
		protected override bool BaseTryGet(string name, int subIndex, out TValue value)
		{
			if (name != null && subIndex > -1)
				return base.BaseTryGet(name, subIndex, out value);

			value = null;
			return false;
		}
		
		protected override TValue BaseGet(string name)
		{
			if (name == null)
				throw new ArgumentNullException("name");
			
			return base.BaseGet(name);
		}
		
		protected override TValue BaseGet(string name, int subIndex)
		{
			if (name == null)
				throw new ArgumentNullException("name");
			
			return base.BaseGet(name, subIndex);
		}
		
		protected override int BaseSet(string name, TValue value)
		{
			if (name == null)
				throw new ArgumentNullException("name");
			
			return base.BaseGet(name, value);
		}
		
		protected virtual int BaseSet(string name, int subIndex, TValue value)
		{
			if (name == null)
				throw new ArgumentNullException("name");
			
			return base.BaseGet(name, subIndex, value);
		}
	}
}