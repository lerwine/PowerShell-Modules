using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Runtime.InteropServices;

namespace Erwine.Leonard.T.GDIPlus
{
    /// <summary>
    /// Model representing a color in terms of hue, saturation and brightness as floating-point values.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct HsbColorF : IEquatable<HsbColorF>, IEquatable<IHsbColorModel<byte>>, IEquatable<IRgbColorModel<byte>>, IHsbColorModel<float>
    {
        private readonly float _alpha, _hue, _saturation, _brightness;

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

        bool IColorModel.IsNormalized { get { return false; } }

        public HsbColorF(float hue, float saturation, float brightness, float alpha)
        {
            if (hue < 0f || hue > ColorExtensions.HUE_MAXVALUE)
                throw new ArgumentOutOfRangeException("hue");
            if (saturation < 0f || saturation > 1f)
                throw new ArgumentOutOfRangeException("saturation");
            if (brightness < 0f || brightness > 1f)
                throw new ArgumentOutOfRangeException("brightness");
            if (alpha < 0f || alpha > 1f)
                throw new ArgumentOutOfRangeException("alpha");
            _hue = (hue == ColorExtensions.HUE_MAXVALUE) ? 0f : hue;
            _saturation = saturation;
            _brightness = brightness;
            _alpha = alpha;
        }

        public HsbColorF(float hue, float saturation, float brightness) : this(hue, saturation, brightness, 1f) { }

        public HsbColorF(HsbColor32 value)
        {
            _hue = value.Hue.ToDegrees();
            _saturation = value.Saturation.ToPercentage();
            _brightness = value.Brightness.ToPercentage();
            _alpha = value.Alpha.ToPercentage();
        }

        public HsbColor32 AsHsb32() { return new HsbColor32(this); }

        IHsbColorModel<byte> IColorModel.AsHsb32() { return AsHsb32(); }

        IHsbColorModel<float> IColorModel.AsHsbF() { return this; }

        public RgbColor32 AsRgb32() { return new RgbColor32(this); }

        IRgbColorModel<byte> IColorModel.AsRgb32() { return AsRgb32(); }

        public RgbColorF AsRgbF() { return new RgbColorF(this); }

        IRgbColorModel<float> IColorModel.AsRgbF() { return AsRgbF(); }

        public HsbColorFNormalized AsNormalized() { return new HsbColorFNormalized(this); }

        IHsbColorModel<float> IHsbColorModel<float>.AsNormalized() { return AsNormalized(); }

        IColorModel<float> IColorModel<float>.AsNormalized() { return AsNormalized(); }

        IColorModel IColorModel.AsNormalized() { return AsNormalized(); }

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

            return AsNormalized().Equals(other, false);
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
            if (other is HsbColorF)
                return Equals((HsbColorF)other);
            if (other is IHsbColorModel<float>)
                return Equals((IHsbColorModel<float>)other);
            if (other is IHsbColorModel<byte>)
                return Equals((IHsbColorModel<byte>)other, exact);
            if (other is IRgbColorModel<float>)
                return Equals((IRgbColorModel<float>)other, exact);
            return other is IRgbColorModel<byte> && Equals((IRgbColorModel<byte>)other, exact);
        }

        public bool Equals(HsbColorF other) { return other._alpha == _alpha && other._hue == _hue && other._saturation   == _saturation && other._brightness == _brightness; }

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
            if (obj is HsbColorF)
                return Equals((HsbColorF)obj);
            if (obj is IHsbColorModel<float>)
                return Equals((IHsbColorModel<float>)obj);
            if (obj is IHsbColorModel<byte>)
                return Equals((IHsbColorModel<byte>)obj, false);
            if (obj is IRgbColorModel<float>)
                return Equals((IRgbColorModel<float>)obj, false);
            return obj is IRgbColorModel<byte> && Equals((IRgbColorModel<byte>)obj, false);
        }

        public override int GetHashCode() { return BitConverter.ToInt32(new byte[] { _hue.FromPercentage(), _saturation.FromPercentage(), _brightness.FromPercentage(), _alpha.FromPercentage() }, 0); }

        public IHsbColorModel<float> MergeAverage(IEnumerable<IHsbColorModel<float>> other)
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
        
        public IHsbColorModel<float> MergeAverage(IEnumerable<IColorModel> other)
        {
            throw new NotImplementedException();
        }

        IColorModel IColorModel.MergeAverage(IEnumerable<IColorModel> other) { return MergeAverage(other); }

        public IHsbColorModel<float> ShiftHue(float percentage)
        {
            throw new NotImplementedException();
        }

        public IHsbColorModel<float> ShiftSaturation(float percentage)
        {
            throw new NotImplementedException();
        }

        public IHsbColorModel<float> ShiftBrightness(float percentage)
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