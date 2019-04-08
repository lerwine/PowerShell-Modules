using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Runtime.InteropServices;

namespace Erwine.Leonard.T.GDIPlus
{
    [StructLayout(LayoutKind.Explicit)]
    public class RgbColor32 : IEquatable<RgbColor32>, IEquatable<IRgbColorModel<float>>, IEquatable<IHsbColorModel<float>>, IRgbColorModel<byte>
    {
        [FieldOffset(0)]
        private readonly int _value;
        [FieldOffset(0)]
        private readonly byte _red;
        [FieldOffset(1)]
        private readonly byte _green;
        [FieldOffset(2)]
        private readonly byte _blue;
        [FieldOffset(3)]
        private readonly byte _alpha;

        /// <summary>
        /// The opaqueness of the color.
        /// </summary>
        public byte Alpha { get { return _alpha; } }

        /// <summary>
        /// The intensity of the red layer.
        /// </summary>
        public byte Red { get { return _red; } }

        /// <summary>
        /// The intensity of the green layer.
        /// </summary>
        public byte Green { get { return _green; } }

        /// <summary>
        /// The intensity of the blue layer.
        /// </summary>
        public byte Blue { get { return _blue; } }

        bool IColorModel.IsNormalized { get { return true; } }

        public RgbColor32(byte red, byte green, byte blue, byte alpha)
        {
            _value = 0;
            _red = red;
            _green = green;
            _blue = blue;
            _alpha = alpha;
        }

        public RgbColor32(byte red, byte green, byte blue) : this(red, green, blue, 255) { }

        public RgbColor32(RgbColorF value)
        {
            _red = value.Red.FromPercentage();
            _green = value.Green.FromPercentage();
            _blue = value.Blue.FromPercentage();
            _alpha = value.Alpha.FromPercentage();
        }

        public RgbColor32(IHsbColorModel<float> value)
        {
            if (value == null)
                throw new ArgumentNullException();
            if (value.Alpha < 0f || value.Alpha > 1f)
                throw new ArgumentOutOfRangeException("value", "Value for alpha is out of range");
            try
            {
                ColorExtensions.HSBtoRGB(value.Hue, value.Saturation, value.Brightness, out float r, out float g, out float b);
                _red = r.FromPercentage();
                _green = g.FromPercentage();
                _blue = b.FromPercentage();
            }
            catch (ArgumentOutOfRangeException exc) { throw new ArgumentOutOfRangeException("value", "Value for " + exc.ParamName + " is out of range"); }
            _alpha = value.Alpha.FromPercentage();
        }

        public RgbColor32(IHsbColorModel<byte> value)
        {
            if (value == null)
                throw new ArgumentNullException();
            ColorExtensions.HSBtoRGB(value.Hue.ToDegrees(), value.Saturation.ToPercentage(), value.Brightness.ToPercentage(), out float r, out float g, out float b);
            _red = r.FromPercentage();
            _green = g.FromPercentage();
            _blue = b.FromPercentage();
            _alpha = value.Alpha;
        }

        public HsbColor32Normalized AsHsb32() { return new HsbColor32Normalized(this); }

        IHsbColorModel<byte> IColorModel.AsHsb32() { return AsHsb32(); }

        public HsbColorFNormalized AsHsbF() { return new HsbColorFNormalized(this); }

        IHsbColorModel<float> IColorModel.AsHsbF() { return AsHsbF(); }

        IRgbColorModel<byte> IColorModel.AsRgb32() { return this; }

        public RgbColorF AsRgbF() { return new RgbColorF(this); }

        IRgbColorModel<float> IColorModel.AsRgbF() { return AsRgbF(); }

        IColorModel<byte> IColorModel<byte>.AsNormalized() { return this; }

        IColorModel IColorModel.AsNormalized() { return this; }

        public bool Equals(IHsbColorModel<byte> other, bool exact)
        {
            if (other == null || _alpha != other.Alpha)
                return false;
            float b;
            if (exact)
            {
                ColorExtensions.RGBtoHSB(_red.ToPercentage(), _green.ToPercentage(), _blue.ToPercentage(), out float h, out float s, out b);
                return other.Hue.ToDegrees() == h && other.Saturation.ToPercentage() == s && other.Brightness.ToPercentage() == b;
            }

            if (!other.IsNormalized)
                other = other.AsNormalized();

            ColorExtensions.HSBtoRGB(other.Hue, other.Saturation, other.Brightness, out float r, out float g, out b);
            return _red == r.FromPercentage() && _green == g.FromPercentage() && _blue == b.FromPercentage();
        }

        public bool Equals(IHsbColorModel<float> other, bool exact)
        {
            if (other == null)
                return false;
            float b;
            if (exact)
            {
                if (other.Alpha != _alpha.ToPercentage())
                    return false;
                ColorExtensions.RGBtoHSB(_red.ToPercentage(), _green.ToPercentage(), _blue.ToPercentage(), out float h, out float s, out b);
                return other.Hue == h && other.Saturation == s && other.Brightness == b;
            }

            if (other.Alpha.FromPercentage() != _alpha)
                return false;

            if (!other.IsNormalized)
                other = other.AsNormalized();

            ColorExtensions.HSBtoRGB(other.Hue, other.Saturation, other.Brightness, out float r, out float g, out b);
            return _red == r.FromPercentage() && _green == g.FromPercentage() && _blue == b.FromPercentage();
        }

