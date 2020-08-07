using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CredentialStorage.Serialization
{
    /// <summary>
    /// Represents an endpoint within a <seealso cref="Host" />.
    /// </summary>
    [DataContract(Name = "segment")]
    public class EndPointSegment
    {
        /// <summary>
        /// Unique identifier for the authentication domain.
        /// </summary>
        /// <returns>String representing a unique identifier GUID of an <seealso cref="AuthDomain" />.</returns>
        [DataMember]
        public string auth { get; set; }
        
        /// <summary>
        /// Unique identifier for the default credential.
        /// </summary>
        /// <returns>String representing a unique identifier GUID of a <seealso cref="Credential" /> within the referenced <see cref="auth" />.</returns>
        [DataMember]
        public string credential { get; set; }
        
        /// <summary>
        /// Path segment name.
        /// </summary>
        /// <returns>String containing the name of a path segment. This can be null or empty if it is the root path.</returns>
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
        /// Relative paths under the current endpoint root.
        /// </summary>
        [DataMember]
        public EndPointSegment[] path { get; set; }
    }
}