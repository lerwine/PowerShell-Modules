using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Runtime.InteropServices;

namespace Erwine.Leonard.T.GDIPlus
{
    [StructLayout(LayoutKind.Sequential)]
    public struct HsbColorFNormalized : IEquatable<HsbColorFNormalized>, IEquatable<IHsbColorModel<byte>>, IEquatable<IRgbColorModel<byte>>, IHsbColorModel<float>
    {
        private readonly float _alpha, _hue, _saturation, _brightness;

        #region Properties

        /// <summary>
        /// The opaqueness of the color.
        /// </summary>
        public float Alpha { get { return _alpha; } }

        /// <summary>
        /// The hue of the color.
        /// </summary>
        public float Hue { get { return _hue; } }

        /// <summary>
        /// The color saturation.
        /// </summary>
        public float Saturation { get { return _saturation; } }

        /// <summary>
        /// The brightness of the color.
        /// </summary>
        public float Brightness { get { return _brightness; } }

        bool IColorModel.IsNormalized { get { return true; } }

        ColorStringFormat IColorModel.DefaultStringFormat { get { return ColorStringFormat.HSLAPercent; } }

        #endregion
        
        #region Constructors

        public HsbColorFNormalized(float hue, float saturation, float brightness, float alpha)
        {
            if (hue < 0f || hue > ColorExtensions.HUE_MAXVALUE)
                throw new ArgumentOutOfRangeException("hue");
            if (saturation < 0f || saturation > 1f)
                throw new ArgumentOutOfRangeException("saturation");
            if (brightness < 0f || brightness > 1f)
                throw new ArgumentOutOfRangeException("brightness");
            if (alpha < 0f || alpha > 1f)
                throw new ArgumentOutOfRangeException("alpha");
            ColorExtensions.HSBtoRGB((hue == ColorExtensions.HUE_MAXVALUE) ? 0f : hue, saturation, brightness, out float r, out float g, out float b);
            ColorExtensions.RGBtoHSB(r, g, b, out hue, out saturation, out brightness);
            _hue = hue;
            _saturation = saturation;
            _brightness = brightness;
            _alpha = alpha;
        }

        public HsbColorFNormalized(float hue, float saturation, float brightness) : this(hue, saturation, brightness, 1f) { }

        public HsbColorFNormalized(RgbColorF value)
        {
            ColorExtensions.RGBtoHSB(value.Red, value.Green, value.Blue, out float h, out float s, out float b);
            _hue = h;
            _saturation = s;
            _brightness = b;
            _alpha = value.Alpha;
        }

        public HsbColorFNormalized(RgbColor32 value)
        {
            ColorExtensions.RGBtoHSB(value.Red.ToPercentage(), value.Green.ToPercentage(), value.Blue.ToPercentage(), out float h, out float s, out float b);
            _hue = h;
            _saturation = s;
            _brightness = b;
            _alpha = value.Alpha.ToPercentage();
        }

        public HsbColorFNormalized(HsbColorF value)
        {
            ColorExtensions.HSBtoRGB(value.Hue, value.Saturation, value.Brightness, out float r, out float g, out float b);
            ColorExtensions.RGBtoHSB(r, g, b, out float hue, out float saturation, out b);
            _hue = hue;
            _saturation = saturation;
            _brightness = b;
            _alpha = value.Alpha;
        }

        public HsbColorFNormalized(IHsbColorModel<byte> value)
        {
            if (value == null)
                throw new ArgumentNullException();
            if (value.Hue < 0f || value.Hue > ColorExtensions.HUE_MAXVALUE)
                throw new ArgumentOutOfRangeException("hue");
            if (value.Saturation < 0f || value.Saturation > 1f)
                throw new ArgumentOutOfRangeException("saturation");
            if (value.Brightness < 0f || value.Brightness > 1f)
                throw new ArgumentOutOfRangeException("brightness");
            if (value.Alpha < 0f || value.Alpha > 1f)
                throw new ArgumentOutOfRangeException("alpha");
            ColorExtensions.HSBtoRGB((value.Hue == ColorExtensions.HUE_MAXVALUE) ? 0f : value.Hue, value.Saturation, value.Brightness, out float r, out float g, out float b);
            ColorExtensions.RGBtoHSB(r, g, b, out float hue, out float saturation, out float brightness);
            _hue = hue;
            _saturation = saturation;
            _brightness = brightness;
            _alpha = value.Alpha;
        }

