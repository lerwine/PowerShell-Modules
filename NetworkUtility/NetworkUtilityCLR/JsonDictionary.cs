using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Web.UI.WebControls;
using System.Web.Script.Serialization;
using System.Collections;

namespace NetworkUtilityCLR
{
    public class JsonDictionary : JSonValue, IDictionary<string, JSonValue>
    {
        private IDictionary<string, JSonValue> _innerDictionary = new Dictionary<string, JSonValue>();
        public const string ElementName = "Dictionary";
        public override string GetElementName() { return ElementName; }
        protected internal override object AsSerializedValue(JavaScriptSerializer serializer)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            foreach (string key in _innerDictionary.Keys)
                result.Add(key, (_innerDictionary[key] == null) ? null : _innerDictionary[key].AsSerializedValue(serializer));
            return result;
        }
		
		public JsonDictionary() { }
		
		public JsonDictionary(IDictionary<string, object> dictionary) { _Deserialize(dictionary); }
		
        public override void Deserialize(IDictionary<string, object> dictionary, JavaScriptSerializer serializer)
        {
			_innerDictionary.Clear();
			_Deserialize(dictionary);
        }
		
		private void _Deserialize(IDictionary<string, object> dictionary)
		{
			if (dictionary == null)
				return;
			
			foreach (string key in dictionary.Keys)
			{
				object o = dictionary[key];
				if (o == null)
					_innerDictionary.Add(key, null);
				else if (o is JSonValue)
					_innerDictionary.Add(key, o as JSonValue);
				else if (o is string)
					_innerDictionary.Add(key, new JsonText(o as string));
				else if (o is ArrayList)
					_innerDictionary.Add(key, new JsonArray(o as ArrayList));
				else if (o is IDictionary<string, object>)
					_innerDictionary.Add(key, new JsonDictionary(o as IDictionary<string, object>));
			}
		}
		
        public ICollection<string> Keys { get { return _innerDictionary.Keys; } }
        public ICollection<JSonValue> Values { get { return _innerDictionary.Values; } }
        public int Count { get { return _innerDictionary.Count; } }
        bool ICollection<KeyValuePair<string,JSonValue>>.IsReadOnly { get { return false; } }
        public JSonValue this[string key]
        {
            get { return _innerDictionary[key]; }
            set { _innerDictionary[key] = value; }
        }
        public bool ContainsKey(string key) { return _innerDictionary.ContainsKey(key); }
        bool ICollection<KeyValuePair<string,JSonValue>>.Contains(KeyValuePair<string,JSonValue> value) { return (_innerDictionary as ICollection<KeyValuePair<string,JSonValue>>).Contains(value); }
        public void Add(string key, JSonValue value) { _innerDictionary.Add(key, value); }
        void ICollection<KeyValuePair<string,JSonValue>>.Add(KeyValuePair<string,JSonValue> value) { (_innerDictionary as ICollection<KeyValuePair<string,JSonValue>>).Add(value); }
        public bool Remove(string key) { return _innerDictionary.Remove(key); }
        bool ICollection<KeyValuePair<string,JSonValue>>.Remove(KeyValuePair<string,JSonValue> value) { return (_innerDictionary as ICollection<KeyValuePair<string,JSonValue>>).Remove(value); }
        public bool TryGetValue(string key, out JSonValue value) { return _innerDictionary.TryGetValue(key, out value); }
        public void Clear() { _innerDictionary.Clear(); }
        public void CopyTo(KeyValuePair<string,JSonValue>[] array, int arrayIndex) { _innerDictionary.CopyTo(array, arrayIndex); }
        public IEnumerator<KeyValuePair<string,JSonValue>> GetEnumerator() { return _innerDictionary.GetEnumerator(); }
        IEnumerator IEnumerable.GetEnumerator() { return (_innerDictionary as IEnumerable).GetEnumerator(); }
    }
}
