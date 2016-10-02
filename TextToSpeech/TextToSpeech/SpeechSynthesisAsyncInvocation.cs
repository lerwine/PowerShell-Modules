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
using System.Threading;
using System.Threading.Tasks;

namespace Erwine.Leonard.T.TextToSpeech
{
    public class SpeechSynthesisAsyncInvocation : PSObject
    {
        private LinkedList<Prompt> _remainingPrompts;
        private SpeechSynthesizer _speechSynthesizer = null;
        private int _promptIndex = -1;
        private ScriptBlock _onSpeakStarted;
        private ScriptBlock _onPromptStarted;
        private ScriptBlock _onStateChanged;
        private ScriptBlock _onSpeakProgress;
        private ScriptBlock _onBookmarkReached;
        private ScriptBlock _onVoiceChange;
        private ScriptBlock _onVisemeReached;
        private ScriptBlock _onPhonemeReached;
        private ScriptBlock _onPromptCompleted;
        private ScriptBlock _onSpeakCompleted;

        public object UserState { get; private set; }

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

        private Collection<InformationRecord> _informationMessages = new Collection<InformationRecord>();

        private Collection<VerboseRecord> _verboseMessages = new Collection<VerboseRecord>();

        private Collection<DebugRecord> _debugMessages = new Collection<DebugRecord>();

        public Hashtable SynchronizedData { get; private set; }
        
        public PSSpeechSynthesizerState State { get; private set; }
        
        private VoiceInfo _voice = null;
        private int? _viseme = null;
        private SynthesizerEmphasis _emphasis = default(SynthesizerEmphasis);
        private TimeSpan _audioPosition = TimeSpan.Zero;
        private int _characterCount = 0;
        private int _characterPosition = 0;
        private string _phoneme = null;
        private string _bookmark = null;

        public VoiceInfo Voice
        {
            get { return _voice; }
            set
            {
                _voice = value;
                _invocationState.Voice = value;
            }
        }

        public int? Viseme
        {
            get { return _viseme; }
            set
            {
                _viseme = value;
                _invocationState.Viseme = value;
            }
        }

        public SynthesizerEmphasis Emphasis
        {
            get { return _emphasis; }
            set
            {
                _emphasis = value;
                _invocationState.Emphasis = value;
            }
        }

        public TimeSpan AudioPosition
        {
            get { return _audioPosition; }
            set
            {
                _audioPosition = value;
                _invocationState.AudioPosition = value;
            }
        }

        public int CharacterCount
        {
            get { return _characterCount; }
            set
            {
                _characterCount = value;
                _invocationState.CharacterCount = value;
            }
        }

        public int CharacterPosition
        {
            get { return _characterPosition; }
            set
            {
                _characterPosition = value;
                _invocationState.CharacterPosition = value;
            }
        }

        public string Phoneme
        {
            get { return _phoneme; }
            set
            {
                _phoneme = value;
                _invocationState.Phoneme = value;
            }
        }

        public string Bookmark
        {
            get { return _bookmark; }
            set
            {
                _bookmark = value;
                _invocationState.Bookmark = value;
            }
        }

        public string Text { get; private set; }

        public SpeechSynthesisAsyncInvocation(Collection<Prompt> prompts, ScriptBlock onSpeakStarted, ScriptBlock onPromptStarted, ScriptBlock onStateChanged,
            ScriptBlock onSpeakProgress, ScriptBlock onBookmarkReached, ScriptBlock onVoiceChange, ScriptBlock onVisemeReached, ScriptBlock onPhonemeReached,
            ScriptBlock onPromptCompleted, ScriptBlock onSpeakCompleted, object userState)
        {
            State = PSSpeechSynthesizerState.NotStarted;
            SynchronizedData = Hashtable.Synchronized(new Hashtable());
            _invocationState = new InvocationState(SynchronizedData, () => _promptIndex);
            _remainingPrompts = new LinkedList<Prompt>(prompts);
            _onSpeakStarted = onSpeakStarted;
            _onPromptStarted = onPromptStarted;
            _onStateChanged = onStateChanged;
            _onSpeakProgress = onSpeakProgress;
            _onBookmarkReached = onBookmarkReached;
            _onVoiceChange = onVoiceChange;
            _onVisemeReached = onVisemeReached;
            _onPhonemeReached = onPhonemeReached;
            _onPromptCompleted = onPromptCompleted;
            _onSpeakCompleted = onSpeakCompleted;
        }

