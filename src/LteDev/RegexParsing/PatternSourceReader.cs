using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;

namespace LteDev.RegexParsing
{
    public sealed class PatternSourceReader : IRevertibleChangeTracking, IEnumerator<char>
    {
        private readonly object _syncRoot = new object();
        private readonly string _pattern;
        private int _originalPosition = -1;
        private char _orignalValue;
        private bool _wasEscaped;
        private int _position = -1;

        public bool IsChanged { get; private set; }

        public char Current { get; private set; }

        public bool IsEscaped { get; private set; }

        public bool EndOfPattern { get; private set; }

        object IEnumerator.Current { get { return Current; } }

        public PatternSourceReader(string pattern)
        {
            if (pattern == null) throw new ArgumentNullException("pattern");
        }

        private bool TryIterateNext(int currentPosition, out char c, out bool isEscaped, out int nextPosition)
        {
            if (currentPosition == _pattern.Length)
            {
                isEscaped = false;
                c = default;
                nextPosition = currentPosition;
                return false;
            }
            nextPosition = currentPosition + 1;
            c = _pattern[currentPosition];
            isEscaped = c == '\\';
            if (isEscaped)
            {
                isEscaped = (currentPosition = nextPosition) < _pattern.Length;
                if (isEscaped)
                {
                    nextPosition++;
                    c = _pattern[currentPosition];
                }
            }
            return true;
        }

        private bool TryIterateNext(int currentPosition, out char c, out int nextPosition)
        {
            if ((nextPosition = currentPosition) == _pattern.Length)
            {
                c = default;
                nextPosition = currentPosition;
                return false;
            }
            c = _pattern[currentPosition++];
            if (c == '\\')
                return false;
            nextPosition = currentPosition;
            return true;
        }

        private bool TryAggregateFollowing<T>(T seed, int count, int position, TryFunc<T, char, bool, T> evaluatedAggregator, out T result)
        {
            char c;
            bool isEscaped;
            int tested = 0;
            result = seed;
            while (TryIterateNext(position, out c, out isEscaped, out position) && evaluatedAggregator(result, c, isEscaped, out seed))
            {
                result = seed;
                if (++tested == count)
                {
                    SetPosition(position);
                    return true;
                }
            }
            return false;
        }

        private bool TryAggregateFollowing<T>(T seed, int count, int position, TryFunc<T, char, T> evaluatedAggregator, out T result)
        {
            char c;
            int tested = 0;
            result = seed;
            while (TryIterateNext(position, out c, out position) && evaluatedAggregator(result, c, out seed))
            {
                result = seed;
                if (++tested == count)
                {
                    SetPosition(position);
                    return true;
                }
            }
            return false;
        }

        private bool TryAggregateFollowingWhile<T>(T seed, int position, TryFunc<T, char, T> evaluatedAggregator, out T result)
        {
            char c;
            result = seed;
            while (TryIterateNext(position, out c, out position))
            {
                if (evaluatedAggregator(result, c, out seed))
                    result = seed;
                else
                {
                    SetPosition(position);
                    return true;
                }
            }
            return false;
        }

        private bool TryAggregateFollowingWhile<T>(T seed, int position, TryFunc<T, char, bool, T> evaluatedAggregator, out T result)
        {
            char c;
            result = seed;
            bool isEscaped;
            while (TryIterateNext(position, out c, out isEscaped, out position))
            {
                if (evaluatedAggregator(result, c, isEscaped, out seed))
                    result = seed;
                else
                {
                    SetPosition(position);
                    return true;
                }
            }
            return false;
        }

        private bool TryAggregateFollowingWhile<T>(T seed, int position, TryFunc<T, char, int, T> evaluatedAggregator, out T result)
        {
            char c;
            result = seed;
            int offset = -1;
            while (TryIterateNext(position, out c, out position))
            {
                if (evaluatedAggregator(result, c, ++offset, out seed))
                    result = seed;
                else
                {
                    SetPosition(position);
                    return true;
                }
            }
            return false;
        }

