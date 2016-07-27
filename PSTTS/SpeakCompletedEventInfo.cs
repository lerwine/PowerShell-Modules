using System.Speech.Synthesis;

namespace PSTTS
{
    public class SpeakCompletedEventInfo : SpeechWaitEvent<SpeakCompletedEventInfo>, ISpeechWaitEvent
    {
        public SpeakCompletedEventInfo(SpeakCompletedEventArgs args)
        {
        }
    }
}