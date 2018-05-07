using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CredentialStorage.Serialization
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public interface ISerializableItem
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {
        /// <summary>
        /// String representing a unique identifier GUID
        /// </summary>
        /// <returns>A string formatted as a GUID value.</returns>
        string id { get; set; }

        /// <summary>
        /// User-displayed title for current item.
        /// </summary>
        /// <returns>String containing title of current item.</returns>
        string title { get; set; }
        
        /// <summary>
        /// Indicates whether item is deleted.
        /// </summary>
        /// <returns>True if item is deleted; otherwise, false.</returns>
        string deleted { get; set; }

        /// <summary>
        /// Verbose description of current item.
        /// </summary>
        /// <returns>String containing description of current item.</returns>
        string description { get; set; }

        /// <summary>
        /// Meta data to be associated with current item.
        /// </summary>
        /// <returns>Meta data associated with curren item.</returns>
        MetaData metaData { get; set; }
    }
}