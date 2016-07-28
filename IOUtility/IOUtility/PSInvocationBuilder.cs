using System;
using System.Collections;
using System.Collections.Generic;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Threading;

namespace IOUtilityCLR
{
    /// <summary>
    /// Class which is used to build a PowerShell script pipeline, along with settings for the invocation.
    /// </summary>
    public class PSInvocationBuilder
    {
        private Hashtable _synchronizedData;

        /// <summary>
        /// Synchronized data that can be shared between the calling process and the pipeline being executed.
        /// </summary>
        public Hashtable SynchronizedData { get { return _synchronizedData; } }

        /// <summary>
        /// Specifies the apartment state to use for the thread which invokes the pipeline.
        /// </summary>
        public ApartmentState ApartmentState { get; set; }

        /// <summary>
        /// Indicates how threads are handled when the pipeline is invoked.
        /// </summary>
        public PSThreadOptions ThreadOptions { get; set; }

        private Dictionary<string, object> _variables = new Dictionary<string, object>();
        /// <summary>
        /// Dictionary of variables and values to be set in the runspace.
        /// </summary>
        public Dictionary<string, object> Variables { get { return _variables; } }

        /// <summary>
        /// Initial session state to use when creating the runspace.
        /// </summary>
        public InitialSessionState InitialSessionState { get; set; }

        private List<PipelineItem> _pipeline = new List<PipelineItem>();

        /// <summary>
        /// List of objects that are to be added to the PowerShell pipeline and invoked.
        /// </summary>
        public List<PipelineItem> Pipeline { get { return _pipeline; } }

        /// <summary>
        /// Input data for the first command of the pipeline.
        /// </summary>
        public IEnumerable Input { get; set; }