        public bool Equals(IRgbColorModel<float> other, bool exact)
        {
            if (other == null)
                return false;

            if (exact)
                return _alpha.ToPercentage() == other.Alpha && _red.ToPercentage() == other.Red && _green.ToPercentage() == other.Green && _blue.ToPercentage() == other.Blue;
            return _alpha == other.Alpha.FromPercentage() && _red == other.Red.FromPercentage() && _green == other.Green.FromPercentage() && _blue == other.Blue.FromPercentage();
        }

        public bool Equals(IColorModel other, bool exact)
        {
            if (other == null)
                return false;
            if (other is RgbColor32)
                return Equals((RgbColor32)other);
            if (other is IRgbColorModel<byte>)
                return Equals((IRgbColorModel<byte>)other);
            if (other is IRgbColorModel<float>)
                return Equals((IRgbColorModel<float>)other, exact);
            if (other is IHsbColorModel<byte>)
                return Equals((IHsbColorModel<byte>)other, exact);
            return other is IHsbColorModel<float> && Equals((IHsbColorModel<float>)other, exact);
        }

        public bool Equals(RgbColor32 other) { return _value == other._value; }

        public bool Equals(IRgbColorModel<float> other) { return Equals(other, false); }

        public bool Equals(IHsbColorModel<float> other) { return Equals(other, false); }

        public bool Equals(IRgbColorModel<byte> other) { return other.Alpha == _alpha && other.Red == _red && other.Green == _green && other.Blue == _blue; }

        public bool Equals(IHsbColorModel<byte> other) { return Equals(other, false); }

        public bool Equals(IColorModel other) { return Equals(other, false); }

        public bool Equals(System.Drawing.Color other) { return other.A == _alpha && other.R == _red && other.G == _green && other.B == _blue; }

        public bool Equals(System.Windows.Media.Color other) { return other.A == _alpha && other.R == _red && other.G == _green && other.B == _blue; }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj is PSObject)
                obj = ((PSObject)obj).BaseObject;
            if (obj is RgbColor32)
                return Equals((RgbColor32)obj);
            if (obj is IRgbColorModel<byte>)
                return Equals((IRgbColorModel<byte>)obj);
            if (obj is IRgbColorModel<float>)
                return Equals((IRgbColorModel<float>)obj, false);
            if (obj is IHsbColorModel<byte>)
                return Equals((IHsbColorModel<byte>)obj, false);
            return obj is IHsbColorModel<float> && Equals((IHsbColorModel<float>)obj, false);
        }

        public override int GetHashCode() { return _value; }

        public RgbColor32 MergeAverage(IEnumerable<IRgbColorModel<byte>> other)
        {
            if (other == null)
                return this;

            double r = _red.ToPercentage(), g = _green.ToPercentage(), b = _blue.ToPercentage(), a = _alpha.ToPercentage();
            ColorExtensions.RGBtoHSB(_red.ToPercentage(), _green.ToPercentage(), _blue.ToPercentage(), out float h, out float s, out float bF);

            double brightness = bF;
            int count = 0;
            foreach (IRgbColorModel<byte> item in other)
            {
                if (item != null)
                {
                    count++;
                    r += (double)item.Red.ToPercentage();
                    g += (double)item.Green.ToPercentage();
                    b += (double)item.Blue.ToPercentage();
                    a += (double)item.Alpha.ToPercentage();
                    ColorExtensions.RGBtoHSB(item.Red.ToPercentage(), item.Green.ToPercentage(), item.Blue.ToPercentage(), out h, out s, out bF);
                    brightness += (double)bF;
                }
            }
            if (count == 0)
                return this;

            ColorExtensions.RGBtoHSB((float)(r / (double)count), (float)(r / (double)count), (float)(r / (double)count), out h, out s, out bF);
            ColorExtensions.HSBtoRGB(h, s, (float)(brightness / (double)count), out float red, out float green, out bF);
            return new RgbColor32(red.FromPercentage(), green.FromPercentage(), bF.FromPercentage(), ((float)(a / (double)count)).FromPercentage());
        }

        IRgbColorModel<byte> IRgbColorModel<byte>.MergeAverage(IEnumerable<IRgbColorModel<byte>> other) { return MergeAverage(other); }

        public RgbColor32 MergeAverage(IEnumerable<IColorModel> other)
        {
            throw new NotImplementedException();
        }

        IColorModel IColorModel.MergeAverage(IEnumerable<IColorModel> other) { return MergeAverage(other); }

        public IRgbColorModel<byte> ShiftHue(float percentage)
        {
            throw new NotImplementedException();
        }

        public IRgbColorModel<byte> ShiftSaturation(float percentage)
        {
            throw new NotImplementedException();
        }

        public IRgbColorModel<byte> ShiftBrightness(float percentage)
        {
            throw new NotImplementedException();
        }

        IColorModel<byte> IColorModel<byte>.ShiftHue(float percentage)
        {
            throw new NotImplementedException();
        }

        IColorModel<byte> IColorModel<byte>.ShiftSaturation(float percentage)
        {
            throw new NotImplementedException();
        }

        IColorModel<byte> IColorModel<byte>.ShiftBrightness(float percentage)
        {
            throw new NotImplementedException();
        }

        IColorModel IColorModel.ShiftHue(float percentage)
        {
            throw new NotImplementedException();
        }

        IColorModel IColorModel.ShiftSaturation(float percentage)
        {
            throw new NotImplementedException();
        }

        IColorModel IColorModel.ShiftBrightness(float percentage)
        {
            throw new NotImplementedException();
        }
    }
}