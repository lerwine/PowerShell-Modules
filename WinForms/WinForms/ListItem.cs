using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Erwine.Leonard.T.WinForms
{
    public class ListItem : IEquatable<ListItem>
    {
        public object Value { get; private set; }

        public string DisplayText { get; private set; }

        private Dictionary<Type, MethodInfo> _equatableMethods = new Dictionary<Type, MethodInfo>();
        private Dictionary<Type, MethodInfo> _comparableMethods = new Dictionary<Type, MethodInfo>();
        private Func<object, bool> _lastResortEquals;

        public ListItem() : this(null) { }

        public ListItem(object value) : this(value, null) { }

        public ListItem(object value, string displayText)
        {
            this.Value = value;

            if (value == null)
            {
                this._equatableMethods = new Dictionary<Type, MethodInfo>();
                this._comparableMethods = new Dictionary<Type, MethodInfo>();
                this._lastResortEquals = (object o) => o == null;
                this.DisplayText = (displayText == null) ? "" : displayText;
                return;
            }

            Type type = this.Value.GetType();
            Type g = typeof(IEquatable<>);
            var genericTypes = type.GetInterfaces().Where(t => t.IsGenericType).Select(t => new { G = t, D = t.GetGenericTypeDefinition() }).ToArray();
            this._equatableMethods = genericTypes.Where(a => a.D.Equals(g)).Select(a => new { G = a.G, Key = a.G.GetGenericArguments().First() })
                .Select(a => new { Key = a.Key, Value = a.G.GetMethod("Equals") })
                .ToDictionary(a => a.Key, a => a.Value);
            g = typeof(IComparable<>);
            this._comparableMethods = genericTypes.Where(a => a.D.Equals(g)).Select(a => new { G = a.G, Key = a.G.GetGenericArguments().First() })
                .Select(a => new { Key = a.Key, Value = a.G.GetMethod("CompareTo") })
                .ToDictionary(a => a.Key, a => a.Value);

            if (this.Value is IComparable)
                this._lastResortEquals = (object o) => o != null && (this.Value as IComparable).CompareTo(o) == 0;
            else if (type.IsValueType)
                this._lastResortEquals = (object o) => o != null && this.Value.Equals(o);
            else
                this._lastResortEquals = (object o) => o != null && (Object.ReferenceEquals(this.Value, o) || this.Value.Equals(o));

            this.DisplayText = (displayText == null) ? value.ToString() : displayText;
        }

        public bool Equals(ListItem other) { return this.Equals((other == null) ? null : other.Value); }

        public override bool Equals(object obj)
        {
            if (this.Value == null)
                return obj == null;

            if (obj == null)
                return false;

            if (obj is ListItem)
                return this.Equals(obj as ListItem);

            Type yType = obj.GetType();

            if (this._equatableMethods.ContainsKey(yType))
                return (bool)(this._equatableMethods[yType].Invoke(this.Value, new object[] { obj }));

            if (this._comparableMethods.ContainsKey(yType))
                return (int)(this._equatableMethods[yType].Invoke(this.Value, new object[] { obj })) == 0;

            Type t = this._equatableMethods.Keys.FirstOrDefault(k => k.IsAssignableFrom(yType));
            if (t != null)
                return (bool)(this._equatableMethods[t].Invoke(this.Value, new object[] { obj }));

            t = this._comparableMethods.Keys.FirstOrDefault(k => k.IsAssignableFrom(yType));
            if (t != null)
                return (int)(this._equatableMethods[t].Invoke(this.Value, new object[] { obj })) == 0;

            return this._lastResortEquals(obj);
        }

        public override int GetHashCode() { return this.DisplayText.GetHashCode(); }

        public override string ToString() { return this.DisplayText; }
    }
}
