using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Speech.AudioFormat;
using System.Speech.Synthesis;
using System.Text;
using System.Threading;

namespace PSTTS
{
    public class SpeechEventHandler
    {
        private List<SpeechContext> _speechInProgress = new List<SpeechContext>();
        private SpeechEventLog _log = new SpeechEventLog();

        public IList<SpeechEventLogItem> GetSpeechEventData()
        {
            Monitor.Enter(_log.SyncRoot);
            try {
                Collection<SpeechEventLogItem> result = new Collection<SpeechEventLogItem>(_log);
                _log.Clear();
                return result;
            } finally { Monitor.Exit(_log.SyncRoot); }
        }

        public Collection<SpeechEventLogItem> GetSpeechEventData(object key)
        {
            Monitor.Enter(_log.SyncRoot);
            try
            {
                Collection<SpeechEventLogItem> result = new Collection<SpeechEventLogItem>(_log.Where(o => ReferenceEquals(key, o.Key)).ToList());
                foreach (SpeechEventLogItem obj in result)
                    _log.Remove(obj);
                return result;
            }
            finally { Monitor.Exit(_log.SyncRoot); }
        }

        public SpeechEventHandler()
        {
            SpeechSynthesizer synthesizer = new SpeechSynthesizer();
            PromptBuilder pb = new PromptBuilder();
        }

        #region StartSpeechAsync

        private void _StartSpeechAsync(PromptBuilder promptBuilder, int? rate, object voice, int? volume, object state, Action<SpeechContext> startSpeech)
        {
            SpeechContext context = new SpeechContext(promptBuilder, state);
            if (voice != null)
            {
                if (voice is VoiceGender)
                    context.SpeechSynthesizer.SelectVoiceByHints((VoiceGender)voice);
                else if (voice is object[])
                {
                    object[] arr = voice as object[];
                    if (arr.Length > 0 && arr[0] != null && arr[0] is VoiceGender)
                    {
                        if (arr.Length == 1)
                            context.SpeechSynthesizer.SelectVoiceByHints((VoiceGender)(arr[0]));
                        else if (arr.Length > 1 && arr[1] != null && arr[1] is VoiceAge)
                            context.SpeechSynthesizer.SelectVoiceByHints((VoiceGender)(arr[0]), (VoiceAge)(arr[1]));
                    }
                }
                else if (voice is string)
                {
                    string s = voice as string;
                    if (s.Length > 0)
                        context.SpeechSynthesizer.SelectVoice(s);
                }
            }
            if (rate.HasValue)
                context.SpeechSynthesizer.Rate = rate.Value;
            if (volume.HasValue)
                context.SpeechSynthesizer.Volume = volume.Value;
            lock (_speechInProgress)
            {
                _speechInProgress.Add(context);
                context.SpeechSynthesizer.BookmarkReached += SpeechSynthesizer_BookmarkReached;
                context.SpeechSynthesizer.PhonemeReached += SpeechSynthesizer_PhonemeReached;
                context.SpeechSynthesizer.SpeakCompleted += SpeechSynthesizer_SpeakCompleted;
                context.SpeechSynthesizer.SpeakProgress += SpeechSynthesizer_SpeakProgress;
                context.SpeechSynthesizer.SpeakStarted += SpeechSynthesizer_SpeakStarted;
                context.SpeechSynthesizer.StateChanged += SpeechSynthesizer_StateChanged;
                context.SpeechSynthesizer.VisemeReached += SpeechSynthesizer_VisemeReached;
                context.SpeechSynthesizer.VoiceChange += SpeechSynthesizer_VoiceChange;
            }
            try { startSpeech(context); }
            catch
            {
                lock (_speechInProgress)
                {
                    _speechInProgress.Remove(context);
                    context.SpeechSynthesizer.BookmarkReached -= SpeechSynthesizer_BookmarkReached;
                    context.SpeechSynthesizer.PhonemeReached -= SpeechSynthesizer_PhonemeReached;
                    context.SpeechSynthesizer.SpeakCompleted -= SpeechSynthesizer_SpeakCompleted;
                    context.SpeechSynthesizer.SpeakProgress -= SpeechSynthesizer_SpeakProgress;
                    context.SpeechSynthesizer.SpeakStarted -= SpeechSynthesizer_SpeakStarted;
                    context.SpeechSynthesizer.StateChanged -= SpeechSynthesizer_StateChanged;
                    context.SpeechSynthesizer.VisemeReached -= SpeechSynthesizer_VisemeReached;
                    context.SpeechSynthesizer.VoiceChange -= SpeechSynthesizer_VoiceChange;
                }
                context.Dispose();
                throw;
            }
        }

