using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;

namespace Erwine.Leonard.T.WPF
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public sealed class ValidateTypeAttribute : ValidateEnumeratedArgumentsAttribute
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {
        readonly string[] _typeNames;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public ValidateTypeAttribute(string typeName, params string[] otherTypeNames)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            IEnumerable<string> typeNames = new string[] { typeName };
            if (otherTypeNames != null)
                typeNames = typeNames.Concat(otherTypeNames);
            _typeNames = typeNames.Where(s => s != null).Distinct(StringComparer.InvariantCultureIgnoreCase).ToArray();
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public string[] TypeNames { get { return _typeNames; } }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        protected override void ValidateElement(object element)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            if (_typeNames.Length == 0 || (_typeNames.Length == 1 && _typeNames[0].Length == 0))
                return;
            if (!GetAllTypeNames(element).Any(n => _typeNames.Any(t => String.Equals(n, t, StringComparison.InvariantCultureIgnoreCase))))
                throw new ValidationMetadataException("Invalid type.");
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static IEnumerable<string> GetAllTypeNames(object obj)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
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

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public override string ToString()
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            return "[" + this.GetType().Name + "('" + String.Join("', '", _typeNames.Select(s => s.Replace("'", "''")).ToArray()) + "')]";
        }
    }
}
