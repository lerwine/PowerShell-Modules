using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Text;

namespace Erwine.Leonard.T.GDIPlus
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class ImageDetail : ImageInfo
    {
        private PSObject _exifTags = new PSObject();

        public PSObject ExifTags { get { return _exifTags; } }

        public ImageDetail() { }

        public ImageDetail(FileInfo file, Bitmap bitmap)
            : base(file, bitmap)
        {
            if (file == null)
                throw new ArgumentNullException("file");

            if (bitmap == null)
                throw new ArgumentNullException("bitmap");

            Type t = typeof(ExifPropertyTag);
            foreach (var value in bitmap.PropertyItems.Select(p =>
              {
                  ExifPropertyTag e;
                  PSObject o = CreateProperty(p, out e);
                  return new { T = e, V = o };
              }).Where(a => a.V != null).GroupBy(a => a.T).Select(g => g.First()))
            {
                _exifTags.Members.Add(new PSNoteProperty(Enum.GetName(t, value.T), value.V));
            }
        }

        private PSObject CreateProperty(PropertyItem propertyItem, out ExifPropertyTag tag)
        {
            try { tag = (ExifPropertyTag)(propertyItem.Id); } catch { tag = ExifPropertyTag.Unknown; }
            if (tag == ExifPropertyTag.Unknown || propertyItem.Value == null)
                return null;
            ExifPropertyType type;
            try { type = (ExifPropertyType)(propertyItem.Type); } catch { type = ExifPropertyType.Undefined; }
            return CreateProperty(propertyItem.Value, propertyItem.Len, type, tag);
        }

        private PSObject CreateProperty(byte[] bytes, int count, ExifPropertyType type, ExifPropertyTag tag)
        {
            Type t = tag.GetType();
            ExifPropertyTypeAttribute[] attributes = t.GetField(Enum.GetName(t, tag)).GetCustomAttributes(typeof(ExifPropertyTypeAttribute), false).OfType<ExifPropertyTypeAttribute>().ToArray();
            if (attributes.Length > 1 && attributes.Any(a => a.Type == type))
                attributes = attributes.Where(a => a.Type == type).ToArray();
            if (attributes.Length > 1 && attributes.Any(a => a.IsPrimary))
                attributes = attributes.Where(a => a.IsPrimary).ToArray();
            ExifPropertyTypeAttribute attr = attributes.DefaultIfEmpty(new ExifPropertyTypeAttribute(type) { HasNullTerminator = type == ExifPropertyType.ASCII }).First();
            PSObject property;
            switch (type)
            {
                case ExifPropertyType.ASCII:
                    if (attr.HasNullTerminator)
                    {
                        if (attr.Count == 0)
                            property = AsStringZMulti(bytes, count);
                        else
                        property = AsStringZ(bytes, count);
                    }
                    else
                        property = AsString(bytes, count);
                    break;
                case ExifPropertyType.Byte:
                    property = AsByteValue(bytes, count);
                    break;
                case ExifPropertyType.Long:
                    property = AsUInt32Value(bytes, count);
                    break;
                case ExifPropertyType.Rational:
                    property = AsUDecimalValue(bytes, count);
                    break;
                case ExifPropertyType.Short:
                    property = AsUInt16Value(bytes, count);
                    break;
                case ExifPropertyType.SLong:
                    property = AsInt32Value(bytes, count);
                    break;
                case ExifPropertyType.SRational:
                    property = AsDecimalValue(bytes, count);
                    break;
                case ExifPropertyType.BestMatching:
#warning Not implemented
                    throw new NotImplementedException();
                default:
                    property = AsHexValues(bytes, count);
                    break;
            }

            property.TypeNames.Add("ExifPropertyTag." + Enum.GetName(typeof(ExifPropertyTag), tag));
            property.TypeNames.Add("ExifPropertyTag." + Enum.GetName(typeof(ExifPropertyTag), tag) + "." + Enum.GetName(typeof(ExifPropertyType), type));
            return property;
        }

        private PSObject AsStringZMulti(byte[] bytes, int length)
        {
            Collection<string> values = new Collection<string>();
            if (bytes.Length == 0 || length == 0)
            {
                values.Add("");
                return PSObject.AsPSObject(values);
            }

            int index = 0;
            int start = 0;
            while (index < length && index < bytes.Length)
            {
                if (bytes[index] == 0)
                {
                    values.Add((start == index) ? "" : Encoding.ASCII.GetString(bytes, start, index - start));
                    start = index + 1;
                }
                index++;
            }

            if (start < index)
                values.Add(Encoding.ASCII.GetString(bytes, start, index - start));
            return PSObject.AsPSObject(values);
        }
        
        private PSObject AsHexValues(byte[] bytes, int count)
        {
            if (bytes.Length == 0 || count
                == 0)
                return PSObject.AsPSObject("");
            return PSObject.AsPSObject("0x" + String.Join("", bytes.Take(count).Select(b => b.ToString("x2"))));
        }

        private IEnumerable<uint> ReadUInt32Values(byte[] bytes, int count)
        {
            int index = 0;
            int c = 0;
            while (index < bytes.Length && c < count)
            {
                if ((bytes.Length - index) > 3)
                {
                    yield return BitConverter.ToUInt32(bytes, index);
                    index += 4;
                    count++;
                }
                else if ((bytes.Length - index) > 2)
                {
                    yield return (uint)(BitConverter.ToUInt16(bytes, index));
                    index += 2;
                    count++;
                }
                else
                {
                    yield return (uint)(bytes[index]);
                    index++;
                    count++;
                }
            }
        }

        private IEnumerable<int> ReadSInt32Values(byte[] bytes, int count)
        {
            int index = 0;
            int c = 0;
            while (index < bytes.Length && c < count)
            {
                if ((bytes.Length - index) > 3)
                {
                    yield return BitConverter.ToInt32(bytes, index);
                    index += 4;
                    count++;
                }
                else if ((bytes.Length - index) > 2)
                {
                    yield return (int)(BitConverter.ToInt16(bytes, index));
                    index += 2;
                    count++;
                }
                else
                {
                    yield return (int)(bytes[index]);
                    index++;
                    count++;
                }
            }
        }

        private IEnumerable<ushort> ReadUInt16Values(byte[] bytes, int count)
        {
            int index = 0;
            int c = 0;
            while (index < bytes.Length && c < count)
            {
                if ((bytes.Length - index) > 1)
                {
                    yield return BitConverter.ToUInt16(bytes, index);
                    index += 2;
                    count++;
                }
                else
                {
                    yield return (ushort)(bytes[index]);
                    index++;
                    count++;
                }
            }
        }

        private IEnumerable<decimal> ReadUDecimalValues(byte[] bytes, int count)
        {
            uint[] values = ReadUInt32Values(bytes, count * 2).ToArray();
            int index = 0;
            int c = 0;
            while (index < values.Length && c < count)
            {
                if ((values.Length - index) > 1)
                {
                    yield return Convert.ToDecimal(values[index]) / Convert.ToDecimal(values[index+1]);
                    index += 2;
                    count++;
                }
                else
                {
                    yield return Convert.ToDecimal(values[index]);
                    index++;
                    count++;
                }
            }
        }

        private IEnumerable<decimal> ReadSDecimalValues(byte[] bytes, int count)
        {
            int[] values = ReadSInt32Values(bytes, count * 2).ToArray();
            int index = 0;
            int c = 0;
            while (index < values.Length && c < count)
            {
                if ((values.Length - index) > 1)
                {
                    yield return Convert.ToDecimal(values[index]) / Convert.ToDecimal(values[index + 1]);
                    index += 2;
                    count++;
                }
                else
                {
                    yield return Convert.ToDecimal(values[index]);
                    index++;
                    count++;
                }
            }
        }

        private PSObject AsDecimalValue(byte[] bytes, int count)
        {
            if (count == 0)
                return null;

            using (IEnumerator<decimal> enumerator = ReadSDecimalValues(bytes, count).GetEnumerator())
            {
                if (!enumerator.MoveNext())
                    return null;

                if (count == 1)
                    return PSObject.AsPSObject(enumerator.Current);

                Collection<decimal> collection = new Collection<decimal>();
                collection.Add(enumerator.Current);
                while (enumerator.MoveNext())
                    collection.Add(enumerator.Current);
                return PSObject.AsPSObject(collection);
            }
        }

        private PSObject AsUDecimalValue(byte[] bytes, int count)
        {
            if (count == 0)
                return null;

            using (IEnumerator<decimal> enumerator = ReadUDecimalValues(bytes, count).GetEnumerator())
            {
                if (!enumerator.MoveNext())
                    return null;

                if (count == 1)
                    return PSObject.AsPSObject(enumerator.Current);

                Collection<decimal> collection = new Collection<decimal>();
                collection.Add(enumerator.Current);
                while (enumerator.MoveNext())
                    collection.Add(enumerator.Current);
                return PSObject.AsPSObject(collection);
            }
        }

        private PSObject AsUInt32Value(byte[] bytes, int count)
        {
            using (IEnumerator<uint> enumerator = ReadUInt32Values(bytes, count).GetEnumerator())
            {
                if (!enumerator.MoveNext())
                    return null;

                if (count == 1)
                    return PSObject.AsPSObject(enumerator.Current);

                Collection<uint> collection = new Collection<uint>();
                collection.Add(enumerator.Current);
                while (enumerator.MoveNext())
                    collection.Add(enumerator.Current);
                return PSObject.AsPSObject(collection);
            }
        }

        private PSObject AsInt32Value(byte[] bytes, int count)
        {
            using (IEnumerator<int> enumerator = ReadSInt32Values(bytes, count).GetEnumerator())
            {
                if (!enumerator.MoveNext())
                    return null;

                if (count == 1)
                    return PSObject.AsPSObject(enumerator.Current);

                Collection<int> collection = new Collection<int>();
                collection.Add(enumerator.Current);
                while (enumerator.MoveNext())
                    collection.Add(enumerator.Current);
                return PSObject.AsPSObject(collection);
            }
        }

        private PSObject AsUInt16Value(byte[] bytes, int count)
        {
            using (IEnumerator<ushort> enumerator = ReadUInt16Values(bytes, count).GetEnumerator())
            {
                if (!enumerator.MoveNext())
                    return null;

                if (count == 1)
                    return PSObject.AsPSObject((int)(enumerator.Current));

                Collection<int> collection = new Collection<int>();
                collection.Add(enumerator.Current);
                while (enumerator.MoveNext())
                    collection.Add(enumerator.Current);
                return PSObject.AsPSObject(collection);
            }
        }

        private PSObject AsByteValue(byte[] bytes, int count)
        {
            if (bytes.Length == 0 || count == 0)
                return null;

            if (count == 1)
                return PSObject.AsPSObject((int)(bytes[0]));


            Collection<int> collection = new Collection<int>();
            for (int i = 0; i < count && i < bytes.Length; i++)
                collection.Add((int)(bytes[i]));
            return PSObject.AsPSObject(collection);
        }

        private PSObject AsStringZ(byte[] bytes, int length)
        {
            if (bytes.Length == 0 || length == 0)
                return PSObject.AsPSObject("");

            int index = 0;
            while (index < length && index < bytes.Length)
            {
                if (bytes[index] == 0)
                    return Encoding.ASCII.GetString(bytes, 0, index);
            }
            return Encoding.ASCII.GetString(bytes);
        }

        private PSObject AsString(byte[] bytes, int length)
        {
            if (bytes.Length == 0 || length == 0)
                return PSObject.AsPSObject("");

            if (bytes.Length < length)
                return Encoding.ASCII.GetString(bytes);

            return Encoding.ASCII.GetString(bytes, 0, length);
        }
        
        public ImageDetail(ImageDetail item)
            : base(item)
        {
        }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
