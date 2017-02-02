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
    /// <summary>
    /// 
    /// </summary>
    public class LoadableXsdResource : LoadableXmlResource<XmlSchema>
    {
        /// <summary>
        /// 
        /// </summary>
        public LoadableXsdResource(Uri uri) : base(uri) { }

        /// <summary>
        /// 
        /// </summary>
        protected override XmlSchema Read(XmlReader xmlReader) { return XmlSchema.Read(xmlReader, null); }
    }
}