        private bool TryAggregateFollowingWhile<T>(T seed, int position, TryFunc<T, char, bool, int, T> evaluatedAggregator, out T result)
        {
            char c;
            result = seed;
            bool isEscaped;
            int offset = -1;
            while (TryIterateNext(position, out c, out isEscaped, out position))
            {
                if (evaluatedAggregator(result, c, isEscaped, ++offset, out seed))
                    result = seed;
                else
                {
                    SetPosition(position);
                    return true;
                }
            }
            return false;
        }

        public bool TryAggregateNext<T>(T seed, int count, TryFunc<T, char, T> evaluatedAggregator, out T result)
        {
            Monitor.Enter(_syncRoot);
            try
            {
                if (_position < -1) throw new ObjectDisposedException(GetType().FullName);
                if (evaluatedAggregator == null) throw new ArgumentNullException("evaluatedAggregator");
                if (count < 0) throw new ArgumentOutOfRangeException("count");
                int position = _position;
                char c;
                if (!EndOfPattern && count > 0 && TryIterateNext(position, out c, out position))
                    return TryAggregateFollowing(seed, count, position, evaluatedAggregator, out result);
                result = seed;
                return false;
            }
            finally { Monitor.Exit(_syncRoot); }
        }

        public bool TryAggregateNext<T>(T seed, int count, TryFunc<T, char, bool, T> evaluatedAggregator, out T result)
        {
            Monitor.Enter(_syncRoot);
            try
            {
                if (_position < -1) throw new ObjectDisposedException(GetType().FullName);
                if (evaluatedAggregator == null) throw new ArgumentNullException("evaluatedAggregator");
                if (count < 0) throw new ArgumentOutOfRangeException("count");
                int position = _position;
                char c;
                if (!EndOfPattern && count > 0 && TryIterateNext(position, out c, out position))
                    return TryAggregateFollowing(seed, count, position, evaluatedAggregator, out result);
                result = seed;
                return false;
            }
            finally { Monitor.Exit(_syncRoot); }
        }

        public bool TryAggregateNext<T>(int count, TryFunc<char, T> firstEvaluatedAggregator, TryFunc<T, char, T> followingEvaluatedAggregator, out T result)
        {
            Monitor.Enter(_syncRoot);
            try
            {
                if (_position < -1) throw new ObjectDisposedException(GetType().FullName);
                if (firstEvaluatedAggregator == null) throw new ArgumentNullException("firstEvaluatedAggregator");
                if (followingEvaluatedAggregator == null) throw new ArgumentNullException("followingEvaluatedAggregator");
                if (count < 0) throw new ArgumentOutOfRangeException("count");
                int position = _position;
                char c;
                if (!EndOfPattern && count > 0 && TryIterateNext(position, out c, out position) && TryIterateNext(position, out c, out position))
                {
                    if (firstEvaluatedAggregator(c, out result))
                        return --count == 0 || TryAggregateFollowing(result, count, position, followingEvaluatedAggregator, out result);
                }
                else
                    result = default;
                return false;
            }
            finally { Monitor.Exit(_syncRoot); }
        }

        public bool TryAggregateNext<T>(int count, TryFunc<char, bool, T> firstEvaluatedAggregator, TryFunc<T, char, bool, T> followingEvaluatedAggregator, out T result)
        {
            Monitor.Enter(_syncRoot);
            try
            {
                if (_position < -1) throw new ObjectDisposedException(GetType().FullName);
                if (firstEvaluatedAggregator == null) throw new ArgumentNullException("firstEvaluatedAggregator");
                if (followingEvaluatedAggregator == null) throw new ArgumentNullException("followingEvaluatedAggregator");
                if (count < 0) throw new ArgumentOutOfRangeException("count");
                int position = _position;
                char c;
                bool isEscaped;
                if (!EndOfPattern && count > 0 && TryIterateNext(position, out c, out position) && TryIterateNext(position, out c, out isEscaped, out position))
                {
                    if (firstEvaluatedAggregator(c, isEscaped, out result))
                        return --count == 0 || TryAggregateFollowing(result, count, position, followingEvaluatedAggregator, out result);
                }
                else
                    result = default;
                return false;
            }
            finally { Monitor.Exit(_syncRoot); }
        }

