using System.Runtime.InteropServices;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Erwine.Leonard.T.GDIPlus
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Fraction32 : IEquatable<Fraction32>, IComparable<Fraction32>, IFraction<int>
    {
        #region Fields

        public static readonly Fraction32 Zero = new(0, 0, 1);

        private readonly int _wholeNumber;
        private readonly int _numerator;
        private readonly int _denominator;

        #endregion

        #region Properties

        public readonly int WholeNumber => _wholeNumber;

        readonly IConvertible IFraction.WholeNumber => _wholeNumber;

        public readonly int Numerator => _numerator;

        readonly IConvertible IFraction.Numerator => _numerator;

        public readonly int Denominator => _denominator;

        readonly IConvertible IFraction.Denominator => _denominator;

        #endregion

        #region Constructors

        public Fraction32(int wholeNumber, int numerator, int denominator)
        {
            _wholeNumber = FractionUtil.GetNormalizedRational(wholeNumber, numerator, denominator, out numerator, out denominator);
            _numerator = numerator;
            _denominator = denominator;
        }

        public Fraction32(int numerator, int denominator)
        {
            _wholeNumber = FractionUtil.GetNormalizedRational(0, numerator, denominator, out numerator, out denominator);
            _numerator = numerator;
            _denominator = denominator;
        }

        public Fraction32(IFraction fraction)
        {
            _wholeNumber = FractionUtil.GetNormalizedRational(FractionUtil.ToInt32(fraction.WholeNumber), FractionUtil.ToInt32(fraction.Numerator), FractionUtil.ToInt32(fraction.Denominator, 1), out int numerator, out int denominator);
            _numerator = numerator;
            _denominator = denominator;
        }

        public Fraction32(int wholeNumber)
        {
            _wholeNumber = wholeNumber;
            _numerator = 0;
            _denominator = 1;
        }

        #endregion

        #region *Parse
        
        public static Fraction32 Parse(string s)
        {
            int wholeNumber = FractionUtil.Parse32(s, out int numerator, out int denominator);
            return new Fraction32(wholeNumber, numerator, denominator);
            
        }

        public static bool TryParse(string s, out Fraction32 value)
        {
            if (FractionUtil.TryParse32(s, out int wholeNumber, out int numerator, out int denominator))
            {
                value = new Fraction32(wholeNumber, numerator, denominator);
                return true;
            }

            value = new Fraction32();
            return false;
        }

        #endregion

        #region Operators
        
        public static Fraction32 operator +(Fraction32 x, Fraction32 y) { return x.Add(y); }

        public static Fraction32 operator -(Fraction32 x, Fraction32 y) { return x.Subtract(y); }

        public static Fraction32 operator *(Fraction32 x, Fraction32 y) { return x.Multiply(y); }

        public static Fraction32 operator /(Fraction32 x, Fraction32 y) { return x.Divide(y); }

        public static bool operator ==(Fraction32 x, Fraction32 y) { return x.Equals(y); }

        public static bool operator !=(Fraction32 x, Fraction32 y) { return !x.Equals(y); }

        public static bool operator <(Fraction32 x, Fraction32 y) { return x.CompareTo(y) < 0; }

        public static bool operator <=(Fraction32 x, Fraction32 y) { return x.CompareTo(y) <= 0; }

        public static bool operator >(Fraction32 x, Fraction32 y) { return x.CompareTo(y) > 0; }

        public static bool operator >=(Fraction32 x, Fraction32 y) { return x.CompareTo(y) >= 0; }
        
        #endregion

        #region Add
        
        public readonly Fraction32 Add(int wholeNumber, int numerator, int denominator)
        {
            if (_numerator == 0 && _wholeNumber == 0)
                return new Fraction32(wholeNumber, numerator, denominator);

            if (numerator == 0 && wholeNumber == 0)
                return (_denominator == 0) ? Fraction32.Zero : this;

            long w1 = _wholeNumber, n1 = _numerator, d1 = _denominator, w2, n2 = numerator, d2 = denominator;
            w2 = FractionUtil.GetNormalizedRational64(wholeNumber, numerator, denominator, out n2, out d2);
            FractionUtil.ToCommonDenominator64(ref n1, ref d1, ref n2, ref d2);
            w1 = FractionUtil.GetNormalizedRational64(w1 + w2, n1 + n2, d1, out n1, out d1);
            return new Fraction32((int)w1, (int)n1, (int)d1);
        }

        readonly IFraction<int> IFraction<int>.Add(int wholeNumber, int numerator, int denominator) { return Add(wholeNumber, numerator, denominator); }

        public readonly Fraction32 Add(int numerator, int denominator) { return Add(0, numerator, denominator); }

        readonly IFraction<int> IFraction<int>.Add(int numerator, int denominator) { return Add(0, numerator, denominator); }

        public readonly Fraction32 Add(Fraction32 other)
        {
            if (_numerator == 0 && _wholeNumber == 0)
                return (other._denominator == 0) ? Fraction32.Zero : other;

            if (other._numerator == 0 && other._wholeNumber == 0)
                return (_denominator == 0) ? Fraction32.Zero : this;

            long w1 = _wholeNumber, n1 = _numerator, d1 = _denominator, w2 = other._wholeNumber, n2 = other._numerator, d2 = other._denominator;
            FractionUtil.ToCommonDenominator64(ref n1, ref d1, ref n2, ref d2);
            w1 = FractionUtil.GetNormalizedRational64(w1 + w2, n1 + n2, d1, out n1, out d1);
            return new Fraction32((int)w1, (int)n1, (int)d1);
        }

        readonly IFraction<int> IFraction<int>.Add(IFraction<int> other)
        {
            ArgumentNullException.ThrowIfNull(other);

            if (other is Fraction32)
                return Add((Fraction32)other);
            
            return Add(other.WholeNumber, other.Numerator, other.Denominator);
        }

        public readonly Fraction32 Add(int wholeNumber) { return Add(wholeNumber, 0, 1); }

        readonly IFraction<int> IFraction<int>.Add(int wholeNumber) { return Add(wholeNumber, 0, 1); }

        public readonly IFraction Add(IFraction other)
        {
            ArgumentNullException.ThrowIfNull(other);

            if (other is IFraction<int>)
                return Add((IFraction<int>)other);

            if (other.Equals(0))
                return this;

            if (_numerator == 0 && _wholeNumber == 0)
                return other;

            if (other.GetMaxUnderlyingValue().CompareTo(int.MaxValue) <= 0 && other.GetMinUnderlyingValue().CompareTo(int.MinValue) >= 0)
                return Add(Convert.ToInt32(other.WholeNumber), Convert.ToInt32(other.Numerator), Convert.ToInt32(other.Denominator));
                
            return new Fraction64(this).Add(other);
        }

        #endregion

        #region AsInverted

        public readonly Fraction32 AsInverted()
        {
            if (_numerator == 0)
            {
                if (_wholeNumber == 0)
                    return Fraction32.Zero;
                return new Fraction32(0, 1, _wholeNumber);
            }

            if (_wholeNumber == 0)
                return new Fraction32(0, _denominator, _numerator);
                
            return new Fraction32(0, _denominator, _numerator + (_wholeNumber * _denominator));
        }

        readonly IFraction<int> IFraction<int>.AsInverted() { return AsInverted(); }

        readonly IFraction IFraction.AsInverted() { return AsInverted(); }

        #endregion

        public readonly int AsRoundedValue()
        {
            if (_numerator == 0 || _numerator < (_denominator >> 1))
                return _wholeNumber;
            
            return _wholeNumber + 1;
        }

        #region CompareTo
        
        public readonly int CompareTo(Fraction32 other)
        {
            int i = _wholeNumber.CompareTo(other._wholeNumber);
            if (i != 0)
                return i;
            if (_wholeNumber == 0)
            {
                if (_numerator == 0 || other._numerator == 0 || _denominator == other._denominator)
                    return _numerator.CompareTo(other._numerator);
            }
            else
            {
                if (_numerator == 0)
                    return other._numerator;
                if (other._numerator == 0)
                    return _numerator;
            }
            
            long n1 = _numerator, d1 = _denominator, n2 = other._numerator, d2 = other._denominator;
            FractionUtil.ToCommonDenominator64(ref n1, ref d1, ref n2, ref d2);
            return n1.CompareTo(n2);
        }

        private readonly int CompareTo(IFraction<int> other)
        {
            if (other == null)
                return 1;
            
            if (other is Fraction32)
                return CompareTo((Fraction32)other);

            int w = FractionUtil.GetNormalizedRational(other.WholeNumber, other.Numerator, other.Denominator, out int n, out int d);

            int i = _wholeNumber.CompareTo(w);
            if (i != 0)
                return i;
            if (_wholeNumber == 0)
            {
                if (_numerator == 0 || n == 0 || _denominator == d)
                    return _numerator.CompareTo(n);
            }
            else
            {
                if (_numerator == 0)
                    return n;
                if (n == 0)
                    return _numerator;
            }
            
            long n1 = _numerator, d1 = _denominator, n2 = n, d2 = d;
            FractionUtil.ToCommonDenominator64(ref n1, ref d1, ref n2, ref d2);
            return n1.CompareTo(n2);
        }

        readonly int IComparable<IFraction<int>>.CompareTo(IFraction<int> other) { return CompareTo(other); }

        public readonly int CompareTo(IFraction other)
        {
            if (other == null)
                return 1;
            
            if (other is IFraction<int>)
                return CompareTo((IFraction<int>)other);
            
            if (other.GetMaxUnderlyingValue().CompareTo(int.MaxValue) <= 0 && other.GetMinUnderlyingValue().CompareTo(int.MinValue) >= 0)
                return CompareTo(new Fraction32(Convert.ToInt32(other.WholeNumber), Convert.ToInt32(other.Numerator), Convert.ToInt32(other.Denominator)));
                
            return new Fraction64(this).CompareTo(other);
        }

        public readonly int CompareTo(object obj) { return FractionUtil.Compare<int>(this, obj); }

        #endregion

        #region Divide
        
        public readonly Fraction32 Divide(int wholeNumber, int numerator, int denominator)
        {
            if (_numerator == 0 && _wholeNumber == 0)
                return Fraction32.Zero;

            if (numerator == 0 && wholeNumber == 0)
                throw new DivideByZeroException();

            long w1 = _wholeNumber, n1 = _numerator, d1 = _denominator, w2;
            w2 = FractionUtil.GetInvertedRational64(wholeNumber, numerator, denominator, out long n2, out long d2);
            
            if (n2 == 0 && w2 == 0)
                throw new DivideByZeroException();

            w1 = FractionUtil.GetNormalizedRational64(w1 * w2, n1 * n2, d1 * d2, out n1, out d1);
            return new Fraction32((int)w1, (int)n1, (int)d1);
        }

        readonly IFraction<int> IFraction<int>.Divide(int wholeNumber, int numerator, int denominator) { return Divide(wholeNumber, numerator, denominator); }

        public readonly Fraction32 Divide(int numerator, int denominator) { return Divide(0, numerator, denominator); }

        readonly IFraction<int> IFraction<int>.Divide(int numerator, int denominator) { return Divide(0, numerator, denominator); }

        public readonly Fraction32 Divide(Fraction32 other)
        {
            if (other._numerator == 0 && other._wholeNumber == 0)
                throw new DivideByZeroException();
            
            return Multiply(other.AsInverted());
        }

        readonly IFraction<int> IFraction<int>.Divide(IFraction<int> other)
        {
            ArgumentNullException.ThrowIfNull(other);

            if (other.Numerator == 0 && other.WholeNumber == 0)
                throw new DivideByZeroException();
            
            if ((other = other.AsInverted()) is Fraction32)
                return Multiply((Fraction32)other);
            
            return Multiply(other.WholeNumber, other.Numerator, other.Denominator);
        }

        public readonly IFraction Divide(IFraction other)
        {
            ArgumentNullException.ThrowIfNull(other);

            if (other is IFraction<int>)
                return Add((IFraction<int>)other);

            if (other.Equals(0))
                throw new DivideByZeroException();

            if (_numerator == 0 && _wholeNumber == 0)
                return other;

            if (other.GetMaxUnderlyingValue().CompareTo(int.MaxValue) <= 0 && other.GetMinUnderlyingValue().CompareTo(int.MinValue) >= 0)
                return Divide(Convert.ToInt32(other.WholeNumber), Convert.ToInt32(other.Numerator), Convert.ToInt32(other.Denominator));
                
            return new Fraction64(this).Divide(other);
        }

        public readonly Fraction32 Divide(int wholeNumber) { return Divide(wholeNumber, 0, 1); }

        readonly IFraction<int> IFraction<int>.Divide(int wholeNumber) { return Divide(wholeNumber, 0, 1); }

        #endregion

        #region Equals
        
        public readonly bool Equals(Fraction32 other)
        {
            if (_numerator == 0)
                return other._numerator == 0 && _wholeNumber == other._wholeNumber;
            
            return _numerator == other._numerator && _denominator == other._denominator && _wholeNumber == other._wholeNumber;
        }

        private readonly bool Equals(IFraction<int> other)
        {
            if (other == null)
                return false;
            
            if (other is Fraction32)
                return Equals((Fraction32)other);

            int w = FractionUtil.GetNormalizedRational(other.WholeNumber, other.Numerator, other.Denominator, out int n, out int d);

            if (_numerator == 0)
                return n == 0 && _wholeNumber == w;
            
            return _numerator == n && _denominator == d && _wholeNumber == w;
        }

        readonly bool IEquatable<IFraction<int>>.Equals(IFraction<int> other) { return Equals(other); }

        public readonly bool Equals(IFraction other)
        {
            if (other == null)
                return false;
            
            if (other is IFraction<int>)
                return Equals((IFraction<int>)other);
            
            if (other.GetMaxUnderlyingValue().CompareTo(int.MaxValue) <= 0 && other.GetMinUnderlyingValue().CompareTo(int.MinValue) >= 0)
                return Equals(new Fraction32(Convert.ToInt32(other.WholeNumber), Convert.ToInt32(other.Numerator), Convert.ToInt32(other.Denominator)));
                
            return new Fraction64(this).Equals(other);
        }

        public override readonly bool Equals(object obj) { return FractionUtil.EqualTo<int>(this, obj); }

        #endregion
        
        public override readonly int GetHashCode() { return ToSingle().GetHashCode(); }

        readonly IComparable IFraction.GetMinUnderlyingValue() { return int.MinValue; }

        readonly IComparable IFraction.GetMaxUnderlyingValue() { return int.MaxValue; }

        readonly TypeCode IConvertible.GetTypeCode() { return TypeCode.Double; }

        #region Multiply
        
        public readonly Fraction32 Multiply(int wholeNumber, int numerator, int denominator)
        {
            if ((_numerator == 0 && _wholeNumber == 0) || (numerator == 0 && wholeNumber == 0))
                return Fraction32.Zero;

            long w1 = _wholeNumber, n1 = _numerator, d1 = _denominator, w2, n2 = numerator, d2 = denominator;
            w2 = FractionUtil.GetNormalizedRational64(wholeNumber, numerator, denominator, out n2, out d2);
            
            if (numerator == 0 && wholeNumber == 0)
                return Fraction32.Zero;

            w1 = FractionUtil.GetNormalizedRational64(w1 * w2, n1 * n2, d1 * d2, out n1, out d1);
            return new Fraction32((int)w1, (int)n1, (int)d1);
        }

        readonly IFraction<int> IFraction<int>.Multiply(int wholeNumber, int numerator, int denominator) { return Multiply(wholeNumber, numerator, denominator); }

        public readonly Fraction32 Multiply(int numerator, int denominator) { return Multiply(0, numerator, denominator); }

        readonly IFraction<int> IFraction<int>.Multiply(int numerator, int denominator) { return Multiply(0, numerator, denominator); }

        public readonly Fraction32 Multiply(Fraction32 other)
        {
            if ((_numerator == 0 && _wholeNumber == 0) || (other._numerator == 0 && other._wholeNumber == 0))
                return Fraction32.Zero;

            long w1 = _wholeNumber, n1 = _numerator, d1 = _denominator, w2 = other._wholeNumber, n2 = other._numerator, d2 = other._denominator;
            
            w1 = FractionUtil.GetNormalizedRational64(w1 * w2, n1 * n2, d1 * d2, out n1, out d1);
            return new Fraction32((int)w1, (int)n1, (int)d1);
        }

        readonly IFraction<int> IFraction<int>.Multiply(IFraction<int> other)
        {
            ArgumentNullException.ThrowIfNull(other);

            if (other is Fraction32)
                return Multiply((Fraction32)other);
            
            return Multiply(other.WholeNumber, other.Numerator, other.Denominator);
        }

        public readonly Fraction32 Multiply(int wholeNumber) { return Multiply(wholeNumber, 0, 1); }

        readonly IFraction<int> IFraction<int>.Multiply(int wholeNumber) { return Multiply(wholeNumber, 0, 1); }

        public readonly IFraction Multiply(IFraction other)
        {
            ArgumentNullException.ThrowIfNull(other);

            if (other is IFraction<int>)
                return Add((IFraction<int>)other);

            if (_numerator == 0 && _wholeNumber == 0)
                return other;

            if (other.Equals(0))
                return Fraction32.Zero;

            if (other.GetMaxUnderlyingValue().CompareTo(int.MaxValue) <= 0 && other.GetMinUnderlyingValue().CompareTo(int.MinValue) >= 0)
                return Multiply(Convert.ToInt32(other.WholeNumber), Convert.ToInt32(other.Numerator), Convert.ToInt32(other.Denominator));
                
            return new Fraction64(this).Multiply(other);
        }

        #endregion

        #region Subtract
        
        public readonly Fraction32 Subtract(int wholeNumber, int numerator, int denominator)
        {
            if (numerator == 0 && wholeNumber == 0)
                return (_denominator == 0) ? Fraction32.Zero : this;

            long w1 = _wholeNumber, n1 = _numerator, d1 = _denominator, w2, n2 = numerator, d2 = denominator;
            w2 = FractionUtil.GetNormalizedRational64(wholeNumber, numerator, denominator, out n2, out d2);
            FractionUtil.ToCommonDenominator64(ref n1, ref d1, ref n2, ref d2);
            w1 = FractionUtil.GetNormalizedRational64(w1 + w2, n1 + n2, d1, out n1, out d1);
            return new Fraction32((int)w1, (int)n1, (int)d1);
        }

        readonly IFraction<int> IFraction<int>.Subtract(int wholeNumber, int numerator, int denominator) { return Subtract(wholeNumber, numerator, denominator); }

        public readonly Fraction32 Subtract(int numerator, int denominator) { return Subtract(0, numerator, denominator); }

        readonly IFraction<int> IFraction<int>.Subtract(int numerator, int denominator) { return Subtract(0, numerator, denominator); }

        public readonly Fraction32 Subtract(Fraction32 other)
        {
            if (other._denominator == 0)
                return Subtract(0, 0, 1);
            return Subtract(other._wholeNumber, other._numerator, other._denominator);
        }

        readonly IFraction<int> IFraction<int>.Subtract(IFraction<int> other)
        {
            ArgumentNullException.ThrowIfNull(other);

            return Subtract(other.WholeNumber, other.Numerator, other.Denominator);
        }

        public readonly Fraction32 Subtract(int wholeNumber) { return Subtract(wholeNumber, 0, 1); }

        readonly IFraction<int> IFraction<int>.Subtract(int wholeNumber) { return Subtract(wholeNumber, 0, 1); }

        public readonly IFraction Subtract(IFraction other)
        {
            ArgumentNullException.ThrowIfNull(other);

            if (other is IFraction<int>)
                return Subtract((IFraction<int>)other);

            if (other.GetMaxUnderlyingValue().CompareTo(int.MaxValue) <= 0 && other.GetMinUnderlyingValue().CompareTo(int.MinValue) >= 0)
                return Subtract(Convert.ToInt32(other.WholeNumber), Convert.ToInt32(other.Numerator), Convert.ToInt32(other.Denominator));
                
            return new Fraction64(this).Subtract(other);
        }

        #endregion

        #region To*

        readonly bool IConvertible.ToBoolean(IFormatProvider provider) { return Convert.ToBoolean(ToDouble(), provider); }

        readonly byte IConvertible.ToByte(IFormatProvider provider) { return Convert.ToByte(AsRoundedValue(), provider); }

        readonly char IConvertible.ToChar(IFormatProvider provider) { return Convert.ToChar(AsRoundedValue(), provider); }

        readonly DateTime IConvertible.ToDateTime(IFormatProvider provider) { return Convert.ToDateTime(ToDouble(), provider); }

        public readonly decimal ToDecimal()
        {
            if (_numerator == 0)
                return Convert.ToDecimal(_wholeNumber);
            return Convert.ToDecimal(_wholeNumber) + (Convert.ToDecimal(_numerator) / Convert.ToDecimal(_denominator));
        }

        readonly decimal IConvertible.ToDecimal(IFormatProvider provider) { return ToDecimal(); }

        public readonly double ToDouble()
        {
            if (_numerator == 0)
                return Convert.ToDouble(_wholeNumber);
            return Convert.ToDouble(_wholeNumber) + (Convert.ToDouble(_numerator) / Convert.ToDouble(_denominator));
        }

        readonly double IConvertible.ToDouble(IFormatProvider provider) { return ToDouble(); }

        readonly short IConvertible.ToInt16(IFormatProvider provider) { return Convert.ToInt16(AsRoundedValue(), provider); }

        readonly int IConvertible.ToInt32(IFormatProvider provider) { return Convert.ToInt32(AsRoundedValue(), provider); }

        readonly long IConvertible.ToInt64(IFormatProvider provider) { return Convert.ToInt64(AsRoundedValue(), provider); }

        readonly sbyte IConvertible.ToSByte(IFormatProvider provider) { return Convert.ToSByte(AsRoundedValue(), provider); }

        public readonly float ToSingle()
        {
            if (_numerator == 0)
                return Convert.ToSingle(_wholeNumber);
            return Convert.ToSingle(_wholeNumber) + (Convert.ToSingle(_numerator) / Convert.ToSingle(_denominator));
        }

        readonly float IConvertible.ToSingle(IFormatProvider provider) { return ToSingle(); }

        public override readonly string ToString()
        {
            if (_numerator == 0)
                return _wholeNumber.ToString();
            
            if (_wholeNumber == 0)
                return _numerator.ToString() + "/" + _denominator.ToString();
                
            return _wholeNumber.ToString() + " " + _numerator.ToString() + "/" + _denominator.ToString();
        }

        readonly string IConvertible.ToString(IFormatProvider provider) { return ToString(); }

        readonly object IConvertible.ToType(Type conversionType, IFormatProvider provider)
        {
            if (conversionType == null || conversionType.AssemblyQualifiedName == typeof(double).AssemblyQualifiedName)
                return ToDouble();
            IConvertible c = this;
            if (conversionType.AssemblyQualifiedName == typeof(float).AssemblyQualifiedName)
                c.ToSingle(provider);
            if (conversionType.AssemblyQualifiedName == typeof(int).AssemblyQualifiedName)
                c.ToInt32(provider);
            if (conversionType.AssemblyQualifiedName == typeof(string).AssemblyQualifiedName)
                c.ToString(provider);
            if (conversionType.AssemblyQualifiedName == typeof(long).AssemblyQualifiedName)
                c.ToInt64(provider);
            if (conversionType.AssemblyQualifiedName == typeof(decimal).AssemblyQualifiedName)
                c.ToDecimal(provider);
            if (conversionType.AssemblyQualifiedName == typeof(uint).AssemblyQualifiedName)
                c.ToUInt32(provider);
            if (conversionType.AssemblyQualifiedName == typeof(ulong).AssemblyQualifiedName)
                c.ToUInt64(provider);
            if (conversionType.AssemblyQualifiedName == typeof(short).AssemblyQualifiedName)
                c.ToInt16(provider);
            if (conversionType.AssemblyQualifiedName == typeof(ushort).AssemblyQualifiedName)
                c.ToUInt16(provider);
            if (conversionType.AssemblyQualifiedName == typeof(sbyte).AssemblyQualifiedName)
                c.ToSByte(provider);
            if (conversionType.AssemblyQualifiedName == typeof(byte).AssemblyQualifiedName)
                c.ToByte(provider);
            if (conversionType.AssemblyQualifiedName == typeof(DateTime).AssemblyQualifiedName)
                c.ToDateTime(provider);
            if (conversionType.AssemblyQualifiedName == typeof(bool).AssemblyQualifiedName)
                c.ToBoolean(provider);
            if (conversionType.AssemblyQualifiedName == typeof(char).AssemblyQualifiedName)
                c.ToChar(provider);
            return Convert.ChangeType(ToDouble(), conversionType);
        }

        readonly ushort IConvertible.ToUInt16(IFormatProvider provider) { return Convert.ToUInt16(AsRoundedValue(), provider); }

        readonly uint IConvertible.ToUInt32(IFormatProvider provider) { return Convert.ToUInt32(AsRoundedValue(), provider); }

        readonly ulong IConvertible.ToUInt64(IFormatProvider provider) { return Convert.ToUInt64(AsRoundedValue(), provider); }

        #endregion
    }
}