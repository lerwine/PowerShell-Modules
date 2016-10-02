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
    public class PSInvocationBuilder : PSInvocationBuilderBase<PSInvocationResult, PSAsyncInvocationResult>
    {
        /// <summary>
        /// Creates new PowerShell command pipeline builder.
        /// </summary>
        public PSInvocationBuilder() : base() { }
        
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