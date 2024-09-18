using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Threading;

namespace LteDev.RegexParsing
{
    [Obsolete("Use PatternSourceReader")]
    public sealed class CharacterStream : IReadOnlyList<char>, IRevertibleChangeTracking
    {
        private readonly object _syncRoot = new();
        private string _value;
        private int _originalStartIndex;
        private int _originalEndIndex;
        private int _startIndex;
        private int _endIndex;

        /// <summary>
        /// Gets the number of characters in the current <see cref="CharacterStream"/> object.
        /// </summary>
        public int Count { get; private set; }

        /// <summary>
        /// Gets a value indicating whether any characters have been removed from the begiinning or end of the current <see cref="CharacterStream"/>.
        /// </summary>
        /// <value><see langword="true"/> if a call to </value>
        public bool IsChanged { get { return _originalStartIndex != _startIndex || _originalEndIndex != _endIndex; } }

        /// <summary>
        /// Gets the <see cref="char"/> value at a specified position in the current <see cref="CharacterStream"/>.
        /// </summary>
        /// <param name="index">A position in the current <see cref="CharacterStream"/>.</param>
        /// <returns>The <see cref="char"/> value at position index.</returns>
        /// <exception cref="IndexOutOfRangeException"><paramref name="index"/> is greater than or equal to the <see cref="Count"/> of <see cref="char"/> in this <see cref="CharacterStream"/> or less than zero.</exception>
        public char this[int index]
        {
            get
            {
                Monitor.Enter(_syncRoot);
                try
                {
                    if (index < 0 || (index += _startIndex) >= _endIndex)
                        throw new IndexOutOfRangeException();
                    return _value[index];
                }
                finally { Monitor.Exit(_syncRoot); }
            }
        }

        public CharacterStream(string value, int startIndex, int length)
        {
            if ((_startIndex = startIndex) < 0 || startIndex > (_value = value ?? "").Length)
                throw new ArgumentOutOfRangeException(nameof(startIndex));
            if ((Count = length) < 0 || (_endIndex = startIndex + length) > _value.Length)
                throw new ArgumentOutOfRangeException(nameof(length));
            _originalStartIndex = _startIndex;
            _originalEndIndex = _endIndex;
        }

        public CharacterStream(string value)
        {
            _originalStartIndex = _startIndex = 0;
            _originalEndIndex = _endIndex = (_value = value ?? "").Length;
        }

        public Match GetMatch(Regex regex)
        {
            ArgumentNullException.ThrowIfNull(regex);
            Monitor.Enter(_syncRoot);
            try { return regex.Match(_value, _startIndex, Count); }
            finally { Monitor.Exit(_syncRoot); }
        }

        public Match GetMatch(Regex regex, int startAt)
        {
            ArgumentNullException.ThrowIfNull(regex);
            Monitor.Enter(_syncRoot);
            try
            {
                if (startAt < 0 || startAt > Count)
                    throw new ArgumentOutOfRangeException(nameof(startAt));
                return regex.Match(_value, _startIndex + startAt, Count - startAt);
            }
            finally { Monitor.Exit(_syncRoot); }
        }

        public Match GetMatch(Regex regex, int startAt, int length)
        {
            ArgumentNullException.ThrowIfNull(regex);
            Monitor.Enter(_syncRoot);
            try
            {
                if (startAt < 0 || startAt > Count)
                    throw new ArgumentOutOfRangeException(nameof(startAt));
                if (length < 0 || (length + startAt) > Count)
                    throw new ArgumentOutOfRangeException(nameof(length));
                return regex.Match(_value, _startIndex + startAt, length);
            }
            finally { Monitor.Exit(_syncRoot); }
        }

