using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace CredentialStorage.Model
{
    public class Realm : IModelItem
    {
        private Serialization.Realm _serializable;
        private string _title;
        private int _order;
        private bool _deleted;
        private string _description;

        public Guid ID { get; private set; }
        
        public string Title
        {
            get { return _title; }
            set
            {
                string s = value ?? "";
                if (_title == s)
                    return;
                _title = s;
                _serializable.title = s;
            }
        }
        
        public int Order
        {
            get { return _order; }
            set
            {
                int i = (value < 0) ? 0 : value;
                if (_order == i)
                    return;
                _order = i;
                _serializable.order = i.ToString();
            }
        }
        
        public bool Deleted
        {
            get { return _deleted; }
            private set
            {
                if (_deleted == value)
                    return;
                _deleted = value;
                _serializable.deleted = value.ToString();
            }
        }
        
        public string Description
        {
            get { return _description; }
            set
            {
                string s = value ?? "";
                if (_description == s)
                    return;
                _description = s;
                _serializable.description = s;
            }
        }

        public Hashtable MetaData { get; private set; }

        public Realm(Serialization.Realm serializable)
        {
            MetaData = new Hashtable();
            _serializable = serializable ?? new Serialization.Realm();
            Guid id;
            if (String.IsNullOrEmpty(_serializable.id) || !Guid.TryParse(_serializable.id, out id))
            {
                id = Guid.NewGuid();
                _serializable.id = id.ToString("N");
            }
            ID = id;
            _title = _serializable.title ?? "";
            _description = _serializable.description ?? "";
            int i;
            if (String.IsNullOrEmpty(_serializable.order) || !Int32.TryParse(_serializable.order, out i))
                i = 0;
            this.Order = i;
            bool b;
            if (String.IsNullOrEmpty(_serializable.deleted))
                b = false;
            else if (!Boolean.TryParse(_serializable.deleted, out b))
            {
                if (!Int32.TryParse(_serializable.deleted, out i))
                    b = false;
                else
                    b = i != 0;
            }

            this.Deleted = b;
        }

        public Realm() : this(null) { }
    }
}