        /// <summary>
        /// A PSInvocationSettings object that defines how Windows PowerShell handles the invocation of the pipeline, such as whether the commands are added to the session history.
        /// </summary>
        public PSInvocationSettings Settings { get; set; }
        /// <summary>
        /// Creates new PowerShell command pipeline builder.
        /// </summary>
        public PSInvocationBuilder()
        {
            _synchronizedData = Hashtable.Synchronized(new Hashtable());
            ThreadOptions = PSThreadOptions.ReuseThread;
        }
        /// <summary>
        /// Adds an argument for a positional parameter of a command without specifying the parameter name.
        /// </summary>
        /// <param name="value">The value of the argument to be added to the last command of the pipeline.</param>
        public void AddArgument(object value) { Pipeline.Add(new ArgumentPipelineItem(value)); }
        /// <summary>
        /// Adds a command to the end of the pipeline of the PowerShell object by specifying the command name.
        /// </summary>
        /// <param name="cmdlet">The name of the command to be added to the end of the pipeline. Do not include spaces immediately before or after the cmdlet name.</param>
        public void AddCommand(string cmdlet) { Pipeline.Add(new CommandPipelineItem(cmdlet)); }
        /// <summary>
        /// Adds a cmdlet to the end of the pipeline of the PowerShell object, and specifies whether the cmdlet should be run within a new local scope.
        /// </summary>
        /// <param name="cmdlet">The name of the command to be added to the end of the pipeline. Do not include spaces immediately before or after the cmdlet name.</param>
        /// <param name="useLocalScope">true to run the cmdlet within a new local scope; otherwise, false.</param>
        public void AddCommand(string cmdlet, bool useLocalScope) { Pipeline.Add(new CommandPipelineItem(cmdlet, useLocalScope)); }
        /// <summary>
        /// Adds a switch parameter to the last command added to the pipeline.
        /// </summary>
        /// <param name="parameterName">The name of the switch parameter to be added.</param>
        public void AddParameter(string parameterName) { Pipeline.Add(new ParameterPipelineItem(parameterName)); }
        /// <summary>
        /// Adds a parameter and value to the last command added to the pipeline.
        /// </summary>
        /// <param name="parameterName">The name of the parameter to be added.</param>
        /// <param name="value">The value of the parameter to be added.</param>
        public void AddParameter(string parameterName, object value) { Pipeline.Add(new ParameterPipelineItem(parameterName, value)); }
        /// <summary>
        /// Adds a set of parameters to the last command of the pipeline. The parameter names and values are taken from the keys and values of the dictionary.
        /// </summary>
        /// <param name="parameters">A dictionary of the parameters to be added.</param>
        public void AddParameters(IDictionary parameters) { Pipeline.Add(new ParameterPipelineItem(parameters)); }
        /// <summary>
        /// Adds a set of parameters to the last command of the pipeline. The parameter values are taken from the values in a list. 
        /// </summary>
        /// <param name="parameters">A list of the parameters to be added.</param>
        public void AddParameters(IList parameters) { Pipeline.Add(new ParameterPipelineItem(parameters)); }
        /// <summary>
        /// Adds a script to the end of the pipeline of the PowerShell object.
        /// </summary>
        /// <param name="script">The script to be added at the end of the pipeline. This parameter takes a piece of code that is treated as a script when the pipeline is invoked.</param>
        public void AddScript(string script) { Pipeline.Add(new ScriptPipelineItem(script)); }
        /// <summary>
        /// Adds a script to the end of the pipeline of the PowerShell object, and indicates whether the script should be run in local scope.
        /// </summary>
        /// <param name="script">The script to be added at the end of the pipeline. This parameter takes a piece of code that is treated as a script when the pipeline is invoked.</param>
        /// <param name="useLocalScope">A <see cref="Boolean"/> value that indicates whether the script should be run in local scope. Be aware that when local scope is used, variables will be defined in a new scope so they will not be available when the script is complete.</param>
        public void AddScript(string script, bool useLocalScope) { Pipeline.Add(new ScriptPipelineItem(script, useLocalScope)); }
        /// <summary>
        /// Adds an additional statement to the end of the pipeline.
        /// </summary>
        public void AddStatement() { Pipeline.Add(new StatementPipelineItem()); }
        /// <summary>
        /// Creates runspace object.
        /// </summary>
        /// <returns>Runspace object initialized and opened.</returns>
        protected Runspace CreateRunspace()
        {
            InitialSessionState iss = InitialSessionState;
            if (InitialSessionState == null)
            {
                iss = InitialSessionState.CreateDefault();
                iss.ExecutionPolicy = Microsoft.PowerShell.ExecutionPolicy.Bypass;
            }
            Runspace runspace = RunspaceFactory.CreateRunspace(iss);
            runspace.ApartmentState = ApartmentState;
            runspace.ThreadOptions = ThreadOptions;
            runspace.Open();
            foreach (string name in Variables.Keys)
                runspace.SessionStateProxy.SetVariable(name, Variables[name]);
            runspace.SessionStateProxy.SetVariable("SynchronizedData", SynchronizedData);
            return runspace;
        }
        /// <summary>
        /// Creates PowerShell object
        /// </summary>
        /// <param name="runspace">Runspace to be used.</param>
        /// <returns>PowerShell object created and initialized.</returns>
        protected PowerShell CreatePowerShell(Runspace runspace)
        {
            PowerShell powershell = PowerShell.Create(RunspaceMode.NewRunspace);
            powershell.Runspace = runspace;
            foreach (PipelineItem item in Pipeline)
            {
                if (item != null)
                    item.AddToPowerShell(powershell);
            }
            return powershell;
        }
        /// <summary>
        /// Creates and invokes the pipeline, returning the results.
        /// </summary>
        /// <returns>An object which represents the results of the invocation.</returns>
        public PSInvocationResult GetResult()
        {
            if (Settings == null)
            {
                if (Input == null)
                    return PSInvocationResult.Create(CreateRunspace, CreatePowerShell, Variables.Keys, SynchronizedData);
                return PSInvocationResult.Create(CreateRunspace, CreatePowerShell, Input, Variables.Keys, SynchronizedData);
            }

            if (Input == null)
                return PSInvocationResult.Create(CreateRunspace, CreatePowerShell, new object[0], Settings, Variables.Keys, SynchronizedData);
            return PSInvocationResult.Create(CreateRunspace, CreatePowerShell, Input, Settings, Variables.Keys, SynchronizedData);

        }

        /// <summary>
        /// Creates and invokes the pipeline asyncrhonously.
        /// </summary>
        /// <returns>An object which represents the executing pipeline.</returns>
        public virtual PSAsyncInvocationResult GetResultAsync()
        {
            if (Settings != null || Input != null)
                throw new InvalidOperationException("Input type must be explicity defined.");
            return PSAsyncInvocationResult.Create(CreateRunspace, CreatePowerShell, Variables.Keys, SynchronizedData);
        }

        #region Nested classes

