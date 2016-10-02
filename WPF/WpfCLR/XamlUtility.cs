using System;
using System.IO;
using System.Management.Automation;
using System.Windows;
using System.Windows.Markup;
using System.Xml;
using System.Xml.Schema;

namespace WpfCLR
{
	public static class XamlUtility
	{
        /// <summary>
        /// Namespace for WPF markup.
        /// </summary>
		public const string XmlNamespaceURI_Presentation = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";
		
        /// <summary>
        /// Namespace for XAML markup.
        /// </summary>
		public const string XmlNamespaceURI_Xaml = "http://schemas.microsoft.com/winfx/2006/xaml";
		
        /// <summary>
        /// Namespace for XML markup.
        /// </summary>
		public const string XmlNamespaceURI_Xml = "http://www.w3.org/2000/xmlns/";
		
        /// <summary>
        /// XAML markup for an empty WPF window.
        /// </summary>
		public const string Xaml_EmptyWindow = @"<Window xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"" xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
</Window>";

		public static XmlDocument CreateXamlMarkup(string rootElementName, string xamlName)
		{
			if (rootNodeName == null)
				throw new ArgumentNullException("rootNodeName");
			
			XmlDocument xmlDocument = new XmlDocument();
			try { xmlDocument.AppendChild(xmlDocument.CreateElement(rootNodeName, XmlNamespaceURI_Presentation)); }
			catch (Exception exception) { throw new ArgumentException("Invalid root element name.", "rootNodeName", exception); }
			xmlDocument.DocumentElement.Attributes.Append(xmlDocument.CreateAttribute("x", XmlNamespaceURI_Xml)).Value = XmlNamespaceURI_Xaml;
			if (xamlName != null)
			{
				try { xmlDocument.DocumentElement.Attributes.Append(xmlDocument.CreateAttribute("name", XmlNamespaceURI_Xaml)).Value = xamlName; }
				catch (Exception exception) { throw new ArgumentException("Invalid XAML name.", "xamlName", exception); }
			}
				
			return xmlDocument;
		}
		
		public static XmlDocument LoadXmlDocument(XmlReader xmlReader)
		{
			if (xmlReader == null)
				throw new ArgumentNullException("xmlReader");
			
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.Load(xmlReader);
			return xmlDocument;
		}
		
		public static XmlDocument LoadXmlDocument(Stream stream, XmlReaderSettings xmlReaderSettings)
		{
			if (stream == null)
				throw new ArgumentNullException("stream");
			
			using (XmlReader xmlReader = XmlReader.Create(stream, xmlReaderSettings))
				return LoadXmlDocument(xmlReader);
		}
		
		public static XmlDocument LoadXmlDocument(Stream stream)
		{
			if (stream == null)
				throw new ArgumentNullException("stream");
			
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.Load(stream);
			return xmlDocument;
		}
		
		public XmlDocument LoadXmlDocument(TextReader textReader, XmlReaderSettings xmlReaderSettings)
		{
			if (textReader == null)
				throw new ArgumentNullException("textReader");
			
			using (XmlReader xmlReader = XmlReader.Create(textReader, xmlReaderSettings))
				return LoadXmlDocument(xmlReader);
		}
		
		public XmlDocument LoadXmlDocument(TextReader textReader)
		{
			if (textReader == null)
				throw new ArgumentNullException("textReader");
			
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.Load(textReader);
			return xmlDocument;
		}
		
		public XmlDocument ParseXmlDocument(string xmlText, XmlReaderSettings xmlReaderSettings)
		{
			if (xmlText == null)
				throw new ArgumentNullException("xmlText");
			
			using (StringReader stringReader = new StringReader(xmlText))
				return LoadXmlDocument(stringReader, xmlReaderSettings);
		}
		
		public XmlDocument ParseXmlDocument(string xmlText)
		{
			if (xmlText == null)
				throw new ArgumentNullException("xmlText");
			
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(xmlText);
			return xmlDocument;
		}
		
		public static bool TryLoadXaml(XmlDocument xmlDocument, out XamlLoadResult result)
		{
			try
			{
				if (xmlDocument == null)
					throw new ArgumentNullException("xmlDocument");
				
				using (XmlNodeReader reader = new XmlNodeReader(xmlDocument))
					result = new XamlLoadResult(xmlDocument, XamlReader.Load(xmlNodeReader));
			}
			catch (XamlParseException exception)
			{
				result = new XamlLoadResult(xmlDocument, exception, exception.LineNumber, exception.LinePosition);
			}
			catch (XmlSchemaException exception)
			{
				result = new XamlLoadResult(xmlDocument, exception, exception.LineNumber, exception.LinePosition);
			}
			catch (XmlException exception)
			{
				result = new XamlLoadResult(xmlDocument, exception, exception.LineNumber, exception.LinePosition);
			}
			catch (Exception exception)
			{
				result = new XamlLoadResult(xmlDocument, exception);
			}
			
			return result.Success;
		}
		
