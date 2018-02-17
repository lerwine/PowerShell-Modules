using System;
using System.Drawing
using System.Management.Automation;

namespace Erwine.Leonard.T.GDIPlus.Commands
{
    public struct HsbColor : IEquatable<HsbColor>, IEquatable<RgbColor>, IEquatable<Color>,
        IComparable<HsbColor>, IComparable<RgbColor>, IComparable<Color>, IComparable
    {
        [StructLayout(LayoutKind.Explicit)]
        struct HsbHash
        {
            [FieldOffset(0)]
            private int _hashCode;
            [FieldOffset(0)]
            private byte _h;
            [FieldOffset(1)]
            private byte _s;
            [FieldOffset(2)]
            private byte _b;

            public byte H { get { return _h; } }
            
            public byte H { get { return _s; } }
            
            public byte B { get { return _b; } }
            
            public HsbHash(byte h, byte s, byte b)
            {
                _hashCode = (byte)0;
                _h = h;
                _s = s;
                _b = b;
            }
            
            public HsbHash(float h, float s, float b)
            {
                _hashCode = (byte)0;
                _h = Convert.ToByte(Math.Round((h * 25.5f) / 36f));
                _s = Convert.ToByte(Math.Round((s * 25.5f) / 36f));
                _b = Convert.ToByte(Math.Round((b * 25.5f) / 36f));
            }

            public override GetHashCode() { return _hashCode; }
        }
        
        public const float HUE_MAXVALUE = 360f;
        
        private float _h;
        private float _s;
        private float _b;
        private HsbHash _hsb;

        public float H { get { return _h; } }
        
        public float S { get { return _s; } }
        
        public float B { get { return _b; } }
        
        public byte HValue { get { return _hsb.H; } }
        
        public byte SValue { get { return _hsb.S; } }
        
        public byte BValue { get { return _hsb.B; } }
        
        public HsbColor(byte hue, byte saturation, byte brightness)
        {
            if (brightness == (byte)0)
            {
                hue = (byte)0;
                saturation = (byte)0;
            }

            _hsb = new HsbHash(hue, saturation, brightness);
            _h = (Convert.ToSingle(hue) * 36f) / 25.5f;
            _s = Convert.ToSingle(saturation) / 255f;
            _b = Convert.ToSingle(brightness) / 255f;
        }

        public HsbColor(float hue, float saturation, float brightness)
        {
            if (hue < 0f || hue > 360f)
                throw new ArgumentOutOfRangeException("Hue scale must be a value from 0.0 to 360.0");
            if (saturation < 0f || saturation > 1f)
                throw new ArgumentOutOfRangeException("Saturation scale must be a value from 0.0 to 1.0");
            if (brightness < 0f || brightness > 1f)
                throw new ArgumentOutOfRangeException("Brightness scale must be a value from 0.0 to 1.0");
            if (brightness == 0f)
            {
                hue = 0f;
                saturation = 0f;
            }
            else if (hue == 360f)
                hue = 0;
            _hsb = new HsbHash(hue, saturation, brightness);
            _h = hue;
            _s = saturation;
            _b = brightness;
        }

        public HsbColor(RgbColor other)
        {
            float h, s, b;
            RGBFtoHSB(other.RScale, other.GScale, other.BScale, out h, out s, out b);
            _h = h;
            _s = s;
            _b = b;
            _hsb = new HsbHash(h, s, b);
        }

        public HsbColor(Color color)
        {
            float h, s, b;
            RGBtoHSB(other.R, other.G, other.B, out h, out s, out b);
            _h = h;
            _s = s;
            _b = b;
            _hsb = new HsbHash(h, s, b);
        }

        public static void RGBtoHSB(byte red, byte green, byte blue, out float h, out float s, out float b)
        {
            RGBFtoHSB(Convert.ToSingle(red) / 255f, Convert.ToSingle(green) / 255f, Convert.ToSingle(blue) / 255f, out h, out s, out b);
        }

        public static void RGBFtoHSB(float red, float green, float blue, out float h, out float s, out float b)
        {
            if (red < 0f || red > 1f)
                throw new ArgumentOutOfRangeException("Red must be a value from 0 to 1, representing a percentage value.", "red");
            if (green < 0f || green > 1f)
                throw new ArgumentOutOfRangeException("Green must be a value from 0 to 1, representing a percentage value.", "green");
            if (blue < 0f || blue > 1f)
                throw new ArgumentOutOfRangeException("Blue must be a value from 0 to 1, representing a percentage value.", "blue");
            
            float max, min;
 
            if (red < green)
            {
                if (blue < red)
                {
                    min = blue;
                    max = green;
                }
                else
                {
                    min = red;
                    max = (blue < green) ? green : blue;
                }
            }
            else if (blue < green)
            {
                min = blue;
                max = red;
            }
            else
            {
                min = green;
                max = (red < blue) ? blue : red;
            }
 
            float delta = max - min;
            if (delta == 0f)
            {
                b = max;
                h = 0f;
                s = 0f;
                return;
            }

            h = ((max == red) ? ((green - blue) / delta) : ((max == green) ? (2f + (blue - red) / delta) :
                (4f + (red - green) / delta))) * 60f;
            if (h < 0f)
                h += 360f;
            float mm = max + min;
            b = mm / 2f;
            if (b <= 0.5f)
                s = delta / mm;
            else
                s = delta / (2f - mm);
        }

        public Color ToColor() { return (new RgbColor(this)).ToColor(); }

        public bool Equals(HsbColor other)
        {
            return _h == other._h && _s == other._s && _b == other._b;
        }

        public bool Equals(RgbColor other)
        {
            float h, s, b;
            RGBFtoHSB(other.RScale, other.GScale, other.BScale, out h, out s, out b);
            return _h == h && _s == s && _b == b;
        }

        public bool Equals(Color other)
        {
            float h, s, b;
            RGBtoHSB(other.R, other.G, other.B, out h, out s, out b);
            return _h == h && _s == s && _b == b;
        }

        public override bool Equals(object obj)
        {
            throw new NotImplementedException();
        }

        public int CompareTo(HsbColor other)
        {
            int result = _h.CompareTo(other._h);
            if (result == 0 && (result = _b.CompareTo(other._b)) == 0)
                return _s.CompareTo(other._s);
            return result;
        }

        public int CompareTo(RgbColor other)
        {
            float h, s, b;
            RGBFtoHSB(other.RScale, other.GScale, other.BScale, out h, out s, out b);
            int result = _h.CompareTo(h);
            if (result == 0 && (result = _b.CompareTo(b)) == 0)
                return _s.CompareTo(s);
            return result;
        }

        public int CompareTo(Color other)
        {
            float h, s, b;
            RGBtoHSB(other.R, other.G, other.B, out h, out s, out b);
            int result = _h.CompareTo(h);
            if (result == 0 && (result = _b.CompareTo(b)) == 0)
                return _s.CompareTo(s);
            return result;
        }

        public int CompareTo(object obj)
        {
            throw new NotImplementedException();
        }

        public override GetHashCode() { return _hsb.GetHashCode(); }

        public override string ToString()
        {
            return "HSB(" + Math.Round(_h, 2).ToString() + "°, " + Math.Round(_s * 100f, 2).ToString() +"%, " +
                Math.Round(_b * 100f, 2).ToString() + "%)";
        }
    }
}