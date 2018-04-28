using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Management.Automation;
using System.Speech.AudioFormat;
using System.Speech.Synthesis;
using System.Threading;

namespace Speech
{
    /// <summary>
    /// Manages a Text-To-Speech invocation
    /// </summary>
    public class TextToSpeechJob : IDisposable, IAsyncResult
    {
        private string _path;
        private SpeechAudioFormatInfo _formatInfo;
        private int _rate = 0;
        private int _volume = 0;
        private VoiceInfo _voice = null;
        private object _syncRoot = new object();
        private object _asyncState = null;
        private EventArgs _lastStatusEvent = EventArgs.Empty;
        private SynthesizerState _state = SynthesizerState.Paused;
        private SpeechSynthesizer _speechSynthesizer = new SpeechSynthesizer();
        private AutoResetEvent _speechProgressEvent = new AutoResetEvent(false);
        private ManualResetEvent _speechCompletedEvent = new ManualResetEvent(false);
        private Collection<object> _outputQueue = new Collection<object>();
        private Collection<object> _output = new Collection<object>();
        private Collection<object> _phoneticEventArgs = new Collection<object>();
        private int _totalPromptCount = 0;
        private Collection<Prompt> _prompts = new Collection<Prompt>();
        private bool _speechStarted = false;
        private bool _isCancelled = false;
        private bool _isCompleted = false;

        /// <summary>
        /// Path of generated WAV file or null if speech is generated to default audio device.
        /// </summary>
        public string OutputPath { get { return _path; } }

        /// <summary>
        /// Rate of speech.
        /// </summary>
        public int Rate { get { return _rate; } }

        /// <summary>
        /// Current voice.
        /// </summary>
        public VoiceInfo Voice { get { return _voice; } }

        /// <summary>
        /// Volume
        /// </summary>
        public int Volume { get { return _volume; } }

        /// <summary>
        /// Current speech synthesis state.
        /// </summary>
        public SynthesizerState State { get { return _state; } }

        /// <summary>
        /// True if speech generation is completed; otherwise, false.
        /// </summary>
        public bool IsCompleted { get { return _isCompleted; } }

        /// <summary>
        /// True if speech generation wass cancelled; otherwise, false.
        /// </summary>
        public bool IsCanceled { get { return _isCancelled; } }

        /// <summary>
        /// Index of current <seealso cref="Prompt"/>.
        /// </summary>
        public int CurrentPromptIndex { get { return _totalPromptCount - _prompts.Count; } }

        /// <summary>
        /// Total number of prompts.
        /// </summary>
        public int TotalPromptCount { get { return _totalPromptCount; } }

        WaitHandle IAsyncResult.AsyncWaitHandle { get { return _speechCompletedEvent; } }

        /// <summary>
        /// User state object associated with Text-To-Speech job.
        /// </summary>
        public object AsyncState { get { return _asyncState; } }

        bool IAsyncResult.CompletedSynchronously { get { return _isCompleted && !_speechStarted; } }

        /// <summary>
        /// Initialize new <see cref="TextToSpeechJob"/>.
        /// </summary>
        /// <param name="prompts">Collection of <seealso cref="Prompt"/>s to be spoken.</param>
        /// <param name="noAutoStart">True if speech should not automatically start; otherwise false to begin speaking immediately.</param>
        /// <param name="rate">Initial rate of speech.</param>
        /// <param name="volume">Initial speech volume.</param>
        /// <param name="voice">Name of voice to select or null for default voice.</param>
        /// <param name="outputPath">Path of WAV file output or null to speak to default audio device.</param>
        /// <param name="formatInfo">Format of output WAV file. This is ignored if <paramref name="outputPath"/> is not specified.</param>
        /// <param name="asyncState">User state object to associate with Text-To-Speech job.</param>
        public TextToSpeechJob(Collection<Prompt> prompts, bool noAutoStart, int? rate, int? volume, string voice, string outputPath, SpeechAudioFormatInfo formatInfo, object asyncState)
        {
            if (!String.IsNullOrEmpty(voice))
                _speechSynthesizer.SelectVoice(voice);
            Initialize(prompts, noAutoStart, rate, volume, outputPath, formatInfo, asyncState);
        }

