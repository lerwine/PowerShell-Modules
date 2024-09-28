#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Erwine.Leonard.T.GDIPlus
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    /// <summary>
    /// Represents a color as an HSB (Hue,Saturation,Brightness) color model.
    /// </summary>
    /// <typeparam name="T">The value type for the HSB componente values.</typeparam>
    public interface IHsbColorModel<T> : IEquatable<IHsbColorModel<T>>, IEquatable<IRgbColorModel<T>>, IColorModel<T>
        where T : struct, IComparable, IFormattable, IConvertible, IComparable<T>, IEquatable<T>
    {
        /// <summary>
        /// The hue of the color.
        /// </summary>
        T Hue { get; }

        /// <summary>
        /// The color saturation.
        /// </summary>
        T Saturation { get; }

        /// <summary>
        /// The brightness of the color.
        /// </summary>
        T Brightness { get; }

        new IHsbColorModel<T> AsNormalized();

        /// <summary>
        /// Merges the current <see cref="IHsbColorModel{T}"/> with other <seealso cref="IHsbColorModel{T}"/> objects by averaging average their values.
        /// </summary>
        /// <param name="other"><see cref="IHsbColorModel{T}"/> models to merge with the current.</param>
        /// <returns>A <see cref="IHsbColorModel{T}"/> model representing the average of all the meged color models.</returns>
        IHsbColorModel<T> MergeAverage(IEnumerable<IHsbColorModel<T>> other);

        /// <summary>
        /// Determines whether the current <see cref="IHsbColorModel{T}"/> is equivalent to another <see cref="IHsbColorModel{T}"/> value.
        /// </summary>
        /// <param name="other">Other <see cref="IHsbColorModel{T}"/> to compare.</param>
        /// <param name="exact"><c>true</c> to perform an exact comparison; otherwise <c>false</c> to compare in terms of the lowest precision.</param>
        /// <returns><c>true</c> if the <paramref name="other"/> is equivalent to the current <see cref="IHsbColorModel{T}"/>; otherwise, <c>false</c>.</returns>
        bool Equals(IRgbColorModel<T> other, bool exact);

        /// <summary>
        /// Returns a <see cref="IHsbColorModel{T}" /> value with the color hue adjusted.
        /// </summary>
        /// <param name="degrees">The number of degrees to shift the hue value, ranging from -360.0 to 360.0. A positive value shifts the hue in the red-to-cyan direction, and a negative value shifts the hue in the cyan-to-red direction.</param>
        /// <returns>A <see cref="IHsbColorModel{T}" /> value with the color hue adjusted.</returns>
        /// <remarks>The values 0.0, -360.0 and 360.0 have no effect since they would result in no hue change.</remarks>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="degrees" /> is less than -360.0 or <paramref name="degrees" /> is greater than 360.0.</exception>
        new IHsbColorModel<T> ShiftHue(float degrees);

        /// <summary>
        /// Returns a <see cref="IHsbColorModel{T}" /> value with the color saturation adjusted.
        /// </summary>
        /// <param name="percentage">The percentage to saturate the color, ranging from -1.0 to 1.0. A positive value increases saturation, a negative value decreases saturation and a zero vale has no effect.</param>
        /// <returns>A <see cref="IHsbColorModel{T}" /> value with the color saturation adjusted.</returns>
        /// <remarks>For positive values, the target saturation value is determined using the following formula: <c>saturation + (MAX_VALUE - saturation) * percentage</c>
        /// <para>For negative values, the target saturation value is determined using the following formula: <c>saturation + saturation * percentage</c></para></remarks>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="percentage" /> is less than -1.0 or <paramref name="percentage" /> is greater than 1.0.</exception>
        new IHsbColorModel<T> ShiftSaturation(float percentage);

        /// <summary>
        /// Returns a <see cref="IHsbColorModel{T}" /> value with the color brightness adjusted.
        /// </summary>
        /// <param name="percentage">The percentage to saturate the color, ranging from -1.0 to 1.0. A positive value increases brightness, a negative value decreases brightness and a zero vale has no effect.</param>
        /// <returns>A <see cref="IHsbColorModel{T}" /> value with the color brightness adjusted.</returns>
        /// <remarks>For positive values, the target brightness value is determined using the following formula: <c>brightness + (MAX_VALUE - brightness) * percentage</c>
        /// <para>For negative values, the target brightness value is determined using the following formula: <c>brightness + brightness * percentage</c></para></remarks>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="percentage" /> is less than -1.0 or <paramref name="percentage" /> is greater than 1.0.</exception>
        new IHsbColorModel<T> ShiftBrightness(float percentage);

        /// <summary>
        /// Gets the AHSB integer value for the current <see cref="IHsbColorModel{T}" /> value.
        /// </summary>
        /// <returns>The AHSB integer value for the current <see cref="IHsbColorModel{T}" /> value.</returns>
        int ToAHSB();
    }
}
