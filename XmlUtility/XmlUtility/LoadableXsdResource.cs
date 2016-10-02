using System;
using System.Collections.Generic;
#if !PSLEGACY
using System.Linq;
#endif
using System.Text;
#if !PSLEGACY
using System.Threading.Tasks;
#endif
using System.Xml;
using System.Xml.Schema;

namespace XmlUtilityCLR
{
    public class LoadableXsdResource : LoadableXmlResource<XmlSchema>
    {
        public LoadableXsdResource(Uri uri) : base(uri) { }

        protected override XmlSchema Read(XmlReader xmlReader) { return XmlSchema.Read(xmlReader, null); }
    }
}