        private void _StartSpeechAsync(PromptBuilder promptBuilder, int? rate, object voice, int? volume, Stream audioDestination, SpeechAudioFormatInfo formatInfo, object state)
        {
            _StartSpeechAsync(promptBuilder, rate, voice, volume, state, context =>
            {
                if (formatInfo == null)
                    context.SpeechSynthesizer.SetOutputToWaveStream(audioDestination);
                else
                    context.SpeechSynthesizer.SetOutputToAudioStream(audioDestination, formatInfo);
                context.SpeechSynthesizer.SpeakAsync(promptBuilder);
            });
        }

        private void _StartSpeechAsync(PromptBuilder promptBuilder, int? rate, object voice, int? volume, string path, SpeechAudioFormatInfo formatInfo, object state)
        {
            _StartSpeechAsync(promptBuilder, rate, voice, volume, state, context =>
            {
                if (formatInfo == null)
                    context.SpeechSynthesizer.SetOutputToWaveFile(path);
                else
                    context.SpeechSynthesizer.SetOutputToWaveFile(path, formatInfo);
                context.SpeechSynthesizer.SpeakAsync(promptBuilder);
            });
        }

        private void _StartSpeechAsync(PromptBuilder promptBuilder, int? rate, object voice, int? volume, object state)
        {
            _StartSpeechAsync(promptBuilder, rate, voice, volume, state, context =>
            {
                context.SpeechSynthesizer.SetOutputToDefaultAudioDevice();
                context.SpeechSynthesizer.SpeakAsync(promptBuilder);
            });
        }

        public void StartSpeechAsync(PromptBuilder promptBuilder, string voice, object state)
        {
            _StartSpeechAsync(promptBuilder, null, voice, null, state);
        }

        public void StartSpeechAsync(PromptBuilder promptBuilder, VoiceGender gender, object state)
        {
            _StartSpeechAsync(promptBuilder, null, gender, null, state);
        }

        public void StartSpeechAsync(PromptBuilder promptBuilder, VoiceGender gender, int volume, object state)
        {
            _StartSpeechAsync(promptBuilder, null, gender, volume, state);
        }

        public void StartSpeechAsync(PromptBuilder promptBuilder, string voice, int volume, object state)
        {
            _StartSpeechAsync(promptBuilder, null, voice, volume, state);
        }

        public void StartSpeechAsync(PromptBuilder promptBuilder, int rate, VoiceGender gender, object state)
        {
            _StartSpeechAsync(promptBuilder, rate, gender, null, state);
        }

        public void StartSpeechAsync(PromptBuilder promptBuilder, int rate, string voice, object state)
        {
            _StartSpeechAsync(promptBuilder, rate, voice, null, state);
        }

        public void StartSpeechAsync(PromptBuilder promptBuilder, VoiceGender gender, VoiceAge age, object state)
        {
            _StartSpeechAsync(promptBuilder, null, new object[] { gender, age }, null, state);
        }

        public void StartSpeechAsync(PromptBuilder promptBuilder, string voice, string path, object state)
        {
            _StartSpeechAsync(promptBuilder, null, voice, null, path, null, state);
        }

        public void StartSpeechAsync(PromptBuilder promptBuilder, string voice, Stream audioDestination, object state)
        {
            _StartSpeechAsync(promptBuilder, null, voice, null, audioDestination, null, state);
        }

