using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Management.Automation.Host;

namespace FileSystemIndexLib
{
	public class CommonSequenceSearcher<T> : IList<T>, IEquatable<CommonSequenceSearcher<T>>
	{
		private IEqualityComparer<T> _comparer;
		private int _parentIndex = 0;
		private int _count = 0;
		private List<T> _innerList = new List<T>();

		public int RelativeIndex { get { return _parentIndex; } }

		public int Count { get { return _count; } }

		public T this[int index]
		{
			get
			{
				if (index < 0 || index >= _count)
					throw new IndexOutOfRangeException();

				return _innerList[_parentIndex + _count];
			}
		}

		T IList<T>.this[int index]
		{
			get { return this[index]; }
			set { throw new NotSupportedException(); }
		}
	
		private CommonSequenceSearcher(List<T> innerList, int parentIndex, int count, IEqualityComparer<T> comparer)
		{
			_innerList = innerList;
			_comparer = comparer;
			_parentIndex = parentIndex;
			_count = count;
		}

		public CommonSequenceSearcher(IEnumerable<T> items) : this(items, null) { }

		public CommonSequenceSearcher(IEnumerable<T> items, IEqualityComparer<T> comparer)
		{
			_innerList = new List<T>();
			_comparer = (comparer == null) ? EqualityComparer<T>.Default : comparer;
			if (items == null)
				return;
			foreach (T i in items)
				_innerList.Add(i);
			_count = _innerList.Count;
		}

		public int IndexOf(T item)
		{
			int index = 0;
			foreach (T i in AsEnumerable())
			{
				if (_comparer.Equals(i, item))
					return index;
				index++;
			}
			return -1;
		}
	
		void IList<T>.Insert(int index, T item) { throw new NotSupportedException(); }

		void IList<T>.RemoveAt(int index) { throw new NotSupportedException(); }
		
		bool ICollection<T>.IsReadOnly { get { return true; } }
	
		void ICollection<T>.Add(T item) { throw new NotSupportedException(); }
	
		void ICollection<T>.Clear() { throw new NotSupportedException(); }
	
		public bool Contains(T item)
		{
			foreach (T i in AsEnumerable())
			{
				if (_comparer.Equals(i, item))
					return true;
			}
			return false;
		}
	
		public void CopyTo(T[] array, int arrayIndex) { (new List<T>(AsEnumerable())).CopyTo(array, arrayIndex); }

		bool ICollection<T>.Remove(T item) { throw new NotSupportedException(); }
		
		public IEnumerator<T> GetEnumerator() { return AsEnumerable().GetEnumerator(); }

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return new List<T>(AsEnumerable()).ToArray().GetEnumerator(); }

		public IEnumerable<T> AsEnumerable()
		{
			for (int i = 0; i < _count; i++)
				yield return _innerList[_parentIndex + i];
		}

		public CommonSequenceSearcher<T> Take(int count)
		{
			if (_count == 0 || count >= _count)
				return this;
			return new CommonSequenceSearcher<T>(_innerList, _parentIndex, (count < 0) ? 0 : count, _comparer);
		}

		public CommonSequenceSearcher<T> Skip(int count)
		{
			if (_count == 0 || count == 0)
				return this;
			if (count >= _count)
				return new CommonSequenceSearcher<T>(_innerList, _parentIndex + _count, 0, _comparer);
			return new CommonSequenceSearcher<T>(_innerList, _parentIndex + count, _count - count, _comparer);
		}

		public CommonSequenceSearcher<T> TakeWhile(Func<T, int, bool> predicate)
		{
			if (predicate == null)
				throw new ArgumentNullException("predicate");

			if (_count == 0)
				return this;

			for (int count = 0; count < _count; count++)
			{
				if (!predicate(_innerList[_parentIndex + count], count))
					return Take(count);
			}

			return this;
		}

		public CommonSequenceSearcher<T> TakeWhile(Func<T, bool> predicate)
		{
			if (predicate == null)
				throw new ArgumentNullException("predicate");

			if (_count == 0)
				return this;

			for (int count = 0; count < _count; count++)
			{
				if (!predicate(_innerList[_parentIndex + count]))
					return Take(count);
			}

			return this;
		}

		public CommonSequenceSearcher<T> SkipWhile(Func<T, int, bool> predicate)
		{
			if (predicate == null)
				throw new ArgumentNullException("predicate");

			if (_count == 0 || !predicate(_innerList[_parentIndex], 0))
				return this;

			for (int count = 1; count < _count; count++)
			{
				if (!predicate(_innerList[_parentIndex + count], count))
					return Skip(count);
			}

			return Skip(_count);
		}

