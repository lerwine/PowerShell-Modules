using System.Speech.Synthesis;

namespace PSTTS
{
    public class SpeakProgressEventInfo : SpeechWaitEvent<SpeakProgressEventInfo>, ISpeechWaitEvent
    {
        public SpeakProgressEventInfo(SpeakProgressEventArgs args)
        {
        }
    }
}