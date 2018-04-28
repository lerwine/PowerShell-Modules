using System;
using System.Speech.Synthesis;

namespace PSTTS
{
    internal class SpeakProgressEventData : AsyncCompletedEventLogItem<SpeakProgressEventArgs>
    {
        public override string Text { get { return Args.Text; } }
        public override int CharacterPosition { get { return Args.CharacterPosition; } }
        public override int CharacterCount { get { return Args.CharacterCount; } }
        public override TimeSpan AudioPosition { get { return Args.AudioPosition; } }
        public SpeakProgressEventData(SpeakProgressEventArgs e, SpeechEventHandler.SpeechContext context) : base(e, context) { }
    }
}