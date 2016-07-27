using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Host;
using System.Management.Automation.Runspaces;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IOUtilityCLR
{
    public class BackgroundPipelineParameters : ICloneable
    {
        public Hashtable SynchronizedData { get; set; }
        public ApartmentState ApartmentState { get; set; }
        public PSThreadOptions ThreadOptions { get; set; }
        public Dictionary<string, object> Variables { get; private set; }
        public Collection<ScriptBlock> PipelineScripts { get; private set; }

        public BackgroundPipelineParameters(ApartmentState apartmentState, PSThreadOptions threadOptions)
        {
            SynchronizedData = Hashtable.Synchronized(new Hashtable());
            Variables = new Dictionary<string, object>();
            PipelineScripts = new Collection<ScriptBlock>();
            ApartmentState = apartmentState;
            ThreadOptions = threadOptions;
        }
        public BackgroundPipelineParameters() : this(ApartmentState.STA, PSThreadOptions.ReuseThread) { }
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