        public HsbColorFNormalized(int ahsb)
        {
            byte[] values = BitConverter.GetBytes(ahsb);
            ColorExtensions.HSBtoRGB(values[2].ToDegrees(), values[1].ToPercentage(), values[0].ToPercentage(), out float r, out float g, out float b);
            ColorExtensions.RGBtoHSB(r, g, b, out float hue, out float saturation, out float brightness);
            _hue = hue;
            _saturation = saturation;
            _brightness = brightness;
            _alpha = values[3].ToPercentage();
        }

        #endregion
        
        #region As* Methods

        public HsbColor32Normalized AsHsb32() { return new HsbColor32Normalized(this); }

        IHsbColorModel<byte> IColorModel.AsHsb32() { return AsHsb32(); }

        IHsbColorModel<float> IColorModel.AsHsbF() { return this; }

        public RgbColor32 AsRgb32() { return new RgbColor32(this); }

        IRgbColorModel<byte> IColorModel.AsRgb32() { return AsRgb32(); }

        public RgbColorF AsRgbF() { return new RgbColorF(this); }

        IRgbColorModel<float> IColorModel.AsRgbF() { return AsRgbF(); }

        IHsbColorModel<float> IHsbColorModel<float>.AsNormalized() { return this; }

        IColorModel<float> IColorModel<float>.AsNormalized() { return this; }

        IColorModel IColorModel.AsNormalized() { return this; }

        #endregion
        
        #region Equals Methods

        public bool Equals(IRgbColorModel<float> other, bool exact)
        {
            if (other == null || _alpha != other.Alpha)
                return false;

            float b;
            if (exact)
            {
                ColorExtensions.RGBtoHSB(other.Red, other.Green, other.Blue, out float h, out float s, out b);
                return _hue == h && _saturation == s && _brightness == b;
            }

            ColorExtensions.HSBtoRGB(_hue, _saturation, _brightness, out float r, out float g, out b);
            return other.Red == r && other.Green == g && other.Blue == b;
        }

        public bool Equals(IRgbColorModel<byte> other, bool exact)
        {
            if (other == null)
                return false;

            float b;
            if (exact)
            {
                if (other.Alpha.ToPercentage() != _alpha)
                    return false;
                ColorExtensions.RGBtoHSB(other.Red.ToPercentage(), other.Green.ToPercentage(), other.Blue.ToPercentage(), out float h, out float s, out b);
                return _hue == h && _saturation == s && _brightness == b;
            }

            if (other.Alpha != _alpha.FromPercentage())
                return false;

            ColorExtensions.HSBtoRGB(_hue, _saturation, _brightness, out float r, out float g, out b);
            return other.Red == r.FromPercentage() && other.Green == g.FromPercentage() && other.Blue == b.FromPercentage();
        }

        public bool Equals(IHsbColorModel<byte> other, bool exact)
        {
            if (other == null)
                return false;
            if (exact)
                return _alpha == other.Alpha.ToPercentage() && _hue == other.Hue.ToDegrees() && _saturation == other.Saturation.ToPercentage() && _brightness == other.Brightness.ToPercentage();
            if (!other.IsNormalized)
                other = other.AsNormalized();
            return _alpha.FromPercentage() == other.Alpha && _hue.FromDegrees() == other.Hue && _saturation.FromPercentage() == other.Saturation && _brightness.FromPercentage() == other.Brightness;
        }

        public bool Equals(IColorModel other, bool exact)
        {
            if (other == null)
                return false;
            if (other is HsbColorFNormalized)
                return Equals((HsbColorFNormalized)other);
            if (other is IHsbColorModel<float>)
                return Equals((IHsbColorModel<float>)other);
            if (other is IHsbColorModel<byte>)
                return Equals((IHsbColorModel<byte>)other, exact);
            if (other is IRgbColorModel<float>)
                return Equals((IRgbColorModel<float>)other, exact);
            return other is IRgbColorModel<byte> && Equals((IRgbColorModel<byte>)other, exact);
        }

