using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CredentialStorage.Serialization
{
    /// <summary>
    /// A logically separated group of endpoints and credentials.
    /// </summary>
    [DataContract(Name = "realm")]
    public class Realm : ISerializableItem
    {
        /// <summary>
        /// Unique identifier for the current item.
        /// </summary>
        /// <returns>String representing a unique identifier GUID.</returns>
        [DataMember]
        public string id { get; set; }
        
        /// <summary>
        /// User-displayed title for current item.
        /// </summary>
        /// <returns>String containing title of current item.</returns>
        [DataMember]
        public string title { get; set; }
        
        /// <summary>
        /// Default display order
        /// </summary>
        /// <returns>A string containing a number value representing the default display order.</returns>
        [DataMember]
        public string order { get; set; }
        
        /// <summary>
        /// Indicates whether item is deleted.
        /// </summary>
        /// <returns>True if item is deleted; otherwise, false.</returns>
        [DataMember]
        public string deleted { get; set; }
        
        /// <summary>
        /// Verbose description of current item.
        /// </summary>
        /// <returns>String containing description of current item.</returns>
        [DataMember]
        public string description { get; set; }

        /// <summary>
        /// Meta data to be associated with current item.
        /// </summary>
        /// <returns>Meta data associated with curren item.</returns>
        [DataMember]
        public MetaData metaData { get; set; }

        /// <summary>
        /// Domains contained within the current realm.
        /// </summary>
        [DataMember]
        public Host[] hosts { get; set; }
        
        /// <summary>
        /// Authentication authorities contained within the current realm.
        /// </summary>
        [DataMember]
        public AuthDomain[] auth { get; set; }
    }
}