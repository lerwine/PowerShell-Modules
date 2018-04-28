using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;

namespace Erwine.Leonard.T.TextToSpeech.Commands
{

    [Cmdlet(VerbsData.Out, "SpeechSynthesizer", DefaultParameterSetName = PARAMETERSETNAME_TEXT)]
    [OutputType(typeof(SpeechSynthesisResult))]
    public class Out_SpeechSynthesizer : SpeechSynthesizerCommandBase<SpeechSynthesisResult>
    {
        private int _promptIndex = -1;
        private bool _cancelRaised = false;

        public class InvocationState
        {
            private Hashtable _synchronizedData;
            private Func<int> _getPromptIndex;
            public InvocationState(Hashtable synchronizedData, Func<int> getPromptIndex)
            {
                _synchronizedData = Hashtable.Synchronized(new Hashtable());
                _getPromptIndex = getPromptIndex;
                SynchronizedData = synchronizedData;
            }

            public TimeSpan AudioPosition
            {
                get
                {
                    object obj = _synchronizedData["AudioPosition"];
                    if (obj != null && obj is TimeSpan)
                        return (TimeSpan)obj;
                    return TimeSpan.Zero;
                }
                internal set { _synchronizedData["AudioPosition"] = value; }
            }
            public string Bookmark
            {
                get { return _synchronizedData["Bookmark"] as string; }
                set { _synchronizedData["Bookmark"] = value; }
            }
            public bool Cancelled
            {
                get
                {
                    object obj = _synchronizedData["Cancelled"];
                    if (obj != null && obj is bool)
                        return (bool)obj;
                    return false;
                }
                internal set { _synchronizedData["Cancelled"] = value; }
            }
            public int CharacterCount
            {
                get
                {
                    object obj = _synchronizedData["CharacterCount"];
                    if (obj != null && obj is int)
                        return (int)obj;
                    return 0;
                }
                internal set { _synchronizedData["CharacterCount"] = value; }
            }
            public int CharacterPosition
            {
                get
                {
                    object obj = _synchronizedData["CharacterPosition"];
                    if (obj != null && obj is int)
                        return (int)obj;
                    return 0;
                }
                internal set { _synchronizedData["CharacterPosition"] = value; }
            }
            public SynthesizerEmphasis Emphasis
            {
                get
                {
                    object obj = _synchronizedData["Emphasis"];
                    if (obj != null && obj is SynthesizerEmphasis)
                        return (SynthesizerEmphasis)obj;
                    return default(SynthesizerEmphasis);
                }
                internal set { _synchronizedData["Emphasis"] = value; }
            }
            public string Phoneme
            {
                get { return _synchronizedData["Phoneme"] as string; }
                set { _synchronizedData["Phoneme"] = value; }
            }
            public int PromptIndex
            {
                get
                {
                    object obj = _synchronizedData["PromptIndex"];
                    if (obj != null && obj is int)
                        return (int)obj;
                    return 0;
                }
                internal set { _synchronizedData["PromptIndex"] = value; }
            }
            public SynthesizerState State
            {
                get
                {
                    object obj = _synchronizedData["SynthesizerState"];
                    if (obj != null && obj is SynthesizerState)
                        return (SynthesizerState)obj;
                    return SynthesizerState.Ready;
                }
                internal set { _synchronizedData["PromptIndex"] = value; }
            }
            public int? Viseme
            {
                get { return _synchronizedData["Viseme"] as int?; }
                set { _synchronizedData["Viseme"] = value; }
            }
            public VoiceInfo Voice
            {
                get { return _synchronizedData["Voice"] as VoiceInfo; }
                set { _synchronizedData["Voice"] = value; }
            }
            public Hashtable SynchronizedData { get; private set; }
        }

        private InvocationState _invocationState;

        private Collection<PSObject> _output = new Collection<PSObject>();

        private Collection<ErrorRecord> _errors = new Collection<ErrorRecord>();

        private Collection<WarningRecord> _warnings = new Collection<WarningRecord>();
        
        private Collection<VerboseRecord> _verboseMessages = new Collection<VerboseRecord>();

        private Collection<DebugRecord> _debugMessages = new Collection<DebugRecord>();
        
