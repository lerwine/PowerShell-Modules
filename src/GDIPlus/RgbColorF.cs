using System.Globalization;
using System.Management.Automation;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Erwine.Leonard.T.GDIPlus
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    /// <summary>
    /// 
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct RgbColorF : IEquatable<RgbColorF>, IEquatable<IRgbColorModel<byte>>, IEquatable<IHsbColorModel<byte>>, IRgbColorModel<float>
    {
        private readonly float _alpha, _red, _green, _blue;

        #region Properties

        /// <summary>
        /// The opaqueness of the color.
        /// </summary>
        public readonly float Alpha => _alpha;

        /// <summary>
        /// The intensity of the red layer.
        /// </summary>
        public readonly float Red => _red;

        /// <summary>
        /// The intensity of the green layer.
        /// </summary>
        public readonly float Green => _green;

        /// <summary>
        /// The intensity of the blue layer.
        /// </summary>
        public readonly float Blue => _blue;

        readonly bool IColorModel.IsNormalized => true;

        readonly ColorStringFormat IColorModel.DefaultStringFormat => ColorStringFormat.RGBAPercent;

        #endregion

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="red"></param>
        /// <param name="green"></param>
        /// <param name="blue"></param>
        /// <param name="alpha"></param>
        public RgbColorF(float red, float green, float blue, float alpha)
        {
            if (red < 0f || red > 1f)
                throw new ArgumentOutOfRangeException(nameof(red));
            if (green < 0f || green > 1f)
                throw new ArgumentOutOfRangeException(nameof(green));
            if (blue < 0f || blue > 1f)
                throw new ArgumentOutOfRangeException(nameof(blue));
            if (alpha < 0f || alpha > 1f)
                throw new ArgumentOutOfRangeException(nameof(alpha));
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
        public RgbColorF(float red, float green, float blue) : this(red, green, blue, 1f) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public RgbColorF(RgbColor32 value)
        {
            _red = value.Red.ToPercentage();
            _green = value.Green.ToPercentage();
            _blue = value.Blue.ToPercentage();
            _alpha = value.Alpha.ToPercentage();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public RgbColorF(IHsbColorModel<float> value)
        {
            ArgumentNullException.ThrowIfNull(value);
            if (value.Alpha < 0f || value.Alpha > 1f)
                throw new ArgumentOutOfRangeException(nameof(value), "Value for alpha is out of range");
            try
            {
                ColorExtensions.HSBtoRGB(value.Hue, value.Saturation, value.Brightness, out float r, out float g, out float b);
                _red = r;
                _green = g;
                _blue = b;
            }
            catch (ArgumentOutOfRangeException exc) { throw new ArgumentOutOfRangeException(nameof(value), "Value for " + exc.ParamName + " is out of range"); }
            _alpha = value.Alpha;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public RgbColorF(IHsbColorModel<byte> value)
        {
            ArgumentNullException.ThrowIfNull(value);
            ColorExtensions.HSBtoRGB(value.Hue.ToDegrees(), value.Saturation.ToPercentage(), value.Brightness.ToPercentage(), out float r, out float g, out float b);
            _red = r;
            _green = g;
            _blue = b;
            _alpha = value.Alpha.ToPercentage();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="argb"></param>
        public RgbColorF(int argb)
        {
            byte[] values = BitConverter.GetBytes(argb);
            _blue = values[0].ToPercentage();
            _green = values[1].ToPercentage();
            _red = values[2].ToPercentage();
            _alpha = values[3].ToPercentage();
        }

        #endregion

        #region As* Methods

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public readonly HsbColor32Normalized AsHsb32() => new(this);

        readonly IHsbColorModel<byte> IColorModel.AsHsb32() => AsHsb32();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public readonly HsbColorFNormalized AsHsbF() => new(this);

        readonly IHsbColorModel<float> IColorModel.AsHsbF() => AsHsbF();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public readonly RgbColor32 AsRgb32() => new(this);

        readonly IRgbColorModel<byte> IColorModel.AsRgb32() => AsRgb32();

        readonly IRgbColorModel<float> IColorModel.AsRgbF() => this;

        readonly IColorModel<float> IColorModel<float>.AsNormalized() => this;

        readonly IColorModel IColorModel.AsNormalized() => this;

        #endregion

        #region Equals Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="exact"></param>
        /// <returns></returns>
        public readonly bool Equals(IHsbColorModel<float> other, bool exact)
        {
            if (other == null || _alpha != other.Alpha)
                return false;

            float b;
            if (exact)
            {
                ColorExtensions.RGBtoHSB(_red, _green, _blue, out float h, out float s, out b);
                return other.Hue == h && other.Saturation == s && other.Brightness == b;
            }

            if (!other.IsNormalized)
                other = other.AsNormalized();

            ColorExtensions.HSBtoRGB(other.Hue, other.Saturation, other.Brightness, out float r, out float g, out b);
            return _red == r && _green == g && _blue == b;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="exact"></param>
        /// <returns></returns>
        public readonly bool Equals(IHsbColorModel<byte> other, bool exact)
        {
            if (other == null)
                return false;
            float b;
            if (exact)
            {
                if (other.Alpha.ToPercentage() != _alpha)
                    return false;
                ColorExtensions.HSBtoRGB(other.Hue.ToDegrees(), other.Saturation.ToPercentage(), other.Brightness.ToPercentage(), out float r, out float g, out b);
                return _red == r && _green == g && _blue == b;
            }

            if (other.Alpha != _alpha.FromPercentage())
                return false;

            if (!other.IsNormalized)
                other = other.AsNormalized();

            ColorExtensions.RGBtoHSB(_red, _green, _blue, out float h, out float s, out b);
            return other.Hue == h.FromDegrees() && other.Saturation == s.FromPercentage() && other.Brightness == b.FromPercentage();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="exact"></param>
        /// <returns></returns>
        public readonly bool Equals(IRgbColorModel<byte> other, bool exact)
        {
            if (other == null)
                return false;

            if (exact)
                return _alpha == other.Alpha.ToPercentage() && _red == other.Red.ToPercentage() && _green == other.Green.ToPercentage() && _blue == other.Blue.ToPercentage();
            return ToARGB() == other.ToARGB();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="exact"></param>
        /// <returns></returns>
        public readonly bool Equals(IColorModel other, bool exact)
        {
            if (other == null)
                return false;
            if (other is RgbColorF)
                return Equals((RgbColorF)other);
            if (other is IRgbColorModel<float>)
                return Equals((IRgbColorModel<float>)other);
            if (other is IRgbColorModel<byte>)
                return Equals((IRgbColorModel<byte>)other, exact);
            if (other is IHsbColorModel<float>)
                return Equals((IHsbColorModel<float>)other, exact);
            return other is IHsbColorModel<byte> && Equals((IHsbColorModel<byte>)other, exact);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public readonly bool Equals(RgbColorF other) => other._alpha == _alpha && other._red == _red && other._green == _green && other._blue == _blue; 

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public readonly bool Equals(IRgbColorModel<byte> other) => Equals(other, false);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public readonly bool Equals(IHsbColorModel<byte> other) => Equals(other, false);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public readonly bool Equals(IRgbColorModel<float> other) => other.Alpha == _alpha && other.Red == _red && other.Green == _green && other.Blue == _blue;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public readonly bool Equals(IHsbColorModel<float> other) => Equals(other, false);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public readonly bool Equals(IColorModel other) => Equals(other, false);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public readonly bool Equals(System.Drawing.Color other) => other.A == _alpha.FromPercentage() && other.R == _red.FromPercentage() && other.G == _green.FromPercentage() && other.B == _blue.FromPercentage();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public readonly bool Equals(System.Windows.Media.Color other) => other.A == _alpha.FromPercentage() && other.R == _red.FromPercentage() && other.G == _green.FromPercentage() && other.B == _blue.FromPercentage();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override readonly bool Equals(object obj)
        {
            if (obj == null)
                return false;
            object value = (obj is PSObject) ? ((PSObject)obj).BaseObject : obj;
            if (value is RgbColorF)
                return Equals((RgbColorF)value);
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
                return Equals(color.AsRgbF());
            return false;
        }

        #endregion

        /// <summary>
        /// Returns the hash code for this value.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override readonly int GetHashCode() => ToARGB(); 

        #region MergeAverage Method

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public readonly RgbColorF MergeAverage(IEnumerable<IRgbColorModel<float>> other)
        {
            if (other == null)
                return this;
            
            double r = _red, g = _green, b = _blue, a = _alpha;
            ColorExtensions.RGBtoHSB(_red, _green, _blue, out float h, out float s, out float bF);

            double brightness = bF;
            int count = 0;
            foreach (IRgbColorModel<float> item in other)
            {
                if (item != null)
                {
                    count++;
                    r += (double)item.Red;
                    g += (double)item.Green;
                    b += (double)item.Blue;
                    a += (double)item.Alpha;
                    ColorExtensions.RGBtoHSB(item.Red, item.Green, item.Blue, out h, out s, out bF);
                    brightness += (double)bF;
                }
            }
            if (count == 0)
                return this;
            
            ColorExtensions.RGBtoHSB((float)(r / (double)count), (float)(r / (double)count), (float)(r / (double)count), out h, out s, out bF);
            ColorExtensions.HSBtoRGB(h, s, (float)(brightness / (double)count), out float red, out float green, out bF);
            return new RgbColorF(red, green, bF, (float)(a / (double)count));
        }

        readonly IRgbColorModel<float> IRgbColorModel<float>.MergeAverage(IEnumerable<IRgbColorModel<float>> other) => MergeAverage(other);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public RgbColorF MergeAverage(IEnumerable<IColorModel> other)
        {
            throw new NotImplementedException();
        }

        IColorModel IColorModel.MergeAverage(IEnumerable<IColorModel> other) => MergeAverage(other);

        #endregion

        #region ShiftHue Method

        /// <summary>
        /// Returns a <see cref="RgbColorF" /> value with the color hue adjusted.
        /// </summary>
        /// <param name="degrees">The number of degrees to shift the hue value, ranging from -360.0 to 360.0. A positive value shifts the hue in the red-to-cyan direction, and a negative value shifts the hue in the cyan-to-red direction.</param>
        /// <returns>A <see cref="RgbColorF" /> value with the color hue adjusted.</returns>
        /// <remarks>The values 0.0, -360.0 and 360.0 have no effect since they would result in no hue change.</remarks>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="degrees" /> is less than -360.0 or <paramref name="degrees" /> is greater than 360.0.</exception>
        public readonly RgbColorF ShiftHue(float degrees)
        {
            if (degrees < -360f || degrees > 360f)
                throw new ArgumentOutOfRangeException(nameof(degrees));
            if (degrees == 0f || degrees == 360f || degrees == -360f)
                return this;
            ColorExtensions.RGBtoHSB(_red, _green, _blue, out float h, out float s, out float b);
            h += degrees;
            if (h < 0f)
                h += 360f;
            else if (h >= 360f)
                h -= 360f;
            ColorExtensions.HSBtoRGB(h, s, b, out float r, out float g, out b);
            return new RgbColorF(r, g, b, _alpha);
        }

        readonly IRgbColorModel<float> IRgbColorModel<float>.ShiftHue(float degrees) => ShiftHue(degrees);

        readonly IColorModel<float> IColorModel<float>.ShiftHue(float degrees) => ShiftHue(degrees);

        readonly IColorModel IColorModel.ShiftHue(float degrees) => ShiftHue(degrees);

        #endregion

        #region ShiftSaturation Method

        /// <summary>
        /// Returns a <see cref="RgbColorF" /> value with the color saturation adjusted.
        /// </summary>
        /// <param name="percentage">The percentage to saturate the color, ranging from -1.0 to 1.0. A positive value increases saturation, a negative value decreases saturation and a zero vale has no effect.</param>
        /// <returns>A <see cref="RgbColorF" /> value with the color saturation adjusted.</returns>
        /// <remarks>For positive values, the target saturation value is determined using the following formula: <c>saturation + (1.0 - saturation) * percentage</c>
        /// <para>For negative values, the target saturation value is determined using the following formula: <c>saturation + saturation * percentage</c></para></remarks>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="percentage" /> is less than -1.0 or <paramref name="percentage" /> is greater than 1.0.</exception>
        public readonly RgbColorF ShiftSaturation(float percentage)
        {
            if (percentage < -1f || percentage > 1f)
                throw new ArgumentOutOfRangeException(nameof(percentage));
            if (percentage == 0f)
                return this;
            ColorExtensions.RGBtoHSB(_red, _green, _blue, out float h, out float s, out float b);
            if ((percentage == 1f) ? s == 1f : percentage == -1f && s == 0f)
                return this;
            ColorExtensions.HSBtoRGB(h, s + ((percentage > 0f) ? (1f - s) : s) * percentage, b, out float r, out float g, out b);
            return new RgbColorF(r, g, b, _alpha);
        }

        readonly IRgbColorModel<float> IRgbColorModel<float>.ShiftSaturation(float percentage) => ShiftSaturation(percentage);

        readonly IColorModel<float> IColorModel<float>.ShiftSaturation(float percentage) => ShiftSaturation(percentage);

        readonly IColorModel IColorModel.ShiftSaturation(float percentage) => ShiftSaturation(percentage);

        #endregion

        #region ShiftBrightness Method

        /// <summary>
        /// Returns a <see cref="RgbColorF" /> value with the color brightness adjusted.
        /// </summary>
        /// <param name="percentage">The percentage to saturate the color, ranging from -1.0 to 1.0. A positive value increases brightness, a negative value decreases brightness and a zero vale has no effect.</param>
        /// <returns>A <see cref="RgbColorF" /> value with the color brightness adjusted.</returns>
        /// <remarks>For positive values, the target brightness value is determined using the following formula: <c>brightness + (1.0 - brightness) * percentage</c>
        /// <para>For negative values, the target brightness value is determined using the following formula: <c>brightness + brightness * percentage</c></para></remarks>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="percentage" /> is less than -1.0 or <paramref name="percentage" /> is greater than 1.0.</exception>
        public readonly RgbColorF ShiftBrightness(float percentage)
        {
            if (percentage < -1f || percentage > 1f)
                throw new ArgumentOutOfRangeException(nameof(percentage));
            if (percentage == 0f)
                return this;
            ColorExtensions.RGBtoHSB(_red, _green, _blue, out float h, out float s, out float b);
            if ((percentage == 1f) ? b == 1f : percentage == -1f && b == 0f)
                return this;
            ColorExtensions.HSBtoRGB(h, s, b + ((percentage > 0f) ? (1f - b) : b) * percentage, out float r, out float g, out b);
            return new RgbColorF(r, g, b, _alpha);
        }

        readonly IRgbColorModel<float> IRgbColorModel<float>.ShiftBrightness(float percentage) => ShiftBrightness(percentage);

        readonly IColorModel<float> IColorModel<float>.ShiftBrightness(float percentage) => ShiftBrightness(percentage);

        readonly IColorModel IColorModel.ShiftBrightness(float percentage) => ShiftBrightness(percentage);

        #endregion

        /// <summary>
        /// Gets the ARGB integer value for the current <see cref="RgbColorF" /> value.
        /// </summary>
        /// <returns>The ARGB integer value for the current <see cref="RgbColorF" /> value.</returns>
        public readonly int ToARGB() => BitConverter.ToInt32([_blue.FromPercentage(), _green.FromPercentage(), _red.FromPercentage(), _alpha.FromPercentage()], 0);

        #region ToString Methods

        /// <summary>
        /// Gets formatted string representing the current color value.
        /// </summary>
        /// <param name="format">The color string format to use.</param>
        /// <returns>The formatted string representing the current color value.</returns>
        public readonly string ToString(ColorStringFormat format)
        {
            float h, s, b;
            switch (format)
            {
                case ColorStringFormat.HSLAHex:
                    ColorExtensions.RGBtoHSB(_red, _green, _blue, out h, out s, out b);
                    return HsbColor32.ToHexidecimalString(h.FromDegrees(), s.FromPercentage(), b.FromPercentage(), _alpha.FromPercentage(), false);
                case ColorStringFormat.HSLAHexOpt:
                    ColorExtensions.RGBtoHSB(_red, _green, _blue, out h, out s, out b);
                    return HsbColor32.ToHexidecimalString(h.FromDegrees(), s.FromPercentage(), b.FromPercentage(), _alpha.FromPercentage(), true);
                case ColorStringFormat.HSLAPercent:
                    ColorExtensions.RGBtoHSB(_red, _green, _blue, out h, out s, out b);
                    return HsbColorF.ToPercentParameterString(h, s, b, _alpha);
                case ColorStringFormat.HSLAValues:
                    ColorExtensions.RGBtoHSB(_red, _green, _blue, out h, out s, out b);
                    return HsbColor32.ToValueParameterString(h.FromDegrees(), s.FromPercentage(), b.FromPercentage(), _alpha);
                case ColorStringFormat.HSLHex:
                    ColorExtensions.RGBtoHSB(_red, _green, _blue, out h, out s, out b);
                    return HsbColor32.ToHexidecimalString(h.FromDegrees(), s.FromPercentage(), b.FromPercentage(), false);
                case ColorStringFormat.HSLHexOpt:
                    ColorExtensions.RGBtoHSB(_red, _green, _blue, out h, out s, out b);
                    return HsbColor32.ToHexidecimalString(h.FromDegrees(), s.FromPercentage(), b.FromPercentage(), true);
                case ColorStringFormat.HSLPercent:
                    ColorExtensions.RGBtoHSB(_red, _green, _blue, out h, out s, out b);
                    return HsbColorF.ToPercentParameterString(h, s, b);
                case ColorStringFormat.HSLValues:
                    ColorExtensions.RGBtoHSB(_red, _green, _blue, out h, out s, out b);
                    return HsbColor32.ToValueParameterString(h.FromDegrees(), s.FromPercentage(), b.FromPercentage());
                case ColorStringFormat.RGBAHex:
                    return RgbColor32.ToHexidecimalString(_red.FromPercentage(), _green.FromPercentage(), _blue.FromPercentage(), _alpha.FromPercentage(), false);
                case ColorStringFormat.RGBAHexOpt:
                    return RgbColor32.ToHexidecimalString(_red.FromPercentage(), _green.FromPercentage(), _blue.FromPercentage(), _alpha.FromPercentage(), true);
                case ColorStringFormat.RGBAValues:
                    return RgbColor32.ToValueParameterString(_red.FromPercentage(), _green.FromPercentage(), _blue.FromPercentage(), _alpha.FromPercentage());
                case ColorStringFormat.RGBHex:
                    return RgbColor32.ToHexidecimalString(_red.FromPercentage(), _green.FromPercentage(), _blue.FromPercentage(), false);
                case ColorStringFormat.RGBHexOpt:
                    return RgbColor32.ToHexidecimalString(_red.FromPercentage(), _green.FromPercentage(), _blue.FromPercentage(), true);
                case ColorStringFormat.RGBPercent:
                    return ToPercentParameterString(_red, _green, _blue);
                case ColorStringFormat.RGBValues:
                    return RgbColor32.ToValueParameterString(_red.FromPercentage(), _green.FromPercentage(), _blue.FromPercentage());
            }
            return ToPercentParameterString(_red, _green, _blue, _alpha);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <param name="a"></param>
        /// <returns></returns>
        public static string ToPercentParameterString(float r, float g, float b, float a) => "rgba(" + Math.Round(Convert.ToDouble(r * 100f), 0).ToString() + "%, " + Math.Round(Convert.ToDouble(g * 100f), 0).ToString() + "%, " +
                Math.Round(Convert.ToDouble(b * 100f), 0).ToString() + "%, " + Math.Round(Convert.ToDouble(a * 100f), 0).ToString() + "%)";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static string ToPercentParameterString(float r, float g, float b) => "rgb(" + Math.Round(Convert.ToDouble(r * 100f), 0).ToString() + "%, " + Math.Round(Convert.ToDouble(g * 100f), 0).ToString() + "%, " +
                Math.Round(Convert.ToDouble(b * 100f), 0).ToString() + "%)";

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override readonly string ToString() => ToPercentParameterString(_red, _green, _blue, _alpha);

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool TryParse(string text, out RgbColorF result) => TryParse(text, false, out result);

        internal static bool TryParse(string text, bool strict, out RgbColorF result)
        {
            if (text != null && (text = text.Trim()).Length > 0)
            {
                Match match = RgbColor32.ParseRegex.Match(text);
                if (match.Success)
                {
                    try
                    {
                        if (match.Groups["h"].Success)
                        {
                            result = text.Length switch
                            {
                                3 => new RgbColorF(int.Parse(new string(new char[] { text[0], text[0], text[1], text[1], text[2], text[2] }), NumberStyles.HexNumber) << 8),
                                4 => new RgbColorF(int.Parse(new string(new char[] { text[0], text[0], text[1], text[1], text[2], text[2] }), NumberStyles.HexNumber) << 8 | int.Parse(new string(new char[] { text[3], text[3] }))),
                                8 => new RgbColorF(int.Parse(text.Substring(0, 6), NumberStyles.HexNumber) << 8 | int.Parse(text.Substring(6), NumberStyles.HexNumber)),
                                _ => new RgbColorF(int.Parse(text, NumberStyles.HexNumber) << 8),
                            };
                            return true;
                        }
                        
                        float alpha = 100f;
                        if (!match.Groups["a"].Success || ((match.Groups["a"].Value.EndsWith("%") ? (float.TryParse(match.Groups["a"].Value.Substring(0, match.Groups["a"].Length - 1), out alpha) && (alpha = alpha / 100f) <= 1f) : (float.TryParse(match.Groups["a"].Value, out alpha) && alpha <= 1f)) && alpha >= 0f))
                        {
                            if (match.Groups["v"].Success)
                            {
                                if (int.TryParse(match.Groups["r"].Value, out int r) && r > -1 && r < 256 && int.TryParse(match.Groups["g"].Value, out int g) && g > -1 && g < 256 &&
                                    int.TryParse(match.Groups["b"].Value, out int b) && b > -1 && b < 256)
                                {
                                    result = new RgbColorF(((byte)r).ToPercentage(), ((byte)g).ToPercentage(), ((byte)b).ToPercentage(), alpha);
                                    return true;
                                }
                            }
                            else if (float.TryParse(match.Groups["r"].Value, out float rF) && rF >= 0f && rF <= 100f &&
                                float.TryParse(match.Groups["g"].Value, out float gF) && gF >= 0f && gF <= 100f &&
                                float.TryParse(match.Groups["b"].Value, out float bF) && bF >= 0f && bF <= 100f)
                            {
                                result = new RgbColorF(rF / 100f, gF / 100f, bF / 100f, alpha);
                                return true;
                            }
                        }
                    }
                    catch { }
                }
                else if (!strict && HsbColorF.TryParse(text, true, out HsbColorF hsb))
                {
                    result = hsb.AsRgbF();
                    return true;
                }
            }
            
            result = default(RgbColorF);
            return false;
        }
    }
}
