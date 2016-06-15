using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;

namespace XmlUtilityCLR
{
    public class LoadableXsdResource : LoadableXmlResource<XmlSchema>
    {
        public LoadableXsdResource(Uri uri) : base(uri) { }

        protected override XmlSchema Read(XmlReader xmlReader) { return XmlSchema.Read(xmlReader); }
    }
}
