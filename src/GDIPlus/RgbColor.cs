using System;
using System.Drawing;
using System.Management.Automation;
using System.Runtime.InteropServices;

namespace Erwine.Leonard.T.GDIPlus.Commands
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public struct RgbColor : IEquatable<RgbColor>, IEquatable<HsbColor>, IEquatable<Color>,
        IComparable<RgbColor>, IComparable<HsbColor>, IComparable<Color>, IComparable
    {
        [StructLayout(LayoutKind.Explicit)]
        struct RgbHash
        {
            [FieldOffset(0)]
            private int _hashCode;
            [FieldOffset(0)]
            private byte _r;
            [FieldOffset(1)]
            private byte _g;
            [FieldOffset(2)]
            private byte _b;

            public byte R { get { return _r; } }
            
            public byte G { get { return _g; } }
            
            public byte B { get { return _b; } }
            
            public RgbHash(byte r, byte g, byte b)
            {
                _hashCode = (byte)0;
                _r = r;
                _g = g;
                _b = b;
            }

            public override int GetHashCode() { return _hashCode; }
        }
        
        private RgbHash _rgb;
        private float _rScale;
        private float _gScale;
        private float _bScale;

        public byte R { get { return _rgb.R; } }
        
        public byte G { get { return _rgb.G; } }
        
        public byte B { get { return _rgb.B; } }
        
        public float RScale { get { return _rScale; } }
        
        public float GScale { get { return _gScale; } }
        
        public float BScale { get { return _bScale; } }

        public RgbColor(byte red, byte green, byte blue)
        {
            _rgb = new RgbHash(red, green, blue);
            _rScale = Convert.ToSingle(red) / 255f;
            _gScale = Convert.ToSingle(green) / 255f;
            _bScale = Convert.ToSingle(blue) / 255f;
        }

        public RgbColor(float red, float green, float blue)
        {
            if (red < 0f || red > 1f)
                throw new ArgumentOutOfRangeException("Red scale must be a value from 0.0 to 1.0");
            if (green < 0f || green > 1f)
                throw new ArgumentOutOfRangeException("Green scale must be a value from 0.0 to 1.0");
            if (blue < 0f || blue > 1f)
                throw new ArgumentOutOfRangeException("Blue scale must be a value from 0.0 to 1.0");
            _rScale = red;
            _gScale = green;
            _bScale = blue;
            _rgb = new RgbHash(Convert.ToByte(Math.Round(red * 255f)), Convert.ToByte(Math.Round(green * 255f)),
                Convert.ToByte(Math.Round(blue * 255f)));
        }

        public RgbColor(HsbColor other)
        {
            throw new NotImplementedException();
        }

        public RgbColor(Color color)
        {
            _rgb = new RgbHash(color.R, color.G, color.B);
            _rScale = Convert.ToSingle(color.R) / 255f;
            _gScale = Convert.ToSingle(color.G) / 255f;
            _bScale = Convert.ToSingle(color.B) / 255f;
        }

        public static void HSBtoRGB(float hue, float saturation, float brightness, out float r, out float g, out float b)
        {
            if (hue < 0f || hue > HsbColor.HUE_MAXVALUE)
                throw new ArgumentOutOfRangeException("Hue must be a value from 0 to " + HsbColor.HUE_MAXVALUE.ToString());
            if (saturation < 0f || saturation > 1f)
                throw new ArgumentOutOfRangeException("Saturation must be a value from 0 to 1");
            if (brightness < 0f || brightness > 1f)
                throw new ArgumentOutOfRangeException("Brightness must be a value from 0 to 1");
            float min, max;
            if (brightness < 0.5f)
            {
                min = brightness - brightness * saturation;
                max = brightness + brightness * saturation;
            }
            else
            {
                min = brightness + brightness * saturation - saturation;
                max = brightness - brightness * saturation + saturation;
            }

            int sextant = Convert.ToInt32(Math.Floor(hue / 60f));
            hue = ((hue >= 300f) ? hue - 360f : hue) / 60f - (float)(2f * Math.Floor(Convert.ToSingle((sextant + 1) % 6) / 2f));
            float mid = hue * (max - min);
            if ((sextant % 2) == 0)
                mid += min;
            else
                mid = min - mid;
            
            switch (sextant)
            {
                case 1:
                    r = mid; g = max; b = min;
                    break;
                case 2:
                    r = min; g = max; b = mid;
                    break;
                case 3:
                    r = min; g = mid; b = max;
                    break;
                case 4:
                    r = mid; g = min; b = max;
                    break;
                default:
                    r = max; g = min; b = mid;
                    break;
            }
        }

        public Color ToColor() { return Color.FromArgb(255, _rgb.R, _rgb.G, _rgb.B); }

        public bool Equals(RgbColor other) { return _rScale == other._rScale && _gScale == other._gScale && _bScale == other._bScale; }

        public bool Equals(HsbColor other) { return other.Equals(this); }

        public bool Equals(Color other) { throw new NotImplementedException(); }

        public override bool Equals(object obj) { throw new NotImplementedException(); }

        public int CompareTo(RgbColor other) { throw new NotImplementedException(); }

        public int CompareTo(HsbColor other) { throw new NotImplementedException(); }

        public int CompareTo(Color other) { throw new NotImplementedException(); }

        public int CompareTo(object obj) { throw new NotImplementedException(); }

        public override int GetHashCode() { return _rgb.GetHashCode(); }

        public override string ToString()
        {
            return "#" + _rgb.R.ToString("x2") + _rgb.G.ToString("x2") + _rgb.B.ToString("x2");
        }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}