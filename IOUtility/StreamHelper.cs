using System;
using System.IO;
using System.Text.RegularExpressions;

namespace IOUtility
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class StreamHelper
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {
        private static int? _minBase64BlockSize = null;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static int MinBase64BlockSize
        {
            get
            {
                if (StreamHelper._minBase64BlockSize.HasValue)
                    return StreamHelper._minBase64BlockSize.Value;
                int minBase64BlockSize = 0;
                string s = "";
                Regex regex = new Regex(@"\s");
                do
                {
                    minBase64BlockSize++;
                    byte[] buffer = new byte[minBase64BlockSize];
                    s = Convert.ToBase64String(buffer, 0, minBase64BlockSize, Base64FormattingOptions.InsertLineBreaks).Trim();
                } while (!regex.IsMatch(s));

                StreamHelper._minBase64BlockSize = minBase64BlockSize;
                return minBase64BlockSize;
            }
        }
        
        public static int ReadInteger(Stream inputStream)
        {
            if (inputStream == null)
                throw new ArgumentNullException("inputStream");

            byte[] buffer = new byte[sizeof(int)];
            if (inputStream.Read(buffer, 0, buffer.Length) != buffer.Length)
                throw new InvalidOperationException("Unexpected end of stream");

            return BitConverter.ToInt32(buffer, 0);
        }

        public static long ReadLongInteger(Stream inputStream)
        {
            if (inputStream == null)
                throw new ArgumentNullException("inputStream");

            byte[] buffer = new byte[sizeof(long)];
            if (inputStream.Read(buffer, 0, buffer.Length) != buffer.Length)
                throw new InvalidOperationException("Unexpected end of stream");

            return BitConverter.ToInt64(buffer, 0);
        }

        public static byte[] ReadLengthEncodedBytes(Stream inputStream)
        {
            if (inputStream == null)
                throw new ArgumentNullException("inputStream");

            int length = StreamHelper.ReadInteger(inputStream);
            if (length < 0)
                throw new InvalidOperationException("Invalid length");

            byte[] buffer = new byte[length];
            if (length > 0 && inputStream.Read(buffer, 0, length) != length)
                throw new InvalidOperationException("Unexpected end of stream");

            return buffer;
        }

        public static void WriteInteger(Stream outputStream, int value)
        {
            if (outputStream == null)
                throw new ArgumentNullException("outputStream");
            byte[] buffer = BitConverter.GetBytes(value);
            outputStream.Write(buffer, 0, buffer.Length);
        }

        public static void WriteLongInteger(Stream outputStream, long value)
        {
            if (outputStream == null)
                throw new ArgumentNullException("outputStream");
            byte[] buffer = BitConverter.GetBytes(value);
            outputStream.Write(buffer, 0, buffer.Length);
        }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        private static void _WriteLengthEncodedBytes(Stream outputStream, byte[] buffer, int offset, int count)
        {
            if (outputStream == null)
                throw new ArgumentNullException("outputStream");

            StreamHelper.WriteInteger(outputStream, count);
            if (count > 0)
                outputStream.Write(buffer, offset, count);
        }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static void WriteLengthEncodedBytes(Stream outputStream, byte[] buffer, int offset, int count)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            if (buffer == null)
                throw new ArgumentNullException("buffer");
            if (offset < 0)
                throw new ArgumentOutOfRangeException("Offset cannot be less than zero.", "offset");
            if (count < 0)
                throw new ArgumentOutOfRangeException("Count cannot be less than zero.", "count");
            if (offset > buffer.Length)
                throw new ArgumentOutOfRangeException("Offset cannot extend past the end of the buffer.", "offset");
            if (offset + count > buffer.Length)
                throw new ArgumentOutOfRangeException("Offset pluc Count cannot extend past the end of the buffer.", "count");
            StreamHelper._WriteLengthEncodedBytes(outputStream, buffer, offset, count);
        }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static void WriteLengthEncodedBytes(Stream outputStream, byte[] buffer)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            if (buffer == null)
                throw new ArgumentNullException("buffer");
            StreamHelper._WriteLengthEncodedBytes(outputStream, buffer, 0, buffer.Length);
        }
    }
}
