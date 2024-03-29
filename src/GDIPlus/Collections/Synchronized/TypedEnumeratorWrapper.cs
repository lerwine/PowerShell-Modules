using System;
using System.Collections;
using System.Collections.Generic;

namespace Erwine.Leonard.T.GDIPlus.Collections.Synchronized
{
    /// <summary>
    /// Wraps an <see cref="IEnumerator"/> as a strongly-typed enumerator.
    /// </summary>
    /// <typeparam name="T">Type of value expected to be returned from <see cref="IEnumerable"/> object.</typeparam>
    public class TypedEnumeratorWrapper<T> : IEnumerator<T>
    {
        private IEnumerator _innerEnumerator;

        /// <summary>
        /// Initializes a new instance of <see cref="TypedEnumeratorWrapper{T}"/> to enumerate results from
        /// an <see cref="IEnumerator"/> obtained from <paramref name="obj"/>.
        /// </summary>
        /// <param name="obj"> <see cref="IEnumerable"/> object to enumerate.</param>
        /// <exception cref="ArgumentNullException"><paramref name="obj"/> is null.</exception>
        public TypedEnumeratorWrapper(IEnumerable obj)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");

            this._innerEnumerator = obj.GetEnumerator();
        }

        /// <summary>
        /// Gets the element in the collection at the current position of the enumerator.
        /// </summary>
        /// <exception cref="InvalidOperationException">The enumeration has not started or the enumeration was finished.</exception>
        /// <exception cref="InvalidCastException">Enumerated item could not be cast to.</exception>
        /// <exception cref="NullReferenceException">Enumerated item was null and type <typeparamref name="T"/> is a value type.</exception>
        public T Current { get { return (T)(this._innerEnumerator.Current); } }

        /// <summary>
        /// Gets the current element in the collection.
        /// </summary>
        /// <exception cref="InvalidOperationException">The enumeration has not started or the enumeration was finished.</exception>
        /// <exception cref="InvalidCastException">Enumerated item could not be cast to.</exception>
        /// <exception cref="NullReferenceException">Enumerated item was null and type <typeparamref name="T"/> is a value type.</exception>
        object IEnumerator.Current { get { return this._innerEnumerator.Current; } }

        /// <summary>
        /// Advances the enumerator to the next element of the collection.
        /// </summary>
        /// <returns>true if the enumerator was successfully advanced to the next element;
        /// false if the enumerator has passed the end of the collection.</returns>
        /// <exception cref="InvalidOperationException">The collection was modified after the enumerator was created.</exception>
        public bool MoveNext() { return this._innerEnumerator.MoveNext(); }

        /// <summary>
        /// Advances the enumerator to the next element of the collection.
        /// </summary>
        /// <exception cref="InvalidOperationException">The collection was modified after the enumerator was created.</exception>
        public void Reset() { this._innerEnumerator.Reset(); }

        /// <summary>
        /// Disposes the internal <see cref="IEnumerator"/> if applicable.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the internal <see cref="IEnumerator"/> if applicable.
        /// </summary>
        /// <param name="disposing">true if method has been called direclty or indirectly by user code;
        /// otherwise false to indicate that this has been called by the runtime inside the finalizer.</param>
        /// <remarks>If <paramref name="disposing"/> is false, then you should not reference any other objects.</remarks>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing && this._innerEnumerator is IDisposable)
                (this._innerEnumerator as IDisposable).Dispose();
        }
    }
}
