using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;

namespace Erwine.Leonard.T.GDIPlus
{
    public class ExifPropertyCollection : IDictionary<IExifKey, object>
    {
		interface IExifKey : IEquatable<IExifKey>, IEquatable<ExifPropertyTag>, IEquatable<int>, IComparable<IExifKey>
		{
			ExifPropertyTag Tag { get; }
			int Key { get; }
			bool IsOtherTag { get; }
		}
		public struct ExifKeyTag : IExifKey
		{
			public static ExifPropertyTag AsExifPropertyTag(int value) { try { return (ExifPropertyTag)value; } catch { return ExifPropertyTag.Unknown; } }
			public static IExifKey Create(int value)
			{
				ExifPropertyTag tag = AsExifPropertyTag(value);
				if (tag == ExifPropertyTag.Unknown)
					return new ExifKeyValue(value);
				return new ExifKeyTag(tag);
			}
			private ExifPropertyTag _value;
			public ExifPropertyTag Tag { get { return _value; } }
			public int Key { get { return (int)_value; } }
			bool IExifKey.IsOtherTag { get { return false; } }
			internal ExifKeyTag(ExifPropertyTag value) { _value = value; }
			public bool Equals(IExifKey value) { return value != null && (value is ExifKeyTag || !value.IsOtherTag) && value.Tag == _value)
			public bool Equals(ExifPropertyTag value) { return value == _value; }
			public bool Equals(int value) { (int)_value == value; }
			public override bool Equals(object obj)
			{
				if (obj == null)
					return false;
				if (obj is IExifKey)
					return Equals(obj as IExifKey);
				if (obj is ExifPropertyTag)
					return Equals((ExifPropertyTag)obj);
				if (obj is int)
					return Equals((int)obj);
				return false;
			}
			public int CompareTo(IExifKey value) { return (value != null && (value is ExifKeyTag || !value.IsOtherTag)) ? CompareTo(value.Tag) : 1; }
			public override int GetHashcode() { return (int)_value; }
		}
		public struct ExifKeyValue : IExifKey
		{
			private int _value;
			ExifPropertyTag IExifKey.Tag { get { return ExifPropertyTag.Unknown; } }
			public int Key { get { return _value; } }
			bool IExifKey.IsOtherTag { get { return true; } }
			internal ExifKeyTag(int value) { _value = value; }
			public bool Equals(IExifKey value) { return (value != null && (value is ExifKeyValue || value.IsOtherTag)) ? Equals(value.Key) : false); }
			bool IEquatable<ExifPropertyTag>.Equals(ExifPropertyTag value) { return false; }
			public bool Equals(int value) { return _value == value; }
			public override bool Equals(object obj)
			{
				if (obj == null)
					return false;
				if (obj is IExifKey)
					return Equals(obj as IExifKey);
				if (obj is int)
					return Equals((int)obj);
				if (obj is ExifPropertyTag)
					return Equals((ExifPropertyTag)obj);
				return false;
			}
			public int CompareTo(IExifKey value) { return (value == null) ? 1 : ((value is ExifKeyValue || value.IsOtherTag) ? _value.CompareTo(value.Key) : -1); }
			public override int GetHashcode() { return _value; }
		}
		private Dictionary<IExifKey, object> _knownValues = new Dictionary<IExifKey, object>();
		private Dictionary<IExifKey, List<RawExifData>> _rawValues = new Dictionary<IExifKey, List<RawExifData>>();
		
		private static IExifKey GetMatchingKey(ICollection<IExifKey> keys, ExifPropertyTag tag)
		{
			foreach (IExifKey k in keys)
			{
				if (k.Equals(tag))
					return k;
			}
			return null;
		}
		
		private static IExifKey GetMatchingKey(ICollection<IExifKey> keys, int id)
		{
			foreach (IExifKey k in keys)
			{
				if (k.Equals(id))
					return k;
			}
			return null;
		}
		
		#region IDictionary<IExifKey, object> Members
		
