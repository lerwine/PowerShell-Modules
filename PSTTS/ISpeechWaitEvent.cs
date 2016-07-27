using System;
using System.Speech.Synthesis;

namespace PSTTS
{
    public interface ISpeechWaitEvent
    {
        SpeechWaitEventType EventType { get; }
        bool Cancelled { get; }
        Exception Error { get; }
        Prompt Prompt { get; }
        object UserState { get; }
        VoiceInfo Voice { get; }
        TimeSpan AudioPosition { get; }
        string Bookmark { get; }
    }
}