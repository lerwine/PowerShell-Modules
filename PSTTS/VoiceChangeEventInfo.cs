using System;
using System.Speech.Synthesis;

namespace PSTTS
{
    public class VoiceChangeEventInfo : SpeechWaitEvent<VoiceChangeEventInfo>, ISpeechWaitEvent
    {
        public VoiceChangeEventInfo(VoiceChangeEventArgs args)
        {
        }

        TimeSpan ISpeechWaitEvent.AudioPosition
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        string ISpeechWaitEvent.Bookmark
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        bool ISpeechWaitEvent.Cancelled
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        Exception ISpeechWaitEvent.Error
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        SpeechWaitEventType ISpeechWaitEvent.EventType
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        Prompt ISpeechWaitEvent.Prompt
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        object ISpeechWaitEvent.UserState
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        VoiceInfo ISpeechWaitEvent.Voice
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
}