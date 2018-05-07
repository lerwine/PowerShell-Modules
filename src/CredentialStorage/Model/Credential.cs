using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CredentialStorage.Model
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class Credential : IModelItem
    {
        private Serialization.Credential _serializable;
        private string _title;
        private string _userName;
        private string _password;
        private string _pin;
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
        
        public string UserName
        {
            get { return _userName; }
            set
            {
                string s = value ?? "";
                if (_userName == s)
                    return;
                _userName = s;
                _serializable.userName = s;
            }
        }
        
        public string Password
        {
            get { return _password; }
            set
            {
                string s = value ?? "";
                if (_password == s)
                    return;
                _password = s;
                _serializable.password = s;
            }
        }
        
        public string PIN
        {
            get { return _pin; }
            set
            {
                string s = value ?? "";
                if (_pin == s)
                    return;
                _pin = s;
                _serializable.pin = s;
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

        public AuthDomain Domain { get; private set; }
        
        public Credential(Serialization.Credential serializable)
        {
            MetaData = new Hashtable();
            _serializable = serializable ?? new Serialization.Credential();
            Guid id;
            if (String.IsNullOrEmpty(_serializable.id) || !Guid.TryParse(_serializable.id, out id))
            {
                id = Guid.NewGuid();
                _serializable.id = id.ToString("N");
            }
            ID = id;
            _title = _serializable.title ?? "";
            _userName = _serializable.userName ?? "";
            _password = _serializable.password ?? "";
            _pin = _serializable.pin ?? "";
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
        
        public Credential() : this(null) { }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}