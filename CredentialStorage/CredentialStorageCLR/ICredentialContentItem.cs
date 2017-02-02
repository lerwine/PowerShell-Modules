using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Xml.Serialization;

namespace CredentialStorageCLR
{
    /// <summary>
    /// 
    /// </summary>
    public interface ICredentialContentItem : ICloneable
    {
        /// <summary>
        /// 
        /// </summary>
        string DisplayText { get; set; }
        /// <summary>
        /// 
        /// </summary>
        Guid Id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        bool IsFolder { get; }
        /// <summary>
        /// 
        /// </summary>

        new ICredentialContentItem Clone();
    }
}