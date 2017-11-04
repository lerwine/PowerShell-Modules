using System;

namespace Erwine.Leonard.T.GDIPlus
{
    [System.AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = true)]
    sealed class FileExtensionMapAttribute : Attribute
    {
        readonly string _extension;

        // This is a positional argument
        public FileExtensionMapAttribute(string extension) { _extension = extension; }

        public string Extension { get { return _extension; } }

        public bool IsPrimary { get; set; }
    }
}