
#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Erwine.Leonard.T.GDIPlus
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = false)]
    public sealed class ExifPropertyTypeAttribute(ExifPropertyType type) : Attribute
    {
        private readonly ExifPropertyType _type = type;
        private int _count = 1;
        private bool _hasNullTerminator = false;
        private bool _isRepeating = false;
        private bool _isPrimary = false;

        public ExifPropertyType Type => _type;
        /// <summary>
        /// Number of values or zero for variable-length strings.
        /// </summary>
        public int Count { get => _count; set => _count = (value < 0) ? 0 : value; }
        public bool HasNullTerminator { get => _hasNullTerminator; set => _hasNullTerminator = value; }
        public bool IsRepeating { get => _isRepeating; set => _isRepeating = value; }
        public bool IsPrimary { get => _isPrimary; set => _isPrimary = value; }
    }
}