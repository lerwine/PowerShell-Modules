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
        /// Determines whether the current value contains normalized values.
        /// </summary>
        bool IsNormalized { get; }

        /// <summary>
        /// Returns the current value as normalized.
        /// </summary>
        /// <returns>The current value with normalized component values.</returns>
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

        IColorModel ShiftHue(float percentage);

        IColorModel ShiftSaturation(float percentage);

        IColorModel ShiftBrightness(float percentage);
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

        new IColorModel<T> ShiftHue(float percentage);

        new IColorModel<T> ShiftSaturation(float percentage);

        new IColorModel<T> ShiftBrightness(float percentage);
    }
}