using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CredentialStorage.Model
{
    public class EndPointSegment : IBaseItem
    {
        public Credential Credential { get; private set; }
        
        public string Name { get; private set; }
        
        public string Title { get; set; }
        
        public bool Deleted { get; private set; }
        
        public string Description { get; set; }

        public Hashtable MetaData { get; set; }

        public EndPointSegment ParentSegment { get; private set; }

        public EndPointRoot Root { get; private set; }

        public EndPointSegment()
        {
            MetaData = new Hashtable();
        }
    }
}