        public void StartSpeechAsync(PromptBuilder promptBuilder, VoiceGender gender, Stream audioDestination, object state)
        {
            _StartSpeechAsync(promptBuilder, null, gender, null, audioDestination, null, state);
        }

        public void StartSpeechAsync(PromptBuilder promptBuilder, VoiceGender gender, string path, object state)
        {
            _StartSpeechAsync(promptBuilder, null, gender, null, path, null, state);
        }

        public void StartSpeechAsync(PromptBuilder promptBuilder, string voice, int volume, Stream audioDestination, object state)
        {
            _StartSpeechAsync(promptBuilder, null, voice, volume, audioDestination, null, state);
        }

        public void StartSpeechAsync(PromptBuilder promptBuilder, VoiceGender gender, VoiceAge age, string path, object state)
        {
            _StartSpeechAsync(promptBuilder, null, new object[] { gender, age }, null, path, null, state);
        }

        public void StartSpeechAsync(PromptBuilder promptBuilder, string voice, string path, SpeechAudioFormatInfo formatInfo, object state)
        {
            _StartSpeechAsync(promptBuilder, null, voice, null, path, formatInfo, state);
        }

        public void StartSpeechAsync(PromptBuilder promptBuilder, string voice, Stream audioDestination, SpeechAudioFormatInfo formatInfo, object state)
        {
            _StartSpeechAsync(promptBuilder, null, voice, null, audioDestination, formatInfo, state);
        }

        public void StartSpeechAsync(PromptBuilder promptBuilder, VoiceGender gender, string path, SpeechAudioFormatInfo formatInfo, object state)
        {
            _StartSpeechAsync(promptBuilder, null, gender, null, path, formatInfo, state);
        }

        public void StartSpeechAsync(PromptBuilder promptBuilder, VoiceGender gender, Stream audioDestination, SpeechAudioFormatInfo formatInfo, object state)
        {
            _StartSpeechAsync(promptBuilder, null, gender, null, audioDestination, formatInfo, state);
        }

        public void StartSpeechAsync(PromptBuilder promptBuilder, int rate, string voice, string path, object state)
        {
            _StartSpeechAsync(promptBuilder, rate, voice, null, path, null, state);
        }

        public void StartSpeechAsync(PromptBuilder promptBuilder, int rate, string voice, Stream audioDestination, object state)
        {
            _StartSpeechAsync(promptBuilder, rate, voice, null, audioDestination, null, state);
        }

        public void StartSpeechAsync(PromptBuilder promptBuilder, int rate, VoiceGender gender, string path, object state)
        {
            _StartSpeechAsync(promptBuilder, rate, gender, null, path, null, state);
        }

        public void StartSpeechAsync(PromptBuilder promptBuilder, VoiceGender gender, int volume, Stream audioDestination, object state)
        {
            _StartSpeechAsync(promptBuilder, null, gender, volume, audioDestination, null, state);
        }

        public void StartSpeechAsync(PromptBuilder promptBuilder, string voice, int volume, string path, object state)
        {
            _StartSpeechAsync(promptBuilder, null, voice, volume, path, null, state);
        }

        public void StartSpeechAsync(PromptBuilder promptBuilder, VoiceGender gender, int volume, string path, object state)
        {
            _StartSpeechAsync(promptBuilder, null, gender, volume, path, null, state);
        }

        public void StartSpeechAsync(PromptBuilder promptBuilder, int rate, VoiceGender gender, Stream audioDestination, object state)
        {
            _StartSpeechAsync(promptBuilder, rate, gender, null, audioDestination, null, state);
        }

        public void StartSpeechAsync(PromptBuilder promptBuilder, int rate, string voice, int volume, object state)
        {
            _StartSpeechAsync(promptBuilder, rate, voice, volume, state);
        }

        public void StartSpeechAsync(PromptBuilder promptBuilder, VoiceGender gender, VoiceAge age, Stream audioDestination, object state)
        {
            _StartSpeechAsync(promptBuilder, null, new object[] { gender, age }, null, audioDestination, null, state);
        }

