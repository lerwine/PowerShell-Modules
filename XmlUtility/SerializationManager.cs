using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace XmlUtility
{
    /// <summary>
    /// 
    /// </summary>
    public class SerializationManager<T>
        where T : class, new()
    {
        private XmlSerializer _serializer = new XmlSerializer(typeof(T));
        private Uri _location = null;
        private string _baseURI = null;
        private XmlReaderSettings _readerSettings = null;
        private bool? _detectEncodingFromByteOrderMarks = null;
        private Encoding _encoding = null;
        private int? _bufferSize = null;
        private XmlWriterSettings _writerSettings = null;
        private bool _append = false;
        private bool? _checkCharacters = null;
        private ConformanceLevel? _conformanceLevel = null;

        /// <summary>
        /// 
        /// </summary>
        public Uri Location { get { return this._location; } private set { this._location = value; } }

        /// <summary>
        /// 
        /// </summary>
        public string LocalPath
        {
            get
            {
                Uri uri = this.Location;
                return (uri != null && uri.Scheme == Uri.UriSchemeFile) ? uri.LocalPath : null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string OriginalString
        {
            get
            {
                Uri uri = this.Location;
                return (uri != null) ? uri.OriginalString : null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string BaseURI { get { return this._baseURI; } private set { this._baseURI = value; } }

        /// <summary>
        /// 
        /// </summary>
        public XmlReaderSettings ReaderSettings
        {
            get { return this._readerSettings; }
            private set
            {
                this._readerSettings = value;
                if (value != null)
                {
                    this.CheckCharacters = value.CheckCharacters;
                    this.ConformanceLevel = value.ConformanceLevel;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool? DetectEncodingFromByteOrderMarks { get { return this._detectEncodingFromByteOrderMarks; } private set { this._detectEncodingFromByteOrderMarks = value; } }

        /// <summary>
        /// 
        /// </summary>
        public Encoding Encoding { get { return this._encoding; } private set { this._encoding = value; } }

        /// <summary>
        /// 
        /// </summary>
        public int? BufferSize { get { return this._bufferSize; } private set { this._bufferSize = value; } }

        /// <summary>
        /// 
        /// </summary>
        public XmlWriterSettings WriterSettings
        {
            get { return this._writerSettings; }
            private set
            {
                this._writerSettings = value;
                if (value != null)
                {
                    this.CheckCharacters = value.CheckCharacters;
                    this.ConformanceLevel = value.ConformanceLevel;
                    this.Encoding = value.Encoding;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool Append { get { return this._append; } private set { this._append = value; } }

        /// <summary>
        /// 
        /// </summary>
        public bool? CheckCharacters { get { return this._checkCharacters; } private set { this._checkCharacters = value; } }

        /// <summary>
        /// 
        /// </summary>
        public ConformanceLevel? ConformanceLevel { get { return this._conformanceLevel; } private set { this._conformanceLevel = value; } }

        #region Load

        #region Load from reader

        /// <summary>
        /// Deserializes the credential storage document contained by the specified <see cref="System.Xml.XmlReader"/>.
        /// </summary>
        /// <param name="xmlReader">The <see cref="System.Xml.XmlReader"/> that contains the credential storage document to deserialize.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="xmlReader"/> is null.</exception>
        /// <exception cref="System.InvalidOperationException">An error occurred during deserialization. The original exception is available
        /// using the <see cref="System.Exception.InnerException"/> property.</exception>
        /// <returns>The <typeparamref name="T"/> being deserialized.</returns>
        public T Load(XmlReader xmlReader) { return this.Load(xmlReader, this.Location); }

        /// <summary>
        /// Deserializes the credential storage document contained by the specified <see cref="System.Xml.XmlReader"/> and setting the specified source URI.
        /// </summary>
        /// <param name="xmlReader">The <see cref="System.Xml.XmlReader"/> that contains the credential storage document to deserialize.</param>
        /// <param name="location">The URI representing the source location of the document's serialized data.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="xmlReader"/> is null.</exception>
        /// <exception cref="System.InvalidOperationException">An error occurred during deserialization. The original exception is available
        /// using the <see cref="System.Exception.InnerException"/> property.</exception>
        /// <returns>The <typeparamref name="T"/> being deserialized.</returns>
        public T Load(XmlReader xmlReader, Uri location)
        {
            T t;
            lock (this._serializer)
                t = (T)(this._serializer.Deserialize(xmlReader));

            this.Location = location;
            this.BaseURI = xmlReader.BaseURI;
            this.ReaderSettings = xmlReader.Settings;
            return t;
        }

        /// <summary>
        /// Deserializes the credential storage document contained by the specified <see cref="System.IO.TextReader"/>.
        /// </summary>
        /// <param name="textReader">The <see cref="System.IO.TextReader"/> that contains the credential storage document to deserialize.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="textReader"/> is null.</exception>
        /// <exception cref="System.InvalidOperationException">An error occurred during deserialization. The original exception is available
        /// using the <see cref="System.Exception.InnerException"/> property.</exception>
        /// <returns>The <typeparamref name="T"/> being deserialized.</returns>
        public T Load(TextReader textReader) { return this.Load(textReader, this.Location); }

        /// <summary>
        /// Deserializes the credential storage document contained by the specified <see cref="System.IO.TextReader"/> and setting the specified source URI.
        /// </summary>
        /// <param name="textReader">The <see cref="System.IO.TextReader"/> that contains the credential storage document to deserialize.</param>
        /// <param name="location">The URI representing the source location of the document's serialized data.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="textReader"/> is null.</exception>
        /// <exception cref="System.InvalidOperationException">An error occurred during deserialization. The original exception is available
        /// using the <see cref="System.Exception.InnerException"/> property.</exception>
        /// <returns>The <typeparamref name="T"/> being deserialized.</returns>
        public T Load(TextReader textReader, Uri location)
        {
            T t;

            if (this.ReaderSettings != null)
                return this.Load(textReader, this.ReaderSettings, location);

            if (this.CheckCharacters.HasValue || this.ConformanceLevel.HasValue)
            {
                XmlReaderSettings settings = new XmlReaderSettings();
                if (this.CheckCharacters.HasValue)
                    settings.CheckCharacters = this.CheckCharacters.Value;
                if (this.ConformanceLevel.HasValue)
                    settings.ConformanceLevel = this.ConformanceLevel.Value;
                return this.Load(textReader, settings, location);
            }

            lock (this._serializer)
                t = (T)(this._serializer.Deserialize(textReader));

            this.Location = location;
            if (textReader is StreamReader)
                this.Encoding = (textReader as StreamReader).CurrentEncoding;

            return t;
        }

        /// <summary>
        /// Deserializes the credential storage document contained by the specified <see cref="System.IO.TextReader"/> and <see cref="System.Xml.XmlReaderSettings"/> objects.
        /// </summary>
        /// <param name="textReader">The <see cref="System.IO.TextReader"/> that contains the credential storage document to deserialize.</param>
        /// <param name="settings">The <see cref="System.Xml.XmlReaderSettings"/> object used to configure the <see cref="System.Xml.XmlReader"/> which is used to deserialize the object.
        /// This value can be null.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="textReader"/> is null.</exception>
        /// <exception cref="System.InvalidOperationException">An error occurred during deserialization. The original exception is available
        /// using the <see cref="System.Exception.InnerException"/> property.</exception>
        /// <returns>The <typeparamref name="T"/> being deserialized.</returns>
        public T Load(TextReader textReader, XmlReaderSettings settings)
        {
            return this.Load(textReader, settings, this.Location);
        }

        /// <summary>
        /// Deserializes the credential storage document contained by the specified <see cref="System.IO.TextReader"/> and <see cref="System.Xml.XmlReaderSettings"/> objects,
        /// and setting the specified source URI.
        /// </summary>
        /// <param name="textReader">The <see cref="System.IO.TextReader"/> that contains the credential storage document to deserialize.</param>
        /// <param name="settings">The <see cref="System.Xml.XmlReaderSettings"/> object used to configure the <see cref="System.Xml.XmlReader"/> which is used to deserialize the object.
        /// This value can be null.</param>
        /// <param name="location">The URI representing the source location of the document's serialized data.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="textReader"/> is null.</exception>
        /// <exception cref="System.InvalidOperationException">An error occurred during deserialization. The original exception is available
        /// using the <see cref="System.Exception.InnerException"/> property.</exception>
        /// <returns>The <typeparamref name="T"/> being deserialized.</returns>
        public T Load(TextReader textReader, XmlReaderSettings settings, Uri location)
        {
            using (XmlReader xmlReader = XmlReader.Create(textReader, settings))
                return this.Load(xmlReader, location);
        }

        /// <summary>
        /// Deserializes the credential storage document contained by the specified <see cref="System.IO.TextReader"/> and <see cref="System.Xml.XmlReaderSettings"/> objects.
        /// </summary>
        /// <param name="textReader">The <see cref="System.IO.TextReader"/> that contains the credential storage document to deserialize.</param>
        /// <param name="settings">The <see cref="System.Xml.XmlReaderSettings"/> object used to configure the <see cref="System.Xml.XmlReader"/> which is used to deserialize the object.
        /// This value can be null.</param>
        /// <param name="baseUri">The base URI for the entity or document being read. This value can be null.
        /// <para>Security Note: The base URI is used to resolve the relative URI of the XML document. Do not use a base URI from an untrusted source.</para></param>
        /// <exception cref="System.ArgumentNullException"><paramref name="textReader"/> is null.</exception>
        /// <exception cref="System.InvalidOperationException">An error occurred during deserialization. The original exception is available
        /// using the <see cref="System.Exception.InnerException"/> property.</exception>
        /// <returns>The <typeparamref name="T"/> being deserialized.</returns>
        public T Load(TextReader textReader, XmlReaderSettings settings, string baseUri)
        {
            return this.Load(textReader, settings, baseUri, this.Location);
        }

        /// <summary>
        /// Deserializes the credential storage document contained by the specified <see cref="System.IO.TextReader"/>, <see cref="System.Xml.XmlReaderSettings"/>, and base URI,
        /// and setting the specified source URI.
        /// </summary>
        /// <param name="textReader">The <see cref="System.IO.TextReader"/> that contains the credential storage document to deserialize.</param>
        /// <param name="settings">The <see cref="System.Xml.XmlReaderSettings"/> object used to configure the <see cref="System.Xml.XmlReader"/> which is used to deserialize the object.
        /// This value can be null.</param>
        /// <param name="baseUri">The base URI for the entity or document being read. This value can be null.
        /// <para>Security Note: The base URI is used to resolve the relative URI of the XML document. Do not use a base URI from an untrusted source.</para></param>
        /// <param name="location">The URI representing the source location of the document's serialized data.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="textReader"/> is null.</exception>
        /// <exception cref="System.InvalidOperationException">An error occurred during deserialization. The original exception is available
        /// using the <see cref="System.Exception.InnerException"/> property.</exception>
        /// <returns>The <typeparamref name="T"/> being deserialized.</returns>
        public T Load(TextReader textReader, XmlReaderSettings settings, string baseUri, Uri location)
        {
            using (XmlReader xmlReader = XmlReader.Create(textReader, settings, baseUri))
                return this.Load(xmlReader, location);
        }

        #endregion

        #region Load from stream

        /// <summary>
        /// Deserializes the credential storage document contained by the specified <see cref="System.IO.Stream"/>.
        /// </summary>
        /// <param name="stream">The <see cref="System.IO.Stream"/> that contains the credential storage document to deserialize.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="stream"/> is null.</exception>
        /// <exception cref="System.InvalidOperationException">An error occurred during deserialization. The original exception is available
        /// using the <see cref="System.Exception.InnerException"/> property.</exception>
        /// <returns>The <typeparamref name="T"/> being deserialized.</returns>
        public T Load(Stream stream) { return this.Load(stream, this.Location); }

        /// <summary>
        /// Deserializes the credential storage document contained by the specified <see cref="System.IO.Stream"/> and setting the specified source URI.
        /// </summary>
        /// <param name="stream">The <see cref="System.IO.Stream"/> that contains the credential storage document to deserialize.</param>
        /// <param name="location">The URI representing the source location of the document's serialized data.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="stream"/> is null.</exception>
        /// <exception cref="System.InvalidOperationException">An error occurred during deserialization. The original exception is available
        /// using the <see cref="System.Exception.InnerException"/> property.</exception>
        /// <returns>The <typeparamref name="T"/> being deserialized.</returns>
        public T Load(Stream stream, Uri location)
        {
            if (this.ReaderSettings != null)
            {
                if (!String.IsNullOrEmpty(this.BaseURI))
                    return this.Load(stream, this.ReaderSettings, this.BaseURI, location);

                return this.Load(stream, this.ReaderSettings, location);
            }

            if (this.DetectEncodingFromByteOrderMarks.HasValue)
            {
                if (this.Encoding != null)
                {
                    if (this.BufferSize.HasValue)
                        return this.Load(stream, this.Encoding, this.DetectEncodingFromByteOrderMarks.Value, this.BufferSize.Value, location);
                    return this.Load(stream, this.Encoding, this.DetectEncodingFromByteOrderMarks.Value, location);
                }
                return this.Load(stream, this.DetectEncodingFromByteOrderMarks.Value, location);
            }

            if (this.Encoding != null)
                return this.Load(stream, this.Encoding, location);

            if (this.CheckCharacters.HasValue || this.ConformanceLevel.HasValue)
            {
                XmlReaderSettings settings = new XmlReaderSettings();
                if (this.CheckCharacters.HasValue)
                    settings.CheckCharacters = this.CheckCharacters.Value;
                if (this.ConformanceLevel.HasValue)
                    settings.ConformanceLevel = this.ConformanceLevel.Value;
                return this.Load(stream, settings, location);
            }

            T t;

            lock (this._serializer)
                t = (T)(this._serializer.Deserialize(stream));

            this.Location = location;

            return t;
        }

        /// <summary>
        /// Deserializes the credential storage document contained by the specified <see cref="System.IO.Stream"/> and <see cref="System.Xml.XmlReaderSettings"/> objects.
        /// </summary>
        /// <param name="stream">The <see cref="System.IO.Stream"/> that contains the credential storage document to deserialize.</param>
        /// <param name="settings">The <see cref="System.Xml.XmlReaderSettings"/> object used to configure the <see cref="System.Xml.XmlReader"/> which is used to deserialize the object.
        /// This value can be null.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="stream"/> is null.</exception>
        /// <exception cref="System.InvalidOperationException">An error occurred during deserialization. The original exception is available
        /// using the <see cref="System.Exception.InnerException"/> property.</exception>
        /// <returns>The <typeparamref name="T"/> being deserialized.</returns>
        public T Load(Stream stream, XmlReaderSettings settings)
        {
            return this.Load(stream, settings, null as Uri);
        }

        /// <summary>
        /// Deserializes the credential storage document contained by the specified <see cref="System.IO.Stream"/> and <see cref="System.Xml.XmlReaderSettings"/> objects,
        /// and setting the specified source URI.
        /// </summary>
        /// <param name="stream">The <see cref="System.IO.Stream"/> that contains the credential storage document to deserialize.</param>
        /// <param name="settings">The <see cref="System.Xml.XmlReaderSettings"/> object used to configure the <see cref="System.Xml.XmlReader"/> which is used to deserialize the object.
        /// This value can be null.</param>
        /// <param name="location">The URI representing the source location of the document's serialized data.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="stream"/> is null.</exception>
        /// <exception cref="System.InvalidOperationException">An error occurred during deserialization. The original exception is available
        /// using the <see cref="System.Exception.InnerException"/> property.</exception>
        /// <returns>The <typeparamref name="T"/> being deserialized.</returns>
        public T Load(Stream stream, XmlReaderSettings settings, Uri location)
        {
            using (XmlReader xmlReader = XmlReader.Create(stream, settings))
                return this.Load(xmlReader, location);
        }

        /// <summary>
        /// Deserializes the credential storage document contained by the specified <see cref="System.IO.Stream"/>, <see cref="System.Xml.XmlReaderSettings"/>, and base URI.
        /// </summary>
        /// <param name="stream">The <see cref="System.IO.Stream"/> that contains the credential storage document to deserialize.</param>
        /// <param name="settings">The <see cref="System.Xml.XmlReaderSettings"/> object used to configure the <see cref="System.Xml.XmlReader"/> which is used to deserialize the object.
        /// This value can be null.</param>
        /// <param name="baseUri">The base URI for the entity or document being read. This value can be null.
        /// <para>Security Note: The base URI is used to resolve the relative URI of the XML document. Do not use a base URI from an untrusted source.</para></param>
        /// <exception cref="System.ArgumentNullException"><paramref name="stream"/> is null.</exception>
        /// <exception cref="System.InvalidOperationException">An error occurred during deserialization. The original exception is available
        /// using the <see cref="System.Exception.InnerException"/> property.</exception>
        /// <returns>The <typeparamref name="T"/> being deserialized.</returns>
        public T Load(Stream stream, XmlReaderSettings settings, string baseUri)
        {
            return this.Load(stream, settings, baseUri, null as Uri);
        }

        /// <summary>
        /// Deserializes the credential storage document contained by the specified <see cref="System.IO.Stream"/>, <see cref="System.Xml.XmlReaderSettings"/>, and base URI,
        /// and setting the specified source URI.
        /// </summary>
        /// <param name="stream">The <see cref="System.IO.Stream"/> that contains the credential storage document to deserialize.</param>
        /// <param name="settings">The <see cref="System.Xml.XmlReaderSettings"/> object used to configure the <see cref="System.Xml.XmlReader"/> which is used to deserialize the object.
        /// This value can be null.</param>
        /// <param name="baseUri">The base URI for the entity or document being read. This value can be null.
        /// <para>Security Note: The base URI is used to resolve the relative URI of the XML document. Do not use a base URI from an untrusted source.</para></param>
        /// <param name="location">The URI representing the source location of the document's serialized data.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="stream"/> is null.</exception>
        /// <exception cref="System.InvalidOperationException">An error occurred during deserialization. The original exception is available
        /// using the <see cref="System.Exception.InnerException"/> property.</exception>
        /// <returns>The <typeparamref name="T"/> being deserialized.</returns>
        public T Load(Stream stream, XmlReaderSettings settings, string baseUri, Uri location)
        {
            using (XmlReader xmlReader = XmlReader.Create(stream, settings, baseUri))
                return this.Load(xmlReader, location);
        }

        /// <summary>
        /// Deserializes the credential storage document contained by the specified <see cref="System.IO.Stream"/>, with the specified byte order mark detection option.
        /// </summary>
        /// <param name="stream">The <see cref="System.IO.Stream"/> that contains the credential storage document to deserialize.</param>
        /// <param name="detectEncodingFromByteOrderMarks">Indicates whether to look for byte order marks at the beginning of the file.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="stream"/> is null.</exception>
        /// <exception cref="System.ArgumentException"><paramref name="stream"/> does not support reading.</exception>
        /// <exception cref="System.InvalidOperationException">An error occurred during deserialization. The original exception is available
        /// using the <see cref="System.Exception.InnerException"/> property.</exception>
        /// <returns>The <typeparamref name="T"/> being deserialized.</returns>
        public T Load(Stream stream, bool detectEncodingFromByteOrderMarks)
        {
            return this.Load(stream, detectEncodingFromByteOrderMarks, null);
        }

        /// <summary>
        /// Deserializes the credential storage document contained by the specified <see cref="System.IO.Stream"/>, with the specified byte order mark detection option.
        /// </summary>
        /// <param name="stream">The <see cref="System.IO.Stream"/> that contains the credential storage document to deserialize.</param>
        /// <param name="detectEncodingFromByteOrderMarks">Indicates whether to look for byte order marks at the beginning of the file.</param>
        /// <param name="location">The URI representing the source location of the document's serialized data.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="stream"/> is null.</exception>
        /// <exception cref="System.ArgumentException"><paramref name="stream"/> does not support reading.</exception>
        /// <exception cref="System.InvalidOperationException">An error occurred during deserialization. The original exception is available
        /// using the <see cref="System.Exception.InnerException"/> property.</exception>
        /// <returns>The <typeparamref name="T"/> being deserialized.</returns>
        public T Load(Stream stream, bool detectEncodingFromByteOrderMarks, Uri location)
        {
            T t;

            using (StreamReader textReader = new StreamReader(stream, detectEncodingFromByteOrderMarks))
                t = this.Load(textReader, location);

            this.DetectEncodingFromByteOrderMarks = detectEncodingFromByteOrderMarks;

            return t;
        }

        /// <summary>
        /// Deserializes the credential storage document contained by the specified <see cref="System.IO.Stream"/>, with the specified character encoding.
        /// </summary>
        /// <param name="stream">The <see cref="System.IO.Stream"/> that contains the credential storage document to deserialize.</param>
        /// <param name="encoding">The character encoding to use.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="stream"/> is null.</exception>
        /// <exception cref="System.ArgumentException"><paramref name="stream"/> does not support reading.</exception>
        /// <exception cref="System.InvalidOperationException">An error occurred during deserialization. The original exception is available
        /// using the <see cref="System.Exception.InnerException"/> property.</exception>
        /// <returns>The <typeparamref name="T"/> being deserialized.</returns>
        public T Load(Stream stream, Encoding encoding) { return this.Load(stream, encoding, null); }

        /// <summary>
        /// Deserializes the credential storage document contained by the specified <see cref="System.IO.Stream"/>, with the specified character encoding.
        /// </summary>
        /// <param name="stream">The <see cref="System.IO.Stream"/> that contains the credential storage document to deserialize.</param>
        /// <param name="encoding">The character encoding to use.</param>
        /// <param name="location">The URI representing the source location of the document's serialized data.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="stream"/> is null.</exception>
        /// <exception cref="System.ArgumentException"><paramref name="stream"/> does not support reading.</exception>
        /// <exception cref="System.InvalidOperationException">An error occurred during deserialization. The original exception is available
        /// using the <see cref="System.Exception.InnerException"/> property.</exception>
        /// <returns>The <typeparamref name="T"/> being deserialized.</returns>
        public T Load(Stream stream, Encoding encoding, Uri location)
        {
            using (StreamReader textReader = new StreamReader(stream, encoding))
                return this.Load(textReader, location);
        }

        /// <summary>
        /// Deserializes the credential storage document contained by the specified <see cref="System.IO.Stream"/>, with the specified character encoding
        /// and byte order mark detection option.
        /// </summary>
        /// <param name="stream">The <see cref="System.IO.Stream"/> that contains the credential storage document to deserialize.</param>
        /// <param name="encoding">The character encoding to use.</param>
        /// <param name="detectEncodingFromByteOrderMarks">Indicates whether to look for byte order marks at the beginning of the file.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="stream"/> or <paramref name="encoding"/> is null.</exception>
        /// <exception cref="System.ArgumentException"><paramref name="stream"/> does not support reading.</exception>
        /// <exception cref="System.InvalidOperationException">An error occurred during deserialization. The original exception is available
        /// using the <see cref="System.Exception.InnerException"/> property.</exception>
        /// <returns>The <typeparamref name="T"/> being deserialized.</returns>
        public T Load(Stream stream, Encoding encoding, bool detectEncodingFromByteOrderMarks)
        {
            return this.Load(stream, encoding, detectEncodingFromByteOrderMarks, null);
        }

        /// <summary>
        /// Deserializes the credential storage document contained by the specified <see cref="System.IO.Stream"/>, with the specified character encoding
        /// and byte order mark detection option.
        /// </summary>
        /// <param name="stream">The <see cref="System.IO.Stream"/> that contains the credential storage document to deserialize.</param>
        /// <param name="encoding">The character encoding to use.</param>
        /// <param name="detectEncodingFromByteOrderMarks">Indicates whether to look for byte order marks at the beginning of the file.</param>
        /// <param name="location">The URI representing the source location of the document's serialized data.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="stream"/> or <paramref name="encoding"/> is null.</exception>
        /// <exception cref="System.ArgumentException"><paramref name="stream"/> does not support reading.</exception>
        /// <exception cref="System.InvalidOperationException">An error occurred during deserialization. The original exception is available
        /// using the <see cref="System.Exception.InnerException"/> property.</exception>
        /// <returns>The <typeparamref name="T"/> being deserialized.</returns>
        public T Load(Stream stream, Encoding encoding, bool detectEncodingFromByteOrderMarks, Uri location)
        {
            T t;

            using (StreamReader textReader = new StreamReader(stream, encoding, detectEncodingFromByteOrderMarks))
                t = this.Load(textReader, location);

            this.DetectEncodingFromByteOrderMarks = detectEncodingFromByteOrderMarks;

            return t;
        }

        /// <summary>
        /// Deserializes the credential storage document contained by the specified <see cref="System.IO.Stream"/>, with the specified character encoding, 
        /// byte order mark detection option, and buffer size.
        /// </summary>
        /// <param name="stream">The <see cref="System.IO.Stream"/> that contains the credential storage document to deserialize.</param>
        /// <param name="encoding">The character encoding to use.</param>
        /// <param name="detectEncodingFromByteOrderMarks">Indicates whether to look for byte order marks at the beginning of the file.</param>
        /// <param name="bufferSize">The minimum buffer size.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="stream"/> or <paramref name="encoding"/> is null.</exception>
        /// <exception cref="System.ArgumentException"><paramref name="stream"/> does not support reading.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException"><paramref name="bufferSize"/> is less than or equal to zero.</exception>
        /// <exception cref="System.InvalidOperationException">An error occurred during deserialization. The original exception is available
        /// using the <see cref="System.Exception.InnerException"/> property.</exception>
        /// <returns>The <typeparamref name="T"/> being deserialized.</returns>
        public T Load(Stream stream, Encoding encoding, bool detectEncodingFromByteOrderMarks, int bufferSize)
        {
            return this.Load(stream, encoding, detectEncodingFromByteOrderMarks, bufferSize, null);
        }

        /// <summary>
        /// Deserializes the credential storage document contained by the specified <see cref="System.IO.Stream"/>, with the specified character encoding, 
        /// byte order mark detection option, and buffer size.
        /// </summary>
        /// <param name="stream">The <see cref="System.IO.Stream"/> that contains the credential storage document to deserialize.</param>
        /// <param name="encoding">The character encoding to use.</param>
        /// <param name="detectEncodingFromByteOrderMarks">Indicates whether to look for byte order marks at the beginning of the file.</param>
        /// <param name="bufferSize">The minimum buffer size.</param>
        /// <param name="location">The URI representing the source location of the document's serialized data.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="stream"/> or <paramref name="encoding"/> is null.</exception>
        /// <exception cref="System.ArgumentException"><paramref name="stream"/> does not support reading.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException"><paramref name="bufferSize"/> is less than or equal to zero.</exception>
        /// <exception cref="System.InvalidOperationException">An error occurred during deserialization. The original exception is available
        /// using the <see cref="System.Exception.InnerException"/> property.</exception>
        /// <returns>The <typeparamref name="T"/> being deserialized.</returns>
        public T Load(Stream stream, Encoding encoding, bool detectEncodingFromByteOrderMarks, int bufferSize, Uri location)
        {
            T t;

            using (StreamReader textReader = new StreamReader(stream, encoding, detectEncodingFromByteOrderMarks, bufferSize))
                t = this.Load(textReader, location);

            this.DetectEncodingFromByteOrderMarks = detectEncodingFromByteOrderMarks;
            this.BufferSize = bufferSize;

            return t;
        }

        #endregion

        #region Load from URI

        /// <summary>
        /// Deserializes the credential storage document contained at the specified <paramref name="inputUri"/>.
        /// </summary>
        /// <param name="inputUri">The URI for the file containing the XML data.
        /// The <see cref="System.Xml.XmlUrlResolver"/> class is used to convert the path to a canonical data representation.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="inputUri"/> is null.</exception>
        /// <exception cref="System.IO.FileNotFoundException">The file cannot be found.</exception>
        /// <exception cref="System.IO.DirectoryNotFoundException">The specified <paramref name="inputUri"/> is invalid, such as being on an unmapped drive.</exception>
        /// <exception cref="System.Security.SecurityException">XML reader object does not have sufficient permissions to access the location of the XML data.</exception>
        /// <exception cref="System.InvalidOperationException">An error occurred during deserialization. The original exception is available
        /// using the <see cref="System.Exception.InnerException"/> property.</exception>
        /// <returns>The <typeparamref name="T"/> being deserialized.</returns>
        public T Load(Uri inputUri)
        {
            if (inputUri == null)
                throw new ArgumentNullException("inputUri");

            if (this.ReaderSettings != null)
                return this.Load(inputUri, this.ReaderSettings);

            if (inputUri.Scheme != Uri.UriSchemeFile)
            {
                XmlReaderSettings settings;
                if (this.CheckCharacters.HasValue)
                {
                    settings = new XmlReaderSettings();
                    settings.CheckCharacters = this.CheckCharacters.Value;
                    if (this.ConformanceLevel.HasValue)
                        settings.ConformanceLevel = this.ConformanceLevel.Value;
                }
                else if (this.ConformanceLevel.HasValue)
                {
                    settings = new XmlReaderSettings();
                    settings.ConformanceLevel = this.ConformanceLevel.Value;
                }
                else
                {
                    using (XmlReader xmlReader = XmlReader.Create(inputUri.OriginalString))
                        return this.Load(xmlReader, inputUri);
                }
                return this.Load(inputUri, settings);
            }

            T t;

            if (this.DetectEncodingFromByteOrderMarks.HasValue)
            {
                if (this.Encoding != null)
                {
                    if (this.BufferSize.HasValue)
                        t = this.Load(inputUri.LocalPath, this.Encoding, this.DetectEncodingFromByteOrderMarks.Value, this.BufferSize.Value);
                    else
                        t = this.Load(inputUri.LocalPath, this.Encoding, this.DetectEncodingFromByteOrderMarks.Value);
                }
                else
                    t = this.Load(inputUri.LocalPath, this.DetectEncodingFromByteOrderMarks.Value);
                this.Location = inputUri;
                return t;
            }

            if (this.Encoding != null)
            {
                t = this.Load(inputUri.LocalPath, this.Encoding);
                this.Location = inputUri;
                return t;
            }

            if (this.CheckCharacters.HasValue || this.ConformanceLevel.HasValue)
            {
                XmlReaderSettings settings = new XmlReaderSettings();
                if (this.CheckCharacters.HasValue)
                    settings.CheckCharacters = this.CheckCharacters.Value;
                if (this.ConformanceLevel.HasValue)
                    settings.ConformanceLevel = this.ConformanceLevel.Value;
                return this.Load(inputUri, settings);
            }

            using (XmlReader xmlReader = XmlReader.Create(inputUri.LocalPath))
                return this.Load(xmlReader, inputUri);
        }

        /// <summary>
        /// Deserializes the credential storage document contained at the specified <paramref name="inputUri"/>, with the specified <see cref="System.Xml.XmlReaderSettings"/>.
        /// </summary>
        /// <param name="inputUri">The URI for the file containing the XML data.
        /// The <see cref="System.Xml.XmlUrlResolver"/> class is used to convert the path to a canonical data representation.</param>
        /// <param name="settings">The <see cref="System.Xml.XmlReaderSettings"/> object used to configure the <see cref="System.Xml.XmlReader"/> which is used to deserialize the object.
        /// This value can be null.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="inputUri"/> is null.</exception>
        /// <exception cref="System.IO.FileNotFoundException">The file cannot be found.</exception>
        /// <exception cref="System.IO.DirectoryNotFoundException">The specified <paramref name="inputUri"/> is invalid, such as being on an unmapped drive.</exception>
        /// <exception cref="System.Security.SecurityException">XML reader object does not have sufficient permissions to access the location of the XML data.</exception>
        /// <exception cref="System.InvalidOperationException">An error occurred during deserialization. The original exception is available
        /// using the <see cref="System.Exception.InnerException"/> property.</exception>
        /// <returns>The <typeparamref name="T"/> being deserialized.</returns>
        public T Load(Uri inputUri, XmlReaderSettings settings)
        {
            if (inputUri == null)
                throw new ArgumentNullException("inputUri");

            using (XmlReader xmlReader = XmlReader.Create((inputUri.Scheme == Uri.UriSchemeFile) ? inputUri.LocalPath : inputUri.OriginalString, settings))
                return this.Load(xmlReader, inputUri);
        }

        #endregion

        #region Load from path

        /// <summary>
        /// Deserializes the credential storage document contained at the specified <paramref name="path"/>.
        /// </summary>
        /// <param name="path">The complete file path to be read.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="path"/> is null.</exception>
        /// <exception cref="System.IO.FileNotFoundException">The file cannot be found.</exception>
        /// <exception cref="System.IO.DirectoryNotFoundException">The specified <paramref name="path"/> is invalid, such as being on an unmapped drive.</exception>
        /// <exception cref="System.Security.SecurityException">XML reader object does not have sufficient permissions to access the location of the XML data.</exception>
        /// <exception cref="System.UriFormatException">The path (URI) format is not correct.</exception>
        /// <exception cref="System.InvalidOperationException">An error occurred during deserialization. The original exception is available
        /// using the <see cref="System.Exception.InnerException"/> property.</exception>
        /// <returns>The <typeparamref name="T"/> being deserialized.</returns>
        public T Load(string path)
        {
            if (path == null)
                throw new ArgumentNullException("path");

            Uri inputUri;
            if (!Uri.TryCreate(path, UriKind.RelativeOrAbsolute, out inputUri))
                throw new UriFormatException("Invalid path or URI string.");

            return this.Load(inputUri);
        }

        /// <summary>
        /// Deserializes the credential storage document contained at the specified <paramref name="path"/>, with the specified <see cref="System.Xml.XmlReaderSettings"/>.
        /// </summary>
        /// <param name="path">The complete file path to be read.</param>
        /// <param name="settings">The <see cref="System.Xml.XmlReaderSettings"/> object used to configure the <see cref="System.Xml.XmlReader"/> which is used to deserialize the object.
        /// This value can be null.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="path"/> is null.</exception>
        /// <exception cref="System.IO.FileNotFoundException">The file cannot be found.</exception>
        /// <exception cref="System.IO.DirectoryNotFoundException">The specified <paramref name="path"/> is invalid, such as being on an unmapped drive.</exception>
        /// <exception cref="System.Security.SecurityException">XML reader object does not have sufficient permissions to access the location of the XML data.</exception>
        /// <exception cref="System.UriFormatException">The path (URI) format is not correct.</exception>
        /// <exception cref="System.InvalidOperationException">An error occurred during deserialization. The original exception is available
        /// using the <see cref="System.Exception.InnerException"/> property.</exception>
        /// <returns>The <typeparamref name="T"/> being deserialized.</returns>
        public T Load(string path, XmlReaderSettings settings)
        {
            if (path == null)
                throw new ArgumentNullException("path");

            Uri inputUri;
            if (!Uri.TryCreate(path, UriKind.RelativeOrAbsolute, out inputUri))
                throw new UriFormatException("Invalid path or URI string.");

            return this.Load(inputUri, settings);
        }

        /// <summary>
        /// Deserializes the credential storage document contained at the specified <paramref name="path"/>, with the specified byte order mark detection option.
        /// </summary>
        /// <param name="path">The complete file path to be read.</param>
        /// <param name="detectEncodingFromByteOrderMarks">Indicates whether to look for byte order marks at the beginning of the file.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="path"/> is null.</exception>
        /// <exception cref="System.ArgumentException"><paramref name="path"/> is an empty string ("").</exception>
        /// <exception cref="System.IO.FileNotFoundException">The file cannot be found.</exception>
        /// <exception cref="System.IO.DirectoryNotFoundException">The specified <paramref name="path"/> is invalid, such as being on an unmapped drive.</exception>
        /// <exception cref="System.IO.IOException"><paramref name="path"/> includes an incorrect or invalid syntax for file name, directory name, or volume label.</exception>
        /// <exception cref="System.InvalidOperationException">An error occurred during deserialization. The original exception is available
        /// using the <see cref="System.Exception.InnerException"/> property.</exception>
        /// <returns>The <typeparamref name="T"/> being deserialized.</returns>
        public T Load(string path, bool detectEncodingFromByteOrderMarks)
        {
            if (path == null)
                throw new ArgumentNullException("path");

            if (this.Encoding != null)
            {
                if (this.BufferSize.HasValue)
                    return this.Load(path, this.Encoding, detectEncodingFromByteOrderMarks, this.BufferSize.Value);
                return this.Load(path, this.Encoding, detectEncodingFromByteOrderMarks);
            }

            Uri inputUri;
            if (!Uri.TryCreate(path, UriKind.RelativeOrAbsolute, out inputUri))
                throw new UriFormatException("Invalid path or URI string.");

            T t;

            using (StreamReader textReader = new StreamReader(path, detectEncodingFromByteOrderMarks))
                t = this.Load(textReader);

            this.DetectEncodingFromByteOrderMarks = detectEncodingFromByteOrderMarks;

            return t;
        }

        /// <summary>
        /// Deserializes the credential storage document contained at the specified <paramref name="path"/>, with the specified byte order mark detection option.
        /// </summary>
        /// <param name="path">The complete file path to be read.</param>
        /// <param name="encoding">The character encoding to use.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="path"/> or <paramref name="encoding"/> is null.</exception>
        /// <exception cref="System.ArgumentException"><paramref name="path"/> is an empty string ("").</exception>
        /// <exception cref="System.IO.FileNotFoundException">The file cannot be found.</exception>
        /// <exception cref="System.IO.DirectoryNotFoundException">The specified <paramref name="path"/> is invalid, such as being on an unmapped drive.</exception>
        /// <exception cref="System.IO.IOException"><paramref name="path"/> includes an incorrect or invalid syntax for file name, directory name, or volume label.</exception>
        /// <exception cref="System.InvalidOperationException">An error occurred during deserialization. The original exception is available
        /// using the <see cref="System.Exception.InnerException"/> property.</exception>
        /// <returns>The <typeparamref name="T"/> being deserialized.</returns>
        public T Load(string path, Encoding encoding)
        {
            if (path == null)
                throw new ArgumentNullException("path");

            if (this.DetectEncodingFromByteOrderMarks.HasValue)
            {
                if (this.BufferSize.HasValue)
                    return this.Load(path, encoding, this.DetectEncodingFromByteOrderMarks.Value, this.BufferSize.Value);

                return this.Load(path, encoding, this.DetectEncodingFromByteOrderMarks.Value);
            }

            Uri inputUri;
            if (!Uri.TryCreate(path, UriKind.RelativeOrAbsolute, out inputUri))
                throw new System.IO.IOException("Invalid path or URI string.");

            using (StreamReader textReader = new StreamReader(path, encoding))
                return this.Load(textReader, inputUri);
        }

        /// <summary>
        /// Deserializes the credential storage document contained at the specified <paramref name="path"/>, with the specified character encoding and 
        /// byte order mark detection option.
        /// </summary>
        /// <param name="path">The complete file path to be read.</param>
        /// <param name="encoding">The character encoding to use.</param>
        /// <param name="detectEncodingFromByteOrderMarks">Indicates whether to look for byte order marks at the beginning of the file.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="path"/> or <paramref name="encoding"/> is null.</exception>
        /// <exception cref="System.ArgumentException"><paramref name="path"/> is an empty string ("").</exception>
        /// <exception cref="System.IO.FileNotFoundException">The file cannot be found.</exception>
        /// <exception cref="System.IO.DirectoryNotFoundException">The specified <paramref name="path"/> is invalid, such as being on an unmapped drive.</exception>
        /// <exception cref="System.NotSupportedException"><paramref name="path"/> includes an incorrect or invalid syntax for file name, directory name, or volume label.</exception>
        /// <exception cref="System.InvalidOperationException">An error occurred during deserialization. The original exception is available
        /// using the <see cref="System.Exception.InnerException"/> property.</exception>
        /// <returns>The <typeparamref name="T"/> being deserialized.</returns>
        public T Load(string path, Encoding encoding, bool detectEncodingFromByteOrderMarks)
        {
            if (path == null)
                throw new ArgumentNullException("path");

            if (this.BufferSize.HasValue)
                return this.Load(path, encoding, detectEncodingFromByteOrderMarks, this.BufferSize.Value);

            Uri inputUri;
            if (!Uri.TryCreate(path, UriKind.RelativeOrAbsolute, out inputUri))
                throw new System.NotSupportedException("Invalid path or URI string.");

            T t;

            using (StreamReader textReader = new StreamReader(path, encoding, detectEncodingFromByteOrderMarks))
                t = this.Load(textReader, inputUri);

            this.DetectEncodingFromByteOrderMarks = detectEncodingFromByteOrderMarks;

            return t;
        }

        /// <summary>
        /// Deserializes the credential storage document contained at the specified <paramref name="path"/>, with the specified character encoding, 
        /// byte order mark detection option, and buffer size.
        /// </summary>
        /// <param name="path">The complete file path to be read.</param>
        /// <param name="encoding">The character encoding to use.</param>
        /// <param name="detectEncodingFromByteOrderMarks">Indicates whether to look for byte order marks at the beginning of the file.</param>
        /// <param name="bufferSize">The minimum buffer size, in number of 16-bit characters.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="path"/> or <paramref name="encoding"/> is null.</exception>
        /// <exception cref="System.ArgumentException"><paramref name="path"/> is an empty string ("").</exception>
        /// <exception cref="System.ArgumentOutOfRangeException"><paramref name="bufferSize"/> is less than or equal to zero.</exception>
        /// <exception cref="System.IO.FileNotFoundException">The file cannot be found.</exception>
        /// <exception cref="System.IO.DirectoryNotFoundException">The specified <paramref name="path"/> is invalid, such as being on an unmapped drive.</exception>
        /// <exception cref="System.NotSupportedException"><paramref name="path"/> includes an incorrect or invalid syntax for file name, directory name,
        /// or volume label.</exception>
        /// <exception cref="System.InvalidOperationException">An error occurred during deserialization. The original exception is available
        /// using the <see cref="System.Exception.InnerException"/> property.</exception>
        /// <returns>The <typeparamref name="T"/> being deserialized.</returns>
        public T Load(string path, Encoding encoding, bool detectEncodingFromByteOrderMarks, int bufferSize)
        {
            if (path == null)
                throw new ArgumentNullException("path");

            Uri inputUri;
            if (!Uri.TryCreate(path, UriKind.RelativeOrAbsolute, out inputUri))
                throw new System.NotSupportedException("Invalid path or URI string.");

            T t;

            using (StreamReader textReader = new StreamReader(path, encoding, detectEncodingFromByteOrderMarks, bufferSize))
                t = this.Load(textReader, inputUri);

            this.DetectEncodingFromByteOrderMarks = detectEncodingFromByteOrderMarks;
            this.BufferSize = bufferSize;

            return t;
        }

        #endregion

        #endregion

        #region Save

        #region Save to writer

        /// <summary>
        /// Serializes the current <typeparamref name="T"/> as XML, using the specified <see cref="System.IO.TextWriter"/>.
        /// </summary>
        /// <param name="textWriter">The <see cref="System.IO.TextWriter"/> used to write the XML document.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="textWriter"/> is null.</exception>
        /// <exception cref="System.InvalidOperationException">An error occurred during serialization. The original exception is available
        /// using the <see cref="System.Exception.InnerException"/> property.</exception>
        public void Save(TextWriter textWriter) { this.Save(textWriter, this.Location); }

        /// <summary>
        /// Serializes the current <typeparamref name="T"/> as XML, using the specified <see cref="System.IO.TextWriter"/>.
        /// </summary>
        /// <param name="textWriter">The <see cref="System.IO.TextWriter"/> used to write the XML document.</param>
        /// <param name="location">The URI representing the source location of the document's serialized data.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="textWriter"/> is null.</exception>
        /// <exception cref="System.InvalidOperationException">An error occurred during serialization. The original exception is available
        /// using the <see cref="System.Exception.InnerException"/> property.</exception>
        public void Save(TextWriter textWriter, Uri location)
        {
            lock (this._serializer)
            {
                this._serializer.Serialize(textWriter, this);
                this.Location = location;
                this.Encoding = textWriter.Encoding;
            }
        }

        /// <summary>
        /// Serializes the current <typeparamref name="T"/> as XML, using the specified <see cref="System.IO.TextWriter"/> and
        /// <see cref="System.Xml.XmlWriterSettings"/> objects.
        /// </summary>
        /// <param name="textWriter">The <see cref="System.IO.TextWriter"/> used to write the XML document.</param>
        /// <param name="settings">The <see cref="System.Xml.XmlWriterSettings"/> object used to configure the XML writer.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="textWriter"/> is null.</exception>
        /// <exception cref="System.InvalidOperationException">An error occurred during serialization. The original exception is available
        /// using the <see cref="System.Exception.InnerException"/> property.</exception>
        public void Save(TextWriter textWriter, XmlWriterSettings settings) { this.Save(textWriter, settings, this.Location); }

        /// <summary>
        /// Serializes the current <typeparamref name="T"/> as XML, using the specified <see cref="System.IO.TextWriter"/> and
        /// <see cref="System.Xml.XmlWriterSettings"/> objects.
        /// </summary>
        /// <param name="textWriter">The <see cref="System.IO.TextWriter"/> used to write the XML document.</param>
        /// <param name="settings">The <see cref="System.Xml.XmlWriterSettings"/> object used to configure the XML writer.</param>
        /// <param name="location">The URI representing the source location of the document's serialized data.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="textWriter"/> is null.</exception>
        /// <exception cref="System.InvalidOperationException">An error occurred during serialization. The original exception is available
        /// using the <see cref="System.Exception.InnerException"/> property.</exception>
        public void Save(TextWriter textWriter, XmlWriterSettings settings, Uri location)
        {
            settings = (settings == null) ? new XmlWriterSettings() : settings.Clone();
            settings.CloseOutput = false;
            using (XmlWriter xmlWriter = XmlWriter.Create(textWriter, settings))
            {
                this.Save(xmlWriter, location);
                xmlWriter.Flush();
            }
        }

        /// <summary>
        /// Serializes the current <typeparamref name="T"/> as XML, using the specified <see cref="System.Xml.XmlWriter"/>.
        /// </summary>
        /// <param name="xmlWriter">The <see cref="System.Xml.XmlWriter"/> used to write the XML document.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="xmlWriter"/> is null.</exception>
        /// <exception cref="System.InvalidOperationException">An error occurred during serialization. The original exception is available
        /// using the <see cref="System.Exception.InnerException"/> property.</exception>
        public void Save(XmlWriter xmlWriter) { this.Save(xmlWriter, this.Location); }

        /// <summary>
        /// Serializes the current <typeparamref name="T"/> as XML, using the specified <see cref="System.Xml.XmlWriter"/>.
        /// </summary>
        /// <param name="xmlWriter">The <see cref="System.Xml.XmlWriter"/> used to write the XML document.</param>
        /// <param name="location">The URI representing the source location of the document's serialized data.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="xmlWriter"/> is null.</exception>
        /// <exception cref="System.InvalidOperationException">An error occurred during serialization. The original exception is available
        /// using the <see cref="System.Exception.InnerException"/> property.</exception>
        public void Save(XmlWriter xmlWriter, Uri location)
        {
            lock (this._serializer)
            {
                this._serializer.Serialize(xmlWriter, this);
                this.Location = location;
                this.WriterSettings = xmlWriter.Settings;
            }
        }

        #endregion

        #region Save to stream

        /// <summary>
        /// Serializes the current <typeparamref name="T"/> as XML, using the specified <see cref="System.IO.Stream"/>.
        /// </summary>
        /// <param name="stream">The <see cref="System.IO.Stream"/> used to write the XML document.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="stream"/> is null.</exception>
        /// <exception cref="System.ArgumentException"><paramref name="stream"/> is not writable.</exception>
        /// <exception cref="System.InvalidOperationException">An error occurred during serialization. The original exception is available
        /// using the <see cref="System.Exception.InnerException"/> property.</exception>
        public void Save(Stream stream) { this.Save(stream, this.Location); }

        /// <summary>
        /// Serializes the current <typeparamref name="T"/> as XML, using the specified <see cref="System.IO.Stream"/>.
        /// </summary>
        /// <param name="stream">The <see cref="System.IO.Stream"/> used to write the XML document.</param>
        /// <param name="location">The URI representing the source location of the document's serialized data.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="stream"/> is null.</exception>
        /// <exception cref="System.ArgumentException"><paramref name="stream"/> is not writable.</exception>
        /// <exception cref="System.InvalidOperationException">An error occurred during serialization. The original exception is available
        /// using the <see cref="System.Exception.InnerException"/> property.</exception>
        public void Save(Stream stream, Uri location)
        {
            if (this.WriterSettings != null)
            {
                this.Save(stream, this.WriterSettings, location);
                return;
            }

            if (this.Encoding != null)
            {
                if (this.BufferSize.HasValue)
                    this.Save(stream, this.Encoding, this.BufferSize.Value);
                else
                    this.Save(stream, this.Encoding);
                return;
            }

            if (this.CheckCharacters.HasValue || this.ConformanceLevel.HasValue)
            {
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.CheckCharacters = this.CheckCharacters.Value;
                settings.ConformanceLevel = this.ConformanceLevel.Value;
                this.Save(stream, settings, location);
                return;
            }

            lock (this._serializer)
            {
                this._serializer.Serialize(stream, this);
                this.Location = location;
            }
        }

        /// <summary>
        /// Serializes the current <typeparamref name="T"/> as XML, using the specified <see cref="System.IO.Stream"/> and
        /// <see cref="System.Xml.XmlWriterSettings"/> objects.
        /// </summary>
        /// <param name="stream">The <see cref="System.IO.Stream"/> used to write the XML document.</param>
        /// <param name="settings">The <see cref="System.Xml.XmlWriterSettings"/> object used to configure the XML writer.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="stream"/> is null.</exception>
        /// <exception cref="System.ArgumentException"><paramref name="stream"/> is not writable.</exception>
        /// <exception cref="System.InvalidOperationException">An error occurred during serialization. The original exception is available
        /// using the <see cref="System.Exception.InnerException"/> property.</exception>
        public void Save(Stream stream, XmlWriterSettings settings) { this.Save(stream, settings, this.Location); }

        /// <summary>
        /// Serializes the current <typeparamref name="T"/> as XML, using the specified <see cref="System.IO.Stream"/> and
        /// <see cref="System.Xml.XmlWriterSettings"/> objects.
        /// </summary>
        /// <param name="stream">The <see cref="System.IO.Stream"/> used to write the XML document.</param>
        /// <param name="settings">The <see cref="System.Xml.XmlWriterSettings"/> object used to configure the XML writer.</param>
        /// <param name="location">The URI representing the source location of the document's serialized data.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="stream"/> is null.</exception>
        /// <exception cref="System.ArgumentException"><paramref name="stream"/> is not writable.</exception>
        /// <exception cref="System.InvalidOperationException">An error occurred during serialization. The original exception is available
        /// using the <see cref="System.Exception.InnerException"/> property.</exception>
        public void Save(Stream stream, XmlWriterSettings settings, Uri location)
        {
            settings = (settings == null) ? new XmlWriterSettings() : settings.Clone();
            settings.CloseOutput = false;
            using (XmlWriter xmlWriter = XmlWriter.Create(stream, settings))
            {
                this.Save(xmlWriter, location);
                xmlWriter.Flush();
            }
        }

        /// <summary>
        /// Serializes the current <typeparamref name="T"/> as XML, using the specified <see cref="System.IO.Stream"/> and encoding,
        /// with the default buffer size.
        /// </summary>
        /// <param name="stream">The <see cref="System.IO.Stream"/> used to write the XML document.</param>
        /// <param name="encoding">The character encoding to use.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="stream"/> or <paramref name="encoding"/> is null.</exception>
        /// <exception cref="System.ArgumentException"><paramref name="stream"/> is not writable.</exception>
        /// <exception cref="System.InvalidOperationException">An error occurred during serialization. The original exception is available
        /// using the <see cref="System.Exception.InnerException"/> property.</exception>
        public void Save(Stream stream, Encoding encoding) { this.Save(stream, encoding, this.Location); }

        /// <summary>
        /// Serializes the current <typeparamref name="T"/> as XML, using the specified <see cref="System.IO.Stream"/> and encoding,
        /// with the default buffer size.
        /// </summary>
        /// <param name="stream">The <see cref="System.IO.Stream"/> used to write the XML document.</param>
        /// <param name="encoding">The character encoding to use.</param>
        /// <param name="location">The URI representing the source location of the document's serialized data.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="stream"/> or <paramref name="encoding"/> is null.</exception>
        /// <exception cref="System.ArgumentException"><paramref name="stream"/> is not writable.</exception>
        /// <exception cref="System.InvalidOperationException">An error occurred during serialization. The original exception is available
        /// using the <see cref="System.Exception.InnerException"/> property.</exception>
        public void Save(Stream stream, Encoding encoding, Uri location)
        {
            if (this.BufferSize.HasValue)
                this.Save(stream, encoding, this.BufferSize.Value, location);
            else
            {
                using (StreamWriter streamWriter = new StreamWriter(stream, encoding))
                    this.Save(streamWriter, location);
            }
        }

        /// <summary>
        /// Serializes the current <typeparamref name="T"/> as XML, using the specified <see cref="System.IO.Stream"/>, encoding,
        /// and buffer size.
        /// </summary>
        /// <param name="stream">The <see cref="System.IO.Stream"/> used to write the XML document.</param>
        /// <param name="encoding">The character encoding to use.</param>
        /// <param name="bufferSize">Sets the buffer size.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="stream"/> or <paramref name="encoding"/> is null.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException"><paramref name="stream"/> is not writable.</exception>
        /// <exception cref="System.ArgumentException"><paramref name="bufferSize"/> is negative.</exception>
        /// <exception cref="System.InvalidOperationException">An error occurred during serialization. The original exception is available
        /// using the <see cref="System.Exception.InnerException"/> property.</exception>
        public void Save(Stream stream, Encoding encoding, int bufferSize)
        {
            this.Save(stream, encoding, bufferSize, this.Location);
            this.BufferSize = bufferSize;
        }

        /// <summary>
        /// Serializes the current <typeparamref name="T"/> as XML, using the specified <see cref="System.IO.Stream"/>, encoding,
        /// and buffer size.
        /// </summary>
        /// <param name="stream">The <see cref="System.IO.Stream"/> used to write the XML document.</param>
        /// <param name="encoding">The character encoding to use.</param>
        /// <param name="bufferSize">Sets the buffer size.</param>
        /// <param name="location">The URI representing the source location of the document's serialized data.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="stream"/> or <paramref name="encoding"/> is null.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException"><paramref name="stream"/> is not writable.</exception>
        /// <exception cref="System.ArgumentException"><paramref name="bufferSize"/> is negative.</exception>
        /// <exception cref="System.InvalidOperationException">An error occurred during serialization. The original exception is available
        /// using the <see cref="System.Exception.InnerException"/> property.</exception>
        public void Save(Stream stream, Encoding encoding, int bufferSize, Uri location)
        {
            using (StreamWriter streamWriter = new StreamWriter(stream, encoding, bufferSize))
                this.Save(streamWriter, location);
            this.BufferSize = bufferSize;
        }

        #endregion

        #region Save to string

        /// <summary>
        /// Serializes the current <typeparamref name="T"/> as XML, to the specified <see cref="System.Text.StringBuilder"/>.
        /// </summary>
        /// <param name="output">The <see cref="System.Text.StringBuilder"/> to write the XML to.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="output"/> is null.</exception>
        public void Save(StringBuilder output) { this.Save(output, this.Location); }

        /// <summary>
        /// Serializes the current <typeparamref name="T"/> as XML, to the specified <see cref="System.Text.StringBuilder"/>.
        /// </summary>
        /// <param name="output">The <see cref="System.Text.StringBuilder"/> to write the XML to.</param>
        /// <param name="location">Location to save to.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="output"/> is null.</exception>
        public void Save(StringBuilder output, Uri location)
        {
            if (this.WriterSettings != null)
            {
                this.Save(output, this.WriterSettings, location);
                return;
            }

            if (this.CheckCharacters.HasValue || this.ConformanceLevel.HasValue)
            {
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.CheckCharacters = this.CheckCharacters.Value;
                settings.ConformanceLevel = this.ConformanceLevel.Value;
                this.Save(output, settings, location);
                return;
            }

            using (XmlWriter xmlWriter = XmlWriter.Create(output))
            {
                this.Save(xmlWriter, location);
                xmlWriter.Flush();
            }
        }

        /// <summary>
        /// Serializes the current <typeparamref name="T"/> as XML, to the specified <see cref="System.Text.StringBuilder"/>,
        /// using the specified <see cref="System.Xml.XmlWriterSettings"/>.
        /// </summary>
        /// <param name="output">The <see cref="System.Text.StringBuilder"/> to write the XML to.</param>
        /// <param name="settings">The <see cref="System.Xml.XmlWriterSettings"/> object used to configure the XML writer.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="output"/> is null.</exception>
        public void Save(StringBuilder output, XmlWriterSettings settings) { this.Save(output, settings, this.Location); }

        /// <summary>
        /// Serializes the current <typeparamref name="T"/> as XML, to the specified <see cref="System.Text.StringBuilder"/>,
        /// using the specified <see cref="System.Xml.XmlWriterSettings"/>.
        /// </summary>
        /// <param name="output">The <see cref="System.Text.StringBuilder"/> to write the XML to.</param>
        /// <param name="settings">The <see cref="System.Xml.XmlWriterSettings"/> object used to configure the XML writer.</param>
        /// <param name="location">Location to save to.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="output"/> is null.</exception>
        public void Save(StringBuilder output, XmlWriterSettings settings, Uri location)
        {
            using (XmlWriter xmlWriter = XmlWriter.Create(output, settings))
            {
                this.Save(xmlWriter, location);
                xmlWriter.Flush();
            }
        }

        #endregion

        #region Save to path

        /// <summary>
        /// Serializes the current <typeparamref name="T"/> as XML, to the specified <paramref name="path"/>.
        /// </summary>
        /// <param name="path">The complete file path to write to.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="path"/> is null.</exception>
        /// <exception cref="System.UnauthorizedAccessException">Access is denied.</exception>
        /// <exception cref="System.IO.DirectoryNotFoundException">The specified <paramref name="path"/> is invalid, such as being on an unmapped drive.</exception>
        /// <exception cref="System.IO.IOException"><paramref name="path"/> includes an incorrect or invalid syntax for file name, directory name, or
        /// volume label syntax.</exception>
        /// <exception cref="System.IO.PathTooLongException">The specified <paramref name="path"/>, file name, or both exceed the system-defined maximum length.
        /// For example, on Windows-based platforms, paths must be less than 248 characters, and file names must be less than 260 characters.</exception>
        /// <exception cref="System.Security.SecurityException">The caller does not have the required permission.</exception>
        /// <exception cref="System.InvalidOperationException">An error occurred during serialization. The original exception is available
        /// using the <see cref="System.Exception.InnerException"/> property.</exception>
        public void Save(string path)
        {
            if (path == null)
                throw new ArgumentNullException("path");

            if (this.WriterSettings != null)
            {
                this.Save(path, this.WriterSettings);
                return;
            }

            if (this.Encoding != null)
            {
                if (this.BufferSize.HasValue)
                    this.Save(path, this.Append, this.Encoding, this.BufferSize.Value);
                else
                    this.Save(path, this.Append, this.Encoding);
                return;
            }

            if (this.CheckCharacters.HasValue || this.ConformanceLevel.HasValue)
            {
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.CheckCharacters = this.CheckCharacters.Value;
                settings.ConformanceLevel = this.ConformanceLevel.Value;
                this.Save(path, settings);
                return;
            }

            Uri inputUri;
            if (!Uri.TryCreate(path, UriKind.RelativeOrAbsolute, out inputUri))
                throw new UriFormatException("Invalid path or URI string.");

            using (XmlWriter xmlWriter = XmlWriter.Create(path))
            {
                this.Save(xmlWriter);
                xmlWriter.Flush();
            }
        }

        /// <summary>
        /// Serializes the current <typeparamref name="T"/> as XML, to the specified <paramref name="path"/>,
        /// using the specified <see cref="System.Xml.XmlWriterSettings"/>.
        /// </summary>
        /// <param name="path">The complete file path to write to.</param>
        /// <param name="settings">The <see cref="System.Xml.XmlWriterSettings"/> object used to configure the XML writer.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="path"/> is null.</exception>
        /// <exception cref="System.UnauthorizedAccessException">Access is denied.</exception>
        /// <exception cref="System.IO.DirectoryNotFoundException">The specified <paramref name="path"/> is invalid, such as being on an unmapped drive.</exception>
        /// <exception cref="System.IO.IOException"><paramref name="path"/> includes an incorrect or invalid syntax for file name, directory name, or
        /// volume label syntax.</exception>
        /// <exception cref="System.IO.PathTooLongException">The specified <paramref name="path"/>, file name, or both exceed the system-defined maximum length.
        /// For example, on Windows-based platforms, paths must be less than 248 characters, and file names must be less than 260 characters.</exception>
        /// <exception cref="System.Security.SecurityException">The caller does not have the required permission.</exception>
        /// <exception cref="System.InvalidOperationException">An error occurred during serialization. The original exception is available
        /// using the <see cref="System.Exception.InnerException"/> property.</exception>
        public void Save(string path, XmlWriterSettings settings)
        {
            using (XmlWriter xmlWriter = XmlWriter.Create(path, settings))
            {
                this.Save(xmlWriter);
                xmlWriter.Flush();
            }
        }

        /// <summary>
        /// Serializes the current <typeparamref name="T"/> as XML, to the specified <paramref name="path"/>,
        /// using the default encoding and buffer size. If the file exists, it can be either overwritten or appended to. If the file does not
        /// exist, this constructor creates a new file.
        /// </summary>
        /// <param name="path">The complete file path to write to.</param>
        /// <param name="append">Determines whether data is to be appended to the file. If the file exists and append is false, the file is overwritten.
        /// If the file exists and append is true, the data is appended to the file. Otherwise, a new file is created.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="path"/> is null.</exception>
        /// <exception cref="System.UnauthorizedAccessException">Access is denied.</exception>
        /// <exception cref="System.IO.DirectoryNotFoundException">The specified <paramref name="path"/> is invalid, such as being on an unmapped drive.</exception>
        /// <exception cref="System.IO.IOException"><paramref name="path"/> includes an incorrect or invalid syntax for file name, directory name, or
        /// volume label syntax.</exception>
        /// <exception cref="System.IO.PathTooLongException">The specified <paramref name="path"/>, file name, or both exceed the system-defined maximum length.
        /// For example, on Windows-based platforms, paths must be less than 248 characters, and file names must be less than 260 characters.</exception>
        /// <exception cref="System.Security.SecurityException">The caller does not have the required permission.</exception>
        /// <exception cref="System.InvalidOperationException">An error occurred during serialization. The original exception is available
        /// using the <see cref="System.Exception.InnerException"/> property.</exception>
        public void Save(string path, bool append)
        {
            if (this.Encoding != null)
            {
                if (this.BufferSize.HasValue)
                    this.Save(path, append, this.Encoding, this.BufferSize.Value);
                else
                    this.Save(path, append, this.Encoding);
                return;
            }

            using (StreamWriter streamWriter = new StreamWriter(path, append))
                this.Save(streamWriter);
            this.Append = append;
        }

        /// <summary>
        /// Serializes the current <typeparamref name="T"/> as XML, to the specified <paramref name="path"/>,
        /// using the specified encoding and default buffer size. If the file exists, it can be either overwritten or appended to. If the file
        /// does not exist, this constructor creates a new file.
        /// </summary>
        /// <param name="path">The complete file path to write to.</param>
        /// <param name="append">Determines whether data is to be appended to the file. If the file exists and append is false, the file is overwritten.
        /// If the file exists and append is true, the data is appended to the file. Otherwise, a new file is created.</param>
        /// <param name="encoding">The character encoding to use.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="path"/> or <paramref name="encoding"/> is null.</exception>
        /// <exception cref="System.ArgumentException"><paramref name="path"/> is empty.
        /// <para>-or-</para>
        /// <para><paramref name="path"/> contains the name of a system device (com1, com2, etc).</para></exception>
        /// <exception cref="System.UnauthorizedAccessException">Access is denied.</exception>
        /// <exception cref="System.IO.DirectoryNotFoundException">The specified <paramref name="path"/> is invalid, such as being on an unmapped drive.</exception>
        /// <exception cref="System.IO.IOException"><paramref name="path"/> includes an incorrect or invalid syntax for file name, directory name, or
        /// volume label syntax.</exception>
        /// <exception cref="System.IO.PathTooLongException">The specified <paramref name="path"/>, file name, or both exceed the system-defined maximum length.
        /// For example, on Windows-based platforms, paths must be less than 248 characters, and file names must be less than 260 characters.</exception>
        /// <exception cref="System.Security.SecurityException">The caller does not have the required permission.</exception>
        /// <exception cref="System.InvalidOperationException">An error occurred during serialization. The original exception is available
        /// using the <see cref="System.Exception.InnerException"/> property.</exception>
        public void Save(string path, bool append, Encoding encoding)
        {
            if (this.BufferSize.HasValue)
            {
                this.Save(path, append, this.Encoding, this.BufferSize.Value);
                return;
            }

            using (StreamWriter streamWriter = new StreamWriter(path, append, encoding))
                this.Save(streamWriter);
            this.Append = append;
        }

        /// <summary>
        /// Serializes the current <typeparamref name="T"/> as XML, to the specified <paramref name="path"/>,
        /// using the specified encoding and buffer size. If the file exists, it can be either overwritten or appended to. If the file does
        /// not exist, this constructor creates a new file.
        /// </summary>
        /// <param name="path">The complete file path to write to.</param>
        /// <param name="append">Determines whether data is to be appended to the file. If the file exists and append is false, the file is overwritten.
        /// If the file exists and append is true, the data is appended to the file. Otherwise, a new file is created.</param>
        /// <param name="encoding">The character encoding to use.</param>
        /// <param name="bufferSize">Sets the buffer size.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="path"/> or <paramref name="encoding"/> is null.</exception>
        /// <exception cref="System.ArgumentException"><paramref name="path"/> is empty.
        /// <para>-or-</para>
        /// <para><paramref name="path"/> contains the name of a system device (com1, com2, etc).</para></exception>
        /// <exception cref="System.UnauthorizedAccessException">Access is denied.</exception>
        /// <exception cref="System.IO.DirectoryNotFoundException">The specified <paramref name="path"/> is invalid, such as being on an unmapped drive.</exception>
        /// <exception cref="System.IO.IOException"><paramref name="path"/> includes an incorrect or invalid syntax for file name, directory name, or
        /// volume label syntax.</exception>
        /// <exception cref="System.IO.PathTooLongException">The specified <paramref name="path"/>, file name, or both exceed the system-defined maximum length.
        /// For example, on Windows-based platforms, paths must be less than 248 characters, and file names must be less than 260 characters.</exception>
        /// <exception cref="System.Security.SecurityException">The caller does not have the required permission.</exception>
        /// <exception cref="System.InvalidOperationException">An error occurred during serialization. The original exception is available
        /// using the <see cref="System.Exception.InnerException"/> property.</exception>
        public void Save(string path, bool append, Encoding encoding, int bufferSize)
        {
            using (StreamWriter streamWriter = new StreamWriter(path, append, encoding, bufferSize))
                this.Save(streamWriter);
            this.Append = append;
            this.BufferSize = bufferSize;
        }

        #endregion

        #endregion
    }
}
