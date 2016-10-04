using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
#if !PSLEGACY
using System.Linq;
#endif
using System.Management.Automation;
using System.Management.Automation.Host;
using System.Management.Automation.Runspaces;
using System.Text;
using System.Threading;
#if !PSLEGACY
using System.Threading.Tasks;
#endif

namespace ActivityLogger
{
    public class BackgroundPipelineParameters : ICloneable
    {
#if PSLEGACY2
        private Hashtable _synchronizedData = null;
        private ApartmentState _apartmentState = default(ApartmentState);
        private PSThreadOptions _threadOptions = default(PSThreadOptions);
        private Dictionary<string, object> _variables = null;
        private Collection<ScriptBlock> _pipelineScripts = null;

#endif
#if PSLEGACY2
        public Hashtable SynchronizedData { get { return _synchronizedData; } set { _synchronizedData = value; } }
#else
        public Hashtable SynchronizedData { get; set; }
#endif

#if PSLEGACY2
        public ApartmentState ApartmentState { get { return _apartmentState; } set { _apartmentState = value; } }
#else
        public ApartmentState ApartmentState { get; set; }
#endif

#if PSLEGACY2
        public PSThreadOptions ThreadOptions { get { return _threadOptions; } set { _threadOptions = value; } }
#else
        public PSThreadOptions ThreadOptions { get; set; }
#endif

#if PSLEGACY2
        public Dictionary<string, object> Variables { get { return _variables; } private set { _variables = value; } }
#else
        public Dictionary<string, object> Variables { get; private set; }
#endif

#if PSLEGACY2
        public Collection<ScriptBlock> PipelineScripts { get { return _pipelineScripts; } private set { _pipelineScripts = value; } }
#else
        public Collection<ScriptBlock> PipelineScripts { get; private set; }
#endif

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
#if PSLEGACY2
            BackgroundPipelineParameters clone = new BackgroundPipelineParameters();
            clone.ApartmentState = ApartmentState;
            clone.ThreadOptions = ThreadOptions;
            clone.PipelineScripts = new Collection<ScriptBlock>(PipelineScripts);
            clone.SynchronizedData = SynchronizedData;
#else
            BackgroundPipelineParameters clone = new BackgroundPipelineParameters
            {
                ApartmentState = ApartmentState,
                ThreadOptions = ThreadOptions,
                PipelineScripts = new Collection<ScriptBlock>(PipelineScripts),
                SynchronizedData = SynchronizedData
            };
#endif
            foreach (object key in SynchronizedData.Keys)
                clone.SynchronizedData.Add(key, SynchronizedData[key]);
            foreach (string key in Variables.Keys)
                clone.Variables.Add(key, Variables[key]);
            return clone;
        }

        object ICloneable.Clone() { return Clone(); }
    }
}
