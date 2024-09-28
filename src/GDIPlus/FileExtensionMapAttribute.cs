#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Erwine.Leonard.T.GDIPlus
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    [System.AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = true)]
    sealed class FileExtensionMapAttribute(string extension) : Attribute
    {
        readonly string _extension = extension;

        public string Extension => _extension;

        public bool IsPrimary { get; set; }
    }
}