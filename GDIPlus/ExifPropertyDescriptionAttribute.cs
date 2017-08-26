using System;

namespace Erwine.Leonard.T.GDIPlus
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
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
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}