		public static bool TryLoadXaml(Stream stream, XmlReaderSettings xmlReaderSettings, out XamlLoadResult result)
		{
			try
			{
				if (stream == null)
					throw new ArgumentNullException("stream");
				
				TryLoadXaml(LoadXmlDocument(stream, xmlReaderSettings), out result);
			}
			catch (XmlSchemaException exception)
			{
				result = new XamlLoadResult(xmlDocument, exception, exception.LineNumber, exception.LinePosition);
			}
			catch (XmlException exception)
			{
				result = new XamlLoadResult(xmlDocument, exception, exception.LineNumber, exception.LinePosition);
			}
			catch (Exception exception)
			{
				result = new XamlLoadResult(xmlDocument, exception);
			}
			
			return result.Success;
		}
		
		public static bool TryLoadXaml(Stream stream, out XamlLoadResult result)
		{
			try
			{
				if (stream == null)
					throw new ArgumentNullException("stream");
				
				TryLoadXaml(LoadXmlDocument(stream), out result);
			}
			catch (XmlSchemaException exception)
			{
				result = new XamlLoadResult(xmlDocument, exception, exception.LineNumber, exception.LinePosition);
			}
			catch (XmlException exception)
			{
				result = new XamlLoadResult(xmlDocument, exception, exception.LineNumber, exception.LinePosition);
			}
			catch (Exception exception)
			{
				result = new XamlLoadResult(xmlDocument, exception);
			}
			
			return result.Success;
		}
		
		public static bool TryLoadXaml(TextReader textReader, XmlReaderSettings xmlReaderSettings, out XamlLoadResult result)
		{
			try
			{
				if (textReader == null)
					throw new ArgumentNullException("textReader");
				
				TryLoadXaml(LoadXmlDocument(textReader, xmlReaderSettings), out result);
			}
			catch (XmlSchemaException exception)
			{
				result = new XamlLoadResult(xmlDocument, exception, exception.LineNumber, exception.LinePosition);
			}
			catch (XmlException exception)
			{
				result = new XamlLoadResult(xmlDocument, exception, exception.LineNumber, exception.LinePosition);
			}
			catch (Exception exception)
			{
				result = new XamlLoadResult(xmlDocument, exception);
			}
			
			return result.Success;
		}
		
		public static bool TryLoadXaml(TextReader textReader, out XamlLoadResult result)
		{
			try
			{
				if (textReader == null)
					throw new ArgumentNullException("textReader");
				
				TryLoadXaml(LoadXmlDocument(textReader), out result);
			}
			catch (XmlSchemaException exception)
			{
				result = new XamlLoadResult(xmlDocument, exception, exception.LineNumber, exception.LinePosition);
			}
			catch (XmlException exception)
			{
				result = new XamlLoadResult(xmlDocument, exception, exception.LineNumber, exception.LinePosition);
			}
			catch (Exception exception)
			{
				result = new XamlLoadResult(xmlDocument, exception);
			}
			
			return result.Success;
		}
		
		public static bool TryParseXaml(string xamlText, XmlReaderSettings xmlReaderSettings, out XamlLoadResult result)
		{
			try
			{
				if (xamlText == null)
					throw new ArgumentNullException("xamlText");
				
				TryLoadXaml(ParseXmlDocument(xamlText, xmlReaderSettings), out result);
			}
			catch (XmlSchemaException exception)
			{
				result = new XamlLoadResult(xmlDocument, exception, exception.LineNumber, exception.LinePosition);
			}
			catch (XmlException exception)
			{
				result = new XamlLoadResult(xmlDocument, exception, exception.LineNumber, exception.LinePosition);
			}
			catch (Exception exception)
			{
				result = new XamlLoadResult(xmlDocument, exception);
			}
			
			return result.Success;
		}
		
		public static bool TryParseXaml(string xamlText, out XamlLoadResult result)
		{
			try
			{
				if (xamlText == null)
					throw new ArgumentNullException("xamlText");
				
				TryLoadXaml(ParseXmlDocument(xamlText, xmlReaderSettings), out result);
			}
			catch (XmlSchemaException exception)
			{
				result = new XamlLoadResult(xmlDocument, exception, exception.LineNumber, exception.LinePosition);
			}
			catch (XmlException exception)
			{
				result = new XamlLoadResult(xmlDocument, exception, exception.LineNumber, exception.LinePosition);
			}
			catch (Exception exception)
			{
				result = new XamlLoadResult(xmlDocument, exception);
			}
			
			return result.Success;
		}
		
		public static bool TryLoadXaml(XmlReader xmlReader, out XamlLoadResult result)
		{
			try
			{
				if (xmlReader == null)
					throw new ArgumentNullException("xmlReader");
				
				TryLoadXaml(LoadXmlDocument(xmlReader), out result);
			}
			catch (XmlSchemaException exception)
			{
				result = new XamlLoadResult(xmlDocument, exception, exception.LineNumber, exception.LinePosition);
			}
			catch (XmlException exception)
			{
				result = new XamlLoadResult(xmlDocument, exception, exception.LineNumber, exception.LinePosition);
			}
			catch (Exception exception)
			{
				result = new XamlLoadResult(xmlDocument, exception);
			}
			
			return result.Success;
		}
	}
}