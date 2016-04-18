using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Xml.Serialization;

namespace CredentialStorageCLR
{
    public interface ICredentialContentItem : ICloneable
    {
        string DisplayText { get; set; }
        Guid Id { get; set; }
        bool IsFolder { get; }

        new ICredentialContentItem Clone();
    }
}