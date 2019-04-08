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

        new IRgbColorModel<T> ShiftHue(float percentage);

        new IRgbColorModel<T> ShiftSaturation(float percentage);

        new IRgbColorModel<T> ShiftBrightness(float percentage);
    }
}