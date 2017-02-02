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
    /// <summary>
    /// 
    /// </summary>
    public abstract class LoadableXmlResource<TTarget> : LoadableTextResource<TTarget>
    {
        /// <summary>
        /// 
        /// </summary>
        public LoadableXmlResource(Uri uri) : base(uri) { }

        /// <summary>
        /// 
        /// </summary>
        public virtual bool CheckCharacters { get { return false; } }
#if !PSLEGACY
        /// <summary>
        /// 
        /// </summary>
        public virtual DtdProcessing DtdProcessing { get { return DtdProcessing.Ignore; } }
#endif
        /// <summary>
        /// 
        /// </summary>
        public virtual XmlSchemaSet Schemas { get { return null; } }

        /// <summary>
        /// 
        /// </summary>
        public virtual ValidationType ValidationType { get { return ValidationType.None; } }

        /// <summary>
        /// 
        /// </summary>
        protected override TTarget CreateTarget()
        {
            using (StringReader stringReader = new StringReader(String.Join("\r\n", Source)))
            {
#if PSLEGACY
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.CheckCharacters = CheckCharacters;
                settings.Schemas = Schemas;
                settings.ValidationType = ValidationType;
#else
                XmlReaderSettings settings = new XmlReaderSettings
                {
                    CheckCharacters = CheckCharacters,
                    Schemas = Schemas,
                    DtdProcessing = DtdProcessing,
                    ValidationType = ValidationType
                };
#endif
                settings.ValidationEventHandler += Xml_ValidationEventHandler;
                using (XmlReader xmlReader = XmlReader.Create(stringReader, settings))
                    return Read(xmlReader);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected void Xml_ValidationEventHandler(object sender, ValidationEventArgs e)
        {
            InnerErrors.Add(ResourceLoadError.Create(e));
        }

        /// <summary>
        /// 
        /// </summary>
        protected abstract TTarget Read(XmlReader xmlReader);
    }

    /// <summary>
    /// 
    /// </summary>
    public class LoadableXmlResource : LoadableXmlResource<XmlDocument>
    {
        /// <summary>
        /// 
        /// </summary>
        public LoadableXmlResource(Uri uri) : base(uri) { }

        /// <summary>
        /// 
        /// </summary>
        protected override XmlDocument Read(XmlReader xmlReader)
        {
            throw new NotImplementedException();
        }
    }
}