		public object this[ExifPropertyTag tag]
		{
			get
			{
				IExifKey key = GetMatchingKey(_knownValues.Keys, tag);
				if (key != null)
					return _knownValues[key];
				
				if ((key = GetMatchingKey(_rawValues.Keys, tag)) == null)
					return null;
				
				object result = RawExifData.ToValue(_rawValues[key], tag);
				_knownValues.Add(key, result);
				_rawValues.Remove(key);
				return result;
			}
		}
		public object this[short id]
		{
			get
			{
				IExifKey key = GetMatchingKey(_knownValues.Keys, id);
				if (key != null)
					return _knownValues[key];
				
				if ((key = GetMatchingKey(_rawValues.Keys, id)) == null)
					return null;
				
				object result = RawExifData.ToValue(_rawValues[key], key.Tag);
				_knownValues.Add(key, result);
				_rawValues.Remove(key);
				return result;
			}
		}
		public object this[IExifKey key]
		{
			get
			{
				if (_knownValues.ContainsKey(key))
					return _knownValues[key];
				
				if (!_rawValues.ContainsKey(key))
					return null;
				
				object result = RawExifData.ToValue(_rawValues[key], key.Tag);
				_knownValues.Add(key, result);
				_rawValues.Remove(key);
				return result;
			}
		}
		object IDictionary<IExifKey, object>.this[IExifKey key]
		{
			get { return this[key]; }
			set { throw new NotSupportedException(); }
		}
		public ICollection<IExifKey> Keys
		{
			get
			{
				ProcessAllRawValues();
				return _knownValues.Keys;
			}
		}
		public ICollection<object> Values
		{
			get
			{
				ProcessAllRawValues();
				return _knownValues.Values;
			}
		}
		
		void IDictionary<IExifKey, object>.Add(IExifKey key, object value) { throw new NotSupportedException(); }
		public bool ContainsKey(IExifKey key) { return _knownValues.ContainsKey(key) || _rawValues.ContainsKey(key); }
		public bool ContainsKey(ExifPropertyTag tag) { return GetMatchingKey(_knownValues.Keys, tag) != null || GetMatchingKey(_rawValues, tag) != null; }
		public bool ContainsKey(int id) { return GetMatchingKey(_knownValues.Keys, id) != null || GetMatchingKey(_rawValues, id) != null; }
		bool IDictionary<IExifKey, object>.Remove(IExifKey key) { throw new NotSupportedException(); }
		public bool TryGetValue(IExifKey key, out object value)
		{
			if (_knownValues.ContainsKey(key))
			{
				value = _knownValues[key];
				return true;
			}
			
			if (!_rawValues.ContainsKey(key))
			{
				value = null;
				return false;
			}
			
			object result = RawExifData.ToValue(_rawValues[key], key.Tag);
			_knownValues.Add(key, result);
			_rawValues.Remove(key);
			value = result;
			return true;
		}
		public bool TryGetValue(ExifPropertyTag tag, out object value)
		{
			IExifKey key = GetMatchingKey(_knownValues.Keys, tag);
			if (key != null)
			{
				value = _knownValues[key];	
				return true;
			}
			
			if ((key = GetMatchingKey(_rawValues.Keys, tag)) == null)
			{
				value = null;
				return false;
			}
			
			object result = RawExifData.ToValue(_rawValues[key], tag);
			_knownValues.Add(key, result);
			_rawValues.Remove(key);
			value = result;
			return true;
		}
		public bool TryGetValue(short id, out object value)
		{
			IExifKey key = GetMatchingKey(_knownValues.Keys, id);
			if (key != null)
			{
				value = _knownValues[key];
				return true;
			}
			
			if ((key = GetMatchingKey(_rawValues.Keys, id)) == null)
			{
				value = null;
				return false;
			}
			
			object result = RawExifData.ToValue(_rawValues[key], key.Tag);
			_knownValues.Add(key, result);
			_rawValues.Remove(key);
			value = result;
			return true;
		}
	  
		#endregion
		
		#region ICollection<KeyValuePair<IExifKey, object>> Members
		
        public int Count { get { return _knownValues.Count + _rawValues.Count; } }

        void ICollection<KeyValuePair<IExifKey, object>>.Add(KeyValuePair<IExifKey, object> item) { throw new NotSupportedException(); }

        void ICollection<KeyValuePair<IExifKey, object>>.Clear() { throw new NotSupportedException(); }

        bool ICollection<KeyValuePair<IExifKey, object>>.Contains(KeyValuePair<IExifKey, object> item) { return _innerDictionary.Contains(item); }

        public void CopyTo(KeyValuePair<IExifKey, object>[] array, int arrayIndex) { _innerDictionary.CopyTo(array, arrayIndex); }

        bool ICollection<KeyValuePair<IExifKey, object>>.Remove(KeyValuePair<IExifKey, object> item) { throw new NotSupportedException(); }

        public IEnumerator<KeyValuePair<IExifKey, object>> GetEnumerator() { return _innerDictionary.GetEnumerator(); }

        IEnumerator IEnumerable.GetEnumerator() { return _innerDictionary.GetEnumerator(); }

