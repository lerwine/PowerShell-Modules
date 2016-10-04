cls

Add-Type -TypeDefinition @'
namespace PSDevToolsClr.Maml
{
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

    public class helpItemsDocument
    {
        public const string XmlNamespace_msh = "http://msh";
        public const string XmlNamespace_maml = "http://schemas.microsoft.com/maml/2004/10";
        public const string XmlNamespace_command = "http://schemas.microsoft.com/maml/dev/command/2004  /10";
        public const string XmlNamespace_dev = "http://schemas.microsoft.com/maml/dev/2004/10";
    }
    [XmlRoot("para", Namespace = helpItemsDocument.XmlNamespace_command)]
    public class paraElement : IXmlSerializable
    {
        private string _value = "";
        public string Value
        {
            get { return this._value; }
            set { this._value = (value == null) ? "" : value; }
        }

        private bool _isComment = false;
        public bool IsComment
        {
            get { return this._isComment; }
            set { this._isComment = value; }
        }

        public paraElement() { }

        public paraElement(string value) : this(value, false) { }

        public paraElement(string value, bool isComment)
        {
            this.Value = value;
            this.IsComment = isComment;
        }

        #region IXmlSerializable Members

        System.Xml.Schema.XmlSchema IXmlSerializable.GetSchema() { return null; }

        public void ReadXml(XmlReader reader)
        {
            reader.Read();
            this.IsComment = reader.NodeType == XmlNodeType.Comment;
            reader.ReadContentAsString();
        }

        public void WriteXml(XmlWriter writer)
        {
            if (this.IsComment)
                writer.WriteComment(this.Value);
            else
                writer.WriteString(this.Value);
        }

        #endregion
    }


    [XmlRoot("alertSet", Namespace = helpItemsDocument.XmlNamespace_maml)]
    public sealed class alertSetElement : Collection<alertSetElement.TitleAndAlert>, IXmlSerializable
    {
        public void Add(string title, params string[] alert)
        {
            TitleAndAlert item = new TitleAndAlert();
            item.title = title;

            if (alert == null)
                return;

            foreach (string a in alert)
                item.alert.Add(new paraElement(a));

            this.Add(item);
        }

        [XmlRoot("alertSet", Namespace = helpItemsDocument.XmlNamespace_maml)]
        public class TitleAndAlert
        {
            public static string Serialize(Collection<TitleAndAlert> collection)
            {
                string results;

                XmlSerializer serializer = new XmlSerializer(typeof(Collection<paraElement>));
                using (MemoryStream stream = new MemoryStream())
                {
                    XmlWriterSettings settings = new XmlWriterSettings();
                    settings.CloseOutput = false;
                    settings.Encoding = Encoding.UTF8;
                    using (XmlWriter xmlWriter = XmlTextWriter.Create(stream, settings))
                    {
                        serializer.Serialize(xmlWriter, collection);
                        xmlWriter.Flush();
                        results = settings.Encoding.GetString(stream.ToArray());
                    }
                }

                return results;
            }

            private string _title = "";
            [XmlElement("title", Namespace = helpItemsDocument.XmlNamespace_maml)]
            public string title
            {
                get { return this._title; }
                set { this._title = (value == null) ? "" : value; }
            }

            private Collection<paraElement> _alert = new Collection<paraElement>();
            [XmlArray("alert", Namespace = helpItemsDocument.XmlNamespace_maml)]
            [XmlArrayItem("para", Namespace = helpItemsDocument.XmlNamespace_maml)]
            public Collection<paraElement> alert
            {
                get { return this._alert; }
                set { this._alert = (value == null) ? new Collection<paraElement>() : value; }
            }
        }

        private string _testReadXml = "";

        public static string GetTestReadXml(string serializedXml)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(alertSetElement));
            alertSetElement result;
            using (MemoryStream stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(serializedXml.Trim())))
            {
                using (XmlReader xmlReader = XmlTextReader.Create(stream))
                    result = serializer.Deserialize(xmlReader) as alertSetElement;
            }

            return result._testReadXml;
        }

        #region IXmlSerializable Members

        System.Xml.Schema.XmlSchema IXmlSerializable.GetSchema() { return null; }

        public void ReadXml(XmlReader reader)
        {
            this._testReadXml = reader.ReadOuterXml();
        }

        public string GetXmlText()
        {
            XmlSerializer serializer = new XmlSerializer(this.GetType());
            string xmlText;
            using (MemoryStream stream = new MemoryStream())
            {
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.CloseOutput = false;
                settings.Encoding = Encoding.UTF8;
                using (XmlWriter xmlWriter = XmlTextWriter.Create(stream, settings))
                {
                    serializer.Serialize(xmlWriter, this);
                    xmlWriter.Flush();
                    xmlText = settings.Encoding.GetString(stream.ToArray());
                }
            }

            return xmlText;
        }

        public void WriteXml(XmlWriter writer)
        {
            Collection<alertSetElement.TitleAndAlert> copy = new Collection<TitleAndAlert>(this);
            XmlSerializer serializer = new XmlSerializer(copy.GetType());
            XmlDocument doc = new XmlDocument();
            using (MemoryStream stream = new MemoryStream())
            {
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.CloseOutput = false;
                settings.Encoding = Encoding.UTF8;
                using (XmlWriter xmlWriter = XmlTextWriter.Create(stream, settings))
                {
                    serializer.Serialize(xmlWriter, copy);
                    xmlWriter.Flush();
                }

                stream.Seek(0, SeekOrigin.Begin);
                doc.Load(stream);
            }

            foreach (XmlElement element in doc.SelectNodes("/ArrayOfTitleAndAlert/TitleAndAlert/*"))
                element.WriteTo(writer);
        }

        #endregion
    }
}
'@ -ReferencedAssemblies 'System.Xml';

$alertSetElement = New-Object -TypeName 'PSDevToolsClr.Maml.alertSetElement';
$alertSetElement.Add('My Title', 'one', 'two', 'three');
$alertSetElement.Add('Another Title', 'four', 'five', 'six');
$txt = $alertSetElement.GetXmlText();
$txt
[PSDevToolsClr.Maml.alertSetElement]::GetTestReadXml($txt);