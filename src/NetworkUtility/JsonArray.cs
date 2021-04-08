using System.Collections;
using System.Collections.Generic;
using System.Web.Script.Serialization;

namespace NetworkUtility
{
    /// <summary>
    /// Represents an array of JSON-serializable values.
    /// </summary>
    public class JsonArray : JsonValue, IList<JsonValue>
    {
        private IList<JsonValue> _innerList = new List<JsonValue>();

        /// <summary>
        /// Default name of element for serializing <see cref="JsonArray"/> objects.
        /// </summary>
        public const string ElementName = "Array";

        /// <summary>
        /// Gets the default name for serializing this <see cref="JsonArray"/> object.
        /// </summary>
        /// <returns>The default name for serializing this <see cref="JsonArray"/> object.</returns>
        public override string GetElementName() { return ElementName; }

        /// <summary>
        /// Converts the current <see cref="JsonArray"/> object to objects which are ready for serialization.
        /// </summary>
        /// <param name="serializer">Serializer that will be used to serialize the data.</param>
        /// <returns></returns>
        protected internal override object AsSerializedValue(JavaScriptSerializer serializer)
        {
            ArrayList result = new ArrayList();
            foreach (JsonValue e in _innerList)
                result.Add((e == null) ? null : e.AsSerializedValue(serializer));
            return result;
        }

        /// <summary>
        /// Initializes a new, empty <see cref="JsonArray"/> object.
        /// </summary>
        public JsonArray() { }

        /// <summary>
        /// Create a new <see cref="JsonArray"/> object from an array of values.
        /// </summary>
        /// <param name="array"></param>
        public JsonArray(ArrayList array) { _Deserialize(array); }

        /// <summary>
        /// Stores deserialized data.
        /// </summary>
        /// <param name="obj">Data being deserialized</param>
        /// <param name="serializer">Object which was used to deserialize data.</param>
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
                else if (o is JsonValue)
                    _innerList.Add(o as JsonValue);
                else if (o is string)
                    _innerList.Add(new JsonText(o as string));
                else if (o is ArrayList)
                    _innerList.Add(new JsonArray(o as ArrayList));
                else if (o is IDictionary<string, object>)
                    _innerList.Add(new JsonDictionary(o as IDictionary<string, object>));
            }
        }
        
        /// <summary>
        /// Gets a <seealso cref="JsonValue"/> object at the specified collection index.
        /// </summary>
        /// <param name="index">Index from which to retrieve the <seealso cref="JsonValue"/>.</param>
        /// <returns><seealso cref="JsonValue"/> from the specified index.</returns>
        public JsonValue this[int index]
        {
            get { return _innerList[index]; }
            set { _innerList[index] = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int IndexOf(JsonValue item) { return _innerList.IndexOf(item); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        public void Insert(int index, JsonValue item) { _innerList.Insert(index, item); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        public void RemoveAt(int index) { _innerList.RemoveAt(index); }

        /// <summary>
        /// 
        /// </summary>
        public int Count { get { return _innerList.Count; } }

        bool ICollection<JsonValue>.IsReadOnly { get { return false; } }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public void Add(JsonValue value) { _innerList.Add(value); }

        /// <summary>
        /// 
        /// </summary>
        public void Clear() { _innerList.Clear(); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(JsonValue item) { return _innerList.Contains(item); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(JsonValue[] array, int arrayIndex) { _innerList.CopyTo(array, arrayIndex); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(JsonValue item) { return _innerList.Remove(item); }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator<JsonValue> GetEnumerator() { return _innerList.GetEnumerator(); }

        IEnumerator IEnumerable.GetEnumerator() { return (_innerList as IEnumerable).GetEnumerator(); }
    }
}