        public void Start()
        {
            Prompt prompt;

            lock (_remainingPrompts)
            {
                switch (State)
                {
                    case PSSpeechSynthesizerState.NotStarted:
                        break;
                    case PSSpeechSynthesizerState.Paused:
                        if (_speechSynthesizer.State == SynthesizerState.Paused)
                            _speechSynthesizer.Resume();
                        else
                            SpeakNextPrompt(null);
                        State = PSSpeechSynthesizerState.Speaking;
                        return;
                    case PSSpeechSynthesizerState.Speaking:
                        throw new PSInvalidOperationException("Speech synthesizer was already started.");
                    default:
                        throw new PSInvalidOperationException("Speech synthesizer was already completed.");
                }

                if (_speechSynthesizer != null)
                    return;
                
                if (_remainingPrompts.First == null)
                {
                    State = PSSpeechSynthesizerState.Success;
                    return;
                }

                _speechSynthesizer = new SpeechSynthesizer();
                _speechSynthesizer.SpeakStarted += SpeechSynthesizer_PromptStarted;
                _speechSynthesizer.StateChanged += SpeechSynthesizer_StateChanged;
                _speechSynthesizer.SpeakProgress += SpeechSynthesizer_SpeakProgress;
                _speechSynthesizer.BookmarkReached += SpeechSynthesizer_BookmarkReached;
                _speechSynthesizer.VoiceChange += SpeechSynthesizer_VoiceChange;
                _speechSynthesizer.VisemeReached += SpeechSynthesizer_VisemeReached;
                _speechSynthesizer.PhonemeReached += SpeechSynthesizer_PhonemeReached;
                _speechSynthesizer.SpeakCompleted += SpeechSynthesizer_PromptCompleted;

                prompt = _remainingPrompts.First.Value;
                _remainingPrompts.RemoveFirst();
                try
                {
                    State = PSSpeechSynthesizerState.Speaking;
                    _promptIndex = 0;
                    _speechSynthesizer.SpeakAsync(prompt);
                }
                catch (Exception exception)
                {
                    State = PSSpeechSynthesizerState.NotStarted;
                    _remainingPrompts.AddFirst(prompt);
                    _speechSynthesizer.Dispose();
                    _speechSynthesizer = null;
                    throw new PSInvalidOperationException("Unable to queue first prompt.", exception);
                }
            }

            RaiseSpeakStarted(prompt);
        }

        public bool Stop()
        {
            Task task;

            lock (_remainingPrompts)
            {
                switch (State)
                {
                    case PSSpeechSynthesizerState.NotStarted:
                        throw new PSInvalidOperationException("Speech synthesizer was not started.");
                    case PSSpeechSynthesizerState.Paused:
                    case PSSpeechSynthesizerState.Speaking:
                        break;
                    default:
                        throw new PSInvalidOperationException("Speech synthesizer was already stopped.");
                }
                State = PSSpeechSynthesizerState.Stopping;
                
                if (_remainingPrompts.First != null)
                    _remainingPrompts.Clear();

                if (_speechSynthesizer.State != SynthesizerState.Ready)
                    _speechSynthesizer.SpeakAsyncCancelAll();
                task = Task.Factory.StartNew(() =>
                {
                    while (_speechSynthesizer != null)
                        Thread.Sleep(100);
                });
            }

            return task.Wait(new TimeSpan(0, 0, 30));
        }

        public void Pause()
        {
            lock (_remainingPrompts)
            {
                switch (State)
                {
                    case PSSpeechSynthesizerState.NotStarted:
                        throw new PSInvalidOperationException("Speech synthesizer was not started.");
                    case PSSpeechSynthesizerState.Paused:
                        throw new InvalidOperationException("Speech synthesizer is already paused.");
                    case PSSpeechSynthesizerState.Speaking:
                        break;
                    default:
                        throw new InvalidOperationException("Speech synthesizer is stopped.");
                }

                if (_speechSynthesizer.State == SynthesizerState.Speaking)
                    _speechSynthesizer.Pause();

                State = PSSpeechSynthesizerState.Paused;
            }
        }
        
