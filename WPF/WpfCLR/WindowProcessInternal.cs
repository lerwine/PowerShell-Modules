using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Host;
using System.Management.Automation.Runspaces;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Threading;
using System.Xml;

namespace WpfCLR
{
    /// <summary>
    /// Internal class which manages the creation and invocation of the WPF window.
    /// </summary>
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
		private Exception _fault = null;
		
		#region Methods
		
		internal WindowProcessInternal(WpfWindow windowObj, XmlDocument windowXaml, bool showAsDialog, PSHost host)
		{
			#region Auto-detect named elements
			
			XmlNamespaceManager nsmgr = new XmlNamespaceManager(windowXaml.NameTable);
			nsmgr.AddNamespace("p", WpfWindow.XmlNamespaceURI_Presentation);
			nsmgr.AddNamespace("x", WpfWindow.XmlNamespaceURI_Xaml);
			
			foreach (XmlAttribute xmlAttribute in windowXaml.SelectNodes("//@x:Name", nsmgr))
			{
				if (!_namedElements.ContainsKey(xmlAttribute.Value))
					_namedElements.Add(xmlAttribute.Value, xmlAttribute.OwnerElement);
			}
			
			#endregion
			
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
					PowerShell command = _powerShell.AddCommand("Invoke-Command");
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
			_windowObj.AddOutput(output);
			
			if (_powerShell != null)
			_windowObj.AddOutput(_powerShell.Streams);
		
			return _windowObj.DialogResult;	
		}
		
        /// <summary>
        /// This gets invoked by an internally-generated script at a point when the main window is to be created.
        /// </summary>
		public void CreateMainWindow()
		{
			try
			{
				using (XmlNodeReader xmlNodeReader = new XmlNodeReader(_windowXaml))
					_mainWindow = (Window)(System.Windows.Markup.XamlReader.Load(xmlNodeReader));
			}
			catch (Exception exception)
			{
				_windowObj.AddError("Unexpected error while creating the main window", exception, "CreateMainWindow");
				_namedElements.Clear();
				return;
			}
			
#if PSLEGACY2
			string[] allKeys = (new List<string>(_namedElements.Keys)).ToArray();
#else
			string[] allKeys = _namedElements.Keys.ToArray();
#endif
			foreach (string name in allKeys)
			{
				object e;
				try { e = _mainWindow.FindName(name); } catch { e = null; }
				if (e == null) {
					_namedElements.Remove(name);
				} else {
					_namedElements[name] = e;
				}
			}
		}
		
        /// <summary>
        /// This gets invoked by an internally-generated script at a point when the main window is to be shown.
        /// </summary>
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
		
        /// <summary>
        /// Disposes this object and the associated PowerShell instance.
        /// </summary>
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