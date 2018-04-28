using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Runtime.InteropServices;
using System.Threading;

namespace Erwine.Leonard.T.GDIPlus
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    [StructLayout(LayoutKind.Explicit)]
    public struct Fraction8 : IEquatable<Fraction8>, IComparable<Fraction8>, IFraction<sbyte>
    {
        #region Fields

        public static readonly Fraction8 Zero = new Fraction8(0, 0, 1);
        
        [FieldOffset(0)]
        private int _hashCode;

        [FieldOffset(0)]
        private sbyte _wholeNumber;

        [FieldOffset(1)]
        private sbyte _numerator;
        
        [FieldOffset(2)]
        private sbyte _denominator;

        #endregion

        #region Properties

        public sbyte WholeNumber { get { return _wholeNumber; } }

        public sbyte Numerator { get { return _numerator; } }

        public sbyte Denominator { get { return _denominator; } }

        IConvertible IFraction.WholeNumber { get { return _wholeNumber; } }

        IConvertible IFraction.Numerator { get { return _numerator; } }

        IConvertible IFraction.Denominator { get { return _denominator; } }

        #endregion

        #region Constructors
        
        public Fraction8(sbyte wholeNumber, sbyte numerator, sbyte denominator)
        {
            int n, d;
            _hashCode = 0;
            _wholeNumber = (sbyte)(FractionUtil.GetNormalizedRational(wholeNumber, numerator, denominator, out n, out d));
            _numerator = (sbyte)n;
            _denominator = (sbyte)d;
        }

        public Fraction8(sbyte numerator, sbyte denominator)
        {
            int n, d;
            _hashCode = 0;
            _wholeNumber = (sbyte)(FractionUtil.GetNormalizedRational(0, numerator, denominator, out n, out d));
            _numerator = (sbyte)n;
            _denominator = (sbyte)d;
        }

        public Fraction8(IFraction other)
        {
            _hashCode = 0;
            sbyte numerator, denominator;
            _wholeNumber = FractionUtil.GetNormalizedRational8(FractionUtil.ToSByte(other.WholeNumber), FractionUtil.ToSByte(other.Numerator), FractionUtil.ToSByte(other.Denominator, 1), out numerator, out denominator);
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
    
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}