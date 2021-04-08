using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using System.Collections;

namespace NetworkUtility
{
    /// <summary>
    /// 
    /// </summary>
    public class JsonDictionary : JsonValue, IDictionary<string, JsonValue>
    {
        private IDictionary<string, JsonValue> _innerDictionary = new Dictionary<string, JsonValue>();

        /// <summary>
        /// 
        /// </summary>
        public const string ElementName = "Dictionary";

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string GetElementName() { return ElementName; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serializer"></param>
        /// <returns></returns>
        protected internal override object AsSerializedValue(JavaScriptSerializer serializer)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            foreach (string key in _innerDictionary.Keys)
                result.Add(key, (_innerDictionary[key] == null) ? null : _innerDictionary[key].AsSerializedValue(serializer));
            return result;
        }
        
        /// <summary>
        /// 
        /// </summary>
        public JsonDictionary() { }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dictionary"></param>
        public JsonDictionary(IDictionary<string, object> dictionary) { _Deserialize(dictionary); }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="serializer"></param>
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
                else if (o is JsonValue)
                    _innerDictionary.Add(key, o as JsonValue);
                else if (o is string)
                    _innerDictionary.Add(key, new JsonText(o as string));
                else if (o is ArrayList)
                    _innerDictionary.Add(key, new JsonArray(o as ArrayList));
                else if (o is IDictionary<string, object>)
                    _innerDictionary.Add(key, new JsonDictionary(o as IDictionary<string, object>));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public ICollection<string> GetKeys() { return _innerDictionary.Keys; }
		
		ICollection<string> IDictionary<string,JsonValue>.Keys { get { return _innerDictionary.Keys; } }

        /// <summary>
        /// 
        /// </summary>
        public ICollection<JsonValue> Values { get { return _innerDictionary.Values; } }

        /// <summary>
        /// 
        /// </summary>
        public int Count { get { return _innerDictionary.Count; } }

        bool ICollection<KeyValuePair<string,JsonValue>>.IsReadOnly { get { return false; } }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public JsonValue this[string key]
        {
            get { return _innerDictionary[key]; }
            set { _innerDictionary[key] = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsKey(string key) { return _innerDictionary.ContainsKey(key); }

        
        bool ICollection<KeyValuePair<string,JsonValue>>.Contains(KeyValuePair<string,JsonValue> value) { return (_innerDictionary as ICollection<KeyValuePair<string,JsonValue>>).Contains(value); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Add(string key, JsonValue value) { _innerDictionary.Add(key, value); }

        void ICollection<KeyValuePair<string,JsonValue>>.Add(KeyValuePair<string,JsonValue> value) { (_innerDictionary as ICollection<KeyValuePair<string,JsonValue>>).Add(value); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Remove(string key) { return _innerDictionary.Remove(key); }

        bool ICollection<KeyValuePair<string,JsonValue>>.Remove(KeyValuePair<string,JsonValue> value) { return (_innerDictionary as ICollection<KeyValuePair<string,JsonValue>>).Remove(value); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetValue(string key, out JsonValue value) { return _innerDictionary.TryGetValue(key, out value); }

        /// <summary>
        /// 
        /// </summary>
        public void Clear() { _innerDictionary.Clear(); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(KeyValuePair<string,JsonValue>[] array, int arrayIndex) { _innerDictionary.CopyTo(array, arrayIndex); }
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator<KeyValuePair<string,JsonValue>> GetEnumerator() { return _innerDictionary.GetEnumerator(); }

        IEnumerator IEnumerable.GetEnumerator() { return (_innerDictionary as IEnumerable).GetEnumerator(); }
    }
}
