using System;
using System.Speech.Synthesis;

namespace PSTTS
{
    public class StateChangedEventInfo : SpeechWaitEvent<StateChangedEventInfo>, ISpeechWaitEvent
    {
        public StateChangedEventInfo(StateChangedEventArgs args)
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