		public CommonSequenceSearcher<T> SkipWhile(Func<T, bool> predicate)
		{
			if (predicate == null)
				throw new ArgumentNullException("predicate");

			if (_count == 0 || !predicate(_innerList[_parentIndex]))
				return this;

			for (int count = 1; count < _count; count++)
			{
				if (!predicate(_innerList[_parentIndex + count]))
					return Skip(count);
			}

			return Skip(_count);
		}

		/// <summary>
		/// Finds longest consecutive elements which are equal.
		/// </summary>
		/// <param name="otherCommonSequenceSearcher">Other <see cref="(CommonSequenceSearcher<T>" /> to compare.</param>
		/// <param name="currentIndex">Zero-based index in current <see cref="(CommonSequenceSearcher<T>" /> where the common sequence begins.</param>
		/// <param name="otherIndex">Zero-based i in <paramref name="otherCommonSequenceSearcher" /> where the common sequence begins.</param>
		/// <returns>Number of items in common sequence, or zero if no common sequence was found.</returns>
		public int GetLongestCommonSequence(CommonSequenceSearcher<T> otherCommonSequenceSearcher, out int currentIndex, out int otherIndex)
		{
			return GetLongestCommonSequence(otherCommonSequenceSearcher, null, out currentIndex, out otherIndex);
		}
		
		/// <summary>
		/// Finds longest consecutive elements which are equal.
		/// </summary>
		/// <param name="otherCommonSequenceSearcher">Other <see cref="(CommonSequenceSearcher<T>" /> to compare.</param>
		/// <param name="ui">PowerShell host user interface for writing debug information.</param>
		/// <param name="currentIndex">Zero-based index in current <see cref="(CommonSequenceSearcher<T>" /> where the common sequence begins.</param>
		/// <param name="otherIndex">Zero-based i in <paramref name="otherCommonSequenceSearcher" /> where the common sequence begins.</param>
		/// <returns>Number of items in common sequence, or zero if no common sequence was found.</returns>
		public int GetLongestCommonSequence(CommonSequenceSearcher<T> otherCommonSequenceSearcher, PSHostUserInterface ui, out int currentIndex, out int otherIndex)
		{
			if (otherCommonSequenceSearcher == null)
				throw new ArgumentNullException("otherCommonSequenceSearcher");

			if (ReferenceEquals(this, otherCommonSequenceSearcher))
			{
				if (ui != null)
					ui.WriteDebugLine("References are the same.");
				currentIndex = 0;
				otherIndex = 0;
				return _count;
			}
			
			if (_count == 0 || otherCommonSequenceSearcher._count == 0)
			{
				if (ui != null)
					ui.WriteDebugLine("Greatest common count is zero.");
				currentIndex = 0;
				otherIndex = 0;
				return 0;
			}

			int currentEndIndex = _parentIndex + _count;
			int otherEndIndex = otherCommonSequenceSearcher._parentIndex + otherCommonSequenceSearcher._count;
			for (int count = (_count < otherCommonSequenceSearcher._count) ? _count : otherCommonSequenceSearcher._count; count > 0; count--)
			{
				for (int currentStartIndex = _parentIndex; currentStartIndex <= (currentEndIndex - count); currentStartIndex++)
				{
					for (int otherStartIndex = otherCommonSequenceSearcher._parentIndex; otherStartIndex <= (otherEndIndex - count); otherStartIndex++)
					{
						if (ui != null)
							ui.WriteDebugLine(String.Format("count = {0}; currentStartIndex = {1}; otherStartIndex = {2}", count, currentStartIndex, otherStartIndex));
						if (Equals(otherCommonSequenceSearcher, currentStartIndex, otherStartIndex, count))
						{
							currentIndex = currentStartIndex - _parentIndex;
							otherIndex = otherStartIndex - otherCommonSequenceSearcher._parentIndex;
							return count;
						}
					}
				}
			}
		
			if (ui != null)
				ui.WriteDebugLine("No matches.");
			currentIndex = 0;
			otherIndex = 0;
			return 0;
		}
		
		private bool Equals(CommonSequenceSearcher<T> other, int currentParentIndex, int otherParentIndex, int count)
		{
			for (int index = 0; index < count; index++)
			{
				if (!_comparer.Equals(_innerList[currentParentIndex + index], other._innerList[otherParentIndex + index]))
					return false;
			}
			
			return true;
		}

		public bool Equals(CommonSequenceSearcher<T> other)
		{
			if (other == null)
				return false;

			if (ReferenceEquals(this, other))
				return true;

			if (_count != other._count)
				return false;
			
			return Equals(other, _parentIndex, other._parentIndex, _count);
		}

		public override bool Equals(object obj) { return Equals(obj as CommonSequenceSearcher<T>); }

		public override int GetHashCode()
		{
			int hashCode = 0;
			unchecked
			{
				foreach (T i in AsEnumerable())
					hashCode += _comparer.GetHashCode(i);
			}
			return hashCode;
		}
	}
}