        public void StartSpeechAsync(PromptBuilder promptBuilder, int rate, VoiceGender gender, int volume, object state)
        {
            _StartSpeechAsync(promptBuilder, rate, gender, volume, state);
        }

        public void StartSpeechAsync(PromptBuilder promptBuilder, VoiceGender gender, VoiceAge age, int volume, object state)
        {
            _StartSpeechAsync(promptBuilder, null, new object[] { gender, age }, volume, state);
        }

        public void StartSpeechAsync(PromptBuilder promptBuilder, int rate, VoiceGender gender, VoiceAge age, object state)
        {
            _StartSpeechAsync(promptBuilder, rate, new object[] { gender, age }, null, state);
        }

        public void StartSpeechAsync(PromptBuilder promptBuilder, int rate, VoiceGender gender, VoiceAge age, int volume, object state)
        {
            _StartSpeechAsync(promptBuilder, rate, new object[] { gender, age }, volume, state);
        }

        public void StartSpeechAsync(PromptBuilder promptBuilder, VoiceGender gender, VoiceAge age, int volume, Stream audioDestination, object state)
        {
            _StartSpeechAsync(promptBuilder, null, new object[] { gender, age }, volume, audioDestination, null, state);
        }

        public void StartSpeechAsync(PromptBuilder promptBuilder, VoiceGender gender, VoiceAge age, int volume, string path, object state)
        {
            _StartSpeechAsync(promptBuilder, null, new object[] { gender, age }, volume, path, null, state);
        }

        public void StartSpeechAsync(PromptBuilder promptBuilder, int rate, VoiceGender gender, VoiceAge age, Stream audioDestination, object state)
        {
            _StartSpeechAsync(promptBuilder, rate, new object[] { gender, age }, null, audioDestination, null, state);
        }

        public void StartSpeechAsync(PromptBuilder promptBuilder, int rate, VoiceGender gender, VoiceAge age, string path, object state)
        {
            _StartSpeechAsync(promptBuilder, rate, new object[] { gender, age }, null, path, null, state);
        }

        public void StartSpeechAsync(PromptBuilder promptBuilder, int rate, string voice, string path, SpeechAudioFormatInfo formatInfo, object state)
        {
            _StartSpeechAsync(promptBuilder, rate, voice, null, path, formatInfo, state);
        }

        public void StartSpeechAsync(PromptBuilder promptBuilder, int rate, string voice, Stream audioDestination, SpeechAudioFormatInfo formatInfo, object state)
        {
            _StartSpeechAsync(promptBuilder, rate, voice, null, audioDestination, formatInfo, state);
        }

        public void StartSpeechAsync(PromptBuilder promptBuilder, VoiceGender gender, int volume, Stream audioDestination, SpeechAudioFormatInfo formatInfo, object state)
        {
            _StartSpeechAsync(promptBuilder, null, gender, volume, audioDestination, formatInfo, state);
        }

        public void StartSpeechAsync(PromptBuilder promptBuilder, int rate, VoiceGender gender, int volume, Stream audioDestination, object state)
        {
            _StartSpeechAsync(promptBuilder, rate, gender, volume, audioDestination, null, state);
        }

        public void StartSpeechAsync(PromptBuilder promptBuilder, int rate, VoiceGender gender, string path, SpeechAudioFormatInfo formatInfo, object state)
        {
            _StartSpeechAsync(promptBuilder, rate, gender, null, path, formatInfo, state);
        }

        public void StartSpeechAsync(PromptBuilder promptBuilder, int rate, string voice, int volume, Stream audioDestination, object state)
        {
            _StartSpeechAsync(promptBuilder, rate, voice, volume, audioDestination, null, state);
        }

        public void StartSpeechAsync(PromptBuilder promptBuilder, VoiceGender gender, int volume, string path, SpeechAudioFormatInfo formatInfo, object state)
        {
            _StartSpeechAsync(promptBuilder, null, gender, volume, path, formatInfo, state);
        }

