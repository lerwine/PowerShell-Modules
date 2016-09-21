using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Threading;
using System.Xml;

namespace WpfCLR
{
    public class WpfWindow
    {
		#region Fields
		
		public const string XmlNamespaceURI_Presentation = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";
		public const string XmlNamespaceURI_Xaml = "http://schemas.microsoft.com/winfx/2006/xaml";
		public const string Xaml_EmptyWindow = @"<Window xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"" xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
</Window>";
		private string _xml = "";
		private XmlDocument _windowXaml;
		private System.Collections.Hashtable _synchronizedData = System.Collections.Hashtable.Synchronized(new System.Collections.Hashtable());
		private ScriptBlock _beforeWindowCreated = null;
		private ScriptBlock _beforeWindowShown = null;
		private ScriptBlock _afterWindowClosed = null;
		private Collection<PSObject> _output = new Collection<PSObject>();
		private Collection<ErrorRecord> _errorRecords = new Collection<ErrorRecord>();
		private Collection<WarningRecord> _warningRecords = new Collection<WarningRecord>();
		private Collection<VerboseRecord> _verboseRecords = new Collection<VerboseRecord>();
		private Collection<DebugRecord> _debugRecords = new Collection<DebugRecord>();
		private bool? _dialogResult;
		
		#endregion
		
		#region Properties
		
		public System.Collections.Hashtable SynchronizedData { get { return _synchronizedData; } }
		public bool? DialogResult { get { return _dialogResult; } set { _dialogResult = value; } }
		public Collection<PSObject> Output { get { return _output; } }
		public Collection<ErrorRecord> ErrorRecords { get { return _errorRecords; } }
		public Collection<WarningRecord> WarningRecords { get { return _warningRecords; } }
		public Collection<VerboseRecord> VerboseRecords { get { return _verboseRecords; } }
		public Collection<DebugRecord> DebugRecords { get { return _debugRecords; } }
		
		public string WindowXaml
		{
			get { return _xml; }
			set
			{
				if (_openWindowProcess != null)
					throw new InvalidOperationException("Cannot change markup while window is open.");
				
				if (String.IsNullOrEmpty(value))
				{
					_xml = "";
					_windowXaml = null;
					return;
				}

				string xml = value.Trim();
				
				if (xml.Length == 0)
					throw new ArgumentException("XML Markup is empty.", "value");
				
				XmlDocument xmlDocument = new XmlDocument();
				
				try { xmlDocument.LoadXml(xml); }
				catch (Exception exception) { throw new ArgumentException("Invalid XML markup", "value", exception); }
				
				if (xmlDocument.DocumentElement == null)
					throw new ArgumentException("XML document is empty", "value");
				
				if (xmlDocument.DocumentElement.NamespaceURI != XmlNamespaceURI_Presentation)
					throw new ArgumentException("XML markup does not represent a WPF document.", "value");
				
				if (xmlDocument.DocumentElement.LocalName != "Window")
					throw new ArgumentException("XAML markup does not represent a window.", "value");
				_xml = value;
				_windowXaml = xmlDocument;
			}
		}
		
		public ScriptBlock BeforeWindowCreated
		{
			get { return _beforeWindowCreated; }
			set { _beforeWindowCreated = value; }
		}
		
		public ScriptBlock BeforeWindowShown
		{
			get { return _beforeWindowShown; }
			set { _beforeWindowShown = value; }
		}
		
		public ScriptBlock AfterWindowClosed
		{
			get { return _afterWindowClosed; }
			set { _afterWindowClosed = value; }
		}
		
		#endregion
		
		#region Methods
		
		public bool? ShowDialog(PSHost host)
		{
			using (WindowProcessInternal openWindowProcess  = new WindowProcessInternal(this, _windowXaml. host))
				return openWindowProcess.ShowDialog(host);
		}
		
		public bool? Show(PSHost host)
		{
			using (WindowProcessInternal openWindowProcess  = new WindowProcessInternal(this, _windowXaml. host))
				return openWindowProcess.Show(host);
		}
		
		#endregion
	}
}