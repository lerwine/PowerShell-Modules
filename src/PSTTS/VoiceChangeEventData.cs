using System.Speech.Synthesis;

namespace PSTTS
{
    internal class VoiceChangeEventData : AsyncCompletedEventLogItem<VoiceChangeEventArgs>
    {
        public override VoiceInfo Voice { get { return Args.Voice; } }
        public VoiceChangeEventData(VoiceChangeEventArgs e, SpeechEventHandler.SpeechContext context) : base(e, context) { }
    }
}