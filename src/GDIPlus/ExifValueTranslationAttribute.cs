
#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Erwine.Leonard.T.GDIPlus
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = false)]
    public sealed class ExifValueTranslationAttribute : Attribute
    {
        private readonly byte[] _sourceValue;
        private readonly string _displayText;

        public byte[] SourceValue => _sourceValue;
        public string DisplayText => _displayText;
        public ExifValueTranslationAttribute(byte sourceValue, string displayText)
        {
            _sourceValue = [sourceValue];
            _displayText = displayText;
        }
        public ExifValueTranslationAttribute(char sourceValue, string displayText)
        {
            _sourceValue = [(byte)sourceValue];
            _displayText = displayText;
        }
        public ExifValueTranslationAttribute(ushort sourceValue, string displayText)
        {
            _sourceValue = BitConverter.GetBytes(sourceValue);
            _displayText = displayText;
        }
    }
}