        public void StartSpeechAsync(PromptBuilder promptBuilder, int rate, string voice, int volume, string path, object state)
        {
            _StartSpeechAsync(promptBuilder, rate, voice, volume, path, null, state);
        }

        public void StartSpeechAsync(PromptBuilder promptBuilder, VoiceGender gender, VoiceAge age, Stream audioDestination, SpeechAudioFormatInfo formatInfo, object state)
        {
            _StartSpeechAsync(promptBuilder, null, new object[] { gender, age }, null, audioDestination, formatInfo, state);
        }

        public void StartSpeechAsync(PromptBuilder promptBuilder, VoiceGender gender, VoiceAge age, string path, SpeechAudioFormatInfo formatInfo, object state)
        {
            _StartSpeechAsync(promptBuilder, null, new object[] { gender, age }, null, path, formatInfo, state);
        }

        public void StartSpeechAsync(PromptBuilder promptBuilder, int rate, VoiceGender gender, Stream audioDestination, SpeechAudioFormatInfo formatInfo, object state)
        {
            _StartSpeechAsync(promptBuilder, rate, gender, null, audioDestination, formatInfo, state);
        }

        public void StartSpeechAsync(PromptBuilder promptBuilder, string voice, int volume, Stream audioDestination, SpeechAudioFormatInfo formatInfo, object state)
        {
            _StartSpeechAsync(promptBuilder, null, voice, volume, audioDestination, formatInfo, state);
        }

        public void StartSpeechAsync(PromptBuilder promptBuilder, int rate, VoiceGender gender, int volume, string path, object state)
        {
            _StartSpeechAsync(promptBuilder, rate, gender, volume, path, null, state);
        }

        public void StartSpeechAsync(PromptBuilder promptBuilder, string voice, int volume, string path, SpeechAudioFormatInfo formatInfo, object state)
        {
            _StartSpeechAsync(promptBuilder, null, voice, volume, path, formatInfo, state);
        }

        public void StartSpeechAsync(PromptBuilder promptBuilder, VoiceGender gender, VoiceAge age, int volume, Stream audioDestination, SpeechAudioFormatInfo formatInfo, object state)
        {
            _StartSpeechAsync(promptBuilder, null, new object[] { gender, age }, volume, audioDestination, formatInfo, state);
        }

        public void StartSpeechAsync(PromptBuilder promptBuilder, int rate, string voice, int volume, string path, SpeechAudioFormatInfo formatInfo, object state)
        {
            _StartSpeechAsync(promptBuilder, rate, voice, volume, path, formatInfo, state);
        }

        public void StartSpeechAsync(PromptBuilder promptBuilder, int rate, string voice, int volume, Stream audioDestination, SpeechAudioFormatInfo formatInfo, object state)
        {
            _StartSpeechAsync(promptBuilder, rate, voice, volume, audioDestination, formatInfo, state);
        }

        public void StartSpeechAsync(PromptBuilder promptBuilder, VoiceGender gender, VoiceAge age, int volume, string path, SpeechAudioFormatInfo formatInfo, object state)
        {
            _StartSpeechAsync(promptBuilder, null, new object[] { gender, age }, volume, path, formatInfo, state);
        }

        public void StartSpeechAsync(PromptBuilder promptBuilder, int rate, VoiceGender gender, int volume, Stream audioDestination, SpeechAudioFormatInfo formatInfo, object state)
        {
            _StartSpeechAsync(promptBuilder, rate, gender, volume, audioDestination, formatInfo, state);
        }

        public void StartSpeechAsync(PromptBuilder promptBuilder, int rate, VoiceGender gender, VoiceAge age, Stream audioDestination, SpeechAudioFormatInfo formatInfo, object state)
        {
            _StartSpeechAsync(promptBuilder, rate, new object[] { gender, age }, null, audioDestination, formatInfo, state);
        }

        public void StartSpeechAsync(PromptBuilder promptBuilder, int rate, VoiceGender gender, VoiceAge age, string path, SpeechAudioFormatInfo formatInfo, object state)
        {
            _StartSpeechAsync(promptBuilder, rate, new object[] { gender, age }, null, path, formatInfo, state);
        }