        /// <summary>
        /// Creates a <see cref="string"/> from a segment of characters in the current <see cref="CharacterStream"/>, starting at a specific position.
        /// </summary>
        /// <param name="startIndex">The starting index of characters to be copied.</param>
        /// <param name="length">The number of charcters to copy.</param>
        /// <returns>A <see cref="string"/> containing characters from the <see cref="CharacterStream"/> according to the specified starting index and length.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Of the following has occurred:
        /// <list type="bullet">
        /// <item><paramref name="startIndex"/> or <paramref name="length"/> is less than zero.</item>
        /// <item><paramref name="startIndex"/> plus <paramref name="length"/> is greater than <see cref="Count"/>.</item>
        /// </list>.</exception>
        public string ToString(int startIndex, int length)
        {
            Monitor.Enter(_syncRoot);
            try
            {
                if (startIndex < 0 || startIndex > Count)
                    throw new ArgumentOutOfRangeException(nameof(startIndex));
                if (length < 0 || (length + startIndex) > Count)
                    throw new ArgumentOutOfRangeException(nameof(length));
                return _value.Substring(_startIndex + startIndex, length);
            }
            finally { Monitor.Exit(_syncRoot); }
        }

        public override string ToString()
        {
            Monitor.Enter(_syncRoot);
            try { return (_endIndex < _value.Length) ? _value.Substring(_startIndex, Count) : (_startIndex > 0) ? _value[_startIndex..] : _value; }
            finally { Monitor.Exit(_syncRoot); }
        }

        /// <summary>
        /// Gets an array containing a segment of <see cref="char"/> values in the current <see cref="CharacterStream"/>, starting at a specific position.
        /// </summary>
        /// <param name="startIndex">The starting index of characters to be copied.</param>
        /// <param name="length">The number of charcters to copy.</param>
        /// <returns>An array of <see cref="char"/> values containing characters from the <see cref="CharacterStream"/> according to the specified starting index and length.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Of the following has occurred:
        /// <list type="bullet">
        /// <item><paramref name="startIndex"/> or <paramref name="length"/> is less than zero.</item>
        /// <item><paramref name="startIndex"/> plus <paramref name="length"/> is greater than <see cref="Count"/>.</item>
        /// </list>.</exception>
        public char[] ToCharArray(int startIndex, int length)
        {
            Monitor.Enter(_syncRoot);
            try
            {
                if (startIndex < 0 || startIndex > Count)
                    throw new ArgumentOutOfRangeException(nameof(startIndex));
                if (length < 0 || (length + startIndex) > Count)
                    throw new ArgumentOutOfRangeException(nameof(length));
                return _value.ToCharArray(_startIndex + startIndex, length);
            }
            finally { Monitor.Exit(_syncRoot); }
        }

        /// <summary>
        /// Gets an array containing the <see cref="char"/> values in the current <see cref="CharacterStream"/>.
        /// </summary>
        /// <returns>An array of <see cref="char"/> values copied from the current <see cref="CharacterStream"/>.</returns>
        public char[] ToCharArray()
        {
            Monitor.Enter(_syncRoot);
            try { return (_endIndex < _value.Length || _startIndex > 0) ? _value.ToCharArray(_startIndex, Count) : _value.ToCharArray(); }
            finally { Monitor.Exit(_syncRoot); }
        }

        /// <summary>
        /// Splits the current <see cref="CharacterStream"/> into a new <see cref="CharacterStream"/> after the specified length.
        /// </summary>
        /// <param name="length">The number of characters to retain in the current <see cref="CharacterStream"/>.</param>
        /// <returns>A <see cref="CharacterStream"/> containing the characters that were in the current <see cref="CharacterStream"/> beyond the specified <paramref name="length"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="length"/> is less than zero or greater than <see cref="Count"/>.</exception>
        public CharacterStream Split(int length)
        {
            Monitor.Enter(_syncRoot);
            try
            {
                if (length < 0 || length > Count)
                    throw new ArgumentOutOfRangeException(nameof(length));
                CharacterStream result = new((length < Count) ? _value.Substring(_startIndex + length, Count - length) : string.Empty);
                _startIndex += length;
                Count -= length;
                return result;
            }
            finally { Monitor.Exit(_syncRoot); }
        }

