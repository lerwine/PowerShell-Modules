using System.Speech.Synthesis;

namespace PSTTS
{
    public class SpeakStartedEventInfo : SpeechWaitEvent<SpeakStartedEventInfo>, ISpeechWaitEvent
    {
        public SpeakStartedEventInfo(SpeakStartedEventArgs args)
        {
        }
    }
}