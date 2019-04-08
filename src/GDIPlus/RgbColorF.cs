using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Runtime.InteropServices;

namespace Erwine.Leonard.T.GDIPlus
{
    [StructLayout(LayoutKind.Sequential)]
    public class RgbColorF : IEquatable<RgbColorF>, IEquatable<IRgbColorModel<byte>>, IEquatable<IHsbColorModel<byte>>, IRgbColorModel<float>
    {
        private readonly float _alpha, _red, _green, _blue;

        /// <summary>
        /// The opaqueness of the color.
        /// </summary>
        public float Alpha { get { return _alpha; } }

        /// <summary>
        /// The intensity of the red layer.
        /// </summary>
        public float Red { get { return _red; } }

        /// <summary>
        /// The intensity of the green layer.
        /// </summary>
        public float Green { get { return _green; } }

        /// <summary>
        /// The intensity of the blue layer.
        /// </summary>
        public float Blue { get { return _blue; } }

        bool IColorModel.IsNormalized { get { return true; } }

        public RgbColorF(float red, float green, float blue, float alpha)
        {
            if (red < 0f || red > 1f)
                throw new ArgumentOutOfRangeException("red");
            if (green < 0f || green > 1f)
                throw new ArgumentOutOfRangeException("green");
            if (blue < 0f || blue > 1f)
                throw new ArgumentOutOfRangeException("blue");
            if (alpha < 0f || alpha > 1f)
                throw new ArgumentOutOfRangeException("alpha");
            _red = red;
            _green = green;
            _blue = blue;
            _alpha = alpha;
        }

        public RgbColorF(float red, float green, float blue) : this(red, green, blue, 1f) { }

        public RgbColorF(RgbColor32 value)
        {
            _red = value.Red.ToPercentage();
            _green = value.Green.ToPercentage();
            _blue = value.Blue.ToPercentage();
            _alpha = value.Alpha.ToPercentage();
        }

        public RgbColorF(IHsbColorModel<float> value)
        {
            if (value == null)
                throw new ArgumentNullException();
            if (value.Alpha < 0f || value.Alpha > 1f)
                throw new ArgumentOutOfRangeException("value", "Value for alpha is out of range");
            try
            {
                ColorExtensions.HSBtoRGB(value.Hue, value.Saturation, value.Brightness, out float r, out float g, out float b);
                _red = r;
                _green = g;
                _blue = b;
            }
            catch (ArgumentOutOfRangeException exc) { throw new ArgumentOutOfRangeException("value", "Value for " + exc.ParamName + " is out of range"); }
            _alpha = value.Alpha;
        }

        public RgbColorF(IHsbColorModel<byte> value)
        {
            if (value == null)
                throw new ArgumentNullException();
            ColorExtensions.HSBtoRGB(value.Hue.ToDegrees(), value.Saturation.ToPercentage(), value.Brightness.ToPercentage(), out float r, out float g, out float b);
            _red = r;
            _green = g;
            _blue = b;
            _alpha = value.Alpha.ToPercentage();
        }

        public HsbColor32Normalized AsHsb32() { return new HsbColor32Normalized(this); }

        IHsbColorModel<byte> IColorModel.AsHsb32() { return AsHsb32(); }

        public HsbColorFNormalized AsHsbF() { return new HsbColorFNormalized(this); }

        IHsbColorModel<float> IColorModel.AsHsbF() { return AsHsbF(); }

        public RgbColor32 AsRgb32() { return new RgbColor32(this); }

        IRgbColorModel<byte> IColorModel.AsRgb32() { return AsRgb32(); }

        IRgbColorModel<float> IColorModel.AsRgbF() { return this; }

        IColorModel<float> IColorModel<float>.AsNormalized() { return this; }

        IColorModel IColorModel.AsNormalized() { return this; }

        public bool Equals(IHsbColorModel<float> other, bool exact)
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

        public bool Equals(IHsbColorModel<byte> other, bool exact)
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

        public bool Equals(IRgbColorModel<byte> other, bool exact)
        {
            if (other == null)
                return false;

            if (exact)
                return _alpha == other.Alpha.ToPercentage() && _red == other.Red.ToPercentage() && _green== other.Green.ToPercentage() && _blue== other.Blue.ToPercentage();
            return _alpha.FromPercentage() == other.Alpha && _red.FromPercentage() == other.Red && _green.FromPercentage() == other.Green && _blue.FromPercentage() == other.Blue;
        }

        public bool Equals(IColorModel other, bool exact)
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

        public bool Equals(RgbColorF other) { return other._alpha == _alpha && other._red == _red && other._green == _green && other._blue == _blue; }

        public bool Equals(IRgbColorModel<byte> other) { return Equals(other, false); }

        public bool Equals(IHsbColorModel<byte> other) { return Equals(other, false); }

        public bool Equals(IRgbColorModel<float> other) { return other.Alpha == _alpha && other.Red == _red && other.Green == _green && other.Blue == _blue; }

        public bool Equals(IHsbColorModel<float> other) { return Equals(other, false); }

        public bool Equals(IColorModel other) { return Equals(other, false); }

        public bool Equals(System.Drawing.Color other) { return other.A == _alpha.FromPercentage() && other.R == _red.FromPercentage() && other.G == _green.FromPercentage() && other.B == _blue.FromPercentage(); }

        public bool Equals(System.Windows.Media.Color other) { return other.A == _alpha.FromPercentage() && other.R == _red.FromPercentage() && other.G == _green.FromPercentage() && other.B == _blue.FromPercentage(); }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj is PSObject)
                obj = ((PSObject)obj).BaseObject;
            if (obj is RgbColorF)
                return Equals((RgbColorF)obj);
            if (obj is IRgbColorModel<float>)
                return Equals((IRgbColorModel<float>)obj);
            if (obj is IRgbColorModel<byte>)
                return Equals((IRgbColorModel<byte>)obj, false);
            if (obj is IHsbColorModel<float>)
                return Equals((IHsbColorModel<float>)obj, false);
            return obj is IHsbColorModel<byte> && Equals((IHsbColorModel<byte>)obj, false);
        }

        public override int GetHashCode() { return BitConverter.ToInt32(new byte[] { _red.FromPercentage(), _green.FromPercentage(), _blue.FromPercentage(), _alpha.FromPercentage() }, 0); }

        public RgbColorF MergeAverage(IEnumerable<IRgbColorModel<float>> other)
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

        IRgbColorModel<float> IRgbColorModel<float>.MergeAverage(IEnumerable<IRgbColorModel<float>> other) { return MergeAverage(other); }

        public RgbColorF MergeAverage(IEnumerable<IColorModel> other)
        {
            throw new NotImplementedException();
        }

        IColorModel IColorModel.MergeAverage(IEnumerable<IColorModel> other) { return MergeAverage(other); }

        public IRgbColorModel<float> ShiftHue(float percentage)
        {
            throw new NotImplementedException();
        }

        public IRgbColorModel<float> ShiftSaturation(float percentage)
        {
            throw new NotImplementedException();
        }

        public IRgbColorModel<float> ShiftBrightness(float percentage)
        {
            throw new NotImplementedException();
        }

        IColorModel<float> IColorModel<float>.ShiftHue(float percentage)
        {
            throw new NotImplementedException();
        }

        IColorModel<float> IColorModel<float>.ShiftSaturation(float percentage)
        {
            throw new NotImplementedException();
        }

        IColorModel<float> IColorModel<float>.ShiftBrightness(float percentage)
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