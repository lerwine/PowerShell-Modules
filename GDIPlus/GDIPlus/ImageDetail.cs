using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace Erwine.Leonard.T.GDIPlus
{
    public class ImageDetail : ImageInfo, IDictionary<ExifPropertyTag, object>
    {
        private IDictionary<ExifPropertyTag, Tuple<ExifPropertyType, int, object>> _innerDictionary = new Dictionary<ExifPropertyTag, Tuple<ExifPropertyType, int, object>>();

        public ICollection<ExifPropertyTag> Keys
        {
            get
            {
                return _innerDictionary.Keys;
            }
        }

        public ICollection<object> Values
        {
            get
            {
                return _innerDictionary.Values.Select(t => t.Item3).ToArray();
            }
        }

        public int Count
        {
            get
            {
                return _innerDictionary.Count;
            }
        }

        public object this[ExifPropertyTag key]
        {
            get
            {
                return _innerDictionary[key];
            }

            set
            {
                Tuple<ExifPropertyType, int, object> item;
                if (_innerDictionary.ContainsKey(key))
                    item = _innerDictionary[key];
                else
                    throw new NotImplementedException();
                _innerDictionary[key] = new Tuple<ExifPropertyType, int, object>(item.Item1, item.Item2, value);
            }
        }

        public ImageDetail() { }

        public ImageDetail(FileInfo file, Bitmap bitmap)
            : base(file, bitmap)
        {
            if (file == null)
                throw new ArgumentNullException("file");

            if (bitmap == null)
                throw new ArgumentNullException("bitmap");

            bitmap.PropertyItems.Select(p =>
            {
                try { return new { E = (ExifPropertyTag)(p.Id), V = p.Value, L = p.Len, T = (ExifPropertyType)(p.Type) }; }
                catch { return null; }
            }).Where(a => a != null).ToDictionary(k => k.E, k => new Tuple<ExifPropertyType, int, object>(k.T, k.L, k.V));
        }

        public ImageDetail(ImageDetail item)
            : base(item)
        {
        }

        public bool ContainsKey(ExifPropertyTag key)
        {
            return _innerDictionary.ContainsKey(key);
        }

        public void Add(ExifPropertyTag key, object value)
        {
            _innerDictionary.Add(key, value);
        }

        public bool Remove(ExifPropertyTag key)
        {
            return _innerDictionary.Remove(key);
        }

        public bool TryGetValue(ExifPropertyTag key, out object value)
        {
            return _innerDictionary.TryGetValue(key, out value);
        }

        public void Add(KeyValuePair<ExifPropertyTag, object> item)
        {
            _innerDictionary.Add(item);
        }

        public void Clear()
        {
            _innerDictionary.Clear();
        }

        public bool Contains(KeyValuePair<ExifPropertyTag, object> item)
        {
            return _innerDictionary.Contains(item);
        }

        public void CopyTo(KeyValuePair<ExifPropertyTag, object>[] array, int arrayIndex)
        {
            _innerDictionary.CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<ExifPropertyTag, object> item)
        {
            return _innerDictionary.Remove(item);
        }

        public IEnumerator<KeyValuePair<ExifPropertyTag, object>> GetEnumerator()
        {
            return _innerDictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _innerDictionary.GetEnumerator();
        }
    }
}
