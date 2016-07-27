using System.Speech.Synthesis;

namespace PSTTS
{
    public class StateChangedEventInfo : SpeechWaitEvent<StateChangedEventInfo>, ISpeechWaitEvent
    {
        public StateChangedEventInfo(StateChangedEventArgs args)
        {
        }
    }
}