        private Task<SpeechSynthesisAsyncResult> _waitTask = null;
        protected Task<SpeechSynthesisAsyncResult> WaitTask
        {
            get
            {
                lock (_remainingPrompts)
                {
                    if (_waitTask == null)
                    {
                        switch (State)
                        {
                            case PSSpeechSynthesizerState.NotStarted:
                                throw new PSInvalidOperationException("Speech synthesizer was not started.");
                            case PSSpeechSynthesizerState.Failed:
                            case PSSpeechSynthesizerState.Stopped:
                            case PSSpeechSynthesizerState.Success:
                                _waitTask = Task<SpeechSynthesisAsyncResult>.FromResult(new SpeechSynthesisAsyncResult(_output, _errors, _warnings, _informationMessages, _verboseMessages, _debugMessages, State == PSSpeechSynthesizerState.Stopped, State == PSSpeechSynthesizerState.Failed, SynchronizedData, UserState));
                                break;
                            default:
                                _waitTask = Task<SpeechSynthesisAsyncResult>.Factory.StartNew(() =>
                                {
                                    while (State != PSSpeechSynthesizerState.Failed && State != PSSpeechSynthesizerState.Stopped && State != PSSpeechSynthesizerState.Success)
                                        Thread.Sleep(100);
                                    return new SpeechSynthesisAsyncResult(_output, _errors, _warnings, _informationMessages, _verboseMessages, _debugMessages, State == PSSpeechSynthesizerState.Stopped, State == PSSpeechSynthesizerState.Failed, SynchronizedData, UserState);
                                });
                                break;
                        }
                    }
                }

                return _waitTask;
            }
        }

        public SpeechSynthesisAsyncResult Wait()
        {
            WaitTask.Wait();
            return WaitTask.Result;
        }

        public SpeechSynthesisAsyncResult Wait(int millisecondsTimeout)
        {
            if (WaitTask.Wait(millisecondsTimeout))
                return WaitTask.Result;
            return null;
        }

