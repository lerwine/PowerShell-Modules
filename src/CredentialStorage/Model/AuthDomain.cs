using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace CredentialStorage.Model
{
    public class AuthDomain : IModelItem
    {
        private Serialization.AuthDomain _serializable;
        private string _name;
        private string _title;
        private bool _deleted;
        private string _description;

        public Guid ID { get; private set; }
        
        public string Name
        {
            get { return _name; }
            set
            {
                string s = value ?? "";
                if (_name == s)
                    return;
                _name = s;
                _serializable.name = s;
            }
        }
        
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

        public AuthDomain(Serialization.AuthDomain serializable)
        {
            MetaData = new Hashtable();
            _serializable = serializable ?? new Serialization.AuthDomain();
            Guid id;
            if (String.IsNullOrEmpty(_serializable.id) || !Guid.TryParse(_serializable.id, out id))
            {
                id = Guid.NewGuid();
                _serializable.id = id.ToString("N");
            }
            ID = id;
            _name = _serializable.name ?? "";
            _title = _serializable.title ?? "";
            _description = _serializable.description ?? "";
            bool b;
            if (String.IsNullOrEmpty(_serializable.deleted))
                b = false;
            else if (!Boolean.TryParse(_serializable.deleted, out b))
            {
                int i;
                if (!Int32.TryParse(_serializable.deleted, out i))
                    b = false;
                else
                    b = i != 0;
            }

            this.Deleted = b;
        }
        
        public AuthDomain() : this(null) { }
    }
}