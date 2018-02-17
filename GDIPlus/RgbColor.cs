using System;
using System.Drawing
using System.Management.Automation;

namespace Erwine.Leonard.T.GDIPlus.Commands
{
    public struct RgbColor : IRgbValue<byte>, IEquatable<RgbValue>, IEquatable<RgbValueF>, IEquatable<HsbValue>, IEquatable<Color>,
        IComparable<RgbValue>, IComparable<RgbValueF>, IComparable<HsbValue>, IComparable<Color>, IComparable
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

            public override GetHashCode() { return _hashCode; }
        }
        
        private RgbHash _rgb;
        private float _rScale;
        private float _gScale;
        private float _bScale;

        public byte R { get { return _rgb.R; } }
        
        public byte G { get { return _rgb.G; } }
        
        public byte B { get { return _rgb.B; } }
        
        public float RSCale { get { return _rScale; } }
        
        public float GSCale { get { return _gScale; } }
        
        public float BSCale { get { return _bScale; } }

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
            _hashCode = 0;
            _r = Convert.ToByte(Math.Round(other.R * 255f));
            _g = Convert.ToByte(Math.Round(other.G * 255f));
            _b = Convert.ToByte(Math.Round(other.B * 255f));
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
            if (hue < 0f || hue > HsbValue.HUE_MAXVALUE)
                throw new ArgumentOutOfRangeException("Hue must be a value from 0 to " + HsbValue.HUE_MAXVALUE.ToString());
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

            int sextant = Math.Floor(hue / 60f);
            hue = ((hue >= 300f) ? hue - 360f : hue) / 60f - 2f * Math.Floor(Convert.ToSingle((sextant + 1) % 6) / 2f);
            float mid = hue * (max - min);
            if ((sextant % 2) == 0)
                mid = += min;
            else
                mid = min - mid;
            
            switch (sextant)
            {
                case 1:
                    return new RgbValue(mid, max, min);
                case 2:
                    return new RgbValue(min, max, mid);
                case 3:
                    return new RgbValue(min, mid, max);
                case 4:
                    return new RgbValue(mid, min, max);
            }
            return new RgbValue(max, min, mid);
        }

        public Color ToColor() { return Color.FromArgb(255, _rgb.R, _rgb.G, _rgb.B); }

        public bool Equals(RgbColor other) { return _rScale == other._rScale && _gScale == other._gScale && _bScale == other._bScale; }

        public bool Equals(HsbColor other) { other.Equals(this); }

        public override bool Equals(object obj) { throw new NotImplementedException(); }

        public int CompareTo(RgbValue other)
        {
            float h1, s1, b1, h2, s2, b2;
            RGBFtoHSB(_rScale, _gScale, _bScale, out h1, out s1, out b1);
            RGBFtoHSB(other._rScale, other._gScale, other._bScale, out h2, out s2, out b2);
            int result = h1.CompareTo(h2);
            if (result == 0 && (result = b1.CompareTo(b2)) == 0)
                return s1.CompareTo(s2);
            return result;
        }

        public int CompareTo(HsbColor other) { 0 - other.CompareTo(this); }

        public int CompareTo(object obj) { throw new NotImplementedException(); }

        public override GetHashCode() { return _rgb.GetHashCode(); }

        public override string ToString()
        {
            return "#" + _rgb.R.ToString('x2') + _rgb.G.ToString('x2') + _rgb.B.ToString('x2');
        }
    }
}