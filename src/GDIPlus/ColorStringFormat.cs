using System;

namespace Erwine.Leonard.T.GDIPlus
{
    /// <summary>
    /// Represents a color string format.
    /// </summary>
    public enum ColorStringFormat
    {
        /// <summary>
        /// Format contains 3 pairs of hexidecimal string values representing the Red, Green and Blue color values respectively.
        /// </summary>
        RGBHex,

        /// <summary>
        /// Format contains 3 single-character hexidecimal string values or 3 pairs of hexidecimal string values representing the Red, Green and Blue color values respectively.
        /// </summary>
        /// <remarks>This is the same as <seealso cref="RGBHex" />, except that if all 3 pairs are the same character repeated twice, then it is shortened to the first character of each pair.</remarks>
        RGBHexOpt,

        /// <summary>
        /// Format contains 4 pairs of hexidecimal string values representing the Red, Green, Blue and Alpha Layer (transparency) color values respectively.
        /// </summary>
        RGBAHex,

        
        /// <summary>
        /// Format contains 4 single-character hexidecimal string values or 4pairs of hexidecimal string values representing the Red, Green, Blue and Alpha Layer (transparency) color values respectively.
        /// </summary>
        /// <remarks>This is the same as <seealso cref="RGBAHex" />, except that if all 4 pairs are the same character repeated twice, then it is shortened to the first character of each pair.</remarks>
        RGBAHexOpt,

        /// <summary>
        /// Format contains 3 pairs of hexidecimal string values representing the Hue, Saturation and Lightness color values respectively.
        /// </summary>
        HSLHex,
        
        /// <summary>
        /// Format contains 3 single-character hexidecimal string values or 3 pairs of hexidecimal string values representing the Hue, Saturation and Lightness color values respectively.
        /// </summary>
        /// <remarks>This is the same as <seealso cref="HSLHex" />, except that if all 3 pairs are the same character repeated twice, then it is shortened to the first character of each pair.</remarks>
        HSLHexOpt,
        
        /// <summary>
        /// Format contains 4 pairs of hexidecimal string values representing the Hue, Saturation, Lightness and Alpha Layer (transparency) color values respectively.
        /// </summary>
        HSLAHex,
        
        /// <summary>
        /// Format contains 4 single-character hexidecimal string values or 4pairs of hexidecimal string values representing the Hue, Saturation, Lightness and Alpha Layer (transparency) color values respectively.
        /// </summary>
        /// <remarks>This is the same as <seealso cref="HSLAHex" />, except that if all 4 pairs are the same character repeated twice, then it is shortened to the first character of each pair.</remarks>
        HSLAHexOpt,
        
        /// <summary>
        /// Uses format &quot;rgb(<c>[0-255]</c>, <c>[0-255]</c>, <c>[0-255]</c>)&quot; where the numbers within the parenthesis represent the Red, Green and Blue color values, respectively.
        /// </summary>
        RGBValues,
        
        /// <summary>
        /// Uses format &quot;rgba(<c>[0-255]</c>, <c>[0-255]</c>, <c>[0-255]</c>, <c>[0.0-1.0]</c>)&quot; where the numbers within the parenthesis represent the Red, Green, Blue and Alpha Layer (transparency) color values, respectively.
        /// </summary>
        RGBAValues,
        
        /// <summary>
        /// Uses format &quot;rgb(<c>[0-255]</c>, <c>[0-255]</c>, <c>[0-255]</c>)&quot; where the numbers within the parenthesis represent the Hue, Saturation and Lightness color values, respectively.
        /// </summary>
        HSLValues,
        
        /// <summary>
        /// Uses format &quot;rgba(<c>[0-255]</c>, <c>[0-255]</c>, <c>[0-255]</c>, <c>[0.0-1.0]</c>)&quot; where the numbers within the parenthesis represent the Hue, Saturation, Lightness and Alpha Layer (transparency) color values, respectively.
        /// </summary>
        HSLAValues,
        
        /// <summary>
        /// Uses format &quot;rgb(<c>[0-100]</c>%, <c>[0-100]</c>%, <c>[0-100]</c>%)&quot; where the numbers within the parenthesis represent the Red, Green and Blue color values, respectively.
        /// </summary>
        RGBPercent,
        
        /// <summary>
        /// Uses format &quot;rgba(<c>[0-100]</c>%, <c>[0-100]</c>%, <c>[0-100]</c>%, <c>[0-100]</c>%)&quot; where the numbers within the parenthesis represent the Red, Green, Blue and Alpha Layer (transparency) color values, respectively.
        /// </summary>
        RGBAPercent,
        
        /// <summary>
        /// Uses format &quot;hsl(<c>[0-100]</c>%, <c>[0-100]</c>%, <c>[0-100]</c>%)&quot; where the numbers within the parenthesis represent the Hue, Saturation and Lightness color values, respectively.
        /// </summary>
        HSLPercent,
        
        /// <summary>
        /// Uses format &quot;hsla(<c>[0-100]</c>%, <c>[0-100]</c>%, <c>[0-100]</c>%, <c>[0-100]</c>%)&quot; where the numbers within the parenthesis represent the Hue, Saturation, Lightness and Alpha Layer (transparency) color values, respectively.
        /// </summary>
        HSLAPercent
    }
}