        /// <summary>
        /// Tests a specific number of <see cref="char"/> values at the beginning of the current <see cref="CharacterStream"/>.
        /// </summary>
        /// <param name="predicate">The function that tests each <see cref="char"/> value.</param>
        /// <param name="count">The number of characters to test.</param>
        /// <returns><see langword="true"/> if the <paramref name="predicate"/> returned <see langword="true"/> for each character (and <paramref name="count"/> is greater than zero); otherwise, <see langword="false"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="predicate"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count"/> is less than zero or greater than <see cref="Count"/>.</exception>
        public bool Test(Func<char, bool> predicate, int count)
        {
            ArgumentNullException.ThrowIfNull(predicate);
            Monitor.Enter(_syncRoot);
            try
            {
                if (count < 0 || count > Count)
                    throw new ArgumentOutOfRangeException(nameof(count));
                if (count > 0)
                {
                    count += _startIndex;
                    for (int i = _startIndex; i < count; i++)
                    {
                        if (!predicate(_value[i]))
                            return false;
                    }
                }
            }
            finally { Monitor.Exit(_syncRoot); }
            return true;
        }

        /// <summary>
        /// Sequentially tests <see cref="char"/> values from the beginning of the current <see cref="CharacterStream"/> with a corresponding sequence of predicate functions.
        /// </summary>
        /// <param name="predicate">Predicate functions to test each sequential character.</param>
        /// <returns><see langword="true"/> if each <paramref name="predicate"/> returned <see langword="true"/> for each sequential character; otherwise, <see langword="false"/> if any <paramref name="predicate"/> returned <see langword="false"/>
        /// or the number if <paramref name="predicate"/> parameters exceeds the number of characters in the current <see cref="CharacterStream"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="predicate"/> is <see langword="null"/>.</exception>
        public bool TestWithEach(params Func<char, bool>[] predicate)
        {
            ArgumentNullException.ThrowIfNull(predicate);
            Monitor.Enter(_syncRoot);
            try
            {
                if (predicate.Length > Count)
                    return false;
                for (int i = _startIndex; i < predicate.Length; i++)
                {
                    Func<char, bool> p = predicate[i];
                    if (p is null)
                        throw new ArgumentNullException(nameof(predicate));
                    if (!p(_value[_startIndex + i]))
                        return false;
                }
            }
            finally { Monitor.Exit(_syncRoot); }
            return true;
        }

        public bool TryReadNext(out char c)
        {
            Monitor.Enter(_syncRoot);
            try
            {
                if (_startIndex < _endIndex)
                {
                    c = _value[_startIndex++];
                    return true;
                }
            }
            finally { Monitor.Exit(_syncRoot); }
            c = default;
            return false;
        }

        /// <summary>
        /// Attempts to evaluate and read a specified number of characters from the beginning of the current <see cref="CharacterStream"/>, removing them from the current <see cref="CharacterStream"/> if successful.
        /// </summary>
        /// <param name="predicate">The predicate that evaluates each <see cref="char"/> value.</param>
        /// <param name="count">The number of characters to evaluate.</param>
        /// <param name="values">The characters that were removed from the beginning of the current <see cref="CharacterStream"/> or an empty array if no characters were removed.</param>
        /// <returns><see langword="true"/> if the <paramref name="predicate"/> returned <see langword="true"/> for the specified <paramref name="count"/> of characters and they were removed from the beginning of the current <see cref="CharacterStream"/>;
        /// otherwise, <see langword="false"/> if <paramref name="count"/> is greater than the number of characters in the current <see cref="CharacterStream"/> or the <paramref name="predicate"/> returned <see langword="false"/>
        /// for any of the <see cref="char"/> values.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="predicate"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count"/> is less than zero or greater than <see cref="Count"/>.</exception>
        public bool TryRead(Func<char, bool> predicate, int count, out char[] values)
        {
            ArgumentNullException.ThrowIfNull(predicate);
            Monitor.Enter(_syncRoot);
            try
            {
                if (count < 0 || count > Count)
                    throw new ArgumentOutOfRangeException(nameof(count));
                values = new char[count];
                if (count == 0)
                    return false;
                for (int i = _startIndex; i < count; i++)
                {
                    char c = _value[_startIndex + i];
                    if (!predicate(c))
                    {
                        values = [];
                        return false;
                    }
                    values[i] = c;
                }
                _startIndex += count;
                Count -= count;
            }
            finally { Monitor.Exit(_syncRoot); }
            return true;
        }

