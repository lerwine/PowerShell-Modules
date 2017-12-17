using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Runtime.InteropServices;
using System.Threading;

namespace Erwine.Leonard.T.GDIPlus
{
    [StructLayout(LayoutKind.Explicit)]
    public struct Fraction8 : IEquatable<Fraction8>, IComparable<Fraction8>, IFraction<sbyte>
    {
        public static readonly Fraction8 Zero = new Fraction8(0, 0, 1);
        
        [FieldOffset(0)]
        private int _hashCode;

        [FieldOffset(0)]
        private sbyte _wholeNumber;

        [FieldOffset(1)]
        private sbyte _numerator;
        
        [FieldOffset(2)]
        private sbyte _denominator;

        public Fraction8(IFraction other)
        {
            _hashCode = 0;
            sbyte numerator, denominator;
            _wholeNumber = FractionUtil.GetNormalizedRational8(FractionUtil.ToSByte(other.WholeNumber), FractionUtil.ToSByte(other.Numerator), FractionUtil.ToSByte(other.Denominator, 1), out numerator, out denominator);
            _numerator = numerator;
            _denominator = denominator;
        }
    }
}