        /// <summary>
        /// Initialize new <see cref="TextToSpeechJob"/>.
        /// </summary>
        /// <param name="prompts">Collection of <seealso cref="Prompt"/>s to be spoken.</param>
        /// <param name="noAutoStart">True if speech should not automatically start; otherwise false to begin speaking immediately.</param>
        /// <param name="rate">Initial rate of speech.</param>
        /// <param name="volume">Initial speech volume.</param>
        /// <param name="gender">Gender of voice.</param>
        /// <param name="outputPath">Path of WAV file output or null to speak to default audio device.</param>
        /// <param name="formatInfo">Format of output WAV file. This is ignored if <paramref name="outputPath"/> is not specified.</param>
        /// <param name="asyncState">User state object to associate with Text-To-Speech job.</param>
        public TextToSpeechJob(Collection<Prompt> prompts, bool noAutoStart, int? rate, int? volume, VoiceGender gender, string outputPath, SpeechAudioFormatInfo formatInfo, object asyncState)
        {
            if (gender != VoiceGender.NotSet)
                _speechSynthesizer.SelectVoiceByHints(gender);
            Initialize(prompts, noAutoStart, rate, volume, outputPath, formatInfo, asyncState);
        }

        /// <summary>
        /// Initialize new <see cref="TextToSpeechJob"/>.
        /// </summary>
        /// <param name="prompts">Collection of <seealso cref="Prompt"/>s to be spoken.</param>
        /// <param name="noAutoStart">True if speech should not automatically start; otherwise false to begin speaking immediately.</param>
        /// <param name="rate">Initial rate of speech.</param>
        /// <param name="volume">Initial speech volume.</param>
        /// <param name="gender">Gender of voice.</param>
        /// <param name="age">Age of voice.</param>
        /// <param name="outputPath">Path of WAV file output or null to speak to default audio device.</param>
        /// <param name="formatInfo">Format of output WAV file. This is ignored if <paramref name="outputPath"/> is not specified.</param>
        /// <param name="asyncState">User state object to associate with Text-To-Speech job.</param>
        public TextToSpeechJob(Collection<Prompt> prompts, bool noAutoStart, int? rate, int? volume, VoiceGender gender, VoiceAge age, string outputPath, SpeechAudioFormatInfo formatInfo, object asyncState)
        {
            if (gender != VoiceGender.NotSet)
            {
                if (age != VoiceAge.NotSet)
                    _speechSynthesizer.SelectVoiceByHints(gender, age);
                else
                    _speechSynthesizer.SelectVoiceByHints(gender);
            }
            Initialize(prompts, noAutoStart, rate, volume, outputPath, formatInfo, asyncState);
        }

