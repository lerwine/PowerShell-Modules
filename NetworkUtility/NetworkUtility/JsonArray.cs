using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Web.UI.WebControls;
using System.Web.Script.Serialization;
using System.Collections;

namespace NetworkUtilityCLR
{
    public class JsonArray : JSonValue, IList<JSonValue>
    {
        private IList<JSonValue> _innerList = new List<JSonValue>();
        
        public const string ElementName = "Array";
        public override string GetElementName() { return ElementName; }
        protected internal override object AsSerializedValue(JavaScriptSerializer serializer)
        {
            ArrayList result = new ArrayList();
            foreach (JSonValue e in _innerList)
                result.Add((e == null) ? null : e.AsSerializedValue(serializer));
            return result;
        }
		public JsonArray() { }
		public JsonArray(ArrayList array) { _Deserialize(array); }
		protected override void OnDeserialize(object obj, JavaScriptSerializer serializer)
		{
			_innerList.Clear();
			_Deserialize(obj as ArrayList);
		}
		private void _Deserialize(ArrayList array)
		{
			if (array == null)
				return;
			
            foreach (object o in array)
			{
				if (o == null)
					_innerList.Add(null);
				else if (o is JSonValue)
					_innerList.Add(o as JSonValue);
				else if (o is string)
					_innerList.Add(new JsonText(o as string));
				else if (o is ArrayList)
					_innerList.Add(new JsonArray(o as ArrayList));
				else if (o is IDictionary<string, object>)
					_innerList.Add(new JsonDictionary(o as IDictionary<string, object>));
			}
		}
		
        public JSonValue this[int index]
        {
            get { return _innerList[index]; }
            set { _innerList[index] = value; }
        }
        public int IndexOf(JSonValue item) { return _innerList.IndexOf(item); }
        public void Insert(int index, JSonValue item) { _innerList.Insert(index, item); }
        public void RemoveAt(int index) { _innerList.RemoveAt(index); }
        public int Count { get { return _innerList.Count; } }
        bool ICollection<JSonValue>.IsReadOnly { get { return false; } }
        public void Add(JSonValue value) { _innerList.Add(value); }
        public void Clear() { _innerList.Clear(); }
        public bool Contains(JSonValue item) { return _innerList.Contains(item); }
        public void CopyTo(JSonValue[] array, int arrayIndex) { _innerList.CopyTo(array, arrayIndex); }
        public bool Remove(JSonValue item) { return _innerList.Remove(item); }
        public IEnumerator<JSonValue> GetEnumerator() { return _innerList.GetEnumerator(); }
        IEnumerator IEnumerable.GetEnumerator() { return (_innerList as IEnumerable).GetEnumerator(); }
    }
}