        /// <summary>
        /// Evaluates and reads characters from the beginning of the current <see cref="CharacterStream"/> until the predicate function returns <see langword="false"/>,
        /// removing each chararacter successfully predicated character from the <see cref="CharacterStream"/>.
        /// </summary>
        /// <param name="predicate">The predicate that evaluates each <see cref="char"/> value.</param>
        /// <returns>The <see cref="char"/> values that were removed from the current <see cref="CharacterStream"/> or an empty array if the current <see cref="CharacterStream"/> is empty or 
        /// the <paramref name="predicate"/> returned <see langword="false"/> for the first character.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="predicate"/> is <see langword="null"/>.</exception>
        public char[] ReadWhile(Func<char, bool> predicate)
        {
            ArgumentNullException.ThrowIfNull(predicate);
            Monitor.Enter(_syncRoot);
            try
            {
                if (Count == 0)
                    return [];
                int position = _startIndex;
                char c = _value[position];
                if (!predicate(c))
                    return [];
                List<char> result = [c];
                while (++position < _endIndex && predicate(c))
                    result.Add(c);
                Count = _endIndex - (_startIndex = position);
                return [.. result];
            }
            finally { Monitor.Exit(_syncRoot); }
        }

        /// <summary>
        /// Evaluates and reads characters from the beginning of the current <see cref="CharacterStream"/> until the predicate function returns <see langword="false"/> or the maximum count was reached,
        /// removing each chararacter successfully predicated character from the <see cref="CharacterStream"/>.
        /// </summary>
        /// <param name="maxCount">The maximum number of characters to read and evaluate.</param>
        /// <param name="predicate">The predicate that evaluates each <see cref="char"/> value.</param>
        /// <param name="result">The <see cref="char"/> values that were removed from the current <see cref="CharacterStream"/> or an empty array if the current <see cref="CharacterStream"/> is empty, <paramref name="maxCount"/> is zero, or 
        /// the <paramref name="predicate"/> returned <see langword="false"/> for the first character.</param>
        /// <returns><see langword="true"/> if the <paramref name="predicate"/> returned true for each <see cref="char"/> value until the <paramref name="maxCount"/> was reached; otherwise <see langword="false"/> if
        /// the <paramref name="predicate"/> returned <see langword="false"/> for a character.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="predicate"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="maxCount"/> is less than zero.</exception>
        public bool ReadWhile(int maxCount, Func<char, bool> predicate, out char[] result)
        {
            ArgumentNullException.ThrowIfNull(predicate);
            Monitor.Enter(_syncRoot);
            try
            {
                if (maxCount < 0)
                    throw new ArgumentOutOfRangeException(nameof(maxCount));
                if (maxCount > Count)
                    maxCount = Count;
                if (maxCount > 0)
                {
                    int position = _startIndex;
                    char c = _value[position];
                    if (predicate(c))
                    {
                        List<char> list = [c];
                        while (++position < _endIndex)
                        {
                            if (predicate(c))
                                list.Add(c);
                            else
                            {
                                Count = _endIndex - (_startIndex = position);
                                result = [.. list];
                                return false;
                            }
                        }
                        Count = _endIndex - (_startIndex = position);
                        result = [.. list];
                        return true;
                    }
                }
                result = [];
                return false;
            }
            finally { Monitor.Exit(_syncRoot); }
        }

