using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

namespace WpfCLR
{
    public class ThisObj
    {
		#region Fields
		
		private WpfWindow _windowObj;
		private GetMainWindowHandler _getMainWindow;
		private ReadOnlyDictionary<string, object> _namedElements = null;
		
		#endregion
		
		#region Properties
		
		public ReadOnlyDictionary<string, object> NamedElements { get { return _namedElements; } }
		
		public Window MainWindow { get { return _getMainWindow(); } }
		
		public System.Collections.Hashtable SynchronizedData { get { return _windowObj.SynchronizedData; } }
		
		public Collection<PSObject> Output { get { return _windowObj.Output; } }
		
		public Collection<ErrorRecord> ErrorRecords { get { return _windowObj.ErrorRecords; } }
		
		public Collection<WarningRecord> WarningRecords { get { return _windowObj.WarningRecords; } }
		
		public Collection<VerboseRecord> VerboseRecords { get { return _windowObj.VerboseRecords; } }
		
		public Collection<DebugRecord> DebugRecords { get { return _windowObj.DebugRecords; } }
		
		#endregion
		
		#region Constructors
		
		internal ThisObj(PSHost host, WpfWindow windowObj, Dictionary<string, object> namedElements, GetMainWindowHandler getMainWindow)
		{
			if (windowObj == null)
				throw new ArgumentNullException("windowObj");
			
			if (namedElements == null)
				throw new ArgumentNullException("namedElements");
			
			if (getMainWindow == null)
				throw new ArgumentNullException("getMainWindow");
			
			_windowObj = windowObj;
			_namedElements = new ReadOnlyDictionary<string, object>(namedElements);
			_getMainWindow = getMainWindow;
		}
		
		#endregion
		
		internal class EventAttachment
		{
			private ScriptBlock _scriptBlock;
			
			private EventAttachment() { }
			
			public static EventAttachment AttachButtonClick(Button button, ScriptBlock scriptBlock)
			{
				EventAttachment eventAttachment = new EventAttachment();
				button.Click += new EventHandler(eventAttachment.Button_Click);
				eventAttachment._scriptBlock = scriptBlock;
				return eventAttachment;
			}
			
			private void Button_Click(object sender, EventArgs e) { Invoke(sender, e); }
			
			private void Invoke(object sender, object eventArgs)
			{
				Runspace runspace;
				if (host == null)
					runspace = RunspaceFactory.CreateRunspace();
				else
					runspace = RunspaceFactory.CreateRunspace(host);
				
				using (runspace)
				{
					runspace.ApartmentState = ApartmentState.STA;
					runspace.ThreadOptions = PSThreadOptions.ReuseThread;
					runspace.Open();
					using (PowerShell powerShell = PowerShell.Create())
					{
						_powerShell.Runspace = _runspace;
						_runspace.SessionStateProxy.SetVariable("this", this._thisObj);
						_runspace.SessionStateProxy.SetVariable("Sender", sender);
						_runspace.SessionStateProxy.SetVariable("EventArgs", eventArgs);
						_powerShell.AddCommand("Add-Type").AddParameter("AssemblyName", (typeof(Window)).Assembly.FullName);
						_powerShell.AddCommand("Add-Type").AddParameter("AssemblyName", (typeof(UIElement)).Assembly.FullName);
						_powerShell.AddCommand("Add-Type").AddParameter("AssemblyName", (typeof(DependencyObject)).Assembly.FullName);
						_powerShell.AddCommand("Add-Type").AddParameter("Path", this.GetType().Assembly.Location);
						_powerShell.AddScript(_scriptBlock.ToString());
						_powerShell.Invoke();
					}
				}
			}
		}
		#region Methods
		
		private Collection<EventAttachment> _eventAttachments = new Collection<EventAttachment>();
		
		internal AttachButtonClick(Button button, ScriptBlock scriptBlock)
		{
			if (button == null)
				throw new ArgumentNullException("button");
			
			if (scriptBlock == null)
				throw new ArgumentNullException("scriptBlock");
			
			_eventAttachments.Add(EventAttachment.AttachButtonClick(button, scriptBlock);
		}
		
		#endregion
    }
	
	public delegate Window GetMainWindowHandler();
}