using System.Runtime.InteropServices;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Erwine.Leonard.T.GDIPlus
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    [StructLayout(LayoutKind.Explicit)]
    public struct Fraction8 : IEquatable<Fraction8>, IComparable<Fraction8>, IFraction<sbyte>
    {
        #region Fields

        public static readonly Fraction8 Zero = new(0, 0, 1);
        
        [FieldOffset(0)]
        private readonly int _hashCode;

        [FieldOffset(0)]
        private readonly sbyte _wholeNumber;

        [FieldOffset(1)]
        private readonly sbyte _numerator;
        
        [FieldOffset(2)]
        private readonly sbyte _denominator;

        #endregion

        #region Properties

        public readonly sbyte WholeNumber => _wholeNumber;

        public readonly sbyte Numerator => _numerator;

        public readonly sbyte Denominator => _denominator;

        readonly IConvertible IFraction.WholeNumber => _wholeNumber;

        readonly IConvertible IFraction.Numerator => _numerator;

        readonly IConvertible IFraction.Denominator => _denominator;

        #endregion

        #region Constructors

        public Fraction8(sbyte wholeNumber, sbyte numerator, sbyte denominator)
        {
            _hashCode = 0;
            _wholeNumber = (sbyte)FractionUtil.GetNormalizedRational(wholeNumber, numerator, denominator, out int n, out int d);
            _numerator = (sbyte)n;
            _denominator = (sbyte)d;
        }

        public Fraction8(sbyte numerator, sbyte denominator)
        {
            _hashCode = 0;
            _wholeNumber = (sbyte)FractionUtil.GetNormalizedRational(0, numerator, denominator, out int n, out int d);
            _numerator = (sbyte)n;
            _denominator = (sbyte)d;
        }

        public Fraction8(IFraction other)
        {
            _hashCode = 0;
            _wholeNumber = FractionUtil.GetNormalizedRational8(FractionUtil.ToSByte(other.WholeNumber), FractionUtil.ToSByte(other.Numerator), FractionUtil.ToSByte(other.Denominator, 1), out sbyte numerator, out sbyte denominator);
            _numerator = numerator;
            _denominator = denominator;
        }

        public Fraction8(sbyte wholeNumber)
        {
            _hashCode = 0;
            _wholeNumber = wholeNumber;
            _numerator = 0;
            _denominator = 1;
        }

        #endregion

        #region Add

        public IFraction<sbyte> Add(sbyte wholeNumber, sbyte numerator, sbyte denominator)
        {
            throw new NotImplementedException();
        }

        public IFraction<sbyte> Add(sbyte numerator, sbyte denominator)
        {
            throw new NotImplementedException();
        }

        public IFraction<sbyte> Add(IFraction<sbyte> other)
        {
            throw new NotImplementedException();
        }

        public IFraction<sbyte> Add(sbyte wholeNumber)
        {
            throw new NotImplementedException();
        }

        public IFraction Add(IFraction other)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region As*

        public IFraction<sbyte> AsInverted()
        {
            throw new NotImplementedException();
        }

        IFraction IFraction.AsInverted()
        {
            throw new NotImplementedException();
        }

        public sbyte AsRoundedValue()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region CompareTo

        public int CompareTo(Fraction8 other)
        {
            throw new NotImplementedException();
        }

        public int CompareTo(IFraction<sbyte> other)
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

        public IFraction<sbyte> Divide(sbyte wholeNumber, sbyte numerator, sbyte denominator)
        {
            throw new NotImplementedException();
        }

        public IFraction<sbyte> Divide(sbyte numerator, sbyte denominator)
        {
            throw new NotImplementedException();
        }

        public IFraction<sbyte> Divide(IFraction<sbyte> other)
        {
            throw new NotImplementedException();
        }

        public IFraction<sbyte> Divide(sbyte wholeNumber)
        {
            throw new NotImplementedException();
        }

        public IFraction Divide(IFraction other)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Equals

        public bool Equals(Fraction8 other)
        {
            throw new NotImplementedException();
        }

        public bool Equals(IFraction<sbyte> other)
        {
            throw new NotImplementedException();
        }

        public bool Equals(IFraction other)
        {
            throw new NotImplementedException();
        }

        public override bool Equals(object obj)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Get*UnderlyingValue

        public IComparable GetMaxUnderlyingValue()
        {
            throw new NotImplementedException();
        }

        public IComparable GetMinUnderlyingValue()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Multiply

        public IFraction<sbyte> Multiply(sbyte wholeNumber, sbyte numerator, sbyte denominator)
        {
            throw new NotImplementedException();
        }

        public IFraction<sbyte> Multiply(sbyte numerator, sbyte denominator)
        {
            throw new NotImplementedException();
        }

        public IFraction<sbyte> Multiply(IFraction<sbyte> other)
        {
            throw new NotImplementedException();
        }

        public IFraction<sbyte> Multiply(sbyte wholeNumber)
        {
            throw new NotImplementedException();
        }

        public IFraction Multiply(IFraction other)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Subtract

        public IFraction<sbyte> Subtract(sbyte wholeNumber, sbyte numerator, sbyte denominator)
        {
            throw new NotImplementedException();
        }

        public IFraction<sbyte> Subtract(sbyte numerator, sbyte denominator)
        {
            throw new NotImplementedException();
        }

        public IFraction<sbyte> Subtract(IFraction<sbyte> other)
        {
            throw new NotImplementedException();
        }

        public IFraction<sbyte> Subtract(sbyte wholeNumber)
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

        public override int GetHashCode() { throw new NotImplementedException(); }

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