using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;

namespace XmlUtilityCLR
{
    public class LoadableXmlSchemaResource : LoadableXmlResource<XmlSchema>
    {
        public LoadableXmlSchemaResource(Uri uri) : base(uri) { }

        protected override XmlSchema Read(XmlReader xmlReader) { return XmlSchema.Read(xmlReader, Xml_ValidationEventHandler); }
    }
}