        public SpeechSynthesisAsyncResult Wait(TimeSpan timeout)
        {
            if (WaitTask.Wait(timeout))
                return WaitTask.Result;
            return null;
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
                        AddRange<InformationRecord>(_informationMessages, powershell.Streams.Information.ReadAll());
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

        private void RaiseSpeakStarted(Prompt prompt)
        {
            Dictionary<string, object> variables = new Dictionary<string, object>();
            variables.Add("Prompt", prompt);
            variables.Add("EventName", "SpeakStarted");
            InvokeScript(_onSpeakStarted, variables);
        }
        
        private void RaiseEvent(ScriptBlock scriptBlock, string eventName, AsyncCompletedEventArgs eventArgs)
        {
            if (eventArgs.Cancelled && State != PSSpeechSynthesizerState.Error)
                State = PSSpeechSynthesizerState.Stopping;
            if (eventArgs.Error != null)
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

        private void SpeechSynthesizer_VoiceChange(object sender, VoiceChangeEventArgs e)
        {
            lock (_remainingPrompts)
                Voice = e.Voice;
            RaiseEvent(_onVoiceChange, "VoiceChange", e);
        }

        private void SpeechSynthesizer_VisemeReached(object sender, VisemeReachedEventArgs e)
        {
            lock (_remainingPrompts)
            {
                AudioPosition = e.AudioPosition;
                Emphasis = e.Emphasis;
                Viseme = e.Viseme;
            }
            RaiseEvent(_onVisemeReached, "VisemeReached", e);
        }

        private void SpeechSynthesizer_StateChanged(object sender, StateChangedEventArgs e)
        {
            lock (_remainingPrompts)
            {
                switch (e.State)
                {
                    case SynthesizerState.Paused:
                        if (State == PSSpeechSynthesizerState.Speaking)
                            State = PSSpeechSynthesizerState.Paused;
                        break;
                    case SynthesizerState.Speaking:
                        if (State == PSSpeechSynthesizerState.NotStarted || State == PSSpeechSynthesizerState.Paused)
                            State = PSSpeechSynthesizerState.Speaking;
                        break;
                }
            }
            RaiseEvent(_onStateChanged, "StateChanged", e);
        }

        private void SpeechSynthesizer_PromptStarted(object sender, SpeakStartedEventArgs e) { RaiseEvent(_onPromptStarted, "SpeakStarted", e); }

        private void SpeechSynthesizer_SpeakProgress(object sender, SpeakProgressEventArgs e)
        {
            lock (_remainingPrompts)
            {
                AudioPosition = e.AudioPosition;
                CharacterCount = e.CharacterCount;
                CharacterPosition = e.CharacterPosition;
                Text = e.Text;
            }
            RaiseEvent(_onSpeakProgress, "SpeakProgress", e);
        }
        
        private void RaiseSpeakCompleted(AsyncCompletedEventArgs args)
        {
            try
            {
                lock (_remainingPrompts)
                {
                    try
                    {
                        if (_remainingPrompts.First != null)
                            _remainingPrompts.Clear();
                    }
                    catch { throw; }
                    finally
                    {
                        if (_speechSynthesizer != null)
                        {
                            if (_speechSynthesizer.State == SynthesizerState.Speaking)
                            {
                                if (State != PSSpeechSynthesizerState.Error)
                                    State = PSSpeechSynthesizerState.Stopping;
                                _speechSynthesizer.SpeakAsyncCancelAll();
                            }
                            else
                            {
                                _speechSynthesizer.SpeakStarted -= SpeechSynthesizer_PromptStarted;
                                _speechSynthesizer.StateChanged -= SpeechSynthesizer_StateChanged;
                                _speechSynthesizer.SpeakProgress -= SpeechSynthesizer_SpeakProgress;
                                _speechSynthesizer.BookmarkReached -= SpeechSynthesizer_BookmarkReached;
                                _speechSynthesizer.VoiceChange -= SpeechSynthesizer_VoiceChange;
                                _speechSynthesizer.VisemeReached -= SpeechSynthesizer_VisemeReached;
                                _speechSynthesizer.PhonemeReached -= SpeechSynthesizer_PhonemeReached;
                                _speechSynthesizer.SpeakCompleted -= SpeechSynthesizer_PromptCompleted;
                                _speechSynthesizer.Dispose();
                                _speechSynthesizer = null;
                            }
                        }
                    }
                }
            }
            catch { throw; }
            finally
            {
                lock (_remainingPrompts)
                {
                    if (_speechSynthesizer == null)
                    {
                        switch (State)
                        {
                            case PSSpeechSynthesizerState.Error:
                                State = PSSpeechSynthesizerState.Failed;
                                break;
                            case PSSpeechSynthesizerState.Stopping:
                                State = PSSpeechSynthesizerState.Stopped;
                                break;
                            case PSSpeechSynthesizerState.Failed:
                            case PSSpeechSynthesizerState.Stopped:
                            case PSSpeechSynthesizerState.Success:
                                break;
                            default:
                                if (args.Cancelled)
                                    State = PSSpeechSynthesizerState.Stopped;
                                else
                                    State = PSSpeechSynthesizerState.Success;
                                break;
                        }
                    }
                }
            }
            RaiseEvent(_onSpeakCompleted, "SpeakCompleted", args);
        }

        private void RaiseTerminalError(Exception exception, string errorId, ErrorCategory errorCategory, object targetObject, AsyncCompletedEventArgs args)
        {
            lock (_remainingPrompts)
            {
                try
                {
                    State = PSSpeechSynthesizerState.Error;
                    _errors.Add(new ErrorRecord(exception, "PromptStartError", ErrorCategory.InvalidOperation, targetObject));
                }
                catch { }
            }
            RaiseSpeakCompleted(args);
        }

        private void SpeakNextPrompt(AsyncCompletedEventArgs args)
        {
            bool wasLastPrompt = false;
            Exception error = null;
            Prompt prompt = null;
            lock (_remainingPrompts)
            {
                if (_speechSynthesizer == null || _remainingPrompts.First == null || (args != null && args.Cancelled))
                    wasLastPrompt = true;
                else if (State == PSSpeechSynthesizerState.Speaking)
                {
                    prompt = _remainingPrompts.First.Value;
                    _remainingPrompts.RemoveFirst();
                    _promptIndex++;
                    try
                    {
                        _invocationState.PromptIndex = _promptIndex;
                        _speechSynthesizer.SpeakAsync(prompt);
                    }
                    catch (Exception exception) { error = exception; }
                }
            }

            if (error != null)
                RaiseTerminalError(error, "PromptStartError", ErrorCategory.InvalidOperation, prompt, args);
            else if (wasLastPrompt)
                RaiseSpeakCompleted(args);
        }

        private void SpeechSynthesizer_PromptCompleted(object sender, SpeakCompletedEventArgs e)
        {
            try { RaiseEvent(_onPromptCompleted, "PromptCompleted", e); } catch { }
            SpeakNextPrompt(e);
        }

        private void SpeechSynthesizer_PhonemeReached(object sender, PhonemeReachedEventArgs e)
        {
            lock (_remainingPrompts)
            {
                AudioPosition = e.AudioPosition;
                Emphasis = e.Emphasis;
                Phoneme = e.Phoneme;
            }
            RaiseEvent(_onPhonemeReached, "PhonemeReached", e);
        }

        private void SpeechSynthesizer_BookmarkReached(object sender, BookmarkReachedEventArgs e)
        {
            lock (_remainingPrompts)
            {
                AudioPosition = e.AudioPosition;
                Bookmark = e.Bookmark;
            }
            RaiseEvent(_onBookmarkReached, "BookmarkReached", e);
        }
    }
}
