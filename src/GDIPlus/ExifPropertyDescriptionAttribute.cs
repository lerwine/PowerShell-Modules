
#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Erwine.Leonard.T.GDIPlus
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public sealed class ExifPropertyDescriptionAttribute(string displayName) : Attribute
    {
        private readonly string _displayName = displayName;
        private string _summary = "";
        private string _remarks = "";

        public string DisplayName => _displayName;
        public string Summary { get => _summary; set => _summary = (value == null) ? "" : value.Trim(); }
        public string Remarks { get => _remarks; set => _remarks = (value == null) ? "" : value.Trim(); }
    }
}