using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Xml.Serialization;

namespace CredentialStorageCLR
{
    /// <summary>
    /// Content item
    /// </summary>
    public interface ICredentialContentItem : ICloneable
    {
        /// <summary>
        /// Display text
        /// </summary>
        string DisplayText { get; set; }

        /// <summary>
        /// Unique identifier
        /// </summary>
        Guid Id { get; set; }

        /// <summary>
        /// Indicates whether node is a folder.
        /// </summary>
        bool IsFolder { get; }

        /// <summary>
        /// Create clone of current node.
        /// </summary>
        /// <returns>Clone of current node.</returns>
        new ICredentialContentItem Clone();
    }
}