        /// <summary>
        /// Base class for items to be added to the pipeline.
        /// </summary>
        public abstract class PipelineItem
        {
            /// <summary>
            /// Adds an item to the PowerShell pipeline.
            /// </summary>
            /// <param name="powershell">PowerShell to add item to.</param>
            public abstract void AddToPowerShell(PowerShell powershell);
        }
        /// <summary>
        /// Represents an argument to be added to a PowerShell pipeline.
        /// </summary>
        public class ArgumentPipelineItem : PipelineItem
        {
            /// <summary>
            /// Value of argument to be aded
            /// </summary>
            public object Value { get; private set; }

            /// <summary>
            /// Create new object to represent an argument that is to be added to a PowerShell pipeline.
            /// </summary>
            /// <param name="value">Value of argument.</param>
            public ArgumentPipelineItem(object value) { this.Value = value; }

            /// <summary>
            /// Adds the argument to the PowerShell pipeline.
            /// </summary>
            /// <param name="powershell">PowerShell to add item to.</param>
            public override void AddToPowerShell(PowerShell powershell) { powershell.AddArgument(Value); }
        }
        /// <summary>
        /// Represents a command to be added to a PowerShell pipeline.
        /// </summary>
        public class CommandPipelineItem : PipelineItem
        {
            /// <summary>
            /// Name of command
            /// </summary>
            public string Cmdlet { get; private set; }

            /// <summary>
            /// Whether to use local scope.
            /// </summary>
            public bool? UseLocalScope { get; private set; }

            /// <summary>
            /// Create new object to represent a command that is to be added to a PowerShell pipeline.
            /// </summary>
            /// <param name="cmdlet">Name of cmdlet</param>
            public CommandPipelineItem(string cmdlet)
            {
                if (cmdlet == null)
                    throw new ArgumentNullException("cmdlet");
                if (cmdlet.Trim().Length == 0)
                    throw new ArgumentException("Cmdlet cannot be empty", "cmdlet");
                Cmdlet = cmdlet;
            }

            /// <summary>
            /// Create new object to represent a command that is to be added to a PowerShell pipeline.
            /// </summary>
            /// <param name="cmdlet">Name of cmdlet</param>
            /// <param name="useLocalScope">Whether to use local scope.</param>
            public CommandPipelineItem(string cmdlet, bool useLocalScope) : this(cmdlet) { UseLocalScope = useLocalScope; }

            /// <summary>
            /// Adds the command to the PowerShell pipeline.
            /// </summary>
            /// <param name="powershell">PowerShell to add command to.</param>
            public override void AddToPowerShell(PowerShell powershell)
            {
                if (UseLocalScope.HasValue)
                    powershell.AddCommand(Cmdlet, UseLocalScope.Value);
                else
                    powershell.AddCommand(Cmdlet);
            }
        }
        /// <summary>
        /// Represents a parameter to be added to a PowerShell pipeline.
        /// </summary>
        public class ParameterPipelineItem : PipelineItem
        {
            private bool _alternate;
            private object _value;

            /// <summary>
            /// Indicates whether parameter is a switch.
            /// </summary>
            public bool IsSwitch { get { return _alternate && ParameterName == null; } }

            /// <summary>
            /// Name of parameter.
            /// </summary>
            public string ParameterName { get; private set; }

            /// <summary>
            /// Value of named parameter.
            /// </summary>
            public object Value { get { return (ParameterName == null) ? null : _value; } }

            /// <summary>
            /// Dictionary for parameter set.
            /// </summary>
            public IDictionary ParameterSet { get { return (ParameterName != null || _alternate) ? null : _value as IDictionary; } }

            /// <summary>
            /// List of parameter values.
            /// </summary>
            public IList Parameters { get { return (ParameterName == null && _alternate) ? _value as IList : null; } }

            /// <summary>
            /// Create new object to represent a switch parameter that is to be added to a PowerShell pipeline.
            /// </summary>
            /// <param name="parameterName">Name of switch parameter.</param>
            public ParameterPipelineItem(string parameterName)
            {
                if (parameterName == null)
                    throw new ArgumentNullException("parameterName");
                if (parameterName.Trim().Length == 0)
                    throw new ArgumentException("Parameter name cannot be empty", "parameterName");
                ParameterName = parameterName;
                _alternate = true;
            }

            /// <summary>
            /// Create new object to represent a parameter that is to be added to a PowerShell pipeline.
            /// </summary>
            /// <param name="parameterName">Name of parameter.</param>
            /// <param name="value">Value of parameter.</param>
            public ParameterPipelineItem(string parameterName, object value) : this(parameterName)
            {
                _value = value;
                _alternate = false;
            }