        public bool TryAggregateNextWhile<T>(T seed, TryFunc<T, char, T> evaluatedAggregator, out T result)
        {
            Monitor.Enter(_syncRoot);
            try
            {
                if (_position < -1) throw new ObjectDisposedException(GetType().FullName);
                if (evaluatedAggregator == null) throw new ArgumentNullException("evaluatedAggregator");
                int position = _position;
                char c;
                if (!EndOfPattern && TryIterateNext(position, out c, out position))
                    return TryAggregateFollowingWhile(seed, position, evaluatedAggregator, out result);
                result = seed;
                return false;

            }
            finally { Monitor.Exit(_syncRoot); }
        }

        public bool TryAggregateNextWhile<T>(T seed, TryFunc<T, char, bool, T> evaluatedAggregator, out T result)
        {
            Monitor.Enter(_syncRoot);
            try
            {
                if (_position < -1) throw new ObjectDisposedException(GetType().FullName);
                if (evaluatedAggregator == null) throw new ArgumentNullException("evaluatedAggregator");
                int position = _position;
                char c;
                if (!EndOfPattern && TryIterateNext(position, out c, out position))
                    return TryAggregateFollowingWhile(seed, position, evaluatedAggregator, out result);
                result = seed;
                return false;

            }
            finally { Monitor.Exit(_syncRoot); }
        }

        public bool TryAggregateNextWhile<T>(T seed, TryFunc<T, char, int, T> evaluatedAggregator, out T result)
        {
            Monitor.Enter(_syncRoot);
            try
            {
                if (_position < -1) throw new ObjectDisposedException(GetType().FullName);
                if (evaluatedAggregator == null) throw new ArgumentNullException("evaluatedAggregator");
                result = seed;
                int position = _position;
                char c;
                if (!EndOfPattern && TryIterateNext(position, out c, out position))
                    return TryAggregateFollowingWhile(seed, position, evaluatedAggregator, out result);
                result = seed;
                return false;
            }
            finally { Monitor.Exit(_syncRoot); }
        }

        public bool TryAggregateNextWhile<T>(T seed, TryFunc<T, char, bool, int, T> evaluatedAggregator, out T result)
        {
            Monitor.Enter(_syncRoot);
            try
            {
                if (_position < -1) throw new ObjectDisposedException(GetType().FullName);
                if (evaluatedAggregator == null) throw new ArgumentNullException("evaluatedAggregator");
                result = seed;
                int position = _position;
                char c;
                if (!EndOfPattern && TryIterateNext(position, out c, out position))
                    return TryAggregateFollowingWhile(seed, position, evaluatedAggregator, out result);
                result = seed;
                return false;
            }
            finally { Monitor.Exit(_syncRoot); }
        }

        public bool TryAggregateNextWhile<T>(TryFunc<char, T> firstEvaluatedAggregator, TryFunc<T, char, T> followingEvaluatedAggregator, out T result)
        {
            Monitor.Enter(_syncRoot);
            try
            {
                if (_position < -1) throw new ObjectDisposedException(GetType().FullName);
                if (firstEvaluatedAggregator == null) throw new ArgumentNullException("firstEvaluatedAggregator");
                if (followingEvaluatedAggregator == null) throw new ArgumentNullException("followingEvaluatedAggregator");
                int position = _position;
                char c;
                if (!EndOfPattern && TryIterateNext(position, out c, out position) && TryIterateNext(position, out c, out position))
                {
                    if (firstEvaluatedAggregator(c, out result))
                        return TryAggregateFollowingWhile(result, position, followingEvaluatedAggregator, out result);
                }
                else
                    result = default;
                return false;
            }
            finally { Monitor.Exit(_syncRoot); }
        }

