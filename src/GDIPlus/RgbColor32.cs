using System;
using System.Collections.Generic;
using System.Globalization;
using System.Management.Automation;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace Erwine.Leonard.T.GDIPlus
{
    /// <summary>
    /// 
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public class RgbColor32 : IEquatable<RgbColor32>, IEquatable<IRgbColorModel<float>>, IEquatable<IHsbColorModel<float>>, IRgbColorModel<byte>, IEquatable<int>, IConvertible
    {
        #region Fields

        /// <summary>
        /// 
        /// </summary>
        public static readonly Regex ParseRegex = new Regex(@"^((?<h>[a-f\d]{3}([a-f\d]([a-f\d]{2}|[a-f\d]{4})?)?)|rgba?\(\s*((?<v>(?<r>\d+)\s*,\s*(?<g>\d+)\s*,\s*(?<b>\d+))|(?<p>(?<h>\d+(\.\d+)?)\s*,\s*(?<s>\d+(\.\d+)?)%\s*,\s*(?<l>\d+(\.\d+)?)%))(\s*,\s*(?<a>\d+(\.\d+)?%?))?\s*\))$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        
        [FieldOffset(0)]
        private readonly int _value;
        [FieldOffset(0)]
        private readonly byte _blue;
        [FieldOffset(1)]
        private readonly byte _green;
        [FieldOffset(2)]
        private readonly byte _red;
        [FieldOffset(3)]
        private readonly byte _alpha;

        #endregion
        
        #region Properties

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

        ColorStringFormat IColorModel.DefaultStringFormat { get { return ColorStringFormat.RGBAHex; } }

        #endregion
        
        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="red"></param>
        /// <param name="green"></param>
        /// <param name="blue"></param>
        /// <param name="alpha"></param>
        public RgbColor32(byte red, byte green, byte blue, byte alpha)
        {
            _value = 0;
            _red = red;
            _green = green;
            _blue = blue;
            _alpha = alpha;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="red"></param>
        /// <param name="green"></param>
        /// <param name="blue"></param>
        public RgbColor32(byte red, byte green, byte blue) : this(red, green, blue, 255) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public RgbColor32(RgbColorF value)
        {
            _red = value.Red.FromPercentage();
            _green = value.Green.FromPercentage();
            _blue = value.Blue.FromPercentage();
            _alpha = value.Alpha.FromPercentage();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="argb"></param>
        public RgbColor32(int argb)
        {
            _red = _green = _blue = _alpha = 0;
            _value = argb;
        }

        #endregion
        
        #region As* Methods

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public HsbColor32Normalized AsHsb32() { return new HsbColor32Normalized(this); }

        IHsbColorModel<byte> IColorModel.AsHsb32() { return AsHsb32(); }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public HsbColorFNormalized AsHsbF() { return new HsbColorFNormalized(this); }

        IHsbColorModel<float> IColorModel.AsHsbF() { return AsHsbF(); }

        IRgbColorModel<byte> IColorModel.AsRgb32() { return this; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public RgbColorF AsRgbF() { return new RgbColorF(this); }

        IRgbColorModel<float> IColorModel.AsRgbF() { return AsRgbF(); }

        IColorModel<byte> IColorModel<byte>.AsNormalized() { return this; }

        IColorModel IColorModel.AsNormalized() { return this; }

        #endregion
        
        #region Equals Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="exact"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="exact"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="exact"></param>
        /// <returns></returns>
        public bool Equals(IRgbColorModel<float> other, bool exact)
        {
            if (other == null)
                return false;

            if (exact)
                return _alpha.ToPercentage() == other.Alpha && _red.ToPercentage() == other.Red && _green.ToPercentage() == other.Green && _blue.ToPercentage() == other.Blue;
            return _alpha == other.Alpha.FromPercentage() && _red == other.Red.FromPercentage() && _green == other.Green.FromPercentage() && _blue == other.Blue.FromPercentage();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="exact"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(RgbColor32 other) { return _value == other._value; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(IRgbColorModel<float> other) { return Equals(other, false); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(IHsbColorModel<float> other) { return Equals(other, false); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(IRgbColorModel<byte> other) { return other.Alpha == _alpha && other.Red == _red && other.Green == _green && other.Blue == _blue; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(IHsbColorModel<byte> other) { return Equals(other, false); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(IColorModel other) { return Equals(other, false); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(System.Drawing.Color other) { return other.A == _alpha && other.R == _red && other.G == _green && other.B == _blue; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(System.Windows.Media.Color other) { return other.A == _alpha && other.R == _red && other.G == _green && other.B == _blue; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(int other) { return _value == other; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            object value = (obj is PSObject) ? ((PSObject)obj).BaseObject : obj;
            if (value is RgbColor32)
                return Equals((RgbColor32)value);
            if (value is IRgbColorModel<float>)
                return Equals((IRgbColorModel<float>)value);
            if (value is IRgbColorModel<byte>)
                return Equals((IRgbColorModel<byte>)value);
            if (value is IHsbColorModel<float>)
                return Equals((IHsbColorModel<float>)value);
            if (value is IHsbColorModel<float>)
                return Equals((IHsbColorModel<float>)value, false);
            if (value is int)
                return Equals((int)value);
            value = ColorExtensions.AsSimplestType(value);

            if (value is string)
                return (string)value == ToString();

            if (value is int)
                return ToARGB() == (int)value;

            if (value is float)
                return ToARGB() == (float)value;

            if (obj is PSObject && ColorExtensions.TryGetColor((PSObject)obj, out IColorModel color))
                return Equals(color.AsRgb32());
            return false;
        }

        #endregion

        /// <summary>
        /// Returns the hash code for this value.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode() { return _value; }

        #region MergeAverage Method

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public RgbColor32 MergeAverage(IEnumerable<IColorModel> other)
        {
            throw new NotImplementedException();
        }

        IColorModel IColorModel.MergeAverage(IEnumerable<IColorModel> other) { return MergeAverage(other); }

        #endregion
        
        #region ShiftHue Method

        /// <summary>
        /// Returns a <see cref="RgbColor32" /> value with the color hue adjusted.
        /// </summary>
        /// <param name="degrees">The number of degrees to shift the hue value, ranging from -360.0 to 360.0. A positive value shifts the hue in the red-to-cyan direction, and a negative value shifts the hue in the cyan-to-red direction.</param>
        /// <returns>A <see cref="RgbColor32" /> value with the color hue adjusted.</returns>
        /// <remarks>The values 0.0, -360.0 and 360.0 have no effect since they would result in no hue change.</remarks>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="degrees" /> is less than -360.0 or <paramref name="degrees" /> is greater than 360.0.</exception>
        public RgbColor32 ShiftHue(float degrees)
        {
            if (degrees < -360f || degrees > 360f)
                throw new ArgumentOutOfRangeException("degrees");
            if (degrees == 0f || degrees == 360f || degrees == -360f)
                return this;
            ColorExtensions.RGBtoHSB(_red.ToPercentage(), _green.ToPercentage(), _blue.ToPercentage(), out float h, out float s, out float b);
            h += degrees;
            if (h < 0f)
                h += 360f;
            else if (h >= 360f)
                h -= 360f;
            ColorExtensions.HSBtoRGB(h, s, b, out float r, out float g, out b);
            return new RgbColor32(r.FromPercentage(), g.FromPercentage(), b.FromPercentage(), _alpha);
        }

        IRgbColorModel<byte> IRgbColorModel<byte>.ShiftHue(float degrees) { return ShiftHue(degrees); }

        IColorModel<byte> IColorModel<byte>.ShiftHue(float degrees) { return ShiftHue(degrees); }

        IColorModel IColorModel.ShiftHue(float degrees) { return ShiftHue(degrees); }

        #endregion
        
        #region ShiftSaturation Method

        /// <summary>
        /// Returns a <see cref="RgbColor32" /> value with the color saturation adjusted.
        /// </summary>
        /// <param name="percentage">The percentage to saturate the color, ranging from -1.0 to 1.0. A positive value increases saturation, a negative value decreases saturation and a zero vale has no effect.</param>
        /// <returns>A <see cref="RgbColor32" /> value with the color saturation adjusted.</returns>
        /// <remarks>For positive values, the target saturation value is determined using the following formula: <c>saturation + (MAX_VALUE - saturation) * percentage</c>
        /// <para>For negative values, the target saturation value is determined using the following formula: <c>saturation + saturation * percentage</c></para></remarks>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="percentage" /> is less than -1.0 or <paramref name="percentage" /> is greater than 1.0.</exception>
        public RgbColor32 ShiftSaturation(float percentage)
        {
            if (percentage < -1f || percentage > 1f)
                throw new ArgumentOutOfRangeException("percentage");
            if (percentage == 0f)
                return this;
            ColorExtensions.RGBtoHSB(_red.ToPercentage(), _green.ToPercentage(), _blue.ToPercentage(), out float h, out float s, out float b);
            if ((percentage == 1f) ? s == 1f : percentage == -1f && s == 0f)
                return this;
            ColorExtensions.HSBtoRGB(h, s + ((percentage > 0f) ? (1f - s) : s) * percentage, b, out float r, out float g, out b);
            return new RgbColor32(r.FromPercentage(), g.FromPercentage(), b.FromPercentage(), _alpha);
        }

        IRgbColorModel<byte> IRgbColorModel<byte>.ShiftSaturation(float percentage) { return ShiftSaturation(percentage); }

        IColorModel<byte> IColorModel<byte>.ShiftSaturation(float percentage) { return ShiftSaturation(percentage); }

        IColorModel IColorModel.ShiftSaturation(float percentage) { return ShiftSaturation(percentage); }

        #endregion
        
        #region ShiftBrightness Method

        /// <summary>
        /// Returns a <see cref="RgbColor32" /> value with the color brightness adjusted.
        /// </summary>
        /// <param name="percentage">The percentage to saturate the color, ranging from -1.0 to 1.0. A positive value increases brightness, a negative value decreases brightness and a zero vale has no effect.</param>
        /// <returns>A <see cref="RgbColor32" /> value with the color brightness adjusted.</returns>
        /// <remarks>For positive values, the target brightness value is determined using the following formula: <c>brightness + (MAX_VALUE - brightness) * percentage</c>
        /// <para>For negative values, the target brightness value is determined using the following formula: <c>brightness + brightness * percentage</c></para></remarks>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="percentage" /> is less than -1.0 or <paramref name="percentage" /> is greater than 1.0.</exception>
        public RgbColor32 ShiftBrightness(float percentage)
        {
            if (percentage < -1f || percentage > 1f)
                throw new ArgumentOutOfRangeException("percentage");
            if (percentage == 0f)
                return this;
            ColorExtensions.RGBtoHSB(_red.ToPercentage(), _green.ToPercentage(), _blue.ToPercentage(), out float h, out float s, out float b);
            if ((percentage == 1f) ? b == 1f : percentage == -1f && b == 0f)
                return this;
            ColorExtensions.HSBtoRGB(h, s, b + ((percentage > 0f) ? (1f - b) : b) * percentage, out float r, out float g, out b);
            return new RgbColor32(r.FromPercentage(), g.FromPercentage(), b.FromPercentage(), _alpha);
        }

        IRgbColorModel<byte> IRgbColorModel<byte>.ShiftBrightness(float percentage) { return ShiftBrightness(percentage); }

        IColorModel<byte> IColorModel<byte>.ShiftBrightness(float percentage) { return ShiftBrightness(percentage); }

        IColorModel IColorModel.ShiftBrightness(float percentage) { return ShiftBrightness(percentage); }

        #endregion

        /// <summary>
        /// Gets the ARGB integer value for the current <see cref="RgbColor32" /> value.
        /// </summary>
        /// <returns>The ARGB integer value for the current <see cref="RgbColor32" /> value.</returns>
        public int ToARGB() { return _value; }

        #region ToString Methods

        /// <summary>
        /// Gets formatted string representing the current color value.
        /// </summary>
        /// <param name="format">The color string format to use.</param>
        /// <returns>The formatted string representing the current color value.</returns>
        public string ToString(ColorStringFormat format)
        {
            float h, s, b;
            switch (format)
            {
                case ColorStringFormat.HSLAHex:
                    ColorExtensions.RGBtoHSB(_red.ToPercentage(), _green.ToPercentage(), _blue.ToPercentage(), out h, out s, out b);
                    return HsbColor32.ToHexidecimalString(h.FromDegrees(), s.FromPercentage(), b.FromPercentage(), _alpha, false);
                case ColorStringFormat.HSLAHexOpt:
                    ColorExtensions.RGBtoHSB(_red.ToPercentage(), _green.ToPercentage(), _blue.ToPercentage(), out h, out s, out b);
                    return HsbColor32.ToHexidecimalString(h.FromDegrees(), s.FromPercentage(), b.FromPercentage(), _alpha, true);
                case ColorStringFormat.HSLAPercent:
                    ColorExtensions.RGBtoHSB(_red.ToPercentage(), _green.ToPercentage(), _blue.ToPercentage(), out h, out s, out b);
                    return HsbColorF.ToPercentParameterString(h, s, b, _alpha.ToPercentage());
                case ColorStringFormat.HSLAValues:
                    ColorExtensions.RGBtoHSB(_red.ToPercentage(), _green.ToPercentage(), _blue.ToPercentage(), out h, out s, out b);
                    return HsbColor32.ToValueParameterString(h.FromDegrees(), s.FromPercentage(), b.FromPercentage(), _alpha);
                case ColorStringFormat.HSLHex:
                    ColorExtensions.RGBtoHSB(_red.ToPercentage(), _green.ToPercentage(), _blue.ToPercentage(), out h, out s, out b);
                    return HsbColor32.ToHexidecimalString(h.FromDegrees(), s.FromPercentage(), b.FromPercentage(), false);
                case ColorStringFormat.HSLHexOpt:
                    ColorExtensions.RGBtoHSB(_red.ToPercentage(), _green.ToPercentage(), _blue.ToPercentage(), out h, out s, out b);
                    return HsbColor32.ToHexidecimalString(h.FromDegrees(), s.FromPercentage(), b.FromPercentage(), true);
                case ColorStringFormat.HSLPercent:
                    ColorExtensions.RGBtoHSB(_red.ToPercentage(), _green.ToPercentage(), _blue.ToPercentage(), out h, out s, out b);
                    return HsbColorF.ToPercentParameterString(h, s, b);
                case ColorStringFormat.HSLValues:
                    ColorExtensions.RGBtoHSB(_red.ToPercentage(), _green.ToPercentage(), _blue.ToPercentage(), out h, out s, out b);
                    return HsbColor32.ToValueParameterString(h.FromDegrees(), s.FromPercentage(), b.FromPercentage());
                case ColorStringFormat.RGBAHexOpt:
                    return ToHexidecimalString(_red, _green, _blue, _alpha, true);
                case ColorStringFormat.RGBAPercent:
                    return RgbColorF.ToPercentParameterString(_red.ToPercentage(), _green.ToPercentage(), _blue.ToPercentage(), _alpha.ToPercentage());
                case ColorStringFormat.RGBAValues:
                    return ToValueParameterString(_red, _green, _blue, _alpha);
                case ColorStringFormat.RGBHex:
                    return ToHexidecimalString(_red, _green, _blue, false);
                case ColorStringFormat.RGBHexOpt:
                    return ToHexidecimalString(_red, _green, _blue, true);
                case ColorStringFormat.RGBPercent:
                    return RgbColorF.ToPercentParameterString(_red.ToPercentage(), _green.ToPercentage(), _blue.ToPercentage());
                case ColorStringFormat.RGBValues:
                    return ToValueParameterString(_red, _green, _blue);
            }
            return _value.ToString("X8");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString() { return _value.ToString("X8"); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <param name="a"></param>
        /// <param name="canShorten"></param>
        /// <returns></returns>
        public static string ToHexidecimalString(byte r, byte g, byte b, byte a, bool canShorten)
        {
            string result = r.ToString("X2") + g.ToString("X2") + b.ToString("X2") + a.ToString("X2");
            if (canShorten && result[0] == result[1] && result[2] == result[3] && result[4] == result[5] && result[6] == result[7])
                return new string(new char[] { result[0], result[2], result[4], result[6] });
            return result;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <param name="a"></param>
        /// <returns></returns>
        public static string ToHexidecimalString(byte r, byte g, byte b, byte a) { return ToHexidecimalString(r, g, b, a, false); }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <param name="canShorten"></param>
        /// <returns></returns>
        public static string ToHexidecimalString(byte r, byte g, byte b, bool canShorten)
        {
            string result = r.ToString("X2") + g.ToString("X2") + b.ToString("X2");
            if (canShorten && result[0] == result[1] && result[2] == result[3] && result[4] == result[5])
                return new string(new char[] { result[0], result[2], result[4] });
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static string ToHexidecimalString(byte r, byte g, byte b) { return ToHexidecimalString(r, g, b, false); }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <param name="a"></param>
        /// <returns></returns>
        public static string ToValueParameterString(byte r, byte g, byte b, byte a)
        {
            return "rgba(" + r.ToString() + ", " + g.ToString() + ", " + b.ToString() + ", " + Math.Round(Convert.ToDouble(a) / 255.0, 2).ToString() + ")";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static string ToValueParameterString(byte r, byte g, byte b)
        {
            return "rgb(" + r.ToString() + ", " + g.ToString() + ", " + b.ToString() + ")";
        }

        #endregion
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool TryParse(string text, out RgbColor32 result) { return TryParse(text, false, out result); }

        internal static bool TryParse(string text, bool strict, out RgbColor32 result)
        {
            if (text != null && (text = text.Trim()).Length > 0)
            {
                Match match = ParseRegex.Match(text);
                if (match.Success)
                {
                    try
                    {
                        if (match.Groups["h"].Success)
                        {
                            switch (text.Length)
                            {
                                case 3:
                                    result = new RgbColor32(int.Parse(new string(new char[] { text[0], text[0], text[1], text[1], text[2], text[2]}), NumberStyles.HexNumber) << 8);
                                    break;
                                case 4:
                                    result = new RgbColor32(int.Parse(new string(new char[] { text[0], text[0], text[1], text[1], text[2], text[2]}), NumberStyles.HexNumber) << 8 | int.Parse(new string(new char[] { text[3], text[3] })));
                                    break;
                                case 8:
                                    result = new RgbColor32(int.Parse(text.Substring(0, 6), NumberStyles.HexNumber) << 8 | int.Parse(text.Substring(6), NumberStyles.HexNumber));
                                    break;
                                default:
                                    result = new RgbColor32(int.Parse(text, NumberStyles.HexNumber) << 8);
                                    break;
                            }
                            return true;
                        }
                        
                        float alpha = 100f;
                        if (!match.Groups["a"].Success || (((match.Groups["a"].Value.EndsWith("%")) ? (float.TryParse(match.Groups["a"].Value.Substring(0, match.Groups["a"].Length - 1), out alpha) && (alpha = alpha / 100f) <= 1f) : (float.TryParse(match.Groups["a"].Value, out alpha) && alpha <= 1f)) && alpha >= 0f))
                        {
                            if (match.Groups["v"].Success)
                            {
                                int r, g, b;
                                if (int.TryParse(match.Groups["r"].Value, out r) && r > -1 && r < 256 && int.TryParse(match.Groups["g"].Value, out g) && g > -1 && g < 256 &&
                                    int.TryParse(match.Groups["b"].Value, out b) && b > -1 && b < 256)
                                {
                                    result = new RgbColor32((byte)r, (byte)g, (byte)b, alpha.FromPercentage());
                                    return true;
                                }
                            }
                            else if (float.TryParse(match.Groups["r"].Value, out float rF) && rF >= 0f && rF <= 100f &&
                                float.TryParse(match.Groups["g"].Value, out float gF) && gF >= 0f && gF <= 100f &&
                                float.TryParse(match.Groups["b"].Value, out float bF) && bF >= 0f && bF <= 100f)
                            {
                                result = new RgbColor32((rF / 100f).FromPercentage(), (gF / 100f).FromPercentage(), (bF / 100f).FromPercentage(), alpha.FromPercentage());
                                return true;
                            }
                        }
                    }
                    catch { }
                }
                else if (!strict && HsbColor32.TryParse(text, true, out HsbColor32 hsb))
                {
                    result = hsb.AsRgb32();
                    return true;
                }
            }
            
            result = default(RgbColor32);
            return false;
        }

        #region  IConvertible Members

        TypeCode IConvertible.GetTypeCode() { return TypeCode.Int32; }
        bool IConvertible.ToBoolean(IFormatProvider provider) { return Convert.ToBoolean(_value, provider); }
        char IConvertible.ToChar(IFormatProvider provider) { return Convert.ToChar(_value, provider); }
        sbyte IConvertible.ToSByte(IFormatProvider provider) { return Convert.ToSByte(_value, provider); }
        byte IConvertible.ToByte(IFormatProvider provider) { return Convert.ToByte(_value, provider); }
        short IConvertible.ToInt16(IFormatProvider provider) { return Convert.ToInt16(_value, provider); }
        ushort IConvertible.ToUInt16(IFormatProvider provider) { return Convert.ToUInt16(_value, provider); }
        int IConvertible.ToInt32(IFormatProvider provider) { return _value; }
        uint IConvertible.ToUInt32(IFormatProvider provider) { return Convert.ToUInt32(_value, provider); }
        long IConvertible.ToInt64(IFormatProvider provider) { return _value; }
        ulong IConvertible.ToUInt64(IFormatProvider provider) { return Convert.ToUInt64(_value, provider); }
        float IConvertible.ToSingle(IFormatProvider provider) { return _value; }
        double IConvertible.ToDouble(IFormatProvider provider) { return _value; }
        decimal IConvertible.ToDecimal(IFormatProvider provider) { return _value; }
        DateTime IConvertible.ToDateTime(IFormatProvider provider) { return Convert.ToDateTime(_value, provider); }
        string IConvertible.ToString(IFormatProvider provider) { return Convert.ToString(_value, provider); }
        object IConvertible.ToType(Type conversionType, IFormatProvider provider) { return Convert.ChangeType(_value, conversionType, provider); }

        #endregion
    }
}
