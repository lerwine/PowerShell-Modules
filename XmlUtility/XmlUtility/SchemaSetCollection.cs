using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Schema;

namespace XmlUtilityCLR
{
    public class SchemaSetCollection : Collection<XmlSchemaSet>
    {
        private object _syncRoot = new object();
        private ValidationEventHandler _validationEventHandler;
        
        public SchemaSetCollection(ValidationEventHandler validationEventHandler) : this(validationEventHandler, new XmlSchemaSet[0]) { }

        public SchemaSetCollection(ValidationEventHandler validationEventHandler, IList<XmlSchemaSet> list)
        {
            this._validationEventHandler = validationEventHandler;

            if (list == null)
                return;

            foreach (XmlSchemaSet item in list)
            {
                if (list == null || this.Any(i => Object.ReferenceEquals(i, item)))
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

                int oldIndex = this.SkipWhile(i => !Object.ReferenceEquals(i, item)).Count();
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

                int oldIndex = this.SkipWhile(i => !Object.ReferenceEquals(i, item)).Count();
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