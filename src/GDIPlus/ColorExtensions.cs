using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erwine.Leonard.T.GDIPlus
{
    public static class ColorExtensions
    {
        public const float HUE_MAXVALUE = 360f;

        public static void HSBtoRGB(float hue, float saturation, float brightness, out float r, out float g, out float b)
        {
            if (hue < 0f || hue > HUE_MAXVALUE)
                throw new ArgumentOutOfRangeException("Hue must be a value from 0 to " + HUE_MAXVALUE.ToString());
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
            hue = ((hue >= 300f) ? hue - HUE_MAXVALUE : hue) / 60f - (float)(2f * Math.Floor(Convert.ToSingle((sextant + 1) % 6) / 2f));
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

        public static void RGBtoHSB(float red, float green, float blue, out float h, out float s, out float b)
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
                h += HUE_MAXVALUE;
            float mm = max + min;
            b = mm / 2f;
            if (b <= 0.5f)
                s = delta / mm;
            else
                s = delta / (2f - mm);
        }

        /// <summary>
        /// Converts 8-bit value to a percentage value.
        /// </summary>
        /// <param name="value">Byte value to convert.</param>
        /// <returns>8-bit value converted to a value ranging from 0.0 to 1.0.</returns>
        public static float ToPercentage(this byte value) { return Convert.ToSingle(value) / 255f; }

        /// <summary>
        /// Converts a percentage value to an 8-bit value.
        /// </summary>
        /// <param name="value">Floating point value to convert.</param>
        /// <returns>8-bit value representing a percentage value.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="value"/> is less than 0.0 or greater than 1.0.</exception>
        public static byte FromPercentage(this float value)
        {
            if (value < 0f || value > 1f)
                throw new ArgumentOutOfRangeException("value");
            return Convert.ToByte(value * 255f);
        }

        /// <summary>
        /// Converts an 8-bit value to degrees.
        /// </summary>
        /// <param name="value">Byte value to convert.</param>
        /// <returns>8-bit value converted to a value ranging from 0.0 to (but not including) 360.0.</returns>
        public static float ToDegrees(this byte value) { return (Convert.ToSingle(value) * HUE_MAXVALUE) / 256f; }

        /// <summary>
        /// Converts a degree value to an 8-bit value.
        /// </summary>
        /// <param name="value">Floating point value to convert.</param>
        /// <returns>8-bit value representing a degree measurement.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="value"/> is less than 0.0 or greater than 360.0.</exception>
        public static byte FromDegrees(this float value)
        {
            if (value < 0f || value > HUE_MAXVALUE)
                throw new ArgumentOutOfRangeException("value");
            return (value == HUE_MAXVALUE) ? (byte)0 : Convert.ToByte((value * 256f) / HUE_MAXVALUE);
        }

        /// <summary>
        /// Converts the current <see cref="IColorModel"/> as a <seealso cref="System.Drawing.Color"/> object.
        /// </summary>
        /// <returns>A <seealso cref="System.Drawing.Color"/> object that represents the same color as the current <seealso cref="IColorModel"/>.</returns>
        public static System.Drawing.Color ToDrawingColor(this IColorModel source)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Converts the current <see cref="IColorModel"/> as a <seealso cref="System.Windows.Media.Color"/> object.
        /// </summary>
        /// <returns>A <seealso cref="System.Windows.Media.Color"/> object that represents the same color as the current <seealso cref="IColorModel"/>.</returns>
        public static System.Windows.Media.Color ToMediaColor(this IColorModel source)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Merges a source <see cref="IColorModel"/> with 1 or more <seealso cref="IColorModel"/> objects by averaging average their values.
        /// </summary>
        /// <typeparam name="T">Value component type.</typeparam>
        /// <param name="source"></param>
        /// <param name="other"></param>
        /// <param name="additionalColors"></param>
        /// <returns></returns>
        public static IHsbColorModel<T> MergeAverage<T>(this IHsbColorModel<T> source, IHsbColorModel<T> other, params IHsbColorModel<T>[] additionalColors)
            where T : struct, IComparable, IFormattable, IConvertible, IComparable<T>, IEquatable<T>
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Merges a source <see cref="IColorModel"/> with 1 or more <seealso cref="IColorModel"/> objects by averaging average their values.
        /// </summary>
        /// <typeparam name="T">Value component type.</typeparam>
        /// <param name="source"></param>
        /// <param name="other"></param>
        /// <param name="additionalColors"></param>
        /// <returns></returns>
        public static IRgbColorModel<T> MergeAverage<T>(this IRgbColorModel<T> source, IRgbColorModel<T> other, params IRgbColorModel<T>[] additionalColors)
            where T : struct, IComparable, IFormattable, IConvertible, IComparable<T>, IEquatable<T>
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Merges a source <see cref="IColorModel"/> of type <typeparamref name="T"/> with 1 or more <seealso cref="IColorModel"/> objects by averaging average their values.
        /// </summary>
        /// <typeparam name="T">Type of <seealso cref="IColorModel"/> value to merge into.</typeparam>
        /// <param name="source">The <see cref="IColorModel"/> of type <typeparamref name="T"/> to be merged with other <seealso cref="IColorModel"/> objects.</param>
        /// <param name="other">The <see cref="IColorModel"/> to merge with the source value.</param>
        /// <param name="additionalColors">Additional <see cref="IColorModel"/> models to merge with the source value.</param>
        /// <returns>A <see cref="IColorModel"/> of type <typeparamref name="T"/> representing the average of all the meged color models.</returns>
        public static T MergeAverage<T>(this T source, IColorModel other, params IColorModel[] additionalColors)
            where T : struct, IColorModel
        {
            throw new NotImplementedException();
        }

    }
}
