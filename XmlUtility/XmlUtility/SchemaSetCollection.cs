using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml.Schema;

namespace IOUtilityCLR
{
    public class SchemaSetCollection : Collection<XmlSchemaSet>
    {
        private object _syncRoot = new object();
        private ValidationEventHandler _validationEventHandler;

        private static bool _Any(IEnumerable<XmlSchemaSet> collection, Predicate<XmlSchemaSet> predicate)
        {
            foreach (XmlSchemaSet item in collection)
            {
                if (predicate == null || predicate(item))
                    return true;
            }

            return false;
        }

        public static bool Any(IEnumerable<XmlSchemaSet> collection)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");

            return SchemaSetCollection._Any(collection, null);
        }
        
        public static bool Any(IEnumerable<XmlSchemaSet> collection, Predicate<XmlSchemaSet> predicate)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");

            if (predicate == null)
                throw new ArgumentNullException("predicate");

            return SchemaSetCollection._Any(collection, null);
        }

        private static IEnumerable<XmlSchemaSet> _SkipWhile(IEnumerable<XmlSchemaSet> collection, Predicate<XmlSchemaSet> predicate)
        {
            IEnumerator<XmlSchemaSet> enumerator = collection.GetEnumerator();
            try
            {
                return SchemaSetCollection._SkipWhile(enumerator, predicate);
            }
            catch
            {
                throw;
            }
            finally
            {
                enumerator.Dispose();
            }
        }

        public static IEnumerable<XmlSchemaSet> SkipWhile(IEnumerable<XmlSchemaSet> collection)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");

            return SchemaSetCollection._SkipWhile(collection, null);
        }

        public static IEnumerable<XmlSchemaSet> SkipWhile(IEnumerable<XmlSchemaSet> collection, Predicate<XmlSchemaSet> predicate)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");

            if (predicate == null)
                throw new ArgumentNullException("predicate");

            return SchemaSetCollection._SkipWhile(collection, predicate);
        }

        private static IEnumerable<XmlSchemaSet> _SkipWhile(IEnumerator<XmlSchemaSet> enumerator, Predicate<XmlSchemaSet> predicate)
        {
            if (enumerator == null)
                throw new ArgumentNullException("enumerator");

            if (predicate == null)
                throw new ArgumentNullException("predicate");

            bool matched = false;
            while (enumerator.MoveNext())
            {
                if (!predicate(enumerator.Current))
                {
                    matched = true;
                    yield return enumerator.Current;
                    break;
                }
            }
            if (matched)
            {
                while (enumerator.MoveNext())
                    yield return enumerator.Current;
            }
        }

        public static IEnumerable<XmlSchemaSet> SkipWhile(IEnumerator<XmlSchemaSet> enumerator, Predicate<XmlSchemaSet> predicate)
        {
            if (enumerator == null)
                throw new ArgumentNullException("enumerator");

            if (predicate == null)
                throw new ArgumentNullException("predicate");

            bool matched = false;
            while (enumerator.MoveNext())
            {
                if (!predicate(enumerator.Current))
                {
                    matched = true;
                    yield return enumerator.Current;
                    break;
                }
            }
            if (matched)
            {
                while (enumerator.MoveNext())
                    yield return enumerator.Current;
            }
        }

        public static int ItemCount(IEnumerable<XmlSchemaSet> collection, Predicate<XmlSchemaSet> predicate)
        {
            IEnumerator<XmlSchemaSet> enumerator = collection.GetEnumerator();
            try
            {
                return SchemaSetCollection.ItemCount(enumerator, predicate);
            }
            catch
            {
                throw;
            }
            finally
            {
                enumerator.Dispose();
            }
        }

        public static int ItemCount(IEnumerator<XmlSchemaSet> enumerator, Predicate<XmlSchemaSet> predicate)
        {
            if (enumerator == null)
                throw new ArgumentNullException("enumerator");

            int count = 0;
            if (predicate == null)
            {
                while (enumerator.MoveNext())
                    count++;
            }
            else
            {
                while (enumerator.MoveNext())
                {
                    if (predicate(enumerator.Current))
                        count++;
                }
            }

            return count;
        }

        public SchemaSetCollection(ValidationEventHandler validationEventHandler) : this(validationEventHandler, new XmlSchemaSet[0]) { }

        public SchemaSetCollection(ValidationEventHandler validationEventHandler, IList<XmlSchemaSet> list)
        {
            this._validationEventHandler = validationEventHandler;

            if (list == null)
                return;

            foreach (XmlSchemaSet item in list)
            {
                if (list == null)
                    continue;
                
                bool refEquals = false;
                foreach (XmlSchemaSet ss in this)
                {
                    if (Object.ReferenceEquals(ss, item))
                    {
                        refEquals = true;
                        break;
                    }
                }
                
                if (refEquals)
                    continue;

                base.InsertItem(this.Count, item);
                if (validationEventHandler != null)
                    item.ValidationEventHandler += this._validationEventHandler;
            }
        }
        
        protected override void InsertItem(int index, XmlSchemaSet item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            if (index < 0)
                throw new ArgumentOutOfRangeException("index", "Index cannot be less than zero.");

            lock (this._syncRoot)
            {
                if (index > this.Count)
                    throw new ArgumentOutOfRangeException("index", "Index cannot be greater than Count.");

                int oldIndex = 0;
                foreach (XmlSchemaSet ss in this)
                {
                    if (Object.ReferenceEquals(ss, item))
                        break;
                    oldIndex++;
                }
                if (oldIndex < this.Count)
                {
                    base.RemoveItem(oldIndex);
                    if (oldIndex < index)
                        index--;
                } else if (this._validationEventHandler != null)
                    item.ValidationEventHandler += this._validationEventHandler;

                base.InsertItem(index, item);
            }
        }

        protected override void RemoveItem(int index)
        {
            lock (this._syncRoot)
            {
                XmlSchemaSet removedItem = this[index];
                base.RemoveItem(index);
                if (this._validationEventHandler != null)
                    removedItem.ValidationEventHandler -= this._validationEventHandler;
            }
        }

        protected override void SetItem(int index, XmlSchemaSet item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            if (index < 0)
                throw new ArgumentOutOfRangeException("index", "Index cannot be less than zero.");

            lock (this._syncRoot)
            {
                if (index > this.Count)
                    throw new ArgumentOutOfRangeException("index", "Index cannot be greater than Count.");

                if (Object.ReferenceEquals(item, this[index]))
                    return;

                int oldIndex = 0;
                foreach (XmlSchemaSet ss in this)
                {
                    if (Object.ReferenceEquals(ss, item))
                        break;
                    oldIndex++;
                }
                if (oldIndex < this.Count)
                    base.SetItem(oldIndex, this[index]);
                else if (this._validationEventHandler != null)
                {
                    this[index].ValidationEventHandler -= this._validationEventHandler;
                    item.ValidationEventHandler += this._validationEventHandler;
                }
                base.SetItem(index, item);
            }
        }

        protected override void ClearItems()
        {
            lock (this._syncRoot)
            {
                foreach (XmlSchemaSet schemaSet in this)
                    schemaSet.ValidationEventHandler -= this._validationEventHandler;

                base.ClearItems();
            }
        }
    }
}