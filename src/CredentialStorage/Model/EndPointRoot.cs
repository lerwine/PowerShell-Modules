using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CredentialStorage.Model
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class EndPointRoot : IBaseItem
    {
        private Serialization.EndPointRoot _serializable;
        private int? _port;
        private string _title;
        private bool _deleted;
        private string _description;

        public int? Port
        {
            get { return _port; }
            set
            {
                if (value.HasValue)
                {
                    int i = (value.Value < 0) ? 0 : value.Value;
                    if (_port.HasValue && _port.Value == i)
                        return;
                    _serializable.port = i.ToString();
                    _port = i;
                    return;
                }
                
                if (!_port.HasValue)
                    return;
                _serializable.port = null;
                _port = null;
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

        public EndPointRoot()
        {
            MetaData = new Hashtable();
        }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}