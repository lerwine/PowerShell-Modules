using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests
{
    public class PSObjectHelper : IList<PSObjectHelper.PropertyDictionary>
    {
        private IList<PSObjectHelper.PropertyDictionary> _targets;

        public class PropertyDictionary : IDictionary<string, PSPropertyHelper>
        {
            private PSObject _target;
            private Dictionary<string, PSPropertyHelper> _innerDictionary;

            public ICollection<string> Keys
            {
                get
                {
                    return _innerDictionary.Keys;
                }
            }

            public ICollection<PSPropertyHelper> Values
            {
                get
                {
                    return _innerDictionary.Values;
                }
            }

            public int Count
            {
                get
                {
                    return _innerDictionary.Count;
                }
            }
            
            bool ICollection<KeyValuePair<string, PSPropertyHelper>>.IsReadOnly { get { return true; } }

            PSPropertyHelper IDictionary<string, PSPropertyHelper>.this[string key]
            {
                get
                {
                    return _innerDictionary[key];
                }
                set { throw new NotSupportedException(); }
            }

            public PSPropertyHelper this[string key]
            {
                get
                {
                    return _innerDictionary[key];
                }
            }

            public PropertyDictionary(PSObject target)
            {
                _target = target;
                if (_target == null)
                    _innerDictionary = new Dictionary<string, PSPropertyHelper>();
                else
                    _innerDictionary = target.Properties.ToDictionary(p => p.Name, p => new PSPropertyHelper(p));
            }

            public bool ContainsKey(string key)
            {
                return _innerDictionary.ContainsKey(key);
            }

            public bool TryGetValue(string key, out PSPropertyHelper value)
            {
                return _innerDictionary.TryGetValue(key, out value);
            }

            public void CopyTo(KeyValuePair<string, PSPropertyHelper>[] array, int arrayIndex)
            {
                _innerDictionary.ToArray().CopyTo(array, arrayIndex);
            }

            public IEnumerator<KeyValuePair<string, PSPropertyHelper>> GetEnumerator()
            {
                return _innerDictionary.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return _innerDictionary.GetEnumerator();
            }

            void IDictionary<string, PSPropertyHelper>.Add(string key, PSPropertyHelper value) { throw new NotSupportedException(); }

            bool IDictionary<string, PSPropertyHelper>.Remove(string key) { throw new NotSupportedException(); }

            void ICollection<KeyValuePair<string, PSPropertyHelper>>.Add(KeyValuePair<string, PSPropertyHelper> item) { throw new NotSupportedException(); }

            void ICollection<KeyValuePair<string, PSPropertyHelper>>.Clear() { throw new NotSupportedException(); }

            bool ICollection<KeyValuePair<string, PSPropertyHelper>>.Contains(KeyValuePair<string, PSPropertyHelper> item)
            {
                return _innerDictionary.Contains(item);
            }

            bool ICollection<KeyValuePair<string, PSPropertyHelper>>.Remove(KeyValuePair<string, PSPropertyHelper> item) { throw new NotSupportedException(); }
        }

        public ReadOnlyCollection<object> Targets { get; private set; }

        public int Count
        {
            get
            {
                return _targets.Count;
            }
        }
        
        bool ICollection<PropertyDictionary>.IsReadOnly { get { return true; } }

        PropertyDictionary IList<PropertyDictionary>.this[int index]
        {
            get
            {
                return _targets[index];
            }
            set { throw new NotSupportedException(); }
        }

        public PropertyDictionary this[int index]
        {
            get
            {
                return _targets[index];
            }
        }

        public PSObjectHelper(object obj)
        {
            PSObject psObject = (obj == null || obj is PSObject) ? obj as PSObject : PSObject.AsPSObject(obj);
            obj = (psObject == null) ? null : psObject.BaseObject;
            PSObject[] targets;
            if (obj == null || obj is PSCustomObject || obj is string || !(obj is IEnumerable))
                targets = new PSObject[] { psObject };
            else
                targets = (obj as IEnumerable).Cast<object>().Select(o => (o == null || o is PSObject) ? o as PSObject : PSObject.AsPSObject(o)).ToArray();
            Targets = new ReadOnlyCollection<object>(targets.Select(o => (o == null) ? null : o.BaseObject).ToArray());
            _targets = targets.Select(t => new PropertyDictionary(t)).ToArray();
        }

        public int IndexOf(PropertyDictionary item)
        {
            return _targets.IndexOf(item);
        }

        public bool Contains(PropertyDictionary item)
        {
            return _targets.Contains(item);
        }

        public void CopyTo(PropertyDictionary[] array, int arrayIndex)
        {
            _targets.CopyTo(array, arrayIndex);
        }

        public IEnumerator<PropertyDictionary> GetEnumerator()
        {
            return _targets.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _targets.GetEnumerator();
        }

        void IList<PropertyDictionary>.Insert(int index, PropertyDictionary item) { throw new NotSupportedException(); }

        void IList<PropertyDictionary>.RemoveAt(int index) { throw new NotSupportedException(); }

        void ICollection<PropertyDictionary>.Add(PropertyDictionary item) { throw new NotSupportedException(); }

        void ICollection<PropertyDictionary>.Clear() { throw new NotSupportedException(); }

        bool ICollection<PropertyDictionary>.Remove(PropertyDictionary item) { throw new NotSupportedException(); }
    }
}
