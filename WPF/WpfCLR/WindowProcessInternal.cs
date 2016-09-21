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
    internal class WindowProcessInternal : IDisposable
    {
		private object _syncRoot = new object();
		private Runspace _runspace = null;
		private PowerShell _powerShell = null;
		private Dictionary<string, object> _namedElements = new Dictionary<string, object>();
		private ThisObj _thisObj;
		private WpfWindow _windowObj;
		private XmlDocument _windowXaml;
		private Window _mainWindow = null;
		private ManualResetEvent _windowClosedEvent = new ManualResetEvent(false);
		private bool? _dialogResult = null;
		
		#region Methods
		
		internal WindowProcessInternal(WpfWindow windowObj, XmlDocument windowXaml, PSHost host)
		{
			_windowObj = windowObj;
			_windowXaml = windowXaml;
			_thisObj = new ThisObj(host, windowObj, _namedElements, new GetMainWindowHandler(GetMainWindow));
			if (host == null)
				_runspace = RunspaceFactory.CreateRunspace();
			else
				_runspace = RunspaceFactory.CreateRunspace(host);
			
			try
			{
				_windowClosedEvent.Reset();
				_runspace.ApartmentState = ApartmentState.STA;
				_runspace.ThreadOptions = PSThreadOptions.ReuseThread;
				_runspace.Open();
				_powerShell = PowerShell.Create();
				try
				{
					_powerShell.Runspace = _runspace;
					_runspace.SessionStateProxy.SetVariable("this", this._thisObj);
					_powerShell.AddCommand("Add-Type").AddParameter("AssemblyName", (typeof(Window)).Assembly.FullName);
					_powerShell.AddCommand("Add-Type").AddParameter("AssemblyName", (typeof(UIElement)).Assembly.FullName);
					_powerShell.AddCommand("Add-Type").AddParameter("AssemblyName", (typeof(DependencyObject)).Assembly.FullName);
					_powerShell.AddCommand("Add-Type").AddParameter("Path", this.GetType().Assembly.Location);
					if (_windowObj.BeforeWindowCreated != null)
						_powerShell.AddScript(_windowObj.BeforeWindowCreated.ToString());
					Command command = _powerShell.AddCommand("Invoke-Command");
					command.AddParameter("ScriptBlock", ScriptBlock.Create(@"$this.Output += @($input);
$Args[0].CreateMainWindow();"));
					command.AddParameter("ArgumentList", this);
					if (_windowObj.BeforeWindowShown != null)
						_powerShell.AddScript(_windowObj.BeforeWindowShown.ToString());
					command = _powerShell.AddCommand("Invoke-Command");
					command.AddParameter("ScriptBlock", ScriptBlock.Create(@"$this.Output += @($input);
if ($this.MainWindow -ne $null) { $Args[0].ShowMainWindow($Args[1]) }"));
					command.AddParameter("ArgumentList", new object[] { this, showAsDialog });
					if (_windowObj.AfterWindowClosed != null)
						_powerShell.AddScript(_windowObj.AfterWindowClosed.ToString());
				}
				catch
				{
					_powerShell.Dispose();
					throw;
				}
			}
			catch
			{
				_runspace.Dispose();
				throw;
			}
		}

		private Window GetMainWindow() { return _mainWindow; }
		
		~WindowProcessInternal() { Dispose(false); }
		
		internal bool? ShowDialog()
		{
			try { return Finalize(_powerShell.Invoke()); }
			catch
			{
				Finalize(null);
				throw;
			}
		}
		
		internal bool? Show()
		{
			IAsyncResult asyncOperationStatus;
			try { asyncOperationStatus = _powerShell.BeginInvoke(); }
			catch
			{
				_windowClosedEvent.Set();
				Finalize(null);
				throw;
			}
			
			asyncOperationStatus.AsyncWaitHandle.WaitOne();
			
			try { return Finalize(_powerShell.EndInvoke(asyncOperationStatus).ReadAll()); }
			catch
			{
				Finalize(null);
				throw;
			}
			finally
			{
				asyncOperationStatus.AsyncWaitHandle.Close();
			}
		}
		
		private bool? Finalize(Collection<PSObject> output)
		{
			if (output != null)
			{
				foreach (PSObject obj in output)
					_windowObj.Output.Add(obj);
			}
			
			if (_powerShell == null)
				return _windowObj.DialogResult;	
			
			foreach (ErrorRecord obj in powerShell.Streams.Error.ReadAll())
				_windowObj.ErrorRecords.Add(obj);
			foreach (WarningRecord obj in powerShell.Streams.Warning.ReadAll())
				_windowObj.WarningRecords.Add(obj);
			foreach (VerboseRecord obj in powerShell.Streams.Verbose.ReadAll())
				_windowObj.VerboseRecords.Add(obj);
			foreach (DebugRecord obj in powerShell.Streams.Debug.ReadAll())
				_windowObj.DebugRecords.Add(obj);
		}
		
		public void CreateMainWindow()
		{
			using (XmlNodeReader xmlNodeReader = new XmlNodeReader(_windowXaml))
				_mainWindow = (Window)(System.Windows.Markup.XamlReader.Load(xmlNodeReader));
			
			XmlNamespaceManager nsmgr = new XmlNamespaceManager(_windowXaml.NameTable);
			nsmgr.AddNamespace("p", WpfWindow.XmlNamespaceURI_Presentation);
			nsmgr.AddNamespace("x", WpfWindow.XmlNamespaceURI_Xaml);
			foreach (XmlAttribute xmlAttribute in _windowXaml.SelectNodes("//@x:Name", nsmgr))
			{
				if (_namedElements.ContainsKey(xmlAttribute.Value))
					continue;
				
				object e;
				try { e = _mainWindow.FindName(xmlAttribute.Value); } catch { e = null; }
				_namedElements.Add(xmlAttribute.Value, e);
			}
		}
		
		public void ShowMainWindow(bool showAsDialog)
		{
			if (_mainWindow == null)
				return;
			
			_mainWindow.Closed += Window_Closed;
			
			if (showAsDialog)
				_windowObj.DialogResult = _mainWindow.ShowDialog();
			else
			{
				_mainWindow.Show();
				_windowClosedEvent.WaitOne();
				_windowObj.DialogResult = _mainWindow.DialogResult;
			}
		}
		
		private void Window_Closed(object sender, EventArgs e) { _windowClosedEvent.Set(); }
		
		#endregion
		
		#region IDisposable members
		
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
				return;
			
			if (_powerShell != null)
				_powerShell.Dispose();
			
			if (_runspace != null)
				_runspace.Dispose();
		}
		
		#endregion
	}
}