        public bool Equals(HsbColorFNormalized other) { return other._alpha == _alpha && other._hue == _hue && other._saturation == _saturation && other._brightness == _brightness; }

        public bool Equals(IHsbColorModel<byte> other) { return Equals(other, false); }

        public bool Equals(IRgbColorModel<byte> other) { return Equals(other, false); }

        public bool Equals(IHsbColorModel<float> other) { return other.Alpha == _alpha && other.Hue == _hue && other.Saturation == _saturation && other.Brightness == _brightness; }

        public bool Equals(IRgbColorModel<float> other) { return Equals(other, false); }

        public bool Equals(IColorModel other) { return Equals(other, false); }

        public bool Equals(System.Drawing.Color other)
        {
            if (other.A != _alpha.FromPercentage())
                return false;
            ColorExtensions.HSBtoRGB(_hue, _saturation, _brightness, out float r, out float g, out float b);
            return r.FromPercentage() == other.R && g.FromPercentage() == other.G && b.FromPercentage() == other.B;
        }

        public bool Equals(System.Windows.Media.Color other)
        {
            if (other.A != _alpha.FromPercentage())
                return false;
            ColorExtensions.HSBtoRGB(_hue, _saturation, _brightness, out float r, out float g, out float b);
            return r.FromPercentage() == other.R && g.FromPercentage() == other.G && b.FromPercentage() == other.B;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj is PSObject)
                obj = ((PSObject)obj).BaseObject;
            if (obj is HsbColorFNormalized)
                return Equals((HsbColorFNormalized)obj);
            if (obj is IHsbColorModel<float>)
                return Equals((IHsbColorModel<float>)obj);
            if (obj is IHsbColorModel<byte>)
                return Equals((IHsbColorModel<byte>)obj, false);
            if (obj is IRgbColorModel<float>)
                return Equals((IRgbColorModel<float>)obj, false);
            return obj is IRgbColorModel<byte> && Equals((IRgbColorModel<byte>)obj, false);
        }

        #endregion
        
        public override int GetHashCode() { return ToAHSB(); }

        #region MergeAverage Method

        public HsbColorFNormalized MergeAverage(IEnumerable<IHsbColorModel<float>> other)
        {
            if (other == null)
                return this;

            ColorExtensions.HSBtoRGB(_hue, _saturation, _brightness, out float rF, out float gF, out float bF);
            double r = rF, g = gF, b = bF, a = _alpha;
            double brightness = _brightness;
            int count = 0;
            foreach (IHsbColorModel<float> item in other)
            {
                if (item != null)
                {
                    count++;
                    ColorExtensions.HSBtoRGB(item.Hue, item.Saturation, item.Brightness, out rF, out gF, out bF);
                    r += (double)rF;
                    g += (double)gF;
                    b += (double)bF;
                    a += (double)item.Alpha;
                    brightness += (double)item.Brightness;
                }
            }
            if (count == 0)
                return this;

            ColorExtensions.RGBtoHSB((float)(r / (double)count), (float)(r / (double)count), (float)(r / (double)count), out float h, out float s, out bF);
            return new HsbColorFNormalized(h, s, bF, (float)(a / (double)count));
        }

        IHsbColorModel<float> IHsbColorModel<float>.MergeAverage(IEnumerable<IHsbColorModel<float>> other) { return MergeAverage(other); }

        public HsbColorFNormalized MergeAverage(IEnumerable<IColorModel> other)
        {
            throw new NotImplementedException();
        }

        IColorModel IColorModel.MergeAverage(IEnumerable<IColorModel> other) { return MergeAverage(other); }

        #endregion
        
        #region ShiftHue Method

        /// <summary>
        /// Returns a <see cref="IHsbColorModel{T}" /> with <seealso cref="float" /> component values with the color hue adjusted.
        /// </summary>
        /// <param name="degrees">The number of degrees to shift the hue value, ranging from -360.0 to 360.0. A positive value shifts the hue in the red-to-cyan direction, and a negative value shifts the hue in the cyan-to-red direction.</param>
        /// <returns>A <see cref="IHsbColorModel{T}" /> with <seealso cref="float" /> with the color hue adjusted.</returns>
        /// <remarks>The values 0.0, -360.0 and 360.0 have no effect since they would result in no hue change.</remarks>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="degrees" /> is less than -360.0 or <paramref name="degrees" /> is greater than 360.0.</exception>
        public IHsbColorModel<float> ShiftHue(float degrees)
        {
            if (degrees < -360f || degrees > 360f)
                throw new ArgumentOutOfRangeException("degrees");
            if (degrees == 0f || degrees == 360f || degrees == -360f)
                return this;
            float hue = _hue + degrees;
            if (hue < 0f)
                hue += 360f;
            else if (hue >= 360f)
                hue -= 360f;
            return new HsbColorF(hue, _saturation, _brightness, _alpha);
        }

