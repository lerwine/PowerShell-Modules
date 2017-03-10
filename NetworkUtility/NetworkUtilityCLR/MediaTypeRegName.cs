using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Mime;
using System.Runtime.InteropServices;
using System.Xml;
using System.Xml.Serialization;

namespace NetworkUtilityCLR
{
    [Serializable]
    [XmlRoot(MediaTypeRegRecord.ElementName_name, Namespace = MediaTypeRegistry.XmlNamespace_Registry, IsNullable = true)]
    public class MediaTypeRegName : MediaTypeRegRawXml
    {
        public override string ElementName { get { return MediaTypeRegRecord.ElementName_name; } }
		
        public string Value
        {
            get
            {
                if (Content.IsEmpty)
                    return null;
                string value;
                if (Content.ChildNodes.Count == 1)
                    value = Content.InnerText.Trim();
                else
                {
                    value = "";
                    foreach (XmlNode node in Content.ChildNodes)
                    {
                        if (node is XmlText || node is XmlCDataSection)
                        {
                            value = (node as XmlCharacterData).InnerText.Trim();
                            break;
                        }
                    }
                }

                int index = 0;
                while (index < value.Length && !Char.IsWhiteSpace(value[index]))
                    index++;
                return (index < value.Length) ? value.Substring(0, index) : value;
            }
        }
    }
}
