using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace LteDev.RegexParsing
{
    class RegexPatternTokenObserable : IObservable<IRegexPatternToken>
    {
        private readonly object _syncRoot = new object();
        private Registration? _first;
        private Registration? _last;

        protected void RaiseCompleted()
        {
            Monitor.Enter(_syncRoot);
            try
            {
                if (_first is not null)
                    Registration.RaiseCompleted(_first);
            }
            finally { Monitor.Exit(_syncRoot); }
        }

        protected void PushError(Exception error)
        {
            Monitor.Enter(_syncRoot);
            try
            {
                if (_first is not null)
                    Registration.PushError(error, _first);
            }
            finally { Monitor.Exit(_syncRoot); }
        }

        protected void PushNext(IRegexPatternToken value)
        {
            Monitor.Enter(_syncRoot);
            try
            {
                if (_first is not null)
                    Registration.PushNext(value, _first);
            }
            finally { Monitor.Exit(_syncRoot); }
        }

        public IDisposable Subscribe(IObserver<IRegexPatternToken> observer)
        {
            ArgumentNullException.ThrowIfNull(observer);
            return new Registration(this, observer);
        }

        class Registration : IDisposable
        {
            private readonly RegexPatternTokenObserable _owner;
            private readonly WeakReference<IObserver<IRegexPatternToken>> _target;
            private Registration? _previous;
            private Registration? _next;

            internal bool TryGetTarget([NotNullWhen(true)] out IObserver<IRegexPatternToken>? target)
            {
                if (_target.TryGetTarget(out target))
                    return true;
                Dispose();
                return false;
            }

            internal static void RaiseCompleted(Registration first)
            {
                ArgumentNullException.ThrowIfNull(first);
                Registration? next = first._next;
                try
                {
                    IObserver<IRegexPatternToken>? target;
                    if (first.TryGetTarget(out target))
                        target.OnCompleted();
                }
                finally
                {
                    if (next is not null)
                        RaiseCompleted(next);
                }
            }

            internal static void PushError(Exception error, Registration first)
            {
                Registration? next = first._next;
                try
                {
                    IObserver<IRegexPatternToken>? target;
                    if (first.TryGetTarget(out target))
                        target.OnError(error);
                }
                finally
                {
                    if (next is not null)
                        PushError(error, next);
                }
            }

            internal static void PushNext(IRegexPatternToken value, Registration first)
            {
                Registration? next = first._next;
                try
                {
                    IObserver<IRegexPatternToken> target;
                    if (first.TryGetTarget(out target))
                        target.OnNext(value);
                }
                finally
                {
                    if (next is not null)
                        PushNext(value, next);
                }
            }

            internal Registration(RegexPatternTokenObserable owner, IObserver<IRegexPatternToken> observer)
            {
                _target = new WeakReference<IObserver<IRegexPatternToken>>(observer);
                Monitor.Enter(owner._syncRoot);
                try
                {
                    if ((_previous = (_owner = owner)._last) is not null)
                        _previous._next = this;
                    else
                        owner._first = this;
                    owner._last = this;
                }
                finally { Monitor.Exit(owner._syncRoot); }
            }

            public void Dispose()
            {
                Monitor.Enter(_owner._syncRoot);
                try
                {
                    if (_next is null)
                    {
                        if (_owner._last is not null && ReferenceEquals(_next, _owner._last) && (_owner._last = _previous) is not null)
                            _previous = _previous._next = null;
                        else
                            _owner._first = null;
                    }
                    else
                    {
                        if ((_next._previous = _previous) is null)
                            _owner._first = _next;
                        else
                        {
                            _previous._next = _next;
                            _previous = null;
                        }
                        _next = null;
                    }
                }
                finally { Monitor.Exit(_owner._syncRoot); }
            }
        }
    }
}