        public void StartSpeechAsync(PromptBuilder promptBuilder, int rate, VoiceGender gender, VoiceAge age, int volume, Stream audioDestination, object state)
        {
            _StartSpeechAsync(promptBuilder, rate, new object[] { gender, age }, volume, audioDestination, null, state);
        }

        public void StartSpeechAsync(PromptBuilder promptBuilder, int rate, VoiceGender gender, VoiceAge age, int volume, string path, object state)
        {
            _StartSpeechAsync(promptBuilder, rate, new object[] { gender, age }, volume, path, null, state);
        }

        public void StartSpeechAsync(PromptBuilder promptBuilder, int rate, VoiceGender gender, int volume, string path, SpeechAudioFormatInfo formatInfo, object state)
        {
            _StartSpeechAsync(promptBuilder, rate, gender, volume, path, formatInfo, state);
        }

        public void StartSpeechAsync(PromptBuilder promptBuilder, int rate, VoiceGender gender, VoiceAge age, int volume, Stream audioDestination, SpeechAudioFormatInfo formatInfo, object state)
        {
            _StartSpeechAsync(promptBuilder, rate, new object[] { gender, age }, volume, audioDestination, formatInfo, state);
        }

        public void StartSpeechAsync(PromptBuilder promptBuilder, int rate, VoiceGender gender, VoiceAge age, int volume, string path, SpeechAudioFormatInfo formatInfo, object state)
        {
            _StartSpeechAsync(promptBuilder, rate, new object[] { gender, age }, volume, path, formatInfo, state);
        }

        #endregion

        public const string NotePropertyName_Key = "Key";
        //public const string NotePropertyName_Rate = "Rate";
        //public const string NotePropertyName_Voice = "Voice";
        //public const string NotePropertyName_Volume = "Volume";
        //public const string NotePropertyName_AudioPosition = "AudioPosition";
        //public const string NotePropertyName_Bookmark = "Bookmark";
        //public const string NotePropertyName_Phoneme = "Phoneme";
        //public const string NotePropertyName_Emphasis = "Emphasis";
        //public const string NotePropertyName_CharacterCount = "CharacterCount";
        //public const string NotePropertyName_CharacterPosition = "CharacterPosition";
        //public const string NotePropertyName_Text = "Text";
        public const string NotePropertyName_State = "State";
        public const string NotePropertyName_UserState = "UserState";

        private void SpeechSynthesizer_BookmarkReached(object sender, BookmarkReachedEventArgs e)
        {
            SpeechContext context;
            lock (_speechInProgress)
                context = _speechInProgress.FirstOrDefault(i => ReferenceEquals(i.SpeechSynthesizer, sender));
            if (context != null)
                _log.Add(new BookmarkReachedEventData(e, context));
        }

        private void SpeechSynthesizer_PhonemeReached(object sender, PhonemeReachedEventArgs e)
        {
            SpeechContext context;
            lock (_speechInProgress)
                context = _speechInProgress.FirstOrDefault(i => ReferenceEquals(i.SpeechSynthesizer, sender));
            if (context != null)
                _log.Add(new PhonemeReachedEventData(e, context));
        }

        private void SpeechSynthesizer_SpeakProgress(object sender, SpeakProgressEventArgs e)
        {
            SpeechContext context;
            lock (_speechInProgress)
                context = _speechInProgress.FirstOrDefault(i => ReferenceEquals(i.SpeechSynthesizer, sender));
            if (context != null)
                _log.Add(new SpeakProgressEventData(e, context));
        }

        private void SpeechSynthesizer_SpeakStarted(object sender, SpeakStartedEventArgs e)
        {
            SpeechContext context;
            lock (_speechInProgress)
                context = _speechInProgress.FirstOrDefault(i => ReferenceEquals(i.SpeechSynthesizer, sender));
            if (context != null)
                _log.Add(new SpeakStartedEventData(e, context));
        }

