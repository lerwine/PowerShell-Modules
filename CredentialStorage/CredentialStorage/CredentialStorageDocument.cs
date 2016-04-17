using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace CredentialStorageCLR
{
    [Serializable]
    [XmlRoot(ElementName = CredentialStorageDocument.RootElementName, IsNullable = false, Namespace = CredentialStorageDocument.NamespaceURI)]
    public class CredentialStorageDocument : CredentialStorageContainer<CredentialStorageDocument>
    {
        public const string RootElementName = "CredentialStorage";

        public CredentialStorageDocument() : base() { }

        public CredentialStorageDocument(CredentialStorageDocument document) : base(document) { }

        public CredentialStorageDocument(IEnumerable<ICredentialContent> collection) : base(collection) { }

        public override bool Equals(ICredentialContainer<CredentialStorageDocument> other)
        {
            return other != null && Object.ReferenceEquals(this, other);
        }

        protected override CredentialStorageDocument Clone() { return new CredentialStorageDocument(this); }

        protected override void OnSiteChanged(ICredentialSite<CredentialStorageDocument> oldSite, ICredentialSite<CredentialStorageDocument> value)
        {
            throw new NotSupportedException();
        }

        public static CredentialStorageDocument LoadXml(string xml)
        {
            using (StringReader reader = new StringReader(xml))
                return CredentialStorageDocument.Load(reader);
        }

        public static CredentialStorageDocument Load(string uri, XmlReaderSettings settings)
        {
            using (XmlReader reader = XmlReader.Create(uri, settings))
                return CredentialStorageDocument.Load(reader);
        }

        public static CredentialStorageDocument Load(string path, bool detectEncodingFromByteOrderMarks)
        {
            using (StreamReader reader = new StreamReader(path, detectEncodingFromByteOrderMarks))
                return CredentialStorageDocument.Load(reader);
        }

        public static CredentialStorageDocument Load(string path, Encoding encoding)
        {
            using (StreamReader reader = new StreamReader(path, encoding))
                return CredentialStorageDocument.Load(reader);
        }

        public static CredentialStorageDocument Load(XmlReader reader)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(CredentialStorageDocument));
            return (CredentialStorageDocument)(serializer.Deserialize(reader));
        }

        public static CredentialStorageDocument Load(TextReader reader)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(CredentialStorageDocument));
            return (CredentialStorageDocument)(serializer.Deserialize(reader));
        }

        public static CredentialStorageDocument Load(Stream stream)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(CredentialStorageDocument));
            return (CredentialStorageDocument)(serializer.Deserialize(stream));
        }

        public static CredentialStorageDocument Load(Stream stream, XmlReaderSettings settings)
        {
            using (XmlReader reader = XmlReader.Create(stream, settings))
                return CredentialStorageDocument.Load(reader);
        }

        public static CredentialStorageDocument Load(Stream stream, bool detectEncodingFromByteOrderMarks)
        {
            using (StreamReader reader = new StreamReader(stream, detectEncodingFromByteOrderMarks))
                return CredentialStorageDocument.Load(reader);
        }

        public static CredentialStorageDocument Load(Stream stream, Encoding encoding)
        {
            using (StreamReader reader = new StreamReader(stream, encoding))
                return CredentialStorageDocument.Load(reader);
        }

        public void Save(string path)
        {
            using (XmlWriter writer = XmlWriter.Create(path))
            {
                this.Save(writer);
                writer.Flush();
            }
        }

        public void Save(string path, XmlWriterSettings settings)
        {
            using (XmlWriter writer = XmlWriter.Create(path, settings))
            {
                this.Save(writer);
                writer.Flush();
            }
        }

        public void Save(XmlWriter writer)
        {
            XmlSerializer serializer = new XmlSerializer(this.GetType());
            serializer.Serialize(writer, this);
        }

        public void Save(TextWriter writer)
        {
            XmlSerializer serializer = new XmlSerializer(this.GetType());
            serializer.Serialize(writer, this);
        }

        public void Save(Stream stream)
        {
            XmlSerializer serializer = new XmlSerializer(this.GetType());
            serializer.Serialize(stream, this);
        }

        public void Save(Stream stream, XmlWriterSettings settings)
        {
            using (XmlWriter writer = XmlWriter.Create(stream, settings))
            {
                this.Save(writer);
                writer.Flush();
            }
        }

        public override string ToString() { return this.ToString(null); }

        public string ToString(XmlWriterSettings settings)
        {
            settings = (settings == null) ? new XmlWriterSettings() : settings.Clone();
            settings.CloseOutput = false;
            using (System.IO.MemoryStream stream = new MemoryStream())
            {
                this.Save(stream, settings);
                return settings.Encoding.GetString(stream.ToArray());
            }
        }
    }
}
