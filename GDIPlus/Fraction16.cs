using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Runtime.InteropServices;
using System.Threading;

namespace Erwine.Leonard.T.GDIPlus
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Fraction16 : IEquatable<Fraction16>, IComparable<Fraction16>, IFraction<short>, IConvertible
    {
        #region Fields

        public static readonly Fraction16 Zero = new Fraction16(0, 0, 1);

        private short _wholeNumber;
        private short _numerator;
        private short _denominator;

        #endregion

        #region Properties

        public short WholeNumber { get { return _wholeNumber; } }

        IConvertible IFraction.WholeNumber { get { return _wholeNumber; } }

        public short Numerator { get { return _numerator; } }

        IConvertible IFraction.Numerator { get { return _numerator; } }

        public short Denominator { get { return _denominator; } }

        IConvertible IFraction.Denominator { get { return _denominator; } }

        #endregion

        #region Constructors
        
        public Fraction16(short wholeNumber, short numerator, short denominator)
        {
            int n, d;
            _wholeNumber = (short)(FractionUtil.GetNormalizedRational(wholeNumber, numerator, denominator, out n, out d));
            _numerator = (short)n;
            _denominator = (short)d;
        }

        public Fraction16(short numerator, short denominator)
        {
            int n, d;
            _wholeNumber = (short)(FractionUtil.GetNormalizedRational(0, numerator, denominator, out n, out d));
            _numerator = (short)n;
            _denominator = (short)d;
        }

        public Fraction16(IFraction fraction)
        {
            int numerator, denominator;
            _wholeNumber = (short)(FractionUtil.GetNormalizedRational(FractionUtil.ToInt32(fraction.WholeNumber), FractionUtil.ToInt32(fraction.Numerator), FractionUtil.ToInt32(fraction.Denominator, 1), out numerator, out denominator));
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

        public float ToSingle() { return (_denominator == 0) ? 0.0f : Convert.ToSingle(_wholeNumber) + (Convert.ToSingle(_numerator) / Convert.ToSingle(_denominator)); }
        
        public double ToDouble() { return (_denominator == 0) ? 0.0 : Convert.ToDouble(_wholeNumber) + (Convert.ToDouble(_numerator) / Convert.ToDouble(_denominator)); }

        public decimal ToDecimal() { return (_denominator == 0) ? 0.0M : Convert.ToDecimal(_wholeNumber) + (Convert.ToDecimal(_numerator) / Convert.ToDecimal(_denominator)); }
        
        public override string ToString() { return ToString(null); }

        private string ToString(IFormatProvider provider)
        {
            if (provider == null)
                provider = System.Globalization.CultureInfo.CurrentCulture;
            return (_numerator == 0 || _denominator == 0) ? _wholeNumber.ToString(provider) :
                ((_wholeNumber == 0) ? _numerator.ToString(provider) + "/" + _denominator.ToString(provider) :
                _wholeNumber.ToString(provider) + " " + _numerator.ToString(provider) + "/" + _denominator.ToString(provider));
        }

        #endregion

        #region IConvertible Explicit Implementation

        TypeCode IConvertible.GetTypeCode() { return TypeCode.Double; }

        bool IConvertible.ToBoolean(IFormatProvider provider)
        {
            return ((IConvertible)ToDouble()).ToBoolean(provider);
        }

        char IConvertible.ToChar(IFormatProvider provider)
        {
            return ((IConvertible)ToDouble()).ToChar(provider);
        }

        sbyte IConvertible.ToSByte(IFormatProvider provider)
        {
            return ((IConvertible)ToDouble()).ToSByte(provider);
        }

        byte IConvertible.ToByte(IFormatProvider provider)
        {
            return ((IConvertible)ToDouble()).ToByte(provider);
        }

        short IConvertible.ToInt16(IFormatProvider provider)
        {
            return ((IConvertible)ToDouble()).ToInt16(provider);
        }

        ushort IConvertible.ToUInt16(IFormatProvider provider)
        {
            return ((IConvertible)ToDouble()).ToUInt16(provider);
        }

        int IConvertible.ToInt32(IFormatProvider provider)
        {
            return ((IConvertible)ToDouble()).ToInt32(provider);
        }

        uint IConvertible.ToUInt32(IFormatProvider provider)
        {
            return ((IConvertible)ToDouble()).ToUInt32(provider);
        }

        long IConvertible.ToInt64(IFormatProvider provider)
        {
            return ((IConvertible)ToDouble()).ToInt64(provider);
        }

        ulong IConvertible.ToUInt64(IFormatProvider provider)
        {
            return ((IConvertible)ToDouble()).ToUInt64(provider);
        }

        float IConvertible.ToSingle(IFormatProvider provider) { return ToSingle(); }

        double IConvertible.ToDouble(IFormatProvider provider) { return ToDouble(); }

        decimal IConvertible.ToDecimal(IFormatProvider provider) { return ToDecimal(); }

        DateTime IConvertible.ToDateTime(IFormatProvider provider)
        {
            return ((IConvertible)ToDouble()).ToDateTime(provider);
        }

        string IConvertible.ToString(IFormatProvider provider) { return ToString(provider); }

        object IConvertible.ToType(Type conversionType, IFormatProvider provider)
        {
            return ((IConvertible)ToDouble()).ToType(conversionType, provider);
        }

        #endregion
    }
}
