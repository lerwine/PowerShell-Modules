using System;
using System.Runtime.InteropServices;

namespace FileSystemIndexLib
{
    [StructLayout(LayoutKind.Explicit)]
    public struct HashCodeEx : IEquatable<HashCodeEx>
    {
        [FieldOffset(0)]
        private int _sha256HashCode;
        [FieldOffset(0)]
        private ushort _w0;
        [FieldOffset(0)]
        private long _longHashCode;
        [FieldOffset(16)]
        private ushort _w1;
        [FieldOffset(32)]
        private long _length;
        [FieldOffset(32)]
        private ushort _w2;
        [FieldOffset(48)]
        private ushort _w3;

        public int Sha256HashCode { get { return _sha256HashCode; } }
        public long LongHashCode { get { return _longHashCode; } }
        public long Length { get { return _length; } }

        public HashCodeEx(int sha256HashCode, long length)
        {
            _w0 = 0;
            _w1 = 0;
            _w2 = 0;
            _w3 = 0;
            _longHashCode = 0;
            _sha256HashCode = sha256HashCode;
            _length = length;
        }

        public override string ToString() { return String.Format("{0:X4}-{1:X4}-{2:X4}-{3:X4}", _w0, _w1, _w2, _w3); }

        public override int GetHashCode() { return _sha256HashCode; }

        public bool Equals(HashCodeEx other)
        {
            return _sha256HashCode == other._sha256HashCode && _length == other._length;
        }

        public override bool Equals(object obj) { return obj != null && obj is HashCodeEx && Equals((HashCodeEx)obj); }
    }
}
