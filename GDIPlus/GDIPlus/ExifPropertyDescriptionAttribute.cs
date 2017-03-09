using System;

namespace Erwine.Leonard.T.GDIPlus
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public sealed class ExifPropertyDescriptionAttribute : Attribute
    {
        private readonly string _displayName;
        private string _summary = "";
        private string _remarks = "";
        public ExifPropertyDescriptionAttribute(string displayName) { _displayName = displayName; }
        public string DisplayName { get { return _displayName; } }
        public string Summary { get { return _summary; } set { _summary = (value == null) ? "" : value.Trim(); } }
        public string Remarks { get { return _remarks; } set { _remarks = (value == null) ? "" : value.Trim(); } }
    }
}