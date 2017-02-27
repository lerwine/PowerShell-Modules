using System;

namespace Erwine.Leonard.T.GDIPlus
{
	/// <summary>
	/// Represents EXIF image property types.
	/// </summary>
	/// <remarks>Many of the definitions were obtained from the document at https://msdn.microsoft.com/en-us/library/ms534416.aspx.</remarks>
    public enum ExifPropertyType
    {
		Undefined,
		BestMatching,
		Byte,
		ASCII,
		Rational,
		Long,
		Short,
		SRational,
    }
}