            /// <summary>
            /// Create new object to represent a parameter set that is to be added to a PowerShell pipeline.
            /// </summary>
            /// <param name="parameters">Parameter set</param>
            public ParameterPipelineItem(IDictionary parameters)
            {
                ParameterName = null;
                _value = parameters;
                _alternate = false;
            }

            /// <summary>
            /// Create new object to represent a parameter list that is to be added to a PowerShell pipeline.
            /// </summary>
            /// <param name="parameters">List of parameters.</param>
            public ParameterPipelineItem(IList parameters)
            {
                ParameterName = null;
                _value = parameters;
                _alternate = true;
            }

            /// <summary>
            /// Adds the parameter(s) to the PowerShell pipeline.
            /// </summary>
            /// <param name="powershell">PowerShell to add parameters to.</param>
            public override void AddToPowerShell(PowerShell powershell)
            {
                if (ParameterName == null)
                {
                    if (_alternate)
                        powershell.AddParameters(_value as IList);
                    else
                        powershell.AddParameters(_value as IDictionary);
                }
                else if (_alternate)
                    powershell.AddParameter(ParameterName);
                else
                    powershell.AddParameter(ParameterName, _value);
            }
        }
        /// <summary>
        /// Represents a script to be added to a PowerShell pipeline.
        /// </summary>
        public class ScriptPipelineItem : PipelineItem
        {
            /// <summary>
            /// Script to be added to pipeline.
            /// </summary>
            public string Script { get; private set; }
            /// <summary>
            /// Whether to use local scope.
            /// </summary>
            public bool? UseLocalScope { get; private set; }

            /// <summary>
            /// Create new object to represent a script that is to be added to a PowerShell pipeline.
            /// </summary>
            /// <param name="script">Script to be added.</param>
            public ScriptPipelineItem(string script)
            {
                if (script == null)
                    throw new ArgumentNullException("script");
                if (script.Trim().Length == 0)
                    throw new ArgumentException("Script cannot be empty", "script");
                Script = script;
                UseLocalScope = null;
            }

            /// <summary>
            /// Create new object to represent a script that is to be added to a PowerShell pipeline.
            /// </summary>
            /// <param name="script">Script to be added.</param>
            /// <param name="useLocalScope">Whether to use local scope.</param>
            public ScriptPipelineItem(string script, bool useLocalScope) : this(script) { this.UseLocalScope = useLocalScope; }

            /// <summary>
            /// Adds the script to the PowerShell pipeline.
            /// </summary>
            /// <param name="powershell">PowerShell to add script to.</param>
            public override void AddToPowerShell(PowerShell powershell)
            {
                if (UseLocalScope.HasValue)
                    powershell.AddScript(Script, UseLocalScope.Value);
                else
                    powershell.AddScript(Script);
            }
        }
        /// <summary>
        /// Represents a statement to be added to a PowerShell pipeline.
        /// </summary>
        public class StatementPipelineItem : PipelineItem
        {
            /// <summary>
            /// Adds the statement to the PowerShell pipeline.
            /// </summary>
            /// <param name="powershell">PowerShell to add statement to.</param>
            public override void AddToPowerShell(PowerShell powershell) { powershell.AddStatement(); }
        }

        #endregion
    }

    /// <summary>
    /// Class which is used to build a PowerShell script pipeline, along with settings for the invocation.
    /// </summary>
    public class PSInvocationBuilder<T> : PSInvocationBuilder
    {
        /// <summary>
        /// Explicitly typed nput data for the first command of the pipeline.
        /// </summary>
        public new PSDataCollection<T> Input { get { return base.Input as PSDataCollection<T>; }  set { base.Input = value; } }

        /// <summary>
        /// Creates and invokes the pipeline asyncrhonously.
        /// </summary>
        /// <returns>An object which represents the executing pipeline.</returns>
        public override PSAsyncInvocationResult GetResultAsync()
        {
            if (Settings == null)
            {
                if (Input == null)
                    return PSAsyncInvocationResult.Create(CreateRunspace, CreatePowerShell, Variables.Keys, SynchronizedData);
                return PSAsyncInvocationResult.Create(CreateRunspace, CreatePowerShell, Input, Variables.Keys, SynchronizedData);
            }

            if (Input == null)
                return PSAsyncInvocationResult.Create(CreateRunspace, CreatePowerShell, new PSDataCollection<T>(), Settings, Variables.Keys, SynchronizedData);
            return PSAsyncInvocationResult.Create(CreateRunspace, CreatePowerShell, Input, Settings, Variables.Keys, SynchronizedData);
        }
    }
}