using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Web.UI.WebControls;
using System.Collections;

namespace NetworkUtilityCLR
{
    public class JsonArray : JSonValue, IList<JSonValue>
    {
        private IList<JSonValue> _innerList = new List<JSonValue>();
        
        public const string ElementName = "Array";
        public override string GetElementName() { return ElementName; }
        protected override object AsSerializedValue(JavaScriptSerializer serializer)
        {
            ArrayList result = new ArrayList();
            foreach (JSonValue e in array)
                result.Add((e == null) ? null : e.AsSerializedValue(serializer));
            return result;
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