        public bool TryAggregateNextWhile<T>(TryFunc<char, bool, T> firstEvaluatedAggregator, TryFunc<T, char, bool, T> followingEvaluatedAggregator, out T result)
        {
            Monitor.Enter(_syncRoot);
            try
            {
                if (_position < -1) throw new ObjectDisposedException(GetType().FullName);
                if (firstEvaluatedAggregator == null) throw new ArgumentNullException("firstEvaluatedAggregator");
                if (followingEvaluatedAggregator == null) throw new ArgumentNullException("followingEvaluatedAggregator");
                int position = _position;
                char c;
                bool isEscaped;
                if (!EndOfPattern && TryIterateNext(position, out c, out position) && TryIterateNext(position, out c, out isEscaped, out position))
                {
                    if (firstEvaluatedAggregator(c, isEscaped, out result))
                        return TryAggregateFollowingWhile(result, position, followingEvaluatedAggregator, out result);
                }
                else
                    result = default;
                return false;
            }
            finally { Monitor.Exit(_syncRoot); }
        }

        public bool TryAggregateNextWhile<T>(TryFunc<char, T> firstEvaluatedAggregator, TryFunc<T, char, int, T> followingEvaluatedAggregator, out T result)
        {
            Monitor.Enter(_syncRoot);
            try
            {
                if (_position < -1) throw new ObjectDisposedException(GetType().FullName);
                if (firstEvaluatedAggregator == null) throw new ArgumentNullException("firstEvaluatedAggregator");
                if (followingEvaluatedAggregator == null) throw new ArgumentNullException("followingEvaluatedAggregator");
                int position = _position;
                char c;
                if (!EndOfPattern && TryIterateNext(position, out c, out position) && TryIterateNext(position, out c, out position))
                {
                    if (firstEvaluatedAggregator(c, out result))
                        return TryAggregateFollowingWhile(result, position, followingEvaluatedAggregator, out result);
                }
                else
                    result = default;
                return false;
            }
            finally { Monitor.Exit(_syncRoot); }
        }

        public bool TryAggregateNextWhile<T>(TryFunc<char, bool, T> firstEvaluatedAggregator, TryFunc<T, char, bool, int, T> followingEvaluatedAggregator, out T result)
        {
            Monitor.Enter(_syncRoot);
            try
            {
                if (_position < -1) throw new ObjectDisposedException(GetType().FullName);
                if (firstEvaluatedAggregator == null) throw new ArgumentNullException("firstEvaluatedAggregator");
                if (followingEvaluatedAggregator == null) throw new ArgumentNullException("followingEvaluatedAggregator");
                int position = _position;
                char c;
                bool isEscaped;
                if (!EndOfPattern && TryIterateNext(position, out c, out position) && TryIterateNext(position, out c, out isEscaped, out position))
                {
                    if (firstEvaluatedAggregator(c, isEscaped, out result))
                        return TryAggregateFollowingWhile(result, position, followingEvaluatedAggregator, out result);
                }
                else
                    result = default;
                return false;
            }
            finally { Monitor.Exit(_syncRoot); }
        }

        public bool MoveNextIf(int count, Func<char, bool, bool> evaluator)
        {
            Monitor.Enter(_syncRoot);
            try
            {
                if (_position < -1) throw new ObjectDisposedException(GetType().FullName);
                if (evaluator == null) throw new ArgumentNullException("evaluator");
                int position;
                char c;
                if (count == 0 || EndOfPattern || !TryIterateNext(_position, out c, out position))
                    return false;
                while (--count > 0)
                {
                    bool isEscaped;
                    if (!(TryIterateNext(position, out c, out isEscaped, out position) && evaluator(c, isEscaped)))
                        return false;
                }
                SetPosition(position);
                return true;
            }
            finally { Monitor.Exit(_syncRoot); }
        }

        public bool MoveNextIf(int count, Func<char, bool> evaluator)
        {
            Monitor.Enter(_syncRoot);
            try
            {
                if (_position < -1) throw new ObjectDisposedException(GetType().FullName);
                if (evaluator == null) throw new ArgumentNullException("evaluator");
                int position;
                char c;
                if (count == 0 || EndOfPattern || !TryIterateNext(_position, out c, out position))
                    return false;
                while (--count > 0)
                {
                    if (!(TryIterateNext(position, out c, out position) && evaluator(c)))
                        return false;
                }
                SetPosition(position);
                return true;
            }
            finally { Monitor.Exit(_syncRoot); }
        }

