using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Web.UI.WebControls;
using System.Collections;

namespace NetworkUtilityCLR
{
    public class JsonDictionary : JSonValue, IDictionary<string, JSonValue>
    {
        private IDictionary<string, JSonValue> _innerDictionary = new Dictionary<string, JSonValue>();
        public const string ElementName = "Dictionary";
        public override string GetElementName() { return ElementName; }
        protected override object AsSerializedValue(JavaScriptSerializer serializer)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            foreach (string key in Keys)
                result.Add(key, (obj[key] == null) ? null : obj[key].AsSerializedValue(serializer));
            return result;
        }

        public override Deserialize(IDictionary<string, object> dictionary, JavaScriptSerializer serializer)
        {
            if (dictionary == null)
                return;
            
            if (type == typeof(JsonDictionary))
            
            if (type == typeof(ListItemCollection))
            {
                // Create the instance to deserialize into.
                ListItemCollection list = new ListItemCollection();

                // Deserialize the ListItemCollection's items.
                ArrayList itemsList = (ArrayList)dictionary["List"];
                for (int i=0; i<itemsList.Count; i++)
                    list.Add(serializer.ConvertToType<ListItem>(itemsList[i]));

                return list;
            }
            return null;
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
        void ICollection<KeyValuePair<string,JSonValue>>.Add(string key, KeyValuePair<string,JSonValue> value) { (_innerDictionary as ICollection<KeyValuePair<string,JSonValue>>).Add(value);
        public bool Remove(string key) { return _innerDictionary.Remove(key); }
        bool ICollection<KeyValuePair<string,JSonValue>>.Remove(KeyValuePair<string,JSonValue> value) { return (_innerDictionary as ICollection<KeyValuePair<string,JSonValue>>).Remove(value); }
        public bool TryGetValue(string key, out JSonValue value) { _innerDictionary.TryGetValue(key, out value); }
        public void Clear() { _innerDictionary.Clear(); }
        public void CopyTo(KeyValuePair<string,JSonValue>[] array, int arrayIndex) { _innerDictionary.CopyTo(array, arrayIndex); }
        public IEnumerator<KeyValuePair<string,JSonValue>> GetEnumerator() { return _innerDictionary.GetEnumerator(); }
        IEnumerator IEnumerable.GetEnumerator() { return (_innerDictionary as IEnumerable).GetEnumerator(); }
    }
}
