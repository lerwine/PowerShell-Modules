using System.Collections;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Erwine.Leonard.T.GDIPlus.Collections.Synchronized
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    /// <summary>
    /// Represents a syncrhonized (thread-safe), strongly typed list of objects that can be accessed by index.
    /// </summary>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    public class SynchronizedList<T> : SynchronizedEnumerableBase<T>, IList<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SynchronizedList{T}"/> class that is empty and has the 
        /// default initial capacity.
        /// </summary>
        public SynchronizedList() : base() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SynchronizedList{T}"/> class that contains elements copied from the 
        /// specified list and has sufficient capacity to accommodate the number of elements copied.
        /// </summary>
        /// <param name="list">The list whose elements are copied to the new list.</param>
        /// <exception cref="ArgumentNullException"><paramref name="list"/> is null.</exception>
        public SynchronizedList(IList<T> list) : base(list) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SynchronizedEnumerableBase{T}"/> class that contains elements copied from the 
        /// specified collection and has sufficient capacity to accommodate the number of elements copied.
        /// </summary>
        /// <param name="collection">The collection whose elements are copied to the new list.</param>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is null.</exception>
        public SynchronizedList(ICollection<T> collection) : base(collection) { }

        #region IList<T> Members

        /// <summary>
        /// Inserts an item to the <see cref="SynchronizedList{T}"/> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param>
        /// <param name="item">The object to insert into the <see cref="SynchronizedList{T}"/>.</param>
        /// <exception cref="System.ArgumentOutOfRangeException"><paramref name="index"/> is not a valid index in
        /// the <see cref="SynchronizedList{T}"/>.</exception>
        /// <exception cref="NotSupportedException">The <see cref="SynchronizedList{T}"/> is read-only
        /// <para>-or-</para>
        /// <para>The <see cref="SynchronizedList{T}"/> has a fixed size.</para></exception>
        /// <exception cref="NullReferenceException"><paramref name="item"/> is null reference in 
        /// the <see cref="SynchronizedList{T}"/>.</exception>
        public void Insert(int index, T item) => base.BaseInsert(index, item);

        /// <summary>
        /// Removes the S<see cref="SynchronizedList{T}"/> item at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is not a valid index in
        /// the <see cref="SynchronizedList{T}"/>.</exception>
        /// <exception cref="NotSupportedException">The <see cref="SynchronizedList{T}"/> is read-only.</exception>
        public void RemoveAt(int index) => base.BaseRemoveAt(index);

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        /// <returns>The element at the specified index.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is not a valid index in
        /// the <see cref="SynchronizedList{T}"/>.</exception>
        /// <exception cref="NotSupportedException">The property is set and the <see cref="SynchronizedList{T}"/>
        /// is read-only.</exception>
        public new T this[int index]
        {
            get => base[index];
            set => base.BaseSet(index, value);
        }

        #endregion

        #region ICollection<T> Members

        bool ICollection<T>.IsReadOnly => false;

        /// <summary>
        /// Adds an item to the <see cref="SynchronizedList{T}"/>.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="SynchronizedList{T}"/>.</param>
        /// <exception cref="System.NotSupportedException">The <see cref="SynchronizedList{T}"/> is read-only
        /// <para>-or-</para>
        /// <para>The <see cref="SynchronizedList{T}"/> has a fixed size.</para></exception>
        public void Add(T item) => base.BaseAdd(item);

        /// <summary>
        /// Removes all items from the <see cref="SynchronizedList{T}"/>.
        /// </summary>
        /// <exception cref="System.NotSupportedException">The <see cref="SynchronizedList{T}"/> is read-only.
        /// <para>-or-</para>
        /// <para>The <see cref="SynchronizedList{T}"/> has a fixed size.</para></exception>
        public void Clear() => base.BaseClear();

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="SynchronizedList{T}"/>.
        /// </summary>
        /// <param name="item">The object to remove from the <see cref="SynchronizedList{T}"/>.</param>
        /// <returns>true if item was successfully removed from the <see cref="SynchronizedList{T}"/>;
        /// otherwise, false. This method also returns false if item is not found in the 
        /// original <see cref="SynchronizedList{T}"/>.</returns>
        /// <exception cref="System.NotSupportedException">The <see cref="SynchronizedList{T}"/> is read-only
        /// <para>-or-</para>
        /// <para>The <see cref="SynchronizedList{T}"/> has a fixed size.</para></exception>
        public bool Remove(T item) => base.BaseRemove(item);

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator() => base.InnerGetEnumerator();

        #endregion
    }
}