        public bool MoveNextIf(params Func<char, bool, bool>[] evaluator)
        {
            Monitor.Enter(_syncRoot);
            try
            {
                if (_position < -1) throw new ObjectDisposedException(GetType().FullName);
                if (evaluator == null) throw new ArgumentNullException("evaluator");
                if (evaluator.Length == 0) throw new ArgumentException("evaluator", "At least one aggregator function must be provided");
                int position;
                char c;
                if (EndOfPattern || !TryIterateNext(_position, out c, out position))
                    return false;
                foreach (Func<char, bool, bool> e in evaluator)
                {
                    if (e == null) throw new ArgumentNullException("evaluator");
                    bool isEscaped;
                    if (!(TryIterateNext(position, out c, out isEscaped, out position) && e(c, isEscaped)))
                        return false;
                }
                SetPosition(position);
                return true;
            }
            finally { Monitor.Exit(_syncRoot); }
        }

        public bool MoveNextIf(params Func<char, bool>[] evaluator)
        {
            Monitor.Enter(_syncRoot);
            try
            {
                if (_position < -1) throw new ObjectDisposedException(GetType().FullName);
                if (evaluator == null) throw new ArgumentNullException("evaluator");
                if (evaluator.Length == 0) throw new ArgumentException("evaluator", "At least one aggregator function must be provided");
                int position;
                char c;
                if (EndOfPattern || !TryIterateNext(_position, out c, out position))
                    return false;
                foreach (Func<char, bool> e in evaluator)
                {
                    if (e == null) throw new ArgumentNullException("evaluator");
                    if (!(TryIterateNext(position, out c, out position) && e(c)))
                        return false;
                }
                SetPosition(position);
                return true;
            }
            finally { Monitor.Exit(_syncRoot); }
        }

        public void AcceptChanges()
        {
            Monitor.Enter(_syncRoot);
            try
            {
                if (_position < -1) throw new ObjectDisposedException(GetType().FullName);
                _originalPosition = _position;
                _orignalValue = Current;
                _wasEscaped = IsEscaped;
                IsChanged = false;
            }
            finally { Monitor.Exit(_syncRoot); }
        }

        public void RejectChanges()
        {
            Monitor.Enter(_syncRoot);
            try
            {
                if (_position < -1) throw new ObjectDisposedException(GetType().FullName);
                _position = _originalPosition;
                Current = _orignalValue;
                IsEscaped = _wasEscaped;
                IsChanged = false;
            }
            finally { Monitor.Exit(_syncRoot); }
        }

        public void Dispose()
        {
            Monitor.Enter(_syncRoot);
            try { _position = -2; }
            finally { Monitor.Exit(_syncRoot); }
        }

        private void OnPositionChanged()
        {
            IsEscaped = (Current = _pattern[_position]) == '\\';
            if (IsEscaped)
            {
                IsEscaped = ++_position < _pattern.Length;
                if (IsEscaped)
                    Current = _pattern[_position];
                else
                    _position--;
            }
        }

        private void SetPosition(int position)
        {
            IsChanged = true;
            if ((_position = position) < _pattern.Length)
                OnPositionChanged();
            else
            {
                IsEscaped = false;
                EndOfPattern = true;
            }
        }

        public bool MoveNext()
        {
            Monitor.Enter(_syncRoot);
            try
            {
                if (_position < -1) throw new ObjectDisposedException(GetType().FullName);
                if (_position < _pattern.Length)
                {
                    IsChanged = true;
                    if (++_position < _pattern.Length)
                    {
                        OnPositionChanged();
                        return true;
                    }
                    IsEscaped = false;
                    EndOfPattern = true;
                }
            }
            finally { Monitor.Exit(_syncRoot); }
            return false;
        }

        public void Reset()
        {
            Monitor.Enter(_syncRoot);
            try
            {
                if (_position < -1) throw new ObjectDisposedException(GetType().FullName);
                IsEscaped = false;
                _position = -1;
            }
            finally { Monitor.Exit(_syncRoot); }
        }
    }
}
