using System;
using System.Speech.Synthesis;

namespace PSTTS
{
    public class SpeakCompletedEventInfo : SpeechWaitEvent<SpeakCompletedEventInfo>, ISpeechWaitEvent
    {
        public SpeakCompletedEventInfo(SpeakCompletedEventArgs args)
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