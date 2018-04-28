using System;

namespace Erwine.Leonard.T.GDIPlus
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public interface IFraction : IEquatable<IFraction>, IComparable<IFraction>, IComparable, IConvertible
    {
        IConvertible WholeNumber { get; }
        IConvertible Numerator { get; }
        IConvertible Denominator { get; }
        IFraction Add(IFraction other);
        IFraction Subtract(IFraction other);
        IFraction Multiply(IFraction other);
        IFraction Divide(IFraction other);
        IFraction AsInverted();
        decimal ToDecimal();
        double ToDouble();
        float ToSingle();
        IComparable GetMinUnderlyingValue();
        IComparable GetMaxUnderlyingValue();
    }

    public interface IFraction<T> : IEquatable<IFraction<T>>, IComparable<IFraction<T>>, IFraction
        where T : struct, IComparable, IFormattable, IConvertible, IComparable<T>, IEquatable<T>
    {
        new T WholeNumber { get; }
        new T Numerator { get; }
        new T Denominator { get; }

        T AsRoundedValue();
        
        IFraction<T> Add(T wholeNumber, T numerator, T denominator);
        IFraction<T> Add(T numerator, T denominator);
        IFraction<T> Add(IFraction<T> other);
        IFraction<T> Add(T wholeNumber);
        IFraction<T> Subtract(T wholeNumber, T numerator, T denominator);
        IFraction<T> Subtract(T numerator, T denominator);
        IFraction<T> Subtract(IFraction<T> other);
        IFraction<T> Subtract(T wholeNumber);
        IFraction<T> Multiply(T wholeNumber, T numerator, T denominator);
        IFraction<T> Multiply(T numerator, T denominator);
        IFraction<T> Multiply(IFraction<T> other);
        IFraction<T> Multiply(T wholeNumber);
        IFraction<T> Divide(T wholeNumber, T numerator, T denominator);
        IFraction<T> Divide(T numerator, T denominator);
        IFraction<T> Divide(IFraction<T> other);
        IFraction<T> Divide(T wholeNumber);
        new IFraction<T> AsInverted();
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}