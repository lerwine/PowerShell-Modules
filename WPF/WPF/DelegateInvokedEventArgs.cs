using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Management.Automation;

namespace WpfCLR
{
    /// <summary>
    /// Arguments which contain output from <seealso cref="ScriptBlock"/> invoked as a delegate.
    /// </summary>
    public class DelegateInvokedEventArgs : EventArgs
    {
        private Collection<PSObject> _output = new Collection<PSObject>();
        private Collection<ErrorRecord> _errorRecords = new Collection<ErrorRecord>();
        private Collection<WarningRecord> _warningRecords = new Collection<WarningRecord>();
        private Collection<VerboseRecord> _verboseRecords = new Collection<VerboseRecord>();
        private Collection<DebugRecord> _debugRecords = new Collection<DebugRecord>();

        /// <summary>
        /// Output return from invoking delegate <seealso cref="ScriptBlock"/>.
        /// </summary>
        public Collection<PSObject> Output { get { return _output; } }

        /// <summary>
        /// Error records produced from invoking delegate <seealso cref="ScriptBlock"/>.
        /// </summary>
        public Collection<ErrorRecord> ErrorRecords { get { return _errorRecords; } }

        /// <summary>
        /// Warning records produced from invoking delegate <seealso cref="ScriptBlock"/>.
        /// </summary>
        public Collection<WarningRecord> WarningRecords { get { return _warningRecords; } }

        /// <summary>
        /// Verbose records produced from invoking delegate <seealso cref="ScriptBlock"/>.
        /// </summary>
        public Collection<VerboseRecord> VerboseRecords { get { return _verboseRecords; } }

        /// <summary>
        /// Debug records produced from invoking delegate <seealso cref="ScriptBlock"/>.
        /// </summary>
        public Collection<DebugRecord> DebugRecords { get { return _debugRecords; } }

        /// <summary>
        /// Initializes new <see cref="DelegateInvokedEventArgs"/> object.
        /// </summary>
        /// <param name="dataStreams">Data streams from <seealso cref="PowerShell"/> object that invoked the delegate <seealso cref="ScriptBlock"/>.</param>
        public DelegateInvokedEventArgs(PSDataStreams dataStreams)
        {
            foreach (ErrorRecord obj in dataStreams.Error.ReadAll())
                ErrorRecords.Add(obj);
            foreach (WarningRecord obj in dataStreams.Warning.ReadAll())
                WarningRecords.Add(obj);
            foreach (VerboseRecord obj in dataStreams.Verbose.ReadAll())
                VerboseRecords.Add(obj);
            foreach (DebugRecord obj in dataStreams.Debug.ReadAll())
                DebugRecords.Add(obj);
        }
        /// <summary>
        /// Initializes empty <see cref="DelegateInvokedEventArgs"/> object.
        /// </summary>
        public DelegateInvokedEventArgs() { }

        /// <summary>
        /// Append objects to the <see cref="Output"/> collection.
        /// </summary>
        /// <param name="collection">Items to append.</param>
        public void AppendOutput(IEnumerable<PSObject> collection)
        {
            if (collection == null)
                return;

            lock (_output)
            {
                foreach (PSObject obj in collection)
                    _output.Add(obj);
            }
        }
    }
}