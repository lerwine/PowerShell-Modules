using System;

namespace Erwine.Leonard.T.GDIPlus
{
	/// <summary>
	/// Represents EXIF image property types.
	/// </summary>
	/// <remarks>Many of the definitions were obtained from the documents at https://msdn.microsoft.com/en-us/library/ms534416.aspx. and https://msdn.microsoft.com/en-us/library/system.drawing.imaging.propertyitem.type.aspx.</remarks>
    public enum ExifPropertyType : short
    {
		/// <summary>
		/// Type is unknown.
		/// </summary>
		Unknown = 0,
		
		/// <summary>
		/// Single-byte value.
		/// </summary>
		Byte = 1,
		
		/// <summary>
		/// ASCII character values (1 byte each).
		/// </summary>
		ASCII = 2,
		
		/// <summary>
		/// Unsigned 16-bit integers (2 bytes each).
		/// </summary>
		Short = 3,
		
		/// <summary>
		/// Unsigned 32-bit integers (4 bytes each).
		/// </summary>
		Long = 4,
		
		/// <summary>
		/// Unsigned 32-bit rational value (8 bytes per pair).
		/// </summary>
		Rational = 5,
		
		/// <summary>
		/// Can represent any value.
		/// </summary>
		BestMatching = 6,
		
		/// <summary>
		/// Signed 32-bit integers (4 bytes each).
		/// </summary>
		SLong = 7,
		
		/// <summary>
		/// Signed 32-bit rational value (8 bytes per pair).
		/// </summary>
		SRational = 10
    }
}
