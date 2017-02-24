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
		private readonly int? _version2Value;
		private int _count = 1;
		private string _summary = "";
		private string _remarks = "";
		public ExifPropertyDetailAttribute(ExifPropertyType tagType) : this(null, tagType, null) { }
		public ExifPropertyDetailAttribute(ExifPropertyType tagType, ExifPropertyType altType) : this(null, tagType, altType) { }
		public ExifPropertyDetailAttribute(ExifPropertyType tagType, int version2Value) : this(version2Value, tagType, null) { }
		public ExifPropertyDetailAttribute(ExifPropertyType tagType, ExifPropertyType altType, int version2Value) : this(version2Value, tagType, altType) { }
		private ExifPropertyDetailAttribute(int? version2Value, ExifPropertyType tagType, ExifPropertyType? altType)
		{
			_tagType = tagType;
			_altType = altType;
			_version2Value = version2Value;
		}
		public ExifPropertyType TagType { get { return _tagType; } }
		public ExifPropertyType? AltType { get { return _altType; } }
		public int? Version2Value { get { return _version2Value; } }
		public int Count { get { return _count; } set { _count = (value < 1) ? 1 : value; } }
		public string Summary { get { return _summary; } set { _summary = (_summary == null) ? "" : value.Trim(); } }
		public string Remarks { get { return _remarks; } set { _remarks = (_remarks == null) ? "" : value.Trim(); } }
    }
}
