using System;
using System.Xml;

namespace WpfCLR
{
	public class XamlLoadResult
	{
		private Exception _error;
		private XmlDocument _xmlDocument;
		private object _rootObject;
		private int _lineNumber;
		private int _linePosition;
		
		public Exception Error { get { return _error; } }
		
		public XmlDocument XmlDocument { get { return _xmlDocument; } }
		
		public object RootObject { get { return _rootObject; } }
		
		public int LineNumber { get { return _lineNumber; } }
		
		public int LinePosition { get { return _linePosition; } }
		
		public bool Success { get { return _error == null && _rootObject != null; } }
		
		public XamlLoadResult(Exception error, XmlDocument xmlDocument, object rootObject, int lineNumber, int linePosition)
		{
			_error = error;
			_xmlDocument = xmlDocument;
			_rootObject = rootObject;
			_lineNumber = lineNumber;
			_linePosition = linePosition;
		}
		
		public XamlLoadResult(Exception error, XmlDocument xmlDocument, int lineNumber, int linePosition) : this(exception, xmlDocument, null, lineNumber, linePosition) { }
		
		public XamlLoadResult(Exception error, XmlDocument xmlDocument) : this(exception, xmlDocument, 0, 0) { }
		
		public XamlLoadResult(XmlDocument xmlDocument, object rootObject) : this(exception, xmlDocument, rootObject, 1, 1) { }
		
		public override string ToString()
		{
			if (_error == null)
			return (_rootObject == null) ? "Nothing loaded." : String.Format("{0} successfully loaded.", _rootObject.GetType().FullName);
		
			if (_lineNumber < 1)
				return (_error.Message == null || _error.Message.Trim().Length == 0) ? String.Format("An unexpected {0} has occurred.", _error.GetType().FullName) :
					String.Format("Load error: {0}", _error.Message);
			
			if (_linePosition < 1)
				return (_error.Message == null || _error.Message.Trim().Length == 0) ? String.Format("An unexpected {0} has occurred on line {1}.", _error.GetType().FullName, _lineNumber) :
					String.Format("Load error on line {0}: {1}", _lineNumber, _error.Message);
					
			return (_error.Message == null || _error.Message.Trim().Length == 0) ? String.Format("An unexpected {0} has occurred on line {1}, position {2}.", _error.GetType().FullName, _lineNumber, _linePosition) :
				String.Format("Load error on line {0}, position {1}: {2}", _lineNumber, _linePosition, _error.Message);
		}
		
		public override int GetHashCode()
		{
			if (_xmlDocument != null)
				return _xmlDocument.GetHashCode();
			
			if (_rootObject != null)
				return _rootObject.GetHashCode();
			
			return (_error != null) ? _error.GetHashCode() : 0;
		}
	}
}