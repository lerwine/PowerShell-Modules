
#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Erwine.Leonard.T.GDIPlus
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = false)]
    public sealed class ExifItemDisplayNameAttribute(int index, string displayText) : Attribute
    {
        private readonly int _index = index;
        private readonly string _displayText = displayText;

        public int Index => _index;
        public string DisplayText => _displayText;
    }
}