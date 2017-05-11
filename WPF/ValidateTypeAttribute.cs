using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;

namespace Erwine.Leonard.T.WPF
{
    public sealed class ValidateTypeAttribute : ValidateEnumeratedArgumentsAttribute
    {
        readonly string[] _typeNames;

        public ValidateTypeAttribute(string typeName, params string[] otherTypeNames)
        {
            IEnumerable<string> typeNames = new string[] { typeName };
            if (otherTypeNames != null)
                typeNames = typeNames.Concat(otherTypeNames);
            _typeNames = typeNames.Where(s => s != null).Distinct(StringComparer.InvariantCultureIgnoreCase).ToArray();
        }

        public string[] TypeNames { get { return _typeNames; } }

        protected override void ValidateElement(object element)
        {
            if (_typeNames.Length == 0 || (_typeNames.Length == 1 && _typeNames[0].Length == 0))
                return;
            if (!GetAllTypeNames(element).Any(n => _typeNames.Any(t => String.Equals(n, t, StringComparison.InvariantCultureIgnoreCase))))
                throw new ValidationMetadataException("Invalid type.");
        }

        public static IEnumerable<string> GetAllTypeNames(object obj)
        {
            PSObject psObject = obj as PSObject;
            if (obj != null && psObject != null)
                obj = psObject.BaseObject;
            if (obj == null)
            {
                yield return "";
                yield break;
            }

            Type t = obj.GetType();
            yield return t.FullName;
            foreach (Type i in t.GetInterfaces())
                yield return i.FullName;
            for (Type b = t.BaseType; b != null; b = b.BaseType)
                yield return b.FullName;
            if (psObject != null)
                psObject = PSObject.AsPSObject(obj);
            foreach (string n in psObject.TypeNames)
                yield return n;
        }

        public override string ToString()
        {
            return "[" + this.GetType().Name + "('" + String.Join("', '", _typeNames.Select(s => s.Replace("'", "''")).ToArray()) + "')]";
        }
    }
}
