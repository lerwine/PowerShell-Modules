using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace LteDev
{
    public static class PSLinq
    {
        public static IEnumerable<PSObject> AsPSEnumerable(IEnumerable source)
        {
            if (source == null)
                yield break;

            if (source is string || source is IDictionary)
                yield return PSObject.AsPSObject(source);
            else
            {
                foreach (object obj in source)
                    yield return (obj == null) ? null : ((obj is PSObject) ? obj as PSObject : PSObject.AsPSObject(obj));
            }
        }

        public static IEnumerable<PSObject> AsPSEnumerable(object obj)
        {
            if (obj == null)
                return new PSObject[] { null };

            if (obj is PSObject)
            {
                PSObject psObject = obj as PSObject;
                if (obj is IEnumerable && !(psObject.BaseObject is string || psObject.BaseObject is IDictionary))
                    return AsPSEnumerable(psObject.BaseObject as IEnumerable);

                return new PSObject[] { psObject };
            }
            if (obj is IEnumerable)
                return AsPSEnumerable(obj as IEnumerable);

            return new PSObject[] { PSObject.AsPSObject(obj) };
        }

        public static PSPropertyInfo GetProperty(this PSObject obj, string name)
        {
            if (obj == null || name == null)
                return null;

            return obj.Properties.FirstOrDefault(p => String.Equals(p.Name, name, StringComparison.InvariantCultureIgnoreCase));
        }

        public static PSObject ValueOf(this PSObject obj, string name)
        {
            if (obj == null)
                return null;

            PSPropertyInfo propertyInfo = obj.GetProperty(name);
            if (propertyInfo == null)
            {
                if (obj.BaseObject is IDictionary)
                {
                    IDictionary d = obj.BaseObject as IDictionary;
                    if (d.Contains(name))
                        return (d[name] == null || d[name] is PSObject) ? d[name] as PSObject : PSObject.AsPSObject(d[name]);
                }
                return null;
            }
            if (propertyInfo.Value == null)
                return null;
            return (propertyInfo.Value is PSObject) ? propertyInfo.Value as PSObject : PSObject.AsPSObject(propertyInfo.Value);
        }

        public static PSObject FirstValueOf(this IEnumerable<PSObject> coll, string name)
        {
            if (coll == null)
                return null;

            foreach (PSObject obj in coll)
            {
                if (obj == null)
                    continue;
                PSPropertyInfo propertyInfo = obj.GetProperty(name);
                if (propertyInfo == null)
                {
                    if (obj.BaseObject is IDictionary)
                    {
                        IDictionary d = obj.BaseObject as IDictionary;
                        if (d.Contains(name))
                            return (d[name] == null || d[name] is PSObject) ? d[name] as PSObject : PSObject.AsPSObject(d[name]);
                    }
                    continue;
                }
                if (propertyInfo.Value == null)
                    return null;
                return (propertyInfo.Value is PSObject) ? propertyInfo.Value as PSObject : PSObject.AsPSObject(propertyInfo.Value);
            }

            return null;
        }

        public static IEnumerable<PSObject> PSObjectValues(this PSObject obj)
        {
            if (obj == null || !(obj.BaseObject is IEnumerable))
                return new PSObject[] { obj };

            return AsPSEnumerable(obj.BaseObject as IEnumerable);
        }

        public static IEnumerable<PSObject> PSObjectValues(this PSObject obj, string name)
        {
            if (obj == null)
                return new PSObject[0];

            PSPropertyInfo propertyInfo = obj.GetProperty(name);
            if (propertyInfo == null)
            {
                if (obj.BaseObject is IDictionary)
                {
                    IDictionary d = obj.BaseObject as IDictionary;
                    if (d.Contains(name))
                        return AsPSEnumerable(d[name]);
                }
                return new PSObject[0];
            }
            return AsPSEnumerable(propertyInfo.Value as IEnumerable);
        }

        public static IEnumerable<PSObject> ManyValues(this IEnumerable<PSObject> coll)
        {
            if (coll == null)
                return new PSObject[0];

            return coll.EnumerateAll(o => o.PSObjectValues());
        }

        public static IEnumerable<PSObject> ManuValues(this IEnumerable<PSObject> coll, string name)
        {
            if (coll == null)
                return new PSObject[0];

            return coll.EnumerateAll(o => o.PSObjectValues(name));
        }

        public static bool AllEnumerated(this PSObject source, Func<PSObject, bool> predicate)
        {
            return source.PSObjectValues().All(predicate);
        }

        public static bool AnyEnumerated(this PSObject source)
        {
            return source.PSObjectValues().Any();
        }

        public static bool AnyEnumerated(this PSObject source, Func<PSObject, bool> predicate)
        {
            return source.PSObjectValues().Any(predicate);
        }

        public static IEnumerable<PSObject> ConcatEnumerated(this PSObject first, PSObject second)
        {
            return first.PSObjectValues().Concat(second.PSObjectValues());
        }

        public static IEnumerable<PSObject> ConcatEnumerated(this PSObject first, IEnumerable<PSObject> second)
        {
            return first.PSObjectValues().Concat(second);
        }

        public static IEnumerable<PSObject> ConcatEnumerated(this IEnumerable<PSObject> first, PSObject second)
        {
            return first.Concat(second.PSObjectValues());
        }

        public static int EnumeratedCount(this PSObject source)
        {
            return source.PSObjectValues().Count();
        }

        public static int EnumeratedCount(this PSObject source, Func<PSObject, bool> predicate)
        {
            return source.PSObjectValues().Count(predicate);
        }

        public static IEnumerable<PSObject> DefaultIfEnumEmpty(this PSObject source, PSObject defaultValue)
        {
            return source.PSObjectValues().DefaultIfEmpty(defaultValue);
        }

        public static PSObject ElementEnumAt(this PSObject source, int index)
        {
            return source.PSObjectValues().ElementAt(index);
        }

        public static PSObject ElementEnumAtOrDefault(this PSObject source, int index)
        {
            return source.PSObjectValues().ElementAtOrDefault(index);
        }

        public static PSObject FirstEnumerated(this PSObject source)
        {
            return source.PSObjectValues().First();
        }

        public static PSObject FirstEnumerated(this PSObject source, Func<PSObject, bool> predicate)
        {
            return source.PSObjectValues().First(predicate);
        }

        public static PSObject FirstEnumeratedOrDefault(this PSObject source)
        {
            return source.PSObjectValues().FirstOrDefault();
        }

        public static PSObject FirstEnumeratedOrDefault(this PSObject source, Func<PSObject, bool> predicate)
        {
            return source.PSObjectValues().FirstOrDefault(predicate);
        }

        public static bool IsNullOrNone(this PSObject source)
        {
            return source == null || !source.PSObjectValues().Any();
        }

        public static PSObject LastEnumerated(this PSObject source)
        {
            return source.PSObjectValues().Last();
        }

        public static PSObject LastEnumerated(this PSObject source, Func<PSObject, bool> predicate)
        {
            return source.PSObjectValues().Last(predicate);
        }

        public static PSObject LastEnumeratedOrDefault(this IEnumerable<PSObject> source)
        {
            return source.ManyValues().LastEnumeratedOrDefault();
        }

        public static PSObject LastEnumeratedOrDefault(this IEnumerable<PSObject> source, Func<PSObject, bool> predicate)
        {
            return source.ManyValues().LastEnumeratedOrDefault(predicate);
        }

        public static long LongEnumeratedCount(this PSObject source)
        {
            return source.PSObjectValues().LongCount();
        }

        public static long LongEnumeratedCount(this PSObject source, Func<PSObject, bool> predicate)
        {
            return source.PSObjectValues().LongCount(predicate);
        }

        public static IEnumerable<PSObject> OfBaseType<T>(this PSObject source)
        {
            return source.PSObjectValues().OfBaseType<T>();
        }

        public static IEnumerable<PSObject> OfBaseType<T>(this IEnumerable<PSObject> source)
        {
            if (source == null)
                yield break;
            foreach (PSObject obj in source)
            {
                if (obj != null && obj.BaseObject is T)
                    yield return obj;
            }
        }

        public static IEnumerable<PSObject> ToEnumerable(this PSObject source, Func<PSObject, object> selector)
        {
            return source.PSObjectValues().Select(selector).Select(o => (o == null || o is PSObject) ? o as PSObject : PSObject.AsPSObject(o));
        }

        public static IEnumerable<PSObject> ToEnumerable(this PSObject source, Func<PSObject, int, object> selector)
        {
            return source.PSObjectValues().Select(selector).Select(o => (o == null || o is PSObject) ? o as PSObject : PSObject.AsPSObject(o));
        }

        public static IEnumerable<PSObject> EnumerateMany(this PSObject source, Func<PSObject, object> selector)
        {
            return source.PSObjectValues().Select(selector).SelectMany(o => AsPSEnumerable(o))
                .Select(o => (o == null || o is PSObject) ? o as PSObject : PSObject.AsPSObject(o));
        }
        public static IEnumerable<PSObject> EnumerateMany(this PSObject source, Func<PSObject, int, object> selector)
        {
            return source.PSObjectValues().Select(selector).SelectMany(o => AsPSEnumerable(o))
                .Select(o => (o == null || o is PSObject) ? o as PSObject : PSObject.AsPSObject(o));
        }

        public static IEnumerable<PSObject> EnumerateAll(this IEnumerable<PSObject> source, Func<PSObject, object> selector)
        {
            return source.ManyValues().Select(selector).SelectMany(o => AsPSEnumerable(o))
                .Select(o => (o == null || o is PSObject) ? o as PSObject : PSObject.AsPSObject(o));
        }

        public static IEnumerable<PSObject> EnumerateAll(this IEnumerable<PSObject> source, Func<PSObject, int, object> selector)
        {
            return source.ManyValues().Select(selector).SelectMany(o => AsPSEnumerable(o))
                .Select(o => (o == null || o is PSObject) ? o as PSObject : PSObject.AsPSObject(o));
        }

        public static PSObject AsSingle(this PSObject source)
        {
            return source.PSObjectValues().Single();
        }

        public static PSObject AsSingle(this PSObject source, Func<PSObject, bool> predicate)
        {
            return source.PSObjectValues().Single(predicate);
        }

        public static PSObject AsSingleOrDefault(this PSObject source)
        {
            return source.PSObjectValues().Single();
        }

        public static PSObject AsSingleOrDefault(this PSObject source, Func<PSObject, bool> predicate)
        {
            return source.PSObjectValues().Single(predicate);
        }

        public static IEnumerable<PSObject> SkipEnumerated(this PSObject source, int count)
        {
            return source.PSObjectValues().Skip(count);
        }

        public static IEnumerable<PSObject> SkipEnumeratedWhile(this PSObject source, Func<PSObject, bool> predicate)
        {
            return source.PSObjectValues().SkipWhile(predicate);
        }

        public static IEnumerable<PSObject> SkipEnumeratedWhile(this PSObject source, Func<PSObject, int, bool> predicate)
        {
            return source.PSObjectValues().SkipWhile(predicate);
        }

        public static IEnumerable<PSObject> TakeEnumerated(this PSObject source, int count)
        {
            return source.PSObjectValues().Take(count);
        }

        public static IEnumerable<PSObject> TakeEnumeratedWhile(this PSObject source, Func<PSObject, bool> predicate)
        {
            return source.PSObjectValues().TakeWhile(predicate);
        }

        public static IEnumerable<PSObject> TakeWhile(this PSObject source, Func<PSObject, int, bool> predicate)
        {
            return source.PSObjectValues().TakeWhile(predicate);
        }

        public static IEnumerable<PSObject> WhereEnumerated(this PSObject source, Func<PSObject, int, bool> predicate)
        {
            return source.PSObjectValues().Where(predicate);
        }

        public static IEnumerable<PSObject> WhereEnumerated(this PSObject source, Func<PSObject, bool> predicate)
        {
            return source.PSObjectValues().Where(predicate);
        }

        public static IEnumerable<PSObject> WhereBaseType<T>(this PSObject source, Func<T, int, bool> predicate)
        {
            return source.PSObjectValues().Where((o, v) => o != null && o.BaseObject is T && predicate((T)(o.BaseObject), v));
        }

        public static IEnumerable<PSObject> WhereBaseType<T>(this PSObject source, Func<T, bool> predicate)
        {
            return source.PSObjectValues().Where(o => o != null && o.BaseObject is T && predicate((T)(o.BaseObject)));
        }

        public static IEnumerable<PSObject> WhereBaseConvert<T>(this PSObject source, Func<T, int, bool> predicate)
        {
            return source.PSObjectValues().Where((o, v) =>
            {
                T r;
                return LanguagePrimitives.TryConvertTo<T>(o, out r) && predicate(r, v);
            });
        }

        public static IEnumerable<PSObject> WhereBaseConvert<T>(this PSObject source, Func<T, bool> predicate)
        {
            return source.PSObjectValues().Where(o =>
            {
                T r;
                return LanguagePrimitives.TryConvertTo<T>(o, out r) && predicate(r);
            });
        }

        public static IEnumerable<string> ToStringValues(this PSObject source)
        {
            if (source == null || source.BaseObject is string)
                return null;

            if (source.BaseObject is IEnumerable<string>)
                return source.BaseObject as IEnumerable<string>;

            if (source.BaseObject is IEnumerable && !(source.BaseObject is IDictionary))
                return (source.BaseObject as IEnumerable).Cast<object>().Select(o => (o != null && o is PSObject) ? (o as PSObject).BaseObject : o)
                    .SelectMany<object, string>(o =>
                {
                    if (o == null || o is string)
                        return new string[] { o as string };
                    if (o is IEnumerable<string>)
                        return o as IEnumerable<string>;
                    string s;
                    if (LanguagePrimitives.TryConvertTo<string>(o, out s))
                        return new string[] { s };
                    return new string[] { o.ToString() };
                });

            string v;
            if (LanguagePrimitives.TryConvertTo<string>(source, out v))
                return new string[] { v };
            return new string[] { source.ToString() };
        }
    
        public static IEnumerable<string> ToStringValues(this IEnumerable<PSObject> source)
        {
            if (source == null)
                return new string[0];
            return source.SelectMany<PSObject, string>(s => s.ToStringValues());
        }
    }
}