        /// <summary>
        /// Sequentially evaluates and reads characters from the beginning of the current <see cref="CharacterStream"/> until the corresponding sequential predicate function returns <see langword="false"/>, removing the characters that
        /// were successfully evaluated.
        /// </summary>
        /// <param name="predicate">The sequence of predicate functions that evaluate each corresponding <see cref="char"/> value.</param>
        /// <returns>The <see cref="char"/> values that were removed from the current <see cref="CharacterStream"/> or an empty array if the current <see cref="CharacterStream"/> is empty or 
        /// the first <paramref name="predicate"/> returned <see langword="false"/> for the first character.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="predicate"/> is <see langword="null"/>.</exception>
        public char[] ReadEachWhile(params Func<char, bool>[] predicate)
        {
            ArgumentNullException.ThrowIfNull(predicate);
            Monitor.Enter(_syncRoot);
            try
            {
                if (predicate.Length == 0 || predicate.Length > Count)
                    return [];
                Func<char, bool> p = predicate[0];
                if (p is null)
                    throw new ArgumentNullException(nameof(predicate));
                int position = _startIndex;
                char c = _value[position];
                if (!p(c))
                    return [];
                List<char> result = new List<char>();
                result.Add(c);
                while (++position < predicate.Length)
                {
                    p = predicate[position];
                    if (p is null)
                        throw new ArgumentNullException(nameof(predicate));
                    if (!p(_value[_startIndex + position]))
                        break;
                    result.Add(c);
                }
                Count = _endIndex - (_startIndex = position);
                return [.. result];
            }
            finally { Monitor.Exit(_syncRoot); }
        }

        /// <summary>
        /// Applies a predicated accumulator to each <see cref="char"/> value from the beginning of the current <see cref="CharacterStream"/> until the predicate function returns <see langword="false"/>, removing the characters that
        /// were successfully evaluated.
        /// </summary>
        /// <typeparam name="T">The type of accumulated value.</typeparam>
        /// <param name="seed">The initial accumulated value.</param>
        /// <param name="predicatedAccumulator">The function that evaluates each <see cref="char"/> value, updating the accumulated value if successfully evaluated.</param>
        /// <returns>The accumulated value derived from the character values removed from the current <see cref="CharacterStream"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="predicatedAccumulator"/> is <see langword="null"/>.</exception>
        public T AccumulateWhile<T>(T seed, TryFunc<T, char, T> predicatedAccumulator)
        {
            ArgumentNullException.ThrowIfNull(predicatedAccumulator);
            Monitor.Enter(_syncRoot);
            try
            {
                if (Count == 0)
                    return seed;
                int position = _startIndex;
                do
                {
                    if (predicatedAccumulator(seed, _value[position], out T accumulated))
                        seed = accumulated;
                    else
                        break;
                } while (++position < _endIndex);
                Count = _endIndex - (_startIndex = position);
            }
            finally { Monitor.Exit(_syncRoot); }
            return seed;
        }

        /// <summary>
        /// Applies a predicated accumulator to each <see cref="char"/> value from the beginning of the current <see cref="CharacterStream"/> until the predicate function returns <see langword="false"/>, removing the characters that
        /// were successfully evaluated.
        /// </summary>
        /// <typeparam name="T">The type of accumulated value.</typeparam>
        /// <param name="predicatedSeedFunc">The function that evaluates the first <see cref="char"/> value, creating the initial accumulated value if a <see langword="true"/> value is returned.</param>
        /// <param name="predicatedAccumulator">The function that evaluates each subsequent <see cref="char"/> value, updating the accumulated value if a <see langword="true"/> value is returned.</param>
        /// <param name="result">The accumulated value derived from the character values removed from the current <see cref="CharacterStream"/>.</param>
        /// <returns><see langword="true"/> if at least one <see cref="char"/> value was evaluated successfully; otherwise <see langword="false"/> if <see cref="Count"/> was zero.</returns>
        public bool AccumulateWhile<T>(TryFunc<char, T> predicatedSeedFunc, TryFunc<T, char, T> predicatedAccumulator, out T? result)
        {
            ArgumentNullException.ThrowIfNull(predicatedSeedFunc);
            ArgumentNullException.ThrowIfNull(predicatedAccumulator);
            Monitor.Enter(_syncRoot);
            try
            {
                if (Count == 0)
                {
                    result = default;
                    return false;
                }
                int position = _startIndex;
                if (!predicatedSeedFunc(_value[position], out result))
                {
                    result = default;
                    return false;
                }
                do
                {
                    T accumulated;
                    if (predicatedAccumulator(result, _value[position], out accumulated))
                        result = accumulated;
                    else
                        break;
                } while (++position < _endIndex);
                Count = _endIndex - (_startIndex = position);
            }
            finally { Monitor.Exit(_syncRoot); }
            return true;
        }

