using System;

namespace Erwine.Leonard.T.GDIPlus
{
	/// <summary>
	/// Assigned details of EXIF image property tag information for <seealso cref="ExifPropertyTag" /> values.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public sealed class ExifPropertyDetailAttribute : Attribute
    {
		private readonly ExifPropertyType _tagType;
		private readonly ExifPropertyType? _altType;
		private readonly bool _isNullTerminatedString;
		private int _count = 1;
		private string _summary = "";
		private string _remarks = "";
		public ExifPropertyDetailAttribute(ExifPropertyType tagType) : this((tagType == ExifPropertyType.ASCII), tagType, null) { }
		public ExifPropertyDetailAttribute(ExifPropertyType tagType, ExifPropertyType altType) : this((tagType == ExifPropertyType.ASCII), tagType, altType) { }
		public ExifPropertyDetailAttribute(bool isNullTerminatedString) : this(isNullTerminatedString, ExifPropertyType.ASCII, null) { }
		public ExifPropertyDetailAttribute(bool isNullTerminatedString, ExifPropertyType altType) : this(isNullTerminatedString, ExifPropertyType.ASCII, altType) { }
		private ExifPropertyDetailAttribute(bool isNullTerminatedString, ExifPropertyType tagType, ExifPropertyType? altType)
		{
			_tagType = tagType;
			_altType = altType;
			_isNullTerminatedString = isNullTerminatedString;
		}
		private ExifPropertyDetailAttribute(ExifPropertyDetailAttribute copyFrom, ExifPropertyType tagType)
			: this((tagType == ExifPropertyType.ASCII), tagType, null)
		{
			Count = copyFrom.Count;
			Summary = copyFrom.Summary;
			Remarks = copyFrom.Remarks;
			if (tagType != copyFrom.TagType && !copyFrom.HasValue)
				_altType = copyFrom.TagType;
			else if (copyFrom.AltType.HasValue && copyFrom.AltType.Value != tagType)
				_altType = copyFrom.AltType;
		}
		public ExifPropertyType TagType { get { return _tagType; } }
		public ExifPropertyType? AltType { get { return _altType; } }
		public int IsNullTerminatedString { get { return _isNullTerminatedString; } }
		public int Count { get { return _count; } set { _count = (value < 1) ? 1 : value; } }
		public string Summary { get { return _summary; } set { _summary = (_summary == null) ? "" : value.Trim(); } }
		public string Remarks { get { return _remarks; } set { _remarks = (_remarks == null) ? "" : value.Trim(); } }
		public static ExifPropertyDetailAttribute GetExifPropertyDetail(ExifPropertyTag tag, ExifPropertyType tagType)
		{
			Type t = tag.GetType();
			string n = Enum.GetName(t, tag);
			ExifPropertyDetailAttribute attribute = t.GetField(n).GetCustomAttribute(typeof(ExifPropertyDetailAttribute), false).OfType<ExifPropertyDetailAttribute>().FirstOrDefault();
			if (attribute == null)
				return new ExifPropertyDetailAttribute((tagType == ExifPropertyType.ASCII), tagType, null);
			if (attribute.TagType == tagType || tagType == ExifPropertyType.Unknown || tagType == ExifPropertyType.BestMatching)
				return attribute;
			return new ExifPropertyDetailAttribute(attribute, tagType);
		}
    }
}
