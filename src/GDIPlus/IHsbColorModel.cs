using System;
using System.Collections.Generic;

namespace Erwine.Leonard.T.GDIPlus
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

        new IHsbColorModel<T> ShiftHue(float percentage);

        new IHsbColorModel<T> ShiftSaturation(float percentage);

        new IHsbColorModel<T> ShiftBrightness(float percentage);
    }
}