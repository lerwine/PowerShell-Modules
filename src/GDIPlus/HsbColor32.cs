using System;
using System.Collections.Generic;
using System.Globalization;
using System.Management.Automation;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace Erwine.Leonard.T.GDIPlus
{
    /// <summary>
    /// An HSB (Hue,Saturation,Brightness) color model using 32 bits.
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public struct HsbColor32 : IEquatable<HsbColor32>, IEquatable<IHsbColorModel<float>>, IEquatable<IRgbColorModel<float>>, IHsbColorModel<byte>, IEquatable<int>, IConvertible
    {
        #region Fields

        /// <summary>
        /// 
        /// </summary>
        public static readonly Regex ParseRegex = new Regex(@"^((?<h>[a-f\d]{3}([a-f\d]([a-f\d]{2}|[a-f\d]{4})?)?)|hs[lb]a?\(\s*((?<b>(?<h>\d+)\s*,\s*(?<s>\d+)\s*,\s*(?<l>\d+))|(?<p>(?<h>\d+(\.\d+)?)\s*,\s*(?<s>\d+(\.\d+)?)%\s*,\s*(?<l>\d+(\.\d+)?)%))(\s*,\s*(?<a>\d+(\.\d+)?%?))?\s*\))$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        [FieldOffset(0)]
        private readonly int _value;
        [FieldOffset(0)]
        private readonly byte _brightness;
        [FieldOffset(1)]
        private readonly byte _saturation;
        [FieldOffset(2)]
        private readonly byte _hue;
        [FieldOffset(3)]
        private readonly byte _alpha;

        #endregion
        
        #region Properties

        /// <summary>
        /// The opaqueness of the color.
        /// </summary>
        public byte Alpha { get { return _alpha; } }

        /// <summary>
        /// The hue of the color.
        /// </summary>
        public byte Hue { get { return _hue; } }

        /// <summary>
        /// The color saturation.
        /// </summary>
        public byte Saturation { get { return _saturation; } }

        /// <summary>
        /// The brightness of the color.
        /// </summary>
        public byte Brightness { get { return _brightness; } }

        bool IColorModel.IsNormalized { get { return false; } }

        ColorStringFormat IColorModel.DefaultStringFormat { get { return ColorStringFormat.HSLAHex; } }

        #endregion
        
        #region Constructors

        /// <summary>
        /// Creates a new <see cref="HsbColor32" /> from HSB values and specified opaqueness.
        /// </summary>
        /// <param name="hue">Color hue in degrees, ranging from 0.0 to 360.0.</param>
        /// <param name="saturation">Color saturation in pecentage, ranging from 0.0 to 1.0, where 0.0 is grayscale.</param>
        /// <param name="brightness">Color brightness in percentage, ranging from 0.0 to 1.0, where 0.0 is completely dark (black) and 1.0 is completely light (white).</param>
        /// <param name="alpha">Color opaqueness (alpha layer) in percentage, where 0.0 is transparent and 1.0 is opaque.</param>
        public HsbColor32(byte hue, byte saturation, byte brightness, byte alpha)
        {
            _value = 0;
            _hue = hue;
            _brightness = saturation;
            _saturation = brightness;
            _alpha = alpha;
        }

        /// <summary>
        /// Creates a new <see cref="HsbColor32" /> from HSB values with no transparency.
        /// </summary>
        /// <param name="hue">Color hue in degrees, ranging from 0.0 to 360.0.</param>
        /// <param name="saturation">Color saturation in pecentage, ranging from 0.0 to 1.0, where 0.0 is grayscale.</param>
        /// <param name="brightness">Color brightness in percentage, ranging from 0.0 to 1.0, where 0.0 is completely dark (black) and 1.0 is completely light (white).</param>
        public HsbColor32(byte hue, byte saturation, byte brightness) : this(hue, saturation, brightness, 255) { }

        /// <summary>
        /// Creates a new <see cref="HsbColor32" /> from a <seealso cref="HsbColorF" /> value.
        /// </summary>
        /// <param name="value"></param>
        public HsbColor32(HsbColorF value)
        {
            _value = 0;
            _hue = value.Hue.FromDegrees();
            _saturation = value.Saturation.FromPercentage();
            _brightness = value.Brightness.FromPercentage();
            _alpha = value.Alpha.FromPercentage();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ahsb"></param>
        public HsbColor32(int ahsb)
        {
            _brightness = _saturation = _hue = _alpha = 0;
            _value = ahsb;
        }

        #endregion
        
        #region As* Methods

        IHsbColorModel<byte> IColorModel.AsHsb32() { return this; }

        /// <summary>
        /// Returns a <seealso cref="HsbColorF" /> value representing the same color as the current <see cref="HsbColor32" />.
        /// </summary>
        /// <returns>A <seealso cref="HsbColorF" /> value representing the same color as the current <see cref="HsbColor32" />.</returns>
        public HsbColorF AsHsbF() { return new HsbColorF(this); }

        IHsbColorModel<float> IColorModel.AsHsbF() { return AsHsbF(); }

        /// <summary>
        /// Returns a <seealso cref="RgbColor32" /> value representing the same color as the current <see cref="HsbColor32" />.
        /// </summary>
        /// <returns>A <seealso cref="RgbColor32" /> value representing the same color as the current <see cref="HsbColor32" />.</returns>
        public RgbColor32 AsRgb32() { return new RgbColor32(this); }

        IRgbColorModel<byte> IColorModel.AsRgb32() { return AsRgb32(); }

        /// <summary>
        /// Returns a <seealso cref="RgbColorF" /> value representing the same color as the current <see cref="HsbColor32" />.
        /// </summary>
        /// <returns>A <seealso cref="RgbColorF" /> value representing the same color as the current <see cref="HsbColor32" />.</returns>
        public RgbColorF AsRgbF() { return new RgbColorF(this); }

        IRgbColorModel<float> IColorModel.AsRgbF() { return AsRgbF(); }

        /// <summary>
        /// Returns a <seealso cref="HsbColor32Normalized" /> value representing the same color as the current <see cref="HsbColor32" /> with the color values normalized for accurate comparisons.
        /// </summary>
        /// <returns>A <seealso cref="RgbColorF" /> value representing the same color as the current <see cref="HsbColor32" /> with the color values normalized for accurate comparisons.</returns>
        public HsbColor32Normalized AsNormalized() { return new HsbColor32Normalized(this); }

        IHsbColorModel<byte> IHsbColorModel<byte>.AsNormalized() { return AsNormalized(); }

        IColorModel<byte> IColorModel<byte>.AsNormalized() { return AsNormalized(); }

        IColorModel IColorModel.AsNormalized() { return AsNormalized(); }

        #endregion
        
        #region Equals Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="exact"></param>
        /// <returns></returns>
        public bool Equals(IRgbColorModel<byte> other, bool exact)
        {
            if (other == null || _alpha != other.Alpha)
                return false;
            float b;
            if (exact)
            {
                ColorExtensions.RGBtoHSB(other.Red, other.Green, other.Blue, out float h, out float s, out b);
                return _hue.ToDegrees() == h && _saturation.ToDegrees() == s && _brightness.ToDegrees() == b;
            }
            ColorExtensions.HSBtoRGB(_hue.ToDegrees(), _saturation.ToPercentage(), _brightness.ToPercentage(), out float r, out float g, out b);
            return other.Red == r.FromPercentage() && other.Green == g.FromPercentage() && other.Blue == b.FromPercentage();
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
            {
                if (other.Alpha != _alpha.ToPercentage())
                    return false;
                ColorExtensions.RGBtoHSB(other.Red, other.Green, other.Blue, out float h, out float s, out float b);
                return _hue.ToDegrees() == h && _saturation.ToDegrees() == s && _brightness.ToDegrees() == b;
            }

            return AsNormalized().Equals(other, false);
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
            if (exact)
                return _alpha.ToPercentage() == other.Alpha && _hue.ToDegrees() == other.Hue && _saturation.ToPercentage() == other.Saturation && _brightness.ToPercentage() == other.Brightness;
            if (!other.IsNormalized)
                other = other.AsNormalized();
            return _alpha == other.Alpha.FromPercentage() && _hue == other.Hue.FromDegrees() && _saturation == other.Saturation.FromPercentage() && _brightness == other.Brightness.FromPercentage();
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
            if (other is HsbColor32)
                return Equals((HsbColor32)other);
            if (other is IHsbColorModel<byte>)
                return Equals((IHsbColorModel<byte>)other);
            if (other is IHsbColorModel<float>)
                return Equals((IHsbColorModel<float>)other, exact);
            if (other is IRgbColorModel<byte>)
                return Equals((IRgbColorModel<byte>)other, exact);
            return other is IRgbColorModel<float> && Equals((IRgbColorModel<float>)other, exact);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(HsbColor32 other) { return _value == other._value; }

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
        public bool Equals(IRgbColorModel<float> other) { return Equals(other, false); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(IHsbColorModel<byte> other) { return other.Alpha == _alpha && other.Hue == _hue && other.Saturation == _saturation && other.Brightness == _brightness; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(IRgbColorModel<byte> other) { return Equals(other, false); }

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
        public bool Equals(System.Drawing.Color other)
        {
            if (other.A != _alpha)
                return false;
            ColorExtensions.HSBtoRGB(_hue.ToDegrees(), _saturation.ToPercentage(), _brightness.ToPercentage(), out float r, out float g, out float b);
            return r.FromPercentage() == other.R && g.FromPercentage() == other.G && b.FromPercentage() == other.B;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(System.Windows.Media.Color other)
        {
            if (other.A != _alpha)
                return false;
            ColorExtensions.HSBtoRGB(_hue.ToDegrees(), _saturation.ToPercentage(), _brightness.ToPercentage(), out float r, out float g, out float b);
            return r.FromPercentage() == other.R && g.FromPercentage() == other.G && b.FromPercentage() == other.B;
        }

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
            if (value is HsbColor32)
                return Equals((HsbColor32)value);
            if (value is IHsbColorModel<byte>)
                return Equals((IHsbColorModel<byte>)value);
            if (value is IHsbColorModel<float>)
                return Equals((IHsbColorModel<float>)value, false);
            if (value is IRgbColorModel<byte>)
                return Equals((IRgbColorModel<byte>)value, false);
            if (value is IRgbColorModel<float>)
                return Equals((IRgbColorModel<float>)value, false);
            if (value is int)
                return Equals((int)value);
            value = ColorExtensions.AsSimplestType(value);

            if (value is string)
                return (string)value == ToString();

            if (value is int)
                return ToAHSB() == (int)value;

            if (value is float)
                return ToAHSB() == (float)value;

            if (obj is PSObject && ColorExtensions.TryGetColor((PSObject)obj, out IColorModel color))
                return Equals(color.AsHsb32());
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
        public IHsbColorModel<byte> MergeAverage(IEnumerable<IHsbColorModel<byte>> other)
        {
            if (other == null)
                return this;

            ColorExtensions.HSBtoRGB(_hue.ToDegrees(), _saturation.ToPercentage(), _brightness.ToPercentage(), out float rF, out float gF, out float bF);
            double r = rF, g = gF, b = bF, a = _alpha.ToPercentage();
            double brightness = _brightness.ToPercentage();
            int count = 0;
            foreach (IHsbColorModel<byte> item in other)
            {
                if (item != null)
                {
                    count++;
                    ColorExtensions.HSBtoRGB(item.Hue.ToPercentage(), item.Saturation.ToPercentage(), item.Brightness.ToPercentage(), out rF, out gF, out bF);
                    r += (double)rF;
                    g += (double)gF;
                    b += (double)bF;
                    a += (double)item.Alpha.ToPercentage();
                    brightness += (double)item.Brightness.ToPercentage();
                }
            }
            if (count == 0)
                return this;

            ColorExtensions.RGBtoHSB((float)(r / (double)count), (float)(r / (double)count), (float)(r / (double)count), out float h, out float s, out bF);
            return new HsbColor32Normalized(h.FromDegrees(), s.FromPercentage(), bF.FromPercentage(), ((float)(a / (double)count)).FromPercentage());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public IHsbColorModel<byte> MergeAverage(IEnumerable<IColorModel> other)
        {
            throw new NotImplementedException();
        }

        IColorModel IColorModel.MergeAverage(IEnumerable<IColorModel> other) { return MergeAverage(other); }

        #endregion
        
        #region ShiftHue Method

        /// <summary>
        /// Returns a <see cref="IColorModel" /> value with the color hue adjusted.
        /// </summary>
        /// <param name="degrees">The number of degrees to shift the hue value, ranging from -360.0 to 360.0. A positive value shifts the hue in the red-to-cyan direction, and a negative value shifts the hue in the cyan-to-red direction.</param>
        /// <returns>A <see cref="IColorModel" /> value with the color hue adjusted.</returns>
        /// <remarks>The values 0.0, -360.0 and 360.0 have no effect since they would result in no hue change.</remarks>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="degrees" /> is less than -360.0 or <paramref name="degrees" /> is greater than 360.0.</exception>
        public HsbColor32 ShiftHue(float degrees)
        {
            if (degrees < -360f || degrees > 360f)
                throw new ArgumentOutOfRangeException("degrees");
            if (degrees == 0f || degrees == 360f || degrees == -360f)
                return this;
            float hue = _hue.ToDegrees() + degrees;
            if (hue < 0f)
                hue += 360f;
            else if (hue >= 360f)
                hue -= 360f;
            return new HsbColor32(hue.FromDegrees(), _saturation, _brightness, _alpha);
        }

        IHsbColorModel<byte> IHsbColorModel<byte>.ShiftHue(float degrees) { return ShiftHue(degrees); }

        IColorModel<byte> IColorModel<byte>.ShiftHue(float degrees) { return ShiftHue(degrees); }

        IColorModel IColorModel.ShiftHue(float degrees) { return ShiftHue(degrees); }

        #endregion
        
        #region ShiftSaturation Method

        /// <summary>
        /// Returns a <see cref="HsbColor32" /> value with the color saturation adjusted.
        /// </summary>
        /// <param name="percentage">The percentage to saturate the color, ranging from -1.0 to 1.0. A positive value increases saturation, a negative value decreases saturation and a zero vale has no effect.</param>
        /// <returns>A <see cref="HsbColor32" /> value with the color saturation adjusted.</returns>
        /// <remarks>For positive values, the target saturation value is determined using the following formula: <c>saturation + (MAX_VALUE - saturation) * percentage</c>
        /// <para>For negative values, the target saturation value is determined using the following formula: <c>saturation + saturation * percentage</c></para></remarks>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="percentage" /> is less than -1.0 or <paramref name="percentage" /> is greater than 1.0.</exception>
        public HsbColor32 ShiftSaturation(float percentage)
        {
            if (percentage < -1f || percentage > 1f)
                throw new ArgumentOutOfRangeException("percentage");
            if (percentage == 0f || (percentage == 1f) ? _saturation == 255 : percentage == -1f && _saturation == 0)
                return this;
            float s = _saturation.ToPercentage();
            return new HsbColor32(_hue, (s + ((percentage > 0f) ? (1f - s) : s) * percentage).FromPercentage(), _brightness, _alpha);
        }

        IHsbColorModel<byte> IHsbColorModel<byte>.ShiftSaturation(float percentage) { return ShiftSaturation(percentage); }

        IColorModel<byte> IColorModel<byte>.ShiftSaturation(float percentage) { return ShiftSaturation(percentage); }

        IColorModel IColorModel.ShiftSaturation(float percentage) { return ShiftSaturation(percentage); }

        #endregion
        
        #region ShiftBrightness Method

        /// <summary>
        /// Returns a <see cref="HsbColor32" /> value with the color brightness adjusted.
        /// </summary>
        /// <param name="percentage">The percentage to saturate the color, ranging from -1.0 to 1.0. A positive value increases brightness, a negative value decreases brightness and a zero vale has no effect.</param>
        /// <returns>A <see cref="HsbColor32" /> value with the color brightness adjusted.</returns>
        /// <remarks>For positive values, the target brightness value is determined using the following formula: <c>brightness + (MAX_VALUE - brightness) * percentage</c>
        /// <para>For negative values, the target brightness value is determined using the following formula: <c>brightness + brightness * percentage</c></para></remarks>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="percentage" /> is less than -1.0 or <paramref name="percentage" /> is greater than 1.0.</exception>
        public HsbColor32 ShiftBrightness(float percentage)
        {
            if (percentage < -1f || percentage > 1f)
                throw new ArgumentOutOfRangeException("percentage");
            if (percentage == 0f || (percentage == 1f) ? _brightness == 255 : percentage == -1f && _brightness == 0)
                return this;
            float b = _brightness.ToPercentage();
            return new HsbColor32(_hue, _saturation, (b + ((percentage > 0f) ? (1f - b) : b) * percentage).FromPercentage(), _alpha);
        }

        IHsbColorModel<byte> IHsbColorModel<byte>.ShiftBrightness(float percentage) { return ShiftBrightness(percentage); }

        IColorModel<byte> IColorModel<byte>.ShiftBrightness(float percentage) { return ShiftBrightness(percentage); }

        IColorModel IColorModel.ShiftBrightness(float percentage) { return ShiftBrightness(percentage); }

        #endregion

        /// <summary>
        /// Gets the AHSB integer value for the current <see cref="HsbColor32" /> value.
        /// </summary>
        /// <returns>The AHSB integer value for the current <see cref="HsbColor32" /> value.</returns>
        public int ToAHSB() { return _value; }

        #region ToString Methods

        /// <summary>
        /// Gets formatted string representing the current color value.
        /// </summary>
        /// <param name="format">The color string format to use.</param>
        /// <returns>The formatted string representing the current color value.</returns>
        public string ToString(ColorStringFormat format)
        {
            float r, g, b;
            switch (format)
            {
                case ColorStringFormat.HSLAHexOpt:
                    return ToHexidecimalString(_hue, _saturation, _brightness, _alpha, true);
                case ColorStringFormat.HSLAPercent:
                    return HsbColorF.ToPercentParameterString(_hue.ToDegrees(), _saturation.ToPercentage(), _brightness.ToPercentage(), _alpha.ToPercentage());
                case ColorStringFormat.HSLAValues:
                    return ToValueParameterString(_hue, _saturation, _brightness, _alpha);
                case ColorStringFormat.HSLHex:
                    return ToHexidecimalString(_hue, _saturation, _brightness, false);
                case ColorStringFormat.HSLHexOpt:
                    return ToHexidecimalString(_hue, _saturation, _brightness, true);
                case ColorStringFormat.HSLPercent:
                    return HsbColorF.ToPercentParameterString(_hue.ToDegrees(), _saturation.ToPercentage(), _brightness.ToPercentage());
                case ColorStringFormat.HSLValues:
                    return ToValueParameterString(_hue, _saturation, _brightness);
                case ColorStringFormat.RGBAHex:
                    ColorExtensions.HSBtoRGB(_hue.ToDegrees(), _saturation.ToPercentage(), _brightness.ToPercentage(), out r, out g, out b);
                    return RgbColor32.ToHexidecimalString(r.FromPercentage(), g.FromPercentage(), b.FromPercentage(), _alpha, false);
                case ColorStringFormat.RGBAHexOpt:
                    ColorExtensions.HSBtoRGB(_hue.ToDegrees(), _saturation.ToPercentage(), _brightness.ToPercentage(), out r, out g, out b);
                    return RgbColor32.ToHexidecimalString(r.FromPercentage(), g.FromPercentage(), b.FromPercentage(), _alpha, true);
                case ColorStringFormat.RGBAPercent:
                    ColorExtensions.HSBtoRGB(_hue.ToDegrees(), _saturation.ToPercentage(), _brightness.ToPercentage(), out r, out g, out b);
                    return RgbColorF.ToPercentParameterString(r, g, b, _alpha.ToPercentage());
                case ColorStringFormat.RGBAValues:
                    ColorExtensions.HSBtoRGB(_hue.ToDegrees(), _saturation.ToPercentage(), _brightness.ToPercentage(), out r, out g, out b);
                    return RgbColor32.ToValueParameterString(r.FromPercentage(), g.FromPercentage(), b.FromPercentage(), _alpha);
                case ColorStringFormat.RGBHex:
                    ColorExtensions.HSBtoRGB(_hue.ToDegrees(), _saturation.ToPercentage(), _brightness.ToPercentage(), out r, out g, out b);
                    return RgbColor32.ToHexidecimalString(r.FromPercentage(), g.FromPercentage(), b.FromPercentage(), false);
                case ColorStringFormat.RGBHexOpt:
                    ColorExtensions.HSBtoRGB(_hue.ToDegrees(), _saturation.ToPercentage(), _brightness.ToPercentage(), out r, out g, out b);
                    return RgbColor32.ToHexidecimalString(r.FromPercentage(), g.FromPercentage(), b.FromPercentage(), true);
                case ColorStringFormat.RGBPercent:
                    ColorExtensions.HSBtoRGB(_hue.ToDegrees(), _saturation.ToPercentage(), _brightness.ToPercentage(), out r, out g, out b);
                    return RgbColorF.ToPercentParameterString(r, g, b);
                case ColorStringFormat.RGBValues:
                    ColorExtensions.HSBtoRGB(_hue.ToDegrees(), _saturation.ToPercentage(), _brightness.ToPercentage(), out r, out g, out b);
                    return RgbColor32.ToValueParameterString(r.FromPercentage(), g.FromPercentage(), b.FromPercentage());
            }
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString() { return _value.ToString("X8"); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="h"></param>
        /// <param name="s"></param>
        /// <param name="b"></param>
        /// <param name="a"></param>
        /// <param name="canShorten"></param>
        /// <returns></returns>
        public static string ToHexidecimalString(byte h, byte s, byte b, byte a, bool canShorten)
        {
            string result = h.ToString("X2") + s.ToString("X2") + b.ToString("X2") + a.ToString("X2");
            if (canShorten && result[0] == result[1] && result[2] == result[3] && result[4] == result[5] && result[6] == result[7])
                return new string(new char[] { result[0], result[2], result[4], result[6] });
            return result;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="h"></param>
        /// <param name="s"></param>
        /// <param name="b"></param>
        /// <param name="a"></param>
        /// <returns></returns>
        public static string ToHexidecimalString(byte h, byte s, byte b, byte a) { return ToHexidecimalString(h, s, b, a, false); }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="h"></param>
        /// <param name="s"></param>
        /// <param name="b"></param>
        /// <param name="canShorten"></param>
        /// <returns></returns>
        public static string ToHexidecimalString(byte h, byte s, byte b, bool canShorten)
        {
            string result = h.ToString("X2") + s.ToString("X2") + b.ToString("X2");
            if (canShorten && result[0] == result[1] && result[2] == result[3] && result[4] == result[5])
                return new string(new char[] { result[0], result[2], result[4] });
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="h"></param>
        /// <param name="s"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static string ToHexidecimalString(byte h, byte s, byte b) { return ToHexidecimalString(h, s, b, false); }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="h"></param>
        /// <param name="s"></param>
        /// <param name="b"></param>
        /// <param name="a"></param>
        /// <returns></returns>
        public static string ToValueParameterString(byte h, byte s, byte b, float a)
        {
            return "hsla(" + h.ToString() + ", " + s.ToString() + ", " + b.ToString() + ", " + Math.Round(a, 2).ToString() + ")";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="h"></param>
        /// <param name="s"></param>
        /// <param name="b"></param>
        /// <param name="a"></param>
        /// <returns></returns>
        public static string ToValueParameterString(byte h, byte s, byte b, byte a) { return ToValueParameterString(h, s, b, Convert.ToSingle(a) / 255f); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="h"></param>
        /// <param name="s"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static string ToValueParameterString(byte h, byte s, byte b)
        {
            return "hsl(" + h.ToString() + ", " + s.ToString() + ", " + b.ToString() + ")";
        }

        #endregion
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool TryParse(string text, out HsbColor32 result) { return TryParse(text, false, out result); }

        internal static bool TryParse(string text, bool strict, out HsbColor32 result)
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
                                    result = new HsbColor32(int.Parse(new string(new char[] { text[0], text[0], text[1], text[1], text[2], text[2]}), NumberStyles.HexNumber) << 8);
                                    break;
                                case 4:
                                    result = new HsbColor32(int.Parse(new string(new char[] { text[0], text[0], text[1], text[1], text[2], text[2]}), NumberStyles.HexNumber) << 8 | int.Parse(new string(new char[] { text[3], text[3] })));
                                    break;
                                case 8:
                                    result = new HsbColor32(int.Parse(text.Substring(0, 6), NumberStyles.HexNumber) << 8 | int.Parse(text.Substring(6), NumberStyles.HexNumber));
                                    break;
                                default:
                                    result = new HsbColor32(int.Parse(text, NumberStyles.HexNumber) << 8);
                                    break;
                            }
                            return true;
                        }
                        
                        float alpha = 100f;
                        if (!match.Groups["a"].Success || (((match.Groups["a"].Value.EndsWith("%")) ? (float.TryParse(match.Groups["a"].Value.Substring(0, match.Groups["a"].Length - 1), out alpha) && (alpha = alpha / 100f) <= 1f) : (float.TryParse(match.Groups["a"].Value, out alpha) && alpha <= 1f)) && alpha >= 0f))
                        {
                            if (match.Groups["b"].Success)
                            {
                                int h, s, l;
                                if (int.TryParse(match.Groups["h"].Value, out h) && h > -1 && h < 256 && int.TryParse(match.Groups["s"].Value, out s) && s > -1 && s < 256 &&
                                    int.TryParse(match.Groups["l"].Value, out l) && l > -1 && l < 256)
                                {
                                    result = new HsbColor32((byte)h, (byte)s, (byte)l, alpha.FromPercentage());
                                    return true;
                                }
                            }
                            else if (float.TryParse(match.Groups["h"].Value, out float hF) && hF >= 0f && hF <= 360f && float.TryParse(match.Groups["s"].Value, out float sF) && sF >= 0f && sF <= 100f && float.TryParse(match.Groups["l"].Value, out float lF) && lF >= 0f && lF <= 100f)
                            {
                                result = new HsbColor32(hF.FromDegrees(), (sF / 100f).FromPercentage(), (lF / 100f).FromPercentage(), alpha.FromPercentage());
                                return true;
                            }
                        }
                    }
                    catch { }
                }
                else if (!strict && RgbColorF.TryParse(text, true, out RgbColorF rgb))
                {
                    ColorExtensions.RGBtoHSB(rgb.Red, rgb.Blue, rgb.Green, out float h, out float s, out float b);
                    result = new HsbColor32(h.FromDegrees(), s.FromPercentage(), b.FromPercentage(), rgb.Alpha.FromPercentage());
                    return true;
                }
            }
            
            result = default(HsbColor32);
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
