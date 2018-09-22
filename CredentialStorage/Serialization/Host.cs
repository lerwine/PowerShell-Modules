using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CredentialStorage.Serialization
{
    /// <summary>
    /// Represents a host name under a <seealso cref="Realm" />.
    /// </summary>
    [DataContract(Name = "host")]
    public class Host : ISerializableItem
    {
        /// <summary>
        /// Unique identifier for the current item.
        /// </summary>
        /// <returns>String representing a unique identifier GUID.</returns>
        [DataMember]
        public string id { get; set; }
        
        /// <summary>
        /// Name of host.
        /// </summary>
        /// <returns>String containing name of host. This can be null or empty if the host name is the same as the DNS domain name.</returns>
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
        /// Sub-level host names in domain.
        /// </summary>
        [DataMember]
        public Host[] hosts { get; set; }
        
        /// <summary>
        /// Endpoints for current host.
        /// </summary>
        [DataMember]
        public EndPointRoot[] endPoints { get; set; }
    }
}