using System.Speech.Synthesis;

namespace PSTTS
{
    public class PhonemeReachedEventInfo : SpeechWaitEvent<PhonemeReachedEventInfo>, ISpeechWaitEvent
    {
        public PhonemeReachedEventInfo(PhonemeReachedEventArgs args)
        {
        }
    }
}