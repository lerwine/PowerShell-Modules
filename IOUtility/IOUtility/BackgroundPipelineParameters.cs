using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Threading;

namespace IOUtilityCLR
{
    /// <summary>
    /// Object which defines a PowerShell pipeline to be invoked.
    /// </summary>
    public class BackgroundPipelineParameters : ICloneable
    {
        /// <summary>
        /// Values to be shared between host process and the invoked powershell instance.
        /// </summary>
        public Hashtable SynchronizedData { get; set; }

        /// <summary>
        /// Sets apartment state for thread which is executing the PowerShell pipeline.
        /// </summary>
        public ApartmentState ApartmentState { get; set; }

        /// <summary>
        /// Sets thread options for invoking the PowerShell pipeline.
        /// </summary>
        public PSThreadOptions ThreadOptions { get; set; }

        /// <summary>
        /// Variables to be defined in the pipeline.
        /// </summary>
        public Dictionary<string, object> Variables { get; private set; }

        /// <summary>
        /// Script blocks to be executed in the background PowerShell pipeline.
        /// </summary>
        public Collection<ScriptBlock> PipelineScripts { get; private set; }

        /// <summary>
        /// Create new object to represent a background PowerShell pipeline invocation.
        /// </summary>
        /// <param name="apartmentState">Apartment state for thread which is executing the PowerShell pipeline.</param>
        /// <param name="threadOptions">Thread options for invoking the PowerShell pipeline.</param>
        public BackgroundPipelineParameters(ApartmentState apartmentState, PSThreadOptions threadOptions)
        {
            SynchronizedData = Hashtable.Synchronized(new Hashtable());
            Variables = new Dictionary<string, object>();
            PipelineScripts = new Collection<ScriptBlock>();
            ApartmentState = apartmentState;
            ThreadOptions = threadOptions;
        }

        /// <summary>
        /// Create new object to represent a background PowerShell pipeline invocation running on a single, re-used thread.
        /// </summary>
        public BackgroundPipelineParameters() : this(ApartmentState.STA, PSThreadOptions.ReuseThread) { }

        /// <summary>
        /// Creates a cloned copy of this object.
        /// </summary>
        /// <returns>Object with values cloned, except for <see cref="BackgroundPipelineParameters.SynchronizedData"/>, in which the same instance is copied.</returns>
        public BackgroundPipelineParameters Clone()
        {
            BackgroundPipelineParameters clone = new BackgroundPipelineParameters
            {
                ApartmentState = this.ApartmentState,
                ThreadOptions = this.ThreadOptions,
                PipelineScripts = new Collection<ScriptBlock>(this.PipelineScripts),
                SynchronizedData = this.SynchronizedData
            };
            foreach (object key in SynchronizedData.Keys)
                clone.SynchronizedData.Add(key, SynchronizedData[key]);
            foreach (string key in Variables.Keys)
                clone.Variables.Add(key, Variables[key]);
            return clone;
        }

        object ICloneable.Clone() { return Clone(); }
    }
}