        private void SpeechSynthesizer_StateChanged(object sender, StateChangedEventArgs e)
        {
            SpeechContext context;
            lock (_speechInProgress)
                context = _speechInProgress.FirstOrDefault(i => ReferenceEquals(i.SpeechSynthesizer, sender));
            if (context != null)
                _log.Add(new StateChangedEventData(e, context));
        }

        private void SpeechSynthesizer_VisemeReached(object sender, VisemeReachedEventArgs e)
        {
            SpeechContext context;
            lock (_speechInProgress)
                context = _speechInProgress.FirstOrDefault(i => ReferenceEquals(i.SpeechSynthesizer, sender));
            if (context != null)
                _log.Add(new VisemeReachedEventData(e, context));
        }

        private void SpeechSynthesizer_VoiceChange(object sender, VoiceChangeEventArgs e)
        {
            SpeechContext context;
            lock (_speechInProgress)
                context = _speechInProgress.FirstOrDefault(i => ReferenceEquals(i.SpeechSynthesizer, sender));
            if (context != null)
                _log.Add(new VoiceChangeEventData(e, context));
        }

        private void SpeechSynthesizer_SpeakCompleted(object sender, SpeakCompletedEventArgs e)
        {
            SpeechContext context;
            lock (_speechInProgress)
            {
                context = _speechInProgress.FirstOrDefault(i => ReferenceEquals(i.SpeechSynthesizer, sender));
                if (context == null)
                    return;
                _speechInProgress.Remove(context);
            }
            _log.Add(new SpeakCompletedEventData(e, context));
            context.Dispose();
        }

        public class SpeechContext : IDisposable
        {   
            private PromptBuilder _promptBuilder;
            private object _key = new object();
            private object _state;
            private SpeechSynthesizer _speechSynthesizer = new SpeechSynthesizer();
            private AutoResetEvent _bookmarkReachedEvent = new AutoResetEvent(false);
            private AutoResetEvent _speechCompletedEvent = new AutoResetEvent(false);
            private AutoResetEvent _speechProgressEvent = new AutoResetEvent(false);
            private AutoResetEvent _speechEvent = new AutoResetEvent(false);
            /// <summary>
            /// Gets raised when a bookmark is reached or speech is completed.
            /// </summary>
            internal AutoResetEvent BookmarkReachedEvent { get { return _bookmarkReachedEvent; } }
            /// <summary>
            /// Gets raised when speech is completed.
            /// </summary>
            internal AutoResetEvent SpeechCompletedEvent { get { return _speechCompletedEvent; } }
            /// <summary>
            /// Gets raised when progreses has changed or or speech is completed.
            /// </summary>
            internal AutoResetEvent SpeechProgressEvent { get { return _speechProgressEvent; } }
            /// <summary>
            /// Gets raised when a bookmark is reached,  progreses has changed, or speech is completed.
            /// </summary>
            internal AutoResetEvent SpeechEvent { get { return _speechEvent; } }
            internal PromptBuilder PromptBuilder { get { return _promptBuilder; } }
            internal object State { get { return _state; } }
            internal object Key { get { return _key; } }
            internal SpeechSynthesizer SpeechSynthesizer { get { return _speechSynthesizer; } }

            internal SpeechContext(PromptBuilder promptBuilder, object state)
            {
                _promptBuilder = promptBuilder;
                _state = state;
            }

            #region IDisposable Support

            private bool _disposedValue = false; // To detect redundant calls

            protected virtual void Dispose(bool disposing)
            {
                if (_disposedValue)
                    return;

                if (disposing)
                    _speechSynthesizer.Dispose();
            }

            // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
            // ~SpeechContext() {
            //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            //   Dispose(false);
            // }

            // This code added to correctly implement the disposable pattern.
            public void Dispose()
            {
                // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
                Dispose(true);
                // TODO: uncomment the following line if the finalizer is overridden above.
                // GC.SuppressFinalize(this);
            }

            #endregion
        }
    }
}