        private void Initialize(Collection<Prompt> prompts, bool noAutoStart, int? rate, int? volume, string outputPath, SpeechAudioFormatInfo formatInfo, object asyncState)
        {
            _asyncState = asyncState;
            _formatInfo = formatInfo;
            if (String.IsNullOrEmpty(outputPath))
                _path = null;
            else
                _path = Path.GetFullPath(outputPath);
            if (prompts != null)
            {
                foreach (Prompt p in prompts)
                {
                    if (p != null)
                        _prompts.Add(p);
                }
            }
            _totalPromptCount = _prompts.Count;
            if (rate.HasValue)
                _speechSynthesizer.Rate = rate.Value;
            if (volume.HasValue)
                _speechSynthesizer.Volume = volume.Value;
            _voice = _speechSynthesizer.Voice;
            _rate = _speechSynthesizer.Rate;
            _volume = _speechSynthesizer.Volume;

            if (_prompts.Count == 0)
            {
                _speechSynthesizer.Dispose();
                _state = SynthesizerState.Ready;
                _speechSynthesizer = null;
                _speechCompletedEvent.Set();
                _isCompleted = true;
                return;
            }
            _speechSynthesizer.BookmarkReached += SpeechSynthesizer_BookmarkReached;
            _speechSynthesizer.PhonemeReached += SpeechSynthesizer_PhonemeReached;
            _speechSynthesizer.SpeakCompleted += SpeechSynthesizer_SpeakCompleted;
            _speechSynthesizer.SpeakProgress += SpeechSynthesizer_SpeakProgress;
            _speechSynthesizer.SpeakStarted += SpeechSynthesizer_SpeakStarted;
            _speechSynthesizer.StateChanged += SpeechSynthesizer_StateChanged;
            _speechSynthesizer.VisemeReached += SpeechSynthesizer_VisemeReached;
            _speechSynthesizer.VoiceChange += SpeechSynthesizer_VoiceChange;
            if (!noAutoStart)
            {
                _state = _speechSynthesizer.State;
                _StartSpeech();
            }
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        ~TextToSpeechJob() { Dispose(false); }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        private bool _ApplyOutputQueue()
        {
            if (_outputQueue.Count == 0)
                return false;
            if (_output.Count == 0)
            {
                _output = _outputQueue;
                _outputQueue = new Collection<object>();
            }
            else
            {
                while (_outputQueue.Count > 0)
                    _output.Add(_outputQueue[0]);
                _outputQueue.Clear();
            }
            return true;
        }

        /// <summary>
        /// Waits indefinitely for speech completion.
        /// </summary>
        /// <returns>True if speech was completed; otherwise this never returns.</returns>
        public bool WaitSpeechCompleted() { return _speechCompletedEvent.WaitOne(); }

        /// <summary>
        /// Waits for a specified number of milliseconds for speech completion.
        /// </summary>
        /// <param name="millisecondsTimeout">Number of milliseconds to wait.</param>
        /// <returns>True if speech completed within the specified number of milliseconds; otherwise, false.</returns>
        public bool WaitSpeechCompleted(int millisecondsTimeout) { return _speechCompletedEvent.WaitOne(millisecondsTimeout); }

        /// <summary>
        /// Waits for a specified timeframe for speech completion.
        /// </summary>
        /// <param name="timeout">Length of time to wait.</param>
        /// <returns>True if speech completed within the specified <seealso cref="TimeSpan"/>; otherwise, false.</returns>
        public bool WaitSpeechCompleted(TimeSpan timeout) { return _speechCompletedEvent.WaitOne(timeout); }

        /// <summary>
        /// Waits indefinitely for speech progress change.
        /// </summary>
        /// <returns>True if speech progress events have occurred; otherwise, false.</returns>
        public bool WaitSpeechProgress()
        {
            lock (_syncRoot)
            {
                lock (_outputQueue)
                {
                    if (_ApplyOutputQueue())
                        return true;
                }
                if (_state != SynthesizerState.Speaking)
                    return false;
            }

            while (!_isCompleted)
            {
                if (_speechProgressEvent.WaitOne(1000))
                    return true;
            }

            lock (_syncRoot)
            {
                if (_ApplyOutputQueue())
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Waits for a specified number of milliseconds for speech progress change.
        /// </summary>
        /// <param name="millisecondsTimeout">Number of milliseconds to wait.</param>
        /// <returns>True if speech progress events have occurred; otherwise, false.</returns>
        public bool WaitSpeechProgress(int millisecondsTimeout)
        {
            lock (_syncRoot)
            {
                lock (_outputQueue)
                {
                    if (_ApplyOutputQueue())
                        return true;
                }
                if (_state != SynthesizerState.Speaking)
                    return false;
            }

            if (!_isCompleted && _speechProgressEvent.WaitOne(millisecondsTimeout))
                return true;

            lock (_outputQueue)
            {
                if (_ApplyOutputQueue())
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Waits for a specified timeframe for speech progress change.
        /// </summary>
        /// <param name="timeout">Length of time to wait</param>
        /// <returns>True if speech progress events have occurred; otherwise, false.</returns>
        public bool WaitSpeechProgress(TimeSpan timeout)
        {
            lock (_syncRoot)
            {
                if (_ApplyOutputQueue())
                    return true;
                if (_state != SynthesizerState.Speaking)
                    return false;
            }

            if (!_isCompleted && _speechProgressEvent.WaitOne(timeout))
                return true;

            lock (_syncRoot)
            {
                if (_ApplyOutputQueue())
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Gets event data output.
        /// </summary>
        /// <returns>Objects which represent progress event data.</returns>
        public Collection<object> GetOutput() { return GetOutput(false); }

        /// <summary>
        /// Gets event data output.
        /// </summary>
        /// <param name="doNotClear">True to leave output in the internal buffer; otherwise, false to clear the event data after returning the values.</param>
        /// <returns>Objects which represent progress event data.</returns>
        public Collection<object> GetOutput(bool doNotClear)
        {
            Collection<object> result;
            lock (_syncRoot)
            {
                _ApplyOutputQueue();
                if (doNotClear)
                    result = new Collection<object>(_output);
                else
                {
                    result = _output;
                    _output = new Collection<object>();
                }
            }
            return result;
        }

        /// <summary>
        /// Get event data from phonetic events.
        /// </summary>
        /// <returns>Objects which represent phonetic event data.</returns>
        public Collection<object> GetPhoneticEventArgs() { return GetPhoneticEventArgs(false); }

        /// <summary>
        /// Get event data from phonetic events.
        /// </summary>
        /// <param name="doNotClear">True to leave phonetic event data in the internal buffer; otherwise, false to clear the event data after returning the values.</param>
        /// <returns>Objects which represent phonetic event data.</returns>
        public Collection<object> GetPhoneticEventArgs(bool doNotClear)
        {
            Collection<object> result;
            lock (_phoneticEventArgs)
            {
                if (doNotClear)
                    result = new Collection<object>(_phoneticEventArgs);
                else
                {
                    result = _phoneticEventArgs;
                    _phoneticEventArgs = new Collection<object>();
                }
            }
            return result;
        }

        /// <summary>
        /// Expunges any phonetic event data.
        /// </summary>
        public void ClearPhoneticEventArgs()
        {
            lock (_phoneticEventArgs)
                _phoneticEventArgs.Clear();
        }

        /// <summary>
        /// This gets called when the current object is being disposed.
        /// </summary>
        /// <param name="disposing">True if being disposed from the public <seealso cref="Dispose()"/> method, otherwise false.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
                return;
            SpeechSynthesizer speechSynthesizer = _speechSynthesizer;
            _speechSynthesizer = null;
            if (speechSynthesizer == null)
                return;
            speechSynthesizer.BookmarkReached -= SpeechSynthesizer_BookmarkReached;
            speechSynthesizer.PhonemeReached -= SpeechSynthesizer_PhonemeReached;
            speechSynthesizer.SpeakCompleted -= SpeechSynthesizer_SpeakCompleted;
            speechSynthesizer.SpeakProgress -= SpeechSynthesizer_SpeakProgress;
            speechSynthesizer.SpeakStarted -= SpeechSynthesizer_SpeakStarted;
            speechSynthesizer.StateChanged -= SpeechSynthesizer_StateChanged;
            speechSynthesizer.VisemeReached -= SpeechSynthesizer_VisemeReached;
            speechSynthesizer.VoiceChange -= SpeechSynthesizer_VoiceChange;
            _speechProgressEvent.Dispose();
            _speechCompletedEvent.Dispose();
            speechSynthesizer.Dispose();
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public void Dispose()
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            GC.SuppressFinalize(this);
            Dispose(true);
        }
        
        private void WriteError(Exception exception, string eventName, object targetObject)
        {
            if (exception is ArgumentException || exception is IndexOutOfRangeException)
                WriteError(exception, eventName, ErrorCategory.InvalidArgument, targetObject);
            else if (exception is FileNotFoundException || exception is DirectoryNotFoundException || exception is System.IO.DriveNotFoundException)
                WriteError(exception, eventName, ErrorCategory.ObjectNotFound, targetObject);
            else if (exception is OutOfMemoryException || exception is PathTooLongException)
                WriteError(exception, eventName, ErrorCategory.LimitsExceeded, targetObject);
            else if (exception is IOException)
                WriteError(exception, eventName, ErrorCategory.ReadError, targetObject);
            else if (exception is FormatException)
                WriteError(exception, eventName, ErrorCategory.SyntaxError, targetObject);
            else if (exception is InvalidCastException)
                WriteError(exception, eventName, ErrorCategory.InvalidType, targetObject);
            else if (exception is InvalidOperationException)
                WriteError(exception, eventName, ErrorCategory.InvalidOperation, targetObject);
            else if (exception is NotImplementedException)
                WriteError(exception, eventName, ErrorCategory.NotImplemented, targetObject);
            else if (exception is NotSupportedException)
                WriteError(exception, eventName, ErrorCategory.NotInstalled, targetObject);
            else if (exception is NullReferenceException)
                WriteError(exception, eventName, ErrorCategory.InvalidData, targetObject);
            else if (exception is OperationCanceledException)
                WriteError(exception, eventName, ErrorCategory.OperationStopped, targetObject);
            else
                WriteError(exception, eventName, ErrorCategory.NotSpecified, targetObject);
        }

        private void WriteError(Exception exception, string eventName, ErrorCategory errorCategory, object targetObject)
        {
            lock (_syncRoot)
            {
                _outputQueue.Add(new ErrorRecord(exception, GetType().FullName + "." + eventName, errorCategory, targetObject));
                _speechProgressEvent.Set();
            }
        }

        private void SpeechSynthesizer_SpeakStarted(object sender, SpeakStartedEventArgs e)
        {
            try
            {
                lock (_syncRoot)
                {
                    _lastStatusEvent = e;
                    _outputQueue.Add(e);
                    _rate = _speechSynthesizer.Rate;
                    _volume = _speechSynthesizer.Volume;
                    _state = SynthesizerState.Speaking;
                    _speechProgressEvent.Set();
                    _speechCompletedEvent.Reset();
                }
            }
            catch (Exception exception) { WriteError(exception, "SpeakStarted", e); }
            if (e.Error != null)
                WriteError(e.Error, "SpeakStarted", e);
        }

        private void SpeechSynthesizer_SpeakCompleted(object sender, SpeakCompletedEventArgs e)
        {
            try
            {
                lock (_syncRoot)
                {
                    _prompts.Remove(e.Prompt);
                    if (_prompts.Count == 0)
                    {
                        _lastStatusEvent = e;
                        _outputQueue.Add(e);
                        _state = SynthesizerState.Ready;
                        _isCompleted = true;
                        _speechProgressEvent.Set();
                        _speechCompletedEvent.Set();
                        if (_path != null)
                            _speechSynthesizer.SetOutputToNull();
                        _speechSynthesizer.Dispose();
                        _speechSynthesizer = null;
                    }
                }
            }
            catch (Exception exception) { WriteError(exception, "SpeakCompleted", e); }
            if (e.Error != null)
                WriteError(e.Error, "SpeakCompleted", e);
        }

        private void SpeechSynthesizer_BookmarkReached(object sender, BookmarkReachedEventArgs e)
        {
            try
            {
                lock (_syncRoot)
                {
                    _lastStatusEvent = e;
                    _rate = _speechSynthesizer.Rate;
                    _volume = _speechSynthesizer.Volume;
                    _outputQueue.Add(e);
                    _speechProgressEvent.Set();
                }
            }
            catch (Exception exception) { WriteError(exception, "BookmarkReached", e); }
            if (e.Error != null)
                WriteError(e.Error, "SpeakStarted", e);
        }

        private void SpeechSynthesizer_StateChanged(object sender, StateChangedEventArgs e)
        {
            try
            {
                lock (_syncRoot)
                {
                    _lastStatusEvent = e;
                    _outputQueue.Add(e);
                    _speechProgressEvent.Set();
                    _state = e.State;
                }
            }
            catch (Exception exception) { WriteError(exception, "StateChanged", e); }
        }

        private void SpeechSynthesizer_VoiceChange(object sender, VoiceChangeEventArgs e)
        {
            try
            {
                lock (_syncRoot)
                {
                    _voice = _speechSynthesizer.Voice;
                    _rate = _speechSynthesizer.Rate;
                    _volume = _speechSynthesizer.Volume;
                    _lastStatusEvent = e;
                    _outputQueue.Add(e);
                    _speechProgressEvent.Set();
                }
            }
            catch (Exception exception) { WriteError(exception, "VoiceChange", e); }
            if (e.Error != null)
                WriteError(e.Error, "SpeakStarted", e);
        }

        private void SpeechSynthesizer_SpeakProgress(object sender, SpeakProgressEventArgs e)
        {
            try
            {
                lock (_syncRoot)
                {
                    _rate = _speechSynthesizer.Rate;
                    _volume = _speechSynthesizer.Volume;
                    if (e.Error != null || e.Cancelled)
                    {
                        _lastStatusEvent = e;
                        _outputQueue.Add(e);
                        _speechProgressEvent.Set();
                    }
                    else
                    {
                        lock (_phoneticEventArgs)
                            _phoneticEventArgs.Add(e);
                    }
                }
            }
            catch (Exception exception) { WriteError(exception, "SpeakProgress", e); }
            if (e.Error != null)
                WriteError(e.Error, "SpeakStarted", e);
        }

        private void SpeechSynthesizer_VisemeReached(object sender, VisemeReachedEventArgs e)
        {
            try
            {
                lock (_syncRoot)
                {
                    lock (_phoneticEventArgs)
                        _phoneticEventArgs.Add(e);
                    _rate = _speechSynthesizer.Rate;
                    _volume = _speechSynthesizer.Volume;
                    if (e.Error != null || e.Cancelled)
                    {
                        _lastStatusEvent = e;
                        _outputQueue.Add(e);
                        _speechProgressEvent.Set();
                    }
                }
            }
            catch (Exception exception) { WriteError(exception, "VisemeReached", e); }
            if (e.Error != null)
                WriteError(e.Error, "SpeakStarted", e);
        }

        private void SpeechSynthesizer_PhonemeReached(object sender, PhonemeReachedEventArgs e)
        {
            try
            {
                lock (_syncRoot)
                {
                    _rate = _speechSynthesizer.Rate;
                    _volume = _speechSynthesizer.Volume;
                    lock (_phoneticEventArgs)
                        _phoneticEventArgs.Add(e);
                    if (e.Error != null || e.Cancelled)
                    {
                        _lastStatusEvent = e;
                        _outputQueue.Add(e);
                        _speechProgressEvent.Set();
                    }
                }
            }
            catch (Exception exception) { WriteError(exception, "PhonemeReached", e); }
            if (e.Error != null)
                WriteError(e.Error, "SpeakStarted", e);
        }

        //public void AddLexicon(Uri uri, string mediaType) { _speechSynthesizer.AddLexicon(uri, mediaType); }

        /// <summary>
        /// Gets prompt currently being spoken.
        /// </summary>
        /// <returns><seealso cref="Prompt"/> currently being spoken or null if speech has comnpleted.</returns>
        public Prompt GetCurrentlySpokenPrompt()
        {
            lock (_syncRoot)
            {
                if (!_isCompleted)
                    return _speechSynthesizer.GetCurrentlySpokenPrompt();
            }

            return null;
        }

        //public ReadOnlyCollection<InstalledVoice> GetInstalledVoices() { return _speechSynthesizer.GetInstalledVoices(); }

        //public ReadOnlyCollection<InstalledVoice> GetInstalledVoices(CultureInfo culture) { return _speechSynthesizer.GetInstalledVoices(culture); }
        
        /// <summary>
        /// Pauses speech.
        /// </summary>
        public void Pause()
        {
            lock (_syncRoot)
            {
                if (_state == SynthesizerState.Speaking)
                {
                    _speechSynthesizer.Pause();
                    _state = SynthesizerState.Paused;
                }
            }
        }
        
        //public void RemoveLexicon(Uri uri) { _speechSynthesizer.RemoveLexicon(uri); }

        private void _StartSpeech()
        {
            _speechStarted = true;
            if (_path != null)
            {
                if (_formatInfo != null)
                    _speechSynthesizer.SetOutputToWaveFile(_path, _formatInfo);
                else
                    _speechSynthesizer.SetOutputToWaveFile(_path);
            }
            else
                _speechSynthesizer.SetOutputToDefaultAudioDevice();

            if (_prompts.Count > 0)
            {
                foreach (Prompt p in _prompts)
                    _speechSynthesizer.SpeakAsync(p);
                _state = SynthesizerState.Speaking;
            }
        }

        /// <summary>
        /// Resumes speech or starts speech if speech was initially paused.
        /// </summary>
        public void Resume()
        {
            lock (_syncRoot)
            {
                if (_state == SynthesizerState.Paused)
                {
                    if (_speechStarted)
                        _speechSynthesizer.Resume();
                    else
                        _StartSpeech();
                }
            }
        }

        /// <summary>
        /// Cancels any speech in progress, including any queued prompts.
        /// </summary>
        public void AsyncCancel()
        {
            lock (_syncRoot)
            {
                if (_isCompleted)
                    return;
                _isCancelled = true;
                if (_speechStarted)
                {
                    if (_state == SynthesizerState.Paused)
                        _speechSynthesizer.Resume();
                    _speechSynthesizer.SpeakAsyncCancelAll();
                }
                else
                {
                    _speechStarted = true;
                    _isCompleted = true;
                }
                _prompts.Clear();
            }
        }
    }
}
