using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace LteDev.ModuleBuilder
{
#pragma warning disable 1591 // Missing XML comment for publicly visible type or member
    public static class ExtensionMethods
    {
        public static PSPropertyInfo GetPropertyInfo(this PSObject obj, string propertyName, bool @static = false)
        {
            if (obj == null || propertyName == null)
                return null;

            return obj.Properties.FirstOrDefault(p => p.IsInstance != @static && String.Equals(p.Name, propertyName, StringComparison.InvariantCultureIgnoreCase));
        }

        public static PSObject GetPropertyValue(this PSObject obj, string propertyName, bool @static = false)
        {
            PSPropertyInfo propertyInfo = obj.GetPropertyInfo(propertyName, @static);
            if (propertyInfo == null || !propertyInfo.IsGettable)
                return null;
            
            return (propertyInfo.Value == null || propertyInfo.Value is PSObject) ? propertyInfo.Value as PSObject : PSObject.AsPSObject(propertyInfo.Value);
        }

        public static bool HasProperty(this PSObject obj, string propertyName, bool @static = false)
        {
            return obj != null && obj.Properties.Any(p => p.IsInstance != @static && String.Equals(p.Name, propertyName, StringComparison.InvariantCultureIgnoreCase));
        }

        public static bool ContainsGettable(this PSObject obj, string propertyName, bool @static = false)
        {
            return obj != null && obj.Properties.Any(p => p.IsGettable && p.IsInstance != @static && String.Equals(p.Name, propertyName, StringComparison.InvariantCultureIgnoreCase));
        }

        public static bool IsEnumerable(this PSObject obj)
        {
            return obj != null && !(obj.BaseObject is string) && obj.BaseObject is IEnumerable;
        }

        public static bool IsSingleValue(this PSObject obj)
        {
            if (obj == null)
                return false;
            if (obj.BaseObject is string)
                return true;
            if (obj.BaseObject is Array)
                return (obj.BaseObject as Array).Length == 1;
            if (obj.BaseObject is ICollection)
                return (obj.BaseObject as ICollection).Count == 1;
            if (!(obj.BaseObject is IEnumerable))
                return true;
            IEnumerator en = (obj.BaseObject as IEnumerable).GetEnumerator();
            return en.MoveNext() && !en.MoveNext();
        }

        public static PSObject GetFirstValue(this PSObject obj)
        {
            if (obj == null)
                return null;
            if (obj.BaseObject is string)
                return obj;
            object o;
            if (obj.BaseObject is IList)
            {
                IList coll = obj.BaseObject as IList;
                int count = (obj.BaseObject is Array) ? (obj.BaseObject as Array).Length : coll.Count;
                if (count == 0)
                    return null;
                o = coll[0];
            }
            else if (obj.BaseObject is IEnumerable)
            {
                IEnumerator en = (obj.BaseObject as IEnumerable).GetEnumerator();
                if (!en.MoveNext())
                    return null;
                o = en.Current;
            } else
                return obj;

            if (o == null)
                return null;

            return (o is PSObject) ? o as PSObject : PSObject.AsPSObject(o);
        }

        public static IEnumerable<PSObject> AsEnumerable(this PSObject obj)
        {
            if (obj == null || obj.BaseObject is string || !(obj.BaseObject is IEnumerable))
                yield return obj;
            else
            {
                foreach (object o in obj.BaseObject as IEnumerable)
                    yield return (o is PSObject) ? o as PSObject : PSObject.AsPSObject(o);
            }
        }

        public static int EnumeratedCount(this PSObject obj)
        {
            if (obj == null || obj.BaseObject is string)
                return 1;

            if (obj.BaseObject is Array)
                return (obj.BaseObject as Array).Length;

            if (obj.BaseObject is ICollection)
                return (obj.BaseObject as ICollection).Count;

            if (obj.BaseObject is IEnumerable)
                return (obj.BaseObject as IEnumerable).Cast<object>().Count();

            return 1;
        }

        public static IEnumerable<string> AsStringValues(this PSObject obj)
        {
            if (obj == null)
                return new string[] { null };
            if (obj.BaseObject is string)
                return new string[] { obj.BaseObject as string };
            if (obj.BaseObject is IEnumerable<string>)
                return obj.BaseObject as IEnumerable<string>;
            if (!(obj.BaseObject is IEnumerable))
                return new string[] { LanguagePrimitives.ConvertTo<string>(obj) };

            return (obj.BaseObject as IEnumerable).Cast<object>().SelectMany(o =>
            {
                if (o == null)
                    return new string[] { null };
                PSObject p;
                object b;
                if (o is PSObject)
                {
                    p = o as PSObject;
                    b = p.BaseObject;
                }
                else
                {
                    b = o;
                    p = PSObject.AsPSObject(b);
                }
                if (b is string)
                    return new string[] { b as string };
                if (b is IEnumerable<string>)
                    return b as IEnumerable<string>;
                if (b is IEnumerable)
                    return (b as IEnumerable).Cast<object>().Select(a =>
                    {
                        if (a == null)
                            return null;

                        if (a is string)
                            return a as string;
                        if (a is PSObject)
                        {
                            PSObject n = a as PSObject;
                            if (n.BaseObject is string)
                                return n.BaseObject as string;
                        }
                        return LanguagePrimitives.ConvertTo<string>(a);
                    });
                return new string[] { LanguagePrimitives.ConvertTo<string>(p) };
            });
        }

        public static bool IsNullOrEmpty(this PSObject obj)
        {
            if (obj == null)
                return true;
            if (obj.BaseObject is string)
                return (obj.BaseObject as string).Length == 0;
            if (obj.BaseObject is string[])
            {
                string[] sArr = obj.BaseObject as string[];
                return sArr.Length == 0 || (sArr.Length == 1 && String.IsNullOrEmpty(sArr[0]));
            }
            if (obj.BaseObject is ICollection<string>)
            {
                ICollection<string> sColl = obj.BaseObject as ICollection<string>;
                return sColl.Count == 0 || (sColl.Count == 1 && String.IsNullOrEmpty((sColl is IList<string>) ? (sColl as IList<string>)[0] : sColl.First()));
            }
            if (obj.BaseObject is ICollection)
            {
                ICollection coll = obj.BaseObject as ICollection;
                int count = (obj.BaseObject is Array) ? (obj.BaseObject as Array).Length : coll.Count;
                if (count > 1)
                    return false;
                if (count == 0)
                    return true;
                object o = (coll is IList) ? (coll as IList)[0] : coll.Cast<object>().First();
                if (o == null)
                    return true;
                if (o is PSObject)
                    o = (o as PSObject).BaseObject;
                return o is string && (o as string).Length == 0;
            }
            if (!(obj.BaseObject is IEnumerable))
                return false;

            IEnumerator en = (obj.BaseObject as IEnumerable).GetEnumerator();
            if (!en.MoveNext())
                return true;
            object c = en.Current;
            if (c == null)
                return true;
            if (c is PSObject)
                c = (c as PSObject).BaseObject;
            if (c is string)
            {
                if ((c as string).Length == 0)
                    return true;
            }
            else
                return false;
            return !en.MoveNext();
        }
    }
#pragma warning restore 1591 // Missing XML comment for publicly visible type or member
}
