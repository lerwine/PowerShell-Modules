using System;
using System.Collections.Generic;

namespace Erwine.Leonard.T.GDIPlus
{
    /// <summary>
    /// Interface for models representing a specific color.
    /// </summary>
    public interface IColorModel : IEquatable<IColorModel>, IEquatable<System.Drawing.Color>, IEquatable<System.Windows.Media.Color>
    {
        /// <summary>
        /// Determines whether the current value contains normalized values.
        /// </summary>
        bool IsNormalized { get; }

        /// <summary>
        /// Gets the default formatted string format.
        /// </summary>
        ColorStringFormat DefaultStringFormat { get; }

        /// <summary>
        /// Returns the current <see cref="IColorModel"/> as a <seealso cref="HsbColor32"/> value.
        /// </summary>
        /// <returns>A <seealso cref="HsbColor32"/> value that represents the same color as the current <seealso cref="IColorModel"/>.</returns>
        IHsbColorModel<byte> AsHsb32();

        /// <summary>
        /// Returns the current <see cref="IColorModel"/> as a <seealso cref="HsbColorF"/> value.
        /// </summary>
        /// <returns>A <seealso cref="HsbColorF"/> value that represents the same color as the current <seealso cref="IColorModel"/>.</returns>
        IHsbColorModel<float> AsHsbF();

        /// <summary>
        /// Returns the current <see cref="IColorModel"/> as a <seealso cref="RgbColor32"/> value.
        /// </summary>
        /// <returns>A <seealso cref="RgbColor32"/> value that represents the same color as the current <seealso cref="IColorModel"/>.</returns>
        IRgbColorModel<byte> AsRgb32();

        /// <summary>
        /// Returns the current <see cref="IColorModel"/> as a <seealso cref="RgbColorF"/> value.
        /// </summary>
        /// <returns>A <seealso cref="RgbColorF"/> value that represents the same color as the current <seealso cref="IColorModel"/>.</returns>
        IRgbColorModel<float> AsRgbF();

        /// <summary>
        /// Returns the current value as a value with its component values normalized for accurate comparison operations.
        /// </summary>
        /// <returns>The current value as a value with its component values normalized for accurate comparison operations.</returns>
        IColorModel AsNormalized();

        /// <summary>
        /// Merges the current <see cref="IColorModel"/> with other <seealso cref="IColorModel"/> objects by averaging average their values.
        /// </summary>
        /// <param name="other"><see cref="IColorModel"/> models to merge with the current.</param>
        /// <returns>A <see cref="IColorModel"/> model representing the average of all the meged color models.</returns>
        IColorModel MergeAverage(IEnumerable<IColorModel> other);

        /// <summary>
        /// Determines whether the current <see cref="IColorModel"/> is equivalent to another <see cref="IColorModel"/> value.
        /// </summary>
        /// <param name="other">Other <see cref="IColorModel"/> to compare.</param>
        /// <param name="exact"><c>true</c> to perform an exact comparison; otherwise <c>false</c> to compare in terms of the lowest precision.</param>
        /// <returns><c>true</c> if the <paramref name="other"/> is equivalent to the current <see cref="IColorModel"/>; otherwise, <c>false</c>.</returns>
        bool Equals(IColorModel other, bool exact);

        /// <summary>
        /// Returns a <see cref="IColorModel" /> value with the color hue adjusted.
        /// </summary>
        /// <param name="degrees">The number of degrees to shift the hue value, ranging from -360.0 to 360.0. A positive value shifts the hue in the red-to-cyan direction, and a negative value shifts the hue in the cyan-to-red direction.</param>
        /// <returns>A <see cref="IColorModel" /> value with the color hue adjusted.</returns>
        /// <remarks>The values 0.0, -360.0 and 360.0 have no effect since they would result in no hue change.</remarks>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="degrees" /> is less than -360.0 or <paramref name="degrees" /> is greater than 360.0.</exception>
        IColorModel ShiftHue(float degrees);

        /// <summary>
        /// Returns a <see cref="IColorModel" /> value with the color saturation adjusted.
        /// </summary>
        /// <param name="percentage">The percentage to saturate the color, ranging from -1.0 to 1.0. A positive value increases saturation, a negative value decreases saturation and a zero vale has no effect.</param>
        /// <returns>A <see cref="IColorModel" /> value with the color saturation adjusted.</returns>
        /// <remarks>For positive values, the target saturation value is determined using the following formula: <c>saturation + (MAX_VALUE - saturation) * percentage</c>
        /// <para>For negative values, the target saturation value is determined using the following formula: <c>saturation + saturation * percentage</c></para></remarks>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="percentage" /> is less than -1.0 or <paramref name="percentage" /> is greater than 1.0.</exception>
        IColorModel ShiftSaturation(float percentage);

