using System;

namespace Erwine.Leonard.T.GDIPlus
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = false)]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public sealed class ExifPropertyTypeAttribute : Attribute
    {
        private readonly ExifPropertyType _type;
        private int _count = 1;
        private bool _hasNullTerminator = false;
        private bool _isRepeating = false;
        private bool _isPrimary = false;
        public ExifPropertyTypeAttribute(ExifPropertyType type) { _type = type; }
        public ExifPropertyType Type { get { return _type; } }
        /// <summary>
        /// Number of values or zero for variable-length strings.
        /// </summary>
        public int Count { get { return _count; } set { _count = (value < 0) ? 0 : value; } }
        public bool HasNullTerminator { get { return _hasNullTerminator; } set { _hasNullTerminator = value; } }
        public bool IsRepeating { get { return _isRepeating; } set { _isRepeating = value; } }
        public bool IsPrimary { get { return _isPrimary; } set { _isPrimary = value; } }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}