        /// <summary>
        /// Applies a sequence of predicated accumulator functions to each sequentially corresponding <see cref="char"/> values from the beginning of the current <see cref="CharacterStream"/> until a predicate function returns <see langword="false"/>,
        /// removing the characters that
        /// were successfully evaluated.
        /// </summary>
        /// <typeparam name="T">The type of accumulated value.</typeparam>
        /// <param name="seed">The initial accumulated value.</param>
        /// <param name="predicatedAccumulator">The functions that evaluate each sequentially corresponding <see cref="char"/> value, updating the accumulated value if successfully evaluated.</param>
        /// <returns>The accumulated value derived from the character values removed from the current <see cref="CharacterStream"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="predicatedAccumulator"/> is <see langword="null"/>.</exception>
        public T AccumulateEachWhile<T>(T seed, params TryFunc<T, char, T>[] predicatedAccumulator)
        {
            ArgumentNullException.ThrowIfNull(predicatedAccumulator);
            Monitor.Enter(_syncRoot);
            try
            {
                if (predicatedAccumulator.Length == 0 || predicatedAccumulator.Length > Count)
                    return seed;
                for (int i = 0; i < predicatedAccumulator.Length; i++)
                {
                    TryFunc<T, char, T> p = predicatedAccumulator[i];
                    if (p is null)
                        throw new ArgumentNullException(nameof(predicatedAccumulator));
                    T accumulated;
                    if (p(seed, _value[_startIndex + i], out accumulated))
                        seed = accumulated;
                    else
                    {
                        Count -= i;
                        _startIndex += i;
                        return seed;
                    }
                }
                Count -= predicatedAccumulator.Length;
                _startIndex += predicatedAccumulator.Length;
            }
            finally { Monitor.Exit(_syncRoot); }
            return seed;
        }

