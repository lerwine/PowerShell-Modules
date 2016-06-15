using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;

namespace XmlUtilityCLR
{
    public abstract class LoadableXmlResource<TTarget> : LoadableTextResource<TTarget>
    {
        public LoadableXmlResource(Uri uri) : base(uri) { }
        public virtual bool CheckCharacters { get { return false; } }
        public virtual DtdProcessing DtdProcessing { get { return DtdProcessing.Ignore; } }
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
                    DtdProcessing = DtdProcessing,
                    ValidationType = ValidationType
                };
                settings.ValidationEventHandler += Xml_ValidationEventHandler;
                using (XmlReader xmlReader = XmlReader.Create(stringReader, settings))
                    return Read(xmlReader);
            }
        }

        protected void Xml_ValidationEventHandler(object sender, ValidationEventArgs e)
        {
            InnerErrors.Add(new ResourceLoadError(e));
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
