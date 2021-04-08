using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace CredentialStorage.Model
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class Host : IModelItem
    {
        private Serialization.Host _serializable;
        private string _title;
        private string _name;
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

        public Host()
        {
            MetaData = new Hashtable();
        }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}