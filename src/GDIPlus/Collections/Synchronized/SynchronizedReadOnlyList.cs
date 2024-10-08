using System.Collections;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Erwine.Leonard.T.GDIPlus.Collections.Synchronized
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    /// <summary>
    /// Represents a syncrhonized (thread-safe), read-only list of objects that can be accessed by index, and whose elements are strongly typed.
    /// </summary>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    public class SynchronizedReadOnlyList<T> : SynchronizedEnumerableBase<T>, IList<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SynchronizedReadOnlyList{T}"/> class with a 
        /// specified number of auto-created elements.
        /// </summary>
        /// <param name="size">Size of the read-only collection.</param>
        /// <exception cref="System.OverflowException"><paramref name="size"/> is less than zero.</exception>
        /// <exception cref="System.MissingMethodException">The type that is specified for <typeparamref name="T"/> does not have a parameterless constructor.</exception>
        public SynchronizedReadOnlyList(int size) : base(new T[size]) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SynchronizedReadOnlyList{T}"/> class that contains elements copied from the 
        /// specified array.
        /// </summary>
        /// <param name="array">The array whose elements are copied to the new list.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="array"/> is null.</exception>
        public SynchronizedReadOnlyList(params T[] array) : base(new List<T>(array)) { }

        /// <summary>
        /// Called from within the base constructor to initialize the inner synchronized list.
        /// </summary>
        /// <param name="collection">The collection whose elements are copied to the new list.</param>
        protected override void Initialize(ICollection<T> collection)
        {
            T[] array;

            if (collection is T[]) // An array indicates that the constructor with an integer argument was called
            {
                array = collection as T[];
                for (int index = 0; index < array.Length; index++)
                    array[index] = CreateNewItem(index);
            }
            else
                array = collection.ToArray();

            base.Initialize(array);
        }

        /// <summary>
        /// Gets called during initialization when an integer value was passed to the constructor, in order to populate an element.
        /// </summary>
        /// <param name="index">Index at which the item will be placed.</param>
        /// <returns>A reference to the newly created object.</returns>
        /// <exception cref="System.MissingMethodException">The type that is specified for <typeparamref name="T"/> does not have a parameterless constructor.</exception>
        protected virtual T CreateNewItem(int index) => Activator.CreateInstance<T>();

        #region Explicit Interface Members

        IEnumerator IEnumerable.GetEnumerator() => base.InnerGetEnumerator();

        T IList<T>.this[int index]
        {
            get => base[index];
            set => throw new NotSupportedException();
        }

        void IList<T>.Insert(int index, T item) => throw new NotSupportedException();

        void IList<T>.RemoveAt(int index) => throw new NotSupportedException();

        bool ICollection<T>.IsReadOnly => true;

        void ICollection<T>.Add(T item) => throw new NotSupportedException();

        void ICollection<T>.Clear() => throw new NotSupportedException();

        bool ICollection<T>.Remove(T item) => throw new NotSupportedException();

        #endregion

        #region Overrides to prevent list from being modified
        protected override void BaseClear() => throw new NotSupportedException();

        protected override bool BaseRemove(T item) => throw new NotSupportedException();

        protected override void BaseRemoveAt(int index) => throw new NotSupportedException();

        protected override int InnerAdd(object value) => throw new NotSupportedException();

        protected override void InnerInsert(int index, object value) => throw new NotSupportedException();

        protected override bool InnerRemove(object value) => throw new NotSupportedException();

        protected override void InnerSet(int index, object value) => throw new NotSupportedException();

        #endregion
    }
}