		#endregion
		
		public void ProcessAllRawValues()
		{
			if (_rawValues.Count > 0)
				_ProcessAllRawValues();
		}
		private void _ProcessAllRawValues()
		{
			foreach (IExifKey key in _rawValues.Keys)
				_knownValues.Add(key, RawExifData.ToValue(_rawValues[key], key.Tag))
			_rawValues.Clear();
		}
		
		public ExifPropertyCollection(PropertyItem[] properties)
		{
			if (properties == null || properties.Length == 0)
				return;
			
			foreach (PropertyItem p in properties)
			{
				if (p == null)
					continue;
				
				IExifKey key = ExifKeyTag.Create(p.Id);
				List<RawExifData> list;
				if (_rawValues.ContainsKey(key))
					list = _rawValues[key];
				else
				{
					list = new List<RawExifData>();
					_rawValues.Add(key, list);
				}
				list.Add(new RawExifData(p));
			}
		}
		
		class RawExifData
		{
			internal static object ToValue(List<RawExifData> items, ExifPropertyTag tag)
			{
				if (items.Count == 1)
					return items[0].ToValue(tag);
				
				List<object> values = new List<object>();
				foreach (RawExifData d in items)
					values.Add(d.ToValue(tag));
				
				if (AllIsOfType<string>(values))
				{
					List<string> result = new List<string>();
					foreach (object v in values)
						result.Add(v as string);
					return result.ToArray();
				}
				
				if (AllIsOfType<ushort>(values))
				{
					List<ushort> result = new List<ushort>();
					foreach (object v in values)
						result.Add((ushort)v);
					return result.ToArray();
				}
				
				if (AllIsOfType<uint>(values))
				{
					List<uint> result = new List<uint>();
					foreach (object v in values)
						result.Add((uint)v);
					return result.ToArray();
				}
				
				if (AllIsOfType<int>(values))
				{
					List<int> result = new List<int>();
					foreach (object v in values)
						result.Add((int)v);
					return result.ToArray();
				}
				
				if (AllIsOfType<decimal>(values))
				{
					List<decimal> result = new List<decimal>();
					foreach (object v in values)
						result.Add((decimal)v);
					return result.ToArray();
				}
				
				if (AllIsOfType<byte>(values))
				{
					List<byte> result = new List<byte>();
					foreach (object v in values)
						result.Add((byte)v);
					return result.ToArray();
				}
				
				return values.ToArray();
			}
			private static bool AllIsOfType<T>(IEnumerable<object> source)
			{
				foreach (object obj in source)
				{
					if (obj == null || !(obj is T || obj is T[]))
						return false;
				}
				
				return true;
			}
			private int _len;
			private short _type;
			private byte[] _value;
			internal RawExifData(PropertyItem property)
			{
				_len = property.Len;
				_type = property.Type;
				_value = property.Value;
			}
			internal object ToValue(ExifPropertyTag tag)
			{
				ExifPropertyType type;
				try { type = (ExifPropertyType)_type; } catch { type = ExifPropertyType.Unknown; }
				ExifPropertyDetailAttribute attribute;
				if (type == ExifPropertyTag.Unknown || type = ExifPropertyTag.BestMatching)
				{
					attribute = ExifPropertyDetailAttribute.GetExifPropertyDetail(tag, type);
					type = attribute.Type;
				}
				else
					attribute = null;
				
				switch (type)
				{
					case ExifPropertyType.Short:
						return (_len == 1) ? AsUInt16(0) :  new List<ushort>(AsUInt16Values()).ToArray();
					case ExifPropertyType.Long:
						return (_len == 1) ? AsUInt32(0) :  new List<uint>(AsUInt32Values()).ToArray();
					case ExifPropertyType.SLong:
						return (_len == 1) ? AsInt32(0) :  new List<int>(AsInt32Values()).ToArray();
					case ExifPropertyType.Rational:
						return (_len == 1) ? AsRational(0) :  new List<decimal>(AsRationalValues()).ToArray();
					case ExifPropertyType.SRational:
						return (_len == 1) ? AsSRational(0) :  new List<decimal>(AsSRationalValues()).ToArray();
					case ExifPropertyType.ASCII:
						if (attribute == null)
							attribute = ExifPropertyDetailAttribute.GetExifPropertyDetail(tag, type);
						return AsASCII(attribute.IsNullTerminatedString);
				}
				
