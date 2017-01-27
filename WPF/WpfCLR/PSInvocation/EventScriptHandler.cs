using System;
using System.Collections;
using System.Management.Automation;
using System.Management.Automation.Host;
using System.Management.Automation.Runspaces;
using System.Threading;

namespace WpfCLR.PSInvocation
{
    public class EventScriptHandler
    {
        private PSHost _host = null;
        private string _initialLocation = "";
        private bool? _useLocalScope = null;
        private ApartmentState? _apartmentState = null;
        private PSThreadOptions? _threadOptions = null;
        private object[] _variableKeys = null;
        private PSObject _this = new PSObject();

        public ScriptBlock HandlerScript { get; private set; }

        public PSHost Host
        {
            get { return _host; }
            set
            {
                if (_variableKeys != null)
                    throw new InvalidOperationException();
                _host = value;
            }
        }

        public string InitialLocation
        {
            get { return _initialLocation; }
            set
            {
                if (_variableKeys != null)
                    throw new InvalidOperationException();
                _initialLocation = (value == null) ? "" : value;
            }
        }

        public bool? UseLocalScope
        {
            get { return _useLocalScope; }
            set
            {
                if (_variableKeys != null)
                    throw new InvalidOperationException();
                _useLocalScope = value;
            }
        }

        public ApartmentState? ApartmentState
        {
            get { return _apartmentState; }
            set
            {
                if (_variableKeys != null)
                    throw new InvalidOperationException();
                _apartmentState = value;
            }
        }

        public PSThreadOptions? ThreadOptions
        {
            get { return _threadOptions; }
            set
            {
                if (_variableKeys != null)
                    throw new InvalidOperationException();
                _threadOptions = value;
            }
        }

        public RunspaceConfiguration Configuration { get; private set; }
        public Hashtable Variables { get; private set; }
        public PSObject This
        {
            get { return _this; }
            set
            {
                if (_variableKeys != null)
                    throw new InvalidOperationException();
                _this = (value == null) ? new PSObject() : value;
            }
        }

        public EventScriptHandler()
        {
            Configuration = RunspaceConfiguration.Create();
            Variables = new Hashtable();
        }
    }
}