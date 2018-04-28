using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace LteDev
{
#pragma warning disable 1591 // Missing XML comment for publicly visible type or member
    public class PSObjectHelper
    {
        public static IEnumerable<PSObject> AsPSObjects(object source)
        {
            if (source == null)
            {
                yield return null;
                yield break;
            }
            object baseObj = source;
            PSObject psObj;
            if (source is PSObject)
            {
                psObj = source as PSObject;
                baseObj = psObj.BaseObject;
            }
            else
                psObj = PSObject.AsPSObject(baseObj);

            if (baseObj is string || baseObj is IDictionary)
                yield return psObj;
            else if (baseObj is IEnumerable)
            {
                foreach (object o in (baseObj as IEnumerable))
                {
                    if (o == null || o is PSObject)
                        yield return o as PSObject;
                    else
                        yield return PSObject.AsPSObject(o);
                }
            }
            else
                yield return psObj;
        }

        public static bool IsEmptyCollection(object source)
        {
            using (IEnumerator<PSObject> enumerator = AsPSObjects(source).GetEnumerator())
                return !enumerator.MoveNext();
        }

        public static bool IsNullOrEmptyCollection(object source)
        {
            if (source == null)
                return true;

            using (IEnumerator<PSObject> enumerator = AsPSObjects(source).GetEnumerator())
                return !enumerator.MoveNext();
        }

        public static bool IsNullOrEmptyCollectionOrSingleNull(object source)
        {
            using (IEnumerator<PSObject> enumerator = AsPSObjects(source).GetEnumerator())
                return !enumerator.MoveNext() || (enumerator.Current == null && !enumerator.MoveNext());
        }

        public static bool IsOfType(PSTypeName type, PSObject obj)
        {
            if (type == null)
                return obj == null;

            if (obj == null)
                return false;

            if (type.Type != null && type.Type.IsInstanceOfType(obj.BaseObject))
                return true;

            List<PSTypeName> typeNames = new List<PSTypeName>();
            Type o = obj.BaseObject.GetType();
            StringComparer comparer = StringComparer.InvariantCulture;
            foreach (string n in obj.TypeNames)
            {   
                PSTypeName tn = new PSTypeName(n);
                if (tn.Type != null && o.IsAssignableFrom(tn.Type) || comparer.Equals(type.Name, tn.Name))
                    return true;
            }
            return false;
        }

        public static bool IsOfType<T>(PSObject obj)
        {
            return obj != null && obj.BaseObject is T;
        }

        public static IEnumerable<PSObject> OfType(PSTypeName type, object source, bool includeNull = false)
        {
            foreach (PSObject obj in AsPSObjects(source))
            {
                if ((obj == null) ? !includeNull : IsOfType(type, obj))
                    yield return obj;
            }
        }

        public static IEnumerable<PSObject> OfType<T>(object source, bool includeNull = false)
        {
            foreach (PSObject obj in AsPSObjects(source))
            {
                if ((obj == null) ? !includeNull : IsOfType<T>(obj))
                    yield return obj;
            }
        }
    }
#pragma warning restore 1591 // Missing XML comment for publicly visible type or member
}
