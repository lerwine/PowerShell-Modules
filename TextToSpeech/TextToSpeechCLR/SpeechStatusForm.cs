using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TextToSpeechCLR
{
    public partial class SpeechStatusForm : Form
    {
        private object _syncRoot = new object();
        SpeechSynthesizer _speechSynthesizer = null;

        public SpeechSynthesizer SpeechSynthesizer
        {
            get { return _speechSynthesizer; }
            set
            {
                lock (_syncRoot)
                {
                    if ((_speechSynthesizer == null) ? value == null : value != null && Object.ReferenceEquals(value, _speechSynthesizer))
                        return;

                    if (_speechSynthesizer != null)
                    {
                        _speechSynthesizer.BookmarkReached -= SpeechSynthesizer_BookmarkReached;
                        _speechSynthesizer.PhonemeReached -= SpeechSynthesizer_PhonemeReached;
                        _speechSynthesizer.SpeakCompleted -= SpeechSynthesizer_SpeakCompleted;
                        _speechSynthesizer.SpeakProgress -= SpeechSynthesizer_SpeakProgress;
                        _speechSynthesizer.SpeakStarted -= SpeechSynthesizer_SpeakStarted;
                        _speechSynthesizer.StateChanged -= SpeechSynthesizer_StateChanged;
                        _speechSynthesizer.VisemeReached -= SpeechSynthesizer_VisemeReached;
                        _speechSynthesizer.VoiceChange -= SpeechSynthesizer_VoiceChange;
                    }

                    _speechSynthesizer = value;
                    if (value == null)
                        return;

                    _speechSynthesizer.BookmarkReached += SpeechSynthesizer_BookmarkReached;
                    _speechSynthesizer.PhonemeReached += SpeechSynthesizer_PhonemeReached;
                    _speechSynthesizer.SpeakCompleted += SpeechSynthesizer_SpeakCompleted;
                    _speechSynthesizer.SpeakProgress += SpeechSynthesizer_SpeakProgress;
                    _speechSynthesizer.SpeakStarted += SpeechSynthesizer_SpeakStarted;
                    _speechSynthesizer.StateChanged += SpeechSynthesizer_StateChanged;
                    _speechSynthesizer.VisemeReached += SpeechSynthesizer_VisemeReached;
                    _speechSynthesizer.VoiceChange += SpeechSynthesizer_VoiceChange;
                }
            }
        }
        private long _eventIndex = 0;
        private Collection<IEventRecord> _eventLog = new Collection<IEventRecord>();
        public ReadOnlyCollection<IEventRecord> EventLog { get; private set; }

        public SpeechStatusForm()
        {
            EventLog = new ReadOnlyCollection<IEventRecord>(_eventLog);
            InitializeComponent();
        }

        protected virtual void OnSpeakStarted(Prompt prompt)
        {
            lock (_eventLog)
                _eventLog.Add(new MilestoneEvent(_eventIndex++, "Speak Started"));
        }

        protected virtual void OnSpeakCompleted(Prompt prompt)
        {
            lock (_eventLog)
                _eventLog.Add(new MilestoneEvent(_eventIndex++, "Speak Completed"));
        }

        protected virtual void OnPromptCanceled(Prompt prompt)
        {
            lock (_eventLog)
                _eventLog.Add(new MilestoneEvent(_eventIndex++, "Prompt Canceled"));

        }

        protected virtual void OnPromptError(Exception error, Prompt prompt)
        {
            lock (_eventLog)
                _eventLog.Add(new ErrorEvent(_eventIndex++, error));
        }

        protected virtual void OnVoiceChange(VoiceInfo voice)
        {
            lock (_eventLog)
                _eventLog.Add(new ChangeEvent<VoiceInfo>(_eventIndex++, voice, String.Format("Voice change: {0}", voice.Name)));
        }

        protected virtual void OnStateChanged(SynthesizerState state, SynthesizerState previousState)
        {
            lock (_eventLog)
                _eventLog.Add(new ChangeEvent<SynthesizerState>(_eventIndex++, state, String.Format("State change: {0}", state.ToString("F"))));
        }

        protected virtual void OnBookmarkReached(string bookmark, TimeSpan audioPosition)
        {
            lock (_eventLog)
                _eventLog.Add(new PositionAndValue<string>(audioPosition, _eventIndex++, String.Format("Bookmark Reached: {0}", bookmark), bookmark));
        }

        protected virtual void OnSpeakProgress(string text, int characterPosition, int characterCount, TimeSpan audioPosition)
        {

        }

        protected virtual void OnPhonemeReached(string phoneme, SynthesizerEmphasis emphasis, TimeSpan audioPosition, TimeSpan duration, string nextPhoneme)
        {

        }

        protected virtual void OnVisemeReached(int viseme, SynthesizerEmphasis emphasis, TimeSpan audioPosition, TimeSpan duration, int nextViseme)
        {

        }

        protected virtual void OnPromptEvent(PromptEventArgs args)
        {
            if (args.Cancelled)
                this.OnPromptCanceled(args.Prompt);

            if (args.Error != null)
                this.OnPromptError(args.Error, args.Prompt);
        }

        private void SpeechStatusForm_Shown(object sender, EventArgs e)
        {

        }

        private void SpeechSynthesizer_VoiceChange(object sender, VoiceChangeEventArgs e)
        {
            this.OnPromptEvent(e);
            this.OnVoiceChange(e.Voice);
        }

        private void SpeechSynthesizer_VisemeReached(object sender, VisemeReachedEventArgs e)
        {
            this.OnPromptEvent(e);
            this.OnVisemeReached(e.Viseme, e.Emphasis, e.AudioPosition, e.Duration, e.NextViseme);
        }

        private void SpeechSynthesizer_StateChanged(object sender, StateChangedEventArgs e)
        {
            this.OnStateChanged(e.State, e.PreviousState);
        }

        private void SpeechSynthesizer_SpeakStarted(object sender, SpeakStartedEventArgs e)
        {
            this.OnPromptEvent(e);
            this.OnSpeakStarted(e.Prompt);
        }

        private void SpeechSynthesizer_SpeakProgress(object sender, SpeakProgressEventArgs e)
        {
            this.OnPromptEvent(e);
            this.OnSpeakProgress(e.Text, e.CharacterPosition, e.CharacterCount, e.AudioPosition);
        }

        private void SpeechSynthesizer_SpeakCompleted(object sender, SpeakCompletedEventArgs e)
        {
            this.OnPromptEvent(e);
            this.OnSpeakCompleted(e.Prompt);
        }

        private void SpeechSynthesizer_PhonemeReached(object sender, PhonemeReachedEventArgs e)
        {
            this.OnPromptEvent(e);
            this.OnPhonemeReached(e.Phoneme, e.Emphasis, e.AudioPosition, e.Duration, e.NextPhoneme);
        }

        private void SpeechSynthesizer_BookmarkReached(object sender, BookmarkReachedEventArgs e)
        {
            this.OnPromptEvent(e);
            this.OnBookmarkReached(e.Bookmark, e.AudioPosition);
        }

        private void backButton_Click(object sender, EventArgs e)
        {


        }

        private void nextButton_Click(object sender, EventArgs e)
        {

        }

        private void pauseButton_Click(object sender, EventArgs e)
        {

        }

        private void resumeButton_Click(object sender, EventArgs e)
        {

        }

        private void startButton_Click(object sender, EventArgs e)
        {

        }

        private void stopButton_Click(object sender, EventArgs e)
        {

        }

        private void SpeechStatusForm_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void SpeechStatusForm_FormClosed(object sender, FormClosedEventArgs e)
        {

        }

        private void deleteQueueEntryButton_Click(object sender, EventArgs e)
        {

        }
    }
}
