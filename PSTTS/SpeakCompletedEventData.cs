using System.Speech.Synthesis;

namespace PSTTS
{
    internal class SpeakCompletedEventData : AsyncCompletedEventLogItem<SpeakCompletedEventArgs>
    {
        public SpeakCompletedEventData(SpeakCompletedEventArgs e, SpeechEventHandler.SpeechContext context) : base(e, context) { }
    }
}