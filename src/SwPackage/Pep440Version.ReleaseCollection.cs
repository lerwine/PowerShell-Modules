using System.Collections.ObjectModel;

namespace SwPackage;

public partial class Pep440Version
{
    class ReleaseCollection : Collection<int>, IEquatable<IEnumerable<int>>, IComparable<IEnumerable<int>>
    {
        public ReleaseCollection() => Add(0);

        internal ReleaseCollection(int initialValue)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(initialValue);
            Add(initialValue);
        }

        protected override void ClearItems()
        {
            base.ClearItems();
            Add(0);
        }

        protected override void InsertItem(int index, int item)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(item);
            base.InsertItem(index, item);
        }

        protected override void RemoveItem(int index)
        {
            base.RemoveItem(index);
            if (Count == 0) Add(0);
        }

        protected override void SetItem(int index, int item)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(item);
            base.SetItem(index, item);
        }

        public int CompareTo(IEnumerable<int>? other)
        {
            if (other == null) return 1;
            if (ReferenceEquals(this, other)) return 0;
            using (IEnumerator<int> x = GetEnumerator())
            using (IEnumerator<int> y = other.GetEnumerator())
            {
                while (x.MoveNext())
                {
                    if (!y.MoveNext())
                    {
                        do
                        {
                            if (x.Current != 0) return 1;
                        }
                        while (x.MoveNext());
                    }
                    int i = x.Current.CompareTo(y.Current);
                    if (i != 0) return i;
                }
                while (y.MoveNext())
                {
                    if (y.Current != 0) return -1;
                }
            }
            return 0;
        }

        public bool Equals(IEnumerable<int>? other)
        {
            if (other == null) return false;
            if (ReferenceEquals(this, other)) return true;
            using (IEnumerator<int> x = GetEnumerator())
            using (IEnumerator<int> y = other.GetEnumerator())
            {
                while (x.MoveNext())
                {
                    if (!y.MoveNext())
                    {
                        do
                        {
                            if (x.Current != 0) return false;
                        }
                        while (x.MoveNext());
                    }
                    if (x.Current != y.Current) return false;
                }
                while (y.MoveNext())
                {
                    if (y.Current != 0) return false;
                }
            }
            return true;
        }

        public override bool Equals(object? obj)
        {
            if (obj == null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj is IEnumerable<int>)
                return Equals((IEnumerable<int>)obj);
            string? s = obj.ToString();
            return !string.IsNullOrEmpty(s) && string.Equals(ToString(), s, StringComparison.OrdinalIgnoreCase);
        }

        public override int GetHashCode()
        {
            int hash = 2;
            unchecked
            {
                foreach (int value in Items)
                    hash = hash * 5 + value;
            }
            return hash;
        }

        public override string ToString()
        {
            int[] values = [.. Items];
            int e = values.Length - 1;
            if (e < 0) return "0";
            if (e == 0) return values[0].ToString();
            if (values[e] == 0)
            {
                do { e--; } while (e > 0 && values[e] == 0);
                if (e == 0) return values[0].ToString();
                return string.Join('.', values.Take(e).ToArray());
            }
            return string.Join('.', values);
        }
    }
}
