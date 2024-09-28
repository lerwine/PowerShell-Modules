using System.Management.Automation;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Erwine.Leonard.T.GDIPlus
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    /// <summary>
    /// 
    /// </summary>
    public static class ColorExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool TryGetColor(PSObject obj, out IColorModel result)
        {
            if (obj != null)
            {
                object baseObj = obj.BaseObject;
                if (baseObj is IColorModel)
                {
                    result = (IColorModel)baseObj;
                    return true;
                }
                string[] names = ["Hue", "Saturation", "Brightness"];
                StringComparer comparer = StringComparer.InvariantCultureIgnoreCase;
                object alpha = obj.Properties.Where(p => comparer.Equals(p.Name, "Alpha") && p.IsInstance && p.IsGettable)
                    .Select(p => AsSimplestType(p.Value)).Where(o => o != null && (o is float || o is byte)).ToArray();

                object[] hsb = names.Select(n => obj.Properties.FirstOrDefault(p => comparer.Equals(p.Name, n))).Where(p => p != null && p.IsInstance && p.IsGettable)
                    .Select(p => AsSimplestType(p.Value)).Where(o => o != null).ToArray();
                if (hsb.Length != 3)
                {
                    names = ["Hue", "Saturation", "Lightness"];
                    hsb = names.Select(n => obj.Properties.FirstOrDefault(p => comparer.Equals(p.Name, n))).Where(p => p != null && p.IsInstance && p.IsGettable)
                        .Select(p => AsSimplestType(p.Value)).Where(o => o != null).ToArray();
                    if (hsb.Length != 3)
                    {
                        names = ["H", "S", "L"];
                        hsb = names.Select(n => obj.Properties.FirstOrDefault(p => comparer.Equals(p.Name, n))).Where(p => p != null && p.IsInstance && p.IsGettable)
                            .Select(p => AsSimplestType(p.Value)).Where(o => o != null).ToArray();
                        if (hsb.Length != 3)
                        {
                            names = ["H", "S", "B"];
                            hsb = names.Select(n => obj.Properties.FirstOrDefault(p => comparer.Equals(p.Name, n))).Where(p => p != null && p.IsInstance && p.IsGettable)
                                .Select(p => AsSimplestType(p.Value)).Where(o => o != null).ToArray();
                        }
                    }
                }
                if (hsb.Length == 3)
                {
                    if (hsb[0] is byte)
                    {
                        if (hsb[1] is byte && hsb[2] is byte)
                        {
                            if (alpha == null)
                            {
                                result = new HsbColor32((byte)hsb[0], (byte)hsb[1], (byte)hsb[2]);
                                return true;
                            }
                            if (alpha is byte)
                            {
                                result = new HsbColor32((byte)hsb[0], (byte)hsb[1], (byte)hsb[2], (byte)alpha);
                                return true;
                            }
                        }
                    }
                    else if (hsb[0] is float h && h >= 0f && h <= 360f && hsb[1] is byte s && s >= 0f && s <= 1f && hsb[2] is byte b && b >= 0f && b <= 1f)
                    {
                        if (alpha == null)
                        {
                            result = new HsbColorF(h, s, b);
                            return true;
                        }
                        if (alpha is float && (float)alpha >= 0f && (float)alpha <= 1f)
                        {
                            result = new HsbColorF(h, s, b, (float)alpha);
                            return true;
                        }
                    }
                }
                names = ["Red", "Green", "Blue"];
                hsb = names.Select(n => obj.Properties.FirstOrDefault(p => comparer.Equals(p.Name, n))).Where(p => p != null && p.IsInstance && p.IsGettable)
                    .Select(p => AsSimplestType(p.Value)).Where(o => o != null).ToArray();
                if (hsb.Length != 3)
                {
                    names = ["R", "G", "B"];
                    hsb = names.Select(n => obj.Properties.FirstOrDefault(p => comparer.Equals(p.Name, n))).Where(p => p != null && p.IsInstance && p.IsGettable)
                        .Select(p => AsSimplestType(p.Value)).Where(o => o != null).ToArray();
                }
                if (hsb.Length == 3)
                {
                    if (hsb[0] is byte)
                    {
                        if (hsb[1] is byte && hsb[2] is byte)
                        {
                            if (alpha == null)
                            {
                                result = new RgbColor32((byte)hsb[0], (byte)hsb[1], (byte)hsb[2]);
                                return true;
                            }
                            if (alpha is byte)
                            {
                                result = new RgbColor32((byte)hsb[0], (byte)hsb[1], (byte)hsb[2], (byte)alpha);
                                return true;
                            }
                        }
                    }
                    else if (hsb[0] is float h && h >= 0f && h <= 360f && hsb[1] is byte s && s >= 0f && s <= 1f && hsb[2] is byte b && b >= 0f && b <= 1f)
                    {
                        if (alpha == null)
                        {
                            result = new RgbColorF(h, s, b);
                            return true;
                        }
                        if (alpha is float && (float)alpha >= 0f && (float)alpha <= 1f)
                        {
                            result = new RgbColorF(h, s, b, (float)alpha);
                            return true;
                        }
                    }
                }
            }
            result = null;
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static object AsSimplestType(object value)
        {
            if (value == null)
                return null;
            object obj = (value is PSObject) ? ((PSObject)value).BaseObject : value;
            if (obj is string || obj is bool || obj is char || obj is int || obj is DateTime || obj is DBNull)
                return obj;

            if (obj is double)
                return ((double)obj <= Convert.ToDouble(float.MaxValue) && (double)obj >= Convert.ToSingle(float.MaxValue)) ? Convert.ToSingle((double)obj) : obj;

            if (obj is long)
                return ((long)obj < 256 && (long)obj > -1) ? (byte)(long)obj : obj;

            if (obj is float)
                return (float)obj;

            if (obj is decimal)
                return ((decimal)obj <= Convert.ToDecimal(float.MaxValue) && (decimal)obj >= Convert.ToDecimal(float.MaxValue)) ? Convert.ToSingle((decimal)obj) : obj;

            if (obj is byte)
                return (byte)obj;

            if (obj is char)
                return new string(new char[] { (char)obj });

            if (obj is uint)
                return ((uint)obj < 256) ? (byte)(uint)obj : obj;

            if (obj is short)
                return ((short)obj > -1 && (short)obj < 256) ? (byte)(short)obj : obj;

            if (obj is ushort)
                return ((ushort)obj < 256) ? (byte)(ushort)obj : obj;

            if (obj is sbyte)
                return ((sbyte)obj > -1) ? (byte)(sbyte)obj : obj;

            if (obj is IConvertible)
            {
                try
                {
                    object c;
                    switch (((IConvertible)obj).GetTypeCode())
                    {
                        case TypeCode.Boolean:
                            if ((c = Convert.ChangeType(obj, TypeCode.Boolean)) != null && c is bool)
                                return c;
                            break;
                        case TypeCode.Byte:
                            if ((c = Convert.ChangeType(obj, TypeCode.Boolean)) != null && c is byte)
                                return (byte)c;
                            break;
                        case TypeCode.Char:
                            if ((c = Convert.ChangeType(obj, TypeCode.Boolean)) != null && c is char)
                                return new string(new char[] { (char)c });
                            break;
                        case TypeCode.DateTime:
                            if ((c = Convert.ChangeType(obj, TypeCode.Boolean)) != null && c is DateTime)
                                return c;
                            break;
                        case TypeCode.DBNull:
                            return DBNull.Value;
                        case TypeCode.Decimal:
                            if ((c = Convert.ChangeType(obj, TypeCode.Boolean)) != null && c is decimal)
                                return ((decimal)c <= Convert.ToDecimal(float.MaxValue) && (decimal)c >= Convert.ToDecimal(float.MaxValue)) ? Convert.ToSingle((decimal)c) : obj;
                            break;
                        case TypeCode.Double:
                            if ((c = Convert.ChangeType(obj, TypeCode.Boolean)) != null && c is double)
                                return ((double)c <= Convert.ToDouble(float.MaxValue) && (double)c >= Convert.ToDouble(float.MaxValue)) ? Convert.ToSingle((double)c) : obj;
                            break;
                        case TypeCode.Int16:
                            if ((c = Convert.ChangeType(obj, TypeCode.Boolean)) != null && c is short)
                                return ((short)c > -1 && (short)c < 256) ? (byte)(short)c : obj;
                            break;
                        case TypeCode.Int32:
                            if ((c = Convert.ChangeType(obj, TypeCode.Boolean)) != null && c is int)
                                return ((int)c > -1 && (int)c < 256) ? (byte)(int)c : obj;
                            break;
                        case TypeCode.Int64:
                            if ((c = Convert.ChangeType(obj, TypeCode.Boolean)) != null && c is long)
                                return ((long)c > -1 && (long)c < 256) ? (byte)(long)c : obj;
                            break;
                        case TypeCode.SByte:
                            if ((c = Convert.ChangeType(obj, TypeCode.Boolean)) != null && c is sbyte)
                                return ((sbyte)c > -1) ? (byte)(sbyte)c : obj;
                            break;
                        case TypeCode.Single:
                            if ((c = Convert.ChangeType(obj, TypeCode.Boolean)) != null && c is float)
                                return (float)c;
                            break;
                        case TypeCode.String:
                            if ((c = Convert.ChangeType(obj, TypeCode.Boolean)) != null && c is string)
                                return c;
                            break;
                        case TypeCode.UInt16:
                            if ((c = Convert.ChangeType(obj, TypeCode.Boolean)) != null && c is ushort)
                                return ((ushort)c < 256) ? (byte)(ushort)c : obj;
                            break;
                        case TypeCode.UInt32:
                            if ((c = Convert.ChangeType(obj, TypeCode.Boolean)) != null && c is uint)
                                return ((uint)c < 256) ? (byte)(uint)c : obj;
                            break;
                        case TypeCode.UInt64:
                            if ((c = Convert.ChangeType(obj, TypeCode.Boolean)) != null && c is ulong)
                                return ((ulong)c < 256) ? (byte)(ulong)c : obj;
                            break;
                    }
                }
                catch { }
            }
            
            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        public const float HUE_MAXVALUE = 360f;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hue"></param>
        /// <param name="saturation"></param>
        /// <param name="brightness"></param>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        public static void HSBtoRGB(float hue, float saturation, float brightness, out float r, out float g, out float b)
        {
            if (hue < 0f || hue > HUE_MAXVALUE)
                throw new ArgumentOutOfRangeException(nameof(hue), "Hue must be a value from 0 to " + HUE_MAXVALUE.ToString());
            if (saturation < 0f || saturation > 1f)
                throw new ArgumentOutOfRangeException(nameof(saturation), "Saturation must be a value from 0 to 1");
            if (brightness < 0f || brightness > 1f)
                throw new ArgumentOutOfRangeException(nameof(brightness), "Brightness must be a value from 0 to 1");
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="red"></param>
        /// <param name="green"></param>
        /// <param name="blue"></param>
        /// <param name="h"></param>
        /// <param name="s"></param>
        /// <param name="b"></param>
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
                throw new ArgumentOutOfRangeException(nameof(value));
            return Convert.ToByte(value * 255f);
        }

        /// <summary>
        /// Converts an 8-bit value to degrees.
        /// </summary>
        /// <param name="value">Byte value to convert.</param>
        /// <returns>8-bit value converted to a value ranging from 0.0 to (but not including) 360.0.</returns>
        public static float ToDegrees(this byte value) { return Convert.ToSingle(value) * HUE_MAXVALUE / 256f; }

        /// <summary>
        /// Converts a degree value to an 8-bit value.
        /// </summary>
        /// <param name="value">Floating point value to convert.</param>
        /// <returns>8-bit value representing a degree measurement.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="value"/> is less than 0.0 or greater than 360.0.</exception>
        public static byte FromDegrees(this float value)
        {
            if (value < 0f || value > HUE_MAXVALUE)
                throw new ArgumentOutOfRangeException(nameof(value));
            return (value == HUE_MAXVALUE) ? (byte)0 : Convert.ToByte(value * 256f / HUE_MAXVALUE);
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
