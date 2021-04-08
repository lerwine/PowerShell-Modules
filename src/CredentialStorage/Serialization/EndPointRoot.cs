using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CredentialStorage.Serialization
{
    /// <summary>
    /// Represents an endpoint within a <seealso cref="Host" />.
    /// </summary>
    [DataContract(Name = "endPoint")]
    public class EndPointRoot
    {
        /// <summary>
        /// Network Port number.
        /// </summary>
        /// <returns>String representing a network port number.</returns>
        [DataMember]
        public string port { get; set; }
        
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
        /// Paths under the current endpoint root.
        /// </summary>
        [DataMember]
        public EndPointSegment[] path { get; set; }
    }
}