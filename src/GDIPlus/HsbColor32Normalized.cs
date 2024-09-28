using System.Management.Automation;
using System.Runtime.InteropServices;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Erwine.Leonard.T.GDIPlus
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    /// <summary>
    /// A normalized HSB (Hue,Saturation,Brightness) color model using 32 bits.
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public struct HsbColor32Normalized : IEquatable<HsbColor32Normalized>, IEquatable<IHsbColorModel<float>>, IEquatable<IRgbColorModel<float>>, IHsbColorModel<byte>, IEquatable<int>, IConvertible
    {
        #region Fields

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
        public readonly byte Alpha => _alpha;

        /// <summary>
        /// The hue of the color.
        /// </summary>
        public readonly byte Hue => _hue;

        /// <summary>
        /// The color saturation.
        /// </summary>
        public readonly byte Saturation => _saturation;

        /// <summary>
        /// The brightness of the color.
        /// </summary>
        public readonly byte Brightness => _brightness;

        readonly bool IColorModel.IsNormalized => true;

        readonly ColorStringFormat IColorModel.DefaultStringFormat => ColorStringFormat.HSLAHex;

        #endregion

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hue"></param>
        /// <param name="saturation"></param>
        /// <param name="brightness"></param>
        /// <param name="alpha"></param>
        public HsbColor32Normalized(byte hue, byte saturation, byte brightness, byte alpha)
        {
            _value = 0;
            ColorExtensions.HSBtoRGB(hue.ToDegrees(), saturation.ToPercentage(), brightness.ToPercentage(), out float r, out float g, out float b);
            ColorExtensions.RGBtoHSB(r, g, b, out float hF, out float sF, out float bF);
            _hue = hF.FromDegrees();
            _brightness = bF.FromPercentage();
            _saturation = sF.FromPercentage();
            _alpha = alpha;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hue"></param>
        /// <param name="saturation"></param>
        /// <param name="brightness"></param>
        public HsbColor32Normalized(byte hue, byte saturation, byte brightness) : this(hue, saturation, brightness, 255) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public HsbColor32Normalized(RgbColorF value)
        {
            _value = 0;
            ColorExtensions.RGBtoHSB(value.Red, value.Green, value.Blue, out float h, out float s, out float b);
            _hue = h.FromDegrees();
            _saturation = s.FromPercentage();
            _brightness = b.FromPercentage();
            _alpha = value.Alpha.FromPercentage();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public HsbColor32Normalized(RgbColor32 value)
        {
            _value = 0;
            ColorExtensions.RGBtoHSB(value.Red.ToPercentage(), value.Green.ToPercentage(), value.Blue.ToPercentage(), out float h, out float s, out float b);
            _hue = h.FromDegrees();
            _saturation = s.FromPercentage();
            _brightness = b.FromPercentage();
            _alpha = value.Alpha;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public HsbColor32Normalized(IHsbColorModel<float> value)
        {
            ArgumentNullException.ThrowIfNull(value);
            _value = 0;
            if (value.Hue < 0f || value.Hue > ColorExtensions.HUE_MAXVALUE)
                throw new ArgumentOutOfRangeException("hue");
            if (value.Saturation < 0f || value.Saturation > 1f)
                throw new ArgumentOutOfRangeException("saturation");
            if (value.Brightness < 0f || value.Brightness > 1f)
                throw new ArgumentOutOfRangeException("brightness");
            if (value.Alpha < 0f || value.Alpha > 1f)
                throw new ArgumentOutOfRangeException("alpha");
            ColorExtensions.HSBtoRGB(value.Hue, value.Saturation, value.Brightness, out float r, out float g, out float b);
            ColorExtensions.RGBtoHSB(r, g, b, out float hF, out float sF, out float bF);
            _hue = hF.FromDegrees();
            _brightness = bF.FromPercentage();
            _saturation = sF.FromPercentage();
            _alpha = value.Alpha.FromPercentage(); ;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public HsbColor32Normalized(HsbColor32 value)
        {
            _value = 0;
            ColorExtensions.HSBtoRGB(value.Hue.ToDegrees(), value.Saturation.ToPercentage(), value.Brightness.ToPercentage(), out float r, out float g, out float b);
            ColorExtensions.RGBtoHSB(r, g, b, out float hF, out float sF, out float bF);
            _hue = hF.FromDegrees();
            _brightness = bF.FromPercentage();
            _saturation = sF.FromPercentage();
            _alpha = value.Alpha;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ahsb"></param>
        public HsbColor32Normalized(int ahsb)
        {
            _hue = _saturation = _brightness = _alpha = 0;
            _value = ahsb;
        }

        #endregion

        #region As* Methods

        readonly IHsbColorModel<byte> IColorModel.AsHsb32() { return this; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public readonly HsbColorFNormalized AsHsbF() { return new HsbColorFNormalized(this); }

        readonly IHsbColorModel<float> IColorModel.AsHsbF() { return AsHsbF(); }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public readonly RgbColor32 AsRgb32() { return new RgbColor32(this); }

        readonly IRgbColorModel<byte> IColorModel.AsRgb32() { return AsRgb32(); }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public readonly RgbColorF AsRgbF() { return new RgbColorF(this); }

        readonly IRgbColorModel<float> IColorModel.AsRgbF() { return AsRgbF(); }

        readonly IHsbColorModel<byte> IHsbColorModel<byte>.AsNormalized() { return this; }

        readonly IColorModel<byte> IColorModel<byte>.AsNormalized() { return this; }

        readonly IColorModel IColorModel.AsNormalized() { return this; }

        #endregion

        #region Equals Methods

        /// <summary>
        /// Indicates whether the current value is equal to another color model object.
        /// </summary>
        /// <param name="other">A color model to compare with this value.</param>
        /// <param name="exact"><c>true</c> to compare exact values; otherwise, <c>false</c> to compare normalized values.</param>
        /// <returns><c>true</c> if the current value is equal to another color model object; otherwise, <c>false</c>.</returns>
        public readonly bool Equals(IRgbColorModel<byte> other, bool exact)
        {
            if (other == null || _alpha != other.Alpha)
                return false;
            float b;
            if (exact)
            {
                ColorExtensions.RGBtoHSB(other.Red, other.Green, other.Blue, out float h, out float s, out b);
                return _hue == h.FromDegrees() && _saturation == s.FromPercentage() && _brightness == b.FromPercentage();
            }
            ColorExtensions.HSBtoRGB(_hue, _saturation, _brightness, out float r, out float g, out b);
            return other.Red == r.FromPercentage() && other.Green == g.FromPercentage() && other.Blue == b.FromPercentage();
        }

        /// <summary>
        /// Indicates whether the current value is equal to another color model object.
        /// </summary>
        /// <param name="other">A color model to compare with this value.</param>
        /// <param name="exact"><c>true</c> to compare exact values; otherwise, <c>false</c> to compare normalized values.</param>
        /// <returns><c>true</c> if the current value is equal to another color model object; otherwise, <c>false</c>.</returns>
        public readonly bool Equals(IRgbColorModel<float> other, bool exact)
        {
            if (other == null)
                return false;
            float b;
            if (exact)
            {
                if (other.Alpha != _alpha.ToPercentage())
                    return false;
                ColorExtensions.RGBtoHSB(other.Red, other.Green, other.Blue, out float h, out float s, out b);
                return _hue.ToDegrees() == h && _saturation.ToDegrees() == s && _brightness.ToDegrees() == b;
            }

            if (other.Alpha.FromPercentage() != _alpha)
                return false;

            ColorExtensions.HSBtoRGB(_hue.ToDegrees(), _saturation.ToPercentage(), _brightness.ToPercentage(), out float r, out float g, out b);
            return other.Red == r.FromPercentage() && other.Green == g.FromPercentage() && other.Blue == b.FromPercentage();
        }

        /// <summary>
        /// Indicates whether the current value is equal to another color model object.
        /// </summary>
        /// <param name="other">A color model to compare with this value.</param>
        /// <param name="exact"><c>true</c> to compare exact values; otherwise, <c>false</c> to compare normalized values.</param>
        /// <returns><c>true</c> if the current value is equal to another color model object; otherwise, <c>false</c>.</returns>
        public readonly bool Equals(IHsbColorModel<float> other, bool exact)
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
        /// Indicates whether the current value is equal to another color model object.
        /// </summary>
        /// <param name="other">A color model to compare with this value.</param>
        /// <param name="exact"><c>true</c> to compare exact values; otherwise, <c>false</c> to compare normalized values.</param>
        /// <returns><c>true</c> if the current value is equal to another color model object; otherwise, <c>false</c>.</returns>
        public readonly bool Equals(IColorModel other, bool exact)
        {
            if (other == null)
                return false;
            if (other is HsbColor32Normalized)
                return Equals((HsbColor32Normalized)other);
            if (other is IHsbColorModel<byte>)
                return Equals((IHsbColorModel<byte>)other);
            if (other is IHsbColorModel<float>)
                return Equals((IHsbColorModel<float>)other, exact);
            if (other is IRgbColorModel<byte>)
                return Equals((IRgbColorModel<byte>)other, exact);
            return other is IRgbColorModel<float> && Equals((IRgbColorModel<float>)other, exact);
        }

        /// <summary>
        /// Indicates whether the current value is equal to another <see cref="HsbColor32Normalized"/> values.
        /// </summary>
        /// <param name="other">A color model to compare with this value.</param>
        /// <returns><c>true</c> if the current value is equal to another color model object; otherwise, <c>false</c>.</returns>
        public readonly bool Equals(HsbColor32Normalized other) { return _value == other._value; }

        /// <summary>
        /// Indicates whether the current value is equal to another color model object.
        /// </summary>
        /// <param name="other">A color model to compare with this value.</param>
        /// <returns><c>true</c> if the current value is equal to another color model object; otherwise, <c>false</c>.</returns>
        public readonly bool Equals(IHsbColorModel<float> other) { return Equals(other, false); }

        /// <summary>
        /// Indicates whether the current value is equal to another color model object.
        /// </summary>
        /// <param name="other">A color model to compare with this value.</param>
        /// <returns><c>true</c> if the current value is equal to another color model object; otherwise, <c>false</c>.</returns>
        public readonly bool Equals(IRgbColorModel<float> other) { return Equals(other, false); }

        /// <summary>
        /// Indicates whether the current value is equal to another color model object.
        /// </summary>
        /// <param name="other">A color model to compare with this value.</param>
        /// <returns><c>true</c> if the current value is equal to another color model object; otherwise, <c>false</c>.</returns>
        public readonly bool Equals(IHsbColorModel<byte> other) { return other.Alpha == _alpha && other.Hue == _hue && other.Saturation == _saturation && other.Brightness == _brightness; }

        /// <summary>
        /// Indicates whether the current value is equal to another color model object.
        /// </summary>
        /// <param name="other">A color model to compare with this value.</param>
        /// <returns><c>true</c> if the current value is equal to another color model object; otherwise, <c>false</c>.</returns>
        public readonly bool Equals(IRgbColorModel<byte> other) { return Equals(other, false); }

        /// <summary>
        /// Indicates whether the current value is equal to another color model object.
        /// </summary>
        /// <param name="other">A color model to compare with this value.</param>
        /// <returns><c>true</c> if the current value is equal to another color model object; otherwise, <c>false</c>.</returns>
        public readonly bool Equals(IColorModel other) { return Equals(other, false); }

        /// <summary>
        /// Indicates whether the current value is equal to a <seealso cref="System.Drawing.Color"/> object.
        /// </summary>
        /// <param name="other">A <seealso cref="System.Drawing.Color"/> object to compare with this value.</param>
        /// <returns><c>true</c> if the current value is equal to the specified <seealso cref="System.Drawing.Color"/> object; otherwise, <c>false</c>.</returns>
        public readonly bool Equals(System.Drawing.Color other)
        {
            if (other.A != _alpha)
                return false;
            ColorExtensions.HSBtoRGB(_hue.ToDegrees(), _saturation.ToPercentage(), _brightness.ToPercentage(), out float r, out float g, out float b);
            return r.FromPercentage() == other.R && g.FromPercentage() == other.G && b.FromPercentage() == other.B;
        }

        /// <summary>
        /// Indicates whether the current value is equal to a <seealso cref="System.Windows.Media.Color"/> object.
        /// </summary>
        /// <param name="other">A <seealso cref="System.Windows.Media.Color"/> object to compare with this value.</param>
        /// <returns><c>true</c> if the current value is equal to the specified <seealso cref="System.Windows.Media.Color"/> object; otherwise, <c>false</c>.</returns>
        public readonly bool Equals(System.Windows.Media.Color other)
        {
            if (other.A != _alpha)
                return false;
            ColorExtensions.HSBtoRGB(_hue.ToDegrees(), _saturation.ToPercentage(), _brightness.ToPercentage(), out float r, out float g, out float b);
            return r.FromPercentage() == other.R && g.FromPercentage() == other.G && b.FromPercentage() == other.B;
        }

        /// <summary>
        /// Indicates whether the AHSB integer value for the current color is equal to an <seealso cref="int"/> value.
        /// </summary>
        /// <param name="other">An integer value to compare with the current color's AHSB value.</param>
        /// <returns><c>true</c> the AHSB integer value for the current color is equal to an <seealso cref="int"/> value; otherwise, <c>false</c>.</returns>
        public readonly bool Equals(int other) { return _value == other; }

        /// <summary>
        /// Indicates whether the current value is equal to another object.
        /// </summary>
        /// <param name="obj">An object to compare with the current value.</param>
        /// <returns><c>true</c> if the current value is equal to object; otherwise, <c>false</c>.</returns>
        public override readonly bool Equals(object obj)
        {
            if (obj == null)
                return false;
            object value = (obj is PSObject) ? ((PSObject)obj).BaseObject : obj;
            if (value is HsbColor32Normalized)
                return Equals((HsbColor32Normalized)value);
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
        public override readonly int GetHashCode() { return _value; }

        #region MergeAverage Method

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public readonly HsbColor32Normalized MergeAverage(IEnumerable<IHsbColorModel<byte>> other)
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

        readonly IHsbColorModel<byte> IHsbColorModel<byte>.MergeAverage(IEnumerable<IHsbColorModel<byte>> other) { return MergeAverage(other); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public HsbColor32Normalized MergeAverage(IEnumerable<IColorModel> other)
        {
            throw new NotImplementedException();
        }

        IColorModel IColorModel.MergeAverage(IEnumerable<IColorModel> other) { return MergeAverage(other); }

        #endregion
        
        #region ShiftHue Method

        /// <summary>
        /// Returns a <see cref="IHsbColorModel{T}" /> with <seealso cref="byte" /> component values with the color hue adjusted.
        /// </summary>
        /// <param name="degrees">The number of degrees to shift the hue value, ranging from -360.0 to 360.0. A positive value shifts the hue in the red-to-cyan direction, and a negative value shifts the hue in the cyan-to-red direction.</param>
        /// <returns>A <see cref="IHsbColorModel{T}" /> with <seealso cref="byte" /> component values with the color hue adjusted.</returns>
        /// <remarks>The values 0.0, -360.0 and 360.0 have no effect since they would result in no hue change.</remarks>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="degrees" /> is less than -360.0 or <paramref name="degrees" /> is greater than 360.0.</exception>
        public readonly IHsbColorModel<byte> ShiftHue(float degrees)
        {
            if (degrees < -360f || degrees > 360f)
                throw new ArgumentOutOfRangeException(nameof(degrees));
            if (degrees == 0f || degrees == 360f || degrees == -360f)
                return this;
            float hue = _hue.ToDegrees() + degrees;
            if (hue < 0f)
                hue += 360f;
            else if (hue >= 360f)
                hue -= 360f;
            return new HsbColor32(hue.FromDegrees(), _saturation, _brightness, _alpha);
        }

        readonly IColorModel<byte> IColorModel<byte>.ShiftHue(float degrees) { return ShiftHue(degrees); }

        readonly IColorModel IColorModel.ShiftHue(float degrees) { return ShiftHue(degrees); }

        #endregion
        
        #region ShiftSaturation Method

        /// <summary>
        /// Returns a <see cref="IHsbColorModel{T}" /> with <seealso cref="byte" /> component values with the color saturation adjusted.
        /// </summary>
        /// <param name="percentage">The percentage to saturate the color, ranging from -1.0 to 1.0. A positive value increases saturation, a negative value decreases saturation and a zero vale has no effect.</param>
        /// <returns>A <see cref="IHsbColorModel{T}" /> with <seealso cref="byte" /> component values with the color saturation adjusted.</returns>
        /// <remarks>For positive values, the target saturation value is determined using the following formula: <c>saturation + (MAX_VALUE - saturation) * percentage</c>
        /// <para>For negative values, the target saturation value is determined using the following formula: <c>saturation + saturation * percentage</c></para></remarks>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="percentage" /> is less than -1.0 or <paramref name="percentage" /> is greater than 1.0.</exception>
        public readonly IHsbColorModel<byte> ShiftSaturation(float percentage)
        {
            if (percentage < -1f || percentage > 1f)
                throw new ArgumentOutOfRangeException(nameof(percentage));
            if (percentage == 0f || (percentage == 1f) ? _saturation == 255 : percentage == -1f && _saturation == 0)
                return this;
            float s = _saturation.ToPercentage();
            return new HsbColor32(_hue, (s + ((percentage > 0f) ? (1f - s) : s) * percentage).FromPercentage(), _brightness, _alpha);
        }

        readonly IColorModel<byte> IColorModel<byte>.ShiftSaturation(float percentage) { return ShiftSaturation(percentage); }

        readonly IColorModel IColorModel.ShiftSaturation(float percentage) { return ShiftSaturation(percentage); }

        #endregion
        
        #region ShiftBrightness Method

        /// <summary>
        /// Returns a <see cref="IHsbColorModel{T}" /> with <seealso cref="byte" /> component values with the color brightness adjusted.
        /// </summary>
        /// <param name="percentage">The percentage to saturate the color, ranging from -1.0 to 1.0. A positive value increases brightness, a negative value decreases brightness and a zero vale has no effect.</param>
        /// <returns>A <see cref="IHsbColorModel{T}" /> with <seealso cref="byte" /> component values with the color brightness adjusted.</returns>
        /// <remarks>For positive values, the target brightness value is determined using the following formula: <c>brightness + (MAX_VALUE - brightness) * percentage</c>
        /// <para>For negative values, the target brightness value is determined using the following formula: <c>brightness + brightness * percentage</c></para></remarks>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="percentage" /> is less than -1.0 or <paramref name="percentage" /> is greater than 1.0.</exception>
        public readonly IHsbColorModel<byte> ShiftBrightness(float percentage)
        {
            if (percentage < -1f || percentage > 1f)
                throw new ArgumentOutOfRangeException(nameof(percentage));
            if (percentage == 0f || (percentage == 1f) ? _brightness == 255 : percentage == -1f && _brightness == 0)
                return this;
            float b = _brightness.ToPercentage();
            return new HsbColor32(_hue, _saturation, (b + ((percentage > 0f) ? (1f - b) : b) * percentage).FromPercentage(), _alpha);
        }

        readonly IColorModel<byte> IColorModel<byte>.ShiftBrightness(float percentage) { return ShiftBrightness(percentage); }

        readonly IColorModel IColorModel.ShiftBrightness(float percentage) { return ShiftBrightness(percentage); }

        #endregion

        /// <summary>
        /// Gets the AHSB integer value for the current <see cref="HsbColor32Normalized" /> value.
        /// </summary>
        /// <returns>The AHSB integer value for the current <see cref="HsbColor32Normalized" /> value.</returns>
        public readonly int ToAHSB() { return _value; }

        #region ToString Methods

        /// <summary>
        /// Gets formatted string representing the current color value.
        /// </summary>
        /// <param name="format">The color string format to use.</param>
        /// <returns>The formatted string representing the current color value.</returns>
        public readonly string ToString(ColorStringFormat format)
        {
            float r, g, b;
            switch (format)
            {
                case ColorStringFormat.HSLAHexOpt:
                    return HsbColor32.ToHexidecimalString(_hue, _saturation, _brightness, _alpha, true);
                case ColorStringFormat.HSLAPercent:
                    return HsbColorF.ToPercentParameterString(_hue.ToDegrees(), _saturation.ToPercentage(), _brightness.ToPercentage(), _alpha.ToPercentage());
                case ColorStringFormat.HSLAValues:
                    return HsbColor32.ToValueParameterString(_hue, _saturation, _brightness, _alpha);
                case ColorStringFormat.HSLHex:
                    return HsbColor32.ToHexidecimalString(_hue, _saturation, _brightness, false);
                case ColorStringFormat.HSLHexOpt:
                    return HsbColor32.ToHexidecimalString(_hue, _saturation, _brightness, true);
                case ColorStringFormat.HSLPercent:
                    return HsbColorF.ToPercentParameterString(_hue.ToDegrees(), _saturation.ToPercentage(), _brightness.ToPercentage());
                case ColorStringFormat.HSLValues:
                    return HsbColor32.ToValueParameterString(_hue, _saturation, _brightness);
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
            return _value.ToString("X8");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override readonly string ToString() { return _value.ToString("X8"); }

        #endregion

        #region  IConvertible Members

        readonly TypeCode IConvertible.GetTypeCode() { return TypeCode.Int32; }

        readonly bool IConvertible.ToBoolean(IFormatProvider provider) { return Convert.ToBoolean(_value, provider); }

        readonly char IConvertible.ToChar(IFormatProvider provider) { return Convert.ToChar(_value, provider); }

        readonly sbyte IConvertible.ToSByte(IFormatProvider provider) { return Convert.ToSByte(_value, provider); }

        readonly byte IConvertible.ToByte(IFormatProvider provider) { return Convert.ToByte(_value, provider); }

        readonly short IConvertible.ToInt16(IFormatProvider provider) { return Convert.ToInt16(_value, provider); }

        readonly ushort IConvertible.ToUInt16(IFormatProvider provider) { return Convert.ToUInt16(_value, provider); }

        readonly int IConvertible.ToInt32(IFormatProvider provider) { return _value; }

        readonly uint IConvertible.ToUInt32(IFormatProvider provider) { return Convert.ToUInt32(_value, provider); }

        readonly long IConvertible.ToInt64(IFormatProvider provider) { return _value; }

        readonly ulong IConvertible.ToUInt64(IFormatProvider provider) { return Convert.ToUInt64(_value, provider); }

        readonly float IConvertible.ToSingle(IFormatProvider provider) { return _value; }

        readonly double IConvertible.ToDouble(IFormatProvider provider) { return _value; }

        readonly decimal IConvertible.ToDecimal(IFormatProvider provider) { return _value; }

        readonly DateTime IConvertible.ToDateTime(IFormatProvider provider) { return Convert.ToDateTime(_value, provider); }

        readonly string IConvertible.ToString(IFormatProvider provider) { return Convert.ToString(_value, provider); }

        readonly object IConvertible.ToType(Type conversionType, IFormatProvider provider) { return Convert.ChangeType(_value, conversionType, provider); }

        #endregion
    }
}
