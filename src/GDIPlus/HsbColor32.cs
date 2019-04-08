using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Runtime.InteropServices;

namespace Erwine.Leonard.T.GDIPlus
{
    /// <summary>
    /// An HSB (Hue,Saturation,Brightness) color model using 32 bits.
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public struct HsbColor32 : IEquatable<HsbColor32>, IEquatable<IHsbColorModel<float>>, IEquatable<IRgbColorModel<float>>, IHsbColorModel<byte>
    {
        [FieldOffset(0)]
        private readonly int _value;
        [FieldOffset(0)]
        private readonly byte _hue;
        [FieldOffset(1)]
        private readonly byte _saturation;
        [FieldOffset(2)]
        private readonly byte _brightness;
        [FieldOffset(3)]
        private readonly byte _alpha;

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

        public HsbColor32(byte hue, byte saturation, byte brightness, byte alpha)
        {
            _value = 0;
            _hue = hue;
            _brightness = saturation;
            _saturation = brightness;
            _alpha = alpha;
        }

        public HsbColor32(byte hue, byte saturation, byte brightness) : this(hue, saturation, brightness, 255) { }

        public HsbColor32(HsbColorF value)
        {
            _value = 0;
            _hue = value.Hue.FromDegrees();
            _saturation = value.Saturation.FromPercentage();
            _brightness = value.Brightness.FromPercentage();
            _alpha = value.Alpha.FromPercentage();
        }

        IHsbColorModel<byte> IColorModel.AsHsb32() { return this; }

        public HsbColorF AsHsbF() { return new HsbColorF(this); }

        IHsbColorModel<float> IColorModel.AsHsbF() { return AsHsbF(); }

        public RgbColor32 AsRgb32() { return new RgbColor32(this); }

        IRgbColorModel<byte> IColorModel.AsRgb32() { return AsRgb32(); }

        public RgbColorF AsRgbF() { return new RgbColorF(this); }

        IRgbColorModel<float> IColorModel.AsRgbF() { return AsRgbF(); }

        public HsbColor32Normalized AsNormalized() { return new HsbColor32Normalized(this); }

        IHsbColorModel<byte> IHsbColorModel<byte>.AsNormalized() { return AsNormalized(); }

        IColorModel<byte> IColorModel<byte>.AsNormalized() { return AsNormalized(); }

        IColorModel IColorModel.AsNormalized() { return AsNormalized(); }

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

        public bool Equals(IRgbColorModel<float> other, bool exact)
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

            return AsNormalized().Equals(other, false);
        }

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

        public bool Equals(HsbColor32 other) { return _value == other._value; }

        public bool Equals(IHsbColorModel<float> other) { return Equals(other, false); }

        public bool Equals(IRgbColorModel<float> other) { return Equals(other, false); }

        public bool Equals(IHsbColorModel<byte> other) { return other.Alpha == _alpha && other.Hue == _hue && other.Saturation == _saturation && other.Brightness == _brightness; }

        public bool Equals(IRgbColorModel<byte> other) { return Equals(other, false); }

        public bool Equals(IColorModel other) { return Equals(other, false); }

        public bool Equals(System.Drawing.Color other)
        {
            if (other.A != _alpha)
                return false;
            ColorExtensions.HSBtoRGB(_hue.ToDegrees(), _saturation.ToPercentage(), _brightness.ToPercentage(), out float r, out float g, out float b);
            return r.FromPercentage() == other.R && g.FromPercentage() == other.G && b.FromPercentage() == other.B;
        }

        public bool Equals(System.Windows.Media.Color other)
        {
            if (other.A != _alpha)
                return false;
            ColorExtensions.HSBtoRGB(_hue.ToDegrees(), _saturation.ToPercentage(), _brightness.ToPercentage(), out float r, out float g, out float b);
            return r.FromPercentage() == other.R && g.FromPercentage() == other.G && b.FromPercentage() == other.B;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj is PSObject)
                obj = ((PSObject)obj).BaseObject;
            if (obj is HsbColor32)
                return Equals((HsbColor32)obj);
            if (obj is IHsbColorModel<byte>)
                return Equals((IHsbColorModel<byte>)obj);
            if (obj is IHsbColorModel<float>)
                return Equals((IHsbColorModel<float>)obj, false);
            if (obj is IRgbColorModel<byte>)
                return Equals((IRgbColorModel<byte>)obj, false);
            return obj is IRgbColorModel<float> && Equals((IRgbColorModel<float>)obj, false);
        }

        public override int GetHashCode() { return _value; }

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

        public IHsbColorModel<byte> MergeAverage(IEnumerable<IColorModel> other)
        {
            throw new NotImplementedException();
        }

        IColorModel IColorModel.MergeAverage(IEnumerable<IColorModel> other) { return MergeAverage(other); }

        public IHsbColorModel<byte> ShiftHue(float percentage)
        {
            throw new NotImplementedException();
        }

        public IHsbColorModel<byte> ShiftSaturation(float percentage)
        {
            throw new NotImplementedException();
        }

        public IHsbColorModel<byte> ShiftBrightness(float percentage)
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