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
        private Collection<PositionAndValue<string>> _BookmarksReached = new Collection<PositionAndValue<string>>();
        public ReadOnlyCollection<PositionAndValue<string>> BookmarksReached { get; private set; } }
        protected virtual void OnSpeakStarted(Prompt prompt)
        {
            throw new NotImplementedException();
        }

        protected virtual void OnSpeakCompleted(Prompt prompt)
        {
            throw new NotImplementedException();
        }

        protected virtual void OnPromptCanceled(Prompt prompt)
        {
            throw new NotImplementedException();
        }

        protected virtual void OnPromptError(Exception error, Prompt prompt)
        {
            throw new NotImplementedException();
        }

        protected virtual void OnVoiceChange(VoiceInfo voice)
        {
            throw new NotImplementedException();
        }

        protected virtual void OnStateChanged(SynthesizerState state, SynthesizerState previousState)
        {
            throw new NotImplementedException();
        }

        protected virtual void OnBookmarkReached(string bookmark, TimeSpan audioPosition)
        {
            throw new NotImplementedException();
        }

        protected virtual void OnSpeakProgress(string text, int characterPosition, int characterCount, TimeSpan audioPosition)
        {
            throw new NotImplementedException();
        }

        protected virtual void OnPhonemeReached(string phoneme, SynthesizerEmphasis emphasis, TimeSpan audioPosition, TimeSpan duration, string nextPhoneme)
        {
            throw new NotImplementedException();
        }

        protected virtual void OnVisemeReached(int viseme, SynthesizerEmphasis emphasis, TimeSpan audioPosition, TimeSpan duration, int nextViseme)
        {
            throw new NotImplementedException();
        }

        protected virtual void OnPromptEvent(PromptEventArgs args)
        {
            if (args.Cancelled)
                this.OnPromptCanceled(args.Prompt);

            if (args.Error != null)
                this.OnPromptError(args.Error, args.Prompt);
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

        public SpeechStatusForm()
        {
            InitializeComponent();
        }

        private void backButton_Click(object sender, EventArgs e)
        {


        }
    }
}