        IColorModel<float> IColorModel<float>.ShiftHue(float degrees) { return ShiftHue(degrees); }

        IColorModel IColorModel.ShiftHue(float degrees) { return ShiftHue(degrees); }

        #endregion
        
        #region ShiftSaturation Method

        /// <summary>
        /// Returns a <see cref="IHsbColorModel{T}" /> with <seealso cref="float" /> with the color saturation adjusted.
        /// </summary>
        /// <param name="percentage">The percentage to saturate the color, ranging from -1.0 to 1.0. A positive value increases saturation, a negative value decreases saturation and a zero vale has no effect.</param>
        /// <returns>A <see cref="IHsbColorModel{T}" /> with <seealso cref="float" /> value with the color saturation adjusted.</returns>
        /// <remarks>For positive values, the target saturation value is determined using the following formula: <c>saturation + (1.0 - saturation) * percentage</c>
        /// <para>For negative values, the target saturation value is determined using the following formula: <c>saturation + saturation * percentage</c></para></remarks>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="percentage" /> is less than -1.0 or <paramref name="percentage" /> is greater than 1.0.</exception>
        public IHsbColorModel<float> ShiftSaturation(float percentage)
        {
            if (percentage < -1f || percentage > 1f)
                throw new ArgumentOutOfRangeException("percentage");
            if (percentage == 0f || (percentage == 1f) ? _saturation == 1f : percentage == -1f && _saturation == 0f)
                return this;
            return new HsbColorF(_hue, _saturation + ((percentage > 0f) ? (1f - _saturation) : _saturation) * percentage, _brightness, _alpha);
        }

        IColorModel<float> IColorModel<float>.ShiftSaturation(float percentage) { return ShiftSaturation(percentage); }

        IColorModel IColorModel.ShiftSaturation(float percentage) { return ShiftSaturation(percentage); }

        #endregion
        
        #region ShiftBrightness Method

        /// <summary>
        /// Returns a <see cref="IHsbColorModel{T}" /> with <seealso cref="float" /> value with the color brightness adjusted.
        /// </summary>
        /// <param name="percentage">The percentage to saturate the color, ranging from -1.0 to 1.0. A positive value increases brightness, a negative value decreases brightness and a zero vale has no effect.</param>
        /// <returns>A <see cref="IHsbColorModel{T}" /> with <seealso cref="float" /> value with the color brightness adjusted.</returns>
        /// <remarks>For positive values, the target brightness value is determined using the following formula: <c>brightness + (1.0 - brightness) * percentage</c>
        /// <para>For negative values, the target brightness value is determined using the following formula: <c>brightness + brightness * percentage</c></para></remarks>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="percentage" /> is less than -1.0 or <paramref name="percentage" /> is greater than 1.0.</exception>
        public IHsbColorModel<float> ShiftBrightness(float percentage)
        {
            if (percentage < -1f || percentage > 1f)
                throw new ArgumentOutOfRangeException("percentage");
            if (percentage == 0f || (percentage == 1f) ? _brightness == 1f : percentage == -1f && _brightness == 0f)
                return this;
            return new HsbColorF(_hue, _saturation, _brightness + ((percentage > 0f) ? (1f - _brightness) : _brightness) * percentage, _alpha);
        }

        IColorModel<float> IColorModel<float>.ShiftBrightness(float percentage) { return ShiftBrightness(percentage); }

        IColorModel IColorModel.ShiftBrightness(float percentage) { return ShiftBrightness(percentage); }

        #endregion

