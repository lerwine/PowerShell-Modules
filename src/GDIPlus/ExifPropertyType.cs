using System;

namespace Erwine.Leonard.T.GDIPlus
{
	/// <summary>
	/// Represents EXIF image property types.
	/// </summary>
	/// <remarks>Many of the definitions were obtained from the document at https://msdn.microsoft.com/en-us/library/ms534416.aspx.</remarks>
    public enum ExifPropertyType
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        Undefined,
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// 8-bit value that can take any value depending upon the field definition.
        /// </summary>
        BestMatching,

        /// <summary>
        /// 8-bit unsigned integer
        /// </summary>
		Byte,

        /// <summary>
        /// 8-bit byte containing one 7-bit ASCII code.
        /// </summary>
		ASCII,

        /// <summary>
        /// Two consecutive 32-bit (4-byte) unsigned values, where the first value is the numerator, and teh second is the denominator.
        /// </summary>
		Rational,

        /// <summary>
        /// 32-bit (4-byte) unsigned integer.
        /// </summary>
        Long,

        /// <summary>
        /// 16-bit (2-byte) unsigned integer.
        /// </summary>
		Short,

        /// <summary>
        /// 32-bit (4-byte) signed integer.
        /// </summary>
		SLong,

        /// <summary>
        /// Two consecutive 32-bit (4-byte) signed values, where the first value is the numerator, and teh second is the denominator.
        /// </summary>
        SRational,
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        Any
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }
}