        public int RemoveFirst(int count)
        {
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));
            if (count < 1)
                return 0;
            Monitor.Enter(_syncRoot);
            try
            {
                if (count < Count)
                {
                    _startIndex += count;
                    Count -= count;
                }
                else
                {
                    count = Count;
                    _startIndex = _endIndex;
                    Count = 0;
                }
            }
            finally { Monitor.Exit(_syncRoot); }
            return count;
        }

        public int RemoveLast(int count)
        {
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));
            if (count < 1)
                return 0;
            Monitor.Enter(_syncRoot);
            try
            {
                if (count < Count)
                {
                    _endIndex -= count;
                    Count -= count;
                }
                else
                {
                    count = Count;
                    _endIndex = _startIndex;
                    Count = 0;
                }
            }
            finally { Monitor.Exit(_syncRoot); }
            return count;
        }

        /// <summary>
        /// Copies the characters from a specified segment of this instance to a specified segment of a destination <see cref="char"/> array.
        /// </summary>
        /// <param name="sourceIndex">The zero-based starting position in this instance where characters will be copied from.</param>
        /// <param name="destination">The array that will hold the copied characters.</param>
        /// <param name="destinationIndex">The zero-based starting position in destination array where characters will be copied.</param>
        /// <param name="count">The number of characters to be copied.</param>
        /// <exception cref="ArgumentNullException"><paramref name="destination"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Of the following has occurred:
        /// <list type="bullet">
        /// <item><paramref name="sourceIndex"/>m, <paramref name="destinationIndex"/> or <paramref name="count"/> is less than zero.</item>
        /// <item><paramref name="sourceIndex"/> plus <paramref name="count"/> is greater than <see cref="Count"/>.</item>
        /// <item><paramref name="destinationIndex"/> plus <paramref name="count"/> is greater than the length the <paramref name="destination"/> array.</item>
        /// </list>.</exception>
        public void CopyTo(int sourceIndex, char[] destination, int destinationIndex, int count)
        {
            Monitor.Enter(_syncRoot);
            try
            {
                if (sourceIndex < 0 || sourceIndex > Count)
                    throw new ArgumentOutOfRangeException(nameof(sourceIndex));
                if (count < 0 || (count + sourceIndex) > Count)
                    throw new ArgumentOutOfRangeException(nameof(count));
                _value.CopyTo(sourceIndex + _startIndex, destination, destinationIndex, count);
            }
            finally { Monitor.Exit(_syncRoot); }
        }

        public IEnumerator<char> GetEnumerator() { return new Enumerator(this); }

        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }

        public void RejectChanges()
        {
            Monitor.Enter(_syncRoot);
            try
            {
                _startIndex = _originalStartIndex;
                _endIndex = _originalEndIndex;
            }
            finally { Monitor.Exit(_syncRoot); }
        }

        public void AcceptChanges()
        {
            Monitor.Enter(_syncRoot);
            try
            {
                if (_originalStartIndex < _startIndex || _originalEndIndex > _endIndex)
                {
                    _value = _value.Substring(_originalStartIndex, Count);
                    _originalStartIndex = _startIndex = 0;
                    _originalEndIndex = _endIndex = Count;
                }
            }
            finally { Monitor.Exit(_syncRoot); }
        }

        class Enumerator : IEnumerator<char>
        {
            private readonly CharacterStream _target;
            private readonly int _startIndex;
            private readonly int _endIndex;
            private int _position;

            public char Current
            {
                get
                {
                    Monitor.Enter(_target._syncRoot);
                    try
                    {
                        if (_position < -1)
                            throw new ObjectDisposedException(GetType().FullName);
                        if (_position < _startIndex || _position == _endIndex || _startIndex != _target._startIndex || _endIndex != _target._endIndex)
                            throw new InvalidOperationException();
                        return _target._value[_position];
                    }
                    finally { Monitor.Exit(_target._syncRoot); }
                }
            }

            object IEnumerator.Current => throw new NotImplementedException();

            internal Enumerator(CharacterStream target)
            {
                ArgumentNullException.ThrowIfNull(target);
                Monitor.Enter(target._syncRoot);
                try
                {
                    _position = (_startIndex = (_target = target)._startIndex) - 1;
                    _endIndex = target._endIndex;
                }
                finally { Monitor.Exit(target._syncRoot); }
            }

            public void Dispose() { _position = -2; }

            public bool MoveNext()
            {
                Monitor.Enter(_target._syncRoot);
                try
                {
                    if (_position < -1)
                        throw new ObjectDisposedException(GetType().FullName);
                    if (_startIndex != _target._startIndex || _endIndex != _target._endIndex)
                        throw new InvalidOperationException();
                    return _position < _endIndex && ++_position < _endIndex;
                }
                finally { Monitor.Exit(_target._syncRoot); }
            }

            public void Reset()
            {
                Monitor.Enter(_target._syncRoot);
                try
                {
                    if (_position < -1)
                        throw new ObjectDisposedException(GetType().FullName);
                    if (_startIndex != _target._startIndex || _endIndex != _target._endIndex)
                        throw new InvalidOperationException();
                    _position = _startIndex - 1;
                }
                finally { Monitor.Exit(_target._syncRoot); }
            }
        }
    }
}
