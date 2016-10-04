using System;
using System.Speech.Synthesis;

namespace PSTTS
{
    public abstract class SpeechWaitEvent
    {

        internal static ISpeechWaitEvent Create(VoiceChangeEventArgs args) { return new VoiceChangeEventInfo(args); }

        internal static ISpeechWaitEvent Create(BookmarkReachedEventArgs args) { return new BookmarkReachedEventInfo(args); }

        internal static ISpeechWaitEvent Create(PhonemeReachedEventArgs args) { return new PhonemeReachedEventInfo(args); }

        internal static ISpeechWaitEvent Create(SpeakCompletedEventArgs args) { return new SpeakCompletedEventInfo(args); }

        internal static ISpeechWaitEvent Create(SpeakProgressEventArgs args) { return new SpeakProgressEventInfo(args); }

        internal static ISpeechWaitEvent Create(SpeakStartedEventArgs args) { return new SpeakStartedEventInfo(args); }

        internal static ISpeechWaitEvent Create(StateChangedEventArgs args) { return new StateChangedEventInfo(args); }

        internal static ISpeechWaitEvent Create(VisemeReachedEventArgs args) { return new VisemeReachedEventInfo(args); }
        public static ISpeechWaitEvent None { get; internal set; }

        public SpeechWaitEventType EventType { get; private set; }
        public bool Cancelled { get; private set; }
        public Exception Error { get; private set; }
        public object UserState { get; private set; }
    }

    public abstract class SpeechWaitEvent<TEvent> : SpeechWaitEvent
        where TEvent : SpeechWaitEvent<TEvent>, ISpeechWaitEvent
    {

        public SpeechWaitEvent() { }
    }
}