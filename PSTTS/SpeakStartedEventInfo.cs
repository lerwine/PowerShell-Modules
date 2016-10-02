using System;
using System.Speech.Synthesis;

namespace PSTTS
{
    public class SpeakStartedEventInfo : SpeechWaitEvent<SpeakStartedEventInfo>, ISpeechWaitEvent
    {
        public SpeakStartedEventInfo(SpeakStartedEventArgs args)
        {
        }

        public TimeSpan AudioPosition
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string Bookmark
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public Prompt Prompt
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public VoiceInfo Voice
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
}