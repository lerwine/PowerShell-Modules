using System.Runtime.InteropServices;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Erwine.Leonard.T.GDIPlus
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Fraction16 : IEquatable<Fraction16>, IComparable<Fraction16>, IFraction<short>, IConvertible
    {
        #region Fields

        public static readonly Fraction16 Zero = new(0, 0, 1);

        private readonly short _wholeNumber;
        private readonly short _numerator;
        private readonly short _denominator;

        #endregion

        #region Properties

        public readonly short WholeNumber => _wholeNumber;

        readonly IConvertible IFraction.WholeNumber => _wholeNumber;

        public readonly short Numerator => _numerator;

        readonly IConvertible IFraction.Numerator => _numerator;

        public readonly short Denominator => _denominator;

        readonly IConvertible IFraction.Denominator => _denominator;

        #endregion

        #region Constructors

        public Fraction16(short wholeNumber, short numerator, short denominator)
        {
            _wholeNumber = (short)FractionUtil.GetNormalizedRational(wholeNumber, numerator, denominator, out int n, out int d);
            _numerator = (short)n;
            _denominator = (short)d;
        }

        public Fraction16(short numerator, short denominator)
        {
            _wholeNumber = (short)FractionUtil.GetNormalizedRational(0, numerator, denominator, out int n, out int d);
            _numerator = (short)n;
            _denominator = (short)d;
        }

        public Fraction16(IFraction fraction)
        {
            _wholeNumber = (short)FractionUtil.GetNormalizedRational(FractionUtil.ToInt32(fraction.WholeNumber), FractionUtil.ToInt32(fraction.Numerator), FractionUtil.ToInt32(fraction.Denominator, 1), out int numerator, out int denominator);
            _numerator = (short)numerator;
            _denominator = (short)denominator;
        }

        public Fraction16(short wholeNumber = 0)
        {
            _wholeNumber = wholeNumber;
            _numerator = 0;
            _denominator = 1;
        }

        #endregion

        #region Add

        public IFraction<short> Add(short wholeNumber, short numerator, short denominator)
        {
            throw new NotImplementedException();
        }

        public IFraction<short> Add(short numerator, short denominator)
        {
            throw new NotImplementedException();
        }

        public IFraction<short> Add(IFraction<short> other)
        {
            throw new NotImplementedException();
        }

        public IFraction<short> Add(short wholeNumber)
        {
            throw new NotImplementedException();
        }

        public IFraction Add(IFraction other)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region As*

        public IFraction<short> AsInverted()
        {
            throw new NotImplementedException();
        }

        IFraction IFraction.AsInverted()
        {
            throw new NotImplementedException();
        }

        public short AsRoundedValue()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region CompareTo

        public int CompareTo(Fraction16 other)
        {
            throw new NotImplementedException();
        }

        public int CompareTo(IFraction<short> other)
        {
            throw new NotImplementedException();
        }

        public int CompareTo(IFraction other)
        {
            throw new NotImplementedException();
        }

        public int CompareTo(object obj)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Divide

        public IFraction<short> Divide(short wholeNumber, short numerator, short denominator)
        {
            throw new NotImplementedException();
        }

        public IFraction<short> Divide(short numerator, short denominator)
        {
            throw new NotImplementedException();
        }

        public IFraction<short> Divide(IFraction<short> other)
        {
            throw new NotImplementedException();
        }

        public IFraction<short> Divide(short wholeNumber)
        {
            throw new NotImplementedException();
        }

        public IFraction Divide(IFraction other)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Equals

        public bool Equals(Fraction16 other)
        {
            throw new NotImplementedException();
        }

        public bool Equals(IFraction<short> other)
        {
            throw new NotImplementedException();
        }

        public bool Equals(IFraction other)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Get*UnderlyingValue

        public IComparable GetMinUnderlyingValue()
        {
            throw new NotImplementedException();
        }

        public IComparable GetMaxUnderlyingValue()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Multiply

        public IFraction<short> Multiply(short wholeNumber, short numerator, short denominator)
        {
            throw new NotImplementedException();
        }

        public IFraction<short> Multiply(short numerator, short denominator)
        {
            throw new NotImplementedException();
        }

        public IFraction<short> Multiply(IFraction<short> other)
        {
            throw new NotImplementedException();
        }

        public IFraction<short> Multiply(short wholeNumber)
        {
            throw new NotImplementedException();
        }

        public IFraction Multiply(IFraction other)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Subtract

        public IFraction<short> Subtract(short wholeNumber, short numerator, short denominator)
        {
            throw new NotImplementedException();
        }

        public IFraction<short> Subtract(short numerator, short denominator)
        {
            throw new NotImplementedException();
        }

        public IFraction<short> Subtract(IFraction<short> other)
        {
            throw new NotImplementedException();
        }

        public IFraction<short> Subtract(short wholeNumber)
        {
            throw new NotImplementedException();
        }

        public IFraction Subtract(IFraction other)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region To*

        public readonly float ToSingle() { return (_denominator == 0) ? 0.0f : Convert.ToSingle(_wholeNumber) + (Convert.ToSingle(_numerator) / Convert.ToSingle(_denominator)); }
        
        public readonly double ToDouble() { return (_denominator == 0) ? 0.0 : Convert.ToDouble(_wholeNumber) + (Convert.ToDouble(_numerator) / Convert.ToDouble(_denominator)); }

        public readonly decimal ToDecimal() { return (_denominator == 0) ? 0.0M : Convert.ToDecimal(_wholeNumber) + (Convert.ToDecimal(_numerator) / Convert.ToDecimal(_denominator)); }
        
        public override readonly string ToString() { return ToString(null); }

        private readonly string ToString(IFormatProvider provider)
        {
            if (provider == null)
                provider = System.Globalization.CultureInfo.CurrentCulture;
            return (_numerator == 0 || _denominator == 0) ? _wholeNumber.ToString(provider) :
                ((_wholeNumber == 0) ? _numerator.ToString(provider) + "/" + _denominator.ToString(provider) :
                _wholeNumber.ToString(provider) + " " + _numerator.ToString(provider) + "/" + _denominator.ToString(provider));
        }

        #endregion

        #region IConvertible Explicit Implementation

        readonly TypeCode IConvertible.GetTypeCode() { return TypeCode.Double; }

        readonly bool IConvertible.ToBoolean(IFormatProvider provider)
        {
            return ((IConvertible)ToDouble()).ToBoolean(provider);
        }

        readonly char IConvertible.ToChar(IFormatProvider provider)
        {
            return ((IConvertible)ToDouble()).ToChar(provider);
        }

        readonly sbyte IConvertible.ToSByte(IFormatProvider provider)
        {
            return ((IConvertible)ToDouble()).ToSByte(provider);
        }

        readonly byte IConvertible.ToByte(IFormatProvider provider)
        {
            return ((IConvertible)ToDouble()).ToByte(provider);
        }

        readonly short IConvertible.ToInt16(IFormatProvider provider)
        {
            return ((IConvertible)ToDouble()).ToInt16(provider);
        }

        readonly ushort IConvertible.ToUInt16(IFormatProvider provider)
        {
            return ((IConvertible)ToDouble()).ToUInt16(provider);
        }

        readonly int IConvertible.ToInt32(IFormatProvider provider)
        {
            return ((IConvertible)ToDouble()).ToInt32(provider);
        }

        readonly uint IConvertible.ToUInt32(IFormatProvider provider)
        {
            return ((IConvertible)ToDouble()).ToUInt32(provider);
        }

        readonly long IConvertible.ToInt64(IFormatProvider provider)
        {
            return ((IConvertible)ToDouble()).ToInt64(provider);
        }

        readonly ulong IConvertible.ToUInt64(IFormatProvider provider)
        {
            return ((IConvertible)ToDouble()).ToUInt64(provider);
        }

        readonly float IConvertible.ToSingle(IFormatProvider provider) { return ToSingle(); }

        readonly double IConvertible.ToDouble(IFormatProvider provider) { return ToDouble(); }

        readonly decimal IConvertible.ToDecimal(IFormatProvider provider) { return ToDecimal(); }

        readonly DateTime IConvertible.ToDateTime(IFormatProvider provider)
        {
            return ((IConvertible)ToDouble()).ToDateTime(provider);
        }

        readonly string IConvertible.ToString(IFormatProvider provider) { return ToString(provider); }

        readonly object IConvertible.ToType(Type conversionType, IFormatProvider provider)
        {
            return ((IConvertible)ToDouble()).ToType(conversionType, provider);
        }

        #endregion
    }
}
