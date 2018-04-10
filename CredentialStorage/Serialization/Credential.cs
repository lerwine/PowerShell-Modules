using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CredentialStorage.Serialization
{
    /// <summary>
    /// Represents authorization credential information associated with a <seealso cref="AuthDomain" />.
    /// </summary>
    [DataContract(Name = "credential")]
    public class Credential : ISerializableItem
    {
        /// <summary>
        /// String representing a unique identifier GUID.
        /// </summary>
        /// <returns>A string formatted as a GUID value.</returns>
        [DataMember]
        public string id { get; set; }
        
        /// <summary>
        /// User-displayed title for current item.
        /// </summary>
        /// <returns>String containing title of current item.</returns>
        [DataMember]
        public string title { get; set; }
        
        /// <summary>
        /// User name / login.
        /// </summary>
        [DataMember]
        public string userName { get; set; }
        
        /// <summary>
        /// Encrypted password.
        /// </summary>
        [DataMember]
        public string password { get; set; }
        
        /// <summary>
        /// Encrypted PIN.
        /// </summary>
        [DataMember]
        public string pin { get; set; }
        
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
    }
}