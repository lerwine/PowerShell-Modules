using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CredentialStorage.Serialization
{
    /// <summary>
    /// Represents an authentication authority within a <seealso cref="Domain" />.
    /// </summary>
    [DataContract(Name = "domain")]
    public class AuthDomain : ISerializableItem
    {
        /// <summary>
        /// Unique identifier for the current item.
        /// </summary>
        /// <returns>String representing a unique identifier GUID.</returns>
        [DataMember]
        public string id { get; set; }
        
        /// <summary>
        /// Name of domain.
        /// </summary>
        [DataMember]
        public string name { get; set; }
        
        /// <summary>
        /// User-displayed title for current item.
        /// </summary>
        /// <returns>String containing title of current item.</returns>
        [DataMember]
        public string title { get; set; }
        
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
        /// Credentials contained within the current realm.
        /// </summary>
        [DataMember]
        public Credential[] credentials { get; set; }
    }
}