        /// <summary>
        /// Returns a <see cref="IColorModel" /> value with the color brightness adjusted.
        /// </summary>
        /// <param name="percentage">The percentage to saturate the color, ranging from -1.0 to 1.0. A positive value increases brightness, a negative value decreases brightness and a zero vale has no effect.</param>
        /// <returns>A <see cref="IColorModel" /> value with the color brightness adjusted.</returns>
        /// <remarks>For positive values, the target brightness value is determined using the following formula: <c>brightness + (MAX_VALUE - brightness) * percentage</c>
        /// <para>For negative values, the target brightness value is determined using the following formula: <c>brightness + brightness * percentage</c></para></remarks>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="percentage" /> is less than -1.0 or <paramref name="percentage" /> is greater than 1.0.</exception>
        IColorModel ShiftBrightness(float percentage);

        /// <summary>
        /// Gets formatted string representing the current color value.
        /// </summary>
        /// <param name="format">The color string format to use.</param>
        /// <returns>The formatted string representing the current color value.</returns>
        string ToString(ColorStringFormat format);
    }

    /// <summary>
    /// Interface for models representing a specific color.
    /// </summary>
    /// <typeparam name="T">The component value type.</typeparam>
    public interface IColorModel<T> : IColorModel
        where T : struct, IComparable, IFormattable, IConvertible, IComparable<T>, IEquatable<T>
    {
        /// <summary>
        /// The opaqueness of the color.
        /// </summary>
        T Alpha { get; }

        /// <summary>
        /// Returns the current value as normalized.
        /// </summary>
        /// <returns>The current value with normalized component values.</returns>
        new IColorModel<T> AsNormalized();

        /// <summary>
        /// Returns a <see cref="IColorModel{T}" /> value with the color hue adjusted.
        /// </summary>
        /// <param name="degrees">The number of degrees to shift the hue value, ranging from -360.0 to 360.0. A positive value shifts the hue in the red-to-cyan direction, and a negative value shifts the hue in the cyan-to-red direction.</param>
        /// <returns>A <see cref="IColorModel{T}" /> value with the color hue adjusted.</returns>
        /// <remarks>The values 0.0, -360.0 and 360.0 have no effect since they would result in no hue change.</remarks>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="degrees" /> is less than -360.0 or <paramref name="degrees" /> is greater than 360.0.</exception>
        new IColorModel<T> ShiftHue(float degrees);

        /// <summary>
        /// Returns a <see cref="IColorModel{T}" /> value with the color saturation adjusted.
        /// </summary>
        /// <param name="percentage">The percentage to saturate the color, ranging from -1.0 to 1.0. A positive value increases saturation, a negative value decreases saturation and a zero vale has no effect.</param>
        /// <returns>A <see cref="IColorModel{T}" /> value with the color saturation adjusted.</returns>
        /// <remarks>For positive values, the target saturation value is determined using the following formula: <c>saturation + (MAX_VALUE - saturation) * percentage</c>
        /// <para>For negative values, the target saturation value is determined using the following formula: <c>saturation + saturation * percentage</c></para></remarks>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="percentage" /> is less than -1.0 or <paramref name="percentage" /> is greater than 1.0.</exception>
        new IColorModel<T> ShiftSaturation(float percentage);

        /// <summary>
        /// Returns a <see cref="IColorModel{T}" /> value with the color brightness adjusted.
        /// </summary>
        /// <param name="percentage">The percentage to saturate the color, ranging from -1.0 to 1.0. A positive value increases brightness, a negative value decreases brightness and a zero vale has no effect.</param>
        /// <returns>A <see cref="IColorModel{T}" /> value with the color brightness adjusted.</returns>
        /// <remarks>For positive values, the target brightness value is determined using the following formula: <c>brightness + (MAX_VALUE - brightness) * percentage</c>
        /// <para>For negative values, the target brightness value is determined using the following formula: <c>brightness + brightness * percentage</c></para></remarks>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="percentage" /> is less than -1.0 or <paramref name="percentage" /> is greater than 1.0.</exception>
        new IColorModel<T> ShiftBrightness(float percentage);
    }
}
