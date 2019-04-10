using System;
using System.Collections.Generic;

namespace Erwine.Leonard.T.GDIPlus
{
    /// <summary>
    /// Represents a color as an RGB (Red,Green,Blue) color model.
    /// </summary>
    /// <typeparam name="T">The value type for the RGB component values.</typeparam>
    public interface IRgbColorModel<T> : IEquatable<IRgbColorModel<T>>, IEquatable<IHsbColorModel<T>>, IColorModel<T>
        where T : struct, IComparable, IFormattable, IConvertible, IComparable<T>, IEquatable<T>
    {
        /// <summary>
        /// The intensity of the red color layer.
        /// </summary>
        T Red { get; }

        /// <summary>
        /// The intensity of the green color layer.
        /// </summary>
        T Green { get; }

        /// <summary>
        /// The intensity of the blue color layer.
        /// </summary>
        T Blue { get; }
        
        /// <summary>
        /// Merges the current <see cref="IRgbColorModel{T}"/> with other <seealso cref="IRgbColorModel{T}"/> objects by averaging average their values.
        /// </summary>
        /// <param name="other"><see cref="IRgbColorModel{T}"/> models to merge with the current.</param>
        /// <returns>A <see cref="IRgbColorModel{T}"/> model representing the average of all the meged color models.</returns>
        IRgbColorModel<T> MergeAverage(IEnumerable<IRgbColorModel<T>> other);

        /// <summary>
        /// Determines whether the current <see cref="IRgbColorModel{T}"/> is equivalent to another <see cref="IRgbColorModel{T}"/> value.
        /// </summary>
        /// <param name="other">Other <see cref="IRgbColorModel{T}"/> to compare.</param>
        /// <param name="exact"><c>true</c> to perform an exact comparison; otherwise <c>false</c> to compare in terms of the lowest precision.</param>
        /// <returns><c>true</c> if the <paramref name="other"/> is equivalent to the current <see cref="IRgbColorModel{T}"/>; otherwise, <c>false</c>.</returns>
        bool Equals(IHsbColorModel<T> other, bool exact);

        /// <summary>
        /// Returns a <see cref="IRgbColorModel{T}" /> value with the color hue adjusted.
        /// </summary>
        /// <param name="degrees">The number of degrees to shift the hue value, ranging from -360.0 to 360.0. A positive value shifts the hue in the red-to-cyan direction, and a negative value shifts the hue in the cyan-to-red direction.</param>
        /// <returns>A <see cref="IRgbColorModel{T}" /> value with the color hue adjusted.</returns>
        /// <remarks>The values 0.0, -360.0 and 360.0 have no effect since they would result in no hue change.</remarks>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="degrees" /> is less than -360.0 or <paramref name="degrees" /> is greater than 360.0.</exception>
        new IRgbColorModel<T> ShiftHue(float degrees);

        /// <summary>
        /// Returns a <see cref="IRgbColorModel{T}" /> value with the color saturation adjusted.
        /// </summary>
        /// <param name="percentage">The percentage to saturate the color, ranging from -1.0 to 1.0. A positive value increases saturation, a negative value decreases saturation and a zero vale has no effect.</param>
        /// <returns>A <see cref="IRgbColorModel{T}" /> value with the color saturation adjusted.</returns>
        /// <remarks>For positive values, the target saturation value is determined using the following formula: <c>saturation + (MAX_VALUE - saturation) * percentage</c>
        /// <para>For negative values, the target saturation value is determined using the following formula: <c>saturation + saturation * percentage</c></para></remarks>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="percentage" /> is less than -1.0 or <paramref name="percentage" /> is greater than 1.0.</exception>
        new IRgbColorModel<T> ShiftSaturation(float percentage);

        /// <summary>
        /// Returns a <see cref="IRgbColorModel{T}" /> value with the color brightness adjusted.
        /// </summary>
        /// <param name="percentage">The percentage to saturate the color, ranging from -1.0 to 1.0. A positive value increases brightness, a negative value decreases brightness and a zero vale has no effect.</param>
        /// <returns>A <see cref="IRgbColorModel{T}" /> value with the color brightness adjusted.</returns>
        /// <remarks>For positive values, the target brightness value is determined using the following formula: <c>brightness + (MAX_VALUE - brightness) * percentage</c>
        /// <para>For negative values, the target brightness value is determined using the following formula: <c>brightness + brightness * percentage</c></para></remarks>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="percentage" /> is less than -1.0 or <paramref name="percentage" /> is greater than 1.0.</exception>
        new IRgbColorModel<T> ShiftBrightness(float percentage);

        /// <summary>
        /// Gets the ARGB integer value for the current <see cref="IRgbColorModel{T}" /> value.
        /// </summary>
        /// <returns>The ARGB integer value for the current <see cref="IRgbColorModel{T}" /> value.</returns>
        int ToARGB();
    }
}