        protected override SpeechSynthesisResult CreateResult(Collection<Prompt> prompts, out bool enumerateCollection)
        {
            bool failed = false;
            _invocationState = new InvocationState(Hashtable.Synchronized(new Hashtable()), () => _promptIndex);
            if (prompts.Count > 0)
            {
                try
                {
                    using (SpeechSynthesizer speechSynthesizer = new SpeechSynthesizer())
                    {
                        speechSynthesizer.SpeakStarted += SpeechSynthesizer_PromptStarted;
                        speechSynthesizer.StateChanged += SpeechSynthesizer_StateChanged;
                        speechSynthesizer.SpeakProgress += SpeechSynthesizer_SpeakProgress;
                        speechSynthesizer.BookmarkReached += SpeechSynthesizer_BookmarkReached;
                        speechSynthesizer.VoiceChange += SpeechSynthesizer_VoiceChange;
                        speechSynthesizer.VisemeReached += SpeechSynthesizer_VisemeReached;
                        speechSynthesizer.PhonemeReached += SpeechSynthesizer_PhonemeReached;
                        speechSynthesizer.SpeakCompleted += SpeechSynthesizer_PromptCompleted;
                        Dictionary<string, object> variables = new Dictionary<string, object>();
                        variables.Add("Prompt", (prompts.Count > 0) ? prompts[0] : null);
                        variables.Add("EventName", "SpeakStarted");
                        InvokeScript(OnSpeakStarted, variables);
                        for (_promptIndex = 0; _promptIndex < prompts.Count && !_cancelRaised; _promptIndex++)
                        {
                            if (prompts[_promptIndex] != null)
                                speechSynthesizer.Speak(prompts[_promptIndex]);
                        }
                    }
                }
                catch (Exception exception)
                {
                    failed = true;
                    _errors.Add(new ErrorRecord(exception, "SpeechSynthesisError", ErrorCategory.InvalidOperation, prompts));
                }
            }

            enumerateCollection = false;
            return new SpeechSynthesisResult(_output, _errors, _warnings, _verboseMessages, _debugMessages, _cancelRaised, failed, _invocationState.SynchronizedData);
        }

        private void InvokeScript(ScriptBlock scriptBlock, Dictionary<string, object> variables)
        {
            try
            {
                using (Runspace runspace = RunspaceFactory.CreateRunspace())
                {
                    runspace.ApartmentState = System.Threading.ApartmentState.STA;
                    runspace.ThreadOptions = PSThreadOptions.ReuseThread;
                    runspace.Open();
                    foreach (string name in variables.Keys)
                        runspace.SessionStateProxy.SetVariable(name, variables[name]);
                    runspace.SessionStateProxy.SetVariable("this", _invocationState);
                    using (PowerShell powershell = PowerShell.Create(RunspaceMode.NewRunspace))
                    {
                        powershell.AddScript(scriptBlock.ToString());
                        AddRange<PSObject>(_output, powershell.Invoke());
                        AddRange<ErrorRecord>(_errors, powershell.Streams.Error.ReadAll());
                        AddRange<WarningRecord>(_warnings, powershell.Streams.Warning.ReadAll());
                        AddRange<VerboseRecord>(_verboseMessages, powershell.Streams.Verbose.ReadAll());
                        AddRange<DebugRecord>(_debugMessages, powershell.Streams.Debug.ReadAll());
                    }
                }
            }
            catch (Exception exception)
            {
                _errors.Add(new ErrorRecord(exception, "PowerShellInvocationError", ErrorCategory.InvalidOperation, scriptBlock));
            }
        }

        private void AddRange<T>(Collection<T> target, IEnumerable<T> source)
        {
            if (source == null)
                return;

            foreach (T obj in source)
                target.Add(obj);
        }

        private void RaiseEvent(ScriptBlock scriptBlock, string eventName, AsyncCompletedEventArgs eventArgs)
        {
            if (eventArgs.Cancelled)
                _cancelRaised = true;
            _errors.Add(new ErrorRecord(eventArgs.Error, eventName + "Error", ErrorCategory.InvalidOperation, eventArgs));
            RaiseEvent(scriptBlock, eventName, eventArgs as EventArgs);
        }

        private void RaiseEvent(ScriptBlock scriptBlock, string eventName, EventArgs eventArgs)
        {
            Dictionary<string, object> variables = new Dictionary<string, object>();
            variables.Add("EventArgs", eventArgs);
            variables.Add("EventName", eventName);
            InvokeScript(scriptBlock, variables);
        }

        private void SpeechSynthesizer_VoiceChange(object sender, VoiceChangeEventArgs e) { RaiseEvent(OnVoiceChange, "VoiceChange", e); }

        private void SpeechSynthesizer_VisemeReached(object sender, VisemeReachedEventArgs e) { RaiseEvent(OnVisemeReached, "VisemeReached", e); }

        private void SpeechSynthesizer_StateChanged(object sender, StateChangedEventArgs e) { RaiseEvent(OnStateChanged, "StateChanged", e); }

        private void SpeechSynthesizer_PromptStarted(object sender, SpeakStartedEventArgs e) { RaiseEvent(OnPromptStarted, "SpeakStarted", e); }

        private void SpeechSynthesizer_SpeakProgress(object sender, SpeakProgressEventArgs e) { RaiseEvent(OnSpeakProgress, "SpeakProgress", e); }

        private void SpeechSynthesizer_PromptCompleted(object sender, SpeakCompletedEventArgs e) { RaiseEvent(OnPromptCompleted, "PromptCompleted", e); }

        private void SpeechSynthesizer_PhonemeReached(object sender, PhonemeReachedEventArgs e) { RaiseEvent(OnPhonemeReached, "PhonemeReached", e); }

        private void SpeechSynthesizer_BookmarkReached(object sender, BookmarkReachedEventArgs e) { RaiseEvent(OnBookmarkReached, "BookmarkReached", e); }
    }
}