        /// <summary>
        /// Gets the AHSB integer value for the current <see cref="HsbColorFNormalized" /> value.
        /// </summary>
        /// <returns>The AHSB integer value for the current <see cref="HsbColorFNormalized" /> value.</returns>
        public int ToAHSB() { return BitConverter.ToInt32(new byte[] { _brightness.FromPercentage(), _saturation.FromPercentage(), _hue.FromPercentage(), _alpha.FromPercentage() }, 0); }

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
                case ColorStringFormat.HSLAHex:
                    return HsbColor32.ToHexidecimalString(_hue.FromDegrees(), _saturation.FromPercentage(), _brightness.FromPercentage(), _alpha.FromPercentage(), false);
                case ColorStringFormat.HSLAHexOpt:
                    return HsbColor32.ToHexidecimalString(_hue.FromDegrees(), _saturation.FromPercentage(), _brightness.FromPercentage(), _alpha.FromPercentage(), true);
                case ColorStringFormat.HSLAValues:
                    return HsbColor32.ToValueParameterString(_hue.FromDegrees(), _saturation.FromPercentage(), _brightness.FromPercentage(), _alpha);
                case ColorStringFormat.HSLHex:
                    return HsbColor32.ToHexidecimalString(_hue.FromDegrees(), _saturation.FromPercentage(), _brightness.FromPercentage(), false);
                case ColorStringFormat.HSLHexOpt:
                    return HsbColor32.ToHexidecimalString(_hue.FromDegrees(), _saturation.FromPercentage(), _brightness.FromPercentage(), true);
                case ColorStringFormat.HSLPercent:
                    return HsbColorF.ToPercentParameterString(_hue, _saturation, _brightness);
                case ColorStringFormat.HSLValues:
                    return HsbColor32.ToValueParameterString(_hue.FromDegrees(), _saturation.FromPercentage(), _brightness.FromPercentage());
                case ColorStringFormat.RGBAHex:
                    ColorExtensions.HSBtoRGB(_hue, _saturation, _brightness, out r, out g, out b);
                    return RgbColor32.ToHexidecimalString(r.FromPercentage(), g.FromPercentage(), b.FromPercentage(), _alpha.FromPercentage(), false);
                case ColorStringFormat.RGBAHexOpt:
                    ColorExtensions.HSBtoRGB(_hue, _saturation, _brightness, out r, out g, out b);
                    return RgbColor32.ToHexidecimalString(r.FromPercentage(), g.FromPercentage(), b.FromPercentage(), _alpha.FromPercentage(), true);
                case ColorStringFormat.RGBAPercent:
                    ColorExtensions.HSBtoRGB(_hue, _saturation, _brightness, out r, out g, out b);
                    return RgbColorF.ToPercentParameterString(r, g, b, _alpha);
                case ColorStringFormat.RGBAValues:
                    ColorExtensions.HSBtoRGB(_hue, _saturation, _brightness, out r, out g, out b);
                    return RgbColor32.ToValueParameterString(r.FromPercentage(), g.FromPercentage(), b.FromPercentage(), _alpha.FromPercentage());
                case ColorStringFormat.RGBHex:
                    ColorExtensions.HSBtoRGB(_hue, _saturation, _brightness, out r, out g, out b);
                    return RgbColor32.ToHexidecimalString(r.FromPercentage(), g.FromPercentage(), b.FromPercentage(), false);
                case ColorStringFormat.RGBHexOpt:
                    ColorExtensions.HSBtoRGB(_hue, _saturation, _brightness, out r, out g, out b);
                    return RgbColor32.ToHexidecimalString(r.FromPercentage(), g.FromPercentage(), b.FromPercentage(), true);
                case ColorStringFormat.RGBPercent:
                    ColorExtensions.HSBtoRGB(_hue, _saturation, _brightness, out r, out g, out b);
                    return RgbColorF.ToPercentParameterString(r, g, b);
                case ColorStringFormat.RGBValues:
                    ColorExtensions.HSBtoRGB(_hue, _saturation, _brightness, out r, out g, out b);
                    return RgbColor32.ToValueParameterString(r.FromPercentage(), g.FromPercentage(), b.FromPercentage());
            }
            return HsbColorF.ToPercentParameterString(_hue, _saturation, _brightness, _alpha);
        }

        public override string ToString() { return HsbColorF.ToPercentParameterString(_hue, _saturation, _brightness, _alpha); }

        #endregion
    }
}
