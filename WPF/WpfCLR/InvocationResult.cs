using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Threading;

namespace WpfCLR
{
	public class InvocationResult
	{
		private bool? _dialogResult;
		private Hashtable _results;
		private Collection<PSObject> _output = new Collection<PSObject>();
		private Collection<ErrorRecord> _errorRecords = new Collection<ErrorRecord>();
		private Collection<WarningRecord> _warningRecords = new Collection<WarningRecord>();
		private Collection<VerboseRecord> _verboseRecords = new Collection<VerboseRecord>();
		private Collection<DebugRecord> _debugRecords = new Collection<DebugRecord>();
		
		public bool? DialogResult { get { return _dialogResult; } }
		public Hashtable Results { get { return _results; } }
		public Collection<PSObject> Output { get { return _output; } }
		public Collection<ErrorRecord> ErrorRecords { get { return _errorRecords; } }
		public Collection<WarningRecord> WarningRecords { get { return _warningRecords; } }
		public Collection<VerboseRecord> VerboseRecords { get { return _verboseRecords; } }
		public Collection<DebugRecord> DebugRecords { get { return _debugRecords; } }
		
		public InvocationResult(bool? dialogResult, Hashtable results, PowerShell powerShell)
		{
			_dialogResult = dialogResult;
			_results = results;
			foreach (ErrorRecord obj in powerShell.Streams.Error.ReadAll())
				_errorRecords.Add(obj);
			foreach (WarningRecord obj in powerShell.Streams.Warning.ReadAll())
				_warningRecords.Add(obj);
			foreach (VerboseRecord obj in powerShell.Streams.Verbose.ReadAll())
				_verboseRecords.Add(obj);
			foreach (DebugRecord obj in powerShell.Streams.Debug.ReadAll())
				_debugRecords.Add(obj);
		}
		
		public InvocationResult(bool? dialogResult, Hashtable results, PowerShell powerShell, IAsyncResult asyncOperationStatus)
		{
			_dialogResult = dialogResult;
			_results = results;
			Collection<PSObject> output = null;
			try { output = powerShell.EndInvoke(asyncOperationStatus).ReadAll(); }
			catch { throw; }
			finally
			{
				if (output != null)
				{
					foreach (PSObject obj in output)
						_output.Add(obj);
				}
				foreach (ErrorRecord obj in powerShell.Streams.Error.ReadAll())
					_errorRecords.Add(obj);
				foreach (WarningRecord obj in powerShell.Streams.Warning.ReadAll())
					_warningRecords.Add(obj);
				foreach (VerboseRecord obj in powerShell.Streams.Verbose.ReadAll())
					_verboseRecords.Add(obj);
				foreach (DebugRecord obj in powerShell.Streams.Debug.ReadAll())
					_debugRecords.Add(obj);
			}
		}
	}
}
