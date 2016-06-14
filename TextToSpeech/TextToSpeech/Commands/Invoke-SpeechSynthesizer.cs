using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Speech.Synthesis;
using System.Text;

namespace Erwine.Leonard.T.TextToSpeech.Commands
{
    [Cmdlet(VerbsLifecycle.Invoke, "SpeechSynthesizer", DefaultParameterSetName = "Prompt")]
    [OutputType(typeof(SpeechSynthesisResult))]
    public class Invoke_SpeechSynthesizer : PSCmdlet
    {
        private SpeechSynthesizer _speechSynthesizer = null;
        private bool _terminated = false;
        private SynthesizerState _state = SynthesizerState.Ready;
        private bool _cancelled = false;
        private Prompt _lastPrompt = null;
        private object _currentPrompt = null;
        private int _index = -1;
        private TimeSpan _audioPosition = TimeSpan.Zero;
        private int _characterPosition = 0;
        private int _characterCount = 0;
        private string _text = "";
        private VoiceInfo _voice = null;
        private string _bookmark = null;
        private string _phoneme = null;
        private int _viseme = 0;

        [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true, ParameterSetName = "Prompt")]
        public Prompt[] Prompt { get; set; }

        [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true, ParameterSetName = "PromptBuilder")]
        public PromptBuilder[] PromptBuilder { get; set; }

        [Parameter(Position = 1)]
        public SpeechSynthesizer SpeechSynthesizer { get; set; }

        [Parameter()]
        public object UserState { get; set; }

        [Parameter()]
        public SwitchParameter NoStateChangedEvents { get; set; }

        [Parameter()]
        public SwitchParameter NoSpeakStartedEvents { get; set; }

        [Parameter()]
        public SwitchParameter NoSpeakProgressEvents { get; set; }

        [Parameter()]
        public SwitchParameter NoVoiceChangeEvents { get; set; }

        [Parameter()]
        public SwitchParameter NoBookmarkReachedEvents { get; set; }

        [Parameter()]
        public SwitchParameter IncludePhonemeReachedEvents { get; set; }

        [Parameter()]
        public SwitchParameter IncludeVisemeReachedEvents { get; set; }

        [Parameter()]
        public SwitchParameter NoSpeakCompletedEvents { get; set; }

        [Parameter()]
        public SwitchParameter ErrorsAsEvents { get; set; }

        private void WriteEvent<TPrompt>(Exception error)
        {
            if (error != null && !ErrorsAsEvents)
                this.WriteError(new ErrorRecord(error, "PromptError", ErrorCategory.InvalidOperation, _currentPrompt));

            this.WriteObject(new SpeechSynthesisResult<TPrompt>
            {
                State = _state,
                AudioPosition = _audioPosition,
                Cancelled = _cancelled,
                Prompt = (TPrompt)_currentPrompt,
                Index = _index = -1,
                CharacterPosition = _characterPosition,
                CharacterCount = _characterCount,
                Text = _text,
                Voice = _voice,
                LastBookmark = _bookmark,
                Phoneme = _phoneme,
                Viseme = _viseme,
                Error = error
            });
        }

        private void WritePhonemeReachedEvent<TPrompt>(Exception error, TimeSpan duration, SynthesizerEmphasis emphasis, string nextPhoneme)
        {
            if (error != null && !ErrorsAsEvents)
                this.WriteError(new ErrorRecord(error, "PromptError", ErrorCategory.InvalidOperation, _currentPrompt));

            this.WriteObject(new SpeechSynthesisResult<TPrompt, string>
            {
                State = _state,
                AudioPosition = _audioPosition,
                Cancelled = _cancelled,
                Prompt = (TPrompt)_currentPrompt,
                Index = _index = -1,
                CharacterPosition = _characterPosition,
                CharacterCount = _characterCount,
                Text = _text,
                Voice = _voice,
                LastBookmark = _bookmark,
                Phoneme = _phoneme,
                Viseme = _viseme,
                Error = error,
                Duration = duration,
                Emphasis = emphasis,
                Next = nextPhoneme
            });
        }

        private void WriteVisemeReachedEvent<TPrompt>(Exception error, TimeSpan duration, SynthesizerEmphasis emphasis, int nextViseme)
        {
            if (error != null && !ErrorsAsEvents)
                this.WriteError(new ErrorRecord(error, "PromptError", ErrorCategory.InvalidOperation, _currentPrompt));

            this.WriteObject(new SpeechSynthesisResult<TPrompt, int>
            {
                State = _state,
                AudioPosition = _audioPosition,
                Cancelled = _cancelled,
                Prompt = (TPrompt)_currentPrompt,
                Index = _index = -1,
                CharacterPosition = _characterPosition,
                CharacterCount = _characterCount,
                Text = _text,
                Voice = _voice,
                LastBookmark = _bookmark,
                Phoneme = _phoneme,
                Viseme = _viseme,
                Error = error,
                Duration = duration,
                Emphasis = emphasis,
                Next = nextViseme
            });
        }

        protected override void BeginProcessing()
        {
            base.BeginProcessing();

            if ((_speechSynthesizer = this.SpeechSynthesizer) == null)
                _speechSynthesizer = new SpeechSynthesizer();

            _speechSynthesizer.StateChanged += SpeechSynthesizer_StateChanged;
            _speechSynthesizer.SpeakStarted += SpeechSynthesizer_SpeakStarted;
            _speechSynthesizer.SpeakProgress += SpeechSynthesizer_SpeakProgress;
            _speechSynthesizer.VoiceChange += SpeechSynthesizer_VoiceChange;
            _speechSynthesizer.BookmarkReached += SpeechSynthesizer_BookmarkReached;
            _speechSynthesizer.PhonemeReached += SpeechSynthesizer_PhonemeReached;
            _speechSynthesizer.VisemeReached += SpeechSynthesizer_VisemeReached;
            _speechSynthesizer.SpeakCompleted += SpeechSynthesizer_SpeakCompleted;
            _state = _speechSynthesizer.State;
            _voice = _speechSynthesizer.Voice;
        }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            lock (_speechSynthesizer)
            {
                if (_terminated)
                    return;

                if (this.ParameterSetName == "Prompt")
                {
                    foreach (Prompt prompt in this.Prompt)
                    {
                        _index++;
                        _currentPrompt = prompt;
                        _speechSynthesizer.Speak(prompt);
                    }
                }
                else
                {
                    foreach (PromptBuilder prompt in this.PromptBuilder)
                    {
                        _index++;
                        _currentPrompt = prompt;
                        _speechSynthesizer.Speak(prompt);
                    }
                }

                if (!_terminated)
                {
                    _terminated = true;
                    if (this.SpeechSynthesizer == null || !Object.ReferenceEquals(this.SpeechSynthesizer, _speechSynthesizer))
                        _speechSynthesizer.Dispose();
                }
            }
        }

        protected override void EndProcessing()
        {
            base.EndProcessing();

            lock (_speechSynthesizer)
            {
                if (!_terminated)
                {
                    _terminated = true;
                    if (_speechSynthesizer.State != SynthesizerState.Ready)
                        _speechSynthesizer.SpeakAsyncCancelAll();
                }
            }
        }

        protected override void StopProcessing()
        {
            base.StopProcessing();

            lock (_speechSynthesizer)
            {
                if (!_terminated)
                {
                    _terminated = true;
                    if (_speechSynthesizer.State != SynthesizerState.Ready)
                        _speechSynthesizer.SpeakAsyncCancelAll();
                }
            }
        }

        private void SpeechSynthesizer_StateChanged(object sender, StateChangedEventArgs e)
        {
            lock (_speechSynthesizer)
            {
                _state = e.State;
                if (this.ParameterSetName == "Prompt")
                    this.WriteEvent<Prompt>(null);
                else
                    this.WriteEvent<PromptBuilder>(null);
            }
        }

        private void SpeechSynthesizer_SpeakStarted(object sender, SpeakStartedEventArgs e)
        {
            lock (_speechSynthesizer)
            {
                _cancelled = e.Cancelled;
                _lastPrompt = e.Prompt;
                if (this.ParameterSetName == "Prompt")
                    this.WriteEvent<Prompt>(e.Error);
                else
                    this.WriteEvent<PromptBuilder>(e.Error);
            }
        }

        private void SpeechSynthesizer_SpeakProgress(object sender, SpeakProgressEventArgs e)
        {
            lock (_speechSynthesizer)
            {
                _cancelled = e.Cancelled;
                _lastPrompt = e.Prompt;
                _audioPosition = e.AudioPosition;
                _characterCount = e.CharacterCount;
                _characterPosition = e.CharacterPosition;
                _text = e.Text;
                if (this.ParameterSetName == "Prompt")
                    this.WriteEvent<Prompt>(e.Error);
                else
                    this.WriteEvent<PromptBuilder>(e.Error);
            }
        }

        private void SpeechSynthesizer_VoiceChange(object sender, VoiceChangeEventArgs e)
        {
            lock (_speechSynthesizer)
            {
                _cancelled = e.Cancelled;
                _lastPrompt = e.Prompt;
                _voice = e.Voice;
                if (this.ParameterSetName == "Prompt")
                    this.WriteEvent<Prompt>(e.Error);
                else
                    this.WriteEvent<PromptBuilder>(e.Error);
            }
        }

        private void SpeechSynthesizer_BookmarkReached(object sender, BookmarkReachedEventArgs e)
        {
            lock (_speechSynthesizer)
            {
                _cancelled = e.Cancelled;
                _lastPrompt = e.Prompt;
                _audioPosition = e.AudioPosition;
                _bookmark = e.Bookmark;
                if (this.ParameterSetName == "Prompt")
                    this.WriteEvent<Prompt>(e.Error);
                else
                    this.WriteEvent<PromptBuilder>(e.Error);
            }
        }

        private void SpeechSynthesizer_PhonemeReached(object sender, PhonemeReachedEventArgs e)
        {
            lock (_speechSynthesizer)
            {
                _cancelled = e.Cancelled;
                _lastPrompt = e.Prompt;
                _audioPosition = e.AudioPosition;
                _phoneme = e.Phoneme;
                if (this.ParameterSetName == "Prompt")
                    this.WritePhonemeReachedEvent<Prompt>(e.Error, e.Duration, e.Emphasis, e.NextPhoneme);
                else
                    this.WritePhonemeReachedEvent<PromptBuilder>(e.Error, e.Duration, e.Emphasis, e.NextPhoneme);
            }
        }

        private void SpeechSynthesizer_VisemeReached(object sender, VisemeReachedEventArgs e)
        {
            lock (_speechSynthesizer)
            {
                _cancelled = e.Cancelled;
                _lastPrompt = e.Prompt;
                _audioPosition = e.AudioPosition;
                _viseme = e.Viseme;
                if (this.ParameterSetName == "Prompt")
                    this.WriteVisemeReachedEvent<Prompt>(e.Error, e.Duration, e.Emphasis, e.NextViseme);
                else
                    this.WriteVisemeReachedEvent<PromptBuilder>(e.Error, e.Duration, e.Emphasis, e.NextViseme);
            }
        }

        private void SpeechSynthesizer_SpeakCompleted(object sender, SpeakCompletedEventArgs e)
        {
            lock (_speechSynthesizer)
            {
                _cancelled = e.Cancelled;
                _lastPrompt = e.Prompt;
                if (this.ParameterSetName == "Prompt")
                    this.WriteEvent<Prompt>(e.Error);
                else
                    this.WriteEvent<PromptBuilder>(e.Error);
            }
        }
    }
}
