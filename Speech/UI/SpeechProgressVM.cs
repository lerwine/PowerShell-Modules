using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Speech.Synthesis;
using System.Text;
using System.Windows;

namespace Speech.UI
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class SpeechProgressVM : DependencyObject
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {
        private object _syncRoot = new object();
        private object _speechSynthesizer = null;

        #region Queue Property Members

        private ObservableCollection<PromptQueueVM> _queue = new ObservableCollection<PromptQueueVM>();
        
        /// <summary>
        /// Defines the name for the <see cref="Queue"/> dependency property.
        /// </summary>
        public const string PropertyName_Queue = "Queue";

        private static readonly DependencyPropertyKey QueuePropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_Queue, typeof(ReadOnlyObservableCollection<PromptQueueVM>), typeof(SpeechProgressVM),
            new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="Queue"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty QueueProperty = QueuePropertyKey.DependencyProperty;

        // TODO: Add summary documentation to the Queue Property.
        /// <summary>
        /// 
        /// </summary>
        public ReadOnlyObservableCollection<PromptQueueVM> Queue
        {
            get { return (ReadOnlyObservableCollection<PromptQueueVM>)(GetValue(QueueProperty)); }
            private set { SetValue(QueuePropertyKey, value); }
        }

        #endregion
        
        #region CurrentQueueIndex Property Members

        /// <summary>
        /// Defines the name for the <see cref="CurrentQueueIndex"/> dependency property.
        /// </summary>
        public const string PropertyName_CurrentQueueIndex = "CurrentQueueIndex";

        private static readonly DependencyPropertyKey CurrentQueueIndexPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_CurrentQueueIndex, typeof(int), typeof(SpeechProgressVM),
            new PropertyMetadata(-1));

        /// <summary>
        /// Identifies the <see cref="CurrentQueueIndex"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CurrentQueueIndexProperty = CurrentQueueIndexPropertyKey.DependencyProperty;

        // TODO: Add summary documentation to the CurrentQueueIndex Property.
        /// <summary>
        /// 
        /// </summary>
        public int CurrentQueueIndex
        {
            get { return (int)(GetValue(CurrentQueueIndexProperty)); }
            private set { SetValue(CurrentQueueIndexPropertyKey, value); }
        }
        
        #endregion

        internal static object CoercePercentageValue(DependencyObject d, object baseValue)
        {

            int? i = baseValue as int?;
            if (!i.HasValue || i.Value < 0)
                return 0;
            if (i.Value > 100)
                return 100;
            return i.Value;
        }

        internal static object CoerceNonEmptyString(DependencyObject d, object baseValue)
        {
            return (baseValue as string) ?? "";
        }

        #region PercentComplete Property Members

        /// <summary>
        /// Defines the name for the <see cref="PercentComplete"/> dependency property.
        /// </summary>
        public const string PropertyName_PercentComplete = "PercentComplete";

        private static readonly DependencyPropertyKey PercentCompletePropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_PercentComplete, typeof(int), typeof(SpeechProgressVM),
            new PropertyMetadata(0, null, CoercePercentageValue));

        /// <summary>
        /// Identifies the <see cref="PercentComplete"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PercentCompleteProperty = PercentCompletePropertyKey.DependencyProperty;

        // TODO: Add summary documentation to the PercentComplete Property.
        /// <summary>
        /// 
        /// </summary>
        public int PercentComplete
        {
            get { return (int)(GetValue(PercentCompleteProperty)); }
            private set { SetValue(PercentCompletePropertyKey, value); }
        }
        
        #endregion

        #region CurrentPrompt Property Members

        /// <summary>
        /// Defines the name for the <see cref="CurrentPrompt"/> dependency property.
        /// </summary>
        public const string PropertyName_CurrentPrompt = "CurrentPrompt";

        private static readonly DependencyPropertyKey CurrentPromptPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_CurrentPrompt, typeof(PromptQueueVM), typeof(SpeechProgressVM),
            new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="CurrentPrompt"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CurrentPromptProperty = CurrentPromptPropertyKey.DependencyProperty;

        // TODO: Add summary documentation to the CurrentPrompt Property.
        /// <summary>
        /// 
        /// </summary>
        public PromptQueueVM CurrentPrompt
        {
            get { return (PromptQueueVM)(GetValue(CurrentPromptProperty)); }
            private set { SetValue(CurrentPromptPropertyKey, value); }
        }
        #endregion

        #region TotalLength Property Members
        
        /// <summary>
        /// Defines the name for the <see cref="TotalLength"/> dependency property.
        /// </summary>
        public const string PropertyName_TotalLength = "TotalLength";

        private static readonly DependencyPropertyKey TotalLengthPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_TotalLength, typeof(long), typeof(SpeechProgressVM),
            new PropertyMetadata(0L));

        /// <summary>
        /// Identifies the <see cref="TotalLength"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TotalLengthProperty = TotalLengthPropertyKey.DependencyProperty;

        // TODO: Add summary documentation to the TotalLength Property.
        /// <summary>
        /// 
        /// </summary>
        public long TotalLength
        {
            get { return (long)(GetValue(TotalLengthProperty)); }
            private set { SetValue(TotalLengthPropertyKey, value); }
        }

        #endregion

        #region CompletedLength Property Members
        
        /// <summary>
        /// Defines the name for the <see cref="CompletedLength"/> dependency property.
        /// </summary>
        public const string PropertyName_CompletedLength = "CompletedLength";

        private static readonly DependencyPropertyKey CompletedLengthPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_CompletedLength, typeof(long), typeof(SpeechProgressVM),
            new PropertyMetadata(0L));

        /// <summary>
        /// Identifies the <see cref="CompletedLength"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CompletedLengthProperty = CompletedLengthPropertyKey.DependencyProperty;

        // TODO: Add summary documentation to the CompletedLength Property.
        /// <summary>
        /// 
        /// </summary>
        public long CompletedLength
        {
            get { return (long)(GetValue(CompletedLengthProperty)); }
            private set { SetValue(CompletedLengthPropertyKey, value); }
        }

        #endregion

        #region VoiceName Property Members
        
        /// <summary>
        /// Defines the name for the <see cref="VoiceName"/> dependency property.
        /// </summary>
        public const string PropertyName_VoiceName = "VoiceName";

        private static readonly DependencyPropertyKey VoiceNamePropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_VoiceName, typeof(string), typeof(SpeechProgressVM),
            new PropertyMetadata("", null, CoerceNonEmptyString));

        /// <summary>
        /// Identifies the <see cref="VoiceName"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty VoiceNameProperty = VoiceNamePropertyKey.DependencyProperty;

        // TODO: Add summary documentation to the VoiceName Property.
        /// <summary>
        /// 
        /// </summary>
        public string VoiceName
        {
            get { return GetValue(VoiceNameProperty) as string; }
            private set { SetValue(VoiceNamePropertyKey, value); }
        }

        #endregion

        #region Gender Property Members
        
        /// <summary>
        /// Defines the name for the <see cref="Gender"/> dependency property.
        /// </summary>
        public const string PropertyName_Gender = "Gender";

        private static readonly DependencyPropertyKey GenderPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_Gender, typeof(string), typeof(SpeechProgressVM),
            new PropertyMetadata("", null, CoerceNonEmptyString));

        /// <summary>
        /// Identifies the <see cref="Gender"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty GenderProperty = GenderPropertyKey.DependencyProperty;

        // TODO: Add summary documentation to the Gender Property.
        /// <summary>
        /// 
        /// </summary>
        public string Gender
        {
            get { return GetValue(GenderProperty) as string; }
            private set { SetValue(GenderPropertyKey, value); }
        }

        #endregion

        #region Age Property Members
        
        /// <summary>
        /// Defines the name for the <see cref="Age"/> dependency property.
        /// </summary>
        public const string PropertyName_Age = "Age";

        private static readonly DependencyPropertyKey AgePropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_Age, typeof(string), typeof(SpeechProgressVM),
            new PropertyMetadata("", null, CoerceNonEmptyString));

        /// <summary>
        /// Identifies the <see cref="Age"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AgeProperty = AgePropertyKey.DependencyProperty;

        // TODO: Add summary documentation to the Age Property.
        /// <summary>
        /// 
        /// </summary>
        public string Age
        {
            get { return GetValue(AgeProperty) as string; }
            private set { SetValue(AgePropertyKey, value); }
        }

        #endregion

        #region Cancelled Property Members
        
        /// <summary>
        /// Defines the name for the <see cref="Cancelled"/> dependency property.
        /// </summary>
        public const string PropertyName_Cancelled = "Cancelled";

        private static readonly DependencyPropertyKey CancelledPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_Cancelled, typeof(bool), typeof(SpeechProgressVM),
            new PropertyMetadata(false));

        /// <summary>
        /// Identifies the <see cref="Cancelled"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CancelledProperty = CancelledPropertyKey.DependencyProperty;

        // TODO: Add summary documentation to the Cancelled Property.
        /// <summary>
        /// 
        /// </summary>
        public bool Cancelled
        {
            get { return (bool)(GetValue(CancelledProperty)); }
            private set { SetValue(CancelledPropertyKey, value); }
        }

        #endregion

        #region PauseRequested Property Members
        
        /// <summary>
        /// Defines the name for the <see cref="PauseRequested"/> dependency property.
        /// </summary>
        public const string PropertyName_PauseRequested = "PauseRequested";

        private static readonly DependencyPropertyKey PauseRequestedPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_PauseRequested, typeof(bool), typeof(SpeechProgressVM),
            new PropertyMetadata(false/*, (d, e) => (d as SpeechProgressVM).PauseRequested_PropertyChanged((bool)(e.OldValue), (bool)(e.NewValue))*/));

        /// <summary>
        /// Identifies the <see cref="PauseRequested"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PauseRequestedProperty = PauseRequestedPropertyKey.DependencyProperty;

        // TODO: Add summary documentation to the PauseRequested Property.
        /// <summary>
        /// 
        /// </summary>
        public bool PauseRequested
        {
            get { return (bool)(GetValue(PauseRequestedProperty)); }
            private set { SetValue(PauseRequestedPropertyKey, value); }
        }
        
        #endregion

        #region IsPaused Property Members
            
        /// <summary>
        /// Defines the name for the <see cref="IsPaused"/> dependency property.
        /// </summary>
        public const string PropertyName_IsPaused = "IsPaused";

        private static readonly DependencyPropertyKey IsPausedPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_IsPaused, typeof(bool), typeof(SpeechProgressVM),
            new PropertyMetadata(false));

        /// <summary>
        /// Identifies the <see cref="IsPaused"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsPausedProperty = IsPausedPropertyKey.DependencyProperty;

        // TODO: Add summary documentation to the IsPaused Property.
        /// <summary>
        /// 
        /// </summary>
        public bool IsPaused
        {
            get { return (bool)(GetValue(IsPausedProperty)); }
            private set { SetValue(IsPausedPropertyKey, value); }
        }

        #endregion

        #region Rate Property Members
        
        /// <summary>
        /// Defines the name for the <see cref="Rate"/> dependency property.
        /// </summary>
        public const string PropertyName_Rate = "Rate";

        private static readonly DependencyPropertyKey RatePropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_Rate, typeof(int), typeof(SpeechProgressVM),
            new PropertyMetadata(0, null, CoercePercentageValue));

        /// <summary>
        /// Identifies the <see cref="Rate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RateProperty = RatePropertyKey.DependencyProperty;

        // TODO: Add summary documentation to the Rate Property.
        /// <summary>
        /// 
        /// </summary>
        public int Rate
        {
            get { return (int)(GetValue(RateProperty)); }
            private set { SetValue(RatePropertyKey, value); }
        }

        #endregion

        #region Volume Property Members
        
        /// <summary>
        /// Defines the name for the <see cref="Volume"/> dependency property.
        /// </summary>
        public const string PropertyName_Volume = "Volume";

        private static readonly DependencyPropertyKey VolumePropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_Volume, typeof(int), typeof(SpeechProgressVM),
            new PropertyMetadata(0, null, CoercePercentageValue));

        /// <summary>
        /// Identifies the <see cref="Volume"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty VolumeProperty = VolumePropertyKey.DependencyProperty;

        // TODO: Add summary documentation to the Volume Property.
        /// <summary>
        /// 
        /// </summary>
        public int Volume
        {
            get { return (int)(GetValue(VolumeProperty)); }
            private set { SetValue(VolumePropertyKey, value); }
        }

        #endregion

        #region BookmarksReached Property Members

        private ObservableCollection<BookmarkReachedVM> _bookmarksReached = new ObservableCollection<BookmarkReachedVM>();

        /// <summary>
        /// Defines the name for the <see cref="BookmarksReached"/> dependency property.
        /// </summary>
        public const string PropertyName_BookmarksReached = "BookmarksReached";

        private static readonly DependencyPropertyKey BookmarksReachedPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_BookmarksReached, typeof(ReadOnlyObservableCollection<BookmarkReachedVM>), typeof(SpeechProgressVM),
            new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="BookmarksReached"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BookmarksReachedProperty = BookmarksReachedPropertyKey.DependencyProperty;

        // TODO: Add summary documentation to the BookmarksReached Property.
        /// <summary>
        /// 
        /// </summary>
        public ReadOnlyObservableCollection<BookmarkReachedVM> BookmarksReached
        {
            get { return (ReadOnlyObservableCollection<BookmarkReachedVM>)(GetValue(BookmarksReachedProperty)); }
            private set { SetValue(BookmarksReachedPropertyKey, value); }
        }

        #endregion

        #region LastBookmarkReached Property Members
        
        /// <summary>
        /// Defines the name for the <see cref="LastBookmarkReached"/> dependency property.
        /// </summary>
        public const string PropertyName_LastBookmarkReached = "LastBookmarkReached";

        private static readonly DependencyPropertyKey LastBookmarkReachedPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_LastBookmarkReached, typeof(BookmarkReachedVM), typeof(SpeechProgressVM),
            new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="LastBookmarkReached"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LastBookmarkReachedProperty = LastBookmarkReachedPropertyKey.DependencyProperty;

        // TODO: Add summary documentation to the LastBookmarkReached Property.
        /// <summary>
        /// 
        /// </summary>
        public BookmarkReachedVM LastBookmarkReached
        {
            get { return (BookmarkReachedVM)(GetValue(LastBookmarkReachedProperty)); }
            private set { SetValue(LastBookmarkReachedPropertyKey, value); }
        }

        #endregion

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public SpeechProgressVM()
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            Queue = new ReadOnlyObservableCollection<PromptQueueVM>(_queue);
            BookmarksReached = new ReadOnlyObservableCollection<BookmarkReachedVM>(_bookmarksReached);
            LastBookmarkReached = new BookmarkReachedVM();
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public void StartSpeech(IEnumerable<PromptBuilder> prompts)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            if (!CheckAccess())
            {
                Dispatcher.Invoke(new Action<IEnumerable<PromptBuilder>>(StartSpeech), prompts);
                return;
            }

            lock (_syncRoot)
            {
                if (_speechSynthesizer != null)
                    throw new InvalidOperationException("Speech was already started.");
                _speechSynthesizer = new object();
            }

            if (prompts != null)
            {
                long totalLength = 0L;
                foreach (PromptBuilder p in prompts)
                {
                    PromptQueueVM vm = new PromptQueueVM(p);
                    totalLength += (long)(vm.Length);
                    _queue.Add(new PromptQueueVM(p));
                }
                TotalLength = totalLength;
            }

            if (_queue.Count == 0)
            {
                SetCompleted();
                return;
            }

            SpeechSynthesizer speechSynthesizer = new SpeechSynthesizer();
            speechSynthesizer.BookmarkReached += SpeechSynthesizer_BookmarkReached;
            speechSynthesizer.SpeakCompleted += SpeechSynthesizer_SpeakCompleted;
            speechSynthesizer.SpeakProgress += SpeechSynthesizer_SpeakProgress;
            speechSynthesizer.SpeakStarted += SpeechSynthesizer_SpeakStarted;
            speechSynthesizer.StateChanged += SpeechSynthesizer_StateChanged;
            speechSynthesizer.VoiceChange += SpeechSynthesizer_VoiceChange;
            _speechSynthesizer = speechSynthesizer;
            UpdateSpeechSynthInfo();
            MoveToNextPrompt();
        }

        private void UpdateSpeechSynthInfo()
        {
            SpeechSynthesizer speechSynthesizer = _speechSynthesizer  as SpeechSynthesizer;
            if (speechSynthesizer == null)
                return;

            SetVoice(speechSynthesizer.Voice);
            Rate = (speechSynthesizer.Rate + 10) * 5;
            Volume = speechSynthesizer.Volume;
        }

        private void SetCompleted()
        {
            SpeechSynthesizer speechSynthesizer;
            lock (_syncRoot)
            {
                speechSynthesizer = _speechSynthesizer as SpeechSynthesizer;
                _speechSynthesizer = new object();
                if (speechSynthesizer != null)
                {
                    speechSynthesizer.BookmarkReached -= SpeechSynthesizer_BookmarkReached;
                    speechSynthesizer.SpeakCompleted -= SpeechSynthesizer_SpeakCompleted;
                    speechSynthesizer.SpeakProgress -= SpeechSynthesizer_SpeakProgress;
                    speechSynthesizer.SpeakStarted -= SpeechSynthesizer_SpeakStarted;
                    speechSynthesizer.StateChanged -= SpeechSynthesizer_StateChanged;
                    speechSynthesizer.VoiceChange -= SpeechSynthesizer_VoiceChange;
                }
                _speechSynthesizer = null;
            }

            if (speechSynthesizer != null)
                speechSynthesizer.Dispose();
        }

        private void MoveToNextPrompt()
        {
            if (!CheckAccess())
            {
                Dispatcher.Invoke(new Action(MoveToNextPrompt));
                return;
            }

            SpeechSynthesizer speechSynthesizer = _speechSynthesizer as SpeechSynthesizer;
            if (speechSynthesizer == null)
            {
                SetCompleted();
                return;
            }
            try
            {
                if (CurrentQueueIndex < _queue.Count)
                {
                    while ((CurrentQueueIndex = CurrentQueueIndex + 1) < _queue.Count)
                    {
                        if (CurrentPrompt != null)
                            CompletedLength += (long)(CurrentPrompt.Length);
                        CurrentPrompt = _queue[CurrentQueueIndex];
                        SetCurrentPercentComplete(0.0f);
                        if (CurrentPrompt.Length > 0)
                        {
                            speechSynthesizer.SpeakAsync(CurrentPrompt.Target);
                            speechSynthesizer = null;
                            break;
                        }
                    }
                }
            }
            finally
            {
                if (speechSynthesizer != null)
                {
                    try
                    {
                        try
                        {
                            speechSynthesizer.BookmarkReached -= SpeechSynthesizer_BookmarkReached;
                            speechSynthesizer.SpeakCompleted -= SpeechSynthesizer_SpeakCompleted;
                            speechSynthesizer.SpeakProgress -= SpeechSynthesizer_SpeakProgress;
                            speechSynthesizer.SpeakStarted -= SpeechSynthesizer_SpeakStarted;
                            speechSynthesizer.StateChanged -= SpeechSynthesizer_StateChanged;
                            speechSynthesizer.VoiceChange -= SpeechSynthesizer_VoiceChange;
                        }
                        finally { speechSynthesizer.Dispose(); }
                    }
                    finally { SetCompleted(); }
                }
            }
        }

        private void SpeechSynthesizer_VoiceChange(object sender, VoiceChangeEventArgs e)
        {
            try
            {
                SetVoice(e.Voice);
                SetAsyncEventInfo(e);
                UpdateSpeechSynthInfo();
            }
            catch (Exception exception) { SetError(exception); }
        }

        private void SetError(Exception exception)
        {
            if (!CheckAccess())
            {
                Dispatcher.Invoke(new Action<Exception>(SetError), exception);
                return;
            }

#warning Not implemented
            throw new NotImplementedException();
        }

        private void SetAsyncEventInfo(AsyncCompletedEventArgs args)
        {
            if (!CheckAccess())
            {
                Dispatcher.Invoke(new Action<AsyncCompletedEventArgs>(SetAsyncEventInfo), args);
                return;
            }

            try
            {
                if (args.Error != null)
                    SetError(args.Error);
                if (args.Cancelled)
                    Cancelled = true;
            }
            catch (Exception exception) { SetError(exception); }
        }

        private void SetVoice(VoiceInfo voice)
        {
            if (!CheckAccess())
            {
                Dispatcher.Invoke(new Action<VoiceInfo>(SetVoice), voice);
                return;
            }

            if (voice == null)
            {
                VoiceName = "";
                Gender = "";
                Age = "";
            }
            else
            {
                VoiceName = (String.IsNullOrEmpty(voice.Name)) ? voice.Id : voice.Name;
                Gender = (voice.Gender == VoiceGender.NotSet) ? "Not Set" : voice.Gender.ToString("F");
                Age = (voice.Age == VoiceAge.NotSet) ? "Not Set" : voice.Age.ToString("F");
            }
        }

        private void SpeechSynthesizer_StateChanged(object sender, StateChangedEventArgs e)
        {
            if (!CheckAccess())
            {
                Dispatcher.Invoke(new Action<object, StateChangedEventArgs>(SpeechSynthesizer_StateChanged), sender, e);
                return;
            }

            try { IsPaused = (e.State == SynthesizerState.Paused); }
            catch (Exception exception) { SetError(exception); }
        }

        private void SpeechSynthesizer_SpeakStarted(object sender, SpeakStartedEventArgs e)
        {
            try
            {
                SetAsyncEventInfo(e);
                UpdateSpeechSynthInfo();
            }
            catch (Exception exception) { SetError(exception); }
        }

        private void SpeechSynthesizer_SpeakProgress(object sender, SpeakProgressEventArgs e)
        {
            try
            {
                SetCurrentPercentComplete((Convert.ToSingle(e.CharacterPosition) * 100.0f) / Convert.ToSingle(e.CharacterCount));
                SetAsyncEventInfo(e);
                UpdateSpeechSynthInfo();
            }
            catch (Exception exception) { SetError(exception); }
        }

        private void SetCurrentPercentComplete(float currentPromptPercentComplete)
        {
            if (!CheckAccess())
                Dispatcher.Invoke(new Action<float>(SetCurrentPercentComplete), currentPromptPercentComplete);
            else
                PercentComplete = Convert.ToInt32(((Convert.ToSingle(CompletedLength) +
                    (Convert.ToSingle(CurrentPrompt.Length) * currentPromptPercentComplete)) * 100.0f) / Convert.ToSingle(TotalLength));
        }

        private void SpeechSynthesizer_SpeakCompleted(object sender, SpeakCompletedEventArgs e)
        {
            MoveToNextPrompt();
        }

        private void SpeechSynthesizer_BookmarkReached(object sender, BookmarkReachedEventArgs e)
        {
            if (!CheckAccess())
            {
                Dispatcher.Invoke(new Action<object, BookmarkReachedEventArgs>(SpeechSynthesizer_BookmarkReached), sender, e);
                return;
            }

            try
            {
                BookmarkReachedVM vm = new BookmarkReachedVM(e, PercentComplete);
                _bookmarksReached.Add(vm);
                LastBookmarkReached = vm;
                UpdateAudioPosition(e.AudioPosition);
                SetAsyncEventInfo(e);
            }
            catch (Exception exception) { SetError(exception); }
        }

        private void UpdateAudioPosition(TimeSpan audioPosition)
        {
#warning Not implemented
            throw new NotImplementedException();
        }
    }
}
