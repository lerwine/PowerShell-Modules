using System.Management.Automation;
using System.Runtime.InteropServices;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Erwine.Leonard.T.GDIPlus
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    /// <summary>
    /// Describes a color in terms of it's hue, saturation and brightness.
    /// </summary>
    public struct HsbColor : IEquatable<HsbColor>, IEquatable<RgbColor>, IEquatable<Color>//,
        //IComparable<HsbColor>, IComparable<RgbColor>, IComparable<Color>, IComparable
    {
        [StructLayout(LayoutKind.Explicit)]
        struct HsbHash
        {
            [FieldOffset(0)]
            private readonly int _hashCode;
            [FieldOffset(0)]
            private readonly byte _h;
            [FieldOffset(1)]
            private readonly byte _s;
            [FieldOffset(2)]
            private readonly byte _b;

            internal readonly byte H => _h;

            internal readonly byte S => _s;

            internal readonly byte B => _b;

            internal HsbHash(byte h, byte s, byte b)
            {
                _hashCode = (byte)0;
                _h = h;
                _s = s;
                _b = b;
            }

            internal HsbHash(float h, float s, float b)
            {
                _hashCode = (byte)0;
                _h = Convert.ToByte(Math.Round(h * 25.5f / 36f));
                _s = Convert.ToByte(Math.Round(s * 25.5f / 36f));
                _b = Convert.ToByte(Math.Round(b * 25.5f / 36f));
            }
            public override readonly int GetHashCode() { return _hashCode; }
        }

        /// <summary>
        /// Maximum hue value, which is equivalent to 0.0.
        /// </summary>
        public const float HUE_MAXVALUE = 360f;
        
        private readonly float _h;
        private readonly float _s;
        private readonly float _b;
        private readonly HsbHash _hsb;

        /// <summary>
        /// The current color's hue in degrees, with values ranging from 0.0 to (but not including) 360.0.
        /// </summary>
        /// <remarks>0 = red; 60 = yellow; 120 = green; 180 = cyan; 240 = blue; 300 = magenta.</remarks>
        public readonly float H => _h;

        /// <summary>
        /// The current color's saturation as a percentage value, ranging from 0.0 to 1.0, with 0.0 being no saturation (gray scale).
        /// </summary>
        public readonly float S => _s;

        /// <summary>
        /// The current color's brightness as a percentage value, ranging from 0.0 to 1.0, with 0.0 being completely dark (black) and 1.0 being white.
        /// </summary>
        public readonly float B => _b;

        /// <summary>
        /// The current color's hue as a byte value.
        /// </summary>
        public readonly byte HValue => _hsb.H;

        /// <summary>
        /// The current color's saturation as a byte value, with 0 being no saturation (gray scale).
        /// </summary>
        public readonly byte SValue => _hsb.S;

        /// <summary>
        /// The current color's brightness as a byte value, with 0 being completely dark (black) and 255 being white.
        /// </summary>
        public readonly byte BValue => _hsb.B;

        /// <summary>
        /// Creates a new <see cref="HsbColor"/> structure according to hue, saturation and brightness in byte values.
        /// </summary>
        /// <param name="hue">The color's hue.</param>
        /// <param name="saturation">The saturation of the color.</param>
        /// <param name="brightness">The color's brightness.</param>
        public HsbColor(byte hue, byte saturation, byte brightness)
        {
            if (brightness == (byte)0)
            {
                hue = (byte)0;
                saturation = (byte)0;
            }

            _hsb = new HsbHash(hue, saturation, brightness);
            _h = Convert.ToSingle(hue) * 36f / 25.5f;
            _s = Convert.ToSingle(saturation) / 255f;
            _b = Convert.ToSingle(brightness) / 255f;
        }

        /// <summary>
        /// Creates a new <see cref="HsbColor"/> structure according to hue in degrees, saturation and brightness in percentage values.
        /// </summary>
        /// <param name="hue">The color's hue.</param>
        /// <param name="saturation">The saturation of the color.</param>
        /// <param name="brightness">The color's brightness.</param>
        public HsbColor(float hue, float saturation, float brightness)
        {
            if (hue < 0f || hue > 360f)
                throw new ArgumentOutOfRangeException(nameof(hue), "Hue scale must be a value from 0.0 to 360.0");
            if (saturation < 0f || saturation > 1f)
                throw new ArgumentOutOfRangeException(nameof(saturation), "Saturation scale must be a value from 0.0 to 1.0");
            if (brightness < 0f || brightness > 1f)
                throw new ArgumentOutOfRangeException(nameof(brightness), "Brightness scale must be a value from 0.0 to 1.0");
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

        /// <summary>
        /// Creates a new <see cref="HsbColor"/> structure from an RGB color model.
        /// </summary>
        /// <param name="other">RGB color model from which to derive the hue, saturation and brightness.</param>
        public HsbColor(RgbColor other)
        {
            RGBFtoHSB(other.RScale, other.GScale, other.BScale, out float h, out float s, out float b);
            _h = h;
            _s = s;
            _b = b;
            _hsb = new HsbHash(h, s, b);
        }

        /// <summary>
        /// Creates a new <see cref="HsbColor"/> structure from a <seealso cref="Color"/> object.
        /// </summary>
        /// <param name="color">RGB color model from which to derive the hue, saturation and brightness.</param>
        public HsbColor(Color color)
        {
            RGBtoHSB(color.R, color.G, color.B, out float h, out float s, out float b);
            _h = h;
            _s = s;
            _b = b;
            _hsb = new HsbHash(h, s, b);
        }

        /// <summary>
        /// Converts the byte values of an RGB color model to a floating-point HSB color model.
        /// </summary>
        /// <param name="red">The intensity of red in the color.</param>
        /// <param name="green">The intensity of green in the color.</param>
        /// <param name="blue">The intensity of blue in the color</param>
        /// <param name="h">Returns the color's hue in degreees.</param>
        /// <param name="s">Returns the color's saturation percentage value.</param>
        /// <param name="b">Returns the color's brightness value.</param>
        public static void RGBtoHSB(byte red, byte green, byte blue, out float h, out float s, out float b)
        {
            RGBFtoHSB(Convert.ToSingle(red) / 255f, Convert.ToSingle(green) / 255f, Convert.ToSingle(blue) / 255f, out h, out s, out b);
        }

        /// <summary>
        /// Converts the percentage values of an RGB color model to a floating-point HSB color model.
        /// </summary>
        /// <param name="red">The intensity of red in the color.</param>
        /// <param name="green">The intensity of green in the color.</param>
        /// <param name="blue">The intensity of blue in the color</param>
        /// <param name="h">Returns the color's hue in degreees.</param>
        /// <param name="s">Returns the color's saturation percentage value.</param>
        /// <param name="b">Returns the color's brightness value.</param>
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

        /// <summary>
        /// Creates a new <seealso cref="Color"/> object that is equivalent to the current HSB color model.
        /// </summary>
        /// <returns>A <seealso cref="Color"/> object that is equivalent to the current HSB color model.</returns>
        public readonly Color ToColor() { return new RgbColor(this).ToColor(); }

        /// <summary>
        /// Determins whether the current HSB color model values are equivalent to the values of another.
        /// </summary>
        /// <param name="other">The other HSB color values to compare to.</param>
        /// <returns><c>true</c> if the <paramref name="other"/> <see cref="HsbColor"/> values are equal to the current values; otherwise, false.</returns>
        public readonly bool Equals(HsbColor other)
        {
            return _h == other._h && _s == other._s && _b == other._b;
        }

        /// <summary>
        /// Determines whether the color of the current HSB model is the same as the color represented by a RGB color model.
        /// </summary>
        /// <param name="other">The <seealso cref="RgbColor"/> model to compare to.</param>
        /// <returns><c>true</c> if the <see cref="RgbColor"/> represents the same color as the current model; otherwise, false.</returns>
        public readonly bool Equals(RgbColor other)
        {
            RGBFtoHSB(other.RScale, other.GScale, other.BScale, out float h, out float s, out float b);
            return _h == h && _s == s && _b == b;
        }

        /// <summary>
        /// Determines whether the color of the current HSB model is the same as the color represented by a RGB <see cref="Color"/> object.
        /// </summary>
        /// <param name="other">The <seealso cref="Color"/> object to compare to.</param>
        /// <returns><c>true</c> if the <see cref="Color"/> represents the same color as the current model; otherwise, false.</returns>
        public readonly bool Equals(Color other)
        {
            RGBtoHSB(other.R, other.G, other.B, out float h, out float s, out float b);
            return _h == h && _s == s && _b == b;
        }

        /// <summary>
        /// Determines whether another object is equivalent to the current model.
        /// </summary>
        /// <param name="obj">Other object to compare to.</param>
        /// <returns><c>true</c> if the other object represents the same color as the current model; otherwise, false.</returns>
        public override readonly bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj is PSObject)
                obj = ((PSObject)obj).BaseObject;
            if (obj is HsbColor)
                return Equals((HsbColor)obj);
            if (obj is RgbColor)
                return Equals((RgbColor)obj);
            if (obj is Color)
                return Equals((Color)obj);
            return false;
        }
        
        //public int CompareTo(HsbColor other)
        //{
        //    int result = _h.CompareTo(other._h);
        //    if (result == 0 && (result = _b.CompareTo(other._b)) == 0)
        //        return _s.CompareTo(other._s);
        //    return result;
        //}

        //public int CompareTo(RgbColor other)
        //{
        //    float h, s, b;
        //    RGBFtoHSB(other.RScale, other.GScale, other.BScale, out h, out s, out b);
        //    int result = _h.CompareTo(h);
        //    if (result == 0 && (result = _b.CompareTo(b)) == 0)
        //        return _s.CompareTo(s);
        //    return result;
        //}

        //public int CompareTo(Color other)
        //{
        //    float h, s, b;
        //    RGBtoHSB(other.R, other.G, other.B, out h, out s, out b);
        //    int result = _h.CompareTo(h);
        //    if (result == 0 && (result = _b.CompareTo(b)) == 0)
        //        return _s.CompareTo(s);
        //    return result;
        //}

        //public int CompareTo(object obj)
        //{
        //    throw new NotImplementedException();
        //}

        public override readonly int GetHashCode() { return _hsb.GetHashCode(); }

        public override readonly string ToString()
        {
            return "HSB(" + Math.Round(_h, 2).ToString() + "°, " + Math.Round(_s * 100f, 2).ToString() +"%, " +
                Math.Round(_b * 100f, 2).ToString() + "%)";
        }
    }
}