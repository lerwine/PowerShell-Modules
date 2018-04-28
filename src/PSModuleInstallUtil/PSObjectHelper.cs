using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace PSModuleInstallUtil
{
    public static class PSObjectHelper
    {
        private static IEnumerable<string> _AsString(IEnumerable collection)
        {
            if (collection != null)
            {
                foreach (object value in collection)
                    yield return AsString(value);
            }
        }

        public static string AsString(Hashtable hashtable, string key, string defaultValue = null)
        {
            return AsString((hashtable.ContainsKey(key)) ? hashtable[key] : null, defaultValue);
        }

        public static string AsString(object value, string defaultValue = null)
        {
            return AsValue<string>(value, (object o) =>
            {
                if (value == null)
                    return null;

                if (value is string)
                    return value as string;

                if (value is IEnumerable)
                    return String.Join(Environment.NewLine, _AsString(value as IEnumerable).ToArray());

                return value.ToString();
            });
        }

        public static string AsTrimmedString(Hashtable hashtable, string key, string defaultValue = null)
        {
            return AsTrimmedString((hashtable.ContainsKey(key)) ? hashtable[key] : null, defaultValue);
        }

        public static string AsTrimmedString(object value, string defaultValue = null)
        {
            string s = AsString(value, defaultValue);
            return (s == null) ? null : s.Trim();
        }

        public static Version AsVersion(Hashtable hashtable, string key, Version defaultValue = null)
        {
            return AsVersion((hashtable.ContainsKey(key)) ? hashtable[key] : null, defaultValue);
        }

        public static Version AsVersion(object value, Version defaultValue = null)
        {
            return AsValue<Version>(value, (object o) =>
            {
                if (o == null)
                    return defaultValue;

                if (o is Version)
                    return o as Version;
                string s = AsTrimmedString(o, "");
                Version v;
                if (s.Length == 0 || !Version.TryParse(s, out v))
                    return defaultValue;
                return v;
            });
        }

        public static string[] AsStringList(Hashtable hashtable, string key)
        {
            return AsStringList((hashtable.ContainsKey(key)) ? hashtable[key] : null);
        }

        public static string[] AsStringList(object value)
        {
            return AsValue<string[]>(value, (object o) =>
            {
                if (value == null)
                    return new string[0];

                if (value is string)
                    return new string[] { value as string };

                if (value is string[])
                    return (value as string[]).Select(s => s ?? "").ToArray();

                if (value is IEnumerable)
                    return _AsString(value as IEnumerable).Select(s => s ?? "").ToArray();

                return new string[] { value.ToString() };
            });
        }

        public static string[] AsTrimmedStringList(Hashtable hashtable, string key)
        {
            return AsTrimmedStringList((hashtable.ContainsKey(key)) ? hashtable[key] : null);
        }

        public static string[] AsTrimmedStringList(object value)
        {
            return AsStringList(value).Select(s => s.Trim()).ToArray();
        }

        public static Guid? AsGuid(Hashtable hashtable, string key, Guid? defaultValue = null)
        {
            return AsGuid((hashtable.ContainsKey(key)) ? hashtable[key] : null, defaultValue);
        }

        public static Guid? AsGuid(object value, Guid? defaultValue = null)
        {
            return AsValue<Guid?>(value, (object o) =>
            {
                if (o == null)
                    return defaultValue;

                if (o is Guid)
                    return o as Guid?;
                string s = AsTrimmedString(o, "");
                Guid v;
                if (s.Length == 0 || !Guid.TryParse(s, out v))
                    return defaultValue;
                return v;
            });
        }

        public static T AsValue<T>(Hashtable hashtable, string key, Func<object, T> convert)
        {
            return AsValue<T>((hashtable.ContainsKey(key)) ? hashtable[key] : null, convert);
        }

        public static T AsValue<T>(object value, Func<object, T> convert)
        {
            return convert((value != null && value is PSObject) ? (value as PSObject).BaseObject : value);
        }
    }
}
