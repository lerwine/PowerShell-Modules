using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Host;
using System.Management.Automation.Runspaces;
using System.Windows;
using System.Windows.Controls;

namespace WpfCLR
{
	/// <summary>
	/// Context object for scripts invoked while the XAML window is being displayed.
	/// </summary>
    public class ThisObj
    {
        #region Fields
		private WpfWindow_obsolete _windowObj;
		private GetMainWindowHandler _getMainWindow;
		private ReadOnlyDictionary<string, object> _namedElements = null;
		private PSHost _host;
		#endregion
		
		#region Properties
		
        /// <summary>
        /// Named elements (control) detected from XAML markup.
        /// </summary>
		/// <remarks>When the <see cref="WpfWindow_obsolete.BeforeWindowCreated"/> <see cref="ScriptBlock"/> is invoked, the values will be the corresponding elements in the XAML markup.</remarks>
		public ReadOnlyDictionary<string, object> NamedElements { get { return _namedElements; } }
		
        /// <summary>
        /// Main WPF window being displayed.
        /// </summary>
		/// <remarks>This will be null if the window hasn't been created yet, or if there was an error while trying to create the window</remarks>
		public Window MainWindow { get { return _getMainWindow(); } }
		
        /// <summary>
        /// Data which will be synchronized with the <see cref="WpfWindow_obsolete.SynchronizedData" /> property of the object responsible for creating the WPF window.
        /// </summary>
		public System.Collections.Hashtable SynchronizedData { get { return _windowObj.SynchronizedData; } }
		
        /// <summary>
        /// Collection which contains the resulting output from displaying the WPF window.
        /// </summary>
		public Collection<PSObject> Output { get { return _windowObj.Output; } }
		
        /// <summary>
        /// Collection which contains the resulting error records generated while displaying the WPF window.
        /// </summary>
		public Collection<ErrorRecord> ErrorRecords { get { return _windowObj.ErrorRecords; } }
		
        /// <summary>
        /// Collection which contains the resulting warning messages generated while displaying the WPF window.
        /// </summary>
		public Collection<WarningRecord> WarningRecords { get { return _windowObj.WarningRecords; } }
		
        /// <summary>
        /// Collection which contains the resulting verbose messages generated while displaying the WPF window.
        /// </summary>
		public Collection<VerboseRecord> VerboseRecords { get { return _windowObj.VerboseRecords; } }
		
        /// <summary>
        /// Collection which contains the resulting debug messages generated while displaying the WPF window.
        /// </summary>
		public Collection<DebugRecord> DebugRecords { get { return _windowObj.DebugRecords; } }
		
		#endregion
		
		#region Constructors
		
		internal ThisObj(PSHost host, WpfWindow_obsolete windowObj, Dictionary<string, object> namedElements, GetMainWindowHandler getMainWindow)
		{
			if (windowObj == null)
				throw new ArgumentNullException("windowObj");
			
			if (namedElements == null)
				throw new ArgumentNullException("namedElements");
			
			if (getMainWindow == null)
				throw new ArgumentNullException("getMainWindow");
			
			_host = host;
			_windowObj = windowObj;
			_namedElements = new ReadOnlyDictionary<string, object>(namedElements);
			_getMainWindow = getMainWindow;
		}
		
		#endregion
		
		internal class EventAttachment
		{
			private ScriptBlock _scriptBlock;
			private PSHost _host;
			private WpfWindow_obsolete _windowObj;
			private EventAttachment() { }
			
			public static EventAttachment AttachButtonClick(Button button, ScriptBlock scriptBlock, WpfWindow_obsolete windowObj, PSHost host)
			{
				EventAttachment eventAttachment = new EventAttachment();
				eventAttachment._scriptBlock = scriptBlock;
				eventAttachment._windowObj = windowObj;
				eventAttachment._host = host;
				button.Click += new RoutedEventHandler(eventAttachment.RoutedEvent_Invoked);
				return eventAttachment;
			}
			
			private void RoutedEvent_Invoked(object sender, RoutedEventArgs e) { Invoke(sender, e); }
			
			private void Invoke(object sender, object eventArgs)
			{
				Runspace runspace;
				try
				{
					if (_host == null)
						runspace = RunspaceFactory.CreateRunspace();
					else
						runspace = RunspaceFactory.CreateRunspace(_host);
					
					using (runspace)
					{
						runspace.ApartmentState = System.Threading.ApartmentState.STA;
						runspace.ThreadOptions = PSThreadOptions.ReuseThread;
						runspace.Open();
						using (PowerShell powerShell = PowerShell.Create())
						{
							powerShell.Runspace = runspace;
							runspace.SessionStateProxy.SetVariable("this", this);
							runspace.SessionStateProxy.SetVariable("Sender", sender);
							runspace.SessionStateProxy.SetVariable("EventArgs", eventArgs);
							powerShell.AddCommand("Add-Type").AddParameter("AssemblyName", (typeof(Window)).Assembly.FullName);
							powerShell.AddCommand("Add-Type").AddParameter("AssemblyName", (typeof(UIElement)).Assembly.FullName);
							powerShell.AddCommand("Add-Type").AddParameter("AssemblyName", (typeof(DependencyObject)).Assembly.FullName);
							powerShell.AddCommand("Add-Type").AddParameter("Path", this.GetType().Assembly.Location);
							powerShell.AddScript(_scriptBlock.ToString());
							try { _windowObj.AddOutput(powerShell.Invoke()); }
							catch { throw; }
							finally { _windowObj.AddOutput(powerShell.Streams); }
						}
					}
				}
				catch (Exception exception)
				{
					_windowObj.AddError("Unexpected error invoking event handler", exception, "EventAttachment_Invoke");
				}
			}
		}
		#region Methods
		
		private Collection<EventAttachment> _eventAttachments = new Collection<EventAttachment>();
		
        /// <summary>
        /// Attach a script block to a button click event.
        /// </summary>
        /// <param name="button">The button to attache the click even to.</param>
        /// <param name="scriptBlock">The script block that will handle the click event.</param>
		/// <exception cref="ArgumentNullException"><paramref name="button"/> or <paramref name="scriptBlock"/> is null.</exception>
		public void AttachButtonClick(Button button, ScriptBlock scriptBlock)
		{
			if (button == null)
				throw new ArgumentNullException("button");
			
			if (scriptBlock == null)
				throw new ArgumentNullException("scriptBlock");
			
			_eventAttachments.Add(EventAttachment.AttachButtonClick(button, scriptBlock, _windowObj, _host));
		}
		
		#endregion
    }
	
	/// <summary>
	/// (internal) Handles event which creates the main WPF window.
	/// </summary>
	/// <returns>A new WPF <seealso cref="Window" />.</returns>
	public delegate Window GetMainWindowHandler();
}