				return (_len == 1) ? AsByte(0) : new List<byte>(AsByteValues()).ToArray();
			}
			private ushort AsUInt16(int startIndex)
			{
				if (startIndex >= _value.Length)
					return 0;
				if (startIndex == _value.Length - 1)
					return (ushort)(_value[startIndex]);
				return BitConverter.ToUInt16(_value, startIndex);
			}
			private IEnumerable<ushort> AsUInt16Values()
			{
				int startIndex = 0;
				for (int i = 0; i < _len && startIndex < _value.Length; i++)
				{
					yield return AsUInt16(startIndex);
					startIndex += 2;
				}
			}
			private uint AsUInt32(int startIndex)
			{
				if (startIndex >= _value.Length)
					return 0;
				if (startIndex == _value.Length - 3)
					return (uint)(AsUInt16(startIndex) << 8 | (uint)(_value[startIndex + 2]));
				if (startIndex == _value.Length - 2)
					return (uint)(AsUInt16(startIndex));
				if (startIndex == _value.Length - 1)
					return (uint)(_value[startIndex]);
				return BitConverter.ToUInt32(_value, startIndex);
			}
			private IEnumerable<uint> AsUInt32Values()
			{
				int startIndex = 0;
				for (int i = 0; i < _len && startIndex < _value.Length; i++)
				{
					yield return AsUInt32(startIndex);
					startIndex += 4;
				}
			}
			private int AsInt32(int startIndex)
			{
				if (startIndex >= _value.Length)
					return 0;
				if (startIndex == _value.Length - 3)
					return (int)(AsUInt16(startIndex) << 8 | (int)(_value[startIndex + 2]));
				if (startIndex == _value.Length - 2)
					return (int)(AsUInt16(startIndex));
				if (startIndex == _value.Length - 1)
					return (int)(_value[startIndex]);
				return BitConverter.ToInt32(_value, startIndex);
			}
			private IEnumerable<int> AsInt32Values()
			{
				int startIndex = 0;
				for (int i = 0; i < _len && startIndex < _value.Length; i++)
				{
					yield return AsInt32(startIndex);
					startIndex += 4;
				}
			}
			private decimal AsRational(int startIndex)
			{
				if (startIndex >= _value.Length)
					return 0.0m;
				if (startIndex > _value.Length - 5)
					return Convert.ToDecimal(AsUInt32(startIndex));
				Convert.ToDecimal(AsUInt32(startIndex)) / Convert.ToDecimal(AsUInt32(startIndex + 4))
			}
			private IEnumerable<decimal> AsRationalValues()
			{
				int startIndex = 0;
				for (int i = 0; i < _len && startIndex < _value.Length; i++)
				{
					yield return AsRational(startIndex);
					startIndex += 8;
				}
			}
			private decimal AsSRational(int startIndex)
			{
				if (startIndex >= _value.Length)
					return 0.0m;
				if (startIndex > _value.Length - 5)
					return Convert.ToDecimal(AsInt32(startIndex));
				Convert.ToDecimal(AsInt32(startIndex)) / Convert.ToDecimal(AsInt32(startIndex + 4))
			}
			private IEnumerable<decimal> AsSRationalValues()
			{
				int startIndex = 0;
				for (int i = 0; i < _len && startIndex < _value.Length; i++)
				{
					yield return AsSRational(startIndex);
					startIndex += 8;
				}
			}
			private object AsASCII(bool isNullTerminatedString)
			{
				if (_len == 0 || _value.Length == 0)
					return "";
				
				if (isNullTerminatedString)
				{
					int index = 0;
					string firstValue = null;
					while (index < _len && index < _value.Length)
					{
						if (_value[index] == 0)
						{
							firstValue = (index == 0) ? "" : Encoding.ASCII.GetString(_value, 0, index);
							index++;
							if (index < _len && index < _value.Length)
								return (new List<string>(AsASCIIValues(firstValue, 1))).ToArray();
							return firstValue;
						}
					}
				}
				
				return Encoding.ASCII.GetString(_value, 0, (_len > _value.Length) ? _value.Length : _len);
			}
			private IEnumerable<string> AsASCIIValues(string firstValue, int startIndex)
			{
				yield return firstValue;
				for (int i = startIndex; i < _len && i < _value.Length; i++)
				{
					if (_value[i] == 0)
					{
						yield return (i == startIndex) ? "" : Encoding.ASCII.GetString(_value, startIndex, i - startIndex);
						startIndex = i + 1;
					}
				}
				
				if (startIndex < _len && startIndex < _value.Length)
					yield return Encoding.ASCII.GetString(_value, startIndex, ((_value.Length < _len) ? _value.Length : _len) - startIndex);
			}
		}
	}
}