using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.Net.Mime;
using System.Runtime.InteropServices;
using System.Xml;
using System.Xml.Serialization;

namespace NetworkUtilityCLR
{
    [Serializable]
    [XmlRoot(MediaTypeRegistryDB.ElementName_people, Namespace = MediaTypeRegistry.XmlNamespace_Registry, IsNullable = true)]
    public class MediaTypeRegPeople : MediaTypeRawXml
    {
        public override string ElementName { get { return MediaTypeRegistryDB.ElementName_people; } }
    }
}
