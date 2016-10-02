using System;
using System.Collections.Generic;
using System.IO;
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
    public abstract class LoadableXmlResource<TTarget> : LoadableTextResource<TTarget>
    {
        public LoadableXmlResource(Uri uri) : base(uri) { }
        public virtual bool CheckCharacters { get { return false; } }
#if !PSLEGACY
        public virtual DtdProcessing DtdProcessing { get { return DtdProcessing.Ignore; } }
#endif
        public virtual XmlSchemaSet Schemas { get { return null; } }
        public virtual ValidationType ValidationType { get { return ValidationType.None; } }

        protected override TTarget CreateTarget()
        {
            using (StringReader stringReader = new StringReader(String.Join("\r\n", Source)))
            {
                XmlReaderSettings settings = new XmlReaderSettings
                {
                    CheckCharacters = CheckCharacters,
                    Schemas = Schemas,
#if !PSLEGACY
                    DtdProcessing = DtdProcessing,
#endif
                    ValidationType = ValidationType
                };
                settings.ValidationEventHandler += Xml_ValidationEventHandler;
                using (XmlReader xmlReader = XmlReader.Create(stringReader, settings))
                    return Read(xmlReader);
            }
        }

        protected void Xml_ValidationEventHandler(object sender, ValidationEventArgs e)
        {
            InnerErrors.Add(ResourceLoadError.Create(e));
        }

        protected abstract TTarget Read(XmlReader xmlReader);
    }

    public class LoadableXmlResource : LoadableXmlResource<XmlDocument>
    {
        public LoadableXmlResource(Uri uri) : base(uri) { }

        protected override XmlDocument Read(XmlReader xmlReader)
        {
            throw new NotImplementedException();
        }
    }
}
