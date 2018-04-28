using System.Speech.Synthesis;

namespace PSTTS
{
    internal class SpeakStartedEventData : AsyncCompletedEventLogItem<SpeakStartedEventArgs>
    {
        public SpeakStartedEventData(SpeakStartedEventArgs e, SpeechEventHandler.SpeechContext context) : base(e, context) { }
    }
}